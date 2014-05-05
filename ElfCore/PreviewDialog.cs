using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using Vixen;
using ElfCore;
//using NAudio.Wave;
//using AviFile;

namespace AdjustablePreview
{
	public partial class PreviewDialog : OutputPlugInUIBase
	{
		#region [ Declares ]

		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		#endregion [ Declares ]

		#region [ Constants ]

		private const int CS_NOCLOSE = 0x0200;
		private const int CS_DROPSHADOW = 0x00020000;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Dictionary<int, List<uint>> _ChannelDictionary; // Channel index : pixel coords
		private Color[] _ChannelColors;
		private Image _originalBackground;
		private SolidBrush _ChannelBrush = new SolidBrush(Color.Black);
		private byte[] _ChannelValues = { };
		private uint[,] _backBuffer;
		private int _startChannel;
		private int _cellSize = 8;
		private int _gridLineWidth = 1;
		
		private Settings _settings = Settings.Instance;

		//private bool _recording = false;
		//private IWaveIn waveIn;
		//private WaveFileWriter writer;
		//private AviManager _aviManager = null;
		//private VideoStream _aviStream = null;
		//private string _tempWavFilename = string.Empty;
		//private string _tempAVIFilename = string.Empty;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public PreviewDialog(XmlNode setupNode, List<Vixen.Channel> Channels, int startChannel)
		{
			int ChannelNumber;
			byte[] bytes;
			List<uint> ChannelPixelCoords;
			int i;
			int Width = 32;
			int Height = 32;
			int EventPeriod = 0;
			//

			InitializeComponent();

			try
			{
				//_settings = new Settings(SettingsStyle.Xml);

				this.Left = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, this.Left);
				this.Top = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, this.Top);
				this.Width = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_WIDTH, this.Width);
				this.Height = _settings.GetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_HEIGHT, this.Height);

				if (setupNode.SelectSingleNode("EventPeriod") != null)
				{
					EventPeriod = Convert.ToInt32(setupNode.SelectSingleNode("EventPeriod").InnerText);
					lblFrameRate.Text = (1000.0 / EventPeriod).ToString("0.0") + " fps";
				}
				_startChannel = startChannel;
				_ChannelDictionary = new Dictionary<int, List<uint>>();

				bool redirectOutputs = bool.Parse(Vixen.Xml.GetNodeAlways(setupNode, "RedirectOutputs", "False").InnerText);

				// Remap Channel colors
				_ChannelColors = new Color[Channels.Count - startChannel];
				int outputChannel;
				for (int ChannelIndex = startChannel; ChannelIndex < Channels.Count; ChannelIndex++)
				{
					if (redirectOutputs)
					{
						outputChannel = ChannelIndex;
					}
					else
					{
						outputChannel = Channels[ChannelIndex].OutputChannel;
					}
					if (outputChannel >= startChannel)
					{
						_ChannelColors[outputChannel - startChannel] = Channels[ChannelIndex].Color;
					}
				}

				// Get Channel pixel lists
				// and remap on the fly
				foreach (XmlNode ChannelNode in setupNode.SelectNodes("Channels/Channel"))
				{
					ChannelNumber = Convert.ToInt32(ChannelNode.Attributes["number"].Value);
					if (ChannelNumber < _startChannel)
						continue;

					ChannelPixelCoords = new List<uint>();
					bytes = Convert.FromBase64String(ChannelNode.InnerText);
					for (i = 0; i < bytes.Length; i += 4)
					{
						ChannelPixelCoords.Add(BitConverter.ToUInt32(bytes, i));
					}

					if (redirectOutputs)
					{
						_ChannelDictionary[ChannelNumber - _startChannel] = ChannelPixelCoords;
					}
					else
					{
						_ChannelDictionary[Channels[ChannelNumber - _startChannel].OutputChannel] = ChannelPixelCoords;
					}
				}
				
				// background image
				byte[] imageByteArray = Convert.FromBase64String(setupNode.SelectSingleNode("BackgroundImage").InnerText);
				if (imageByteArray.Length > 0)
				{
					// bytes are the bytes from the original file
					System.IO.MemoryStream stream = new System.IO.MemoryStream(imageByteArray);
					_originalBackground = new Bitmap(stream);
					stream.Close();
					stream.Dispose();
				}

				// Display properties
				XmlNode DisplayNode = setupNode.SelectSingleNode("Display");
				_cellSize = Convert.ToInt32(DisplayNode.SelectSingleNode("PixelSize").InnerText);
				Width = Convert.ToInt32(DisplayNode.SelectSingleNode("Width").InnerText);
				Height = Convert.ToInt32(DisplayNode.SelectSingleNode("Height").InnerText);

				if (DisplayNode.SelectSingleNode("GridWidth") != null)
					_gridLineWidth = Convert.ToInt32(DisplayNode.SelectSingleNode("GridWidth").InnerText);

				// If no picture, then on the resolution.  Picture wins.
				// May have to store the size resulting in the setup.
				if (_originalBackground == null)
				{
					SetPictureBoxSize(Width * (_cellSize + _gridLineWidth), Height * (_cellSize + _gridLineWidth));
				}
				else
				{
					SetPictureBoxSize(_originalBackground.Width, _originalBackground.Height);
				}

				SetBrightness((int.Parse(DisplayNode.SelectSingleNode("Brightness").InnerText) - 10) / 10.0f);

				_backBuffer = new uint[Height, Width];
			}
			catch (NullReferenceException e)
			{
				throw new Exception(e.Message + "\n\nHave you run the setup?");
			}
		}

		#endregion [ Constructors ]

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

		#endregion [ Properties ]

		#region [ Methods ]

		public void UpdateWith(byte[] ChannelValues)
		{
			_ChannelValues = ChannelValues;
			Recalc();
			pictureBoxShowGrid.Refresh();
		}

		private void SetPictureBoxSize(int width, int height)
		{
			
			int widthDelta = width - pictureBoxShowGrid.Width;
			int heightDelta = height - pictureBoxShowGrid.Height;

			this.Width += widthDelta;
			this.Height += heightDelta;

			pictureBoxShowGrid.Width += widthDelta;
			pictureBoxShowGrid.Height += heightDelta;
		}

		private void SetBrightness(float value)
		{
			// Value is between -1.0 and 1.0
			if (_originalBackground == null)
				return;

			Image img = new Bitmap(_originalBackground);
			ColorMatrix cMatrix = new ColorMatrix(new float[][] {
						new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
						new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
						new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
						new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
						new float[] { value, value, value, 0.0f, 1.0f }
					});

			Graphics g = Graphics.FromImage(img);
			ImageAttributes attrs = new ImageAttributes();
			attrs.SetColorMatrix(cMatrix);

			g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attrs);
			g.Dispose();

			attrs.Dispose();
			pictureBoxShowGrid.BackgroundImage = img;

			cMatrix = null;
			g = null;
			attrs = null;
			img = null;
		}

		private void Recalc()
		{
			int i = 0;
			uint value;
			byte r, g, b;
			int x, y;
			Color color;

			byte maxIntensity;
			byte existingR, existingG, existingB;
			float existingA, newA;
			int maxX, maxY;

			Array.Clear(_backBuffer, 0, _backBuffer.Length);
			maxX = _backBuffer.GetLength(1);
			maxY = _backBuffer.GetLength(0);

			foreach (byte level in _ChannelValues)
			{
				color = _ChannelColors[i];
				if (_ChannelDictionary.ContainsKey(i) && level > 0)
				{
					// if Channel is drawn in the preview...
					foreach (uint xy in _ChannelDictionary[i])
					{
						x = (int)(xy >> 16);
						y = (int)(xy & 0xffff);
						if (x >= maxX || y >= maxY)
							continue;
						value = _backBuffer[y, x];

						if (value == 0)
						{
							// no previous color
							maxIntensity = level;
							r = color.R;
							g = color.G;
							b = color.B;
						}
						else
						{
							// there's a previous color to combine with
							maxIntensity = (byte)Math.Max(level, (byte)(value >> 24));
							existingA = (float)(value >> 24) / maxIntensity;
							newA = (float)level / maxIntensity;

							existingR = (byte)((value >> 16) & 0xff);
							existingG = (byte)((value >> 8) & 0xff);
							existingB = (byte)(value & 0xff);

							r = (byte)((int)(existingR * existingA + color.R * newA) >> 1);
							g = (byte)((int)(existingG * existingA + color.G * newA) >> 1);
							b = (byte)((int)(existingB * existingA + color.B * newA) >> 1);
						}
						value = (uint)((maxIntensity << 24) | (r << 16) | (g << 8) | b);
						_backBuffer[y, x] = value;
					}
				}
				i++;
			}
		}

		//private void StopRecordingWav()
		//{
		//    waveIn.StopRecording();
		//}

		public void Finished()
		{
			Array.Clear(_ChannelValues, 0, _ChannelValues.Length);
			Array.Clear(_ChannelColors, 0, _ChannelColors.Length);
			Array.Clear(_backBuffer, 0, _backBuffer.Length);
			_ChannelDictionary = null;

			//if (_recording)
			//{
			//    _aviManager.Close();
			//    StopRecordingWav();
			//    if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
			//    {
			//        _aviManager.AddAudioStream(_tempWavFilename, 0);
			//        System.IO.File.Move(_tempAVIFilename, saveFileDialog1.FileName);
			//        //AviManager.MakeFileFromStream(saveFileDialog1.FileName, editableStream);
			//        //editableStream.Close();
			//        //editableStream = null;
			//    }


			//    try
			//    {
			//        File.Delete(_tempAVIFilename);
			//        File.Delete(_tempWavFilename);
			//    }
			//    catch (IOException)
			//    { }
			//}
		}

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

		#endregion [ Methods ]

		#region [ Events ]

		private void pictureBoxShowGrid_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.Transparent, pictureBoxShowGrid.ClientRectangle);

			int height = _backBuffer.GetLength(0);
			int width = _backBuffer.GetLength(1);
			int pixelX, pixelY;
			int cellBoundary = _cellSize + _gridLineWidth;
			uint value;
			int x, y;

			for (pixelY = 0, y = 0; y < height; y++, pixelY += cellBoundary)
			{
				for (pixelX = 0, x = 0; x < width; x++, pixelX += cellBoundary)
				{
					value = _backBuffer[y, x];
					if ((value & 0xff000000) > 0)
					{
						// if alpha is 0, it's completely transparent
						_ChannelBrush.Color = Color.FromArgb((int)value);
						e.Graphics.FillRectangle(_ChannelBrush, pixelX, pixelY, _cellSize, _cellSize);
					}
				}
			}
			//if (_recording)
			//{
			//    // append this new image to the recording stream
			//    _aviStream.AddFrame((Bitmap)pictureBoxShowGrid.Image);
			//}
		}

		private void PreviewDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = (e.CloseReason == CloseReason.UserClosing);
			if (e.Cancel)
				this.Hide();

			if (this.WindowState == FormWindowState.Normal)
			{
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_LEFT, this.Left);
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_TOP, this.Top);
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_WIDTH, this.Width);
				_settings.SetValue(Constants.PREVIEW_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
			}
		}

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

		private void PreviewDialog_Shown(object sender, EventArgs e)
		{
			if (this.MdiParent != null)
				this.MdiParent = null;
			GetVixenProcess();
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

		//void waveIn_DataAvailable(object sender, WaveInEventArgs e)
		//{
		//    if (this.InvokeRequired)
		//    {
		//        //Debug.WriteLine("Data Available");
		//        this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
		//    }
		//    else
		//    {
		//        //Debug.WriteLine("Flushing Data Available");
		//        writer.Write(e.Buffer, 0, e.BytesRecorded);
		//        int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);
		//        if (secondsRecorded >= 30)
		//        {
		//            StopRecordingWav();
		//        }
		//    }
		//}
		
		//void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
		//{
		//    if (this.InvokeRequired)
		//    {
		//        this.BeginInvoke(new EventHandler<StoppedEventArgs>(waveIn_RecordingStopped), sender, e);
		//    }
		//    else
		//    {
		//        waveIn.Dispose();
		//        waveIn = null;
		//        writer.Close();
		//        writer = null;
		//        if (e.Exception != null)
		//        {
		//            MessageBox.Show(String.Format("A problem was encountered during recording {0}", e.Exception.Message));
		//        }
		//    }
		//}

		#endregion [ Events ]
	}
}