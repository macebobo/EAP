using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfCore.ToolGroups
{
	[ElfEditToolGroup("Shape")]
	public class ToolGroup_Shape : BaseToolGroup, IToolGroup
	{
		public ToolGroup_Shape()
			: base()
		{
			this.ID = (int)ToolID.Shape_ToolGroup;
		}
	}
}
