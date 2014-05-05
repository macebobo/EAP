using ElfCore.Interfaces;
using System.IO;
using System.Xml;

namespace ElfCore.Util
{
	public class Settings
	{
		public enum SettingsStyle
		{
			Registry,
			Xml
		}

		#region [ Constants ]

		public const string SAVE_PATH_DELIMITER = "|";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private static readonly Settings _instance = new Settings();
		private ISettings _iSettings = null;
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
					_iSettings = new Registry_Settings();
				}
				else
				{
					_iSettings = new Xml_Settings("Elf.settings");
				}
			}
		}

		internal ISettings ISettings
		{
			get { return _iSettings; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		static Settings()
		{ }

		private Settings()
		{
			//_iSettings = new Xml_Settings();
		}

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
			return _iSettings.GetValue(path, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Integer value found on the path, or the defaultValue</returns>
		public int GetValue(string path, int defaultValue)
		{
			return _iSettings.GetValue(path, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Byte value found on the path, or the defaultValue</returns>
		public byte GetValue(string path, byte defaultValue)
		{
			return _iSettings.GetValue(path, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public float GetValue(string path, float defaultValue)
		{
			return _iSettings.GetValue(path, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Double value found on the path, or the defaultValue</returns>
		public double GetValue(string path, double defaultValue)
		{
			return _iSettings.GetValue(path, defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public string GetValue(string path, string defaultValue)
		{
			return _iSettings.GetValue(path, defaultValue);
		}

		#endregion [ Get Methods ]

		#region [ Remove Methods ]

		/// <summary>
		/// Removes the node at this path
		/// </summary>
		/// <param name="path">path to the node to remove</param>
		public void RemoveValue(string path)
		{
			_iSettings.RemoveValue(path);
		}

		#endregion [ Remove Methods ]

		#region [ SetValue Methods ]

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">String value to save</param>
		public void SetValue(string path, string value)
		{
			_iSettings.SetValue(path, value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Integer value to save</param>
		public void SetValue(string path, int value)
		{
			_iSettings.SetValue(path, value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Floating point value to save</param>
		public void SetValue(string path, float value)
		{
			_iSettings.SetValue(path, value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Boolean value to save</param>
		public void SetValue(string path, bool value)
		{
			_iSettings.SetValue(path, value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Double value to save</param>
		public void SetValue(string path, double value)
		{
			_iSettings.SetValue(path, value);
		}

		#endregion [ SetValue Methods ]

		public string AppendPath(string path, string newNode)
		{
			return path + SAVE_PATH_DELIMITER + newNode;
		}

		public void Save()
		{
			_iSettings.Save();
		}

	}

	#region [ Class Registry_Settings ]

	internal class Registry_Settings : ElfDisposable, ISettings
	{
		
		#region [ Private Variables ]

		private Registry _registry = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		#endregion [ Properties ]

		#region [ Constructors ]

		public Registry_Settings()
		{ 
			_registry = new Registry(string.Format(Registry.REGISTRY_ADDIN_PATH, "Elf Profile Editor"));
		}

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
			return _registry.GetValue(PathForRegistry(path), defaultValue);
		}

		///// <summary>
		///// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		///// </summary>
		///// <param name="node">Xml node to search</param>
		///// <param name="path">Path to locate the value</param>
		///// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		///// <returns>Boolean value found on the path, or the defaultValue</returns>
		//public bool GetValue(XmlNode node, string path, bool defaultValue)
		//{
		//	string Result = GetValue(node, path, defaultValue.ToString());
		//	bool Value = defaultValue;
		//	if (bool.TryParse(Result, out Value))
		//		return Value;
		//	else
		//		return defaultValue;
		//}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Integer value found on the path, or the defaultValue</returns>
		public int GetValue(string path, int defaultValue)
		{
			return _registry.GetValue(PathForRegistry(path), defaultValue);
		}

		///// <summary>
		///// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		///// </summary>
		///// <param name="node">Xml node to search</param>
		///// <param name="path">Path to locate the value</param>
		///// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		///// <returns>Integer value found on the path, or the defaultValue</returns>
		//public int GetValue(XmlNode node, string path, int defaultValue)
		//{
		//	string Result = GetValue(node, path, defaultValue.ToString());
		//	int Value = defaultValue;
		//	if (int.TryParse(Result, out Value))
		//		return Value;
		//	else
		//		return defaultValue;
		//}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public float GetValue(string path, float defaultValue)
		{
			return _registry.GetValue(PathForRegistry(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public byte GetValue(string path, byte defaultValue)
		{
			return _registry.GetValue(PathForRegistry(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Floating point value found on the path, or the defaultValue</returns>
		public double GetValue(string path, double defaultValue)
		{
			return _registry.GetValue(PathForRegistry(path), defaultValue);
		}
		
		///// <summary>
		///// Retrieves a value from the xml object along the path indicated. If the value is not present on that path, returns the default value.
		///// </summary>
		///// <param name="node">Xml node to search</param>
		///// <param name="path">Path to locate the value</param>
		///// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		///// <returns>Floating point value found on the path, or the defaultValue</returns>
		//public float GetValue(XmlNode node, string path, float defaultValue)
		//{
		//	string Result = GetValue(node, path, defaultValue.ToString());
		//	float Value = defaultValue;
		//	if (float.TryParse(Result, out Value))
		//		return Value;
		//	else
		//		return defaultValue;
		//}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>String value found on the path, or the defaultValue</returns>
		public string GetValue(string path, string defaultValue)
		{
			return _registry.GetValue(PathForRegistry(path), defaultValue);
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
			_registry.SetValue(PathForRegistry(path), value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Integer value to save</param>
		public void SetValue(string path, int value)
		{
			_registry.SetValue(PathForRegistry(path), value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Floating point value to save</param>
		public void SetValue(string path, float value)
		{
			_registry.SetValue(PathForRegistry(path), value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Boolean value to save</param>
		public void SetValue(string path, bool value)
		{
			_registry.SetValue(PathForRegistry(path), value);
		}

		/// <summary>
		/// Saves the value to the settings object along the path indicated
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Double value to save</param>
		public void SetValue(string path, double value)
		{
			_registry.SetValue(PathForRegistry(path), value);
		}

		#endregion [ Save Methods ]

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			if (_registry != null)
			{
				_registry.Dispose();
				_registry = null;
			}
		}

		/// <summary>
		/// Removes the entry at this path
		/// </summary>
		/// <param name="path">path to the entry to remove</param>
		public void RemoveValue(string path)
		{
			_registry.RemoveEntry(path);
		}

		private string PathForRegistry(string path)
		{
			path = path.Replace(Settings.SAVE_PATH_DELIMITER, @"\");
			return path;
		}

		public void Save()
		{ }
	}
		
	#endregion [ Class Registry_Settings ]

	#region [ Class Xml_Settings ]

	internal class Xml_Settings : ElfDisposable, ISettings
	{
		#region [ Private Variables ]

		private XmlDocument _settingsXmlDoc = null;
		private XmlHelper _xmlHelper = XmlHelper.Instance;
		private string _settingsFileName = string.Empty;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public Xml_Settings()
		{
			_settingsXmlDoc = new XmlDocument();
		}

		public Xml_Settings(string filename)
			: this()
		{
			_settingsFileName = filename;

			bool LoadXmlFile = false;

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
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public string AppendPath(string path, string newNode)
		{
			return path + Settings.SAVE_PATH_DELIMITER + newNode;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			_settingsXmlDoc = null;
			_xmlHelper = null;
		}

		/// <summary>
		/// Converts the generic path to an XPath
		/// </summary>
		private string PathForXml(string path)
		{
			path = path.Replace(Settings.SAVE_PATH_DELIMITER, "/");
			path = path.Replace(" ", string.Empty);
			path = path.Replace("-", string.Empty);
			return path;
		}

		#region [ Get Methods ]

		public string GetValue_Attribute(string path, string name)
		{
			if (_settingsXmlDoc == null)
				return string.Empty;
			XmlNode Node = _settingsXmlDoc.DocumentElement.SelectSingleNode(PathForXml(path));
			if (Node == null)
				return string.Empty;
			return _xmlHelper.GetAttributeValue(Node, name);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Boolean value found on the path, or the defaultValue</returns>
		public bool GetValue(string path, bool defaultValue)
		{
			if (_settingsXmlDoc == null)
				return defaultValue;
			return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
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
			if (_settingsXmlDoc == null)
				return defaultValue;
			return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Byte value found on the path, or the defaultValue</returns>
		public byte GetValue(string path, byte defaultValue)
		{
			if (_settingsXmlDoc == null)
				return defaultValue;
			return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
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
			if (_settingsXmlDoc == null)
				return defaultValue;
			return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
		}

		/// <summary>
		/// Retrieves a value from the settings object along the path indicated. If the value is not present on that path, returns the default value.
		/// </summary>
		/// <param name="path">Path to locate the value</param>
		/// <param name="defaultValue">Value to retrieve if the path or data is missing</param>
		/// <returns>Value found on the path, or the defaultValue</returns>
		public double GetValue(string path, double defaultValue)
		{
			if (_settingsXmlDoc == null)
				return defaultValue;
			return _xmlHelper.GetNodeValue(_settingsXmlDoc.DocumentElement, PathForXml(path), defaultValue);
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
			if (_settingsXmlDoc == null)
				return defaultValue;
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
		public string GetValueAttribute(XmlNode node, string path, string attributeName, string defaultValue)
		{
			return _xmlHelper.GetAttributeValue(node.SelectSingleNode(path), attributeName, defaultValue);
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
			return _xmlHelper.GetAttributeValue(node.SelectSingleNode(path), attributeName, defaultValue);
		}

		#endregion [ Get Methods ]

		#region [ Remove Methods ]

		/// <summary>
		/// Removes the node at this path
		/// </summary>
		/// <param name="path">path to the node to remove</param>
		public void RemoveValue(string path)
		{
			if (_settingsXmlDoc == null)
				return;

			if (!path.Contains(Constants.XML_SETTINGS_ROOT))
				path = AppendPath(Constants.XML_SETTINGS_ROOT, path);
			if (path.Contains(Settings.SAVE_PATH_DELIMITER))
				path = PathForXml(path);
				_xmlHelper.RemoveNode(_settingsXmlDoc, path);
		}

		#endregion [ Remove Methods ]

		#region [ SetValue Methods ]

		public void SetValue_Attribute(string path, string name, string value)
		{
			if (_settingsXmlDoc == null)
				return;

			XmlNode Node = _xmlHelper.GetNode(_settingsXmlDoc, PathForXml(AppendPath(Constants.XML_SETTINGS_ROOT, path)));
			_xmlHelper.SetAttribute(Node, name, value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, string value)
		{
			if (_settingsXmlDoc == null)
				return;

			_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, int value)
		{
			if (_settingsXmlDoc != null)
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString());
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, byte value)
		{
			if (_settingsXmlDoc != null)
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString());
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, float value)
		{
			if (_settingsXmlDoc != null)
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString("0.00"));
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, double value)
		{
			if (_settingsXmlDoc != null)
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value.ToString("0.00"));
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(string path, bool value)
		{
			if (_settingsXmlDoc != null)
				_xmlHelper.SetNodeValue(_settingsXmlDoc, Constants.XML_SETTINGS_ROOT + "/" + PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(XmlNode node, string path, string value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(XmlNode node, string path, int value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(XmlNode node, string path, byte value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(XmlNode node, string path, bool value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		/// <summary>
		/// Saves the value using the path indicated.
		/// </summary>
		/// <param name="node">Xml node to search</param>
		/// <param name="path">Path on the xml object to save</param>
		/// <param name="value">Value to save</param>
		public void SetValue(XmlNode node, string path, float value)
		{
			_xmlHelper.SetValue(node, PathForXml(path), value);
		}

		#endregion [ SetValue Methods ]

		public void Save()
		{
			if ((_settingsFileName ?? string.Empty).Length > 0)
				_settingsXmlDoc.Save(_settingsFileName);
		}

		#endregion [ Methods ]
	}

	#endregion [ Class Xml_Settings ]
}
