using System;
using System.Windows.Forms;

using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    [ElfTool("Zoom")]
    [ElfToolCore]
    public class ZoomTool : BaseTool, ITool {
        #region [ Enums ]

        private enum ZoomMode {
            NotSet = -1,
            ZoomIn,
            ZoomOut
        }

        #endregion [ Enums ]

        #region [ Constants ]

        private const string PERCENT_FORMAT = "0%";
        private const float ZOOM_CLICK_ADJ = 0.5f;

        #endregion [ Constants ]

        #region [ Private Variables ]

        // Settings from the ToolStrip
        //private ZoomMode _zoomMode = ZoomMode.ZoomIn;

        // Controls from ToolStrip
        private ToolStripComboBox cboZoomFactor;
        private ToolStripButton cmdZoom100;
        private ToolStripButton cmdZoomIn;
        private ToolStripButton cmdZoomOut;

        //private bool _isSetting = false;

        #endregion [ Private Variables ]

        #region [ Constructors ]

        public ZoomTool() {
            ID = (int) ToolID.Zoom;
            Name = "Zoom";
            ToolBoxImage = ElfRes.zoom;
            ToolBoxImageSelected = ElfRes.zoom_selected;
            base.Cursor = CreateCursor(ElfRes.zoom, new CanvasPoint(7, 7));
            MultiGestureKey1 = Keys.Z;
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
            cboZoomFactor = (ToolStripComboBox) GetItem<ToolStripComboBox>("Zoom_cboZoomFactor");
            cmdZoomIn = (ToolStripButton) GetItem<ToolStripButton>("Zoom_ZoomIn");
            cmdZoomOut = (ToolStripButton) GetItem<ToolStripButton>("Zoom_ZoomOut");
            cmdZoom100 = (ToolStripButton) GetItem<ToolStripButton>("Zoom_Zoom100");

            AttachEvents(true);
            // Set the initial value for the contol from what we had retrieve from Settings
            //SetZoom();
        }


        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {}


        /// <summary>
        ///     Canvas MouseMove event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            return true;
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            //_isSetting = true;
            float Zoom = Scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100);
            float NewZoom = Zoom;

            if (buttons == MouseButtons.Left) {
                NewZoom += ZOOM_CLICK_ADJ;
            }
            else if (buttons == MouseButtons.Right) {
                NewZoom -= ZOOM_CLICK_ADJ;
            }

            if (NewZoom < Scaling.MIN_ZOOM) {
                NewZoom = Scaling.MIN_ZOOM;
            }
            else if (NewZoom > Scaling.MAX_ZOOM) {
                NewZoom = Scaling.MAX_ZOOM;
            }

            if (Zoom != NewZoom) {
                SetToolBarControlsToZoom(NewZoom);
                if (buttons == MouseButtons.Left) {
                    Profile.SetClickZoom(actualCanvasPoint, NewZoom);
                }
                else if (buttons == MouseButtons.Right) {
                    Scaling.Zoom = NewZoom;
                }

                // Fire the event to indicate that this tool has finished working.
                SaveUndo();

                //_isSetting = false;
            }
            return true;
        }


        /// <summary>
        ///     Occurs when this Tool has been selected from the main Toolbar
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();
            SetToolBarControlsToZoom();
        }


        public override void ShutDown() {
            base.ShutDown();
            cboZoomFactor = null;
            cmdZoomIn = null;
            cmdZoomOut = null;
            cmdZoom100 = null;
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
                cboZoomFactor.SelectedIndexChanged += cboZoomFactor_SelectedIndexChanged;
                cboZoomFactor.KeyPress += cboZoomFactor_KeyPress;
                cboZoomFactor.Leave += cboZoomFactor_Leave;
                cmdZoomIn.Click += cmdZoomIn_Click;
                cmdZoomOut.Click += cmdZoomOut_Click;
                cmdZoom100.Click += cmdZoom100_Click;
                cboZoomFactor.KeyPress += _toolStrip_Form.SignedFloatOnly_KeyPress;
            }
            else {
                cboZoomFactor.SelectedIndexChanged -= cboZoomFactor_SelectedIndexChanged;
                cboZoomFactor.KeyPress -= cboZoomFactor_KeyPress;
                cboZoomFactor.Leave -= cboZoomFactor_Leave;
                cmdZoomIn.Click -= cmdZoomIn_Click;
                cmdZoomOut.Click -= cmdZoomOut_Click;
                cmdZoom100.Click -= cmdZoom100_Click;
                cboZoomFactor.KeyPress -= _toolStrip_Form.SignedFloatOnly_KeyPress;
            }

            base.AttachEvents(attach);
        }


        /// <summary>
        ///     Gets the text out of the ZoomFactor drop down list box and converts it to a decimal value that is used within the
        ///     Profile.
        /// </summary>
        private float GetZoomAmountFromDropDown() {
            // Blanking out the textbox sets the value to 100%
            if (cboZoomFactor.Text.Length == 0) {
                cboZoomFactor.Text = Scaling.ZOOM_100.ToString(PERCENT_FORMAT);
            }

            string Value = cboZoomFactor.Text.Replace("%", string.Empty).Trim();
            return (float) Convert.ToDecimal(Value) / 100.0f;
        }


        /// <summary>
        ///     Saves the undo information, using the new zoom amount in the text.
        /// </summary>
        private void SaveUndo() {
            Profile.SaveUndo("Zoom to " + Scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100).ToString("0%"));
        }


        /// <summary>
        ///     Adjust the toolbar controls to the specified zoom level.
        /// </summary>
        private void SetToolBarControlsToZoom(float zoomLevel) {
            //ZoomFactor.SelectedIndex = (int)zoomLevel - 1;
            cboZoomFactor.Text = zoomLevel.ToString(PERCENT_FORMAT);
            cmdZoomIn.Enabled = true;
            cmdZoomOut.Enabled = true;

            if (zoomLevel == Scaling.MIN_ZOOM) {
                cmdZoomOut.Enabled = false;
            }

            else if (zoomLevel == Scaling.MAX_ZOOM) {
                cmdZoomIn.Enabled = false;
            }

            cmdZoom100.Enabled = (zoomLevel != Scaling.ZOOM_100);
        }


        /// <summary>
        ///     Adjust the toolbar controls to the zoom level on the current Profile.
        /// </summary>
        private void SetToolBarControlsToZoom() {
            if (Profile != null) {
                SetToolBarControlsToZoom(Scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100));
            }
        }


        private void SetZoomFromDropDown() {
            float Zoom = GetZoomAmountFromDropDown();
            if (Scaling.Zoom != Zoom) {
                Scaling.Zoom = Zoom;
                SaveUndo();
            }
            SetToolBarControlsToZoom();
        }

        #endregion [ Methods ]

        /// <summary>
        ///     Validate that the value entered in the text box is a proper number. If so, set the value and format the text in the
        ///     box with a percent sign
        /// </summary>
        private void cboZoomFactor_Leave(object sender, EventArgs e) {
            SetZoomFromDropDown();
        }


        private void cboZoomFactor_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) 13) {
                SetZoomFromDropDown();
                cboZoomFactor.SelectAll();
            }
        }


        private void cboZoomFactor_SelectedIndexChanged(object sender, EventArgs e) {
            SetZoomFromDropDown();
            //if (_setting)
            //    return;

            //_setting = true;
            //Profile.Zoom = ZoomFactor.SelectedIndex + 1;
            //SetToolBarControlsToZoom();
            //SaveUndo();
            //_setting = false;
        }


        private void cmdZoom100_Click(object sender, EventArgs e) {
            //_isSetting = true;
            Scaling.Zoom = 1;
            SetToolBarControlsToZoom();
            SaveUndo();
            //_isSetting = false;
        }


        private void cmdZoomIn_Click(object sender, EventArgs e) {
            //_isSetting = true;
            Scaling.Zoom += 1;
            SetToolBarControlsToZoom();
            SaveUndo();
            //_isSetting = false;
        }


        private void cmdZoomOut_Click(object sender, EventArgs e) {
            //_isSetting = true;
            Scaling.Zoom -= 1;
            SetToolBarControlsToZoom();
            SaveUndo();
            //_isSetting = false;
        }
    }
}