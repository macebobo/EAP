using ElfCore.Channels;
using ElfCore.Util;
using System.ComponentModel;
using System.Xml;

namespace ElfCore.Profiles.Vixen
{
	/// <summary>
	/// Handles all the profile specific data and properties.
	/// </summary>
	[ElfProfile(ProfileType.Vixen25x)]
	public class Vixen25x : BaseVixen, INotifyPropertyChanged //, IProfile
	{

		#region [ Properties ]

		/// <summary>
		/// ID of the ProfileType
		/// </summary>
		public override ProfileType ProfileTypeID
		{
			get { return ProfileType.Vixen25x; }
			set { base.ProfileTypeID = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.
		/// </summary>        
		public Vixen25x()
			: base()
		{ }

		/// <summary>
		/// Constructor with a profile's filename to open
		/// </summary>
		/// <param name="filename">Name of the Profile file.</param>
		public Vixen25x(string filename)
			: base(filename)
		{ }

		#endregion [ Constructors ]

		#region [ Private Methods ]

		protected override void AddGeneralPlugIn(XmlNode node)
		{
			_loadedPlugIns.Add(new Vixen25x_GeneralPlugIn(node));
		}

		/// <summary>
		/// Create the PlugIn object for this.
		/// </summary>
		protected override XmlNode CreatePlugInNode(XmlDocument doc)
		{
			Vixen25x_GeneralPlugIn PlugIn = new Vixen25x_GeneralPlugIn()
			{
				Name = EP_NAME,
				ID = 0,
				Key = -165848267,
				Enabled = true,
				FromChannel = this.Channels.MinID,
				ToChannel = this.Channels.MaxID
			};
			return PlugIn.CreateNode(doc);
		}

		/// <summary>
		/// Creates a new Channel object, loading from the topmost Channel node in the Profile xml.
		/// </summary>
		/// <param name="node">Xml node containing initial Channel data.</param>
		/// <returns>Channel object.</returns>
		protected override BaseChannel LoadChannelFromXml(XmlNode node)
		{
			BaseChannel Channel = base.LoadChannelFromXml(node);
			Channel.Name = _xmlHelper.GetAttributeValue(node, Attribute_Name, "Channel");
			return Channel;
		}

		/// <summary>
		/// Saves the Channel Name.
		/// </summary>
		protected override void SaveChannelName(XmlNode node, BaseChannel channel)
		{
			_xmlHelper.AddAttribute(node, Attribute_Name, channel.Name);
		}

		#endregion [ Private Methods ]
	}

	public class Vixen25x_GeneralPlugIn : GeneralPlugIn
	{
		public Vixen25x_GeneralPlugIn()
			: base()
		{ }

		public Vixen25x_GeneralPlugIn(XmlNode plugInNode)
			: base(plugInNode)
		{ }

		public override XmlNode CreateNode(XmlDocument doc)
		{
			XmlNode Node = base.CreateNode(doc);
			XmlHelper.Instance.AddAttribute(Node, "type", "Output");
			return Node;
		}

	}
}
/*
<?xml version="1.0" encoding="utf-8"?>
<Profile>
  <ChannelObjects>
    <Channel name="Channel 1" color="-1" output="0" id="4975673233446305637" enabled="True" />
    <Channel name="Channel 2" color="-1" output="1" id="5110935551168474821" enabled="True" />
    <Channel name="Channel 3" color="-1" output="2" id="4633543325811758683" enabled="True" />
    <Channel name="Channel 4" color="-1" output="3" id="5708266768659145277" enabled="True" />
    <Channel name="Channel 5" color="-1" output="4" id="5706369564757840435" enabled="True" />
    <Channel name="Channel 6" color="-1" output="5" id="4917308204787022262" enabled="True" />
    <Channel name="Channel 7" color="-1" output="6" id="4686729233131397808" enabled="True" />
    <Channel name="Channel 8" color="-1" output="7" id="4755041948377970777" enabled="True" />
    <Channel name="Channel 9" color="-1" output="8" id="4993418939113526297" enabled="True" />
    <Channel name="Channel 10" color="-1" output="9" id="5588731558945428864" enabled="True" />
  </ChannelObjects>
  <Outputs>0,1,2,3,4,5,6,7,8,9</Outputs>
  <PlugInData>
    <PlugIn name="Adjustable preview" key="-399317235" id="0" enabled="True" type="Output" from="1" to="10">
      <BackgroundImage>
      </BackgroundImage>
      <RedirectOutputs>False</RedirectOutputs>
      <Display>
        <Height>32</Height>
        <Width>64</Width>
        <PixelSize>8</PixelSize>
        <Brightness>10</Brightness>
      </Display>
      <Channels>
        <Channel number="2">DQAbAA0AGgAOABoADgAZAA4AFwAPABYADwAUAA8AEgAPABEADwAQAA8ADwAPAA4ADQAUAAoAGAAHAB0ABQAhAAMAJQACACYAAgApAAIAKwACAC0AAgAvAAMAMgAEADMABQA1AAgANgAKADcADAA3AA4ANwAPADcAEQA3ABIANgATADYAEwA1ABMAMwAUADEAFAAvABQALQAUACsAFAAqABQAKQAUACgAFAAnAA==</Channel>
      </Channels>
    </PlugIn>
  </PlugInData>
  <SortOrders lastSort="1">
    <SortOrder name="Created Sort Order">0,2,7,4,3,5,1,6,9,8</SortOrder>
  </SortOrders>
  <DisabledChannels>
  </DisabledChannels>
</Profile>
*/