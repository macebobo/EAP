using ElfCore.Controllers;
using ElfCore.Profiles;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ElfCore.Forms
{
	public partial class ChangeLatticeSize : Form
	{
		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private int _bgWidth;
		private int _bgHeight;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden]
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		public int LatticeHeight
		{
			get { return Convert.ToInt32(txtHeight.Text); }
		}

		public int LatticeWidth
		{
			get { return Convert.ToInt32(txtWidth.Text); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChangeLatticeSize()
		{
			InitializeComponent();

			if (Profile == null)
				throw new NullReferenceException("Profile is null");
			if (Profile.Scaling == null)
				throw new NullReferenceException("Profile.Scaling is null");

			txtWidth.Text = Profile.Scaling.LatticeSize.Width.ToString();
			txtHeight.Text = Profile.Scaling.LatticeSize.Height.ToString();

			bool HasBGImage = Profile.Background.HasData && (Profile.Background.Image != null);

			if (!HasBGImage)
			{
				lblNoBGImage.Visible = true;

				cmdResize.Visible = false;
				lblBGYRes.Visible = false;
				lblBGXRes.Visible = false;
				lblBGCells.Visible = false;
				lblBGX2.Visible = false;
				lblBGHeight.Visible = false;
				lblBGWidth.Visible = false;
				lblBGPixels.Visible = false;
				lblBGX1.Visible = false;
				_lblBGHeight.Visible = false;
				_lblBGWidth.Visible = false;
			}
			else
			{
				_bgHeight = Profile.Background.Image.Height;
				_bgWidth = Profile.Background.Image.Width;
			}
			CalcImageCellSize();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void CalcImageCellSize()
		{
			if ((Profile.Scaling.CellSize == 0) || (_bgWidth == 0))
				return;

			lblBGWidth.Text = _bgWidth.ToString();
			lblBGHeight.Text = _bgHeight.ToString();

			lblBGXRes.Text = (((float)_bgWidth / Profile.Scaling.CellGridF)).ToString();
			lblBGYRes.Text = (((float)_bgHeight / Profile.Scaling.CellGridF)).ToString();
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void cmdResize_Click(object sender, EventArgs e)
		{
			if ((Profile.Scaling.CellSize == 0) || (_bgWidth == 0))
				return;

			txtWidth.Text = ((int)Math.Ceiling((float)_bgWidth / Profile.Scaling.CellGridF)).ToString();
			txtHeight.Text = ((int)Math.Ceiling((float)_bgHeight / Profile.Scaling.CellGridF)).ToString();
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			if (txtWidth.TextLength == 0)
			{
				MessageBox.Show("Width cannot be left blank.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtWidth.Focus();
				return;
			}
			if (txtHeight.TextLength == 0)
			{
				MessageBox.Show("Height cannot be left blank.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtHeight.Focus();
				return;
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		private void TextBox_Enter(object sender, EventArgs e)
		{
			((TextBox)sender).SelectAll();
		}

		/// <summary>
		/// Only allow the character 0 through 9 in this textbox.
		/// </summary>
		private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b')
				e.Handled = ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)));
		}

		#endregion [ Events ]

	}
}
