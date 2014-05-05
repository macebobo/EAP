using System.Collections.Generic;
using System.Drawing;

#pragma warning disable 1591

namespace ElfCore
{
	///// <summary>
	///// http://msdn.microsoft.com/en-us/magazine/cc164113.aspx
	///// </summary>
	//public class ChannelEventArgs : System.EventArgs
	//{
	//    #region [ Properties ]

	//    public List<Channel> Channels { get; set; }
	//    public EventSubCategory Category { get; set; }

	//    #endregion [ Properties ]

	//    #region [ Constructors ]

	//    public ChannelEventArgs(EventSubCategory subcategory, List<Channel> channels)
	//    {
	//        this.Channels = channels;
	//    }

	//    public ChannelEventArgs(EventSubCategory subcategory, Channel channel)
	//        : base(category, subcategory)
	//    {
	//        this.Channels = new List<Channel>();
	//        this.Channels.Add(channel);
	//    }

	//    #endregion [ Constructors ]
	//}

	///// <summary>
	///// http://msdn.microsoft.com/en-us/magazine/cc164113.aspx
	///// </summary>
	//public class DataEventArgs : System.EventArgs
	//{		
	//    #region [ Properties ]

	//    public EventCategory Category { get; set; }
	//    public EventSubCategory SubCategory { get; set; }
	//    public Point AttentionPoint { get; set; }

	//    #endregion [ Properties ]

	//    #region [ Constructors ]

	//    public DataEventArgs(EventCategory category, EventSubCategory subcategory)
	//    {
	//        this.Category = category;
	//        this.SubCategory = subcategory;
	//    }

	//    public DataEventArgs(EventCategory category, EventSubCategory subcategory, Point attentionPoint)
	//        : this(category, subcategory)
	//    {
	//        this.AttentionPoint = attentionPoint;
	//    }

	//    #endregion [ Constructors ]
	//}

	/// <summary>
	/// http://msdn.microsoft.com/en-us/magazine/cc164113.aspx
	/// </summary>
	public class UndoEventArgs : System.EventArgs
	{
		#region [ Properties ]

		public string Text { get; set; }
		public bool HasData { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public UndoEventArgs()
		{
			this.Text = string.Empty;
			this.HasData = false;
		}

		public UndoEventArgs(string text, bool hasData)
		{
			this.Text = text;
			this.HasData = hasData;
		}

		#endregion [ Constructors ]
	}

	public class ZoomEventArgs : System.EventArgs
	{
		#region [ Properties ]

		public Point ZoomPoint = new Point();
		public float ZoomLevel = 1.0f;

		#endregion [ Properties ]

		#region [ Constructors ]

		public ZoomEventArgs()
		{ }

		public ZoomEventArgs(Point zoomPoint, float zoomLevel)
		{
			this.ZoomPoint = zoomPoint;
			this.ZoomLevel = zoomLevel;
		}

		#endregion [ Constructors ]
	}
}