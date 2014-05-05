using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore
{
	/// <summary>
	/// Handles the loading, saving and brightness manipulate of the Canvas background image.
	/// </summary>
	public class BackgroundImage : IDisposable, ICloneable
	{
		#region [ Private Variables ]

		private bool _disposed = false;

		/// <summary>
		/// Composited image, consisting of the Background color, with the image loaded, then brightness adjusted.
		/// </summary>
		public Bitmap _composite = null;

		/// <summary>
		/// Original, unaltered image.
		/// </summary>
		public Bitmap _image = null;

		/// <summary>
		/// Brightness value
		/// </summary>
		private int _brightness = 10;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Summary:
		///		Gets the brightness value added to the background image. Value values are 0 to 20
		/// 
		/// Returns: 
		///		The integer value representing the set brightness value. A value of 10 indicates no changes in brightness.
		/// </summary>
		public int Brightness
		{
			get { return _brightness; }
			set
			{
				if (_brightness == value)
					return;
				// Make sure the vlaue stays in the nominal range
				_brightness = (int)(Math.Max(Math.Min(value, 20), 0));
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
		public Color Color { get; set; }

		/// <summary>
		/// Indicates that the background was cleared. This is to differentiate from being blank for Undo/Redo reasons
		/// </summary>
		public bool Cleared { get; private set; }

		/// <summary>
		/// Name of the file the image was loaded from, or saved to
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Indicates whether an image file has been loaded, or if loaded, was it cleared out.
		/// </summary>
		public bool HasData { get; private set; }
	
		/// <summary>
		/// Summary:
		///		Gets the brightness adjusted, filled image to be displayed as the background for the Canvas. When setting this property,
		///		the original, unaltered bitmap is used. It will then be adjusted for brightness, and have its background filled with the color
		///		indicated.
		///		
		/// Returns:
		///		The modified image.
		/// </summary>
		public Bitmap Image
		{
			get 
			{
				if (Visible)
					return _composite;
				else
					return null;
			}
			set
			{
				if (value == null)
				{
					Clear(false);
					return;
				}
				_image = value;
				HasData = true;
			}
		}

		/// <summary>
		/// Summary:
		///     Gets the width and height, in pixels, of this image.
		///
		/// Returns:
		///     A System.Drawing.Size structure that represents the width and height, in
		///     pixels, of this image.
		/// </summary>
		public Size Size { get; set; }

		/// <summary>
		/// Summary:
		///		Gets the visibility flag, indicating if the Background Image should be displayed on the CanvasPane PictureBox control in the CanvasWindow form.
		/// 
		/// Returns: 
		///		The visibility indicator
		/// </summary>
		public bool Visible { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public BackgroundImage()
		{
			Filename = string.Empty;
			HasData = false;
			Visible = true;
			Cleared = true;
			Color = Color.Black;
		}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~BackgroundImage()
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
				Clear(true);
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
		/// Moves a copy of the composite image to the background image property on the Canvas
		/// </summary>
		private void AssignBackgroundImageToCanvas()
		{
			RemoveBackgroundImageFromCanvas();

			if ((Workshop.Canvas != null) && (_composite != null))
				Workshop.Canvas.BackgroundImage = (Bitmap)_composite.Clone();
		}

		/// <summary>
		/// Clone the original bitmap, resize it to the new zoom value, change the brightness to the correct value, fill the background with our chosen color, then overlay the brightness adjusted image ontop
		/// of the filled background.
		/// </summary>
		private void BuildCompositeImage()
		{
			if (_composite != null)
				_composite.Dispose();

			if (this.Size.IsEmpty || (_image == null))
				return;

			Cleared = false;
			float fBrightness = (_brightness - 10) / 10.0f;

			// Scale the image by the Zoom factor
			Size NewSize = new Size((int)(this.Size.Width * UISettings.ʃZoomF), (int)(this.Size.Height * UISettings.ʃZoomF));

			_composite = new Bitmap(NewSize.Width, NewSize.Height);
			using (Graphics g = Graphics.FromImage(_composite))
			{
				if (this.Color.IsEmpty)
					g.Clear(Color.Transparent);
				else
					g.Clear(this.Color);

				if (_image != null)
				{
					if (_brightness != 0.0)
					{
						ColorMatrix cMatrix = new ColorMatrix(new float[][] {
							new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
							new float[] { fBrightness, fBrightness, fBrightness, 0.0f, 1.0f }
						});
						ImageAttributes attrs = new ImageAttributes();
						attrs.SetColorMatrix(cMatrix);
						g.DrawImage(_image, new Rectangle(0, 0, NewSize.Width, NewSize.Height), 0, 0, this.Size.Width, this.Size.Height, GraphicsUnit.Pixel, attrs);
					}
				}
			}
		}

		/// <summary>
		/// Clears out the current values for the background image.
		/// </summary>
		public void Clear(bool setFlag)
		{
			if (setFlag)				
				this.Cleared = true;

			this.Filename = string.Empty;
			this.Visible = true;
			this.HasData = false;
			this.Size = Size.Empty;
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
			RemoveBackgroundImageFromCanvas();
		}

		/// <summary>
		/// Clone this object, making a new copy of the image object
		/// </summary>
		public object Clone()
		{
			BackgroundImage Cloned = (BackgroundImage)this.MemberwiseClone();

			// MemberwiseClone copies references, so we need to create new image for the Clone
			if (this.Image != null)
				Cloned.Image = new Bitmap(this.Image);

			return Cloned;
		}

		/// <summary>
		/// Get the unmodified image
		/// </summary>
		/// <returns></returns>
		public Bitmap GetBaseImage()
		{
			return _image;
		}

		/// <summary>
		/// Loads the image in from a file
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the file loaded successfully, false otherwise.</returns>
		public bool LoadFromFile(string filename)
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

			MemoryStream Stream = null;
			FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] fileBytes = new byte[fs.Length];
			fs.Read(fileBytes, 0, (int)fs.Length);
			fs.Close();
			fs.Dispose();

			try
			{
				Stream = new MemoryStream(fileBytes);
				_image = (Bitmap)Bitmap.FromStream(Stream);

				// Roll back to the start of the stream.
				//Stream.Seek(0, 0);
				//this.Hash = Workshop.GetMD5HashFromStream(Stream);

				if (_image != null)
					this.Size = _image.Size;
				this.Filename = filename;

				BuildCompositeImage();
				AssignBackgroundImageToCanvas();

				return true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return false;
			}
			finally
			{
				Stream.Close();
				Stream.Dispose();
				//HasData = (Hash.Length > 0);
				HasData = (_image != null);
			}
		}

		/// <summary>
		/// Load the image from an encoded string from the Profile Xml.
		/// </summary>
		/// <param name="encodedStream">string encoded byte array, holding image data. If the string is empty, then no image is encoded</param>
		private bool LoadFromStream(string encodedStream)
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

			byte[] imageByteArray = Convert.FromBase64String(encodedStream);
			if (imageByteArray.Length == 0)
				return false;

			// bytes are the bytes from the original file
			MemoryStream Stream = new MemoryStream(imageByteArray);
			_image = (Bitmap)Bitmap.FromStream(Stream);

			// Roll back to the start of the stream.
			//Stream.Seek(0, 0);
			//this.Hash = Workshop.GetMD5HashFromStream(Stream);

			Stream.Close();
			Stream.Dispose();
			Stream = null;

			HasData = (_image != null);

			return true;
		}

		/// <summary>
		/// Load in the settings from the Profile that are related to the background image.
		/// </summary>
		/// <param name="profile">XmlDocument that contains the profile data</param>
		/// <param name="settings">Settings object, used to facilitate retrieving the data</param>
		public void LoadSettings(XmlNode parentNode, Settings settings)
		{
			this.Filename = settings.GetValueAttribute(parentNode, Constants.XML_BACKGROUND_IMAGE, "filename", string.Empty);
			if (!LoadFromStream(settings.GetValue(parentNode, Constants.XML_BACKGROUND_IMAGE, string.Empty)))
				LoadFromFile(Filename);

			try
			{
				this.Color = Color.FromArgb(settings.GetValueAttribute(parentNode, Constants.XML_BACKGROUND_IMAGE, "color", Color.Black.ToArgb()));
			}
			catch
			{
				this.Color = Color.Black;
			}

			if (_image != null)
				this.Size = new Size( _image.Width, _image.Height);
			_brightness = settings.GetValue(parentNode, string.Format("{0}/{1}", Constants.XML_DISPLAY, Constants.XML_BRIGHTNESS), 10);

			//BuildCompositeImage();
		}

		/// <summary>
		/// Removes the background image from the Canvas and destroys it
		/// </summary>
		private void RemoveBackgroundImageFromCanvas()
		{
			if ((Workshop.Canvas != null) && (Workshop.Canvas.BackgroundImage != null))
			{
				Workshop.Canvas.BackgroundImage.Dispose();
				Workshop.Canvas.BackgroundImage = null;
			}
		}

		/// <summary>
		/// Saves the data back to the Profile Xml object
		/// </summary>
		public void SaveSettings(XmlNode parentNode, Settings settings)
		{
			string Encoded = string.Empty;

			if (HasData) 
			{
				if (this.Filename.Length > 0)
				{
					// Load in the file
					FileStream fs = new FileStream(this.Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
					byte[] ImageBytes = new byte[fs.Length];
					fs.Read(ImageBytes, 0, (int)fs.Length);
					Encoded = Convert.ToBase64String(ImageBytes);
					fs.Close();
					fs.Dispose();
				}
				else
				{
					// Convert the saved image to a byte array
					try
					{
						MemoryStream ms = new MemoryStream();
						_image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
						Encoded = Convert.ToBase64String(ms.ToArray());
						ms.Dispose();
					}
					catch
					{
						Encoded = Convert.ToBase64String(new byte[0]);
					}
				}
			}
			else
				Encoded = Convert.ToBase64String(new byte[0]);

			//settings.SetValue(parentNode, string.Format("/{0}/{1}", Constants.XML_DISPLAY, Constants.XML_BRIGHTNESS), _brightness);
			//settings.SetValue(parentNode, string.Format("/{0}", Constants.XML_BACKGROUND_IMAGE), Encoded);
			//settings.SetValue(parentNode, string.Format("/{0}[@filename]", Constants.XML_BACKGROUND_IMAGE), this.Filename);
			//settings.SetValue(parentNode, string.Format("/{0}[@color]", Constants.XML_BACKGROUND_IMAGE), Color.ToArgb());
		}

		/// <summary>
		/// Record all the Background settings into the ChangeSet
		/// </summary>
		public void SaveUndoData(UndoData.ChangeSet changes)
		{
			changes.BackgroundImage.Cleared = this.Cleared;
			if (HasData)
			{
				if (!Cleared && (_image != null))
				{
					changes.BackgroundImage.BaseImage = new Bitmap(_image);
					changes.BackgroundImage.Brightness = _brightness;
					changes.BackgroundImage.Color = this.Color;
					changes.BackgroundImage.Filename = this.Filename;
					changes.BackgroundImage.Visible = this.Visible;
				}
			}
		}

		/// <summary>
		/// Builds the composite image, removes the old background image and puts the composite one in its place
		/// </summary>
		public void Set()
		{
			BuildCompositeImage();
			AssignBackgroundImageToCanvas();
		}

		#endregion [ Methods ]
	}

	internal class BackgroundImageListItem
	{
		public string Text = "No Background Image";
		public Image Icon = ElfRes.no_background_image;

		public override string ToString()
		{
			return Text;
		}
	}
}
