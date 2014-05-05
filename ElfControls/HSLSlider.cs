using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     A vertical slider control that shows a range for a color property (a.k.a. Hue, Saturation, Brightness,
    ///     Red, Green, Blue) and sends an event when the slider is changed.
    /// </summary>
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\HSLSlider.bmp")]
    public partial class HSLSlider : UserControl {
        #region [ Enums ]

        #region MarkerCardinal enum

        public enum MarkerCardinal {
            Top,
            Left,
            Bottom,
            Right,
            Bar
        }

        #endregion

        #region eDrawStyle enum

        public enum eDrawStyle {
            Hue,
            HueOffset,
            Saturation,
            SaturationWithInversion,
            Luminance,
            Transparency,
            Bar
        }

        #endregion

        #region eMarkerLayout enum

        /// <summary>
        ///     Indicates which markers should be drawn.
        /// </summary>
        public enum eMarkerLayout {
            TopOrLeft,
            BottomOrRight,
            Both,
            None
        }

        #endregion

        #region eMarkerStyle enum

        /// <summary>
        ///     Indicates which style of markers should be drawn.
        /// </summary>
        public enum eMarkerStyle {
            Arrow,
            Circle,
            Diamond,
            DragBar,
            Triangle,
            Vee
        }

        #endregion

        #endregion [ Enums ]

        #region [ Private Variables ]

        // Slider properties

        // These variables keep track of how to fill in the content inside the box;
        private bool _barBorder = true;
        private Color _color;
        private double _deltaValue;
        private eDrawStyle _drawStyle = eDrawStyle.Hue;
        private int _drawingWidth;
        private bool _fillMarker;
        private ColorManager.HSL _hsl;
        private Color _indicatorColor = Color.Black;
        private List<double> _indicatorMarks = new List<double>();
        private bool _isDragging;
        private Color _markerColor;

        // Drawing variables
        private eMarkerLayout _markerLayout = eMarkerLayout.BottomOrRight;
        private int _markerSize = 6;
        private eMarkerStyle _markerStyle = eMarkerStyle.Triangle;
        private double _maxValue = 100;
        private double _minValue;
        private Orientation _orientation;
        private bool _reverseFill;
        private bool _snapToIndicator;
        private int _startOffset;
        private double _value = 50;
        private int _z1;
        private int _z2;
        private int _z3;
        private int _z4;

        #endregion [ Private Variables ]

        #region [ Private Properties ]

        /// <summary>
        ///     Determines the thickness of the bar, based on where the zone breakdowns are
        /// </summary>
        private int BarThickness {
            get { return _z3 - _z2; }
        }

        /// <summary>
        ///     Indicates if this control is is currently oriented vertically.
        /// </summary>
        private bool IsVertical {
            get { return (_orientation == Orientation.Vertical); }
        }

        /// <summary>
        ///     Indicates whether the marker should be shown.
        /// </summary>
        private bool ShowMarker {
            get { return (_markerLayout != eMarkerLayout.None); }
        }

        #endregion [ Private Properties ]

        #region [ Public Properties ]

        /// <summary>
        ///     The drawstyle of the contol (Hue, Saturation, Luminance).
        /// </summary>
        [DefaultValue(typeof (eDrawStyle), "Hue")]
        [Description("The drawstyle of the contol (Hue, Saturation, Luminance).")]
        public eDrawStyle DrawStyle {
            get { return _drawStyle; }
            set {
                _drawStyle = value;
                Refresh();
            }
        }

        /// <summary>
        ///     The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
        /// </summary>
        [Browsable(false)]
        public ColorManager.HSL HSL {
            get { return _hsl; }
            set {
                _hsl = value;
                _color = ColorManager.HSL_to_RGB(_hsl);
                Refresh();
            }
        }

        /// <summary>
        ///     Orientation of the control.
        /// </summary>
        [DefaultValue(typeof (Orientation), "Horizontal")]
        [Description("Orientation of the control.")]
        public Orientation Orientation {
            get { return _orientation; }
            set {
                _orientation = value;
                // If the orientation changed, but we are still sized for the other orientation, flip the values for the width and height.
                if (((_orientation == Orientation.Horizontal) && (Height > Width)) || ((_orientation == Orientation.Vertical) && (Width > Height))) {
                    int w = Width;
                    Width = Height;
                    Height = w;
                }
                Refresh();
            }
        }

        /// <summary>
        ///     Indicates whether a border line should be drawn about the slider bar itself.
        /// </summary>
        [DefaultValue(typeof (bool), "true")]
        [Description("Indicates whether a border line should be drawn about the slider bar itself.")]
        public bool BarBorder {
            get { return _barBorder; }
            set {
                _barBorder = value;
                Refresh();
            }
        }

        /// <summary>
        ///     The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
        /// </summary>
        public Color Color {
            get { return _color; }
            set {
                _color = value;
                HSL = ColorManager.RGB_to_HSL(_color);

                //	Redraw the control based on the new color.
                Refresh();
            }
        }

        /// <summary>
        ///     Indicates whether the marker should be filled with the value indicated.
        /// </summary>
        [DefaultValue(typeof (bool), "false")]
        [Description("Indicates whether the marker should be filled with the value indicated.")]
        public bool FillMarker {
            get { return _fillMarker; }
            set {
                _fillMarker = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (double), "1")]
        [Description("Transparency of the color.")]
        public double Transparency {
            get { return _hsl.Alpha; }
            set {
                _hsl.Alpha = value;
                _color = ColorManager.HSL_to_RGB(_hsl);
                Value = value * 100;
            }
        }

        /// <summary>
        ///     Size, in pixels, of the marker.
        /// </summary>
        [DefaultValue(typeof (int), "6")]
        [Description("Size, in pixels, of the marker.")]
        public int MarkerSize {
            get { return _markerSize; }
            set {
                _markerSize = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Color to draw the marker.
        /// </summary>
        [DefaultValue(typeof (Color), "ControlDarkDark")]
        [Description("Color to draw the marker.")]
        public Color MarkerColor {
            get { return _markerColor; }
            set {
                _markerColor = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets the style of marker, left/top, bottom/right, or both.
        /// </summary>
        [DefaultValue(typeof (eMarkerLayout), "BottomOrRight")]
        [Description("Gets or sets the style of marker, left/top, bottom/right, or both.")]
        public eMarkerLayout MarkerLayout {
            get { return _markerLayout; }
            set {
                _markerLayout = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Gets or Sets the style of marker that should be used.
        /// </summary>
        [DefaultValue(typeof (eMarkerStyle), "Triangle")]
        [Description("Gets or Sets the style of marker that should be used.")]
        public eMarkerStyle MarkerStyle {
            get { return _markerStyle; }
            set {
                _markerStyle = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Maximum value for the control.
        /// </summary>
        [DefaultValue(typeof (double), "100")]
        [Description("Maximum value for the control.")]
        public double MaxValue {
            get { return _maxValue; }
            set {
                _maxValue = value;

                if (_maxValue < _minValue) {
                    MaxValue = _minValue;
                }
                if (_value > MaxValue) {
                    Value = MaxValue;
                }
                Refresh();
            }
        }

        /// <summary>
        ///     Minimum value for the control.
        /// </summary>
        [DefaultValue(typeof (double), "0")]
        [Description("Minimum value for the control.")]
        public double MinValue {
            get { return _minValue; }
            set {
                _minValue = value;

                if (_minValue > _maxValue) {
                    MinValue = _maxValue;
                }
                if (_value < _minValue) {
                    Value = _minValue;
                }
                Refresh();
            }
        }

        [Description("Gets or sets padding within the control.")]
        public new Padding Padding {
            get { return base.Padding; }
            set {
                base.Padding = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Value the control is currently set at.
        /// </summary>
        [DefaultValue(typeof (double), "50")]
        [Description("Value the control is currently set at.")]
        public double Value {
            get { return _value; }
            set {
                _value = value;
                _value = (_value > _maxValue) ? _maxValue : ((_value < _minValue) ? _minValue : _value);
                Refresh();
                OnChanged();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Reverse the direction the color fill is drawn at.")]
        public bool ReverseFill {
            get { return _reverseFill; }
            set {
                _reverseFill = value;
                Refresh();
            }
        }

        [Description("Points at which indicator marks are drawn.")]
        public List<double> IndicatorMarks {
            get { return _indicatorMarks; }
            set {
                _indicatorMarks = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Color to draw the indicator Marks..
        /// </summary>
        [DefaultValue(typeof (Color), "Black")]
        [Description("Color to draw the indicator Marks..")]
        public Color IndictorColor {
            get { return _indicatorColor; }
            set {
                _indicatorColor = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Causes the marker to snap to the indicator when nearby.")]
        public bool SnapToIndicator {
            get { return _snapToIndicator; }
            set { _snapToIndicator = value; }
        }

        #endregion [ Public Properties ]

        #region [ Constructors ]

        public HSLSlider() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            //	Initialize Colors
            _hsl = new ColorManager.HSL(0, 1, 1);
            _color = ColorManager.HSL_to_RGB(_hsl);
            _markerColor = SystemColors.ControlDarkDark;
            _markerStyle = eMarkerStyle.Triangle;
            _markerSize = 6;
            _markerLayout = eMarkerLayout.BottomOrRight;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        #region [ Drawing Methods ]

        /// <summary>
        ///     Generic method used to generate the color bar.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="drawArea">Area to create the bar</param>
        /// <param name="startColor">Initial starting color</param>
        private void DrawBar(Graphics g, Rectangle drawArea, ColorManager.HSL startColor, int sgn, bool allowWrap) {
            var HSL = new ColorManager.HSL(startColor);
            int X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            int StartPoint = 0;
            int EndPoint = 0;

            if (IsVertical) {
                X1 = drawArea.Left;
                X2 = drawArea.Right;
                StartPoint = drawArea.Top;
                EndPoint = drawArea.Bottom;
            }
            else {
                Y1 = drawArea.Top;
                Y2 = drawArea.Bottom;
                StartPoint = drawArea.Left;
                EndPoint = drawArea.Right;
            }

            using (var LinePen = new Pen(Color.White)) {
                for (int i = StartPoint; i <= EndPoint; i++) {
                    HSL = GetDeltaColor(HSL, sgn, allowWrap);
                    LinePen.Color = ColorManager.HSL_to_RGB(HSL);
                    if (IsVertical) {
                        Y1 = i;
                        Y2 = i;
                    }
                    else {
                        X1 = i;
                        X2 = i;
                    }

                    g.DrawLine(LinePen, X1, Y1, X2, Y2);
                }

                LinePen.Color = _indicatorColor;
                Point Position;
                for (int i = 0; i < _indicatorMarks.Count; i++) {
                    Position = CalcPositionFromValue(_indicatorMarks[i]);
                    if (IsVertical) {
                        Y1 = Position.Y;
                        Y2 = Position.Y;
                    }
                    else {
                        X1 = Position.X;
                        X2 = Position.X;
                    }
                    g.DrawLine(LinePen, X1, Y1, X2, Y2);
                }
            }
        }


        /// <summary>
        ///     Draws the bar area with just an inset bar.
        /// </summary>
        private void DrawBar_Bar(Graphics g, Rectangle drawArea) {
            using (Brush Brush = new SolidBrush(_color)) {
                g.FillRectangle(Brush, drawArea);
            }
        }


        /// <summary>
        ///     Fills in the content of the control showing all values of Hue (from 0 to 360)
        /// </summary>
        private void DrawBar_Hue(Graphics g, Rectangle drawArea, bool offset) {
            if (offset) {
                DrawBar(g, drawArea, new ColorManager.HSL(1 - ValuePercent(_value), 1, 1), +1, true);
            }
            else {
                DrawBar(g, drawArea, new ColorManager.HSL(0.5, 1, 1), +1, true);
            }
        }


        /// <summary>
        ///     Fills in the content of the control showing all values of Luminance (0 to 100%) (ie a grayscale bar)
        /// </summary>
        private void DrawBar_Luminance(Graphics g, Rectangle drawArea) {
            if (_reverseFill) {
                DrawBar(g, drawArea, new ColorManager.HSL(0, 0, 0), -1, false);
            }
            else {
                DrawBar(g, drawArea, new ColorManager.HSL(0, 0, 1), -1, false);
            }
        }


        /// <summary>
        ///     Fills in the content of the control showing all values of Saturation (0 to 100%) for the given pure Hue
        /// </summary>
        private void DrawBar_Saturation(Graphics g, Rectangle drawArea, bool reverseFill, bool inverse) {
            ColorManager.HSL StartColor = null;
            int Sgn = -1;
            double Hue = _hsl.H;

            if (inverse) {
                Sgn = +1;
                Hue = AdjustValue(_hsl.H + 0.5, true);
            }

            if (reverseFill) {
                StartColor = new ColorManager.HSL(Hue, 0, 0.5);
            }
            else {
                StartColor = new ColorManager.HSL(Hue, 1, 1);
            }

            DrawBar(g, drawArea, StartColor, Sgn, false);
        }


        /// <summary>
        ///     Fills in the content of the control showing all values of Saturation (0 to 100%) for the given pure Hue
        /// </summary>
        private void DrawBar_Alpha(Graphics g, Rectangle drawArea) {
            // Draw the hatch pattern.
            using (var HatchBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.Silver)) {
                g.FillRectangle(HatchBrush, drawArea);
            }

            if (_reverseFill) {
                DrawBar(g, drawArea, new ColorManager.HSL(_hsl.H, 1, 1, 0), -1, false);
            }
            else {
                DrawBar(g, drawArea, new ColorManager.HSL(_hsl.H, 1, 1, 1), -1, false);
            }
        }


        /// <summary>
        ///     Draws a border line around the border of the bar.
        /// </summary>
        private void DrawBarBorder(Graphics g, Rectangle drawArea) {
            using (var DarkPen = new Pen(SystemColors.ControlDarkDark)) {
                g.DrawLine(DarkPen, new Point(drawArea.Left, drawArea.Top), new Point(drawArea.Right, drawArea.Top));
                g.DrawLine(DarkPen, new Point(drawArea.Left, drawArea.Top), new Point(drawArea.Left, drawArea.Bottom - 1));
            }
            using (var LightPen = new Pen(SystemColors.ControlLight)) {
                g.DrawLine(LightPen, new Point(drawArea.Left, drawArea.Bottom), new Point(drawArea.Right, drawArea.Bottom));
                g.DrawLine(LightPen, new Point(drawArea.Right, drawArea.Top), new Point(drawArea.Right, drawArea.Bottom));
            }
        }


        /// <summary>
        ///     Draws the particular style marker for the line
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="drawArea">Pre-calculated drawing area</param>
        /// <param name="topLeft">
        ///     Indicates if this is the top marker for horizontal orientation, or the left marker for vertical
        ///     orientation.
        /// </param>
        private void DrawMarker(Graphics g, Rectangle drawArea, bool topLeft) {
            if (!ShowMarker) {
                return;
            }

            GraphicsPath Path = null;
            float PenWidth = 1;
            bool Fill = true;

            switch (_markerStyle) {
                case eMarkerStyle.Arrow:
                    Path = GetArrowPath(GetMarkerCardinal(topLeft), drawArea, PenWidth);
                    Fill = false;
                    break;

                case eMarkerStyle.Circle:
                    Path = new GraphicsPath();
                    Path.AddEllipse(drawArea);
                    break;

                case eMarkerStyle.Diamond:
                    Path = GetDiamondPath(drawArea);
                    PenWidth = 1;
                    break;

                case eMarkerStyle.DragBar:
                    Path = new GraphicsPath();
                    Path.AddRectangle(drawArea);
                    PenWidth = 1;
                    break;

                case eMarkerStyle.Triangle:
                    Path = GetTrianglePath(GetMarkerCardinal(topLeft), drawArea);
                    PenWidth = 1;
                    break;

                case eMarkerStyle.Vee:
                    Path = GetVeePath(GetMarkerCardinal(topLeft), drawArea);
                    PenWidth = 1;
                    Fill = false;
                    break;

                default:
                    break;
            }

            if (Path != null) {
                if (_fillMarker && Fill) {
                    using (Brush MarkBrush = new SolidBrush(GetMarkerFillColor())) {
                        g.FillPath(MarkBrush, Path);
                    }
                }
                else {
                    if (Fill) {
                        using (Brush MarkBrush = new SolidBrush(_markerColor)) {
                            g.FillPath(MarkBrush, Path);
                        }
                    }
                }
                using (var MarkPen = new Pen(_markerColor, PenWidth)) {
                    g.DrawPath(MarkPen, Path);
                }

                //g.DrawRectangle(Pens.Red, drawArea);
                Path.Dispose();
                Path = null;
            }
        }


        /// <summary>
        ///     Draws the slider arrows on both sides of the control.
        /// </summary>
        /// <param name="position">
        ///     position value of the slider, lowest being at the bottom.  The range is between 0 and the
        ///     controls height-9.  The values will be adjusted if too large/small
        /// </param>
        /// <param name="unconditional">
        ///     If Unconditional is true, the slider is drawn, otherwise some logic is performed to
        ///     determine is drawing is really neccessary.
        /// </param>
        private void DrawSlider(Graphics g) {
            if (!ShowMarker) {
                return;
            }

            Point MarkerPoint = CalcPositionFromValue(Value);

            int HalfSize = _markerSize / 2;
            var DrawArea = new Rectangle(0, 0, HalfSize * 2, HalfSize * 2);

            if (IsVertical) {
                if (_markerStyle != eMarkerStyle.DragBar) {
                    if ((_markerLayout == eMarkerLayout.TopOrLeft) || (_markerLayout == eMarkerLayout.Both)) {
                        DrawArea.Location = new Point(_z1, MarkerPoint.Y - HalfSize);
                        DrawMarker(g, DrawArea, true);
                    }
                    if ((_markerLayout == eMarkerLayout.BottomOrRight) || (_markerLayout == eMarkerLayout.Both)) {
                        DrawArea.Location = new Point(_z3, MarkerPoint.Y - HalfSize);
                        DrawMarker(g, DrawArea, false);
                    }
                }
                else {
                    DrawArea = new Rectangle(0, 0, ClientRectangle.Width - Padding.Horizontal - 1, _markerSize);
                    DrawArea.Location = new Point(_startOffset, MarkerPoint.Y - HalfSize);
                    DrawMarker(g, DrawArea, true);
                }
            }
            else {
                if (_markerStyle != eMarkerStyle.DragBar) {
                    if ((_markerLayout == eMarkerLayout.TopOrLeft) || (_markerLayout == eMarkerLayout.Both)) {
                        DrawArea.Location = new Point(MarkerPoint.X - HalfSize, _z1);
                        DrawMarker(g, DrawArea, true);
                    }
                    if ((_markerLayout == eMarkerLayout.BottomOrRight) || (_markerLayout == eMarkerLayout.Both)) {
                        DrawArea.Location = new Point(MarkerPoint.X - HalfSize, _z3);
                        DrawMarker(g, DrawArea, false);
                    }
                }
                else {
                    DrawArea = new Rectangle(0, 0, _markerSize, ClientRectangle.Height - Padding.Vertical - 1);
                    DrawArea.Location = new Point(MarkerPoint.X - HalfSize, _startOffset);
                    DrawMarker(g, DrawArea, true);
                }
            }
        }

        #region [ Marker Path Methods ]

        /// <summary>
        ///     Generate a GraphicsPath for Arrow markers.
        /// </summary>
        /// <param name="cardinal">Enum used to indicate which direction the marker should be pointed.</param>
        /// <param name="drawArea">Rectangle that defines the area the marker should be drawn within.</param>
        /// <param name="penSize">Thickness of the pen used, used to offset the marker slightly.</param>
        private GraphicsPath GetArrowPath(MarkerCardinal cardinal, Rectangle drawArea, float penSize) {
            float ArrowHead = _markerSize / 2f;
            float HalfSize = _markerSize / 2f;
            var Path = new GraphicsPath();
            var Points = new List<PointF>();
            PointF Offset = PointF.Empty;
            switch (cardinal) {
                case MarkerCardinal.Top:
                    Offset = new PointF(0, -penSize);
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    break;
                case MarkerCardinal.Bottom:
                    Offset = new PointF(0, penSize);
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
                    break;
                case MarkerCardinal.Right:
                    Offset = new PointF(penSize, 0);
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    break;
                case MarkerCardinal.Left:
                default:
                    Offset = new PointF(-penSize, 0);
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    break;
            }

            PointF Edit;
            for (int i = 0; i < Points.Count; i++) {
                Edit = Points[i];
                Edit.X += Offset.X;
                Edit.Y += Offset.Y;
                Points[i] = Edit;
            }

            Path.AddLine(Points[0], Points[1]);
            Path.AddLine(Points[1], Points[2]);
            Path.AddLine(Points[2], Points[1]);
            Path.AddLine(Points[1], Points[3]);
            Points = null;
            return Path;
        }


        /// <summary>
        ///     Generate a GraphicsPath for Diamond markers.
        /// </summary>
        /// <param name="drawArea">Rectangle that defines the area the marker should be drawn within.</param>
        private GraphicsPath GetDiamondPath(Rectangle drawArea) {
            float HalfSize = _markerSize / 2f;
            var Path = new GraphicsPath();
            var Points = new List<PointF>();

            Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
            Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
            Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
            Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));

            Path.AddPolygon(Points.ToArray());
            Points = null;
            return Path;
        }


        /// <summary>
        ///     Generate a GraphicsPath for Triangle markers.
        /// </summary>
        /// <param name="cardinal">Enum used to indicate which direction the marker should be pointed.</param>
        /// <param name="drawArea">Rectangle that defines the area the marker should be drawn within.</param>
        /// <param name="penSize">Thickness of the pen used, used to offset the marker slightly.</param>
        private GraphicsPath GetTrianglePath(MarkerCardinal cardinal, Rectangle drawArea) {
            float HalfSize = _markerSize / 2f;
            var Path = new GraphicsPath();
            var Points = new List<PointF>();

            switch (cardinal) {
                case MarkerCardinal.Top:
                    Points.Add(new PointF(drawArea.Left, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top + _markerSize));
                    break;
                case MarkerCardinal.Bottom:
                    Points.Add(new PointF(drawArea.Left, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Bottom));
                    break;
                case MarkerCardinal.Right:
                    Points.Add(new PointF(drawArea.Right, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Right, drawArea.Bottom));
                    break;
                case MarkerCardinal.Left:
                default:
                    Points.Add(new PointF(drawArea.Left, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    Points.Add(new PointF(drawArea.Left, drawArea.Bottom));
                    break;
            }

            Path.AddPolygon(Points.ToArray());
            Points = null;
            return Path;
        }


        /// <summary>
        ///     Generate a GraphicsPath for "V" markers.
        /// </summary>
        /// <param name="cardinal">Enum used to indicate which direction the marker should be pointed.</param>
        /// <param name="drawArea">Rectangle that defines the area the marker should be drawn within.</param>
        private GraphicsPath GetVeePath(MarkerCardinal cardinal, Rectangle drawArea) {
            float HalfSize = _markerSize / 2f;
            var Path = new GraphicsPath();
            var Points = new List<PointF>();

            switch (cardinal) {
                case MarkerCardinal.Top:
                    drawArea.Offset((int) HalfSize, 0);
                    Points.Add(new PointF(drawArea.Left - HalfSize, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Top));
                    break;
                case MarkerCardinal.Bottom:
                    drawArea.Offset((int) HalfSize, 0);
                    Points.Add(new PointF(drawArea.Left - HalfSize, drawArea.Bottom));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left + HalfSize, drawArea.Bottom));
                    break;
                case MarkerCardinal.Right:
                    drawArea.Offset(0, (int) HalfSize);
                    Points.Add(new PointF(drawArea.Right, drawArea.Top - HalfSize));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top + HalfSize));
                    break;
                case MarkerCardinal.Left:
                default:
                    drawArea.Offset(0, (int) HalfSize);
                    Points.Add(new PointF(drawArea.Left, drawArea.Top - HalfSize));
                    Points.Add(new PointF(drawArea.Right, drawArea.Top));
                    Points.Add(new PointF(drawArea.Left, drawArea.Top + HalfSize));
                    break;
            }

            Path.AddLine(Points[0], Points[1]);
            Path.AddLine(Points[1], Points[2]);

            Points = null;
            return Path;
        }

        #endregion [ Marker Path Methods ]

        #endregion [ Drawing Methods ]

        #region [ Calc Methods ]

        private double AdjustValue(double value, bool allowWrap) {
            if (allowWrap) {
                while (value > 1) {
                    value -= 1;
                }
                while (value < 0) {
                    value += 1;
                }
            }
            else {
                if (value > 1) {
                    value = 1;
                }
                else if (value < 0) {
                    value = 0;
                }
            }
            return value;
        }


        /// <summary>
        ///     Calculates the radii and other measures needed to correctly render the torus and disks
        /// </summary>
        private void CalculateSizes() {
            if (IsVertical) {
                // the length/hieght of the rectangle to draw color bands.
                _drawingWidth = ClientRectangle.Height - Padding.Vertical;
                _startOffset = Padding.Top;
                if (_barBorder) {
                    _drawingWidth -= 2;
                    _startOffset += 1;
                }

                _z1 = _startOffset;
                _z2 = _z1;
                if (ShowMarker && ((_markerLayout == eMarkerLayout.TopOrLeft) || (_markerLayout == eMarkerLayout.Both))) {
                    _z2 += _markerSize;
                }
                _z4 = ClientRectangle.Width - Padding.Right;
                _z3 = _z4 - 1;
                if (ShowMarker && ((_markerLayout == eMarkerLayout.BottomOrRight) || (_markerLayout == eMarkerLayout.Both))) {
                    _z3 -= _markerSize;
                }
            }
            else {
                _drawingWidth = ClientRectangle.Width - Padding.Horizontal;
                _startOffset = Padding.Left;
                if (_barBorder) {
                    _drawingWidth -= 2;
                    _startOffset += 1;
                }

                _z1 = _startOffset;
                _z2 = _z1;
                if (ShowMarker && ((_markerLayout == eMarkerLayout.TopOrLeft) || (_markerLayout == eMarkerLayout.Both))) {
                    _z2 += _markerSize;
                }
                _z4 = ClientRectangle.Height - Padding.Bottom;
                _z3 = _z4 - 1;
                if (ShowMarker && ((_markerLayout == eMarkerLayout.BottomOrRight) || (_markerLayout == eMarkerLayout.Both))) {
                    _z3 -= _markerSize;
                }
            }
            _drawingWidth--;

            switch (_drawStyle) {
                case eDrawStyle.Bar:
                    _deltaValue = 1;
                    break;
                case eDrawStyle.SaturationWithInversion:
                    _deltaValue = 1.0 / _drawingWidth;
                    _deltaValue *= (3f / 2f);
                    break;
                default:
                    _deltaValue = 1.0 / _drawingWidth;
                    break;
            }


            if (_barBorder) {
                _z2++;
                _z3--;
            }
        }


        /// <summary>
        ///     Calculates the slider value from the position indicated.
        /// </summary>
        /// <param name="pt">Position to use to get the value.</param>
        private double CalcValueFromPosition(Point pt) {
            double Pct = 0;
            if (_orientation == Orientation.Horizontal) {
                int X = pt.X;
                X -= _startOffset;
                Pct = X / (double) _drawingWidth;
            }
            else {
                int Y = pt.Y;
                Y -= _startOffset;
                Pct = Y / (double) _drawingWidth;
            }
            Pct = AdjustValue(Pct, false);
            double TotalValue = _maxValue - _minValue;

            double Value = (TotalValue * Pct) + _minValue;

            // If snapping, check to see if there is an indicator nearby that is within a few percents of this value. If so, set the value to that of the indicator
            if (_snapToIndicator) {
                double IndPct = 0;
                double Threshold = 1 / 100.0;
                for (int i = 0; i < _indicatorMarks.Count; i++) {
                    IndPct = ValuePercent(_indicatorMarks[i]);
                    if (Math.Abs(Pct - IndPct) < Threshold) {
                        Value = _indicatorMarks[i];
                        break;
                    }
                }
            }

            return Value;
        }


        /// <summary>
        ///     Determine the position of the control that corresponds to this value. If oriented vertically, returns the point
        ///     along the left side
        ///     of the bar, horizontally, returns the point along to the topside.
        /// </summary>
        /// <param name="value">Value to use to calculate the point.</param>
        private Point CalcPositionFromValue(double value) {
            if (_orientation == Orientation.Vertical) {
                int y = (int) (_drawingWidth * ValuePercent(value)) + _startOffset;
                if (y <= _startOffset) {
                    y = _startOffset + 1;
                }
                return new Point(Padding.Left + (_barBorder ? 1 : 0), y);
            }
            int x = (int) (_drawingWidth * ValuePercent(value)) + _startOffset;
            if (x <= _startOffset) {
                x = _startOffset + 1;
            }
            return new Point(x, Padding.Top + (_barBorder ? 1 : 0));
        }


        /// <summary>
        ///     Calculates what percent of the absolute total the value is set to.
        /// </summary>
        private double ValuePercent(double value) {
            double TotalValue = _maxValue - _minValue;
            double RelValue = value - _minValue;
            double Pct = 0;

            if (TotalValue != 0) {
                Pct = RelValue / TotalValue;
            }
            return Pct;
        }

        #endregion [ Calc Methods ]

        /// <summary>
        ///     Determines the next color to use in the bar based on the drawing style and the last color used.
        /// </summary>
        /// <param name="oldColor">Last color used.</param>
        /// <param name="sgn">Multiplication value for the delta. Used to subtract the value, instead of adding it.</param>
        private ColorManager.HSL GetDeltaColor(ColorManager.HSL oldColor, int sgn, bool allowWrap) {
            var HSL = new ColorManager.HSL();
            double Hue = oldColor.Hue;
            double Saturation = oldColor.Saturation;
            double Luminance = oldColor.Luminance;
            double Alpha = oldColor.Alpha;

            if (_reverseFill) {
                sgn *= -1;
            }

            switch (_drawStyle) {
                case eDrawStyle.Hue:
                case eDrawStyle.HueOffset:
                    Hue += (_deltaValue * sgn);
                    Hue = AdjustValue(Hue, allowWrap);
                    return new ColorManager.HSL(Hue, Saturation, Luminance, Alpha);

                case eDrawStyle.Luminance:
                    Luminance += (_deltaValue * sgn);
                    Luminance = AdjustValue(Luminance, allowWrap);
                    return new ColorManager.HSL(Hue, Saturation, Luminance, Alpha);

                case eDrawStyle.Saturation:
                case eDrawStyle.SaturationWithInversion:
                    Saturation += (_deltaValue * sgn);
                    Saturation = AdjustValue(Saturation, allowWrap);

                    Luminance += ((_deltaValue * sgn) / 2);
                    Luminance = AdjustValue(Luminance, allowWrap);
                    return new ColorManager.HSL(Hue, Saturation, Luminance, Alpha);


                case eDrawStyle.Transparency:
                    Alpha += (_deltaValue * sgn);
                    Alpha = AdjustValue(Alpha, allowWrap);
                    return new ColorManager.HSL(Hue, Saturation, Luminance, Alpha);

                default:
                    return ColorManager.RGB_to_HSL(SystemColors.Control);
            }
        }


        /// <summary>
        ///     Determines what color the markers should be filled with if we are filling them, based on what style we are drawing
        ///     in.
        /// </summary>
        private Color GetMarkerFillColor() {
            float CalcValue = 0;
            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    CalcValue = (float) Value / 360f;
                    CalcValue = CalcValue > 1 ? 1 : CalcValue < 0 ? 0 : CalcValue;
                    return ColorManager.HSL_to_RGB(new ColorManager.HSL(CalcValue));

                case eDrawStyle.Luminance:
                    CalcValue = 1 - (float) (Value / 100.0);
                    CalcValue = CalcValue > 1 ? 1 : CalcValue < 0 ? 0 : CalcValue;
                    return ColorManager.HSL_to_RGB(new ColorManager.HSL(_hsl.H, 0, CalcValue));

                case eDrawStyle.Saturation:
                    CalcValue = 1 - (float) (Value / 100.0);
                    CalcValue = CalcValue > 1 ? 1 : CalcValue < 0 ? 0 : CalcValue;
                    return ColorManager.HSL_to_RGB(new ColorManager.HSL(_hsl.H, CalcValue, 0.5 + CalcValue / 2));

                case eDrawStyle.SaturationWithInversion:
                    CalcValue = 1 - (float) (Value / 100.0);
                    CalcValue = CalcValue > 1 ? 1 : CalcValue < -1 ? -1 : CalcValue;
                    if (CalcValue < 0) {
                        CalcValue = -CalcValue;
                        return ColorManager.HSL_to_RGB(new ColorManager.HSL(AdjustValue(_hsl.H + 0.5, true), CalcValue, 0.5 + CalcValue / 2));
                    }
                    return ColorManager.HSL_to_RGB(new ColorManager.HSL(_hsl.H, CalcValue, 0.5 + CalcValue / 2));

                case eDrawStyle.Bar:
                    return SystemColors.Control;

                case eDrawStyle.Transparency:
                default:
                    return Color.Transparent;
            }
        }


        /// <summary>
        ///     Determines which direction the marker is pointed at, based on the flag passed in, and the current orientation of
        ///     the control
        /// </summary>
        /// <param name="topLeft">
        ///     Indicates if the marker is the top marker for horizontal orientation, or the left marker for
        ///     vertical.
        /// </param>
        private MarkerCardinal GetMarkerCardinal(bool topLeft) {
            if (IsVertical && topLeft) {
                return MarkerCardinal.Left;
            }
            if (!IsVertical && topLeft) {
                return MarkerCardinal.Top;
            }
            if (IsVertical && !topLeft) {
                return MarkerCardinal.Right;
            }
            return MarkerCardinal.Bottom;
        }


        /// <summary>
        ///     Common code for the MouseDown, MouseMove and MouseUp event delegates
        /// </summary>
        /// <param name="e"></param>
        private void MoveMarker(MouseEventArgs e) {
            Value = CalcValueFromPosition(e.Location);
            Refresh();
        }


        /// <summary>
        ///     Calls all the functions neccessary to redraw the entire control.
        /// </summary>
        private void PaintControl(Graphics g) {
            g.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle DrawArea;
            CalculateSizes();
            int Size = 0;

            if (IsVertical) {
                Size = ClientRectangle.Height - Padding.Vertical;
                if (_barBorder) {
                    Size -= 2;
                }
                DrawArea = new Rectangle(_z2, _startOffset, BarThickness, Size);
            }
            else {
                Size = ClientRectangle.Width - Padding.Horizontal;
                if (_barBorder) {
                    Size -= 2;
                }
                DrawArea = new Rectangle(_startOffset, _z2, Size, BarThickness);
            }

            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    DrawBar_Hue(g, DrawArea, false);
                    break;

                case eDrawStyle.HueOffset:
                    DrawBar_Hue(g, DrawArea, false);
                    var HalfBar = new Rectangle(DrawArea.Location, DrawArea.Size);
                    if (IsVertical) {
                        HalfBar.Width /= 2;
                        if (_markerLayout == eMarkerLayout.BottomOrRight) {
                            HalfBar.Location = new Point(DrawArea.Right - HalfBar.Width, HalfBar.Top);
                        }
                    }
                    else {
                        HalfBar.Height /= 2;
                        if (_markerLayout == eMarkerLayout.BottomOrRight) {
                            HalfBar.Location = new Point(HalfBar.Left, DrawArea.Bottom - HalfBar.Height);
                        }
                    }
                    DrawBar_Hue(g, HalfBar, true);

                    break;

                case eDrawStyle.Saturation:
                    DrawBar_Saturation(g, DrawArea, _reverseFill, false);
                    break;

                case eDrawStyle.SaturationWithInversion:
                    Rectangle InvArea;
                    Rectangle SatArea;
                    int Third = 0;

                    if (IsVertical) {
                        Third = DrawArea.Height / 3;
                        if (_reverseFill) {
                            SatArea = new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width, 2 * Third);
                            InvArea = new Rectangle(DrawArea.Left, 2 * Third, DrawArea.Width, Third);
                        }
                        else {
                            InvArea = new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width, Third);
                            SatArea = new Rectangle(DrawArea.Left, Third, DrawArea.Width, 2 * Third);
                        }
                    }
                    else {
                        Third = DrawArea.Width / 3;
                        if (_reverseFill) {
                            SatArea = new Rectangle(DrawArea.Left, DrawArea.Top, 2 * Third, DrawArea.Height);
                            InvArea = new Rectangle(2 * Third, DrawArea.Top, Third, DrawArea.Height);
                        }
                        else {
                            SatArea = new Rectangle(Third, DrawArea.Top, 2 * Third, DrawArea.Height);
                            InvArea = new Rectangle(DrawArea.Left, DrawArea.Top, Third, DrawArea.Height);
                        }
                    }

                    if (_reverseFill) {
                        DrawBar_Saturation(g, SatArea, _reverseFill, false);
                        DrawBar_Saturation(g, InvArea, !_reverseFill, true);
                    }
                    else {
                        DrawBar_Saturation(g, InvArea, !_reverseFill, true);
                        DrawBar_Saturation(g, SatArea, _reverseFill, false);
                    }
                    break;

                case eDrawStyle.Luminance:
                    DrawBar_Luminance(g, DrawArea);
                    break;

                case eDrawStyle.Transparency:
                    DrawBar_Alpha(g, DrawArea);
                    break;

                case eDrawStyle.Bar:
                    DrawBar_Bar(g, DrawArea);
                    break;
                default:
                    break;
            }

            if (_barBorder) {
                DrawBarBorder(g, DrawArea);
            }

            DrawSlider(g);
        }

        #endregion [ Methods ]

        #region [ Events ]

        #region [ Event Declarations ]

        public event EventHandler Changed;

        #endregion [ Event Declarations ]

        #region [ Event Triggers ]

        private void OnChanged() {
            if (Changed != null) {
                Changed(this, new EventArgs());
            }
        }


        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) //	Only respond to left mouse button events
            {
                return;
            }

            _isDragging = true; //	Begin dragging which notifies MouseMove function that it needs to update the marker

            MoveMarker(e);
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            if (!_isDragging) //	Only respond when the mouse is dragging the marker.
            {
                return;
            }

            MoveMarker(e);
        }


        protected override void OnMouseUp(MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) //	Only respond to left mouse button events
            {
                return;
            }

            _isDragging = false;

            MoveMarker(e);
        }


        protected override void OnMouseWheel(MouseEventArgs e) {
            Value += e.Delta;
        }


        protected override void OnPaint(PaintEventArgs e) {
            PaintControl(e.Graphics);
        }


        protected override void OnResize(EventArgs e) {
            CalculateSizes();
            Refresh();
        }

        #endregion [ Event Triggers ]

        #endregion [ Events ]
    }
}