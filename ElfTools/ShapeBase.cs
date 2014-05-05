using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools {
    /// <summary>
    ///     Common functionality for several shape tools
    /// </summary>
    public abstract class ShapeBase : BaseTool, ITool {
        #region [ Constants ]

        public const string TOOLGROUP_NAME = "Shape_ToolGroup";

        #endregion [ Constants ]

        #region [ Private Variables ]

        // Settings from the ToolStrip
        protected DashStyle _dashStyle = DashStyle.Solid;
        protected string _dashStyleControlName = string.Empty;
        protected bool _fill = false;
        protected int _lineThickness = 1;
        protected string _lineThicknessControlName = string.Empty;
        protected string _noFillControlName = string.Empty;
        protected string _yesFillControlName = string.Empty;
        protected ToolStripDropDownButton cboDashStyle = null;
        protected ToolStripComboBox cboLineThickness = null;

        // Controls from ToolStrip
        protected ToolStripButton cmdDoFill = null;
        protected ToolStripButton cmdDoNotFill = null;

        #endregion [ Private Variables ]

        #region [ Constructors ]

        public ShapeBase() {
            ID = (int) ToolID.NotSet;
            Name = "SHAPEBASE";
            ToolBoxImage = ElfRes.undefined;
            ToolBoxImageSelected = ElfRes.undefined;
            DoesSelection = true;
            ToolGroupName = TOOLGROUP_NAME;
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
            _lineThickness = LoadValue(Constants.LINE_THICKNESS, 1);
            _fill = LoadValue(Constants.FILL, false);
            _dashStyle = EnumHelper.GetEnumFromValue<DashStyle>(LoadValue(Constants.DASH_STYLE, (int) DashStyle.Solid));

            if (GetItem<ToolStripLabel>(1) == null) {
                return;
            }

            // Get a pointer to the controls on the toolstrip that belongs to us.
            if ((_dashStyleControlName ?? string.Empty).Length > 0) {
                cboDashStyle = (ToolStripDropDownButton) GetItem<ToolStripDropDownButton>(_dashStyleControlName);
                SetDashStyleDropDownButton(cboDashStyle);
                SetDropDownMenuSelected(FindDropMenuItemFromValue(cboDashStyle, (int) _dashStyle));
            }

            if ((_lineThicknessControlName ?? string.Empty).Length > 0) {
                cboLineThickness = (ToolStripComboBox) GetItem<ToolStripComboBox>(_lineThicknessControlName);
                cboLineThickness.SelectedIndex = _lineThickness - 1;
            }

            if ((_yesFillControlName ?? string.Empty).Length > 0) {
                cmdDoFill = (ToolStripButton) GetItem<ToolStripButton>(_yesFillControlName);
                AddButtonFaces(cmdDoFill.Name,
                    new ButtonImages(ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Check),
                        ImageHandler.AddAnnotation(ElfRes.fill_selected, Annotation.Check)));
                cmdDoFill.Checked = _fill;
                SetToolbarSelectedImage(cmdDoFill);
            }
            if ((_noFillControlName ?? string.Empty).Length > 0) {
                cmdDoNotFill = (ToolStripButton) GetItem<ToolStripButton>(_noFillControlName);
                AddButtonFaces(cmdDoNotFill.Name,
                    new ButtonImages(ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Delete),
                        ImageHandler.AddAnnotation(ElfRes.fill_selected, Annotation.Delete)));
                cmdDoNotFill.Checked = !_fill;
                SetToolbarSelectedImage(cmdDoNotFill);
            }
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            base.MouseDown(buttons, latticePoint, actualCanvasPoint);
            _mouseDownCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
            _currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
        }


        /// <summary>
        ///     Canvas MouseMove event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (!_isMouseDown) {
                return false;
            }

            if (!base.MouseMove(buttons, latticePoint, actualCanvasPoint)) {
                return false;
            }

            _currentLatticePoint = latticePoint;
            _currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
            _constrainedCanvasPoint = _workshop.ConstrainLine(_currentMouseCanvasPoint, _mouseDownCanvasPoint);

            // Draw the captured bitmap onto the CanvasPane PictureBox
            DisplayCapturedCanvas();

            Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, false);

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

            _currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);

            Cursor LastCursor = Profile.Cursor;
            Profile.Cursor = Cursors.WaitCursor;

            // Render the shape onto the active Channel
            Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, true);

#if DEBUG
            Workshop.Instance.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
#endif

            // Write out the changes onto the active Channel
            Profile.Channels.Active.Empty();
            Profile.Channels.Active.LatticeBuffer = _latticeBuffer;

            // Redraw the canvas to expose our changes.
            Profile.Refresh();

            // Clean up
            PostDrawCleanUp();

            _mouseDownLatticePoint = Point.Empty;
            Profile.Cursor = LastCursor;

            return true;
        }


        /// <summary>
        ///     Save this toolstrip settings back to the Settings object.
        /// </summary>
        public override void SaveSettings() {
            if (cboLineThickness != null) {
                SaveValue(Constants.LINE_THICKNESS, _lineThickness);
            }

            if (cboDashStyle != null) {
                SaveValue(Constants.DASH_STYLE, (int) _dashStyle);
            }

            if (cmdDoFill != null) {
                SaveValue(Constants.FILL, _fill);
            }
        }


        /// <summary>
        ///     Method fires when we are closing out of the editor, want to clean up all our objects.
        /// </summary>
        public override void ShutDown() {
            base.ShutDown();

            cmdDoFill = null;
            cmdDoNotFill = null;
            cboLineThickness = null;
            cboDashStyle = null;
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
                if (cboDashStyle != null) {
                    foreach (ToolStripMenuItem Item in cboDashStyle.DropDownItems) {
                        Item.Click += cboDashStyle_Click;
                    }
                }

                if (cboLineThickness != null) {
                    cboLineThickness.SelectedIndexChanged += cboLineThickness_SelectedIndexChanged;
                }

                if (cmdDoFill != null) {
                    cmdDoFill.Click += DoFill_Click;
                }

                if (cmdDoNotFill != null) {
                    cmdDoNotFill.Click += DoNotFill_Click;
                }
            }
            else {
                if (cboDashStyle != null) {
                    foreach (ToolStripMenuItem Item in cboDashStyle.DropDownItems) {
                        Item.Click -= cboDashStyle_Click;
                    }
                }

                if (cboLineThickness != null) {
                    cboLineThickness.SelectedIndexChanged -= cboLineThickness_SelectedIndexChanged;
                }

                if (cmdDoFill != null) {
                    cmdDoFill.Click -= DoFill_Click;
                }

                if (cmdDoNotFill != null) {
                    cmdDoNotFill.Click -= DoNotFill_Click;
                }
            }
            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Draw the shape
        /// </summary>
        /// <param name="p1">Upper Left point in pixels</param>
        /// <param name="p2">Lower Right point in pixels</param>
        /// <param name="finalRender">
        ///     True if this drawing is to be the final render, false if its to be while the user is still
        ///     doing a select
        /// </param>
        protected virtual void Render(CanvasPoint p1, CanvasPoint p2, bool finalRender) {
            if (finalRender) {
                p1 = _workshop.CalcLatticePoint(p1);
                p2 = _workshop.CalcLatticePoint(p2);
            }

            GraphicsPath DrawPath = CreateRenderPath(p1, p2, finalRender);

            using (Pen DrawPen = finalRender ? RenderPen() : _workshop.GetMarqueePen()) {
                try {
                    (finalRender ? _latticeBufferGraphics : _canvasControlGraphics).DrawPath(DrawPen, DrawPath);
                }
                catch (OutOfMemoryException) {}

                if (_fill && finalRender) {
                    _latticeBufferGraphics.FillPath(Brushes.White, DrawPath);
                }
            }

            DrawPath.Dispose();
            DrawPath = null;
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
        protected abstract GraphicsPath CreateRenderPath(CanvasPoint p1, CanvasPoint p2, bool finalRender);


        /// <summary>
        ///     Creates the Pen used to render the shape onto the Paint Pane
        /// </summary>
        protected override Pen RenderPen() {
            var RenderPen = new Pen(Color.White, _lineThickness);
            RenderPen.DashStyle = _dashStyle;
            return RenderPen;
        }


        /// <summary>
        ///     Changes the display of the drop down menu and the check status of the drop down items depending on which selection
        ///     whas made.
        /// </summary>
        /// <param name="menuItem">Selected menu item.</param>
        private void SetDropDownMenuSelected(ToolStripMenuItem menuItem) {
            cboDashStyle.Image = menuItem.Image;
            cboDashStyle.Text = menuItem.Text;
            cboDashStyle.ToolTipText = menuItem.ToolTipText;

            foreach (ToolStripMenuItem Item in cboDashStyle.DropDownItems) {
                if (Item != menuItem) {
                    Item.Checked = false;
                }
            }

            _dashStyle = (DashStyle) Convert.ToInt32(menuItem.Tag);
        }

        #endregion [ Methods ]

        private void cboLineThickness_SelectedIndexChanged(object sender, EventArgs e) {
            string Value = cboLineThickness.SelectedItem.ToString();
            if (Value.Length > 0) {
                _lineThickness = Convert.ToInt32(Value);
            }
        }


        private void cboDashStyle_Click(object sender, EventArgs e) {
            var Menu = (ToolStripMenuItem) sender;
            SetDropDownMenuSelected(Menu);
        }


        private void DoFill_Click(object sender, EventArgs e) {
            if (cmdDoFill.Checked) {
                return;
            }
            cmdDoNotFill.Checked = false;
            cmdDoFill.Checked = true;
            SetToolbarSelectedImage(cmdDoNotFill);
            SetToolbarSelectedImage(cmdDoFill);
            _fill = true;
        }


        private void DoNotFill_Click(object sender, EventArgs e) {
            if (cmdDoNotFill.Checked) {
                return;
            }
            cmdDoFill.Checked = false;
            cmdDoNotFill.Checked = true;
            SetToolbarSelectedImage(cmdDoNotFill);
            SetToolbarSelectedImage(cmdDoFill);
            _fill = false;
        }
    }
}