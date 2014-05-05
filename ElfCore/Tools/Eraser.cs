using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	[ElfEditTool("Eraser")]
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
			base.ID = (int)ToolID.Erase;
			base.Name = "Eraser";
			base.ToolBoxImage = ElfRes.eraser;
			_nib = new Nib();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Attaches or detaches events to objects, such as Click events to buttons.
		/// </summary>
		/// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
		protected override void AttachEvents(bool attach)
		{
			// If we've already either attached or detached, exit out.
			if (attach && _eventsAttached)
				return;

			if (_nib != null)
				_nib.AttachEvents(attach);

			if (attach)
			{
				if (_nib != null)
					_nib.Changed += new EventHandler(this.Nib_Changed);
				if (Profile != null)
					Profile.ScalingChanged += new EventHandler(Profile_ScalingChanged);
			}
			else
			{
				if (_nib != null)
					_nib.Changed -= this.Nib_Changed;
				if (Profile != null)
					Profile.ScalingChanged -= Profile_ScalingChanged;
			}

			base.AttachEvents(attach);
		}

		/// <summary>
		/// Customized version of CapturedCanvas, checks to see if the control button is being held down initially, if so, then it captures
		/// just the background image (if any). Else it captures all but the active Channel(s)
		/// </summary>
		protected override void CaptureCanvas()
		{
			PictureBox Canvas = Profile.GetCanvas();
			if (Canvas == null)
				return;

			// Check to see if we are doing an erase through all Channels (control key is pressed)
			if (ControlKeyPressed())
			{
				// If there is no background image (or it isn't set for some reason) get a blank, black image
				if ((Profile.Background.Image == null) || (Canvas.BackgroundImage == null))
				{
					_capturedCanvas = new Bitmap(Scaling.CanvasSize.Width, Scaling.CanvasSize.Height);
					Graphics g = Graphics.FromImage(_capturedCanvas);
					g.Clear(Color.Black);
					g.Dispose();
					g = null;
				}
				else
					// Grab the background image for the erase all
					_capturedCanvas = new Bitmap(Canvas.BackgroundImage);
			}
			else
				// Get all the Channels except the active one.
				_capturedCanvas = _workshop.CaptureCanvas(true);

			#if DEBUG
				Workshop.ExposePane(_capturedCanvas, Panes.CapturedCanvas);
			#endif

		}

		/// <summary>
		/// Erases an area off the Channel at the given point, area defined by the Nib of the tool
		/// </summary>
		/// <param name="Channel">Channel to erase</param>
		/// <param name="pt">Position to erase at, values are in Pixels</param>
		private void Erase(BaseChannel Channel, Point pt)
		{
			_nib.OffsetRects(pt);

			if ((_nib.Shape == Nib.NibShape.Square) || (_nib.Size < 3))
			{
				// Paint black on the paint pane to erase lit cells
				_latticeBufferGraphics.FillRectangle(Brushes.Black, _nib.Rect_Lattice);

				// Draw a snippet of the background over the current Channel
				_canvasControlGraphics.DrawImage(_capturedCanvas, _nib.Rect_Canvas, _nib.Rect_Canvas, GraphicsUnit.Pixel);
			}
			else
			{
				// Draw on the paint pane
				_latticeBufferGraphics.FillEllipse(Brushes.Black, _nib.Rect_Lattice);

				// Draw a snippet of the background over the current Channel
				GraphicsPath Path = new GraphicsPath();
				Path.AddEllipse(_nib.Rect_Canvas);
				_canvasControlGraphics.SetClip(new Region(Path), CombineMode.Replace);
				_canvasControlGraphics.DrawImage(_capturedCanvas, _nib.Rect_Canvas, _nib.Rect_Canvas, GraphicsUnit.Pixel);
				_canvasControlGraphics.ResetClip();
				Path.Dispose();
			}

			#if DEBUG
				Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			#endif

		}
		
		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			_nib.NibSize_Control = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			_nib.SquareNib_Control = (ToolStripButton)GetItem<ToolStripButton>(1);
			_nib.RoundNib_Control = (ToolStripButton)GetItem<ToolStripButton>(2);

			// Load the Settings values
			_nib.LoadSettings(_savePath);			
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			base.MouseDown(buttons, latticePoint, actualCanvasPoint);

			_latticeBuffer = new Bitmap(Profile.Channels.Active.LatticeBuffer);
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
			
			// Create this bitmap as a transparent one, so that we can use the black mark on clear to apply to all Channels (if needed)
			_latticeBufferGraphics.Clear(Color.Transparent);

			SetMaskClip();
			
			_nib.AdjustForCellSize();
			Erase(Profile.Channels.Active, _currentLatticePoint);

			#if DEBUG
				Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			#endif
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (Profile == null)
				return false;

			if (!_isMouseDown)
				return false;

			_currentLatticePoint = latticePoint;
			Erase(Profile.Channels.Active, latticePoint);

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			_canvasControlGraphics.ResetClip();

			if (ControlKeyPressed())
			{
				foreach (BaseChannel Ch in Profile.Channels.Sorted)
					Profile.Channels.PopulateChannelFromBitmap_Erase(_latticeBuffer, Ch);
			}
			else
				Profile.Channels.PopulateChannelFromBitmap_Erase(_latticeBuffer, Profile.Channels.Active);

			Profile.Refresh();

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

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();
			_nib = null;
		}
	
		#endregion [ Methods ]

		#region [ Event Delegates ]

		/// <summary>
		/// Occurs when one of the scaling variables within the Profile changes. 
		/// </summary>
		private void Profile_ScalingChanged(object sender, EventArgs e)
		{
			Profile.Cursor = _nib.Cursor;
		}

		/// <summary>
		/// Occurs when one of the properties on the Nib has changed and the cursor needs to be recreated.
		/// </summary>
		protected void Nib_Changed(object sender, EventArgs e)
		{
		    Profile.Cursor = _nib.Cursor;
		}

		#endregion [ Event Delegates ]

	}
}

