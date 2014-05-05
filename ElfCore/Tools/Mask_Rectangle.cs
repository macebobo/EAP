using ElfCore.Interfaces;
using ElfCore.Util;
using System.Drawing;
using System.Drawing.Drawing2D;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[ElfEditTool("Rectangle Mask")]
	public class MaskRectangleTool : MaskBase, ITool
	{

		#region [ Constructors ]

		public MaskRectangleTool() : base()
		{
			base.ID = (int)ToolID.Mask_Rectangle;
			base.Name = "Rectangular Marquee";
			base.ToolBoxImage = ElfRes.mask_rectangle;
			base.MaskTypeCursorModifier = ElfRes.mask_rectangle_modifier;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the Mask.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="scaling">Not used for this tool</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, UnitScale scaling)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
			DrawPath.AddRectangle(DrawArea);
			return DrawPath;
		}

		#endregion [ Methods ]

	}
}
