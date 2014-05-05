using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace ElfCore.Util
{
	/// <summary>
	/// http://stackoverflow.com/questions/3280362/most-elegant-xml-serialization-of-color-structure
	/// </summary>
	public class XmlGraphicsPath
	{

		#region [ Private Variables ]

		private GraphicsPath _path = new GraphicsPath();

		#endregion [ Private Variables ]

		#region [ Properties ]

		[XmlText]
		public string Default
		{
			get { return FromBaseType(_path); }
			set { _path = ToBaseType(value); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public XmlGraphicsPath()
		{ }

		public XmlGraphicsPath(GraphicsPath g)
		{
			_path = g;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public static string FromBaseType(GraphicsPath path)
		{
			string Encoded = string.Empty;

			if ((path == null) || (path.PointCount == 0))
				return string.Empty;

			// Create a | delimited string, listing the FillMode, PathPoints and PathTypes
			// Path points and PathTypes are saved to a semi-colon seperated list. Point data is saved a comma seperated pairs

			Encoded = path.FillMode.ToString() + "|";
			string List = string.Empty;
			for (int i = 0; i < path.PathPoints.Length; i++)
			{
				List += ((List.Length > 0) ? ";" : string.Empty) + path.PathPoints[i].X.ToString("0.0000") + "," + path.PathPoints[i].Y.ToString("0.0000");
			}
			Encoded += List + "|";
			List = string.Empty;
			for (int i = 0; i < path.PathTypes.Length; i++)
			{
				List += ((List.Length > 0) ? ";" : string.Empty) + path.PathTypes[i].ToString();
			}
			Encoded += List;

			return Encoded;
		}

		public static GraphicsPath ToBaseType(string value)
		{
			GraphicsPath Restored = new GraphicsPath();
			string[] List;
			PointF[] PathPoints;
			byte[] PathTypes;

			if ((value ?? string.Empty).Length == 0)
				return Restored;

			string[] GPathParts = value.Split('|');

			List = GPathParts[1].Split(';');
			PathPoints = new PointF[List.Length];
			for (int i = 0; i < List.Length; i++)
			{
				PathPoints[i] = new PointF((float)Convert.ToDecimal(List[i].Split(',')[0]), (float)Convert.ToDecimal(List[i].Split(',')[1]));
			}

			List = GPathParts[2].Split(';');
			PathTypes = new byte[List.Length];
			for (int i = 0; i < List.Length; i++)
				PathTypes[i] = Convert.ToByte(List[i]);

			Restored = new GraphicsPath(PathPoints, PathTypes, EnumHelper.GetValueFromDescription<FillMode>(GPathParts[0]));

			return Restored;
		}

		#endregion [ Methods ]

		#region [ Operators ]

		public static implicit operator GraphicsPath(XmlGraphicsPath g)
		{
			return g._path;
		}

		public static implicit operator XmlGraphicsPath(GraphicsPath g)
		{
			return new XmlGraphicsPath(g);
		}

		#endregion [ Operators ]
	}
}
