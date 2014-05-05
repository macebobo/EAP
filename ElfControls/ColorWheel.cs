using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     Creates a color wheel
    /// </summary>
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorWheel.bmp")]
    public partial class ColorWheel : UserControl {
        #region [ Enums ]

        public enum eDrawStyle {
            Saturation,
            Brightness,
            Torus
        }

        #endregion [ Enums ]

        #region [ Constants ]

        /// <summary>
        ///     COLOR_COUNT represents the number of distinct colors used to create the circular gradient. Its value is somewhat
        ///     arbitrary -- change this to 6,
        ///     for example, to see what happens. 1536 (6 * 256) seems a good compromise -- it's enough to get a full range of
        ///     colors, but it doesn't overwhelm the
        ///     processor attempting to generate the image. The color wheel contains 6 sections, and each section displays 256
        ///     colors. Seems like a reasonable compromise.
        /// </summary>
        private const int COLOR_COUNT = 360; //6 * 256;

        private const double DEGREES_PER_RADIAN = (180 / Math.PI);

        #endregion [ Constants ]

        #region [ Private Variables ]

        // Testing
        private readonly eDrawStyle _drawStyle = eDrawStyle.Torus;
        private Point _centerPoint = Point.Empty;
        private int _centerRadius;
        private Color _color = Color.Red;
        private Point _colorMarker = Point.Empty;
        private Region _diskRegion;
        private double _dotFrequency = 0.101;
        private bool _dotOutline = false;

        private bool _drawColorOnBackground;
        private bool _drawTestDots = false;

        // Torus Variables
        private int _edgePadding;
        private bool _emboss = true;
        private bool _hideMarkers;

        private ColorManager.HSL _hsl = new ColorManager.HSL(0, 0, 0, 1);

        /// <summary>
        ///     This is to keep our rotation on the wheel, even if Luminance and/or Saturation are 0 or 100, keeps it from flipping
        ///     to red.
        /// </summary>
        private double _hueAngle;

        private Point _hueMarker = Point.Empty;
        private double _hueMarkerWidth = 2.0;
        private int _innerRadius;
        private bool _isDragging;
        private UInt32 _markerSize = 10;
        private bool _mouseDown;

        private bool _mouseInDisk;
        private bool _mouseInTorus;
        private bool _mouseInTriangle;
        private int _outerRadius;
        private double _redOffset = -90;
        private bool _renderForGetPixel;
        private bool _showFocusRectangle = false;
        private int _testDotSize = 6;
        private UInt32 _thickness;
        private Region _torusRegion;
        private Region _triangleRegion;

        #endregion [ Private Variables ]

        #region [ Public Properties ]

        [Browsable(false)]
        public new bool CanFocus {
            get { return true; }
        }

        /// <summary>
        ///     The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
        /// </summary>
        [DefaultValue(typeof (Color), "Red")]
        [Description("Selected Color.")]
        public Color Color {
            get { return _color; }
            set {
                SetColor(value);
                _hueAngle = AngleFromHue(_hsl.H);
                Reset_Marker(true);
                Refresh();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Draw the selected color on the control background.")]
        public bool DrawColorOnBackground {
            get { return _drawColorOnBackground; }
            set {
                _drawColorOnBackground = value;
                Refresh();
            }
        }

        /// <summary>
        ///     The drawstyle of the contol (Hue, Saturation, Brightness)
        /// </summary>
        //[DefaultValue(typeof(eDrawStyle), "Hue")]
        //public eDrawStyle DrawStyle
        //{
        //    get { return _drawStyle; }
        //    set
        //    {
        //        _drawStyle = value;
        //        Reset_Marker(true);
        //        this.Refresh();
        //    }
        //}
        [DefaultValue(typeof (bool), "True")]
        public bool EmbossGraphic {
            get { return _emboss; }
            set {
                _emboss = value;
                Refresh();
            }
        }

        /// <summary>
        ///     The attribute of a visual sensation according to which an area appears to be similar to one of the perceived
        ///     colors: red, yellow, green, and blue, or to a combination of two of them.
        /// </summary>
        [DefaultValue(typeof (double), "1")]
        public double Hue {
            get { return _hsl.Hue; }
            set {
                if (value < 0) {
                    _hsl.Hue = 0;
                }
                else if (value > 1) {
                    _hsl.Hue = 1;
                }
                else {
                    _hsl.Hue = value;
                }
                _color = ColorManager.HSL_to_RGB(_hsl);
                _hueAngle = AngleFromHue(_hsl.H);
                OnColorChanged();
            }
        }

        [DefaultValue(typeof (double), "2")]
        public double HueMarkerWidth {
            get { return _hueMarkerWidth; }
            set {
                if (value <= 0) {
                    _hueMarkerWidth = 1;
                }
                else if (value > 10) {
                    _hueMarkerWidth = 10;
                }
                else {
                    _hueMarkerWidth = value;
                }
                Refresh();
            }
        }

        /// <summary>
        ///     The radiance weighted by the effect of each wavelength on a typical human observer, measured in candela per square
        ///     meter (cd/m2).
        ///     Often the term luminance is used for the relative luminance, Y/Yn, where Yn is the luminance of the reference white
        ///     point.
        /// </summary>
        public double Luminance {
            get { return _hsl.Luminance; }
            set {
                if (value < 0) {
                    _hsl.Luminance = 0;
                }
                else if (value > 1) {
                    _hsl.Luminance = 1;
                }
                else {
                    _hsl.Luminance = value;
                }
                _color = ColorManager.HSL_to_RGB(_hsl);
                OnColorChanged();
            }
        }

        [DefaultValue(typeof (int), "10")]
        public UInt32 MarkerSize {
            get { return _markerSize; }
            set {
                _markerSize = value;
                Refresh();
            }
        }

        public new Padding Padding {
            get { return base.Padding; }
            set {
                base.Padding = value;
                if (_drawStyle == eDrawStyle.Torus) {
                    Refresh();
                }
            }
        }

        [DefaultValue(typeof (int), "-90")]
        [Description("Degree offset for the color Red.")]
        public double RedOffset {
            get { return _redOffset; }
            set {
                _redOffset = value;
                Reset_Marker(true);
                Refresh();
            }
        }

        [DefaultValue(typeof (byte), "255")]
        [Description("Redness of the color.")]
        public byte Red {
            get { return _color.R; }
            set {
                _color = Color.FromArgb(value, _color.G, _color.B);
                _hsl = ColorManager.RGB_to_HSL(_color);
                _hueAngle = AngleFromHue(_hsl.H);
                OnColorChanged();
            }
        }

        [DefaultValue(typeof (byte), "0")]
        [Description("Greenness of the color.")]
        public byte Green {
            get { return _color.G; }
            set {
                _color = Color.FromArgb(_color.R, value, _color.B);
                _hsl = ColorManager.RGB_to_HSL(_color);
                _hueAngle = AngleFromHue(_hsl.H);
                OnColorChanged();
            }
        }

        [DefaultValue(typeof (byte), "255")]
        [Description("Blueness of the color.")]
        public byte Blue {
            get { return _color.B; }
            set {
                _color = Color.FromArgb(_color.R, _color.G, value);
                _hsl = ColorManager.RGB_to_HSL(_color);
                _hueAngle = AngleFromHue(_hsl.H);
                OnColorChanged();
            }
        }

        /// <summary>
        ///     The "colorfulness of a stimulus relative to its own brightness".
        /// </summary>
        public double Saturation {
            get { return _hsl.Saturation; }
            set {
                if (value < 0) {
                    _hsl.Saturation = 0;
                }
                else if (value > 1) {
                    _hsl.Saturation = 1;
                }
                else {
                    _hsl.Saturation = value;
                }
                _color = ColorManager.HSL_to_RGB(_hsl);
                OnColorChanged();
            }
        }

        [DefaultValue(typeof (UInt32), "0")]
        [Description("Thickness of the torus. If 0, thickness is 10% of the outer radius.")]
        public UInt32 TorusThickness {
            get { return _thickness; }
            set {
                _thickness = value;
                Refresh();
            }
        }

        #endregion [ Public Properties ]

        #region [ Private Properties ]

        /// <summary>
        ///     The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
        /// </summary>
        [Browsable(false)]
        public ColorManager.HSL HSL {
            get { return _hsl; }
            set {
                if (_mouseDown) {
                    return;
                }
                _hsl = value;
                _color = ColorManager.HSL_to_RGB(_hsl);
                Reset_Marker(true);
                Refresh();
            }
        }

        /// <summary>
        ///     Position of the Hue Marker. Using a Property instead of a variable for debugging.
        /// </summary>
        private Point HueMarker {
            get { return _hueMarker; }
            set { _hueMarker = value; }
        }

        /// <summary>
        ///     Position of the Color Marker. Using a Property instead of a variable for debugging.
        /// </summary>
        private Point ColorMarker {
            get { return _colorMarker; }
            set { _colorMarker = value; }
        }

        //private Color PureColor
        //{
        //    get { return ColorManager.HSL_to_RGB(new ColorManager.HSL(_hsl.H)); }
        //}

        #endregion [  Private Properties ]

        #region [ Constructors ]

        public ColorWheel() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //	Initialize Colors
            _hsl = new ColorManager.HSL(1, 1, 1, 1);
            _color = ColorManager.HSL_to_RGB(_hsl);
            _drawStyle = eDrawStyle.Torus;
            _hueAngle = -1 * RedOffset;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Returns the angle on the torus that corresponds to a given hue value. Returns angle in degrees.
        /// </summary>
        /// <param name="hue">Hue value, from 0.0 to 1.0</param>
        /// <returns>The angle in degrees, from 0 to 359.99...</returns>
        private double AngleFromHue(double hue) {
            double Angle = (hue * 360 - _redOffset) % 360;
            if (Angle < 0) {
                Angle += 360;
            }
            return Angle;
        }


        /// <summary>
        ///     Calculates the hue that this Point would be if drawn a line from the center point to where
        ///     it would intercept the hue torus.
        /// </summary>
        private double AngleFromTorusPoint(Point pt) {
            double xDiff = pt.X - _centerPoint.X;
            double yDiff = pt.Y - _centerPoint.Y;
            double Theta = RadianToDegree(Math.Atan2(-yDiff, xDiff));
            if (Theta < 0) {
                Theta += 360;
            }
            return Theta;
        }


        /// <summary>
        ///     Calculates the color from the marker point on the shade triangle.
        /// </summary>
        private Color CalcColorFromColorMarker() {
            using (var b = new Bitmap(ClientRectangle.Width, ClientRectangle.Height)) {
                using (Graphics g1 = Graphics.FromImage(b)) {
                    _hideMarkers = true;
                    _renderForGetPixel = true;
                    var pe = new PaintEventArgs(g1, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
                    InvokePaintBackground(this, pe);
                    InvokePaint(this, pe);
                    //b.Save("c:\\test.png", System.Drawing.Imaging.ImageFormat.Png);
                    _renderForGetPixel = false;
                    _hideMarkers = false;
                }
                return b.GetPixel(_colorMarker.X, _colorMarker.Y);
            }
        }


        /// <summary>
        ///     Resets the controls color (both HSL and RGB variables) based on the current marker position
        /// </summary>
        private void CalcColorFromHueMarker() {
            //_hsl.H = HueFromAngle(AngleFromTorusPoint(HueMarker));
            Hue = HueFromAngle(_hueAngle);
            //_hsl.H = HueFromAngle(_hueAngle);
            //_color = ColorManager.HSL_to_RGB(_hsl);
            //OnColorChanged();
        }


        /// <summary>
        ///     Calculates the radii and other measures needed to correctly render the torus and disks
        /// </summary>
        private void CalculateSizes() {
            int MinSize = GetMinSize();

            if (MinSize == ClientRectangle.Width) {
                _edgePadding = Padding.Horizontal / 2;
            }
            else {
                _edgePadding = Padding.Vertical / 2;
            }

            _edgePadding += 3;

            var Thickness = (int) _thickness;
            if (_thickness == 0) {
                Thickness = (int) (MinSize * 0.1);
            }

            _outerRadius = (MinSize / 2) - _edgePadding;
            _innerRadius = _outerRadius - Thickness;
            _centerRadius = _outerRadius - (Thickness / 2);

            _centerPoint = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
        }


        /// <summary>
        ///     Calculates the distance between 2 points
        /// </summary>
        private double ComputeDistance(Point pt1, Point pt2) {
            return Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
        }


        /// <summary>
        ///     Converts an angle from Degrees to one of Radians
        /// </summary>
        /// <param name="angle">Angle value in Degrees</param>
        /// <returns>Corresponding angle in Radians</returns>
        private double DegreeToRadian(double angle) {
            return Math.PI * angle / 180.0;
        }


        /// <summary>
        ///     Draws the color marker on the shade triangle
        /// </summary>
        /// <param name="g">Graphics Object</param>
        private void DrawColorMarker(Graphics g) {
            if (_hideMarkers) {
                return;
            }

            // Find the color marker based on the default color.
            if ((_colorMarker.X == 0) && (_colorMarker.Y == 0)) {
                _colorMarker = LocateColorMarker(_hsl);
            }

            //			g.DrawString(_colorMarker.ToString(), this.Font, Brushes.Black, new Point(0, 0));

            if ((_hsl.L == 1.0) && (_hsl.S == 1.0)) {
                return;
            }

            using (Brush b = new SolidBrush(ColorManager.HSL_to_RGB(_hsl))) {
                g.FillEllipse(b, ColorMarker.X - (_markerSize / 2), ColorMarker.Y - (_markerSize / 2), _markerSize, _markerSize);
            }

            using (var MarkerPen = new Pen(SystemColors.ControlDarkDark, 2f)) {
                g.DrawEllipse(MarkerPen, ColorMarker.X - (_markerSize / 2), ColorMarker.Y - (_markerSize / 2), _markerSize, _markerSize);
            }

            using (var MarkerPen = new Pen(SystemColors.ControlLight, 2f)) {
                g.DrawEllipse(MarkerPen, ColorMarker.X - (_markerSize / 2) + 1, ColorMarker.Y - (_markerSize / 2) + 1, _markerSize - 2,
                    _markerSize - 2);
            }
        }


        /// <summary>
        ///     Evaluates the DrawStyle of the control and calls the appropriate
        ///     drawing function for content
        /// </summary>
        private void DrawContent(Graphics g) {
            switch (_drawStyle) {
                case eDrawStyle.Torus:
                    Draw_Style_Torus(g);
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation(g);
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance(g);
                    break;
            }
        }


        /// <summary>
        ///     Draws the hue marker on the torus
        /// </summary>
        /// <param name="g">Graphics Object</param>
        private void DrawHueMarker(Graphics g) {
            if (_hideMarkers) {
                return;
            }

            //double Theta = AngleFromHue(_hsl.H);

            var Corners = new List<Point>();
            //double Delta = 1;
            //Corners.Add(GetTorusPoint(Theta - Delta, _innerRadius + 1));
            //Corners.Add(GetTorusPoint(Theta - Delta, _outerRadius - 1));
            //Corners.Add(GetTorusPoint(Theta + Delta, _outerRadius - 1));
            //Corners.Add(GetTorusPoint(Theta + Delta, _innerRadius + 1));

            Corners.Add(GetTorusPoint(_hueAngle - _hueMarkerWidth, _innerRadius + 1));
            Corners.Add(GetTorusPoint(_hueAngle - _hueMarkerWidth, _outerRadius - 1));
            Corners.Add(GetTorusPoint(_hueAngle + _hueMarkerWidth, _outerRadius - 1));
            Corners.Add(GetTorusPoint(_hueAngle + _hueMarkerWidth, _innerRadius + 1));

            using (var Path = new GraphicsPath()) {
                double TempHue = HueFromAngle(_hueAngle);
                Path.AddLines(Corners.ToArray());
                Path.CloseFigure();
                using (var MarkerBrush = new SolidBrush(ColorManager.HSL_to_RGB(new ColorManager.HSL(TempHue, 1, 1)))) {
                    g.FillPath(MarkerBrush, Path);
                }
                using (var MarkerPen = new Pen(SystemColors.ControlDarkDark, 2)) {
                    g.DrawPath(MarkerPen, Path);
                }
            }
        }


        /// <summary>
        ///     Create the embossing look around the disk/torus
        /// </summary>
        /// <param name="g">Graphics object to use</param>
        /// <param name="drawInnerRing">
        ///     Indicates whether the inner radius of the torus should be shaded. Set to false for shading
        ///     a disk.
        /// </param>
        private void DrawTorusShading(Graphics g, bool drawInnerRing) {
            if (!_emboss) {
                return;
            }

            int NumSteps = 6;
            int Alpha = 255 / (NumSteps + 5);
            int DeltaAlpha = Alpha;
            Color Light = Color.White;
            Color Shadow = SystemColors.ControlDark;
            int SweepAngle = 180;
            int StartAngle_Light = 45;
            int StartAngle_Shadow = 225;
            float PenWidth = 1f;
            float DeltaPen = GetMinSize() * 0.001f;
            int DeltaTheta = 4;

            Point P1 = _centerPoint;
            P1.Offset(-_outerRadius, -_outerRadius);

            Point P2 = _centerPoint;
            P2.Offset(-_innerRadius, -_innerRadius);

            var OuterRect = new Rectangle(P1, new Size(_outerRadius * 2, _outerRadius * 2));
            var InnerRect = new Rectangle(P2, new Size(_innerRadius * 2, _innerRadius * 2));

            for (int i = 1; i <= NumSteps; i++) {
                using (var ShadowPen = new Pen(Color.FromArgb(Alpha, Light), PenWidth)) {
                    using (var LightPen = new Pen(Color.FromArgb(Alpha, Shadow), PenWidth)) {
                        g.DrawArc(LightPen, OuterRect, StartAngle_Light, SweepAngle);
                        g.DrawArc(ShadowPen, OuterRect, StartAngle_Shadow, SweepAngle);

                        if (drawInnerRing) {
                            g.DrawArc(ShadowPen, InnerRect, StartAngle_Light, SweepAngle);
                            g.DrawArc(LightPen, InnerRect, StartAngle_Shadow, SweepAngle);
                        }

                        PenWidth += DeltaPen;
                        SweepAngle -= (DeltaTheta * 2);
                        StartAngle_Light += DeltaTheta;
                        StartAngle_Shadow += DeltaTheta;
                        Alpha += DeltaAlpha;
                    }
                }
            }
            //Debug.WriteLine(PenWidth);
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Luminance or Brightness value.
        /// </summary>
        private void Draw_Style_Luminance(Graphics g) {
            if (_drawColorOnBackground) {
                g.Clear(ColorManager.HSL_to_RGB(_hsl));
            }
            else {
                g.Clear(BackColor);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            if ((_innerRadius == 0) && (_outerRadius == 0)) {
                CalculateSizes();
            }

            Point P1 = _centerPoint;
            P1.Offset(-_outerRadius, -_outerRadius);

            var OuterRect = new Rectangle(P1, new Size(_outerRadius * 2, _outerRadius * 2));

            // Define a region for the torus to allow use to determine where the user clicked.
            using (var Path = new GraphicsPath()) {
                Path.AddEllipse(OuterRect);

                if (_diskRegion != null) {
                    _diskRegion.Dispose();
                    _diskRegion = null;
                }
                _diskRegion = new Region(Path);

                var Points = new List<PointF>();

                // Pad the radius a bit so we don't get an anti-alias bug on the right side
                Points.AddRange(GetTorusPoints(_outerRadius + 2));

                using (var Brush = new PathGradientBrush(Points.ToArray())) {
                    Brush.CenterColor = ColorManager.HSL_to_RGB(new ColorManager.HSL(0, 0, 0));
                    Brush.SurroundColors = GetTorusColors().ToArray();
                    g.FillPath(Brush, Path);
                }
                Points = null;
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Saturation value.
        /// </summary>
        private void Draw_Style_Saturation(Graphics g) {
            if (_drawColorOnBackground) {
                g.Clear(ColorManager.HSL_to_RGB(_hsl));
            }
            else {
                g.Clear(BackColor);
            }
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if ((_innerRadius == 0) && (_outerRadius == 0)) {
                CalculateSizes();
            }

            Point P1 = _centerPoint;
            P1.Offset(-_outerRadius, -_outerRadius);

            var OuterRect = new Rectangle(P1, new Size(_outerRadius * 2, _outerRadius * 2));

            // Define a region for the torus to allow use to determine where the user clicked.
            using (var Path = new GraphicsPath()) {
                Path.AddEllipse(OuterRect);

                if (_diskRegion != null) {
                    _diskRegion.Dispose();
                    _diskRegion = null;
                }
                _diskRegion = new Region(Path);

                var Points = new List<PointF>();
                // Pad the radius a bit so we don't get an anti-alias bug on the right side
                Points.AddRange(GetTorusPoints(_outerRadius + 2));

                using (var Brush = new PathGradientBrush(Points.ToArray())) {
                    Brush.CenterColor = Color.FromArgb(128, 128, 128);
                    //Brush.CenterColor = ColorManager.HSL_to_RGB(new ColorManager.HSL(0, 0, 0.5));
                    Brush.SurroundColors = GetTorusColors().ToArray();
                    g.FillPath(Brush, Path);
                }
                Points = null;
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Hue value.
        /// </summary>
        private void Draw_Style_Torus(Graphics g) {
            if (!_renderForGetPixel) {
                if (_drawColorOnBackground) {
                    g.Clear(ColorManager.HSL_to_RGB(_hsl));
                }
                else {
                    g.Clear(BackColor);
                }
                g.SmoothingMode = SmoothingMode.AntiAlias;

                //if ((_innerRadius == 0) && (_outerRadius == 0))
                CalculateSizes();

                Point P1 = _centerPoint;
                P1.Offset(-_outerRadius, -_outerRadius);

                Point P2 = _centerPoint;
                P2.Offset(-_innerRadius, -_innerRadius);

                var OuterRect = new Rectangle(P1, new Size(_outerRadius * 2, _outerRadius * 2));
                var InnerRect = new Rectangle(P2, new Size(_innerRadius * 2, _innerRadius * 2));

                // Define a region for the torus to allow use to determine where the user clicked.
                using (var Path = new GraphicsPath()) {
                    Path.AddEllipse(OuterRect);
                    Path.AddEllipse(InnerRect);

                    if (_torusRegion != null) {
                        _torusRegion.Dispose();
                        _torusRegion = null;
                    }
                    _torusRegion = new Region(Path);

                    var Points = new List<PointF>();
                    // Pad the radius a bit so we don't get an anti-alias bug on the right side
                    Points.AddRange(GetTorusPoints(_outerRadius + 2));

                    using (var Brush = new PathGradientBrush(Points.ToArray())) {
                        Brush.CenterColor = Color.FromArgb(128, 128, 128);
                        Brush.SurroundColors = GetTorusColors().ToArray();
                        g.FillPath(Brush, Path);
                    }
                    Points = null;
                }

                // Do the Shading around the Torus
                DrawTorusShading(g, true);
            }
            else {
                g.Clear(SystemColors.Control);
            }

            DrawTriangle(g);
        }


        /// <summary>
        ///     Sets up a series of colors and display them on the shade triangle
        /// </summary>
        private void DrawTestDots(Graphics g) {
            if (_drawTestDots) {
                for (double l = 1; l >= 0; l -= _dotFrequency) {
                    for (double s = 0; s <= 1.0; s += _dotFrequency) {
                        TestDot(g, s, l);
                    }
                }
            }
        }


        /// <summary>
        ///     Draws the shade triangle, with 1 vertex pointed at the position on the torus that corresponds to the currently
        ///     selected hue, the other
        ///     two points 60 deg away, colored black and white. This shape will be filled with color that ranges from the hue at
        ///     full saturation and brightness,
        ///     then fades to white and black in the direction of the proper vertex.
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawTriangle(Graphics g) {
            double TempHue = HueFromAngle(_hueAngle);

            //g.DrawString(_hueAngle.ToString() + "\n" + TempHue.ToString(), this.Font, Brushes.Black, new Point(0, 0));

            PathGradientBrush PathGradientBrush = null;
            var Colors = new List<Color>();
            List<Point> Points = null;
            var HSL = new ColorManager.HSL(TempHue, 0.75, 1);
            Color CentralColor = ColorManager.HSL_to_RGB(HSL);
            Color PtColor = Color.Empty;
            Color PureColor = ColorManager.HSL_to_RGB(new ColorManager.HSL(TempHue));

            // Render the triangle with half alpha slightly larger, so we don't the the graphics glitch where there are gaps in the outline.
            using (var Path = new GraphicsPath()) {
                if (!_renderForGetPixel) {
                    Points = GetTriangleCorners(_innerRadius + 1);
                    Path.AddPolygon(Points.ToArray());
                    g.FillPath(SystemBrushes.ControlDarkDark, Path);

                    PathGradientBrush = new PathGradientBrush(Points.ToArray());
                    Colors.Add(Color.FromArgb(128, PureColor));
                    Colors.Add(Color.FromArgb(128, Color.White));
                    Colors.Add(Color.FromArgb(128, Color.Black));
                    PathGradientBrush.CenterColor = Color.FromArgb(128, CentralColor);
                    PathGradientBrush.SurroundColors = Colors.ToArray();
                    g.FillPath(PathGradientBrush, Path);
                }

                // Render the triangle proper sized.			
                Points = GetTriangleCorners(_innerRadius);

                if (_renderForGetPixel) {
                    var Distance = (int) ComputeDistance(Points[0], Points[1]);
                    int Radius = Distance / 4;
                    var Bubble = new Rectangle(0, 0, Radius * 2, Radius * 2);
                    Point MidWay = Point.Empty;

                    MidWay.X = Math.Min(Points[0].X, Points[1].X);
                    MidWay.X += Math.Abs(Points[0].X - Points[1].X) / 2;
                    MidWay.Y = Math.Min(Points[0].Y, Points[1].Y);
                    MidWay.Y += Math.Abs(Points[0].Y - Points[1].Y) / 2;
                    Bubble.Location = MidWay;
                    Bubble.Offset(-Radius, -Radius);
                    using (var FillBrush = new SolidBrush(ColorManager.HSL_to_RGB(new ColorManager.HSL(TempHue, 0.5, 1)))) {
                        g.FillEllipse(FillBrush, Bubble);
                    }

                    MidWay.X = Math.Min(Points[0].X, Points[2].X);
                    MidWay.X += Math.Abs(Points[0].X - Points[2].X) / 2;
                    MidWay.Y = Math.Min(Points[0].Y, Points[2].Y);
                    MidWay.Y += Math.Abs(Points[0].Y - Points[2].Y) / 2;
                    Bubble.Location = MidWay;
                    Bubble.Offset(-Radius, -Radius);
                    using (var FillBrush = new SolidBrush(ColorManager.HSL_to_RGB(new ColorManager.HSL(TempHue, 1, 0.5)))) {
                        g.FillEllipse(FillBrush, Bubble);
                    }

                    MidWay.X = Math.Min(Points[2].X, Points[1].X);
                    MidWay.X += Math.Abs(Points[2].X - Points[1].X) / 2;
                    MidWay.Y = Math.Min(Points[2].Y, Points[1].Y);
                    MidWay.Y += Math.Abs(Points[2].Y - Points[1].Y) / 2;
                    Bubble.Location = MidWay;
                    Bubble.Offset(-Radius, -Radius);
                    using (var FillBrush = new SolidBrush(ColorManager.HSL_to_RGB(new ColorManager.HSL(TempHue, 0, 0.5)))) {
                        g.FillEllipse(FillBrush, Bubble);
                    }

                    Radius += Convert.ToInt32(Radius * 0.1);
                    Bubble = new Rectangle(0, 0, Radius * 2, Radius * 2);
                    Bubble.Location = Points[0];
                    Bubble.Offset(-Radius, -Radius);
                    using (var FillBrush = new SolidBrush(PureColor)) {
                        g.FillEllipse(FillBrush, Bubble);
                    }

                    Bubble.Location = Points[1];
                    Bubble.Offset(-Radius, -Radius);
                    g.FillEllipse(Brushes.White, Bubble);

                    Bubble.Location = Points[2];
                    Bubble.Offset(-Radius, -Radius);
                    g.FillEllipse(Brushes.Black, Bubble);
                }

                Path.Reset();
                Path.AddPolygon(Points.ToArray());

                if (_triangleRegion != null) {
                    _triangleRegion.Dispose();
                    _triangleRegion = null;
                }
                _triangleRegion = new Region(Path);
                Colors.Clear();

                PathGradientBrush = new PathGradientBrush(Points.ToArray());

                Colors.Add(PureColor);
                Colors.Add(Color.White);
                Colors.Add(Color.Black);

                HSL = new ColorManager.HSL(TempHue, 0.75, 1);
                PathGradientBrush.CenterPoint = LocateColorMarker(HSL);
                PathGradientBrush.CenterColor = CentralColor;
                PathGradientBrush.SurroundColors = Colors.ToArray();

                g.FillPath(PathGradientBrush, Path);
                PathGradientBrush.Dispose();

                if (!_renderForGetPixel) {
                    using (var OutlinePen = new Pen(SystemColors.ControlDarkDark, 1f)) {
                        g.DrawPath(OutlinePen, Path);
                    }
                }
            }

            DrawTestDots(g);
        }


        /// <summary>
        ///     Determines if the width or height is smaller, returns that value.
        /// </summary>
        private int GetMinSize() {
            return Math.Min(ClientRectangle.Width, ClientRectangle.Height);
        }


        /// <summary>
        ///     Create an array of COLOR_COUNT colors, looping through all the  hues between 0 and 255, broken into COLOR_COUNT
        ///     intervals.
        ///     HSV is particularly well-suited for this, because the only value that changes as you create colors is the Hue.
        /// </summary>
        private List<Color> GetTorusColors() {
            var Colors = new List<Color>();
            double Theta = 0;

            for (int i = 0; i <= COLOR_COUNT - 1; i++) {
                Theta = ((double) i / COLOR_COUNT) * 360;
                Colors.Add(ColorManager.HSL_to_RGB(new ColorManager.HSL(HueFromAngle(Theta - _redOffset), 1, 1)));
            }
            return Colors;
        }


        /// <summary>
        ///     Create a list of Points equidistant around the torus to give us our hue spread.
        /// </summary>
        /// <param name="radius">Distance from the centerpoint</param>
        /// <returns></returns>
        private List<PointF> GetTorusPoints(int radius) {
            var Points = new List<PointF>();
            double Theta = 0;

            for (int i = 0; i <= COLOR_COUNT - 1; i++) {
                Theta = ((double) i / COLOR_COUNT) * 360;
                Points.Add(GetTorusPoint(Theta - _redOffset, radius));
            }
            return Points;
        }


        /// <summary>
        ///     Given the center of a circle and its radius, along with the angle corresponding to the point, find the coordinates.
        ///     In other words, convert from polar to rectangular coordinates.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <param name="centerPoint"></param>
        /// <returns></returns>
        private Point GetTorusPoint(double angle, int radius) {
            double radians = angle / DEGREES_PER_RADIAN;

            return new Point((int) (_centerPoint.X + Math.Floor(radius * Math.Cos(radians))),
                (int) (_centerPoint.Y - Math.Floor(radius * Math.Sin(radians))));
        }


        /// <summary>
        ///     Returns the points of an equilateral triangle that fits within a circle of the given radius.
        ///     One of the points corresponds to the hue of the currently selected color.
        /// </summary>
        /// <param name="radius">Radius of the circle the triangle will fit within.</param>
        private List<Point> GetTriangleCorners(int radius) {
            var Vertices = new List<Point>();
            //double Theta = AngleFromHue(_hsl.H);
            //Vertices.Add(GetTorusPoint(Theta, radius));
            //Vertices.Add(GetTorusPoint(Theta + 120, radius));
            //Vertices.Add(GetTorusPoint(Theta + 240, radius));
            Vertices.Add(GetTorusPoint(_hueAngle, radius));
            Vertices.Add(GetTorusPoint(_hueAngle + 120, radius));
            Vertices.Add(GetTorusPoint(_hueAngle + 240, radius));

            return Vertices;
        }


        /// <summary>
        ///     Returns the hue that corresponds to a particular angle
        /// </summary>
        /// <param name="hue">The angle in degrees, from 0 to 359.99...</param>
        /// <returns>Hue value, from 0.0 to 1.0</returns>
        private double HueFromAngle(double theta) {
            theta += _redOffset;
            theta %= 360;
            if (theta < 0) {
                theta += 360;
            }
            double Hue = theta / 360.0;
            if (Hue < 0) {
                Hue += 1;
            }
            return Hue;
        }


        /// <summary>
        ///     Determines if the current point lies within the region.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="region">Region to check.</param>
        /// <param name="point">Point to check</param>
        /// <returns>Returns true if the point lies within the region, false otherwise.</returns>
        private bool InRegion(Graphics g, Region region, Point point) {
            return ((g != null) && (region != null) && region.IsVisible(point, g));
        }


        /// <summary>
        ///     Determines the point within the shade triangle that corresponds to the currently selected color.
        /// </summary>
        private Point LocateColorMarker(Color color) {
            return LocateColorMarker(ColorManager.RGB_to_HSL(color));
        }


        /// <summary>
        ///     Determines the point within the shade triangle that corresponds to the currently selected color.
        /// </summary>
        /// <returns></returns>
        private Point LocateColorMarker(ColorManager.HSL hsl) {
            //double HueAngle = _hueAngle;
            double HueAngle = AngleFromHue(hsl.H);
            Point HueMarker = GetTorusPoint(HueAngle, _centerRadius);
            double Radius = _centerRadius;

            switch (_drawStyle) {
                case eDrawStyle.Torus:
                    double X = 0;
                    double Y = 0;
                    double Ly = Math.Abs(1 - hsl.L);
                    double CosTheta = Math.Cos(DegreeToRadian(30));

                    List<Point> Vertices = GetTriangleCorners(_innerRadius);

                    // Length of a side of the triangle.
                    double d = ComputeDistance(Vertices[0], Vertices[2]);

                    // Distance from one point to the opposite base.
                    double W = d * CosTheta;

                    // This forms a right angle, with white at top left, black at bottom left and hue at top right
                    X = d * hsl.S * hsl.L;
                    Y = d * Ly;

                    // We need to distort this triangle so that the hue point ends up using the following offsets.
                    double XPct = X / d;
                    X += (XPct * (W - d));
                    Y += (XPct * d / 2);

                    // Now rotate the point to correspond to the hue angle and offset it to line up with the actual triangle.
                    double Theta = DegreeToRadian(AngleFromHue(hsl.H));

                    return new Point((int) (X * Math.Cos(Theta) + Y * Math.Sin(Theta) + Vertices[1].X),
                        (int) (-X * Math.Sin(Theta) + Y * Math.Cos(Theta) + Vertices[1].Y));

                case eDrawStyle.Brightness:
                    Radius *= hsl.L;
                    return new Point((int) (Radius * Math.Cos(HueAngle)), (int) (Radius * Math.Sin(HueAngle)));

                case eDrawStyle.Saturation:
                    Radius *= hsl.S;
                    return new Point((int) (Radius * Math.Cos(HueAngle)), (int) (Radius * Math.Sin(HueAngle)));

                default:
                    return Point.Empty;
            }
        }


        /// <summary>
        ///     Common code for the MouseDown, MouseMove and MouseUp event delegates
        /// </summary>
        /// <param name="e"></param>
        private void MoveMarker(MouseEventArgs e) {
            if (_mouseInTorus) {
                if (_torusRegion.IsVisible(e.Location)) {
                    // Determine the closest hue to where the mouse position is and set the marker to that spot on the torus.
                    _hueAngle = AngleFromTorusPoint(e.Location);

                    // Now set the marker point to the hue that corresponds to this angle.
                    HueMarker = GetTorusPoint(_hueAngle, _centerRadius);

                    // Calculate the color from the marker position.
                    ColorMarker = LocateColorMarker(_hsl);
                    CalcColorFromHueMarker();
                }
            }
            else if (_mouseInTriangle) {
                if (_triangleRegion.IsVisible(e.Location)) {
                    _colorMarker = e.Location;
                    ColorManager.HSL HSL = ColorManager.RGB_to_HSL(CalcColorFromColorMarker());

                    _hsl.L = HSL.L;
                    _hsl.S = HSL.S;
                    _color = ColorManager.HSL_to_RGB(_hsl);
                    ColorMarker = LocateColorMarker(_hsl);
                    OnColorChanged();
                }
            }

            Refresh();
        }


        /// <summary>
        ///     Calls all the functions neccessary to redraw the entire control.
        /// </summary>
        private void PaintControl(Graphics g) {
            DrawContent(g);
            if (_drawStyle == eDrawStyle.Torus) {
                DrawHueMarker(g);
            }
            DrawColorMarker(g);
            if (Focused && _showFocusRectangle) {
                ControlPaint.DrawFocusRectangle(g, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
            }
        }


        /// <summary>
        ///     Converts an angle from Radians to one of Degrees
        /// </summary>
        /// <param name="angle">Angle value in Radians</param>
        /// <returns>Corresponding angle in Degrees</returns>
        private double RadianToDegree(double angle) {
            return angle * (180 / Math.PI);
        }


        /// <summary>
        ///     Resets the marker position of the slider to match the controls color.  Gives the option of redrawing the slider.
        /// </summary>
        /// <param name="redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
        private void Reset_Marker(bool redraw) {
            switch (_drawStyle) {
                case eDrawStyle.Torus:
                    if (_innerRadius == 0) {
                        CalculateSizes();
                    }
                    HueMarker = GetTorusPoint(AngleFromHue(_hsl.H), _centerRadius);
                    ColorMarker = LocateColorMarker(_hsl);
                    break;
                case eDrawStyle.Brightness:
                case eDrawStyle.Saturation:
                    if (_outerRadius == 0) {
                        CalculateSizes();
                    }
                    ColorMarker = LocateColorMarker(_hsl);
                    break;
            }

            Refresh();
        }


        /// <summary>
        ///     Sets the local color variable, converts it to the HSL equivalent and calls the ColorChanged event.
        /// </summary>
        /// <param name="newColor">Color value to set.</param>
        private void SetColor(Color newColor) {
            _color = newColor;
            _hsl = ColorManager.RGB_to_HSL(_color);
            OnColorChanged();
        }


        /// <summary>
        ///     Create a dot to test the position of colors on the shade triangle
        /// </summary>
        private void TestDot(Graphics g, double sat, double lum) {
            int W = _testDotSize;
            var HSL = new ColorManager.HSL(_hsl.H, sat, lum);
            Point pt = LocateColorMarker(HSL);
            var R = new Rectangle(pt.X - W / 2, pt.Y - W / 2, W, W);
            using (var b = new SolidBrush(ColorManager.HSL_to_RGB(HSL))) {
                g.FillEllipse(b, R);
                if (_dotOutline) {
                    g.DrawEllipse(Pens.Black, R);
                }
            }
            HSL = null;
        }

        #endregion [ Methods ]

        #region [ Events ]

        #region [ Event Declarations ]

        public event EventHandler ColorChanged;

        #endregion [ Event Declarations ]

        #region [ Event Triggers ]

        private void OnColorChanged() {
            if (ColorChanged != null) {
                ColorChanged(this, new EventArgs());
            }
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Refresh();
        }


        protected override void OnMouseDown(MouseEventArgs e) {
            // If the dragging flag is set, something happened that we couldn't clean up. Reset this and other flags.
            if (_isDragging) {
                _isDragging = false;
                _mouseInDisk = false;
                _mouseInTorus = false;
                _mouseInTriangle = false;
            }

            //	Only respond to left mouse button events
            if (e.Button != MouseButtons.Left) {
                return;
            }
            _mouseDown = true;

            // Check to see if we are within the hue torus. If so, then trap the mouse until the user releases the button
            using (Graphics g = CreateGraphics()) {
                if (InRegion(g, _diskRegion, e.Location)) {
                    _mouseInDisk = true;
                }

                if (InRegion(g, _torusRegion, e.Location)) {
                    _mouseInTorus = true;
                }

                if (InRegion(g, _triangleRegion, e.Location)) {
                    _mouseInTriangle = true;
                }
            }

            if (!_mouseInDisk && !_mouseInTorus && !_mouseInTriangle) {
                return;
            }

            //	Begin dragging which notifies MouseMove function that it needs to update the marker
            _isDragging = true;

            MoveMarker(e);
        }


        protected override void OnMouseLeave(EventArgs e) {
            _mouseDown = false;
            base.OnMouseLeave(e);
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            // Only respond when the mouse is dragging the marker.
            if (!_isDragging) {
                return;
            }

            MoveMarker(e);
        }


        protected override void OnMouseUp(MouseEventArgs e) {
            _mouseDown = false;

            //	Only respond to left mouse button events
            if (e.Button != MouseButtons.Left) {
                return;
            }

            if (!_isDragging) {
                return;
            }

            _isDragging = false;

            if (!_mouseInDisk && !_mouseInTorus && _mouseInTriangle) {
                return;
            }

            MoveMarker(e);

            _mouseInTorus = false;
            _mouseInDisk = false;
            _mouseInTriangle = false;
        }


        protected override void OnPaint(PaintEventArgs e) {
            PaintControl(e.Graphics);
        }


        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            Refresh();
        }


        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            Refresh();
        }


        protected override void OnResize(EventArgs e) {
            CalculateSizes();
            Reset_Marker(true);
            Refresh();
        }

        #endregion [ Event Triggers ]

        #endregion [ Events ]
    }
}