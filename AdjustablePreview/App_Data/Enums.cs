namespace ElfCore
{
	public enum ConstrainDirection
	{ 
		NotSet,
		Horizontal,
		Vertical
	}

	public enum Scaling
	{ 
		Cell,
		Pixel
	}

	public enum Tool : int
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
		PlugIn = 2000,
	}

	#region [ Enums ]

	public enum ChannelEventType
	{
		Cells,
		Color,
		Dirty,
		Name,
		Selected,
		Visibility
	}

	public enum ProfileEventType
	{ 
		CellSize,
		Dirty,
		InactiveChannelAlpha,
		LatticeSize
	}

	public enum UIEventType
	{
		CellSize,
		/// <summary>
		/// Current mouse position on the Canvas
		/// </summary>
		MousePoint,
		/// <summary>
		/// Point where the MouseDown event occurred on the Canvas
		/// </summary>
		MouseDownPoint,
		Dirty,
		ShowGridLines,
		InactiveChannelAlpha,
		LatticeSize,
		Loading,
		MouseSelectionSize,
		ShowRuler,
		MaskDisplayChanged,
		ShowPaintGridLines,
		SuperimposeGridLines,
		Zoom
	}

	#endregion [ Enums ]

	//public enum EventCategory : int
	//{
	//    NotSet = -1,
	//    //Channel,
	//    Clipboard,
	//    Mask,
	//    Tool,
	//    //ToolSettings,
	//    UI,
	//    Undo
	//}



	//public enum EventSubCategory
	//{ 
	//    NotSet = -1,

	//    // Channels
	//    //Channel_Active,
	//    //Channel_Cells,
	//    //Channel_ClearAll,
	//    //Channel_ClearSelected,
	//    //Channel_ColorChanged,
	//    //Channel_Import,
	//    //Channel_NameChanged,
	//    //Channel_Selected,
	//    //Channel_VisibilityChanged,

	//    // Clipboard
	//    //Clipboard_Cut,
	//    //Clipboard_Copy,
	//    Clipboard_Delete,
	//    Clipboard_Paste,

	//    //// Mask
	//    Mask_Cleared,
	//    Mask_Defined,
	//    //Mask_Inverted,
	//    Mask_Moved,
	//    ShowMaskMarquee,
	//    ShowMaskOverlay,

	//    //// Tool
	//    //Tool_Mask,
	//    //Tool_MultiChannel,
	//    //Tool_Selected,
	//    //Tool_Shape,

	//    // Background Image
	//    BackgroundImage_Brightness,
	//    BackgroundImage_Clear,
	//    BackgroundImage_Load,
	//    BackgroundImage_Visible,
	//    SuperimposeGridOnBackground,

	//    // UI
	//    //CanvasPosition,
	//    //CellSize,
	//    ////Crop,
	//    //CurrentMouseCellPixel,
	//    //Dirty,
	//    //GridLineWidth,
	//    //InactiveChannelAlpha,
	//    //LatticeSize,
	//    //Loading,
	//    //MouseDownCellPixel,
	//    //MouseSelectionSize,
	//    ////NewSize,
	//    //RespectChannelOutputsDuringPlayback,
	//    //ShowGridLineWhilePainting,
	//    //ShowRuler,
	//    ////UseOriginalUI,
	//    //Zoom,

	//    // Undo
	//    Undo,
	//    //UndoStackPushed,
	//    //UndoStackPopped,
	//    //UndoStackClear,

	//    Redo,
	//    //RedoStackPushed,
	//    //RedoStackPopped,
	//    //RedoStackClear
	//}

	public enum Panes
	{ 
		LatticeBuffer,
		MaskCanvas,
		MaskLattice,
		Canvas,
		ActiveChannel,
		ImageStamp,
		ClipboardChannel,
		MoveChannel
	}

	///// <summary>
	///// Enumeration to be used for those Win32 function that return BOOL  
	///// </summary>  
	//public enum Bool
	//{
	//    False = 0,
	//    True
	//}

	///// <summary>  
	///// Enumeration for the raster operations used in BitBlt.20.  
	///// In C++ these are actually #define. But to use these  
	///// constants with C#, a new enumeration type is defined.  
	///// </summary>
	//public enum TernaryRasterOperations  
	//{    
	//    SRCCOPY     = 0x00CC0020, /* dest = source                   */    
	//    SRCPAINT    = 0x00EE0086, /* dest = source OR dest           */    
	//    SRCAND      = 0x008800C6, /* dest = source AND dest          */    
	//    SRCINVERT   = 0x00660046, /* dest = source XOR dest          */    
	//    SRCERASE    = 0x00440328, /* dest = source AND (NOT dest )   */    
	//    NOTSRCCOPY  = 0x00330008, /* dest = (NOT source)             */    
	//    NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */    
	//    MERGECOPY   = 0x00C000CA, /* dest = (source AND pattern)     */    
	//    MERGEPAINT  = 0x00BB0226, /* dest = (NOT source) OR dest     */    
	//    PATCOPY     = 0x00F00021, /* dest = pattern                  */
	//    PATPAINT    = 0x00FB0A09, /* dest = DPSnoo                   */    
	//    PATINVERT   = 0x005A0049, /* dest = pattern XOR dest         */    
	//    DSTINVERT   = 0x00550009, /* dest = (NOT dest)               */    
	//    BLACKNESS   = 0x00000042, /* dest = BLACK                    */    
	//    WHITENESS   = 0x00FF0062, /* dest = WHITE                    */
	//}

}



