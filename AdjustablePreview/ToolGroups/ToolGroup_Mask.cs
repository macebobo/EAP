using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	////[AdjPrevTool("Mask_ToolGroup")]
	public class ToolGroup_Mask : BaseToolGroup, IToolGroup
	{
				/// <summary>
		/// Numeric identifier for the tool group.
		/// </summary>
		//public override int ID
		//{
		//    get { return (int)Tool.Mask_ToolGroup; }
		//    set { }
		//}

		public string Name
		{
			get { return Constants.TOOLGROUP_MASK; }
		}

		public Bitmap ToolBoxImage
		{
			get { return ElfRes.mask_rectangle; }
		}

		public ToolGroup_Mask()
		{
			this.ID = (int)Tool.Mask_ToolGroup;
		}
	}
}
