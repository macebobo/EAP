﻿using ElfCore.Channels;
using ElfCore.Util;
using System.ComponentModel;
using System.Xml;

namespace ElfCore.Profiles.Vixen
{
	/// <summary>
	/// Handles all the profile specific data and properties.
	/// </summary>
	[ElfProfile(ProfileType.VixenPlus)]
	public class VixenPlus : BaseVixen, INotifyPropertyChanged //, IProfile
	{

		#region [ Properties ]

		/// <summary>
		/// ID of the ProfileType
		/// </summary>
		public override ProfileType ProfileTypeID
		{
			get { return ProfileType.VixenPlus; }
			set { base.ProfileTypeID = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.
		/// </summary>        
		public VixenPlus()
			: base()
		{ }

		/// <summary>
		/// Constructor with a profile's filename to open
		/// </summary>
		/// <param name="filename">Name of the Profile file.</param>
		public VixenPlus(string filename)
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
			VixenPlus_GeneralPlugIn PlugIn = new VixenPlus_GeneralPlugIn()
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
			Channel.Name = node.InnerText;
			return Channel;
		}

		/// <summary>
		/// Saves the Channel Name.
		/// </summary>
		protected override void SaveChannelName(XmlNode node, BaseChannel channel)
		{
			node.InnerText = channel.Name;
		}

		#endregion [ Private Methods ]
	}

	public class VixenPlus_GeneralPlugIn : GeneralPlugIn
	{
		public VixenPlus_GeneralPlugIn()
			: base()
		{ }

		public VixenPlus_GeneralPlugIn(XmlNode plugInNode)
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
    <Channel color="-1" output="0" id="4618963281901076567" enabled="True">Channel 1</Channel>
    <Channel color="-1" output="1" id="5071790647660683975" enabled="True">Channel 2</Channel>
    <Channel color="-1" output="2" id="5532585377925515175" enabled="True">Channel 3</Channel>
    <Channel color="-1" output="3" id="5277201664490490364" enabled="True">Channel 4</Channel>
    <Channel color="-1" output="4" id="5085533573342859031" enabled="True">Channel 5</Channel>
    <Channel color="-1" output="5" id="5536428817357548196" enabled="True">Channel 6</Channel>
    <Channel color="-1" output="6" id="5644486036674465413" enabled="True">Channel 7</Channel>
    <Channel color="-1" output="7" id="5685171898020576150" enabled="True">Channel 8</Channel>
    <Channel color="-1" output="8" id="5490068301014211132" enabled="True">Channel 9</Channel>
    <Channel color="-1" output="9" id="4764363232824992544" enabled="True">Channel 10</Channel>
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
        <Channel number="0">DQAPAA0AEAANABEADAASAAwAFAALABYACwAYAAoAGQAKABoACgAbAAsAGwALABwADAAcAA0AHAAOABsADwAaABAAGgARABoAEgAbABIAHAASAB8AEgAhABIAJAASACUAEQAnABEAKAAQACkA</Channel>
      </Channels>
    </PlugIn>
  </PlugInData>
  <SortOrders lastSort="-1" />
  <DisabledChannels>
  </DisabledChannels>
</Profile>

<?xml version="1.0" encoding="utf-8"?>
<Groups>
  <Group Name="group 1" Zoom="100%" Color="-65536">
    <Channels>0,1,2,3</Channels>
  </Group>
</Groups> 
 */