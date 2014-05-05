#define VIXEN_VERSION_21
//#define VIXEN_VERSION_25

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ElfCore;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Util;
using Vixen;

namespace AdjustablePreview
{
#if VIXEN_VERSION_21 && VIXEN_VERSION_25
#error Both Versions 2.1 and 2.5 are specified
#error Check VIXEN_VERSION_21 and VIXEN_VERSION_25 defs
#elif !VIXEN_VERSION_21 && !VIXEN_VERSION_25
#error No Vixen Version defined
#endif

	public class Preview : IEventDrivenOutputPlugIn
	{
		#region [ Constants ]

		public const string PLUGIN_NAME = "Adjustable preview";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private static string _traceFilename = string.Empty;
		private static TextWriterTraceListener _textListener;
		private PreviewDialog _previewDialog;
		private List<Vixen.Channel> _vixenChannels;
		private SetupData _setupData;
		private XmlNode _setupNode;
		private List<Form> _dialogList;
		private int _startChannel; // index of first Channel of data range sent
		private int _eventPeriod;
		private TraceSwitch _traceSwitch = new TraceSwitch("TraceLevelSwitch", "Switch in config file");
		private string _profileFileName = string.Empty;

		private static StringBuilder _tracing = new StringBuilder();

		private AppearanceController _appearance = AppearanceController.Instance;
		private XmlHelper _xmlHelper = XmlHelper.Instance;
		private Workshop _workshop = null;
		private Settings _settings = Settings.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Does not use any hardware
		/// </summary>
		public HardwareMap[] HardwareMap
		{
			get { return new HardwareMap[] { }; }
		}

		/// <summary>
		/// Name of the PlugIn Author(s)
		/// </summary>
		public string Author
		{
			get { return "K.C. Oaks/Rob Anderson"; }
		}
		
		/// <summary>
		/// Description of the PlugIn
		/// </summary>
		public string Description
		{
			get { return "Adjustable sequence preview plugin for Vixen (with enhanced editor)"; }
		}

		/// <summary>
		/// Public Name of the PlugIn
		/// </summary>
		public string Name
		{
			get { return PLUGIN_NAME; }
		}

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden()]
		private ElfCore.Profiles.BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Preview()
		{
			try
			{
				_workshop = Workshop.Instance;
				Workshop.RunMode = RunMode.PlugIn;
				_workshop.Initialize();

				_traceFilename = Path.Combine(Workshop.GetProfilePath(), "AdjustablePreview.log");
				//_traceFile = File.Exists(_traceFilename) ? File.Open(_traceFilename, FileMode.Append) : File.Create(_traceFilename);
				//_textListener = new TextWriterTraceListener(_traceFile);
				_textListener = new TextWriterTraceListener(_traceFilename);
				Trace.Listeners.Add(_textListener);

				WriteTraceMessage("------------------------------------------------------------------------" + Environment.NewLine, TraceLevel.Error);
				WriteTraceMessage(DateTime.Now.ToString(), TraceLevel.Error);
				WriteTraceMessage("Constructor Start", TraceLevel.Verbose);

				_vixenChannels = new List<Vixen.Channel>();
				_dialogList = new List<Form>();

				WriteTraceMessage(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), TraceLevel.Verbose);

				_settings.Style = Settings.SettingsStyle.Xml;

				//_settings = new Settings(SettingsStyle.Xml);

				//WriteTraceMessage("GetAllTools Begin", TraceLevel.Verbose);
				//AdjustablePreview.PlugIn.ToolHostProvider.GetAllTools();
				//WriteTraceMessage("GetAllTools End", TraceLevel.Verbose);
			}
			catch (Exception ex)
			{
				WriteTraceMessage(ex.Message, TraceLevel.Verbose);
				WriteTraceMessage(ex.ToString(), TraceLevel.Verbose);
				WriteTraceMessage(ex.StackTrace, TraceLevel.Verbose);
			}
			finally
			{
				WriteTraceMessage("Constructor End", TraceLevel.Verbose);
			}
		}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~Preview()
		{
			Trace.Listeners.Remove(_textListener);
			if (_textListener != null)
			{
				//_textListener.Dispose();
				_textListener = null;
			}
		}

		#endregion [ Destructors ]

		#region [ Methods ]

		public void Setup()
		{
			ElfCore.Forms.Editor EditorForm = null;

			WriteTraceMessage("Setup Start", TraceLevel.Verbose);

			if ((_vixenChannels == null) || (_vixenChannels.Count == 0))
			{
				MessageBox.Show("The item you are trying to create a preview for has no Channels.", this.Name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			//Workshop = LoadData();
			LoadData();
			
			EditorForm = new ElfCore.Forms.Editor();
			//_workshop.Profile_Filename = _profileFileName;
			_workshop.Dirty = false;

			if (EditorForm.ShowDialog() == DialogResult.Yes)
				Save(_setupData, _setupNode);

			EditorForm.Dispose();
			EditorForm = null;

			WriteTraceMessage("Setup End", TraceLevel.Verbose);
		}

		public void Initialize(IExecutable executableObject, SetupData setupData, XmlNode setupNode)
		{
			WriteTraceMessage("Initialize Start", TraceLevel.Verbose);

			_vixenChannels.Clear();
			_vixenChannels.AddRange(executableObject.Channels);
			_setupData = setupData;
			_setupNode = setupNode;

			try
			{
				_eventPeriod = ((Vixen.EventSequence)executableObject).EventPeriod;
				XmlNode Node = setupNode.OwnerDocument.CreateElement("EventPeriod");
				Node.InnerText = _eventPeriod.ToString();
				_setupNode.AppendChild(Node);

				_profileFileName = ((Vixen.EventSequence)executableObject).FileName as string ?? string.Empty;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}

			try
			{
				if (_profileFileName.Length == 0)
					_profileFileName = executableObject.FileName as string ?? string.Empty;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			
			// They specified in 1-base, we use 0-base
			_startChannel = Convert.ToInt32(_setupNode.Attributes["from"].Value) - 1;

			// Create an empty image, if there is none
			_setupData.GetBytes(_setupNode, "BackgroundImage", new byte[0]);

			WriteTraceMessage("Initialize End", TraceLevel.Verbose);
		}

		#if VIXEN_VERSION_21
		public List<Form> Startup()
#else
        public void Startup()
#endif
		{
			WriteTraceMessage("Startup Start", TraceLevel.Verbose);

			try
			{

#if VIXEN_VERSION_21
				if ((_vixenChannels == null) || (_vixenChannels.Count == 0))
				{
					WriteTraceMessage("Startup No Channels", TraceLevel.Verbose);
					return _dialogList;
				}
#endif

				ISystem SystemInterface = (ISystem)Interfaces.Available["ISystem"];
				System.Reflection.ConstructorInfo ci = typeof(PreviewDialog).GetConstructor(new Type[] { typeof(XmlNode), typeof(List<Vixen.Channel>), typeof(int) });
				_previewDialog = (PreviewDialog)SystemInterface.InstantiateForm(ci, _setupNode, _vixenChannels, _startChannel);

#if VIXEN_VERSION_21
				return _dialogList;
#endif
			}
			catch (Exception ex)
			{
				WriteTraceMessage(ex.StackTrace, TraceLevel.Verbose);
#if VIXEN_VERSION_21
				return null;
#endif
			}
			finally
			{
				WriteTraceMessage("Startup End", TraceLevel.Verbose);
			}
		}

		public void Shutdown()
		{
			WriteTraceMessage("Shutdown Start", TraceLevel.Verbose);

			if (_previewDialog != null)
			{
				if (_previewDialog.InvokeRequired)
				{
					_previewDialog.Invoke((MethodInvoker)delegate()
					{
						_previewDialog.Close();
						_previewDialog.Dispose();
					});
				}
				else
				{
					_previewDialog.Close();
					_previewDialog.Dispose();
				}
				_previewDialog = null;
				GC.Collect();
			}

			try
			{
				WriteTraceMessage("_Channels.Clear() TRY", TraceLevel.Verbose);
				_vixenChannels.Clear();
			}
			catch
			{
				WriteTraceMessage("_Channels.Clear() CATCH", TraceLevel.Verbose);
			}
			WriteTraceMessage("_Channels.Clear() END TRY", TraceLevel.Verbose);

			_setupData = null;
			_setupNode = null;

			WriteTraceMessage("Shutdown End", TraceLevel.Verbose);
		}

		/// <summary>
		/// Called during playback, indicating the alpha values of the various channels
		/// </summary>
		/// <param name="ChannelValues">Byte array, containing the alpha values of the Channels</param>
		public void Event(byte[] ChannelValues)
		{
			if (_previewDialog == null || _previewDialog.Disposing || _previewDialog.IsDisposed)
				return;

			_previewDialog.UpdateWith(ChannelValues);
		}

		/// <summary>
		/// Returns this PlugIn's public name
		/// </summary>
		public override string ToString()
		{
			return Name;
		}
				
		private void LoadData()
		{
			WriteTraceMessage("LoadData Start", TraceLevel.Verbose);

			int ID;
			XmlNode Node = null;
			ElfCore.Channels.BaseChannel NewChannel = null;

			if ((_setupData == null) || (_setupNode == null))
				throw new Exception("Setup Information Missing");

			// Pull the data out from the nodes
			//Profile.Channels.SetupStartChannel = _startChannel;
			//_workshop.UI.RedirectOutputs = _setupData.GetBoolean(_setupNode, Constants.XML_REDIRECT_OUTPUTS, false);
			//_workshop.UI.Background.LoadSettings(_setupNode, _settings);

			// Get display properties
			Node = _setupNode.SelectSingleNode(Constants.XML_DISPLAY);

			if (Node != null)
			{
				Profile.Scaling.LatticeSize = new Size(_xmlHelper.GetNodeValue(Node, Constants.XML_WIDTH, 64),
												   _xmlHelper.GetNodeValue(Node, Constants.XML_HEIGHT, 32));

				Profile.Scaling.CellSize = _xmlHelper.GetNodeValue(Node, Constants.XML_PIXELSIZE, Profile.Scaling.CellSize.GetValueOrDefault(1));
				Profile.Scaling.ShowGridLines = (_xmlHelper.GetNodeValue(Node, Constants.XML_GRIDLINE_WIDTH, 1) == 1);

				//try
				//{
				//	int Alpha = _xmlHelper.GetNodeValue(Node, Constants.XML_INACTIVE_Channel_ALPHA, 128);
				//	if ((Alpha < 0) || (Alpha > 255))
				//		Alpha = 128;
				//	Profile.InactiveChannelAlpha = (byte)Alpha;
				//}
				//catch
				//{ }

				Node = null;
			}

			// Load the Channel information
			for (int i = _startChannel; i < _vixenChannels.Count; i++)
			{
				//_workshop.Profile.Channels.Add(new ElfCore.Core.Channel(_vixenChannels[i]));
			}
			NewChannel = null;

			// Get Channel pixel lists
			foreach (XmlNode ChannelNode in _setupNode.SelectNodes(Constants.XML_ChannelS + "/" + Constants.XML_Channel))
			{
				if (ChannelNode is XmlWhitespace)
					continue;
				ID = Convert.ToInt32(ChannelNode.Attributes[Constants.NUMBER].Value);
				//if (ID < Profile.Channels.SetupStartChannel)
				//	continue;

				NewChannel = _workshop.Profile.Channels.Get(ID);
				NewChannel.DeserializeLattice(ChannelNode.InnerText);
			}

			// Now determine if there is a list of SortOrders in the Profile Xml. If so, then retrieve that list and pass it onto the
			// Channel Controller
			Node = _setupNode.OwnerDocument.DocumentElement.SelectSingleNode("//SortOrders");
			int InitialSort = _xmlHelper.GetAttributeValue(Node, "lastSort", 0);
			if (InitialSort == -1)
				InitialSort = 0;
			else
				InitialSort = InitialSort - 1;

			string OrderList = string.Empty;
			string Name = string.Empty;

			//if (Node.ChildNodes.Count == 0)
			//    Profile.Channels.AddSortOrder();
			//else
			//{
			//    foreach (XmlNode Child in Node.ChildNodes)
			//    {
			//        OrderList = Child.InnerText;
			//        Name = _xmlHelper.GetAttributeValue(Child, "name", string.Empty);
			//        Profile.Channels.AddSortOrder(Name, OrderList);
			//    }
			//}
			//Profile.Channels.ActiveSortOrder = InitialSort;

			// Make sure the first Channel knows it's selected
			if (_workshop.Profile.ChannelCount > 0)
				_workshop.Profile.Channels.Select(0);

			_workshop.Profile.Clear_UndoStacks();

			WriteTraceMessage("LoadData End", TraceLevel.Verbose);
		}

		private void Save(SetupData setupData, XmlNode saveNode)
		{
			WriteTraceMessage("Save Start", TraceLevel.Verbose);

			string XPath = string.Format(Constants.XML_DISPLAY, this.Name) + Constants.XML_DISPLAY;

			XmlNode DisplayNode = saveNode.SelectSingleNode(Constants.XML_DISPLAY);
			XPath += "/";

			if (DisplayNode == null)
				DisplayNode = _xmlHelper.CreateNode(saveNode, Constants.XML_DISPLAY);

			_xmlHelper.SetValue(DisplayNode, Constants.XML_HEIGHT, Profile.Scaling.LatticeSize.Height);
			_xmlHelper.SetValue(DisplayNode, Constants.XML_WIDTH, Profile.Scaling.LatticeSize.Width);
			_xmlHelper.SetValue(DisplayNode, Constants.XML_PIXELSIZE, Profile.Scaling.CellSize.GetValueOrDefault(1));
			_xmlHelper.SetValue(DisplayNode, Constants.SHOW_GRIDLINES, Profile.Scaling.ShowGridLines.GetValueOrDefault(true));
			//_xmlHelper.SetValue(DisplayNode, Constants.XML_INACTIVE_Channel_ALPHA, Profile.InactiveChannelAlpha);
			
			// Set any background image
			//_workshop.UI.Background.SaveSettings(saveNode, _settings);

			// Create Channel pixel lists
			XmlNode ChannelsNode = Xml.GetEmptyNodeAlways(saveNode, Constants.XML_ChannelS);
			XmlNode ChannelNode;

			foreach (ElfCore.Channels.BaseChannel Channel in _workshop.Profile.Channels.Sorted)
			{
				ChannelNode = Xml.SetNewValue(ChannelsNode, Constants.XML_Channel, string.Empty);
				Xml.SetAttribute(ChannelNode, Constants.NUMBER, Channel.Index.ToString());
				ChannelNode.InnerText = Channel.SerializeLattice();
			}

			//setupData.SetBoolean(saveNode, Constants.XML_REDIRECT_OUTPUTS, _workshop.UI.RedirectOutputs);

			WriteTraceMessage("Save End", TraceLevel.Verbose);
		}

		public static void WriteTraceMessage(string message, TraceLevel traceLevel)
		{
			try
			{
				const string Bullet = "; ";

				MethodBase Method = (new StackTrace()).GetFrame(1).GetMethod();
				string MethodName = Bullet + "Method: " + Method.DeclaringType.Name + "." + Method.Name + "()" + Bullet;
				string TraceLevel = Bullet + traceLevel.ToString().PadRight(7);
				string TraceMessage = string.Empty;
				string Date = String.Format("{0:MM/dd/yyyy HH:mm:ss}", DateTime.Now);

				// Strip out extra whitespacing
				message = message.Replace("\t", " ").Replace("\r", string.Empty).Replace("\n", " ");
				while (message.Contains("  "))
					message = message.Replace("  ", " ");

				TraceMessage = (Date + TraceLevel + MethodName.PadRight(50) + message.Trim());

				Method = null;
			}
			catch
			{}
		}		
		
		#endregion [ Methods ]

	}
}


