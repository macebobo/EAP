using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     Provides a interface to edit color values
    /// </summary>
    [ToolboxBitmap(@"~\ElfControls\Resources\ColorBox.bmp")]
    public partial class ColorBox : UserControl {
        #region [ Enums ]

        public enum eDrawStyle {
            Hue,
            Saturation,
            Brightness,
            Red,
            Green,
            Blue
        }

        #endregion [ Enums ]

        #region [ Private Variables ]

        // These variables keep track of how to fill in the content inside the box;
        private eDrawStyle _drawStyle = eDrawStyle.Hue;
        private ColorManager.HSL _hsl;
        private bool _isDragging;
        private int _markerX;
        private int _markerY;
        private Color _rgb;

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     The drawstyle of the contol (Hue, Saturation, Brightness, Red, Green or Blue)
        /// </summary>
        public eDrawStyle DrawStyle {
            get { return _drawStyle; }
            set {
                _drawStyle = value;

                //	Redraw the control based on the new eDrawStyle
                Reset_Marker(true);
                Redraw_Control();
            }
        }

        /// <summary>
        ///     The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
        /// </summary>
        public ColorManager.HSL HSL {
            get { return _hsl; }
            set {
                _hsl = value;
                _rgb = ColorManager.HSL_to_RGB(_hsl);

                //	Redraw the control based on the new color.
                Reset_Marker(true);
                Redraw_Control();
            }
        }

        /// <summary>
        ///     The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
        /// </summary>
        public Color RGB {
            get { return _rgb; }
            set {
                _rgb = value;
                _hsl = ColorManager.RGB_to_HSL(_rgb);

                //	Redraw the control based on the new color.
                Reset_Marker(true);
                Redraw_Control();
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorBox() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //	Initialize Colors
            _hsl = new ColorManager.HSL();
            _hsl.H = 1.0;
            _hsl.S = 1.0;
            _hsl.L = 1.0;
            _rgb = ColorManager.HSL_to_RGB(_hsl);
            _drawStyle = eDrawStyle.Hue;
        }

        #endregion [ Constructors ]

        #region [ Events ]

        #region [ Event Declarations ]

        public new event EventHandler Scroll;

        #endregion [ Event Declarations ]

        #region [ Event Triggers ]

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Redraw_Control();
        }


        protected override void OnMouseDown(MouseEventArgs e) {
            //	Only respond to left mouse button events
            if (e.Button != MouseButtons.Left) {
                return;
            }

            //	Begin dragging which notifies MouseMove function that it needs to update the marker
            _isDragging = true;

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) {
                x = 0;
            }
            if (x > Width - 4) {
                x = Width - 4; //	Calculate marker position
            }
            if (y < 0) {
                y = 0;
            }
            if (y > Height - 4) {
                y = Height - 4;
            }

            //	If the marker hasn't moved, no need to redraw it.
            //	or send a scroll notification
            if (x == _markerX && y == _markerY) {
                return;
            }

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB(); //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
            {
                Scroll(this, e);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            if (!_isDragging) //	Only respond when the mouse is dragging the marker.
            {
                return;
            }

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) {
                x = 0;
            }
            if (x > Width - 4) {
                x = Width - 4; //	Calculate marker position
            }
            if (y < 0) {
                y = 0;
            }
            if (y > Height - 4) {
                y = Height - 4;
            }

            if (x == _markerX && y == _markerY) //	If the marker hasn't moved, no need to redraw it.
            {
                return; //	or send a scroll notification
            }

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB(); //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
            {
                Scroll(this, e);
            }
        }


        protected override void OnMouseUp(MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) //	Only respond to left mouse button events
            {
                return;
            }

            if (!_isDragging) {
                return;
            }

            _isDragging = false;

            int x = e.X - 2, y = e.Y - 2;
            if (x < 0) {
                x = 0;
            }
            if (x > Width - 4) {
                x = Width - 4; //	Calculate marker position
            }
            if (y < 0) {
                y = 0;
            }
            if (y > Height - 4) {
                y = Height - 4;
            }

            if (x == _markerX && y == _markerY) //	If the marker hasn't moved, no need to redraw it.
            {
                return; //	or send a scroll notification
            }

            DrawMarker(x, y, true); //	Redraw the marker
            ResetHSLRGB(); //	Reset the color

            if (Scroll != null) //	Notify anyone who cares that the controls marker (selected color) has changed
            {
                Scroll(this, e);
            }
        }


        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Redraw_Control();
        }


        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Redraw_Control();
        }

        #endregion [ Event Triggers ]

        #endregion [ Events ]

        #region [ Methods ]

        /// <summary>
        ///     Redraws only the content over the marker
        /// </summary>
        private void ClearMarker() {
            Graphics g = CreateGraphics();

            //	Determine the area that needs to be redrawn
            int start_x, start_y, end_x, end_y;
            int red = 0;
            int green = 0;
            int blue = 0;
            var hsl_start = new ColorManager.HSL();
            var hsl_end = new ColorManager.HSL();

            //	Find the markers corners
            start_x = _markerX - 5;
            start_y = _markerY - 5;
            end_x = _markerX + 5;
            end_y = _markerY + 5;

            //	Adjust the area if part of it hangs outside the content area
            if (start_x < 0) {
                start_x = 0;
            }
            if (start_y < 0) {
                start_y = 0;
            }
            if (end_x > Width - 4) {
                end_x = Width - 4;
            }
            if (end_y > Height - 4) {
                end_y = Height - 4;
            }

            //	Redraw the content based on the current draw style:
            //	The code get's a little messy from here
            switch (_drawStyle) {
                    //	S=0,S=1,S=2,S=3.....S=100
                    //	L=100
                    //	L=99
                    //	L=98		Drawstyle
                    //	L=97		   Hue
                    //	...
                    //	L=0
                case eDrawStyle.Hue:

                    hsl_start.H = _hsl.H;
                    hsl_end.H = _hsl.H; //	Hue is constant
                    hsl_start.S = (double) start_x / (Width - 4); //	Because we're drawing horizontal lines, s will not change
                    hsl_end.S = (double) end_x / (Width - 4); //	from line to line

                    for (int i = start_y; i <= end_y; i++) //	For each horizontal line:
                    {
                        hsl_start.L = 1.0 - (double) i / (Height - 4); //	Brightness (L) WILL change for each horizontal
                        hsl_end.L = hsl_start.L; //	line drawn

                        var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), ColorManager.HSL_to_RGB(hsl_start),
                            ColorManager.HSL_to_RGB(hsl_end), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                    //	H=0,H=1,H=2,H=3.....H=360
                    //	L=100
                    //	L=99
                    //	L=98		Drawstyle
                    //	L=97		Saturation
                    //	...
                    //	L=0
                case eDrawStyle.Saturation:

                    hsl_start.S = _hsl.S;
                    hsl_end.S = _hsl.S; //	Saturation is constant
                    hsl_start.L = 1.0 - (double) start_y / (Height - 4); //	Because we're drawing vertical lines, L will
                    hsl_end.L = 1.0 - (double) end_y / (Height - 4); //	not change from line to line

                    for (int i = start_x; i <= end_x; i++) //	For each vertical line:
                    {
                        hsl_start.H = (double) i / (Width - 4); //	Hue (H) WILL change for each vertical
                        hsl_end.H = hsl_start.H; //	line drawn

                        var br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), ColorManager.HSL_to_RGB(hsl_start),
                            ColorManager.HSL_to_RGB(hsl_end), 90, false);
                        g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                    }
                    break;
                    //		  H=0,H=1,H=2,H=3.....H=360
                    //	S=100
                    //	S=99
                    //	S=98		Drawstyle
                    //	S=97		Brightness
                    //	...
                    //	S=0
                case eDrawStyle.Brightness:

                    hsl_start.L = _hsl.L;
                    hsl_end.L = _hsl.L; //	Luminance is constant
                    hsl_start.S = 1.0 - (double) start_y / (Height - 4); //	Because we're drawing vertical lines, S will
                    hsl_end.S = 1.0 - (double) end_y / (Height - 4); //	not change from line to line

                    for (int i = start_x; i <= end_x; i++) //	For each vertical line:
                    {
                        hsl_start.H = (double) i / (Width - 4); //	Hue (H) WILL change for each vertical
                        hsl_end.H = hsl_start.H; //	line drawn

                        var br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), ColorManager.HSL_to_RGB(hsl_start),
                            ColorManager.HSL_to_RGB(hsl_end), 90, false);
                        g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                    }

                    break;
                    //		  B=0,B=1,B=2,B=3.....B=100
                    //	G=100
                    //	G=99
                    //	G=98		Drawstyle
                    //	G=97		   Red
                    //	...
                    //	G=0
                case eDrawStyle.Red:

                    red = _rgb.R; //	Red is constant
                    int start_b = Round(255 * (double) start_x / (Width - 4)); //	Because we're drawing horizontal lines, B
                    int end_b = Round(255 * (double) end_x / (Width - 4)); //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++) //	For each horizontal line:
                    {
                        green = Round(255 - (255 * (double) i / (Height - 4))); //	green WILL change for each horizontal line drawn

                        var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1),
                            Color.FromArgb(red, green, start_b), Color.FromArgb(red, green, end_b), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                    //		  B=0,B=1,B=2,B=3.....B=100
                    //	R=100
                    //	R=99
                    //	R=98		Drawstyle
                    //	R=97		  Green
                    //	...
                    //	R=0
                case eDrawStyle.Green:

                    green = _rgb.G;
                    ; //	Green is constant
                    int start_b2 = Round(255 * (double) start_x / (Width - 4)); //	Because we're drawing horizontal lines, B
                    int end_b2 = Round(255 * (double) end_x / (Width - 4)); //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++) //	For each horizontal line:
                    {
                        red = Round(255 - (255 * (double) i / (Height - 4))); //	red WILL change for each horizontal line drawn

                        var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1),
                            Color.FromArgb(red, green, start_b2), Color.FromArgb(red, green, end_b2), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
                    //		  R=0,R=1,R=2,R=3.....R=100
                    //	G=100
                    //	G=99
                    //	G=98		Drawstyle
                    //	G=97		   Blue
                    //	...
                    //	G=0
                case eDrawStyle.Blue:

                    blue = _rgb.B;
                    //	Blue is constant
                    int start_r = Round(255 * (double) start_x / (Width - 4)); //	Because we're drawing horizontal lines, R
                    int end_r = Round(255 * (double) end_x / (Width - 4)); //	will not change from line to line

                    for (int i = start_y; i <= end_y; i++) //	For each horizontal line:
                    {
                        green = Round(255 - (255 * (double) i / (Height - 4))); //	green WILL change for each horizontal line drawn

                        var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1),
                            Color.FromArgb(start_r, green, blue), Color.FromArgb(end_r, green, blue), 0, false);
                        g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                    }

                    break;
            }
        }


        /// <summary>
        ///     Draws the marker (circle) inside the box
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="unconditional"></param>
        private void DrawMarker(int x, int y, bool unconditional) {
            if (x < 0) {
                x = 0;
            }
            if (x > Width - 4) {
                x = Width - 4;
            }
            if (y < 0) {
                y = 0;
            }
            if (y > Height - 4) {
                y = Height - 4;
            }

            if (_markerY == y && _markerX == x && !unconditional) {
                return;
            }

            ClearMarker();

            _markerX = x;
            _markerY = y;

            Graphics g = CreateGraphics();

            Pen pen;
            //	The selected color determines the color of the marker drawn over
            //	it (black or white)
            ColorManager.HSL _hsl = GetColor(x, y);
            if (_hsl.L < (double) 200 / 255) {
                pen = new Pen(Color.White);
            }
            else if (_hsl.H < (double) 26 / 360 || _hsl.H > (double) 200 / 360) {
                if (_hsl.S > (double) 70 / 255) {
                    //	White marker if selected color is dark
                    pen = new Pen(Color.White);
                }
                else {
                    //	Else use a black marker for lighter colors
                    pen = new Pen(Color.Black);
                }
            }
            else {
                pen = new Pen(Color.Black);
            }

            //	Draw the marker : 11 x 11 circle
            g.DrawEllipse(pen, x - 3, y - 3, 10, 10);

            //	Force the border to be redrawn, just in case the marker has been drawn over it.
            DrawBorder();
        }


        /// <summary>
        ///     Draws the border around the control.
        /// </summary>
        private void DrawBorder() {
            Graphics g = CreateGraphics();

            Pen pencil;

            //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
            //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line)
            //	inside the gray/white rectangle

            pencil = new Pen(Color.FromArgb(172, 168, 153)); //	The same gray color used by Photoshop
            g.DrawLine(pencil, Width - 2, 0, 0, 0); //	Draw top line
            g.DrawLine(pencil, 0, 0, 0, Height - 2); //	Draw left hand line

            pencil = new Pen(Color.White);
            g.DrawLine(pencil, Width - 1, 0, Width - 1, Height - 1); //	Draw right hand line
            g.DrawLine(pencil, Width - 1, Height - 1, 0, Height - 1); //	Draw bottome line

            pencil = new Pen(Color.Black);
            g.DrawRectangle(pencil, 1, 1, Width - 3, Height - 3); //	Draw inner black rectangle
        }


        /// <summary>
        ///     Evaluates the DrawStyle of the control and calls the appropriate
        ///     drawing function for content
        /// </summary>
        private void DrawContent() {
            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    Draw_Style_Hue();
                    break;
                case eDrawStyle.Saturation:
                    Draw_Style_Saturation();
                    break;
                case eDrawStyle.Brightness:
                    Draw_Style_Luminance();
                    break;
                case eDrawStyle.Red:
                    Draw_Style_Red();
                    break;
                case eDrawStyle.Green:
                    Draw_Style_Green();
                    break;
                case eDrawStyle.Blue:
                    Draw_Style_Blue();
                    break;
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Hue value.
        /// </summary>
        private void Draw_Style_Hue() {
            // UL = Selected Hue, no saturation
            // UR = Selected Hue, 100% saturation
            // LL = Selected Hue, no saturation, 0 brightness
            // LR = Selected Hue, 100% saturation, 0 brightness

            Graphics g = CreateGraphics();

            var hsl_start = new ColorManager.HSL();
            var hsl_end = new ColorManager.HSL();
            hsl_start.H = _hsl.H;
            hsl_end.H = _hsl.H;
            hsl_start.S = 0.0;
            hsl_end.S = 1.0;


            for (int i = 0; i < Height - 4; i++) {
                //	Calculate luminance at this line (Hue and Saturation are constant)
                hsl_start.L = 1.0 - (double) i / (Height - 4);
                hsl_end.L = hsl_start.L;

                var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), ColorManager.HSL_to_RGB(hsl_start),
                    ColorManager.HSL_to_RGB(hsl_end), 0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Saturation value.
        /// </summary>
        private void Draw_Style_Saturation() {
            Graphics g = CreateGraphics();

            var hsl_start = new ColorManager.HSL();
            var hsl_end = new ColorManager.HSL();
            hsl_start.S = _hsl.S;
            hsl_end.S = _hsl.S;
            hsl_start.L = 1.0;
            hsl_end.L = 0.0;

            //	For each vertical line in the control:
            for (int i = 0; i < Width - 4; i++) {
                //	Calculate Hue at this line (Saturation and Luminance are constant)
                hsl_start.H = (double) i / (Width - 4);
                hsl_end.H = hsl_start.H;

                var br = new LinearGradientBrush(new Rectangle(2, 2, 1, Height - 4), ColorManager.HSL_to_RGB(hsl_start),
                    ColorManager.HSL_to_RGB(hsl_end), 90, false);
                g.FillRectangle(br, new Rectangle(i + 2, 2, 1, Height - 4));
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Luminance or Brightness value.
        /// </summary>
        private void Draw_Style_Luminance() {
            Graphics g = CreateGraphics();

            var hsl_start = new ColorManager.HSL();
            var hsl_end = new ColorManager.HSL();
            hsl_start.L = _hsl.L;
            hsl_end.L = _hsl.L;
            hsl_start.S = 1.0;
            hsl_end.S = 0.0;

            for (int i = 0; i < Width - 4; i++) //	For each vertical line in the control:
            {
                hsl_start.H = (double) i / (Width - 4); //	Calculate Hue at this line (Saturation and Luminance are constant)
                hsl_end.H = hsl_start.H;

                var br = new LinearGradientBrush(new Rectangle(2, 2, 1, Height - 4), ColorManager.HSL_to_RGB(hsl_start),
                    ColorManager.HSL_to_RGB(hsl_end), 90, false);
                g.FillRectangle(br, new Rectangle(i + 2, 2, 1, Height - 4));
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Red value.
        /// </summary>
        private void Draw_Style_Red() {
            Graphics g = CreateGraphics();

            int red = _rgb.R;

            for (int i = 0; i < Height - 4; i++) {
                //	Calculate Green at this line (Red and Blue are constant)
                int green = Round(255 - (255 * (double) i / (Height - 4)));

                var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0,
                    false);
                g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Green value.
        /// </summary>
        private void Draw_Style_Green() {
            Graphics g = CreateGraphics();

            int green = _rgb.G;

            for (int i = 0; i < Height - 4; i++) {
                //	Calculate Red at this line (Green and Blue are constant)
                int red = Round(255 - (255 * (double) i / (Height - 4)));

                var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0,
                    false);
                g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
            }
        }


        /// <summary>
        ///     Draws the content of the control filling in all color values with the provided Blue value.
        /// </summary>
        private void Draw_Style_Blue() {
            Graphics g = CreateGraphics();

            int blue = _rgb.B;
            for (int i = 0; i < Height - 4; i++) {
                //	Calculate Green at this line (Red and Blue are constant)
                int green = Round(255 - (255 * (double) i / (Height - 4)));

                var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(0, green, blue), Color.FromArgb(255, green, blue),
                    0, false);
                g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
            }
        }


        /// <summary>
        ///     Calls all the functions neccessary to redraw the entire control.
        /// </summary>
        private void Redraw_Control() {
            DrawBorder();
            DrawContent();
            DrawMarker(_markerX, _markerY, true);
        }


        /// <summary>
        ///     Resets the marker position of the slider to match the controls color.  Gives the option of redrawing the slider.
        /// </summary>
        /// <param name="redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
        private void Reset_Marker(bool redraw) {
            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    _markerX = Round((Width - 4) * _hsl.S);
                    _markerY = Round((Height - 4) * (1.0 - _hsl.L));
                    break;
                case eDrawStyle.Saturation:
                    _markerX = Round((Width - 4) * _hsl.H);
                    _markerY = Round((Height - 4) * (1.0 - _hsl.L));
                    break;
                case eDrawStyle.Brightness:
                    _markerX = Round((Width - 4) * _hsl.H);
                    _markerY = Round((Height - 4) * (1.0 - _hsl.S));
                    break;
                case eDrawStyle.Red:
                    _markerX = Round((Width - 4) * (double) _rgb.B / 255);
                    _markerY = Round((Height - 4) * (1.0 - (double) _rgb.G / 255));
                    break;
                case eDrawStyle.Green:
                    _markerX = Round((Width - 4) * (double) _rgb.B / 255);
                    _markerY = Round((Height - 4) * (1.0 - (double) _rgb.R / 255));
                    break;
                case eDrawStyle.Blue:
                    _markerX = Round((Width - 4) * (double) _rgb.R / 255);
                    _markerY = Round((Height - 4) * (1.0 - (double) _rgb.G / 255));
                    break;
            }

            if (redraw) {
                DrawMarker(_markerX, _markerY, true);
            }
        }


        /// <summary>
        ///     Resets the controls color (both HSL and RGB variables) based on the current marker position
        /// </summary>
        private void ResetHSLRGB() {
            int red, green, blue;

            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    _hsl.S = (double) _markerX / (Width - 4);
                    _hsl.L = 1.0 - (double) _markerY / (Height - 4);
                    _rgb = ColorManager.HSL_to_RGB(_hsl);
                    break;
                case eDrawStyle.Saturation:
                    _hsl.H = (double) _markerX / (Width - 4);
                    _hsl.L = 1.0 - (double) _markerY / (Height - 4);
                    _rgb = ColorManager.HSL_to_RGB(_hsl);
                    break;
                case eDrawStyle.Brightness:
                    _hsl.H = (double) _markerX / (Width - 4);
                    _hsl.S = 1.0 - (double) _markerY / (Height - 4);
                    _rgb = ColorManager.HSL_to_RGB(_hsl);
                    break;
                case eDrawStyle.Red:
                    blue = Round(255 * (double) _markerX / (Width - 4));
                    green = Round(255 * (1.0 - (double) _markerY / (Height - 4)));
                    _rgb = Color.FromArgb(_rgb.R, green, blue);
                    _hsl = ColorManager.RGB_to_HSL(_rgb);
                    break;
                case eDrawStyle.Green:
                    blue = Round(255 * (double) _markerX / (Width - 4));
                    red = Round(255 * (1.0 - (double) _markerY / (Height - 4)));
                    _rgb = Color.FromArgb(red, _rgb.G, blue);
                    _hsl = ColorManager.RGB_to_HSL(_rgb);
                    break;
                case eDrawStyle.Blue:
                    red = Round(255 * (double) _markerX / (Width - 4));
                    green = Round(255 * (1.0 - (double) _markerY / (Height - 4)));
                    _rgb = Color.FromArgb(red, green, _rgb.B);
                    _hsl = ColorManager.RGB_to_HSL(_rgb);
                    break;
            }
        }


        /// <summary>
        ///     Kindof self explanitory, I really need to look up the .NET function that does this.
        /// </summary>
        /// <param name="val">double value to be rounded to an integer</param>
        /// <returns></returns>
        private int Round(double val) {
            var ret_val = (int) val;

            var temp = (int) (val * 100);

            if ((temp % 100) >= 50) {
                ret_val += 1;
            }

            return ret_val;
        }


        /// <summary>
        ///     Returns the graphed color at the x,y position on the control
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private ColorManager.HSL GetColor(int x, int y) {
            var _hsl = new ColorManager.HSL();

            switch (_drawStyle) {
                case eDrawStyle.Hue:
                    _hsl.H = _hsl.H;
                    _hsl.S = (double) x / (Width - 4);
                    _hsl.L = 1.0 - (double) y / (Height - 4);
                    break;
                case eDrawStyle.Saturation:
                    _hsl.S = _hsl.S;
                    _hsl.H = (double) x / (Width - 4);
                    _hsl.L = 1.0 - (double) y / (Height - 4);
                    break;
                case eDrawStyle.Brightness:
                    _hsl.L = _hsl.L;
                    _hsl.H = (double) x / (Width - 4);
                    _hsl.S = 1.0 - (double) y / (Height - 4);
                    break;
                case eDrawStyle.Red:
                    _hsl =
                        ColorManager.RGB_to_HSL(Color.FromArgb(_rgb.R, Round(255 * (1.0 - (double) y / (Height - 4))),
                            Round(255 * (double) x / (Width - 4))));
                    break;
                case eDrawStyle.Green:
                    _hsl =
                        ColorManager.RGB_to_HSL(Color.FromArgb(Round(255 * (1.0 - (double) y / (Height - 4))), _rgb.G,
                            Round(255 * (double) x / (Width - 4))));
                    break;
                case eDrawStyle.Blue:
                    _hsl =
                        ColorManager.RGB_to_HSL(Color.FromArgb(Round(255 * (double) x / (Width - 4)), Round(255 * (1.0 - (double) y / (Height - 4))),
                            _rgb.B));
                    break;
            }

            return _hsl;
        }

        #endregion [ Methods ]
    }
}