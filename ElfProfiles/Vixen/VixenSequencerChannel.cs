using System.Drawing;

namespace ElfProfiles.Vixen {
    public class VixenSequencerChannel {
        #region [ Fields ]

        public Color Color { get; set; }
        public byte[] DimmingCurve { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public int OutputChannel { get; set; }

        #endregion [ Fields ]

        #region [ Constructors ]

        public VixenSequencerChannel() {
            Color = Color.White;
            Enabled = true;
            Name = string.Empty;
            OutputChannel = 0;
        }


        public VixenSequencerChannel(string name, Color color, int outputChannel) {
            Name = name;
            Color = color;
            OutputChannel = outputChannel;
        }

        #endregion [ Constructors ]
    }
}