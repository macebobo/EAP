using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;

using Microsoft.VisualBasic;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Text")]
    [ElfToolCore]
    public class TextTool : BaseTool, ITool {
        #region [ Private Variables ]

        // Settings from the ToolStrip
        private string _fontName = string.Empty;
        private float _fontSize;
        private FontStyle _fontStyle = FontStyle.Regular;

        private ListBoxUtil _listboxUtil = new ListBoxUtil();
        private FontData _selectedFontData;
        private Rectangle _stampBounds = Rectangle.Empty;
        private ImageStampChannel _stampChannel;
        private string _text = string.Empty;

        // Controls from ToolStrip
        private ToolStripComboBox cboFontName;
        private ToolStripComboBox cboFontSize;
        private ToolStripButton cmdBold;
        private ToolStripButton cmdEditText;
        private ToolStripButton cmdItalic;
        private ToolStripButton cmdUnderline;

        #endregion [ Private Variables ]

        #region [ Constants ]

        private const string FONT_NAME = "FontName";
        private const string FONT_SIZE = "Size";
        private const string FONT_STYLE = "Style";
        private const string LAST_TEXT = "LastText";

        #endregion [ Constants ]

        #region [ Constructors ]

        public TextTool() {
            ID = (int) ToolID.Text;
            Name = "Text";
            ToolBoxImage = ElfRes.text;
            ToolBoxImageSelected = ElfRes.text_selected;
            Cursor = Cursors.IBeam;
            MultiGestureKey1 = Keys.T;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Clears out the contents of the TextChannel
        /// </summary>
        public override void Cancel() {
            _stampChannel.Empty();
            Profile.Refresh();
        }


        /// <summary>
        ///     Load in the saved values from the Settings Xml file. The path to be used should be
        ///     ToolSettings|[Name of this tool].
        ///     We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
        ///     format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
        ///     for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            _stampChannel = new ImageStampChannel();
            _stampChannel.Name = "Text Stamp";
            _workshop.StampChannels.Add(_stampChannel);

            // Load the Settings values
            _fontName = LoadValue(FONT_NAME, "Arial");
            _fontSize = LoadValue(FONT_SIZE, 8f);
            _fontStyle = (FontStyle) LoadValue(FONT_STYLE, (int) FontStyle.Regular);
            _text = LoadValue(LAST_TEXT, string.Empty);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            cboFontName = (ToolStripComboBox) GetItem<ToolStripComboBox>("Text_cboFontName");
            cboFontSize = (ToolStripComboBox) GetItem<ToolStripComboBox>("Text_cboFontSize");
            cmdBold = (ToolStripButton) GetItem<ToolStripButton>("Text_cmdBold");
            cmdItalic = (ToolStripButton) GetItem<ToolStripButton>("Text_cmdItalic");
            cmdUnderline = (ToolStripButton) GetItem<ToolStripButton>("Text_cmdUnderline");
            cmdEditText = (ToolStripButton) GetItem<ToolStripButton>("Text_cmdEditText");

            // Set all the selected images for the control
            AddButtonFaces(cmdBold.Name, new ButtonImages(ElfRes.bold, ElfRes.bold_selected));
            AddButtonFaces(cmdItalic.Name, new ButtonImages(ElfRes.italic, ElfRes.italic_selected));
            AddButtonFaces(cmdUnderline.Name, new ButtonImages(ElfRes.underline, ElfRes.underline_selected));

            // Set the initial value for the contol from what we had retrieve from Settings
            LoadFontList();
            if (cboFontName.Items.Count > 0) {
                if (!_listboxUtil.Set(cboFontName, _fontName)) {
                    cboFontName.SelectedIndex = 0;
                }
            }

            cboFontSize.Text = _fontSize + " pt";
            cmdBold.Checked = ((_fontStyle & FontStyle.Bold) == FontStyle.Bold);
            cmdItalic.Checked = ((_fontStyle & FontStyle.Italic) == FontStyle.Italic);
            cmdUnderline.Checked = ((_fontStyle & FontStyle.Underline) == FontStyle.Underline);

            SetToolbarSelectedImage(cmdBold);
            SetToolbarSelectedImage(cmdItalic);
            SetToolbarSelectedImage(cmdUnderline);

            DetermineAvailableStylesForFont();
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (!_stampChannel.HasLatticeData) {
                _isMouseDown = false;
                PromptForText();
            }
        }


        /// <summary>
        ///     Canvase MouseLeave event was fired. Hide the TextChannel.
        /// </summary>
        public override void MouseLeave() {
            _stampChannel.Visible = false;
            Profile.Refresh();
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

            Point Offset = latticePoint;

            Offset.X -= _stampBounds.Width / 2;
            Offset.Y -= _stampBounds.Height / 2;
            _stampChannel.Origin = Offset;
            _stampChannel.Visible = true;

            Profile.Refresh();
            return true;
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            Point Offset = latticePoint;

            Offset.X -= _stampBounds.Width / 2;
            Offset.Y -= _stampBounds.Height / 2;
            _stampChannel.Origin = Offset;

            List<Point> Cells = null;

            foreach (Channel Channel in Profile.Channels.Selected) {
                Cells = Workshop.CloneList<Point>(_stampChannel.Lattice);
                Channel.Paint(Cells, Offset);
            }

            PostDrawCleanUp();

            return true;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(FONT_NAME, _fontName);
            SaveValue(FONT_SIZE, _fontSize);
            SaveValue(FONT_STYLE, (int) _fontStyle);
            SaveValue(LAST_TEXT, _text);
        }


        /// <summary>
        ///     Method that fires when this Tool is selected in the ToolBox.
        ///     For this tool, it will set a flag in the Workshop object to indicate to show the Text Channel
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();
        }


        /// <summary>
        ///     Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a
        ///     ShutDown.
        /// </summary>
        public override void OnUnselected() {
            _stampChannel.Visible = false;
            if (Profile != null) {
                Profile.Refresh();
            }
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            if (_stampChannel != null) {
                if ((_workshop != null) && (_workshop.StampChannels != null) && (_workshop.StampChannels.Contains(_stampChannel))) {
                    _workshop.StampChannels.Remove(_stampChannel);
                }

                _stampChannel.Dispose();
                _stampChannel = null;
            }

            // Controls from ToolStrip
            cboFontName = null;
            cboFontSize = null;
            cmdBold = null;
            cmdItalic = null;
            cmdUnderline = null;
            cmdEditText = null;
            _selectedFontData = null;
            _listboxUtil = null;
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
                cboFontName.SelectedIndexChanged += cboFontName_SelectedIndexChanged;
                cboFontSize.SelectedIndexChanged += cboFontSize_SelectedIndexChanged;
                cboFontSize.KeyPress += FontSize_KeyPress;
                cmdBold.Click += cmdFontStyle_Click;
                cmdItalic.Click += cmdFontStyle_Click;
                cmdUnderline.Click += cmdFontStyle_Click;
                cmdEditText.Click += cmdEditText_Click;
            }
            else {
                cboFontName.SelectedIndexChanged -= cboFontName_SelectedIndexChanged;
                cboFontSize.SelectedIndexChanged -= cboFontSize_SelectedIndexChanged;
                cboFontSize.KeyPress -= FontSize_KeyPress;
                cmdBold.Click -= cmdFontStyle_Click;
                cmdItalic.Click -= cmdFontStyle_Click;
                cmdUnderline.Click -= cmdFontStyle_Click;
                cmdEditText.Click -= cmdEditText_Click;
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Based on the settings, create a Font object. Use that with the Text entered to create the text on the TextChannel.
        /// </summary>
        private void CreateTextAndSetStamp() {
            Font Font = CreateFont();
            SizeF StringSize;

            if (Font == null) {
                Font = CreateFont("Arial");
            }

            // Determine the dimensions of the Text
            using (Graphics g = Profile.GetCanvasGraphics()) {
                StringSize = g.MeasureString(_text, Font);
            }

            // Create the stamp
            var TextStamp = new Bitmap((int) StringSize.Width + 10, (int) StringSize.Height + 10);
            using (Graphics g = Graphics.FromImage(TextStamp)) {
                // Don't want to anti-alias this, 1 bit only
                //g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                // White text on a black background
                g.Clear(Color.Black);
                g.DrawString(_text, Font, Brushes.White, new Point(0, 0));

                // Next get the bitmap for this stamp and load it in
                _stampChannel.Stamp = TextStamp;
                _stampChannel.Visible = true;
                _stampBounds = _stampChannel.GetBounds();

#if DEBUG
                Workshop.Instance.ExposePane(TextStamp, Panes.ImageStamp);
#endif

                Profile.Refresh();
            }

            if (Font != null) {
                Font.Dispose();
                Font = null;
            }
            if (TextStamp != null) {
                TextStamp.Dispose();
                TextStamp = null;
            }
        }


        /// <summary>
        ///     Create a Font object based on the settings
        /// </summary>
        private Font CreateFont() {
            return CreateFont(_fontName);
        }


        /// <summary>
        ///     Create a Font object, using the font name passed in.
        /// </summary>
        /// <param name="fontName">Name of the font to use</param>
        private Font CreateFont(string fontName) {
            if ((fontName.Length > 0) && (_fontSize > 0f)) {
                return new Font(fontName, _fontSize, _fontStyle);
            }
            return null;
        }


        /// <summary>
        ///     Some fonts do not support certain styles. Brush Script MT is Italic only.
        ///     In these cases, we will need to unclick and disable the invalid style options.
        /// </summary>
        private void DetermineAvailableStylesForFont() {
            if (_selectedFontData == null) {
                return;
            }

            cmdBold.Enabled = (_selectedFontData.DoesBold);
            cmdItalic.Enabled = (_selectedFontData.DoesItalic);
            cmdUnderline.Enabled = (_selectedFontData.DoesUnderline);

            if (!cmdBold.Enabled) {
                cmdBold.Checked = false;
            }

            if (!cmdItalic.Enabled) {
                cmdItalic.Checked = false;
            }

            if (!cmdUnderline.Enabled) {
                cmdUnderline.Checked = false;
            }

            if (!_selectedFontData.DoesRegular) {
                // Find a valid style for this weirdo
                if (cmdBold.Enabled) {
                    cmdBold.Checked = true;
                }

                else if (cmdItalic.Enabled) {
                    cmdItalic.Checked = true;
                }

                else if (cmdUnderline.Enabled) {
                    cmdUnderline.Checked = true;
                }
            }

            SetFontStyle();
        }


        /// <summary>
        ///     Create the list of fonts available on this system, populate the combo box on the settings toolstrip
        /// </summary>
        private void LoadFontList() {
            cboFontName.BeginUpdate();
            cboFontName.Items.Clear();

            FontData fData = null;

            var ifc = new InstalledFontCollection();
            for (int i = 0; i < ifc.Families.Length; i++) {
                fData = new FontData(ifc.Families[i]);
                _listboxUtil.Add(cboFontName, new ListBoxUtil.Item(fData.Name, fData.Name, fData));
            }

            cboFontName.EndUpdate();

            if (ifc != null) {
                ifc.Dispose();
                ifc = null;
            }
        }


        /// <summary>
        ///     Prompts the user to input the text he wants stamped onto the Canvas.
        /// </summary>
        private void PromptForText() {
            string Text = Interaction.InputBox("Please enter your text here:", "Text", _text);
            if (Text.Length > 0) {
                _text = Text;
                CreateTextAndSetStamp();
            }
        }


        /// <summary>
        ///     Creates the proper font style enumeration value based on which style buttons are currently checked.
        /// </summary>
        private void SetFontStyle() {
            var Style = FontStyle.Regular;

            if (cmdBold.Checked && cmdBold.Enabled) {
                Style |= FontStyle.Bold;
            }

            if (cmdItalic.Checked && cmdItalic.Enabled) {
                Style |= FontStyle.Italic;
            }

            if (cmdUnderline.Checked && cmdUnderline.Enabled) {
                Style |= FontStyle.Underline;
            }

            _fontStyle = Style;

            SetToolbarSelectedImage(cmdBold);
            SetToolbarSelectedImage(cmdItalic);
            SetToolbarSelectedImage(cmdUnderline);
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Ask the user for their text string, then create an image stamp based on it, if the length > 0
        /// </summary>
        private void cmdEditText_Click(object sender, EventArgs e) {
            PromptForText();
        }


        /// <summary>
        ///     Set the font style based on the combination of buttons clicked.
        /// </summary>
        private void cmdFontStyle_Click(object sender, EventArgs e) {
            SetFontStyle();
            CreateTextAndSetStamp();
        }


        /// <summary>
        ///     Sets the font's name based on the selected value.
        /// </summary>
        private void cboFontName_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboFontName.SelectedIndex > -1) {
                _selectedFontData = (FontData) _listboxUtil.GetItem(cboFontName).StoredObject;
                if (_selectedFontData != null) {
                    _fontName = _selectedFontData.Name;
                    DetermineAvailableStylesForFont();
                }
            }
            if (_isSelected) {
                CreateTextAndSetStamp();
            }
        }


        /// <summary>
        ///     Sets the font's size based on the selected value.
        /// </summary>
        private void cboFontSize_SelectedIndexChanged(object sender, EventArgs e) {
            if (!cboFontSize.Text.Contains(" pt")) {
                cboFontSize.Text += " pt";
            }

            float Size = 0;
            if (float.TryParse(cboFontSize.Text.Replace(" pt", string.Empty), out Size)) {
                _fontSize = Size;
            }

            if (_isSelected) {
                CreateTextAndSetStamp();
            }
        }


        /// <summary>
        ///     Only numbers and the decimal allowed
        /// </summary>
        private void FontSize_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsDigit(e.KeyChar) || (e.KeyChar == '.')) {
                // We like these, do nothing
                e.Handled = false;
            }
            else {
                e.Handled = true;
            }
        }

        #region Nested type: FontData

        private class FontData {
            #region [ Public Field Variables ]

            public readonly FontFamily Family;
            public readonly string Name = string.Empty;

            #endregion [ Public Field Variables ]

            #region [ Properties ]

            public bool DoesRegular {
                get { return Family.IsStyleAvailable(FontStyle.Regular); }
            }

            public bool DoesBold {
                get { return Family.IsStyleAvailable(FontStyle.Bold); }
            }

            public bool DoesItalic {
                get { return Family.IsStyleAvailable(FontStyle.Italic); }
            }

            public bool DoesUnderline {
                get { return Family.IsStyleAvailable(FontStyle.Underline); }
            }

            #endregion [ Properties ]

            #region [ Constructors ]

            public FontData() {}


            public FontData(FontFamily family) : this() {
                Family = family;
                Name = family.Name;
            }

            #endregion [ Constructors ]
        }

        #endregion
    }
}