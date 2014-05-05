using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfTools.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfTools.Tools {
    /// <summary>
    ///     Allows the user to simply just draw the mask around the target area
    /// </summary>
    [ElfTool("Lasso Mask")]
    [ElfToolCore]
    public class MaskLassoTool : MaskBase, ITool {
        #region [ Private Variables ]

        private List<CanvasPoint> _maskPoints = new List<CanvasPoint>();
        //private Point _prevMouseCellPixel = Point.Empty;
        //bool _closeFigure = false;

        #endregion [ Private Variables ]

        #region [ Constructors ]

        public MaskLassoTool() {
            ID = (int) ToolID.Mask_Lasso;
            Name = "Lasso Marquee";
            ToolBoxImage = ElfRes.mask_lasso;
            ToolBoxImageSelected = ElfRes.mask_lasso_selected;
            MaskTypeCursorModifier = ElfRes.lasso_modifier;
            MultiGestureKey1 = Keys.Shift | Keys.M;
            MultiGestureKey2 = Keys.L;
            _freehandStyle = true;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Canvas MouseDown event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            base.MouseDown(buttons, latticePoint, actualCanvasPoint);

            if (_moving) {
                return;
            }

            if (_maskPoints == null) {
                _maskPoints = new List<CanvasPoint>();
            }
            _maskPoints.Add(_workshop.CalcCanvasPoint(latticePoint));
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

            if (!_isMouseDown || _moving) {
                return false;
            }

            _maskPoints.Add(_workshop.CalcCanvasPoint(latticePoint));

            // Draw the captured bitmap onto the CanvasPane PictureBox
            DisplayCapturedCanvas();

            Render(CanvasPoint.Empty, CanvasPoint.Empty, UnitScale.Lattice, false);

            return true;
        }


        /// <summary>
        ///     Canvas MouseUp event was fired
        /// </summary>
        /// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
        /// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
        /// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
        public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint) {
            if (_moving) {
                return false;
            }

            _maskPoints.Add(_workshop.CalcCanvasPoint(latticePoint));
            base.MouseUp(buttons, latticePoint, actualCanvasPoint);
            return true;
        }


        /// <summary>
        ///     Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a
        ///     ShutDown.
        ///     Remove all the click events from these shared controls
        /// </summary>
        public override void OnUnselected() {
            base.OnUnselected();
            _maskPoints = null;
        }


        /// <summary>
        ///     Create the graphics path needed to draw the path.
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        protected override GraphicsPath CreateRenderPath(CanvasPoint p1, CanvasPoint p2, UnitScale scaling) {
            var DrawPath = new GraphicsPath();
            CanvasPoint LastPoint = CanvasPoint.Empty;
            CanvasPoint CurrentPoint = CanvasPoint.Empty;

            int CellScale = Scaling.CellScale;

            foreach (CanvasPoint p in _maskPoints) {
                CurrentPoint = (scaling == UnitScale.Lattice) ? p : new CanvasPoint(p.X / CellScale, p.Y / CellScale);

                if (!LastPoint.IsEmpty) {
                    if (LastPoint.Equals(CurrentPoint)) {
                        continue;
                    }
                    DrawPath.AddLine(LastPoint, CurrentPoint);
                }
                LastPoint = CurrentPoint;
            }
            //if (_closeFigure)
            DrawPath.CloseFigure();
            return DrawPath;
        }


        /// <summary>
        ///     Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
        /// </summary>
        protected override void PostDrawCleanUp() {
            base.PostDrawCleanUp();
            _maskPoints.Clear();
            //_prevMouseCellPixel = Point.Empty;
            //_closeFigure = false;
        }

        #endregion [ Methods ]
    }
}