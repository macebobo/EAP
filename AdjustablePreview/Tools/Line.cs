using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Line")]
	public class LineTool : ShapeBase, ITool
	{
		#region [ Constructors ]

		public LineTool() : base()
		{
			this.ID = (int)Tool.Line;
			this.Name = "Line";
			this.ToolBoxImage = ElfRes.line;
			//this.Cursor = CustomCursors.MemoryCursor(ElfRes.cross_line);
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.line_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			DrawPath.AddLine(p1, p2);
			return DrawPath;
		}

		#endregion [ Methods ]

	}
}
