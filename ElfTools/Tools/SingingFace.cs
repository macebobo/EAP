using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Singing Face")]
    [ElfToolCore]
    public class SingingFaceTool : MultiChannelBase, ITool {
        #region [ Private Variables ]

        // Settings from the ToolStrip
        private int _numChannelsForEyes;
        private int _numChannelsForMouth;

        // Controls from ToolStrip
        private ToolStripComboBox cboNumChannelsForEyes;
        private ToolStripComboBox cboNumChannelsForMouth;
        private ToolStripLabel lblWarning;

        #endregion [ Private Variables ]

        #region [ Constants ]

        private const string NUM_ChannelS_EYES = "NumberOfChannelsInEyes";
        private const string NUM_ChannelS_MOUTH = "NumberOfChannelsInMouth";

        #endregion [ Constants ]

        #region [ Constructors ]

        public SingingFaceTool() {
            ID = (int) ToolID.SingingFace;
            Name = "Singing Face";
            ToolBoxImage = ElfRes.singing_face;
            ToolBoxImageSelected = ElfRes.singing_face_selected;
            //this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_singing_face);
            Cursor = CreateCursor(ElfRes.cross_base, ElfRes.singing_face_modifier, new Point(15, 15));
            MultiGestureKey1 = Keys.Shift | Keys.H;
            MultiGestureKey2 = Keys.S;
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
            _numChannelsForEyes = LoadValue(NUM_ChannelS_EYES, 2);
            _numChannelsForMouth = LoadValue(NUM_ChannelS_MOUTH, 5);

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            cboNumChannelsForEyes = (ToolStripComboBox) GetItem<ToolStripComboBox>("SingingFace_cboNumChannelsForEyes");
            cboNumChannelsForMouth = (ToolStripComboBox) GetItem<ToolStripComboBox>("SingingFace_cboNumChannelsForMouth");
            lblWarning = (ToolStripLabel) GetItem<ToolStripLabel>("SingingFace_lblWarning");

            // Set the initial value for the contol from what we had retrieve from Settings
            cboNumChannelsForEyes.SelectedIndex = _numChannelsForEyes - 1;
            cboNumChannelsForMouth.SelectedIndex = _numChannelsForMouth - 2;

            lblWarning.Visible = false;

            ListBoxUtil.SetDropDownWidth(cboNumChannelsForEyes);
            ListBoxUtil.SetDropDownWidth(cboNumChannelsForMouth);
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            SaveValue(NUM_ChannelS_EYES, _numChannelsForEyes);
            SaveValue(NUM_ChannelS_MOUTH, _numChannelsForMouth);
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cboNumChannelsForEyes = null;
            cboNumChannelsForMouth = null;
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
                cboNumChannelsForEyes.SelectedIndexChanged += cboNumChannelsForEyes_SelectedIndexChanged;
                cboNumChannelsForMouth.SelectedIndexChanged += cboNumChannelsForMouth_SelectedIndexChanged;
            }
            else {
                cboNumChannelsForEyes.SelectedIndexChanged -= cboNumChannelsForEyes_SelectedIndexChanged;
                cboNumChannelsForMouth.SelectedIndexChanged -= cboNumChannelsForMouth_SelectedIndexChanged;
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
            var PathList = new List<GraphicsPath>();
            Rectangle DrawArea;
            var Path = new GraphicsPath();

            if (finalRender) {
                p1 = _workshop.CalcCanvasPoint(p1);
                p2 = _workshop.CalcCanvasPoint(p2);
            }

            if (finalRender) {
                DrawArea = _workshop.NormalizedRectangle(_workshop.CalcLatticePoint(p1), _workshop.CalcLatticePoint(p2));
            }
            else {
                DrawArea = _workshop.NormalizedRectangle(p1, p2);
            }

            if ((DrawArea.Width <= 0) || (DrawArea.Height <= 0)) {
                Path.AddLine(p1, p2);
                PathList.Add(Path);
                return PathList;
            }

            float Width = Math.Min(DrawArea.Width, DrawArea.Height);
            float Height = Width;
            float Top = DrawArea.Top;
            float Left = DrawArea.Left;
            int X, Y;

            var Points = new float[20][];
            // Even elements are x, odd are y
            // A X & Y
            Points[0] = new float[2] {0.083f, 0.404f};
            Points[1] = new float[2] {0.209f, 0.377f};
            // B
            Points[2] = new float[2] {0.594f, 0.915f};
            Points[3] = new float[2] {0.377f, 0.209f};
            // C
            Points[4] = new float[3] {0.083f, 0.292f, 0.404f};
            Points[5] = new float[3] {0.209f, 0.022f, 0.377f};
            // D
            Points[6] = new float[3] {0.915f, 0.707f, 0.594f};
            Points[7] = new float[3] {0.209f, 0.022f, 0.377f};
            // E
            Points[8] = new float[10] {0.000f, 0.084f, 0.311f, 0.373f, 0.431f, 0.570f, 0.630f, 0.690f, 0.917f, 1.000f};
            Points[9] = new float[10] {0.527f, 0.624f, 0.624f, 0.712f, 0.624f, 0.624f, 0.712f, 0.624f, 0.624f, 0.527f};
            // F
            Points[10] = new float[8] {0.000f, 0.186f, 0.251f, 0.290f, 0.711f, 0.7497f, 0.8142f, 1.000f};
            Points[11] = new float[8] {0.527f, 0.881f, 0.771f, 0.855f, 0.855f, 0.7710f, 0.8810f, 0.527f};
            // G
            Points[12] = new float[8] {0.000f, 0.186f, 0.251f, 0.290f, 0.711f, 0.7497f, 0.8142f, 1.000f};
            Points[13] = new float[8] {0.527f, 1.000f, 0.891f, 0.975f, 0.975f, 0.8910f, 1.0000f, 0.527f};
            // H
            Points[14] = new float[2] {0.29f, 0.425f};
            Points[15] = new float[2] {0.624f, 0.35f};

            // I
            //                           arc1.x 0  width 1  pt2  2   pt3  3   pt4  4   pt5  5   pt6  6	pt7 7 	 arc2.x 8 width 9  pt10 10   pt11 11  pt12 12  pt13 13
            Points[16] = new float[14]
            {0.0856f, 0.2310f, 0.3107f, 0.3708f, 0.4308f, 0.5696f, 0.6296f, 0.6897f, 0.6757f, 0.2310f, 0.7497f, 0.7105f, 0.2900f, 0.2507f};
            Points[17] = new float[14]
            {0.6238f, 0.2310f, 0.6238f, 0.7123f, 0.6238f, 0.6238f, 0.7123f, 0.6238f, 0.6238f, 0.2310f, 0.7710f, 0.8552f, 0.8552f, 0.7710f};

            // J
            Points[18] = new float[2] {0.2017f, 0.7987f};
            Points[19] = new float[2] {0.8545f, 0.8545f};

            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < Points[i].Length; j++) {
                    if (i % 2 == 0) {
                        Points[i][j] *= Width;
                        Points[i][j] += Left;
                    }
                    else {
                        Points[i][j] *= Height;
                        Points[i][j] += Top;
                    }
                }
            }

            #region [ Eyes ]

            // SingingFace_NumChannelsForEyes = 0, A + B + C + D
            // SingingFace_NumChannelsForEyes = 1, A + B, C + D
            // SingingFace_NumChannelsForEyes = 2, A, B, C + D
            // SingingFace_NumChannelsForEyes = 3, A, B, C, D

            // A
            Path = new GraphicsPath();

            var LeftEyeClosed = new GraphicsPath();
            LeftEyeClosed.AddLine(new PointF(Points[0][0], Points[1][0]), new PointF(Points[0][1], Points[1][1]));

            // B
            var RightEyeClosed = new GraphicsPath();
            RightEyeClosed.AddLine(new PointF(Points[2][0], Points[3][0]), new PointF(Points[2][1], Points[3][1]));

            // C
            var LeftEyeOpen = new GraphicsPath();
            LeftEyeOpen.AddLine(new PointF(Points[4][0], Points[5][0]), new PointF(Points[4][1], Points[5][1]));
            LeftEyeOpen.AddLine(new PointF(Points[4][1], Points[5][1]), new PointF(Points[4][2], Points[5][2]));

            // D
            var RightEyeOpen = new GraphicsPath();
            RightEyeOpen.AddLine(new PointF(Points[6][0], Points[7][0]), new PointF(Points[6][1], Points[7][1]));
            RightEyeOpen.AddLine(new PointF(Points[6][1], Points[7][1]), new PointF(Points[6][2], Points[7][2]));

            switch (_numChannelsForEyes) {
                case 2:
                    Path.AddPath(LeftEyeOpen, false);
                    Path.AddPath(RightEyeOpen, false);
                    PathList.Add(Path);
                    Path = new GraphicsPath();
                    Path.AddPath(LeftEyeClosed, false);
                    Path.AddPath(RightEyeClosed, false);
                    PathList.Add(Path);
                    break;

                case 3:
                    Path.AddPath(LeftEyeOpen, false);
                    Path.AddPath(RightEyeOpen, false);
                    PathList.Add(Path);
                    PathList.Add(LeftEyeClosed);
                    PathList.Add(RightEyeClosed);
                    break;

                case 4:
                    PathList.Add(LeftEyeOpen);
                    PathList.Add(RightEyeOpen);
                    PathList.Add(LeftEyeClosed);
                    PathList.Add(RightEyeClosed);
                    break;

                case 1:
                default:
                    Path.AddPath(LeftEyeClosed, false);
                    Path.AddPath(RightEyeClosed, false);
                    Path.AddPath(LeftEyeOpen, false);
                    Path.AddPath(RightEyeOpen, false);
                    PathList.Add(Path);
                    break;
            }

            LeftEyeClosed = null;
            LeftEyeOpen = null;
            RightEyeClosed = null;
            RightEyeOpen = null;

            #endregion [ Eyes ]

            #region [ Mouths ]

            if ((_numChannelsForMouth >= 3) && (_numChannelsForMouth <= 6)) {
                // E
                Path = new GraphicsPath();
                Path.AddLine(new PointF(Points[8][0], Points[9][0]), new PointF(Points[8][1], Points[9][1]));
                Path.AddLine(new PointF(Points[8][1], Points[9][1]), new PointF(Points[8][2], Points[9][2]));
                Path.AddLine(new PointF(Points[8][2], Points[9][2]), new PointF(Points[8][3], Points[9][3]));
                Path.AddLine(new PointF(Points[8][3], Points[9][3]), new PointF(Points[8][4], Points[9][4]));
                Path.AddLine(new PointF(Points[8][4], Points[9][4]), new PointF(Points[8][5], Points[9][5]));
                Path.AddLine(new PointF(Points[8][5], Points[9][5]), new PointF(Points[8][6], Points[9][6]));
                Path.AddLine(new PointF(Points[8][6], Points[9][6]), new PointF(Points[8][7], Points[9][7]));
                Path.AddLine(new PointF(Points[8][7], Points[9][7]), new PointF(Points[8][8], Points[9][8]));
                Path.AddLine(new PointF(Points[8][8], Points[9][8]), new PointF(Points[8][9], Points[9][9]));
                PathList.Add(Path);

                // F
                Path = new GraphicsPath();
                Path.AddLine(new PointF(Points[10][0], Points[11][0]), new PointF(Points[10][1], Points[11][1]));
                Path.AddLine(new PointF(Points[10][1], Points[11][1]), new PointF(Points[10][2], Points[11][2]));
                Path.AddLine(new PointF(Points[10][2], Points[11][2]), new PointF(Points[10][3], Points[11][3]));
                Path.AddLine(new PointF(Points[10][3], Points[11][3]), new PointF(Points[10][4], Points[11][4]));
                Path.AddLine(new PointF(Points[10][4], Points[11][4]), new PointF(Points[10][5], Points[11][5]));
                Path.AddLine(new PointF(Points[10][5], Points[11][5]), new PointF(Points[10][6], Points[11][6]));
                Path.AddLine(new PointF(Points[10][6], Points[11][6]), new PointF(Points[10][7], Points[11][7]));
                PathList.Add(Path);

                // G
                Path = new GraphicsPath();
                Path.AddLine(new PointF(Points[12][0], Points[13][0]), new PointF(Points[12][1], Points[13][1]));
                Path.AddLine(new PointF(Points[12][1], Points[13][1]), new PointF(Points[12][2], Points[13][2]));
                Path.AddLine(new PointF(Points[12][2], Points[13][2]), new PointF(Points[12][3], Points[13][3]));
                Path.AddLine(new PointF(Points[12][3], Points[13][3]), new PointF(Points[12][4], Points[13][4]));
                Path.AddLine(new PointF(Points[12][4], Points[13][4]), new PointF(Points[12][5], Points[13][5]));
                Path.AddLine(new PointF(Points[12][5], Points[13][5]), new PointF(Points[12][6], Points[13][6]));
                Path.AddLine(new PointF(Points[12][6], Points[13][6]), new PointF(Points[12][7], Points[13][7]));
                PathList.Add(Path);
            }

            if ((_numChannelsForMouth == 2) || (_numChannelsForMouth == 4) || (_numChannelsForMouth == 5) || (_numChannelsForMouth == 6)) {
                // H
                Path = new GraphicsPath();
                Path.AddEllipse(Points[14][0], Points[15][0], Points[14][1] - Left, Points[15][1] - Top);
                PathList.Add(Path);
            }

            if (_numChannelsForMouth >= 5) {
                // I
                X = 16;
                Y = 17;
                Path = new GraphicsPath();

                // arc1.x 0  width 1  pt2  2   pt3  3   pt4  4   pt5  5   pt6  6   arc2.x 7 width 8  pt10 9   pt11 10  pt12 11  pt13 12
                // Arc1, 2-3, 4-5, 6-7, Arc2, 10-11, 12-13
                Path.AddArc(new RectangleF(Points[X][0], Points[Y][0], Points[X][1] - Left, Points[Y][1] - Top), 90, 180);
                Path.AddLine(new PointF(Points[X][2], Points[Y][2]), new PointF(Points[X][3], Points[Y][3]));
                Path.AddLine(new PointF(Points[X][4], Points[Y][4]), new PointF(Points[X][5], Points[Y][5]));
                Path.AddLine(new PointF(Points[X][6], Points[Y][6]), new PointF(Points[X][7], Points[Y][7]));

                Path.AddArc(new RectangleF(Points[X][8], Points[Y][8], Points[X][9] - Left, Points[Y][9] - Top), 270, 180);
                Path.AddLine(new PointF(Points[X][10], Points[Y][10]), new PointF(Points[X][11], Points[Y][11]));
                Path.AddLine(new PointF(Points[X][12], Points[Y][12]), new PointF(Points[X][13], Points[Y][13]));

                Path.CloseFigure();
                PathList.Add(Path);
            }

            if ((_numChannelsForMouth == 2) || (_numChannelsForMouth == 6)) {
                // J
                Path = new GraphicsPath();
                Path.AddLine(new PointF(Points[18][0], Points[19][0]), new PointF(Points[18][1], Points[19][1]));
                PathList.Add(Path);
            }

            #endregion [ Mouths ]

            return PathList;
        }


        /// <summary>
        ///     Calculate the number of Channels that are needed.
        /// </summary>
        protected override int NumberOfChannels() {
            return _numChannelsForEyes + _numChannelsForMouth;
        }

        #endregion [ Methods ]

        private void cboNumChannelsForEyes_SelectedIndexChanged(object sender, EventArgs e) {
            _numChannelsForEyes = cboNumChannelsForEyes.SelectedIndex + 1;
            cboNumChannelsForEyes.ToolTipText = cboNumChannelsForEyes.SelectedItem.ToString();
        }


        private void cboNumChannelsForMouth_SelectedIndexChanged(object sender, EventArgs e) {
            _numChannelsForMouth = cboNumChannelsForMouth.SelectedIndex + 2;
            cboNumChannelsForMouth.ToolTipText = cboNumChannelsForMouth.SelectedItem.ToString();
        }
    }
}