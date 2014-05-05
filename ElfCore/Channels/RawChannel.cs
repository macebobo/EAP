using System.ComponentModel;
using System.Drawing;

namespace ElfCore.Channels
{
	public class RawChannel
	{
		#region [ Properties ]

		[Category("Appearance"), Description("Border Color of the Cells")]
		public Color BorderColor { get; set; }

		//[CategoryAttribute("Data"), DescriptionAttribute("Boundary of all the cells for this Channel.")]
		//public Rectangle Bounds { get; private set; }

		//[CategoryAttribute("Data"), DescriptionAttribute("Number of cells currently for this Channel.")]
		//public int CellCount { get; private set; }

		[Category("Behavior"), Description("Indicated whether the Channel is visible or hidden during playback.")]
		public bool Enabled { get; set; }

		[Category("Data"), Description("Encoded raster data.")]
		public string EncodedRasterData { get; set; }

		[Category("Data"), Description("Encoded vector data.")]
		public string EncodedVectorData { get; set; }
		
		[Description("ID of the Channel")]
		public int ID { get; set; }

		[Category("Behavior"), Description("Indicated whether the Channel is currently included for this Profile configuration.")]
		public bool Included { get; set; }

		[Category("Behavior"), Description("Indicated whether the Channel can be altered during editing.")]
		public bool Locked { get; set; }

		[Category("Data"), Description("Point offset from where the Channel should be drawn from")]
		public Point Origin{ get; set; }
		
		[Description("Name of the Channel")]
		public string Name { get; set; }

		[Category("Appearance"), Description("Color of the Cells in the Renderer")]
		public Color RenderColor { get; set; }

		[Category("Appearance"), Description("Color of the Cells in the Sequencer")]
		public Color SequencerColor { get; set; }

		[Category("Behavior"), Description("Indicated whether the Channel is visible or hidden while editing.")]
		public bool Visible { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public RawChannel()
		{
			BorderColor = Color.Empty;
			//Bounds = Rectangle.Empty;
			//CellCount = 0;
			Enabled = true;
			EncodedRasterData = string.Empty;
			EncodedVectorData = string.Empty;
			ID = -1;
			Included = true;
			Locked = false;
			Name = string.Empty;
			Origin = Point.Empty;
			RenderColor = Color.Empty;
			SequencerColor = Color.White;
			Visible = true;
		}

		#endregion [ Constructors ]

	}
}

