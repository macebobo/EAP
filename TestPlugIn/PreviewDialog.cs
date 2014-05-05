using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Imaging;
using Vixen;

namespace TestPlugIn
{
	public partial class PreviewDialog : Form
	{
		public PreviewDialog()
		{
			InitializeComponent();
		}

		public PreviewDialog(XmlNode setupNode, List<Channel> channels, int startChannel)
		{
			InitializeComponent();
		}

		public void UpdateWith(byte[] channelValues)
		{
		}

	}
}
