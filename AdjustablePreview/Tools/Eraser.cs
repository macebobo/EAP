using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Eraser")]
	public class EraserTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		private Nib _nib;

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

		public EraserTool()
		{
			this.ID = (int)Tool.Erase;
			this.Name = "Eraser";
			this.ToolBoxImage = ElfRes.eraser;
			_nib = new Nib();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Customized version of CapturedCanvas, checks to see if the control button is being held down initially, if so, then it captures
		/// just the background image (if any). Else it captures all but the active Channel(s)
		/// </summary>
		protected override void CaptureCanvas()
		{
			// Check to see if we are doing an erase through all Channels (control key is pressed)
			if (ControlKeyPressed())
			{
				// If there is no background image (or it isn't set for some reason) get a blank, black image
				if ((_workshop.UI.Background.Image == null) || (Workshop.Canvas.BackgroundImage == null))
				{
					_capturedCanvas = new Bitmap(Workshop.Canvas.Width, Workshop.Canvas.Height);
					Graphics g = Graphics.FromImage(_capturedCanvas);
					g.Clear(Color.Black);
					g.Dispose();
					g = null;
				}
				else
					// Grab the background image for the erase all
					_capturedCanvas = new Bitmap(Workshop.Canvas.BackgroundImage);
			}
			else
				// Get all the Channels except the active one.
				_capturedCanvas = _workshop.CaptureCanvas(true);

			Editor.ExposePane(_capturedCanvas, Panes.Canvas);
		}

		/// <summary>
		/// Erases an area off the Channel at the given point, area defined by the Nib of the tool
		/// </summary>
		/// <param name="Channel">Channel to erase</param>
		/// <param name="pt">Position to erase at, values are in Pixels</param>
		private void Erase(Channel Channel, Point pt)
		{
			_nib.OffsetRects(pt);

			if (_nib.Shape == Nib.NibShape.Square) 
			{
				// Paint black on the paint pane to erase lit cells
				_latticeBufferGraphics.FillRectangle(Brushes.Black, _nib.Rect_Lattice);

				// Draw a snippet of the background over the current Channel
				_canvasControlGraphics.DrawImage(_capturedCanvas, _nib.Rect_Canvas, _nib.Rect_Canvas, GraphicsUnit.Pixel);
			}
			else
			{
				// Draw on the paint pane
				_latticeBufferGraphics.FillEllipse(Brushes.Black, _nib.Rect_Canvas);

				// Draw a snippet of the background over the current Channel
				GraphicsPath Path = new GraphicsPath();
				Path.AddEllipse(_nib.Rect_Lattice);
				_canvasControlGraphics.SetClip(new Region(Path), CombineMode.Replace);
				_canvasControlGraphics.DrawImage(_capturedCanvas, _nib.Rect_Canvas, _nib.Rect_Canvas, GraphicsUnit.Pixel);
				_canvasControlGraphics.ResetClip();
				Path.Dispose();
			}

			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
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
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			base.MouseDown(buttons, mouseCell, mousePixel);

			_latticeBuffer = new Bitmap(_workshop.Channels.Active.LatticeBuffer);
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
			
			// Create this bitmap as a transparent one, so that we can use the black mark on clear to apply to all Channels (if needed)
			_latticeBufferGraphics.Clear(Color.Transparent);

			SetMaskClip();
			
			_nib.AdjustForCellSize();
			Erase(_workshop.Channels.Active, _currentMouseCell);
			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
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

			Erase(_workshop.Channels.Active, mouseCell);
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

			if (ControlKeyPressed())
			{
				foreach (Channel Ch in _workshop.Channels.Sorted)
					_workshop.Channels.PopulateChannelFromBitmap_Erase(_latticeBuffer, Ch);
			}
			else
				_workshop.Channels.PopulateChannelFromBitmap_Erase(_latticeBuffer, _workshop.Channels.Active);

			Workshop.Canvas.Refresh();

			// Object cleanup
			PostDrawCleanUp();
			return true;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			_nib.SaveSettings(_settings, _savePath);
		}

		public override void Selected()
		{
			base.Selected();

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
		}

		#endregion [ Events ]

	}
}

