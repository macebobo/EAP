using System;
using System.Drawing;
using System.Windows.Forms;

using ElfCore.Controllers;

namespace ElfCore.Forms
{
	public partial class EditChannel : Form
	{

		private bool _isReadOnly = false;
		private Workshop _workshop = Workshop.Instance;

		#region [ Properties ]

		/// <summary>
		/// Name of the Channel.
		/// </summary>
		public string ChannelName
		{
			get { return txtName.Text; }
			set { txtName.Text = value; }
		}

		public Color SequencerColor
		{
			get { return cddSequencerColor.Color; }
			set { cddSequencerColor.Color = value; }
		}

		public Color RenderColor
		{
			get { return cddRenderColor.Color; }
			set { cddRenderColor.Color = value; }
		}

		public Color BorderColor
		{
			get { return cddBorderColor.Color; }
			set { cddBorderColor.Color = value; }
		}

		/// <summary>
		/// Is this Channel enabled?
		/// </summary>
		public bool IsEnabled
		{
			get { return chkEnabled.Checked; }
			set { chkEnabled.Checked = value; }
		}

		/// <summary>
		/// Is this Channel locked?
		/// </summary>
		public bool IsLocked
		{
			get { return chkLocked.Checked; }
			set { chkLocked.Checked = value; }
		}

		/// <summary>
		/// Is this Channel visible?
		/// </summary>
		public bool IsVisible
		{
			get { return chkVisible.Checked; }
			set { chkVisible.Checked = value; }
		}

		public bool ReadOnlyMode
		{
			set
			{
				_isReadOnly = value;
				if (!value)
					return;
				txtName.ReadOnly = true;
				chkEnabled.Enabled = false;
				chkLocked.Enabled = false;
				chkVisible.Enabled = false;
				cddBorderColor.Enabled = false;
				_cddBorderColor.Enabled = false;
				cddSequencerColor.Enabled = false;
				_cddSequencerColor.Enabled = false;
				cddRenderColor.Enabled = false;
				_cddRenderColor.Enabled = false;
				cmdCancel.Visible = false;
				cmdOk.Left = (Width - cmdOk.Width) / 2;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public EditChannel()
		{
			InitializeComponent();
			cddRenderColor.CustomColors = _workshop.CustomColors;
			cddSequencerColor.CustomColors = _workshop.CustomColors;
			cddBorderColor.CustomColors = _workshop.CustomColors;
			cddBorderColor.Enabled = false;
			_cddBorderColor.Enabled = false;
		}

		#endregion [ Constructors ]

		#region [ Events ]

		private void cmdOk_Click(object sender, EventArgs e)
		{
			if (!_isReadOnly && (txtName.Text.Trim().Length == 0))
			{
				MessageBox.Show("Name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtName.Focus();
			}
		}

		#endregion [ Events ]
	}
}
