using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace ElfCore.Util
{
	public class XmlPointList
	{

		#region [ Private Variables ]

		private List<Point> _list = new List<Point>();

		#endregion [ Private Variables ]

		#region [ Properties ]

		[XmlText]
		public string Default
		{
			get { return FromBaseType(_list); }
			set { _list = ToBaseType(value); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public XmlPointList()
		{ }

		public XmlPointList(List<Point> l) 
		{ 
			_list = l; 
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public static string FromBaseType(XmlPointList pl)
		{
			List<byte> Encoded = new List<byte>();

			foreach (Point p in pl._list)
			{
				Encoded.AddRange(BitConverter.GetBytes(Channels.BaseChannel.SerializeCell(p)));
			}
			return Convert.ToBase64String(Encoded.ToArray());
		}

		public static XmlPointList ToBaseType(string value)
		{
			byte[] bytes = Convert.FromBase64String(value);
			List<Point> List = new List<Point>();
			for (int i = 0; i < bytes.Length; i += 4)
			{
				List.Add(Channels.BaseChannel.DecodeCell(BitConverter.ToUInt32(bytes, i)));
			}
			return List;
		}

		#endregion [ Methods ]

		#region [ Operators ]

		public static implicit operator List<Point>(XmlPointList p)
		{
			return p._list;
		}

		public static implicit operator XmlPointList(List<Point> list)
		{
			return new XmlPointList(list);
		}

		#endregion [ Operators ]
	}
}
