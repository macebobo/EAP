using ElfCore.Controllers;
using ElfCore.PlugIn;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace ElfCore.Core
{
	[Serializable]
	public class UISettings : ElfBase, INotifyPropertyChanged
	{

		#region [ Constants ]

		// Property name constants
		public const string Property_InactiveChannelAlpha = "InactiveChannelAlpha";
		public const string Property_MouseDownPosition = "MouseDownPosition";
		public const string Property_MousePosition = "MousePosition";
		public const string Property_MouseSelectionSize = "MouseSelectionSize";
		public const string Property_ShowRuler = "ShowRuler";
		public const string Property_ShowMaskBlocky = "ShowMaskBlocky";
		public const string Property_ShowMaskMarquee = "ShowMaskMarquee";
		public const string Property_ShowMaskOverlay = "ShowMaskOverlay";
		public const string Property_SuperimposeGridOnBackground = "SuperimposeGridOnBackground";
		public const string Property_TraceLevel = "TraceLevel";
		public const string Property_NoTraceDuringPlayback = "NoTraceDuringPlayback";
		public const string Property_TraceLogFilename = "TraceLogFilename";
		public const string Property_DisableUndo = "DisableUndo";
		public const string Property_ShowExcludedChannels = "ShowExcludedChannels";

		#endregion [ Constants ]
				
		#region [ Private Variables ]

		private Workshop _workshop = null;

		#region [ NonSerialized ]

		private bool _superimposeGridOnBackground = false;

		//[XmlIgnore()]
		//private bool _useOriginalUI = false;

		// UI Menu settings
		private bool _showRuler = true;
		//private bool _showGridlinesWhilePainting = true;
		private byte _inactiveChannelAlpha = 128;
		private bool _disableUndo = false;
		private bool _showExcludedChannels = false;

		// Mouse position
		private Point _currentMouseCell = Point.Empty;
		private Point _mouseDownCell = Point.Empty;
		private Size _mouseSelectionSize = Size.Empty;

		// Mask settings
		private bool _showMaskMarquee = true;
		private bool _showMaskOverlay = true;
		private bool _showMaskBlocky = false;

		private List<PlugInTool> _tools = null;

		#endregion [ NonSerialized ]

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Shortcut to the Active Profile
		/// </summary>
		[DebuggerHidden]
		public BaseProfile ActiveProfile
		{
			get { return _workshop.ProfileController.Active; }
		}

		public byte InactiveChannelAlpha
		{
			get { return _inactiveChannelAlpha; }
			set
			{
				if (_inactiveChannelAlpha != value)
				{
					_inactiveChannelAlpha = value;
					OnPropertyChanged(Property_InactiveChannelAlpha, false);
				}
			}
		}

		/// <summary>
		/// Point (in Cells) at which the Mouse button was clicked last
		/// </summary>
		public Point MouseDownPosition
		{
			get { return _mouseDownCell; }
			set
			{
				if (!_mouseDownCell.Equals(value))
				{
					_mouseDownCell = value;
					if (!_mouseDownCell.IsEmpty)
						MouseSelectionSize = new Size(Math.Abs(_currentMouseCell.X - _mouseDownCell.X), Math.Abs(_currentMouseCell.Y - _mouseDownCell.Y));
					else
						MouseSelectionSize = Size.Empty;
					OnPropertyChanged(Property_MouseDownPosition, false);
				}
			}
		}

		/// <summary>
		/// Location of the mouse cursor in Cells
		/// </summary>
		public Point MousePosition
		{
			get { return _currentMouseCell; }
			set
			{
				if (!_currentMouseCell.Equals(value))
				{
					_currentMouseCell = value;
					if (!_mouseDownCell.IsEmpty)
						MouseSelectionSize = new Size(Math.Abs(_currentMouseCell.X - _mouseDownCell.X), Math.Abs(_currentMouseCell.Y - _mouseDownCell.Y));
					else
						MouseSelectionSize = Size.Empty;
					OnPropertyChanged(Property_MousePosition, false);
				}
			}
		}

		/// <summary>
		/// Distance between the current mouse position and where the mouse button was down
		/// </summary>
		public Size MouseSelectionSize
		{
			get { return _mouseSelectionSize; }
			set
			{
				if (!_mouseSelectionSize.Equals(value))
				{
					_mouseSelectionSize = value;
					OnPropertyChanged(Property_MouseSelectionSize, false);
				}
			}
		}

		/// <summary>
		/// Indicates whether excluded channels appear on the channel explorer when editing in PlugIn mode.
		/// </summary>
		public bool ShowExcludedChannels
		{
			get { return _showExcludedChannels; }
			set
			{
				if (_showExcludedChannels != value)
				{
					_showExcludedChannels = value;
					OnPropertyChanged(Property_ShowExcludedChannels, true);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Rulers are display on the CanvasWindow.
		/// </summary>
		public bool ShowRuler
		{
			get { return _showRuler; }
			set
			{
				if (_showRuler != value)
				{
					_showRuler = value;
					OnPropertyChanged(Property_ShowRuler, true);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Marquee should display its outline blocky and conforming to the cells
		/// </summary>
		public bool ShowMaskBlocky
		{
			get { return _showMaskBlocky; }
			set 
			{
				_showMaskBlocky = value;
				OnPropertyChanged(Property_ShowMaskBlocky, true);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Marquee outline should be displayed around the masked area
		/// </summary>
		public bool ShowMaskMarquee
		{
			get { return _showMaskMarquee; }
			set
			{
				if (_showMaskMarquee != value)
				{
					_showMaskMarquee = value;
					OnPropertyChanged(Property_ShowMaskMarquee, true);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the area around Marquee outline is displayed with a translucent color overlaying it
		/// </summary>
		public bool ShowMaskOverlay
		{
			get { return _showMaskOverlay; }
			set
			{
				if (_showMaskOverlay != value)
				{
					_showMaskOverlay = value;
					OnPropertyChanged(Property_ShowMaskOverlay, true);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether grid should display on top of the background image
		/// </summary>
		public bool SuperimposeGridOnBackground
		{
			get { return _superimposeGridOnBackground; }
			set
			{
				if (_superimposeGridOnBackground != value)
				{
					_superimposeGridOnBackground = value;
					OnPropertyChanged(Property_SuperimposeGridOnBackground, true);
				}
			}
		}

		/// <summary>
		/// List of all the Tools
		/// </summary>
		internal List<PlugInTool> Tools
		{
			get { return _tools; }
			set { _tools = value; }
		}

		/// <summary>
		/// Indicates whether Undo is disabled.
		/// </summary>
		public bool DisableUndo
		{
			get { return _disableUndo; }
			set
			{
				if (_disableUndo != value)
				{
					_disableUndo = value;
					OnPropertyChanged(Property_DisableUndo, true);
				}
			}
		}

		#endregion [ Properties ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the mouse button is pressed property changes.
		/// </summary>
		public EventHandler MouseDown;
		
		/// <summary>
		/// Occurs when the mouse position on the Canvas changes.
		/// </summary>
		public EventHandler MousePoint;

		#endregion [ Event Handlers ]

		#region [ Constructor ]

		public UISettings()
			: base()
		{
			_tools = new List<PlugInTool>();
			_workshop = Workshop.Instance;
		}

		#endregion [ Constructor ]

		#region [ Methods ]

		/// <summary>
		/// Load the UI settings
		/// </summary>
		public void LoadSettings()
		{
			Settings Settings = Settings.Instance;

			InactiveChannelAlpha = Settings.GetValue(Property_InactiveChannelAlpha, (byte)128);
			ShowRuler = Settings.GetValue(Property_ShowRuler, true);
			DisableUndo = Settings.GetValue(Property_DisableUndo, true);
			ShowExcludedChannels = Settings.GetValue(Property_ShowExcludedChannels, false);
			//_workshop.TraceLevel = EnumHelper.GetEnumFromValue<TraceLevel>(Settings.GetValue(Property_TraceLevel, (int)TraceLevel.Off));
			//_workshop.NoTraceDuringPlayback = Settings.GetValue(Property_NoTraceDuringPlayback, false);
			//_workshop.TraceLogFilename = Settings.GetValue(Property_TraceLogFilename, "ElfCore.log");
		}

		public void SaveSettings()
		{
			Settings Settings = Settings.Instance;

			Settings.SetValue(Property_InactiveChannelAlpha, InactiveChannelAlpha);
			Settings.SetValue(Property_ShowRuler, ShowRuler);
			Settings.SetValue(Property_DisableUndo, DisableUndo);
			Settings.SetValue(Property_ShowExcludedChannels, ShowExcludedChannels);
			//Settings.SetValue(Property_TraceLevel, (int)_workshop.TraceLevel);
			//Settings.SetValue(Property_NoTraceDuringPlayback, _workshop.NoTraceDuringPlayback);
			//Settings.SetValue(Property_TraceLogFilename, _workshop.TraceLogFilename);
		}

		#endregion [ Methods ]

	}
}
