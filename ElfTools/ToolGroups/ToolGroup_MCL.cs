using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfTools.ToolGroups {
    [ElfToolGroup("MultiChannelLine")]
    [ElfToolCore]
    public class ToolGroup_MultiChannelLine : BaseToolGroup, IToolGroup {
        public ToolGroup_MultiChannelLine() {
            base.ID = (int) ToolID.MultiChannel_ToolGroup;
            MultiGestureKey1 = Keys.H;
        }
    }
}