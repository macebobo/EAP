using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace ElfControls {
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorGrid.bmp")]
    public partial class ColorGrid : UserControl {
        #region [ Enums ]

        public enum ColorPalette {
            Custom,
            Default,
            KnownColors,
            SystemColors
        }

        #endregion [ Enums ]

        #region [ Private Variables ]

        private readonly Palette _baseList = new Palette();
        private readonly Palette _combinedList = new Palette();
        private bool _allowAddColors = true;
        private bool _allowEmptyColor = true;
        private Image _cancelButtonImage;
        private Color _color = Color.Red;
        private ColorPalette _colorPalette = ColorPalette.Default;

        private Palette _customList = new Palette();

        private int _customStartIndex = -1;
        private bool _embiggenHotTracked;
        private byte _gridPadding = 4;
        private Size _gridSize = new Size(16, 16);
        private bool _hotTracking;
        private int _hoverIndex = -1;
        private Color _initialColor = Color.Empty;
        private bool _initialColorSet;
        private Image _okButtonImage;
        private Control _parentUserControl;
        private int _selectedIndex = -1;
        private bool _showNames;
        private bool _showRGBOnToolTip;
        private bool _stretchToFitParent;
        private bool _suspendNativeAddButtonClick;

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Indicates whether the "add colors" button should shown.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the \"add colors\" button should shown.")]
        public bool AllowAddColors {
            get { return _allowAddColors; }
            set {
                _allowAddColors = value;
                cmdAddColor.Visible = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Indicates whether the "No Color" button should be shown in the dropdown.
        /// </summary>
        [DefaultValue(typeof (bool), "True")]
        [Description("Indicates whether the \"No Color\" button should be shown in the dropdown.")]
        public bool AllowEmptyColor {
            get { return _allowEmptyColor; }
            set {
                _allowEmptyColor = value;
                cmdEmptyColor.Visible = value;
                Refresh();
            }
        }

        [Description("Custom image to use for the Cancel button on the Select Color dialog.")]
        public Image CancelButtonImage {
            get { return _cancelButtonImage; }
            set { _cancelButtonImage = value; }
        }

        [DefaultValue(typeof (Color), "Red")]
        public Color Color {
            get { return _color; }
            set {
                _color = value;
                if (!_initialColorSet) {
                    _initialColor = value;
                    _initialColorSet = true;
                }

                SelectedIndex = IndexOf(value, true);
            }
        }

        /// <summary>
        ///     List of customized colors.
        /// </summary>
        [Description("List of customized colors.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Palette CustomColors {
            get { return _customList; }
            set {
                if (value == null) {
                    return;
                }
                _combinedList.Clear();
                _customList = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Indicates whether to display the hot tracked color larger than the others.")]
        public bool EmbiggenHotTracked {
            get { return _embiggenHotTracked; }
            set { _embiggenHotTracked = value; }
        }

        [DefaultValue(typeof (byte), "4")]
        [Description("Spacing between the color blocks.")]
        public byte GridPadding {
            get { return _gridPadding; }
            set {
                _gridPadding = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (Size), "16, 16")]
        [Description("Size of the individual color blocks.")]
        public Size GridSize {
            get { return _gridSize; }
            set {
                _gridSize = value;
                if ((_gridSize.Width >= 16) && (_gridSize.Height >= 16)) {
                    cmdAddColor.Size = _gridSize;
                    cmdEmptyColor.Size = _gridSize;
                }
                Refresh();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        public bool HotTracking {
            get { return _hotTracking; }
            set { _hotTracking = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public NamedColor NamedColor {
            get {
                if (_selectedIndex >= 0) {
                    return _combinedList[_selectedIndex];
                }
                return null;
            }
        }

        [Description("Custom image to use for the OK button on the Select Color dialog.")]
        public Image OKButtonImage {
            get { return _okButtonImage; }
            set { _okButtonImage = value; }
        }

        public new Padding Padding {
            get { return base.Padding; }
            set {
                base.Padding = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (ColorPalette), "Default")]
        [Description("Color Palette to display.")]
        public ColorPalette Palette {
            get { return _colorPalette; }
            set {
                _colorPalette = value;
                LoadPredefinedPalette();
            }
        }

        [Browsable(false)]
        public Control ParentUserControl {
            get { return _parentUserControl; }
            set { _parentUserControl = value; }
        }

        [Browsable(false)]
        public int SelectedIndex {
            get { return _selectedIndex; }
            set {
                if (_selectedIndex != value) {
                    _selectedIndex = value;
                    if ((_selectedIndex != -1) && (_combinedList.Count > 0)) {
                        _color = _combinedList[_selectedIndex].Color;
                    }
                    Refresh();
                    OnSelectedIndexChange();
                }
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Shows the colors as individual lines, with the name adjacent.")]
        public bool ShowNames {
            get { return _showNames; }
            set {
                _showNames = value;
                Refresh();
            }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Shows the RGB values of the color in the ToolTip.")]
        public bool ShowRGBValuesOnToolTip {
            get { return _showRGBOnToolTip; }
            set { _showRGBOnToolTip = value; }
        }

        [DefaultValue(typeof (bool), "False")]
        [Description("Expand the height of this control to fit its parent control if shorter.")]
        public bool StretchToFitParent {
            get { return _stretchToFitParent; }
            set {
                _stretchToFitParent = value;
                Refresh();
            }
        }

        /// <summary>
        ///     Instructs the Add button to not perform its native activity but trigger the AddButtonClick event instead.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuspendNativeAddButtonClick {
            get { return _suspendNativeAddButtonClick; }
            set { _suspendNativeAddButtonClick = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorGrid() {
            _color = Color.Red;
            _combinedList = new Palette();
            InitializeComponent();

            if (ColorExists(_color, false)) {
                SelectedIndex = IndexOf(_color, true);
            }
            else {
                SelectedIndex = 0;
            }

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Adds a new NamedColor object to the custom color list.
        /// </summary>
        /// <param name="color">NamedColor object to add.</param>
        public void AddColor(NamedColor color) {
            _combinedList.Clear();
            _customList.Add(color);
            Refresh();
        }


        /// <summary>
        ///     Adds a new color to the custom color list.
        /// </summary>
        /// <param name="color">Color value to add.</param>
        /// <param name="name">Name of the color.</param>
        public void AddColor(Color color, string name) {
            _combinedList.Clear();
            _customList.Add(new NamedColor(color, name));
            Refresh();
        }


        /// <summary>
        ///     Clears the list of colors.
        /// </summary>
        public void Clear() {
            _colorPalette = ColorPalette.Custom;
            _baseList.Clear();
            _customList.Clear();
            _combinedList.Clear();
            _customStartIndex = -1;
            Refresh();
        }


        /// <summary>
        ///     Checks the list of colors to see if this color is already present.
        /// </summary>
        /// <param name="color">Color to check.</param>
        /// <param name="inCustomList">Indicates whether to just check in the custom color list.</param>
        /// <returns>
        ///     Returns true if the color is present in the color list. If inCustomColor is true, it will only check the
        ///     custom list, if false, checks entire list. If the color is not prsent, returns false.
        /// </returns>
        public bool ColorExists(Color color, bool inCustomList) {
            int Index = IndexOf(color, true);
            if (Index == -1) {
                return false;
            }
            if (inCustomList) {
                return (Index >= _customStartIndex);
            }
            return true;
        }


        private void EstablishPalettes() {
            if (_baseList.Count == 0) {
                LoadPredefinedPalette();
            }
            if (_combinedList.Count == 0) {
                _combinedList.AddRange(_baseList);
                if (_customList.Count > 0) {
                    _customStartIndex = _combinedList.Count;
                    _combinedList.AddRange(_customList);
                }
            }
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
        ///     Determines the index of a particular color based on the position.
        /// </summary>
        /// <param name="point">Point at which to determine what color resides there.</param>
        /// <returns>Index of the color, if found. -1 if no color exists there.</returns>
        private int IndexOf(Point point) {
            EstablishPalettes();

            Point Loc;
            for (int i = 0; i < _combinedList.Count; i++) {
                Loc = _combinedList[i].Location;
                if (_showNames) {
                    if ((point.Y >= Loc.Y) && (point.Y < Loc.Y + _gridSize.Height)) {
                        return i;
                    }
                }
                else {
                    if ((point.X >= Loc.X) && (point.X < Loc.X + _gridSize.Width) && (point.Y >= Loc.Y) && (point.Y < Loc.Y + _gridSize.Height)) {
                        return i;
                    }
                }
            }
            return -1;
        }


        /// <summary>
        ///     Determines the index of a color based on its value.
        /// </summary>
        /// <param name="color">Color to search for.</param>
        /// <param name="ignoreAlpha">Indicates whether the transparency of the color should be taken into account.</param>
        /// <returns>Index of the color, if found. -1 if no color is found.</returns>
        private int IndexOf(Color color, bool ignoreAlpha) {
            EstablishPalettes();

            if (color.IsKnownColor && ignoreAlpha) {
                for (int i = 0; i < _combinedList.Count; i++) {
                    if (_combinedList[i].Name == color.Name) {
                        return i;
                    }
                }
            }

            // Not a known color or there is no match.
            for (int i = 0; i < _combinedList.Count; i++) {
                if ((ignoreAlpha ? true : _combinedList[i].Color.A == color.A) && _combinedList[i].Color.R == color.R &&
                    _combinedList[i].Color.G == color.G && _combinedList[i].Color.B == color.B) {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        ///     Checks to see if the entire selected color is visible.
        /// </summary>
        private bool IsVisible() {
            return Parent.ClientRectangle.IntersectsWith(new Rectangle(_combinedList[_selectedIndex].Location, _gridSize));
        }


        ///// <summary>
        ///// Loads in a list of custom colors.
        ///// </summary>
        ///// <param name="colorList">List of colors.</param>
        ///// <param name="ignoreDuplicateColors">Indicates if duplicated color values in the custom list should be ignored.</param>
        //public void LoadCustomColors(List<NamedColor> colorList, bool ignoreDuplicateColors)
        //{
        //	foreach (NamedColor nColor in colorList)
        //	{
        //		if (ignoreDuplicateColors & ColorExists(nColor.Color, true))
        //			continue;
        //		AddColor(nColor);
        //	}
        //	this.Refresh();
        //}

        /// <summary>
        ///     Load the initial starting palette of colors.
        /// </summary>
        private void LoadDefaultPalette() {
            _baseList.Clear();

            _baseList.Add(new NamedColor("White", 255, 255, 255));
            _baseList.Add(new NamedColor("95% Black", 224, 224, 224));
            _baseList.Add(new NamedColor("Silver", 192, 192, 192));
            _baseList.Add(new NamedColor("Gray", 128, 128, 128));
            _baseList.Add(new NamedColor("25% Black", 64, 64, 64));
            _baseList.Add(new NamedColor("Black", 0, 0, 0));
            _baseList.Add(new NamedColor("Light Pink", 255, 192, 192));
            _baseList.Add(new NamedColor("Medium Pink", 255, 128, 128));
            _baseList.Add(new NamedColor("Red", 255, 0, 0));
            _baseList.Add(new NamedColor("Dusky Red", 192, 0, 0));
            _baseList.Add(new NamedColor("Maroon", 128, 0, 0));
            _baseList.Add(new NamedColor("Dark Red", 64, 0, 0));
            _baseList.Add(new NamedColor("Pumpkin Blush", 255, 224, 192));
            _baseList.Add(new NamedColor("Light Orange", 255, 192, 128));
            _baseList.Add(new NamedColor("Orange", 255, 165, 0));
            _baseList.Add(new NamedColor("Dark Pumpkin", 192, 64, 0));
            _baseList.Add(new NamedColor("Sienna", 128, 64, 0));
            _baseList.Add(new NamedColor("Bruised Orange", 128, 64, 64));
            _baseList.Add(new NamedColor("Lemonade", 255, 255, 192));
            _baseList.Add(new NamedColor("Light Yellow", 255, 255, 128));
            _baseList.Add(new NamedColor("Yellow", 255, 255, 0));
            _baseList.Add(new NamedColor("Dark Yellow", 192, 192, 0));
            _baseList.Add(new NamedColor("Olive", 128, 128, 0));
            _baseList.Add(new NamedColor("Scourged Yellow", 64, 64, 0));
            _baseList.Add(new NamedColor("Faded Green", 192, 255, 192));
            _baseList.Add(new NamedColor("Light Green", 128, 255, 128));
            _baseList.Add(new NamedColor("Lime", 0, 255, 0));
            _baseList.Add(new NamedColor("Kelly Green", 0, 192, 0));
            _baseList.Add(new NamedColor("Green", 0, 128, 0));
            _baseList.Add(new NamedColor("Forest Green", 0, 64, 0));
            _baseList.Add(new NamedColor("Fairy Dust", 192, 255, 255));
            _baseList.Add(new NamedColor("Light Cyan", 128, 255, 255));
            _baseList.Add(new NamedColor("Cyan", 0, 255, 255));
            _baseList.Add(new NamedColor("Dark Cyan", 0, 192, 192));
            _baseList.Add(new NamedColor("Teal", 0, 128, 128));
            _baseList.Add(new NamedColor("Dark Teal", 0, 64, 64));
            _baseList.Add(new NamedColor("Powder Blue", 192, 192, 255));
            _baseList.Add(new NamedColor("Light Blue", 128, 128, 255));
            _baseList.Add(new NamedColor("Blue", 0, 0, 255));
            _baseList.Add(new NamedColor("Dark Blue", 0, 0, 192));
            _baseList.Add(new NamedColor("Navy", 0, 0, 128));
            _baseList.Add(new NamedColor("Dark Navy", 0, 0, 64));
            _baseList.Add(new NamedColor("Aurora Pink", 255, 192, 255));
            _baseList.Add(new NamedColor("Light Magenta", 255, 128, 255));
            _baseList.Add(new NamedColor("Magenta", 255, 0, 255));
            _baseList.Add(new NamedColor("Dark Magenta", 192, 0, 192));
            _baseList.Add(new NamedColor("Purple", 128, 0, 128));
            _baseList.Add(new NamedColor("Dark Purple", 64, 0, 64));

            Refresh();
        }


        /// <summary>
        ///     Loads in the palette of all KnownColors, excluding Transparent and the SystemColors.
        /// </summary>
        private void LoadKnownColorsPalette() {
            _baseList.Clear();

            // Get the list of all the colors in the System.Drawing.KnowColor enum, including the system colors.
            var AllColors = new List<string>();
            AllColors.AddRange(Enum.GetNames(typeof (KnownColor)));

            List<string> SystemEnvironmentColors = GetSystemColorNames();
            SystemEnvironmentColors.Add("Transparent");
            foreach (string ColorName in AllColors) {
                if (!SystemEnvironmentColors.Contains(ColorName)) {
                    _baseList.Add(new NamedColor(ColorName, Color.FromName(ColorName)));
                }
            }
            AllColors = null;
            SystemEnvironmentColors = null;

            Refresh();
        }


        /// <summary>
        ///     Loads in the palette of colors defined by the enumeration.
        /// </summary>
        private void LoadPredefinedPalette() {
            _combinedList.Clear();
            _baseList.Clear();
            switch (_colorPalette) {
                case ColorPalette.Default:
                    LoadDefaultPalette();
                    break;
                case ColorPalette.KnownColors:
                    LoadKnownColorsPalette();
                    break;
                case ColorPalette.SystemColors:
                    LoadSystemColorsPalette();
                    break;
            }
        }


        /// <summary>
        ///     Loads in a palette of SystemColors.
        /// </summary>
        private void LoadSystemColorsPalette() {
            _baseList.Clear();
            foreach (string ColorName in GetSystemColorNames()) {
                _baseList.Add(new NamedColor(ColorName, Color.FromName(ColorName)));
            }
            Refresh();
        }


        /// <summary>
        ///     Loads in a list of colors.
        /// </summary>
        /// <param name="palette">List of colors.</param>
        public void LoadPalette(List<NamedColor> palette) {
            _combinedList.Clear();
            _baseList.Clear();
            _baseList.AddRange(palette);
        }


        /// <summary>
        ///     Paints all the colors on the grid.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        private void PaintGrid(Graphics g) {
            EstablishPalettes();

            int WorkingWidth = ClientRectangle.Width - Padding.Horizontal;

            var GridArea = new Rectangle(0, 0, _gridSize.Width, _gridSize.Height);
            var EmbiggenedGridArea = new Rectangle(0, 0, _gridSize.Width + _gridPadding * 3, _gridSize.Height + _gridPadding * 3);
            var GridOutline = new Rectangle(0, 0, GridArea.Width - 1, GridArea.Height - 1);
            var TextBounds = new RectangleF(_gridSize.Width + _gridPadding * 2, _gridPadding, Width - (_gridSize.Width + _gridPadding * 3),
                _gridSize.Height);
            Rectangle SelectedArea;
            Color TextColor = SystemColors.WindowText;
            SizeF Measure = SizeF.Empty;
            bool IsSelected = false;
            bool IsHotTracking = false;
            string Name = string.Empty;
            int HalfPad = _gridPadding / 2;
            int DeltaX = _gridPadding + _gridSize.Width;
            int DeltaY = _gridPadding + _gridSize.Height;
            int MinX = Padding.Left + _gridPadding;
            int MinY = Padding.Top + _gridPadding;
            int X = MinX;
            int Y = MinY;
            int MaxX = ClientRectangle.Width - Padding.Right - _gridPadding;
            var BoldFont = new Font(Font, FontStyle.Bold);

            if (_showNames) {
                EmbiggenedGridArea = new Rectangle(0, 0, _gridSize.Width + _gridPadding * 2, _gridSize.Height + _gridPadding * 2);
            }

            // Setup the StringFormat object
            var sf = new StringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter;
            using (var GridBrush = new SolidBrush(Color.Black)) {
                using (var OutlinePen = new Pen(SystemColors.ControlDarkDark)) {
                    OutlinePen.Alignment = PenAlignment.Inset;
                    OutlinePen.Width = 0.5f;

                    for (int i = 0; i < _combinedList.Count; i++) {
                        if ((i == _customStartIndex) && (_customList.Count > 0)) {
                            if (X > MinX) {
                                X = MinX;
                                Y += DeltaY;
                            }
                            g.DrawLine(OutlinePen, X, Y, MaxX, Y);
                            Y += _gridPadding;
                        }

                        GridArea.Location = new Point(X, Y);
                        GridOutline.Location = new Point(X, Y);
                        TextBounds.Y = Y;

                        Name = _combinedList[i].Name;
                        IsSelected = (i == _selectedIndex);
                        IsHotTracking = (_hotTracking && (i == _hoverIndex));

                        if (_showNames) {
                            SelectedArea = new Rectangle(0, GridArea.Top - HalfPad, Width, GridArea.Height + _gridPadding);
                        }
                        else {
                            SelectedArea = GridArea;
                            SelectedArea.Inflate(1, 1);
                        }

                        if (IsSelected) {
                            g.FillRectangle(SystemBrushes.Highlight, SelectedArea);
                            TextColor = SystemColors.HighlightText;
                        }

                        else if (IsHotTracking) {
                            g.FillRectangle(SystemBrushes.HotTrack, SelectedArea);
                            TextColor = SystemColors.HighlightText;
                        }
                        else {
                            TextColor = SystemColors.WindowText;
                        }

                        using (var HatchBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.Silver)) {
                            g.FillRectangle(HatchBrush, GridArea);
                        }

                        GridBrush.Color = _combinedList[i].Color;

                        // Paint the color
                        g.FillRectangle(GridBrush, GridArea);
                        _combinedList[i].Location = GridArea.Location;

                        // Draw the Name
                        if (_showNames) {
                            Measure = g.MeasureString(Name, (IsSelected || IsHotTracking) ? BoldFont : Font);
                            TextBounds.Offset(0, (_gridSize.Height - Measure.Height) / 2f);
                            TextBounds.Height = Measure.Height;
                            using (var TextBrush = new SolidBrush(TextColor)) {
                                g.DrawString(Name, ((IsSelected || IsHotTracking) ? BoldFont : Font), TextBrush, TextBounds, sf);
                            }
                        }

                        // Border the color
                        g.DrawRectangle(OutlinePen, GridOutline);

                        // Move to the next position
                        if (!_showNames) {
                            X += DeltaX;
                            //if ((X + DeltaX) > this.Size.Width)
                            if ((X + DeltaX) > MaxX) {
                                //if (MaxX == 0)
                                //{
                                //    MaxX = X - DeltaX + _gridSize.Width;
                                //}

                                X = MinX;
                                Y += DeltaY;
                            }
                        }
                        else {
                            Y += DeltaY;
                            MaxX = (int) TextBounds.Right;
                        }
                    }
                }
            }

            // If these buttons are present, increment the height and reposition these buttons at the bottom.
            //if ((_allowAddColors || _allowEmptyColor) && !_showNames)
            if (_allowAddColors || _allowEmptyColor) {
                if (X > MinX) {
                    X = MinX;
                    Y += DeltaY;
                }
                if (_allowAddColors) {
                    cmdAddColor.Location = new Point(X, Y);
                    X += cmdAddColor.Width + _gridPadding;
                }
                if (_allowEmptyColor) {
                    cmdEmptyColor.Location = new Point(X, Y);
                }
                Y += Math.Max(cmdAddColor.Height, cmdEmptyColor.Height) + _gridPadding;
                Y += _gridPadding;
            }
            else {
                if (X > MinX) {
                    X = MinX;
                    Y += DeltaY;
                }
            }

            if (_stretchToFitParent && (Y < (Parent.ClientRectangle.Height - Padding.Bottom))) {
                Y = Parent.ClientRectangle.Height - _gridPadding - Padding.Vertical;
            }
            Height = Y;

            if (_embiggenHotTracked && (_hoverIndex >= 0)) {
                NamedColor eNamed = _combinedList[_hoverIndex];
                var Center = new Point(eNamed.Location.X + _gridSize.Width / 2, eNamed.Location.Y + _gridSize.Height / 2);
                EmbiggenedGridArea.Location = new Point(Center.X - EmbiggenedGridArea.Width / 2, Center.Y - EmbiggenedGridArea.Height / 2);
                using (var HatchBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.Silver)) {
                    g.FillRectangle(HatchBrush, EmbiggenedGridArea);
                }
                using (var GridBrush = new SolidBrush(eNamed.Color)) {
                    g.FillRectangle(GridBrush, EmbiggenedGridArea);
                }
                using (var GridPen = new Pen(SystemColors.HotTrack, 2)) {
                    g.DrawRectangle(GridPen, EmbiggenedGridArea);
                }
            }
        }

        #endregion [ Methods ]

        #region [ Events ]

        #region [ Event Declarations ]

        public event EventHandler SelectedIndexChange;
        public event EventHandler AddButtonClick;

        #endregion [ Event Declarations ]

        #region [ Event Triggers ]

        protected void OnAddButtonClick() {
            if (_suspendNativeAddButtonClick) {
                if (AddButtonClick != null) {
                    AddButtonClick(null, null);
                }
            }
        }


        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            int index = IndexOf(e.Location);
            if (index != -1) {
                SelectedIndex = index;
            }
        }


        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (_hotTracking) {
                _hoverIndex = -1;
                Refresh();
            }
            toolTip1.Hide(this);
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            string Text = string.Empty;

            try {
                Point pos = PointToClient(MousePosition);
                int index = IndexOf(e.Location);
                if ((index != -1) && (_hoverIndex != index)) // Only update the tooltip and hotTracking if the actual index changes.
                {
                    _hoverIndex = index;
                    if (_showRGBOnToolTip) {
                        Text = _combinedList[index].GetRGBString();
                    }
                    else {
                        Text = _combinedList[index].ToString();
                    }
                    toolTip1.Show(Text, this, new Point(e.Location.X, e.Location.Y + Cursor.Size.Height / 2));
                    if (_hotTracking) {
                        Refresh();
                    }
                }
                else if (index == -1) {
                    toolTip1.Hide(this);
                }
            }
            catch {}
        }


        protected override void OnPaint(PaintEventArgs e) {
            PaintGrid(e.Graphics);
        }


        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Refresh();
        }


        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    this.Refresh();
        //    MakeVisible();
        //}

        protected void OnSelectedIndexChange() {
            if (SelectedIndexChange != null) {
                SelectedIndexChange(null, null);
            }
        }


        protected override void OnSystemColorsChanged(EventArgs e) {
            base.OnSystemColorsChanged(e);
            if (_colorPalette == ColorPalette.SystemColors) {
                LoadSystemColorsPalette();
            }
        }

        #endregion [ Event Triggers ]

        private void cmdAddColor_Click(object sender, EventArgs e) {
            if (_suspendNativeAddButtonClick) {
                OnAddButtonClick();
                return;
            }

            ColorDropDown Parent = null;
            if ((_parentUserControl != null) && (_parentUserControl is ColorDropDown)) {
                Parent = (ColorDropDown) _parentUserControl;
                Parent.FreezeDropDown(false);
            }

            var Selected = new NamedColor();
            if (_selectedIndex >= 0) {
                Selected = _combinedList[_selectedIndex];
            }

            if (!Selected.Color.Equals(_color)) {
                Selected = new NamedColor(_color);
            }

            AddColorDialog.OKButton_Image = _okButtonImage;
            AddColorDialog.CancelButton_Image = _cancelButtonImage;
            AddColorDialog.Color = Selected.Color;
            AddColorDialog.ColorName = Selected.Name;

            if (AddColorDialog.ShowDialog() != DialogResult.Cancel) {
                int Index = IndexOf(AddColorDialog.Color, true);

                if (Index >= 0) {
                    SelectedIndex = Index;
                }
                else {
                    AddColor(AddColorDialog.Color, AddColorDialog.ColorName);
                    SelectedIndex = IndexOf(AddColorDialog.Color, true);
                    if (Parent != null) {
                        Parent.CloseDropDown();
                    }
                }
            }

            if (Parent != null) {
                Parent.UnFreezeDropDown();
            }

            Refresh();
        }


        private void cmdEmptyColor_Click(object sender, EventArgs e) {
            Color = Color.Empty;
        }

        #endregion [ Events ]
    }
}