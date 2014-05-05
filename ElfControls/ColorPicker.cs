using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace ElfControls {
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorPicker.bmp")]
    public partial class ColorPicker : UserControl {
        #region [ Enums ]

        public enum eDrawStyle {
            Hue,
            Saturation,
            Brightness
        }

        #endregion [ Enums ]

        #region [ Private Variables ]

        private ColorManager.CMYK _cmyk;
        private Color _color = Color.Red;
        private bool _colorEvent;
        private ColorManager.HSL _hsl;
        private bool _isUpdating;
        private bool _showKnownNames = true;

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
            get { return new NamedColor(Color, ColorName); }
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
            get { return colorWheel1.RedOffset; }
            set { colorWheel1.RedOffset = value; }
        }

        /// <summary>
        ///     Override custom name with known name of the color, if any.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Override custom name with known name of the color, if any.")]
        public bool ShowKnownNames {
            get { return _showKnownNames; }
            set {
                _showKnownNames = value;
                UpdateUI(colorWheel1.Color);
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorPicker() {
            InitializeComponent();
            Color = Color.Red;
        }


        public ColorPicker(Color color) : this() {
            Color = color;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Determines if the color passed in is in the KnownColor enumeration based on its RGB value. If so, returns the name,
        ///     else returns an empty string.
        /// </summary>
        /// <param name="color">Color to check.</param>
        private string GetKnownColorName(Color color) {
            Array colorsArray = Enum.GetValues(typeof (KnownColor));
            var allColors = new KnownColor[colorsArray.Length];
            Array.Copy(colorsArray, allColors, colorsArray.Length);
            List<string> SystemEnvironmentColors = GetSystemColorNames();
            string SystemName = string.Empty;
            string Name = string.Empty;
            string KnownName = string.Empty;

            for (int i = allColors.Length - 1; i >= 0; i--) {
                KnownName = allColors[i].ToString();
                if (Color.FromName(KnownName).ToArgb() == color.ToArgb()) {
                    Name = KnownName;
                    if (SystemEnvironmentColors.Contains(Name)) {
                        SystemName = Name;
                    }
                    else {
                        return Name;
                    }
                }
            }
            return SystemName;
        }


        /// <summary>
        ///     Generates a list of all the names of the System.Drawing.SystemColor members.
        /// </summary>
        /// <returns>List of SystemColor color names.</returns>
        private List<string> GetSystemColorNames() {
            var SystemEnvironmentColors = new List<string>();
            foreach (MemberInfo member in (typeof (SystemColors)).GetProperties()) {
                SystemEnvironmentColors.Add(member.Name);
            }
            return SystemEnvironmentColors;
        }


        /// <summary>
        ///     Convert the hex data into RGB data.
        /// </summary>
        /// <param name="hex_data">Hex string to parse</param>
        private Color ParseHexData(string hex_data) {
            if (hex_data.Length != 6) {
                return Color.Black;
            }

            string r_text, g_text, b_text;
            int r, g, b;

            r_text = hex_data.Substring(0, 2);
            g_text = hex_data.Substring(2, 2);
            b_text = hex_data.Substring(4, 2);

            r = int.Parse(r_text, NumberStyles.HexNumber);
            g = int.Parse(g_text, NumberStyles.HexNumber);
            b = int.Parse(b_text, NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }


        private int Round(double val) {
            var ret_val = (int) val;
            var temp = (int) (val * 100);
            if ((temp % 100) >= 50) {
                ret_val += 1;
            }
            return ret_val;
        }


        /// <summary>
        ///     Updates the controls with the value from the color.
        /// </summary>
        /// <param name="color">Color data to parse and display</param>
        private void UpdateUI(Color color) {
            _isUpdating = true;
            _color = color;
            _hsl = ColorManager.RGB_to_HSL(_color);
            _cmyk = ColorManager.RGB_to_CMYK(_color);

            if (!_colorEvent) {
                colorWheel1.Color = _color;
            }

            txtHue.Text = Round(colorWheel1.Hue * 360).ToString();
            txtSat.Text = Round(colorWheel1.Saturation * 100).ToString();
            txtLuminance.Text = Round(colorWheel1.Luminance * 100).ToString();

            txtRed.Text = colorWheel1.Red.ToString();
            txtGreen.Text = colorWheel1.Green.ToString();
            txtBlue.Text = colorWheel1.Blue.ToString();

            pnlPendingColor.Color = _color;
            pnlPendingColor.Update();

            WriteHexData(_color);

            if (_showKnownNames) {
                txtName.Text = GetKnownColorName(_color);
            }

            _isUpdating = false;
        }


        private void WriteHexData(Color rgb) {
            string red = Convert.ToString(rgb.R, 16);
            if (red.Length < 2) {
                red = "0" + red;
            }
            string green = Convert.ToString(rgb.G, 16);
            if (green.Length < 2) {
                green = "0" + green;
            }
            string blue = Convert.ToString(rgb.B, 16);
            if (blue.Length < 2) {
                blue = "0" + blue;
            }

            txtHex.Text = "#" + red.ToLower() + green.ToLower() + blue.ToLower();
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
        ///     Only allow the character 0 through 9 in this textbox.
        /// </summary>
        private void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar != '\b') & (e.KeyChar != '-')) {
                e.Handled = ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)));
            }
        }


        /// <summary>
        ///     Only allow the character 0 through 9, A through F and the # sign in this textbox.
        /// </summary>
        private void HexOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != '\b') {
                if (Uri.IsHexDigit(e.KeyChar) || (e.KeyChar == '#')) {
                    e.Handled = false; // We like these, do nothing
                }
                else {
                    e.Handled = true;
                }
            }
        }


        private void colorWheel1_ColorChanged(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }

            _colorEvent = true;
            UpdateUI(colorWheel1.Color);
            _colorEvent = false;
        }


        private void lblOriginalColor_Click(object sender, EventArgs e) {
            Color = lblOriginalColor.BackColor;
        }


        private void TextBox_Click(object sender, EventArgs e) {
            ((TextBox) sender).SelectAll();
        }


        private void txtHex_Leave(object sender, EventArgs e) {
            string Hex = txtHex.Text.Replace("#", string.Empty);
            if (Hex.Length > 6) {
                WriteHexData(_color);
                txtHex.SelectAll();
                txtHex.Focus();
                return;
            }
            UpdateUI(ParseHexData(Hex));
        }


        private void txtHue_Leave(object sender, EventArgs e) {
            if (txtHue.TextLength == 0) {
                txtHue.Focus();
                return;
            }

            int Hue = int.Parse(txtHue.Text) % 360;
            if (Hue < 0) {
                Hue += 360;
            }

            _hsl.Hue = Hue / 360.0;
            colorWheel1.Hue = _hsl.H;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtSat_Leave(object sender, EventArgs e) {
            if (txtSat.TextLength == 0) {
                txtSat.Focus();
                return;
            }

            int Saturation = int.Parse(txtSat.Text);
            _hsl.Saturation = (double) Saturation / 100;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtLuminance_Leave(object sender, EventArgs e) {
            if (txtLuminance.TextLength == 0) {
                txtLuminance.Focus();
                return;
            }

            int Luminance = int.Parse(txtLuminance.Text);
            _hsl.Luminance = (double) Luminance / 100;
            UpdateUI(ColorManager.HSL_to_RGB(_hsl));
        }


        private void txtRed_Leave(object sender, EventArgs e) {
            if (txtRed.TextLength == 0) {
                txtRed.Focus();
                return;
            }

            int Red = int.Parse(txtRed.Text);
            if (Red > 255) {
                Red = 255;
            }
            colorWheel1.Red = (byte) Red;
            UpdateUI(colorWheel1.Color);
        }


        private void txtGreen_Leave(object sender, EventArgs e) {
            if (txtGreen.TextLength == 0) {
                txtGreen.Focus();
                return;
            }

            int Green = int.Parse(txtGreen.Text);
            if (Green > 255) {
                Green = 255;
            }
            colorWheel1.Green = (byte) Green;
            UpdateUI(colorWheel1.Color);
        }


        private void txtBlue_Leave(object sender, EventArgs e) {
            if (txtBlue.TextLength == 0) {
                txtBlue.Focus();
                return;
            }

            int Blue = int.Parse(txtBlue.Text);
            if (Blue > 255) {
                Blue = 255;
            }
            colorWheel1.Blue = (byte) Blue;
            UpdateUI(colorWheel1.Color);
        }


        /// <summary>
        ///     Check the value of the textbox against the max. value set in the control's tag. If the tag is not set, exit.
        ///     If the tag is not set to a number, throw an error. If the textbox value is blank, set it to 0.
        ///     If the textbox value is less than 0, set it to 0. If the textbox value is greater than the max value, set
        ///     the textbox to the max value.
        /// </summary>
        private void ValueRangeCheck_TextChanged(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }

            var tb = (TextBox) sender;
            string MaxString = tb.Tag as string ?? string.Empty;
            if (MaxString.Length == 0) {
                return;
            }
            string Value = tb.Text;
            if (Value.Length == 0) {
                return;
            }

            int MaxVal = 0;
            int TBVal = 0;
            if (!Int32.TryParse(MaxString, out MaxVal)) {
                throw new ArgumentOutOfRangeException("Tag for " + tb.Name + " has not been properly set.");
            }
            if (!Int32.TryParse(Value, out TBVal)) {
                tb.Text = "0";
                tb.SelectAll();
                tb.Focus();
                return;
            }

            if ((TBVal < 0) & (tb != txtHue)) {
                tb.Text = "0";
                tb.SelectAll();
                tb.Focus();
                return;
            }
            if (TBVal > MaxVal) {
                tb.Text = MaxString;
                tb.SelectAll();
                tb.Focus();
            }
        }
    }
}