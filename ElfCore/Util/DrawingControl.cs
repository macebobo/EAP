using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ElfCore.Util
{
	/// <summary>
	/// Suspends drawing of a control and it's child controls
	/// http://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children
	/// </summary>
	public class DrawingControl
	{
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

		private const int WM_SETREDRAW = 11;

		public static void SuspendDrawing(Control parent)
		{
			SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
		}

		public static void ResumeDrawing(Control parent)
		{
			SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
			parent.Refresh();
			//parent.PerformLayout();
		}
	}
}