using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Polygon")]
    [ElfToolCore]
    public class PolygonTool : ShapeBase, ITool {
        #region [ Constants ]

        private const string DEGREE_SIGN = "°";
        private const string DEFAULT_NUMPOINTS = "5";
        private const string DEFAULT_STARTANGLE = "0" + DEGREE_SIGN;
        private const string NUMBER_OF_POINTS = "NumPoints";
        private const string IS_STAR = "IsStar";

        #endregion [ Constants ]

        #region [ Private Variables ]

        private bool _isStar;
        private int _numPoints;

        /// <summary>
        ///     Starting Angle of the polygon (in degrees)
        /// </summary>
        private float _startAngle = float.MaxValue;

        // Controls from ToolStrip
        private ToolStripButton cmdDrawAsStar;
        private ToolStripLabel lblWarningNumSides;
        private ToolStripTextBox txtNumPoints;
        private ToolStripTextBox txtStartAngle;

        #endregion [ Private Variables ]

        #region [ Constructors ]

        public PolygonTool() {
            ID = (int) ToolID.Polygon;
            Name = "Polygon";
            ToolBoxImage = ElfRes.polygon;
            ToolBoxImageSelected = ElfRes.polygon_selected;
            base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.polygon_modifier, new CanvasPoint(15, 15));
            MultiGestureKey1 = Keys.Y;

            _dashStyleControlName = "Polygon_cmdNoFill";
            _yesFillControlName = "Polygon_cmdFill";
            _dashStyleControlName = "Polygon_cboDashStyle";
            _lineThicknessControlName = "Polygon_cboLineSize";
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Load in the saved values from the Settings Xml file. The path to be used should be
        ///     ToolSettings|[Name of this tool].
        ///     We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
        ///     format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
        ///     for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            // Load the Settings values
            _isStar = LoadValue(IS_STAR, false);
            _numPoints = LoadValue(NUMBER_OF_POINTS, 5);
            _startAngle = LoadValue(Constants.START_ANGLE, 0.0f);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            cmdDrawAsStar = (ToolStripButton) GetItem<ToolStripButton>("Polygon_cmdDrawAsStar");
            txtNumPoints = (ToolStripTextBox) GetItem<ToolStripTextBox>("Polygon_txtNumPoints");
            txtStartAngle = (ToolStripTextBox) GetItem<ToolStripTextBox>("Polygon_txtStartAngle");
            lblWarningNumSides = (ToolStripLabel) GetItem<ToolStripLabel>("Polygon_lblWarningNumSides");

            AddButtonFaces(cmdDrawAsStar.Name, new ButtonImages(ElfRes.star, ElfRes.star_selected));

            // Assign the DashStyle enum value to the tag of each dash menu item
            SetDashStyleDropDownButton(cboDashStyle);

            // Set the initial value for the contol from what we had retrieve from Settings
            cmdDrawAsStar.Enabled = (_numPoints > 4);
            if (cmdDrawAsStar.Enabled) {
                cmdDrawAsStar.Checked = _isStar;
            }

            SetToolbarSelectedImage(cmdDrawAsStar);

            txtNumPoints.Text = _numPoints.ToString();
            txtStartAngle.Text = _startAngle.ToString("0.0") + DEGREE_SIGN;
            lblWarningNumSides.Visible = false;
        }


        /// <summary>
        ///     Canvas MouseDown event was fired. This version differs from the one in BaseTool in that the pushing of the Undo
        ///     data happens in MouseUp instead of MouseDown
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (_numPoints < 3) {
                return;
            }

            base.MouseDown(buttons, latticePoint, actualCanvasPoint);
        }


        /// <summary>
        ///     Canvas MouseMove event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (Profile == null) {
                return false;
            }

            if (_numPoints < 3) {
                return false;
            }

            return base.MouseMove(buttons, latticePoint, actualCanvasPoint);
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (_numPoints < 3) {
                return false;
            }

            return base.MouseUp(buttons, latticePoint, actualCanvasPoint);
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            base.SaveSettings();
            SaveValue(IS_STAR, _isStar);
            SaveValue(NUMBER_OF_POINTS, _numPoints);
            SaveValue(Constants.START_ANGLE, _startAngle);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cmdDrawAsStar = null;
            txtNumPoints = null;
            txtStartAngle = null;
            lblWarningNumSides = null;
        }


        /// <summary>
        ///     Attaches or detaches events to objects, such as Click events to buttons.
        /// </summary>
        /// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
        protected override void AttachEvents(bool attach) {
            // If we've already either attached or detached, exit out.
            if (attach && _eventsAttached) {
                return;
            }

            if (attach) {
                cmdDrawAsStar.Click += IsStar_Click;
                txtNumPoints.Leave += NumPoints_Leave;
                txtNumPoints.KeyPress += _toolStrip_Form.NumberOnly_KeyPress;
                txtStartAngle.Leave += StartAngle_Leave;
                txtStartAngle.KeyPress += _toolStrip_Form.SignedFloatOnly_KeyPress;
            }
            else {
                cmdDrawAsStar.Click -= IsStar_Click;
                txtNumPoints.Leave -= NumPoints_Leave;
                txtNumPoints.KeyPress -= _toolStrip_Form.NumberOnly_KeyPress;
                txtStartAngle.Leave -= StartAngle_Leave;
                txtStartAngle.KeyPress -= _toolStrip_Form.SignedFloatOnly_KeyPress;
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Create the graphics path needed to draw the path.
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        protected override GraphicsPath CreateRenderPath(CanvasPoint p1, CanvasPoint p2, bool finalRender) {
            Rectangle DrawArea;
            var Path = new GraphicsPath();

            DrawArea = _workshop.NormalizedRectangle(p1, p2);

            if (_numPoints == 4) {
                if ((_startAngle % 90) == 0) {
                    Path.AddRectangle(DrawArea);
                    return Path;
                }
            }

            // Reverse the sign on the start angle to get it to rotate anti-clockwise
            float StartAngle = -1 * _startAngle;

            var PI = (float) Math.PI;
            float r = Math.Min(DrawArea.Width, DrawArea.Height) / 2;
            var Verts = new PointF[_numPoints];
            var Origin = new Point(DrawArea.X + DrawArea.Width / 2, DrawArea.Y + DrawArea.Height / 2);
            float CurrentAngle = 0;

            // Internal angle is n-2 x PI / n (in radians)
            float InternalAngle = ((_numPoints - 2) * PI) / _numPoints;

            // Start angle is 1/2 the internal angle of the poly
            float StartingAngle = InternalAngle / 2;
            if (StartAngle != 0) {
                //StartingAngle += PI * _startAngle / 180;
                StartingAngle += _workshop.DegreeToRadian(StartAngle);
            }

            for (int i = 0; i < _numPoints; i++) {
                // i/n is the fraction of the circle we are going around
                // 2PI is the circle (in radian)
                // Starting Angle
                CurrentAngle = PI * 2 * (i / (float) _numPoints);
                CurrentAngle += StartingAngle;
                Verts[i] = _workshop.PointFromEllipse(DrawArea, _workshop.RadianToDegree(CurrentAngle));
            }

            if (_isStar && (_numPoints > 4)) {
                int NewSpot = 0;

                // Rearrange the verts so that the lines cross through.
                // Even numbered Polygons are actually 2 sets of lines (think Star of David)
                if (_numPoints % 2 == 0) {
                    var EvenVerts = new PointF[_numPoints / 2];
                    var OddVerts = new PointF[_numPoints / 2];
                    for (int i = 0; i < _numPoints / 2; i++) {
                        NewSpot = i * 2;
                        if (NewSpot > _numPoints) {
                            break;
                        }
                        EvenVerts[i] = Verts[NewSpot];
                    }
                    for (int i = 0; i < _numPoints / 2; i++) {
                        NewSpot = (i * 2) + 1;
                        if (NewSpot > _numPoints) {
                            break;
                        }
                        OddVerts[i] = Verts[NewSpot];
                    }
                    Path.AddLines(EvenVerts);
                    Path.CloseFigure();
                    Path.AddLines(OddVerts);
                    Path.CloseFigure();
                }
                else {
                    var StarVerts = new PointF[_numPoints];
                    for (int i = 0; i < _numPoints; i++) {
                        NewSpot = i * 2;
                        if (NewSpot > _numPoints) {
                            NewSpot -= _numPoints;
                        }
                        StarVerts[i] = Verts[NewSpot];
                    }
                    Path.AddLines(StarVerts);
                    Path.CloseFigure();
                }
            }
            else {
                Path.AddLines(Verts);
                Path.CloseFigure();
            }
            return Path;
        }


        /// <summary>
        ///     Draw the shape
        /// </summary>
        /// <param name="p1">Upper Left point in pixels</param>
        /// <param name="p2">Lower Right point in pixels</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        protected override void Render(CanvasPoint p1, CanvasPoint p2, bool finalRender) {
            if (finalRender) {
                p1 = _workshop.CalcLatticePoint(p1);
                p2 = _workshop.CalcLatticePoint(p2);
            }

            GraphicsPath DrawPath = CreateRenderPath(p1, p2, finalRender);

            using (Pen DrawPen = finalRender ? RenderPen() : _workshop.GetMarqueePen()) {
                try {
                    (finalRender ? _latticeBufferGraphics : _canvasControlGraphics).DrawPath(DrawPen, DrawPath);
                }
                catch (OutOfMemoryException) {}
            }

            if (_fill && finalRender) {
                if (!_isStar) {
                    _latticeBufferGraphics.FillPath(Brushes.White, DrawPath);
                }
                else {
                    if (_numPoints % 2 == 1) {
                        _latticeBufferGraphics.FillClosedCurve(Brushes.White, DrawPath.PathPoints, FillMode.Winding, 0.0f);
                    }
                    else {
                        // Treat as 2 curves, the points are broken out with the second following the first, both
                        // with the same number of points each
                        var Verts = new PointF[_numPoints / 2];

                        for (int i = 0; i < _numPoints / 2; i++) {
                            Verts[i] = DrawPath.PathPoints[i];
                        }
                        _latticeBufferGraphics.FillPolygon(Brushes.White, Verts);

                        for (int i = 0; i < _numPoints / 2; i++) {
                            Verts[i] = DrawPath.PathPoints[i + _numPoints / 2];
                        }
                        _latticeBufferGraphics.FillPolygon(Brushes.White, Verts);
                    }
                }
            }


            DrawPath.Dispose();
            DrawPath = null;
        }

        #endregion [ Methods ]

        private void IsStar_Click(object sender, EventArgs e) {
            _isStar = cmdDrawAsStar.Enabled && cmdDrawAsStar.Checked;
            SetToolbarSelectedImage(cmdDrawAsStar);
        }


        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void NumPoints_Leave(object sender, EventArgs e) {
            // Blanking out the textbox sets the value to 5
            if (txtNumPoints.TextLength == 0) {
                txtNumPoints.Text = DEFAULT_NUMPOINTS;
            }

            _numPoints = ValidateInteger(txtNumPoints, _numPoints);

            if (_numPoints < 3) {
                lblWarningNumSides.Visible = true;
                txtNumPoints.SelectAll();
                txtNumPoints.Focus();
            }
            else {
                lblWarningNumSides.Visible = false;
            }

            cmdDrawAsStar.Enabled = (_numPoints > 4);
            if (!cmdDrawAsStar.Enabled) {
                cmdDrawAsStar.Checked = false;
            }
        }


        /// <summary>
        ///     Validate that the value entered in the text box is a proper number. If so, set the value and format the text in the
        ///     box with a degree sign
        /// </summary>
        private void StartAngle_Leave(object sender, EventArgs e) {
            // Blanking out the textbox sets the value to 0
            if (txtStartAngle.TextLength == 0) {
                txtStartAngle.Text = DEFAULT_STARTANGLE;
            }

            string Value = txtStartAngle.Text.Replace(DEGREE_SIGN, string.Empty);

            float Angle = 0;

            if (float.TryParse(Value, out Angle)) {
                _startAngle = Angle;
            }

            txtStartAngle.Text = _startAngle.ToString("0.0") + DEGREE_SIGN;
        }
    }
}