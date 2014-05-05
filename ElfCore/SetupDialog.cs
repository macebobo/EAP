using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Vixen;

namespace AdjustablePreview
{
	public partial class SetupDialog : Form
	{
		private XmlNode m_setupNode = null;
		private SetupData m_setupData = null;
		private string m_backgroundImageFileName = string.Empty;
		private bool m_dirty = false;
		private int m_cellSize;
		private Dictionary<int, List<uint>> m_ChannelDictionary = new Dictionary<int, List<uint>>();
		private List<Vixen.Channel> m_Channels = null;
		private bool m_controlDown = false;
		private int m_startChannel; // index of first Channel in range of data sent
		private Bitmap m_originalBackground = null;
		private bool m_resizing = false;
		private Workshop _workshop = null;
		private Settings _settings;

		public SetupDialog(Workshop workshop, Settings settings)
		{
			InitializeComponent();
			//_registry = new Registry(string.Format(Registry.REGISTRY_ADDIN_PATH, "Adjustable Preview"));
			
			_settings = settings;
			_workshop = workshop;

			m_cellSize = _workshop.UI.CellSize;
			toolStripComboBoxPixelSize.SelectedIndex = _workshop.UI.CellSize - 1;

			toolStripComboBoxChannels.BeginUpdate();
			m_Channels = new List<Vixen.Channel>();
			//m_startChannel = workshop.SetupStartChannel;
			//List<uint> ChannelPixelCoords;

			//byte[] bytes;

			//foreach (Channel Channel in _workshop.Channels)
			//{
			//    toolStripComboBoxChannels.Items.Add(Channel.ToString(true));
			//    m_Channels.Add(Channel.VixenChannel);

			//    ChannelPixelCoords = new List<uint>();
			//    bytes = Convert.FromBase64String(Channel.EncodeChannelBytes());
			//    for (int i = 0; i < bytes.Length; i += 4)
			//    {
			//        ChannelPixelCoords.Add(BitConverter.ToUInt32(bytes, i));
			//    }
			//    m_ChannelDictionary[Channel.Index - m_startChannel] = ChannelPixelCoords;
			//}
			toolStripComboBoxChannels.EndUpdate();

			checkBoxRedirectOutputs.Checked = _workshop.UI.RespectChannelOutputsDuringPlayback;

			SetPictureBoxSize(_workshop.UI.LatticeSize.Width * (m_cellSize + 1), _workshop.UI.LatticeSize.Width * (m_cellSize + 1));
			UpdateSizeUI();

			// Get any background image
			SetBackground(_workshop.UI.Background.Image);
			//trackBarBrightness.Value = _workshop.ConvertBrightnessValueToSlide(_workshop.UI.Background.Brightness);
			trackBarBrightness.Value = _workshop.UI.Background.Brightness;
			trackBarBrightness_ValueChanged(null, null);

			//toolStripComboBoxChannels.SelectedIndex = _workshop.ActiveChannelIndex;

			pictureBoxSetupGrid.Refresh();

			this.Left = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, this.Left);
			this.Top = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, this.Top);
			this.Width = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, this.Width);
			this.Height = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
		}

		public SetupDialog(SetupData setupData, XmlNode setupNode, List<Vixen.Channel> Channels, int startChannel)
		{
			InitializeComponent();
			//_registry = new Registry(string.Format(Registry.REGISTRY_ADDIN_PATH, "Adjustable Preview"));
			int i;

			m_setupData = setupData;
			m_setupNode = setupNode;
			m_startChannel = startChannel;
			toolStripComboBoxPixelSize.SelectedIndex = 7;

			toolStripComboBoxChannels.BeginUpdate();			
			for (i = m_startChannel; i < Channels.Count; i++)
			{
				toolStripComboBoxChannels.Items.Add(string.Format("{0}: {1}", i + 1, Channels[i].Name));
			}
			toolStripComboBoxChannels.EndUpdate();

			m_Channels = new List<Vixen.Channel>();
			for (i = m_startChannel; i < Channels.Count; i++)
			{
				m_Channels.Add(Channels[i]);
			}

			LoadFromSetupNode();

			this.Left = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, this.Left);
			this.Top = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, this.Top);
			this.Width = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, this.Width);
			this.Height = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
		}

		private void LoadFromSetupNode()
		{
			checkBoxRedirectOutputs.Checked = m_setupData.GetBoolean(m_setupNode, "RedirectOutputs", false);

			// Get display properties
			XmlNode displayNode;
			int width, height;
			if ((displayNode = m_setupNode.SelectSingleNode("Display")) != null)
			{
				height = Convert.ToInt32(displayNode.SelectSingleNode("Height").InnerText);
				width = Convert.ToInt32(displayNode.SelectSingleNode("Width").InnerText);
				m_cellSize = Convert.ToInt32(displayNode.SelectSingleNode("PixelSize").InnerText);
			}
			else
			{
				height = 32;
				width = 64;
				m_cellSize = 8;
			}

			SetPictureBoxSize(width * (m_cellSize + 1), height * (m_cellSize + 1));
			UpdateSizeUI();

			// Get any background image
			byte[] imageByteArray = Convert.FromBase64String(m_setupNode.SelectSingleNode("BackgroundImage").InnerText);
			if (imageByteArray.Length > 0)
			{
				// bytes are the bytes from the original file
				MemoryStream stream = new MemoryStream(imageByteArray);
				SetBackground(new Bitmap(stream));
				stream.Dispose();
			}
			else
			{
				SetBackground(null);
			}

			if (displayNode != null)
			{
				trackBarBrightness.Value = m_setupData.GetInteger(displayNode, "Brightness", 10);
				trackBarBrightness_ValueChanged(null, null);
			}

			// Get Channel pixel lists
			int ChannelNumber;
			byte[] bytes;
			List<uint> ChannelPixelCoords;
			int i;
			foreach (XmlNode ChannelNode in m_setupNode.SelectNodes("Channels/Channel"))
			{
				ChannelNumber = Convert.ToInt32(ChannelNode.Attributes["number"].Value);
				if (ChannelNumber < m_startChannel)
					continue;

				ChannelPixelCoords = new List<uint>();
				bytes = Convert.FromBase64String(ChannelNode.InnerText);
				for (i = 0; i < bytes.Length; i += 4)
				{
					ChannelPixelCoords.Add(BitConverter.ToUInt32(bytes, i));
				}
				m_ChannelDictionary[ChannelNumber - m_startChannel] = ChannelPixelCoords;
			}

			pictureBoxSetupGrid.Refresh();
		}

		private void UpdateSizeUI()
		{
			toolStripTextBoxResolutionX.Text = (pictureBoxSetupGrid.Width / (m_cellSize + 1)).ToString();
			toolStripTextBoxResolutionY.Text = (pictureBoxSetupGrid.Height / (m_cellSize + 1)).ToString();
			toolStripComboBoxPixelSize.SelectedIndex = m_cellSize - 1;
		}

		private void pictureBoxSetupGrid_MouseEvent(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0)
				return;

			int xIndex = e.X / (m_cellSize + 1);
			int yIndex = e.Y / (m_cellSize + 1);

			if (!(xIndex < pictureBoxSetupGrid.Width && yIndex < pictureBoxSetupGrid.Height))
				return;

			if (e.Button == MouseButtons.Left)
			{
				// left - paint
				SetPixelChannelReference(toolStripComboBoxChannels.SelectedIndex, xIndex, yIndex);
			}
			else if (e.Button == MouseButtons.Right)
			{
				// right - erase
				if (!m_controlDown)
				{
					// Selected Channel
					ResetPixelChannelReference(toolStripComboBoxChannels.SelectedIndex, xIndex, yIndex);
				}
				else
				{
					// Any Channel
					ResetPixelChannelReference(-1, xIndex, yIndex);
				}
			}
			else
			{
				// mouse over - show assigned Channels, if any
				uint xy = (uint)(xIndex << 16) | (uint)yIndex;
				StringBuilder sb = new StringBuilder();
				foreach (int ChannelNumber in m_ChannelDictionary.Keys)
				{
					if (ChannelNumber >= m_Channels.Count)
						continue;
					if (m_ChannelDictionary[ChannelNumber].Contains(xy))
					{
						sb.AppendFormat("{0}, ", toolStripComboBoxChannels.Items[ChannelNumber]);
					}
				}
				if (sb.Length > 0)
				{
					string s = sb.ToString();
					labelChannel.Text = s.Substring(0, s.Length - 2);
				}
				else
				{
					labelChannel.Text = string.Empty;
				}
			}
		}

		private void SetPixelChannelReference(int ChannelIndex, int x, int y)
		{
			//if (ChannelIndex == -1)
			//    return;

			//List<uint> coordList;
			//if (!m_ChannelDictionary.TryGetValue(ChannelIndex, out coordList))
			//{
			//    coordList = new List<uint>();
			//    m_ChannelDictionary[ChannelIndex] = coordList;
			//}
			//uint coord = (uint)(x << 16) | (uint)y;
			//if (!coordList.Contains(coord))
			//{
			//    coordList.Add(coord);
			//}
			//_workshop.Channels[ChannelIndex].PaintCell(new Point(x, y));
			//pictureBoxSetupGrid.Invalidate(new Rectangle(x * (m_cellSize + 1), y * (m_cellSize + 1), m_cellSize, m_cellSize));
			//m_dirty = true;
		}

		private void ResetPixelChannelReference(int ChannelIndex, int x, int y)
		{
			//Point Cell = new Point(x, y);
			//if (ChannelIndex == -1)
			//{
			//    // Nasty way of doing it, but the other way requires maintaining a list of
			//    // Channels that are affecting a given cell in a hash.
			//    uint xy = (uint)(x << 16) | (uint)y;
			//    foreach (List<uint> pixelList in m_ChannelDictionary.Values)
			//    {
			//        pixelList.Remove(xy);
			//    }
			//    pictureBoxSetupGrid.Invalidate(new Rectangle(x * (m_cellSize + 1), y * (m_cellSize + 1), m_cellSize, m_cellSize));
			//    foreach (Channel Channel in _workshop.Channels)
			//        Channel.EraseCell(Cell);
			//}
			//else
			//{
			//    List<uint> coordList;
			//    if (m_ChannelDictionary.TryGetValue(ChannelIndex, out coordList))
			//    {
			//        coordList.Remove((uint)((x << 16) | y));
			//        pictureBoxSetupGrid.Invalidate(new Rectangle(x * (m_cellSize + 1), y * (m_cellSize + 1), m_cellSize, m_cellSize));
			//        _workshop.Channels[ChannelIndex].EraseCell(Cell);
			//    }
			//}
			//m_dirty = true;
		}

		private void pictureBoxSetupGrid_Paint(object sender, PaintEventArgs e)
		{
			int xpos, ypos;
			int selectedChannelIndex;
			Vixen.Channel Channel;
			bool hasBackgroundImage = m_originalBackground != null;
			SolidBrush ChannelBrush = new SolidBrush(Color.Black);

			selectedChannelIndex = toolStripComboBoxChannels.SelectedIndex;
			//full color: if selectedChannelIndex = -1 or cell contains selectedChannelIndex (and draw that color)
			//if -1, draw the top-most color
			//better to clear the whole thing, draw all Channels, then redraw the selected Channel?
			// Clear the whole thing
			if (hasBackgroundImage)
			{
				e.Graphics.FillRectangle(Brushes.Transparent, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
			}
			else
			{
				e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
			}
			// Draw all Channels (dimmed), minus the selected one
			foreach (int ChannelIndex in m_ChannelDictionary.Keys)
			{
				if (ChannelIndex == selectedChannelIndex)
					continue;
				if (ChannelIndex >= m_Channels.Count)
					continue;
				ChannelBrush.Color = Color.FromArgb(128, m_Channels[ChannelIndex].Color);
				foreach (uint xy in m_ChannelDictionary[ChannelIndex])
				{
					xpos = (int)(xy >> 16) * (m_cellSize + 1);
					ypos = (int)(xy & 0xffff) * (m_cellSize + 1);
					if (e.ClipRectangle.Contains(xpos, ypos))
					{
						e.Graphics.FillRectangle(ChannelBrush, xpos, ypos, m_cellSize, m_cellSize);
					}
				}
			}
			// Draw the selected Channel (full intensity)
			if (m_ChannelDictionary.ContainsKey(selectedChannelIndex))
			{
				Channel = m_Channels[selectedChannelIndex];
				foreach (uint xy in m_ChannelDictionary[selectedChannelIndex])
				{
					xpos = (int)(xy >> 16) * (m_cellSize + 1);
					ypos = (int)(xy & 0xffff) * (m_cellSize + 1);
					if (e.ClipRectangle.Contains(xpos, ypos))
					{
						e.Graphics.FillRectangle(Channel.Brush, xpos, ypos, m_cellSize, m_cellSize);
					}
				}
			}

			ChannelBrush.Dispose();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Save();
			m_dirty = false;
		}

		private void SaveSettingsToRegistry()
		{
			if (this.WindowState == FormWindowState.Normal)
			{
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, this.Left);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, this.Top);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, this.Width);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
			}
			//_settings.SetValue(Constants.ORIGINAL_UI, _workshop.UI.UseOriginalUI);
			_settings.Save();
		}

		private void SetPictureBoxSize(int width, int height)
		{
			pictureBoxSetupGrid.Width = width;
			pictureBoxSetupGrid.Height = height;
			UpdatePosition();
			pictureBoxSetupGrid.Refresh();
		}

		private void UpdatePosition()
		{
			Point point = new Point();
			if (pictureBoxSetupGrid.Width > panelPictureBoxContainer.Width)
			{
				point.X = 0;
			}
			else
			{
				point.X = (panelPictureBoxContainer.Width - pictureBoxSetupGrid.Width) / 2 + ClientRectangle.Left;
			}
			if (pictureBoxSetupGrid.Height > panelPictureBoxContainer.Height)
			{
				point.Y = 0;
			}
			else
			{
				point.Y = (panelPictureBoxContainer.Height - pictureBoxSetupGrid.Height) / 2 + ClientRectangle.Top;
			}

			pictureBoxSetupGrid.Location = point;

			// Update brightness trackbar position, too
			int width = trackBarBrightness.Right - labelBrightness.Left;
			int center = panel1.Width / 2;
			labelBrightness.Left = center - width / 2;
			trackBarBrightness.Left = labelBrightness.Left + width - trackBarBrightness.Width;
		}

		private void SetupDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettingsToRegistry();

			if (this.DialogResult != DialogResult.None)
				return;

			if (m_dirty)
			{
				switch (MessageBox.Show("Keep changes?", "Vixen preview", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						Save();
						this.DialogResult = DialogResult.Yes;
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.No:
						this.DialogResult = DialogResult.No;
						break;
				}
			}
			else
				this.DialogResult = DialogResult.No;

		}

		private void Save()
		{
			// Set display properties
			_workshop.UI.LatticeSize = new Size(Convert.ToInt32(toolStripTextBoxResolutionX.Text), Convert.ToInt32(toolStripTextBoxResolutionY.Text));
			_workshop.UI.CellSize = Convert.ToInt32(m_cellSize);

			_workshop.UI.Background.Brightness = trackBarBrightness.Value;
			_workshop.UI.Background.Image = m_originalBackground;
			_workshop.UI.Background.Filename = m_backgroundImageFileName;

			Channel NewChannel = null;

			List<byte> ChannelPixelCoords = new List<byte>();
			foreach (int ChannelNumber in m_ChannelDictionary.Keys)
			{
				ChannelPixelCoords.Clear();
				NewChannel = new Channel(ChannelNumber + m_startChannel);
				foreach (uint xy in m_ChannelDictionary[ChannelNumber])
				{
					ChannelPixelCoords.AddRange(BitConverter.GetBytes(xy));
				}
				NewChannel.DecodeChannelBytes(Convert.ToBase64String(ChannelPixelCoords.ToArray()));
				_workshop.Channels.Add(NewChannel);
			}
			NewChannel = null;

			_workshop.UI.RespectChannelOutputsDuringPlayback = checkBoxRedirectOutputs.Checked;
		}

		private void toolStripDropDownButtonUpdate_Click(object sender, EventArgs e)
		{
			int width, height;

			m_cellSize = toolStripComboBoxPixelSize.SelectedIndex + 1;
			_workshop.UI.CellSize = m_cellSize;
			try
			{
				width = Int32.Parse(toolStripTextBoxResolutionX.Text) * (m_cellSize + 1);
			}
			catch
			{
				width = pictureBoxSetupGrid.Width;
			}
			try
			{
				height = Int32.Parse(toolStripTextBoxResolutionY.Text) * (m_cellSize + 1);
			}
			catch
			{
				height = pictureBoxSetupGrid.Height;
			}
			_workshop.UI.LatticeSize = new System.Drawing.Size(Int32.Parse(toolStripTextBoxResolutionX.Text), Int32.Parse(toolStripTextBoxResolutionY.Text));

			SetPictureBoxSize(width, height);
		}

		private void toolStripButtonResetSize_Click(object sender, EventArgs e)
		{
			if (m_originalBackground != null)
			{
				SetPictureBoxSize(m_originalBackground.Width, m_originalBackground.Height);
				UpdateSizeUI();
			}
		}

		private void allChannelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_ChannelDictionary.Clear();
			pictureBoxSetupGrid.Refresh();
			m_dirty = true;
		}

		private void selectedChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (toolStripComboBoxChannels.SelectedIndex != -1)
			//{
			//    m_ChannelDictionary.Remove(toolStripComboBoxChannels.SelectedIndex);
			//    _workshop.Channels[toolStripComboBoxChannels.SelectedIndex].ClearLattice();
			//    pictureBoxSetupGrid.Refresh();
			//    m_dirty = true;
			//}
		}

		private void toolStripButtonLoadImage_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				//FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				//byte[] fileBytes = new byte[fs.Length];
				//fs.Read(fileBytes, 0, (int)fs.Length);
				//fs.Close();
				//fs.Dispose();
				//MemoryStream stream = new MemoryStream(fileBytes);
				//Bitmap bmp = new Bitmap(stream);
				//stream.Close();
				//stream.Dispose();
				//SetPictureBoxSize(bmp.Width, bmp.Height);

				//SetBackground(bmp);
				//SetBrightness(0);

				//_workshop.UI.Background.Image = new Bitmap(bmp);
				//_workshop.UI.Background.Brightness = 10;

				//m_backgroundImageFileName = openFileDialog.FileName;
				//_workshop.UI.Background.Filename = m_backgroundImageFileName;
				m_dirty = true;
			}
		}

		private void toolStripButtonClearImage_Click(object sender, EventArgs e)
		{
			pictureBoxSetupGrid.BackgroundImage = null;
			SetBackground(null);
			m_backgroundImageFileName = string.Empty;
			_workshop.UI.Background.Clear(true);
			m_dirty = true;
		}

		private void toolStripButtonSaveImage_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				Bitmap image = new Bitmap(m_originalBackground);
				image.Save(Path.ChangeExtension(saveFileDialog.FileName, ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
				MessageBox.Show("File saved.", "Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void toolStripComboBoxChannels_SelectedIndexChanged(object sender, EventArgs e)
		{
			//_workshop.ActiveChannelIndex = toolStripComboBoxChannels.SelectedIndex;
			//if (_workshop.ActiveChannelIndex < 0)
			//    _workshop.ActiveChannelIndex = 0;
			//pictureBoxSetupGrid.Refresh();
		}

		private void SetupDialog_KeyDown(object sender, KeyEventArgs e)
		{
			m_controlDown = e.Control;
		}

		private void SetupDialog_KeyUp(object sender, KeyEventArgs e)
		{
			m_controlDown = e.Control;
		}

		//private void SetBrightness(float value)
		//{
		//    // Value is between -1.0 and 1.0
		//    if (m_originalBackground != null)
		//    {
		//        Bitmap img = new Bitmap(m_originalBackground);
		//        ColorMatrix cMatrix = new ColorMatrix(new float[][] {
		//                new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
		//                new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
		//                new float[] { value, value, value, 0.0f, 1.0f }
		//            });

		//        Graphics gr = Graphics.FromImage(img);
		//        ImageAttributes attrs = new ImageAttributes();
		//        attrs.SetColorMatrix(cMatrix);
		//        gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
		//            0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attrs);
		//        gr.Dispose();
		//        attrs.Dispose();
		//        pictureBoxSetupGrid.BackgroundImage = img;

		//        _workshop.UI.BackgroundImage_Brightness = value;
		//    }
		//}

		private void trackBarBrightness_ValueChanged(object sender, EventArgs e)
		{
			//SetBrightness((trackBarBrightness.Value - 10) / 10.0f);
			_workshop.UI.Background.Brightness = trackBarBrightness.Value;
		}

		private void SetBackground(Bitmap bitmap)
		{
			if (m_originalBackground != null)
			{
				m_originalBackground.Dispose();
			}
			if (bitmap != null)
				m_originalBackground = new Bitmap(bitmap);
			else
				m_originalBackground = null;

			labelBrightness.Visible = trackBarBrightness.Visible = toolStripButtonSaveImage.Enabled = bitmap != null;
			toolStripTextBoxResolutionX.Enabled = toolStripTextBoxResolutionY.Enabled = bitmap == null;
			if (bitmap != null)
			{
				trackBarBrightness.Value = 10;
			}
		}

		private void SetupDialog_Resize(object sender, EventArgs e)
		{
			if (!m_resizing)
			{
				UpdatePosition();
			}
		}

		private void SetupDialog_ResizeBegin(object sender, EventArgs e)
		{
			m_resizing = true;
		}

		private void SetupDialog_ResizeEnd(object sender, EventArgs e)
		{
			m_resizing = false;
			UpdatePosition();
		}

		private void cmdNewUI_Click(object sender, EventArgs e)
		{
			//_workshop.UI.UseOriginalUI = false;
			//this.DialogResult = DialogResult.Retry;
			//this.Hide();
		}

		public void UpdateUIFromData()
		{
			toolStripTextBoxResolutionX.Text = _workshop.UI.LatticeSize.Width.ToString();
			toolStripTextBoxResolutionY.Text = _workshop.UI.LatticeSize.Height.ToString();
			toolStripComboBoxPixelSize.SelectedIndex = _workshop.UI.CellSize - 1;
			toolStripDropDownButtonUpdate_Click(null, null);
		}

	}
}