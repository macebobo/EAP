using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Profiles;
using System;
using System.ComponentModel;
using System.Drawing;

#pragma warning disable 1591

namespace ElfCore.Util
{
	// http://msdn.microsoft.com/en-us/magazine/cc164113.aspx

	/// <summary>
	/// Base EventArgs class for handling events dealing an individual Channel.
	/// </summary>
	public class ChannelEventArgs : PropertyChangedEventArgs
	{
		#region [ Properties ]

		public Channel Channel { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelEventArgs(Channel channel, string propertyName)
			: base(propertyName)
		{
			Channel = channel;
		}

		#endregion [ Constructors ]
	}

	/// <summary>
	/// Base EventArgs class for handling events dealing with a list of Channels
	/// </summary>
	public class ChannelListEventArgs : PropertyChangedEventArgs
	{
		#region [ Properties ]

		public ChannelList Channels { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelListEventArgs(ChannelList channels, string propertyName)
			: base(propertyName)
		{
			Channels = channels;
		}

		public ChannelListEventArgs(Channel channel, string propertyName)
			: base(propertyName)
		{
			Channels = new ChannelList();
			Channels.Add(channel);
		}

		public ChannelListEventArgs(ChannelList channels)
			: this(channels, string.Empty)
		{ }

		public ChannelListEventArgs(Channel channel)
			: this(channel, string.Empty)
		{ }

		#endregion [ Constructors ]
	}

	public class ChannelGroupEventArgs : EventArgs
	{
		#region [ Properties ]

		public ChannelGroup ChannelGroup { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelGroupEventArgs(ChannelGroup group)
		{
			ChannelGroup = group;
		}

		#endregion [ Constructors ]
	}
	
	public class DirtyEventArgs : EventArgs
	{
		#region [ Properties ]

		public bool IsDirty { get; private set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public DirtyEventArgs(bool isDirty)
		{
			IsDirty = isDirty;
		}

		#endregion [ Constructors ]
	}

	public class MessageEventArgs : EventArgs
	{
		#region [ Properties ]

		public string Message { get; private set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public MessageEventArgs(string message)
		{
			Message = message;
		}

		#endregion [ Constructors ]
	}


	public class MultiKeyGestureEventArgs : EventArgs
	{
		#region [ Properties ]

		public MultiKeyGesture Gesture { get; private set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public MultiKeyGestureEventArgs(MultiKeyGesture gesture)
		{
			Gesture = gesture;
		}

		#endregion [ Constructors ]
	}

	public class ProfileEventArgs : EventArgs
	{
		#region [ Properties ]

		public BaseProfile Profile { get; private set; }
		public BaseProfile OldProfile { get; private set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public ProfileEventArgs(BaseProfile profile)
		{
			Profile = profile;
			OldProfile = null;
		}

		public ProfileEventArgs(BaseProfile newProfile, BaseProfile oldProfile)
		{
			Profile = newProfile;
			OldProfile = oldProfile;
		}

		#endregion [ Constructors ]
	}

	public class ShuffleEventArgs : EventArgs
	{
		#region [ Properties ]

		public Shuffle Shuffle { get; private set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public ShuffleEventArgs(Shuffle shuffle)
		{
			Shuffle = shuffle;
		}

		#endregion [ Constructors ]
	}

	public class UndoEventArgs : EventArgs
	{
		#region [ Properties ]

		public string Text { get; set; }
		public bool HasData { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public UndoEventArgs()
		{
			Text = string.Empty;
			HasData = false;
		}

		public UndoEventArgs(string text, bool hasData)
		{
			Text = text;
			HasData = hasData;
		}

		#endregion [ Constructors ]
	}

	public class ZoomEventArgs : EventArgs
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
			ZoomPoint = zoomPoint;
			ZoomLevel = zoomLevel;
		}

		#endregion [ Constructors ]
	}


}