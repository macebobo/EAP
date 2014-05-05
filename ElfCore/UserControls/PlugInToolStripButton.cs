using System.Windows.Forms;
using System.ComponentModel;

using ElfCore.PlugIn;

namespace ElfCore
{
	public class ToolStripButtonWithKeys : ToolStripButton
	{

		private string _pathNode = string.Empty;

		[Localizable(true)]
		[DefaultValue(typeof(Keys), "None"), Description("First key in the multi-gesture keystroke")]
		public Keys MultiGestureKey1 { get; set; }

		[Localizable(true)]
		[DefaultValue(typeof(Keys), "None"), Description("Second key in the multi-gesture keystroke")]
		public Keys MultiGestureKey2 { get; set; }

		/// <summary>
		/// First key in the multi-gesture keystroke
		/// </summary>
		[Description("Name of the Position in the path for this button when generating the list of multi-gesture keys.")]
		public string PathNode
		{
			get
			{
				if (_pathNode.Length == 0)
				{
					string Node = Text;
					Node = Node.Replace(" ", string.Empty).Replace(".", string.Empty);
					Node = Node.Replace("&", string.Empty).Replace(",", string.Empty);
					return Node;
				}
				return _pathNode;
			}
			set { _pathNode = value; }
		}
	}

	public class PlugInToolStripButton : ToolStripButtonWithKeys
	{
		public PlugInTool PlugInTool
		{ get; set; }

		public PlugInToolGroup PlugInToolGroup
		{ get; set; }
	}
}
