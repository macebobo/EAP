using System;
using System.Windows.Forms;

namespace ElfCore
{
	public partial class ConfigureKeys : Form
	{
		#region [ Private Variables ]

		//private bool _shift = false;
		//private bool _control = false;
		//private bool _alt = false;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public ConfigureKeys()
		{
			InitializeComponent();
			txtExisting.Text = string.Empty;
		}

		#endregion [ Constructors ]

		#region [ Events ]

		private void txtNewKey_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void cmdAssign_Click(object sender, EventArgs e)
		{

		}

		private void cmdDefault_Click(object sender, EventArgs e)
		{

		}

		private void txtNewKey_KeyUp(object sender, KeyEventArgs e)
		{
			//_shift = false;
			//_control = false;
			//_alt = false;
		}

		private void txtNewKey_KeyDown(object sender, KeyEventArgs e)
		{
			//Debug.WriteLine(e.KeyCode.ToString());
		}

		#endregion [ Events ]
	}
}