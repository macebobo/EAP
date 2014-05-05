using System.Drawing;
using System.Drawing.Drawing2D;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Ellipse Mask")]
	public class MaskEllipseTool : MaskBase, ITool
	{
	
		#region [ Constructors ]

		public MaskEllipseTool()
		{
			this.ID = (int)Tool.Mask_Ellipse;
			this.Name = "Elliptical Marquee";
			this.ToolBoxImage = ElfRes.mask_ellipse;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_MASK;
			this.MaskTypeCursorModifier = ElfRes.mask_ellipse_modifier;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, Scaling scaling)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			DrawPath.AddEllipse(_workshop.NormalizedRectangle(p1, p2));
			return DrawPath;
		}
			
		#endregion [ Methods ]
			
	}
}
