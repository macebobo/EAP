using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Util;

namespace ElfProfiles.Vixen {
    public class PlaybackProfile {
        #region [ Private Variables ]

        private readonly PictureBox _canvasPane;
        private readonly List<VixenSequencerChannel> _channels;
        private readonly List<GraphicsPath> _paths = new List<GraphicsPath>();
        private readonly int _startChannel;
        private byte[] _alphas;
        private Background _background;
        private List<Color> _colors = new List<Color>();
        private Scaling _scaling;
        private XmlHelper _xmlHelper = XmlHelper.Instance;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public Size CanvasSize {
            get { return _scaling.CanvasSize; }
        }

        public byte[] Alphas {
            get { return _alphas; }
            set { _alphas = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public PlaybackProfile(XmlNode setupNode, List<VixenSequencerChannel> channels, int startChannel, PictureBox canvasPane) {
            _channels = channels;
            _startChannel = startChannel;
            _canvasPane = canvasPane;
            Load(setupNode);
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        private void Load(XmlNode setupNode) {
            int outputChannel;
            int NumChannels = _channels.Count - _startChannel;

            _scaling = BaseVixen.LoadScaling(setupNode.SelectSingleNode("Scaling"));
            _background = BaseVixen.LoadBackground(setupNode.SelectSingleNode("Background"));
            _background.Set(_scaling, _canvasPane);

            // Remap channel colors
            _colors = new List<Color>();
            for (int i = 0; i < NumChannels; i++) {
                _colors.Add(Color.White);
                _paths.Add(new GraphicsPath());
            }

            for (int channelIndex = _startChannel; channelIndex < _channels.Count; channelIndex++) {
                outputChannel = _channels[channelIndex].OutputChannel;
                if (outputChannel >= _startChannel) {
                    _colors[outputChannel - _startChannel] = _channels[channelIndex].Color;
                }
            }

            // Get channel pixel lists
            // and remap on the fly
            Channel Channel = null;
            XmlNode CellsNode = null;
            XmlNode RenderColorNode = null;
            foreach (XmlNode channelNode in setupNode.SelectNodes("Channels/Channel")) {
                outputChannel = Convert.ToInt32(channelNode.Attributes["output"].Value);
                if (outputChannel < _startChannel) {
                    continue;
                }

                CellsNode = channelNode.SelectSingleNode("Cells");
                if ((CellsNode == null) || (CellsNode.InnerText.Length == 0)) {
                    continue;
                }

                RenderColorNode = channelNode.SelectSingleNode("RenderColor");
                if ((RenderColorNode != null) && (RenderColorNode.InnerText.Length != 0)) {
                    _colors[outputChannel - _startChannel] = Color.FromArgb(Convert.ToInt32(RenderColorNode.InnerText));
                }

                Channel = new Channel();
                Channel.SuppressEvents = true;
                Channel.DeserializeLattice(CellsNode.InnerText, true);
                _paths[_channels[outputChannel - _startChannel].OutputChannel] = Channel.GetGraphicsPath(_scaling);
                Channel = null;
            }
        }


        public void Draw(PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.Transparent, _canvasPane.ClientRectangle);
            if (_alphas == null) {
                return;
            }
            for (int i = 0; i < _paths.Count; i++) {
                using (var Brush = new SolidBrush(Color.FromArgb(_alphas[i], _colors[i]))) {
                    e.Graphics.FillPath(Brush, _paths[i]);
                }
            }
        }

        #endregion [ Methods ]
    }
}