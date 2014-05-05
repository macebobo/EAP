using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ElfCore.Controllers;
using ElfCore.Properties;
using ElfCore.Util;

namespace ElfCore.Forms
{
	/// <summary>
	/// http://www.codeproject.com/Articles/5454/A-Pretty-Good-Splash-Screen-in-C
	/// </summary>
	public partial class ProfileSplash : Form
	{

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public int Timeout
		{
			set
			{
				tmrVanish.Interval = value;
				tmrVanish.Enabled = true;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ProfileSplash()
		{
			InitializeComponent();

			lblVersion.Text = AssemblyVersion + " " + AssemblyTrademark;
			
			lblDetails.Text = AssemblyCopyright + GetAssemblyVersions();

			if (_workshop.RunMode == RunMode.PlugIn)
			{
				SuspendLayout();
				BackgroundImage = Resources.PreviewSplash;
				ResumeLayout(false);
				PerformLayout();
			}
			ClientSize = BackgroundImage.Size;
		}

		public ProfileSplash(bool splash)
			: this()
		{
			if (splash)
			{
				cmdOk.Visible = false;
				tmrVanish.Enabled = true;
				//tmrVanish.Start();
				//UpdateTimer.Stop();
				if (_workshop.RunMode == RunMode.PlugIn)
					tmrVanish.Interval = 3500;
			}

		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private string GetAssemblyVersions()
		{
			string Versions = string.Empty;
			FileInfo FI = new FileInfo(Assembly.GetExecutingAssembly().Location);
			string Path = FI.DirectoryName + "\\";

			Versions += GetAssemblyVersion(Path, "ElfCore.dll");
			Versions += GetAssemblyVersion(Path, "ElfControls.dll");
			Versions += GetAssemblyVersion(Path, "ElfTools.dll");
			Versions += GetAssemblyVersion(Path, "Docking.dll");

			FI = null;
			return Versions;
		}

		private string GetAssemblyVersion(string path, string filename)
		{
			string name = filename.Replace(".dll", string.Empty);
			FileInfo FI = new FileInfo(path + filename);
			if ((FI == null) || !FI.Exists)
				return Environment.NewLine + name + " not found";
			else
			{
				Assembly a = Assembly.LoadFile(path + filename);
				FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(a.Location);
				return  Environment.NewLine + name + " v" + fvi.FileVersion;
			}
		}

		#endregion [ Methods ]

		#region [ Assembly Attribute Accessors ]

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		public string AssemblyTrademark
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyTrademarkAttribute)attributes[0]).Trademark;
			}
		}

		#endregion [ Assembly Attribute Accessors ]

		#region [ Events ]

		private void tmrVanish_Tick(object sender, EventArgs e)
		{
			tmrVanish.Enabled = false;
			Close();
		}

		#endregion [ Events ]
	}
}

		
		
