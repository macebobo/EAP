using System.Drawing.Text;

using ElfControls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Util
{
	public static class ImageHandler
	{
		#region [ Static Variables ]

		/// <summary>
		/// swatch [swoch]
		/// noun
		/// 1. a sample of cloth or other material.
		/// 2. a sample, patch, or characteristic specimen of anything.
		/// </summary>
		public static Dictionary<string, Bitmap> ColorSwatches = new Dictionary<string, Bitmap>();

		public static Dictionary<string, Bitmap> RawImages = new Dictionary<string, Bitmap>();

		public static Dictionary<string, Bitmap> TintedImages = new Dictionary<string, Bitmap>();

		#endregion [ Static Variables ]

		#region [ Static Methods ]

		#region [ AddAnnotation ]

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation)
		{
			return AddAnnotation(image, annotation, AnchorStyles.Top | AnchorStyles.Left, Point.Empty, true);
		}

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <param name="offset">Amount to offset the source image</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation, Point offset)
		{
			return AddAnnotation(image, annotation, AnchorStyles.Top | AnchorStyles.Left, offset, true);
		}

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation, AnchorStyles anchor)
		{
			return AddAnnotation(image, annotation, anchor, Point.Empty, true);
		}

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <param name="offset">Amount to offset the source image</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation, AnchorStyles anchor, Point offset)
		{
			return AddAnnotation(image, annotation, anchor, offset, true);
		}

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <param name="overlap">Indicates whether the annotation image should be over the image, or if the image should be on top of the annotation image.</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation, AnchorStyles anchor, bool overlap)
		{
			return AddAnnotation(image, annotation, anchor, Point.Empty, overlap);
		}

		/// <summary>
		/// Combines an image with a bitmap representing an Annotation
		/// </summary>
		/// <param name="image">Source image to modify</param>
		/// <param name="annotation">Annotation enum that indicates what Icon to insert</param>
		/// <param name="anchor">AnchorStyles enum that indicates how the annotation should be positioned relative to the image.</param>
		/// <param name="offset">Amount to offset the source image</param>
		/// <param name="overlap">Indicates whether the annotation image should be over the image, or if the image should be on top of the annotation image.</param>
		/// <returns>The composited image</returns>
		public static Bitmap AddAnnotation(Bitmap image, Annotation annotation, AnchorStyles anchor, Point offset, bool overlap)
		{
			if (image == null)
				throw new ArgumentNullException("image cannot be null.");

			Bitmap Copy = new Bitmap(image.Width, image.Height);

			Bitmap annotationImage = GetAnnotationImage(annotation);
			if (annotationImage == null)
				throw new Exception("annotation not found: " + annotation.ToString());

			int Width = annotationImage.Width;
			int Height = annotationImage.Height;
			int X = 0;
			int Y = 0;

			if ((anchor & AnchorStyles.Left) == AnchorStyles.Left)
				X = 0;
			else if ((anchor & AnchorStyles.Right) == AnchorStyles.Right)
				X = Copy.Width - Width;
			else
				X = (Copy.Width - Width) / 2;

			if ((anchor & AnchorStyles.Top) == AnchorStyles.Top)
				Y = 0;
			else if ((anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
				Y = Copy.Height - Height;
			else
				Y = (Copy.Height - Height) / 2;

			using (Graphics g = Graphics.FromImage(Copy))
			{
				g.Clear(Color.Transparent);
				if (overlap)
					g.DrawImage(image, new Rectangle(offset.X, offset.Y, image.Width, image.Height));
				g.DrawImage(annotationImage, new Rectangle(X, Y, Width, Height));
				if (!overlap)
					g.DrawImage(image, new Rectangle(offset.X, offset.Y, image.Width, image.Height));
			}
			

			return Copy;
		}

		#endregion [ AddAnnotation ]

		/// <summary>
		/// Blends two colors together.
		/// </summary>
		/// <param name="c1">First color to blend.</param>
		/// <param name="c2">Second color to blend.</param>
		/// <returns>Returns a new color that is the average of the Reds, Greens and Blues of the two colors.</returns>
		public static Color ColorMixer(Color c1, Color c2)
		{
			int Red = (c1.R + c2.R) / 2;
			int Green = (c1.G + c2.G) / 2;
			int Blue = (c1.B + c2.B) / 2;

			return Color.FromArgb((byte)Red, (byte)Green, (byte)Blue);
		}

		/// <summary>
		/// Creates a copy of the bitmap.
		/// </summary>
		/// <param name="b">Image to copy.</param>
		public static Bitmap CopyImage(Bitmap b)
		{
			if (b == null)
				return null;
			Bitmap b2 = new Bitmap(b.Width, b.Height);
			using (Graphics g = Graphics.FromImage(b2))
				g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel);
			return b2;
		}

		/// <summary>
		/// Create a 16x16 image of a 12x12 square of the color of the channel passed in, with a black border
		/// </summary>
		/// <param name="color">Color used to generate the swatch</param>
		/// <returns>Returns the generated bitmap</returns>
		public static Bitmap CreateColorSwatch(Color color)
		{
			Swatch s = new Swatch(color);
			s.Key = Guid.NewGuid().ToString();
			return CreateColorSwatch(ref s);
		}

		/// <summary>
		/// Create a 16x16 image of a 12x12 square of the color of the channel passed in, with a black border
		/// </summary>
		/// <param name="swatchData">Color and properties used to generate the swatch</param>
		/// <returns>Returns the generated bitmap</returns>
		public static Bitmap CreateColorSwatch(ref Swatch swatchData)
		{

			if (swatchData.Key.Length == 0)
				swatchData.Key = swatchData.MakeKey();

			// If this color is already present, then jump out.
			if (ColorSwatches.ContainsKey(swatchData.Key))
				return null;

			GraphicsPath Path = new GraphicsPath();
			Path.FillMode = FillMode.Winding;

			Bitmap Swatch = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
			int MaxWidth = 15;
			int X = 0;

			using (Graphics g = Graphics.FromImage(Swatch))
			{
				Rectangle R = new Rectangle(X, X, MaxWidth, MaxWidth);
				Path.AddRectangle(R);
				g.FillPath(new SolidBrush(Color.FromArgb(85, Color.DimGray)), Path);
				Path.Reset();
				R = new Rectangle(X + 1, X + 1, MaxWidth, MaxWidth);
				Path.AddRectangle(R);
				g.FillPath(new SolidBrush(Color.FromArgb(85, Color.DimGray)), Path);

				ColorManager.HSL DarkerColor = new ColorManager.HSL(swatchData.Color);
				Color CornerColor = ControlPaint.Dark(swatchData.Color);

				if (DarkerColor.L < 1/8f)
				{
					// Starting with a very dark color, make the dark corner into a light corner.
					CornerColor = ControlPaint.LightLight(swatchData.Color);
					CornerColor = ControlPaint.LightLight(CornerColor);
				}
				else
				{
					DarkerColor.L /= 2f;
					if (DarkerColor.L > 0.5)
						// light color, make the darker color even darker
						CornerColor = ControlPaint.DarkDark(swatchData.Color);
					else
						CornerColor = ControlPaint.Dark(swatchData.Color);
				}
				DarkerColor = null;

				R = new Rectangle(X, X, MaxWidth - 2, MaxWidth - 2);

				using (LinearGradientBrush br = new LinearGradientBrush(R, Color.Blue, Color.White, 60f))
				{
					// Create a ColorBlend object. Note that you must initialize it before you save it in the brush's InterpolationColors property.
					ColorBlend colorBlend = new ColorBlend();
					colorBlend.Colors = new Color[] { swatchData.Color, swatchData.Color, swatchData.Color, CornerColor };
					colorBlend.Positions = new float[] { 0.0f, 0.5f, 0.6f, 1f };
					br.InterpolationColors = colorBlend;
					g.FillRectangle(br, R);
				}

				if (swatchData.BorderColor.IsEmpty)
					g.DrawRectangle(Pens.Black, R);
				else
					using (Pen Pen = new Pen(swatchData.BorderColor))
						g.DrawRectangle(Pen, R);

				if (!swatchData.IsEnabled)
				{
					// draw as a rectangle with a diagonal slash hatch pattern
					HatchBrush hBrush = new HatchBrush(HatchStyle.LightUpwardDiagonal, SystemColors.GrayText, Color.Transparent);

					g.FillRectangle(hBrush, R);
					hBrush.Dispose();
					hBrush = null;
					g.DrawRectangle(SystemPens.GrayText, R);
				}

				// Tag the image with the integer value of the color
				Swatch.Tag = swatchData.Key;

				if (!swatchData.IsIncluded)
					Swatch = AddAnnotation(Swatch, Annotation.Exclude, AnchorStyles.Top | AnchorStyles.Right);

				if (!swatchData.IsVisible)
				    Swatch = AddAnnotation(Swatch, Annotation.Invisible, AnchorStyles.Bottom | AnchorStyles.Left);

				if (swatchData.IsLocked)
					Swatch = AddAnnotation(Swatch, Annotation.Locked, AnchorStyles.Bottom | AnchorStyles.Right);

				return Swatch;
			}
		}
		
		/// <summary>
		/// Creates a new color swatch and adds it to the dictionary
		/// </summary>
		/// <param name="swatchData">Data to create the color swatch</param>
		public static string CreateAndAddColorSwatch(Swatch swatchData)
		{
			Bitmap Swatch = CreateColorSwatch(ref swatchData);
			if (Swatch != null)
			{
				swatchData.Key = swatchData.MakeKey();
				ColorSwatches.Add(swatchData.Key, Swatch);
			}
			Swatch = null;
			return swatchData.Key;
		}

		/// <summary>
		/// Creates a new color swatch and adds it to the dictionary
		/// </summary>
		/// <param name="color">Color to make the swatch</param>
		public static string CreateAndAddColorSwatch(Color color)
		{
			Swatch SwatchData = new Swatch(color);
			Bitmap Swatch = CreateColorSwatch(ref SwatchData);
			if (Swatch != null)
			{
				SwatchData.Key = SwatchData.MakeKey();
				ColorSwatches.Add(SwatchData.Key, Swatch);
			}
			Swatch = null;
			return SwatchData.Key;
		}

		/// <summary>
		/// Creates an image that contains the text specified.
		/// </summary>
		/// <param name="message">Message to print on bitmap.</param>
		/// <param name="background">Color to use for the background.</param>
		/// <param name="foreground">Color to use for the foreground.</param>
		/// <returns></returns>
		public static Bitmap CreateErrorMessageImage(string message, Color background, Color foreground)
		{
			Bitmap Output = null;
			message += "  ";
			int Width, Height;

			using (Font Font = new Font("Arial", 12f))
			{
				using (Bitmap Temp = new Bitmap(16, 16))
				using (Graphics g = Graphics.FromImage(Temp))
				{
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
					StringFormat Format = new StringFormat();
					Format.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, message.Length) });
					Region[] r = g.MeasureCharacterRanges(message, Font, new Rectangle(0, 0, 1000, 1000), Format);
					RectangleF rect = r[0].GetBounds(g);
					Width = (int)rect.Width;
					Height = (int)rect.Height;
					Format = null;
				}
				Output = new Bitmap(Width + 32, Height + 16);
				using (Graphics g = Graphics.FromImage(Output))
				{
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
					g.Clear(background);
					using (Pen Pen = new Pen(foreground))
						g.DrawRectangle(Pen, new Rectangle(0, 0, Output.Width - 1, Output.Height - 1));
					using (SolidBrush Brush = new SolidBrush(foreground))
						g.DrawString(message, Font, Brush, new Rectangle(16, 8, Width + 20, Height), new StringFormat());
				}
			}
			return Output;
		}

		/// <summary>
		/// Creates a fuzzy drop shadow on the graphics device, defined by the GraphicsPath passed in.
		/// http://www.codeproject.com/Articles/15847/Fuzzy-DropShadows-in-GDI
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="path">Path for the fuzzy drop shadow to follow.</param>
		/// <param name="shadowColor">Color to make the drop shadow. In the example on the website, Color.DimGray was used.</param>
		/// <param name="transparency">Transparency of the shadow. In the example on the website, a value of 180 was used.</param>
		public static void FuzzyDropShadow(Graphics g, GraphicsPath path, Color shadowColor, byte transparency)
		{
			using (PathGradientBrush Brush = new PathGradientBrush(path))
			{
				// Set the wrapmode so that the colors will layer themselves from the outer edge in
				Brush.WrapMode = WrapMode.Clamp;

				// Create a color blend to manage our colors and positions and
				// since we need 3 colors set the default length to 3
				ColorBlend ColorBlend = new ColorBlend(3);

				// Here is the important part of the shadow making process, remember the clamp mode on the colorblend object layers the colors from
				// the outside to the center so we want our transparent color first followed by the actual shadow color. Set the shadow color to a 
				// slightly transparent DimGray, I find that it works best.
				ColorBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(transparency, shadowColor), Color.FromArgb(transparency, shadowColor) };

				// Our color blend will control the distance of each color layer we want to set our transparent color to 0 indicating that the 
				// transparent color should be the outer most color drawn, then our Dimgray color at about 10% of the distance from the edgen
				//ColorBlend.Positions = new float[] { 0f, .1f, 1f };
				ColorBlend.Positions = new float[] { 0f, .4f, 1f };

				// Assign the color blend to the pathgradientbrush
				Brush.InterpolationColors = ColorBlend;

				// Fill the shadow with our pathgradientbrush
				g.FillPath(Brush, path);
			}
		}

		/// <summary>
		/// Gets the image format from the image.
		/// </summary>
		public static ImageFormat GetImageFormat(Image img)
		{
			if (img.RawFormat.Equals(ImageFormat.Jpeg))
				return ImageFormat.Jpeg;
			if (img.RawFormat.Equals(ImageFormat.Bmp))
				return ImageFormat.Bmp;
			if (img.RawFormat.Equals(ImageFormat.Png))
				return ImageFormat.Png;
			if (img.RawFormat.Equals(ImageFormat.Emf))
				return ImageFormat.Emf;
			if (img.RawFormat.Equals(ImageFormat.Exif))
				return ImageFormat.Exif;
			if (img.RawFormat.Equals(ImageFormat.Gif))
				return ImageFormat.Gif;
			if (img.RawFormat.Equals(ImageFormat.Icon))
				return ImageFormat.Icon;
			if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
				return ImageFormat.MemoryBmp;
			if (img.RawFormat.Equals(ImageFormat.Tiff))
				return ImageFormat.Tiff;
			else
				return ImageFormat.Wmf;
		}

		/// <summary>
		/// Based on the raw format of the bitmap object, return the extension that best fits.
		/// </summary>
		/// <param name="bitmap">Bitmap object to be examined.</param>
		/// <returns>Returns extention based on raw format.</returns>
		public static string GetFileExtension(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap cannot be null.");

			ImageFormat Format = GetImageFormat(bitmap);

			if (Format.Equals(ImageFormat.Jpeg))
				return "jpg";
			else if (Format.Equals(ImageFormat.Bmp))
				return "bmp";
			else if (Format.Equals(ImageFormat.Emf))
				return "emf";
			else if (Format.Equals(ImageFormat.Exif))
				return "exf";
			else if (Format.Equals(ImageFormat.Gif))
				return "gif";
			else if (Format.Equals(ImageFormat.MemoryBmp))
				return "mbmp";
			else if (Format.Equals(ImageFormat.Png))
				return "png";
			else if (Format.Equals(ImageFormat.Tiff))
				return "tif";
			else if (Format.Equals(ImageFormat.Wmf))
				return "wmf";
			else
				return string.Empty;
		}

		/// <summary>
		/// Returns the image from the Resources that correspond to the enum.
		/// </summary>
		private static Bitmap GetAnnotationImage(Annotation annotation)
		{
			switch (annotation)
			{
				case Annotation.Add:
					return ElfRes.annotation_add;
				case Annotation.Alert:
					return ElfRes.annotation_alert;
				case Annotation.As:
					return ElfRes.annotation_as;
				case Annotation.Blocked:
					return ElfRes.annotation_blocked;
				case Annotation.Check:
					return ElfRes.annotation_check;
				case Annotation.Clear:
					return ElfRes.annotation_clear;
				case Annotation.ClearStar:
					return ElfRes.annotation_clear2;
				case Annotation.Close:
					return ElfRes.annotation_close;
				case Annotation.Complete:
					return ElfRes.annotation_complete;
				case Annotation.Create:
					return ElfRes.annotation_create;
				case Annotation.Delete:
					return ElfRes.annotation_delete;
				case Annotation.Down:
					return ElfRes.annotation_down;
				case Annotation.Edit:
					return ElfRes.annotation_edit;
				case Annotation.Error:
					return ElfRes.annotation_error;
				case Annotation.Exclude:
					return ElfRes.annotation_exclude;
				case Annotation.Export:
					return ElfRes.annotation_export;
				case Annotation.Find:
					return ElfRes.annotation_find;
				case Annotation.Flyout:
					return ElfRes.annotation_flyout;
				case Annotation.Grid:
					return ElfRes.annotation_grid;
				case Annotation.Help:
					return ElfRes.annotation_help;
				case Annotation.Image:
					return ElfRes.annotation_image;
				case Annotation.Import:
					return ElfRes.annotation_import;
				case Annotation.Include:
					return ElfRes.annotation_include;
				case Annotation.Info:
					return ElfRes.annotation_info;
				case Annotation.Invisible:
					return ElfRes.annotation_invisible;
				case Annotation.Left:
					return ElfRes.annotation_left;
				case Annotation.Locked:
					return ElfRes.annotation_locked;
				case Annotation.Monitor:
					return ElfRes.annotation_monitor;
				case Annotation.New:
					return ElfRes.annotation_new;
				case Annotation.Not:
					return ElfRes.annotation_not;
				case Annotation.OneToOne:
					return ElfRes.annotation_1to1;
				case Annotation.Open:
					return ElfRes.annotation_open;
				case Annotation.Paint:
					return ElfRes.annotation_paint;
				case Annotation.Pause:
					return ElfRes.annotation_pause;
				case Annotation.Play:
					return ElfRes.annotation_play;
				case Annotation.PlayRound:
					return ElfRes.annotation_playround;
				case Annotation.PlusOne:
					return ElfRes.annotation_plusone;
				case Annotation.Redo:
					return ElfRes.annotation_redo;
				case Annotation.Refresh:
					return ElfRes.annotation_refresh;
				case Annotation.Remove:
					return ElfRes.annotation_remove;
				case Annotation.Required:
					return ElfRes.annotation_required;
				case Annotation.Right:
					return ElfRes.annotation_right;
				case Annotation.Save:
					return ElfRes.annotation_save;
				case Annotation.SecurityAlert:
					return ElfRes.annotation_security_alert;
				case Annotation.SecurityBad:
					return ElfRes.annotation_security_bad;
				case Annotation.SecurityGood:
					return ElfRes.annotation_security_good;
				case Annotation.Shortcut:
					return ElfRes.annotation_shortcut;
				case Annotation.SortAscending:
					return ElfRes.annotation_sort_ascending;
				case Annotation.SortDescending:
					return ElfRes.annotation_sort_descending;
				case Annotation.Star:
					return ElfRes.annotation_star;
				case Annotation.Stop:
					return ElfRes.annotation_stop;
				case Annotation.Trash:
					return ElfRes.annotation_trash;
				case Annotation.Undo:
					return ElfRes.annotation_undo;
				case Annotation.Up:
					return ElfRes.annotation_up;
				case Annotation.Vector:
					return ElfRes.annotation_vector;
				case Annotation.Visible:
					return ElfRes.annotation_visible;
				case Annotation.Warning:
					return ElfRes.annotation_warning;
				default:
					return null;
			}
		}

		/// <summary>
		/// Loads a bitmap from file.
		/// </summary>
		/// <param name="filename">Name of the file to load.</param>
		/// <returns>Bitmap object</returns>
		public static Bitmap LoadBitmapFromFile(string filename)
		{
			Bitmap Loaded = null;

			try
			{
				Loaded = new Bitmap(filename);
				return CopyImage(Loaded);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				//Controllers.Workshop.Instance.WriteTraceMessage(ex.ToString(), TraceLevel.Error);
				return null;
			}
			finally
			{
				if (Loaded != null)
				{
					Loaded.Dispose();
					Loaded = null;
				}
			}
		}

		/// <summary>
		/// Loads a bitmap from an encoded string.
		/// </summary>
		/// <param name="encodedStream">Encoded byte string</param>
		/// <returns>Bitmap object</returns>
		public static Bitmap LoadBitmapFromEncoded(string encodedStream)
		{
			byte[] imageByteArray = Convert.FromBase64String(encodedStream);
			if (imageByteArray.Length == 0)
				return null;

			return new Bitmap(new MemoryStream(imageByteArray));
		}

		/// <summary>
		/// http://stackoverflow.com/questions/9356694/tint-property-when-drawing-image-with-vb-net
		/// Tints a bitmap using the specified color and intensity.
		/// </summary>
		/// <param name="b">Bitmap to be tinted</param>
		/// <param name="color">Color to use for tint</param>
		/// <returns>A bitmap with the requested Tint</returns>
		public static Bitmap TintBitmap(Bitmap b, Color color)
		{
			Bitmap b2 = new Bitmap(b.Width, b.Height);
			ImageAttributes ia = new ImageAttributes();

			ColorMatrix cMatrix = new ColorMatrix(new float[][] {
							new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
							new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
							new float[] { (float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, 0.0f, 1.0f }
						});

			ia.SetColorMatrix(cMatrix);
			using (Graphics g = Graphics.FromImage(b2))
				g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, ia);

			cMatrix = null;

			return b2;
		}

		#endregion [ Static Methods ]

		#region [ Dead Code ]

		///// <summary>
		///// http://www.codeproject.com/Articles/33838/Image-Processing-using-C
		///// </summary>
		//public static Bitmap SetGrayscale(Bitmap bitmap)
		//{
		//    Bitmap temp = (Bitmap)bitmap;
		//    Bitmap bmap = (Bitmap)temp.Clone();
		//    Color c;
		//    for (int i = 0; i < bmap.Width; i++)
		//    {
		//        for (int j = 0; j < bmap.Height; j++)
		//        {
		//            c = bmap.GetPixel(i, j);
		//            byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

		//            bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
		//        }
		//    }
		//    return (Bitmap)bmap.Clone();
		//}

		///// <summary>
		///// http://www.codeproject.com/Articles/33838/Image-Processing-using-C
		///// </summary>
		//public static Bitmap SetInvert(Bitmap bitmap)
		//{
		//    Bitmap temp = (Bitmap)bitmap;
		//    Bitmap bmap = (Bitmap)temp.Clone();
		//    Color c;
		//    for (int i = 0; i < bmap.Width; i++)
		//    {
		//        for (int j = 0; j < bmap.Height; j++)
		//        {
		//            c = bmap.GetPixel(i, j);
		//            bmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
		//        }
		//    }
		//    return (Bitmap)bmap.Clone();
		//}

		///// <summary>
		///// http://www.codeproject.com/Articles/33838/Image-Processing-using-C
		///// </summary>
		///// <param name="contrast"></param>
		///// <param name="bitmap"></param>
		//public static Bitmap SetContrast(double contrast, Bitmap bitmap)
		//{
		//    Bitmap temp = (Bitmap)bitmap;
		//    Bitmap bmap = (Bitmap)temp.Clone();
		//    if (contrast < -100)
		//        contrast = -100;
		//    if (contrast > 100)
		//        contrast = 100;
		//    contrast = (100.0 + contrast) / 100.0;
		//    contrast *= contrast;
		//    Color c;
		//    for (int i = 0; i < bmap.Width; i++)
		//    {
		//        for (int j = 0; j < bmap.Height; j++)
		//        {
		//            c = bmap.GetPixel(i, j);
		//            double pR = c.R / 255.0;
		//            pR -= 0.5;
		//            pR *= contrast;
		//            pR += 0.5;
		//            pR *= 255;
		//            if (pR < 0)
		//                pR = 0;
		//            if (pR > 255)
		//                pR = 255;

		//            double pG = c.G / 255.0;
		//            pG -= 0.5;
		//            pG *= contrast;
		//            pG += 0.5;
		//            pG *= 255;
		//            if (pG < 0)
		//                pG = 0;
		//            if (pG > 255)
		//                pG = 255;

		//            double pB = c.B / 255.0;
		//            pB -= 0.5;
		//            pB *= contrast;
		//            pB += 0.5;
		//            pB *= 255;
		//            if (pB < 0)
		//                pB = 0;
		//            if (pB > 255)
		//                pB = 255;

		//            bmap.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
		//        }
		//    }
		//    return (Bitmap)bmap.Clone();
		//}

		///// <summary>
		///// http://www.smokycogs.com/blog/image-processing-in-c-sharp-adjusting-the-gamma/
		///// </summary>
		///// <param name="r"></param>
		///// <param name="g"></param>
		///// <param name="b"></param>
		//public static void ApplyGamma(Bitmap bitmapImage, double r, double g, double b)
		//{
		//    byte A, R, G, B;
		//    Color pixelColor;

		//    byte[] redGamma = new byte[256];
		//    byte[] greenGamma = new byte[256];
		//    byte[] blueGamma = new byte[256];

		//    for (int i = 0; i < 256; ++i)
		//    {
		//        redGamma[i] = (byte)Math.Min(255, (int)((255.0
		//            * Math.Pow(i / 255.0, 1.0 / r)) + 0.5));
		//        greenGamma[i] = (byte)Math.Min(255, (int)((255.0
		//            * Math.Pow(i / 255.0, 1.0 / g)) + 0.5));
		//        blueGamma[i] = (byte)Math.Min(255, (int)((255.0
		//             * Math.Pow(i / 255.0, 1.0 / b)) + 0.5));
		//    }

		//    for (int y = 0; y < bitmapImage.Height; y++)
		//    {
		//        for (int x = 0; x < bitmapImage.Width; x++)
		//        {
		//            pixelColor = bitmapImage.GetPixel(x, y);
		//            A = pixelColor.A;
		//            R = redGamma[pixelColor.R];
		//            G = greenGamma[pixelColor.G];
		//            B = blueGamma[pixelColor.B];
		//            bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
		//        }
		//    }
		//}

		///// <summary>
		///// Creates the ColorMatrix to change brightness.
		///// </summary>
		///// <param name="brightness">Brightness percent. Valid range is -1.0 to +1.0</param>
		//public static ColorMatrix GetBrightnessMatrix(float brightness)
		//{
		//    return new ColorMatrix(new float[][] {
		//                    new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
		//                    new float[] { brightness, brightness, brightness, 0.0f, 1.0f }
		//                });
		//}

		///// <summary>
		///// http://www.codeproject.com/Tips/78995/Image-colour-manipulation-with-ColorMatrix
		///// </summary>
		///// <param name="saturation"></param>
		//public static ColorMatrix GetSaturationMatrix(float saturation)
		//{
		//    float rWeight = 0.3086f;
		//    float gWeight = 0.6094f;
		//    float bWeight = 0.0820f;

		//    float a = (1.0f - saturation) * rWeight + saturation;
		//    float b = (1.0f - saturation) * rWeight;
		//    float c = (1.0f - saturation) * rWeight;
		//    float d = (1.0f - saturation) * gWeight;
		//    float e = (1.0f - saturation) * gWeight + saturation;
		//    float f = (1.0f - saturation) * gWeight;
		//    float g = (1.0f - saturation) * bWeight;
		//    float h = (1.0f - saturation) * bWeight;
		//    float i = (1.0f - saturation) * bWeight + saturation;

		//    // ColorMatrix elements
		//    float[][] ptsArray = {
		//                                new float[] {a,  b,  c,  0, 0},
		//                                new float[] {d,  e,  f,  0, 0},
		//                                new float[] {g,  h,  i,  0, 0},
		//                                new float[] {0,  0,  0,  1, 0},
		//                                new float[] {0, 0, 0, 0, 1}
		//                            };
		//    // Create ColorMatrix
		//    return new ColorMatrix(ptsArray);
		//}

		///// <summary>
		///// Converts a ColorMatrix object into an array.
		///// </summary>
		//public static float[][] ColorMatrixToArray(ColorMatrix matrix)
		//{
		//    float[][] ptsArray = { 
		//                            new float[] { matrix.Matrix00, matrix.Matrix01, matrix.Matrix02, matrix.Matrix03, matrix.Matrix04 },
		//                            new float[] { matrix.Matrix10, matrix.Matrix11, matrix.Matrix12, matrix.Matrix13, matrix.Matrix14 },
		//                            new float[] { matrix.Matrix20, matrix.Matrix21, matrix.Matrix22, matrix.Matrix23, matrix.Matrix24 },
		//                            new float[] { matrix.Matrix30, matrix.Matrix31, matrix.Matrix32, matrix.Matrix33, matrix.Matrix34 }, 
		//                            new float[] { matrix.Matrix40, matrix.Matrix41, matrix.Matrix42, matrix.Matrix43, matrix.Matrix44 }
		//                         };
		//    return ptsArray;
		//}

		///// <summary>
		///// Multiply 2 ColorMatrices
		///// http://www.codeproject.com/Articles/7836/Multiple-Matrices-With-ColorMatrix-in-C
		///// </summary>
		//public static ColorMatrix MultiplyColorMatrices(ColorMatrix matrix1, ColorMatrix matrix2)
		//{
		//    float[][] f1 = ColorMatrixToArray(matrix1);
		//    float[][] f2 = ColorMatrixToArray(matrix2);

		//    float[][] X = new float[5][];
		//    for (int d = 0; d < 5; d++)
		//        X[d] = new float[5];
		//    int size = 5;
		//    float[] column = new float[5];
		//    for (int j = 0; j < 5; j++)
		//    {
		//        for (int k = 0; k < 5; k++)
		//        {
		//            column[k] = f1[k][j];
		//        }
		//        for (int i = 0; i < 5; i++)
		//        {
		//            float[] row = f2[i];
		//            float s = 0;
		//            for (int k = 0; k < size; k++)
		//            {
		//                s += row[k] * column[k];
		//            }
		//            X[i][j] = s;
		//        }
		//    }
		//    return new ColorMatrix(X);
		//}

		///// <summary>
		///// Creates a blank (identity) ColorMatrix. 
		///// </summary>
		//public static ColorMatrix GetIdentityColorMatrix()
		//{
		//    return new ColorMatrix(new float[][] {
		//                    new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
		//                    new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
		//                    new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
		//                });
		//}

		#endregion [ Dead Code ]

	}

	public struct Swatch
	{
		public Color Color;
		public Color BorderColor;
		public string Key;
		public bool IsLocked;
		public bool IsIncluded;
		public bool IsEnabled;
		public bool IsVisible;

		public Swatch(Color color)
		{
			Color = color;
			BorderColor = Color.Empty;
			Key = string.Empty;
			IsLocked = false;
			IsIncluded = true;
			IsEnabled = true;
			IsVisible = true;
		}

		public Swatch(Color color, bool enabled)
			: this(color)
		{
			IsEnabled = enabled;
		}

		public Swatch(Color color, Color borderColor, bool locked, bool included, bool enabled, bool visible)
			: this(color, enabled)
		{
			BorderColor = borderColor;
			IsLocked = locked;
			IsIncluded = included;
			IsVisible = visible;
		}

		public string MakeKey()
		{
			string SwatchKey = XmlColor.FromBaseType(Color);
			if (!BorderColor.IsEmpty)
				SwatchKey += ", BORDER: " + XmlColor.FromBaseType(BorderColor);
			SwatchKey += ",LOCKED: " + IsLocked;
			SwatchKey += ",INCLUDED: " + IsIncluded;
			SwatchKey += ",ENABLED: " + IsEnabled;
			SwatchKey += ",VISIBLE: " + IsVisible;
			return SwatchKey;
		}
	}
}

