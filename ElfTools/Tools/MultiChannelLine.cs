using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Multi-Channel Line")]
    [ElfToolCore]
    public class MultiChannelLineTool : MultiChannelBase, ITool {
        #region [ Constants ]

        private const string DEGREE_SIGN = "°";

        #endregion

        #region [ Enums ]

        private enum MultiChannelLineArrangement {
            NotSet = -1,
            Arch = 0,
            Line = 1,
            Circle,
            Fan,
            Star,
            StarBurst
        }

        #endregion [ Enums ]

        #region [ Private Variables ]

        // Settings from the ToolStrip
        private Cursor _archCursor;
        private MultiChannelLineArrangement _arrangement = MultiChannelLineArrangement.Arch;
        private List<ToolStripButton> _arrangementButtonList;

        /// <summary>
        ///     Indicates whether we can use the ReversePathList() method if the travel indication is anti-clockwise/right to left
        /// </summary>
        private bool _canUseReversePathing = true;

        private Cursor _circleCursor;
        private Cursor _defaultCursor;

        private bool _detailedLayout;
        private TravelDirection _direction = TravelDirection.LeftToRight;
        private Cursor _fanCursor;
        private Cursor _lineCursor;
        private int _lineThickness = 1;
        private int _numChannels;
        private Cursor _starCursor;
        private float _startAngle = float.MaxValue;
        private ToolStripComboBox cboLineThickness;

        // Controls from ToolStrip
        private ToolStripButton cmdDetailedLayout;
        private ToolStripButton cmdReverseOrder;
        private ToolStripButton cmdSimpleLayout;
        private ToolStripButton cmdStandardOrder;
        private ToolStripLabel lblStartAngle;
        private ToolStripLabel lblWarning;
        private ToolStripTextBox txtNumChannels;
        private ToolStripTextBox txtStartAngle;

        #endregion [ Private Variables ]

        #region [ Constants ]

        private const string ARRANGEMENT = "Arrangement";
        private const string LAYOUT_DETAILED = "LayoutDetailed";

        #endregion [ Constants ]

        #region [ Properties ]

        /// <summary>
        ///     Checks to see if we are reversing the standard channel order (ie Right to Left or Anti-Clockwise)
        /// </summary>
        private bool IsReverseOrder {
            get { return (_direction == TravelDirection.Anticlockwise); }
        }

        /// <summary>
        ///     Cursor to use on the Canvas window when the mouse is within its bounds.
        ///     Returns the cursor based on the arrangement selected
        /// </summary>
        public override Cursor Cursor {
            get {
                switch (_arrangement) {
                    case MultiChannelLineArrangement.Arch:
                        base.Cursor = _archCursor;
                        break;

                    case MultiChannelLineArrangement.Circle:
                        base.Cursor = _circleCursor;
                        break;

                    case MultiChannelLineArrangement.Fan:
                        base.Cursor = _fanCursor;
                        break;

                    case MultiChannelLineArrangement.Line:
                        base.Cursor = _lineCursor;
                        break;

                    case MultiChannelLineArrangement.Star:
                    case MultiChannelLineArrangement.StarBurst:
                        base.Cursor = _starCursor;
                        break;

                    default:
                        base.Cursor = _defaultCursor;
                        break;
                }
                return base.Cursor;
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public MultiChannelLineTool() {
            ID = (int) ToolID.MultiChannelLine;
            Name = "Multi-Channel Line";
            ToolBoxImage = ElfRes.multichannel_lines;
            ToolBoxImageSelected = ElfRes.multichannel_lines_selected;
            MultiGestureKey1 = Keys.Shift | Keys.H;
            MultiGestureKey2 = Keys.L;

            _archCursor = CreateCursor(ElfRes.cross_base, ElfRes.arch_modifier, new Point(15, 15));
            _circleCursor = CreateCursor(ElfRes.cross_base, ElfRes.ellipse_modifier, new Point(15, 15));
            _lineCursor = CreateCursor(ElfRes.cross_base, ElfRes.line_modifier, new Point(15, 15));
            _fanCursor = CreateCursor(ElfRes.cross_base, ElfRes.fan_modifier, new Point(15, 15));
            _starCursor = CreateCursor(ElfRes.cross_base, ElfRes.star_modifier, new Point(15, 15));
            _defaultCursor = CreateCursor(ElfRes.cross_base, new Point(15, 15));
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
            _arrangementButtonList = new List<ToolStripButton>();
            base.Initialize();

            // Load the Settings values
            _lineThickness = LoadValue(Constants.LINE_THICKNESS, 1);
            _numChannels = LoadValue(Constants.NUM_ChannelS, 8);
            _startAngle = LoadValue(Constants.START_ANGLE, 0f);
            _arrangement = EnumHelper.GetEnumFromValue<MultiChannelLineArrangement>(LoadValue(ARRANGEMENT, (int) MultiChannelLineArrangement.Arch));
            _direction = EnumHelper.GetEnumFromValue<TravelDirection>(LoadValue(Constants.DIRECTION, (int) TravelDirection.LeftToRight));
            _detailedLayout = LoadValue(LAYOUT_DETAILED, false);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            cboLineThickness = (ToolStripComboBox) GetItem<ToolStripComboBox>("MCL_cboLineSize");
            cmdStandardOrder = (ToolStripButton) GetItem<ToolStripButton>("MCL_cmdStandardOrder");
            cmdReverseOrder = (ToolStripButton) GetItem<ToolStripButton>("MCL_cmdReverseOrder");
            txtNumChannels = (ToolStripTextBox) GetItem<ToolStripTextBox>("MCL_txtNumChannels");
            txtStartAngle = (ToolStripTextBox) GetItem<ToolStripTextBox>("MCL_txtStartAngle");
            lblStartAngle = (ToolStripLabel) GetItem<ToolStripLabel>("_MCL_txtStartAngle");

            cmdSimpleLayout = (ToolStripButton) GetItem<ToolStripButton>("MCL_cmdSimpleLayout");
            cmdDetailedLayout = (ToolStripButton) GetItem<ToolStripButton>("MCL_cmdDetailedLayout");

            lblWarning = (ToolStripLabel) GetItem<ToolStripLabel>("MCL_lblWarning");

            // Add and Attach all the different arrangement style buttons
            AddArrangementButton(MultiChannelLineArrangement.Arch);
            AddArrangementButton(MultiChannelLineArrangement.Line);
            AddArrangementButton(MultiChannelLineArrangement.Circle);
            AddArrangementButton(MultiChannelLineArrangement.Fan);
            AddArrangementButton(MultiChannelLineArrangement.Star);
            AddArrangementButton(MultiChannelLineArrangement.StarBurst);

            // Set all the selected images for the control
            AddButtonFaces(cmdStandardOrder.Name, new ButtonImages(ElfRes.channel, ElfRes.channels_selected));
            AddButtonFaces(cmdReverseOrder.Name, new ButtonImages(ElfRes.channel, ElfRes.channels_selected));
            AddButtonFaces(cmdSimpleLayout.Name, new ButtonImages(ElfRes.layout_simple, ElfRes.layout_simple_selected));
            AddButtonFaces(cmdDetailedLayout.Name, new ButtonImages(ElfRes.layout_detail, ElfRes.layout_detail_selected));

            // Set the initial value for the contol from what we had retrieve from Settings
            cboLineThickness.SelectedIndex = _lineThickness - 1;
            cmdStandardOrder.Checked = (_direction == TravelDirection.LeftToRight);
            cmdReverseOrder.Checked = (_direction == TravelDirection.RightToLeft);
            txtNumChannels.Text = _numChannels.ToString();
            txtStartAngle.Text = _startAngle.ToString("0.0") + DEGREE_SIGN;
            lblWarning.Visible = false;

            cmdSimpleLayout.Checked = !_detailedLayout;
            cmdDetailedLayout.Checked = _detailedLayout;

            SetToolbarSelectedImage(cmdSimpleLayout);
            SetToolbarSelectedImage(cmdDetailedLayout);

            if (_arrangement == MultiChannelLineArrangement.Line) {
                txtStartAngle.Enabled = false;
                lblStartAngle.Enabled = false;
            }

            SetChannelDirectionButtons();
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(Constants.LINE_THICKNESS, _lineThickness);
            SaveValue(Constants.NUM_ChannelS, _numChannels);
            SaveValue(Constants.START_ANGLE, _startAngle);
            SaveValue(ARRANGEMENT, (int) _arrangement);
            SaveValue(Constants.DIRECTION, (int) _direction);
            SaveValue(LAYOUT_DETAILED, _detailedLayout);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cmdStandardOrder = null;
            cmdReverseOrder = null;
            cboLineThickness = null;
            txtNumChannels = null;
            txtStartAngle = null;
            _arrangementButtonList = null;

            lblWarning = null;
            lblStartAngle = null;

            if (_archCursor != null) {
                CustomCursors.DestroyCreatedCursor(_archCursor);
                _archCursor.Dispose();
                _archCursor = null;
            }
            if (_circleCursor != null) {
                CustomCursors.DestroyCreatedCursor(_circleCursor);
                _circleCursor.Dispose();
                _circleCursor = null;
            }
            if (_lineCursor != null) {
                CustomCursors.DestroyCreatedCursor(_lineCursor);
                _lineCursor.Dispose();
                _lineCursor = null;
            }
            if (_fanCursor != null) {
                CustomCursors.DestroyCreatedCursor(_fanCursor);
                _fanCursor.Dispose();
                _fanCursor = null;
            }
            if (_starCursor != null) {
                CustomCursors.DestroyCreatedCursor(_starCursor);
                _starCursor.Dispose();
                _starCursor = null;
            }
            if (_defaultCursor != null) {
                CustomCursors.DestroyCreatedCursor(_defaultCursor);
                _defaultCursor.Dispose();
                _defaultCursor = null;
            }
        }


        /// <summary>
        ///     Find the button that corresponds to this particular arrangement, add it to the list of buttons.
        ///     Set its Tag with the enum passed in, and if that enum value matches the value of the one from settings,
        ///     check the button.
        /// </summary>
        /// <param name="arr">Arrangement enum value to set as the tag and to check to see if the button is to be set to Checked</param>
        private void AddArrangementButton(MultiChannelLineArrangement arr) {
            var Button = (ToolStripButton) GetItem<ToolStripButton>("MCL_" + arr);
            Button.Tag = arr;
            Button.Checked = (arr == _arrangement);
            _arrangementButtonList.Add(Button);
            AddButtonFaces(Button.Name, new ButtonImages(GetArrangementBitmap(arr, false), GetArrangementBitmap(arr, true)));

            AddButtonFaces(cmdStandardOrder.Name + "_" + arr,
                new ButtonImages(GetArrangementDirectionBitmap(arr, true, false), GetArrangementDirectionBitmap(arr, true, true)));
            AddButtonFaces(cmdReverseOrder.Name + "_" + arr,
                new ButtonImages(GetArrangementDirectionBitmap(arr, false, false), GetArrangementDirectionBitmap(arr, false, true)));
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
                cboLineThickness.SelectedIndexChanged += LineThickness_SelectedIndexChanged;
                cmdStandardOrder.Click += StandardOrder_Click;
                cmdReverseOrder.Click += ReverseOrder_Click;
                txtNumChannels.Leave += NumChannels_Leave;
                txtNumChannels.KeyPress += _toolStrip_Form.NumberOnly_KeyPress;
                txtStartAngle.Leave += StartAngle_Leave;
                txtStartAngle.KeyPress += _toolStrip_Form.SignedFloatOnly_KeyPress;
                cmdSimpleLayout.Click += SimpleLayout_Click;
                cmdDetailedLayout.Click += DetailedLayout_Click;

                foreach (ToolStripButton Button in _arrangementButtonList) {
                    Button.Click += Arrangement_Click;
                }
            }
            else {
                cboLineThickness.SelectedIndexChanged -= LineThickness_SelectedIndexChanged;
                cmdStandardOrder.Click -= StandardOrder_Click;
                cmdReverseOrder.Click -= ReverseOrder_Click;
                txtNumChannels.Leave -= NumChannels_Leave;
                txtNumChannels.KeyPress -= _toolStrip_Form.NumberOnly_KeyPress;
                txtStartAngle.Leave -= StartAngle_Leave;
                txtStartAngle.KeyPress -= _toolStrip_Form.SignedFloatOnly_KeyPress;
                cmdSimpleLayout.Click -= SimpleLayout_Click;
                cmdDetailedLayout.Click -= DetailedLayout_Click;

                foreach (ToolStripButton Button in _arrangementButtonList) {
                    Button.Click -= Arrangement_Click;
                }
            }

            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Generate the series of Graphics Paths, one per Channel. If this is not the final render, all paths are merged
        ///     together
        ///     into a single one.
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        protected override List<GraphicsPath> CreateRenderPathList(Point p1, Point p2, bool finalRender) {
            List<GraphicsPath> Paths = null;

            switch (_arrangement) {
                case MultiChannelLineArrangement.Arch:
                    Paths = ArchPaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.Circle:
                    Paths = CirclePaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.Fan:
                    Paths = FanPaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.Line:
                    Paths = LinePaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.Star:
                    Paths = StarPaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.StarBurst:
                    Paths = StarBurstPaths(p1, p2, finalRender);
                    break;

                case MultiChannelLineArrangement.NotSet:
                default:
                    return null;
            }

            if (IsReverseOrder && _canUseReversePathing) // this handles right to left as well
            {
                Paths = ReversePathList(Paths);
            }

            return Paths;
        }


        /// <summary>
        ///     Returns the appropriate bitmap for the arrangement and selected status.
        /// </summary>
        private Bitmap GetArrangementBitmap(MultiChannelLineArrangement arr, bool selected) {
            switch (arr) {
                case MultiChannelLineArrangement.Arch:
                    return selected ? ElfRes.multichannel_arch_selected : ElfRes.multichannel_arch;

                case MultiChannelLineArrangement.Circle:
                    return selected ? ElfRes.multichannel_circle_selected : ElfRes.multichannel_circle;

                case MultiChannelLineArrangement.Fan:
                    return selected ? ElfRes.multichannel_fan_selected : ElfRes.multichannel_fan;

                case MultiChannelLineArrangement.Line:
                    return selected ? ElfRes.multichannel_line_selected : ElfRes.multichannel_line;

                case MultiChannelLineArrangement.Star:
                    return selected ? ElfRes.multichannel_star_selected : ElfRes.multichannel_star;

                case MultiChannelLineArrangement.StarBurst:
                    return selected ? ElfRes.multichannel_starcenter_selected : ElfRes.multichannel_starcenter;

                default:
                    return ElfRes.undefined;
            }
        }


        private Bitmap GetArrangementDirectionBitmap(MultiChannelLineArrangement arr, bool left_clockwise, bool selected) {
            switch (arr) {
                case MultiChannelLineArrangement.Circle:
                case MultiChannelLineArrangement.Star:
                case MultiChannelLineArrangement.StarBurst:
                    if (left_clockwise) {
                        return selected ? ElfRes.clockwise_selected : ElfRes.clockwise;
                    }
                    return selected ? ElfRes.anticlockwise_selected : ElfRes.anticlockwise;

                case MultiChannelLineArrangement.Line:
                    if (left_clockwise) {
                        return selected ? ElfRes.left_selected : ElfRes.left;
                    }
                    return selected ? ElfRes.right_selected : ElfRes.right;

                case MultiChannelLineArrangement.Arch:
                case MultiChannelLineArrangement.Fan:
                    if (left_clockwise) {
                        return selected ? ElfRes.arch_left2right_selected : ElfRes.arch_left2right;
                    }
                    return selected ? ElfRes.arch_right2left_selected : ElfRes.arch_right2left;

                default:
                    return ElfRes.undefined;
            }
        }


        /// <summary>
        ///     Calculate the number of Channels that are needed.
        /// </summary>
        protected override int NumberOfChannels() {
            return _numChannels;
        }


        /// <summary>
        ///     Creates the Pen used to render the shape onto the Paint Pane
        /// </summary>
        protected override Pen RenderPen() {
            return new Pen(Color.White, _lineThickness);
        }


        /// <summary>
        ///     Sets the art and tooltip text for the channel order buttons, depending on the current arrangement.
        /// </summary>
        private void SetChannelDirectionButtons() {
            switch (_arrangement) {
                case MultiChannelLineArrangement.Circle:
                case MultiChannelLineArrangement.Star:
                case MultiChannelLineArrangement.StarBurst:
                    cmdStandardOrder.ToolTipText = "Start populating Channels in a Clockwise order";
                    cmdReverseOrder.ToolTipText = "Start populating Channels in an Anti-Clockwise order";
                    break;

                case MultiChannelLineArrangement.Line:
                    cmdStandardOrder.ToolTipText = "Start populating Channels on the left side, moving right";
                    cmdReverseOrder.ToolTipText = "Start populating Channels on the right side, moving left";
                    break;

                case MultiChannelLineArrangement.Arch:
                case MultiChannelLineArrangement.Fan:
                case MultiChannelLineArrangement.NotSet:
                default:
                    cmdStandardOrder.ToolTipText = "Start populating Channels on the left side, moving right";
                    cmdReverseOrder.ToolTipText = "Start populating Channels on the right side, moving left";
                    break;
            }

            SetToolbarSelectedImage(cmdStandardOrder, cmdStandardOrder.Name + "_" + _arrangement);
            SetToolbarSelectedImage(cmdReverseOrder, cmdReverseOrder.Name + "_" + _arrangement);
        }

        #region [ Arrangement Graphics Path Methods ]

        /// <summary>
        ///     Generate the GraphicPaths for the Arch
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> ArchPaths(Point p1, Point p2, bool finalRender) {
            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
            Rectangle DoubleDraw = DrawArea;
            float StartAngle = _startAngle;
            float SweepAngle = 0;
            float Range = 180 - (StartAngle * 2);
            DoubleDraw.Height *= 2;

            SweepAngle = Range / (_numChannels);
            StartAngle += 180;

            for (int i = 0; i < _numChannels; i++) {
                Path = new GraphicsPath();
                Path.AddArc(DoubleDraw, StartAngle, SweepAngle);
                Path.Flatten();
                Paths.Add(Path);
                StartAngle += SweepAngle;
            }

            if (!finalRender && _detailedLayout) {
                Path = new GraphicsPath();
                Path.AddRectangle(DrawArea);

                var FirstChannelPath = (GraphicsPath) Paths[IsReverseOrder ? Paths.Count - 1 : 0].Clone();

                if (StartAngle != 0) {
                    var Origin = new PointF(DrawArea.Left + DrawArea.Width / 2, DrawArea.Bottom);
                    PointF Arc = FirstChannelPath.PathPoints[IsReverseOrder ? FirstChannelPath.PointCount - 1 : 0];
                    Path.AddLine(Origin, Arc);

                    GraphicsPath LastPath = Paths[IsReverseOrder ? 0 : Paths.Count - 1];
                    Arc = LastPath.PathPoints[IsReverseOrder ? 0 : LastPath.PointCount - 1];
                    Path.AddLine(Origin, Arc);
                }
                Paths.Add(Path);

                // Emphasize the first channel
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }
                Paths.Add(FirstChannelPath);
            }

            _canUseReversePathing = true;
            return Paths;
        }


        /// <summary>
        ///     Generate the GraphicPaths for the Circle
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> CirclePaths(Point p1, Point p2, bool finalRender) {
            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
            float SweepAngle = 0;
            float StartAngle = -_startAngle;

            // Series of arcs around the ellipse defined by the DrawArea.
            SweepAngle = 360f / _numChannels;

            for (int i = 0; i < _numChannels; i++) {
                Path = new GraphicsPath();
                Path.AddArc(DrawArea, StartAngle, SweepAngle);
                Paths.Add(Path);
                StartAngle += SweepAngle;
            }

            if (!finalRender && _detailedLayout) {
                var DetailLayoutPath = new GraphicsPath();
                DetailLayoutPath.AddRectangle(DrawArea);
                if (StartAngle != 0) {
                    var Origin = new PointF(DrawArea.Left + DrawArea.Width / 2, DrawArea.Top + DrawArea.Height / 2);
                    PointF Arc = Paths[0].PathPoints[0];
                    DetailLayoutPath.AddLine(Origin, Arc);
                }

                // Emphasize the first channel
                GraphicsPath FirstChannelPath = null;
                int FirstChannelIdx = 0;
                if (IsReverseOrder) {
                    FirstChannelIdx = Paths.Count - 1;
                }
                FirstChannelPath = (GraphicsPath) Paths[FirstChannelIdx].Clone();
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }

                Paths.Add(DetailLayoutPath);
                Paths.Add(FirstChannelPath);

                DetailLayoutPath = null;
                FirstChannelPath = null;
            }

            _canUseReversePathing = true;
            return Paths;
        }


        /// <summary>
        ///     Generate the GraphicPaths for the Fan
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> FanPaths(Point p1, Point p2, bool finalRender) {
            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
            Rectangle DoubleDraw = DrawArea;
            float SweepAngle = 0;
            float StartAngle = _startAngle;

            // Series of lines from the center of the bottom of the DrawArea, arching over this point with the end
            // points forming an ellipse
            DoubleDraw.Height *= 2;
            var Origin = new Point(DoubleDraw.Left + (DoubleDraw.Width / 2), DoubleDraw.Top + (DoubleDraw.Height / 2));
            float Range = 180 - (StartAngle * 2);
            SweepAngle = Range / (_numChannels - 1);
            StartAngle += 180;

            for (int i = 0; i < _numChannels; i++) {
                Path = new GraphicsPath();
                Path.AddLine(Origin, _workshop.PointFromEllipse(DoubleDraw, StartAngle));
                Paths.Add(Path);
                StartAngle += SweepAngle;
            }

            if (!finalRender && _detailedLayout) {
                var DetailLayoutPath = new GraphicsPath();
                DetailLayoutPath.AddRectangle(DrawArea);
                if (StartAngle != 0) {
                    PointF Arc = Paths[0].PathPoints[0];
                    Path.AddLine(Origin, Arc);

                    GraphicsPath LastPath = Paths[Paths.Count - 1];
                    Arc = LastPath.PathPoints[LastPath.PointCount - 1];
                    DetailLayoutPath.AddLine(Origin, Arc);
                }

                // Emphasize the first channel
                GraphicsPath FirstChannelPath = null;
                int FirstChannelIdx = 0;
                if (IsReverseOrder) {
                    FirstChannelIdx = Paths.Count - 1;
                }
                FirstChannelPath = (GraphicsPath) Paths[FirstChannelIdx].Clone();
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }

                Paths.Add(DetailLayoutPath);
                Paths.Add(FirstChannelPath);

                DetailLayoutPath = null;
                FirstChannelPath = null;
            }

            _canUseReversePathing = true;
            return Paths;
        }


        /// <summary>
        ///     Generate the GraphicPaths for the Line
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> LinePaths(Point p1, Point p2, bool finalRender) {
            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;

            float m = 0f;
            float b = 0f;
            float Delta = Math.Abs(p2.X - p1.X) / (float) _numChannels;

            PointF S1 = PointF.Empty;
            PointF S2 = PointF.Empty;
            float X = Math.Min(p2.X, p1.X);

            // equation of a line: y = mx + b
            // m = y2-y1/x2-x1 = slope
            // b = y - mx for any point on that line

            if (Math.Abs(p2.X - p1.X) != 0) {
                // Not vertical
                m = (p2.Y - p1.Y) / (float) (p2.X - p1.X);
                b = p1.Y - (m * p1.X);

                S1.X = Math.Min(p1.X, p2.X);
                S1.Y = m * S1.X + b;

                for (int i = 0; i < _numChannels; i++) {
                    S2.X = S1.X + Delta;
                    S2.Y = m * S2.X + b;
                    Path = new GraphicsPath();
                    Path.AddLine(S1, S2);
                    Paths.Add(Path);
                    S1 = S2;
                }
            }
            else {
                // Vertical
                Delta = Math.Abs(p2.Y - p1.Y) / (float) _numChannels;
                S1.X = p1.X;
                S2.X = p1.X;
                S1.Y = Math.Min(p1.Y, p2.Y);

                for (int i = 0; i < _numChannels; i++) {
                    S2.Y = S1.Y + Delta;
                    Path = new GraphicsPath();
                    Path.AddLine(S1, S2);
                    Paths.Add(Path);
                    S1 = S2;
                }
            }

            if (!finalRender && _detailedLayout) {
                // Emphasize the first channel
                int FirstChannelIdx = 0;
                if (IsReverseOrder) {
                    FirstChannelIdx = Paths.Count - 1;
                }
                var FirstChannelPath = (GraphicsPath) Paths[FirstChannelIdx].Clone();
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }
                Paths.Add(FirstChannelPath);
            }

            _canUseReversePathing = true;
            return Paths;
        }


        /// <summary>
        ///     Generate the GraphicPaths for the Star
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> StarPaths(Point p1, Point p2, bool finalRender) {
            //if (finalRender)
            //    System.Diagnostics.Debugger.Break();

            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
            float SweepAngle = 0;
            float StartAngle = -_startAngle;

            SweepAngle = 360f / (_numChannels * 2); // measured in degrees

            if (IsReverseOrder) {
                SweepAngle *= -1;
            }

            //StartAngle -= 90; // set the zero point to be vertical

            for (int i = 0; i < _numChannels; i++) {
                Path = new GraphicsPath();
                Path.AddLine(_workshop.PointFromEllipse(DrawArea, StartAngle), _workshop.PointFromEllipse(DrawArea, StartAngle + 180));
                Paths.Add(Path);
                StartAngle += SweepAngle;
            }

            if (!finalRender && _detailedLayout) {
                var DetailLayoutPath = new GraphicsPath();
                DetailLayoutPath.AddRectangle(DrawArea);

                // Emphasize the first channel
                var FirstChannelPath = (GraphicsPath) Paths[0].Clone();
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }

                Paths.Add(DetailLayoutPath);
                Paths.Add(FirstChannelPath);

                DetailLayoutPath = null;
                FirstChannelPath = null;
            }

            _canUseReversePathing = false;
            return Paths;
        }


        /// <summary>
        ///     Generate the GraphicPaths for the StarBurst
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        private List<GraphicsPath> StarBurstPaths(Point p1, Point p2, bool finalRender) {
            var Paths = new List<GraphicsPath>();
            GraphicsPath Path = null;
            Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
            float SweepAngle = 0;
            float StartAngle = -_startAngle;

            var Origin = new Point(DrawArea.Left + (DrawArea.Width / 2), DrawArea.Top + (DrawArea.Height / 2));
            SweepAngle = 360f / _numChannels; // measured in degrees

            //StartAngle -= 90; // set the zero point to be vertical

            if (IsReverseOrder) {
                SweepAngle *= -1;
            }

            for (int i = 0; i < _numChannels; i++) {
                Path = new GraphicsPath();
                Path.AddLine(Origin, _workshop.PointFromEllipse(DrawArea, StartAngle));
                Paths.Add(Path);
                StartAngle += SweepAngle;
            }

            if (!finalRender && _detailedLayout) {
                var DetailLayoutPath = new GraphicsPath();
                DetailLayoutPath.AddRectangle(DrawArea);

                // Emphasize the first channel
                GraphicsPath FirstChannelPath = null;
                int FirstChannelIdx = 0;
                if (IsReverseOrder) {
                    FirstChannelIdx = Paths.Count - 1;
                }
                FirstChannelPath = (GraphicsPath) Paths[FirstChannelIdx].Clone();
                using (var WidePen = new Pen(Color.Red, 2f)) {
                    FirstChannelPath.Widen(WidePen);
                }

                Paths.Add(DetailLayoutPath);
                Paths.Add(FirstChannelPath);

                DetailLayoutPath = null;
                FirstChannelPath = null;
            }

            _canUseReversePathing = false;
            return Paths;
        }


        /// <summary>
        ///     If the travel direction is Right to Left, or Anti-clockwise, we merely need to flip the list.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private List<GraphicsPath> ReversePathList(List<GraphicsPath> paths) {
            var Reverse = new Stack<GraphicsPath>();

            foreach (GraphicsPath P in paths) {
                Reverse.Push(P);
            }

            paths.Clear();

            while (Reverse.Count > 0) {
                paths.Add(Reverse.Pop());
            }

            Reverse = null;

            return paths;
        }

        #endregion [ Arrangement Graphics Path Methods ]

        ///// <summary>
        ///// Changes the art and toottip for the StandardOrder and ReverseOrder buttons to settings valid for circular arrangements
        ///// </summary>
        //private void SetCircularDirectionButtons()
        //{
        //	btnStandardOrder.ToolTipText = "Start populating Channels in a Clockwise order";
        //	btnStandardOrder.Image = ElfRes.clockwise;
        //	btnReverseOrder.ToolTipText = "Start populating Channels in an Anti-Clockwise order";
        //	btnReverseOrder.Image = ElfRes.anticlockwise;
        //}

        ///// <summary>
        ///// Changes the art and toottip for the StandardOrder and ReverseOrder buttons to settings valid for linear arrangements
        ///// </summary>
        //private void SetLinearDirectionButtons()
        //{
        //	btnStandardOrder.ToolTipText = "Start populating Channels on the left side, moving right";
        //	btnStandardOrder.Image = ElfRes.arch_left2right;
        //	btnReverseOrder.ToolTipText = "Start populating Channels on the right side, moving left";
        //	btnReverseOrder.Image = ElfRes.arch_right2left;
        //}

        #endregion [ Methods ]

        private void LineThickness_SelectedIndexChanged(object sender, EventArgs e) {
            string Value = cboLineThickness.SelectedItem.ToString();
            if (Value.Length > 0) {
                _lineThickness = Convert.ToInt32(Value);
            }
        }


        /// <summary>
        ///     Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
        ///     If not, reset the text in the text box with the original value of our variable
        /// </summary>
        private void NumChannels_Leave(object sender, EventArgs e) {
            _numChannels = ValidateInteger((ToolStripTextBox) sender, _numChannels);
        }


        /// <summary>
        ///     Validate that the value entered in the text box is a proper number. If so, set the value and format the text in the
        ///     box with a degree sign
        /// </summary>
        private void StartAngle_Leave(object sender, EventArgs e) {
            string Value = txtStartAngle.Text.Replace(DEGREE_SIGN, string.Empty);

            float Angle = 0;

            if (float.TryParse(Value, out Angle)) {
                _startAngle = Angle;
            }

            txtStartAngle.Text = _startAngle.ToString("0.0") + DEGREE_SIGN;
        }


        /// <summary>
        ///     Indicates which arrangement to use.
        /// </summary>
        private void Arrangement_Click(object sender, EventArgs e) {
            var Arrangement = (ToolStripButton) sender;
            _arrangement = (MultiChannelLineArrangement) Arrangement.Tag;

            for (int i = 0; i < _arrangementButtonList.Count; i++) {
                _arrangementButtonList[i].Checked = ((MultiChannelLineArrangement) _arrangementButtonList[i].Tag == _arrangement);
                SetToolbarSelectedImage(_arrangementButtonList[i]);
            }
            Profile.Cursor = Cursor;

            bool IsLineTool = (_arrangement == MultiChannelLineArrangement.Line);

            txtStartAngle.Enabled = !IsLineTool;
            lblStartAngle.Enabled = !IsLineTool;

            SetChannelDirectionButtons();
        }


        /// <summary>
        ///     Standard channel order, equivalent to clockwise for circular arrangements
        /// </summary>
        private void StandardOrder_Click(object sender, EventArgs e) {
            if (cmdStandardOrder.Checked) {
                return;
            }
            cmdReverseOrder.Checked = false;
            cmdStandardOrder.Checked = true;
            _direction = TravelDirection.LeftToRight;

            SetToolbarSelectedImage(cmdStandardOrder, cmdStandardOrder.Name + "_" + _arrangement);
            SetToolbarSelectedImage(cmdReverseOrder, cmdReverseOrder.Name + "_" + _arrangement);
        }


        /// <summary>
        ///     Reversed channel order, equivalent to anticlockwise for circular arrangements
        /// </summary>
        private void ReverseOrder_Click(object sender, EventArgs e) {
            if (cmdReverseOrder.Checked) {
                return;
            }
            cmdStandardOrder.Checked = false;
            cmdReverseOrder.Checked = true;
            _direction = TravelDirection.RightToLeft;

            SetToolbarSelectedImage(cmdStandardOrder, cmdStandardOrder.Name + "_" + _arrangement);
            SetToolbarSelectedImage(cmdReverseOrder, cmdReverseOrder.Name + "_" + _arrangement);
        }


        /// <summary>
        ///     Simple rendering for layout
        /// </summary>
        private void SimpleLayout_Click(object sender, EventArgs e) {
            if (cmdSimpleLayout.Checked) {
                return;
            }
            cmdDetailedLayout.Checked = false;
            cmdSimpleLayout.Checked = true;
            _detailedLayout = false;
            SetToolbarSelectedImage(cmdDetailedLayout);
            SetToolbarSelectedImage(cmdSimpleLayout);
        }


        /// <summary>
        ///     Simple rendering for layout
        /// </summary>
        private void DetailedLayout_Click(object sender, EventArgs e) {
            if (cmdDetailedLayout.Checked) {
                return;
            }
            cmdDetailedLayout.Checked = true;
            cmdSimpleLayout.Checked = false;
            _detailedLayout = true;
            SetToolbarSelectedImage(cmdDetailedLayout);
            SetToolbarSelectedImage(cmdSimpleLayout);
        }
    }
}