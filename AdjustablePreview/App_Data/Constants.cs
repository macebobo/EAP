namespace ElfCore
{
	public class Constants
	{
		public string PLUGIN_NAME = "Adjustable preview";

		#region [ ToolGroup Names ]

		public const string TOOLGROUP_SHAPE = "Shape_ToolGroup";
		public const string TOOLGROUP_MULTIChannel = "MultiChannelLine_ToolGroup";
		public const string TOOLGROUP_MASK = "Mask_ToolGroup";

		#endregion [ ToolGroup Names ]

		public const string DEGREE_SIGN = "°";

		#region [ XPath Constants ]

		public const string XML_PLUGIN = "/Profile/PlugInData/PlugIn[@name=\"{0}\"]";

		public const string XML_DISPLAY = "Display";
		public const string XML_HEIGHT = "Height";
		public const string XML_WIDTH = "Width";
		public const string XML_PIXELSIZE = "PixelSize";
		public const string XML_BRIGHTNESS = "Brightness";
		public const string XML_GRIDLINE_WIDTH = "GridLineWidth";
		public const string XML_INACTIVE_Channel_ALPHA = "InactiveChannelAlpha";
		public const string XML_BACKGROUND_IMAGE = "BackgroundImage";
		public const string XML_Channel = "Channel";
		public const string XML_ChannelS = "Channels";
		public const string XML_REDIRECT_OUTPUTS = "RedirectOutputs";

		public const string XML_SETTINGS_ROOT = "AdjustablePreview";

		// Attribute names
		public const string FILENAME = "filename";
		public const string NUMBER = "number";

		#endregion [ XPath Constants ]

		#region [ Settings Constants ]

		// Window Settings
		public const string SAVE_PATH_DELIMITER = "|";

		public const string SETUP_DIALOG = "Setup Dialog";
		public const string PREVIEW_DIALOG = "Preview Dialog";

		public const string WINDOW_LEFT = SAVE_PATH_DELIMITER + "Left";
		public const string WINDOW_TOP = SAVE_PATH_DELIMITER + "Top";
		public const string WINDOW_WIDTH = SAVE_PATH_DELIMITER + "Width";
		public const string WINDOW_HEIGHT = SAVE_PATH_DELIMITER + "Height";
		public const string WINDOW_STATE = SAVE_PATH_DELIMITER + "WindowState";

		// UI Settings
		public const string UI = "User Interface";

		public const string SHOW_GRIDLINES = UI + SAVE_PATH_DELIMITER + "ShowGridLines";
		public const string SHOW_GRIDLINES_WHILEPAINTING = UI + SAVE_PATH_DELIMITER + "PaintWithGridLines";
		public const string RESPECT = UI + SAVE_PATH_DELIMITER + "RespectChannelOutputsDuringPlayback";
		public const string SHOW_RULERS = UI + SAVE_PATH_DELIMITER + "ShowRulers";

		// Common Tool Settings
		public const string TOOLSETTINGS = "ToolSettings";

		public const string NIB_SIZE = "NibSize";
		public const string NIB_SHAPE = "NibShape";
		public const string LINE_THICKNESS = "LineThickness";
		public const string DASH_STYLE = "DashStyle";
		public const string RADIUS = "Radius";
		public const string NUM_ChannelS = "NumChannels";
		public const string FILL = "Fill";
		public const string START_ANGLE = "StartAngle";
		public const string DIRECTION = "Direction";

		#endregion [ Settings Constants ]
	}
}
