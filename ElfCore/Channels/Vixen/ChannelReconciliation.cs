using System.Drawing;

namespace ElfCore.Channels.Vixen
{
	public class ChannelReconciliation
	{
		public string Name { get; set; }
		public Color Color { get; set; }
		public bool Enabled { get; set; }

		public ChannelReconciliation(BaseChannel channel)
		{
			this.Name = channel.Name;
			this.Color = channel.SequencerColor;
			this.Enabled = channel.Enabled;
		}
	}
}
