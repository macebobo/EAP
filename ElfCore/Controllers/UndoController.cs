using System.Drawing;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Core.UndoData;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElfCore.Controllers
{
	/// <summary>
	/// Handles all the Undo and Redo operations by taking a snapshot of the data before and after an operation and saves the differences.
	/// </summary>
	public class UndoController : ElfBase
	{
		#region [ Enum ]

		enum Activity : int
		{ 
			Undo,
			Redo
		}

		#endregion [ Enum ]

		#region [ Constants ]

		public const string UNCHANGED = "UNCHANGED";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Snapshot _lastSnapshot = null;
		private Stack<UndoRedo> _undoStack = null;
		private Stack<UndoRedo> _redoStack = null;
		private BaseProfile _profile = null;
		private bool _applyingChangeSet = false;
		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Indicates whether there are any Redos waiting on the stack
		/// </summary>
		public bool HasRedo
		{
			get { return (_redoStack.Count > 0); }
		}

		/// <summary>
		/// Indicates whether there are any Undos waiting on the stack
		/// </summary>
		public bool HasUndo
		{
			get { return (_undoStack.Count > 0); }
		}

		/// <summary>
		/// Returns the action text of the topmost Redo. If there are none present in the stack, returns an empty string.
		/// </summary>
		public string RedoText 
		{
			get
			{
				if (_redoStack.Count > 0)
					return _redoStack.Peek().Action;
				else
					return string.Empty;
			} 
		}

		/// <summary>
		/// Returns the action text of the top most Undo. If there are none present in the stack, returns an empty string.
		/// </summary>
		public string UndoText
		{
			get
			{
				if (_undoStack.Count > 0)
					return _undoStack.Peek().Action;
				else
					return string.Empty;
			}
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public UndoController()
			: base()
		{
			_undoStack = new Stack<UndoRedo>();
			_redoStack = new Stack<UndoRedo>();
		}

		public UndoController(BaseProfile profile)
			: this()
		{
			_profile = profile;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Applies changes to the data
		/// </summary>
		/// <param name="changeset">Set of data changes, either from an Undo or a Redo event</param>
		/// <param name="useUndoChangeSet">Indicates whether the Undo or the Redo Changeset should be used</param>
		private void ApplyChangeset(UndoRedo changeset, Activity activity)
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			//_workshop.WriteTraceMessage("Activity: " + activity.ToString(), TraceLevel.Verbose);

			if (changeset == null)
			{
				//_workshop.WriteTraceMessage("ChangeSet is null, Exiting", TraceLevel.Error);
				return;
			}
			
			_applyingChangeSet = true;
			bool Undoing = (activity == Activity.Undo);
			bool ChangedScaling = false;
			string Serialized = string.Empty;
			string ListString = string.Empty;
			List<string> UndoChannelIDs = null;
			List<int> ChannelIDs = null;
			SortedList<int, string> SerialList;
			Mask Mask = null;
			Channel Channel = null;
			Channel TargetChannel = null;

			_profile.SuppressEvents = true;

			#region [ Background ]

			Serialized = changeset.Background(Undoing);
			if (Serialized.Length > 0)
			{
				Background UndoBackground = Extends.DeserializeObjectFromXml<Background>(Serialized);

				if (UndoBackground != null)
				{
					if (UndoBackground.Cleared)
					{
						_profile.Background.Clear(true);
					}
					else
					{
						_profile.Background.SuppressEvents = true;
						_profile.Background.Clear();
						_profile.Background.Size = _profile.Scaling.CanvasSize;
						_profile.Background.Visible = UndoBackground.Visible;
						_profile.Background.Filename = UndoBackground.Filename ?? string.Empty;
						_profile.Background.Color = UndoBackground.Color;
						_profile.Background.GridColor = UndoBackground.GridColor;
						_profile.Background.Brightness = UndoBackground.Brightness;
						_profile.Background.Hue = UndoBackground.Hue;
						_profile.Background.Saturation = UndoBackground.Saturation;
						_profile.Background.OverlayGrid = UndoBackground.OverlayGrid;
						_profile.Background.TempImageFilename = UndoBackground.TempImageFilename;
						_profile.Background.TempCompositeImageFilename = UndoBackground.TempCompositeImageFilename;

						// Set the background image into the Canvas
						_profile.Background.Set(false);

						_profile.Background.SuppressEvents = false;
					}
				}
			}

			#endregion [ Background ]

			#region [ Scaling ]

			Scaling Scaling = changeset.Scaling(Undoing);
			if (Scaling != null)
			{
				if (!Scaling.LatticeSize.IsEmpty)
				{
					_profile.Scaling.LatticeSize = Scaling.LatticeSize;
					ChangedScaling = true;
				}

				if (Scaling.CellSize != null)
				{
					_profile.Scaling.CellSize = Scaling.CellSize.GetValueOrDefault();
					ChangedScaling = true;
				}

				if (Scaling.ShowGridLines != null)
				{
					_profile.Scaling.ShowGridLines = Scaling.ShowGridLines.GetValueOrDefault(true);
					ChangedScaling = true;
				}

				if (Scaling.Zoom != null)
				{
					_profile.Scaling.Zoom = Scaling.Zoom.GetValueOrDefault();
					//ChangedScaling = true;
				}

				//if (ChangedScaling)
				//	_profile.RecalculateScalingValues();
			}

			#endregion [ Scaling ]
			
			#region [ Mask ]

			Serialized = changeset.Mask(Undoing);
			if (Serialized.Length > 0)
			{
				Mask = Extends.DeserializeObjectFromXml<Mask>(Serialized);
				if (Mask != null)
					_profile.SetMask(Mask);
			}

			#endregion [ Mask ]

			#region [ Channels ]

			// Deserialize Channel data
			SerialList = changeset.Channels(Undoing);
			if (SerialList != null)
			{
				foreach(KeyValuePair<int, string> ChannelData in SerialList)
				{
					Channel = Extends.DeserializeObjectFromXml<Channel>(ChannelData.Value);
					TargetChannel = _profile.Channels.Get(Channel.ID);
					if (TargetChannel != null)
					{
						TargetChannel.SequencerColor = Channel.SequencerColor;
						TargetChannel.RenderColor = Channel.RenderColor;
						TargetChannel.BorderColor = Channel.BorderColor;
						TargetChannel.Name = Channel.Name;
						TargetChannel.Lattice = Channel.Lattice;
						TargetChannel.Enabled = Channel.Enabled;
						TargetChannel.Grouped = Channel.Grouped;
						TargetChannel.Included = Channel.Included;
						TargetChannel.Visible = Channel.Visible;
						TargetChannel.Origin = new Point(Channel.Origin.X, Channel.Origin.Y);
						// Trigger the object to regenerate this value;
						TargetChannel.ChannelExplorerImageKey = null;
					}
					else
						_profile.Channels.Add(Channel);
				}

				Channel = null;
				TargetChannel = null;
			}

			#endregion [ Channels ]

			#region [ ChannelList ]

			// Find any differences in the list of Channels.
			ListString = changeset.ChannelList(Undoing);
			if ((ListString ?? string.Empty).Length > 0)
			{
				UndoChannelIDs = new List<string>();
				UndoChannelIDs.AddRange(ListString.Split(','));
				ChannelIDs = new List<int>();
				
				foreach (Channel pChannel in _profile.Channels)
					ChannelIDs.Add(pChannel.ID);

				// Find any Channels that exists in ChannelIDs but not in UndoChannelIDs and remove them from the Profile.
				foreach (int ID in ChannelIDs)
				{
					if (!UndoChannelIDs.Contains(ID.ToString()))
						_profile.Channels.Remove(_profile.Channels.Get(ID));
				}
			}

			#endregion [ ChannelList ]

			#region [ Clipboard ]

			bool ClipboardChanged = false;
			string Data = changeset.ClipboardChannels(Undoing);

			// Deserialize Clipboard Channel data
			if (Data != UNCHANGED)
			{
				ClipboardChanged = true;
				List<string> ClipboardChannels = new List<string>();
				ClipboardChannels.AddRange(Data.Split(','));
				_workshop.Clipboard.Channels.Clear();
				foreach (string ChannelData in ClipboardChannels)
				{
					if (ChannelData.Length > 0)
					{
						Channel = new Channel();
						Channel.DeserializeLattice(ChannelData);
						_workshop.Clipboard.Channels.Add(Channel);
					}
				}
				Channel = null;
				ClipboardChannels = null;
			}

			Serialized = changeset.ClipboardMask(Undoing);
			if (Serialized != UNCHANGED)
			{
				ClipboardChanged = true;
				Mask = Extends.DeserializeObjectFromXml<Mask>(Serialized);
				if (Mask != null)
					_workshop.Clipboard.SetMask(Mask);
			}

			if (ClipboardChanged)
				_workshop.Clipboard.DisplayDiagnosticData();

			#endregion [ Clipboard ]

			#region [ ShuffleController ]

			Serialized = changeset.ShuffleController(Undoing);
			if (Serialized.Length > 0)
			{
				_profile.Channels.ShuffleController.Deserialize(Serialized);
			}

			#endregion [ ShuffleController ]

			if (ChangedScaling)
				_profile.Channels.UpdateChannels();

			_profile.SuppressEvents = false;
			_applyingChangeSet = false;

			UndoChannelIDs = null;
			ChannelIDs = null;

			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Clears out the Undo stacks
		/// </summary>
		/// <param name="suppressEvents">Indiciates whether the Changed events should supressed from firing when performing this task</param>
		public void Clear(bool suppressEvents)
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			if (_undoStack.Count > 0)
			{
				_undoStack.Clear();
				if (!suppressEvents)
					OnUndoChanged();
			}
			if (_redoStack.Count > 0)
			{
				_redoStack.Clear();
				if (!suppressEvents)
					OnRedoChanged();
			}
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Clears out the Undo stacks
		/// </summary>
		public void Clear()
		{
			Clear(false);
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(UndoController source)
		{
			if (source == null)
				return;

			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			SuppressEvents = true;

			_lastSnapshot = (Snapshot)source._lastSnapshot;
			Stack<UndoRedo> Temp = new Stack<UndoRedo>();

			// Flip the Undo stack over onto temp.
			while (_undoStack.Count > 0)
				Temp.Push(source._undoStack.Pop());

			// Now push all the items in temp into the original stack and this one.
			UndoRedo Item = null;
			while (Temp.Count > 0)
			{
				Item = Temp.Pop();
				_undoStack.Push(Item);
				source._undoStack.Push(Item);
			}

			// Flip the Redo stack over onto temp.
			while (_redoStack.Count > 0)
				Temp.Push(source._redoStack.Pop());

			// Now push all the items in temp into the original stack and this one.
			while (Temp.Count > 0)
			{
				Item = Temp.Pop();
				_redoStack.Push(Item);
				source._redoStack.Push(Item);
			}

			SuppressEvents = false;
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			base.DisposeChildObjects();

			if (_lastSnapshot != null)
			{
				_lastSnapshot.Dispose();
				_lastSnapshot = null;
			}
			UndoRedo Data = null;
			if (_undoStack != null)
			{
				while (_undoStack.Count > 0)
				{
					Data = _undoStack.Pop();
					if (Data != null)
					{
						Data.Dispose();
						Data = null;
					}
				}
				_undoStack = null;
			}
			if (_redoStack != null)
			{
				_redoStack = null;
				while (_redoStack.Count > 0)
				{
					Data = _redoStack.Pop();
					if (Data != null)
					{
						Data.Dispose();
						Data = null;
					}
				}
			}
			_profile = null;
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Compares the current snapshot to the last one, records the differences in a ChangeSet
		/// </summary>
		/// <param name="action">Text to indicate the last operation the program performed to warrent undoing</param>
		/// <param name="current">The current Snapshot of the data</param>
		/// <returns>ChangeSet object that holds the differences between the current snapshot and the last one</returns>
		private UndoRedo FindSnapshotChanges(string action, Snapshot current)
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			//_workshop.WriteTraceMessage("Action: " + action, TraceLevel.Verbose);

			UndoRedo Changes = new UndoRedo(action);

			if ((current == null) || (_lastSnapshot == null))
				return Changes;

			string FoundSerialized = string.Empty;

			// Background
			if (current.Data.Background != _lastSnapshot.Data.Background)
			{
				Changes.Undo.Background = _lastSnapshot.Data.Background;
				Changes.Redo.Background = current.Data.Background;
			}

			// Scaling
			if (current.Data.Scaling.LatticeSize != _lastSnapshot.Data.Scaling.LatticeSize)
			{
				Changes.Undo.Scaling.LatticeSize = _lastSnapshot.Data.Scaling.LatticeSize;
				Changes.Redo.Scaling.LatticeSize = current.Data.Scaling.LatticeSize;
			}

			if (current.Data.Scaling.CellSize != _lastSnapshot.Data.Scaling.CellSize)
			{
				Changes.Undo.Scaling.CellSize = _lastSnapshot.Data.Scaling.CellSize;
				Changes.Redo.Scaling.CellSize = current.Data.Scaling.CellSize;
			}

			if (current.Data.Scaling.ShowGridLines != _lastSnapshot.Data.Scaling.ShowGridLines)
			{
				Changes.Undo.Scaling.ShowGridLines = _lastSnapshot.Data.Scaling.ShowGridLines;
				Changes.Redo.Scaling.ShowGridLines = current.Data.Scaling.ShowGridLines;
			}

			if (current.Data.Scaling.Zoom != _lastSnapshot.Data.Scaling.Zoom)
			{
				Changes.Undo.Scaling.Zoom = _lastSnapshot.Data.Scaling.Zoom;
				Changes.Redo.Scaling.Zoom = current.Data.Scaling.Zoom;
			}

			// Mask
			if (current.Data.Mask != _lastSnapshot.Data.Mask)
			{
				Changes.Undo.Mask = _lastSnapshot.Data.Mask;
				Changes.Redo.Mask = current.Data.Mask;
			}
	
			// Channel List
			if (current.Data.ChannelList != _lastSnapshot.Data.ChannelList)
			{
				Changes.Undo.ChannelList = _lastSnapshot.Data.ChannelList;
				Changes.Redo.ChannelList = current.Data.ChannelList;
			}

			// Channels
			foreach (KeyValuePair<int, string> CurrentChannel in current.Data.Channels)
			{ 
				// Find this channel ID in the last snapshot.
				if (_lastSnapshot.Data.Channels.ContainsKey(CurrentChannel.Key))
					FoundSerialized = _lastSnapshot.Data.Channels[CurrentChannel.Key];
				else
					FoundSerialized = string.Empty;

				// If the Channel was not found in the last snapshot, then add it to the list of Redo changes
				if ((FoundSerialized ?? string.Empty).Length == 0)
				{
					Changes.Redo.Channels.Add(CurrentChannel.Key, CurrentChannel.Value);
					continue;
				}
				// If there are differences between the current and last versions of this Channel, then add to Undo and Redo
				if (FoundSerialized != CurrentChannel.Value)
				{
					Changes.Undo.Channels.Add(CurrentChannel.Key, FoundSerialized);
					Changes.Redo.Channels.Add(CurrentChannel.Key, CurrentChannel.Value);
				}
			}
			// Now look for removed channels
			foreach (KeyValuePair<int, string> OldChannel in _lastSnapshot.Data.Channels)
			{
				// Find this channel ID in the new snapshot.
				if (current.Data.Channels.ContainsKey(OldChannel.Key))
					FoundSerialized = current.Data.Channels[OldChannel.Key];
				else
					FoundSerialized = string.Empty;

				// If the Channel was not found in the new snapshot, then add it to the list of Undo changes
				if ((FoundSerialized ?? string.Empty).Length == 0)
				{
					Changes.Undo.Channels.Add(OldChannel.Key, OldChannel.Value);
					continue;
				}
			}

			// Clipboard 
			
			// Mask
			if (current.Data.ClipboardMask != _lastSnapshot.Data.ClipboardMask)
			{
				Changes.Undo.ClipboardMask = _lastSnapshot.Data.ClipboardMask;
				Changes.Redo.ClipboardMask = current.Data.ClipboardMask;
			}
			else
			{
				Changes.Undo.ClipboardMask = UNCHANGED;
				Changes.Redo.ClipboardMask = UNCHANGED;
			}

			// Channels
			if (current.Data.ClipboardChannels != _lastSnapshot.Data.ClipboardChannels)
			{
				Changes.Undo.ClipboardChannels = _lastSnapshot.Data.ClipboardChannels;
				Changes.Redo.ClipboardChannels = current.Data.ClipboardChannels;
			}
			else
			{
				Changes.Undo.ClipboardChannels = UNCHANGED;
				Changes.Redo.ClipboardChannels = UNCHANGED;
			}

			// ShuffleController
			if (current.Data.ShuffleController != _lastSnapshot.Data.ShuffleController)
			{
				Changes.Undo.ShuffleController = _lastSnapshot.Data.ShuffleController;
				Changes.Redo.ShuffleController = current.Data.ShuffleController;
			}
			else
			{
				Changes.Undo.ShuffleController = UNCHANGED;
				Changes.Redo.ShuffleController = UNCHANGED;
			}

			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
			return Changes;
		}

		/// <summary>
		/// Creates the initial snapshot of the data
		/// </summary>
		public void GetInitialSnapshot()
		{
			if (_workshop.UI.DisableUndo)
				return;

			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			_lastSnapshot = new Snapshot(_profile);
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Rewinds the last Undo performed, reapplying the changes
		/// </summary>
		public void Redo()
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);

			if (_redoStack.Count == 0)
			{
				//_workshop.WriteTraceMessage("Stack Empty, Exit", TraceLevel.Error);
				return;
			}

			UndoRedo Changes = _redoStack.Pop();
			_undoStack.Push(Changes);
			
			ApplyChangeset(Changes, Activity.Redo);
			
			// Get a new Snapshot of the data
			_lastSnapshot = new Snapshot(_profile);

			// Fire the events
			OnRedoChanged();
			OnUndoChanged();
			OnCompleted();
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		/// <summary>
		/// The program has just performed an operation that can be undone. Grab a snapshot of the data
		/// and save the differences between this and the last as a Changeset
		/// </summary>
		/// <param name="action">Text of the operation complete, this will appear in the Undo menu in the Editor</param>
		public void SaveUndo(string action)
		{
			if (_workshop.UI.DisableUndo)
				return;

			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			//_workshop.WriteTraceMessage("Action: " + action, TraceLevel.Verbose);

			if (_applyingChangeSet)
			{
				//_workshop.WriteTraceMessage("Stack Empty, Exit", TraceLevel.Error);
				return;
			}

			Snapshot Current = new Snapshot(_profile);

			// Find the changes between the current Snapshot and the last snapshot taken.
			UndoRedo Changes = FindSnapshotChanges(action, Current);

			if (!Changes.IsEmpty)
			{
				_undoStack.Push(Changes);
				_redoStack.Clear();
			}

			_lastSnapshot = Current;
			Current = null;
		
			// Fire the events
			OnRedoChanged();
			OnUndoChanged();
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}
				
		/// <summary>
		/// Looks at the last set of changes and applies the old values.
		/// </summary>
		public void Undo()
		{
			//_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			if (_undoStack.Count == 0)
			{
				//_workshop.WriteTraceMessage("Stack Empty, Exit", TraceLevel.Error);
				return;
			}

			UndoRedo Changes = _undoStack.Pop();
			_redoStack.Push(Changes);

			ApplyChangeset(Changes, Activity.Undo);
			
			// Get a new Snapshot of the data
			_lastSnapshot = new Snapshot(_profile);
			
			// Fire the events
			OnRedoChanged();
			OnUndoChanged();
			OnCompleted();
			//_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		#region [ Debug Methods ]

		/// <summary>
		/// Returns a text representative of the data in the Redo stack.
		/// </summary>
		public string Debug_RedoStack()
		{
			StringBuilder Output = new StringBuilder();
			Stack<UndoRedo> TempStack = new Stack<UndoRedo>();
			UndoRedo Data = null;
			// Pop out each data item, get the contents, and then push it onto the temporary stack.
			while (_redoStack.Count > 0)
			{
				Data = _redoStack.Pop();
				if (Output.Length > 0)
					Output.AppendLine("-----------------------------------------------------------------");
				Output.AppendLine(Data.ToDebugString(false));
				TempStack.Push(Data);
			}
			// Push the data back onto the original stack.
			while (TempStack.Count > 0)
				_redoStack.Push(TempStack.Pop());

			TempStack = null;
			Data = null;
			return Output.ToString();
		}

		/// <summary>
		/// Returns a text representative of the data in the Undo stack.
		/// </summary>
		public string Debug_UndoStack()
		{
			StringBuilder Output = new StringBuilder();
			Stack<UndoRedo> TempStack = new Stack<UndoRedo>();
			UndoRedo Data = null;
			// Pop out each data item, get the contents, and then push it onto the temporary stack.
			while (_undoStack.Count > 0)
			{
				Data = _undoStack.Pop();
				if (Output.Length > 0)
					Output.AppendLine("-----------------------------------------------------------------");
				Output.AppendLine(Data.ToDebugString(true));
				TempStack.Push(Data);
			}
			// Push the data back onto the original stack.
			while (TempStack.Count > 0)
				_undoStack.Push(TempStack.Pop());

			TempStack = null;
			Data = null;
			return Output.ToString();
		}

		/// <summary>
		/// Returns a text version of the current Snapshot
		/// </summary>
		/// <returns></returns>
		public string Debug_UndoSnapshot()
		{
			if ((_lastSnapshot != null) && (_lastSnapshot.Data != null))
				return _lastSnapshot.Data.ToDebugString();
			else
				return string.Empty;
		}

		#endregion [ Debug Methods ]

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		/// <summary>
		/// Occurs when the Redo stack changes
		/// </summary>
		private void OnRedoChanged()
		{
			if (RedoChanged == null)
				return;
			bool HasData = (_redoStack.Count > 0);
			string TopMost = HasData ? _redoStack.Peek().ToString() : string.Empty;
			RedoChanged(this, new UndoEventArgs(TopMost, HasData));
		}

		/// <summary>
		/// Occurs when the Undo stack changes
		/// </summary>
		private void OnUndoChanged()
		{
			if (UndoChanged == null)
				return;
			bool HasData = (_undoStack.Count > 0);
			string TopMost = HasData ? _undoStack.Peek().ToString() : string.Empty;
			UndoChanged(this, new UndoEventArgs(TopMost, HasData));
		}

		/// <summary>
		/// Occurs when an Undo or Redo event is triggered.
		/// </summary>
		private void OnCompleted()
		{
			if (Completed != null)
				Completed(this, new EventArgs());
		}

		#endregion [ Event Triggers ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when either an Undo or a Redo is triggered.
		/// </summary>
		public EventHandler Completed;

		/// <summary>
		/// Occurs when a new item is the top item on the Redo stack
		/// </summary>
		public EventHandlers.UndoEventHandler RedoChanged;

		/// <summary>
		/// Occurs when a new item is the top item on the Undo stack
		/// </summary>
		public EventHandlers.UndoEventHandler UndoChanged;

		#endregion [ Event Handlers ]

		#endregion [ Events ]
	}
}
