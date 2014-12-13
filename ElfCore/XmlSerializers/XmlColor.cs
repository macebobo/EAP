using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;

namespace ElfCore.XmlSerializers
{
	/// <summary>
	/// http://stackoverflow.com/questions/3280362/most-elegant-xml-serialization-of-color-structure
	/// </summary>
	[XmlRoot(Namespace = "http://halloween.tittivillus.com")]
	public class XmlColor
	{

		#region [ Private Variables ]

		private Color _color = Color.Black;

		#endregion [ Private Variables ]

		#region [ Properties ]

		[XmlText]
		public string Default
		{
			get { return FromBaseType(_color); }
			set { _color = ToBaseType(value); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public XmlColor()
		{ }

		public XmlColor(Color c) 
		{ 
			_color = c; 
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public static string FromBaseType(Color color)
		{
			if (color.IsEmpty)
				return "EMPTY";

			if (color.IsNamedColor)
				return color.Name;

			int colorValue = color.ToArgb();

			if (((uint)colorValue >> 24) == 0xFF)
				return String.Format("#{0:X6}", colorValue & 0x00FFFFFF);
			else
				return String.Format("#{0:X8}", colorValue);
		}

		public static Color ToBaseType(string value)
		{
			if (value == "EMPTY")
				return Color.Empty;

			try
			{
				if (value[0] == '#')
				{
					return Color.FromArgb((value.Length <= 7 ? unchecked((int)0xFF000000) : 0) +
						Int32.Parse(value.Substring(1), NumberStyles.HexNumber));
				}
				else
				{
					return Color.FromName(value);
				}
			}
			catch (Exception)
			{
			}

			return Color.Black;
		}

		#endregion [ Methods ]

		#region [ Operators ]

		public static implicit operator Color(XmlColor x)
		{
			return x._color;
		}

		public static implicit operator XmlColor(Color c)
		{
			return new XmlColor(c);
		}

		#endregion [ Operators ]
	}
}
