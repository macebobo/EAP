using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Spray")]
    [ElfToolCore]
    public class SprayTool : BaseTool, ITool {
        #region [ Private Variables ]

        // Settings from the ToolStrip

        // Used for rendering
        private readonly Random _random = new Random();
        private Rectangle _bounds = Rectangle.Empty;
        private bool _hasMask;
        private int _intensity = 3;
        private LockBitmap _lockBitmap;

        // Masking variables
        private Region _maskRegion;
        private Rectangle _nibRect;
        private SolidBrush _paintBrush;
        private int _radius = 5;
        private TrackBar tbIntensityTracker;
        private Timer tmrSpray;
        private ToolStripTextBox txtRadius;

        #endregion [ Private Variables ]

        #region [ Constants ]

        private const string INTENSITY = "Intensity";
        private const string DEFAULT_RADIUS = "5";

        #endregion [ Constants ]

        #region [ Properties ]

        /// <summary>
        ///     Cursor to use on the Canvas window when the mouse is within its bounds. A safe cursor to use might be:
        ///     Cursors.Cross
        /// </summary>
        public override Cursor Cursor {
            get {
                float Radius = _radius * Scaling.CellScaleF;
                float Width = 0;
                float Height = 0;
                PointF CenterPt;

                if (Radius >= 16) {
                    Width = 2 * (Radius + 1);
                    Height = 2 * (Radius + 1);
                    CenterPt = new PointF(Radius + 1, Radius + 1);
                }
                else {
                    // compositing the fuzzy circle with the spray can image
                    Width = 2 * (Radius + 1) + 16;
                    Height = 2 * (Radius + 1) + 13;
                    CenterPt = new PointF((Radius + 1) + 16, (Radius + 1));
                }

                var CursorBmp = new Bitmap((int) Width, (int) Height);

                Graphics g = Graphics.FromImage(CursorBmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Draw concentric circles of white with increasing transparency towards the outside
                var AlphaRad = (float) Math.Ceiling(255 / Radius);

                byte Alpha = 0;
                if (!Byte.TryParse(AlphaRad.ToString(), out Alpha)) {
                    Alpha = 128;
                }

                Color AlphaColor = Color.FromArgb(Alpha, Color.White);

                for (int i = 0; i <= Radius; i++) {
                    g.FillEllipse(new SolidBrush(AlphaColor), new RectangleF(CenterPt.X - i, CenterPt.Y - i, i * 2, i * 2));
                }
                g.DrawImage(ElfRes.spray, new Point(0, (int) CenterPt.Y - 3));
                g.Dispose();

                if (base.Cursor != null) {
                    CustomCursors.DestroyCreatedCursor(base.Cursor.Handle);
                }

                base.Cursor = CustomCursors.CreateCursor(CursorBmp, (int) CenterPt.X, (int) CenterPt.Y);

                return base.Cursor;
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public SprayTool() {
            ID = (int) ToolID.Spray;
            Name = "Spray";
            ToolBoxImage = ElfRes.spray;
            ToolBoxImageSelected = ElfRes.spray_selected;
            MultiGestureKey1 = Keys.A;
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
            _radius = LoadValue(Constants.RADIUS, _radius);
            _intensity = LoadValue(INTENSITY, _intensity);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Create the track bar control, insert it into a new ToolStripControlHost control, then
            // insert it onto the controlbar before the last item.
            tbIntensityTracker = new TrackBar();
            tbIntensityTracker.AutoSize = false;
            tbIntensityTracker.Size = new Size(60, 20);
            tbIntensityTracker.Maximum = 3;
            tbIntensityTracker.LargeChange = 1;
            tbIntensityTracker.TickStyle = TickStyle.None;
            var item = new ToolStripControlHost(tbIntensityTracker);
            item.AutoSize = true;
            SettingsToolStrip.Items.Insert(SettingsToolStrip.Items.Count - 1, item);

            // Get a pointer to the controls on the toolstrip that belongs to us.
            txtRadius = (ToolStripTextBox) GetItem<ToolStripTextBox>("Spray_txtRadius");
            tmrSpray = ((Tools_ToolStripContainer) _toolStrip_Form).ToolTimer;

            //tbIntensityTracker = ((ToolStripControlHost)GetItem<ToolStripControlHost>(2)).Control as TrackBar;

            // Set the initial value for the contol from what we had retrieve from Settings
            txtRadius.Text = _radius.ToString();
            tbIntensityTracker.Value = _intensity;
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            base.MouseDown(buttons, latticePoint, actualCanvasPoint);

            _hasMask = Profile.HasMask;
            if (_hasMask) {
                _maskRegion = new Region(Profile.GetMaskOutline(UnitScale.Lattice));
            }

            _paintBrush = Profile.Channels.Active.GetBrush();
            _nibRect = new Rectangle(0, 0, 1, 1);

            Size LatticeSize = Scaling.LatticeSize;

            _latticeBuffer = new Bitmap(LatticeSize.Width, LatticeSize.Height);
            _bounds = new Rectangle(new Point(0, 0), LatticeSize);

            SetMaskClip();

            _lockBitmap = new LockBitmap(_latticeBuffer);
#if DEBUG
            Workshop.Instance.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
#endif

            tmrSpray.Enabled = true;
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

            _currentLatticePoint = latticePoint;
            return true;
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint)) {
                return false;
            }

            tmrSpray.Enabled = false;
            _canvasControlGraphics.ResetClip();

            foreach (Channel Channel in Profile.Channels.Selected) {
                Profile.Channels.PopulateChannelFromBitmap(_latticeBuffer, Channel, false);
                Channel.DedupeData();
            }

            Profile.Refresh();

#if DEBUG
            _workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
#endif

            PostDrawCleanUp();
            return true;
        }


        /// <summary>
        ///     Method that fires when this Tool is selected in the ToolBox.
        ///     For this tool, it sets the timer interval to 1
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();

            tmrSpray.Enabled = false;
            tmrSpray.Interval = 1;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(Constants.RADIUS, _radius);
            SaveValue(INTENSITY, _intensity);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();
            txtRadius = null;
        }


        /// <summary>
        ///     Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a
        ///     ShutDown.
        /// </summary>
        public override void OnUnselected() {
            tmrSpray.Enabled = false;
            base.OnUnselected();
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
                txtRadius.Leave += txtRadius_Leave;
                txtRadius.KeyPress += _toolStrip_Form.NumberOnly_KeyPress;
                tbIntensityTracker.Scroll += tbIntensityTracker_Scroll;
                tmrSpray.Tick += tmrSpray_Tick;
                if (Profile != null) {
                    Profile.ScalingChanged += Profile_ScalingChanged;
                }
            }
            else {
                txtRadius.Leave -= txtRadius_Leave;
                txtRadius.KeyPress -= _toolStrip_Form.NumberOnly_KeyPress;
                tbIntensityTracker.Scroll -= tbIntensityTracker_Scroll;
                tmrSpray.Tick -= tmrSpray_Tick;
                if (Profile != null) {
                    Profile.ScalingChanged -= Profile_ScalingChanged;
                }
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Checks to see if there is a mask in place. If so, is this point visible therein?
        /// </summary>
        /// <param name="pt">Point to check</param>
        private bool IsValidMaskPoint(Point pt) {
            if (!_hasMask) {
                return true;
            }
            return _maskRegion.IsVisible(pt);
        }


        /// <summary>
        ///     Paint the spray point onto the painting bitmap and the canvas
        /// </summary>
        /// <param name="pt">Location (in cells) in which to paint</param>
        private void PaintPoint(Point pt) {
            if (!(_bounds.Contains(pt) && IsValidMaskPoint(pt))) {
                return;
            }

            _lockBitmap.SetPixel(pt.X, pt.Y, Color.White);
            int CellZoom = Scaling.CellZoom;
            Point P = _workshop.CalcCanvasPoint(pt);
            var Grid = (int) Scaling.GridLineWidthZoom;
            P.Offset(Grid, Grid);
            _canvasControlGraphics.FillRectangle(_paintBrush, new Rectangle(P, new Size(CellZoom, CellZoom)));
        }


        /// <summary>
        ///     Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
        /// </summary>
        protected override void PostDrawCleanUp() {
            if (_paintBrush != null) {
                _paintBrush.Dispose();
                _paintBrush = null;
            }
            if (_maskRegion != null) {
                _maskRegion.Dispose();
                _maskRegion = null;
            }
            _lockBitmap = null;
            base.PostDrawCleanUp();
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void txtRadius_Leave(object sender, EventArgs e) {
            if (txtRadius.TextLength == 0) {
                txtRadius.Text = DEFAULT_RADIUS;
            }

            _radius = ValidateInteger(txtRadius, _radius);
            Profile.Cursor = Cursor;
        }


        /// <summary>
        ///     Spray out a number of points onto the Channel
        /// </summary>
        private void tmrSpray_Tick(object sender, EventArgs e) {
            int X = 0;
            int Y = 0;
            int R = 0;
            int Theta = 0;

            float Darkness = (_radius * (float) _intensity / 4f);
            if (Darkness < 1) {
                Darkness = 1f;
            }

            _lockBitmap.LockBits();

            for (int i = 0; i < (int) Math.Round(Darkness); i++) {
                R = _random.Next(_radius);
                Theta = _random.Next(360);
                X = (int) (_currentLatticePoint.X + R * (float) Math.Cos(_workshop.DegreeToRadian(Theta)));
                Y = (int) (_currentLatticePoint.Y + R * (float) Math.Sin(_workshop.DegreeToRadian(Theta)));
                PaintPoint(new Point(X, Y));
            }

            _lockBitmap.UnlockBits();
        }


        /// <summary>
        ///     Event fires when the slider of the track bar is moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbIntensityTracker_Scroll(object sender, EventArgs e) {
            _intensity = tbIntensityTracker.Value;
        }


        /// <summary>
        ///     Occurs when one of the scaling variables within the Profile changes.
        /// </summary>
        private void Profile_ScalingChanged(object sender, EventArgs e) {
            Profile.Cursor = Cursor;
        }
    }
}