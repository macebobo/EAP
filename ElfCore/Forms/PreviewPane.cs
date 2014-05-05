using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using ElfCore.Util;
using ElfCore.Controllers;
using ElfCore.Profiles;
using ElfCore.Channels;

namespace ElfCore.Forms
{
	public partial class PreviewPane : Form
	{
		#region [ Declares ]

		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

		[DllImport("USER32.DLL")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		#endregion [ Declares ]

		#region [ Constants ]

		private const int CS_NOCLOSE = 0x0200;
		private const int CS_DROPSHADOW = 0x00020000;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private byte[] _channelValues;
		//private uint[,] _backBuffer;
		//private int _startChannel;
		//private int _cellSize = 8;
		//private int _gridLineWidth = 1;
		private Workshop _workshop = Workshop.Instance;
		private BaseProfile _profile = null;
		private List<Color> _colors = new List<Color>();
		private List<GraphicsPath> _paths = new List<GraphicsPath>();
		protected int _numChannels = -1;
		private ProfileType _profileType = ProfileType.NotSet;

		#endregion [ Private Variables ]

		#region [ Properties ]

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				//cp.ClassStyle |= CS_NOCLOSE;
				return cp;
			}
		}

		public Profiles.BaseProfile Profile { get; set; }

		public ProfileType ProfileType
		{
			get { return _profileType; }
			set { _profileType = value; }
		}

		#endregion [ Properties ]
		
		#region [ Constructors ]

		public PreviewPane()
		{
			InitializeComponent();
		}

		public PreviewPane(XmlNode setupNode, ProfileType profileType, int numChannels, int startChannel)
			: this()
		{
			if (setupNode == null)
				throw new ArgumentNullException("Setup Node is null");
			_workshop.ProfileController.Load(setupNode, profileType);
			_profile = _workshop.ProfileController.Active;

			CanvasPane.BackColor = _profile.Background.Color;
			_profile.Background.Set();

			for (int i = startChannel; i < startChannel + numChannels; i++)
			{
				_colors.Add(_profile.Channels[i].GetColor());
				_paths.Add(_profile.Channels[i].GetGraphicsPath());
			}

			SetCanvasPaneSize(_profile.Scaling.CanvasSize.Width, _profile.Scaling.CanvasSize.Height);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void GetVixenProcess()
		{
			// Get a handle to the Vixen application. 
			IntPtr handle = FindWindow(null, "Vixen");

			// Verify that Vixen is a running process. 
			if (handle == IntPtr.Zero)
				return;

			// Make Vixen the foreground application 
			SetForegroundWindow(handle);
		}

		private void SetCanvasPaneSize(int width, int height)
		{
			int widthDelta = width - CanvasPane.Width;
			int heightDelta = height - CanvasPane.Height;

			this.Width += widthDelta;
			this.Height += heightDelta;
			CanvasPane.Width += widthDelta;
			CanvasPane.Height += heightDelta;
		}

		public void UpdateWith(byte[] channelValues)
		{
			_channelValues = channelValues;
			CanvasPane.Refresh();
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void cmdStop_Click(object sender, EventArgs e)
		{
			GetVixenProcess();
			SendKeys.Send("{F8}");
		}

		private void cmdPause_Click(object sender, EventArgs e)
		{
			GetVixenProcess();
			SendKeys.Send("{F7}");
		}

		private void cmdPlayFromPoint_Click(object sender, EventArgs e)
		{
			GetVixenProcess();
			SendKeys.Send("{F6}");
		}

		private void cmdPlay_Click(object sender, EventArgs e)
		{
			GetVixenProcess();
			SendKeys.Send("{F5}");
		}

		/// <summary>
		/// Using the AviFile Library
		/// http://www.codeproject.com/Articles/7388/A-Simple-C-Wrapper-for-the-AviFile-Library
		/// </summary>
		private void cmdRecord_Click(object sender, EventArgs e)
		{
			//_tempWavFilename = Path.GetTempFileName();
			//_tempAVIFilename = Path.GetTempFileName();
			//FileInfo fileInfo = new FileInfo(_tempWavFilename);
			//fileInfo.Attributes = FileAttributes.Temporary;
			//fileInfo = new FileInfo(_tempAVIFilename);
			//fileInfo.Attributes = FileAttributes.Temporary;

			//_recording = true;
			//lblRecording.Visible = true;

			//// Start the image recording
			//_aviManager = new AviManager(_tempAVIFilename, false);
			//_aviStream = _aviManager.AddVideoStream(true, (1000.0 / ((Vixen.VixenMDI)this.Parent).Sequence.EventPeriod), (Bitmap)pictureBoxShowGrid.Image);

			//// Start the sound recording
			//waveIn = new WasapiLoopbackCapture();
			//writer = new WaveFileWriter(_tempWavFilename, waveIn.WaveFormat);
			//waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
			//waveIn.RecordingStopped += waveIn_RecordingStopped;
			//waveIn.StartRecording();		
		}

		private void PreviewDialog_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F5:
					cmdPlay_Click(null, new EventArgs());
					break;
				case Keys.F6:
					cmdPlayFromPoint_Click(null, new EventArgs());
					break;
				case Keys.F7:
					cmdPause_Click(null, new EventArgs());
					break;
				case Keys.F8:
					cmdStop_Click(null, new EventArgs());
					break;
			}
		}

		private void CanvasPane_Paint(object sender, PaintEventArgs e)
		{
			for (int i = 0; i < _numChannels; i++)
			{
				using (SolidBrush ChannelBrush = new SolidBrush(Color.FromArgb(_channelValues[i], _colors[i])))
					e.Graphics.FillPath(ChannelBrush, _paths[i]);
			}
		}

		#endregion [ Events ]
	}
}
