using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Serialization;
using ElfCore.Controllers;
using ElfCore.Util;
using System.Diagnostics;
using ElfCore.Interfaces;
using ElfCore.Core;
/*
namespace ElfCore.Channels
{
	[Serializable]
	public class BaseCellChannel : BaseChannel, ICellChannel, ICloneable, INotifyPropertyChanged
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
		public const string Property_Color = "Color";
		public const string Property_Enabled = "Enabled";
		public const string Property_Grouped = "Grouped";
		public const string Property_ID = "ID";
		public const string Property_Selected = "Selected";
		public const string Property_Lattice = "Lattice";
		public const string Property_Name = "Name";
		public const string Property_Origin = "Origin";
		public const string Property_Visible = "Visible";

		// Xml Node and Attribute names
		protected const string ROOT = "Channel";
		protected const string CELLS = "Cells";
		protected const string VECTOR = "Vector";
		protected const string SUBCHANNELS = "SubChannels";
		protected const string SUBCHANNEL = "SubChannel";

		protected const string COLOR = "color";
		protected const string BORDER_COLOR = "borderColor";
		protected const string OUTPUT = "output";
		protected const string TICKS = "id";
		protected const string ENABLED = "enabled";
		protected const string NUMBER = "number";

		#endregion [ Constants ]

		#region [ Protected Variables ]

		protected List<Point> _lattice = new List<Point>();
		
		/// <summary>
		/// Indicates whether the Lattice has changed in any way since that last time the GraphicsPath object has been created. Reset when the GraphicsPath object is rebuilt.
		/// </summary>
		[XmlIgnore()]
		protected bool _latticeChanged = true;

		[XmlIgnore()]
		protected Bitmap _latticeBuffer = null;

		#endregion [ Protected Variables ]

		#region [ Properties ]

		/// <summary>
		/// Indicates whether there is data defined for this Channel.
		/// </summary>
		public bool HasData
		{
			get { return (_lattice.Count > 0); }
		}

		/// <summary>
		/// List of Cells points that represent the pseudo-bitmap mode of rendering.
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
					OnLatticeChanged();
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
		/// Sets an indicator to this Channel that it needs to refresh its GraphicsPath
		/// </summary>
		public bool DataChanged
		{
			set { _latticeChanged = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public BaseCellChannel() : base()
		{ }

		public BaseCellChannel(int index)
			: base(index)
		{ }

		public BaseCellChannel(XmlNode node)
			: base(node)
		{ }

		#endregion [ Constructors ]

		#region [ Protected Methods ]

		/// <summary>
		/// Generate a bitmap based on the data
		/// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
		/// There is no grid and zoom and paint points are 1x1 pixels
		/// </summary>
		/// <returns></returns>
		protected Bitmap CreateLatticeBuffer()
		{
			//Editor.Stopwatch.Start();

			Rectangle Bounds = new Rectangle(0, 0, Profile.Scaling.LatticeSize.Width, Profile.Scaling.LatticeSize.Height);
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
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			_lattice = null;
			if (_latticeBuffer != null)
			{
				_latticeBuffer.Dispose();
				_latticeBuffer = null;
			}
		}

		/// <summary>
		/// Called when the Lattice has been changed. 
		/// </summary>
		[DebuggerHidden()]
		protected void OnLatticeChanged()
		{
			_latticeChanged = true;
			OnPropertyChanged(Property_Lattice, true);
		}
		
		#endregion [ protected Methods ]

		#region [ Public Methods ]

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
			if (Locked)
				return;

			Dirty = true;
			Origin = new Point(0, 0);
			if (destroyData)
				_lattice = new List<Point>();
			else
				_lattice.Clear();
			OnLatticeChanged();
		}

		/// <summary>
		/// Clone this Channel and its underlying objects.
		/// </summary>
		public override object Clone()
		{
			object MyClone = base.Clone();

			// MemberwiseClone copies references, so we need to create a new Lattice for the Clone
			((BaseCellChannel)MyClone).Lattice = new List<Point>();
			((BaseCellChannel)MyClone).Lattice.AddRange(this.Lattice);
			return MyClone;
		}

		/// <summary>
		/// Used by Clipboard to Cut cells. The collection cannot be null.
		/// </summary>
		/// <param name="dataList">List of Cells to remove from the Lattice</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void CutData(List<Point> collection)
		{
			if (collection == null)
				throw new System.ArgumentNullException("collection is null.");

			if (Locked)
				return;

			foreach (Point Cell in collection)
				_lattice.Remove(Cell);
			OnLatticeChanged();
		}

		/// <summary>
		/// Generate a bitmap based on the data
		/// The bitmap is equivalent to the one on the CanvasPane in CanvasWindow
		/// NOTE: This function appears in CanvasWindow, BaseTool and Channel
		/// </summary>
		/// <param name="paintColor">Color in which to paint the cells</param>
		public Bitmap DrawChannel(Color paintColor, Color backgroundColor)
		{
			//// Create a new image of the right size 
			//Bitmap CanvasBuffer = new Bitmap(Profile.CanvasSize.Width, Profile.CanvasSize.Height);
			//using (Graphics g = Graphics.FromImage(CanvasBuffer))
			//{
			//    using (SolidBrush ChannelBrush = new SolidBrush(paintColor))
			//    {
			//        Rectangle CellRect = Rectangle.Empty;

			//        // Paint the background 
			//        g.Clear(backgroundColor);

			//        foreach (Point Cell in _lattice)
			//        {
			//            if ((Cell.X * Profile.CellScale + Profile.CellZoom < Profile.CanvasSize.Width) &&
			//                (Cell.Y * Profile.CellScale + Profile.CellZoom < Profile.CanvasSize.Height))
			//            {
			//                CellRect = new Rectangle(Cell.X * Profile.CellScale, Cell.Y * Profile.CellScale, Profile.CellZoom, Profile.CellZoom);
			//                g.FillRectangle(ChannelBrush, CellRect);
			//                if (!_borderColor.IsEmpty)
			//                    using (Pen BorderPen = new Pen(_borderColor))
			//                        g.DrawRectangle(BorderPen, CellRect);
			//            }
			//        }
			//    }
			//    return CanvasBuffer;
			//}
			return null;
		}

		/// <summary>
		/// Converts the encoded unsigned integer into a System.Drawing.Point structure.
		/// </summary>
		/// <param name="encodedPixel">Unsigned int, stored in the Lattice.</param>
		public static Point DecodeCell(uint encodedPixel)
		{
			return new Point((int)(encodedPixel >> 16), (int)(encodedPixel & 0xffff));
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
			if (Locked)
				return;

			byte[] bytes = Convert.FromBase64String(encoded);
			if (clear)
				_lattice.Clear();
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
		/// Get rid of all extra Cells that are duplicates.
		/// </summary>
		public void DedupeData()
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
		/// Converts the x,y of a Cell to a value that can be saved in the list.
		/// </summary>
		/// <param name="cell">Cell to serialize.</param>
		public static uint SerializeCell(Point cell)
		{
			if ((cell.X < 0) || (cell.Y < 0))
				return 0xFFFFFFFF;
			return (uint)((cell.X << 16) | cell.Y);
		}

		/// <summary>
		/// Converts the data array values to a string to be stored in the Profile Xml.
		/// </summary>
		public string SerializeLattice()
		{
			DedupeData();
			//Stopwatch watch = new Stopwatch();
			//watch.Start();
	
			List<byte> ChannelPixelCoords = new List<byte>();

			foreach (Point Cell in _lattice)
			{
				ChannelPixelCoords.AddRange(BitConverter.GetBytes(SerializeCell(Cell)));
			}
			return Convert.ToBase64String(ChannelPixelCoords.ToArray());

			//string Converted = Convert.ToBase64String(ChannelPixelCoords.ToArray());
			//watch.Stop();
			//Debug.WriteLine("EncodeChannelBytes: " + watch.ElapsedTicks + " ticks, " + watch.ElapsedMilliseconds + " ms");
			//return Converted;
		}

		/// <summary>
		/// Erase a single cell from the Lattice array.
		/// Used by the original Adjustable Preview window to erase a cell
		/// </summary>
		/// <param name="cell">Cell to remove from the Lattice</param>
		public void Erase(Point cell)
		{
			if (Locked)
				return;
			_lattice.Remove(cell);
			OnLatticeChanged();
		}

		/// <summary>
		/// Determines whether the data within the specified Channel object matches that of the current Channel object. Channel cannot be null.
		/// </summary>
		/// <param name="channel">Channel object to compare to the current one.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public bool Differs(ICellChannel channel)
		{
			//if (channel == null)
				throw new System.ArgumentNullException("channel cannot be null.");

			//return !(Workshop.ListEqual<Point>(_lattice, channel.Lattice) &&
			//        this.Color.Equals(channel.Color) &&
			//        (this.Name == channel.Name) &&
			//        (this.Locked == channel.Locked) &&
			//        this.Origin.Equals(channel.Origin) &&
			//        this.Visible == channel.Visible &&
			//        this.Grouped == channel.Grouped);
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

			foreach (Point Cell in _lattice)
			{
				MinX = Math.Min(MinX, Cell.X);
				MinY = Math.Min(MinY, Cell.Y);
				MaxX = Math.Max(MaxX, Cell.X);
				MaxY = Math.Max(MaxY, Cell.Y);
			}
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
				Rectangle CellRect = new Rectangle(0, 0, Profile.Scaling.CellZoom, Profile.Scaling.CellZoom);

				foreach (Point pt in _lattice)
				{
					CellRect.Location = new Point(pt.X * Profile.Scaling.CellScale, pt.Y * Profile.Scaling.CellScale);
					_graphicsPath.AddRectangle(CellRect);
				}
				_latticeChanged = false;
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
			OnPropertyChanged("Lattice", true);
		}

		/// <summary>
		/// Offsets the rendering of the Channel.
		/// </summary>
		/// <param name="offset">Amount to offset the rendering.</param>
		public void MoveData(PointF offset)
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
			OnLatticeChanged();
		}

		/// <summary>
		/// Adds a Cell to the Lattice, then triggers an event to cause the Channel to be redrawn.
		/// </summary>
		/// <param name="cell">Cell to add to the Lattice.</param>
		public void Paint(Point cell)
		{
			if (Locked)
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
		public void Paint(List<Point> collection)
		{
			if (collection == null)
				throw new System.ArgumentNullException("collection is null.");
	
			if (Locked)
				return;

			_lattice.AddRange(collection);
			DedupeData();
			OnLatticeChanged();
		}

		/// <summary>
		/// Adds a list of Cells to the Lattice, offset by the given amount, then triggers an event to cause the Channel to be redrawn.
		/// Used by the Image Stamp  and Text tools
		/// </summary>
		/// <param name="collection">Lists of Cells to add to the Lattice</param>
		/// <param name="offset">Offset point to start the painting.</param>
		/// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
		public void Paint(List<Point> collection, Point offset)
		{
			if (collection == null)
				throw new System.ArgumentNullException("collection is null.");

			if (Locked)
				return;

			// Go through each cell in the list, offset it, and add it to the channel
			Point Cell;
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

			OnPropertyChanged("Lattice", true);

			//Editor.Stopwatch.Stop();
			//Debug.WriteLine("PopulateFromLatticeBuffer: " + Editor.Stopwatch.Report());
		}

		#endregion [ Public Methods ]




		#region ICellChannel Members


		public bool Differs(IChannel channel)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

*/