using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Util;

using ElfProfiles.Annotations;
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
            [UsedImplicitly] get { return _alphas; }
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
            var toChannel = _xmlHelper.GetAttributeValue(setupNode, GeneralPlugIn.Attribute_To, 0);

            using (var previewScaling = BaseVixen.LoadScaling(setupNode.SelectSingleNode("Scaling"))) {
                _canvasSize = previewScaling.CanvasSize;

                using (var previewBackground = BaseVixen.LoadBackground(setupNode.SelectSingleNode("Background"))) {
                    if (previewBackground.HasData) {
                        previewBackground.BuildCompositeImage(previewScaling);

                        var dest = new Bitmap(previewBackground.CompositeImage.Width, previewBackground.CompositeImage.Height,
                            PixelFormat.Format32bppPArgb);
                        using (var gr = Graphics.FromImage(dest)) {
                            gr.DrawImage(previewBackground.CompositeImage, new Rectangle(Point.Empty, dest.Size));
                        }
                        if (_canvasPane.Image != null) {
                            _canvasPane.Dispose();
                        }
                        _canvasPane.Image = dest;
                        previewBackground.Dispose();
                    }
                }

                for (var i = _startChannel; i < toChannel; i++) {
                    var rendorColor = Color.HotPink;
                    var currentChannel = i;
                    foreach (var vChannel in _channels.Where(vChannel => vChannel.OutputChannel == currentChannel)) {
                        rendorColor = vChannel.Color;
                    }
                    var renderColorNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/RenderColor", i));
                    if ((renderColorNode != null) && (renderColorNode.InnerText.Length != 0)) {
                        rendorColor = Color.FromArgb(Convert.ToInt32(renderColorNode.InnerText));
                    }
                    _colors.Add(rendorColor);

                    var cellsNode = setupNode.SelectSingleNode(string.Format("Channels/Channel[@output='{0}']/Cells", i));
                    if (cellsNode == null) {
                        _paths.Add(new GraphicsPath());
                        continue;
                    }
                    using (var channel = new Channel()) {
                        channel.SuppressEvents = true;
                        channel.DeserializeLattice(cellsNode.InnerText, true);
                        _paths.Add((GraphicsPath) channel.GetGraphicsPath(previewScaling).Clone());
                    }
                }
            }
        }


        public void Draw(PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.Transparent, _canvasPane.ClientRectangle);
            if (_alphas == null) {
                return;
            }
            for (var i = 0; i < _alphas.Length; i++) {
                using (var brush = new SolidBrush(Color.FromArgb(_alphas[i], _colors[i]))) {
                    e.Graphics.FillPath(brush, _paths[i]);
                }
            }
        }

        #endregion [ Methods ]
    }
}