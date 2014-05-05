using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ElfCore
{
	/// <summary>
	/// Handles all the Undo and Redo operations by taking a snapshot of the data before and after an operation and saves the differences.
	/// </summary>
	public class UndoController
	{
		#region [ Enum ]

		enum Activity : int
		{ 
			Undo,
			Redo
		}

		#endregion [ Enum ]

		//#region [ Declares ]

		//[DllImport("msvcrt.dll")]
		//private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

		//#endregion [ Declares ]

		#region [ Public Static Variables ]

		//public static bool Undoing = false;

		#endregion [ Public Static Variables ]

		#region [ Private Variables ]

		public UndoData.Snapshot _lastSnapshot = null;
		public Stack<UndoData.UndoRedo> _undoStack = null;
		public Stack<UndoData.UndoRedo> _redoStack = null;
		private bool _applyingChangeSet = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Determines if there are any Redos waiting on the stack
		/// </summary>
		public bool HasRedo
		{
			get { return (_redoStack.Count > 0); }
		}

		/// <summary>
		/// Determines if there are any Undos waiting on the stack
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
		{
			_undoStack = new Stack<UndoData.UndoRedo>();
			_redoStack = new Stack<UndoData.UndoRedo>();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Applies changes to the data
		/// </summary>
		/// <param name="undoRedoData">Set of data changes, either from an Undo or a Redo event</param>
		/// <param name="useUndoChangeSet">Indicates whether the Undo or the Redo Changeset should be used</param>
		private void ApplyChangeset(UndoData.UndoRedo undoRedoData, Activity activity)
		{
			_applyingChangeSet = true;
			bool UseUndoChangeSet = (activity == Activity.Undo);
			Workshop Workshop = Workshop.Instance;

			Debug.WriteLine("\n" + (UseUndoChangeSet ? "Undo " : "Redo ") + undoRedoData.Action);
			Editor.Stopwatch.Reset();
			Editor.Stopwatch.Start();

			if (undoRedoData == null)
				return;

			UndoData.Background Background = undoRedoData.BackgroundImage(UseUndoChangeSet);
			if (Background != null)
			{
				if (Background.Cleared)
				{
					Workshop.UI.Background.Clear(true);
				}
				else
				{
					if (Background.BaseImage != null)
					{
						if (Workshop.UI.Background.Image != null)
							Workshop.UI.Background.Image.Dispose();
						Workshop.UI.Background.Image = new Bitmap(Background.BaseImage);
						Workshop.UI.Background.Size = Background.BaseImage.Size;
					}
					if (Background.Visible != null)
						Workshop.UI.Background.Visible = Background.Visible.GetValueOrDefault();
					if (Background.Filename != null)
						Workshop.UI.Background.Filename = Background.Filename ?? string.Empty;
					if ((Background.Color != null) && !Background.Color.IsEmpty)
						Workshop.UI.Background.Color = Background.Color;
					if (Background.Brightness != null)
						Workshop.UI.Background.Brightness = Background.Brightness.GetValueOrDefault();
					
					// Set the background image into the Canvas
					Workshop.UI.Background.Set();
				}
			}

			UndoData.Interface UI = undoRedoData.UI(UseUndoChangeSet);
			if (UI != null)
			{
				if (UI.LatticeSize != null)
					Workshop.UI.LatticeSize = UI.LatticeSize.GetValueOrDefault();

				if (UI.CellSize != null)
					Workshop.UI.CellSize = UI.CellSize.GetValueOrDefault();

				if (UI.GridLineWidth != null)
					Workshop.UI.GridLineWidth = UI.GridLineWidth.GetValueOrDefault();

				if (UI.InactiveChannelAlpha != null)
					Workshop.UI.InactiveChannelAlpha = UI.InactiveChannelAlpha.GetValueOrDefault();

				if (UI.Zoom != null)
					Workshop.UI.Zoom = UI.Zoom.GetValueOrDefault();
			}
	
			if (undoRedoData.Mask(UseUndoChangeSet) != null)
				Workshop.SetMask(undoRedoData.Mask(UseUndoChangeSet));

			if (undoRedoData.Channels(UseUndoChangeSet) != null)
			{
				Channel TargetChannel = null;
				foreach (Channel CS_Channel in undoRedoData.Channels(UseUndoChangeSet))
				{
					TargetChannel = Workshop.Channels.GetChannelByID(CS_Channel.ID);
					if (TargetChannel == null)
						continue;
					if (!TargetChannel.Color.Equals(CS_Channel.Color))
						TargetChannel.Color = CS_Channel.Color;
					if (TargetChannel.Name != CS_Channel.Name)
						TargetChannel.Name = CS_Channel.Name;
					TargetChannel.Lattice = CS_Channel.Lattice;
				}
			}

			Editor.Stopwatch.Stop();
			Debug.WriteLine(Editor.Stopwatch.Report());

			_applyingChangeSet = false;
		}

		/// <summary>
		/// Clears out the Undo stacks
		/// </summary>
		public void Clear()
		{
			if (_undoStack.Count > 0)
			{
				_undoStack.Clear();
				OnUndoChanged();
			}
			if (_redoStack.Count > 0)
			{
				_redoStack.Clear();
				OnRedoChanged();
			}
		}

		/// <summary>
		/// Compares the current snapshot to the last one, records the differences in a ChangeSet
		/// </summary>
		/// <param name="action">Text to indicate the last operation the program performed to warrent undoing</param>
		/// <param name="current">The current Snapshot of the data</param>
		/// <param name="createUndo">Indicates whether to create data for an Undo or a Redo</param>
		/// <returns>UndoData.ChangeSet object that holds the differences between the current snapshot and the last one</returns>
		private UndoData.UndoRedo FindSnapshotChanges(string action, UndoData.Snapshot current)
		{
			UndoData.UndoRedo Changes = new UndoData.UndoRedo(action);

			// If the background was cleared before and currently, then just copy the data over whole-hog 
			if (current.Data.BackgroundImage.Cleared != _lastSnapshot.Data.BackgroundImage.Cleared)
			{
				Changes.Undo.BackgroundImage.Assign(_lastSnapshot);
				Changes.Redo.BackgroundImage.Assign(current);
			}
			else
			{
				if (!Workshop.BitmapEquals(current.Data.BackgroundImage.BaseImage, _lastSnapshot.Data.BackgroundImage.BaseImage))
				{
					Changes.Undo.BackgroundImage.BaseImage = _lastSnapshot.Data.BackgroundImage.BaseImage;
					Changes.Redo.BackgroundImage.BaseImage = current.Data.BackgroundImage.BaseImage;
				}

				if (string.Compare(current.Data.BackgroundImage.Filename, _lastSnapshot.Data.BackgroundImage.Filename, true) != 0)
				{
					Changes.Undo.BackgroundImage.Filename = _lastSnapshot.Data.BackgroundImage.Filename;
					Changes.Redo.BackgroundImage.Filename = current.Data.BackgroundImage.Filename;
				}

				if (current.Data.BackgroundImage.Brightness != _lastSnapshot.Data.BackgroundImage.Brightness)
				{
					Changes.Undo.BackgroundImage.Brightness = _lastSnapshot.Data.BackgroundImage.Brightness;
					Changes.Redo.BackgroundImage.Brightness = current.Data.BackgroundImage.Brightness;
				}

				if (current.Data.BackgroundImage.Visible != _lastSnapshot.Data.BackgroundImage.Visible)
				{
					Changes.Undo.BackgroundImage.Visible = _lastSnapshot.Data.BackgroundImage.Visible;
					Changes.Redo.BackgroundImage.Visible = current.Data.BackgroundImage.Visible;
				}

				if (!current.Data.BackgroundImage.Color.Equals(_lastSnapshot.Data.BackgroundImage.Color))
				{
					Changes.Undo.BackgroundImage.Color = _lastSnapshot.Data.BackgroundImage.Color;
					Changes.Redo.BackgroundImage.Color = current.Data.BackgroundImage.Color;
				}

				Changes.Undo.BackgroundImage.Cleared = _lastSnapshot.Data.BackgroundImage.Cleared;
				Changes.Redo.BackgroundImage.Cleared = current.Data.BackgroundImage.Cleared;
			}

			if (current.Data.UI.LatticeSize != _lastSnapshot.Data.UI.LatticeSize)
			{
				Changes.Undo.UI.LatticeSize = _lastSnapshot.Data.UI.LatticeSize;
				Changes.Redo.UI.LatticeSize = current.Data.UI.LatticeSize;
			}

			if (current.Data.UI.CellSize != _lastSnapshot.Data.UI.CellSize)
			{
				Changes.Undo.UI.CellSize = _lastSnapshot.Data.UI.CellSize;
				Changes.Redo.UI.CellSize = current.Data.UI.CellSize;
			}

			if (current.Data.UI.GridLineWidth != _lastSnapshot.Data.UI.GridLineWidth)
			{
				Changes.Undo.UI.GridLineWidth = _lastSnapshot.Data.UI.GridLineWidth;
				Changes.Redo.UI.GridLineWidth = current.Data.UI.GridLineWidth;
			}

			if (current.Data.UI.InactiveChannelAlpha != _lastSnapshot.Data.UI.InactiveChannelAlpha)
			{
				Changes.Undo.UI.InactiveChannelAlpha = _lastSnapshot.Data.UI.InactiveChannelAlpha;
				Changes.Redo.UI.InactiveChannelAlpha = current.Data.UI.InactiveChannelAlpha;
			}

			if (current.Data.UI.Zoom != _lastSnapshot.Data.UI.Zoom)
			{
				Changes.Undo.UI.Zoom = _lastSnapshot.Data.UI.Zoom;
				Changes.Redo.UI.Zoom = current.Data.UI.Zoom;
			}

			if (current.Data.Mask.Differs(_lastSnapshot.Data.Mask))
			{
				Changes.Undo.Mask = _lastSnapshot.Data.Mask;
				Changes.Redo.Mask = current.Data.Mask;
			}

			for (int i = 0; i < current.Data.Channels.Count; i++)
				if (current.Data.Channels[i].Differs(_lastSnapshot.Data.Channels[i]))
				{
					if (Changes.Undo.Channels == null)
						Changes.Undo.Channels = new List<Channel>();
					if (Changes.Redo.Channels == null)
						Changes.Redo.Channels = new List<Channel>();
					Changes.Undo.Channels.Add(_lastSnapshot.Data.Channels[i]);
					Changes.Redo.Channels.Add(current.Data.Channels[i]);
				}

			return Changes;
		}

		/// <summary>
		/// Creates the initial snapshot of the data
		/// </summary>
		public void GetInitialSnapshot()
		{
			_lastSnapshot = new UndoData.Snapshot();
		}

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
		/// Rewinds the last Undo performed, reapplying the changes
		/// </summary>
		public void Redo()
		{
			if (_redoStack.Count == 0)
				return;
			UndoData.UndoRedo Changes = _redoStack.Pop();
			_undoStack.Push(Changes);
			ApplyChangeset(Changes, Activity.Redo);
			// Get a new Snapshot of the data
			_lastSnapshot = new UndoData.Snapshot();

			// Fire the events
			OnRedoChanged();
			OnUndoChanged();

		}

		/// <summary>
		/// The program has just performed an operation that can be undone. Grab a snapshot of the data
		/// and save the differences between this and the last as a Changeset
		/// </summary>
		/// <param name="action">Text of the operation complete, this will appear in the Undo menu in the Editor</param>
		public void SaveUndo(string action)
		{
			if (Editor.Loading || _applyingChangeSet)
				return;

			UndoData.Snapshot Current = new UndoData.Snapshot();

			// Find the changes between the current Snapshot and the last snapshot taken.
			UndoData.UndoRedo Changes = FindSnapshotChanges(action, Current);

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
		}
				
		/// <summary>
		/// Looks at the last set of changes and applies the old values.
		/// </summary>
		public void Undo()
		{
			if (_undoStack.Count == 0)
				return;
			UndoData.UndoRedo Changes = _undoStack.Pop();
			_redoStack.Push(Changes);
			ApplyChangeset(Changes, Activity.Undo);
			// Get a new Snapshot of the data
			_lastSnapshot = new UndoData.Snapshot();
			
			// Fire the events
			OnRedoChanged();
			OnUndoChanged();
		}

		#endregion [ Methods ]

		#region [ Event Handlers ]

		public delegate void UndoEventHandler(object sender, UndoEventArgs e);

		/// <summary>
		/// Occurs when a new item is the top item on the Undo stack
		/// </summary>
		public UndoEventHandler UndoChanged;

		/// <summary>
		/// Occurs when a new item is the top item on the Redo stack
		/// </summary>
		public UndoEventHandler RedoChanged;

		#endregion [ Event Handlers ]
	}
}
