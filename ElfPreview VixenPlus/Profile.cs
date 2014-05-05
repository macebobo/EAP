using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Util;

using ElfProfiles.Vixen;

using Vixen = VixenPlus;

namespace ElfPreview {
    public class PlaybackProfile {
        #region [ Private Variables ]

        private readonly PictureBox _canvasPane;
        private readonly List<Vixen.Channel> _channels;
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<GraphicsPath> _paths = new List<GraphicsPath>();
        private readonly int _startChannel;
        private readonly XmlHelper _xmlHelper = XmlHelper.Instance;
        private byte[] _alphas;
        private Size _canvasSize;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public byte[] Alphas {
            get { return _alphas; }
            set { _alphas = value; }
        }

        public Size CanvasSize {
            get { return _canvasSize; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public PlaybackProfile(XmlNode setupNode, List<Vixen.Channel> channels, int startChannel, PictureBox canvasPane) {
            _channels = channels;
            _startChannel = startChannel;
            _canvasPane = canvasPane;
            Load(setupNode);
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        private void Load(XmlNode setupNode) {
            Color RendorColor;
            XmlNode CellsNode = null;
            XmlNode RenderColorNode = null;

            int ToChannel = _xmlHelper.GetAttributeValue(setupNode, GeneralPlugIn.Attribute_To, 0);

            using (Scaling PreviewScaling = BaseVixen.LoadScaling(setupNode.SelectSingleNode("Scaling"))) {
                _canvasSize = PreviewScaling.CanvasSize;

                using (Background PreviewBackground = BaseVixen.LoadBackground(setupNode.SelectSingleNode("Background"))) {
                    if (PreviewBackground.HasData) {
                        PreviewBackground.BuildCompositeImage(PreviewScaling);

                        var dest = new Bitmap(PreviewBackground.CompositeImage.Width, PreviewBackground.CompositeImage.Height,
                            PixelFormat.Format32bppPArgb);
                        using (Graphics gr = Graphics.FromImage(dest)) {
                            gr.DrawImage(PreviewBackground.CompositeImage, new Rectangle(Point.Empty, dest.Size));
                        }
                        if (_canvasPane.Image != null) {
                            _canvasPane.Dispose();
                        }
                        _canvasPane.Image = dest;
                        PreviewBackground.Dispose();
                    }
                }

                for (int i = _startChannel; i < ToChannel; i++) {
                    RendorColor = Color.HotPink;
                    foreach (Vixen.Channel VChannel in _channels) {
                        if (VChannel.OutputChannel == i) {
                            RendorColor = VChannel.Color;
                        }
                    }
                    RenderColorNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/RenderColor", i));
                    if ((RenderColorNode != null) && (RenderColorNode.InnerText.Length != 0)) {
                        RendorColor = Color.FromArgb(Convert.ToInt32(RenderColorNode.InnerText));
                    }
                    _colors.Add(RendorColor);
                    RenderColorNode = null;

                    CellsNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/Cells", i));
                    if (CellsNode == null) {
                        _paths.Add(new GraphicsPath());
                        continue;
                    }
                    using (var Channel = new Channel()) {
                        Channel.SuppressEvents = true;
                        Channel.DeserializeLattice(CellsNode.InnerText, true);
                        _paths.Add((GraphicsPath) Channel.GetGraphicsPath(PreviewScaling).Clone());
                    }
                    CellsNode = null;
                }
            }
        }


        public void Draw(PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.Transparent, _canvasPane.ClientRectangle);
            if (_alphas == null) {
                return;
            }
            for (int i = 0; i < _alphas.Length; i++) {
                using (var Brush = new SolidBrush(Color.FromArgb(_alphas[i], _colors[i]))) {
                    e.Graphics.FillPath(Brush, _paths[i]);
                }
            }
        }

        #endregion [ Methods ]
    }
}