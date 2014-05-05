using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	////[AdjPrevTool("MultiChannelLine_ToolGroup")]
	public class ToolGroup_MultiChannelLine : BaseToolGroup, IToolGroup
	{
		/// <summary>
		/// Numeric identifier for the tool group.
		/// </summary>
		public override int ID
		{
			get { return (int)Tool.MultiChannel_ToolGroup; }
			set { }
		}

		public string Name
		{
			get { return Constants.TOOLGROUP_MULTIChannel; }
		}

		public Bitmap ToolBoxImage
		{
			get { return ElfRes.multiChannel_lines; }
		}

	}
}
