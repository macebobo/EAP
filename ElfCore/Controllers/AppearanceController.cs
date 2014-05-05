using ElfCore.Util;
using System.Collections.Generic;

namespace ElfCore.Controllers
{
	/// <summary>
	/// Controls how the application appears to the user
	/// </summary>
	public class AppearanceController
	{

		#region [ Private Variables ]

		/// <summary>
		/// Used to declare this class as a Singleton
		/// </summary>
		private static readonly AppearanceController _instance = new AppearanceController();

		private List<Skin> _skins;
		private Skin _currentSkin;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public static AppearanceController Instance
		{
			get { return _instance; }
		}

		public Skin CurrentSkin
		{
			get { return _currentSkin; }
			set { _currentSkin = value; }
		}

		public List<Skin> Skins
		{
			get { return _skins; }
			set { _skins = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>
		/// Singleton constructor
		/// </summary>
		static AppearanceController()
		{ }

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public AppearanceController()
		{
			_skins = new List<Skin>();
			LoadSkins();

			if (_skins.Count > 0)
				_currentSkin = _skins[0];
			else
			{
				_currentSkin = new Skin();
				_currentSkin.InitializeDefaultSkin();
				_skins.Add(_currentSkin);
			}
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Loads the list of all available skins.
		/// </summary>
		private void LoadSkins()
		{ 
			// NYI
		}

		#endregion [ Methods ]

		#region [ Events ]

		#endregion [ Events ]
	}
}
