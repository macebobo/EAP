using System;
using ElfCore.Util;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using Vixen;

namespace ElfPreview
{
	public class PlaybackProfile
	{

		#region [ Private Variables ]

		private XmlHelper _xmlHelper = XmlHelper.Instance;
		private List<Color> _colors = new List<Color>();
		private List<GraphicsPath> _paths = new List<GraphicsPath>();
		private List<Vixen.Channel> _channels;
		private int _startChannel = 0;
		private byte[] _alphas;
		private PictureBox _canvasPane;
		private Size _canvasSize;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public byte[] Alphas
		{
			get { return _alphas; }
			set { _alphas = value; }
		}

		public Size CanvasSize
		{
			get { return _canvasSize; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public PlaybackProfile(XmlNode setupNode, List<Vixen.Channel> channels, int startChannel, PictureBox canvasPane)
		{
			_channels = channels;
			_startChannel = startChannel;
			_canvasPane = canvasPane;
			Load(setupNode);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private void Load(XmlNode setupNode)
		{
			Color RendorColor;
			XmlNode CellsNode = null;
			XmlNode RenderColorNode = null;

			int ToChannel = _xmlHelper.GetAttributeValue(setupNode, ElfProfiles.Vixen.GeneralPlugIn.Attribute_To, 0);

			using (ElfCore.Core.Scaling PreviewScaling = ElfProfiles.Vixen.BaseVixen.LoadScaling(setupNode.SelectSingleNode("Scaling")))
			{
				_canvasSize = PreviewScaling.CanvasSize;

				using (ElfCore.Core.Background PreviewBackground = ElfProfiles.Vixen.BaseVixen.LoadBackground(setupNode.SelectSingleNode("Background")))
				{
					if (PreviewBackground.HasData)
					{
						PreviewBackground.BuildCompositeImage(PreviewScaling);

						var dest = new Bitmap(PreviewBackground.CompositeImage.Width, PreviewBackground.CompositeImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
						using (var gr = Graphics.FromImage(dest))
						{
							gr.DrawImage(PreviewBackground.CompositeImage, new Rectangle(Point.Empty, dest.Size));
						}
						if (_canvasPane.Image != null)
							_canvasPane.Dispose();
						_canvasPane.Image = dest;
						PreviewBackground.Dispose();
					}
				}

				for (int i = _startChannel; i < ToChannel; i++)
				{
					RendorColor = Color.HotPink;
					foreach (Vixen.Channel VChannel in _channels)
					{
						if (VChannel.OutputChannel == i)
							RendorColor = VChannel.Color;
					}
					RenderColorNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/RenderColor", i));
					if ((RenderColorNode != null) && (RenderColorNode.InnerText.Length != 0))
					{
						RendorColor = Color.FromArgb(Convert.ToInt32(RenderColorNode.InnerText));
					}
					_colors.Add(RendorColor);
					RenderColorNode = null;

					CellsNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/Cells", i));
					if (CellsNode == null)
					{
						_paths.Add(new GraphicsPath());
						continue;
					}
					using (ElfCore.Channels.Channel Channel = new ElfCore.Channels.Channel())
					{
						Channel.SuppressEvents = true;
						Channel.DeserializeLattice(CellsNode.InnerText, true);
						_paths.Add((GraphicsPath)Channel.GetGraphicsPath(PreviewScaling).Clone());
					}
					CellsNode = null;
				}
			}
		}

		public void Draw(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.Transparent, _canvasPane.ClientRectangle);
			if (_alphas == null)
				return;
			for (int i = 0; i < _alphas.Length; i++)
			{
				using (SolidBrush Brush = new SolidBrush(Color.FromArgb(_alphas[i], _colors[i])))
					e.Graphics.FillPath(Brush, _paths[i]);
			}
		}

		#endregion [ Methods ]

	}
}
