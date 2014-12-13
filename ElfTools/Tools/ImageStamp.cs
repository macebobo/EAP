using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ElfCore.Channels;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Image Stamp")]
    [ElfToolCore]
    public class ImageStampTool : BaseTool, ITool {
        #region [ Enums ]

        #region Nested type: ImageStamp

        private enum ImageStamp {
            NotSet = -1,
            CandyCane,
            Ghost,
            JackOLantern,
            MiniTree,
            Snowflake,
            Snowman,
            Custom
        }

        #endregion

        #region Nested type: ImageStampSize

        private enum ImageStampSize {
            NotSet = -1,
            VerySmall,
            Small,
            Medium,
            Large
        }

        #endregion

        #endregion [ Enums ]

        #region [ Private Variables ]

        // Settings from the ToolStrip
        private ToolStripSeparator IS_Sep;
        private Timer LoadTimer;
        private ToolStripLabel _cboSize;
        private string _customFilename = string.Empty;
        private bool _fileNotFound;
        private bool _flipHorizontally;
        private bool _flipVertically;
        private ImageStamp _imageStamp = ImageStamp.NotSet;
        private ImageStampSize _imageStampSize = ImageStampSize.NotSet;
        private ToolStripLabel _imgWarning;
        private bool _loading;
        private CanvasPoint _offset;
        private ImageStampChannel _stampChannel;
        private ToolStripLabel _txtFilename;

        // Controls from ToolStrip
        private ToolStripDropDownButton cboSelectStamp;
        private ToolStripComboBox cboSize;
        private ToolStripButton cmdBrowse;
        private ToolStripButton cmdFlipH;
        private ToolStripButton cmdFlipV;
        private ToolStripLabel imgWarning;
        private ToolStripLabel lblDimensions;
        private ToolStripTextBox txtFilename;

        // Used for rendering

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Gets and Sets the cursor for this tool. If there is no image stamp loaded (ie a custom stamp with no file
        ///     indicated, or is missing)
        ///     then sets the cursor to be the default NO cursor.
        /// </summary>
        //public override Cursor Cursor
        //{
        //	get 
        //	{
        //		if (_stampChannel.HasLatticeData)
        //			return base.Cursor;
        //		else
        //			return Cursors.No;
        //	}
        //	set { base.Cursor = value; }
        //}

        #endregion [ Properties ]

        #region [ Constants ]
        private const string IS_IMAGE = "Image";

        private const string IS_SIZE = "Size";
        private const string IS_FLIPHORIZONTAL = "FlipHorizontal";
        private const string IS_FLIPVERTICAL = "FlipVertical";
        private const string IS_FILENAME = "CustomFileName";

        #endregion [ Constants ]

        #region [ Constructors ]

        public ImageStampTool() {
            ID = (int) ToolID.ImageStamp;
            Name = "Image Stamp";
            ToolBoxImage = ElfRes.imagestamp;
            ToolBoxImageSelected = ElfRes.imagestamp_selected;
            MultiGestureKey1 = Keys.I;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Load in the saved values from the Settings Xml file. The path to be used should be
        ///     ToolSettings\[Name of this tool].
        ///     This fires when the tool is first loaded up at application start.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            Cursor = base.CreateCursor(ElfRes.imagestamp, new Point(7, 7));

            _stampChannel = new ImageStampChannel();
            _workshop.StampChannels.Add(_stampChannel);

            // Load the Settings values
            _imageStamp = EnumHelper.GetEnumFromValue<ImageStamp>(LoadValue(IS_IMAGE, (int) ImageStamp.CandyCane));
            _imageStampSize = EnumHelper.GetEnumFromValue<ImageStampSize>(LoadValue(IS_SIZE, (int) ImageStampSize.Small));
            _customFilename = LoadValue(IS_FILENAME, string.Empty);
            _flipHorizontally = LoadValue(IS_FLIPHORIZONTAL, false);
            _flipVertically = LoadValue(IS_FLIPVERTICAL, false);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            cboSelectStamp = (ToolStripDropDownButton) GetItem<ToolStripDropDownButton>("ImageStamp_cboSelectStamp");
            cboSize = (ToolStripComboBox) GetItem<ToolStripComboBox>("ImageStamp_cboSize");
            txtFilename = (ToolStripTextBox) GetItem<ToolStripTextBox>("ImageStamp_txtFilename");
            cmdBrowse = (ToolStripButton) GetItem<ToolStripButton>("ImageStamp_cmdBrowse");
            cmdFlipH = (ToolStripButton) GetItem<ToolStripButton>("ImageStamp_cmdFlipH");
            cmdFlipV = (ToolStripButton) GetItem<ToolStripButton>("ImageStamp_cmdFlipV");
            _cboSize = (ToolStripLabel) GetItem<ToolStripLabel>("_ImageStamp_cboSize");
            _txtFilename = (ToolStripLabel) GetItem<ToolStripLabel>("_ImageStamp_txtFilename");
            IS_Sep = (ToolStripSeparator) GetItem<ToolStripSeparator>("ImageStamp_Sep1");
            imgWarning = (ToolStripLabel) GetItem<ToolStripLabel>("ImageStamp_WarningIcon");
            _imgWarning = (ToolStripLabel) GetItem<ToolStripLabel>("_ImageStamp_WarningIcon");
            lblDimensions = (ToolStripLabel) GetItem<ToolStripLabel>("ImageStamp_Dimensions");

            LoadTimer = ((Tools_ToolStripContainer) _toolStrip_Form).ToolTimer;

            // Assign the ImageStamp enum value to the tag of each stamp menu item
            cboSelectStamp.DropDownItems[0].Tag = (int) ImageStamp.CandyCane;
            cboSelectStamp.DropDownItems[1].Tag = (int) ImageStamp.Ghost;
            cboSelectStamp.DropDownItems[2].Tag = (int) ImageStamp.JackOLantern;
            cboSelectStamp.DropDownItems[3].Tag = (int) ImageStamp.MiniTree;
            cboSelectStamp.DropDownItems[4].Tag = (int) ImageStamp.Snowflake;
            cboSelectStamp.DropDownItems[5].Tag = (int) ImageStamp.Snowman;
            cboSelectStamp.DropDownItems[6].Tag = (int) ImageStamp.Custom;

            AddButtonFaces(cmdFlipH.Name, new ButtonImages(ElfRes.flipH, ElfRes.flipH_selected));
            AddButtonFaces(cmdFlipV.Name, new ButtonImages(ElfRes.flipV, ElfRes.flipV_selected));

            // Set the initial value for the contol from what we had retrieve from Settings
            cmdFlipH.Checked = _flipHorizontally;
            cmdFlipV.Checked = _flipVertically;
            SetControls(FindDropMenuItemFromValue(cboSelectStamp, (int) _imageStamp));
            cboSize.SelectedIndex = (int) _imageStampSize;

            txtFilename.Text = _customFilename;
            txtFilename.ToolTipText = txtFilename.Text;

            SetToolbarSelectedImage(cmdFlipH);
            SetToolbarSelectedImage(cmdFlipV);
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            base.MouseDown(buttons, latticePoint, actualCanvasPoint);
        }


        /// <summary>
        ///     Canvase MouseLeave event was fired. Hide the StampChannel.
        /// </summary>
        public override void MouseLeave() {
            //_workshop.ImageStampChannel.Visible = false;
            _stampChannel.Visible = false;
            Profile.Refresh();
        }


        /// <summary>
        ///     Offset the StampChannel by the pre-calculated amount from the cursor. Show the channel (in case it was hidden if
        ///     the cursor
        ///     had left the CanvasWindow) and refresh the Profile.
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (Profile == null) {
                return false;
            }

            latticePoint.Offset(_offset);
            //_workshop.ImageStampChannel.Origin = latticePoint;
            //_workshop.ImageStampChannel.Visible = true;

            _stampChannel.Origin = latticePoint;
            _stampChannel.Visible = true;

            // Refresh so we can see the updated position of the stamp.
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
            if (_loading || (Profile == null)) {
                return false;
            }

            if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint)) {
                return false;
            }

            if (!_stampChannel.HasLatticeData) {
                return false;
            }

            //_workshop.ImageStampChannel.Visible = true;
            _stampChannel.Visible = true;

            Point Offset = latticePoint;

            // Back out the earlier offset from the cursor
            Offset.Offset(_offset.X, _offset.Y);

            // Paste the image stamp image into the lattice buffer.
            foreach (Channel Channel in Profile.Channels.Selected) {
                _latticeBuffer = Channel.LatticeBuffer;
                using (Graphics g = Graphics.FromImage(_latticeBuffer)) {
                    //g.DrawImage(_workshop.ImageStampChannel.Stamp, Offset);
                    g.DrawImage(_stampChannel.Stamp, Offset);

                    _workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
                    Profile.Channels.PopulateChannelFromBitmap(_latticeBuffer, Channel);
                }
            }
            Profile.Refresh();
            PostDrawCleanUp();
            return true;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(IS_IMAGE, (int) _imageStamp);
            SaveValue(IS_SIZE, (int) _imageStampSize);
            SaveValue(IS_FILENAME, _customFilename);
            SaveValue(IS_FLIPHORIZONTAL, _flipHorizontally);
            SaveValue(IS_FLIPVERTICAL, _flipVertically);
        }


        /// <summary>
        ///     Method that fires when this Tool is selected in the ToolBox.
        ///     For this tool, it will set a flag in the Workshop object to indicate to show the ImageStamp Channel
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();

            LoadTimer.Enabled = false;
            LoadTimer.Interval = 100;

            LoadImageStamp();
        }


        /// <summary>
        ///     Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a
        ///     ShutDown.
        /// </summary>
        public override void OnUnselected() {
            //_workshop.ImageStampChannel.Visible = false;
            //_workshop.ImageStampChannel.Empty();

            //_stampChannel.Visible = false;
            //_stampChannel.Empty();

            LoadTimer.Enabled = false;
            LoadTimer.Tick -= tmrStamp_Tick;
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

            cboSelectStamp = null;
            cboSize = null;
            txtFilename = null;
            cmdBrowse = null;
            cmdFlipH = null;
            cmdFlipV = null;
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
                cboSize.SelectedIndexChanged += cboSize_SelectedIndexChanged;
                cmdBrowse.Click += cmdBrowse_Click;
                cmdFlipH.Click += cmdFlipH_Click;
                cmdFlipV.Click += cmdFlipV_Click;
                LoadTimer.Tick += tmrStamp_Tick;

                foreach (ToolStripMenuItem Item in cboSelectStamp.DropDownItems) {
                    Item.Click += cboSelectStamp_Click;
                }
            }
            else {
                cboSize.SelectedIndexChanged -= cboSize_SelectedIndexChanged;
                cmdBrowse.Click -= cmdBrowse_Click;
                cmdFlipH.Click -= cmdFlipH_Click;
                cmdFlipV.Click -= cmdFlipV_Click;
                LoadTimer.Tick -= tmrStamp_Tick;

                foreach (ToolStripMenuItem Item in cboSelectStamp.DropDownItems) {
                    Item.Click -= cboSelectStamp_Click;
                }
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Determine which image to load either out of the assembly resources, or from a file.
        /// </summary>
        /// <returns>Bitmap representing the Image Stamp</returns>
        private Bitmap GetImageStampBitmap() {
            Bitmap Stamp = null;

            switch (_imageStamp) {
                    #region [ Pre-defined stamps ]

                case ImageStamp.CandyCane:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.candycane_5;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.candycane_10;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.candycane_20;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.candycane_30;
                            break;
                    }
                    break;

                case ImageStamp.Ghost:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.ghost_5;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.ghost_10;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.ghost_20;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.ghost_30;
                            break;
                    }
                    break;

                case ImageStamp.JackOLantern:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.jack_5;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.jack_10;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.jack_20;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.jack_30;
                            break;
                    }
                    break;

                case ImageStamp.MiniTree:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.tree_5;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.tree_10;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.tree_20;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.tree_30;
                            break;
                    }
                    break;

                case ImageStamp.Snowflake:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.snowflake_7;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.snowflake_10;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.snowflake_20;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.snowflake_30;
                            break;
                    }
                    break;

                case ImageStamp.Snowman:
                    switch (_imageStampSize) {
                        case ImageStampSize.VerySmall:
                            Stamp = ElfRes.snowman_5x6;
                            break;
                        case ImageStampSize.Small:
                            Stamp = ElfRes.snowman_7x11;
                            break;
                        case ImageStampSize.Medium:
                            Stamp = ElfRes.snowman_9x17;
                            break;
                        case ImageStampSize.Large:
                            Stamp = ElfRes.snowman_11x24;
                            break;
                    }
                    break;

                    #endregion [ Pre-defined stamps ]

                case ImageStamp.Custom:
                    if ((_customFilename ?? string.Empty).Length == 0) {
                        return null;
                    }

                    // Verify the file exists
                    var FI = new FileInfo(_customFilename);
                    _fileNotFound = ((FI == null) || !FI.Exists);
                    FI = null;

                    imgWarning.Visible = _fileNotFound;
                    _imgWarning.Visible = _fileNotFound;

                    if (!_fileNotFound) {
                        Stamp = ImageHandler.LoadBitmapFromFile(_customFilename);
                    }
                    break;
            }

            if (Stamp == null) {
                return null;
            }

            if (_flipHorizontally) {
                Stamp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            if (_flipVertically) {
                Stamp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            return Stamp;
        }


        /// <summary>
        ///     Load the ImageStamp, move it to the proper Channel
        /// </summary>
        private void LoadImageStamp() {
            // First, clear out the image stamp Channel
            //_workshop.ImageStampChannel.Empty();
            _stampChannel.Empty();

            // Next get the bitmap for this stamp and load it in
            //_workshop.ImageStampChannel.Stamp = GetImageStampBitmap();
            _stampChannel.Stamp = GetImageStampBitmap();

            _offset = new Point(-(_stampChannel.StampSize.Width / 2), -(_stampChannel.StampSize.Height / 2));

#if DEBUG
            //_workshop.ExposePane(_workshop.ImageStampChannel.Stamp, Panes.ImageStamp);
            _workshop.ExposePane(_stampChannel.Stamp, Panes.ImageStamp);
#endif

            SetCursor();

            if (_stampChannel.HasLatticeData) {
                lblDimensions.Text = _stampChannel.StampSize.Width + "x" + _stampChannel.StampSize.Height;
            }
            else {
                lblDimensions.Text = "n/a";
            }
        }


        /// <summary>
        ///     Sets the toolstrip controls based on the imagestamp selected
        /// </summary>
        /// <param name="menu"></param>
        private void SetControls(ToolStripMenuItem menu) {
            cboSelectStamp.Image = menu.Image;
            cboSelectStamp.Text = menu.Text;
            cboSelectStamp.ToolTipText = menu.ToolTipText;

            foreach (ToolStripMenuItem Item in cboSelectStamp.DropDownItems) {
                Item.Checked = (Item == menu);
            }

            //_imageStamp = EnumHelper.GetEnumFromValue<ImageStamp>(Convert.ToInt32(Menu.Tag));
            _imageStamp = (ImageStamp) menu.Tag;

            bool IsCustom = (_imageStamp == ImageStamp.Custom);

            cboSize.Visible = !IsCustom;
            _cboSize.Visible = !IsCustom;
            IS_Sep.Visible = IsCustom;
            _txtFilename.Visible = IsCustom;
            txtFilename.Visible = IsCustom;
            cmdBrowse.Visible = IsCustom;
            imgWarning.Visible = IsCustom && _fileNotFound;
            _imgWarning.Visible = IsCustom && _fileNotFound;
        }


        /// <summary>
        ///     Sets the cursor for the current profile to be either the default one for this tool, or the NO cursor if no stamp is
        ///     loaded.
        /// </summary>
        private void SetCursor() {
            if (Profile == null) {
                return;
            }
            if (_stampChannel.HasLatticeData) {
                Profile.Cursor = base.Cursor;
            }
            else {
                Profile.Cursor = Cursors.No;
            }
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Select a file to use as an image stamp.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBrowse_Click(object sender, EventArgs e) {
            _loading = true;

            var OpenImageFileDialog = new OpenFileDialog();
            OpenImageFileDialog.Filter =
                "All Image Formats|*.png;*.bmp;*.gif;*.tif|Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
            OpenImageFileDialog.FilterIndex = 0;
            OpenImageFileDialog.FileName = _customFilename;
            OpenImageFileDialog.Title = "Select File for Image Stamp";

            if (OpenImageFileDialog.ShowDialog() == DialogResult.Cancel) {
                OpenImageFileDialog.Dispose();
                OpenImageFileDialog = null;
                _loading = false;
                return;
            }

            string Filename = OpenImageFileDialog.FileName;

            var Stamp = new Bitmap(Filename);
            if ((Stamp.Width > 100) || (Stamp.Height > 100)) {
                MessageBox.Show("Custom Image stamp file cannot be larger than 100x100 pixels.", @"Load Custom Image Stamp", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _loading = false;
                return;
            }
            _customFilename = Filename;

            txtFilename.Text = Filename;
            txtFilename.ToolTipText = txtFilename.Text;

            Stamp.Dispose();
            Stamp = null;

            OpenImageFileDialog.Dispose();
            OpenImageFileDialog = null;

            LoadImageStamp();
            LoadTimer.Enabled = true;
        }


        /// <summary>
        ///     Instructs us to flip the image stamp top to bottom, or back again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdFlipH_Click(object sender, EventArgs e) {
            _flipHorizontally = cmdFlipH.Checked;
            SetToolbarSelectedImage(cmdFlipH);
            SetToolbarSelectedImage(cmdFlipV);
            LoadImageStamp();
        }


        /// <summary>
        ///     Instructs to flip the image stamp left to right, or back again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdFlipV_Click(object sender, EventArgs e) {
            _flipVertically = cmdFlipV.Checked;
            SetToolbarSelectedImage(cmdFlipH);
            SetToolbarSelectedImage(cmdFlipV);
            LoadImageStamp();
        }


        /// <summary>
        ///     Pre-selected Image Stamp has been picket, or else the custom option is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSelectStamp_Click(object sender, EventArgs e) {
            SetControls((ToolStripMenuItem) sender);
            bool IsCustom = (_imageStamp == ImageStamp.Custom);
            //_stampChannel.Empty();
            LoadImageStamp();

            //if (!IsCustom || (IsCustom && (_customFilename.Length > 0)))
            //	LoadImageStamp();
            //else if (IsCustom && (_customFilename.Length == 0))
            //{
            //	_stampChannel.Empty();
            //	Profile.Cursor = Cursors.No;
            //}
        }


        /// <summary>
        ///     Resets the loading flag so that the user can proceed to do their stamping
        /// </summary>
        private void tmrStamp_Tick(object sender, EventArgs e) {
            LoadTimer.Enabled = false;
            _loading = false;
        }


        /// <summary>
        ///     Stamp Size set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSize_SelectedIndexChanged(object sender, EventArgs e) {
            _imageStampSize = EnumHelper.GetEnumFromValue<ImageStampSize>(cboSize.SelectedIndex);
            LoadImageStamp();
        }
    }
}