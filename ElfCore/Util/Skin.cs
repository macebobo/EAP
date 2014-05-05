using System.Drawing;

namespace ElfCore.Util
{
	/// <summary>
	/// Collection of colors and images used to change the appearance of the application
	/// </summary>
	public class Skin
	{

		#region [ Private Variables ]

		private Color _baseColor = Color.Empty;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public Color BaseColor
		{
			get { return _baseColor; }
			set { _baseColor = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public Skin()
		{ }

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create a default skin based on the Color Black and use the unmodified images stored in the Resources
		/// </summary>
		public void InitializeDefaultSkin()
		{
			_baseColor = Color.Black;
		}

		#endregion [ Methods ]

		#region [ Events ]

		#endregion [ Events ]
	}
}
