using System.Windows.Forms;

using ElfCore.Channels;

namespace ElfCore.Forms
{
	public partial class ChannelProperties : Form
	{
		private RawChannel _channelProperties;
		//private Channels.RasterChannel _channel;

		public ChannelProperties()
		{
			InitializeComponent();
		}

		public RawChannel Properties
		{
			get { return _channelProperties; }
			set
			{
				_channelProperties = value;
				propertyGrid1.SelectedObject = _channelProperties;
			}
		}

		//public Channels.RasterChannel Channel
		//{
		//    get { return _channel; }
		//    set
		//    {
		//        _channel = value;
		//        this.propertyGrid1.SelectedObject = _channel;
		//    }
		//}
		
	}
}
