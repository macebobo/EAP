using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ElfControls {
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorPicker.bmp")]
    public partial class ColorPicker : UserControl {

        #region [ Private Variables ]

        private Color _color = Color.Red;
        private bool _colorEvent;
        private ColorManager.HSL _hsl;
        private bool _isInternalUpdate;

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Currently selected color.
        /// </summary>
        [DefaultValue(typeof (Color), "Red")]
        [Description("Selected Color.")]
        public Color Color {
            get { return _color; }
            set {
                _color = value;
                lblOriginalColor.BackColor = value;
                UpdateUI(value);
            }
        }

        [Description("Name of the custom color.")]
        public string ColorName {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public NamedColor NamedColor {
            set {
                if (value == null) {
                    return;
                }
                Color = value.Color;
                ColorName = value.Name;
            }
        }

        [DefaultValue(typeof (int), "-90")]
        [Description("Degree offset for the color Red.")]
        public double RedOffset {
            set { colorWheel1.RedOffset = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorPicker() {
            InitializeComponent();
            Color = Color.Red;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Determines if the color passed in is in the KnownColor enumeration based on its RGB value. If so, returns the name,
        ///     else returns an empty string.
        /// </summary>
        /// <param name="color">Color to check.</param>
        private string GetKnownColorName(Color color) {
            var colorsArray = Enum.GetValues(typeof (KnownColor));
            var allColors = new KnownColor[colorsArray.Length];
            Array.Copy(colorsArray, allColors, colorsArray.Length);
            var systemEnvironmentColors = GetSystemColorNames();
            var systemName = string.Empty;

            for (var i = allColors.Length - 1; i >= 0; i--) {
                var knownName = allColors[i].ToString();

                if (Color.FromName(knownName).ToArgb() != color.ToArgb()) {
                    continue;
                }

                var colorName = knownName;

                if (systemEnvironmentColors.Contains(colorName)) {
                    systemName = colorName;
                }
                else {
                    return colorName;
                }
            }
            return systemName;
        }


        /// <summary>
        ///     Generates a list of all the names of the System.Drawing.SystemColor members.
        /// </summary>
        /// <returns>List of SystemColor color names.</returns>
        private static List<string> GetSystemColorNames() {
            var systemEnvironmentColors = new List<string>();
            foreach (var member in (typeof (SystemColors)).GetProperties()) {
                systemEnvironmentColors.Add(member.Name);
            }
            return systemEnvironmentColors;
        }


        /// <summary>
        ///     Convert the hex data into RGB data.
        /// </summary>
        /// <param name="hexData">Hex string to parse</param>
        private static Color ParseHexData(string hexData) {
            return hexData.Length == 6 ? Color.FromArgb(int.Parse(hexData, NumberStyles.HexNumber)) : Color.Black;
        }


        /// <summary>
        ///     Updates the controls with the value from the color.
        /// </summary>
        /// <param name="color">Color data to parse and display</param>
        private void UpdateUI(Color color) {
            _isInternalUpdate = true;
            _color = color;
            _hsl = ColorManager.RGB_to_HSL(_color);
            ColorManager.RGB_to_CMYK(_color);

            if (!_colorEvent) {
                colorWheel1.Color = _color;
            }

            txtHue.Text = Math.Round(colorWheel1.Hue * 360f, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
            txtSat.Text = Math.Round(colorWheel1.Saturation * 100f, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
            txtLuminance.Text = Math.Round(colorWheel1.Luminance * 100f, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);

            txtRed.Text = colorWheel1.Red.ToString(CultureInfo.InvariantCulture);
            txtGreen.Text = colorWheel1.Green.ToString(CultureInfo.InvariantCulture);
            txtBlue.Text = colorWheel1.Blue.ToString(CultureInfo.InvariantCulture);

            pnlPendingColor.Color = _color;
            pnlPendingColor.Update();

            WriteHexData(_color);

            txtName.Text = GetKnownColorName(_color);

            _isInternalUpdate = false;
        }


        private void WriteHexData(Color rgb) {
            txtHex.Text = @"#" + (rgb.R.ToString("X2") + rgb.G.ToString("X2") + rgb.B.ToString("X2")).ToLower();
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Only allow the character 0 through 9 in this textbox.
        /// </summary>
        private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != '\b') {
                e.Handled = ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)));
            }
        }


        /// <summary>
        ///     Only allow the character 0 through 9, A through F and the # sign in this textbox.
        /// </summary>
        private void HexOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\b') {
                return;
            }

            e.Handled = !(Uri.IsHexDigit(e.KeyChar) || (e.KeyChar == '#'));
        }


        private void colorWheel1_ColorChanged(object sender, EventArgs e) {
            if (_isInternalUpdate) {
                return;
            }

            _colorEvent = true;
            UpdateUI(colorWheel1.Color);
            _colorEvent = false;
        }


        private void txtHex_Leave(object sender, EventArgs e) {
            var hex = txtHex.Text.Replace("#", string.Empty);
            if (hex.Length > 6) {
                WriteHexData(_color);
                txtHex.SelectAll();
                txtHex.Focus();
                return;
            }
            UpdateUI(ParseHexData(hex));
        }


        private void txtHue_Leave(object sender, EventArgs e) {
            if (txtHue.TextLength == 0) {
                txtHue.Focus();
                return;
            }

            var hue = int.Parse(txtHue.Text) % 360;
            if (hue < 0) {
                hue += 360;
            }

            _hsl.Hue = hue / 360.0;
            colorWheel1.Hue = _hsl.H;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtSat_Leave(object sender, EventArgs e) {
            if (txtSat.TextLength == 0) {
                txtSat.Focus();
                return;
            }

            var saturation = int.Parse(txtSat.Text);
            _hsl.Saturation = (double) saturation / 100;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtLuminance_Leave(object sender, EventArgs e) {
            if (txtLuminance.TextLength == 0) {
                txtLuminance.Focus();
                return;
            }

            var luminance = int.Parse(txtLuminance.Text);
            _hsl.Luminance = (double) luminance / 100;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtRed_Leave(object sender, EventArgs e) {
            if (txtRed.TextLength == 0) {
                txtRed.Focus();
                return;
            }

            var red = int.Parse(txtRed.Text);
            if (red > 255) {
                red = 255;
            }
            colorWheel1.Red = (byte) red;
            UpdateUI(colorWheel1.Color);
        }


        private void txtGreen_Leave(object sender, EventArgs e) {
            if (txtGreen.TextLength == 0) {
                txtGreen.Focus();
                return;
            }

            var green = int.Parse(txtGreen.Text);
            if (green > 255) {
                green = 255;
            }
            colorWheel1.Green = (byte) green;
            UpdateUI(colorWheel1.Color);
        }


        private void txtBlue_Leave(object sender, EventArgs e) {
            if (txtBlue.TextLength == 0) {
                txtBlue.Focus();
                return;
            }

            var blue = int.Parse(txtBlue.Text);
            if (blue > 255) {
                blue = 255;
            }
            colorWheel1.Blue = (byte) blue;
            UpdateUI(colorWheel1.Color);
        }


        /// <summary>
        ///     Check the value of the textbox against the max. value set in the control's tag. If the tag is not set, exit.
        ///     If the tag is not set to a number, throw an error. If the textbox value is blank, set it to 0.
        ///     If the textbox value is less than 0, set it to 0. If the textbox value is greater than the max value, set
        ///     the textbox to the max value.
        /// </summary>
        private void ValueRangeCheck_TextChanged(object sender, EventArgs e) {
            if (_isInternalUpdate) {
                return;
            }

            var tb = (TextBox) sender;
            var maxString = tb.Tag as string ?? string.Empty;
            if (maxString.Length == 0) {
                return;
            }

            var value = tb.Text;
            if (value.Length == 0) {
                return;
            }

            int maxVal;
            if (!Int32.TryParse(maxString, out maxVal)) {
                throw new ArgumentOutOfRangeException("Tag for " + tb.Name + " has not been properly set.");
            }

            int tbVal;
            if (!Int32.TryParse(value, out tbVal)) {
                tb.Text = @"0";
                tb.SelectAll();
                tb.Focus();
                return;
            }

            if ((tbVal < 0) & (tb != txtHue)) {
                tb.Text = @"0";
                tb.SelectAll();
                tb.Focus();
                return;
            }

            if (tbVal <= maxVal) {
                return;
            }

            tb.Text = maxString;
            tb.SelectAll();
            tb.Focus();
        }
    }
}
