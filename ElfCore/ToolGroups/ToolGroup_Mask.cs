using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfCore.ToolGroups
{
	[ElfEditToolGroup("Mask")]
	public class ToolGroup_Mask : BaseToolGroup, IToolGroup
	{
		public ToolGroup_Mask()
			: base()
		{
			this.ID = (int)ToolID.Mask_ToolGroup;
		}
	}
}
