using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore
{
	public partial class ChangeLatticeSize : Form
	{
		#region [ Private Variables ]

		private Workshop _workshop;
		private int _bgWidth;
		private int _bgHeight;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public Workshop Workshop
		{
			get 
			{
				_workshop.UI.LatticeSize = new Size(Convert.ToInt32(txtXRes.Text), Convert.ToInt32(txtYRes.Text));
				return _workshop;
			}
			set 
			{
				_workshop = value;
				txtXRes.Text = UISettings.ʃLatticeSize.Width.ToString();
				txtYRes.Text = UISettings.ʃLatticeSize.Height.ToString();

				if (!_workshop.UI.Background.HasData)
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
					_bgHeight = _workshop.UI.Background.Image.Height;
					_bgWidth = _workshop.UI.Background.Image.Width;
				}
				CalcImageCellSize();
			}
		}

		public int CanvasHeight
		{
			get { return Convert.ToInt32(txtYRes.Text); }
		}

		public int CanvasWidth
		{
			get { return Convert.ToInt32(txtXRes.Text); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChangeLatticeSize()
		{
			InitializeComponent();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void CalcImageCellSize()
		{
			if ((_workshop.UI.CellSize == 0) || (_bgWidth == 0))
				return;

			lblBGWidth.Text = _bgWidth.ToString();
			lblBGHeight.Text = _bgHeight.ToString();

			lblBGXRes.Text = (Math.Ceiling((float)_bgWidth / (float)(_workshop.UI.CellSize + _workshop.UI.GridLineWidth))).ToString();
			lblBGYRes.Text = (Math.Ceiling((float)_bgHeight / (float)(_workshop.UI.CellSize + _workshop.UI.GridLineWidth))).ToString();
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void cmdResize_Click(object sender, EventArgs e)
		{
			txtXRes.Text = lblBGXRes.Text;
			txtYRes.Text = lblBGYRes.Text;
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			if (txtXRes.TextLength == 0)
			{
				MessageBox.Show("Width cannot be left blank.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtXRes.Focus();
				return;
			}
			if (txtYRes.TextLength == 0)
			{
				MessageBox.Show("Height cannot be left blank.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtYRes.Focus();
				return;
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		#endregion [ Events ]

	}
}
