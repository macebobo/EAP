using System.Windows.Forms;

using ElfCore.Interfaces;
using ElfCore.Util;

namespace ElfTools.ToolGroups {
    [ElfToolGroup("Shape")]
    [ElfToolCore]
    public class ToolGroup_Shape : BaseToolGroup, IToolGroup {
        public ToolGroup_Shape() {
            ID = (int) ToolID.Shape_ToolGroup;
            MultiGestureKey1 = Keys.S;
        }
    }
}