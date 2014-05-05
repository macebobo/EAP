using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfTools.ToolGroups {
    [ElfToolGroup("Mask")]
    [ElfToolCore]
    public class ToolGroup_Mask : BaseToolGroup, IToolGroup {
        public ToolGroup_Mask() {
            ID = (int) ToolID.Mask_ToolGroup;
            MultiGestureKey1 = Keys.M;
        }
    }
}