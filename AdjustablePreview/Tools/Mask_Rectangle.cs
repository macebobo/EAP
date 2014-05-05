using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Rectangle Mask")]
	public class MaskRectangleTool : MaskBase, ITool
	{

		#region [ Constructors ]

		public MaskRectangleTool()
		{
			this.ID = (int)Tool.Mask_Rectangle;
			this.Name = "Rectanglular Marquee";
			this.ToolBoxImage = ElfRes.mask_rectangle;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_MASK;
			this.MaskTypeCursorModifier = ElfRes.mask_rectangle_modifier;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the Mask.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="scaling">Not used for this tool</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, Scaling scaling)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			Rectangle DrawArea = _workshop.NormalizedRectangle(p1, p2);
			DrawPath.AddRectangle(DrawArea);
			return DrawPath;
		}

		#endregion [ Methods ]

	}
}
