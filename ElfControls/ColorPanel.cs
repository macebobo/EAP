using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElfControls {
    [DefaultEvent("Click")]
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\ColorPanel.bmp")]
    public class ColorPanel : Label {
        #region [ Private Variables ]

        private Color _color;
        private bool _paintColor = true;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public override bool AutoSize {
            get { return false; }
            set { }
        }

        public Color Color {
            get { return _color; }
            set {
                _color = value;
                Invalidate();
            }
        }

        public bool PaintColor {
            get { return _paintColor; }
            set {
                _paintColor = value;
                Invalidate();
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public ColorPanel() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        #endregion [ Constructors ]

        #region [ Events ]

        #region [ Event Triggers ]

        protected override void OnPaint(PaintEventArgs e) {
            if (!_paintColor || _color.IsEmpty) {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(BackColor);
                e.Graphics.DrawLine(SystemPens.ControlDarkDark, 0, 0, ClientSize.Width, ClientSize.Height);
                e.Graphics.DrawLine(SystemPens.ControlDarkDark, ClientSize.Width, 0, 0, ClientSize.Height);
                return;
            }

            using (var HatchBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.Silver)) {
                e.Graphics.FillRectangle(HatchBrush, ClientRectangle);
            }

            using (var FillBrush = new SolidBrush(_color)) {
                e.Graphics.FillRectangle(FillBrush, ClientRectangle);
            }
        }

        #endregion [ Event Triggers ]

        #endregion [ Events ]
    }
}