using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace ElfPreview
{
	public partial class PreviewDialog : Vixen.OutputPlugInUIBase
	{

		#region [ Private Variables ]

		private PlaybackProfile _profile = null;
		private Settings _settings = Settings.Instance;
		private Size _borderSize;
		private float _deltaFPSTime = 0;
		private DateTime _lastCheckedTime;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public PreviewDialog()
		{
			InitializeComponent();
		}

		public PreviewDialog(XmlNode setupNode, List<Vixen.Channel> channels, int startChannel)
			: this()
		{
			if (setupNode == null)
				throw new ArgumentNullException("Setup Node is null");

			this.Left = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, this.Left);
			this.Top = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, this.Top);

			lblTime.Visible = false;
			cmdRecord.Visible = false;
			lblRecording.Visible = false;
			_profile = new PlaybackProfile(setupNode, channels, startChannel, this.CanvasPane);
			SetCanvasPaneSize(_profile.CanvasSize);
			_lastCheckedTime = DateTime.Now;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void SetCanvasPaneSize(Size canvasSize)
		{
			// Get the difference in the actual size vs the size of the client rectangle
			_borderSize.Width = this.Size.Width - ClientRectangle.Width;
			_borderSize.Height = (this.Size.Height - ClientRectangle.Height) + pnlControls.Height;

			CanvasPane.Size = canvasSize;

			this.Width = canvasSize.Width + _borderSize.Width;
			this.Height = canvasSize.Height + _borderSize.Height;
		}

		public void UpdateWith(byte[] channelValues, string sequenceName)
		{
			this.Text = sequenceName;
			_profile.Alphas = channelValues;
			CanvasPane.Invalidate();

			// Calculate FPS
			TimeSpan ElapsedTime = DateTime.Now.Subtract(_lastCheckedTime);
			_lastCheckedTime = DateTime.Now;
			float Elapsed = (float)ElapsedTime.TotalSeconds;
			_deltaFPSTime += Elapsed;

			if (_deltaFPSTime > 1)
			{
				float fps2 = 1 / Elapsed;
				lblFrameRate.Text = string.Format("FPS: {0:N1}", fps2);
				_deltaFPSTime -= 1;
			}
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void cmdStop_Click(object sender, EventArgs e)
		{
			if (base.ExecutionParent != null)
				base.ExecutionParent.Notify(Vixen.Notification.KeyDown, new KeyEventArgs(Keys.F8));
			else
				pnlControls.Enabled = false;
		}

		private void cmdPause_Click(object sender, EventArgs e)
		{
			if (base.ExecutionParent != null)
				base.ExecutionParent.Notify(Vixen.Notification.KeyDown, new KeyEventArgs(Keys.F7));
			else
				pnlControls.Enabled = false;
		}

		private void cmdPlayFromPoint_Click(object sender, EventArgs e)
		{
			if (base.ExecutionParent != null)
				base.ExecutionParent.Notify(Vixen.Notification.KeyDown, new KeyEventArgs(Keys.F6));
			else
				pnlControls.Enabled = false;
		}

		private void cmdPlay_Click(object sender, EventArgs e)
		{
			if (base.ExecutionParent != null)
				base.ExecutionParent.Notify(Vixen.Notification.KeyDown, new KeyEventArgs(Keys.F5));
			else
				pnlControls.Enabled = false;
		}

		/// <summary>
		/// Using the AviFile Library
		/// http://www.codeproject.com/Articles/7388/A-Simple-C-Wrapper-for-the-AviFile-Library
		/// </summary>
		private void cmdRecord_Click(object sender, EventArgs e)
		{
				
		}

		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			if (base.ExecutionParent != null)
				base.ExecutionParent.Notify(Vixen.Notification.KeyDown, e);
		}

		private void CanvasPane_Paint(object sender, PaintEventArgs e)
		{
			_profile.Draw(e);
		}

		private void Form_Shown(object sender, EventArgs e)
		{
			if (this.MdiParent != null)
				this.MdiParent = null;
		}

		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = (e.CloseReason == CloseReason.UserClosing);
			if (e.Cancel)
			{
				cmdStop_Click(null, null);
				this.Hide();
			}

			if (this.WindowState == FormWindowState.Normal)
			{
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, this.Left);
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, this.Top);
			}
			_settings.Save();
		}

		#endregion [ Events ]

	}
}
