using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ElfCore
{
	[Serializable]
	public class Channel : ICloneable, IDisposable
	{

		#region [ Private Variables ]

		private bool _grouped = false;

		/// <summary>
		/// Unique identifier for this Channel
		/// ˂Channel color="-32768" output="3" id="634798630941625207" enabled="True"˃Big Jack Mouth Mid˂/Channel˃
		/// </summary>
		private int _id = -1;

		//private Workshop _workshop = null;

		private Color _color = Color.White;
		private Color _borderColor = Color.Empty;
		private Point _origin = new Point(0, 0);
		private bool _dirty = false;
		private string _name = string.Empty;
		private bool _visible = true;
		private bool _enabled = true;

		/// <summary>
		/// Hide is a temporary version of visible, as some process need to momentary hide the channel to do a screen grab.
		/// </summary>
		private bool _hidden = false;

		private bool _isSelected = false;

		private Workshop _workshop = Workshop.Instance;
		private List<Point> _lattice = new List<Point>();

		[NonSerialized()]
		private bool _disposed = false;
		[NonSerialized()]
		private Vixen.Channel _vixenChannel = null;
		[NonSerialized()]
		private GraphicsPath _graphicsPath = new GraphicsPath();
		[NonSerialized()]
		private bool _latticeChanged = true;
		[NonSerialized()]
		private Bitmap _latticeBuffer = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Color to use to draw the edges of the celled. Used only in special cases
		/// </summary>
		public Color BorderColor
		{
			get { return _borderColor; }
			set 
			{
				if (Locked)
					return;
				if (!_borderColor.Equals(value))
				{
					_borderColor = value;
					this.Dirty = true;
				}
			}
		}

		/// <summary>
		/// Color of the Channel, as displayed on the Vixen grid
		/// </summary>
		public Color Color
		{
			get
			{
				//if (_vixenChannel != null)
				//    return _vixenChannel.Color;
				//else
					return _color; // Used for the move Channel
			}
			set
			{
				if (Locked)
					return;
				if (!_color.Equals(value))
				{
					_color = value;
					this.ColorSwatchKey = value.ToArgb().ToString();
					//if (_vixenChannel != null)
					//    _vixenChannel.Color = value;
					this.Dirty = true;
					ImageController.CreateAndAddColorSwatch(this.Color);
					OnChanged(ChannelEventType.Color);
				}
			}
		}

		/// <summary>
		/// Key used to find the correct color swatch for this channel
		/// </summary>
		public string ColorSwatchKey = string.Empty;

		/// <summary>
		/// Indicates this data has changed
		/// </summary>
		public bool Dirty
		{
			get { return _dirty; }
			set 
			{
				if (Locked)
					return;
				if (_dirty != value)
				{
					_dirty = value;
					if (_dirty)
						OnChanged(ChannelEventType.Dirty);
				}
			}
		}

		/// <summary>
		/// Indicates that this Channel should be used
		/// </summary>
		public bool Enabled
		{
			get 
			{ 
				//return _vixenChannel.Enabled; 
				return _enabled;
			}
			set
			{
				if (Locked)
					return;
				//_vixenChannel.Enabled = value;
				_enabled = value;
			}
		}

		/// <summary>
		/// Is this Channel grouped with others?
		/// </summary>
		public bool Grouped
		{
			get { return _grouped; }
			set 
			{
				if (Locked)
					return;
				_grouped = value; 
			}
		}

		/// <summary>
		/// Unique identifier for this Channel.
		/// </summary>
		public string GUID { get; private set; }

		/// <summary>
		/// Returns true if there is any data in the Lattice
		/// </summary>
		public bool HasData
		{
			get { return (_lattice.Count > 0); }
		}

		/// <summary>
		/// Current position on the sorted Channels list.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Return a flag indicating that this Channel is hidden, do not draw on the Canvas
		/// </summary>
		public bool IsHidden
		{
			get { return _hidden; }
		}

		/// <summary>
		/// List of Cells
		/// </summary>
		public List<Point> Lattice 
		{
			get { return _lattice; }
			set
			{
				if (!Workshop.ListEqual<Point>(_lattice, value))
				{
					_lattice = new List<Point>();
					if (value != null)
					{
						_lattice = new List<Point>();
						_lattice.AddRange(value);
					}
					_latticeChanged = true;
					this.Dirty = true;
				}
			} 
		}

		/// <summary>
		/// Bitmap object of the Lattice Buffer for this Channel
		/// </summary>
		public Bitmap LatticeBuffer
		{
			get { return CreateLatticeBuffer(); }
			set { PopulateFromLatticeBuffer(value); }
		}

		/// <summary>
		/// Sends an indicator to this channel that it needs to refresh its GraphicsPath
		/// </summary>
		public bool LatticeChanged
		{
			set { _latticeChanged = value; }
		}

		/// <summary>
		/// Indicates whether this Channel is Locked. If so, it is immune to any form of edit.
		/// </summary>
		public bool Locked { get; set; }

		/// <summary>
		/// Name used to describe this Channel. If this is a normal Channel, then it retrieves the Name property within the Vixen.Channel object this wraps, otherwise
		/// sets and returns a local _name variable
		/// </summary>
		public string Name
		{
			get
			{
				//if (_vixenChannel != null)
				//    return this.ToString();
				//else
					return _name;
			}
			set
			{
				if (Locked)
					return;
				//if (_vixenChannel != null)
				//{
				//    if (_vixenChannel.Name != value)
				//    {
				//        _vixenChannel.Name = value;
				//        this.Dirty = true;
				//        OnChanged(ChannelEventType.Name);
				//    }
				//}
				//else
				//{
					if (_name != value)
					{
						_name = value;
						this.Dirty = true;
						OnChanged(ChannelEventType.Name);
					}
				//}
			}
		}

		/// <summary>
		/// Point offset from where the Channel should be drawn from
		/// </summary>
		public Point Origin
		{
			get { return _origin; }
			set { _origin = value; }
		}

		/// <summary>
		/// Unique ID for this Channel. Corresponds to the "output" attribute in the Profile Xml
		/// </summary>
		public int ID
		{
			get { return _id; }
			set 
			{
				if (Locked)
					return;
				_id = value; 
			}
		}

		/// <summary>
		/// Indicates that this Channel is selected in the ChannelExplorer
		/// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnChanged(ChannelEventType.Selected);
				}
			}
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public bool SuppressEvents { get; set; }

		/// <summary>
		/// Can this Channel be seen in the editor window
		/// </summary>
		public bool Visible 
		{
			get { return _visible; }
			set
			{
				if (Locked)
					return;
				if (value != _visible)
				{
					_visible = value;
					OnChanged(ChannelEventType.Visibility);
				}
			} 
		}

		/// <summary>
		/// Link to the Vixen.Channel object this object wraps around
		/// </summary>
		public Vixen.Channel VixenChannel
		{
			get { return _vixenChannel; }
			set 
			{ 
				_vixenChannel = value;
				if (_vixenChannel != null)
				{
					_id = _vixenChannel.OutputChannel;
					this.Color = _vixenChannel.Color;
					_name = _vixenChannel.Name;
					_enabled = _vixenChannel.Enabled;
				}
			}
		}

		#endregion [ Properties ]

		#region [ Event Handlers ]

		public System.EventHandler CellsChanged;
		public System.EventHandler ColorChanged;
		public System.EventHandler DirtyChanged;
		public System.EventHandler NameChanged;
		public System.EventHandler SelectedChanged;
		public System.EventHandler VisibilityChanged;

		#endregion [ Event Handlers ]

		#region [ Constructors ]

		public Channel()
		{
			if (_disposed)
				GC.ReRegisterForFinalize(true);
			_disposed = false;

			this.SuppressEvents = true;
				this.Locked = false;			
				this.IsSelected = false;
				this.GUID = Guid.NewGuid().ToString();
				_lattice = new List<Point>();
				this.Index = -1;
				this.Visible = true;
			this.SuppressEvents = false;

			this.Dirty = false;
		}

		public Channel(int index)
			: this()
		{
			this.Index = index;
			this.Dirty = false;
		}

		public Channel(Vixen.Channel vixenChannel)
			: this()
		{
			this.VixenChannel = vixenChannel;
			this.ColorSwatchKey = this.Color.ToArgb().ToString();
			this.Dirty = false;
		}

		//public Channel(Vixen.Channel vixenChannel, int index) 
		//    : this(vixenChannel)
		//{
		//    this.Index = index;
		//}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~Channel()
		{
			//Execute the code that does the cleanup.
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				//_workshop = null;
				_lattice = null;
				_vixenChannel = null;
				if (_graphicsPath != null)
				{
					_graphicsPath.Dispose();
					_graphicsPath = null;
				}
				if (_latticeBuffer != null)
				{
					_latticeBuffer.Dispose();
					_latticeBuffer = null;
				}
			}

			// Remember that we've executed this code
			_disposed = true;
		}

		public void Dispose()
		{
			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);
		}

		#endregion [ Destructors ]

		#region [ Methods ]

		/// <summary>
		/// Erases all the pixels
		/// </summary>
		public void ClearLattice()
		{
			ClearLattice(false);
		}

		/// <summary>
		/// If true, destroys and recreates the Lattice array
		/// </summary>
		public void ClearLattice(bool destroyLattice)
		{
			if (Locked)
				return;

			Dirty = true;
			Origin = new Point(0, 0);
			if (destroyLattice)
				_lattice = new List<Point>();
			else
				_lattice.Clear();
			_latticeChanged = true;

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Clone this Channel, link to the underlying Vixen.Channel object and copy the Lattice points 
		/// </summary>
		public object Clone()
		{
			Channel Cloned = (Channel)this.MemberwiseClone();

			// MemberwiseClone copies references, so we need to create a new Lattice for the Clone
			Cloned.Lattice = new List<Point>();
			Cloned.Lattice.AddRange(this.Lattice);
			//foreach (Point pt in this.Lattice)
				//Cloned.Lattice.Add(pt);
			return Cloned;
		}

		/// <summary>
		/// Generate a bitmap based on the data
		/// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
		/// There is no grid and zoom and paint points are 1x1 pixels
		/// </summary>
		/// <returns></returns>
		private Bitmap CreateLatticeBuffer()
		{
			//Editor.Stopwatch.Start();
			
			Rectangle Bounds = new Rectangle(0, 0, UISettings.ʃLatticeSize.Width, UISettings.ʃLatticeSize.Height);
			Bitmap Bmp = new Bitmap(Bounds.Width, Bounds.Height);
			
			using (Graphics g = Graphics.FromImage(Bmp))
				g.Clear(Color.Black);	// paint the background 

			LockBitmap lockBitmap = new LockBitmap(Bmp);
			lockBitmap.LockBits();

			foreach (Point pt in _lattice)
			{
				if (Bounds.Contains(pt))
					lockBitmap.SetPixel(pt.X, pt.Y, Color.White);
			}

			lockBitmap.UnlockBits();
			lockBitmap = null;

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("CreateLatticeBuffer: " + Editor.Stopwatch.Report());
			return Bmp;
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
			Bitmap CanvasBuffer = new Bitmap(UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height);
			using (Graphics g = Graphics.FromImage(CanvasBuffer))
			{
				using (SolidBrush ChannelBrush = new SolidBrush(paintColor))
				{
					Rectangle CellRect = Rectangle.Empty;

					// Paint the background 
					g.Clear(backgroundColor);

					foreach (Point Cell in _lattice)
					{
						if ((Cell.X * UISettings.ʃCellScale + UISettings.ʃCellZoom < UISettings.ʃCanvasSize.Width) &&
							(Cell.Y * UISettings.ʃCellScale + UISettings.ʃCellZoom < UISettings.ʃCanvasSize.Height))
						{
							CellRect = new Rectangle(Cell.X * UISettings.ʃCellScale, Cell.Y * UISettings.ʃCellScale, UISettings.ʃCellZoom, UISettings.ʃCellZoom);
							g.FillRectangle(ChannelBrush, CellRect);
							if (!_borderColor.IsEmpty)
								using (Pen BorderPen = new Pen(_borderColor))
									g.DrawRectangle(BorderPen, CellRect);
						}
					}
				}
				return CanvasBuffer;
			}
		}

		/// <summary>
		/// Converts the encoded unsigned integer into a Point
		/// </summary>
		/// <param name="encodedPixel">Unsigned int, stored in the pixel array</param>
		public static Point DecodeCell(uint encodedPixel)
		{
			return new Point((int)(encodedPixel >> 16), (int)(encodedPixel & 0xffff));
		}

		/// <summary>
		/// Converts the string of encoded data from the profile into pixel values.
		/// </summary>
		public void DecodeChannelBytes(string encoded)
		{
			DecodeChannelBytes(encoded, true);
		}

		/// <summary>
		/// Converts the string of encoded data from the profile into pixel values.
		/// </summary>
		public void DecodeChannelBytes(string encoded, bool clearCells)
		{
			//Editor.Stopwatch.Start();
			if (Locked)
				return;

			byte[] bytes = Convert.FromBase64String(encoded);
			if (clearCells)
				_lattice.Clear();
			for (int i = 0; i < bytes.Length; i += 4)
			{
				_lattice.Add(DecodeCell(BitConverter.ToUInt32(bytes, i)));
			}

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("DecodeChannelBytes: " + Editor.Stopwatch.ElapsedTicks + " ticks, "+ Editor.Stopwatch.ElapsedMilliseconds + " ms");
			if (!clearCells)
				DedupePixels();

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Get rid of all extra pixels that are duplicates
		/// </summary>
		public void DedupePixels()
		{
			//Stopwatch watch = new Stopwatch();
			//watch.Start();

			if (Locked)
				return;

			Dirty = true;
			PopulateFromLatticeBuffer(CreateLatticeBuffer());			

			//watch.Stop();
			//Debug.WriteLine("DedupePixels: " + watch.ElapsedTicks + " ticks, " + watch.ElapsedMilliseconds + " ms");

		}

		/// <summary>
		/// Converts the x,y of a pixel to a value that can be saved in the list
		/// </summary>
		/// <param name="pixel">Point struct holding the coordinates of the pixel</param>
		public static uint EncodeCell(Point pixel)
		{
			if ((pixel.X < 0) || (pixel.Y < 0))
				return 0xFFFFFFFF;
			return (uint)((pixel.X << 16) | pixel.Y);
		}

		/// <summary>
		/// Converts the pixel values to a string to be stored in the profile Xml
		/// </summary>
		public string EncodeChannelBytes()
		{
			DedupePixels();
			//Stopwatch watch = new Stopwatch();
			//watch.Start();
	
			List<byte> ChannelPixelCoords = new List<byte>();

			foreach (Point Cell in _lattice)
			{
				ChannelPixelCoords.AddRange(BitConverter.GetBytes(EncodeCell(Cell)));
			}
			return Convert.ToBase64String(ChannelPixelCoords.ToArray());

			//string Converted = Convert.ToBase64String(ChannelPixelCoords.ToArray());
			//watch.Stop();
			//Debug.WriteLine("EncodeChannelBytes: " + watch.ElapsedTicks + " ticks, " + watch.ElapsedMilliseconds + " ms");
			//return Converted;
		}

		/// <summary>
		/// Used by Clipboard to Cut cells
		/// </summary>
		/// <param name="cells">List of points to remove from the Lattice</param>
		public void EraseCell(List<Point> cells)
		{
			if (Locked)
				return;

			foreach (Point Cell in cells)
				_lattice.Remove(Cell);
			_latticeChanged = true;

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Used by the original Adjustable Preview window to erase a cell
		/// </summary>
		/// <param name="cell">Point to remove from the Lattice</param>
		public void EraseCell(Point cell)
		{
			if (Locked)
				return;

			_lattice.Remove(cell);
			_latticeChanged = true;
			
			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Determines whether the data within the specified Channel object matches that of the current Channel object
		/// </summary>
		/// <param name="other">The Channel object to compare to the current one.</param>
		public bool Differs(Channel other)
		{
			return !(Workshop.ListEqual<Point>(_lattice, other.Lattice) &&
					this.Color.Equals(other.Color) &&
					(this.Name == other.Name) &&
					(this.Locked == other.Locked) &&
					this.Origin.Equals(other.Origin) &&
					this.Visible == other.Visible &&
					this.Grouped == other.Grouped);
		}

		/// <summary>
		/// Determine the bounding rectangle for all the cells defined.
		/// </summary>
		/// <returns></returns>
		public Rectangle GetBounds()
		{
			if ((_lattice == null) || (_lattice.Count == 0))
				return Rectangle.Empty;

			int MaxX = int.MinValue;
			int MaxY = int.MinValue;
			int MinX = int.MaxValue;
			int MinY = int.MaxValue;
		
			foreach (Point Cell in _lattice)
			{
				MinX = Math.Min(MinX, Cell.X);
				MinY = Math.Min(MinY, Cell.Y);
				MaxX = Math.Max(MaxX, Cell.X);
				MaxY = Math.Max(MaxY, Cell.Y);
			}
			//return new Rectangle(MinX, MinY, (MaxX - MinX), (MaxY - MinY));
			return new Rectangle(MinX, MinY, MaxX, MaxY);
		}

		/// <summary>
		/// Creates a graphics path that represents all the cells in the Canvas
		/// </summary>
		public GraphicsPath GetGraphicsPath()
		{
			if (_latticeChanged || (_graphicsPath.PointCount == 0))
			{
				_graphicsPath.Reset();
				Rectangle CellRect = new Rectangle(0, 0, UISettings.ʃCellZoom, UISettings.ʃCellZoom);

				foreach (Point pt in _lattice)
				{
					CellRect.Location = new Point(pt.X * UISettings.ʃCellScale, pt.Y * UISettings.ʃCellScale);
					_graphicsPath.AddRectangle(CellRect);
				}
				_latticeChanged = false;
			}
			return _graphicsPath;
		}

		/// <summary>
		/// Turns on the Hidden flag to suppress redrawing of this Channel. Use the method <seealso cref="Show">Show()</seealso> to allow normal drawing.
		/// </summary>
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
		public void ImportBitmap(Bitmap image, bool clearFirst)
		{
			if (Locked)
				return;

			Color CheckPixel;

			if (image == null)
				return;
			if (clearFirst)
				_lattice.Clear();
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					CheckPixel = image.GetPixel(x, y);
					if (CheckPixel.GetBrightness() > 0.5f)
						//if ((CheckPixel.R >= 128) && (CheckPixel.G >= 128) && (CheckPixel.B >= 128))
						_lattice.Add(new Point(x, y));
				}

			_latticeChanged = true;

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Used in both Crop and Editor to move all the points within the Lattice
		/// </summary>
		/// <param name="offset">Amount to offset the cells within the Lattice</param>
		public void MoveCells(PointF offset)
		{
			if (Locked)
				return;

			List<Point> NewCells = new List<Point>();
			Point Offset = new Point((int)offset.X, (int)offset.Y);

			foreach (Point Cell in _lattice)
			{
				NewCells.Add(new Point(Cell.X + Offset.X, Cell.Y + Offset.Y));
			}

			Lattice = NewCells;
			_latticeChanged = true;
			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Used by the original Adjustable Preview window to paint a pixel
		/// </summary>
		/// <param name="pixel">Point struct holding the coordinates of the pixel</param>
		public void PaintCell(Point cell)
		{
			if (Locked)
				return;

			_lattice.Add(cell);
			_latticeChanged = true;

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}
		
		/// <summary>
		/// Used by Clipboard to Paste cells
		/// </summary>
		/// <param name="cells">Lists of points to add to the Lattice</param>
		public void PaintCells(List<Point> cells)
		{
			if (Locked)
				return;

			_lattice.AddRange(cells);
			DedupePixels();

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
		}

		/// <summary>
		/// Used by the Image Stamp  and Text tools
		/// </summary>
		/// <param name="cells">Lists of points to add to the Lattice</param>
		/// <param name="offset">Offset point to start the painting.</param>
		public void PaintCells(List<Point> cells, Point offset)
		{
			if (Locked)
				return;

			// Go through each cell in the list, offset it, and add it to the channel
			Point Cell;
			for (int i = 0; i < cells.Count; i++)
			{
				Cell = cells[i];
				Cell.Offset(offset);
				cells[i] = Cell;
			}

			_lattice.AddRange(cells);
			DedupePixels();

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);
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

			if (Locked)
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
						_lattice.Add(new Point(x, y));
				}

			lockBitmap.UnlockBits();
			lockBitmap = null;
			_latticeChanged = true;

			this.Dirty = true;
			OnChanged(ChannelEventType.Cells);

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("PopulateFromLatticeBuffer: " + Editor.Stopwatch.Report());
		}

		/// <summary>
		/// Turns off the Hidden flag to allow normal redrawing of this Channel
		/// </summary>
		public void Show()
		{
			_hidden = false;
		}

		/// <summary>
		/// Returns the Name of the Channel
		/// </summary>
		public override string ToString()
		{
			//if (_vixenChannel != null)
			//	return _vixenChannel.Name;
			//else
				return _name;
		}

		/// <summary>
		/// Returns the Name of the Channel and its Output Channel
		/// </summary>
		/// <param name="includeIndex">Indicates whether the Output Channel number should be included in the output</param>
		public string ToString(bool includeIndex)
		{
			if (includeIndex)
				return string.Format("{0}: {1}", this.Index + 1, this.ToString());
			else
				return this.ToString();
		}

		#endregion [ Methods ]

		#region [ Custom Event Methods ]

		/// <summary>
		/// Determines which event to fire, based on the enum passed it. If that delegate is defined, then calls it.
		/// </summary>
		/// <param name="eventType">Enum that indicates which event to fire..</param>
		private void OnChanged(ChannelEventType eventType)
		{
			if (this.SuppressEvents)
				return;

			switch (eventType)
			{
				case ChannelEventType.Cells:
					if (CellsChanged == null)
						return;
					CellsChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Color:
					if (ColorChanged == null)
						return;
					ColorChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Dirty:
					if (DirtyChanged == null)
						return;
					DirtyChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Selected:
					if (SelectedChanged == null)
						return;
					SelectedChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Name:
					if (NameChanged == null)
						return;
					NameChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Visibility:
					if (VisibilityChanged == null)
						return;
					VisibilityChanged(this, new System.EventArgs());
					break;
			}
		}

	
		#endregion [ Custom Event Methods ]

		#region [ DEAD CODE ]
		
	///// <summary>
		///// Fire the selected event when the IsSelected property on this channel has changed.
		///// </summary>
		//private void OnSelected()
		//{
		//    if (Selected == null)
		//        return;

		//    //Debug.WriteLine(this.ToString(true) + " -- Selected: " + this._isSelected);

		//    DataEventArgs e = new DataEventArgs(EventCategory.Channel, EventSubCategory.Channel_Selected);
		//    Selected(this, e);
		//}

		///// <summary>
		///// Generate a bitmap based on the data
		///// The bitmap is equivalent to the one on the CanvasPane in CanvasWindow
		///// </summary>
		///// <returns></returns>
		//public Bitmap DrawChannel()
		//{
		//    return DrawChannel(this.Color, Color.Transparent);
		//}


		//public void MoveCells(Point offset)
		//{
		//    Dirty = true;
		//    List<Point> NewCells = new List<Point>();

		//    foreach (Point Cell in _lattice)
		//    {
		//        NewCells.Add(new Point(Cell.X + offset.X, Cell.Y + offset.Y));
		//    }

		//    _lattice = NewCells;
		//}

		///// <summary>
		///// Adds a new point to the Lattice
		///// </summary>
		///// <param name="x">Horizontal coordinate</param>
		///// <param name="y">Vertical coordinate</param>
		//public void PaintCell(int x, int y)
		//{
		//    Dirty = true;
		//    _lattice.Add(new Point(x, y));
		//}

		//public void PaintCells(Channel Channel)
		//{
		//    Dirty = true;
		//    PaintCells(Channel.Lattice);
		//}

		///// <summary>
		///// Cloned list of the current cells
		///// </summary>
		//public List<Point> ClonedLattice
		//{
		//    get 
		//    {
		//        List<Point> Cloned = new List<Point>();
		//        foreach (Point c in Lattice)
		//            Cloned.Add(new Point(c.X, c.Y));
		//        return Cloned;
		//    }
		//}

		#endregion [ DEAD CODE ]

	}
}
