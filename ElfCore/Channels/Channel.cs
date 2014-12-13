using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

using ElfCore.XmlSerializers;

using LatticePoint = System.Drawing.Point;

namespace ElfCore.Channels
{
	[Serializable, DebuggerDisplay("Name = {_name}")]
	public class Channel : ElfBase, ICloneable, INotifyPropertyChanged, IComparable, IItem
	{

		#region [ Enums ]

		public enum SaveMode
		{
			ChannelObjects,
			Profile
		}

		#endregion [ Enums ]

		#region [ Constants ]

		// Property Names
		public const string Property_BorderColor = "BorderColor";
		public const string Property_RenderColor = "RenderColor";
		public const string Property_SequencerColor = "SequencerColor";
		public const string Property_Enabled = "Enabled";
		public const string Property_Grouped = "Grouped";
		public const string Property_ID = "ID";
		public const string Property_Included = "Included";
		public const string Property_Lattice = "Lattice";
		public const string Property_Locked = "Locked";
		public const string Property_Name = "Name";
		public const string Property_Origin = "Origin";
		public const string Property_Selected = "Selected";
		public const string Property_Visible = "Visible";


		#endregion [ Constants ]

		#region [ Protected Variables ]

		[XmlIgnore()]
		protected XmlHelper _xmlHelper = XmlHelper.Instance;

		[XmlIgnore()]
		protected Workshop _workshop = Workshop.Instance;

		protected bool _grouped = false;

		/// <summary>
		/// Output Channel ID for this Channel as stored in the Profile
		/// ˂Channel color="-32768" output="3" id="634798630941625207" enabled="True"˃Big Jack Mouth Mid˂/Channel˃
		/// id attribute in this node appears to be the create time of the Channel, as expressed in Ticks, and is not unique within the Profile.
		/// </summary>
		protected int _id = -1;

		protected Color _sequencerColor = Color.Empty;
		protected Color _renderColor = Color.Empty;
		protected Color _borderColor = Color.Empty;
		protected Point _origin = new Point(0, 0);
		protected string _name = string.Empty;
		protected bool _visible = true;
		protected bool _enabled = true;
		protected bool _included = true;
		protected bool _locked = false;
		protected bool _loading = false;

		/// <summary>
		/// Hide is a temporary version of visible, as some process need to momentary hide the channel to do a screen grab.
		/// </summary>
		protected bool _hidden = false;
		protected bool _isSelected = false;

		[NonSerialized()]
		protected GraphicsPath _graphicsPath = new GraphicsPath();

		protected List<LatticePoint> _lattice = new List<LatticePoint>();

		[NonSerialized()]
		protected ChannelList _subChannels = new ChannelList();

		protected string _cxImageKey = null;

		/// <summary>
		/// Indicates whether the Lattice has changed in any way since that last time the GraphicsPath object has been created. Reset when the GraphicsPath object is rebuilt.
		/// </summary>
		protected bool _latticeChanged = true;

		#endregion [ Protected Variables ]

		#region [ Properties ]

		/// <summary>
		/// Transparency level of the channel as it's drawn.
		/// </summary>
		[XmlIgnore()]
		public byte Alpha { get; set; }

		/// <summary>
		/// Color to use to draw the Channel object.
		/// </summary>
		[XmlIgnore()]
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				if (!CanEdit() || _borderColor.Equals(value))
					return;
				_borderColor = value;
				ChannelExplorerImageKey = GenCXImageKey();
				OnPropertyChanged(Property_BorderColor, true);
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("BorderColor")]
		public string BorderColorSerialized
		{
			get { return XmlColor.FromBaseType(_borderColor); }
			set { _borderColor = XmlColor.ToBaseType(value); }
		}

		/// <summary>
		/// Key used to find the correct color swatch for this Channel
		/// </summary>
		public string ChannelExplorerImageKey
		{
			get
			{
				if (_cxImageKey == null)
					ChannelExplorerImageKey = GenCXImageKey();
				return _cxImageKey;
			}
			set { _cxImageKey = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Channel is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;
				_enabled = value;
				ChannelExplorerImageKey = GenCXImageKey();
				OnPropertyChanged(Property_Enabled, true);
			}
		}

		/// <summary>
		/// Indicates whether this Channel is grouped with other Channels.
		/// </summary>
		public bool Grouped
		{
			get { return _grouped; }
			set
			{
				if (!CanEdit() || (_grouped == value))
					return;
				_grouped = value;
				OnPropertyChanged(Property_Grouped, true);
			}
		}

		/// <summary>
		/// Indicates whether there is Lattice data defined for this Channel.
		/// </summary>
		[XmlIgnore()]
		public bool HasLatticeData
		{
			get { return (_lattice.Count > 0); }
		}

		/// <summary>
		/// Indicates whether there is Vector data defined for this Channel.
		/// </summary>
		[XmlIgnore()]
		public bool HasVectorData
		{
			get { return false; }
		}

		/// <summary>
		/// Unique Output ID for this Channel. Corresponds to the "output" attribute in the Profile Xml
		/// </summary>
		public int ID
		{
			get { return _id; }
			set
			{
				if (!CanEdit() || (_id == value))
					return;
				_id = value;
				OnPropertyChanged(Property_ID, true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Channel is included in this Profile.
		/// </summary>
		public bool Included
		{
			get { return _included; }
			set
			{
				if (_included == value)
					return;
				_included = value;
				ChannelExplorerImageKey = GenCXImageKey();
				OnPropertyChanged(Property_Included, true);
			}
		}

		/// <summary>
		/// Position in the Active SortedList
		/// </summary>
		[XmlIgnore()]
		public int Index { get; set; }

		/// <summary>
		/// Indicates whether this Channel is hidden and does not render.
		/// </summary>
		[XmlIgnore()]
		public bool IsHidden
		{
			get { return _hidden; }
		}

		/// <summary>
		/// Indicates whether this Channel is selected in the Channel Explorer.
		/// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (_isSelected)
						OnPropertyChanged(Property_Selected, false);
				}
			}
		}

		/// <summary>
		/// List of Cells points that represent the pseudo-bitmap mode of rendering.
		/// </summary>
		[XmlIgnore()]
		public List<LatticePoint> Lattice
		{
			get { return _lattice; }
			set
			{
				if (!Workshop.ListEqual<LatticePoint>(_lattice, value))
				{
					_lattice = new List<LatticePoint>();
					if (value != null)
					{
						_lattice = new List<LatticePoint>();
						_lattice.AddRange(value);
					}
					OnLatticeChanged();
				}
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("Lattice")]
		public string LatticeSerialized
		{
			get { return SerializeLattice(false, true); }
			set { DeserializeLattice(value); }
		}

		/// <summary>
		/// Bitmap object of the Lattice Buffer for this Channel
		/// </summary>
		[XmlIgnore()]
		public Bitmap LatticeBuffer
		{
			get { return CreateLatticeBuffer(); }
			set { PopulateFromLatticeBuffer(value); }
		}

		/// <summary>
		/// Sets an indicator to this Channel that it needs to refresh its GraphicsPath
		/// </summary>
		[XmlIgnore()]
		public bool LatticeChanged
		{
			set { _latticeChanged = value; }
		}

		/// <summary>
		/// Indicates whether the data is being loaded in.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore()]
		public bool Loading
		{
			get { return _loading; }
			set
			{
				_loading = value;
				SuppressEvents = value;
			}
		}

		/// <summary>
		/// Indicates whether this Channel is Locked. If so, it is immune to any form of edit.
		/// </summary>
		[XmlIgnore()]
		public bool Locked
		{
			get { return _locked; }
			set
			{
				if (_locked != value)
				{
					_locked = value;
					ChannelExplorerImageKey = GenCXImageKey();
					OnPropertyChanged(Property_Locked, true);
				}
			}
		}

		/// <summary>
		/// Name used to describe this Channel. If this is a normal Channel, then it retrieves the Name property within the Vixen.Channel object this wraps, otherwise
		/// sets and returns a local _name variable
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (!CanEdit() || (_name == value))
					return;
				_name = value;
				OnPropertyChanged(Property_Name, true);
			}
		}

		/// <summary>
		/// Point offset from where the Channel should be drawn from
		/// </summary>
		public Point Origin
		{
			get { return _origin; }
			set
			{
				if (!CanEdit() || _origin.Equals(value))
					return;
				_origin = value;
				OnPropertyChanged(Property_Origin, true);
			}
		}

		/// <summary>
		/// Profile that owns this Channel.
		/// </summary>
		[XmlIgnore()]
		public BaseProfile Profile { get; set; }

		/// <summary>
		/// Base color used to render the Channel.
		/// </summary>
		[XmlIgnore()]
		public Color RenderColor
		{
			get { return _renderColor; }
			set
			{
				if (!CanEdit())
					return;
				if (!_renderColor.Equals(value))
				{
					_renderColor = value;
					ChannelExplorerImageKey = GenCXImageKey();
					OnPropertyChanged(Property_RenderColor, true);
				}
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("RenderColor")]
		public string RenderColorSerialized
		{
			get { return XmlColor.FromBaseType(_renderColor); }
			set { _renderColor = XmlColor.ToBaseType(value); }
		}

		/// <summary>
		/// Color used by the sequencing program for displaying the Channel.
		/// </summary>
		[XmlIgnore()]
		public Color SequencerColor
		{
			get { return _sequencerColor; }
			set
			{
				if (!CanEdit())
					return;
				if (!_sequencerColor.Equals(value))
				{
					_sequencerColor = value;
					ChannelExplorerImageKey = GenCXImageKey();
					OnPropertyChanged(Property_SequencerColor, true);
				}
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("SequencerColor")]
		public string SequencerColorSerialized
		{
			get { return XmlColor.FromBaseType(_sequencerColor); }
			set { _sequencerColor = XmlColor.ToBaseType(value); }
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore()]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<Channel>(this);
				return base.Serialized;
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("Vector")]
		public string VectorSerialized
		{
			get { return SerializeVector(); }
			set { DeserializeVector(value); }
		}

		/// <summary>
		/// Indicates whether this Channel can be rendered.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (value == _visible)
					return;
				_visible = value;
				ChannelExplorerImageKey = GenCXImageKey();
				OnPropertyChanged(Property_Visible, false);
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		[DebuggerHidden()]
		public Channel()
			: base()
		{
			base.SuppressEvents = true;
			_locked = false;
			_isSelected = false;
			_id = -1;
			Index = -1;
			_visible = true;
			_enabled = true;
			_included = true;
			_lattice = new List<LatticePoint>();
			_sequencerColor = Color.White;
			_borderColor = Color.Empty;
			_renderColor = Color.Empty;
			base.Dirty = false;
			base.SuppressEvents = false;
		}

		[DebuggerHidden()]
		public Channel(int index)
			: this()
		{
			Index = index;
		}

		public Channel(RawChannel rawChannel)
			: this()
		{
			base.SuppressEvents = true;
			_loading = true;
			_borderColor = rawChannel.BorderColor;
			_enabled = rawChannel.Enabled;
			_id = rawChannel.ID;
			_included = rawChannel.Included;
			_locked = rawChannel.Locked;
			_name = rawChannel.Name;
			_renderColor = rawChannel.RenderColor;
			_sequencerColor = rawChannel.SequencerColor;
			_visible = rawChannel.Visible;
			LatticeSerialized = rawChannel.EncodedRasterData;
			VectorSerialized = rawChannel.EncodedVectorData;
			_loading = false;
			base.SuppressEvents = false;
		}

		#endregion [ Constructors ]

		#region [ Private Methods ]

		private bool CanEdit()
		{
			if (_loading)
				return true;
			return (!_locked & _enabled & _included & _visible);
		}

		#endregion [ Private Methods ]

		#region [ Protected Methods ]

		private string GenCXImageKey()
		{
			return ImageHandler.CreateAndAddColorSwatch(new Swatch(GetColor(), _borderColor, _locked, _included, _enabled, _visible));
		}

		/// <summary>
		/// Generate a bitmap based on the data
		/// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
		/// There is no grid and zoom and paint points are 1x1 pixels
		/// </summary>
		/// <returns></returns>
		protected Bitmap CreateLatticeBuffer()
		{
			if (Profile != null)
				return CreateLatticeBuffer(Color.Black, Profile.Scaling.LatticeSize);
			else
				return null;
		}

		/// <summary>
		/// Generate a bitmap based on the data.
		/// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
		/// There is no grid and zoom and paint points are 1x1 pixels
		/// </summary>
		/// <param name="backgroundColor">Color to use as the image background.</param>
		protected Bitmap CreateLatticeBuffer(Color backgroundColor)
		{
			return CreateLatticeBuffer(backgroundColor, Profile.Scaling.LatticeSize);
		}

		/// <summary>
		/// Generate a bitmap based on the data.
		/// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
		/// There is no grid and zoom and paint points are 1x1 pixels
		/// </summary>
		/// <param name="backgroundColor">Color to use as the image background.</param>
		protected Bitmap CreateLatticeBuffer(Color backgroundColor, Size size)
		{
			Rectangle Bounds = new Rectangle(0, 0, size.Width, size.Height);
			Bitmap Bmp = new Bitmap(Bounds.Width, Bounds.Height);

			using (Graphics g = Graphics.FromImage(Bmp))
				g.Clear(backgroundColor);	// paint the background 

			LockBitmap lockBitmap = new LockBitmap(Bmp);
			lockBitmap.LockBits();

			foreach (LatticePoint pt in _lattice)
			{
				if (Bounds.Contains(pt))
					lockBitmap.SetPixel(pt.X, pt.Y, Color.White);
			}

			lockBitmap.UnlockBits();
			lockBitmap = null;
			return Bmp;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			_lattice = null;
			_workshop = null;
			if (_graphicsPath != null)
			{
				_graphicsPath.Dispose();
				_graphicsPath = null;
			}
		}

		#endregion [ Protected Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Clone this Channel and its underlying objects.
		/// </summary>
		public override object Clone()
		{
			object MyClone = base.Clone();

			// MemberwiseClone copies references, so we need to create a new Lattice for the Clone
			((Channel)MyClone).Lattice = new List<LatticePoint>();
			((Channel)MyClone).Lattice.AddRange(Lattice);
			return MyClone;
		}

		/// <summary>
		/// Implement the IComparable interface method
		/// </summary>
		public int CompareTo(object obj)
		{
			if (obj is Channel)
			{
				return ID.CompareTo((obj as Channel).ID);
			}
			else
				throw new ArgumentException("Object is not a RasterChannel");

		}

		/// <summary>
		/// Used by Clipboard to Cut cells. The collection cannot be null.
		/// </summary>
		/// <param name="dataList">List of Cells to remove from the Lattice</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void CutData(List<LatticePoint> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null.");

			if (!CanEdit())
				return;

			foreach (LatticePoint Cell in collection)
				_lattice.Remove(Cell);
			OnLatticeChanged();
		}

		/// <summary>
		/// Converts the encoded unsigned integer into a System.Drawing.Point structure.
		/// </summary>
		/// <param name="encodedPixel">Unsigned int, stored in the Lattice.</param>
		public static LatticePoint DecodeCell(uint encodedPixel)
		{
			return new LatticePoint((int)(encodedPixel >> 16), (int)(encodedPixel & 0xffff));
		}

		/// <summary>
		/// Get rid of all extra Cells that are duplicates.
		/// </summary>
		public void DedupeData()
		{
			//Stopwatch watch = new Stopwatch();
			//watch.Start();

			if (!CanEdit())
				return;

			Dirty = true;
			using (Bitmap Lattice = CreateLatticeBuffer())
			{
				if (Lattice != null)
					PopulateFromLatticeBuffer(Lattice);
			}
			//watch.Stop();
			//Debug.WriteLine("DedupePixels: " + watch.ElapsedTicks + " ticks, " + watch.ElapsedMilliseconds + " ms");

		}

		/// <summary>
		/// Converts the string of encoded data from the profile into cell values.
		/// </summary>
		/// <param name="encoded">String of encoded data.</param>
		public void DeserializeLattice(string encoded)
		{
			DeserializeLattice(encoded, true);
		}

		/// <summary>
		/// Converts the string of encoded data from the profile into cell values.
		/// </summary>
		/// <param name="encoded">String of encoded data.</param>
		/// <param name="clear">Indicates whether the existing data should be removed.</param>
		public void DeserializeLattice(string encoded, bool clear)
		{
			//Editor.Stopwatch.Start();
			if (!CanEdit())
				return;

			if (clear)
				_lattice.Clear();

			if (encoded.Length == 0)
			{
				OnLatticeChanged();
				return;
			}

			byte[] bytes = Convert.FromBase64String(encoded);

			for (int i = 0; i < bytes.Length; i += 4)
			{
				_lattice.Add(DecodeCell(BitConverter.ToUInt32(bytes, i)));
			}

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("DecodeChannelBytes: " + Editor.Stopwatch.ElapsedTicks + " ticks, "+ Editor.Stopwatch.ElapsedMilliseconds + " ms");
			if (!clear)
				DedupeData();
			else
				OnLatticeChanged();
		}

		/// <summary>
		/// Decodes the vector data from storage in the Profile Xml document
		/// </summary>
		/// <param name="encoded">Encoded vector data</param>
		public void DeserializeVector(string encoded)
		{
			DeserializeVector(encoded, true);
		}

		/// <summary>
		/// Decodes the vector data from storage in the Profile Xml document
		/// </summary>
		/// <param name="encoded">Encoded vector data</param>
		/// <param name="clear">Indicates whether the existing data should be removed.</param>
		public void DeserializeVector(string encoded, bool clear)
		{ }

		/// <summary>
		/// Determines whether the data within the specified Channel object matches that of the current Channel object. Channel cannot be null.
		/// </summary>
		/// <param name="channel">Channel object to compare to the current one.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public bool Differs(Channel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel cannot be null.");

			return !(Workshop.ListEqual<LatticePoint>(_lattice, channel.Lattice) &&
					SequencerColor.Equals(channel.SequencerColor) &&
					RenderColor.Equals(channel.RenderColor) &&
					BorderColor.Equals(channel.BorderColor) &&
					(Name == channel.Name) &&
					(Locked == channel.Locked) &&
					(Enabled == channel.Enabled) &&
					(Included == channel.Included) &&
					Origin.Equals(channel.Origin) &&
					Visible == channel.Visible &&
					Grouped == channel.Grouped);
		}

		/// <summary>
		/// Generate a bitmap based on the data
		/// The bitmap is equivalent to the one on the CanvasPane in CanvasWindow
		/// NOTE: This function appears in CanvasWindow, BaseTool and Channel
		/// </summary>
		/// <param name="paintColor">Color in which to paint the cells</param>
		public Bitmap DrawChannel(Color paintColor, Color backgroundColor)
		{
			// Create a new image of the right size 
			Size CanvasSize = Profile.Scaling.CanvasSize;
			Bitmap CanvasBuffer = new Bitmap(CanvasSize.Width, CanvasSize.Height);
			using (Graphics g = Graphics.FromImage(CanvasBuffer))
			{
				using (SolidBrush ChannelBrush = new SolidBrush(paintColor))
				{
					g.FillPath(ChannelBrush, GetGraphicsPath());
				}
				return CanvasBuffer;
			}
		}

		/// <summary>
		/// Empties the Channel of all Cells.
		/// </summary>
		public virtual void Empty()
		{
			Empty(false);
		}

		/// <summary>
		/// Empties the Channel of all Cells.
		/// </summary>
		/// <param name="destroyData">Indicates whether the Lattice array should be destroyed and rebuilt</param>
		public virtual void Empty(bool destroyData)
		{
			if (!CanEdit())
				return;

			Dirty = true;
			Origin = new Point(0, 0);
			if (destroyData)
				_lattice = new List<LatticePoint>();
			else
				_lattice.Clear();
			OnLatticeChanged();
		}

		/// <summary>
		/// Erase a single cell from the Lattice array.
		/// Used by the original Adjustable Preview window to erase a cell
		/// </summary>
		/// <param name="cell">Cell to remove from the Lattice</param>
		public void Erase(LatticePoint cell)
		{
			if (!CanEdit())
				return;
			_lattice.Remove(cell);
			OnLatticeChanged();
		}

		/// <summary>
		/// Checks to see if the Render color has been set. If so, return that, else return the sequencer color.
		/// </summary>
		/// <returns></returns>
		public Color GetColor()
		{
			if (_renderColor.IsEmpty)
				return _sequencerColor;
			else
				return _renderColor;
		}
		
		/// <summary>
		/// Returns a rectangle that bounds this Channel.
		/// </summary>
		public Rectangle GetBounds()
		{
			if ((_lattice == null) || (_lattice.Count == 0))
				return Rectangle.Empty;

			int MaxX = int.MinValue;
			int MaxY = int.MinValue;
			int MinX = int.MaxValue;
			int MinY = int.MaxValue;

			foreach (LatticePoint Cell in _lattice)
			{
				MinX = Math.Min(MinX, Cell.X);
				MinY = Math.Min(MinY, Cell.Y);
				MaxX = Math.Max(MaxX, Cell.X);
				MaxY = Math.Max(MaxY, Cell.Y);
			}
			return new Rectangle(MinX, MinY, (MaxX - MinX) + 1, (MaxY - MinY) + 1);
		}

		/// <summary>
		/// Selects the correct color used to render the Channel.
		/// </summary>
		/// <param name="alpha">Amount of transparency to apply to the color. 0 is completely transparent, 255 is completely solid.</param>
		public SolidBrush GetBrush(byte alpha)
		{
			return new SolidBrush(Color.FromArgb(alpha, GetColor()));
		}

		/// <summary>
		/// Selects the correct color used to render the Channel.
		/// </summary>
		public SolidBrush GetBrush()
		{
			return new SolidBrush(GetColor());
		}

		/// <summary>
		/// Creates a graphics path that represents all the cells in the Canvas
		/// </summary>
		public GraphicsPath GetGraphicsPath()
		{
			return GetGraphicsPath(Profile.Scaling);
		}

		/// <summary>
		/// Creates a graphics path that represents all the cells in the Canvas. 
		/// </summary>
		/// <param name="scaling">Precalculated scaling data</param>
		public GraphicsPath GetGraphicsPath(Scaling scaling)
		{
			if (_latticeChanged || (_graphicsPath.PointCount == 0))
			{
				_graphicsPath.Reset();
				int CellSize = scaling.CellSize.GetValueOrDefault(1);
				Rectangle CellRect = new Rectangle(0, 0, CellSize, CellSize);

				int CellGrid = scaling.CellGrid;
				int GridLineWidth = scaling.GridLineWidth;

				foreach (LatticePoint pt in _lattice)
				{
					CellRect.Location = new LatticePoint(GridLineWidth + (pt.X * CellGrid), GridLineWidth + (pt.Y * CellGrid));
					_graphicsPath.AddRectangle(CellRect);
				}
				_latticeChanged = false;

				float Zoom = scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100);
				if (Zoom != Scaling.ZOOM_100)
				{
					// Rescale the path to that of the zoom.
					using (Matrix ScaleMatrix = new Matrix())
					{
						ScaleMatrix.Scale(Zoom, Zoom);
						_graphicsPath.Transform(ScaleMatrix);
					}
				}

			}
			return _graphicsPath;
		}

		/// <summary>
		/// Turns on the Hidden flag to suppress redrawing of this Channel. Use the method <seealso cref="Show">Show()</seealso> to allow normal drawing.
		/// </summary>
		[DebuggerHidden()]
		public void Hide()
		{
			_hidden = true;
		}

		/// <summary>
		/// Populates cells from the image passed in. 
		/// This is the slower method, but seems more robust
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		public void ImportBitmap(Bitmap image)
		{
			ImportBitmap(image, true);
		}

		/// <summary>
		/// Populates cells from the image passed in. 
		/// This is the slower method, but seems more robust
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		/// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
		public virtual bool ImportBitmap(Bitmap image, bool clearFirst)
		{
			if (!CanEdit())
				return false;

			Color CheckPixel;

			if (image == null)
				return false;

			if (clearFirst)
				_lattice.Clear();
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					CheckPixel = image.GetPixel(x, y);
					if (CheckPixel.GetBrightness() > 0.5f)
						_lattice.Add(new LatticePoint(x, y));
				}

			_latticeChanged = true;
			OnPropertyChanged(Property_Lattice, true);
			return true;
		}

		/// <summary>
		/// Offsets the rendering of the Channel.
		/// </summary>
		/// <param name="offset">Amount to offset the rendering.</param>
		public void MoveData(PointF offset)
		{
			if (!CanEdit())
				return;

			List<LatticePoint> NewCells = new List<LatticePoint>();
			Point Offset = new Point((int)offset.X, (int)offset.Y);

			foreach (LatticePoint Cell in _lattice)
			{
				NewCells.Add(new LatticePoint(Cell.X + Offset.X, Cell.Y + Offset.Y));
			}

			Lattice = NewCells;
			OnLatticeChanged();
		}

		/// <summary>
		/// Adds a Cell to the Lattice, then triggers an event to cause the Channel to be redrawn.
		/// </summary>
		/// <param name="cell">Cell to add to the Lattice.</param>
		public void Paint(LatticePoint cell)
		{
			if (!CanEdit())
				return;

			_lattice.Add(cell);
			OnLatticeChanged();
		}

		/// <summary>
		/// Adds a list of Cells to the Lattice, then triggers an event to cause the Channel to be redrawn.
		/// Used by Clipboard to Paste cells. The list of Cells cannot be null.
		/// </summary>
		/// <param name="collection">Lists of Cells to add to the Lattice</param>
		/// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
		public void Paint(List<LatticePoint> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null.");

			if (!CanEdit())
				return;

			_lattice.AddRange(collection);
			//DedupeData();
			OnLatticeChanged();
		}

		/// <summary>
		/// Adds a list of Cells to the Lattice, offset by the given amount, then triggers an event to cause the Channel to be redrawn.
		/// Used by the Image Stamp  and Text tools
		/// </summary>
		/// <param name="collection">Lists of Cells to add to the Lattice</param>
		/// <param name="offset">Offset point to start the painting.</param>
		/// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
		public void Paint(List<LatticePoint> collection, Point offset)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null.");

			if (!CanEdit())
				return;

			// Go through each cell in the list, offset it, and add it to the channel
			LatticePoint Cell;
			for (int i = 0; i < collection.Count; i++)
			{
				Cell = collection[i];
				Cell.Offset(offset);
				collection[i] = Cell;
			}

			_lattice.AddRange(collection);

			DedupeData();
			OnLatticeChanged();
		}

		/// <summary>
		/// Populates cells from the lattice buffer passed in. 
		/// This is the faster method, but doesn't seem to work all that well with non-native bitmapped images.
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		public void PopulateFromLatticeBuffer(Bitmap image)
		{
			PopulateFromLatticeBuffer(image, true);
		}

		/// <summary>
		/// Populates cells from the lattice buffer passed in.
		/// This is the faster method, but doesn't seem to work all that well with non-native bitmapped images.
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		/// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
		public void PopulateFromLatticeBuffer(Bitmap image, bool clearFirst)
		{
			//Editor.Stopwatch.Reset();
			//Editor.Stopwatch.Start();

			if (!CanEdit())
				return;

			LockBitmap lockBitmap = new LockBitmap(image);
			lockBitmap.LockBits();
			Color CheckPixel;

			if (image == null)
				return;
			if (clearFirst)
				_lattice.Clear();
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					CheckPixel = lockBitmap.GetPixel(x, y);
					if (CheckPixel.GetBrightness() > 0.5f)
						_lattice.Add(new LatticePoint(x, y));
				}

			lockBitmap.UnlockBits();
			lockBitmap = null;
			_latticeChanged = true;

			OnPropertyChanged(Property_Lattice, true);

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("PopulateFromLatticeBuffer: " + Editor.Stopwatch.Report());
		}

		/// <summary>
		/// Converts the x,y of a Cell to a value that can be saved in the list.
		/// </summary>
		/// <param name="cell">Cell to serialize.</param>
		public static uint SerializeCell(LatticePoint cell)
		{
			if ((cell.X < 0) || (cell.Y < 0))
				return 0xFFFFFFFF;
			return (uint)((cell.X << 16) | cell.Y);
		}

		/// <summary>
		/// Converts the data array values to a string to be stored in the Profile Xml.
		/// </summary>
		/// <param name="dedupeData">Indicates whether the data should be deduped first.</param>
		/// <returns>Returns cell data encoded into a string.</returns>
		public string SerializeLattice(bool dedupeData)
		{
			return SerializeLattice(dedupeData, false);
		}

		/// <summary>
		/// Converts the data array values to a string to be stored in the Profile Xml.
		/// </summary>
		/// <param name="dedupeData">Indicates whether the data should be deduped first.</param>
		/// <param name="flatten">Indicates whether data from subchannels (if any) should be flattened into the encoded string</param>
		/// <returns>Returns cell data encoded into a string.</returns>
		public string SerializeLattice(bool dedupeData, bool flatten)
		{
			if (dedupeData)
				DedupeData();

			List<byte> ChannelPixelCoords = new List<byte>();

			foreach (LatticePoint Cell in _lattice)
				ChannelPixelCoords.AddRange(BitConverter.GetBytes(SerializeCell(Cell)));

			if (flatten)
			{
				foreach (Channel SubChannel in _subChannels)
				{
					foreach (LatticePoint Cell in SubChannel.Lattice)
						ChannelPixelCoords.AddRange(BitConverter.GetBytes(SerializeCell(Cell)));
				}
			}

			return Convert.ToBase64String(ChannelPixelCoords.ToArray());
		}

		/// <summary>
		/// Converts the Lattice cell data into a string.
		/// </summary>
		public string SerializeLattice()
		{
			return SerializeLattice(true, true);
		}

		/// <summary>
		/// Converts the Vector data into a string.
		/// </summary>
		public string SerializeVector()
		{
			return string.Empty;
		}

		/// <summary>
		/// Turns off the Hidden flag to allow normal redrawing of this Channel
		/// </summary>
		[DebuggerHidden()]
		public void Show()
		{
			_hidden = false;
		}

		/// <summary>
		/// Returns the Name of the Channel
		/// </summary>
		public override string ToString()
		{
			return _name;
		}

		/// <summary>
		/// Returns the Name of the Channel and its ID
		/// </summary>
		/// <param name="includeID">Indicates whether the ID should be included in the output</param>
		public string ToString(bool includeID)
		{
			if (includeID)
				return string.Format("{0}: {1}", ID + 1, ToString());
			else
				return ToString();
		}

		#endregion [ Public Methods ]

		#region [ Public Static Methods ]

		/// <summary>
		/// Generates a GraphicsPath object based on the scaling information and encoded cell data. Used for playback
		/// </summary>
		public static GraphicsPath GeneratePath(Scaling scaling, string encoded)
		{
			GraphicsPath Path = new GraphicsPath();
			if (((encoded ?? string.Empty).Length == 0) || (scaling == null))
				return Path;

			byte[] bytes = Convert.FromBase64String(encoded);
			List<LatticePoint> Lattice = new List<LatticePoint>();
			Point Cell;
			for (int i = 0; i < bytes.Length; i += 4)
			{
				Cell = DecodeCell(BitConverter.ToUInt32(bytes, i));
				if (!Lattice.Contains(Cell))
					Lattice.Add(Cell);
			}

			int CellSize = scaling.CellSize.GetValueOrDefault(1);
			Size CellSize_Size = new Size(CellSize, CellSize);

			int CellGrid = scaling.CellGrid;
			int GridLineWidth = scaling.GridLineWidth;

			foreach (LatticePoint pt in Lattice)
			{
				Path.AddRectangle(new Rectangle(new LatticePoint(GridLineWidth + (pt.X * CellGrid), GridLineWidth + (pt.Y * CellGrid)), CellSize_Size));
			}

			float Zoom = scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100);
			if (Zoom != Scaling.ZOOM_100)
			{
				// Rescale the path to that of the zoom.
				using (Matrix ScaleMatrix = new Matrix())
				{
					ScaleMatrix.Scale(Zoom, Zoom);
					Path.Transform(ScaleMatrix);
				}
			}

			Lattice = null;
			return Path;
		}

		#endregion [ Public Static Methods ]

		#region [ Event Triggers ]

		/// <summary>
		/// Called when the Lattice has been changed. 
		/// </summary>
		[DebuggerHidden()]
		protected void OnLatticeChanged()
		{
			_latticeChanged = true;
			Dirty = true;
			OnPropertyChanged(Property_Lattice, true);
		}

		#endregion [ Event Triggers ]

	}

	#region [ Class ChannelList ]

	public class ChannelList : CollectionBase, IList<Channel>, ICollection<Channel>
	{
		public ChannelList()
		{ }

		public ChannelList(Channel item)
			: this()
		{
			List.Add(item);
		}

		public void Add(Channel item)
		{
			List.Add(item);
		}

		public void AddRange(ChannelList collection)
		{
			foreach (Channel Channel in collection)
				Add(Channel);
		}

		public void AddRange(Channel[] collection)
		{
			for (int i = 0; i < collection.Length; i++)
				Add(collection[i]);
		}

		public bool Contains(Channel item)
		{
			return List.Contains(item);
		}

		public void CopyTo(Channel[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(Channel item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, Channel item)
		{
			List.Insert(index, item);
		}

		public ChannelList OrderByAscending()
		{
			ChannelList ReturnList = new ChannelList();
			Channel[] Arr = ToArray();
			Array.Sort(Arr);
			ReturnList.AddRange(Arr);
			return ReturnList;
		}

		public ChannelList OrderByDescending()
		{
			ChannelList ReturnList = new ChannelList();
			Channel[] Arr = ToArray();
			Array.Sort(Arr);
			Stack<Channel> Stack = new Stack<Channel>();
			for (int i = 0; i < List.Count; i++)
				Stack.Push(Arr[i]);
			while (Stack.Count > 0)
				ReturnList.Add(Stack.Pop());
			Stack = null;
			return ReturnList;
		}

		public void Remove(Channel item)
		{
			if (List.Contains(item))
				List.Remove(item);
		}

		bool ICollection<Channel>.Remove(Channel item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public Channel this[int index]
		{
			get { return (Channel)List[index]; }
			set { List[index] = value; }
		}

		public Channel[] ToArray()
		{
			Channel[] Arr = new Channel[Count];
			for (int i = 0; i < Count; i++)
				Arr[i] = this[i];
			return Arr;
		}

		public Channel Where(int id)
		{
			foreach (Channel item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		public ChannelList WhereList(bool isSelected)
		{
			ChannelList ReturnList = new ChannelList();
			foreach (Channel item in List)
				if (item.IsSelected == isSelected)
					ReturnList.Add(item);
			return ReturnList;
		}

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		#region [ IEnumerable ]

		/// <summary>
		/// Allows for "foreach" statements to be used on an instance of this class, to loop through all the Channels.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<Channel> IEnumerable<Channel>.GetEnumerator()
		{
			return new ChannelListEnumerator(List.GetEnumerator());
		}

		#endregion [ IEnumerable ]

		private class ChannelListEnumerator : IEnumerator<Channel>
		{
			private IEnumerator _enumerator;

			public ChannelListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public Channel Current
			{
				get { return (Channel)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{
			}
		}
		
	}

	#endregion [ Class ChannelList ]
}



