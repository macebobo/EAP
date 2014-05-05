using System;
using System.Drawing;
using System.Windows.Forms;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Paintbrush")]
    [ElfToolCore]
    public class PaintbrushTool : BaseTool, ITool {
        #region [ Private Variables ]

        // Settings from the ToolStrip
        private Rectangle _bounds = Rectangle.Empty;
        private Bitmap _gridwork;

        // Masking variables
        private bool _hasMask;
        private Region _maskRegion;
        private Nib _nib;
        private Brush _paintBrush;

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Custom Cursor, created based on the nib size and shape
        /// </summary>
        public override Cursor Cursor {
            get { return _nib.Cursor; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public PaintbrushTool() {
            ID = (int) ToolID.Paint;
            Name = "Paintbrush";
            ToolBoxImage = ElfRes.paintbrush;
            ToolBoxImageSelected = ElfRes.paintbrush_selected;
            MultiGestureKey1 = Keys.P;
            _nib = new Nib();
            _nib.Changed += Nib_Changed;
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

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            _nib.NibSize_Control = (ToolStripComboBox) GetItem<ToolStripComboBox>("Paint_cboNibSize");
            _nib.SquareNib_Control = (ToolStripButton) GetItem<ToolStripButton>("Paint_cmdSquareNib");
            _nib.RoundNib_Control = (ToolStripButton) GetItem<ToolStripButton>("Paint_cmdRoundNib");

            // Load the Settings values
            _nib.LoadSettings(_savePath);
            _nib.AttachEvents(true);
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            base.MouseDown(buttons, latticePoint, actualCanvasPoint);

            _constrainingDirection = ConstrainDirection.NotSet;
            _paintBrush = Profile.Channels.Active.GetBrush();
            _latticeBuffer = ImageHandler.CopyImage(Profile.Channels.Active.LatticeBuffer);
            _latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);

            SetMaskClip();

            _hasMask = Profile.HasMask;
            if (_hasMask) {
                _maskRegion = new Region(Profile.GetMaskOutline(UnitScale.Lattice));
            }

#if DEBUG
            Workshop.Instance.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
#endif

            _nib.AdjustForCellSize();
            Paint(Profile.Channels.Active, latticePoint);
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
            Point ConstrainedCell = _workshop.ConstrainPaint(_currentLatticePoint, actualCanvasPoint, ref _constrainingDirection);

            Paint(Profile.Channels.Active, _currentLatticePoint); //ConstrainedCell);

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

            _canvasControlGraphics.ResetClip();
            Profile.Channels.PopulateChannelFromBitmap(_latticeBuffer, Profile.Channels.Active);
            Profile.Refresh();
            PostDrawCleanUp();
            return true;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        /// <param name="settings"></param>
        public override void SaveSettings() {
            _nib.SaveSettings(_settings, _savePath);
        }


        /// <summary>
        ///     Method that fires when this Tool is selected in the ToolBox, or when any Canvas UI values change (canvas resize,
        ///     cell size change, zoom change, grid line width)
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();
            if (Profile != null) {
                CreatePaintGrid();
            }
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();
            _nib.AttachEvents(false);
            _nib = null;
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

            if (_nib != null) {
                _nib.AttachEvents(attach);
            }

            if (attach) {
                if (_nib != null) {
                    _nib.Changed += Nib_Changed;
                }
                if (Profile != null) {
                    Profile.ScalingChanged += Profile_ScalingChanged;
                }
            }
            else {
                if (_nib != null) {
                    _nib.Changed -= Nib_Changed;
                }
                if (Profile != null) {
                    Profile.ScalingChanged -= Profile_ScalingChanged;
                }
            }

            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Create a grid of black lines over a transparent background, to simulate the grid lines
        /// </summary>
        private void CreatePaintGrid() {
            // If we already have a bitmap defined, clear it out.
            if (_gridwork != null) {
                _gridwork.Dispose();
                _gridwork = null;
            }
            Size Size = Scaling.CanvasSize;

            _gridwork = new Bitmap(Size.Width, Size.Height, Profile.GetCanvasGraphics());

            int CellScale = Scaling.CellScale;
            int CellZoom = Scaling.CellZoom;

            using (Graphics g = Graphics.FromImage(_gridwork)) {
                g.Clear(Color.Transparent);
                var GridCell = new Rectangle(0, 0, CellScale, CellScale);
                for (int x = CellZoom; x < Size.Width; x += CellScale) {
                    g.DrawLine(Pens.Black, new Point(x, 0), new Point(x, Size.Height));
                }
                for (int y = CellZoom; y < Size.Height; y += CellScale) {
                    g.DrawLine(Pens.Black, new Point(0, y), new Point(Size.Width, y));
                }
            }
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
        ///     Paints the Channel color on the Canvas, and also hidden on the Paint Pane.
        /// </summary>
        /// <param name="channel">Channel that gets painted upon</param>
        /// <param name="pt">Position the nib shape/sized area is centered upon, in cells</param>
        private void Paint(Channel channel, Point pt) {
            _nib.OffsetRects(pt);

            if ((_nib.Shape == Nib.NibShape.Square) || (_nib.Size < 3)) {
                _latticeBufferGraphics.FillRectangle(Brushes.White, _nib.Rect_Lattice);
                _canvasControlGraphics.FillRectangle(_paintBrush, _nib.Rect_Canvas);
            }
            else {
                _latticeBufferGraphics.FillEllipse(Brushes.White, _nib.Rect_Lattice);
                _canvasControlGraphics.FillEllipse(_paintBrush, _nib.Rect_Canvas);
            }

#if DEBUG
            Workshop.Instance.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
#endif
        }


        /// <summary>
        ///     Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
        /// </summary>
        protected override void PostDrawCleanUp() {
            base.PostDrawCleanUp();

            if (_paintBrush != null) {
                _paintBrush.Dispose();
                _paintBrush = null;
            }
            if (_maskRegion != null) {
                _maskRegion.Dispose();
                _maskRegion = null;
            }
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Occurs when one of the scaling variables within the Profile changes.
        /// </summary>
        private void Profile_ScalingChanged(object sender, EventArgs e) {
            Profile.Cursor = _nib.Cursor;
        }


        /// <summary>
        ///     Occurs when one of the properties on the Nib has changed and the cursor needs to be recreated.
        /// </summary>
        protected void Nib_Changed(object sender, EventArgs e) {
            Profile.Cursor = _nib.Cursor;
        }
    }
}