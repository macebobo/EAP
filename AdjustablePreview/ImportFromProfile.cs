using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore
{
	public partial class ImportFromProfile : Form
	{
		#region [ Private Variables ]

		private bool _loading = false;
		private Workshop _workshop = null;
		private int _focusSelectedChannel = -1;
		private string _profileFileName = string.Empty;
		private List<ImportData> _importedChannels = new List<ImportData>();
		private XmlHelper _xmlHelper = new XmlHelper();
		private List<Mapped> _mapping = new List<Mapped>();
		private Size _importedSize = Size.Empty;
		private Size _newLatticeSize = Size.Empty;
		private Color _selectedColor;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public bool ClearChannelsBeforeImport
		{
			get { return chkClearClannel.Checked; }
		}

		public Workshop Workshop
		{
			set
			{
				_workshop = value;
				if (_workshop == null)
					return;

				lblCurrentSize.Text = string.Format("{0} x {1}", UISettings.ʃLatticeSize.Width, UISettings.ʃLatticeSize.Height);

				if (!_importedSize.IsEmpty)
					CheckForNewLatticeSize();
				lblCurrentProfile.Text = _workshop.Profile_Name;
				LoadChannels();
			}
		}

		private string ImportChannelBytes
		{
			get { return _importedChannels[lstImportedChannels.SelectedIndex].EncodedBytes; }
		}

		private int ImportedChannelNum
		{
			get { return _importedChannels[lstImportedChannels.SelectedIndex].ID; }
		}

		public List<Mapped> MappedChannels
		{
			get { return _mapping; }
		}

		public Size NewLatticeSize
		{
			get { return _newLatticeSize; }
		}

		private int ID
		{
			get 
			{
				if (_workshop == null)
					return -1;
				return _workshop.Channels.Sorted[ChannelList.SelectedIndex].ID; 
			}
		}

		public string ProfileFileName
		{
			get { return _profileFileName; }
			set
			{
				_profileFileName = value;
				LoadProfileChannels();
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ImportFromProfile()
		{
			InitializeComponent();
			_selectedColor = ColorMixer(Color.Gold, SystemColors.Highlight);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void CheckForNewLatticeSize()
		{
			string Message = string.Empty;

			if ((_importedSize.Width > UISettings.ʃLatticeSize.Width) && (_importedSize.Height <= UISettings.ʃLatticeSize.Height))
			{
				Message = "Width of the importing profile is wider than the current profile." + Environment.NewLine +
						  "Expand the width of the current profile to accomodate extra data? " + Environment.NewLine +
						  "If No, then points off the edge of the canvas will be cut off";
			}
			else if ((_importedSize.Width <= UISettings.ʃLatticeSize.Width) && (_importedSize.Height > UISettings.ʃLatticeSize.Height))
			{
				Message = "Height of the importing profile is taller than the current profile." + Environment.NewLine +
						  "Expand the height of the current profile to accomodate extra data? " + Environment.NewLine +
						  "If No, then points off the edge of the canvas will be cut off";
			}
			else if ((_importedSize.Width > UISettings.ʃLatticeSize.Width) && (_importedSize.Height > UISettings.ʃLatticeSize.Height))
			{
				Message = "Size of the importing profile is larger than the current profile." + Environment.NewLine +
						  "Expand the size of the current profile to accomodate extra data? " + Environment.NewLine +
						  "If No, then points off the edge of the canvas will be cut off";
			}

			if (Message.Length > 0)
			{
				if (MessageBox.Show(Message, "Import Channels", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					_newLatticeSize = new Size(Math.Max(_importedSize.Width, UISettings.ʃLatticeSize.Width), Math.Max(_importedSize.Height, UISettings.ʃLatticeSize.Height));
				}
			}
		}

		private Color ColorMixer(Color c1, Color c2)
		{
			int Red = (c1.R + c2.R) / 2;
			int Green = (c1.G + c2.G) / 2;
			int Blue = (c1.B + c2.B) / 2;

			return Color.FromArgb((byte)Red, (byte)Green, (byte)Blue);
		}

		private bool IsMapped()
		{
			return IsMapped(this.ImportedChannelNum, this.ID); 
		}

		private bool IsMapped(int importChannelNum, int ourChannelNum)
		{
			if ((importChannelNum == -1) && (ourChannelNum == -1))
				return false;

			foreach (Mapped m in _mapping)
			{
				if (((m.ImportChannelNum == importChannelNum) || (importChannelNum == -1)) && ((m.ID == ourChannelNum) || (ourChannelNum == -1)))
					return true;
			}
			return false;
		}

		public void LoadChannels()
		{
			//foreach (Channel Channel in _data.Channels)
			//    ChannelList.Items.Add(Channel.ToString(true));

			//if (ChannelList.Items.Count > 0)
			//{
			//    _loading = true;
			//    _data.SelectedChannels.Clear();
			//    _data.SelectedChannels.Add(_data.ActiveChannel);
			//    ChannelList.SelectedIndex = _data.ActiveChannelIndex;
			//    _loading = false;
			//}
			//ChannelList.SelectedIndex = 0;
		}

		private void LoadProfileChannels()
		{
			FileInfo fi = new FileInfo(_profileFileName);
			XmlNode PluginData = null;
			XmlNode DataNode = null;
			XmlDocument Profile = new XmlDocument();
			ImportData Import = null;
			Profile.Load(_profileFileName);
			int Start = 0;
			int End = 0;

			lblProfileName.Text = fi.Name;

			PluginData = Profile.SelectSingleNode("Profile/PlugInData/PlugIn[@name='Adjustable preview']");

			if (PluginData == null)
				throw new Exception("Invalid Profile file");

			Start = Convert.ToInt32(_xmlHelper.GetTheAttribute(PluginData, "from"));
			End = Convert.ToInt32(_xmlHelper.GetTheAttribute(PluginData, "to"));

			_importedSize = new Size(_xmlHelper.GetNodeIntValue(PluginData, "Display/Width"), _xmlHelper.GetNodeIntValue(PluginData, "Display/Height"));

			lblSize.Text = string.Format("{0} x {1}", _importedSize.Width, _importedSize.Height);

			if (_workshop != null)
				CheckForNewLatticeSize();

			foreach (XmlNode Node in Profile.SelectNodes("//Profile/ChannelObjects/Channel"))
			{
				Import = new ImportData();
				Import.Name = Node.InnerText;
				Import.ID = Convert.ToInt32(_xmlHelper.GetTheAttribute(Node, "output"));
				if ((Import.ID < Start) || (Import.ID > End))
					continue;

				DataNode = PluginData.SelectSingleNode("Channels/Channel[@number='" + Import.ID + "']");
				if (DataNode != null)
					Import.EncodedBytes = DataNode.InnerText;
				_importedChannels.Add(Import);

				lstImportedChannels.Items.Add(Import.ToString(true));
			}
			lstImportedChannels.SelectedIndex = 0;
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void cmdMap_Click(object sender, EventArgs e)
		{
			Mapped Map = new Mapped(this.ImportedChannelNum, ID, ImportChannelBytes);

			// Check to see if another imported Channel is mapped to the selected Channel
			foreach (Mapped m in _mapping)
			{
				if (m.ID == Map.ID)
				{
					if (MessageBox.Show("A different Channel is already mapped to \"" + _workshop.Channels.GetChannelByID(Map.ID).ToString(true) + "\"." + Environment.NewLine +
										"Map this one as well and overlay the data?", "Map Channel", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
						return;
				}
			}

			_mapping.Add(Map);
			ChannelList.Refresh();
			lstImportedChannels.Refresh();

			cmdMap.Enabled = false;
			cmdUnmap.Enabled = true;
			Map = null;
		}

		private void cmdMapAll_Click(object sender, EventArgs e)
		{
			_mapping.Clear();
			//int ID = -1;

			foreach (ImportData Data in _importedChannels)
			{
				_mapping.Add(new Mapped(Data.ID, Data.ID, _workshop.Channels.GetChannelByID(Data.ID).EncodeChannelBytes()));
				//ID = Data.ID;

				//foreach (Channel Channel in _data.Channels)
				//{
					//if (Channel.ID == ID)
					//{
						//_mapping.Add(new Mapped(ID, ID, Channel.EncodeChannelBytes()));
						//break;
					//}
				//}
			}
			ChannelList.Refresh();
			lstImportedChannels.Refresh();
		}

		private void cmdUnmap_Click(object sender, EventArgs e)
		{
			int ImportChannelNum = this.ImportedChannelNum;
			int OurChannelNum = this.ID;

			for (int i = 0; i < _mapping.Count; i++)
			{
				if ((_mapping[i].ImportChannelNum == ImportChannelNum) && (_mapping[i].ID == OurChannelNum))
				{
					_mapping.RemoveAt(i);
					break;
				}
			}
			ChannelList.Refresh();
			lstImportedChannels.Refresh();

			cmdMap.Enabled = true;
			cmdUnmap.Enabled = false;
		}

		/// <summary>
		/// Draws the custom list item for each Channel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstChannels_DrawItem(object sender, DrawItemEventArgs e)
		{
			Rectangle r1 = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
			Rectangle Bounds;
			StringFormat sf = new StringFormat();
			Channel Channel = null;
			int X = e.Bounds.X;
			int Y = e.Bounds.Y;
			int BottomY = Y + e.Bounds.Height;
			int MidY = Y + e.Bounds.Height / 2;
			string Text = string.Empty;
			int ChannelNum = -1;

			Channel = _workshop.Channels.Sorted[e.Index];
			ChannelNum = Channel.ID;
			bool Mapped = IsMapped(-1, ChannelNum);

			if (((e.State & DrawItemState.Selected) == DrawItemState.Selected) && !Mapped)
			{
				if (((e.State & DrawItemState.Focus) == DrawItemState.Focus) || ((e.Index == _focusSelectedChannel) && !ChannelList.Focused))
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
				else
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.ActiveCaption), e.Bounds);
			}
			else if (((e.State & DrawItemState.Selected) == DrawItemState.Selected) && Mapped)
			{
				e.Graphics.FillRectangle(new SolidBrush(_selectedColor), e.Bounds);
			}
			else if (Mapped)
			{
				Debug.WriteLine(ChannelNum.ToString());
				e.Graphics.FillRectangle(Brushes.Gold, e.Bounds);
			}
			else
				e.DrawBackground();

			Text = Channel.ToString(true);

			// Draw a swatch of color for the Channel, with a black rectangle;
			e.Graphics.FillRectangle(Channel.VixenChannel.Brush, new Rectangle(X + 2, Y + 2, 12, 12));
			e.Graphics.DrawRectangle(Pens.Black, new Rectangle(X + 2, Y + 2, 12, 12));
			X += 16;

			Bounds = new Rectangle(X, Y, e.Bounds.Width - (X - e.Bounds.X), e.Bounds.Height);

			e.Graphics.DrawString(Text, ChannelList.Font, new SolidBrush(SystemColors.WindowText), Bounds, sf);

			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				_focusSelectedChannel = e.Index;
				e.DrawFocusRectangle();
			}
		}

		private void lstImportedChannels_DrawItem(object sender, DrawItemEventArgs e)
		{
			Bitmap Visible = ElfRes.visible;
			Rectangle r1 = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
			Rectangle Bounds;
			StringFormat sf = new StringFormat();
			ImportData Imported = null;
			int X = e.Bounds.X;
			int Y = e.Bounds.Y;
			int BottomY = Y + e.Bounds.Height;
			int MidY = Y + e.Bounds.Height / 2;
			string Text = string.Empty;

			Imported = _importedChannels[e.Index];
			int ID = Imported.ID;
			bool Mapped = IsMapped(ID, -1);

			if (((e.State & DrawItemState.Selected) == DrawItemState.Selected) && !Mapped)
			{
				if (((e.State & DrawItemState.Focus) == DrawItemState.Focus) || ((e.Index == _focusSelectedChannel) && !lstImportedChannels.Focused))
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
				else
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.ActiveCaption), e.Bounds);
			}
			else if (((e.State & DrawItemState.Selected) == DrawItemState.Selected) && Mapped)
			{
				e.Graphics.FillRectangle(new SolidBrush(_selectedColor), e.Bounds);
			}
			else if (Mapped)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.Gold), e.Bounds);
			}
			else
				e.DrawBackground();

			Text = Imported.ToString(true);

			Bounds = new Rectangle(X, Y, e.Bounds.Width - (X - e.Bounds.X), e.Bounds.Height);

			e.Graphics.DrawString(Text, lstImportedChannels.Font, new SolidBrush(SystemColors.WindowText), Bounds, sf);

			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				_focusSelectedChannel = e.Index;
				e.DrawFocusRectangle();
			}
		}

		private void lstImportedChannels_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			if (IsMapped())
			{
				cmdUnmap.Enabled = true;
				cmdMap.Enabled = false;
			}
			else
			{
				cmdUnmap.Enabled = false;
				cmdMap.Enabled = true;
			}
		}

		private void lstChannels_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			if (IsMapped())
			{
				cmdUnmap.Enabled = true;
				cmdMap.Enabled = false;
			}
			else
			{
				cmdUnmap.Enabled = false;
				cmdMap.Enabled = true;
			}
		}

		#endregion [ Events ]
	}

	public class Mapped
	{
		public int ImportChannelNum = -1;
		public int ID = -1;
		public string EncodedBytes = string.Empty;

		public Mapped()
		{ }

		public Mapped(int importChannelNum, int ourChannelNum, string encodedBytes) : this()
		{
			this.ImportChannelNum = importChannelNum;
			this.ID = ourChannelNum;
			this.EncodedBytes = encodedBytes;
		}
	}

	public class ImportData
	{
		public string EncodedBytes = string.Empty;
		public string Name = string.Empty;
		public int ID = 0;

		public ImportData()
		{ }

		public override string ToString()
		{
			return Name;
		}

		public string ToString(bool includeIndex)
		{
			if (includeIndex)
				return string.Format("{0}: {1}", this.ID + 1, this.ToString());
			else
				return this.ToString();
		}
	}
}
