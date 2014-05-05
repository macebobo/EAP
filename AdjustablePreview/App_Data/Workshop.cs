using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ElfCore.PlugIn;
using System.Runtime.Serialization.Formatters.Binary;

namespace ElfCore
{
	public sealed class Workshop
	{
		#region [ Declares ]

		//[DllImport("msvcrt.dll")]
		//private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

		#endregion [ Declares ]
		
		#region [ Private Variables ]

		private static readonly Workshop _instance = new Workshop();
 
		// Vixen Profile data
		private string _profile_filename = string.Empty;

		public static Cursor LastCursor = Cursors.Default;
		
		#endregion [ Private Variables ]

		#region [ Field Variables ]

		/// <summary>
		/// PictureBox control that renders the Channels and the current work.
		/// </summary>
		public static PictureBox Canvas = null;

		/// <summary>
		/// Offset for the dashes on the MarqueePen
		/// </summary>
		public int MarqueeDashedLineOffset = 0;

		/// <summary>
		/// Tells the CanvasWindow to paint the ImageStamp Channel. Used for the ImageStamp and Text tools
		/// </summary>
		public bool ShowImageStamp = false;

		#endregion [ Field Variables ]

		#region [ Properties ]

		public static Workshop Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Clipboard controller
		/// </summary>
		public Clipboard Clipboard { get; private set; }

		/// <summary>
		/// Rectangle of cells defined to be the cropping area of the canvas.
		/// </summary>
		public Rectangle CropArea { get; set; }
		
		/// <summary>
		/// Tool that has been currently selected
		/// </summary>
		internal ToolHost CurrentTool { get; set; }

		public bool Dirty
		{
			get { return UI.Dirty || Channels.Dirty; }
			set
			{
				UI.SuppressEvents = true;
				UI.Dirty = false;
				UI.SuppressEvents = false;
				Channels.Dirty = false;
			}
		}

		/// <summary>
		/// Object that holds the mask information for both the Canvas and the Lattice
		/// </summary>
		public Mask Mask { get; private set; }

		/// <summary>
		/// Name of the file of the Profile currently being modified
		/// </summary>
		public string Profile_Filename
		{
			get { return _profile_filename; }
			set
			{
				_profile_filename = value;
				if (value.Length > 0)
				{
					FileInfo fi = new FileInfo(value);
					this.Profile_Name = fi.Name;
					this.Profile_Name = Profile_Name.Replace(fi.Extension, string.Empty); 
					fi = null;
				}
			}
		}

		/// <summary>
		/// Name of the Profile that is currently being modified
		/// </summary>
		public string Profile_Name { get; private set; }

		/// <summary>
		/// List of PlugIn Tools to use, loaded in Preview
		/// </summary>
		internal List<ToolHost> Tools { get; set; }
		
		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public UISettings UI { get; private set; }

		/// <summary>
		/// Object that controls the list of Channels and their various functions
		/// </summary>
		public ChannelController Channels { get; private set; }
		
		/// <summary>
		/// Object that handles the data manipulation needed to perform Undo and Redo operations
		/// </summary>
		public UndoController UndoController { get; private set; }
			
		/// <summary>
		/// Path to the user's Windows Profile folder
		/// </summary>
		public string UserDirectory { get; private set; }
		
		#endregion [ Properties ]

		#region [ Event Handlers ]

		//public delegate void DataEventHandler(object sender, DataEventArgs e);
		public delegate void ZoomEventHandler(object sender, ZoomEventArgs e);
		//public delegate void ChannelEventHandler(object sender, ChannelEventArgs e);

		//public DataEventHandler Changed;
		
		/// <summary>
		/// Occurs when the Zoom tool is selected and the user clicks on the Canvas.
		/// </summary>
		public ZoomEventHandler ClickZoom;

		/// <summary>
		/// Occurs when the Dirty property changes.
		/// </summary>
		public EventHandler DirtyChanged;
		
		#endregion [ Event Handlers ]

		#region [ Constructors ]

		static Workshop()
		{ }

		private Workshop()
		{ }

		#endregion [ Constructors ]

		#region [ Custom Event Methods ]
		
		//private void OnClipboardChanged(EventSubCategory specificEvent)
		//{
		//    if (Changed == null)
		//        return;
		//    Changed(this, new DataEventArgs(EventCategory.Clipboard, specificEvent));
		//}

		//private void OnMaskChanged(EventSubCategory specificEvent)
		//{
		//    if (Changed == null)
		//        return;
		//    Changed(this, new DataEventArgs(EventCategory.Mask, specificEvent));
		//}

		

		#endregion [ Custom Event Methods ]

		#region [ Static Methods ]

		/// <summary>
		/// Compare two arrays for equality
		/// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
		/// </summary>
		/// <typeparam name="T">Type of the data stored in the array</typeparam>
		/// <param name="a1">First array to compare</param>
		/// <param name="a2">Second array to compare</param>
		/// <returns>Returns true if both arrays exactly match each other. Also returns true if both parameters point to the exact same data</returns>
		public static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Length != a2.Length)
				return false;

			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!comparer.Equals(a1[i], a2[i]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Compares two bitmaps on a per-pixel basis
		/// http://social.msdn.microsoft.com/Forums/en-US/vbgeneral/thread/bd0eec9e-f811-4fab-a245-08b2882d005c
		/// </summary>
		/// <param name="b1">First Bitmap to compare</param>
		/// <param name="b2">Second Bitmap to compare</param>
		/// <returns>Returns true if both are null, both share the same pointer, or if both match exactly at the pixel level</returns>
		public static bool BitmapEquals(Bitmap b1, Bitmap b2)
		{
			// Verify that both are not null
			if ((b1 == null) && (b2 == null))
				return true;

			// If one or the other are null, then we don't match
			if ((b1 == null) || (b2 == null))
				return false;

			// Verify that they are not using the same pointer
			if (object.ReferenceEquals(b1, b2))
				return true;

			// Verify the dimensions are the same
			if (b1.Size != b2.Size)
				return false;

			// Verify that the pixel formats are the same
			if (b1.PixelFormat != b2.PixelFormat)
				return false;

			LockBitmap lb1 = new LockBitmap(b1);
			lb1.FillPixelArray();
			LockBitmap lb2 = new LockBitmap(b2);
			lb2.FillPixelArray();

			bool Result = ArraysEqual<byte>(lb1.Pixels, lb2.Pixels);

			lb1 = null;
			lb2 = null;

			return Result;
		}
		
		/// <summary>
		/// Create a new point in Cell values from the Pixel-based point
		/// </summary>
		/// <param name="pixelPoint">Point with values in cells</param>
		/// <returns>Point with values in pixels</returns>
		public static Point CellPoint(Point pixelPoint)
		{
			return new Point(pixelPoint.X / UISettings.ʃCellScale, pixelPoint.Y / UISettings.ʃCellScale);
		}

		/// <summary>
		/// Create a new point in Cell values from the Pixel-based point
		/// </summary>
		/// <param name="pixelPoint">Point with values in cells</param>
		/// <returns>Point with values in pixels</returns>
		public static PointF CellPointF(PointF pixelPoint)
		{
			return new PointF((int)(pixelPoint.X / UISettings.ʃCellScaleF), (int)(pixelPoint.Y / UISettings.ʃCellScaleF));
		}

		/// <summary>
		/// Clones a list, so that one gets a copy of a list by value, not by reference
		/// http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/5c9b4c31-850d-41c4-8ea3-fae734b348c4
		/// </summary>
		/// <typeparam name="T">Type of object in the list</typeparam>
		/// <param name="oldList">List of objects to be cloned</param>
		/// <returns>A duplicate of the original list with the same values, but without the same object references</returns>
		public static List<T> CloneList<T>(List<T> oldList)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, oldList);
			stream.Position = 0;
			return (List<T>)formatter.Deserialize(stream);
		} 

		/// <summary>
		/// Restores the Canvas cursor to what it was before the static method WaitCursor was called.
		/// </summary>
		public static void EndWaitCursor()
		{
			Workshop.Canvas.Cursor = Workshop.LastCursor;
		}

		/// <summary>
		/// Create an MD5 hash for a stream. This is a way of identifing if data has changed. If the hash differs, the data is not the same.
		/// </summary>
		/// <param name="stream">Data stream to hash</param>
		public static string GetMD5HashFromStream(Stream stream)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(stream);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Gets the path to the user's Window's Profile directory
		/// </summary>
		public static string GetProfilePath()
		{
			string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen");
			DirectoryInfo DInfo = new DirectoryInfo(SavePath);
			if (!DInfo.Exists)
			{
				// Create the folder
				DInfo.Create();
			}
			return SavePath;
		}

		/// <summary>
		/// Compare Lists for equality
		/// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
		/// </summary>
		/// <typeparam name="T">Type of the data stored in the array</typeparam>
		/// <param name="a1">First List to compare</param>
		/// <param name="a2">Second List to compare</param>
		/// <returns>Returns true if both Lists exactly match each other. Also returns true if both parameters point to the exact same data</returns>
		public static bool ListEqual<T>(List<T> a1, List<T> a2)
		{
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Count != a2.Count)
				return false;

			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Count; i++)
			{
				if (!comparer.Equals(a1[i], a2[i]))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point
		/// </summary>
		/// <param name="cellPoint">Point with values in cells</param>
		/// <returns>Point with values in pixels</returns>
		public static Point PixelPoint(Point cellPoint)
		{
			return new Point(cellPoint.X * UISettings.ʃCellScale, cellPoint.Y * UISettings.ʃCellScale);
		}

		/// <summary>
		/// Create a new point in Pixel values from the Cell-based point
		/// </summary>
		/// <param name="cellPoint">Point with values in cells</param>
		/// <returns>PointF with values in pixels</returns>
		public static PointF PixelPointF(PointF cellPoint)
		{
			return new PointF(cellPoint.X * UISettings.ʃCellScaleF, cellPoint.Y * UISettings.ʃCellScaleF);
		}

		/// <summary>
		/// Brings up the file save dialog to save the bitmap file to disk
		/// </summary>
		/// <param name="bmp">Bitmap object to save</param>
		/// <param name="filename">Default file name for the file</param>
		/// <returns>If the save is successful, returns the new file name, otherwise returns an empty string</returns>
		public static string SaveBitmap(Bitmap bmp, string filename, string title = null)
		{
			SaveFileDialog SaveImageFileDialog = new SaveFileDialog();
			SaveImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
			SaveImageFileDialog.Title = title ?? "Save Image";
			SaveImageFileDialog.FileName = filename;
			string Ext = string.Empty;

			if (filename.Length > 0)
				Ext = filename.Substring(filename.Length - 3).ToLower();
			else
				Ext = "png";

			string[] Filters = SaveImageFileDialog.Filter.Split('|');

			for (int i = 0; i < Filters.Length; i++)
			{
				// there will be a matched pair for each of these elements, look at the even numbered element
				// Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*
				if (i % 2 == 0)
					i++;
				if (i >= Filters.Length)
					break;

				if (Filters[i].Replace("*.", "") == Ext)
				{
					SaveImageFileDialog.FilterIndex = (i / 2) + 1;
					break;
				}
			}

			if (SaveImageFileDialog.ShowDialog() != DialogResult.OK)
				return string.Empty;

			filename = SaveImageFileDialog.FileName;
			ImageFormat Format;

			switch (SaveImageFileDialog.FilterIndex)
			{
				case 1: // Bitmap
					Format = ImageFormat.Bmp;
					filename = Path.ChangeExtension(filename, ".bmp");
					break;
				case 3: // PNG
					Format = ImageFormat.Png;
					filename = Path.ChangeExtension(filename, ".png");
					break;
				case 4: // GIF
					Format = ImageFormat.Gif;
					filename = Path.ChangeExtension(filename, ".gif");
					break;
				default:
					Format = ImageFormat.Jpeg;
					filename = Path.ChangeExtension(filename, ".jpg");
					break;
			}

			try
			{
				Bitmap b = new Bitmap(bmp);
				b.Save(filename, Format);
				b.Dispose();
				b = null;
			}
			catch
			{
				MessageBox.Show("Unable to save this file, possibly due to where it is being saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return string.Empty;
			}

			MessageBox.Show("File saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return filename;
		}

		/// <summary>
		/// Sets the Canvas cursor to the system Wait cursor
		/// </summary>
		public static void WaitCursor()
		{
			Workshop.LastCursor = Workshop.Canvas.Cursor;
			Workshop.Canvas.Cursor = Cursors.WaitCursor;
		}

		#endregion [ Static Methods ]

		#region [ Private Methods ]

		internal ToolHost GetPlugInTool(int toolID)
		{
			//if (Workshop.IsPlugInTool(tool))
			//{
			//int Index = tool - (int)Tool.PlugIn;
			//return _pluginTools[Index];
			//}
			//else
			//return null;
			foreach (ToolHost THost in this.Tools)
			{
				if (THost.ID == toolID)
					return THost;
			}
			return null;
		}

		#endregion [ Private Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Generate a bitmap of the Canvas, excluding the active Channel(s)
		/// </summary>
		/// <param name="canvasPane"></param>
		/// <returns></returns>
		public Bitmap CaptureCanvas()
		{
			return CaptureCanvas(false);
		}

		/// <summary>
		/// Generate a bitmap of the Canvas. If indicated, exclude the selected Channel(s)
		/// </summary>
		/// <param name="excludeSelectedChannels">Indicates whether selected Channels should be included in this screen grab</param>
		/// <returns>Bitmap of the current canvas</returns>
		public Bitmap CaptureCanvas(bool excludeSelectedChannels)
		{
			if (Workshop.Canvas == null)
				return null;

			int width = Workshop.Canvas.Size.Width;
			int height = Workshop.Canvas.Size.Height;

			Bitmap bm = new Bitmap(width, height, Workshop.Canvas.CreateGraphics());

			if (excludeSelectedChannels)
			{
				Channels.HideSelected();
				Workshop.Canvas.DrawToBitmap(bm, new Rectangle(0, 0, width, height));
				Channels.ShowSelected();
				Workshop.Canvas.Refresh();
			}
			else
				Workshop.Canvas.DrawToBitmap(bm, new Rectangle(0, 0, width, height));

			return bm;
		}

		/// <summary>
		/// Clears out the masked area, moves the cells back from the Move Channel to their proper one, 
		/// and instructs the CanvasWindow to stop displaying the marquee
		/// </summary>
		public void ClearMask()
		{
			if (!Mask.HasMask)
				return;
			Mask.Clear();
			//OnMaskChanged(EventSubCategory.Mask_Cleared);
		}

		/// <summary>
		/// Triggers an event to fire, due to the user clicking with the Zoom tool on a point on the canvas.
		/// </summary>
		/// <param name="zoomPoint">Point on the canvas the user has clicked</param>
		/// <param name="zoomLevel">New zoom amount to use.</param>
		public void SetClickZoom(Point zoomPoint, float zoomLevel)
		{
			if (ClickZoom == null)
			{
				this.UI.Zoom = zoomLevel;
				return;
			}

			//this.UI.Zoom = zoomLevel;

			ClickZoom(this, new ZoomEventArgs(zoomPoint, zoomLevel));
		}

		/// <summary>
		/// Calculates the distance between 2 points
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public float ComputeDistance(Point pt1, Point pt2)
		{
			return (float)Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
		}

		/// <summary>
		/// Returns a point that is constrained to the various 45 deg angles relative to the mouse down point, but only if the
		/// Control Key is pressed.
		/// </summary>
		/// <param name="currentPoint">Point to be constrained</param>
		/// <param name="referencePoint">Reference point for constrainment. Typically this is the mouse down point</param>
		public Point ConstrainLine(Point currentPoint, Point referencePoint)
		{
			// If the Control key is not pressed, then nothing to do.
			if ((Control.ModifierKeys == Keys.None) || ((Control.ModifierKeys | Keys.Control) != Keys.Control))
				return currentPoint;

			Point Constrained = currentPoint;
			int X = Math.Abs(currentPoint.X - referencePoint.X);
			int Y = Math.Abs(currentPoint.Y - referencePoint.Y);
			int D = Math.Max(X, Y);
			Point[] Snaps = new Point[8];
			float[] Distance = new float[8];

			float MinDistance = float.MaxValue;
			float ThisDistance = 0f;

			for (int i = 0; i < 8; i++)
				Snaps[i] = referencePoint;

			Snaps[0].Offset(D, 0);
			Snaps[1].Offset(D, -D);
			Snaps[2].Offset(0, -D);
			Snaps[3].Offset(-D, -D);
			Snaps[4].Offset(-D, 0);
			Snaps[5].Offset(-D, D);
			Snaps[6].Offset(0, D);
			Snaps[7].Offset(D, D);

			for (int i = 0; i < 8; i++)
			{
				ThisDistance = ComputeDistance(currentPoint, Snaps[i]);
				MinDistance = Math.Min(MinDistance, ThisDistance);
				if (MinDistance == ThisDistance)
					Constrained = Snaps[i];
			}
			return Constrained;
		}

		/// <summary>
		/// Calculates the position of the point where it should lie if contraining conditions are met.
		/// </summary>
		/// <param name="currentPoint">Point to be constrained</param>
		/// <param name="referencePoint">Reference point for constrainment. Typically this is the mouse down point</param>
		/// <param name="direction">Direction to force the constraint.</param>
		public Point ConstrainPaint(Point currentPoint, Point referencePoint, ref ConstrainDirection direction)
		{
			// If the Control key is not pressed, then nothing to do.
			if ((Control.ModifierKeys == Keys.None) || ((Control.ModifierKeys | Keys.Control) != Keys.Control))
			{
				direction = ConstrainDirection.NotSet;
				return currentPoint;
			}

			if (currentPoint.Equals(referencePoint))
				return currentPoint;

			if (direction == ConstrainDirection.NotSet)
			{ 
				// determine a constraining direction based on the current point and the reference point
				int X = Math.Abs(currentPoint.X - referencePoint.X);
				int Y = Math.Abs(currentPoint.Y - referencePoint.Y);
				int D = Math.Max(X, Y);
				Point[] Snaps = new Point[4];
				float[] Distance = new float[4];
				float MinDistance = float.MaxValue;
				float ThisDistance = 0f;
				int Index = -1;

				for (int i = 0; i < Snaps.Count(); i++)
					Snaps[i] = referencePoint;

				// Horizontal
				Snaps[0].Offset(D, 0);
				Snaps[1].Offset(-D, 0);

				// Vertical
				Snaps[2].Offset(0, D);
				Snaps[3].Offset(0, D);

				for (int i = 0; i < Snaps.Count(); i++)
				{
					ThisDistance = ComputeDistance(currentPoint, Snaps[i]);
					MinDistance = Math.Min(MinDistance, ThisDistance);
					if (MinDistance == ThisDistance)
						Index = i;
				}

				if (Index < 3)
					direction = ConstrainDirection.Horizontal;
				else
					direction = ConstrainDirection.Vertical;
			}

			System.Diagnostics.Debug.WriteLine(direction.ToString());

			if (direction == ConstrainDirection.Horizontal)
				return new Point(currentPoint.X, referencePoint.Y);
			else if (direction == ConstrainDirection.Vertical)
				return new Point(referencePoint.X, currentPoint.Y);
			else
				return currentPoint;
		}

		/// <summary>
		/// Converts an angle from Degrees to one of Radians
		/// </summary>
		/// <param name="angle">Angle value in Degrees</param>
		/// <returns>Corresponding angle in Radians</returns>
		public float DegreeToRadian(float angle)
		{
			return (float)(Math.PI * angle / 180.0);
		}

		///// <summary>
		///// Fires off the Delete event, deleting all selected cells
		///// </summary>
		//public void DeleteCells()
		//{
		//    OnClipboardChanged(EventSubCategory.Clipboard_Delete);
		//}

		/// <summary>
		/// Determines the size of a span of cells in pixels
		/// d(c) = d(p) / (z * (Cs + Glw))
		/// </summary>
		/// <param name="cellLength">Length of pixels</param>
		/// <returns>Count of Cells</returns>
		public int GetCellLength(int pixelDistance)
		{
			return (int)(pixelDistance / UISettings.ʃCellScale);
		}

		/// <summary>
		/// Calculates the width and height of the background image, in current Cell Size, factoring in GridWidth
		/// </summary>
		public Size GetImageSizeInCells()
		{
			if (UI.Background.HasData)
				return new Size(GetCellLength(UI.Background.Image.Width), GetCellLength(UI.Background.Image.Height));
			else
				return UI.LatticeSize;
		}

		/// <summary>
		/// Determines the size of a span of Cells in Pixels
		/// d(p) = d(c) * z * (Cs + Glw)
		/// </summary>
		/// <param name="pixelDistance">Length of Cells</param>
		/// <returns>Count of pixels</returns>
		public float GetPixelLength(float cellLength)
		{
			return cellLength * UISettings.ʃCellScaleF;
		}

		/// <summary>
		/// Creates the Pen object used for marquee displays
		/// </summary>
		/// <returns></returns>
		public Pen GetMarqueePen()
		{
			Pen MarqueePen = new Pen(Color.White, 1);
			MarqueePen.DashStyle = DashStyle.Custom;
			MarqueePen.DashPattern = new float[2] { 5, 5 };
			MarqueePen.DashOffset = MarqueeDashedLineOffset;

			MarqueeDashedLineOffset += 2;
			if (MarqueeDashedLineOffset > 10)
				MarqueeDashedLineOffset = 2;

			return MarqueePen;
		}

		/// <summary>
		/// Called right after this object is created, spawns all the internal classes, such as UISettings, ChannelController, etc.
		/// We cannot do this in the constructor itself due to the fact that a number of the object use the Instance of 
		/// Workshop, which does not exist at the time of constructing.
		/// </summary>
		public void Initialize()
		{
			this.Mask = new Mask();
			this.UI = new UISettings();
			this.Clipboard = new Clipboard();
			this.UndoController = new UndoController();
			this.Channels = new ChannelController();
			this.UserDirectory = Environment.GetEnvironmentVariable("userprofile") + @"\";
			this.CropArea = Rectangle.Empty;

			this.UI.Zooming += new EventHandler(UISettings_Changed);
			this.UI.DirtyChanged += new EventHandler(Object_DirtyChanged);
			this.Channels.DirtyChanged += new EventHandler(Object_DirtyChanged);
		}

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		public void MoveMask(Point offset)
		{
			//System.Drawing.Drawing2D.Matrix MoveMatrix = new System.Drawing.Drawing2D.Matrix();
			//MoveMatrix.Translate(offset.X, offset.Y);

			//// Translate a clone of the path by offset, and replace it with the new one.
			//GraphicsPath MovePath = (GraphicsPath)_maskOutline.Clone();
			//_maskOutline = null;
			//MovePath.Transform(MoveMatrix);
			//MoveMatrix.Dispose();
			//_maskOutline = MovePath;

			//_maskRegion.Translate(offset.X, offset.Y);
			if (!Mask.HasMask)
				return;
			Mask.Move(offset, Scaling.Pixel);
			//OnMaskChanged(EventSubCategory.Mask_Moved);
		}

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		public void MoveMask(PointF offset)
		{
			//System.Drawing.Drawing2D.Matrix MoveMatrix = new System.Drawing.Drawing2D.Matrix();
			//MoveMatrix.Translate(offset.X, offset.Y);

			//// Translate a clone of the path by offset, and replace it with the new one.
			//GraphicsPath MovePath = (GraphicsPath)_maskOutline.Clone();
			//_maskOutline = null;
			//MovePath.Transform(MoveMatrix);
			//MoveMatrix.Dispose();
			//_maskOutline = MovePath;

			//_maskRegion.Translate(offset.X, offset.Y);
			//OnMaskChanged(SpecificEventType.Mask_Moved);
			if (!Mask.HasMask)
				return;
			Mask.Move(offset, Scaling.Pixel);
			//OnMaskChanged(EventSubCategory.Mask_Moved);
		}

		/// <summary>
		/// Normalizes 2 points, so that p1 represents the upper left corner and p2 represents the lower right corner
		/// </summary>
		/// <param name="p1">First defining point in the rectangle</param>
		/// <param name="p2">Second defining point in the rectangle</param>
		public Rectangle NormalizedRectangle(Point p1, Point p2)
		{
			Rectangle rc = new Rectangle();

			// Normalize the rectangle.
			rc.X = Math.Min(p1.X, p2.X);
			rc.Y = Math.Min(p1.Y, p2.Y);
			rc.Width = Math.Abs(p2.X - p1.X);
			rc.Height = Math.Abs(p2.Y - p1.Y);

			if ((p2.X - p1.X) < 0)
			{
				rc.X += 1;
				rc.Width -= 2;
			}

			if ((p2.Y - p1.Y) < 0)
			{
				rc.Y += 1;
				rc.Height -= 2;
			}

			if (rc.Width <= 0)
				rc.Width = 1;
			if (rc.Height <= 0)
				rc.Height = 1;
			return rc;
		}

		///// <summary>
		///// First off the Paste event
		///// </summary>
		//public void Paste()
		//{
		//    OnClipboardChanged(EventSubCategory.Clipboard_Paste);
		//}

		/// <summary>
		/// Find a point along an ellipse based on the angle in degrees
		/// </summary>
		/// <param name="bounds">Rectangle bounding the ellipse</param>
		/// <param name="angle">Angle in degrees</param>
		public PointF PointFromEllipse(RectangleF bounds, float angle)
		{
			float a = bounds.Width / 2.0f;
			float b = bounds.Height / 2.0f;
			float rad = DegreeToRadian(angle);

			float xCenter = (bounds.Left + bounds.Right) / 2;
			float yCenter = (bounds.Top + bounds.Bottom) / 2;

			float x = xCenter + (a * (float)Math.Cos(rad));
			float y = yCenter + (b * (float)Math.Sin(rad));

			return new PointF(x, y);
		}

		/// <summary>
		/// Converts an angle from Radians to one of Degrees
		/// </summary>
		/// <param name="angle">Angle value in Radians</param>
		/// <returns>Corresponding angle in Degrees</returns>
		public float RadianToDegree(float angle)
		{
			return (float)(angle * (180.0 / Math.PI));
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="maskData">Mask object</param>
		public void SetMask(Mask maskData)
		{
			Mask.Define(maskData, true);
			//OnMaskChanged(EventSubCategory.Mask_Defined);
		}

		#endregion [ Public Methods ]

		#region [ Events ]

		//private void Mask_Changed(object sender, DataEventArgs e)
		//{
		//    //if (!UndoController.Undoing)
		//        OnMaskChanged(e.SubCategory);
		//}

		/// <summary>
		/// One or more of the properties in UISettings that affects the Channels has been altered. Inform the ChannelController and the current Tool objects.
		/// </summary>
		private void UISettings_Changed(object sender, EventArgs e)
		{
			if (Channels != null)
				Channels.UIChanged();
		}

		/// <summary>
		/// Either the ChannelController or the UISettings object's Dirty property changed. Fire the DirtyChanged event for this object to let it's owners know this.
		/// </summary>
		private void Object_DirtyChanged(object sender, EventArgs e)
		{
			if (DirtyChanged != null)
				DirtyChanged(sender, e);
		}

		#endregion [ Events ]	

		#region [ DEAD CODE ]

		///// <summary>
		///// Takes the Canvas and manipulates the scrollbars such that the point is in the middle of the CanvasWindow. Only works if there are scroll bars present on the Canvas Window.
		///// </summary>
		///// <param name="center">Point to center around</param>
		//public void CenterCanvasOnPoint(Point center)
		//{
		//    UISettings_Changed(this, new DataEventArgs(GeneralDataEvent.UI, SpecificEventType.CanvasPosition, center));
		//}

		//private void OnChannelsChanged(ChannelEventArgs e)
		//{
		//    if (e.SubCategory == EventSubCategory.Dirty)
		//    {
		//        Dirty = true;
		//        return;
		//    }

		//    if (ChannelsChanged == null)
		//        return;
		//    ChannelsChanged(this, e);
		//}

		//private void OnUISettingsChanged(DataEventArgs e)
		//{
		//    if (e.SubCategory == EventSubCategory.Dirty)
		//    {
		//        Dirty = true;
		//        return;
		//    }

		//    if (Changed == null)
		//        return;
		//    Changed(this, e);
		//}

		///// <summary>
		///// Catch the Changed event from the ChannelController object
		///// </summary>
		//private void Channels_Changed(object sender, ChannelEventArgs e)
		//{
		//    OnChannelsChanged(e);
		//}

		///// <summary>
		///// Catch the Selected event from the ChannelController object
		///// </summary>
		//private void Channels_Selected(object sender, ChannelEventArgs e)
		//{
		//    OnChannelsChanged(e);
		//}
			
		//private void UISettings_Changed(object sender, DataEventArgs e)
		//{
		//    //if (!UndoController.Undoing)
		//        OnUISettingsChanged(e);
		//}

		///// <summary>
		///// Finds the Channel in the list by its unique identifier.
		///// </summary>
		///// <param name="guid">GUID, unique to this Channel</param>
		///// <returns>Returns the matching Channel if found, null otherwise</returns>
		//public Channel GetChannel(string guid)
		//{
		//    return _Channels.Where(c => c.GUID == guid).FirstOrDefault();
		//}

		///// <summary>
		///// Finds the Channel by its OutputChannel value and returns it
		///// </summary>
		///// <param name="ChannelNum">OutputChannel value on the Channel desired</param>
		///// <returns>If the Channel is not found, returns null, otherwise returns the correct Channel</returns>
		//public Channel GetChannelByChannelNum(int ChannelNum)
		//{
		//    return _Channels.Where(c => c.OutputChannel == ChannelNum).FirstOrDefault();
		//    //foreach (Channel Channel in _Channels)
		//    //{
		//    //    if (Channel.OutputChannel == ChannelNum)
		//    //        return Channel;
		//    //}
		//    //return null;
		//}

		///// <summary>
		///// Fires off the CellsChanged event
		///// </summary>
		//public void CellsChanged()
		//{
		//    OnChannelChanged(SpecificEventType.Channel_Cells);
		//}

		///// <summary>
		///// Clears out all the cells on all Channels, then fires the ClearAll event.
		///// </summary>
		//public void ClearAllChannels()
		//{
		//    foreach (Channel Channel in _Channels)
		//        Channel.ClearLattice();
		//    OnChannelChanged(SpecificEventType.Channel_ClearAll);
		//}

		///// <summary>
		///// Clears out all the cells on the selected Channels, then fires the ClearSelected event.
		///// </summary>
		//public void ClearSelectedChannels()
		//{
		//    //UndoData.v1 Undo = new UndoData.v1();
		//    //Undo.Action = "Clear Channel";
		//    //for (int i = 0; i < _selectedChannels.Count; i++)
		//    //{
		//    //    Undo.AffectedChannels.Add(_selectedChannels[i].Index);
		//    //    Undo.Cells.Add(_selectedChannels[i].ClonedLattice);
		//    //}
		//    //Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    //Undo.SpecificEvent = SpecificEventType.Channel_Cells;
		//    //this.UndoController.Push(Undo);
		//    //Undo = null;

		//    foreach (Channel Channel in SelectedChannels)
		//        Channel.ClearLattice();

		//    UndoController.SaveUndo("Clear Channel" + ((SelectedChannels.Count > 1) ? "s" : ""));
		//    OnChannelChanged(SpecificEventType.Channel_ClearSelected);
		//}


		//public void PopulateChannelFromBitmap(PictureBox canvasPane, Bitmap bitmap, Channel drawChannel, bool clearFirst)
		//{
		//    Cursor LastCursor = canvasPane.Cursor;
		//    canvasPane.Cursor = Cursors.WaitCursor;

		//    if (clearFirst)
		//        drawChannel.ClearLattice();

		//    drawChannel.LatticeBuffer = bitmap;
		//    //Editor.ExposePane(bitmap, Panes.Paint);
		//    canvasPane.Cursor = LastCursor;
		//}

		//public void PopulateChannelFromBitmap(PictureBox canvasPane, Bitmap bitmap, Channel drawChannel)
		//{
		//    PopulateChannelFromBitmap(canvasPane, bitmap, drawChannel, true);
		//}

		//public void PopulateChannelFromBitmap_Erase(PictureBox canvasPane, Bitmap bitmap, Channel drawChannel)
		//{
		//    Cursor LastCursor = canvasPane.Cursor;
		//    canvasPane.Cursor = Cursors.WaitCursor;

		//    // Grab the bitmap representing this Channel 
		//    Bitmap ChannelBitmap = drawChannel.LatticeBuffer;
		//    Graphics g = Graphics.FromImage(ChannelBitmap);
		//    g.DrawImage(bitmap, 0, 0);
		//    g.Dispose();

		//    drawChannel.ClearLattice();
		//    drawChannel.LatticeBuffer = ChannelBitmap;

		//    //Editor.ExposePane(ChannelBitmap, Panes.ActiveChannel);
		//    canvasPane.Cursor = LastCursor;
		//}
			

		///// <summary>
		///// Adds the Channel event to the given Channel. This method is only called in Preview.LoadData()
		///// </summary>
		///// <param name="Channel">Channel to which to add the event</param>
		//public void AddEventToChannel(Channel Channel)
		//{
		//    Channel.Changed += new ChannelController.ChannelEventHandler(Channel_Changed);
		//}


		//private void OnToolChanged()
		//{
		//    OnToolChanged(SpecificEventType.Tool_Selected);
		//}

		//private void OnToolChanged(SpecificEventType specificEvent)
		//{
		//    if (Changed == null)
		//        return;
		//    CanvasEventArgs e = new CanvasEventArgs(GeneralCanvasEvent.Tool, specificEvent);
		//    Changed(this, e);
		//}
		//private void OnUndoStackChanged(SpecificEventType specificEvent)
		//{
		//    if (Changed == null)
		//        return;
		//    DataEventArgs e = new DataEventArgs(GeneralDataEvent.Undo, specificEvent);
		//    Changed(this, e);
		//}		

		//private void OnChannelChanged()
		//{
		//    OnChannelChanged(SpecificEventType.Channel_Active);
		//}

		//private void OnChannelChanged(SpecificEventType specificEvent)
		//{
		//    if (Changed == null)
		//        return;
		//    DataEventArgs e = new DataEventArgs(GeneralDataEvent.Channel, specificEvent);
		//    Changed(this, e);
		//}

		///// <summary>
		///// Multiple Channels are selected
		///// </summary>
		//private void OnChannelsSelected()
		//{
		//    if (Changed == null)
		//        return;
		//    DataEventArgs e = new DataEventArgs(GeneralDataEvent.Channel, SpecificEventType.Channel_Selected);
		//    Changed(this, e);
		//}


		///// <summary>
		///// Selected Channels is only populated when there is more than one Channel selected, other than the focused Channel
		///// </summary>
		//public List<Channel> SelectedChannels
		//{
		//    get { return _selectedChannels; }
		//    set
		//    {
		//        _selectedChannels = value;
		//        if ((_selectedChannels != null) && (_selectedChannels.Count > 1))
		//            OnChannelChanged();
		//    }
		//}

		//public int SetupStartChannel
		//{
		//    get { return _setupStartChannel; }
		//    set { _setupStartChannel = value; }
		//}

		//internal static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
		//{
		//    while (toCheck != null && toCheck != typeof(object))
		//    {
		//        var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
		//        if (generic == cur)
		//        {
		//            return true;
		//        }
		//        toCheck = toCheck.BaseType;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Converts the value used on the Brightness sliding scale to a value usable to the system.
		///// Converts the range (0 - 20) to (-1.0 - +1.0)
		///// </summary>
		///// <param name="slideValue">Value from the Brightness sliding scale</param>
		///// <returns>Converted value</returns>
		//public float ConvertBrightnessSlideToValue(int slideValue)
		//{
		//    slideValue = Math.Max(Math.Min(slideValue, 20), 0);
		//    return (slideValue - 10) / 10.0f;
		//}

		///// <summary>
		///// Converts the value used to change the brightness on the bitmap to one usable on the Brightness sliding scale
		///// Converts the range (-1.0 - +1.0) to (0 - 20)
		///// </summary>
		///// <param name="brightness">Decimal brightness value</param>
		///// <returns>Converted value</returns>
		//public int ConvertBrightnessValueToSlide(float brightness)
		//{
		//    brightness = Math.Max(Math.Min(brightness, 1.0f), -1.0f);
		//    return Convert.ToInt32((brightness * 10) + 10);
		//}

		//public void CreateUndo_Channel(string menuText)
		//{
		//    if (XmlHelper.IsNumeric(menuText))
		//    {
		//        int Value = Convert.ToInt32(menuText);
		//        menuText = EnumHelper.GetEnumFromValue<Tool>(Value).ToString();
		//    }

		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = menuText;
		//    Undo.AffectedChannels.Add(this.ActiveChannelIndex);
		//    Undo.Cells.Add(this.ActiveChannel.ClonedLattice);
		//    Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    Undo.SpecificEvent = SpecificEventType.Channel_Cells;
		//    this.UndoController.Push(Undo);
		//    Undo = null;
		//}

		//public void CreateUndo_Channel(string menuText, int numChannels)
		//{
		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = menuText;
		//    Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    Undo.SpecificEvent = SpecificEventType.Channel_Cells;

		//    if ((this.ActiveChannelIndex + numChannels) > this.Channels.Count)
		//        numChannels = this.Channels.Count - this.ActiveChannelIndex;

		//    for (int i = this.ActiveChannelIndex; i < this.ActiveChannelIndex + numChannels; i++)
		//    {
		//        if (i >= this.Channels.Count)
		//            return;
		//        Undo.AffectedChannels.Add(this.Channels[i].Index);
		//        Undo.Cells.Add(this.Channels[i].ClonedLattice);
		//    }

		//    this.UndoController.Push(Undo);
		//    Undo = null;
		//}

		//public void CreateUndo_Channel(string menuText, List<int> ChannelIndices)
		//{
		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = menuText;
		//    Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    Undo.SpecificEvent = SpecificEventType.Channel_Cells;
		//    int Index = -1;

		//    for (int i = 0; i < ChannelIndices.Count; i++)
		//    {
		//        Index = ChannelIndices[i];
		//        if ((Index >= this.Channels.Count) || (Index < 0))
		//            continue;
		//        Undo.AffectedChannels.Add(Index);
		//        Undo.Cells.Add(this.Channels[Index].ClonedLattice);
		//    }

		//    this.UndoController.Push(Undo);
		//    Undo = null;
		//}

		///// <summary>
		///// Create a Channel undo using the list of Channels.
		///// </summary>
		///// <param name="menuText">Text to be displayed on the Edit menu</param>
		///// <param name="ChannelList">List of Channels to save data for</param>
		//public void CreateUndo_Channel(string menuText, List<Channel> ChannelList)
		//{
		//    CreateUndo_Channel(menuText, ChannelList, false);
		//}

		///// <summary>
		///// Create a Channel undo using the list of Channels. If indicated, saves the current Mask in the Undo object as well
		///// </summary>
		///// <param name="menuText">Text to be displayed on the Edit menu</param>
		///// <param name="ChannelList">List of Channels to save data for</param>
		///// <param name="saveMask">Indicates if the Mask should be saved as well</param>
		//public void CreateUndo_Channel(string menuText, List<Channel> ChannelList, bool saveMask)
		//{
		//    UndoData.v1 Undo = new UndoData.v1();
		//    Undo.Action = menuText;
		//    Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    Undo.SpecificEvent = SpecificEventType.Channel_Cells;
		//    int Index = -1;

		//    for (int i = 0; i < ChannelList.Count; i++)
		//    {
		//        Index = ChannelList[i].Index;
		//        if ((Index >= this.Channels.Count) || (Index < 0))
		//            continue;
		//        Undo.AffectedChannels.Add(Index);
		//        Undo.Cells.Add(this.Channels[Index].ClonedLattice);
		//    }

		//    if (saveMask)
		//        Undo.Mask.Set(_mask, false);

		//    this.UndoController.Push(Undo);
		//    Undo = null;
		//}


		///// <summary>
		///// Fires off the Undo event
		///// </summary>
		//public void Undo()
		//{
		//    OnUndoStackChanged(SpecificEventType.Undo);
		//}

		//private void UndoStack_Changed(object sender, DataEventArgs e)
		//{
		//    if (!UndoController.Undoing)
		//        OnUndoStackChanged(e.SpecificEventType);
		//}

		//private UndoStack _redoStack = null;

		//public Channel ClipboardChannel
		//{
		//    get { return _Channels[_clipboardChannelIndex]; }
		//}

		//public int ClipboardChannelIndex
		//{
		//    get { return _clipboardChannelIndex; }
		//    set { _clipboardChannelIndex = value; }
		//}

		//public Channel ImageStampChannel
		//{
		//    get { return _Channels[_imageStampChannelIndex]; }
		//}

		//public int ImageStampChannelIndex
		//{
		//    get { return _imageStampChannelIndex; }
		//    set { _imageStampChannelIndex = value; }
		//}

		//public Channel MoveChannel
		//{
		//    get { return _Channels[_moveChannelIndex]; }
		//}

		//public int MoveChannelIndex
		//{
		//    get { return _moveChannelIndex; }
		//    set { _moveChannelIndex = value; }
		//}

		//public UndoStack RedoStack
		//{
		//    get { return _redoStack; }
		//}

		//internal ToolHost GetPlugInTool()
		//{
		//    return GetPlugInTool(_currentTool);
		//}
		
		///// <summary>
		///// Fires off the Copy Event
		///// </summary>
		//public void Copy()
		//{
		//    OnClipboardChanged(SpecificEventType.Clipboard_Copy);
		//}
		
		//public void Redo()
		//{
		//    OnUISettingsChanged(SpecificEventType.Redo);
		//}

		///// <summary>
		///// Sets the mask to be the values passed in.
		///// </summary>
		///// <param name="maskOutline">Graphics path to define the new outline</param>
		///// <param name="maskRegion">Region that defines the cells contained in the mask</param>
		//public void SetMask(GraphicsPath maskOutline, Region maskRegion)
		//{
		//    _mask.Set(maskOutline, maskRegion);
		//}

		//public bool UsingMaskTool()
		//{
		//    return (_currentTool == (int)_maskTool);
		//}

		///// <summary>
		///// The Channel currently selected. If there are multiple Channels selected, then the Active Channel is the topmost of these selected Channels
		///// </summary>
		//public Channel ActiveChannel
		//{
		//    get
		//    {
		//        if ((_Channels == null) || (_Channels.Count == 0))
		//            //if (!HasActiveChannel)
		//            return null;

		//        else if (_activeChannelIndex < 0)
		//        {
		//            ActiveChannelIndex = 0;
		//        }
		//        else if (_activeChannelIndex >= _Channels.Count)
		//            ActiveChannelIndex = _Channels.Count - 1;

		//        return _Channels[_activeChannelIndex];
		//    }
		//}

		///// <summary>
		///// Index of the Active Channel
		///// </summary>
		//public int ActiveChannelIndex
		//{
		//    get { return _activeChannelIndex; }
		//    set
		//    {
		//        if (_activeChannelIndex != value)
		//        {
		//            _priorActiveChannelIndex = _activeChannelIndex;
		//            _activeChannelIndex = value;
		//            OnChannelChanged();
		//        }
		//    }
		//}

		///// <summary>
		///// List of all the Channels
		///// </summary>
		//public List<Channel> Channels
		//{
		//    set { _Channels = value; }
		//    get { return _Channels; }
		//}

		// User Interface settings
		//private UISettings UI = null;

		// Tool Variables
		//private ToolHost _currentTool = null;
		//private List<ToolHost> _tools = null;

		// Channel Variables
		//private ChannelController _ChannelController = null;

		//private List<Channel> _Channels = new List<Channel>();
		//private List<Channel> _selectedChannels = new List<Channel>();
		//private int _activeChannelIndex = -1;
		//private int _priorActiveChannelIndex = 0;
		//private int _setupStartChannel = 0;

		// Mask Variable
		//private Mask Mask = null;

		// Crop Variables
		//private Rectangle _cropArea = Rectangle.Empty;

		// Misc Variables
		//private string UserDirectory = string.Empty;


		///// <summary>
		///// Indicates whethere there
		///// </summary>
		//public bool HasActiveChannel
		//{
		//    get { return _activeChannelIndex >= 0; }
		//}

		///// <summary>
		///// Temporary Channel used to hold and display the ImageStamp
		///// </summary>
		//public Channel ImageStampChannel { get; private set; }

		//public Channel MoveChannel { get; private set; }

		//public int PriorActiveChannelIndex
		//{
		//    get { return _priorActiveChannelIndex; }
		//    set { _priorActiveChannelIndex = value; }
		//}
	
		#endregion [ DEAD CODE ]

	}
}
