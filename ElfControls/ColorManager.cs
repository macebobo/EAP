using System.Drawing;

namespace ElfControls {
    public class ColorManager {
        #region [ Constructors ]

        #endregion [ Constructors ]

        #region [ Public Static Methods ]

        /// <summary>
        ///     Sets the absolute brightness of a colour
        /// </summary>
        /// <param name="c">Original colour</param>
        /// <param name="brightness">The luminance level to impose</param>
        /// <returns>an adjusted colour</returns>
        public static Color SetBrightness(Color c, double brightness) {
            HSL hsl = RGB_to_HSL(c);
            hsl.L = brightness;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Modifies an existing brightness level
        /// </summary>
        /// <remarks>
        ///     To reduce brightness use a number smaller than 1. To increase brightness use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="brightness">The luminance delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifyBrightness(Color c, double brightness) {
            HSL hsl = RGB_to_HSL(c);
            hsl.L *= brightness;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Sets the absolute saturation level
        /// </summary>
        /// <remarks>Accepted values 0-1</remarks>
        /// <param name="c">An original colour</param>
        /// <param name="Saturation">The saturation value to impose</param>
        /// <returns>An adjusted colour</returns>
        public static Color SetSaturation(Color c, double Saturation) {
            HSL hsl = RGB_to_HSL(c);
            hsl.S = Saturation;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Modifies an existing Saturation level
        /// </summary>
        /// <remarks>
        ///     To reduce Saturation use a number smaller than 1. To increase Saturation use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="Saturation">The saturation delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifySaturation(Color c, double Saturation) {
            HSL hsl = RGB_to_HSL(c);
            hsl.S *= Saturation;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Sets the absolute Hue level
        /// </summary>
        /// <remarks>Accepted values 0-1</remarks>
        /// <param name="c">An original colour</param>
        /// <param name="Hue">The Hue value to impose</param>
        /// <returns>An adjusted colour</returns>
        public static Color SetHue(Color c, double Hue) {
            HSL hsl = RGB_to_HSL(c);
            hsl.H = Hue;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Modifies an existing Hue level
        /// </summary>
        /// <remarks>
        ///     To reduce Hue use a number smaller than 1. To increase Hue use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="Hue">The Hue delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifyHue(Color c, double Hue) {
            HSL hsl = RGB_to_HSL(c);
            hsl.H *= Hue;
            return HSL_to_RGB(hsl);
        }


        /// <summary>
        ///     Converts a colour from HSL to RGB
        /// </summary>
        /// <remarks>Adapted from the algoritm in Foley and Van-Dam</remarks>
        /// <param name="hsl">The HSL value</param>
        /// <returns>A Color structure containing the equivalent RGB values</returns>
        public static Color HSL_to_RGB(HSL hsl) {
            int Max, Mid, Min, Alpha;
            double q;

            Max = Round(hsl.L * 255);
            Min = Round((1.0 - hsl.S) * (hsl.L / 1.0) * 255);
            q = (double) (Max - Min) / 255;
            Alpha = (int) (hsl.Alpha * 255);
            Color Converted;

            if (hsl.H >= 0 && hsl.H <= (double) 1 / 6) {
                Mid = Round(((hsl.H - 0) * q) * 1530 + Min);
                Converted = Color.FromArgb(Max, Mid, Min);
            }
            else if (hsl.H <= (double) 1 / 3) {
                Mid = Round(-((hsl.H - (double) 1 / 6) * q) * 1530 + Max);
                Converted = Color.FromArgb(Mid, Max, Min);
            }
            else if (hsl.H <= 0.5) {
                Mid = Round(((hsl.H - (double) 1 / 3) * q) * 1530 + Min);
                Converted = Color.FromArgb(Min, Max, Mid);
            }
            else if (hsl.H <= (double) 2 / 3) {
                Mid = Round(-((hsl.H - 0.5) * q) * 1530 + Max);
                Converted = Color.FromArgb(Min, Mid, Max);
            }
            else if (hsl.H <= (double) 5 / 6) {
                Mid = Round(((hsl.H - (double) 2 / 3) * q) * 1530 + Min);
                Converted = Color.FromArgb(Mid, Min, Max);
            }
            else if (hsl.H <= 1.0) {
                Mid = Round(-((hsl.H - (double) 5 / 6) * q) * 1530 + Max);
                Converted = Color.FromArgb(Max, Min, Mid);
            }
            else {
                Converted = Color.FromArgb(0, 0, 0);
            }

            return Color.FromArgb(Alpha, Converted);
        }


        /// <summary>
        ///     Converts RGB to HSL
        /// </summary>
        /// <remarks>
        ///     Takes advantage of whats already built in to .NET by using the Color.GetHue, Color.GetSaturation and
        ///     Color.GetBrightness methods
        /// </remarks>
        /// <param name="c">A Color to convert</param>
        /// <returns>An HSL value</returns>
        public static HSL RGB_to_HSL(Color c) {
            var hsl = new HSL();

            int Max, Min, Diff, Sum;

            //	Of our RGB values, assign the highest value to Max, and the Smallest to Min
            if (c.R > c.G) {
                Max = c.R;
                Min = c.G;
            }
            else {
                Max = c.G;
                Min = c.R;
            }
            if (c.B > Max) {
                Max = c.B;
            }
            else if (c.B < Min) {
                Min = c.B;
            }

            Diff = Max - Min;
            Sum = Max + Min;
            hsl.Alpha = (double) c.A / 255;

            //	Luminance - a.k.a. Brightness - Adobe photoshop uses the logic that the
            //	site VBspeed regards (regarded) as too primitive = superior decides the 
            //	level of brightness.
            hsl.L = (double) Max / 255;

            //	Saturation
            if (Max == 0) {
                hsl.S = 0; //	Protecting from the impossible operation of division by zero.
            }
            else {
                hsl.S = (double) Diff / Max; //	The logic of Adobe Photoshops is this simple.
            }

            //	Hue		R is situated at the angel of 360 eller noll degrees; 
            //			G vid 120 degrees
            //			B vid 240 degrees
            double q;
            if (Diff == 0) {
                q = 0; // Protecting from the impossible operation of division by zero.
            }
            else {
                q = (double) 60 / Diff;
            }

            if (Max == c.R) {
                if (c.G < c.B) {
                    hsl.H = (360 + q * (c.G - c.B)) / 360;
                }
                else {
                    hsl.H = q * (c.G - c.B) / 360;
                }
            }
            else if (Max == c.G) {
                hsl.H = (120 + q * (c.B - c.R)) / 360;
            }
            else if (Max == c.B) {
                hsl.H = (240 + q * (c.R - c.G)) / 360;
            }
            else {
                hsl.H = 0.0;
            }

            return hsl;
        }


        /// <summary>
        ///     Converts RGB to CMYK
        /// </summary>
        /// <param name="c">A color to convert.</param>
        /// <returns>A CMYK object</returns>
        public static CMYK RGB_to_CMYK(Color c) {
            var Cmyk = new CMYK();
            double low = 1.0;
            Cmyk.Alpha = (double) c.A / 255;

            Cmyk.C = (double) (255 - c.R) / 255;
            if (low > Cmyk.C) {
                low = Cmyk.C;
            }

            Cmyk.M = (double) (255 - c.G) / 255;
            if (low > Cmyk.M) {
                low = Cmyk.M;
            }

            Cmyk.Y = (double) (255 - c.B) / 255;
            if (low > Cmyk.Y) {
                low = Cmyk.Y;
            }

            if (low > 0.0) {
                Cmyk.K = low;
            }

            return Cmyk;
        }


        /// <summary>
        ///     Converts CMYK to RGB
        /// </summary>
        /// <param name="cmyk">A color to convert</param>
        /// <returns>A Color object</returns>
        public static Color CMYK_to_RGB(CMYK cmyk) {
            int red, green, blue;
            var Alpha = (int) (cmyk.Alpha * 255);

            red = Round(255 - (255 * cmyk.C));
            green = Round(255 - (255 * cmyk.M));
            blue = Round(255 - (255 * cmyk.Y));

            return Color.FromArgb(Alpha, red, green, blue);
        }


        /// <summary>
        ///     Custom rounding function.
        /// </summary>
        /// <param name="val">Value to round</param>
        /// <returns>Rounded value</returns>
        private static int Round(double val) {
            var ret_val = (int) val;

            var temp = (int) (val * 100);

            if ((temp % 100) >= 50) {
                ret_val += 1;
            }

            return ret_val;
        }

        #endregion [ Public Static Methods ]

        #region [ class HSL ]

        public class HSL {
            #region [ Private Variables ]

            private double _alpha;
            private double _h;
            private double _l;
            private double _s;

            #endregion [ Private Variables ]

            #region [ Properties ]

            public double Alpha {
                get { return _alpha; }
                set {
                    _alpha = value;
                    _alpha = _alpha > 1 ? 1 : _alpha < 0 ? 0 : _alpha;
                }
            }

            /// <summary>
            ///     The "attribute of a visual sensation according to which an area appears to be similar to one of the perceived
            ///     colors: red, yellow, green, and blue, or to a combination of two of them".
            /// </summary>
            public double Hue {
                get { return H; }
                set { H = value; }
            }

            /// <summary>
            ///     Hue. The "attribute of a visual sensation according to which an area appears to be similar to one of the perceived
            ///     colors: red, yellow, green, and blue, or to a combination of two of them".
            /// </summary>
            public double H {
                get { return _h; }
                set {
                    _h = value;
                    _h = _h > 1 ? 1 : _h < 0 ? 0 : _h;
                }
            }

            /// <summary>
            ///     The "colorfulness of a stimulus relative to its own brightness".
            /// </summary>
            public double Saturation {
                get { return S; }
                set { S = value; }
            }

            /// <summary>
            ///     Saturation. The "colorfulness of a stimulus relative to its own brightness".
            /// </summary>
            public double S {
                get { return _s; }
                set {
                    _s = value;
                    _s = _s > 1 ? 1 : _s < 0 ? 0 : _s;
                }
            }

            /// <summary>
            ///     The radiance weighted by the effect of each wavelength on a typical human observer, measured in candela per square
            ///     meter (cd/m2).
            ///     Often the term luminance is used for the relative luminance, Y/Yn, where Yn is the luminance of the reference white
            ///     point.
            /// </summary>
            public double Luminance {
                get { return L; }
                set { L = value; }
            }

            /// <summary>
            ///     Luminance. The radiance weighted by the effect of each wavelength on a typical human observer, measured in candela
            ///     per square meter (cd/m2).
            ///     Often the term luminance is used for the relative luminance, Y/Yn, where Yn is the luminance of the reference white
            ///     point.
            /// </summary>
            public double L {
                get { return _l; }
                set {
                    _l = value;
                    _l = _l > 1 ? 1 : _l < 0 ? 0 : _l;
                }
            }

            #endregion [ Properties ]

            #region [ Constructors ]

            public HSL() {
                _h = 0;
                _s = 0;
                _l = 0;
                _alpha = 1;
            }


            /// <summary>
            ///     Create a "pure" color version of this color.
            /// </summary>
            /// <param name="hue"></param>
            public HSL(double hue) : this() {
                _h = hue;
                _s = 1;
                _l = 1;
            }


            public HSL(double hue, double saturation, double luminosity) : this(hue) {
                _s = saturation;
                _l = luminosity;
            }


            public HSL(double hue, double saturation, double luminosity, double alpha) : this(hue, saturation, luminosity) {
                _alpha = alpha;
            }


            public HSL(HSL colorToCopy) {
                _alpha = colorToCopy.Alpha;
                _h = colorToCopy.H;
                _s = colorToCopy.S;
                _l = colorToCopy.L;
            }


            public HSL(Color rgb) {
                HSL colorToCopy = RGB_to_HSL(rgb);
                _alpha = colorToCopy.Alpha;
                _h = colorToCopy.H;
                _s = colorToCopy.S;
                _l = colorToCopy.L;
            }

            #endregion [ Constructors ]

            #region [ Methods ]

            public override string ToString() {
                return "A: " + (_alpha * 255) + " H: " + (_h * 360) + " S: " + (_s * 100) + " L: " + (_l * 100);
            }

            #endregion [ Methods ]
        }

        #endregion [ class HSL ]

        #region [ class CMYK ]

        public class CMYK {
            #region [ Private Variables ]

            private double _alpha;
            private double _c;
            private double _k;
            private double _m;
            private double _y;

            #endregion [ Private Variables ]

            #region [ Properties ]

            public double Alpha {
                get { return _alpha; }
                set {
                    _alpha = value;
                    _alpha = _alpha > 1 ? 1 : _alpha < 0 ? 0 : _alpha;
                }
            }

            public double C {
                get { return _c; }
                set {
                    _c = value;
                    _c = _c > 1 ? 1 : _c < 0 ? 0 : _c;
                }
            }

            public double M {
                get { return _m; }
                set {
                    _m = value;
                    _m = _m > 1 ? 1 : _m < 0 ? 0 : _m;
                }
            }

            public double Y {
                get { return _y; }
                set {
                    _y = value;
                    _y = _y > 1 ? 1 : _y < 0 ? 0 : _y;
                }
            }

            public double K {
                get { return _k; }
                set {
                    _k = value;
                    _k = _k > 1 ? 1 : _k < 0 ? 0 : _k;
                }
            }

            #endregion [ Properties ]

            #region [ Constructors ]

            public CMYK() {
                _c = 0;
                _m = 0;
                _y = 0;
                _k = 0;
                _alpha = 1;
            }


            public CMYK(double cyan, double magenta, double yellow, double black) : this() {
                _c = cyan;
                _m = magenta;
                _y = yellow;
                _k = black;
            }


            public CMYK(double cyan, double magenta, double yellow, double black, double alpha) : this(cyan, magenta, yellow, black) {
                _alpha = alpha;
            }

            #endregion [ Constructors ]

            #region [ Methods ]

            public override string ToString() {
                return "A: " + (_alpha * 255) + " C: " + (_c * 255) + " M: " + (_m * 255) + " Y: " + (_y * 255) + " K: " + (_k * 255);
            }

            #endregion [ Methods ]
        }

        #endregion [ class CMYK ]
    }
}