using System;
using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Icicles")]
	public class IcicleTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		private Point _endPoint_1 = Point.Empty;
		private Point _endPoint_2 = Point.Empty;
		private Point _endPoint_3 = Point.Empty;

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
			this.ID = (int)Tool.Icicles;
			this.Name = "Icicles";
			this.ToolBoxImage = ElfRes.icicles;
			//this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_icicles);
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.icicles_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void DrawIcicles(Graphics g, Pen drawPen, Point p1, Point p2, Point p3)
		{
			//Preview.WriteTraceMessage("P1: " + p1.ToString(), System.Diagnostics.TraceLevel.Info);
			//Preview.WriteTraceMessage("P2: " + p2.ToString(), System.Diagnostics.TraceLevel.Info);
			//Preview.WriteTraceMessage("P3: " + p3.ToString(), System.Diagnostics.TraceLevel.Info);

			if (p1.Equals(p2))
				return;

			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
			g.DrawLine(drawPen, p1, p2);

			bool OddStrand = true;
			float DeltaY = 0f;
			float Height = 0f;
			int Counter = 0;

			if ((p3.Y != p2.Y) && (DrawArea.Width > 0))
			{
				float Gap = Math.Abs((float)(p2.X - p1.X)) / (float)_numberOfStrands;
				float X = DrawArea.Left + Gap / 2f;
				float Y;

				// equation of a line: y = mx + b
				// m = y2-y1/x2-x1 = slope
				// b = y - mx for any point on that line
				float m = (float)(p2.Y - p1.Y) / (float)(p2.X - p1.X);
				float b = (float)p1.Y - (m * p1.X);

				// Height is the distance of the 3rd point from the line in the Y direction
				Y = (int)(m * p3.X + b);
				Height = Math.Abs(p3.Y - Y);

				while (X < DrawArea.Right)
				{
					OddStrand = !OddStrand;
					if (OddStrand)
						DeltaY = Height / 2;
					else
						DeltaY = Height;

					// Find the position on the line that corresponds to this X value.
					Y = (int)(m * X + b) + 1;

					try
					{
						g.DrawLine(drawPen, new PointF(X, Y), new PointF(X, Y + DeltaY));
					}
					catch (OutOfMemoryException)
					{ }

					X += Gap;
					// emergency catch
					if (Counter++ > _numberOfStrands)
						break;
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

			// Load the Settings values
			_numberOfStrands = LoadValue(NUM_STRANDS, 10);

			// Get a pointer to the controls on the toolstrip that belongs to us.
			txtNumStrands = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);

			// Attach events to these controls
			txtNumStrands.Leave += new System.EventHandler(this.txtNumStrands_Leave);

			// Set the initial value for the contol from what we had retrieve from Settings
			txtNumStrands.Text = _numberOfStrands.ToString();
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;

			if (!_secondPart)
			{
				_canvasControlGraphics = Workshop.Canvas.CreateGraphics();

				// Trap the mouse into the Canvas while we are working
				//TrapMouse();

				_latticeBuffer = _workshop.Channels.Active.LatticeBuffer;
				_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);

				// Grab a snapshot of the canvas as is
				CaptureCanvas();

				// Create Undo data to be able to reverse out our changes
				//_workshop.CreateUndo_Channel(this.UndoText);

				_endPoint_1 = mouseCell;
			}
			else
			{
				_endPoint_3 = mouseCell;
				_secondPart = true;
			}
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown && !_secondPart)
				return false;

			Point EP1 = Workshop.PixelPoint(_endPoint_1);
			Point EP2 = Point.Empty;
			Point EP3 = Point.Empty;

			// Draw the captured bitmap onto the CanvasPane PictureBox
			DisplayCapturedCanvas();

			if (!_secondPart)
			{
				// Convert this point from cells back to pixels since we will be doing live drawing in this event
				EP2 = _workshop.ConstrainLine(Workshop.PixelPoint(mouseCell), EP1);

				using (Pen MarqueePen = _workshop.GetMarqueePen())
				{
					try
					{
						_canvasControlGraphics.DrawLine(MarqueePen, EP1, EP2);
					}
					catch (OutOfMemoryException)
					{ }
				}
			}
			else
			{
				EP2 = Workshop.PixelPoint(_endPoint_2);
				EP3 = Workshop.PixelPoint(mouseCell);

				using (Pen MarqueePen = _workshop.GetMarqueePen())
				{
					try
					{
						DrawIcicles(_canvasControlGraphics, MarqueePen, EP1, EP2, EP3);
					}
					catch (OutOfMemoryException)
					{ }
				}
			}
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="workshop">Workshop object that contains Channel information</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;

			_mouseDown = false;

			// Release the capture of the mouse cursor
			if (!_secondPart)
			{
				//ReleaseMouse();

				_endPoint_2 = _workshop.ConstrainLine(mouseCell, _endPoint_1);

				//_endPoint_2 = mouseCell;
				_secondPart = true;
			}
			else
			{
				_endPoint_3 = mouseCell;
				Workshop.WaitCursor();

				using(Pen DrawPen = new Pen(Color.White))
				{
					DrawIcicles(_latticeBufferGraphics, DrawPen, _endPoint_1, _endPoint_2, _endPoint_3);
				}

				_workshop.Channels.Active.ClearLattice();
				_workshop.Channels.Active.LatticeBuffer = _latticeBuffer;

				Workshop.Canvas.Refresh();
				PostDrawCleanUp();
				Workshop.EndWaitCursor();
				_secondPart = false;
			}

			return true;
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

		#region [ ToolStrip Events ]

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

		#endregion [ ToolStrip Events ]
			
	}
}
