using System;
using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Paintbrush")]
	public class PaintbrushTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		private Nib _nib;

		// Used for rendering
		private Bitmap _gridwork = null;
		private Brush _paintBrush = null;
		private Rectangle _bounds = Rectangle.Empty;

		// Masking variables
		private Region _maskRegion = null;
		private bool _hasMask = false;

	
		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Custom Cursor, created based on the nib size and shape
		/// </summary>
		public override Cursor Cursor
		{
			get { return _nib.Cursor; }
		}
	
		#endregion [ Properties ]

		#region [ Constructors ]

		public PaintbrushTool()
		{
			this.ID = (int)Tool.Paint;
			this.Name = "Paintbrush";
			this.ToolBoxImage = ElfRes.paint_tool;
			_nib = new Nib();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create a grid of black lines over a transparent background, to simulate the grid lines
		/// </summary>
		private void CreatePaintGrid()
		{
			// If we already have a bitmap defined, clear it out.
			if (_gridwork != null)
			{
				_gridwork.Dispose();
				_gridwork = null;
			}
			Size Size = Workshop.Canvas.Size;

			_gridwork = new Bitmap(Size.Width, Size.Height, Workshop.Canvas.CreateGraphics());
			using (Graphics g = Graphics.FromImage(_gridwork))
			{
				g.Clear(Color.Transparent);
				Rectangle GridCell = new Rectangle(0, 0, UISettings.ʃCellScale, UISettings.ʃCellScale);
				for (int x = UISettings.ʃCellZoom; x < Size.Width; x += UISettings.ʃCellScale)
				{
					g.DrawLine(Pens.Black, new Point(x, 0), new Point(x, Size.Height));
				}
				for (int y = UISettings.ʃCellZoom; y < Size.Height; y += UISettings.ʃCellScale)
				{
					g.DrawLine(Pens.Black, new Point(0, y), new Point(Size.Width, y));
				}
			}
		}

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		/// <param name="settings">Settings object, handles getting and saving settings data</param>
		/// <param name="workshop">Workshop object, contains lots of useful methods and ways to hold data.</param>
		public override void Initialize()
		{
			base.Initialize();

			// Get a pointer to the controls on the toolstrip that belongs to us.
			_nib.NibSize_Control = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			_nib.SquareNib_Control = (ToolStripButton)GetItem<ToolStripButton>(1);
			_nib.RoundNib_Control = (ToolStripButton)GetItem<ToolStripButton>(2);

			// Load the Settings values
			_nib.LoadSettings(_savePath);

			_nib.AttachControlEvents();
		}

		/// <summary>
		/// Checks to see if there is a mask in place. If so, is this point visible therein?
		/// </summary>
		/// <param name="pt">Point to check</param>
		private bool IsValidMaskPoint(Point pt)
		{
			if (!_hasMask)
				return true;
			else
				return _maskRegion.IsVisible(pt);
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			base.MouseDown(buttons, mouseCell, mousePixel);

			_constrainingDirection = ConstrainDirection.NotSet;
			_paintBrush = new SolidBrush(_workshop.Channels.Active.Color);
			_latticeBuffer = (Bitmap)_workshop.Channels.Active.LatticeBuffer.Clone();
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);

			SetMaskClip();

			_hasMask = _workshop.Mask.HasMask;
			if (_hasMask)
				_maskRegion = new Region(_workshop.Mask.LatticeMask.Outline);
	
			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			_nib.AdjustForCellSize();
			Paint(_workshop.Channels.Active, mouseCell);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;

			_currentMouseCell = mouseCell;
			Point ConstrainedCell = _workshop.ConstrainPaint(_currentMouseCell, mousePixel, ref _constrainingDirection);

			Paint(_workshop.Channels.Active, _currentMouseCell); //ConstrainedCell);
						
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseUp(buttons, mouseCell, mousePixel))
				return false;

			_canvasControlGraphics.ResetClip();
			_workshop.Channels.PopulateChannelFromBitmap(_latticeBuffer, _workshop.Channels.Active);
			Workshop.Canvas.Refresh();
			PostDrawCleanUp();
			return true;
		}

		/// <summary>
		/// Paints the Channel color on the Canvas, and also hidden on the Paint Pane.
		/// </summary>
		/// <param name="Channel">Channel that gets painted upon</param>
		/// <param name="pt">Position the nib shape/sized area is centered upon, in cells</param>
		private void Paint(Channel Channel, Point pt)
		{
			_nib.OffsetRects(pt);
			
			if (_nib.Shape == Nib.NibShape.Square)
			{
				_latticeBufferGraphics.FillRectangle(Brushes.White, _nib.Rect_Lattice);
				_canvasControlGraphics.FillRectangle(_paintBrush, _nib.Rect_Canvas);
			}
			else
			{
				if (_nib.Size < 3)
				{
					_latticeBufferGraphics.FillRectangle(Brushes.White, _nib.Rect_Lattice);
					_canvasControlGraphics.FillRectangle(_paintBrush, _nib.Rect_Canvas);
				}
				else
				{
					_latticeBufferGraphics.FillEllipse(Brushes.White, _nib.Rect_Lattice);
					_canvasControlGraphics.FillEllipse(_paintBrush, _nib.Rect_Canvas);
				}
			}

			// paint the grid over our painted area
			if ((_workshop.UI.ShowGridLineWhilePainting) && (_workshop.UI.GridLineWidth > 0))
				_canvasControlGraphics.DrawImage(_gridwork, _nib.Rect_Canvas, _nib.Rect_Canvas, GraphicsUnit.Pixel);
			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();

			if (_paintBrush != null)
			{
				_paintBrush.Dispose();
				_paintBrush = null;
			}
			if (_maskRegion != null)
			{
				_maskRegion.Dispose();
				_maskRegion = null;
			}
		}
		
		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		/// <param name="settings"></param>
		public override void SaveSettings()
		{
			_nib.SaveSettings(_settings, _savePath);
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox, or when any Canvas UI values change (canvas resize, cell size change, zoom change, grid line width)
		/// </summary>
		public override void Selected()
		{
			base.Selected();
			CreatePaintGrid();
		
			// Attach to the UI events so that the cursor will correctly resize
			_workshop.UI.CellSizeChanged += new System.EventHandler(UI_Changed);
			_workshop.UI.DisplayGridLines += new System.EventHandler(UI_Changed);
			_workshop.UI.Zooming += new System.EventHandler(UI_Changed);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();
			_nib = null;
		}

		public override void Unselected()
		{
			base.Unselected();

			// Detach from the UI events, we don't want to to fire if this is not the selected Tool
			_workshop.UI.CellSizeChanged -= new System.EventHandler(UI_Changed);
			_workshop.UI.DisplayGridLines -= new System.EventHandler(UI_Changed);
			_workshop.UI.Zooming -= new System.EventHandler(UI_Changed);
		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Occurs when one of the critical properties in the UI changes: CellSize, GridLineWidth, Zoom
		/// </summary>
		private void UI_Changed(object sender, System.EventArgs e)
		{
			Workshop.Canvas.Cursor = _nib.Cursor;
			CreatePaintGrid();
		}

		#endregion [ Events ]
	}
}


