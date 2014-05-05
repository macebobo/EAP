using System.Drawing;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Util;

using ElfProfiles.Annotations;

using ElfRes = ElfProfiles.Properties.Resources;

namespace ElfProfiles.Vixen {
    /// <summary>
    ///     Handles all the profile specific data and properties.
    /// </summary>
    [ElfProfile]
    public class VixenPlus : BaseVixen {
        #region [ Properties ]

        public override Bitmap IconImage {
            get { return ElfRes.vixenplus; }
        }

        public override string FormatName {
            get { return "Vixen Plus Profile"; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        /// <summary>
        ///     The default Constructor.
        /// </summary>
        public VixenPlus() {
            ID = 3;
        }


        ///// <summary>
        ///// Constructor with a profile's filename to open
        ///// </summary>
        ///// <param name="filename">Name of the Profile file.</param>
        //public VixenPlus(string filename)
        //	: base(filename)
        //{ }

        #endregion [ Constructors ]

        #region [ Private Methods ]

        /// <summary>
        ///     Adds a "general" output plugin node to this list _loadedPlugIns. This is typically some other output plugin found
        ///     in the Profile beside the one
        ///     that contains our data.
        /// </summary>
        protected override void AddGeneralPlugIn(XmlNode node) {
            _loadedPlugIns.Add(new Vixen25x_GeneralPlugIn(node));
        }


        /// <summary>
        ///     Create the PlugIn node for either ElfPreview or AdjustablePreview data.
        /// </summary>
        /// <param name="doc">XmlDocument that owns this node</param>
        /// <param name="elfPluginNode">Indicates whether this is an Elf Plugin Node.</param>
        /// <returns>XmlNode formatted with PlugIn data.</returns>
        protected override XmlNode CreatePlugInNode(XmlDocument doc, bool elfPluginNode) {
            var plugIn = new VixenPlusGeneralPlugIn {
                Name = elfPluginNode ? EP_NAME : AP_NAME, ID = 0, Key = elfPluginNode ? -165848267 : -399317235, Enabled = true, FromChannel = MinID,
                ToChannel = MaxID
            };
            return plugIn.CreateNode(doc);
        }


        /// <summary>
        ///     Creates a new Channel object, loading from the topmost Channel node in the Profile xml.
        /// </summary>
        /// <param name="node">Xml node containing initial Channel data.</param>
        /// <returns>Channel object.</returns>
        protected override RawChannel LoadChannelFromXml(XmlNode node) {
            var channel = base.LoadChannelFromXml(node);
            channel.Name = node.InnerText;
            return channel;
        }


        /// <summary>
        ///     Saves the Channel Name.
        /// </summary>
        protected override void SaveChannelName(XmlNode node, RawChannel channel) {
            node.InnerText = channel.Name;
        }

        #endregion [ Private Methods ]

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

            if (System.String.Compare(_detectedTypeName, FormatName, System.StringComparison.OrdinalIgnoreCase) == 0) {
                return true;
            }

            // 2.1.x -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
            //			PlugInData/Plugin		-> No Output attribute
            
            return !_hasChannelNameInAttribute && _hasOutputAttribute;
        }

        #endregion [ Public Methods ]
    }

    public class VixenPlusGeneralPlugIn : GeneralPlugIn {
        public VixenPlusGeneralPlugIn() {}

        [UsedImplicitly]
        public VixenPlusGeneralPlugIn(XmlNode plugInNode) : base(plugInNode) {}


        public override XmlNode CreateNode(XmlDocument doc) {
            var node = base.CreateNode(doc);
            XmlHelper.Instance.AddAttribute(node, "type", "Output");
            return node;
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