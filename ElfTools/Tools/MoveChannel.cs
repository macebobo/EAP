using System;
using System.Drawing;
using System.Windows.Forms;

using ElfCore.Channels;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Move Channel")]
    [ElfToolCore]
    public class MoveChannelTool : BaseTool, ITool {
        #region [ Private Variables ]

        // Settings from the ToolStrip
        private readonly IntPtr _grabCursorHandle = IntPtr.Zero;
        private Cursor _grabbingCursor;
        private bool _moveAllChannels;
        private CanvasPoint _moveEndCanvasPoint = CanvasPoint.Empty;
        private CanvasPoint _moveOffset = CanvasPoint.Empty;
        private CanvasPoint _moveStartCanvasPoint;
        private ChannelList _movingChannels;

        // Controls from ToolStrip
        private ToolStripButton cmdAllChannels;
        private ToolStripButton cmdExecute;
        private ToolStripTextBox txtOffsetX;
        private ToolStripTextBox txtOffsetY;

        // Used for moving

        #endregion [ Private Variables ]

        #region [ Constants ]

        private const string OFFSET_X = "Offset_X";
        private const string OFFSET_Y = "Offset_Y";
        private const string ALL_ChannelS = "AllChannels";

        #endregion [ Constants ]

        #region [ Properties ]

        public override string UndoText {
            get {
                if (cmdAllChannels.Checked) {
                    return "Move All Channels";
                }

                if (Profile.Channels.Selected.Count > 0) {
                    return "Move Channels";
                }

                return "Move Channel";
            }
            set { base.UndoText = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public MoveChannelTool() {
            ID = (int) ToolID.MoveChannel;
            Name = "Move Channel";
            ToolBoxImage = ElfRes.move_channel;
            ToolBoxImageSelected = ElfRes.move_channel_selected;
            Cursor = base.CreateCursor(ElfRes.pan, new Point(7, 7));
            AffectMultipleChannels = true;
            MultiGestureKey1 = Keys.V;
            _grabbingCursor = base.CreateCursor(ElfRes.cursor_grabbed, new Point(7, 7));
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Load in the saved values from the Settings Xml file. The path to be used should be
        ///     ToolSettings|[Name of this tool].
        ///     We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
        ///     format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
        ///     for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
        /// </summary>
        public override void Initialize() {
            base.Initialize();

            // Load the Settings values
            _moveOffset = new Point(LoadValue(OFFSET_X, 0), LoadValue(OFFSET_Y, 0));
            _moveAllChannels = LoadValue(ALL_ChannelS, false);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            cmdAllChannels = (ToolStripButton) GetItem<ToolStripButton>("MoveChannel_cmdAllChannels");
            cmdExecute = (ToolStripButton) GetItem<ToolStripButton>("MoveChannel_cmdExecute");
            txtOffsetX = (ToolStripTextBox) GetItem<ToolStripTextBox>("MoveChannel_txtOffsetX");
            txtOffsetY = (ToolStripTextBox) GetItem<ToolStripTextBox>("MoveChannel_txtOffsetY");

            // Set all the selected images for the control
            AddButtonFaces(cmdAllChannels.Name, new ButtonImages(ElfRes.channel, ElfRes.channels_selected));

            // Set the initial value for the contol from what we had retrieve from Settings
            txtOffsetX.Text = _moveOffset.X.ToString();
            txtOffsetY.Text = _moveOffset.Y.ToString();
            cmdAllChannels.Checked = _moveAllChannels;
            SetToolbarSelectedImage(cmdAllChannels);
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            _isMouseDown = true;

            Profile.Cursor = _grabbingCursor;
            _moveStartCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);

            _movingChannels = null;

            if (cmdAllChannels.Checked) {
                _movingChannels = Profile.Channels.GetAllChannels();
            }
            else {
                _movingChannels = Profile.Channels.Selected;
            }
        }


        /// <summary>
        ///     Handles keystrokes for the tool. Returns true if the keystroke was handled within the tool
        /// </summary>
        /// <param name="e"></param>
        public override bool KeyDown(KeyEventArgs e) {
            _moveStartCanvasPoint = new CanvasPoint(0, 0);
            _moveEndCanvasPoint = Point.Empty;

            int Amount = Scaling.CellScale;

            if (Control.ModifierKeys == Keys.Shift) {
                Amount *= 5;
            }

            switch (e.KeyCode) {
                case Keys.Up:
                    _moveEndCanvasPoint = new CanvasPoint(0, -Amount);
                    break;

                case Keys.Down:
                    _moveEndCanvasPoint = new CanvasPoint(0, Amount);
                    break;

                case Keys.Left:
                    _moveEndCanvasPoint = new CanvasPoint(-Amount, 0);
                    break;

                case Keys.Right:
                    _moveEndCanvasPoint = new CanvasPoint(Amount, 0);
                    break;
            }

            if (_moveEndCanvasPoint.IsEmpty) {
                return false;
            }

            if (cmdAllChannels.Checked) {
                _movingChannels = Profile.Channels.GetAllChannels();
            }
            else {
                _movingChannels = Profile.Channels.Selected;
            }

            MoveTheseChannels();

            return true;
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

            if (!_isMouseDown) {
                return false;
            }

            _moveEndCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
            MoveTheseChannels();
            _moveStartCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
            return true;
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            _isMouseDown = false;

            _moveEndCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
            MoveTheseChannels();
            Profile.Cursor = Cursor;

            // Fire the event to indicate that this tool has finished working.
            EndOperation();

            return true;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(OFFSET_X, _moveOffset.X);
            SaveValue(OFFSET_Y, _moveOffset.Y);
            //SaveValue(ALL_ChannelS, _moveAllChannels);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cmdAllChannels = null;
            cmdExecute = null;
            txtOffsetX = null;
            txtOffsetY = null;
            _movingChannels = null;
            if (_grabbingCursor != null) {
                CustomCursors.DestroyCreatedCursor(_grabCursorHandle);
                _grabbingCursor.Dispose();
                _grabbingCursor = null;
            }
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
                cmdAllChannels.Click += cmdAllChannels_Click;
                cmdExecute.Click += cmdExecute_Click;
                txtOffsetX.Leave += txtOffsetX_Leave;
                txtOffsetX.TextChanged += txtOffsetX_TextChanged;
                txtOffsetX.KeyPress += _toolStrip_Form.SignedNumberOnly_KeyPress;
                txtOffsetY.Leave += txtOffsetY_Leave;
                txtOffsetY.TextChanged += txtOffsetY_TextChanged;
                txtOffsetY.KeyPress += _toolStrip_Form.SignedNumberOnly_KeyPress;
            }
            else {
                cmdAllChannels.Click -= cmdAllChannels_Click;
                cmdExecute.Click -= cmdExecute_Click;
                txtOffsetX.Leave -= txtOffsetX_Leave;
                txtOffsetX.TextChanged -= txtOffsetX_TextChanged;
                txtOffsetX.KeyPress -= _toolStrip_Form.SignedNumberOnly_KeyPress;
                txtOffsetY.Leave -= txtOffsetY_Leave;
                txtOffsetY.TextChanged -= txtOffsetY_TextChanged;
                txtOffsetY.KeyPress -= _toolStrip_Form.SignedNumberOnly_KeyPress;
            }
            base.AttachEvents(attach);
        }


        private RectangleF GetChannelGroupBound(ChannelList ChannelList) {
            Rectangle ChanRect;
            float minX, minY, maxX, maxY;
            minX = minY = Int32.MaxValue;
            maxX = maxY = Int32.MinValue;

            foreach (Channel Channel in ChannelList) {
                ChanRect = Channel.GetBounds();
                if (ChanRect.X < minX) {
                    minX = ChanRect.X;
                }
                if (ChanRect.Y < minY) {
                    minY = ChanRect.Y;
                }
                if (ChanRect.X + ChanRect.Width > maxX) {
                    maxX = ChanRect.X + ChanRect.Width;
                }
                if (ChanRect.Y + ChanRect.Height > maxY) {
                    maxY = ChanRect.Y + ChanRect.Height;
                }
            }
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }


        /// <summary>
        ///     Move all the selected Channels
        /// </summary>
        private void MoveTheseChannels() {
            Point Offset =
                _workshop.CalcLatticePoint(new Point(_moveEndCanvasPoint.X - _moveStartCanvasPoint.X, _moveEndCanvasPoint.Y - _moveStartCanvasPoint.Y));

            foreach (Channel Channel in _movingChannels) {
                Channel.MoveData(Offset);
            }
            Profile.Refresh();
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Execute the move using the offsets in the tool strip
        /// </summary>
        private void cmdExecute_Click(object sender, EventArgs e) {
            if (cmdAllChannels.Checked) {
                _movingChannels = Profile.Channels.GetAllChannels();
            }
            else {
                _movingChannels = Profile.Channels.Selected;
            }

            _moveStartCanvasPoint = new Point(0, 0);
            _moveEndCanvasPoint = new Point(_moveOffset.X * Scaling.CellScale, _moveOffset.Y * Scaling.CellScale);
            MoveTheseChannels();
            Profile.SaveUndo(UndoText);
            Profile.Refresh();
        }


        private void cmdAllChannels_Click(object sender, EventArgs e) {
            _moveAllChannels = cmdAllChannels.Checked;
            SetToolbarSelectedImage(cmdAllChannels);
        }


        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void txtOffsetX_Leave(object sender, EventArgs e) {
            if (txtOffsetX.TextLength == 0) {
                txtOffsetX.Text = "0";
            }

            int X = ValidateInteger(txtOffsetX, _moveOffset.X);
            _moveOffset = new Point(X, _moveOffset.Y);
        }


        private void txtOffsetX_TextChanged(object sender, EventArgs e) {
            int X = 0;
            if (Int32.TryParse(txtOffsetX.Text, out X)) {
                _moveOffset = new Point(X, _moveOffset.Y);
            }
        }


        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void txtOffsetY_Leave(object sender, EventArgs e) {
            if (txtOffsetY.TextLength == 0) {
                txtOffsetY.Text = "0";
            }

            int Y = ValidateInteger(txtOffsetY, _moveOffset.Y);
            _moveOffset = new Point(_moveOffset.X, Y);
        }


        private void txtOffsetY_TextChanged(object sender, EventArgs e) {
            int Y = 0;
            if (Int32.TryParse(txtOffsetY.Text, out Y)) {
                _moveOffset = new Point(_moveOffset.X, Y);
            }
        }
    }
}