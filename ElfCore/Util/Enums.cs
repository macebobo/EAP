namespace ElfCore.Util
{
	public enum ConstrainDirection
	{ 
		NotSet,
		Horizontal,
		Vertical
	}

	public enum Panes
	{
		LatticeBuffer,
		MaskCanvas,
		MaskLattice,
		CapturedCanvas,
		ActiveChannel,
		ImageStamp,
		Clipboard,
		MoveChannel,
		Snapshot,
		Undo,
		Redo
	}

	public enum RunMode
	{ 
		/// <summary>
		/// Run as a Plugin
		/// </summary>
		PlugIn,

		/// <summary>
		/// Run as a standalone editor.
		/// </summary>
		Standalone
	}

	public enum ToolID : int
	{
		NotSet = 0,
		Paint = 1,
		Spray = 3,
		Erase = 4,
		ImageStamp = 8,
		Text = 9,
		Fill = 10,
		MoveChannel = 11,
		Zoom = 12,
		Crop = 13,

		Shape_ToolGroup = 5,
		Rectangle = 501,
		Ellipse = 502,
		Polygon = 503,
		Line = 504,
		Icicles = 505,

		MultiChannel_ToolGroup = 6,
		MultiChannelLine = 601,
		MegaTree = 602,
		SingingFace = 603,

		Mask_ToolGroup = 7,
		Mask_Rectangle = 701,
		Mask_Ellipse = 702,
		Mask_Paint = 703,
		Mask_Freehand = 704,
		Mask_Lasso = 705,

		PlugInToolGroup = 1000,
		PlugIn = 2000
	}

	public enum TravelDirection
	{
		/// <summary>
		/// Used for changing Shuffles of Channels, indicating the selected Channel should be moved to the top of the list.
		/// </summary>
		ToTop = -99,

		/// <summary>
		/// Used for changing Shuffles of Channels, indicating the selected Channel should be moved up by 1.
		/// </summary>
		Up = -1,

		/// <summary>
		/// Used in MultiChannelLine, indicates linear progression of Channels from Right to Left.
		/// </summary>
		RightToLeft = -1,

		/// <summary>
		/// Used in MultiChannelLine and MegaTree to indicate a circular progression of Channels in an anti-clockwise direction.
		/// </summary>
		Anticlockwise = -1,

		/// <summary>
		/// Direction is not set
		/// </summary>
		NotSet = 0,

		/// <summary>
		/// Used for changing Shuffles of Channels, indicating the selected Channel should be moved down by 1.
		/// </summary>
		Down = +1,

		/// <summary>
		/// Used in MultiChannelLine, indicates linear progression of Channels from Left to Right.
		/// </summary>
		LeftToRight = +1,

		/// <summary>
		/// Used in MultiChannelLine and MegaTree to indicate a circular progression of Channels in a clockwise direction.
		/// </summary>
		Clockwise = +1,

		/// <summary>
		/// Used for changing Shuffles of Channels, indicating the selected Channel should be moved to the bottom of the list.
		/// </summary>
		ToBottom = +99

	}

	public enum UnitScale
	{
		Canvas,
		Lattice
	}
	
	/// <summary>
	/// annotations enum indicates which small image can be added to another image to modify it, such as adding an Edit image to another image
	/// </summary>
	public enum Annotation
	{
		Add,
		Alert,
		As,
		Blocked,
		Check,
		Clear,
		ClearStar,
		Close,
		Complete,
		Create,
		Delete,
		Down,
		Edit,
		Error,
		Exclude,
		Export,
		Find,
		Flyout,
		Grid,
		Help,
		Image,
		Import,
		Include,
		Info,
		Invisible,
		Left,
		Locked,
		Monitor,
		New,
		Not,
		OneToOne,
		Open,
		Paint,
		Pause,
		Play,
		PlayRound,
		PlusOne,
		Redo,
		Refresh,
		Remove,
		Required,
		Right,
		Save,
		SecurityAlert,
		SecurityBad,
		SecurityGood,
		Shortcut,
		SortAscending,
		SortDescending,
		Star,
		Stop,
		Trash,
		Undo,
		Up,
		Vector,
		Visible,
		Warning,
		NotSet = 0
	}

	public enum WallpaperStyle
	{ 
		Fill,
		Fit,
		Stretch,
		Tile,
		Center
	}
}



