using ElfCore.Controllers;
using ElfCore.Profiles;
using System.Collections.Generic;
using System.Text;

using ElfCore.Util;

namespace ElfCore.Core.UndoData
{
	#region [ Class ChangeSet ]

	/// <summary>
	/// Stores the differences between one snapshot and another.
	/// </summary>
	public class ChangeSet : ElfBase
	{
		#region [ Constants ]

		private const string EMPTY = "EMPTY";
		private const string TAB = "\t";

		#endregion [ Constants ]

		#region [ Properties ]

		//public byte? InactiveChannelAlpha = null;

		/// <summary>
		/// Serialized Background data.
		/// </summary>
		public string Background { get; set; }

		/// <summary>
		/// Serialized UI data.
		/// </summary>
		public Scaling Scaling { get; set; }

		/// <summary>
		/// List of all serialized Channel data.
		/// </summary>
		public SortedList<int, string> Channels { get; set; }

		/// <summary>
		/// List of all serialized Channel data from the Clipboard.
		/// </summary>
		public string ClipboardChannels { get; set; }

		/// <summary>
		/// Comma seperated string holding the IDs of the Channels.
		/// </summary>
		public string ChannelList { get; set; }

		/// <summary>
		/// Encoded string for the ShuffleController
		/// </summary>
		public string ShuffleController { get; set; }

		/// <summary>
		/// Gets a value indicating whether this ChangeSet is empty. If any of the objects or data that was 
		/// initially set to null is not null, then returns false. 
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				if ((Background ?? string.Empty).Length > 0)
					return false;
				if ((ChannelList ?? string.Empty).Length > 0)
					return false;
				if (!Scaling.IsEmpty)
					return false;
				if ((Mask ?? string.Empty).Length > 0)
					return false;
				if ((Channels != null) && (Channels.Count > 0))
					return false;
				if ((ClipboardMask ?? string.Empty).Length > 0)
					return false;
				if ((ClipboardChannels ?? string.Empty).Length > 0)
					return false;
				if ((ShuffleController ?? string.Empty).Length > 0)
					return false;
				//if (InactiveChannelAlpha != null)
					//return false;
				return true;
			}
		}

		/// <summary>
		/// Serialized Mask data.
		/// </summary>
		public string Mask { get; set; }

		/// <summary>
		/// Serialized Mask from the Clipboard.
		/// </summary>
		public string ClipboardMask { get; set; }

		#endregion [ Properties ]

		#region [ Constructor ]

		public ChangeSet()
			: base()
		{
			Background = string.Empty;
			ChannelList = null;
			Channels = new SortedList<int, string>();
			ClipboardChannels = string.Empty;
			Mask = string.Empty;
			ClipboardMask = string.Empty;
			//this.InactiveChannelAlpha = null;
			Scaling = new Scaling();
			ShuffleController = string.Empty;
		}

		#endregion [ Constructor ]

		#region [ Methods ]

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			if (Scaling != null)
			{
				Scaling.Dispose();
				Scaling = null;
			}
			if (Channels != null)
			{
				Channels.Clear();
				Channels = null;
			}
		}

		/// <summary>
		/// Returns a string representation of the data contained in this object.
		/// </summary>
		public override string ToDebugString()
		{
			if (IsEmpty)
				return EMPTY;

			StringBuilder Output = new StringBuilder();

			Output.AppendLine("BACKGROUND:");
			if (Background.Length == 0)
				Output.AppendLine(TAB + EMPTY);
			else
				Output.AppendLine(FormatForDebug(Background));

			Output.AppendLine("SCALING:");
			if (Scaling.IsEmpty)
				Output.AppendLine(TAB + EMPTY);
			else
				Output.AppendLine(Scaling.ToDebugString(1));

			Output.AppendLine("MASK:");
			if (Mask.Length == 0)
				Output.AppendLine(TAB + EMPTY);
			else
				Output.AppendLine(FormatForDebug(Mask));

			//Output.AppendLine("INACTIVE CHANNEL ALPHA:");
			//Output.AppendLine(TAB + ((this.InactiveChannelAlpha == null) ? EMPTY : this.InactiveChannelAlpha.GetValueOrDefault().ToString()));

			Output.AppendLine("CHANNEL LIST:");
			Output.AppendLine(FormatForDebug(ChannelList));

			Output.AppendLine("CHANNELS: ");
			if ((Channels != null) && (Channels.Count > 0))
			{
				foreach (KeyValuePair<int, string> KVP in Channels)
				{
					Output.AppendLine(TAB + "CHANNEL: " + KVP.Key);
					Output.AppendLine(FormatForDebug(KVP.Value));
				}
			}
			else
				Output.AppendLine(TAB + EMPTY);

			Output.AppendLine("CLIPBOARD MASK:");
			if (ClipboardMask.Length == 0)
				Output.AppendLine(TAB + EMPTY);
			else
				Output.AppendLine(FormatForDebug(ClipboardMask));

			Output.AppendLine("CLIPBOARD CHANNEL DATA:");
			if (ClipboardChannels.Length == 0)
				Output.AppendLine(TAB + EMPTY);
			else
				Output.AppendLine(FormatForDebug(ClipboardChannels));

			Output.AppendLine("SORT ORDER LIST:");
			Output.AppendLine(FormatForDebug(ShuffleController));

			return Output.ToString();
		}

		/// <summary>
		/// Clean up the Xml for displaying for the Diagnostic panes.
		/// </summary>
		private string FormatForDebug(string value)
		{
			if (value == null)
				return TAB + EMPTY;
			value = TAB + value.Replace(TAB, string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("    ", " ");
			value = value.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
			value = value.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty);
			return value;
		}

		#endregion [ Methods ]
	}

	#endregion [ Class ChangeSet ]

	#region [ Class UndoRedo ]

	/// <summary>
	/// Stores the differences between one snapshot and another.
	/// </summary>
	public class UndoRedo : ElfBase
	{
		#region [ Properties ]

		/// <summary>
		/// Text of the action to undo, to be displayed in the Undo menu under Edit
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// Indicates whether the ChangeSets are empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Undo.IsEmpty && Redo.IsEmpty; }
		}

		/// <summary>
		/// Undo data
		/// </summary>
		public ChangeSet Undo { get; set; }

		/// <summary>
		/// Redo data
		/// </summary>
		public ChangeSet Redo { get; set; }

		#endregion [ Properties ]

		#region [ Constructor ]

		public UndoRedo()
			: base()
		{
			Action = string.Empty;
			Undo = new ChangeSet();
			Redo = new ChangeSet();
		}

		public UndoRedo(string action)
			: this()
		{
			Action = action;
		}

		#endregion [ Constructor ]

		#region [ Methods ]

		/// <summary>
		/// Background Image
		/// </summary>
		public string Background(bool undoData)
		{
			return undoData ? Undo.Background : Redo.Background;
		}

		/// <summary>
		/// User interface
		/// </summary>
		public Scaling Scaling(bool undoData)
		{
			return undoData ? Undo.Scaling : Redo.Scaling;
		}

		//public byte? InactiveChannelAlpha(bool undoData)
		//{
		//	return undoData ? Undo.InactiveChannelAlpha : Redo.InactiveChannelAlpha;
		//}

		/// <summary>
		/// List of all Channels that have been changed.
		/// </summary>
		public SortedList<int, string> Channels(bool undoData)
		{
			return undoData ? Undo.Channels : Redo.Channels;
		}

		/// <summary>
		/// List of the data in the Clipboard channels.
		/// </summary>
		public string ClipboardChannels(bool undoData)
		{
			return undoData ? Undo.ClipboardChannels : Redo.ClipboardChannels;
		}

		/// <summary>
		/// Object that holds the mask information from the Clipboard.
		/// </summary>
		public string ClipboardMask(bool undoData)
		{
			return undoData ? Undo.ClipboardMask : Redo.ClipboardMask;
		}

		/// <summary>
		/// List of all Channels present in the Profile.
		/// </summary>
		public string ChannelList(bool undoData)
		{
			return undoData ? Undo.ChannelList : Redo.ChannelList;
		}

		/// <summary>
		/// Object that holds the mask information for both the Canvas and the Lattice
		/// </summary>
		public string Mask(bool undoData)
		{
			return undoData ? Undo.Mask : Redo.Mask;
		}

		public string ShuffleController(bool undoData)
		{ 
			return undoData ? Undo.ShuffleController : Redo.ShuffleController;
		}

		public override string ToString()
		{
			return Action ?? string.Empty;
		}

		/// <summary>
		/// Returns a string representation of the data contained in this object.
		/// </summary>
		/// <param name="showUndoData">Indicates if the Undo part of the dataset should be output.</param>
		public string ToDebugString(bool showUndoData)
		{
			StringBuilder Output = new StringBuilder();

			Output.AppendLine("ACTION: " + Action);
			if (showUndoData)
				Output.Append(Undo.ToDebugString());
			else
				Output.Append(Redo.ToDebugString());
			return Output.ToString();
		}

		#endregion [ Methods ]
	}

	#endregion [ Class UndoRedo ]

	#region [ Class Snapshot ]

	/// <summary>
	/// Current status of the current Profile and settings, saved in a ChangeSet object.
	/// </summary>
	public class Snapshot : ElfBase
	{
		#region [ Public Fields ]

		public ChangeSet Data { get; private set; }

		#endregion [ Public Fields ]

		#region [ Constructor ]

		/// <summary>
		/// Initializes the object by taking a snapshot of the data.
		/// </summary>
		/// <param name="profile">Profile data to capture</param>
		public Snapshot(BaseProfile profile)
			: base()
		{
			//Controllers.Workshop.Instance.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			Data = new ChangeSet();
			
			string List = string.Empty;
			string Serialized = string.Empty;

			// Record all the current UI and Background settings into the ChangeSet
			//Data.Background = Util.Extends.SerializeObjectToXml<Background>(profile.Background);
			Data.Background = profile.Background.Serialized;

			Data.Scaling = new Scaling
			{
				LatticeSize = profile.Scaling.LatticeSize,
				CellSize = profile.Scaling.CellSize,
				ShowGridLines = profile.Scaling.ShowGridLines,
				Zoom = profile.Scaling.Zoom
			};

			// Record the current Mask
			Data.Mask = profile.SerializeMask();

			// Record a copy of all the Channels
			List = string.Empty;
			for (int i = 0; i < profile.Channels.Count; i++)
			{
				List += ((List.Length > 0) ? "," : string.Empty) + profile.Channels[i].ID;
				//Serialized = Util.Extends.SerializeObjectToXml<RasterChannel>(profile.Channels[i]);
				//Data.Channels.Add(profile.Channels[i].ID, Serialized);
				Data.Channels.Add(profile.Channels[i].ID, profile.Channels[i].Serialized);
			}
			Data.ChannelList = List;

			// Record a copy of the data in the Clipboard
			Workshop Workshop = Workshop.Instance;
			Serialized = string.Empty;
			for (int i = 0; i < Workshop.Clipboard.Channels.Count; i++)
			{
				//Serialized += (Serialized.Length > 0 ? "," : string.Empty) + Workshop.Clipboard.Channels[i].SerializeLattice(false);
				Serialized += (Serialized.Length > 0 ? "," : string.Empty) + Workshop.Clipboard.Channels[i].Serialized;
			}
			Data.ClipboardChannels = Serialized;

			// Record the Mask stored in the Clipboard
			//Data.ClipboardMask = Util.Extends.SerializeObjectToXml<Mask>(Workshop.Clipboard.Mask);
			Data.ClipboardMask = Workshop.Clipboard.Mask.Serialized;
			
			// Record a copy of all the Shuffles
			Data.ShuffleController = Extends.SerializeObjectToXml<ShuffleController>(profile.Channels.ShuffleController);
			//Data.ShuffleController = profile.Channels.ShuffleController.Serialized;

			//Controllers.Workshop.Instance.WriteTraceMessage("END", TraceLevel.Verbose);
		}

		#endregion [ Constructor ]

	}

	#endregion [ Class Snapshot ]
}
