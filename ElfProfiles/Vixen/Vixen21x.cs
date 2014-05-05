using System.Drawing;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Interfaces;
using ElfCore.Util;

using ElfRes = ElfProfiles.Properties.Resources;

namespace ElfProfiles.Vixen {
    /// <summary>
    ///     Handles all the profile specific data and properties.
    /// </summary>
    [ElfProfile]
    public class Vixen21x : BaseVixen, IProfile {
        #region [ Properties ]

        public override Bitmap IconImage {
            get { return ElfRes.vixen2x; }
        }

        public override string FormatName {
            get { return "Vixen 2.1.x Profile"; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        /// <summary>
        ///     The default Constructor.
        /// </summary>
        public Vixen21x() {
            ID = 1;
            DefaultSavePath = "";
        }


        ///// <summary>
        ///// Constructor with a profile's filename to open
        ///// </summary>
        ///// <param name="filename">Name of the Profile file.</param>
        //public Vixen21x(string filename)
        //	: base(filename)
        //{ }

        #endregion [ Constructors ]

        #region [ Protected Methods ]

        /// <summary>
        ///     Adds a "general" output plugin node to this list _loadedPlugIns. This is typically some other output plugin found
        ///     in the Profile beside the one
        ///     that contains our data.
        /// </summary>
        protected override void AddGeneralPlugIn(XmlNode node) {
            _loadedPlugIns.Add(new Vixen21x_GeneralPlugIn(node));
        }


        /// <summary>
        ///     Create the PlugIn node for either ElfPreview or AdjustablePreview data.
        /// </summary>
        /// <param name="doc">XmlDocument that owns this node</param>
        /// <param name="elfPluginNode">Indicates whether this is an Elf Plugin Node.</param>
        /// <returns>XmlNode formatted with PlugIn data.</returns>
        protected override XmlNode CreatePlugInNode(XmlDocument doc, bool elfPluginNode) {
            var PlugIn = new Vixen21x_GeneralPlugIn {
                Name = elfPluginNode ? EP_NAME : AP_NAME, ID = 0, Key = elfPluginNode ? -165848267 : -399317235, Enabled = true, FromChannel = MinID,
                ToChannel = MaxID
            };
            return PlugIn.CreateNode(doc);
        }


        /// <summary>
        ///     Creates a new Channel object, loading from the topmost Channel node in the Profile xml.
        /// </summary>
        /// <param name="node">Xml node containing initial Channel data.</param>
        /// <returns>Channel object.</returns>
        protected override RawChannel LoadChannelFromXml(XmlNode node) {
            RawChannel Channel = base.LoadChannelFromXml(node);
            Channel.Name = node.InnerText;
            return Channel;
        }


        /// <summary>
        ///     Saves the Channel Name.
        /// </summary>
        protected override void SaveChannelName(XmlNode node, RawChannel channel) {
            node.InnerText = channel.Name;
        }

        #endregion [ Protected Methods ]

        #region [ Public Methods ]

        /// <summary>
        ///     Determines whether the file indicated is a valid file format for this type of Profile.
        /// </summary>
        /// <param name="filename">Filename of file containing profile data.</param>
        /// <returns>Returns true if this type of Profile can open this file exactly, false otherwise.</returns>
        public override bool ValidateFile(string filename) {
            if (!base.ValidateFile(filename)) {
                return false;
            }

            if (string.Compare(_detectedTypeName, FormatName, true) == 0) {
                return true;
            }

            // 2.1.x -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
            //			PlugInData/Plugin		-> No Output attribute
            if (!_hasChannelNameInAttribute && !_hasOutputAttribute) {
                return true;
            }

            return false;
        }

        #endregion [ Public Methods ]
    }

    #region [ Vixen21x_GeneralPlugIn ]

    public class Vixen21x_GeneralPlugIn : GeneralPlugIn {
        public Vixen21x_GeneralPlugIn() {}

        public Vixen21x_GeneralPlugIn(XmlNode plugInNode) : base(plugInNode) {}
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