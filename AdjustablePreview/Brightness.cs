using System;
using System.Windows.Forms;

namespace ElfCore
{
	public partial class Brightness : Form
	{

		#region [ Private Variables ]

		private int _initialValue = 10;
		private Workshop _workshop = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public Workshop Workshop
		{
			set
			{
				_workshop = value;
				_initialValue = _workshop.UI.Background.Brightness;
				tbBrightness.Value = _initialValue;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Brightness()
		{
			InitializeComponent();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#endregion [ Methods ]

		/// <summary>
		/// Set the Brightness of the background image
		/// </summary>
		/// <param name="value">Value to use to set the brightness. Valid range is 1 to 20</param>
		private void SetBrightness(int value)
		{
			_workshop.UI.Background.Brightness = value;
			_workshop.UI.Background.Set();
		}

		#region [ Events ]

		private void tbBrightness_ValueChanged(object sender, EventArgs e)
		{
			SetBrightness(tbBrightness.Value);
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			SetBrightness(_initialValue);
		}

		#endregion [ Events ]
	}
}
