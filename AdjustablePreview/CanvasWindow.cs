using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// http://www.techotopia.com/index.php/Using_Bitmaps_for_Persistent_Graphics_in_C_Sharp
namespace ElfCore
{
	/// <summary>
	/// http://www.codeproject.com/Articles/1663/Drawing-a-rubber-band-line-in-GDI
	/// </summary>
	public partial class CanvasWindow : ToolWindow
	{
	
		#region [ Private Variables ]

		/// <summary>
		/// Workshop is a Singleton type of class. Simply getting the Instance variable from the Static object will get the object already loaded with our data
		/// </summary>
		private Workshop _workshop = Workshop.Instance;
		private Settings _settings = Settings.Instance;

		private StringFormat _rulerTextFormatter = null;
		private float _hScrollPct = 0.5f;
		private float _vScrollPct = 0.5f;
		private bool _movingMask = false;
		private bool _mouseOverCanvasPane = false;
		private bool _isFormShown = false;
		private bool _settingScroll = false;
		private bool _settingZoom = false;

		private MouseButtons _mouseButtons;

		#endregion [ Private Variables ]

		#region [ Properties ]
				
		public Workshop Workshop
		{
			set
			{
				_workshop = value;
				if (value == null)
					return;
				//_workshop.Changed += new Workshop.DataEventHandler(Workshop_Changed);
				_workshop.ClickZoom += new Workshop.ZoomEventHandler(Workshop_ClickZoom);
				
				_workshop.Channels.ChannelColorChanged += new EventHandler(ChannelController_ColorChanged);
				_workshop.Channels.ChannelSelected += new EventHandler(ChannelController_Selected);
				_workshop.Channels.ChannelVisibilityChanged += new EventHandler(ChannelController_VisibilityChanged);

				_workshop.UI.LatticeSizeChanged += new EventHandler(this.UI_LatticeScaleChanged);
				_workshop.UI.CellSizeChanged += new EventHandler(this.UI_LatticeScaleChanged);
				_workshop.UI.DisplayGridLines += new EventHandler(this.UI_LatticeScaleChanged);
				_workshop.UI.Zooming += new EventHandler(this.UI_Zooming);
				_workshop.UI.DisplayRuler += new EventHandler(this.UI_DisplayRuler);
				_workshop.UI.Refresh += new EventHandler(this.UI_Refresh);
				_workshop.UI.MaskDisplayChanged += new EventHandler(this.UI_Refresh);

				_workshop.Mask.Defined += new EventHandler(Mask_Defined);
				_workshop.Mask.Cleared += new EventHandler(Mask_Cleared);

				Workshop.Canvas = this.CanvasPane;
				_workshop.UI.Background.Set();
			}
		}
				
		#endregion [ Properties ]

		#region [ Constructors ]

		public CanvasWindow()
		{
			InitializeComponent();
			
			_rulerTextFormatter = new StringFormat();
			_rulerTextFormatter.LineAlignment = StringAlignment.Center;
			_rulerTextFormatter.Alignment = StringAlignment.Center;

			pnlEasel.HorizontalScroll.LargeChange = 1000;
			pnlEasel.VerticalScroll.LargeChange = 1000;

			SetCanvasSize();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		///// <summary>
		///// Determine if this tool inherits MaskBase, if so, then send it information that the mask is cleared.
		///// </summary>
		//private void InformToolMaskIsCleared()
		//{
		//    if (_workshop.CurrentTool == null)
		//        return;

		//    if (_workshop.CurrentTool.Tool.GetType().IsSubclassOf(typeof(Tools.MaskBase)))
		//        ((Tools.MaskBase)_workshop.CurrentTool.Tool).MaskIsCleared();

		//    CanvasPane.Refresh();
		//}

		/// <summary>
		/// Forces the refresh of the Ruler controls, if they are visible. This should be called everytime there is a chance the tick marks could be moved, or if the mouse cursor
		/// is within the CanvasPane and it's ticks need to update.
		/// </summary>
		private void RefreshRulers()
		{
			if (RulerHorz.Visible)
				RulerHorz.Refresh();
			if (RulerVert.Visible)
				RulerVert.Refresh();
		}

		/// <summary>
		/// Moves the position of the control CanvasPane on the form.
		/// </summary>
		private void SetCanvasPosition()
		{
			if (!_isFormShown)
				return;

			//Debug.WriteLine("HScroll Visible: " + pnlWorkspace.HorizontalScroll.Visible);
			//Debug.WriteLine("HScroll Pct: " + GetScrollPercent(pnlWorkspace.HorizontalScroll));
			//Debug.WriteLine("VScroll Visible: " + pnlWorkspace.VerticalScroll.Visible);
			//Debug.WriteLine("VScroll Pct: " + GetScrollPercent(pnlWorkspace.VerticalScroll));

			int ClientRectangleWidth = pnlWorkspace.Width;
			int ClientRectangleHeight = pnlWorkspace.Height;

			if ((UISettings.ʃCanvasSize.Width < ClientRectangleWidth) &&
				(UISettings.ʃCanvasSize.Height < ClientRectangleHeight))
			{
				pnlEasel.Size = new Size(ClientRectangleWidth, ClientRectangleHeight);
				_hScrollPct = 0.5f;
				_vScrollPct = 0.5f;
			}
			else
			{
				// CanvasSize from the UISettings object is already scaled to zoom
				pnlEasel.Size = new Size(UISettings.ʃCanvasSize.Width + (ClientRectangleWidth / 2),
										 UISettings.ʃCanvasSize.Height + (ClientRectangleHeight / 2));


			}

			CanvasPane.Location = new Point((pnlEasel.Width - CanvasPane.Width) / 2,
											(pnlEasel.Height - CanvasPane.Height) / 2);

			if (!_settingZoom)
				pnlWorkspace.PerformLayout();
		}
		
		/// <summary>
		/// Updates the size of the CanvasPane control to match what UISettings has calculated for it. Updates the Easel control to best fit and determine if the scrollbars should display.
		/// This method is called whenever CellSize, GridLineWidth, Zoom, or the LatticeSize has been changed, as well as initially when the form first loads.
		/// </summary>
		private void SetCanvasSize()
		{
			if ((UISettings.ʃCanvasSize.Width <= 0) || (UISettings.ʃCanvasSize.Height <= 0))
				return;

			CanvasPane.Size = new Size(UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height);
		}

		/// <summary>
		/// Determines what the percentage a given scroll bar is at
		/// </summary>
		/// <param name="scroll">ScrollProperties attribute from a scroll bar control</param>
		/// <returns>The calculated percentage a scroll bar is currently at</returns>
		private float GetScrollPercent(ScrollProperties scroll)
		{
			int MaxScroll = scroll.Maximum - scroll.LargeChange - 1;
			return (float)scroll.Value / (float)MaxScroll;
		}

		/// <summary>
		/// Set the scrolling amount based on pre-calculated percentages.
		/// </summary>
		private void SetScrollPercent()
		{
			SetScrollPercent(pnlWorkspace.HorizontalScroll, _hScrollPct);
			SetScrollPercent(pnlWorkspace.VerticalScroll, _vScrollPct);
			if (!_settingZoom)
				pnlWorkspace.PerformLayout();
		}

		/// <summary>
		/// Sets the scroll value of a scroll bar control based on the percentage passed in
		/// </summary>
		/// <param name="scroll">ScrollProperties attribute from a scroll bar control</param>
		/// <param name="scrollpercent">Percent of scrolling to set. 0 indicated all the way left.</param>
		private void SetScrollPercent(ScrollProperties scroll, float scrollpercent)
		{
			int MaxScroll = scroll.Maximum - scroll.LargeChange - 1;
			if (scrollpercent < 0f)
				scrollpercent = 0f;
			else if (scrollpercent > 100f)
				scrollpercent = 100f;
			scroll.Value = (int)(scrollpercent * MaxScroll);
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ CanvasPane Events ]

		/// <summary>
		/// Occurs when the cursor enters the CanvasPane
		/// </summary>
		private void CanvasPane_MouseEnter(object sender, EventArgs e)
		{
			_mouseOverCanvasPane = true;
			CanvasPane.Refresh();
		}

		/// <summary>
		/// Occurs when the cursor leaves the CanvasPane
		/// </summary>
		private void CanvasPane_MouseLeave(object sender, EventArgs e)
		{
			_mouseOverCanvasPane = false;
			CanvasPane.Refresh();
		}

		/// <summary>
		/// Occurs when the mouse moves over the CanvasPane.
		/// Update the CurrentMouseCell in UISettings with the mouse position, and calls the Rulers to update.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanvasPane_MouseMove(object sender, MouseEventArgs e)
		{
			_workshop.UI.MousePosition = Workshop.CellPoint(new Point(e.X, e.Y));
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.MouseMove(e.Button, _workshop.UI.MousePosition, e.Location);
			RefreshRulers();
		}

		/// <summary>
		/// Create the display of the Canvas for the CanvasPane
		/// </summary>
		private void CanvasPane_Paint(object sender, PaintEventArgs e)
		{
			Matrix MoveMatrix = null;
			GraphicsPath Path = null;

			try
			{
				e.Graphics.FillRectangle(((_workshop.UI.Background.Image != null) ? Brushes.Transparent : Brushes.Black),
											e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);

				if (_workshop.UI.SuperimposeGridOnBackground)
				{
					using (Pen GridPen = new Pen(Color.Black))
					{
						for (int x = 0; x <= e.ClipRectangle.Right; x += UISettings.ʃCellScale)
							e.Graphics.DrawLine(GridPen, x, 0, x, e.ClipRectangle.Bottom);
						for (int y = 0; y <= e.ClipRectangle.Bottom; y += UISettings.ʃCellScale)
							e.Graphics.DrawLine(GridPen, 0, y, e.ClipRectangle.Right, y);
					}
				}

				foreach (Channel Channel in _workshop.Channels.GetAllChannels().OrderByDescending(c => c.Index).ToList())
				{
					// If the Channel is not Visible, or is Hidden, or is one of the selected Channels, do not draw it here in this block
					if (!Channel.Visible || Channel.IsHidden || Channel.IsSelected) 
						continue;

					using (SolidBrush ChannelBrush = new SolidBrush(Color.FromArgb(_workshop.UI.InactiveChannelAlpha, Channel.Color)))
					{
						Path = Channel.GetGraphicsPath();
						e.Graphics.FillPath(ChannelBrush, Path);
					}
				}

				// Draw the Selected Channels on top of the unselected ones (for clarity)
				if (_workshop.Channels.Selected.Count > 0)
				{
					foreach (Channel Channel in _workshop.Channels.Selected.OrderByDescending(c => c.Index).ToList())
					{
						// Do not draw invisible or Hidden Channels
						if (!Channel.Visible || Channel.IsHidden)
							continue;

						using (SolidBrush ChannelBrush = new SolidBrush(Channel.Color))
						{
							Path = Channel.GetGraphicsPath();
							e.Graphics.FillPath(ChannelBrush, Path);
						}
					}
				}

				// Draw the "move" Channel (full intensity) if it has data
				if (_workshop.Channels.MoveChannel.HasData)
				{
					using (SolidBrush ChannelBrush = new SolidBrush(_workshop.Channels.MoveChannel.Color))
					{
						Path = _workshop.Channels.MoveChannel.GetGraphicsPath();
						e.Graphics.FillPath(ChannelBrush, Path);
					}
				}

				// Draw the "image stamp" Channel (full intensity) if it has data and the mouse is over the PictureBox
				if (_workshop.ShowImageStamp && _mouseOverCanvasPane)
				{
					Point Offset = _workshop.Channels.ImageStampChannel.Origin;
					//Rectangle Bounds = _workshop.Channels.ImageStampChannel.GetBounds();
										
					//Debug.Write(Offset.ToString() + "\t");

					//Offset.X *= UISettings.ʃCellScale;
					//Offset.Y *= UISettings.ʃCellScale;

					//Offset.X -= Bounds.Width / 2;
					//Offset.Y -= Bounds.Height / 2;

					Offset = Workshop.PixelPoint(Offset);

					//Debug.WriteLine(Offset);

					using (SolidBrush ChannelBrush = new SolidBrush(Color.White))
					{
						Path = (GraphicsPath)_workshop.Channels.ImageStampChannel.GetGraphicsPath().Clone();
						MoveMatrix = new Matrix();
						MoveMatrix.Translate(Offset.X, Offset.Y);
						Path.Transform(MoveMatrix);
						MoveMatrix.Dispose();
						e.Graphics.FillPath(ChannelBrush, Path);
						Path.Dispose();
					}
				}

				// Render the Mask marquee and overlay, as needed
				if (_workshop.Mask.HasMask && !_movingMask)
				{
					if (_workshop.UI.ShowMaskMarquee)
						using (Pen MarqueePen = _workshop.GetMarqueePen())
							e.Graphics.DrawPath(MarqueePen, _workshop.Mask.CanvasMask.Outline);

					if (_workshop.UI.ShowMaskOverlay)
						e.Graphics.DrawImage(_workshop.Mask.Overlay, new Point(0, 0));
				}

				RefreshRulers();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				Path = null;
				MoveMatrix = null;
			}
		}

		/// <summary>
		/// Occurs when the user clicks the mouse button on the CanvasPane
		/// </summary>
		private void CanvasPane_Click(object sender, EventArgs e)
		{
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.MouseClick(_mouseButtons, _workshop.UI.MousePosition, _workshop.UI.MousePosition);
		}

		/// <summary>
		/// Occurs when the user double-clicks the mouse button on the CanvasPane
		/// </summary>
		private void CanvasPane_DoubleClick(object sender, EventArgs e)
		{
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.MouseDoubleClick(_mouseButtons, _workshop.UI.MousePosition, _workshop.UI.MousePosition);
		}

		/// <summary>
		/// Occurs when the user holds the mouse button down on the CanvasPane
		/// </summary>
		private void CanvasPane_MouseDown(object sender, MouseEventArgs e)
		{
			_mouseButtons = e.Button;
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.MouseDown(e.Button, _workshop.UI.MousePosition, e.Location);
		}
		
		/// <summary>
		/// Occurs when the user releases the mouse button over the CanvasPane
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanvasPane_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseButtons = e.Button;
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.MouseUp(e.Button, _workshop.UI.MousePosition, e.Location);
		}

		#endregion [ CanvasPane Events ]

		#region [ Form Events ]

		/// <summary>
		/// Event that fires when the window is first displayed
		/// </summary>
		private void Form_Shown(object sender, EventArgs e)
		{
			_isFormShown = true;
			SetCanvasPosition();
			_hScrollPct = 0.5f;
			_vScrollPct = 0.5f;
		}

		/// <summary>
		/// Event that fires whenever the size of the form changes
		/// </summary>
		private void Form_SizeChanged(object sender, EventArgs e)
		{
			SetCanvasPosition();
		}
		
		#endregion [ Form Events ]

		#region [ ChannelController Events ]

		/// <summary>
		/// Event that fires from the ChannelController when the Color on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_ColorChanged(object sender, EventArgs e)
		{
			CanvasPane.Refresh();
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Selected flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_Selected(object sender, EventArgs e)
		{
			CanvasPane.Refresh();
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Visibility flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_VisibilityChanged(object sender, EventArgs e)
		{
			CanvasPane.Refresh();
		}
		
		#endregion [ ChannelController Events ]

		#region [ UISettings Events ]
				
		private void UI_LatticeScaleChanged(object sender, EventArgs e)
		{
			if (_settingZoom)
				return;

			DrawingControl.SuspendDrawing(pnlWorkspace);
			SetCanvasSize();
			SetCanvasPosition();

			// Regenerate the composite background image with the correct zoom scale
			_workshop.UI.Background.Set();

			if (_workshop.UI.Zoom > UISettings.MIN_ZOOM)
				SetScrollPercent();
			DrawingControl.ResumeDrawing(pnlWorkspace);

			CanvasPane.Refresh();
		}

		private void UI_Zooming(object sender, EventArgs e)
		{
			if (!_settingZoom)
			{
				DrawingControl.SuspendDrawing(pnlWorkspace);
				SetCanvasSize();
				SetCanvasPosition();

				// Regenerate the composite background image with the correct zoom scale
				_workshop.UI.Background.Set();

				if (_workshop.UI.Zoom > UISettings.MIN_ZOOM)
					SetScrollPercent();
				DrawingControl.ResumeDrawing(pnlWorkspace);
			}

			CanvasPane.Refresh();
		}

		private void UI_DisplayRuler(object sender, EventArgs e)
		{
			bool Visible = _workshop.UI.ShowRuler;
			RulerHorz.Visible = Visible;
			RulerVert.Visible = Visible;
			SetCanvasPosition();
		}

		private void UI_Refresh(object sender, EventArgs e)
		{
			CanvasPane.Refresh();
		}

		#endregion [ UISettings Events ]

		#region [ Mask Events ]

		/// <summary>
		/// Occurs when the Mask is Defined
		/// </summary>
		public void Mask_Defined(object sender, EventArgs e)
		{
			MarqueeAnimationTimer.Enabled = true;
		}

		/// <summary>
		/// Occurs when the Mask is Cleared
		/// </summary>
		public void Mask_Cleared(object sender, EventArgs e)
		{
			MarqueeAnimationTimer.Enabled = false;
		}

		#endregion [ Mask Events ]

		#region [ Workshop Events ]

		///// <summary>
		///// This event fires when a value in the Workshop or one of its child objects has been changed.
		///// </summary>
		//private void Workshop_Changed(object sender, DataEventArgs e)
		//{
		//    switch (e.Category)
		//    {				

		//        //#region [ Mask ]

		//        //case EventCategory.Mask:
		//        //    switch (e.SubCategory)
		//        //    {
		//        //        //case EventSubCategory.Mask_Defined:
		//        //        //    MarqueeAnimationTimer.Enabled = true;
		//        //        //    break;

		//        //        case EventSubCategory.Mask_Cleared:
		//        //            MarqueeAnimationTimer.Enabled = false;
		//        //            InformToolMaskIsCleared();

		//        //            //if (_workshop.MoveChannel.HasData())
		//        //            //{
		//        //            //    Editor.Clipboard.RestoreFromMoveChannel();
		//        //            //    ExposePane(_workshop.MoveChannel, Panes.MoveChannel);
		//        //            //    ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //            //}

		//        //            CanvasPane.Refresh();
		//        //            break;

		//        //        case EventSubCategory.Mask_Moved:
		//        //            //ExposePane(_maskPane, Panes.Mask);
		//        //            CanvasPane.Refresh();
		//        //            break;
		//        //    }
		//        //    break;

		//        //#endregion [ Mask ]

		//        #region [ DEAD CODE ]

		//        //#region [ Channel ]

		//        //case EventCategory.Channel:
		//        //    CanvasPane.Refresh();
		//        //    break;

		//        //switch (e.SpecificEventType)
		//        //{
		//        //    //case SpecificEventType.Channel_Active:
		//        //    case SpecificEventType.Channel_Selected:
		//        //        //if (_workshop.MoveChannel.HasData())
		//        //        //{
		//        //        //    Editor.Clipboard.RestoreFromMoveChannel(_workshop.Channels[_workshop.PriorActiveChannelIndex]);
		//        //        //    Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //        //    _workshop.DeleteCells();
		//        //        //    _workshop.ClearMask();
		//        //        //}
		//        //        //Canvas.Refresh();
		//        //        break;

		//        //    default:
		//        //        //if (Canvas.Cursor == NotCursor)
		//        //        //Workshop_Changed(sender, new CanvasEventArgs(GeneralCanvasEvent.Tool, SpecificEventType.NotSet));
		//        //        //Canvas.Refresh();
		//        //        break;
		//        //}
		//        //CanvasPane.Refresh();
		//        //break;

		//        //#endregion [ Channel ]

		//        //#region [ UI ]

		//        //case EventCategory.UI:
		//        //    switch (e.SubCategory)
		//        //    {
		//        //        //case EventSubCategory.CellSize:
		//        //        //case EventSubCategory.GridLineWidth:
		//        //        //case EventSubCategory.LatticeSize:
		//        //        //    SetCanvasSize();
		//        //        //    SetCanvasPosition();
		//        //        //    pnlWorkspace.PerformLayout();
		//        //        //    break;

		//        //        //case EventSubCategory.Zoom:
		//        //        //    if (!_settingZoom)
		//        //        //    {
		//        //        //        DrawingControl.SuspendDrawing(pnlWorkspace);
		//        //        //        SetCanvasSize();
		//        //        //        SetCanvasPosition();

		//        //        //        // Regenerate the composite background image with the correct zoom scale
		//        //        //        _workshop.UI.Background.Set();

		//        //        //        if (_workshop.UI.Zoom > UISettings.MIN_ZOOM)
		//        //        //            SetScrollPercent();
		//        //        //        DrawingControl.ResumeDrawing(pnlWorkspace);
		//        //        //    }
		//        //        //    break;

		//        //        //case EventSubCategory.ShowRuler:
		//        //        //    bool Visible = _workshop.UI.ShowRuler;
		//        //        //    //RulerCorner.Visible = Visible;
		//        //        //    RulerHorz.Visible = Visible;
		//        //        //    RulerVert.Visible = Visible;
		//        //        //    SetCanvasPosition();
		//        //        //    break;

		//        //        //case EventSubCategory.SuperimposeGridOnBackground:
		//        //        //    CanvasPane.Refresh();
		//        //        //    break;

		//        //        #region [ DEAD CODE ]

		//        //        //case SpecificEventType.BackgroundImage_Clear:
		//        //        //    CanvasPane.BackgroundImage = null;
		//        //        //    CanvasPane.Refresh();
		//        //        //    break;

		//        //        //case SpecificEventType.BackgroundImage_Load:
		//        //        //    CanvasPane.BackgroundImage = _workshop.UI.BackgroundImage;
		//        //        //    //SetCanvasSize(_data.GetImageSizeInCells());
		//        //        //    CanvasPane.Refresh();
		//        //        //    break;

		//        //        //case SpecificEventType.Zoom:
		//        //        //_workshop.ClearMask();
		//        //        //SetCanvasSize();
		//        //        //if (_workshop.UI.Background.HasData)
		//        //        //{
		//        //        //    if (_workshop.UI.Zoom > 1)
		//        //        //    {
		//        //        //        // stretch the background image
		//        //        //        Bitmap StretchBackground = new Bitmap(_workshop.UI.Background.Image, (int)(_workshop.UI.Background.Image.Width * _workshop.UI.Zoom), (int)(_workshop.UI.Background.Image.Height * _workshop.UI.Zoom));
		//        //        //        CanvasPane.BackgroundImage = StretchBackground;
		//        //        //    }
		//        //        //    else
		//        //        //    {
		//        //        //        CanvasPane.BackgroundImage = _workshop.UI.Background.Image;
		//        //        //        //SetBrightness(_workshop.UI.BackgroundImage_Brightness);
		//        //        //    }
		//        //        //}

		//        //        //break;

		//        //        //case SpecificEventType.LatticeSize:
		//        //        //    SetCanvasSize(_workshop.UI.LatticeSize);
		//        //        //    break;

		//        //        //case SpecificEventType.UseOriginalUI:
		//        //        //    CanvasPane.Cursor = Cursors.Default;
		//        //        //    break;

		//        //        //case SpecificEventType.BackgroundImage_Brightness:
		//        //        //    SetBrightness(_workshop.UI.BackgroundImage_Brightness);
		//        //        //    break;

		//        //        #endregion [ DEAD CODE ]
		//        //    }
		//        //    break;

		//        //#endregion [ UI ]


		//        //#region [ Clipboard ]

		//        //case GeneralDataEvent.Clipboard:
		//        //    switch (e.SpecificEventType)
		//        //    {
		//        //case SpecificEventType.Clipboard_Cut:
		//        //    if (!Editor.Clipboard.Cut())
		//        //        MessageBox.Show("Selection is Empty!", "Cut", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//        //    else
		//        //    {
		//        //        _workshop.ClearMask();
		//        //        Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //        Editor.ExposePane(_workshop.MoveChannel, Panes.MoveChannel);
		//        //    }
		//        //    break;

		//        //case SpecificEventType.Clipboard_Copy:
		//        //    if (!Editor.Clipboard.Copy())
		//        //        MessageBox.Show("Selection is Empty!", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//        //    Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //    break;

		//        //        case SpecificEventType.Clipboard_Delete:
		//        //            Editor.Clipboard.Delete();
		//        //            using (Region Region = new System.Drawing.Region(_workshop.Mask.CanvasMask.Outline))
		//        //                CanvasPane.Invalidate(Region);
		//        //            Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //            break;

		//        //        case SpecificEventType.Clipboard_Paste:
		//        //            if (Editor.Clipboard.HasData)
		//        //            {
		//        //                Editor.Clipboard.Paste();
		//        //                using (Region Region = new System.Drawing.Region(_workshop.Mask.CanvasMask.Outline))
		//        //                    CanvasPane.Invalidate(Region);
		//        //                //Editor.ExposePane(_maskPane, Panes.Mask);
		//        //                Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		//        //                Editor.ExposePane(_workshop.MoveChannel, Panes.MoveChannel);
		//        //            }
		//        //            break;
		//        //    }
		//        //    break;

		//        //#endregion [ Clipboard ]


		//        //#region [ Tool ]

		//        //case GeneralCanvasEvent.ToolSettings:
		//        //case GeneralCanvasEvent.Tool:

		//        //    switch (e.SpecificEventType)
		//        //    {

		//        //case SpecificEventType.Tool_Selected:

		//        //    switch (_workshop.CurrentTool)
		//        //    {
		//        //        case (int)Tool.Text:
		//        //            _workshop.ImageStampChannel.ClearAllCells();
		//        //            break;

		//        //        case (int)Tool.ImageStamp:
		//        //            LoadImageStamp();
		//        //            break;
		//        //    }
		//        //    if (_workshop.CurrentPlugInTool != null)
		//        //        Canvas.Cursor = _workshop.CurrentPlugInTool.Tool.Cursor;
		//        //    break;



		//        //}

		//        ////SetToolCursor(_workshop.CurrentTool);
		//        //Canvas.Refresh();

		//        //break;

		//        //#endregion [ Tool ]

		//        //#region [ Undo ]

		//        //case GeneralDataEvent.Undo:
		//        //    switch (e.SpecificEventType)
		//        //    {
		//        //        //case SpecificEventType.UndoStackPushed:
		//        //        //_data.RedoStack.Clear();
		//        //        //break;

		//        //        case SpecificEventType.Undo:
		//        //            PerformUndo(_workshop.UndoController.Pop());
		//        //            break;

		//        //        //case SpecificEventType.Redo:
		//        //        //PerformUndo(_data.RedoStack.Pop());
		//        //        //break;

		//        //    }
		//        //    break;

		//        //#endregion [ Undo ]
		//        #endregion [ DEAD CODE ]
		//    }
		//}

		/// <summary>
		/// This event fires when the user, using the Zoom Tool, has clicked on the Canvas. The normal Changed event is suppressed,
		/// and this one fires instead. The CanvasPane should be scrolled such that the click point is centered on the screen, and the new Zoom Level
		/// set.
		/// </summary>
		private void Workshop_ClickZoom(object sender, ZoomEventArgs e)
		{
			_settingZoom = true;
			_settingScroll = true;

			DrawingControl.SuspendDrawing(pnlWorkspace);

			float OldZoom = _workshop.UI.Zoom;

			// Perform the changes to zoom
			_workshop.UI.Zoom = e.ZoomLevel;

			SetCanvasSize();
			SetCanvasPosition();

			// Regenerate the composite background image with the correct zoom scale
			_workshop.UI.Background.Set();

			PointF ZoomPoint = e.ZoomPoint;

			// Convert the zoom point from old zoom, to the new value.
			// We do this because the Zoom event has already fired by the time this event fires, and we need to scale the 
			// point accordingly
			ZoomPoint = new PointF((ZoomPoint.X / OldZoom) * _workshop.UI.Zoom, (ZoomPoint.Y / OldZoom) * _workshop.UI.Zoom);
			OldZoom = _workshop.UI.Zoom;
			_hScrollPct = ZoomPoint.X / UISettings.ʃCanvasSize.Width;
			_vScrollPct = ZoomPoint.Y / UISettings.ʃCanvasSize.Height;

			//Debug.WriteLine("Zoom Point: " + ZoomPoint.ToString());
			//Debug.Write("Scroll Range: " + (pnlWorkspace.HorizontalScroll.Maximum - pnlWorkspace.HorizontalScroll.LargeChange + 1));
			//Debug.WriteLine(",: " + (pnlWorkspace.VerticalScroll.Maximum - pnlWorkspace.VerticalScroll.LargeChange + 1));
			//Debug.WriteLine("Canvas Size: " + UISettings.ʃCanvasSize);
			//Debug.Write("H%: " + _hScrollPct.ToString("0.0%"));		
			//Debug.WriteLine(", V%: " + _vScrollPct.ToString("0.0%"));

			_settingScroll = false;
			_settingZoom = false;

			SetScrollPercent();
			DrawingControl.ResumeDrawing(pnlWorkspace);
		}

		#endregion [ Workshop Events ]

		/// <summary>
		/// Timer that animates the marquee around a masked area
		/// </summary>
		private void MarqueeAnimationTimer_Tick(object sender, EventArgs e)
		{
			if (_workshop.Mask.HasMask) 
			{
				GraphicsPath TempPath = (GraphicsPath)_workshop.Mask.CanvasMask.Outline.Clone();
				TempPath.Widen(new Pen(Color.White, 1));
				CanvasPane.Invalidate(new Region(TempPath));
				TempPath.Dispose();
			}
		}

		/// <summary>
		/// This event fires when one or more of the pnlWorkspace scroll bars are scrolled.
		/// </summary>
		private void pnlWorkspace_Scroll(object sender, ScrollEventArgs e)
		{
			RefreshRulers();
			if (!_settingScroll)
			{
				_hScrollPct = GetScrollPercent(pnlWorkspace.HorizontalScroll);
				_vScrollPct = GetScrollPercent(pnlWorkspace.VerticalScroll);
			}
		}

		/// <summary>
		/// Paint on the ruler ticks
		/// </summary>
		private void RulerHorz_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillRectangle(new SolidBrush(RulerHorz.BackColor), e.ClipRectangle);

			Pen DrawPen = new Pen(Color.Black);
			SolidBrush TextBrush = new SolidBrush(RulerHorz.ForeColor);

			RectangleF Rect;
			string Text = string.Empty;

			// Determine the offset for the Ruler
			// First account for the difference in where the ruler's origin is vs the Workspace's origin.
			int StartTick = pnlWorkspace.Location.X - RulerHorz.Location.X;

			// Next, determine where the origin of the Canvas is relative to this point
			StartTick += pnlEasel.Location.X;
			StartTick += CanvasPane.Location.X;
			
			//Debug.WriteLine(CanvasPane.Location);

			int Tick = 0;
			int Bump = 0;
			int TextPos = 0;

			for (int x = 0; x < UISettings.ʃLatticeSize.Width; x++)
			{
				Tick = StartTick + (x * UISettings.ʃCellScale);
				Bump = (x % 10 == 0) ? 12 : ((x % 10 == 5) ? 8 : 6);
				e.Graphics.DrawLine(DrawPen, new Point(Tick, RulerHorz.Height - Bump), new Point(Tick, RulerHorz.Height));

				if (x % 10 == 0)
				{
					Text = x.ToString();
					TextPos = RulerHorz.Height - Bump;
					Rect = new RectangleF(new Point(Tick, TextPos), e.Graphics.MeasureString(Text, RulerHorz.Font));
					TextPos -= (int)Rect.Height;
					Rect.Location = new PointF(Tick - (Rect.Width / 2), TextPos);
					e.Graphics.DrawString(Text, RulerHorz.Font, TextBrush, Rect, _rulerTextFormatter);
				}
			}

			// If the mouse is over the CanvasPane, draw the position tick mark
			if (_mouseOverCanvasPane)
			{
				Tick = StartTick + Workshop.PixelPoint(_workshop.UI.MousePosition).X;
				e.Graphics.DrawLine(DrawPen, new Point(Tick, RulerHorz.Top), new Point(Tick, RulerHorz.Height));
			}

			DrawPen.Dispose();
			DrawPen = null;
			TextBrush.Dispose();
			TextBrush = null;
		}

		/// <summary>
		/// Paint on the ruler ticks
		/// </summary>
		private void RulerVert_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle);
			Pen DrawPen = new Pen(Color.Black);
			RectangleF Rect;
			SolidBrush TextBrush = new SolidBrush(RulerVert.ForeColor);
			string Text = string.Empty;

			// Determine the offset for the Ruler
			// First account for the difference in where the ruler's origin is vs the Workspace's origin.
			int StartTick = pnlWorkspace.Location.Y - RulerVert.Location.Y;

			// Next, determine where the origin of the Canvas is relative to this point
			StartTick += pnlEasel.Location.Y;
			StartTick += CanvasPane.Location.Y;
			int Tick = 0;
			int Bump = 0;
			int TextPos = 0;

			for (int y = 0; y < UISettings.ʃLatticeSize.Height; y++)
			{
				Tick = StartTick + (y * UISettings.ʃCellScale);
				Bump = (y % 10 == 0) ? 12 : ((y % 10 == 5) ? 8 : 6);
				e.Graphics.DrawLine(DrawPen, new Point(RulerVert.Width - Bump, Tick), new Point(RulerVert.Width, Tick));

				if (y % 10 == 0)
				{
					Text = y.ToString();
					TextPos = RulerVert.Width - Bump;
					Rect = new RectangleF(new Point(TextPos, Tick), e.Graphics.MeasureString(Text, RulerVert.Font));
					TextPos -= (int)Rect.Width;
					Rect.Location = new PointF(TextPos, Tick - (Rect.Height / 2));
					e.Graphics.DrawString(Text, RulerVert.Font, TextBrush, Rect, _rulerTextFormatter);
				}
			}

			// If the mouse is over the CanvasPane, draw the position tick mark
			if (_mouseOverCanvasPane)
			{
				Tick = StartTick + Workshop.PixelPoint(_workshop.UI.MousePosition).Y;
				e.Graphics.DrawLine(DrawPen, new Point(RulerVert.Left, Tick), new Point(RulerVert.Width, Tick));
			}
			
			DrawPen.Dispose();
			DrawPen = null;
			TextBrush.Dispose();
			TextBrush = null;
		}

		#endregion [ Events ]

		#region [ DEAD CODE ]

		///// <summary>
		///// Sets the brightness value of a given image. Typically used for the Canvas background image.
		///// </summary>
		///// <param name="sourceImage">Image to change the brightness of. This bitmap is not altered in the process</param>
		///// <param name="value">Amount to change the brightness. Negative numbers darken, positive lighten</param>
		///// <returns>Copy of the original image with brightness changed.</returns>
		//private Bitmap SetBrightness(Bitmap sourceImage, float value)
		//{
		//    Bitmap img = new Bitmap(sourceImage);

		//    // If the value is 0, no change is needed
		//    if (value == 0)
		//        return img;

		//    // Value is between -1.0 and 1.0
		//    if (img != null)
		//    {
		//        ColorMatrix cMatrix = new ColorMatrix(new float[][] {
		//                new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
		//                new float[] { value, value, value, 0.0f, 1.0f }
		//            });

		//        Graphics gr = Graphics.FromImage(img);
		//        ImageAttributes attrs = new ImageAttributes();
		//        attrs.SetColorMatrix(cMatrix);
		//        gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attrs);
		//        gr.Dispose();
		//        attrs.Dispose();
		//    }
		//    return img;
		//}

		///// <summary>
		///// Darkens or lightens an image
		///// </summary>
		///// <param name="value">Value is between -1.0 and 1.0</param>
		//private void SetBrightness(float value)
		//{
		//    if (_workshop.UI.BackgroundImage == null)
		//        return;

		//    CanvasPane.BackgroundImage = SetBrightness(new Bitmap(_workshop.UI.BackgroundImage), value);
		//    _brightnessAdjustedBitmap = new Bitmap(CanvasPane.BackgroundImage);
		//    CanvasPane.Refresh();
		//}

		///// <summary>
		///// Removes all the Cells for a given Channel
		///// </summary>
		///// <param name="index">Index of the Channel to clear</param>
		//private void ClearChannel(int index)
		//{
		//    if ((index < 0) || (index >= _workshop.Channels.Count))
		//        return;

		//    _workshop.Channels[index].ClearLattice();
		//}

		//public bool Dirty
		//{
		//    get { return _workshop.Dirty; }
		//    set { _workshop.Dirty = value; }
		//}

		///// <summary>
		///// Clear out the background image on CanvasPane
		///// </summary>
		//private void ClearImage()
		//{
		//    CanvasPane.BackgroundImage = null;
		//    if (_workshop.UI.BackgroundImage != null)
		//    {
		//        _workshop.UI.BackgroundImage.Dispose();
		//        _workshop.UI.BackgroundImage = null;
		//    }
		//    this.Dirty = true;
		//}

		///// <summary>
		///// Draw the Channel onto the CanvasPane
		///// NOTE: This function appears in CanvasWindow, BaseTool and Channel
		///// </summary>
		///// <param name="g">Graphics object used to draw the Channel</param>
		///// <param name="clipRect">Rectangle defining the part of the Channel to draw</param>
		///// <param name="ChannelBrush">Brush to use to draw with</param>
		///// <param name="Channel">Channel data</param>
		///// <param name="offset">Offset from the origin for the cells of the Channel</param>
		//private void DrawChannel(Graphics g, Rectangle clipRect, Brush ChannelBrush, Channel Channel, Point offset)
		//{
		//    int X, Y;
		//    foreach (Point Cell in Channel.Lattice)
		//    {
		//        if (offset.IsEmpty)
		//        {
		//            X = (Cell.X + Channel.Origin.X) * UISettings.ʃCellScale;
		//            Y = (Cell.Y + Channel.Origin.Y) * UISettings.ʃCellScale;
		//        }
		//        else
		//        {
		//            X = (Cell.X + Channel.Origin.X + offset.X) * UISettings.ʃCellScale;
		//            Y = (Cell.Y + Channel.Origin.Y + offset.Y) * UISettings.ʃCellScale;
		//        }

		//        if (clipRect.Contains(X, Y))
		//        {
		//            Rectangle CellRect = new Rectangle(X + 1, Y + 1, UISettings.ʃCellZoom, UISettings.ʃCellZoom);
		//            g.FillRectangle(ChannelBrush, CellRect);
		//            if (!Channel.BorderColor.IsEmpty)
		//                using (Pen BorderPen = new Pen(Channel.BorderColor))
		//                    g.DrawRectangle(BorderPen, CellRect);
		//        }
		//    }
		//}

		///// <summary>
		///// Performs the indicated Undo
		///// </summary>
		///// <param name="UndoData"></param>
		//private void PerformUndo(UndoData.v1 UndoData)
		//{
		//    //UndoData.v1 RedoData = new UndoData.v1();
		//    //RedoData.Action = UndoData.v1.Action;
		//    //RedoData.GeneralEvent = UndoData.v1.GeneralEvent;
		//    //RedoData.SpecificEvent = UndoData.v1.SpecificEvent;

		//    int Index = 0;

		//    switch (UndoData.GeneralEvent)
		//    {
		//        #region [ Channel ]

		//        case GeneralDataEvent.Channel:
		//            switch (UndoData.SpecificEvent)
		//            {
		//                case SpecificEventType.Channel_Cells:
		//                    //RedoData.AffectedChannels = UndoData.AffectedChannels;
		//                    for (int i = 0; i < UndoData.AffectedChannels.Count; i++)
		//                    {
		//                        Index = UndoData.AffectedChannels[i];
		//                        if (Index < 0)
		//                            continue;
		//                        if (i == 0)
		//                            _workshop.ActiveChannelIndex = Index;

		//                        _workshop.Channels[Index].ClearLattice();
		//                        _workshop.Channels[Index].PaintCells(UndoData.Cells[i]);
		//                    }

		//                    if (UndoData.Mask.HasMask)
		//                        _workshop.SetMask(UndoData.Mask);
		//                    //if (UndoData.MaskOutline != null)
		//                        //_workshop.SetMask(UndoData.MaskOutline, UndoData.MaskRegion);

		//                    break;

		//                case SpecificEventType.Channel_Import:
		//                    //RedoData.AffectedChannels = UndoData.AffectedChannels;
		//                    for (int i = 0; i < UndoData.AffectedChannels.Count; i++)
		//                    {
		//                        Index = UndoData.AffectedChannels[i];
		//                        if (i == 0)
		//                            _workshop.ActiveChannelIndex = Index;
		//                        //RedoData.Cells.Add(_data.Channels[Index].Cells);
		//                        _workshop.Channels[Index].ClearLattice();
		//                        _workshop.Channels[Index].PaintCells(UndoData.Cells[i]);
		//                        //_data.Channels[Index].Cells = UndoData.Cells[i];
		//                    }
		//                    //RedoData.LatticeSize = _data.UI.LatticeSize;
		//                    _workshop.UI.LatticeSize = UndoData.LatticeSize;
		//                    SetCanvasSize(UndoData.LatticeSize);
		//                    break;

		//                case SpecificEventType.Channel_Color:
		//                    //RedoData.AffectedChannels = UndoData.AffectedChannels;
		//                    Index = UndoData.AffectedChannels[0];
		//                    //RedoData.ChannelColor = UndoData.ChannelColor;
		//                    _workshop.Channels[Index].Color = UndoData.ChannelColor;
		//                    break;

		//                case SpecificEventType.Channel_Name:
		//                    //RedoData.AffectedChannels = UndoData.AffectedChannels;
		//                    Index = UndoData.AffectedChannels[0];
		//                    //RedoData.ChannelName = UndoData.ChannelName;
		//                    _workshop.Channels[Index].Name = UndoData.ChannelName;
		//                    break;
		//            }

		//            break;

		//        #endregion [ Channel ]

		//        #region [ UI ]

		//        case GeneralDataEvent.UI:
		//            switch (UndoData.SpecificEvent)
		//            {
		//                case SpecificEventType.Zoom:
		//                    //RedoData.Zoom = _data.UI.Zoom;
		//                    _workshop.UI.Zoom = UndoData.Zoom;
		//                    break;

		//                case SpecificEventType.CellSize:
		//                    //RedoData.CellSize = _data.UI.CellSize;
		//                    _workshop.UI.CellSize = UndoData.CellSize;
		//                    break;

		//                case SpecificEventType.GridLineWidth:
		//                    //RedoData.GridLineWidth = _data.UI.GridLineWidth;
		//                    _workshop.UI.GridLineWidth = UndoData.GridLineWidth;
		//                    break;

		//                case SpecificEventType.LatticeSize:
		//                    //RedoData.CanvasSize = _data.UI.CanvasSize;
		//                    _workshop.UI.LatticeSize = UndoData.LatticeSize;
		//                    SetCanvasSize(UndoData.LatticeSize);
		//                    break;

		//                case SpecificEventType.Crop:
		//                    _workshop.UI.LatticeSize = UndoData.LatticeSize;
		//                    SetCanvasSize(UndoData.LatticeSize);
		//                    for (int i = 0; i < UndoData.AffectedChannels.Count; i++)
		//                    {
		//                        Index = UndoData.AffectedChannels[i];
		//                        if (Index < 0)
		//                            continue;
		//                        if (i == 0)
		//                            _workshop.ActiveChannelIndex = Index;

		//                        _workshop.Channels[Index].ClearLattice();
		//                        _workshop.Channels[Index].PaintCells(UndoData.Cells[i]);
		//                    }
		//                    break;

		//                case SpecificEventType.BackgroundImage_Clear:
		//                case SpecificEventType.BackgroundImage_Load:

		//                    //RedoData.BackgroundImage = _data.UI.BackgroundImage;
		//                    //RedoData.BackgroundImageFilename = _data.UI.BackgroundImageFilename;
		//                    //RedoData.Brightness = _data.UI.Brightness;

		//                    _workshop.UI.BackgroundImage = UndoData.BackgroundImage;
		//                    _workshop.UI.BackgroundImage_Filename = UndoData.BackgroundImageFilename;
		//                    _workshop.UI.BackgroundImage_Brightness = UndoData.Brightness;
		//                    SetBrightness(UndoData.Brightness);

		//                    break;

		//            }

		//            break;

		//        #endregion [ UI ]

		//        #region [ Mask ]

		//        case GeneralDataEvent.Mask:
		//            switch (UndoData.SpecificEvent)
		//            {
		//                case SpecificEventType.Mask_Defined:
		//                case SpecificEventType.Mask_Cleared:
		//                    _workshop.Mask.Set(UndoData.Mask, true);
		//                    MarqueeAnimationTimer.Enabled = _workshop.Mask.HasMask;

		//                    // Call the selected method on the current tool to get it to refresh it's toolbar
		//                    // Really only relevent if the current tool is a masking tool...
		//                    _workshop.CurrentTool.Tool.Selected(CanvasPane);
		//                    break;

		//                //case SpecificEventType.Mask_Moved:
		//                //RedoData.SetMask(_data.MaskOutline, _data.MaskRegion);
		//                //_workshop.SetMask(UndoData.MaskOutline, UndoData.MaskRegion);
		//                //RedoData.AffectedChannels = UndoData.AffectedChannels;
		//                //RedoData.Cells.Add(UndoData.Cells[0]);

		//                //_data.MoveChannel.ClearAllCells();
		//                //_data.MoveChannel.PaintCells(UndoData.Cells[0]);

		//                //_data.MoveChannel.Cells = UndoData.Cells[0];
		//                //ExposePane(_maskPane, Panes.Mask);
		//                //ExposePane(_workshop.MoveChannel, Panes.MoveChannel);

		//                //break;
		//            }

		//            break;

		//        #endregion [ Mask ]

		//    }

		//    //_data.RedoStack.Push(//RedoData);
		//    CanvasPane.Refresh();
		//}

		///// <summary>
		///// This is called when the Lattice size, CellSize, GridLineWidth, or Zoom is changed.
		///// </summary>
		//private void SetCanvasSize()
		//{
		//    SetCanvasSize(_workshop.UI.LatticeSize);
		//}

		///// <summary>
		///// This is called when the Lattice size, CellSize, GridLineWidth, or Zoom is changed.
		///// Size of the canvas is ALWAYS expressed in Cells, not pixels
		///// </summary>
		//private void SetCanvasSize(Size newLatticeSize)
		//{
		//    this.pnlWorkspace.SuspendLayout();
		//    this.pnlEasel.SuspendLayout();

		//    int Width = newLatticeSize.Width * UISettings.ʃCellScale + (int)(_workshop.UI.GridLineWidth * _workshop.UI.Zoom);
		//    int Height = newLatticeSize.Height * UISettings.ʃCellScale + (int)(_workshop.UI.GridLineWidth * _workshop.UI.Zoom);

		//    if ((Width <= 0) || (Height <= 0))
		//        return;

		//    CanvasPane.Size = new Size(Width, Height);
		//    UISettings.ʃCanvasSize = CanvasPane.Size;
		//    UISettings.ʃLatticeSize = newLatticeSize;

		//    SetCanvasPosition();
		//    _workshop.UI.Background.Set();
		//    CanvasPane.Refresh();

		//    this.pnlWorkspace.ResumeLayout(false);
		//    this.pnlEasel.ResumeLayout(false);

		//}
/*
		/// <summary>
		/// Centers the middle of the CanvasPane to the center of the screen.
		/// </summary>
		public void SetCanvasScroll()
		{
			SetCanvasScroll(new Point(CanvasPane.Width / 2, CanvasPane.Height / 2));
		}

		/// <summary>
		/// Centers a point on the CanvasPane to the center of the screen.
		/// </summary>
		/// <param name="canvasPoint"></param>
		public void SetCanvasScroll(Point canvasPoint)
		{
			_settingScroll = true;
			int Value = 0;

			Debug.WriteLine("Attention Point: " + canvasPoint.ToString());

			//if (pnlWorkspace.HorizontalScroll.Visible)
			//{
			Value = pnlWorkspace.HorizontalScroll.Maximum - pnlWorkspace.HorizontalScroll.LargeChange + 1;
			// Calculate the percent from left the point is on the Canvas Pane and multiply the scroll value by that percentage

			_hScrollPct = (float)canvasPoint.X / (float)_workshop.UI.CanvasSize.Width;

			Debug.WriteLine("H%: " + _hScrollPct);

			Value = (int)(Value * _hScrollPct);

			// Make sure we do not violate any boundaries
			if (Value > pnlWorkspace.HorizontalScroll.Maximum)
				Value = pnlWorkspace.HorizontalScroll.Maximum;
			else if (Value < pnlWorkspace.HorizontalScroll.Minimum)
				Value = pnlWorkspace.HorizontalScroll.Minimum;

			pnlWorkspace.HorizontalScroll.Value = Value;
			//}

			//if (pnlWorkspace.VerticalScroll.Visible)
			//{
			Value = pnlWorkspace.VerticalScroll.Maximum - pnlWorkspace.VerticalScroll.LargeChange + 1;
			// Calculate the percent from top the point is on the Canvas Pane and multiply the scroll value by that percentage

			_vScrollPct = (float)canvasPoint.Y / (float)_workshop.UI.CanvasSize.Height;

			Debug.WriteLine("V%: " + _vScrollPct);

			Value = (int)(Value * _vScrollPct);

			// Make sure we do not violate any boundaries
			if (Value > pnlWorkspace.VerticalScroll.Maximum)
				Value = pnlWorkspace.VerticalScroll.Maximum;
			else if (Value < pnlWorkspace.VerticalScroll.Minimum)
				Value = pnlWorkspace.VerticalScroll.Minimum;

			pnlWorkspace.VerticalScroll.Value = Value;
			//}

			pnlWorkspace.PerformLayout();

			//Debug.WriteLine("Max: " + pnlWorkspace.HorizontalScroll.Maximum);
			//Debug.WriteLine("Value: " + pnlWorkspace.HorizontalScroll.Value);
			//Debug.WriteLine("Large Change: " + pnlWorkspace.HorizontalScroll.LargeChange);
			_settingScroll = false;
		}
		*/
		#endregion [ DEAD CODE ]
	}
	
}

