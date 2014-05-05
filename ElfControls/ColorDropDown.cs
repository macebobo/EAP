using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ElfControls.Properties;

namespace ElfControls {
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorDropdown.bmp")]
    public partial class ColorDropDown : DropDownControl {
        #region [ Private Variables ]

        private Image _cancelButtonImage;
        private Color _color = Color.White;
        private Size _dropdownSize = new Size(185, 300);
        private bool _embiggenHotTracked;
        private bool _hotTracking;
        private bool _isUpdating = true;
        private bool _knownColorsInList = true;
        private Size _listGridSize = new Size(16, 16);
        private byte _listPadding = 4;
        private Image _noColorImage;
        private Image _okButtonImage;
        private byte _paletteGridPadding = 4;
        private Size _paletteGridSize = new Size(16, 16);
        private bool _showRGBOnToolTip;
        private bool _showSystemColors = true;
        private bool _systemColorsInList = true;

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Indicates whether the "No Color" button should be shown in the dropdown.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the \"No Color\" button should be shown in the dropdown.")]
        public bool AllowEmptyColor {
            get { return PaletteColorGrid.AllowEmptyColor; }
            set { PaletteColorGrid.AllowEmptyColor = value; }
        }

        /// <summary>
        ///     Indicates whether the "add colors" button should show
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the \"add colors\" button should show.")]
        public bool AllowAddColors {
            get { return PaletteColorGrid.AllowAddColors; }
            set { PaletteColorGrid.AllowAddColors = value; }
        }

        [DefaultValue(typeof (BorderStyle), "None")]
        [Description("Indicates whether the control should have a border.")]
        public new BorderStyle BorderStyle {
            get { return base.BorderStyle; }
            set {
                base.BorderStyle = value;
                SetColorPanelBorder();
                Refresh();
            }
        }

        /// <summary>
        ///     Indicates whether the System Colors tabs should be shown in the dropdown.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the System Colors tabs should be shown in the dropdown.")]
        public bool DisplaySystemColorPalette {
            get { return _showSystemColors; }
            set {
                _showSystemColors = value;
                if (value && !tabsColors.TabPages.Contains(tabSystemColors)) {
                    tabsColors.TabPages.Add(tabSystemColors);
                }

                else if (!value && tabsColors.TabPages.Contains(tabSystemColors)) {
                    tabsColors.TabPages.Remove(tabSystemColors);
                }
            }
        }

        [Description("Custom image to use for the Cancel button on the Select Color dialog.")]
        public Image CancelButtonImage {
            get { return _cancelButtonImage; }
            set {
                _cancelButtonImage = value;
                PaletteColorGrid.CancelButtonImage = value;
            }
        }

        /// <summary>
        ///     Currently selected color.
        /// </summary>
        [DefaultValue(typeof (Color), "Red")]
        [Description("Currently selected color.")]
        public Color Color {
            get { return _color; }
            set {
                _isUpdating = true;
                _color = value;

                if (_color.IsEmpty) {
                    cpnlDisplayColor.Color = SystemColors.Control;
                    pctNoColor.Visible = true;
                }
                else {
                    cpnlDisplayColor.Color = _color;
                    pctNoColor.Visible = false;
                }
                PaletteColorGrid.Color = value;
                KnownColorsList.Color = value;
                SystemColorsList.Color = value;
                OnColorChanged();
                _isUpdating = false;
            }
        }

        /// <summary>
        ///     List of customize colors.
        /// </summary>
        [Description("List of custom defined colors.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Palette CustomColors {
            get { return PaletteColorGrid.CustomColors; }
            set { PaletteColorGrid.CustomColors = value; }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Indicates whether to display the hot tracked color larger than the others.")]
        public bool EmbiggenHotTracked {
            get { return _embiggenHotTracked; }
            set {
                _embiggenHotTracked = value;
                PaletteColorGrid.EmbiggenHotTracked = value;
                SystemColorsList.EmbiggenHotTracked = value;
                KnownColorsList.EmbiggenHotTracked = value;
            }
        }

        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                var SubFont = new Font(value.FontFamily, value.SizeInPoints - 1f);
                KnownColorsList.Font = SubFont;
                SystemColorsList.Font = SubFont;
            }
        }

        [DefaultValue(typeof (bool), "False")]
        public bool HotTracking {
            get { return _hotTracking; }
            set {
                _hotTracking = value;
                PaletteColorGrid.HotTracking = value;
                SystemColorsList.HotTracking = value;
                KnownColorsList.HotTracking = value;
            }
        }

        /// <summary>
        ///     Indicates whether the KnownColors list should be show as a list, instead of a color grid.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the KnownColors list should be show as a list, instead of a color grid.")]
        public bool KnownColorsInList {
            get { return _knownColorsInList; }
            set {
                _knownColorsInList = value;
                KnownColorsList.ShowNames = value;
            }
        }

        /// <summary>
        ///     Spacing between the color rows for the color lists.
        /// </summary>
        [DefaultValue(typeof (byte), "4")]
        [Description("Spacing between the color rows for the color lists.")]
        public byte ListGridPadding {
            get { return _listPadding; }
            set {
                _listPadding = value;
                SystemColorsList.GridPadding = value;
                KnownColorsList.GridPadding = value;
            }
        }

        /// <summary>
        ///     Size of the color grid rectangles for lists.
        /// </summary>
        [DefaultValue(typeof (Size), "16, 16")]
        [Description("Size of the color grid rectangles for lists.")]
        public Size ListGridSize {
            get { return _listGridSize; }
            set {
                _listGridSize = value;
                SystemColorsList.GridSize = _listGridSize;
                KnownColorsList.GridSize = _listGridSize;
            }
        }

        [Description("Custom image to use for the OK button on the Select Color dialog.")]
        public Image NoColorIndicatorImage {
            get { return _noColorImage; }
            set {
                _noColorImage = value;
                if (value == null) {
                    pctNoColor.Image = Resources.not_small;
                    pctNoColor.Size = new Size(10, 10);
                }
                else {
                    pctNoColor.Image = value;
                    pctNoColor.Size = value.Size;
                }
                OnResize(new EventArgs());
            }
        }

        [Description("Custom image to use for the OK button on the Select Color dialog.")]
        public Image OKButtonImage {
            get { return _okButtonImage; }
            set {
                _okButtonImage = value;
                PaletteColorGrid.OKButtonImage = value;
            }
        }

        /// <summary>
        ///     Spacing between the color grid rectangles in the palette color grid.
        /// </summary>
        [DefaultValue(typeof (byte), "4")]
        [Description("Spacing between the color grid rectangles in the palette color grid.")]
        public byte PaletteGridPadding {
            get { return _paletteGridPadding; }
            set {
                _paletteGridPadding = value;
                PaletteColorGrid.GridPadding = value;
            }
        }

        /// <summary>
        ///     Size of the color grid rectangles in the palette color grid.
        /// </summary>
        [DefaultValue(typeof (Size), "16, 16")]
        [Description("Size of the color grid rectangles in the palette color grid.")]
        public Size PaletteGridSize {
            get { return _paletteGridSize; }
            set {
                _paletteGridSize = value;
                PaletteColorGrid.GridSize = value;
            }
        }

        public override ComboBoxStyle DropDownStyle {
            get { return base.DropDownStyle; }
            set {
                base.DropDownStyle = value;
                SetColorPanelBorder();
                OnResize(new EventArgs());
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Display the color RGB values on the tooltip.")]
        public bool ShowRGBValuesOnToolTip {
            get { return _showRGBOnToolTip; }
            set {
                _showRGBOnToolTip = value;
                PaletteColorGrid.ShowRGBValuesOnToolTip = value;
                SystemColorsList.ShowRGBValuesOnToolTip = value;
                KnownColorsList.ShowRGBValuesOnToolTip = value;
            }
        }

        /// <summary>
        ///     Indicates whether the SystemColors list should be show as a list, instead of a color grid.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the SystemColors list should be show as a list, instead of a color grid.")]
        public bool SystemColorsInList {
            get { return _systemColorsInList; }
            set {
                _systemColorsInList = value;
                SystemColorsList.ShowNames = value;
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorDropDown() {
            _isUpdating = true;
            InitializeComponent();
            Text = string.Empty;
            InitializeDropDown(tabsColors);
            pctNoColor.Visible = false;

            PaletteColorGrid.ParentUserControl = this;
            KnownColorsList.ParentUserControl = this;
            SystemColorsList.ParentUserControl = this;
            SetColorPanelBorder();
            _isUpdating = false;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Adds a new color to the Custom Color list.
        /// </summary>
        /// <param name="color">Color to add</param>
        /// <param name="name">Name of the color</param>
        public void AddCustomColor(Color color, string name) {
            PaletteColorGrid.AddColor(color, name);
        }


        /// <summary>
        ///     Adds a new color to the Custom Color list.
        /// </summary>
        /// <param name="name">Name of the color</param>
        /// <param name="color">Color to add</param>
        public void AddCustomColor(string name, Color color) {
            PaletteColorGrid.AddColor(color, name);
        }


        /// <summary>
        ///     Adds a new color to the Custom Color list.
        /// </summary>
        /// <param name="name">Name of the color</param>
        /// <param name="color">Color to add</param>
        public void AddCustomColor(string name, byte red, byte green, byte blue) {
            PaletteColorGrid.AddColor(Color.FromArgb(red, green, blue), name);
        }


        /// <summary>
        ///     Adds a new color to the Custom Color list.
        /// </summary>
        /// <param name="color">Color to add</param>
        public void AddCustomColor(Color color) {
            AddCustomColor(color, string.Empty);
        }


        /// <summary>
        ///     Clears out all the custom colors.
        /// </summary>
        public void ClearCustomColors() {
            PaletteColorGrid.Clear();
        }


        ///// <summary>
        ///// Loads in a list of custom colors.
        ///// </summary>
        ///// <param name="colorList">List of colors.</param>
        ///// <param name="ignoreDuplicateColors">Indicates if duplicated color values in the custom list should be ignored.</param>
        //public void LoadCustomColors(List<NamedColor> colorList, bool ignoreDuplicateColors)
        //{
        //	this.PaletteColorGrid.LoadCustomColors(colorList, ignoreDuplicateColors);
        //}

        /// <summary>
        ///     Load in a predefined palette of colors into the CustomList colorgrid.
        /// </summary>
        /// <param name="palette">List of NamedColor objects.</param>
        public void LoadPalette(List<NamedColor> palette) {
            PaletteColorGrid.LoadPalette(palette);
        }


        protected override void OpenDropDown() {
            base.OpenDropDown();
            PaletteColorGrid.Focus();
        }


        private void SetColorPanelBorder() {
            if (BorderStyle == BorderStyle.None) {
                cpnlDisplayColor.BorderStyle = BorderStyle.FixedSingle;
            }
            else {
                cpnlDisplayColor.BorderStyle = BorderStyle.None;
            }
        }

        #endregion [ Methods ]

        #region [ Events ]

        #region [ Event Declarations ]

        public event EventHandler ColorChanged;

        #endregion [ Event Declarations ]

        #region [ Event Triggers ]

        protected void OnColorChanged() {
            if (ColorChanged != null) {
                ColorChanged(null, null);
            }
        }


        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            cpnlDisplayColor.Bounds = AnchorClientBounds;
            pctNoColor.Location = new Point(((AnchorClientBounds.Width - pctNoColor.Width) / 2) + AnchorClientBounds.Left,
                ((AnchorClientBounds.Height - pctNoColor.Height) / 2) + AnchorClientBounds.Top);
        }

        #endregion [ Event Triggers ]

        private void cmdSelect_Click(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }
            FreezeDropDown(false);
            var frm = new SelectColorForm(_color);
            if (frm.ShowDialog() != DialogResult.Cancel && !frm.Color.Equals(_color)) {
                Color = frm.Color;
                CloseDropDown();
                OnColorChanged();
            }
            else {
                UnFreezeDropDown();
            }
        }


        private void PaletteColorGrid_SelectedIndexChange(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }
            Color = PaletteColorGrid.Color;
            CloseDropDown();
            OnColorChanged();
        }


        private void pnlDisplayColor_Click(object sender, EventArgs e) {
            OpenDropDown();
        }


        private void pctNoColor_Click(object sender, EventArgs e) {
            OpenDropDown();
        }


        private void SystemColorsList_SelectedIndexChange(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }
            Color = SystemColorsList.Color;
            CloseDropDown();
            OnColorChanged();
        }


        private void KnownColorsList_SelectedIndexChange(object sender, EventArgs e) {
            if (_isUpdating) {
                return;
            }
            Color = KnownColorsList.Color;
            CloseDropDown();
            OnColorChanged();
        }


        private void tabsColors_SelectedIndexChanged(object sender, EventArgs e) {
            tabsColors.TabPages[tabsColors.SelectedIndex].Controls[0].Focus();
        }

        #endregion [ Events ]
    }
}