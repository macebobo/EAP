using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace ElfCore.Profiles.Vixen
{
	/// <summary>
	/// Handles all the profile specific data and properties.
	/// </summary>
	public class BaseVixen : BaseProfile, INotifyPropertyChanged //, IProfile
	{
		
		#region [ Constants ]

		// AP Constants
		private const string AP_NAME = "Adjustable preview";
		private const string BACKGROUND_IMAGE = "BackgroundImage";
		private const string PIXELSIZE = "PixelSize";
		private const string SHOW_GRID_LINES = "ShowGridLines";
		private const string GRID_LINE_WIDTH = "GridLineWidth";
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
		private const string XmlNode_Channels = "Channels";
		private const string XmlNode_DialogPositions = "DialogPositions";
		private const string XmlNode_DisabledChannels = "DisabledChannels";
		private const string XmlNode_Display = "Display";
		private const string XmlNode_Height = "Height";
		private const string XmlNode_Outputs = "Outputs";
		private const string XmlNode_PlugInData = "PlugInData";
		private const string XmlNode_PlayBackDialog = "PreviewDialog";
		private const string XmlNode_Profile = "Profile";
		private const string XmlNode_SortOrder = "SortOrder";
		private const string XmlNode_SortOrders = "SortOrders";
		private const string XmlNode_Width = "Width";

		private const string XmlNode_Cells = "Cells";
		private const string XmlNode_SubChannel = "SubChannel";
		private const string XmlNode_SubChannels = "SubChannels";
		private const string XmlNode_Vector = "Vector";

		private const string Attribute_BorderColor = "borderColor";
		private const string Attribute_Color = "color";
		private const string Attribute_Enabled = "enabled";
		private const string Attribute_ID = "id";
		private const string Attribute_Locked = "locked";
		private const string Attribute_Output = "output";
		private const string Attribute_Point_X = "x";
		private const string Attribute_Point_Y = "y";
		private const string Attribute_Visible = "visible";
		//internal const string XmlNode_RedirectOutputs = "RedirectOutputs";

		internal const string Attribute_Name = "name";
		internal const string XmlNode_PlugIn = "PlugIn";

		private const string XmlNode_ChannelGroup = "ChannelGroup";
		private const string XmlNode_ChannelGroups = "ChannelGroups";

		// Attribute names
		private const string Attribute_Type = "type";
		private const string Attribute_LastSort = "lastSort";

		#endregion [ Constants ]

		#region [ Private Variables ]

		protected XmlHelper _xmlHelper = XmlHelper.Instance;
		protected XmlNode _setupNode = null;
		protected GeneralPlugInList _loadedPlugIns = new GeneralPlugInList();

		#endregion [ Private Variables ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.
		/// </summary>        
		public BaseVixen()
			: base()
		{ }
		
		/// <summary>
		/// Constructor with a profile's filename to open
		/// </summary>
		/// <param name="filename">Name of the Profile file.</param>
		public BaseVixen(string filename)
			: base(filename)
		{ }

		#endregion [ Constructors ]

		#region [ Private Methods ]

		protected virtual void AddGeneralPlugIn(XmlNode node)
		{
			_loadedPlugIns.Add(new GeneralPlugIn(node));
		}

		/// <summary>
		/// Looks at the PlugIn node and checks to see that the Channel range it has is still valid, accounting
		/// for any deleted channels.
		/// </summary>
		/// <param name="node">XmlNode containing PlugIn data.</param>
		/// <returns>Altered XmlNode.</returns>
		private XmlNode AdjustPlugInNode_Channels(XmlNode node)
		{
			if (_xmlHelper.GetAttributeValue(node, Attribute_Name) == EP_NAME)
				return node;
			int ChannelCount = this.ChannelCount;
			int ToChannel = _xmlHelper.GetAttributeValue(node, GeneralPlugIn.Attribute_To, 0) - 1;
			if (ToChannel >= ChannelCount)
				_xmlHelper.SetAttribute(node, GeneralPlugIn.Attribute_To, ChannelCount);
			return node;
		}
		
		/// <summary>
		///  Clears out all the value for the properies and protected virtual variables. Used to initialize the object initially, and when loading new data
		/// </summary>
		protected override void Clear()
		{
			base.Clear();
		}

		/// <summary>
		/// Create the PlugIn node for this.
		/// </summary>
		protected virtual XmlNode CreatePlugInNode(XmlDocument doc)
		{
			throw new NotImplementedException("BaseVixen.CreatePlugInNode");
		}

		/// <summary>
		/// Loads in the Profile data from the Adjustable Preview data node
		/// </summary>
		/// <param name="data">GeneralPlugIn object containing Adjustable Preview data.</param>
		/// <returns>Returns false if the object is null, or the data is empty, true otherwise.</returns>
		private bool LoadAPData(GeneralPlugIn data)
		{
			_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);

			if ((data == null) || (data.PlugInNode == null))
			{
				_workshop.WriteTraceMessage("data|data.PlugInNode = NULL", TraceLevel.Warning);
				_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
				return false;
			}

			int Number = 0;
			string Encoded = string.Empty;
			BaseChannel Channel = null;

			XmlNodeList ChannelList = data.PlugInNode.SelectNodes(XmlNode_Channels + "/" + XmlNode_Channel);
			if ((ChannelList == null) || (ChannelList.Count == 0))
				return false;

			XmlNode Node = data.PlugInNode.SelectSingleNode(XmlNode_Display);
			this.Scaling.ShowGridLines = (_xmlHelper.GetNodeValue(Node, GRID_LINE_WIDTH, "1") == "1");
			this.Scaling.CellSize = _xmlHelper.GetNodeValue(Node, PIXELSIZE, this.Scaling.CellSize.GetValueOrDefault(1));
			this.Scaling.LatticeSize = new Size(_xmlHelper.GetNodeValue(Node, XmlNode_Width, 64),
										_xmlHelper.GetNodeValue(Node, XmlNode_Height, 32));

			this.Background.Brightness = _xmlHelper.GetNodeValue(Node, Background.Property_Brightness, Background.Default_Brightness);

			// Load in the Channel Cell data
			foreach (XmlNode ChannelNode in ChannelList)
			{
				Number = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Number, 0);
				Encoded = ChannelNode.InnerText;

				// Find the Channel that corresponds to this number
				Channel = this.Channels.Get(Number);
				if (Channel != null)
				{
					Channel.DeserializeLattice(Encoded);
					if ((Number < data.FromChannel) || (Number > data.ToChannel))
						Channel.Included = false;
				}
			}
			ChannelList = null;
			Channel = null;

			// Load in the Background data
			this.Background.SuppressEvents = true;
			Encoded = _xmlHelper.GetNodeValue(data.PlugInNode, BACKGROUND_IMAGE, string.Empty);
			if (Encoded.Length > 0)
				this.Background.LoadFromStream(Encoded);

			_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
			return true;
		}

		/// <summary>
		/// Loads in the Profile data from the Elf Preview data node
		/// </summary>
		/// <param name="data">GeneralPlugIn object containing Elf Preview data.</param>
		/// <returns>Returns false if the object is null, or the data is empty, true otherwise.</returns>
		private bool LoadEPData(GeneralPlugIn data)
		{
			_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);

			if ((data == null) || (data.PlugInNode == null))
			{
				_workshop.WriteTraceMessage("data|data.PlugInNode = NULL", TraceLevel.Warning);
				_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
				return false;
			}

			int Number = 0;
			string Encoded = string.Empty;
			BaseChannel Channel = null;

			XmlNodeList ChannelList = data.PlugInNode.SelectNodes(XmlNode_Channels + "/" + XmlNode_Channel);
			if ((ChannelList == null) || (ChannelList.Count == 0))
				return false;

			XmlNode Node = data.PlugInNode.SelectSingleNode(XmlNode_Background);
			if (Node != null)
			{
				this.Background.SuppressEvents = true;
				this.Background.Filename = _xmlHelper.GetNodeValue(Node, Background.Property_Filename, string.Empty);
				this.Background.SaveEncodedImage = _xmlHelper.GetNodeValue(Node, Background.Property_SaveEncodedImage, true);
				this.Background.Color = _xmlHelper.GetNodeValue(Node, Background.Property_Color, Color.Black);
				this.Background.GridColor = _xmlHelper.GetNodeValue(Node, Background.Property_GridColor, Color.Empty);
				this.Background.Brightness = _xmlHelper.GetNodeValue(Node, Background.Property_Brightness, 10);
				this.Background.Saturation = _xmlHelper.GetNodeValue(Node, Background.Property_Saturation, 1f);
				this.Background.Hue = _xmlHelper.GetNodeValue(Node, Background.Property_Hue, 10);
				this.Background.OverlayGrid = _xmlHelper.GetNodeValue(Node, Background.Property_OverlayGrid, false);
				this.Background.Visible = _xmlHelper.GetNodeValue(Node, Background.Property_Visible, true);
				this.Background.WallpaperStyle = EnumHelper.GetEnumFromValue<WallpaperStyle>(_xmlHelper.GetNodeValue(Node, Background.Property_WallpaperStyle, 1));

				if (this.Background.SaveEncodedImage)
				{
					Encoded = _xmlHelper.GetNodeValue(Node, XmlNode_Bitmap);
					if (Encoded.Length > 0)
						this.Background.LoadFromStream(Encoded);
				}
				else
				{
					if (this.Background.Filename.Length > 0)
					{
						FileInfo FI = new FileInfo(this.Background.Filename);
						if (FI.Exists)
							this.Background.Image = ImageHandler.LoadBitmapFromFile(this.Background.Filename);
						else
						{
							this.Background.Image = ImageHandler.CreateErrorMessageImage("Background file \"" + this.Background.Filename + "\" not found.", this.Background.Color, Color.White);
							this.Background.WallpaperStyle = WallpaperStyle.Fill;
						}
					}
				}
				
				this.Background.SuppressEvents = false;
			}
			// </Background>

			// <Scaling>
			// Load in Scaling information
			Node = data.PlugInNode.SelectSingleNode(XmlNode_Scaling);
			this.Scaling = LoadScaling(Node);

			//<Channels>
			foreach (XmlNode ChannelNode in ChannelList)
			{
				Number = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Output, 0);

				// Find the Channel that corresponds to this number
				Channel = this.Channels.Get(Number);
				Channel.Loading = true;

				if ((Number < data.FromChannel) || (Number > data.ToChannel))
					Channel.Included = false;

				Channel.Locked = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Locked, false);
				Channel.Visible = _xmlHelper.GetAttributeValue(ChannelNode, Attribute_Visible, true);
				Channel.RenderColor = _xmlHelper.GetNodeValue(ChannelNode, BaseChannel.Property_RenderColor, Color.Empty);
				Channel.BorderColor = _xmlHelper.GetNodeValue(ChannelNode, BaseChannel.Property_BorderColor, Color.Empty);
				
				Node = ChannelNode.SelectSingleNode(BaseChannel.Property_Origin);
				if (Node != null)
					Channel.Origin = new Point(_xmlHelper.GetAttributeValue(Node, Attribute_Point_X, 0),
											   _xmlHelper.GetAttributeValue(Node, Attribute_Point_Y, 0));
				
				Encoded = _xmlHelper.GetNodeValue(ChannelNode, XmlNode_Cells, string.Empty);
				if (Encoded.Length > 0)
					Channel.DeserializeLattice(Encoded);
				
				Encoded = _xmlHelper.GetNodeValue(ChannelNode, XmlNode_Vector, string.Empty);
				if (Encoded.Length > 0)
					Channel.DeserializeVector(Encoded);

				Channel.Loading = false;
			}
			//</Channels>

			//<ChannelGroups>
			ChannelGroup Group = null;
			XmlNodeList GroupList = data.PlugInNode.SelectNodes("ChannelGroups");
			if (GroupList != null)
				foreach (XmlNode GroupNode in GroupList)
				{
					Group = new ChannelGroup();
					Group.ID = _xmlHelper.GetAttributeValue(GroupNode, Attribute_ID, 0);
					Group.Name = _xmlHelper.GetAttributeValue(GroupNode, Attribute_Name, string.Empty);
					Group.SerializedList = GroupNode.InnerText;
					this.Channels.Groups.List.Add(Group);
				}
			//</ChannelGroups>

			_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
			return true;
		}

		/// <summary>
		/// Creates a new Channel object, loading from the topmost Channel node in the Profile xml.
		/// </summary>
		/// <param name="node">Xml node containing initial Channel data.</param>
		/// <returns>Channel object.</returns>
		protected virtual BaseChannel LoadChannelFromXml(XmlNode node)
		{
			BaseChannel Channel = new BaseChannel();
			Channel.Loading = true;
			Channel.Profile = this;

			string Value = _xmlHelper.GetAttributeValue(node, Attribute_Color, Color.White.ToArgb().ToString());
			Channel.SequencerColor = Color.FromArgb(Convert.ToInt32(Value));

			Value = _xmlHelper.GetAttributeValue(node, Attribute_Output, "0");
			Channel.ID = Convert.ToInt32(Value);

			Value = _xmlHelper.GetAttributeValue(node, BaseChannel.Property_Enabled.ToLower());
			Channel.Enabled = Convert.ToBoolean(Value);

			// Channel.Name loaded from one of the derived classes.
			return Channel;
		}

		/// <summary>
		/// Populate the ChannelObjects, Outputs and DisableChannels nodes of the Profile xml document.
		/// </summary>
		private void PopulateChannelNodes(XmlNode channelObjectsNode, XmlNode outputsNode, XmlNode disableChannelsNode)
		{
			XmlNode Node = null;
			XmlDocument Doc = channelObjectsNode.OwnerDocument;
			string OutputIDs = string.Empty;
			string DisabledIDs = string.Empty;

			channelObjectsNode.RemoveAll();

			foreach (BaseChannel Channel in this.Channels)
			{
				Node = Doc.CreateElement(XmlNode_Channel);
				channelObjectsNode.AppendChild(Node);
				_xmlHelper.AddAttribute(Node, Attribute_Color, Channel.SequencerColor.ToArgb().ToString());
				_xmlHelper.AddAttribute(Node, Attribute_Output, Channel.ID.ToString());
				_xmlHelper.AddAttribute(Node, Attribute_ID, DateTime.Now.Ticks.ToString());
				_xmlHelper.AddAttribute(Node, Attribute_Enabled, Channel.Enabled.ToString());
				SaveChannelName(Node, Channel);
				OutputIDs += ((OutputIDs.Length > 0) ? "," : string.Empty) + Channel.ID;
				if (!Channel.Enabled)
					DisabledIDs += ((DisabledIDs.Length > 0) ? "," : string.Empty) + Channel.ID;
			}
			outputsNode.InnerText = OutputIDs;
			disableChannelsNode.InnerText = DisabledIDs;

			Doc = null;
			Node = null;
		}

		/// <summary>
		/// Clears out and Populate the SortOrders node of the Profile xml.
		/// </summary>
		private void PopulateSortOrders(XmlNode sortOrders)
		{
			string List = string.Empty;
			XmlNode Node = null;

			if (sortOrders == null)
				return;
			sortOrders.RemoveAll();

			int SelectedIndex = Channels.ShuffleController.ActiveIndex;
			if (SelectedIndex == 0)
				SelectedIndex = -1;
			_xmlHelper.SetAttribute(sortOrders, Attribute_LastSort, SelectedIndex);

			foreach (Shuffle Item in Channels.ShuffleController.All)
			{
				if (Item.IsNativeShuffle)
					continue;
				Node = sortOrders.OwnerDocument.CreateElement(XmlNode_SortOrder);
				_xmlHelper.AddAttribute(Node, Property_Name.ToLower(), Item.Name);
				Node.InnerText = Item.SerializedList;
				sortOrders.AppendChild(Node);
			}
			Node = null;
		}

		/// <summary>
		/// Saves the Channel Name. This method must be overwritten by the class that inherits this class.
		/// </summary>
		protected virtual void SaveChannelName(XmlNode node, BaseChannel channel)
		{
			throw new NotImplementedException("SaveChannelName");
		}

		/// <summary>
		/// Saves the data to the Profile Xml that is specific to this PlugIn
		/// </summary>
		/// <param name="data">GeneralPlugIn object containing Elf Preview data.</param>
		private void SaveEPData(XmlNode plugInNode)
		{
			if (plugInNode == null)
				return;

			XmlDocument Doc = plugInNode.OwnerDocument;
			XmlNode Node = null;
			XmlNode ParentNode = null;

			#region [ Background ]

			ParentNode = plugInNode.SelectSingleNode(XmlNode_Background);
			if (ParentNode == null)
				ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Background);
			ParentNode.RemoveAll();
			
			if (this.Background.Filename.Length > 0)
				_xmlHelper.SetValue(ParentNode, Background.Property_Filename, this.Background.Filename);

			if (!this.Background.Color.IsEmpty)
				_xmlHelper.SetValue(ParentNode, Background.Property_Color, this.Background.Color);

			if (!this.Background.GridColor.IsEmpty)
				_xmlHelper.SetValue(ParentNode, Background.Property_GridColor, this.Background.GridColor);

			_xmlHelper.SetValue(ParentNode, Background.Property_Brightness, this.Background.Brightness);
			_xmlHelper.SetValue(ParentNode, Background.Property_Saturation, this.Background.Saturation);
			_xmlHelper.SetValue(ParentNode, Background.Property_Hue, this.Background.Hue);
			_xmlHelper.SetValue(ParentNode, Background.Property_OverlayGrid, this.Background.OverlayGrid);
			_xmlHelper.SetValue(ParentNode, Background.Property_Visible, this.Background.Visible);
			_xmlHelper.SetValue(ParentNode, Background.Property_WallpaperStyle, (int)this.Background.WallpaperStyle);
			_xmlHelper.SetValue(ParentNode, Background.Property_SaveEncodedImage, this.Background.SaveEncodedImage);

			if (this.Background.SaveEncodedImage)
			{
				string Encoded = string.Empty;
				if (this.Background.Image != null)
				{
					MemoryStream ms = new MemoryStream();
					this.Background.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					Encoded = Convert.ToBase64String(ms.ToArray());
					ms.Dispose();
				}
				if (Encoded.Length > 0)
					_xmlHelper.SetValue(ParentNode, XmlNode_Bitmap, Encoded);
			}
			ParentNode = null;

			#endregion [ Background ]

			#region  [ Scaling ]

			ParentNode = plugInNode.SelectSingleNode(XmlNode_Scaling);
			if (ParentNode == null)
				ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Scaling);
			ParentNode.RemoveAll();

			_xmlHelper.SetValue(ParentNode, XmlNode_Height, this.Scaling.LatticeSize.Height);
			_xmlHelper.SetValue(ParentNode, XmlNode_Width, this.Scaling.LatticeSize.Width);
			_xmlHelper.SetValue(ParentNode, Property_CellSize, this.Scaling.CellSize.GetValueOrDefault(1));
			_xmlHelper.SetValue(ParentNode, Property_ShowGridLines, this.Scaling.ShowGridLines.GetValueOrDefault(true));
			_xmlHelper.SetValue(ParentNode, Property_Zoom, this.Scaling.Zoom.GetValueOrDefault(1));

			ParentNode = null;

			#endregion  [ Scaling ]

			#region [ Channels ]

			ParentNode = plugInNode.SelectSingleNode(XmlNode_Channels);
			if (ParentNode == null)
				ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_Channels);
			ParentNode.RemoveAll();

			XmlNode OriginNode = null;
			foreach (BaseChannel Channel in this.Channels.GetAllChannels())
			{
				Node = Doc.CreateElement(XmlNode_Channel);
				ParentNode.AppendChild(Node);

				_xmlHelper.SetAttribute(Node, Attribute_Output, Channel.ID);
				_xmlHelper.SetAttribute(Node, Attribute_Name, Channel.Name);
				if (Channel.Locked)
					_xmlHelper.SetAttribute(Node, Attribute_Locked, Channel.Locked);
				if (!Channel.Visible)
				_xmlHelper.SetAttribute(Node, Attribute_Visible, Channel.Visible);

				if (!Channel.Origin.IsEmpty)
				{
					OriginNode = _xmlHelper.CreateNode(Doc, Node, BaseChannel.Property_Origin);
					_xmlHelper.SetAttribute(OriginNode, Attribute_Point_X, Channel.Origin.X);
					_xmlHelper.SetAttribute(OriginNode, Attribute_Point_Y, Channel.Origin.X);
				}

				if (!Channel.RenderColor.IsEmpty)
					_xmlHelper.SetValue(Node, BaseChannel.Property_RenderColor, Channel.RenderColor);
				if (!Channel.BorderColor.IsEmpty)
					_xmlHelper.SetValue(Node, BaseChannel.Property_BorderColor, Channel.BorderColor);
				if (Channel.HasLatticeData)
					_xmlHelper.SetValue(Node, XmlNode_Cells, Channel.SerializeLattice(dedupeData: true, flatten: false));
				if (Channel.HasVectorData)
					_xmlHelper.SetValue(Node, XmlNode_Vector, Channel.SerializeVector());
			}
			OriginNode = null;
			ParentNode = null;

			#endregion [ Channels ]

			#region [ ChannelGroups ]

			ParentNode = _xmlHelper.CreateNode(Doc, plugInNode, XmlNode_ChannelGroups);
			foreach (ChannelGroup Group in this.Channels.Groups.List)
			{
				Node = _xmlHelper.CreateNode(Doc, ParentNode, XmlNode_ChannelGroup);
				Node.InnerText = Group.SerializedList;
				_xmlHelper.SetAttribute(Node, Attribute_Name, Group.Name);
				_xmlHelper.SetAttribute(Node, Attribute_ID, Group.ID.ToString());
			}
			ParentNode = null;

			#endregion [ ChannelGroups ]
		}
		
		#endregion [ Private Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Loads in the Profile data from the file passed in.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public override bool Load(string filename)
		{
			_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);

			if ((filename ?? string.Empty).Length == 0)
				throw new ArgumentException("Missing filename");

			FileInfo FI = new FileInfo(filename);
			if (!FI.Exists)
				throw new FileNotFoundException("File not found.", filename);

			string Name = FI.Name;
			FI = null;

			XmlDocument Doc = new XmlDocument();
			Doc.Load(filename);
			if (Name.Contains("."))
				Name = Name.Substring(0, Name.LastIndexOf('.'));
			this.Name = Name;
			this.Filename = filename;

			if (this.Load(Doc))
			{
				_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
				return true;
			}
			else
			{
				this.Name = string.Empty;
				this.Filename = string.Empty;
				_workshop.WriteTraceMessage("Load(XmlDocument) return FALSE", TraceLevel.Warning);
				_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
				return false;
			}
		}

		/// <summary>
		/// Loads in the profile data coming from Vixen.
		/// </summary>
		/// <param name="setupNode">XmlNode containing the plugin data</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public override bool Load(XmlNode setupNode, List<Channels.Properties> rawChannels)
		{
			if (setupNode == null)
				throw new ArgumentNullException("SetupData is null.");
			_setupNode = setupNode;
			return this.Load(setupNode.OwnerDocument, rawChannels);
		}

		/// <summary>
		/// Loads in the profile that is already loaded into the XmlDocument object.
		/// </summary>
		/// <param name="doc">XmlDocument object that contains the Profile.</param>
		private bool Load(XmlDocument doc, List<Channels.Properties> rawChannels)
		{
			BaseChannel Channel = null;
			XmlNode Root = null;
			XmlNode Node = null;
			XmlNodeList NodeList = null;
			bool SaveSuppressEvents = this.SuppressEvents;
			string NativeShuffle = string.Empty;

			this.SuppressEvents = true;

			Clear();
			Root = doc.DocumentElement;

			if ((rawChannels == null) || (rawChannels.Count == 0))
			{
				// Profile/ChannelObjects/Channel
				// Load the Channel data. Channels are stored in 2 spots in a Vixen 2.x Profile. 
				// Once at the top, used by Vixen itself, and also under PlugInData, used by the output PlugIn.
				NodeList = Root.SelectNodes(XmlNode_ChannelObjects + "/" + XmlNode_Channel);
				if (NodeList != null)
				{
					foreach (XmlNode ChannelNode in NodeList)
					{
						Channel = LoadChannelFromXml(ChannelNode);
						NativeShuffle += ((NativeShuffle.Length > 0) ? "," : string.Empty) + Channel.ID;
						_channelController.Add(Channel);
						Channel.Index = _channelController.Count - 1;
					}
				}
			}
			else
			{
				foreach (Channels.Properties Item in rawChannels)
				{
					Channel = new BaseChannel(Item);
					NativeShuffle += ((NativeShuffle.Length > 0) ? "," : string.Empty) + Channel.ID;
					_channelController.Add(Channel);
					Channel.Index = _channelController.Count - 1;
				}
			}

			// Profile/SortOrders
			this.Channels.ShuffleController.SuppressEvents = true;
			Node = Root.SelectSingleNode(XmlNode_SortOrders);
			if (Node != null)
			{
				int LastSort = _xmlHelper.GetAttributeValue(Node, Attribute_LastSort, 0);

				NodeList = Node.SelectNodes(XmlNode_SortOrder);
				foreach (XmlNode ShuffleNode in NodeList)
				{
					this.Channels.ShuffleController.Add(new Shuffle(_xmlHelper.GetAttributeValue(ShuffleNode, Attribute_Name), ShuffleNode.InnerText));
				}
				this.Channels.ShuffleController.SuppressEvents = false;
				if (LastSort >= 0)
					this.Channels.ShuffleController.ActiveIndex = LastSort;
			}

			NodeList = Root.SelectNodes("//" + XmlNode_PlugIn);
			if (NodeList != null)
			{
				foreach (XmlNode PlugInNode in NodeList)
				{
					_loadedPlugIns.Add(new GeneralPlugIn(PlugInNode));
				}
			}

			bool Loaded = true;
			if (_loadedPlugIns.Count > 0)
			{
				if (!LoadEPData(_loadedPlugIns.Where(EP_NAME)))
				{
					if (!LoadAPData(_loadedPlugIns.Where(AP_NAME)))
						Loaded = false;
				}
			}
			else
			{ 
				if (!LoadEPData(new GeneralPlugIn(Root)))
					Loaded = false;
			}

			if (!Loaded)
			{
				// Neither Adjustable preview nor Elf data has been found.
				// Initialize with default settings.
				this.Scaling.CellSize = 7;
				this.Scaling.LatticeSize = new Size(64, 32);
				this.Scaling.ShowGridLines = true;
				this.Background.Brightness = 10;
			}

			Root = null;
			Node = null;
			NodeList = null;
			Channel = null;

			Channels.Active = Channels[0];
			InitializeUndo();

			_channelController.SetLoading(false);
			this.SuppressEvents = SaveSuppressEvents;
			OnLoaded();

			return true;
		}
		
		/// <summary>
		/// Loads in the profile that is already loaded into the XmlDocument object.
		/// </summary>
		/// <param name="doc">XmlDocument object that contains the Profile.</param>
		private bool Load(XmlDocument doc)
		{
			return Load(doc, null);
		}

		/// <summary>
		/// Used to load in Vixen Channels in the event of a brand new Profile, where there
		/// is no channel data in the XmlDocument at all.
		/// </summary>
		/// <param name="rawChannelList">List of Channel Properties, used to populate the Channels</param>
		private void LoadChannelData(List<Channels.Properties> rawChannelList)
		{
			this.SuppressEvents = true;
			_channelController.SetLoading(true);

			foreach (Channels.Properties Raw in rawChannelList)
			{
				_channelController.Add(new BaseChannel() 
				{ 
					Name = Raw.Name,
					SequencerColor = Raw.SequencerColor,
					Enabled = Raw.Enabled
				});
			}

			if (Channels.Count > 0)
			{
				Channels.Active = Channels[0];
				InitializeUndo();
			}

			_channelController.SetLoading(false);
			this.SuppressEvents = false;
		}

		/// <summary>
		/// Saves the current Profile changes. 
		/// If we are editing a file, then will save to that file name.
		/// If we are editing from an Xml node, save changes back to the parent XmlDocument.
		/// </summary>
		public override bool Save()
		{
			Cursor LastCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if (_setupNode != null)
					return Save(_setupNode);
				else if (_filename.Length > 0)
					return Save(_filename);
				else
					return false;
			}
			catch
			{
				throw;
			}
			finally
			{
				this.Cursor = LastCursor;
			}
		}

		/// <summary>
		/// Saves the Profile to file using the filename passed in
		/// </summary>
		/// <param name="filename">Filename to use to save the file to</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public override bool Save(string filename)
		{
			_workshop.WriteTraceMessage("BEGIN", TraceLevel.Verbose);
			_workshop.WriteTraceMessage("filename: " + filename ?? string.Empty, TraceLevel.Info);
			
			if ((filename ?? string.Empty).Length == 0)
				throw new Exception("Missing filename");

			this.Filename = filename;
			XmlDocument Doc = new XmlDocument();
			
			// Create the root node and xml encoding
			Doc.AppendChild(Doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
			XmlNode Root = Doc.CreateElement(XmlNode_Profile);
			Doc.AppendChild(Root);
			_xmlHelper.SetAttribute(Root, Attribute_Name, EnumHelper.GetEnumDescription(this.ProfileTypeID));

			// Create the child nodes that live under the Root.
			XmlNode ChannelObjects = _xmlHelper.CreateNode(Doc, Root, XmlNode_ChannelObjects);
			XmlNode Outputs = _xmlHelper.CreateNode(Doc, Root, XmlNode_Outputs);
			XmlNode PlugInData = _xmlHelper.CreateNode(Doc, Root, XmlNode_PlugInData);
			XmlNode SortOrders = _xmlHelper.CreateNode(Doc, Root, XmlNode_SortOrders);
			XmlNode DisabledChannels = _xmlHelper.CreateNode(Doc, Root, XmlNode_DisabledChannels);

			PopulateChannelNodes(ChannelObjects, Outputs, DisabledChannels);

			// Create the PlugIn node for this.
			XmlNode Node = CreatePlugInNode(Doc);
			SaveEPData(Node);
			PlugInData.AppendChild(Node);

			// Add in all the other plugins
			int ID = 1;
			if (_loadedPlugIns != null)
			{
				foreach (GeneralPlugIn gPlugIn in _loadedPlugIns.WhereNot(EP_NAME))
				{
					if (gPlugIn.Name == AP_NAME)
						continue;
					gPlugIn.ID = ID++;
					Node = AdjustPlugInNode_Channels(gPlugIn.CreateNode(Doc));
					PlugInData.AppendChild(Node);
				}
			}

			// Save the SortOrders
			PopulateSortOrders(SortOrders);

			Doc.Save(filename);

			Doc = null;
			ChannelObjects = null;
			Outputs = null;
			PlugInData = null;
			SortOrders = null;
			DisabledChannels = null;
			Root = null;

			this.SetClean();

			_workshop.WriteTraceMessage("END", TraceLevel.Verbose);
			return true;
		}

		/// <summary>
		/// Saves the Profile to the XmlNode passed in.
		/// </summary>
		/// <param name="saveNode">Xml node that holds the Profile data.</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public override bool Save(XmlNode saveNode)
		{
			SaveEPData(saveNode);
			return true;
		}
		
		#endregion [ Public Methods ]

		#region [ Public Static Methods ]

		/// <summary>
		/// Detected the ProfileType of a file if it is one of the Vixen family of Profiles. 
		/// </summary>
		/// <param name="filename">Name of the file to check.</param>
		/// <returns>Returns NotSet if the Profile is not one of the Vixen type of Profiles, else the proper enum
		/// for the type of Vixen Profile. If this is an unknown type of Vixen profile, then returns NotSet</returns>
		public static ProfileType DetectedProfileType(string filename)
		{
			XmlDocument Doc = null;
			XmlNode Root = null;
			XmlNode Node = null;
			FileInfo FI = null;
			bool FoundName = false;
			string Text = string.Empty;
			string GroupFilename = filename.Replace(".pro", ".vgr");
			string XPath = string.Empty;

			try
			{
				// Try to open the file up as an Xml file.
				Doc = new XmlDocument();
				Doc.PreserveWhitespace = true;
				Doc.Load(filename);
				Root = Doc.DocumentElement;

				// Look for a root node of <Profile>
				if (Root.Name != XmlNode_Root)
					return ProfileType.NotSet;

				// Check to see if we've stored the type in an attribute in the Profile node. This might be overwriten 
				// by the sequencer, but if not, it's a nice shortcut.
				string SavedType = XmlHelper.GetAttribute(Root, Attribute_ID, string.Empty);
				if (SavedType.Length > 0)
				{
					ProfileType FoundType = EnumHelper.GetValueFromDescription<ProfileType>(SavedType);
					if (FoundType != ProfileType.NotSet)
						return FoundType;
				}

				// The main way to tell apart these versions is
				// 2.1.x -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
				//			PlugInData/Plugin		-> No Output attribute
				// 2.5.x -	ChannelObjects/Channels -> Channel name in the name attribute
				//			PlugInData/Plugin		-> type="Output"
				// Plus -	ChannelObjects/Channels -> Channel name written in the InnerText of the node
				//			PlugInData/Plugin		-> type="Output"
				//			There can also be a [ProfileName].vgr file containing channel group information.

				// See if there is a channel group file in the same directory with the same name as the Profile file.
				FI = new FileInfo(GroupFilename);
				if (FI.Exists)
					return ProfileType.VixenPlus;

				// Look for the ChannelObjects node
				XPath = string.Format("{0}/{1}", XmlNode_ChannelObjects, XmlNode_Channel);
				Node = Root.SelectSingleNode(XPath);
				if (Node != null)
				{
					foreach (XmlNode Channel in Node)
					{
						// Determine if the channel name is inside of the inner text of the individual Channel node, or as an attribute.
						Text = Channel.InnerText.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
						if (Text.Length != 0)
						{
							FoundName = true;
							break;
						}
					}

					if (!FoundName)
						return ProfileType.Vixen25x;
				}

				// Look at the PlugIn node for ElfPreview or Adjustable preview and determine if there is a type attribute
				XPath = "{0}/{1}[@{2}='{3}']";

				Node = Root.SelectSingleNode(string.Format(XPath, XmlNode_PlugInData, XmlNode_PlugIn, Attribute_Name, EP_NAME));
				if (Node == null)
					Node = Root.SelectSingleNode(string.Format(XPath, XmlNode_PlugInData, XmlNode_PlugIn, Attribute_Name, AP_NAME));
				if (Node == null)
					Node = Root.SelectSingleNode(string.Format("{0}/{1}", XmlNode_PlugInData, XmlNode_PlugIn));
				if (Node != null)
				{
					if (XmlHelper.GetAttribute(Node, Attribute_Type, string.Empty) == "Output")
						return ProfileType.VixenPlus;
					else
						return ProfileType.Vixen21x;
				}

				return ProfileType.NotSet;
			}
			catch (Exception ex)
			{
				Workshop.Instance.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
				return ProfileType.NotSet;
			}
			finally
			{
				Doc = null;
				Root = null;
				Node = null;
			}
		}

		/// <summary>
		/// Load Scaling information out of the XmlNode
		/// </summary>
		/// <param name="node">XmlNode containing Scaling information</param>
		/// <returns>Returns a Scaling object, populated from the data in the XmlNode</returns>
		public static ElfCore.Core.Scaling LoadScaling(XmlNode node)
		{
			ElfCore.Core.Scaling Scaling = new Scaling();
			ElfCore.Util.XmlHelper XmlHelper = XmlHelper.Instance;

			if (node != null)
			{
				Scaling.SuppressEvents = true;
				Scaling.LatticeSize = new Size(XmlHelper.GetNodeValue(node, XmlNode_Width, 64),
											   XmlHelper.GetNodeValue(node, XmlNode_Height, 32));
				Scaling.CellSize = XmlHelper.GetNodeValue(node, Property_CellSize, 7);
				Scaling.ShowGridLines = XmlHelper.GetNodeValue(node, Property_ShowGridLines, true);
				Scaling.Zoom = XmlHelper.GetNodeValue(node, Property_Zoom, 1f);
				Scaling.SuppressEvents = false;
			}
			XmlHelper = null;
			return Scaling;
		}

		/// <summary>
		/// Load Background information out of the XmlNode
		/// </summary>
		/// <param name="node">XmlNode containing Background information</param>
		/// <returns>Returns a Background object, populated from the data in the XmlNode</returns>
		public static ElfCore.Core.Background LoadBackground(XmlNode node)
		{
			Background Background = new Background();
			ElfCore.Util.XmlHelper XmlHelper = XmlHelper.Instance;

			if (node != null)
			{
				Background.SuppressEvents = true;
				Background.Filename = XmlHelper.GetNodeValue(node, Background.Property_Filename, string.Empty);
				Background.SaveEncodedImage = XmlHelper.GetNodeValue(node, Background.Property_SaveEncodedImage, true);
				Background.Color = XmlHelper.GetNodeValue(node, Background.Property_Color, Color.Black);
				Background.GridColor = XmlHelper.GetNodeValue(node, Background.Property_GridColor, Color.Empty);
				Background.Brightness = XmlHelper.GetNodeValue(node, Background.Property_Brightness, 10);
				Background.Saturation = XmlHelper.GetNodeValue(node, Background.Property_Saturation, 1f);
				Background.Hue = XmlHelper.GetNodeValue(node, Background.Property_Hue, 10);
				Background.OverlayGrid = XmlHelper.GetNodeValue(node, Background.Property_OverlayGrid, false);
				Background.Visible = XmlHelper.GetNodeValue(node, Background.Property_Visible, true);
				Background.WallpaperStyle = EnumHelper.GetEnumFromValue<WallpaperStyle>(XmlHelper.GetNodeValue(node, Background.Property_WallpaperStyle, 1));

				if (Background.SaveEncodedImage)
				{
					string Encoded = XmlHelper.GetNodeValue(node, XmlNode_Bitmap);
					if (Encoded.Length > 0)
						Background.LoadFromStream(Encoded);
				}
				else
				{
					if (Background.Filename.Length > 0)
					{
						FileInfo FI = new FileInfo(Background.Filename);
						if (FI.Exists)
							Background.Image = ImageHandler.LoadBitmapFromFile(Background.Filename);
						else
						{
							Background.Image = ImageHandler.CreateErrorMessageImage("Background file \"" + Background.Filename + "\" not found.", Background.Color, Color.White);
							Background.WallpaperStyle = WallpaperStyle.Fill;
						}
					}
				}
			}

			XmlHelper = null;
			return Background;
		}
		
		#endregion [ Public Static Methods ]

	}

}

