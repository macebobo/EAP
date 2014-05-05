using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfCore.ToolGroups
{
	[ElfEditToolGroup("MultiChannelLine")]
	public class ToolGroup_MultiChannelLine : BaseToolGroup, IToolGroup
	{
		public ToolGroup_MultiChannelLine()
			: base()
		{
			base.ID = (int)ToolID.MultiChannel_ToolGroup;
		}
	}
}
