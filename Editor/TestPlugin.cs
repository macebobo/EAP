using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Editor
{
	/// <summary>
	/// Build location for work: C:\Program Files (x86)\Vixen\Plugins\Output
	/// Build location for home: C:\Vixen 2.1.1.0\Plugins\Output
	/// </summary>
	public partial class TestPlugin : Form
	{
		public TestPlugin()
		{
			InitializeComponent();

			//string Filename = @"C:\Vixen 2.1.1.0\Profiles\Sequencing.pro";
			string Filename = @"C:\Program Files (x86)\Vixen 2.1.1.0\Profiles\Sequencing.pro";
			XmlDocument Doc = new XmlDocument();
			Doc.Load(Filename);

			XmlNode SetupNode = Doc.SelectSingleNode("//PlugInData/PlugIn[@name='Adjustable preview']");

			List<Vixen.Channel> Channels = new List<Vixen.Channel>();

			foreach (XmlNode channelNode in Doc.SelectNodes("//ChannelObjects/Channel"))
			{
				if (channelNode is XmlWhitespace)
					continue;
				Channels.Add(new Vixen.Channel(channelNode));
			}
			
			Vixen.SetupData SetupData = new Vixen.SetupData();
			FakeVixen Fake = new FakeVixen();
			Fake.Channels = Channels;

			ElfPreview.PlugIn PlugIn = new ElfPreview.PlugIn();
			PlugIn.Initialize(Fake, SetupData, SetupNode);
			PlugIn.Setup();

			//Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string Filename = @"C:\Program Files (x86)\Vixen 2.1.1.0\Profiles\Sequencing.pro";
			XmlDocument Doc = new XmlDocument();
			Doc.Load(Filename);

			XmlNode SetupNode = Doc.SelectSingleNode("//PlugInData/PlugIn[@name='Adjustable preview']");

			List<Vixen.Channel> Channels = new List<Vixen.Channel>();

			foreach (XmlNode channelNode in Doc.SelectNodes("//ChannelObjects/Channel"))
			{
				if (channelNode is XmlWhitespace)
					continue;
				Channels.Add(new Vixen.Channel(channelNode));
			}

			Vixen.SetupData SetupData = new Vixen.SetupData();
			FakeVixen Fake = new FakeVixen();
			Fake.Channels = Channels;

			ElfPreview.PlugIn PlugIn = new ElfPreview.PlugIn();
			PlugIn.Initialize(Fake, SetupData, SetupNode);
			PlugIn.Setup();

			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//Editor frmEditor = new Editor();
			//frmEditor.Show();
		}
	}
}
