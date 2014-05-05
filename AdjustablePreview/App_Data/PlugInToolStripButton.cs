using System.Windows.Forms;

namespace ElfCore
{
	public class PlugInToolStripButton : ToolStripButton
	{
		internal PlugIn.ToolHost ToolHost
		{ get; set; }
	}
}
