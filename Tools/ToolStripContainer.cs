using System.Windows.Forms;

namespace Tools
{
	public partial class ToolStripContainer : ElfTools.Tools.ToolStripForm
	{
		#region [ Constructors ]

		public ToolStripContainer()
		{
			InitializeComponent();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		//public override void AddCustomControls()
		//{ }

		public override ToolStrip GetToolStrip(int ownerTool)
		{
			switch (ownerTool)
			{
				case 0:
				default:
					return this.TestTool1_ToolStrip;
			}
		}

		#endregion [ Methods ]
	}
}
