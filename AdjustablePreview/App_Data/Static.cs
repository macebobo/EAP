using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ElfCore
{
	public static class Extends
	{
		/// <summary>
		/// Clone a generic list.
		/// http://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
		/// </summary>
		/// <typeparam name="T">Type stored in the list</typeparam>
		/// <param name="listToClone">The original list to be cloned</param>
		/// <returns>A cloned copy of the list.</returns>
		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

	}

	#region [ Moved to Workshop ]
	
	///// <summary>
	///// This class holds several generic static methods
	///// </summary>
	//public static class Static
	//{
	//    #region [ Declares ]

	//    [DllImport("msvcrt.dll")]
	//    private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

	//    #endregion [ Declares ]

	//    /// <summary>
	//    /// Compare two arrays for equality
	//    /// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
	//    /// </summary>
	//    /// <typeparam name="T">Type of the data stored in the array</typeparam>
	//    /// <param name="a1">First array to compare</param>
	//    /// <param name="a2">Second array to compare</param>
	//    /// <returns>Returns true if both arrays exactly match each other. Also returns true if both parameters point to the exact same data</returns>
	//    public static bool ArraysEqual<T>(T[] a1, T[] a2)
	//    {
	//        if (ReferenceEquals(a1, a2))
	//            return true;

	//        if (a1 == null || a2 == null)
	//            return false;

	//        if (a1.Length != a2.Length)
	//            return false;

	//        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
	//        for (int i = 0; i < a1.Length; i++)
	//        {
	//            if (!comparer.Equals(a1[i], a2[i]))
	//                return false;
	//        }
	//        return true;
	//    }

	//    /// <summary>
	//    /// Compares two bitmaps on a per-pixel basis
	//    /// http://social.msdn.microsoft.com/Forums/en-US/vbgeneral/thread/bd0eec9e-f811-4fab-a245-08b2882d005c
	//    /// </summary>
	//    /// <param name="b1">First Bitmap to compare</param>
	//    /// <param name="b2">Second Bitmap to compare</param>
	//    /// <returns>Returns true if both are null, both share the same pointer, or if both match exactly at the pixel level</returns>
	//    public static bool BitmapEquals(Bitmap b1, Bitmap b2)
	//    {
	//        // Verify that both are not null
	//        if ((b1 == null) && (b2 == null))
	//            return true;

	//        // If one or the other are null, then we don't match
	//        if ((b1 == null) || (b2 == null))
	//            return false;

	//        // Verify that they are not using the same pointer
	//        if (object.ReferenceEquals(b1, b2))
	//            return true;

	//        // Verify the dimensions are the same
	//        if (b1.Size != b2.Size)
	//            return false;

	//        // Verify that the pixel formats are the same
	//        if (b1.PixelFormat != b2.PixelFormat)
	//            return false;

	//        LockBitmap lb1 = new LockBitmap(b1);
	//        lb1.LockBits();
	//        LockBitmap lb2 = new LockBitmap(b2);
	//        lb2.LockBits();

	//        bool Result = Static.ArraysEqual<byte>(lb1.Pixels, lb2.Pixels);
				
	//        lb1.UnlockBits();
	//        lb1 = null;
	//        lb2.UnlockBits();
	//        lb2 = null;

	//        return Result;
	//    }

	//    /// <summary>
	//    /// Gets the path to the user's Window's Profile directory
	//    /// </summary>
	//    public static string GetProfilePath()
	//    {
	//        string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen");
	//        DirectoryInfo DInfo = new DirectoryInfo(SavePath);
	//        if (!DInfo.Exists)
	//        {
	//            // Create the folder
	//            DInfo.Create();
	//        }
	//        return SavePath;
	//    }

	//    /// <summary>
	//    /// Compare Lists for equality
	//    /// http://stackoverflow.com/questions/713341/comparing-arrays-in-c-sharp
	//    /// </summary>
	//    /// <typeparam name="T">Type of the data stored in the array</typeparam>
	//    /// <param name="a1">First List to compare</param>
	//    /// <param name="a2">Second List to compare</param>
	//    /// <returns>Returns true if both Lists exactly match each other. Also returns true if both parameters point to the exact same data</returns>
	//    public static bool ListEqual<T>(List<T> a1, List<T> a2)
	//    {
	//        if (ReferenceEquals(a1, a2))
	//            return true;

	//        if (a1 == null || a2 == null)
	//            return false;

	//        if (a1.Count != a2.Count)
	//            return false;

	//        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
	//        for (int i = 0; i < a1.Count; i++)
	//        {
	//            if (!comparer.Equals(a1[i], a2[i]))
	//                return false;
	//        }
	//        return true;
	//    }

	//    /// <summary>
	//    /// Clone a generic list.
	//    /// http://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
	//    /// </summary>
	//    /// <typeparam name="T">Type stored in the list</typeparam>
	//    /// <param name="listToClone">The original list to be cloned</param>
	//    /// <returns>A cloned copy of the list.</returns>
	//    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	//    {
	//        return listToClone.Select(item => (T)item.Clone()).ToList();
	//    }

	//    /// <summary>
	//    /// Brings up the file save dialog to save the bitmap file to disk
	//    /// </summary>
	//    /// <param name="bmp">Bitmap object to save</param>
	//    /// <param name="defaultFilename">Default file name for the file</param>
	//    /// <returns>Returns true if the image saved successfully</returns>
	//    public static bool SaveBitmap(Bitmap bmp, string defaultFilename)
	//    {
	//        SaveFileDialog SaveImageFileDialog = new SaveFileDialog();
	//        SaveImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
	//        SaveImageFileDialog.Title = "Save Image";
	//        SaveImageFileDialog.FileName = defaultFilename;
	//        string Ext = string.Empty;

	//        if (defaultFilename.Length > 0)
	//            Ext = defaultFilename.Substring(defaultFilename.Length - 3).ToLower();
	//        else
	//            Ext = "png";

	//        string[] Filters = SaveImageFileDialog.Filter.Split('|');

	//        for (int i = 0; i < Filters.Length; i++)
	//        {
	//            // there will be a matched pair for each of these elements, look at the even numbered element
	//            // Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*
	//            if (i % 2 == 0)
	//                i++;
	//            if (i >= Filters.Length)
	//                break;

	//            if (Filters[i].Replace("*.", "") == Ext)
	//            {
	//                SaveImageFileDialog.FilterIndex = (i / 2) + 1;
	//                break;
	//            }
	//        }

	//        if (SaveImageFileDialog.ShowDialog() != DialogResult.OK)
	//            return false;

	//        defaultFilename = SaveImageFileDialog.FileName;
	//        ImageFormat Format;

	//        switch (SaveImageFileDialog.FilterIndex)
	//        {
	//            case 1: // Bitmap
	//                Format = ImageFormat.Bmp;
	//                defaultFilename = Path.ChangeExtension(defaultFilename, ".bmp");
	//                break;
	//            case 3: // PNG
	//                Format = ImageFormat.Png;
	//                defaultFilename = Path.ChangeExtension(defaultFilename, ".png");
	//                break;
	//            case 4: // GIF
	//                Format = ImageFormat.Gif;
	//                defaultFilename = Path.ChangeExtension(defaultFilename, ".gif");
	//                break;
	//            default:
	//                Format = ImageFormat.Jpeg;
	//                defaultFilename = Path.ChangeExtension(defaultFilename, ".jpg");
	//                break;
	//        }

	//        try
	//        {
	//            Bitmap b = new Bitmap(bmp);
	//            b.Save(defaultFilename, Format);
	//        }
	//        catch
	//        {
	//            MessageBox.Show("Unable to save this file, possibly due to where it is being saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	//            return false;
	//        }

	//        MessageBox.Show("File saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
	//        return true;
	//    }

	//    //public static void ShowWithParentFormLock(this Form childForm, Form parentForm)
	//    //{
	//    //    childForm.ShowWithParentFormLock(parentForm, null);
	//    //}

	//    //public static void ShowWithParentFormLock(Form childForm, Form parentForm)//, Action actionAfterClose)
	//    //{
	//    //    if (childForm == null)
	//    //        throw new ArgumentNullException("childForm");
	//    //    if (parentForm == null)
	//    //        throw new ArgumentNullException("parentForm");
	//    //    EventHandler activatedDelegate = (object sender, EventArgs e) =>
	//    //    {
	//    //        childForm.Focus();
	//    //        //To Do: Add ability to flash form to notify user that focus changed 
	//    //    };
	//    //    childForm.FormClosed += (sender, closedEventArgs) =>
	//    //    {
	//    //        try
	//    //        {
	//    //            parentForm.Focus();
	//    //            //if (actionAfterClose != null)
	//    //            //    actionAfterClose();
	//    //        }
	//    //        finally
	//    //        {
	//    //            try
	//    //            {
	//    //                parentForm.Activated -= activatedDelegate;
	//    //                if (!childForm.IsDisposed || !childForm.Disposing)
	//    //                    childForm.Dispose();
	//    //            }
	//    //            catch { }
	//    //        }
	//    //    };
	//    //    parentForm.Activated += activatedDelegate;
	//    //    childForm.Show(parentForm);
	//    //} 
	//}

	#endregion [ Moved to Workshop ]

}
