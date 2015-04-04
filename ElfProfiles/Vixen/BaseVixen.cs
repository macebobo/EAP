using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Profiles;
using ElfCore.Util;

using ElfRes = ElfProfiles.Properties.Resources;

namespace ElfProfiles.Vixen {
    /// <summary>
    ///     Handles all the profile specific data and properties.
    /// </summary>
    public class BaseVixen : ElfDisposable, IProfile {
        #region [ Constants ]

        // AP Constants
        public const string AP_NAME = "Adjustable preview";
        private const string XmlNode_BackgroundImage = "BackgroundImage";
        private const string XmlNode_PixelSize = "PixelSize";
        private const string XmlNode_GridLineWidth = "GridLineWidth";
        private const string Attribute_Number = "number";

        // EP Constants
        public const string EP_NAME = "Elf Preview";
        private const string XmlNode_Scaling = "Scaling";
        private const string XmlNode_Background = "Background";
        private const string XmlNode_Bitmap = "Bitmap";

        // Xml node names
        private const string XmlNode_Root = "Profile";
        private const string XmlNode_ChannelObjects = "ChannelObjects";
        private const string XmlNode_Channel = "Channel";
        private const string XmlNode_ChannelGroup = "ChannelGroup";
        private const string XmlNode_ChannelGroups = "ChannelGroups";
        private const string XmlNode_Channels = "Channels";
        private const string XmlNode_DialogPositions = "DialogPositions";
        private const string XmlNode_DisabledChannels = "DisabledChannels";
        private const string XmlNode_Display = "Display";
        private const string XmlNode_Height = "Height";
        private const string XmlNode_Outputs = "Outputs";
        internal const string XmlNode_PlugInData = "PlugInData";
        internal const string XmlNode_PlugIn = "PlugIn";
        private const string XmlNode_PlayBackDialog = "PreviewDialog";
        private const string XmlNode_Profile = "Profile";
        private const string XmlNode_SortOrder = "SortOrder";
        private const string XmlNode_Shuffles = "SortOrders";
        private const string XmlNode_Width = "Width";

        private const string XmlNode_Cells = "Cells";
        private const string XmlNode_SubChannel = "SubChannel";
        private const string XmlNode_SubChannels = "SubChannels";
        private const string XmlNode_Vector = "Vector";

        private const string XmlNode_CellSize = "CellSize";
        private const string XmlNode_ShowGridLines = "ShowGridLines";
        private const string XmlNode_Zoom = "Zoom";

        // Attribute names
        private const string Attribute_BorderColor = "borderColor";
        private const string Attribute_Color = "color";
        private const string Attribute_Enabled = "enabled";
        private const string Attribute_ID = "id";
        private const string Attribute_LastSort = "lastSort";
        private const string Attribute_Locked = "locked";
        protected const string Attribute_Name = "name";
        private const string Attribute_Output = "output";
        private const string Attribute_Point_X = "x";
        private const string Attribute_Point_Y = "y";
        private const string Attribute_Type = "type";
        private const string Attribute_Version = "version";
        private const string Attribute_Visible = "visible";

        #endregion [ Constants ]

        #region [ Protected Variables ]

        protected List<RawChannel> _channels;
        protected string _detectedTypeName = string.Empty;
        protected bool _hasChannelNameInAttribute = false;
        protected bool _hasOutputAttribute = false;
        protected GeneralPlugInList _loadedPlugIns = new GeneralPlugInList();
        protected XmlNode _setupNode = null;
        protected XmlHelper _xmlHelper = XmlHelper.Instance;

        #endregion [ Protected Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Returns the largest ID in the list.
        /// </summary>
        protected int MaxID {
            get {
                int MaxID = -1;
                foreach (RawChannel Channel in Channels) {
                    MaxID = Math.Max(MaxID, Channel.ID);
                }
                return MaxID;
            }
        }

        /// <summary>
        ///     Returns the smallest ID in the list.
        /// </summary>
        protected int MinID {
            get {
                int MinID = Int32.MaxValue;
                foreach (RawChannel Channel in Channels) {
                    MinID = Math.Min(MinID, Channel.ID);
                }
                return MinID;
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        /// <summary>
        ///     The default Constructor.
        /// </summary>
        public BaseVixen() {
            Channels = new List<RawChannel>();
            Background = new Background();
            Scaling = new Scaling();
        }


        ///// <summary>
        ///// Constructor with a profile's filename to open
        ///// </summary>
        ///// <param name="filename">Name of the Profile file.</param>
        //public BaseVixen(string filename)
        //	: this()
        //{ }

        #endregion [ Constructors ]

        #region [ Private Methods ]

        /// <summary>
        ///     Adds a "general" output plugin node to this list _loadedPlugIns. This is typically some other output plugin found
        ///     in the Profile beside the one
        ///     that contains our data.
        /// </summary>
        protected virtual void AddGeneralPlugIn(XmlNode node) {
            throw new NotImplementedException("AddGeneralPlugIn is not implemented");
        }


        /// <summary>
        ///     Looks at the PlugIn node and checks to see that the Channel range it has is still valid, accounting
        ///     for any deleted channels.
        /// </summary>
        /// <param name="node">XmlNode containing PlugIn data.</param>
        /// <returns>Altered XmlNode.</returns>
        private XmlNode AdjustPlugInNode_Channels(XmlNode node) {
            if (_xmlHelper.GetAttributeValue(node, Attribute_Name) == EP_NAME) {
                return node;
            }
            int ChannelCount = Channels.Count;
            int ToChannel = _xmlHelper.GetAttributeValue(node, GeneralPlugIn.Attribute_To, 0) - 1;
            if (ToChannel >= ChannelCount) {
                _xmlHelper.SetAttribute(node, GeneralPlugIn.Attribute_To, ChannelCount);
            }
            return node;
        }


        /// <summary>
        ///     Clears out all the value for the properies and protected virtual variables. Used to initialize the object
        ///     initially, and when loading new data
        /// </summary>
        protected void Clear() {
            //base.Clear();
        }


        /// <summary>
        ///     Create the PlugIn node for either ElfPreview or AdjustablePreview data.
        /// </summary>
        /// <param name="doc">XmlDocument that owns this node</param>
        /// <param name="elfPluginNode">Indicates whether this is an Elf Plugin Node.</param>
        /// <returns>XmlNode formatted with PlugIn data.</returns>
        protected virtual XmlNode CreatePlugInNode(XmlDocument doc, bool elfPluginNode) {
            throw new NotImplementedException("BaseVixen.CreatePlugInNode");
        }


        /// <summary>
        ///     Finds the Channel by its ID value and returns it.
        /// </summary>
        /// <param name="id">ID value on the Channel desired</param>
        /// <returns>If the Channel is not found, returns null, otherwise returns the correct Channel</returns>
        private RawChannel GetChannel(int id) {
            foreach (RawChannel item in Channels) {
                if (item.ID == id) {
                    return item;
                }
            }
            return null;
        }


        /// <summary>
        ///     Loads in the Profile data from the Adjustable Preview data node
        /// </summary>
        /// <param name="data">GeneralPlugIn object containing Adjustable Preview data.</param>
        /// <returns>Returns false if the object is null, or the data is empty, true otherwise.</returns>
        private bool LoadAPData(GeneralPlugIn data) {
            if ((data == null) || (data.PlugInNode == null)) {
                return false;
            }

            int Number = 0;
            string Encoded = string.Empty;
            RawChannel Channel = null;

            XmlNodeList ChannelList = data.PlugInNode.SelectNodes(XmlNode_Channels + "/" + XmlNode_Channel);
            if ((ChannelList == null) || (ChannelList.Count == 0)) {
                return false;
            }

            XmlNode Node = data.PlugInNode.SelectSingleNode(XmlNode_Display);
            Scaling.ShowGridLines = (_xmlHelper.GetNodeValue(Node, XmlNode_GridLineWidth, "1") == "1");
            Scaling.CellSize = _xmlHelper.GetNodeValue(Node, XmlNode_PixelSize, Scaling.CellSize.GetValueOrDefault(1));
            Scaling.LatticeSize = new Size(_xmlHelper.GetNodeValue(Node, XmlNode_Width, 64), _xmlHelper.GetNodeValue(Node, XmlNode_Height, 32));

            // 10 is the default brightness for adjustable preview, indicating no change in brightness.
            float Brightness = _xmlHelper.GetNodeValue(Node, Background.Property_Brightness, 10);
            Brightness -= 10;
            Brightness /= 10;
            Background.Brightness = Brightness;

            // Load in the Channel Cell data
            foreach (XmlNode ChannelNode in ChannelList) {
                Number = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Number, 0);
                Encoded = ChannelNode.InnerText;

                // Find the Channel that corresponds to this number
                Channel = GetChannel(Number);
                if (Channel != null) {
                    Channel.EncodedRasterData = Encoded;
                    if ((Number < data.FromChannel) || (Number > data.ToChannel)) {
                        Channel.Included = false;
                    }
                }
            }
            ChannelList = null;
            Channel = null;

            // Load in the Background data
            Background.SuppressEvents = true;
            Encoded = _xmlHelper.GetNodeValue(data.PlugInNode, XmlNode_BackgroundImage, string.Empty);
            if (Encoded.Length > 0) {
                Background.LoadFromStream(Encoded);
            }

            return true;
        }


        /// <summary>
        ///     Loads in the Profile data from the Elf Preview data node
        /// </summary>
        /// <param name="data">GeneralPlugIn object containing Elf Preview data.</param>
        /// <returns>Returns false if the object is null, or the data is empty, true otherwise.</returns>
        private bool LoadEPData(GeneralPlugIn data) {
            if ((data == null) || (data.PlugInNode == null)) {
                return false;
            }

            int Number = 0;
            string Encoded = string.Empty;
            RawChannel Channel = null;

            XmlNodeList ChannelList = data.PlugInNode.SelectNodes(XmlNode_Channels + "/" + XmlNode_Channel);
            if ((ChannelList == null) || (ChannelList.Count == 0)) {
                return false;
            }

            XmlNode Node = data.PlugInNode.SelectSingleNode(XmlNode_Background);
            if (Node != null) {
                Background.SuppressEvents = true;
                Background.Filename = _xmlHelper.GetNodeValue(Node, Background.Property_Filename, string.Empty);
                Background.SaveEncodedImage = _xmlHelper.GetNodeValue(Node, Background.Property_SaveEncodedImage, true);
                Background.Color = _xmlHelper.GetNodeValue(Node, Background.Property_Color, Color.Black);
                Background.GridColor = _xmlHelper.GetNodeValue(Node, Background.Property_GridColor, Color.Empty);
                Background.Brightness = _xmlHelper.GetNodeValue(Node, Background.Property_Brightness, Background.Default_Brightness);
                Background.Saturation = _xmlHelper.GetNodeValue(Node, Background.Property_Saturation, Background.Default_Saturation);
                Background.Hue = _xmlHelper.GetNodeValue(Node, Background.Property_Hue, Background.Default_Hue);
                Background.OverlayGrid = _xmlHelper.GetNodeValue(Node, Background.Property_OverlayGrid, false);
                Background.Visible = _xmlHelper.GetNodeValue(Node, Background.Property_Visible, Background.Default_Visible);
                Background.WallpaperStyle =
                    EnumHelper.GetEnumFromValue<WallpaperStyle>(_xmlHelper.GetNodeValue(Node, Background.Property_WallpaperStyle,
                        (int) WallpaperStyle.Fill));
                Background.WallpaperAnchor =
                    Background.GetAnchorEnum(_xmlHelper.GetNodeValue(Node, Background.Property_Anchor, (int) (AnchorStyles.Top | AnchorStyles.Left)));

                if (Background.SaveEncodedImage) {
                    Encoded = _xmlHelper.GetNodeValue(Node, XmlNode_Bitmap);
                    if (Encoded.Length > 0) {
                        Background.LoadFromStream(Encoded);
                    }
                }
                else {
                    if (Background.Filename.Length > 0) {
                        var FI = new FileInfo(Background.Filename);
                        if (FI.Exists) {
                            Background.Image = ImageHandler.LoadBitmapFromFile(Background.Filename);
                        }
                        else {
                            Background.Image = ImageHandler.CreateErrorMessageImage("Background file \"" + Background.Filename + "\" not found.",
                                Background.Color, Color.White);
                            Background.WallpaperStyle = WallpaperStyle.Fill;
                            Background.WallpaperAnchor = AnchorStyles.Top | AnchorStyles.Left;
                        }
                    }
                }

                Background.SuppressEvents = false;
            }
            // </Background>

            // <Scaling>
            // Load in Scaling information
            Node = data.PlugInNode.SelectSingleNode(XmlNode_Scaling);
            Scaling = LoadScaling(Node);

            //<Channels>
            foreach (XmlNode ChannelNode in ChannelList) {
                Number = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Output, 0);

                // Find the Channel that corresponds to this number
                Channel = GetChannel(Number);
                if (Channel == null) {
                    continue;
                }
                if ((Number < data.FromChannel) || (Number > data.ToChannel)) {
                    Channel.Included = false;
                }

                Channel.Locked = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Locked, false);
                Channel.Visible = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Visible, true);
                Channel.RenderColor = _xmlHelper.GetNodeValue(ChannelNode, ElfCore.Channels.Channel.PropertyRenderColor, Color.Empty);
                Channel.BorderColor = _xmlHelper.GetNodeValue(ChannelNode, ElfCore.Channels.Channel.PropertyBorderColor, Color.Empty);

                Node = ChannelNode.SelectSingleNode(ElfCore.Channels.Channel.PropertyOrigin);
                if (Node != null) {
                    Channel.Origin = new Point(_xmlHelper.GetAttributeValue(Node, Attribute_Point_X, 0),
                        _xmlHelper.GetAttributeValue(Node, Attribute_Point_Y, 0));
                }

                Encoded = _xmlHelper.GetNodeValue(ChannelNode, XmlNode_Cells, string.Empty);
                if (Encoded.Length > 0) {
                    Channel.EncodedRasterData = Encoded;
                }

                Encoded = _xmlHelper.GetNodeValue(ChannelNode, XmlNode_Vector, string.Empty);
                if (Encoded.Length > 0) {
                    Channel.EncodedVectorData = Encoded;
                }

                //Channel.Loading = false;
            }
            //</Channels>

            //<ChannelGroups>
            //ChannelGroup Group = null;
            //XmlNodeList GroupList = data.PlugInNode.SelectNodes("ChannelGroups");
            //if (GroupList != null)
            //	foreach (XmlNode GroupNode in GroupList)
            //	{
            //		Group = new ChannelGroup();
            //		Group.ID = _xmlHelper.GetAttributeValue(GroupNode, Attribute_ID, 0);
            //		Group.Name = _xmlHelper.GetAttributeValue(GroupNode, Attribute_Name, string.Empty);
            //		Group.SerializedList = GroupNode.InnerText;
            //		this.Channels.Groups.List.Add(Group);
            //	}
            //</ChannelGroups>

            return true;
        }


        /// <summary>
        ///     Creates a new Channel object, loading from the topmost Channel node in the Profile xml.
        /// </summary>
        /// <param name="node">Xml node containing initial Channel data.</param>
        /// <returns>Channel object.</returns>
        protected virtual RawChannel LoadChannelFromXml(XmlNode node) {
            var Channel = new RawChannel();
            //Channel.Loading = true;
            //Channel.Profile = this;

            string Value = _xmlHelper.GetAttributeValue(node, Attribute_Color, Color.White.ToArgb().ToString());
            Channel.SequencerColor = Color.FromArgb(Convert.ToInt32(Value));

            Value = _xmlHelper.GetAttributeValue(node, Attribute_Output, "0");
            Channel.ID = Convert.ToInt32(Value);

            Value = _xmlHelper.GetAttributeValue(node, ElfCore.Channels.Channel.PropertyEnabled.ToLower());
            Channel.Enabled = Convert.ToBoolean(Value);

            // Channel.Name loaded from one of the derived classes.
            return Channel;
        }


        /// <summary>
        ///     Populate the ChannelObjects, Outputs and DisableChannels nodes of the Profile xml document.
        /// </summary>
        private void PopulateChannelNodes(XmlNode channelObjectsNode, XmlNode outputsNode, XmlNode disableChannelsNode) {
            XmlNode Node = null;
            XmlDocument Doc = channelObjectsNode.OwnerDocument;
            string OutputIDs = string.Empty;
            string DisabledIDs = string.Empty;

            channelObjectsNode.RemoveAll();

            foreach (RawChannel Channel in Channels) {
                Node = Doc.CreateElement(XmlNode_Channel);
                channelObjectsNode.AppendChild(Node);
                if (Channel.SequencerColor.IsEmpty) {
                    _xmlHelper.AddAttribute(Node, Attribute_Color, Channel.RenderColor.ToArgb().ToString());
                }
                else {
                    _xmlHelper.AddAttribute(Node, Attribute_Color, Channel.SequencerColor.ToArgb().ToString());
                }
                _xmlHelper.AddAttribute(Node, Attribute_Output, Channel.ID.ToString());
                _xmlHelper.AddAttribute(Node, Attribute_ID, DateTime.Now.Ticks.ToString());
                _xmlHelper.AddAttribute(Node, Attribute_Enabled, Channel.Enabled.ToString());
                SaveChannelName(Node, Channel);
                OutputIDs += ((OutputIDs.Length > 0) ? "," : string.Empty) + Channel.ID;
                if (!Channel.Enabled) {
                    DisabledIDs += ((DisabledIDs.Length > 0) ? "," : string.Empty) + Channel.ID;
                }
            }
            outputsNode.InnerText = OutputIDs;
            disableChannelsNode.InnerText = DisabledIDs;

            Doc = null;
            Node = null;
        }


        /// <summary>
        ///     Clears out and Populate the SortOrders node of the Profile xml.
        /// </summary>
        private void SaveShuffles(XmlNode shuffleListNode) {
            string List = string.Empty;
            string Name = string.Empty;
            XmlNode Node = null;

            if (shuffleListNode == null) {
                return;
            }
            shuffleListNode.RemoveAll();

            if (ActiveShuffleIndex == 0) {
                _xmlHelper.SetAttribute(shuffleListNode, Attribute_LastSort, -1);
            }
            else {
                _xmlHelper.SetAttribute(shuffleListNode, Attribute_LastSort, ActiveShuffleIndex - 1);
            }

            foreach (string Shuffle in ShuffleList) {
                if (Shuffle.Length == 0) {
                    continue;
                }
                Name = Shuffle.Substring(0, Shuffle.IndexOf(','));
                List = Shuffle.Substring(Name.Length + 1);

                if (Name == ElfCore.Core.Shuffle.NATIVE_SHUFFLE) {
                    continue;
                }

                Node = shuffleListNode.OwnerDocument.CreateElement(XmlNode_SortOrder);
                _xmlHelper.AddAttribute(Node, Attribute_Name, Name);
                Node.InnerText = List;
                shuffleListNode.AppendChild(Node);
            }
            Node = null;
        }


        /// <summary>
        ///     Saves the Channel Name. This method must be overwritten by the class that inherits this class.
        /// </summary>
        protected virtual void SaveChannelName(XmlNode node, RawChannel channel) {
            throw new NotImplementedException("SaveChannelName");
        }


        /// <summary>
        ///     Saves the data to the Profile Xml that is specific to this PlugIn, in Elf Preview format.
        /// </summary>
        /// <param name="data">GeneralPlugIn object containing Elf Preview data.</param>
        private void SaveEPData(XmlNode plugInNode) {
            if (plugInNode == null) {
                return;
            }

            XmlDocument Doc = plugInNode.OwnerDocument;
            XmlNode Node = null;
            XmlNode ParentNode = null;

            #region [ Background ]

            ParentNode = plugInNode.SelectSingleNode(XmlNode_Background);
            if (ParentNode == null) {
                ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Background);
            }
            ParentNode.RemoveAll();

            if (Background.Filename.Length > 0) {
                _xmlHelper.SetValue(ParentNode, Background.Property_Filename, Background.Filename);
            }

            if (!Background.Color.IsEmpty) {
                _xmlHelper.SetValue(ParentNode, Background.Property_Color, Background.Color);
            }

            if (!Background.GridColor.IsEmpty) {
                _xmlHelper.SetValue(ParentNode, Background.Property_GridColor, Background.GridColor);
            }

            _xmlHelper.SetValue(ParentNode, Background.Property_Brightness, Background.Brightness);
            _xmlHelper.SetValue(ParentNode, Background.Property_Saturation, Background.Saturation);
            _xmlHelper.SetValue(ParentNode, Background.Property_Hue, Background.Hue);
            _xmlHelper.SetValue(ParentNode, Background.Property_OverlayGrid, Background.OverlayGrid);
            _xmlHelper.SetValue(ParentNode, Background.Property_Visible, Background.Visible);
            _xmlHelper.SetValue(ParentNode, Background.Property_WallpaperStyle, (int) Background.WallpaperStyle);
            _xmlHelper.SetValue(ParentNode, Background.Property_Anchor, (int) Background.WallpaperAnchor);
            _xmlHelper.SetValue(ParentNode, Background.Property_SaveEncodedImage, Background.SaveEncodedImage);

            if (Background.SaveEncodedImage) {
                string Encoded = string.Empty;
                if (Background.Image != null) {
                    var ms = new MemoryStream();
                    Background.Image.Save(ms, ImageFormat.Png);
                    Encoded = Convert.ToBase64String(ms.ToArray());
                    ms.Dispose();
                }
                if (Encoded.Length > 0) {
                    _xmlHelper.SetValue(ParentNode, XmlNode_Bitmap, Encoded);
                }
            }
            ParentNode = null;

            #endregion [ Background ]

            #region  [ Scaling ]

            ParentNode = plugInNode.SelectSingleNode(XmlNode_Scaling);
            if (ParentNode == null) {
                ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Scaling);
            }
            ParentNode.RemoveAll();

            _xmlHelper.SetValue(ParentNode, XmlNode_Height, Scaling.LatticeSize.Height);
            _xmlHelper.SetValue(ParentNode, XmlNode_Width, Scaling.LatticeSize.Width);
            _xmlHelper.SetValue(ParentNode, XmlNode_CellSize, Scaling.CellSize.GetValueOrDefault(1));
            _xmlHelper.SetValue(ParentNode, XmlNode_ShowGridLines, Scaling.ShowGridLines.GetValueOrDefault(true));
            _xmlHelper.SetValue(ParentNode, XmlNode_Zoom, Scaling.Zoom.GetValueOrDefault(1));

            ParentNode = null;

            #endregion  [ Scaling ]

            #region [ Channels ]

            ParentNode = plugInNode.SelectSingleNode(XmlNode_Channels);
            if (ParentNode == null) {
                ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Channels);
            }
            ParentNode.RemoveAll();

            XmlNode OriginNode = null;

            foreach (RawChannel Channel in Channels) {
                Node = Doc.CreateElement(XmlNode_Channel);
                ParentNode.AppendChild(Node);

                _xmlHelper.SetAttribute(Node, Attribute_Output, Channel.ID);
                _xmlHelper.SetAttribute(Node, Attribute_Name, Channel.Name);
                if (Channel.Locked) {
                    _xmlHelper.SetAttribute(Node, Attribute_Locked, Channel.Locked);
                }
                if (!Channel.Visible) {
                    _xmlHelper.SetAttribute(Node, Attribute_Visible, Channel.Visible);
                }

                if (!Channel.Origin.IsEmpty) {
                    OriginNode = _xmlHelper.CreateNode(Doc, Node, ElfCore.Channels.Channel.PropertyOrigin);
                    _xmlHelper.SetAttribute(OriginNode, Attribute_Point_X, Channel.Origin.X);
                    _xmlHelper.SetAttribute(OriginNode, Attribute_Point_Y, Channel.Origin.X);
                }

                if (!Channel.RenderColor.IsEmpty && !Channel.RenderColor.Equals(Channel.SequencerColor)) {
                    _xmlHelper.SetValue(Node, ElfCore.Channels.Channel.PropertyRenderColor, Channel.RenderColor);
                }

                if (!Channel.BorderColor.IsEmpty) {
                    _xmlHelper.SetValue(Node, ElfCore.Channels.Channel.PropertyBorderColor, Channel.BorderColor);
                }

                if (Channel.EncodedRasterData.Length > 0) {
                    _xmlHelper.SetValue(Node, XmlNode_Cells, Channel.EncodedRasterData);
                }
                if (Channel.EncodedVectorData.Length > 0) {
                    _xmlHelper.SetValue(Node, XmlNode_Vector, Channel.EncodedVectorData);
                }
            }
            OriginNode = null;
            ParentNode = null;

            #endregion [ Channels ]

            #region [ ChannelGroups ]

            //ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_ChannelGroups);
            //foreach (ChannelGroup Group in this.Channels.Groups.List)
            //{
            //	Node = _xmlHelper.CreateNode(Doc, ParentNode, XmlNode_ChannelGroup);
            //	Node.InnerText = Group.SerializedList;
            //	_xmlHelper.SetAttribute(Node, Attribute_Name, Group.Name);
            //	_xmlHelper.SetAttribute(Node, Attribute_ID, Group.ID.ToString());
            //}
            //ParentNode = null;

            #endregion [ ChannelGroups ]
        }


        /// <summary>
        ///     Saves the data to the Profile Xml that is specific to this PlugIn, in Adjustable Preview format.
        /// </summary>
        /// <param name="data">GeneralPlugIn object containing Elf Preview data.</param>
        private void SaveAPData(XmlNode plugInNode) {
            if (plugInNode == null) {
                return;
            }

            XmlDocument Doc = plugInNode.OwnerDocument;
            XmlNode Node = null;
            XmlNode ParentNode = null;

            #region [ Background ]

            Node = plugInNode.SelectSingleNode(XmlNode_BackgroundImage);
            if (Node == null) {
                Node = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_BackgroundImage);
            }

            string Encoded = string.Empty;
            if (Background.Image != null) {
                var ms = new MemoryStream();
                Background.Image.Save(ms, ImageFormat.Png);
                Encoded = Convert.ToBase64String(ms.ToArray());
                ms.Dispose();
            }
            Node.InnerText = Encoded;

            Node = plugInNode.SelectSingleNode("RedirectOutputs");
            if (Node == null) {
                Node = _xmlHelper.CreateNode(Doc, plugInNode, "RedirectOutputs");
            }
            Node.InnerText = "False";

            #endregion [ Background ]

            #region  [ Display ]

            ParentNode = plugInNode.SelectSingleNode(XmlNode_Display);
            if (ParentNode == null) {
                ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Display);
            }
            ParentNode.RemoveAll();

            _xmlHelper.SetValue(ParentNode, XmlNode_Height, Scaling.LatticeSize.Height);
            _xmlHelper.SetValue(ParentNode, XmlNode_Width, Scaling.LatticeSize.Width);
            _xmlHelper.SetValue(ParentNode, XmlNode_PixelSize, Scaling.CellSize.GetValueOrDefault(1));

            int Brightness = (int) Math.Ceiling(Background.Brightness * 10) + 10;
            _xmlHelper.SetValue(ParentNode, Background.Property_Brightness, Brightness);

            ParentNode = null;

            #endregion  [ Display ]

            #region [ Channels ]

            ParentNode = plugInNode.SelectSingleNode(XmlNode_Channels);
            if (ParentNode == null) {
                ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Channels);
            }
            ParentNode.RemoveAll();

            foreach (RawChannel Channel in Channels) {
                Node = Doc.CreateElement(XmlNode_Channel);
                ParentNode.AppendChild(Node);

                _xmlHelper.SetAttribute(Node, Attribute_Number, Channel.ID);

                if (Channel.EncodedRasterData.Length > 0) {
                    Node.InnerText = Channel.EncodedRasterData;
                }
            }
            ParentNode = null;

            #endregion [ Channels ]
        }

        #endregion [ Private Methods ]

        #region [ Public Methods ]

        /// <summary>
        ///     Loads in the profile that is already loaded into the XmlDocument object.
        /// </summary>
        /// <param name="doc">XmlDocument object that contains the Profile.</param>
        private bool Load(BaseProfile profile, XmlDocument doc, List<RawChannel> rawChannels) {
            RawChannel rawChannel = null;
            XmlNode Root = null;
            XmlNode Node = null;
            XmlNodeList NodeList = null;
            string NativeShuffle = string.Empty;

            Clear();
            Root = doc.DocumentElement;

            if ((rawChannels == null) || (rawChannels.Count == 0)) {
                // Profile/ChannelObjects/Channel
                // Load the Channel data. Channels are stored in 2 spots in a Vixen 2.x Profile. 
                // Once at the top, used by Vixen itself, and also under PlugInData, used by the output PlugIn.
                NodeList = Root.SelectNodes(XmlNode_ChannelObjects + "/" + XmlNode_Channel);
                if (NodeList != null) {
                    foreach (XmlNode ChannelNode in NodeList) {
                        rawChannel = LoadChannelFromXml(ChannelNode);
                        NativeShuffle += ((NativeShuffle.Length > 0) ? "," : string.Empty) + rawChannel.ID;
                        Channels.Add(rawChannel);
                    }
                }
            }
            else {
                foreach (RawChannel rChannel in rawChannels) {
                    NativeShuffle += ((NativeShuffle.Length > 0) ? "," : string.Empty) + rChannel.ID;
                    Channels.Add(rChannel);
                }
            }

            Channel Channel = null;
            int Idx = 0;
            foreach (RawChannel rChannel in Channels) {
                Channel = new Channel {
                    Loading = true, BorderColor = rChannel.BorderColor, Enabled = rChannel.Enabled, ID = rChannel.ID, Included = rChannel.Included,
                    Index = Idx++, Name = rChannel.Name, RenderColor = rChannel.RenderColor, SequencerColor = rChannel.SequencerColor,
                };
                Channel.DeserializeLattice(rChannel.EncodedRasterData);
                Channel.Loading = false;
            }

            // Profile/SortOrders
            ShuffleList = new List<string>();
            Node = Root.SelectSingleNode(XmlNode_Shuffles);
            if (Node != null) {
                ActiveShuffleIndex = _xmlHelper.GetAttributeValue(Node, Attribute_LastSort, 0) + 1;
                NodeList = Node.SelectNodes(XmlNode_SortOrder);
                foreach (XmlNode ShuffleNode in NodeList) {
                    ShuffleList.Add(_xmlHelper.GetAttributeValue(ShuffleNode, Attribute_Name) + "," + ShuffleNode.InnerText);
                }
            }

            NodeList = Root.SelectNodes("//" + XmlNode_PlugIn);
            if (NodeList != null) {
                foreach (XmlNode PlugInNode in NodeList) {
                    _loadedPlugIns.Add(new GeneralPlugIn(PlugInNode));
                }
            }

            bool Loaded = true;
            if (_loadedPlugIns.Count > 0) {
                if (!LoadEPData(_loadedPlugIns.Where(EP_NAME))) {
                    if (!LoadAPData(_loadedPlugIns.Where(AP_NAME))) {
                        Loaded = false;
                    }
                    else {
                        profile.Dirty = true;
                    }
                }
            }
            else {
                if (!LoadEPData(new GeneralPlugIn(Root))) {
                    Loaded = false;
                }
            }

            if (!Loaded) {
                // Neither Adjustable preview nor Elf data has been found.
                // Initialize with default settings.
                Scaling.CellSize = 7;
                Scaling.LatticeSize = new Size(64, 32);
                Scaling.ShowGridLines = true;
                Background.Brightness = 10;
            }

            Root = null;
            Node = null;
            NodeList = null;
            Channel = null;

            return true;
        }


        /// <summary>
        ///     Loads in the profile that is already loaded into the XmlDocument object.
        /// </summary>
        /// <param name="doc">XmlDocument object that contains the Profile.</param>
        private bool Load(BaseProfile profile, XmlDocument doc) {
            return Load(profile, doc, null);
        }


        //private void LoadChannelData(List<ElfCore.Channels.RawChannel> rawChannelList)
        //{
        //	foreach (ElfCore.Channels.RawChannel Raw in rawChannelList)
        //	{
        //		Channels.Add(new RawChannel()
        //		{
        //			Name = Raw.Name,
        //			SequencerColor = Raw.SequencerColor,
        //			Enabled = Raw.Enabled
        //		});
        //	}
        //}

        #endregion [ Public Methods ]

        #region [ Public Static Methods ]

        /// <summary>
        ///     Used to load in Vixen Channels in the event of a brand new Profile, where there
        ///     is no channel data in the XmlDocument at all.
        /// </summary>
        /// <param name="rawChannelList">List of Channel Properties, used to populate the Channels</param>
        /// <summary>
        ///     Load Scaling information out of the XmlNode
        /// </summary>
        /// <param name="node">XmlNode containing Scaling information</param>
        /// <returns>Returns a Scaling object, populated from the data in the XmlNode</returns>
        public static Scaling LoadScaling(XmlNode node) {
            var Scaling = new Scaling();
            XmlHelper XmlHelper = XmlHelper.Instance;

            if (node != null) {
                Scaling.SuppressEvents = true;
                Scaling.LatticeSize = new Size(XmlHelper.GetNodeValue(node, XmlNode_Width, 64), XmlHelper.GetNodeValue(node, XmlNode_Height, 32));
                Scaling.CellSize = XmlHelper.GetNodeValue(node, XmlNode_CellSize, 7);
                Scaling.ShowGridLines = XmlHelper.GetNodeValue(node, XmlNode_ShowGridLines, true);
                Scaling.Zoom = XmlHelper.GetNodeValue(node, XmlNode_Zoom, 1f);
                Scaling.SuppressEvents = false;
            }
            XmlHelper = null;
            return Scaling;
        }


        /// <summary>
        ///     Load Background information out of the XmlNode
        /// </summary>
        /// <param name="node">XmlNode containing Background information</param>
        /// <returns>Returns a Background object, populated from the data in the XmlNode</returns>
        public static Background LoadBackground(XmlNode node) {
            var Background = new Background();
            XmlHelper XmlHelper = XmlHelper.Instance;

            if (node != null) {
                Background.SuppressEvents = true;
                //if (Encoded.Length > 0)
                //	Background.LoadFromStream(Encoded);
                Background.SaveEncodedImage = XmlHelper.GetNodeValue(node, Background.Property_SaveEncodedImage, true);
                Background.Color = XmlHelper.GetNodeValue(node, Background.Property_Color, Color.Black);
                Background.GridColor = XmlHelper.GetNodeValue(node, Background.Property_GridColor, Color.Empty);
                Background.Brightness = XmlHelper.GetNodeValue(node, Background.Property_Brightness, Background.Default_Brightness);
                Background.Saturation = XmlHelper.GetNodeValue(node, Background.Property_Saturation, Background.Default_Saturation);
                Background.Hue = XmlHelper.GetNodeValue(node, Background.Property_Hue, Background.Default_Hue);
                Background.OverlayGrid = XmlHelper.GetNodeValue(node, Background.Property_OverlayGrid, false);
                Background.Visible = XmlHelper.GetNodeValue(node, Background.Property_Visible, true);
                Background.WallpaperStyle =
                    EnumHelper.GetEnumFromValue<WallpaperStyle>(XmlHelper.GetNodeValue(node, Background.Property_WallpaperStyle,
                        (int) WallpaperStyle.Fill));
                Background.WallpaperAnchor =
                    Background.GetAnchorEnum(XmlHelper.GetNodeValue(node, Background.Property_Anchor, (int) (AnchorStyles.Top | AnchorStyles.Left)));
                Background.SuppressEvents = false;

                if (Background.SaveEncodedImage) {
                    string Encoded = XmlHelper.GetNodeValue(node, XmlNode_Bitmap);
                    if (Encoded.Length > 0) {
                        Background.LoadFromStream(Encoded);
                    }
                }
                else {
                    Background.Filename = XmlHelper.GetNodeValue(node, Background.Property_Filename, string.Empty);
                    if (Background.Filename.Length > 0) {
                        var FI = new FileInfo(Background.Filename);
                        if (FI.Exists) {
                            Background.Image = ImageHandler.LoadBitmapFromFile(Background.Filename);
                        }
                    }
                }
            }

            XmlHelper = null;
            return Background;
        }

        #endregion [ Public Static Methods ]

        #region [ IProfile ]

        #region [ Properties ]

        /// <summary>
        ///     Index of the Shuffle that is current.
        /// </summary>
        public int ActiveShuffleIndex { get; set; }

        /// <summary>
        ///     Object that contains data and methods specific to the User Interface
        /// </summary>
        public Background Background { get; set; }

        /// <summary>
        ///     Object that controls the list of Channels and their various functions
        /// </summary>
        public List<RawChannel> Channels { get; set; }

        /// <summary>
        ///     Path where files are saved by default.
        /// </summary>
        public virtual string DefaultSavePath { get; set; }

        /// <summary>
        ///     Indicate whether this type of Profile should be currently displayed as an option in the Editor.
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        ///     Extention for the filename
        /// </summary>
        public virtual string FileExtension {
            get { return "pro"; }
            set { }
        }

        /// <summary>
        ///     Name of the file this Profile is stored in
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        ///     Name of the type of Profile.
        /// </summary>
        public virtual string FormatName {
            get { return "BASE_VIXEN"; }
        }

        /// <summary>
        ///     Image file that represents the Icon of the sequencing program used by this type of Profile.
        /// </summary>
        public virtual Bitmap IconImage {
            get { return ElfRes.undefined; }
        }

        /// <summary>
        ///     Unique ID assigned to this Profile type.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     Name given to the particular instance of a profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Scaling data, including Cell Size, Canvas Size, etc.
        /// </summary>
        public Scaling Scaling { get; set; }

        /// <summary>
        ///     List of Shuffles. Each shuffle is represented by the name of the shuffle followed by a comma and then a list of
        ///     channel ID, comma seperated
        ///     ex: "New Suffle,0,1,2,3,4,5,6,7"
        /// </summary>
        public List<string> ShuffleList { get; set; }

        #endregion [ Properties ]

        #region [ Methods ]

        public bool Load(BaseProfile profile) {
            return Load(profile, Filename);
        }


        /// <summary>
        ///     Loads in the Profile data from the file passed in.
        /// </summary>
        /// <param name="filename">Name of the file to load</param>
        /// <returns>Returns true if the load is successful, false otherwise</returns>
        public virtual bool Load(BaseProfile profile, string filename) {
            if ((filename ?? string.Empty).Length == 0) {
                throw new ArgumentException("Missing filename");
            }

            var FI = new FileInfo(filename);
            if (!FI.Exists) {
                throw new FileNotFoundException("File not found.", filename);
            }

            string Name = FI.Name;
            FI = null;

            var Doc = new XmlDocument();
            Doc.Load(filename);
            if (Name.Contains(".")) {
                Name = Name.Substring(0, Name.LastIndexOf('.'));
            }
            this.Name = Name;
            Filename = filename;

            if (Load(profile, Doc)) {
                return true;
            }
            this.Name = string.Empty;
            Filename = string.Empty;
            return false;
        }


        /// <summary>
        ///     Loads in the profile data coming from Vixen.
        /// </summary>
        /// <param name="setupNode">XmlNode containing the plugin data</param>
        /// <returns>Returns true if the load is successful, false otherwise</returns>
        public virtual bool Load(BaseProfile profile, XmlNode setupNode, List<RawChannel> rawChannels) {
            if (setupNode == null) {
                throw new ArgumentNullException("SetupData is null.");
            }
            _setupNode = setupNode;
            return Load(profile, setupNode.OwnerDocument, rawChannels);
        }


        /// <summary>
        ///     Saves the current Profile changes.
        ///     If we are editing a file, then will save to that file name.
        ///     If we are editing from an Xml node, save changes back to the parent XmlDocument.
        /// </summary>
        public virtual bool Save() {
            if (_setupNode != null) {
                return Save(_setupNode);
            }
            if (Filename.Length > 0) {
                return Save(Filename);
            }
            return false;
        }


        /// <summary>
        ///     Saves the Profile to file using the filename passed in
        /// </summary>
        /// <param name="filename">Filename to use to save the file to</param>
        /// <returns>Returns true if the save is successful, false otherwise</returns>
        public virtual bool Save(string filename) {
            if ((filename ?? string.Empty).Length == 0) {
                throw new Exception("Missing filename");
            }

            Filename = filename;
            var Doc = new XmlDocument();

            // Create the root node and xml encoding
            Doc.AppendChild(Doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            XmlNode Root = Doc.CreateElement(XmlNode_Profile);
            Doc.AppendChild(Root);
            _xmlHelper.SetAttribute(Root, Attribute_Version, FormatName);

            // Create the child nodes that live under the Root.
            XmlNode ChannelObjects = _xmlHelper.CreateNode(Doc, Root, XmlNode_ChannelObjects);
            XmlNode Outputs = _xmlHelper.CreateNode(Doc, Root, XmlNode_Outputs);
            XmlNode PlugInData = _xmlHelper.CreateNode(Doc, Root, XmlNode_PlugInData);
            XmlNode Shuffles = _xmlHelper.CreateNode(Doc, Root, XmlNode_Shuffles);
            XmlNode DisabledChannels = _xmlHelper.CreateNode(Doc, Root, XmlNode_DisabledChannels);

            PopulateChannelNodes(ChannelObjects, Outputs, DisabledChannels);

            // Create the PlugIn node for this.
            XmlNode Node = CreatePlugInNode(Doc, true);
            SaveEPData(Node);
            PlugInData.AppendChild(Node);

            // Add in all the other plugins
            int ID = 1;
            if (_loadedPlugIns != null) {
                foreach (GeneralPlugIn gPlugIn in _loadedPlugIns.WhereNot(EP_NAME)) {
                    if (gPlugIn.Name == AP_NAME) {
                        gPlugIn.Enabled = false;
                    }

                    gPlugIn.ID = ID++;
                    Node = AdjustPlugInNode_Channels(gPlugIn.CreateNode(Doc));
                    PlugInData.AppendChild(Node);
                }
            }

            // Save the Shuffles
            SaveShuffles(Shuffles);

            Doc.Save(filename);

            Doc = null;
            ChannelObjects = null;
            Outputs = null;
            PlugInData = null;
            Shuffles = null;
            DisabledChannels = null;
            Root = null;

            return true;
        }


        /// <summary>
        ///     Saves the Profile to the XmlNode passed in.
        /// </summary>
        /// <param name="saveNode">Xml node that holds the Profile data.</param>
        /// <returns>Returns true if the save is successful, false otherwise</returns>
        public virtual bool Save(XmlNode saveNode) {
            SaveEPData(saveNode);
            return true;
        }


        /// <summary>
        ///     Determines whether the file indicated is a valid file format for this type of Profile.
        /// </summary>
        /// <param name="filename">Filename of file containing profile data.</param>
        /// <returns>Returns true if this type of Profile can open this file exactly, false otherwise.</returns>
        public virtual bool ValidateFile(string filename) {
            // At this level, we are just checking broadly to see if this is in the Vixen family of Profiles.
            var Doc = new XmlDocument();
            XmlNode Root = null;

            if (string.Compare(filename.Substring(filename.LastIndexOf(".") + 1), FileExtension, true) != 0) {
                return false;
            }

            try {
                Doc.Load(filename);
                Doc.PreserveWhitespace = true;
            }
            catch {
                return false;
            }
            Root = Doc.DocumentElement;

            // Look for a root node of <Profile>
            if (Root.Name != XmlNode_Root) {
                return false;
            }

            if (Doc.DocumentElement.SelectSingleNode("/Profile/ChannelObjects") == null) {
                return false;
            }

            _detectedTypeName = _xmlHelper.GetAttributeValue(Root, Attribute_ID, string.Empty);

            XmlNode Node = Root.SelectSingleNode(string.Format("{0}/{1}", XmlNode_ChannelObjects, XmlNode_Channel));
            _hasChannelNameInAttribute = (_xmlHelper.GetAttributeValue(Node, Attribute_Name, string.Empty).Length != 0);

            Node = Doc.DocumentElement.SelectSingleNode(string.Format("{0}/{1}", XmlNode_PlugInData, XmlNode_PlugIn));
            _hasOutputAttribute = (_xmlHelper.GetAttributeValue(Node, Attribute_Output, -99) != -99);

            Node = null;
            Root = null;
            Doc = null;
            return true;
        }


        /// <summary>
        ///     Clean up all child objects here, unlink all events and dispose
        /// </summary>
        protected override void DisposeChildObjects() {
            _xmlHelper = null;
            _setupNode = null;
            _loadedPlugIns = null;
            _channels = null;
        }

        #endregion [ Methods ]

        #endregion [ IProfile ]
    }
}

#region [ DEAD CODE ]

///// <summary>
///// Detected the ProfileType of a file if it is one of the Vixen family of Profiles. 
///// </summary>
///// <param name="filename">Name of the file to check.</param>
///// <returns>Returns NotSet if the Profile is not one of the Vixen type of Profiles, else the proper enum
///// for the type of Vixen Profile. If this is an unknown type of Vixen profile, then returns NotSet</returns>
//public static ProfileType DetectedProfileType(string filename)
//{
//	XmlDocument Doc = null;
//	XmlNode Root = null;
//	XmlNode Node = null;
//	FileInfo FI = null;
//	bool FoundName = false;
//	string Text = string.Empty;
//	string GroupFilename = filename.Replace(".pro", ".vgr");
//	string XPath = string.Empty;

//	try
//	{
//		// Try to open the file up as an Xml file.
//		Doc = new XmlDocument();
//		Doc.PreserveWhitespace = true;
//		Doc.Load(filename);
//		Root = Doc.DocumentElement;

//		// Look for a root node of <Profile>
//		if (Root.Name != XmlNode_Root)
//			return ProfileType.NotSet;

//		// Check to see if we've stored the type in an attribute in the Profile node. This might be overwriten 
//		// by the sequencer, but if not, it's a nice shortcut.
//		string SavedType = XmlHelper.GetAttribute(Root, Attribute_ID, string.Empty);
//		if (SavedType.Length > 0)
//		{
//			ProfileType FoundType = EnumHelper.GetValueFromDescription<ProfileType>(SavedType);
//			if (FoundType != ProfileType.NotSet)
//				return FoundType;
//		}

//		// The main way to tell apart these versions is
//		// 2.1.x -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
//		//			PlugInData/Plugin		-> No Output attribute
//		// 2.5.x -	ChannelObjects/Channels -> Channel name in the name attribute
//		//			PlugInData/Plugin		-> type="Output"
//		// Plus -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
//		//			PlugInData/Plugin		-> type="Output"
//		//			There can also be a [ProfileName].vgr file containing channel group information.

//		// See if there is a channel group file in the same directory with the same name as the Profile file.
//		FI = new FileInfo(GroupFilename);
//		//if (FI.Exists)
//		//	return ProfileType.VixenPlus;

//		// Look for the ChannelObjects node
//		XPath = string.Format("{0}/{1}", XmlNode_ChannelObjects, XmlNode_Channel);
//		Node = Root.SelectSingleNode(XPath);
//		if (Node != null)
//		{
//			foreach (XmlNode Channel in Node)
//			{
//				// Determine if the channel name is inside of the inner text of the individual Channel node, or as an attribute.
//				Text = Channel.InnerText.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
//				if (Text.Length != 0)
//				{
//					FoundName = true;
//					break;
//				}
//			}

//			//if (!FoundName)
//			//	return ProfileType.Vixen25x;
//		}

//		// Look at the PlugIn node for ElfPreview or Adjustable preview and determine if there is a type attribute
//		XPath = "{0}/{1}[@{2}='{3}']";

//		Node = Root.SelectSingleNode(string.Format(XPath, XmlNode_PlugInData, XmlNode_PlugIn, Attribute_Name, EP_NAME));
//		if (Node == null)
//			Node = Root.SelectSingleNode(string.Format(XPath, XmlNode_PlugInData, XmlNode_PlugIn, Attribute_Name, AP_NAME));
//		if (Node == null)
//			Node = Root.SelectSingleNode(string.Format("{0}/{1}", XmlNode_PlugInData, XmlNode_PlugIn));
//		//if (Node != null)
//		//{
//		//	if (XmlHelper.GetAttribute(Node, Attribute_Type, string.Empty) == "Output")
//		//		return ProfileType.VixenPlus;
//		//	else
//		//		return ProfileType.Vixen21x;
//		//}

//		return ProfileType.NotSet;
//	}
//	catch (Exception ex)
//	{
//		Workshop.Instance.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
//		return ProfileType.NotSet;
//	}
//	finally
//	{
//		Doc = null;
//		Root = null;
//		Node = null;
//	}
//}

#endregion [ DEAD CODE ]