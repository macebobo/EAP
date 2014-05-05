using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Polygon")]
	public class PolygonTool : ShapeBase, ITool
	{
		#region [ Constants ]

		private const string DEFAULT_NUMPOINTS = "5";
		private const string DEFAULT_STARTANGLE = "0" + Constants.DEGREE_SIGN;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private bool _isStar = false;
		private int _numPoints = 0;
		
		/// <summary>
		/// Starting Angle of the polygon (in degrees)
		/// </summary>
		private float _startAngle = float.MaxValue;

		// Controls from ToolStrip
		private ToolStripButton BtnDrawAsStar = null;
		private ToolStripTextBox NumPoints = null;
		private ToolStripTextBox StartAngle = null;
		private ToolStripLabel Warning_NumSides = null;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string NUMBER_OF_POINTS = "NumPoints";
		private const string IS_STAR = "IsStar";

		#endregion [ Constants ]

		#region [ Constructors ]

		public PolygonTool() : base()
		{
			this.ID = (int)Tool.Polygon;
			this.Name = "Polygon";
			this.ToolBoxImage = ElfRes.polygon;
			//this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_polygon);
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.polygon_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender)
		{
			Rectangle DrawArea;
			GraphicsPath Path = new GraphicsPath();

			DrawArea = _workshop.NormalizedRectangle(p1, p2);

			if (_numPoints == 4)
			{
				if ((_startAngle % 90) == 0)
				{
					Path.AddRectangle(DrawArea);
					return Path;
				}
			}
			
			// Reverse the sign on the start angle to get it to rotate anti-clockwise
			float StartAngle = -1 * this._startAngle;

			float PI = (float)Math.PI;
			float r = Math.Min(DrawArea.Width, DrawArea.Height) / 2;
			PointF[] Verts = new PointF[_numPoints];
			Point Origin = new Point(DrawArea.X + DrawArea.Width / 2, DrawArea.Y + DrawArea.Height / 2);
			float CurrentAngle = 0;

			// Internal angle is n-2 x PI / n (in radians)
			float InternalAngle = (float)(((_numPoints - 2) * PI) / _numPoints);

			// Start angle is 1/2 the internal angle of the poly
			float StartingAngle = InternalAngle / 2;
			if (StartAngle != 0)
				//StartingAngle += PI * _startAngle / 180;
				StartingAngle += _workshop.DegreeToRadian(StartAngle);

			for (int i = 0; i < _numPoints; i++)
			{
				// i/n is the fraction of the circle we are going around
				// 2PI is the circle (in radian)
				// Starting Angle
				CurrentAngle = (float)(PI * 2 * ((float)i / (float)_numPoints));
				CurrentAngle += StartingAngle;
				Verts[i] = _workshop.PointFromEllipse(DrawArea, _workshop.RadianToDegree(CurrentAngle));
			}

			if (_isStar && (_numPoints > 4))
			{
				int NewSpot = 0;

				// Rearrange the verts so that the lines cross through.
				// Even numbered Polygons are actually 2 sets of lines (think Star of David)
				if (_numPoints % 2 == 0)
				{
					PointF[] EvenVerts = new PointF[_numPoints / 2];
					PointF[] OddVerts = new PointF[_numPoints / 2];
					for (int i = 0; i < _numPoints / 2; i++)
					{
						NewSpot = i * 2;
						if (NewSpot > _numPoints)
							break;
						EvenVerts[i] = Verts[NewSpot];
					}
					for (int i = 0; i < _numPoints / 2; i++)
					{
						NewSpot = (i * 2) + 1;
						if (NewSpot > _numPoints)
							break;
						OddVerts[i] = Verts[NewSpot];
					}
					Path.AddLines(EvenVerts);
					Path.CloseFigure();
					Path.AddLines(OddVerts);
					Path.CloseFigure();
				}
				else
				{
					PointF[] StarVerts = new PointF[_numPoints];
					for (int i = 0; i < _numPoints; i++)
					{
						NewSpot = i * 2;
						if (NewSpot > _numPoints)
							NewSpot -= _numPoints;
						StarVerts[i] = Verts[NewSpot];
					}
					Path.AddLines(StarVerts);
					Path.CloseFigure();
				}
			}
			else
			{
				Path.AddLines(Verts);
				Path.CloseFigure();
			}
			return Path;
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
			_isStar = LoadValue(IS_STAR, false);
			_numPoints = LoadValue(NUMBER_OF_POINTS, 5);
			_startAngle = LoadValue(Constants.START_ANGLE, 0.0f);
			
			// Get a pointer to the controls on the toolstrip that belongs to us.
			BtnDrawAsStar = (ToolStripButton)GetItem<ToolStripButton>(3);
			NumPoints = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			StartAngle = (ToolStripTextBox)GetItem<ToolStripTextBox>(2);
			Warning_NumSides = (ToolStripLabel)GetItem<ToolStripLabel>(5);

			// Attach all events that would normally go within the form to methods in this class
			BtnDrawAsStar.Click += new System.EventHandler(this.IsStar_Click);
			NumPoints.Leave += new System.EventHandler(this.NumPoints_Leave);
			StartAngle.Leave += new System.EventHandler(this.StartAngle_Leave);

			// Assign the DashStyle enum value to the tag of each dash menu item
			SetDashStyleDropDownButton(DashStyleDD);

			// Set the initial value for the contol from what we had retrieve from Settings
			BtnDrawAsStar.Enabled = (_numPoints > 4);
			if (BtnDrawAsStar.Enabled)
				BtnDrawAsStar.Checked = _isStar;

			NumPoints.Text = _numPoints.ToString();
			StartAngle.Text = _startAngle.ToString("0.0") + Constants.DEGREE_SIGN;
			Warning_NumSides.Visible = false;
		}

		/// <summary>
		/// Canvas MouseDown event was fired. This version differs from the one in BaseTool in that the pushing of the Undo
		/// data happens in MouseUp instead of MouseDown
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (_numPoints < 3)
				return;

			base.MouseDown(buttons, mouseCell, mousePixel);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (_numPoints < 3)
				return false;

			return base.MouseMove(buttons, mouseCell, mousePixel);
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (_numPoints < 3)
				return false;

			return base.MouseUp(buttons, mouseCell, mousePixel);
		}

		/// <summary>
		/// Draw the shape
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected override void Render(Point p1, Point p2, bool finalRender)
		{
			if (finalRender)
			{
				p1 = Workshop.CellPoint(p1);
				p2 = Workshop.CellPoint(p2);
			}

			GraphicsPath DrawPath = CreateRenderPath(p1, p2, finalRender);

			using (Pen DrawPen = finalRender ? RenderPen() : _workshop.GetMarqueePen())
			{
				try
				{
					(finalRender ? _latticeBufferGraphics : _canvasControlGraphics).DrawPath(DrawPen, DrawPath);
				}
				catch (OutOfMemoryException)
				{ }
			}

			if (_fill && finalRender)
			{
				if (!_isStar)
					_latticeBufferGraphics.FillPath(Brushes.White, DrawPath);
				else
				{
					if (_numPoints % 2 == 1)
						_latticeBufferGraphics.FillClosedCurve(Brushes.White, DrawPath.PathPoints, FillMode.Winding, 0.0f);
					else
					{
						// Treat as 2 curves, the points are broken out with the second following the first, both
						// with the same number of points each
						PointF[] Verts = new PointF[_numPoints / 2];

						for (int i = 0; i < _numPoints / 2; i++)
							Verts[i] = DrawPath.PathPoints[i];
						_latticeBufferGraphics.FillPolygon(Brushes.White, Verts);

						for (int i = 0; i < _numPoints / 2; i++)
							Verts[i] = DrawPath.PathPoints[i + _numPoints / 2];
						_latticeBufferGraphics.FillPolygon(Brushes.White, Verts);
					}
				}
			}


			DrawPath.Dispose();
			DrawPath = null;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			base.SaveSettings();
			SaveValue(IS_STAR, _isStar);
			SaveValue(NUMBER_OF_POINTS, _numPoints);
			SaveValue(Constants.START_ANGLE, _startAngle);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			BtnDrawAsStar = null;
			NumPoints = null;
			StartAngle = null;
			Warning_NumSides = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		private void IsStar_Click(object sender, EventArgs e)
		{
			_isStar = BtnDrawAsStar.Enabled && BtnDrawAsStar.Checked;
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void NumPoints_Leave(object sender, EventArgs e)
		{
			// Blanking out the textbox sets the value to 5
			if (NumPoints.TextLength == 0)
				NumPoints.Text = DEFAULT_NUMPOINTS;

			_numPoints = ValidateInteger(NumPoints, _numPoints);

			if (_numPoints < 3)
			{
				Warning_NumSides.Visible = true;
				NumPoints.SelectAll();
				NumPoints.Focus();
			}
			else
				Warning_NumSides.Visible = false;

			BtnDrawAsStar.Enabled = (_numPoints > 4);
			if (!BtnDrawAsStar.Enabled)
				BtnDrawAsStar.Checked = false;
		}

		/// <summary>
		/// Validate that the value entered in the text box is a proper number. If so, set the value and format the text in the box with a degree sign
		/// </summary>
		private void StartAngle_Leave(object sender, EventArgs e)
		{
			// Blanking out the textbox sets the value to 0
			if (StartAngle.TextLength == 0)
				StartAngle.Text = DEFAULT_STARTANGLE;

			string Value = StartAngle.Text.Replace(Constants.DEGREE_SIGN, string.Empty);

			float Angle = 0;

			if (float.TryParse(Value, out Angle))
				_startAngle = Angle;

			StartAngle.Text = _startAngle.ToString("0.0") + Constants.DEGREE_SIGN;
		}

		#endregion [ ToolStrip Events ]
	
	}
}


