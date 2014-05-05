using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore
{
	public class ImageController
	{
		/// <summary>
		/// swatch [swoch]
		/// noun
		/// 1. a sample of cloth or other material.
		/// 2. a sample, patch, or characteristic specimen of anything.
		/// </summary>
		public static Dictionary<string, Bitmap> ColorSwatches = new Dictionary<string, Bitmap>();

		public static Dictionary<string, Bitmap> RawImages = new Dictionary<string, Bitmap>();

		public static Dictionary<string, Bitmap> TintedImages = new Dictionary<string, Bitmap>();

		#region [ Static Methods ]

		/// <summary>
		/// Create a 16x16 image of a 12x12 square of the color of the channel passed in, with a black border
		/// </summary>
		/// <param name="channel">Channel holding the color desired. If null, then creates a swatch of SystemColors.Control color</param>
		/// <returns>Returns the generated bitmap</returns>
		public static Bitmap CreateColorSwatch(Channel channel)
		{
			return CreateColorSwatch((channel == null) ? SystemColors.Control : channel.Color);
		}

		/// <summary>
		/// Create a 16x16 image of a 12x12 square of the color of the channel passed in, with a black border
		/// </summary>
		/// <param name="color">Color to use to create the swatch</param>
		/// <returns>Returns the generated bitmap</returns>
		public static Bitmap CreateColorSwatch(Color color)
		{
			// If this color is already present, then jump out.
			if (ImageController.ColorSwatches.ContainsKey(color.ToArgb().ToString()))
				return null;

			AppearanceController Appearance = AppearanceController.Instance;

			Bitmap Swatch = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
			using (Graphics g = Graphics.FromImage(Swatch))
			{
				Rectangle R = new Rectangle(2, 2, 12, 12);

				R.Offset(new Point(1, 1));
				// Create a drop shadow using a black pen with 50% alpha
				using (Pen ShadowPen = new Pen(Color.FromArgb(128, Color.Black)))
					g.DrawRectangle(ShadowPen, R);

				R.Offset(new Point(-1, -1));

				//using (SolidBrush Brush = new SolidBrush(color))
				//    g.FillRectangle(Brush, R);

				HSLColor DarkerColor = new HSLColor(color);
				DarkerColor.Luminosity /= 2f;
				if (DarkerColor.Luminosity > 128)
					DarkerColor.Luminosity /= 2f;

				using (LinearGradientBrush br = new LinearGradientBrush(R, Color.Blue, Color.White, 45f))
				{
					// Create a ColorBlend object. Note that you
					// must initialize it before you save it in the
					// brush's InterpolationColors property.
					ColorBlend colorBlend = new ColorBlend();
					colorBlend.Colors = new Color[] { color, color, DarkerColor };
					colorBlend.Positions = new float[] { 0.0f, 0.6f, 1f };
					br.InterpolationColors = colorBlend;
					g.FillRectangle(br, R);
				}

				//using (LinearGradientBrush Brush = new LinearGradientBrush(R, color, Color.Black, 45))
				//    g.FillPolygon(Brush, Points);
				//    //g.FillRectangle(Brush, R);

				using (Pen Pen = new Pen(Appearance.CurrentSkin.BaseColor))
					g.DrawRectangle(Pen, R);

				// Tag the image with the integer value of the color
				Swatch.Tag = color.ToArgb().ToString();
				return Swatch;


			}
		}

		/// <summary>
		/// Creates a new color swatch and adds it to the dictionary
		/// </summary>
		/// <param name="color">Color of the swatch to made</param>
		public static void CreateAndAddColorSwatch(Color color)
		{
			Bitmap Swatch = CreateColorSwatch(color);
 			if (Swatch != null)
				ColorSwatches.Add(color.ToArgb().ToString(), Swatch);
			Swatch = null;
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
			Graphics g = Graphics.FromImage(b2);
			g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, ia);

			cMatrix = null;
			g.Dispose();
			g = null;

			return b2;
		}

		#endregion [ Static Methods ]

		#region [ DEAD CODE ]

		///// <summary>
		///// Create the requested image and composite it based on the Verb passed in.
		///// </summary>
		///// <param name="objectType">ImageType enum indicating the object</param>
		///// <param name="verb">Verb enum to indicate what to composite with the image (if any)</param>
		///// <returns>Composited Bitmap</returns>
		//public static Bitmap GetBitmap(ImageType objectType, Verb verb)
		//{
		//    Bitmap Source = null;
		//    Bitmap VerbImage = null;

		//    switch (verb)
		//    {
		//        case Verb.Add:
		//            VerbImage = WFRes.verb_add;
		//            break;
		//        case Verb.Check:
		//            VerbImage = WFRes.check_green;
		//            break;
		//        case Verb.Delete:
		//            VerbImage = WFRes.verb_delete;
		//            break;
		//        case Verb.Edit:
		//            VerbImage = WFRes.verb_edit;
		//            break;
		//        case Verb.From:
		//            VerbImage = WFRes.verb_from;
		//            break;
		//        case Verb.New:
		//            VerbImage = WFRes.verb_new;
		//            break;
		//        case Verb.Not:
		//            VerbImage = WFRes.verb_not;
		//            break;
		//        case Verb.To:
		//            VerbImage = WFRes.verb_to;
		//            break;
		//        default:
		//            VerbImage = null;
		//            break;
		//    }

		//    Source = GetBitmap(objectType);

		//    if (VerbImage != null)
		//    {
		//        Graphics g = Graphics.FromImage(Source);
		//        g.DrawImage(VerbImage, new Rectangle(0, 0, 16, 16));
		//        g.Dispose();
		//    }
		//    return Source;
		//}

		///// <summary>
		///// Finds the bitmap in the program resources that corresponds to the enumeration passed in.
		///// These bitmaps are used to populate the tree and menu system with images.
		///// </summary>
		///// <param name="objectType">Enum indicating the bitmap desired</param>
		///// <returns>If the bitmap is defined for the enum, returns it, else returns null</returns>
		//public static Bitmap GetBitmap(ImageType objectType)
		//{
		//    switch (objectType)
		//    {
		//        case ImageType.DiscoverMenu:
		//            return WFRes.DiscoverMenu;

		//        case ImageType.DiscoverMenu_Open:
		//            return WFRes.DiscoverMenu_open;

		//        case ImageType.DiscoverMenu_Page:
		//            return WFRes.DiscoverMenu_Page;

		//        case ImageType.Filter:
		//            return WFRes.filter1;

		//        case ImageType.ApplicationObject:
		//            return WFRes.application_object;

		//        case ImageType.ApplicationSubObject:
		//            return WFRes.application_object_sub;

		//        case ImageType.Project:
		//            return WFRes.wrench_orange;

		//        case ImageType.SSRS:
		//            return WFRes.ReportingServices;

		//        case ImageType.SSRS_Open:
		//            return WFRes.SSRS_open;

		//        case ImageType.SSRS_Page:
		//            return WFRes.SSRS_Page;

		//        case ImageType.WebReport:
		//            return WFRes.WebReport;

		//        case ImageType.WebReport_Open:
		//            return WFRes.WebReport_open;

		//        case ImageType.WebReport_Page:
		//            return WFRes.WebReport_Page;

		//        case ImageType.FilterType_Type:
		//            return WFRes.type;

		//        case ImageType.FilterType_Filter:
		//            return WFRes.filter1;

		//        case ImageType.FilterType_Option:
		//            return WFRes.equalizer;

		//        case ImageType.FilterType_GroupBy:
		//            return WFRes.group;

		//        case ImageType.FilterType_OrderBy:
		//            return WFRes.order;

		//        case ImageType.FilterType_Note:
		//            return WFRes.notebook;

		//        case ImageType.ControlType_Checkbox:
		//            return WFRes.Boolean;

		//        case ImageType.ControlType_Date:
		//            return WFRes.Date;

		//        case ImageType.ControlType_DateTime:
		//            return WFRes.DateTime;

		//        case ImageType.ControlType_DateRange:
		//            return WFRes.DateRange;

		//        case ImageType.ControlType_Domain:
		//            return WFRes.Domain;

		//        case ImageType.ControlType_DropDown:
		//            return WFRes.Combo;

		//        case ImageType.ControlType_Include:
		//            return WFRes.Include;

		//        case ImageType.ControlType_IntegerRange:
		//            return WFRes.IntegerRange;

		//        case ImageType.ControlType_ListBox:
		//            return WFRes.ListBox;

		//        case ImageType.ControlType_ListRange:
		//            return WFRes.ListRange;

		//        case ImageType.ControlType_MonthYear:
		//            return WFRes.MonthYear;

		//        case ImageType.ControlType_Note:
		//            return WFRes.notebook;

		//        case ImageType.ControlType_PIT:
		//            return WFRes.PIT;

		//        case ImageType.ControlType_Radio:
		//            return WFRes.Radio;

		//        case ImageType.ControlType_TextBox:
		//            return WFRes.String;

		//        case ImageType.ControlType_TwoValues:
		//            return WFRes.twovalues;

		//        case ImageType.Database:
		//            return WFRes.database;

		//        case ImageType.Option:
		//            return WFRes.option;

		//        case ImageType.Template:
		//            return WFRes.template;

		//        case ImageType.ClosedFolder:
		//            return WFRes.closed_folder;

		//        case ImageType.OpenFolder:
		//            return WFRes.open_folder;

		//        case ImageType.Version:
		//            return WFRes.version;

		//        case ImageType.Description:
		//            return WFRes.desc;

		//        case ImageType.ReportName:
		//            return WFRes.name;

		//        case ImageType.ReportPath:
		//            return WFRes.terminal;

		//        case ImageType.RegularExpression:
		//            return WFRes.instructions;

		//        case ImageType.DataStore:
		//            return WFRes.DataStore;

		//        case ImageType.Javascript:
		//            return WFRes.javascript;

		//        case ImageType.FieldValue:
		//            return WFRes.socket;

		//        case ImageType.Num:
		//            return WFRes.number;

		//        case ImageType.Num2:
		//            return WFRes.number2;

		//        case ImageType.Add:
		//            return WFRes.new_big;

		//        case ImageType.True:
		//            return WFRes.radiobutton_on;

		//        case ImageType.False:
		//            return WFRes.radiobutton_off;

		//        case ImageType.DataType:
		//            return WFRes.information;

		//        case ImageType.Information:
		//            return WFRes.information;

		//        case ImageType.Html:
		//            return WFRes.html;

		//        case ImageType.Name:
		//            return WFRes.name2;

		//        default:
		//            return WFRes.question;
		//    }
		//}

		///// <summary>
		///// Gets the bitmap that corresponds to the source object
		///// </summary>
		///// <param name="sourceObject"></param>
		///// <returns></returns>
		//public static Bitmap GetBitmap(Base sourceObject)
		//{
		//    return GetBitmap(sourceObject.ImageType);
		//}

		///// <summary>
		///// Determines which Icon is currently selected and returns the enum equivalent to the closed image.
		///// If it is not in the list, returns the original value.
		///// </summary>
		///// <param name="imageIndex"></param>
		///// <returns></returns>
		//public static ImageType GetClosedFolder(int imageIndex)
		//{
		//    ImageType OpenIcon = EnumHelper.GetEnumFromValue<ImageType>(imageIndex);

		//    switch (OpenIcon)
		//    {
		//        case ImageType.DiscoverMenu_Open:
		//            return ImageType.DiscoverMenu;

		//        case ImageType.SSRS_Open:
		//            return ImageType.SSRS;

		//        case ImageType.WebReport_Open:
		//            return ImageType.WebReport;

		//        case ImageType.ClosedFolder:
		//        case ImageType.OpenFolder:
		//            return ImageType.ClosedFolder;

		//        default:
		//            return OpenIcon;
		//    }
		//}

		///// <summary>
		///// Determines which Icon is currently selected and returns the enum equivalent to the open image.
		///// If it is not in the list, returns the original value.
		///// </summary>
		///// <param name="imageIndex"></param>
		///// <returns></returns>
		//public static ImageType GetOpenFolder(int imageIndex)
		//{
		//    ImageType ClosedIcon = EnumHelper.GetEnumFromValue<ImageType>(imageIndex);

		//    switch (ClosedIcon)
		//    {
		//        case ImageType.DiscoverMenu:
		//            return ImageType.DiscoverMenu_Open;

		//        case ImageType.SSRS:
		//            return ImageType.SSRS_Open;

		//        case ImageType.WebReport:
		//            return ImageType.WebReport_Open;

		//        case ImageType.ClosedFolder:
		//        case ImageType.OpenFolder:
		//            return ImageType.OpenFolder;

		//        default:
		//            return ClosedIcon;
		//    }
		//}

		#endregion [ DEAD CODE ]

	}
}

