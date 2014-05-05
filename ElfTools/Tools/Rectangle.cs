using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Rectangle")]
    [ElfToolCore]
    public class RectangleTool : ShapeBase, ITool {
        #region [ Enums ]

        #region Nested type: CornerStyle

        private enum CornerStyle {
            NotSet,
            Square,
            Round
        }

        #endregion

        #region Nested type: Corners

        [Flags]
        private enum Corners {
            None = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 4,
            BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        #endregion

        #endregion [ Enums ]

        #region [ Private Variables ]

        // Settings from the ToolStrip
        private int _cornerRadius = -1;
        private CornerStyle _cornerStyle = CornerStyle.NotSet;
        private ToolStripLabel _txtCornerRadius;

        // Controls from ToolStrip
        private ToolStripButton cmdRoundedCorner;
        private ToolStripButton cmdSquareCorner;
        private ToolStripTextBox txtCornerRadius;

        #endregion [ Private Variables ]

        #region [ Constants ]

        public const string CORNER_STYLE = "CornerStyle";

        #endregion [ Constants ]

        #region [ Constructors ]

        public RectangleTool() {
            ID = (int) ToolID.Rectangle;
            Name = "Rectangle";
            ToolBoxImage = ElfRes.rectangle;
            ToolBoxImageSelected = ElfRes.rectangle_selected;
            base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.rectangle_modifier, new Point(15, 15));
            MultiGestureKey1 = Keys.Shift | Keys.S;
            MultiGestureKey2 = Keys.R;

            _noFillControlName = "Rectangle_cmdNoFill";
            _yesFillControlName = "Rectangle_cmdFill";
            _dashStyleControlName = "Rectangle_cboDashStyle";
            _lineThicknessControlName = "Rectangle_cboLineSize";
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
            _cornerRadius = LoadValue(Constants.RADIUS, 0);
            _cornerStyle = EnumHelper.GetEnumFromValue<CornerStyle>(LoadValue(CORNER_STYLE, (int) CornerStyle.Square));

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            txtCornerRadius = (ToolStripTextBox) GetItem<ToolStripTextBox>("Rectangle_txtCornerRadius");
            _txtCornerRadius = (ToolStripLabel) GetItem<ToolStripLabel>("_Rectangle_txtCornerRadius");

            cmdSquareCorner = (ToolStripButton) GetItem<ToolStripButton>("Rectangle_cmdSquareCorner");
            cmdRoundedCorner = (ToolStripButton) GetItem<ToolStripButton>("Rectangle_cmdRoundedCorner");

            AddButtonFaces(cmdSquareCorner.Name, new ButtonImages(ElfRes.square_corner, ElfRes.square_corner_selected));
            AddButtonFaces(cmdRoundedCorner.Name, new ButtonImages(ElfRes.rounded_corner, ElfRes.rounded_corner_selected));

            // Assign the DashStyle enum value to the tag of each dash menu item
            SetDashStyleDropDownButton(cboDashStyle);

            // Set the initial value for the contol from what we had retrieve from Settings
            cmdSquareCorner.Checked = (_cornerStyle == CornerStyle.Square);
            cmdRoundedCorner.Checked = (_cornerStyle == CornerStyle.Round);
            txtCornerRadius.Enabled = cmdRoundedCorner.Checked;
            _txtCornerRadius.Enabled = cmdRoundedCorner.Checked;

            SetToolbarSelectedImage(cmdSquareCorner);
            SetToolbarSelectedImage(cmdRoundedCorner);

            txtCornerRadius.Text = _cornerRadius.ToString();
        }


        // http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/d805a3fd-79a7-42c2-9416-83df5df04ca8


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(Constants.LINE_THICKNESS, _lineThickness);
            SaveValue(Constants.FILL, _fill);
            SaveValue(Constants.DASH_STYLE, (int) _dashStyle);
            SaveValue(CORNER_STYLE, (int) _cornerStyle);
            SaveValue(Constants.RADIUS, _cornerRadius);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cmdDoFill = null;
            cmdDoNotFill = null;
            cmdSquareCorner = null;
            cmdRoundedCorner = null;
            cboLineThickness = null;
            cboDashStyle = null;
            txtCornerRadius = null;
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
                cmdSquareCorner.Click += cmdSquareCorner_Click;
                cmdRoundedCorner.Click += cmdRoundCorner_Click;
                txtCornerRadius.Leave += txtCornerRadius_Leave;
                txtCornerRadius.KeyPress += _toolStrip_Form.NumberOnly_KeyPress;
            }
            else {
                cmdSquareCorner.Click -= cmdSquareCorner_Click;
                txtCornerRadius.KeyPress -= _toolStrip_Form.NumberOnly_KeyPress;
                cmdRoundedCorner.Click -= cmdRoundCorner_Click;
                txtCornerRadius.Leave -= txtCornerRadius_Leave;
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Create the graphics path needed to draw the path.
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        protected override GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender) {
            var DrawPath = new GraphicsPath();
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);

            //if (finalRender)
            //{
            //    System.Diagnostics.Debug.WriteLine(p1);
            //    System.Diagnostics.Debug.WriteLine(p2);
            //    System.Diagnostics.Debug.WriteLine(DrawArea);
            //}

            float CellScale = Scaling.CellScaleF;
            float Radius = _cornerRadius * CellScale;

            if (!finalRender) {
                Radius *= CellScale;
            }

            if ((_cornerStyle == CornerStyle.Round) && (_cornerRadius > 0)) {
                DrawPath = RoundedRectangle(DrawArea, Radius, Corners.All);
            }
            else {
                DrawPath.AddRectangle(DrawArea);
            }

            return DrawPath;
        }


        private GraphicsPath RoundedRectangle(Rectangle bounds, float radius, Corners corners) {
            // Make sure the Path fits inside the rectangle
            //bounds.Width--;
            //bounds.Height--;

            // Scale the radius if it's too large to fit.
            if (radius > (bounds.Width)) {
                radius = bounds.Width;
            }
            if (radius > (bounds.Height)) {
                radius = bounds.Height;
            }

            var path = new GraphicsPath();

            if (radius <= 0) {
                path.AddRectangle(bounds);
            }
            else {
                if ((corners & Corners.TopLeft) == Corners.TopLeft) {
                    path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
                }
                else {
                    path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Top);
                }

                if ((corners & Corners.TopRight) == Corners.TopRight) {
                    path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
                }
                else {
                    path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Top);
                }

                if ((corners & Corners.BottomRight) == Corners.BottomRight) {
                    path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                }
                else {
                    path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);
                }

                if ((corners & Corners.BottomLeft) == Corners.BottomLeft) {
                    path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
                }
                else {
                    path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Bottom);
                }
            }

            path.CloseFigure();
            return path;
        }

        #endregion [ Methods ]

        private void cmdSquareCorner_Click(object sender, EventArgs e) {
            if (cmdSquareCorner.Checked) {
                return;
            }
            cmdRoundedCorner.Checked = false;
            cmdSquareCorner.Checked = true;
            _cornerStyle = CornerStyle.Square;
            txtCornerRadius.Enabled = false;
            _txtCornerRadius.Enabled = false;
            SetToolbarSelectedImage(cmdSquareCorner);
            SetToolbarSelectedImage(cmdRoundedCorner);
        }


        private void cmdRoundCorner_Click(object sender, EventArgs e) {
            if (cmdRoundedCorner.Checked) {
                return;
            }
            cmdSquareCorner.Checked = false;
            cmdRoundedCorner.Checked = true;
            _cornerStyle = CornerStyle.Round;
            txtCornerRadius.Enabled = true;
            _txtCornerRadius.Enabled = true;
            SetToolbarSelectedImage(cmdSquareCorner);
            SetToolbarSelectedImage(cmdRoundedCorner);
        }


        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void txtCornerRadius_Leave(object sender, EventArgs e) {
            _cornerRadius = ValidateInteger((ToolStripTextBox) sender, _cornerRadius);
        }
    }
}