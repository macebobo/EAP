using System;
using System.Windows.Forms;
using ElfCore.Controllers;
using ElfCore.Profiles;

namespace ElfCore.Forms
{
	public partial class Brightness : Form
	{

		#region [ Private Variables ]

		private int _initialValue = 10;
		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Brightness()
		{
			InitializeComponent();

			_initialValue = Profile.Background.Brightness;
			tbBrightness.Value = _initialValue;
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
			Profile.Background.Brightness = value;
			Profile.Background.Set();
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
