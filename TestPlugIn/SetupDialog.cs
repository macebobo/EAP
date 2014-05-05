using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Vixen;
using System.Xml;
using System.IO;

namespace TestPlugIn
{
	public partial class SetupDialog : ElfCore.Forms.Editor
	{
		public SetupDialog()
		{
			InitializeComponent();
		}

		public SetupDialog(SetupData setupData, XmlNode setupNode, List<Channel> channels, int startChannel)
		{
			InitializeComponent();
		}
	}
}
