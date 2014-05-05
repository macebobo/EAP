using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Util;

using ElfPreview.Annotations;

using Vixen = VixenPlus;

namespace ElfPreview {
    public partial class PreviewDialog : Vixen.OutputPlugInUIBase {
        #region [ Private Variables ]

        private readonly PlaybackProfile _profile;
        private readonly Settings _settings = Settings.Instance;
        private Size _borderSize;
        private float _deltaFPSTime;
        private DateTime _lastCheckedTime;

        #endregion [ Private Variables ]

        #region [ Constructors ]

        public PreviewDialog() {
            InitializeComponent();
        }


        [UsedImplicitly]
        public PreviewDialog(XmlNode setupNode, List<Vixen.Channel> channels, int startChannel) : this() {
            if (setupNode == null) {
                throw new ArgumentNullException("setupNode");
            }

            Left = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, Left);
            Top = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, Top);

            lblTime.Visible = false;
            cmdRecord.Visible = false;
            lblRecording.Visible = false;
            _profile = new PlaybackProfile(setupNode, channels, startChannel, CanvasPane);
            SetCanvasPaneSize(_profile.CanvasSize);
            _lastCheckedTime = DateTime.Now;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        private void SetCanvasPaneSize(Size canvasSize) {
            // Get the difference in the actual size vs the size of the client rectangle
            _borderSize.Width = Size.Width - ClientRectangle.Width;
            _borderSize.Height = (Size.Height - ClientRectangle.Height) + pnlControls.Height;

            CanvasPane.Size = canvasSize;

            Width = canvasSize.Width + _borderSize.Width;
            Height = canvasSize.Height + _borderSize.Height;
        }


        public void UpdateWith(byte[] channelValues, string sequenceName) {
            Text = sequenceName;
            _profile.Alphas = channelValues;
            CanvasPane.Invalidate();

            // Calculate FPS
            var elapsedTime = DateTime.Now.Subtract(_lastCheckedTime);
            _lastCheckedTime = DateTime.Now;
            var elapsed = (float) elapsedTime.TotalSeconds;
            _deltaFPSTime += elapsed;

            if (!(_deltaFPSTime > 1)) {
                return;
            }

            var fps2 = 1 / elapsed;
            lblFrameRate.Text = string.Format("FPS: {0:N1}", fps2);
            _deltaFPSTime -= 1;
        }

        #endregion [ Methods ]

        #region [ Events ]

        private void cmdStop_Click(object sender, EventArgs e) {
            ExecOnParentOrDisable(Keys.F8);
        }


        private void cmdPause_Click(object sender, EventArgs e) {
            ExecOnParentOrDisable(Keys.F7);
        }


        private void cmdPlayFromPoint_Click(object sender, EventArgs e) {
            ExecOnParentOrDisable(Keys.F6);
        }


        private void cmdPlay_Click(object sender, EventArgs e) {
            ExecOnParentOrDisable(Keys.F5);
        }


        private void ExecOnParentOrDisable(Keys key) {
            if (!ExecOnParent(new KeyEventArgs(key))) {
                pnlControls.Enabled = false;
            }
        }

        private bool ExecOnParent(KeyEventArgs keyEventArgs) {
            if (ExecutionParent == null) {
                return false;
            }

            ExecutionParent.Notify(Vixen.Notification.KeyDown, keyEventArgs);
            return true;
        }

        /// <summary>
        ///     Using the AviFile Library
        ///     http://www.codeproject.com/Articles/7388/A-Simple-C-Wrapper-for-the-AviFile-Library
        /// </summary>
        private void cmdRecord_Click(object sender, EventArgs e) {}


        private void Form_KeyDown(object sender, KeyEventArgs e) {
            ExecOnParent(e);
        }


        private void CanvasPane_Paint(object sender, PaintEventArgs e) {
            _profile.Draw(e);
        }


        private void Form_Shown(object sender, EventArgs e) {
                MdiParent = null;
        }


        private void Form_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            if (e.Cancel) {
                cmdStop_Click(null, null);
                Hide();
            }

            if (WindowState == FormWindowState.Normal) {
                _settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, Left);
                _settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, Top);
            }
            _settings.Save();
        }

        #endregion [ Events ]
    }
}