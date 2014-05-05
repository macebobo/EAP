using System.IO;
using System.Xml;
using Microsoft.Win32;

namespace ElfCore
{
	public class Settings
	{
		public enum SettingsStyle
		{
			Registry,
			Xml
		}

		#region [ Private Variables ]

		private static readonly Settings _instance = new Settings();

		private XmlDocument _settingsXmlDoc = null;
		private Registry _registry = null;
		private XmlHelper _xmlHelper = null;
		private bool _useRegistry = true;
		private string _settingsFileName = string.Empty;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public static Settings Instance
		{
			get { return _instance; }
		}

		public SettingsStyle Style
		{
			set
			{
				if (value == SettingsStyle.Registry)
				{
					_useRegistry = true;
					_registry = new Registry(string.Format(Registry.REGISTRY_ADDIN_PATH, "Adjustable Preview"));
				}
				else
				{
					bool LoadXmlFile = true;

					_settingsFileName = Path.Combine(Workshop.GetProfilePath(), "AdjPreview.settings");
					_settingsXmlDoc = new XmlDocument();

					if (!File.Exists(_settingsFileName))
						LoadXmlFile = false;
					else
					{
						FileInfo fi = new FileInfo(_settingsFileName);
						if (fi.Length == 0)
							LoadXmlFile = false;
						else
							LoadXmlFile = XmlHelper.IsValidXml(_settingsFileName);
					}

					if (LoadXmlFile)
						_settingsXmlDoc.Load(_settingsFileName);
					else
						_settingsXmlDoc.LoadXml("<" + Constants.XML_SETTINGS_ROOT + " />");

					_xmlHelper = new XmlHelper();
					_useRegistry = false;
				}
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		static Settings()
		{ }

		public Settings()
		{ }

		//public Settings(SettingsStyle style) : this()
		//{
		//    if (style == SettingsStyle.Registry)
		//    {
		//        _useRegistry = true;
		//        _registry = new Registry(string.Format(Registry.REGISTRY_ADDIN_PATH, "Adjustable Preview"));
		//    }
		//    else
		//    {
		//        bool LoadXmlFile = true;

		//        _settingsFileName = Path.Combine(_workshop.GetProfilePath(), "AdjPreview.settings");
		//        _settingsXmlDoc = new XmlDocument();

		//        if (!File.Exists(_settingsFileName))
		//            LoadXmlFile = false;
		//        else
		//        {
		//            FileInfo fi = new FileInfo(_settingsFileName);
		//            if (fi.Length == 0)
		//                LoadXmlFile = false;
		//            else
		//                LoadXmlFile = XmlHelper.IsValidXml(_settingsFileName);
		//        }

		//        if (LoadXmlFile)
		//            _settingsXmlDoc.Load(_settingsFileName);
		//        else
		//            _settingsXmlDoc.LoadXml("<" + Constants.XML_SETTINGS_ROOT + " />");

		//        _xmlHelper = new XmlHelper();
		//        _useRegistry = false;
		//    }			
		//}

		#endregion [ Constructors ]

		#region [ Get Methods ]

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Boolean value found on the path, or the defaultValue</returns>
		public bool GetValue(string path, bool defaultValue)
		{
			if (_useRegistry)
				return _registry.GetValue(PathForRegistry(path), defaultValue);
			else
				return _xmlHelper.GetNodeBoolValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Boolean value found on the path, or the defaultValue</returns>
		public bool GetValue(XmlNode node, string path, bool defaultValue)
		{
			string Result = GetValue(node, path, defaultValue.ToString());
			bool Value = defaultValue;
			if (bool.TryParse(Result, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Integer value found on the path, or the defaultValue</returns>
		public int GetValue(string path, int defaultValue)
		{
			if (_useRegistry)
				return _registry.GetValue(PathForRegistry(path), defaultValue);
			else
				return _xmlHelper.GetNodeIntValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Integer value found on the path, or the defaultValue</returns>
		public int GetValue(XmlNode node, string path, int defaultValue)
		{
			string Result = GetValue(node, path, defaultValue.ToString());
			int Value = defaultValue;
			if (int.TryParse(Result, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public float GetValue(string path, float defaultValue)
		{
			if (_useRegistry)
				return _registry.GetValue(PathForRegistry(path), defaultValue);
			else
				return _xmlHelper.GetNodeFloatValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public float GetValue(XmlNode node, string path, float defaultValue)
		{
			string Result = GetValue(node, path, defaultValue.ToString());
			float Value = defaultValue;
			if (float.TryParse(Result, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public string GetValue(string path, string defaultValue)
		{
			if (_useRegistry)
				return _registry.GetValue(PathForRegistry(path), defaultValue);
			else
				return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public string GetValue(XmlNode node, string path, string defaultValue)
		{
			return _xmlHelper.GetNodeValue(node, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="attributeName">Name of the attribute to load in</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public string GetValueAttribute(XmlNode node, string path, string attributeName , string defaultValue)
		{
			return _xmlHelper.GetTheAttribute(node.SelectSingleNode(path), attributeName, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path to locate the value</param>
		/// <param name="attributeName">Name of the attribute to load in</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public int GetValueAttribute(XmlNode node, string path, string attributeName, int defaultValue)
		{
			return _xmlHelper.GetTheAttribute(node.SelectSingleNode(path), attributeName, defaultValue);
		}

		#endregion [ Get Methods ]

		#region [ Save Methods ]

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">String value to save</param>
		public void SetValue(string path, string value)
		{
			if (_useRegistry)
				_registry.SetValue(PathForRegistry(path), value);
			else
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Integer value to save</param>
		public void SetValue(string path, int value)
		{
			if (_useRegistry)
				_registry.SetValue(PathForRegistry(path), value);
			else
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString());
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Floating point value to save</param>
		public void SetValue(string path, float value)
		{
			if (_useRegistry)
				_registry.SetValue(PathForRegistry(path), value);
			else
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString("0.00"));
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Boolean value to save</param>
		public void SetValue(string path, bool value)
		{
			if (_useRegistry)
				_registry.SetValue(PathForRegistry(path), value);
			else
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value to the XmlNode object along the path indicated
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">String value to save</param>
		public void SetValue(XmlNode node, string path, string value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value to the XmlNode object along the path indicated
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Integer value to save</param>
		public void SetValue(XmlNode node, string path, int value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value to the XmlNode object along the path indicated
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Boolean value to save</param>
		public void SetValue(XmlNode node, string path, bool value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value to the XmlNode object along the path indicated
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Floating point value to save</param>
		public void SetValue(XmlNode node, string path, float value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		#endregion [ Save Methods ]

		/// <summary>
		/// Concatenates the path with a name, delimited by the pre-defined path delimiter character.
		/// </summary>
		/// <param name="path">Pre-built path</param>
		/// <param name="newNode">Name to be appended</param>
		public string AppendPath(string path, string newNode)
		{
			return path + Constants.SAVE_PATH_DELIMITER + newNode;
		}

		public void Save()
		{
			if (_useRegistry)
				return;

			_settingsXmlDoc.Save(_settingsFileName);
		}

		private string PathForXml(string path)
		{
			path = path.Replace(@"|", "/");
			path = path.Replace(" ", string.Empty);
			path = path.Replace("-", string.Empty);
			return path;
		}

		private string PathForRegistry(string path)
		{
			path = path.Replace(@"|", @"\");
			return path;
		}

		/// <summary>
		/// Delete the settings file from the profile path, and remove any entry from the registry
		/// </summary>
		public void Zap()
		{ 
			// First zap the file
			string FileName = Path.Combine(Workshop.GetProfilePath(), "AdjPreview.settings");
			FileInfo FI = new FileInfo(FileName);
			if (FI.Exists)
				FI.Delete();
			FI = null;

			// Now zap the entry in the registry
			string RPath = string.Format(Registry.REGISTRY_ADDIN_PATH, "Adjustable Preview").Replace("\\Settings", string.Empty);
			RegistryKey registrykeyHKLM = Microsoft.Win32.Registry.CurrentUser;
			string keyPath = RPath;
			registrykeyHKLM.DeleteSubKeyTree(keyPath);
			registrykeyHKLM.Close();
			registrykeyHKLM = null;
		}
		
	}
}
