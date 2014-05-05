using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElfControls {
    public partial class SelectColorForm : Form {
        private bool _addButtonEventAttached;
        private bool _showPalette;

        #region [ Properties ]

        public Color Color {
            get { return ColorPicker.Color; }
            set {
                //ColorPicker.Color = value;
                cgPalette.Color = value;
            }
        }

        public string ColorName {
            get { return ColorPicker.ColorName; }
            set { ColorPicker.ColorName = value; }
        }

        public Palette CustomColors {
            get { return cgPalette.CustomColors; }
            set { cgPalette.CustomColors = value; }
        }

        public Image OKButton_Image {
            get { return cmdOk.Image; }
            set {
                if (value != null) {
                    cmdOk.Image = value;
                }
            }
        }

        public Image CancelButton_Image {
            get { return cmdCancel.Image; }
            set {
                if (value != null) {
                    cmdCancel.Image = value;
                }
            }
        }

        public bool ShowPalette {
            get { return _showPalette; }
            set {
                _showPalette = value;
                SuspendLayout();
                if (_showPalette) {
                    Size = new Size(420, 400);
                    ColorPicker.Location = new Point(188, 5);
                    cgPalette.Visible = true;
                    cgPalette.SuspendNativeAddButtonClick = true;
                    if (!_addButtonEventAttached) {
                        _addButtonEventAttached = true;
                        cgPalette.AddButtonClick += cgPalette_AddButtonClick;
                    }
                }
                else {
                    cgPalette.Visible = false;
                    cgPalette.SuspendNativeAddButtonClick = false;
                    Size = new Size(237, 400);
                    ColorPicker.Location = new Point(7, 5);
                }
                int ButtonWidth = cmdOk.Width + cmdCancel.Width + 8;
                cmdOk.Left = (ClientRectangle.Width - ButtonWidth) / 2;
                cmdCancel.Left = cmdOk.Left + cmdOk.Width + 8;
                ResumeLayout(false);
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public SelectColorForm() {
            InitializeComponent();
            ShowPalette = false;
        }


        public SelectColorForm(Color color) : this() {
            Color = color;
        }


        public SelectColorForm(NamedColor namedColor) : this(namedColor.Color) {
            ColorName = namedColor.Name;
        }

        #endregion [ Constructors ]

        #region [ Events ]

        private void cgPalette_SelectedIndexChange(object sender, EventArgs e) {
            if (cgPalette.NamedColor != null) {
                ColorPicker.NamedColor = cgPalette.NamedColor;
            }
            else {
                ColorPicker.Color = cgPalette.Color;
            }
        }


        private void cgPalette_AddButtonClick(object sender, EventArgs e) {
            cgPalette.AddColor(ColorPicker.Color, ColorPicker.ColorName);
        }

        #endregion [ Events ]
    }
}