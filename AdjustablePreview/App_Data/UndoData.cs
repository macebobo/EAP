using System;
using System.Collections.Generic;
using System.Drawing;

namespace ElfCore.UndoData
{
	#region [ ChangeSet ]

	/// <summary>
	/// Stores the differences between one snapshot and another.
	/// </summary>
	public class ChangeSet
	{
		#region [ Properties ]

		public Background BackgroundImage { get; set; }
		public Interface UI { get; set; }

		/// <summary>
		/// List of all Channels that have been changed.
		/// </summary>
		public List<Channel> Channels { get; set; }

		/// <summary>
		/// Gets a value indicating whether this ChangeSet is empty. If any of the objects or data that was 
		/// initially set to null is not null, then returns false. 
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				if (!BackgroundImage.IsEmpty)
					return false;
				if (!UI.IsEmpty)
					return false;
				if (Mask != null)
					return false;
				if (Channels != null)
					return false;
				return true;
			}
		}

		/// <summary>
		/// Object that holds the mask information for both the Canvas and the Lattice
		/// </summary>
		public Mask Mask { get; set; }

		#endregion [ Properties ]

		#region [ Constructor ]

		public ChangeSet()
		{			
			BackgroundImage = new Background();
			UI = new Interface();
			Mask = null;
			Channels = null;
		}

		#endregion [ Constructor ]
	}

	#endregion [ ChangeSet ]

	#region [ UndoRedo ]

	/// <summary>
	/// Stores the differences between one snapshot and another.
	/// </summary>
	public class UndoRedo
	{
		#region [ Public Fields ]

		/// <summary>
		/// Text of the action to undo, to be displayed in the Undo menu under Edit
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// Undo data
		/// </summary>
		public ChangeSet Undo { get; set; }

		/// <summary>
		/// Redo data
		/// </summary>
		public ChangeSet Redo { get; set; }

		#endregion [ Public Fields ]

		#region [ Properties ]

		/// <summary>
		/// Indicates whether the ChangeSets are empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Undo.IsEmpty && Redo.IsEmpty; }
		}

		#endregion [ Properties ]

		#region [ Constructor ]

		public UndoRedo()
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
		public UndoData.Background BackgroundImage(bool undoData)
		{
			return undoData ? Undo.BackgroundImage : Redo.BackgroundImage;
		}

		/// <summary>
		/// User interface
		/// </summary>
		public UndoData.Interface UI(bool undoData)
		{
			return undoData ? Undo.UI : Redo.UI;
		}

		///// <summary>
		///// Name of the file used to load the Background Image
		///// </summary>
		//public string BackgroundImage_Filename(bool undoData)
		//{
		//    return undoData ? Undo.BackgroundImage_Filename : Redo.BackgroundImage_Filename;
		//}

		///// <summary>
		///// Gets a value indicating whether the BackGround image should be displayed on the Canvas
		///// </summary>
		//public bool? BackgroundImage_Visible(bool undoData)
		//{
		//    return undoData ? Undo.BackgroundImage_Visible : Redo.BackgroundImage_Visible;
		//}

		///// <summary>
		///// Gets a value to change the brightness of the BackgroundImage. Valid range is -1.0 to +1.0
		///// </summary>
		//public float? BackgroundImage_Brightness(bool undoData)
		//{
		//    return undoData ? Undo.BackgroundImage_Brightness : Redo.BackgroundImage_Brightness;
		//}

		///// <summary>
		///// Size of the Cell in pixels with no zoom factored
		///// </summary>
		//public int? CellSize(bool undoData)
		//{
		//    return undoData ? Undo.CellSize : Redo.CellSize;
		//}

		/// <summary>
		/// List of all Channels that have been changed.
		/// </summary>
		public List<Channel> Channels(bool undoData)
		{
			return undoData ? Undo.Channels : Redo.Channels;
		}

		///// <summary>
		///// Width of the Grid Line between Cells. 0 indicates no lines should be shown.
		///// </summary>
		//public int? GridLineWidth(bool undoData)
		//{
		//    return undoData ? Undo.GridLineWidth : Redo.GridLineWidth;
		//}

		///// <summary>
		///// Amount of alpha (transparency) to use when displaying Channels that are not currently active.
		///// </summary>
		//public byte? InactiveChannelAlpha(bool undoData)
		//{
		//    return undoData ? Undo.InactiveChannelAlpha : Redo.InactiveChannelAlpha;
		//}

		///// <summary>
		///// Size of the Lattice
		///// </summary>
		//public Size? LatticeSize(bool undoData)
		//{
		//    return undoData ? Undo.LatticeSize : Redo.LatticeSize;
		//}

		/// <summary>
		/// Object that holds the mask information for both the Canvas and the Lattice
		/// </summary>
		public Mask Mask(bool undoData)
		{
			return undoData ? Undo.Mask : Redo.Mask;
		}

		///// <summary>
		///// Zoom amount. Does not allow the zoom to exceed preset boundaries
		///// </summary>
		//public float? Zoom(bool undoData)
		//{
		//    return undoData ? Undo.Zoom : Redo.Zoom;
		//}

		public override string ToString()
		{
			return Action ?? string.Empty;
		}

		#endregion [ Methods ]
	}

	#endregion [ UndoRedo ]

	#region [ Snapshot ]

	/// <summary>
	/// Quick snapshot of various settings, so we can compare what has changed.
	/// </summary>
	public class Snapshot
	{
		#region [ Public Fields ]

		public ChangeSet Data { get; private set; }

		#endregion [ Public Fields ]

		#region [ Constructor ]

		/// <summary>
		/// Initializes the object by taking a snapshot of the data.
		/// </summary>
		public Snapshot()
		{
			Data = new ChangeSet();

			Workshop Workshop = Workshop.Instance;

			// Record all the current UI and Background settings into the ChangeSet
			Workshop.UI.SaveUndoData(Data);

			// Record the current Mask
			Data.Mask = (Mask)Workshop.Mask.Clone();

			// Record a copy of all the Channels
			Data.Channels = new List<Channel>();
			foreach (Channel Ch in Workshop.Channels.GetAllChannels())
				Data.Channels.Add((Channel)Ch.Clone());
		}

		#endregion [ Constructor ]

	}

	#endregion [ Snapshot ]

	#region [ Background class ]

	public class Background
	{
		public Bitmap BaseImage = null;
		public bool? Visible = null;
		public string Filename = null;
		public Color Color = Color.Empty;
		public int? Brightness = null;
		public bool Cleared = false;

		public Background()
		{ }

		public bool IsEmpty	
		{
			get
			{
				if (this.BaseImage != null)
					return false;
				if (Visible != null)
					return false;
				if (Filename != null)
					return false;
				if (!Color.IsEmpty)
					return false;
				if (Brightness != null)
					return false;
				return true;
			}
		}

		public void Assign(Snapshot snap)
		{
			this.Cleared = snap.Data.BackgroundImage.Cleared;
			if (!this.Cleared)
			{
				this.BaseImage = (Bitmap)snap.Data.BackgroundImage.BaseImage.Clone();
				this.Visible = snap.Data.BackgroundImage.Visible;
				this.Filename = snap.Data.BackgroundImage.Filename;
				this.Color = snap.Data.BackgroundImage.Color;
				this.Brightness = snap.Data.BackgroundImage.Brightness;
			}
		}
	}

	#endregion [ Background class ]

	#region [ Interface class ]

	public class Interface
	{
		public Size? LatticeSize = null;
		public int? CellSize = null;
		public int? GridLineWidth = null;
		public byte? InactiveChannelAlpha = null;
		public float? Zoom = null;

		public Interface()
		{
			this.LatticeSize = null;
			this.CellSize = null;
			this.GridLineWidth = null;
			this.InactiveChannelAlpha = null;
			this.Zoom = null;
		}

		public bool IsEmpty
		{
			get
			{
				if (this.LatticeSize != null)
					return false;
				if (CellSize != null)
					return false;
				if (GridLineWidth != null)
					return false;
				if (InactiveChannelAlpha != null)
					return false;
				if (Zoom != null)
					return false;
				return true;
			}
		}
	}

	#endregion [ UI class ]

	#region [ DEAD CODE ]

	/*
	#region [ v1 ]

	public class v1
	{
		#region [ Private Variables ]

		private Channel _moveChannel = null;

		#endregion [ Private Variables ]

		#region [ Properties & Fields ]

		/// <summary>
		/// Text of the action to undo, to be displayed in the Undo menu under Edit
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// List of the indices of the Channels affected by this undo
		/// </summary>
		public List<int> AffectedChannels { get; set; }

		/// <summary>
		/// Bitmap that is used in the background, only populated if the BackgroundImage changes
		/// </summary>
		public Bitmap BackgroundImage { get; set; }

		/// <summary>
		/// Filename for the bitmap that is used in the background, only populated if the BackgroundImage changes
		/// </summary>
		public string BackgroundImageFilename { get; set; }

		/// <summary>
		/// Brightness setting for the background Channel
		/// </summary>
		public float Brightness { get; set; }

		/// <summary>
		/// CellSize of the Canvas
		/// </summary>
		public int CellSize { get; set; }

		/// <summary>
		/// Color of the Channel that is being changed
		/// </summary>
		public Color ChannelColor { get; set; }

		/// <summary>
		/// List of a List of Cells that need to be saved for undo. Each List of Cells in the outer list represent Channel cell data
		/// </summary>
		public List<List<Point>> Cells { get; set; }

		/// <summary>
		/// Name of the Channel that is being changed
		/// </summary>
		public string ChannelName { get; set; }

		/// <summary>
		/// General event enum for the Undo
		/// </summary>
		public GeneralDataEvent GeneralEvent { get; set; }

		/// <summary>
		/// Width of the displayed grid line
		/// </summary>
		public int GridLineWidth { get; set; }

		/// <summary>
		/// Size of the Lattice
		/// </summary>
		public Size LatticeSize { get; set; }

		/// <summary>
		/// Masking that is present that needs to be saved for undo
		/// </summary>
		public Mask Mask { get; set; }

		public Channel MoveChannel
		{
			get { return _moveChannel; }
			set { _moveChannel = (Channel)value.Clone(); }
		}

		/// <summary>
		/// Specific event enum for the Undo
		/// </summary>
		public SpecificEventType SpecificEvent { get; set; }

		/// <summary>
		/// Amount of the Zoom that is being changed
		/// </summary>
		public float Zoom { get; set; }

		#endregion [ Properties & Fields ]

		#region [ Constructor ]

		public v1()
		{
			this.Cells = new List<List<Point>>();
			this.AffectedChannels = new List<int>();
			this.Mask = new Mask();
		}

		#endregion [ Constructor ]

		#region [ Methods ]

		public void SetMask(Mask maskToCopy)
		{
			this.Mask.Set(maskToCopy, false);
		}

		#endregion [ Methods ]

	}

	#endregion [ v1 ]
	*/
	#endregion [ DEAD CODE ]
}
