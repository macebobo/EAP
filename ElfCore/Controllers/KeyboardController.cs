using ElfCore.Util;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;

namespace ElfCore.Controllers
{
	public class KeyboardController : ElfDisposable
	{
		#region [ Constants ]

		public const string KeyboardDefaultName = "Elf_Keyboard.xml";

		private const string XmlNode_Config = "KeyboardConfig";
		//private const string XmlNode_Filename = "Filename";
		//private const string XmlNode_Current = "CurrentConfig";
		private const string XmlNode_Shortcut = "Shortcut";
		private const string XmlNode_Count = "Count";
		private const string XmlNode_Key = "Key";
		
		private const string Attribute_Gesture = "gesture";
		private const string Attribute_ID = "id";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private SortedList<string, MultiKeyGesture> _currentGestures = null;
		private SortedList<string, MultiKeyGesture> _defaultGestures = null;
		private Dictionary<string, MultiKeyGesture> _gestureDictionary = null;
		private Settings _settings = Settings.Instance;
		private string _filename = string.Empty;
		private string _lastKeyDown = string.Empty;

		#endregion [ Private Variables ]

		#region [ Static Variables ]

		private static KeysConverter _converter = new KeysConverter();

		#endregion [ Static Variables ]

		#region [ Properties ]

		/// <summary>
		/// List of all the loaded MultiKeyGestures
		/// </summary>
		public SortedList<string, MultiKeyGesture> CurrentGestures
		{
			get { return _currentGestures; }
			set 
			{
				if (value != null)
				{
					_currentGestures = new SortedList<string, MultiKeyGesture>();
					foreach (KeyValuePair<string, MultiKeyGesture> KVP in value)
					{
						_currentGestures.Add(KVP.Key, new MultiKeyGesture(KVP.Value.ID, KVP.Value.Gesture, KVP.Value.MenuItem));
					}
					BuildDictionary();
				}
			}
		}

		/// <summary>
		/// List of all the MultiKeyGestures set at DesignTime.
		/// </summary>
		public SortedList<string, MultiKeyGesture> DefaultGestures
		{
			get { return _defaultGestures; }
		}

		/// <summary>
		/// Name of the file the current list of Gestures is loaded from.
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		/// <summary>
		/// Dictionary of the current set of command that have gestures associated with them.
		/// </summary>
		public Dictionary<string, MultiKeyGesture> GestureDictionary
		{
			get { return _gestureDictionary; }
		}

		//public bool IsSuspended
		//{
		//	get { return _keyboardHook.IsSuspended; }
		//	set { _keyboardHook.IsSuspended = value; }
		//}

		#endregion [ Properties ]

		#region [ Constructors ]

		public KeyboardController()
		{
			_currentGestures = new SortedList<string, MultiKeyGesture>();
			_defaultGestures = new SortedList<string, MultiKeyGesture>();
			_gestureDictionary = new Dictionary<string, MultiKeyGesture>();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void AssignList(SortedList<string, MultiKeyGesture> list)
		{
			if (list == null)
				return;

			MultiKeyGesture m = null;
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in list)
			{
				if (_currentGestures.ContainsKey(KVP.Key))
				{
					m = _currentGestures[KVP.Key];
					m.Gesture = KVP.Value.Gesture;
				}
				else
				{
					m = new MultiKeyGesture(KVP.Value.ID, KVP.Value.Gesture, KVP.Value.MenuItem);
					_currentGestures.Add(KVP.Value.ID, m);
				}
			}
			BuildDictionary();
		}

		/// <summary>
		/// Assign the current list of gestures to be that of the default list.
		/// </summary>
		public void AssignToDefault()
		{
			AssignList(_defaultGestures);
		}

		/// <summary>
		/// Adds all the MenuItems and/or Buttons from a given ToolStrip.
		/// </summary>
		public void AttachToolStrip(ToolStrip strip, string rootID)
		{
			foreach (ToolStripItem Item in strip.Items)
			{
				if (Item is ToolStripMenuItemWithKeys)
					AttachMenuItem((ToolStripMenuItemWithKeys)Item, rootID);
				else if (Item is ToolStripButtonWithKeys)
					AttachButton((ToolStripButtonWithKeys)Item, rootID);
			}
		}

		/// <summary>
		/// Adds the Menu to the collection of MultiKeyGestures if a valid ID can be created for it. If the menu has submenus, then don't
		/// add the menu, but call this method for all the submenus
		/// </summary>
		public void AttachMenuItem(ToolStripMenuItemWithKeys menuItem, string rootID)
		{
			if ((menuItem == null) || (menuItem.Tag != null))
				return;

			if (menuItem.HasDropDownItems)
			{
				foreach (object subMenu in menuItem.DropDownItems)
				{
					if (subMenu is ToolStripMenuItemWithKeys)
						AttachMenuItem((ToolStripMenuItemWithKeys)subMenu, rootID);
				}
			}
			else
			{
				string ID = CreateMenuItemID(menuItem, rootID);
					if (ID.Length > 0)
						_defaultGestures.Add(ID, new MultiKeyGesture(ID, menuItem.MultiGestureKey1, menuItem.MultiGestureKey2, menuItem));
			}
		}

		/// <summary>
		/// Adds the button to the collection of MultiKeyGestures if a valid ID can be generated, based on the MultiGestureKey1 & 2 properties.
		/// If the button is a PlugInToolStripButton, then checks to see if it contains a PlugInToolGroup object. If so, then finds all the child
		/// buttons on that ToolGroup's Child ToolBox.
		/// </summary>
		public void AttachButton(ToolStripButtonWithKeys button, string rootID)
		{
			if (button == null)
				return;
			string ID = CreateButtonID(button, rootID);
			if (ID.Length > 0)
				_defaultGestures.Add(ID, new MultiKeyGesture(ID, button.MultiGestureKey1, button.MultiGestureKey2, button));

			if (button is PlugInToolStripButton)
			{ 
				// Check to see if the plugin is a ToolHost. If so, interogate it to get the child buttons
				PlugInToolStripButton pButton = (PlugInToolStripButton)button;
				if (pButton.PlugInToolGroup != null)
				{
					AttachToolStrip(pButton.PlugInToolGroup.ChildToolBox, rootID);
				}
				else if (pButton.PlugInTool != null)
				{
					AttachToolStrip(pButton.PlugInTool.SettingsToolStrip, "Tool");
				}
			}
		}

		/// <summary>
		/// Create a dictionary of valid entries with Gestures, filtering out those that don't have any.
		/// </summary>
		public void BuildDictionary()
		{
			_gestureDictionary = new Dictionary<string, MultiKeyGesture>();
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in _currentGestures)
			{
				if (KVP.Value.Gesture.Length > 0)
				{
					if (!_gestureDictionary.ContainsKey(KVP.Value.Gesture))
						_gestureDictionary.Add(KVP.Value.Gesture, KVP.Value);
				}
			}
		}

		/// <summary>
		/// Clears out the last remembered key.
		/// </summary>
		public void ClearBuffer()
		{
			_lastKeyDown = string.Empty;
		}

		/// <summary>
		/// Creates an ID for the menu item based on who owns it.
		/// </summary>
		/// <param name="menuItem">ToolStripItem to generate the ID</param>
		private string CreateMenuItemID(ToolStripItem menuItem, string rootID)
		{
			if (menuItem == null)
				return string.Empty;
			
			string ID = ",";

			if (menuItem is ToolStripMenuItemWithKeys)
				ID += ((ToolStripMenuItemWithKeys)menuItem).PathNode;
			else
				ID += menuItem.Name;

			while (menuItem.OwnerItem != null)
			{ 
				menuItem = menuItem.OwnerItem;
				if (menuItem is ToolStripMenuItemWithKeys)
					ID = "," + ((ToolStripMenuItemWithKeys)menuItem).PathNode + ID;
				else
					ID = "," + menuItem.Name + ID;
			}
			return rootID + ID;
		}

		/// <summary>
		/// Creates an ID for the button based on who owns it. If the button object is an PlugInToolStripButton,
		/// then it is said to be owned by the ToolBox.
		/// </summary>
		/// <param name="button">ToolStripItem to generate the ID</param>
		private string CreateButtonID(ToolStripItem button, string rootID)
		{
			if (button == null)
				return string.Empty;
			
			string ID = ",";

			if (button is ToolStripButtonWithKeys)
				ID += ((ToolStripButtonWithKeys)button).PathNode;
			else
				ID += button.Name;

			while (button.OwnerItem != null)
			{
				button = button.OwnerItem;
				if (button is ToolStripButtonWithKeys)
					ID = "," + ((ToolStripButtonWithKeys)button).PathNode + ID;
				else
					ID = "," + button.Name + ID;
			}
			return rootID + ID;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			if (_currentGestures != null)
			{
				_currentGestures.Clear();
				_currentGestures = null;
			}
			if (_defaultGestures != null)
			{
				_defaultGestures.Clear();
				_defaultGestures = null;
			}
			if (_gestureDictionary != null)
			{
				_gestureDictionary.Clear();
				_gestureDictionary = null;
			}
		}

		/// <summary>
		/// Determines if the Keys enum passed in only contains meta keys.
		/// </summary>
		/// <param name="keyCode">Value to check</param>
		private bool IsOnlyModifiers(int keyValue)
		{
			return ((keyValue == 16) || /* Shift */
					(keyValue == 17) || /* Control */
					(keyValue == 18) || /* Alt */
					(keyValue == 91));  /* Windows */
		}

		/// <summary>
		/// Load in the keyboard configuration xml file and populate the gestures into the pre-populated list.
		/// </summary>
		/// <param name="filename">File to load</param>
		/// <param name="list">Pre-populated list.</param>
		public static void LoadConfigFile(string filename, SortedList<string, MultiKeyGesture> list)
		{
			if (((filename ?? string.Empty).Length == 0) || (list == null))
				return;

			XmlHelper XmlHelper = XmlHelper.Instance;
			XmlDocument Doc = new XmlDocument();
			string ID = string.Empty;
			string Gesture = string.Empty;
			Doc.Load(filename);

			foreach (XmlNode Node in Doc.DocumentElement.ChildNodes)
			{
				ID = XmlHelper.GetAttribute(Node, Attribute_ID, string.Empty);
				Gesture = XmlHelper.GetAttribute(Node, Attribute_Gesture, string.Empty);
				if (ID.Length == 0)
					continue;
				list[ID].Gesture = Gesture;
			}
			Doc = null;
			XmlHelper = null;
		}

		/// <summary>
		/// Retrieve stored values from the settings object.
		/// </summary>
		public void LoadSettings()
		{
			string ID = string.Empty;
			string Gesture = string.Empty;
			MultiKeyGesture m = null;
			_currentGestures.Clear();

			int Count = _settings.GetValue(_settings.AppendPath(XmlNode_Config, XmlNode_Count), (int)0);
			for (int i = 0; i < Count; i++)
			{
				Settings_LoadGesture(i, out ID, out Gesture);
				if (ID.Length == 0)
					continue;
				m = new MultiKeyGesture(ID, Gesture, _defaultGestures[ID].MenuItem);
				_currentGestures.Add(ID, m);
			}
			if (Count == 0)
				AssignToDefault();
			BuildDictionary();
		}

		private void Settings_LoadGesture(int index, out string id, out string gesture)
		{
			string Path = _settings.AppendPath(XmlNode_Config, XmlNode_Key + index.ToString());
			gesture = _settings.GetValue(Path, string.Empty);
			id = ((Xml_Settings)_settings.ISettings).GetValue_Attribute(Path, Attribute_ID);
			//gesture = ((Xml_Settings)_settings.ISettings).GetValue_Attribute(Path, Attribute_Gesture);
		}

		/// <summary>
		/// Loads up the list of MultiKeyGesture from an Xml file.
		/// </summary>
		public void Load()
		{
			if ((_filename ?? string.Empty).Length == 0)
				return;

			OnMessage("Loading Keyboard Configuration");
			FileInfo FI = new FileInfo(_filename);
			if (!FI.Exists)
			{
				FI = null;
				OnMessage("Keyboard Configuration file not found");
				return;
			}

			_currentGestures = new SortedList<string, MultiKeyGesture>();
			XmlHelper XmlHelper = XmlHelper.Instance;
			XmlDocument Doc = new XmlDocument();
			string Gesture = string.Empty;
			string ID = string.Empty;
			MultiKeyGesture m = null;

			Doc.Load(_filename);
			foreach (XmlNode Node in Doc.DocumentElement.ChildNodes)
			{
				Gesture = XmlHelper.GetAttributeValue(Node, Attribute_Gesture);
				ID = XmlHelper.GetAttributeValue(Node, Attribute_ID);

				// Find the entry in the default list for this command
				if (!_defaultGestures.ContainsKey(ID))
					continue;
				m = _defaultGestures[ID];
				_currentGestures.Add(ID, new MultiKeyGesture(ID, Gesture, m.MenuItem));
			}
			m = null;
			Doc = null;
			FI = null;
			XmlHelper = null;
		}

		/// <summary>
		/// Dumps the list of current MultiKeyGesture to string. Used for debugging.
		/// </summary>
		/// <returns>Each MultiKeyGesture per line, with the ID seperated from the human readable gesture by a tab.</returns>
		public string GetList()
		{
			StringBuilder Output = new StringBuilder();
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in _currentGestures)
			{
				Output.Append(KVP.Key);
				Output.Append("\t");
				Output.AppendLine(KVP.Value.ToString());
			}

			return Output.ToString();
		}

		/// <summary>
		/// Saves the list of keyboard shortcuts to an Xml file.
		/// </summary>
		public void Save()
		{
			Save(_filename, _currentGestures);
		}

		/// <summary>
		/// Save the list of multikey gestures to an Xml file.
		/// </summary>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="list">List of MultiKeyGesture objects to save.</param>
		public static void Save(string filename, SortedList<string, MultiKeyGesture> list)
		{
			if ((filename ?? string.Empty).Length == 0)
				return;
			if ((list == null) || (list.Count == 0))
				return;

			XmlHelper XmlHelper = XmlHelper.Instance;
			XmlDocument Doc = new XmlDocument();
			Doc.AppendChild(Doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
			XmlNode Root = Doc.CreateElement(XmlNode_Config);
			Doc.AppendChild(Root);
			XmlNode Node = null;

			foreach (KeyValuePair<string, MultiKeyGesture> KVP in list)
			{
				Node = Doc.CreateElement(XmlNode_Shortcut);
				Root.AppendChild(Node);
				//Node = XmlHelper.CreateNode(Root, XmlNode_Shortcut);
				XmlHelper.SetAttribute(Node, Attribute_Gesture, KVP.Value.Gesture);
				XmlHelper.SetAttribute(Node, Attribute_ID, KVP.Value.ID);
			}

			if (filename.Length == 0)
			{
				FileInfo FI = new FileInfo(Assembly.GetExecutingAssembly().Location);
				string Path = FI.DirectoryName + "\\";
				filename = Path + KeyboardDefaultName;
			}

			Doc.Save(filename);

			Root = null;
			Doc = null;
		}
		
		/// <summary>
		/// Store values into the settings object.
		/// </summary>
		public void SaveSettings()
		{
			_settings.SetValue(_settings.AppendPath(XmlNode_Config, XmlNode_Count), _currentGestures.Count);
			int Count = 0;

			// Save the current keyboard configuration into the settings Xml file.
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in _currentGestures)
			{
				Settings_SaveGesture(KVP.Value, Count++);
			}
		}

		private void Settings_SaveGesture(MultiKeyGesture m, int index)
		{
			if (m == null)
				return;
			string Path = _settings.AppendPath(XmlNode_Config, XmlNode_Key + index.ToString());
			((Xml_Settings)_settings.ISettings).SetValue(Path, m.Gesture);
			((Xml_Settings)_settings.ISettings).SetValue_Attribute(Path, Attribute_ID, m.ID);
		}

		/// <summary>
		/// Returns the MultiKeyGesture from the current list as indicated by the string version of the keys
		/// </summary>
		/// <param name="gesture">Human readable string derived from the keys contained in the MultiKeyGesture</param>
		/// <returns>Returns the entry that matches the gesture string if found, else returns null.</returns>
		public MultiKeyGesture Where(string gesture)
		{
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in _currentGestures)
			{
				if (KVP.Value.Gesture == gesture)
					return KVP.Value;
			}
			return null;
		}

		#endregion [ Methods ]

		#region [ Static Methods ]

		/// <summary>
		/// Converts the Keys enum to a human-readable string.
		/// </summary>
		/// <param name="keyCode">Value to convert</param>
		public static string KeysToString(Keys keyCode)
		{
			if (keyCode == Keys.None)
				return string.Empty;
			else
				return _converter.ConvertToString(keyCode);
		}

		/// <summary>
		/// Converts the human-readable string into the Keys enum. If unable to convert, or is blank, 
		/// then return Keys.None
		/// </summary>
		/// <param name="value">Value to convert</param>
		public static Keys StringToKeys(string value)
		{
			if ((value ?? string.Empty).Length == 0)
				return Keys.None;

			if (_converter.IsValid(value))
				return (Keys)_converter.ConvertFromString(value);
			else
				return Keys.None;
		}

		/// <summary>
		/// Converts the human-readable string into the Keys enum. If unable to convert, or is blank, 
		/// then return Keys.None
		/// </summary>
		/// <param name="value">Value to convert</param>
		public static void StringToKeys(string value, out Keys key1, out Keys key2)
		{
			key1 = Keys.None;
			key2 = Keys.None;

			if ((value ?? string.Empty).Length == 0)
				return;

			if (value.Contains(","))
			{
				key1 = StringToKeys(value.Split(',')[0]);
				key2 = StringToKeys(value.Split(',')[1]);
			}
			else
				key1 = StringToKeys(value);
		}

		#endregion [ Static Methods ]

		#region [ Events ]

		#region [ Event Declarations ]

		/// <summary>
		/// Occurs when the user has entered a valid chord.
		/// </summary>
		public EventHandlers.MultiKeyGestureEventHandler MultiKeyGestureOccurred;

		/// <summary>
		/// Occurs when this controller has a message for the user.
		/// </summary>
		public EventHandlers.MessageEventHandler MessageOccurred;

		#endregion [ Event Declarations ]

		#region [ Event Triggers ]

		private void OnMultiKeyGestureOccurred(MultiKeyGesture gesture)
		{
			if (MultiKeyGestureOccurred != null)
				MultiKeyGestureOccurred(this, new MultiKeyGestureEventArgs(gesture));
		}

		private void OnMessage(string message)
		{
			if (MessageOccurred != null)
				MessageOccurred(this, new MessageEventArgs(message));
		}

		#endregion [ Event Triggers ]

		#region [ Event Handlers ]

		public void Keyboard_KeyDown(KeyEventArgs e)
		{
			if (IsOnlyModifiers(e.KeyValue))
				return;
			
			string Key = KeysToString(e.KeyData);
			string MatchKey = string.Empty;

			Debug.WriteLine("LastKey: " + _lastKeyDown + "\tKey: " + Key);

			if (Key.Length == 0)
			{
				ClearBuffer();
				return;
			}

			if ((Key.Length > 0) && (_lastKeyDown.Length > 0))
				MatchKey = _lastKeyDown + "," + Key;
			else
				MatchKey = Key;

			if (_gestureDictionary.ContainsKey(MatchKey))
			{
				OnMultiKeyGestureOccurred(_gestureDictionary[MatchKey]);
				ClearBuffer();
				Key = string.Empty;
				e.Handled = true;
			}
			else
			{
				if (_lastKeyDown.Length == 0)
				{
					OnMessage(string.Format("({0}) was pressed. Waiting on second key of chord...", Key));
					_lastKeyDown = Key;
				}
				else
				{
					OnMessage(string.Format("The key combination ({0}) is not a command.", MatchKey));
					ClearBuffer();
				}
				Key = string.Empty;
			}

			Debug.WriteLine("LastKey: " + _lastKeyDown + "\tKey: " + Key);
		}

		public void Keyboard_KeyDownJustCapture(KeyEventArgs e)
		{
			if (IsOnlyModifiers(e.KeyValue))
				return;

			string Key = KeysToString(e.KeyData);
			string MatchKey = string.Empty;

			if (Key.Length == 0)
			{
				ClearBuffer();
				return;
			}

			if ((Key.Length > 0) && (_lastKeyDown.Length > 0))
				MatchKey = _lastKeyDown + "," + Key;
			else
				MatchKey = Key;

			_lastKeyDown = Key;
			Key = string.Empty;

			OnMessage(MatchKey);
			e.Handled = true;
		}

		#endregion [ Event Handlers ]

		#endregion [ Events ]
	}
}
