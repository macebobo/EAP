using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	//[AdjPrevTool("Shape_ToolGroup")]
	public class ToolGroup_Shape : BaseToolGroup, IToolGroup
	{
		/// <summary>
		/// Numeric identifier for the tool group.
		/// </summary>
		public override int ID
		{
			get { return (int)Tool.Shape_ToolGroup; }
			set { }
		}

		public string Name
		{
			get { return Constants.TOOLGROUP_SHAPE; }
		}

		public Bitmap ToolBoxImage
		{
			get { return ElfRes.rectangle; }
		}
	}
}
