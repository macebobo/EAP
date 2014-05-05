using System.ComponentModel;
using System.Drawing;

namespace ElfCore.Channels
{
	[DefaultPropertyAttribute("Name")]
	public class Properties
	{

		#region [ Private Variables ]

		private int _outputID;
		private Color _borderColor;
		private Color _sequencerColor;
		private Color _renderColor;
		private bool _enabled;
		private bool _locked;
		private string _name;
		private bool _visible;
		private int _cellCount;
		public Rectangle _bounds;

		#endregion [ Private Variables ]

		#region [ Properties ]

		[DescriptionAttribute("ID of the Channel")]
		public int OutputID { get; set; }

		[DescriptionAttribute("Name of the Channel")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[CategoryAttribute("Appearance"), DescriptionAttribute("Color of the Cells in the Sequencer")]
		public Color SequencerColor
		{
			get { return _sequencerColor; }
			set { _sequencerColor = value; }
		}

		[CategoryAttribute("Appearance"), DescriptionAttribute("Color of the Cells in the Renderer")]
		public Color RenderColor
		{
			get { return _renderColor; }
			set { _renderColor = value; }
		}

		[CategoryAttribute("Appearance"), DescriptionAttribute("Border Color of the Cells")]
		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}
		

		[CategoryAttribute("Behavior"), DescriptionAttribute("Determines whether the Channel is visible or hidden during playback.")]
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		[CategoryAttribute("Behavior"), DescriptionAttribute("Determines whether the Channel is visible or hidden while editing.")]
		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		[CategoryAttribute("Behavior"), DescriptionAttribute("Determines whether the Channel can be altered during editing.")]
		public bool Locked
		{
			get { return _locked; }
			set { _locked = value; }
		}

		[CategoryAttribute("Data"), DescriptionAttribute("Number of cells currently for this Channel.")]
		public int CellCount
		{
			get { return _cellCount; }
			private set { _cellCount = value; }
		}

		[CategoryAttribute("Data"), DescriptionAttribute("Boundary of all the cells for this Channel.")]
		public Rectangle Bounds
		{
			get { return _bounds; }
			private set { _bounds = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public Properties()
		{
			_borderColor = Color.Empty;
			_sequencerColor = Color.White;
			_enabled = true;
			_locked = false;
			_name = "[New Channel]";
			_visible = true;
			_cellCount = 0;
			_outputID = 0;
			_bounds = Rectangle.Empty;
		}

		public Properties(Channel channel)
			: this()
		{
			_borderColor = channel.BorderColor;
			_sequencerColor = channel.RenderColor;
			_enabled = channel.Enabled;
			_locked = channel.Locked;
			_name = channel.Name;
			_visible = channel.Visible;
			_cellCount = channel.Lattice.Count;
			_bounds = channel.GetBounds();
			_outputID = channel.ID;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void Save(Channel channel)
		{
			if (!_locked)
				channel.Locked = _locked;
			channel.BorderColor = _borderColor;
			channel.SequencerColor = _sequencerColor;
			channel.Enabled = _enabled;
			channel.Name = _name;
			channel.Visible = _visible;
			channel.ID = _outputID;
			if (_locked)
				channel.Locked = _locked;
		}

		#endregion [ Methods ]
	}
}
