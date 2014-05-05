using ElfCore.Controllers;
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
	[ElfEditTool("Icicles")]
	public class IcicleTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		private LatticePoint _endPoint_1 = Point.Empty;
		private LatticePoint _endPoint_2 = Point.Empty;
		private LatticePoint _endPoint_3 = Point.Empty;

		// Number of dangling strands
		private int _numberOfStrands = 10;
		private bool _secondPart = false;

		// Controls from ToolStrip
		private ToolStripTextBox txtNumStrands = null;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string NUM_STRANDS = "NumberOfStrands";
		private const string DEFAULT_NUM_STRANDS = "20";

		#endregion [ Constants ]

		#region [ Constructors ]

		public IcicleTool()
		{
			this.ID = (int)ToolID.Icicles;
			this.Name = "Icicles";
			this.ToolBoxImage = ElfRes.icicles;
			this.ToolGroupName = Constants.TOOLGROUP_SHAPE;
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.icicles_modifier, new Point(15, 15));
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

			if (attach)
			{
				txtNumStrands.Leave += new EventHandler(txtNumStrands_Leave);
			}
			else
			{
				txtNumStrands.Leave -= txtNumStrands_Leave;
			}

			base.AttachEvents(attach);
		}

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="p2">Icicle drop point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		private GraphicsPath CreateRenderPath(Point p1, Point p2, Point p3, bool finalRender)
		{
			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
			GraphicsPath Path = new GraphicsPath();

			bool OddStrand = true;
			float DeltaY = 0f;
			float VGap = 0f;
			int Counter = 0;

			Path.AddLine(p1, p2);
			Path.CloseFigure();

			//if (!p3.IsEmpty)
				//Debugger.Break();

			if (p3.IsEmpty || (DrawArea.Width == 0))
				return Path;

			

			//if ((p3.Y != p2.Y) && (DrawArea.Width > 0))
			//{
			float HGap = Math.Abs((float)(p2.X - p1.X)) / (float)_numberOfStrands;
			float X = DrawArea.Left + HGap / 2f;
			float Y;
			PointF LinePoint;
			PointF DropPoint;

			// equation of a line: y = mx + b
			// m = y2-y1/x2-x1 = slope
			// b = y - mx for any point on that line
			float m = (float)(p2.Y - p1.Y) / (float)(p2.X - p1.X);
			float b = (float)p1.Y - (m * p1.X);

			// Calculate the distance from the 3rd point to the line by casting a ray vertically from the point to where it intercepts the line.
			// Determine the Y position of a point on the line where X = p3.X
			// y = b + mx
			float Y3 = b + m * p3.X;
			VGap = Math.Abs(p3.Y - Y3);

			// Height is the distance of the 3rd point from the line in the Y direction
			//Y = (int)(m * p3.X + b);
			//Height = Math.Abs(p3.Y - Y);

			while (X < DrawArea.Right)
			{
				OddStrand = !OddStrand;
				if (OddStrand)
					DeltaY = VGap / 2;
				else
					DeltaY = VGap;

				if (DeltaY < 1f)
				{
					X += HGap;
					continue;
				}

				// Find the position on the line that corresponds to this X value.
				Y = (int)(m * X + b);//				+1;

				// Now normalize the values of X and Y so that they are on center of a cell, if we are only doing live rendering, not final
				if (!finalRender)
				{
					LinePoint = new PointF(X, Y);
					LinePoint = _workshop.CalcLatticePointF(LinePoint);
					LinePoint = _workshop.CalcCanvasPointF_OC(LinePoint);
					// Reposition the point on the line now
					LinePoint = new PointF(LinePoint.X, m * LinePoint.X + b);

					DropPoint = new PointF(X, Y + DeltaY);
					DropPoint = _workshop.CalcLatticePointF(DropPoint);
					DropPoint = _workshop.CalcCanvasPointF_OC(DropPoint);

					Path.AddLine(LinePoint, DropPoint);
				}
				else
				{
					Path.AddLine(new PointF(X, Y+1), new PointF(X, Y + DeltaY));
				}
					Path.CloseFigure();

				X += HGap;
				// emergency catch
				if (Counter++ > _numberOfStrands)
					break;
			}
			//}
			return Path;
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

			// Load the Settings values
			_numberOfStrands = LoadValue(NUM_STRANDS, 10);

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			txtNumStrands = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);

			// Set the initial value for the contol from what we had retrieve from Settings
			txtNumStrands.Text = _numberOfStrands.ToString();
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			_isMouseDown = true;

			_mouseDownLatticePoint = latticePoint;
			_mouseDownCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);

			if (!_secondPart)
			{
				_canvasControlGraphics = Profile.GetCanvasGraphics();

				_latticeBuffer = Profile.Channels.Active.LatticeBuffer;
				_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);

				// Grab a snapshot of the canvas as is
				CaptureCanvas();

				_endPoint_1 = latticePoint;
			}
			else
			{
				_endPoint_3 = latticePoint;
				_secondPart = true;
			}
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!_isMouseDown && !_secondPart)
				return false;

			Point EP1 = _workshop.CalcCanvasPoint_OC(_endPoint_1);
			Point EP2 = Point.Empty;
			Point EP3 = Point.Empty;

			// Draw the captured bitmap onto the CanvasPane PictureBox
			DisplayCapturedCanvas();

			if (!_secondPart)
			{
				// Convert this point from cells back to pixels since we will be doing live drawing in this event
				EP2 = _workshop.ConstrainLine(_workshop.CalcCanvasPoint_OC(latticePoint), EP1);
				Render(EP1, EP2, Point.Empty, false);
			}
			else
			{
				EP2 = _workshop.CalcCanvasPoint_OC(_endPoint_2);
				EP3 = _workshop.CalcCanvasPoint(latticePoint);
				Render(EP1, EP2, EP3, false);
			}
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
			if (!_isMouseDown)
				return false;

			_isMouseDown = false;

			// If we've accidentally clicked instead of dragged out the start line, just abort.
			if (!_secondPart && (latticePoint == _mouseDownLatticePoint))
			{
				PostDrawCleanUp();
				return false;
			}

			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);

			if (!_secondPart)
			{
				_endPoint_2 = _workshop.ConstrainLine(latticePoint, _endPoint_1);
				_secondPart = true;
			}
			else
			{
				_endPoint_3 = latticePoint;
				Workshop.WaitCursor(Profile);

				Render(_endPoint_1, _endPoint_2, _endPoint_3, true);

				#if DEBUG
					Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
				#endif

				Profile.Channels.Active.Empty();
				Profile.Channels.Active.LatticeBuffer = _latticeBuffer;
				Profile.Refresh();

				PostDrawCleanUp();
				Workshop.EndWaitCursor(Profile);
				_secondPart = false;
			}

			return true;
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();
			_endPoint_1 = Point.Empty;
			_endPoint_2 = Point.Empty;
			_endPoint_3 = Point.Empty;
		}

		/// <summary>
		/// Draw the Icicles
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="p3"></param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		private void Render(Point p1, Point p2, Point p3, bool finalRender)
		{
			//Debug.Write("\tP1: " + p1.ToString());
			//Debug.Write("\tP2: " + p2.ToString());
			//Debug.WriteLine("\tP3: " + p3.ToString());

			//if (finalRender)
			//{
				//p1 = _workshop.CalcLatticePoint(p1);
				//p2 = _workshop.CalcLatticePoint(p2);

				//Debug.Write("\tP1: " + p1.ToString());
				//Debug.Write("\tP2: " + p2.ToString());
				//Debug.WriteLine("\tP3: " + p3.ToString());
			//}

			GraphicsPath DrawPath = CreateRenderPath(p1, p2, p3, finalRender);

			using (Pen DrawPen = finalRender ? RenderPen() : _workshop.GetMarqueePen())
			{
				try
				{
					(finalRender ? _latticeBufferGraphics : _canvasControlGraphics).DrawPath(DrawPen, DrawPath);
				}
				catch (OutOfMemoryException)
				{ }
			}

			DrawPath.Dispose();
			DrawPath = null;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			_settings.SetValue(_savePath + Constants.SAVE_PATH_DELIMITER + NUM_STRANDS, _numberOfStrands);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			txtNumStrands = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void txtNumStrands_Leave(object sender, EventArgs e)
		{
			if (txtNumStrands.TextLength == 0)
				txtNumStrands.Text = DEFAULT_NUM_STRANDS;

			_numberOfStrands = ValidateInteger((ToolStripTextBox)sender, _numberOfStrands);
		}

		#endregion [ ToolStrip Event Delegates ]
			
	}
}
