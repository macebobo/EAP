using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Line")]
    [ElfToolCore]
    public class LineTool : ShapeBase, ITool {
        #region [ Constructors ]

        public LineTool() {
            ID = (int) ToolID.Line;
            Name = "Line";
            ToolBoxImage = ElfRes.line;
            ToolBoxImageSelected = ElfRes.line_selected;
            base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.line_modifier, new Point(15, 15));
            MultiGestureKey1 = Keys.L;

            _dashStyleControlName = "Line_cboDashStyle";
            _lineThicknessControlName = "Line_cboLineSize";
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
            DrawPath.AddLine(p1, p2);
            return DrawPath;
        }

        #endregion [ Methods ]
    }
}