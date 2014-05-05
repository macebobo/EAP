using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    [ElfTool("Ellipse Mask")]
    [ElfToolCore]
    public class MaskEllipseTool : MaskBase, ITool {
        #region [ Constructors ]

        public MaskEllipseTool() {
            ID = (int) ToolID.Mask_Ellipse;
            Name = "Elliptical Marquee";
            ToolBoxImage = ElfRes.mask_ellipse;
            ToolBoxImageSelected = ElfRes.mask_ellipse_selected;
            MaskTypeCursorModifier = ElfRes.mask_ellipse_modifier;
            MultiGestureKey1 = Keys.Shift | Keys.M;
            MultiGestureKey2 = Keys.E;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Create the graphics path needed to draw the path.
        /// </summary>
        /// <param name="p1">Upper Left point</param>
        /// <param name="p2">Lower Right point</param>
        protected override GraphicsPath CreateRenderPath(Point p1, Point p2, UnitScale scaling) {
            var DrawPath = new GraphicsPath();
            DrawPath.AddEllipse(_workshop.NormalizedRectangle(p1, p2));
            return DrawPath;
        }

        #endregion [ Methods ]
    }
}