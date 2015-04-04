using System.IO;

using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml.Serialization;

using ElfCore.XmlSerializers;

namespace ElfCore.Core
{
	/// <summary>
	/// Handles the loading, saving and brightness manipulate of the Canvas background image.
	/// </summary>
	//[XmlRoot(Namespace = "http://halloween.tittivillus.com")]
	public class Background : ElfBase
	{
		#region [ Constants ]

		private const string NULL = "NULL";

		// Default values
		public const bool Default_Visible = true;
		public const float Default_Brightness = 0f;
		public const float Default_Saturation = 1.0f;
		public const float Default_Hue = 0f;

		// Property name constants
		public const string Property_Anchor = "Anchor";
		public const string Property_Color = "Color";
		public const string Property_FillMode = "FillMode";
		public const string Property_GridColor = "GridColor";
		public const string Property_Brightness = "Brightness";
		public const string Property_Filename = "Filename";
		public const string Property_Hue = "Hue";
		public const string Property_OverlayGrid = "OverlayGrid";
		public const string Property_Saturation = "Saturation";
		public const string Property_Visible = "Visible";
		public const string Property_WallpaperStyle = "WallpaperStyle";
		public const string Property_TempImage = "TempImage";
		public const string Property_TempComposite = "TempComposite";
		public const string Property_SaveEncodedImage = "SaveEncodedImage";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private AnchorStyles _anchor;
		private float _brightness;
		private float _hue;
		private float _saturation;
		private Color _color;
		private Color _gridColor;
		private Bitmap _composite = null;
		private string _filename;
		private Bitmap _image = null;
		private bool _visible = true;
		private bool _overlayGrid = false;
		private XmlHelper _xmlHelper = XmlHelper.Instance;
		private BaseProfile _profile = null;
		private Size _size = Size.Empty;
		private WallpaperStyle _wallpaperStyle;
		private string _tempFileName = string.Empty;
		private string _tempCompositeFileName = string.Empty;
		private bool _saveEncodedImage;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Summary:
		///		Gets the brightness value added to the background image. -1.0 to +1.0
		/// 
		/// Returns: 
		///		The floating point value representing the set brightness value. A value of 0 indicates no changes in brightness.
		/// </summary>
		public float Brightness
		{
			get { return _brightness; }
			set
			{
				if (_brightness != value)
				{
					// Make sure the vlaue stays in the nominal range
					_brightness = Math.Max(Math.Min(value, +1f), -1f);
					OnPropertyChanged(this, Property_Brightness);
				}
			}
		}

		/// <summary>
		/// PictureBox control to assign our image to.
		/// </summary>
		[XmlIgnore]
		private PictureBox Canvas
		{
			get 
			{
				if (_profile != null)
					return _profile.GetCanvas();
				else
					return null;
			}
		}

		/// <summary>
		/// Summary:
		///		Gets the Color to use as the background if the image is missing or not visible. Also, this color will display through the 
		///		background image if that image has transparent areas.
		///		
		/// Returns:
		///		The background Color
		/// </summary>
		[XmlIgnore]
		public Color Color
		{
			get { return _color; }
			set
			{
				if (!_color.Equals(value))
				{
					_color = value;
					OnPropertyChanged(this, Property_Color);
				}
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement(Property_Color)]
		public string ColorSerialized
		{
			get { return XmlColor.FromBaseType(_color); }
			set { _color = XmlColor.ToBaseType(value); }
		}

		/// <summary>
		/// Indicates that the background was cleared. This is to differentiate from being blank for Undo/Redo reasons.
		/// When applying a changeset, empty data can imply that there was no change. But if we are undoing the addition
		/// of a background image (from null), then the _image would have been null initially. If Cleared is set (either from a 
		/// Clear() call or loaded up with no data, then we can distinguish from no change to no data.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool Cleared { get; set; }

		/// <summary>
		/// Summary:
		///		Gets the image for the background, with color adjustments and grid added in.
		///		
		/// Returns:
		///		The modified image.
		/// </summary>
		[XmlIgnore]
		public Bitmap CompositeImage
		{
			get { return _composite; }
			set
			{
				if (value == null)
				{
					Clear(false);
					return;
				}
				_composite = value;
			}
		}

		/// <summary>
		/// Name of the file the image was loaded from, or saved to
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set
			{
				if (string.Compare(_filename, value, true) != 0)
				{
					_filename = value;
					OnPropertyChanged(this, Property_Filename);
				}
			}
		}

		public AnchorStyles WallpaperAnchor
		{
			get { return _anchor; }
			set
			{
				if (_anchor != value)
				{
					_anchor = value;
					OnPropertyChanged(this, Property_Anchor);
				}
			}
		}

		/// <summary>
		/// Layout format for the background image on the Canvas.
		/// </summary>
		public WallpaperStyle WallpaperStyle
		{
			get { return _wallpaperStyle; }
			set
			{
				if (_wallpaperStyle != value)
				{
					_wallpaperStyle = value;
					OnPropertyChanged(this, Property_WallpaperStyle);
				}
			}
		}

		/// <summary>
		/// Summary:
		///		Gets the Grid Color to use to paint the grid over the background image (if indicated).
		///		
		/// Returns:
		///		The Grid Color
		/// </summary>
		[XmlIgnore]
		public Color GridColor
		{
			get { return _gridColor; }
			set
			{
				if (!_gridColor.Equals(value))
				{
					_gridColor = value;
					OnPropertyChanged(this, Property_GridColor);
				}
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement(Property_GridColor)]
		public string GridColorSerialized
		{
			get { return XmlColor.FromBaseType(_gridColor); }
			set { _gridColor = XmlColor.ToBaseType(value); }
		}
		
		/// <summary>
		/// Indicates whether an image file has been loaded, or if loaded, was it cleared out.
		/// </summary>
		[XmlIgnore]
		public bool HasData 
		{
			get
			{
				if (Cleared)
					return false;

				if (_image != null)
					return true;

				if (_overlayGrid && !_gridColor.IsEmpty && !_gridColor.Equals(_color))
					return true;

				if (!_color.IsEmpty && !_color.Equals(Color.Black))
					return true;

				return false;
			}
		}

		/// <summary>
		/// Summary:
		///		Gets the hue value adjustment to the background image. Value values are -1.0 to +1.0
		/// 
		/// Returns: 
		///		The floating point value representing the adjusted hue value. Represents the fraction of 360 deg of the color circle
		/// </summary>
		public float Hue
		{
			get { return _hue; }
			set
			{
				if (_hue != value)
				{					
					// Make sure the value stays in the nominal range
					_hue = Math.Max(Math.Min(value, +1f), -1f);
					OnPropertyChanged(this, Property_Hue);
				}
			}
		}
	
		/// <summary>
		/// Summary:
		///		Gets the unaltered image for the background.
		///		
		/// Returns:
		///		The original image.
		/// </summary>
		[XmlIgnore]
		public Bitmap Image
		{
			get { return _image; }
			set
			{
				if (ReferenceEquals(_image, value))
					return;
				_image = value;
				OnPropertyChanged(this, "Image");
			}
		}

		/// <summary>
		/// Indicates whether the grid lines should overlay the background image. Gridlines are drawn in the background color.
		/// </summary>
		public bool OverlayGrid
		{
			get { return _overlayGrid; }
			set
			{
				if (_overlayGrid != value)
				{
					_overlayGrid = value;
					OnPropertyChanged(this, Property_OverlayGrid);
				}
			}
		}

		/// <summary>
		/// Save the image to the temporary directory under a temp filename. Retrieves it from same.
		/// Note: this is moved to the first position so that it is serialized/deserialized first. Otherwise the process of deserialization
		/// would overwrite other values.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement(Property_TempComposite)]
		public string TempCompositeImageFilename
		{
			get { return _tempCompositeFileName; }
			set
			{
				_tempCompositeFileName = value;
				if ((_tempCompositeFileName.Length > 0) && (_tempFileName != NULL))
					_composite = ImageHandler.LoadBitmapFromFile(_tempCompositeFileName);
			}
		}

		/// <summary>
		/// Save the image to the temporary directory under a temp filename. Retrieves it from same.
		/// Note: this is moved to the first position so that it is serialized/deserialized first. Otherwise the process of deserialization
		/// would overwrite other values.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement(Property_TempImage)]
		public string TempImageFilename
		{
			get { return _tempFileName; }
			set
			{
				_tempFileName = value;
				if ((_tempFileName.Length > 0) && (_tempFileName != NULL))
					_image = ImageHandler.LoadBitmapFromFile(_tempFileName);
			}
		}

		/// <summary>
		/// Summary:
		///		Gets the saturation value adjustment to the background image. -2.0 - +1.0
		/// 
		/// Returns: 
		///		The integer value representing the adjusted saturation value.
		/// </summary>
		public float Saturation
		{
			get { return _saturation; }
			set
			{
				if (_saturation != value)
				{
					// Make sure the vlaue stays in the nominal range
					_saturation = Math.Max(Math.Min(value, +1f), -2f);
					OnPropertyChanged(this, Property_Saturation);
				}
			}
		}

		/// <summary>
		/// Indicated whether the image should be 64-bit encoded and saved with in the Profile file itself.
		/// If false, then just the filename is save and is then loaded from disk when the Profile is loaded.
		/// </summary>
		public bool SaveEncodedImage
		{
			get { return _saveEncodedImage; }
			set { _saveEncodedImage = value; }
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<Background>(this);
				return base.Serialized;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore]
		public bool SuppressTempFiles { get; set; }

		/// <summary>
		/// Summary:
		///     Gets the width and height, in pixels, of this image.
		///
		/// Returns:
		///     A System.Drawing.Size structure that represents the width and height, in
		///     pixels, of this image.
		/// </summary>
		[XmlIgnore]
		public Size Size
		{
			get { return _size; }
			set { _size = value; }
		}

		/// <summary>
		/// Summary:
		///		Gets the visibility flag, indicating if the Background Image should be displayed on the CanvasPane PictureBox control in the CanvasWindow form.
		/// 
		/// Returns: 
		///		The visibility indicator
		/// </summary>
		public bool Visible 
		{
			get { return _visible;  }
			set 
			{
				if (_visible != value)
				{
					_visible = value;
					OnPropertyChanged(this, Property_Visible);
				}
			} 
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Background()
			: base()
		{
			SuppressTempFiles = false;
			Clear(true);
		}

		public Background(BaseProfile profile)
			: this()
		{
			_profile = profile;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Removes any existing background image from the Canvas and replaces it with the prebuilt composite image.
		/// </summary>
		private void ApplyComposite(PictureBox canvasPane)
		{
			//if (_profile == null)
			//	return;

			if (canvasPane == null)
				throw new ArgumentNullException("CanvasPane cannot be null.");

			if (_composite == null)
				return;

			canvasPane.BackgroundImage = null;
			canvasPane.BackgroundImage = _composite;


			//if (_composite == null)
			//	return;

			//var dest = new Bitmap(_composite.Width, _composite.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			//using (var gr = Graphics.FromImage(dest))
			//{
			//	gr.DrawImage(_composite, new Rectangle(Point.Empty, dest.Size));
			//}
			//if (canvasPane.Image != null)
			//	canvasPane.Dispose();
			//canvasPane.Image = dest;
		}

		/// <summary>
		/// Clone the original bitmap, resize it to the new zoom value, change the brightness to the correct value, fill the background with our chosen color, then overlay the brightness adjusted image ontop
		/// of the filled background.
		/// </summary>
		public void BuildCompositeImage(Scaling scaling)
		{
			//if (!HasData)
			//	return;

			if (_composite != null)
			{
				_composite.Dispose();
				_composite = null;
			}

			float Zoom = scaling.Zoom.GetValueOrDefault(1.0f);
			Size CanvasSize = scaling.CanvasSize;
			if (CanvasSize.IsEmpty)
				return;
			int CellScale = scaling.CellScale;
			int X, Y;
			int W, H;
			int zW, zH;
			int zWC, zHC;

			ImageAttributes attrs = new ImageAttributes();

			// Create a temporary bitmap that gets the original image color adjusted.
			Bitmap Temp = new Bitmap(CanvasSize.Width, CanvasSize.Height);
			using (Graphics g = Graphics.FromImage(Temp))
			{
				g.Clear(Color.Transparent);

				if (_image != null)
				{
					W = _image.Width;
					H = _image.Height;
					zW = (int)Math.Ceiling(_image.Width * Zoom);
					zH = (int)Math.Ceiling(_image.Height * Zoom);
					zWC = (int)Math.Ceiling(CanvasSize.Width * Zoom);
					zHC = (int)Math.Ceiling(CanvasSize.Height * Zoom);
					Rectangle FinalSize = new Rectangle(0, 0, zW, zH);

					float XPct = (float)W / (float)CanvasSize.Width;
					float YPct = (float)H / (float)CanvasSize.Height;

					QColorMatrix qm = new QColorMatrix();
					qm.RotateHue((int)(_hue * 360));
					qm.SetSaturation2(_saturation);
					qm.SetBrightness(_brightness);
					attrs.SetColorMatrix(qm.ToColorMatrix());
					qm = null;

					switch (_wallpaperStyle)
					{ 
						case WallpaperStyle.Fill:
							switch(_anchor)
							{
								case AnchorStyles.Bottom | AnchorStyles.Right:
									FinalSize.Offset(CanvasSize.Width - zW, CanvasSize.Height - zH);
									g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
									break;
								case AnchorStyles.Bottom | AnchorStyles.Left:
									FinalSize.Offset(0, CanvasSize.Height - zH);
									g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
									break;
								case AnchorStyles.Top | AnchorStyles.Right:
									FinalSize.Offset(CanvasSize.Width - zW, 0);
									g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
									break;
								case AnchorStyles.Top | AnchorStyles.Left:
								default:
									g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
									break;
							}
							break;
						
						case WallpaperStyle.Fit:
							if (XPct < YPct)
								FinalSize = new Rectangle(0, 0, CanvasSize.Width, (int)Math.Ceiling(H / XPct));
							else
								FinalSize = new Rectangle(0, 0, (int)Math.Ceiling(zW / YPct), CanvasSize.Height);
							g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
							break;
						
						case WallpaperStyle.Stretch:
							g.DrawImage(_image, new Rectangle(0, 0, CanvasSize.Width, CanvasSize.Height), 0, 0, W, H, GraphicsUnit.Pixel, attrs);
							break;
						
						case WallpaperStyle.Tile:
							int Cols = (int)Math.Ceiling(1 / XPct);
							int Rows = (int)Math.Ceiling(1 / YPct);
							for (int i = 0; i < Cols; i++)
								for (int j = 0; j < Rows; j++)
								{
									X = i * W;
									Y = j * H;
									FinalSize = new Rectangle(X, Y, zW, zH);
									g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
								}
							break;
						
						case WallpaperStyle.Center:
							X = (CanvasSize.Width - zW) / 2;
							Y = (CanvasSize.Height - zH) / 2;
							FinalSize.Offset(X, Y);
							g.DrawImage(_image, FinalSize, 0, 0, W, H, GraphicsUnit.Pixel, attrs);
							break;
					}
					qm = null;
				}
			}

			_composite = new Bitmap(CanvasSize.Width, CanvasSize.Height);
			using (Graphics g = Graphics.FromImage(_composite))
			{			

				// Create a base fill of the background color.
				g.Clear(_color);

				// Drop in our Temp image ontop of the base color
				g.DrawImage(Temp, new Rectangle(0, 0, CanvasSize.Width, CanvasSize.Height));
				Temp.Dispose();
				Temp = null;

				// Draw the grid over top the composite image.
				if (_overlayGrid && scaling.ShowGridLines.GetValueOrDefault(true))
				{
					GraphicsPath Grid = new GraphicsPath();
					Grid.FillMode = FillMode.Alternate;

					// Add a rectangle to cover the entire Canvas
					Grid.AddRectangle(new Rectangle(0, 0, CanvasSize.Width, CanvasSize.Height));

					// Now add rectangles that represent all the possible Cells and add them to the path
					int CellSize = scaling.CellSize.GetValueOrDefault(1);
					int CellGrid = scaling.CellGrid;
					int GridLineWidth = scaling.GridLineWidth;
					Rectangle CellRect = new Rectangle(0, 0, CellSize, CellSize);

					for (int i = 0; i <= scaling.LatticeSize.Width; i++)
						for (int j = 0; j <= scaling.LatticeSize.Height; j++)
					{
						CellRect.Location = new Point(GridLineWidth + (i * CellGrid), GridLineWidth + (j * CellGrid));
						Grid.AddRectangle(CellRect);
					}

					if (Zoom != Scaling.ZOOM_100)
					{
						// Rescale the path to that of the zoom.
						using (Matrix ScaleMatrix = new Matrix())
						{
							ScaleMatrix.Scale(Zoom, Zoom);
							Grid.Transform(ScaleMatrix);
						}
					}

					g.FillPath(new SolidBrush(_gridColor), Grid);
				}
			}
			_tempCompositeFileName = SaveTempFile(_composite, true);
		}

		/// <summary>
		/// Clears out the current values for the background image, sets the Cleared flag and removes the background image from the current canvas
		/// </summary>
		public void Clear(bool setFlag)
		{
			if (setFlag)				
			    Cleared = true;

			Clear();
			if (_profile != null)
			{
				PictureBox Canvas = _profile.GetCanvas();
				if (Canvas != null)
				{
					Canvas.BackgroundImage = null;
					Canvas = null;
				}
			}
		}

		/// <summary>
		/// Clears out the current values for the background image.
		/// </summary>
		public void Clear()
		{
			_filename = string.Empty;
			_wallpaperStyle = WallpaperStyle.Fill;
			_anchor = AnchorStyles.Left | AnchorStyles.Top;
			_visible = true;
			_color = Color.Black;
			_gridColor = Color.Black;
			_brightness = Default_Brightness;
			_hue = Default_Hue;
			_saturation = Default_Saturation;
			_overlayGrid = false;
			_tempFileName = string.Empty;
			_tempCompositeFileName = string.Empty;
			_saveEncodedImage = true;
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
			if (_composite != null)
			{
				_composite.Dispose();
				_composite = null;
			}
		}

		/// <summary>
		/// Clone this object, making a new copy of the image object
		/// </summary>
		public override object Clone()
		{
			Background MyClone = (Background)base.Clone();

			if (MyClone.TempImageFilename.Length > 0)
				MyClone.Image = ImageHandler.LoadBitmapFromFile(MyClone.TempImageFilename);
			if (MyClone.TempCompositeImageFilename.Length > 0)
				MyClone.CompositeImage = ImageHandler.LoadBitmapFromFile(MyClone.TempCompositeImageFilename);

			return MyClone;
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(Background source)
		{
			SuppressEvents = true;

			_brightness = source.Brightness;
			_hue = source.Hue;
			_saturation = source.Saturation;
			_color = source.Color;
			_gridColor = source.GridColor;
			_filename = source.Filename;
			_visible = source.Visible;
			_overlayGrid = source.OverlayGrid;
			_size = source.Size;
			_wallpaperStyle = source.WallpaperStyle;
			_anchor = source.WallpaperAnchor;
			_saveEncodedImage = source.SaveEncodedImage;
			Cleared = source.Cleared;
			_image = ImageHandler.CopyImage(source.Image);
			_composite = ImageHandler.CopyImage(source.CompositeImage);
			_tempCompositeFileName = source.TempCompositeImageFilename;
			_tempFileName = source.TempImageFilename;
		
			SuppressEvents = false;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();

			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
			if (_composite != null)
			{
				_composite.Dispose();
				_composite = null;
			}
			if (_tempCompositeFileName.Length > 0)
			{
				File.Delete(_tempCompositeFileName);
			}
			if (_tempFileName.Length > 0)
			{
				File.Delete(_tempFileName);
			}
			_xmlHelper = null;
		}

		/// <summary>
		/// Converts the Brightness stored value into a floating point number.
		/// </summary>
		/// <returns></returns>
		public float GetBrightnessPercent()
		{ 
			return (_brightness - 10.0f) / 10.0f;
		}

		/// <summary>
		/// Gets a temporary filename to save the image or the composite image.
		/// </summary>
		/// <param name="composite">Indicates whether the name should include an indication that this is the composite image.</param>
		private string GetTempFilename(bool composite)
		{
			string Filename = Path.GetTempFileName();
			Filename = Filename.Replace(".tmp", ".png");
			Filename = Filename.Replace("\\tmp", composite ? "\\bgc_tmp" : "\\bg_tmp");
			return Filename;
		}

		/// <summary>
		/// Loads the image in from a file
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="applyComposite">Indicates whether the composite image should be generated and applied to the Profile's Canvas.</param>
		/// <returns>Returns true if the file loaded successfully, false otherwise.</returns>
		public bool LoadFromFile(string filename, bool applyComposite)
		{
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
			if (_composite != null)
			{
				_composite.Dispose();
				_composite = null;
			}
			Filename = filename;
			_image = ImageHandler.LoadBitmapFromFile(filename);
			if (_image == null)
			{
				MessageBox.Show("Unable to load this file.", @"Load Image File", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}

			_tempFileName = SaveTempFile(_image, false);

			if (applyComposite)
			{
				BuildCompositeImage(_profile.Scaling);
				ApplyComposite(_profile.GetCanvas());
			}

			return true;
		}

		/// <summary>
		/// Loads the image in from a file
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the file loaded successfully, false otherwise.</returns>
		public bool LoadFromFile(string filename)
		{
			return LoadFromFile(filename, true);
		}

		/// <summary>
		/// Load the image from an encoded string from the Profile Xml.
		/// </summary>
		/// <param name="encodedStream">string encoded byte array, holding image data. If the string is empty, then no image is encoded</param>
		public bool LoadFromStream(string encodedStream)
		{
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
			if (_composite != null)
			{
				_composite.Dispose();
				_composite = null;
			}
			_image = ImageHandler.LoadBitmapFromEncoded(encodedStream);
			_tempFileName = SaveTempFile(_image, false);

			return HasData;
		}

		/// <summary>
		/// Saves the bitmap to a temporary file.
		/// </summary>
		/// <param name="bitmap">Image or CompositeImage</param>
		/// <param name="isComposite">Indicates if this is the composite image.</param>
		/// <returns>Filename to the temp file.</returns>
		private string SaveTempFile(Bitmap bitmap, bool isComposite)
		{
			if (SuppressTempFiles)
				return NULL;

			string Filename = string.Empty;
			if (bitmap != null)
			{
				Filename = GetTempFilename(isComposite);
				bitmap.Save(Filename, ImageFormat.Png);
			}
			return Filename;
		}

		/// <summary>
		/// Builds the composite image, removes the old background image and puts the composite one in its place
		/// </summary>
		public void Set()
		{
			Set(true);
		}

		/// <summary>
		/// Builds the composite image, removes the old background image and puts the composite one in its place
		/// </summary>
		public void Set(bool createComposite)
		{
			if (createComposite)
				BuildCompositeImage(_profile.Scaling);
			ApplyComposite(_profile.GetCanvas());
		}

		/// <summary>
		/// Sets the background image to the particular picturebox using the particular scaling
		/// </summary>
		/// <param name="scaling">Scaling to use instead of from the Profile.</param>
		/// <param name="canvasPane">PictureBox to use instead of from the Profile.</param>
		public void Set(Scaling scaling, PictureBox canvasPane)
		{
			BuildCompositeImage(scaling);
			ApplyComposite(canvasPane);
		}

		/// <summary>
		/// Converts an integer value to the corner Anchor values.
		/// </summary>
		/// <param name="anchorVal">Integer representing two AnchorStyle enums combined, ie AnchorStyle.Top | AnchorStyle.Left</param>
		/// <returns>The AnchorStyles enum derived from the integer.</returns>
		public static AnchorStyles GetAnchorEnum(int anchorVal)
		{
			AnchorStyles Anchor = AnchorStyles.None;
			if (((int)AnchorStyles.Top & anchorVal) == (int)AnchorStyles.Top)
				Anchor = AnchorStyles.Top;
			else
				Anchor = AnchorStyles.Bottom;

			if (((int)AnchorStyles.Left & anchorVal) == (int)AnchorStyles.Left)
				Anchor |= AnchorStyles.Left;
			else
				Anchor |= AnchorStyles.Right;

			return Anchor;
		}

		#endregion [ Methods ]

		#region [ Event Triggers ]

		/// <summary>
		/// Called when a property's value has been changed. Raises the PropertyChanged event with the name of the Property.
		/// If indicated, sets the Dirty flag for this object.
		/// </summary>
		/// <param name="sender">Object whose property changed.</param>
		/// <param name="propertyName">Name of the property that has been changed.</param>
		protected override void OnPropertyChanged(object sender, string propertyName)
		{
			_serialized = string.Empty;
			Cleared = false;

			if (SuppressEvents)
				return;

			base.OnPropertyChanged(sender, propertyName);
		}

		#endregion [ Event Triggers ]
	}
}

