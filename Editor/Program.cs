using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Editor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new TestPlugin());
			RunPlugIn();
		}

		static void RunPlugIn()
		{

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

			PlugIn.Shutdown();
		}
	}

}
