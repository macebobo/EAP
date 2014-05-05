using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace SettingsZapper
{
	public partial class Main : Form
	{
		private const string REGISTRY_ADDIN_PATH = @"Software\Vixen\{0}";

		public Main()
		{
			InitializeComponent();
			Zap();
		}

		private void CloseMe_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Delete the settings file from the profile path, and remove any entry from the registry
		/// </summary>
		public void Zap()
		{
			// First zap the file
			try
			{
				string FileName = Path.Combine(GetProfilePath(), "AdjPreview.settings");
				FileInfo FI = new FileInfo(FileName);
				if (FI.Exists)
					FI.Delete();
				FI = null;
			}
			catch { }

			// Now zap the entry in the registry
			try
			{
				string RPath = string.Format(REGISTRY_ADDIN_PATH, "Adjustable Preview");
				RegistryKey registrykeyHKLM = Microsoft.Win32.Registry.CurrentUser;
				string keyPath = RPath;
				registrykeyHKLM.DeleteSubKeyTree(keyPath);
				registrykeyHKLM.Close();
				registrykeyHKLM = null;
			}
			catch { }
		}

		public static string GetProfilePath()
		{
			string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen");
			DirectoryInfo DInfo = new DirectoryInfo(SavePath);
			if (!DInfo.Exists)
			{
				// Create the folder
				DInfo.Create();
			}
			return SavePath;
		}
	}
}
