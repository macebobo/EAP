using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ElfCore
{
	/// <summary>
	/// Handles all the profile specific data and properties.
	/// </summary>
	public class Profile
	{

		#region [ Private Variables ]

		private BackgroundImage _backgroundImage = null;
		private CanvasWindow _canvasWindow = null;
		private int _cellSize = 1;
		private bool _dirty = false;
		private string _filename = string.Empty;
		private byte _inactiveChannelAlpha = 128;
		private Size _latticeSize = Size.Empty;
		private string _name = string.Empty;
		private bool _redirectOutputs = false;
		private XmlDocument _xmlDoc = null;
		private XmlHelper _xmlHelper = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public BackgroundImage Background
		{
			get { return _backgroundImage; }
			set { _backgroundImage = value; }
		}

		/// <summary>
		/// Form that is used to display the profile
		/// </summary>
		public CanvasWindow Form
		{
			get { return _canvasWindow; }
			set { _canvasWindow = value; }
		}

		/// <summary>
		/// Object that controls the list of Channels and their various functions
		/// </summary>
		public ChannelController Channels { get; private set; }

		/// <summary>
		/// Size of the Cells (in pixels)
		/// </summary>
		public int CellSize
		{
			get { return _cellSize; }
			set
			{
				if (_cellSize != value)
				{
					Dirty = true;
					_cellSize = value;
					//OnChanged(ProfileEventType.CellSize);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether any of the settings in the is object have been modified
		/// </summary>
		public bool Dirty
		{
			get { return _dirty; }
			set
			{
				if (_dirty != value)
				{
					_dirty = value;
					//if (_dirty)
						//OnChanged(ProfileEventType.Dirty);
				}
			}
		}

		/// <summary>
		/// Amount of alpha (transparency) to use when displaying Channels that are not currently active.
		/// </summary>
		public byte InactiveChannelAlpha
		{
			get { return _inactiveChannelAlpha; }
			set
			{
				if (_inactiveChannelAlpha != value)
				{
					Dirty = true;
					if ((value < 0) || (value > 255))
						value = 128;
					_inactiveChannelAlpha = value;
					//OnChanged(ProfileEventType.InactiveChannelAlpha);
				}
			}
		}

		/// <summary>
		/// Size of the Lattice
		/// </summary>
		public Size LatticeSize
		{
			get { return _latticeSize; }
			set
			{
				if (!_latticeSize.Equals(value) && !value.IsEmpty)
				{
					Dirty = true;
					_latticeSize = value;
				}
			}
		}
		
		/// <summary>
		/// Indicates whether the channel order should respected, or display the channels in their natural, raw order
		/// </summary>
		public bool RedirectOutputs
		{
			get { return _redirectOutputs; }
			set
			{
				if (_redirectOutputs != value)
				{
					Dirty = true;
					_redirectOutputs = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public bool SuppressEvents { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public Profile()
		{
			_backgroundImage = new BackgroundImage();
			_xmlDoc = new XmlDocument();
			_xmlHelper = new XmlHelper();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Loads in the Profile data from the file passed in.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public bool Load(string filename)
		{
			if ((filename ?? string.Empty).Length == 0)
				throw new Exception("Missing filename");

			if (!File.Exists(filename))
				throw new FileNotFoundException("File not found.", filename);

			_xmlDoc.Load(filename);

			_filename = filename;

			//...

			return true;
		}

		/// <summary>
		/// Saves the Profile to file using the current filename
		/// </summary>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public bool Save()
		{
			return Save(_filename);
		}

		/// <summary>
		/// Saves the Profile to file using the filename passed in
		/// </summary>
		/// <param name="filename">Filename to use to save the file to</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public bool Save(string filename)
		{
			if ((filename ?? string.Empty).Length == 0)
				throw new Exception("Missing filename");


			return true;
		}

		#endregion [ Methods ]

		#region [ Event Handlers ]

		///// <summary>
		///// Occurs when the CellSize property changes.
		///// </summary>
		//public EventHandler CellSizeChanged;

		///// <summary>
		///// Occurs when the Dirty property changes.
		///// </summary>
		//public EventHandler DirtyChanged;

		///// <summary>
		///// Occurs when the InactiveChannelAlpha property changes.
		///// </summary>
		//public EventHandler InactiveChannelAlphaChanged;

		///// <summary>
		///// Occurs when the LatticeSize property changes.
		///// </summary>
		//public EventHandler LatticeSizeChanged;

		///// <summary>
		///// Occurs when the ShowGridLines property changes.
		///// </summary>
		//public EventHandler DisplayGridLines;

		#endregion [ Event Handlers ]

		#region [ Custom Event Methods ]

		///// <summary>
		///// Handles the throwing off events, based on the enum passed in
		///// </summary>
		///// <param name="eventType">Enumeration indicating the type of event to throw</param>
		//private void OnChanged(ProfileEventType eventType)
		//{
		//    //if (this.SuppressEvents)
		//    //	return;

		//    switch (eventType)
		//    {
		//        case ProfileEventType.CellSize:
		//            if (CellSizeChanged == null)
		//                return;
		//            CellSizeChanged(this, new System.EventArgs());
		//            break;

		//        case ProfileEventType.Dirty:
		//            if (DirtyChanged == null)
		//                return;
		//            DirtyChanged(this, new System.EventArgs());
		//            break;

		//        case ProfileEventType.InactiveChannelAlpha:
		//            if (InactiveChannelAlphaChanged == null)
		//                return;
		//            InactiveChannelAlphaChanged(this, new System.EventArgs());
		//            break;

		//        case ProfileEventType.LatticeSize:
		//            if (LatticeSizeChanged == null)
		//                return;
		//            LatticeSizeChanged(this, new System.EventArgs());
		//            break;

		//    }
		//}

		#endregion [ Custom Event Methods ]

		#region [ Events ]

		#endregion [ Events ]
	}
}
