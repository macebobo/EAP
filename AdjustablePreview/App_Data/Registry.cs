using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ElfCore
{
	public class Registry : IDisposable
	{
		#region [ Constants ]

		public const string REGISTRY_PATH = @"Software\Vixen\Settings";
		public const string REGISTRY_ADDIN_PATH = @"Software\Vixen\{0}\Settings";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private RegistryKey _key = null;
		private bool _disposed = false;
		private string _currentPath = string.Empty;
		private string _defaultPath = string.Empty;

		#endregion [ Private Variables ]

		#region [ Properties ]
		
		private string CurrentKeyPath
		{
			set { _currentPath = value; }
			get
			{
				if (_currentPath.Length == 0)
					_currentPath = _defaultPath;
				return _currentPath;
			}
		}

		private string DefaultKeyPath
		{
			set { _defaultPath = value; }
			get { return _defaultPath; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Registry()
		{
			if (_disposed)
			{
				GC.ReRegisterForFinalize(true);
			}
			_disposed = false;

			CurrentKeyPath = REGISTRY_PATH;
			_defaultPath = REGISTRY_PATH;
		}

		public Registry(string registryPath)
			: this()
		{
			CurrentKeyPath = registryPath;
			_defaultPath = registryPath;
		}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~Registry()
		{
			//Execute the code that does the cleanup.
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				if (_key != null)
				{
					_key.Close();
					_key = null;
				}
			}

			// Remember that we've executed this code
			_disposed = true;
		}

		public void Dispose()
		{
			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);
		}

		#endregion [ Destructors ]

		#region [ Methods ]

		public void OpenKey()
		{
			string path = CurrentKeyPath;
			_key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, true);
			if (_key == null)
				_key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
		}

		public void OpenKey(string path)
		{
			if (_key != null)
			{
				_key.Close();
				_key = null;
			}

			CurrentKeyPath = path;
			OpenKey();
		}

		#region [ GetValue ]

		/// <summary>
		/// Retrieves a string value from the registry. If not present returns the default value
		/// </summary>
		public string GetValue(string name, string defaultValue)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}

			if (_key == null)
				OpenKey();

			return _key.GetValue(name, defaultValue).ToString();
		}

		/// <summary>
		/// Retrieves a string value from the registry. If not present returns an empty string
		/// </summary>
		public string GetValue(string name)
		{
			return GetValue(name, string.Empty);
		}

		/// <summary>
		/// Retrieves a boolean value from the registry. If not present, or not stored as a boolean value, returns the default value
		/// </summary>
		public bool GetValue(string name, bool defaultValue)
		{
			bool Value = defaultValue;
			string RegValue = GetValue(name, defaultValue.ToString());

			if (bool.TryParse(RegValue, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves an Int32 value from the registry. If not present, or not stored as a Int32 value, returns the default value
		/// </summary>
		public int GetValue(string name, int defaultValue)
		{
			int Value = defaultValue;
			string RegValue = GetValue(name, defaultValue.ToString());

			if (Int32.TryParse(RegValue, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves an Float value from the registry. If not present, or not stored as a float value, returns the default value
		/// </summary>
		public float GetValue(string name, float defaultValue)
		{
			float Value = defaultValue;
			string RegValue = GetValue(name, defaultValue.ToString());

			if (float.TryParse(RegValue, out Value))
				return Value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Retrieves a object (as a string) value from the registry. If not present returns the default value
		/// </summary>
		public string GetValue(string name, object defaultValue)
		{
			return GetValue(name, (string)defaultValue).ToString();
		}

		/// <summary>
		/// Retrieves a DateTime value from the registry. If not present, or not stored as a DateTime value, returns the default value
		/// </summary>
		public DateTime GetValue(string name, DateTime defaultValue)
		{
			DateTime Value = defaultValue;
			string RegValue = GetValue(name, defaultValue.ToString());

			if (DateTime.TryParse(RegValue, out Value))
				return Value;
			else
				return defaultValue;
		}

		#endregion [ GetValue ]

		#region [ SetValue ]

		public void SetValue(string name, string value)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}
			else
			{
				if (_currentPath != DefaultKeyPath)
				{
					_currentPath = DefaultKeyPath;
					OpenKey();
				}
			}

			_key.SetValue(name, value);
		}

		public void SetValue(string name, int value)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}
			else
			{
				if (_currentPath != DefaultKeyPath)
				{
					_currentPath = DefaultKeyPath;
					OpenKey();
				}
			}
			
			_key.SetValue(name, value);
		}

		public void SetValue(string name, decimal value)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}
			else
			{
				if (_currentPath != DefaultKeyPath)
				{
					_currentPath = DefaultKeyPath;
					OpenKey();
				}
			}
			
			_key.SetValue(name, value);
		}

		public void SetValue(string name, float value)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}
			else
			{
				if (_currentPath != DefaultKeyPath)
				{
					_currentPath = DefaultKeyPath;
					OpenKey();
				}
			}
			
			_key.SetValue(name, value);
		}

		public void SetValue(string name, bool value)
		{
			if (name.Contains(@"\"))
			{
				string Path = name.Substring(0, name.LastIndexOf(@"\"));
				name = name.Replace(Path, string.Empty).Substring(1);
				OpenKey(DefaultKeyPath + @"\" + Path);
			}
			else
			{
				if (_currentPath != DefaultKeyPath)
				{
					_currentPath = DefaultKeyPath;
					OpenKey();
				}
			}
			
			_key.SetValue(name, value);
		}

		#endregion [ SetValue ]

		#endregion [ Methods ]

		#region [ Static Methods ]

		#endregion [ Static Methods ]

	}
}


