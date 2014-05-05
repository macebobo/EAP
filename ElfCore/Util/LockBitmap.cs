using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ElfCore.Util
{
	/// <summary>
	/// http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp
	/// </summary>
	public class LockBitmap
	{
		#region [ Private Variables ]

		private Bitmap _source = null;
		private BitmapData _bitmapData = null;

		#endregion [ Private Variables ]

		#region [ Public Fields ]

		public byte[] Pixels { get; set; }
		public int Depth { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		/// <summary>
		/// The address of the first pixel data in the bitmap. This can also be thought of as the first scan line in the bitmap.
		/// </summary>
		public IntPtr Scan0 { get; private set; }

		public int DataLength { get; private set; }

		/// <summary>
		/// The stride is the width of a single row of pixels (a scan line), rounded up to a four-byte boundary. 
		/// If the stride is positive, the bitmap is top-down. If the stride is negative, the bitmap is bottom-up.
		/// </summary>
		public int Stride { get; private set; }

		#endregion [ Public Fields ]

		#region [ Constructors ]

		public LockBitmap(Bitmap source)
		{
			_source = source;
			Scan0 = IntPtr.Zero;
			DataLength = 0;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void FillPixelArray()
		{
			LockBits();
			UnlockBits();
		}

		/// <summary>
		/// Get the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Color GetPixel(int x, int y)
		{
			Color clr = Color.Empty;

			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (i > Pixels.Length - cCount)
				throw new IndexOutOfRangeException();

			if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				byte a = Pixels[i + 3]; // a
				clr = Color.FromArgb(a, r, g, b);
			}
			if (Depth == 24) // For 24 bpp get Red, Green and Blue
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				clr = Color.FromArgb(r, g, b);
			}
			if (Depth == 8)
			// For 8 bpp get color value (Red, Green and Blue values are the same)
			{
				byte c = Pixels[i];
				clr = Color.FromArgb(c, c, c);
			}
			return clr;
		}

		/// <summary>
		/// Lock bitmap data
		/// </summary>
		public void LockBits()
		{
			try
			{
				// Get width and height of bitmap
				Width = _source.Width;
				Height = _source.Height;

				// get total locked pixels count
				int PixelCount = Width * Height;

				// Create rectangle to lock
				Rectangle rect = new Rectangle(0, 0, Width, Height);

				// get source bitmap pixel format size
				Depth = Bitmap.GetPixelFormatSize(_source.PixelFormat);

				// Check if bpp (Bits Per Pixel) is 8, 24, or 32
				if (Depth != 8 && Depth != 24 && Depth != 32)
					throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");

				// Lock bitmap and return bitmap data
				_bitmapData = _source.LockBits(rect, ImageLockMode.ReadWrite, _source.PixelFormat);

				// create byte array to copy pixel values
				int step = Depth / 8;

				Pixels = new byte[PixelCount * step];
				Scan0 = _bitmapData.Scan0;
				Stride = _bitmapData.Stride;

				// Copy data from pointer to array
				Marshal.Copy(Scan0, Pixels, 0, Pixels.Length);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}

		/// <summary>
		/// Set the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
				Pixels[i + 3] = color.A;
			}
			if (Depth == 24) // For 24 bpp set Red, Green and Blue
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
			}
			if (Depth == 8)
			// For 8 bpp set color value (Red, Green and Blue values are the same)
			{
				Pixels[i] = color.B;
			}
		}

		/// <summary>
		/// Unlock bitmap data
		/// </summary>
		public void UnlockBits()
		{
			try
			{
				// Copy data from byte array to pointer
				Marshal.Copy(Pixels, 0, Scan0, Pixels.Length);

				// Unlock bitmap data
				_source.UnlockBits(_bitmapData);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}

		#endregion [ Methods ]
	}
}
