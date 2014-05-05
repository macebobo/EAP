using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Ellipse")]
    [ElfToolCore]
    public class EllipseTool : ShapeBase, ITool {
        #region [ Constructors ]

        public EllipseTool() {
            ID = (int) ToolID.Ellipse;
            Name = "Ellipse";
            ToolBoxImage = ElfRes.ellipse;
            ToolBoxImageSelected = ElfRes.ellipse_selected;
            //this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_circle);
            base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.ellipse_modifier, new Point(15, 15));
            MultiGestureKey1 = Keys.E;

            _dashStyleControlName = "Ellipse_cmdNoFill";
            _yesFillControlName = "Ellipse_cmdFill";
            _dashStyleControlName = "Ellipse_cboDashStyle";
            _lineThicknessControlName = "Ellipse_cboLineSize";
        }

        #endregion [ Constructors ]

        #region [ Methods ]

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
            DrawPath.AddEllipse(_workshop.NormalizedRectangle(p1, p2));
            return DrawPath;
        }

        #endregion [ Methods ]
    }
}