using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore
{
	[Serializable]
	public class UISettings : ICloneable
	{
		#region [ Public Static Variables ]

		/// <summary>
		/// (CellSize + GridLineWidth) * Zoom
		/// </summary>
		public static int ʃCellScale = 0;

		/// <summary>
		/// (CellSize + GridLineWidth) * Zoom
		/// </summary>
		public static float ʃCellScaleF = 0;

		/// <summary>
		/// CellSize * Zoom
		/// </summary>
		public static int ʃCellZoom = 0;

		/// <summary>
		/// CellSize * Zoom
		/// </summary>
		public static float ʃCellZoomF = 0;

		/// <summary>
		/// Size (Width, Height) of the Lattice
		/// </summary>
		public static Size ʃLatticeSize = Size.Empty;

		/// <summary>
		/// Size (Width, Height) of the Canvas
		/// </summary>
		public static Size ʃCanvasSize = Size.Empty;

		/// <summary>
		/// Zoom
		/// </summary>
		public static float ʃZoomF = 0;

		#endregion [ Public Static Variables ]

		#region [ Constants ]

		public const float MAX_ZOOM = 5f;
		public const float MIN_ZOOM = 1f;
		public const float ZOOM_100 = 1f;

		#endregion [ Constants ]
				
		#region [ Private Variables ]

		private BackgroundImage _backgroundImage = null;
		private Size _latticeSize = Size.Empty;
		private int _cellSize = 0;
		private int _gridLineWidth = 0;
		private byte _inactiveChannelAlpha = 128;
		private float _zoom = 1.0f;

		#region [ NonSerialized ]

		[NonSerialized()]
		private bool _superimposeGridOnBackground = false;

		[NonSerialized()]
		private bool _dirty = false;

		//[NonSerialized()]
		//private bool _useOriginalUI = false;

		// UI Menu settings
		[NonSerialized()]
		private bool _respectChannelOutputsDuringPlayback = true;
		[NonSerialized()]
		private bool _showRuler = true;
		[NonSerialized()]
		private bool _showGridlinesWhilePainting = true;

		// Mouse position
		[NonSerialized()]
		private Point _currentMouseCell = Point.Empty;
		[NonSerialized()]
		private Point _mouseDownCell = Point.Empty;
		[NonSerialized()]
		private Size _mouseSelectionSize = Size.Empty;

		// Mask settings
		[NonSerialized()]
		private bool _showMaskMarquee = true;
		[NonSerialized()]
		private bool _showMaskOverlay = true;
		[NonSerialized()]
		private bool _showMaskBlocky = false;

		[NonSerialized()]
		private List<PlugIn.ToolHost> _tools = null;

		#endregion [ NonSerialized ]

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public BackgroundImage Background
		{
			get { return _backgroundImage; }
		}

		/// <summary>
		/// Size of the Canvas, calculated with CellSize, GridlineWidth, & LatticeSize, all multiplied by the Zoom
		/// </summary>
		public Size CanvasSize
		{
			get { return new Size(_latticeSize.Width * UISettings.ʃCellScale, _latticeSize.Height * UISettings.ʃCellScale); }
		}
				
		/// <summary>
		/// Size of the Cell in pixels with no zoom factored
		/// </summary>
		public int CellSize
		{
			get { return _cellSize; }
			set
			{
				if (_cellSize != value)
				{
					Dirty = true;
					_cellSize = value;
					CalculateStaticValues();
					OnChanged(UIEventType.CellSize);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether any of the settings in the is object have been modified
		/// </summary>
		public bool Dirty
		{
			get { return _dirty; }
			set 
			{
				if (_dirty != value)
				{
					_dirty = value;
					if (_dirty)
						OnChanged(UIEventType.Dirty);
				}
			}
		}

		/// <summary>
		/// Width of the Grid Line between Cells. 0 indicates no lines should be shown.
		/// </summary>
		public int GridLineWidth
		{
			get { return _gridLineWidth; }
			set
			{
				if (_gridLineWidth != value)
				{
					Dirty = true;
					_gridLineWidth = value;
					if ((value < Int32.MaxValue) && (value >= 0))
					{
						// Only fire on valid values
						CalculateStaticValues();
						OnChanged(UIEventType.ShowGridLines);
					}
				}
			}
		}

		/// <summary>
		/// Amount of alpha (transparency) to use when displaying Channels that are not currently active.
		/// </summary>
		public byte InactiveChannelAlpha
		{
			get { return _inactiveChannelAlpha; }
			set
			{
				if (_inactiveChannelAlpha != value)
				{
					Dirty = true;
					if ((value < 0) || (value > 255))
						value = 128;
					_inactiveChannelAlpha = value;
					OnChanged(UIEventType.InactiveChannelAlpha);
				}
			}
		}

		/// <summary>
		/// Size of the Lattice
		/// </summary>
		public Size LatticeSize
		{
			get { return _latticeSize; }
			set
			{
				if (!_latticeSize.Equals(value) && !value.IsEmpty)
				{
					Dirty = true;
					_latticeSize = value;
					
					// Set the static value to this
					UISettings.ʃLatticeSize = value;
					CalculateStaticValues();

					_backgroundImage.Size = this.CanvasSize;

					OnChanged(UIEventType.LatticeSize);
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
					OnChanged(UIEventType.MouseDownPoint);
					if (!_mouseDownCell.IsEmpty)
						MouseSelectionSize = new Size(Math.Abs(_currentMouseCell.X - _mouseDownCell.X), Math.Abs(_currentMouseCell.Y - _mouseDownCell.Y));
					else
						MouseSelectionSize = Size.Empty;
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
					OnChanged(UIEventType.MousePoint);
					if (!_mouseDownCell.IsEmpty)
						MouseSelectionSize = new Size(Math.Abs(_currentMouseCell.X - _mouseDownCell.X), Math.Abs(_currentMouseCell.Y - _mouseDownCell.Y));
					else
						MouseSelectionSize = Size.Empty;
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
					OnChanged(UIEventType.MouseSelectionSize);
				}
			}
		}

		/// <summary>
		/// Indicates whether the channel order should respected, or display the channels in their natural, raw order
		/// </summary>
		public bool RespectChannelOutputsDuringPlayback
		{
			get { return _respectChannelOutputsDuringPlayback; }
			set
			{
				if (_respectChannelOutputsDuringPlayback != value)
				{
					Dirty = true;
					_respectChannelOutputsDuringPlayback = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether we should overlay a black grid of lines while using the paint tool. If GridLineWidth is 0, then
		/// this is ignored.
		/// </summary>
		public bool ShowGridLineWhilePainting
		{
			get { return _showGridlinesWhilePainting; }
			set 
			{
				if (_showGridlinesWhilePainting != value)
				{
					_showGridlinesWhilePainting = value;
					Dirty = true;
					OnChanged(UIEventType.ShowPaintGridLines);
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
					Dirty = true;
					OnChanged(UIEventType.ShowRuler);
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
				Dirty = true;
				_showMaskBlocky = value;
				OnChanged(UIEventType.MaskDisplayChanged);
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
					Dirty = true;
					OnChanged(UIEventType.MaskDisplayChanged);
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
					Dirty = true;
					OnChanged(UIEventType.MaskDisplayChanged);
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
					Dirty = true;
					_superimposeGridOnBackground = value;
					OnChanged(UIEventType.SuperimposeGridLines);
				}
			}
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public bool SuppressEvents { get; set; }

		/// <summary>
		/// List of all the Tools
		/// </summary>
		internal List<PlugIn.ToolHost> Tools
		{
			get { return _tools; }
			set { _tools = value; }
		}

		/// <summary>
		/// Zoom amount. Does not allow the zoom to exceed preset boundaries
		/// </summary>
		public float Zoom
		{
			get { return _zoom; }
			set
			{
				if (value < MIN_ZOOM)
					_zoom = MIN_ZOOM;
				else if (value > MAX_ZOOM)
					_zoom = MAX_ZOOM;
				else
					_zoom = value;
				
				UISettings.ʃZoomF = value;
				CalculateStaticValues();

				OnChanged(UIEventType.Zoom);
			}
		}

		#endregion [ Properties ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the CellSize property changes.
		/// </summary>
		public EventHandler CellSizeChanged;

		/// <summary>
		/// Occurs when the Dirty property changes.
		/// </summary>
		public EventHandler DirtyChanged;

		/// <summary>
		/// Occurs when the InactiveChannelAlpha property changes.
		/// </summary>
		public EventHandler InactiveChannelAlphaChanged;
		
		/// <summary>
		/// Occurs when the LatticeSize property changes.
		/// </summary>
		public EventHandler LatticeSizeChanged;
		//public EventHandler LoadingChanged;

		/// <summary>
		/// Occurs when the mouse button is pressed property changes.
		/// </summary>
		public EventHandler MouseDown;
		
		/// <summary>
		/// Occurs when the mouse position on the Canvas changes.
		/// </summary>
		public EventHandler MousePoint;
		
		///// <summary>
		///// Occurs when the mouse button is clicked down and the mouse position on the Chanvas changes.
		///// </summary>
		//public EventHandler MouseSelection;
		
		/// <summary>
		/// Occurs when the ShowGridLines property changes.
		/// </summary>
		public EventHandler DisplayGridLines;

		/// <summary>
		/// Occurs when the ShowRuler property changes.
		/// </summary>
		public EventHandler DisplayRuler;

		/// <summary>
		/// Occurs when the ShowMaskMarquee or the ShowMaskOverlay properties change.
		/// </summary>
		public EventHandler MaskDisplayChanged;

		/// <summary>
		/// Occurs when the Zoom property changes.
		/// </summary>
		public EventHandler Zooming;

		/// <summary>
		/// Occurs when the code determines the Canvas needs to be refreshed
		/// </summary>
		public EventHandler Refresh;

		#endregion [ Event Handlers ]

		#region [ Constructor ]
		
		public UISettings()
		{
			_tools = new List<PlugIn.ToolHost>();
			_backgroundImage = new BackgroundImage();
			
			this.SuppressEvents = true;
			this.GridLineWidth = 1;
			this.LatticeSize = new Size(64, 32);
			this.CellSize = 7;
			this.SuppressEvents = false;
		}

		#endregion [ Constructor ]

		#region [ Methods ]

		/// <summary>
		/// Calculate these derived values from the other properties.
		/// </summary>
		private void CalculateStaticValues()
		{
			UISettings.ʃCellScaleF = (_cellSize + _gridLineWidth) * _zoom;
			UISettings.ʃCellScale = (int)UISettings.ʃCellScaleF;
			UISettings.ʃCellZoomF = _cellSize * _zoom;
			UISettings.ʃCellZoom = (int)UISettings.ʃCellZoomF;
			UISettings.ʃCanvasSize = this.CanvasSize;
			UISettings.ʃZoomF = _zoom;
		}

		public object Clone()
		{
			return (UISettings)this.MemberwiseClone();
		}

		/// <summary>
		/// Record all the Current UI and Background settings into the ChangeSet
		/// </summary>
		public void SaveUndoData(UndoData.ChangeSet changes)
		{
			Background.SaveUndoData(changes);

			changes.UI.LatticeSize = _latticeSize;
			changes.UI.CellSize = _cellSize;
			changes.UI.GridLineWidth = _gridLineWidth;
			changes.UI.InactiveChannelAlpha = _inactiveChannelAlpha;
			changes.UI.Zoom = _zoom;
		}

		/// <summary>
		/// Load the UI settings
		/// </summary>
		public void LoadSettings()
		{
			Settings _settings = Settings.Instance;

			RespectChannelOutputsDuringPlayback = _settings.GetValue(Constants.RESPECT, true);
			ShowGridLineWhilePainting = _settings.GetValue(Constants.SHOW_GRIDLINES_WHILEPAINTING, true);
			ShowRuler = _settings.GetValue(Constants.SHOW_RULERS, true);

		}

		#endregion [ Methods ]

		#region [ Custom Event Methods ]

		private void OnChanged(UIEventType eventType)
		{
			if (this.SuppressEvents)
				return;

			switch (eventType)
			{
				case UIEventType.CellSize:
					if (CellSizeChanged == null)
						return;
					CellSizeChanged(this, new System.EventArgs());
					break;
				
				case UIEventType.Dirty:
					if (DirtyChanged == null)
						return;
					DirtyChanged(this, new System.EventArgs());
					break;
				
				case UIEventType.InactiveChannelAlpha:
					if (InactiveChannelAlphaChanged == null)
						return;
					InactiveChannelAlphaChanged(this, new System.EventArgs());
					break;
				
				case UIEventType.LatticeSize:
					if (LatticeSizeChanged == null)
						return;
					LatticeSizeChanged(this, new System.EventArgs());
					break;
				
				//case UIEventType.Loading:
				//    if (LoadingChanged == null)
				//        return;
				//    LoadingChanged(this, new System.EventArgs());
				//    break;
				
				case UIEventType.MouseDownPoint:
					if (MouseDown == null)
						return;
					MouseDown(this, new System.EventArgs());
					break;
				
				case UIEventType.MousePoint:
					if (MousePoint == null)
						return;
					MousePoint(this, new System.EventArgs());
					break;

				//case UIEventType.MouseSelectionSize:
				//    if (MouseSelection == null)
				//        return;
				//    MouseSelection(this, new System.EventArgs());
				//    break;
				
				case UIEventType.ShowGridLines:
					if (DisplayGridLines == null)
						return;
					DisplayGridLines(this, new System.EventArgs());
					break;

				case UIEventType.MaskDisplayChanged:
					if (MaskDisplayChanged == null)
						return;
					MaskDisplayChanged(this, new System.EventArgs());
					break;

				case UIEventType.ShowRuler:
					if (DisplayRuler == null)
						return;
					DisplayRuler(this, new System.EventArgs());
					break;
				
				case UIEventType.Zoom:
					if (Zooming == null)
						return;
					Zooming(this, new System.EventArgs());
					break;

				case UIEventType.SuperimposeGridLines:
					if (Refresh == null)
						return;
					Refresh(this, new System.EventArgs());
					break;
			}
		}

		#endregion [ Custom Event Methods ]

		#region [ DEAD CODE ]

		//private Bitmap _backgroundImage = null;
		//private Bitmap _unalteredBackgroundImage = null;
		//private string _backgroundImageFilename = string.Empty;
		//private float _brightness = 0;
		//private bool _bgVisible = true;

		///// <summary>
		///// Gets a value indicating whether the BackGround image should be displayed on the Canvas
		///// </summary>
		//public bool BackgroundImage_Visible
		//{
		//    get { return _bgVisible; }
		//    set
		//    {
		//        if (_bgVisible != value)
		//        {
		//            _bgVisible = value;
		//            OnChanged(SpecificEventType.BackgroundImage_Visible);
		//        }
		//    }
		//}

		///// <summary>
		///// Background Image
		///// </summary>
		//public Bitmap BackgroundImage
		//{
		//    get { return _backgroundImage; }
		//    set
		//    {
		//        Dirty = true;
		//        _backgroundImage = value;
		//        this.BackgroundImage_Brightness = 0f;
		//        if (value == null)
		//        {
		//            OnChanged(SpecificEventType.BackgroundImage_Clear);
		//            _unalteredBackgroundImage = null;
		//        }
		//        else
		//        {
		//            OnChanged(SpecificEventType.BackgroundImage_Load);
		//        }
		//    }
		//}

		///// <summary>
		///// Name of the file used to load the Background Image
		///// </summary>
		//public string BackgroundImage_Filename
		//{
		//    get { return _backgroundImageFilename; }
		//    set 
		//    {
		//        if (_backgroundImageFilename != value)
		//        {
		//            Dirty = true;
		//            _backgroundImageFilename = value;
		//        }
		//    }
		//}

		///// <summary>
		///// Gets a value to change the brightness of the BackgroundImage. Valid range is -1.0 to +1.0
		///// </summary>
		//public float BackgroundImage_Brightness
		//{
		//    get { return _brightness; }
		//    set
		//    {
		//        if (value < -1.0f)
		//            value = -1.0f;
		//        else if (value > 1.0f)
		//            value = 1.0f;
		//        if (_brightness != value)
		//        {
		//            Dirty = true;
		//            _brightness = value;
		//            OnChanged(SpecificEventType.BackgroundImage_Brightness);
		//        }
		//    }
		//}



		#endregion [ DEAD CODE ]

	}
}
