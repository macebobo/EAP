using ElfCore.Channels;
using ElfCore.Util;
using System.ComponentModel;
using System.Xml;

namespace ElfCore.Profiles.Vixen
{
	/// <summary>
	/// Handles all the profile specific data and properties.
	/// </summary>
	[ElfProfile(ProfileType.Vixen21x)]
	public class Vixen21x : BaseVixen, INotifyPropertyChanged //, IProfile
	{
		#region [ Properties ]

		/// <summary>
		/// ID of the ProfileType
		/// </summary>
		public override ProfileType ProfileTypeID
		{
			get { return ProfileType.Vixen21x; }
			set { base.ProfileTypeID = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.
		/// </summary>        
		public Vixen21x()
			: base()
		{ }

		/// <summary>
		/// Constructor with a profile's filename to open
		/// </summary>
		/// <param name="filename">Name of the Profile file.</param>
		public Vixen21x(string filename)
			: base(filename)
		{ }

		#endregion [ Constructors ]

		#region [ Protected Methods ]

		protected override void AddGeneralPlugIn(XmlNode node)
		{
			_loadedPlugIns.Add(new Vixen21x_GeneralPlugIn(node));
		}

		/// <summary>
		/// Create the PlugIn object for this.
		/// </summary>
		protected override XmlNode CreatePlugInNode(XmlDocument doc)
		{
			Vixen21x_GeneralPlugIn PlugIn = new Vixen21x_GeneralPlugIn()
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

		#endregion [ Protected Methods ]
	}

	#region [ Vixen21x_GeneralPlugIn ] 

	public class Vixen21x_GeneralPlugIn : GeneralPlugIn
	{
		public Vixen21x_GeneralPlugIn()
			: base()
		{ }

		public Vixen21x_GeneralPlugIn(XmlNode plugInNode)
			: base(plugInNode)
		{ }

	}

	#endregion [ Vixen21x_GeneralPlugIn ] 
}
/*
<?xml version="1.0"?>
<Profile>
  <ChannelObjects>
    <Channel color="-1" output="0" id="635074269877959962" enabled="True">Channel 1</Channel>
    <Channel color="-1" output="1" id="635074269877979972" enabled="True">Channel 2</Channel>
    <Channel color="-1" output="2" id="635074269877979972" enabled="True">Channel 3</Channel>
    <Channel color="-1" output="3" id="635074269877979972" enabled="True">Channel 4</Channel>
    <Channel color="-1" output="4" id="635074269877979972" enabled="True">Channel 5</Channel>
    <Channel color="-1" output="5" id="635074269877979972" enabled="True">Channel 6</Channel>
    <Channel color="-1" output="6" id="635074269877979972" enabled="True">Channel 7</Channel>
    <Channel color="-1" output="7" id="635074269877979972" enabled="True">Channel 8</Channel>
    <Channel color="-1" output="8" id="635074269877979972" enabled="True">Channel 9</Channel>
    <Channel color="-1" output="9" id="635074269877979972" enabled="True">Channel 10</Channel>
  </ChannelObjects>
  <Outputs>0,1,2,3,4,5,6,7,8,9</Outputs>
  <PlugInData>
    <PlugIn name="Adjustable preview" key="-399317235" id="0" enabled="True" from="1" to="10">
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
        <Channel number="2">CgAUAAoAFQAKABcACgAZAAoAGgAKAB0ACgAfAAsAIAAMACEADQAhAA4AIAAOAB8ADwAdABAAGQAQABYAEQAUABEAEgARABEAEgARABMAEgATABMAEwAVABQAGAAVABwAFgAfABYAIgAWACMA</Channel>
      </Channels>
    </PlugIn>
  </PlugInData>
  <SortOrders lastSort="1">
    <SortOrder name="Created Sort Order">0,8,1,3,6,2,4,5,7,9</SortOrder>
  </SortOrders>
  <DisabledChannels>
  </DisabledChannels>
</Profile>
*/ 
