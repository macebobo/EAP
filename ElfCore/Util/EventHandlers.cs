namespace ElfCore.Util
{
	public class EventHandlers
	{
		public delegate void DirtyEventHandler(object sender, DirtyEventArgs e);
		public delegate void ChannelEventHandler(object sender, ChannelEventArgs e);
		public delegate void ChannelListEventHandler(object sender, ChannelListEventArgs e);
		public delegate void ChannelGroupEventHandler(object sender, ChannelGroupEventArgs e);
		public delegate void MultiKeyGestureEventHandler(object sender, MultiKeyGestureEventArgs e);
		public delegate void MessageEventHandler(object sender, MessageEventArgs e);
		public delegate void ProfileEventHandler(object sender, ProfileEventArgs e);
		public delegate void ShuffleEventHandler(object sender, ShuffleEventArgs e);
		public delegate void UndoEventHandler(object sender, UndoEventArgs e);
		public delegate void ZoomEventHandler(object sender, ZoomEventArgs e);
	}
}
