using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal class PanelScroll : Panel
{
	#region [ Constants ]

	private const int SB_HORZ = 0;
	private const int SB_VERT = 1;
	private const int WM_VSCROLL = 0x0115;
	private const int WM_HSCROLL = 0x0114;
	private const int WM_MOUSEWHEEL = 0x020A;

	#endregion [ Constants ]

	#region [ Declares ]

	[DllImport("user32.dll")]
	private static extern int GetScrollPos(int hWnd, int nBar);

	[DllImport("user32.dll")]
	static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

	#endregion [ Declares ]

	#region [ Properties ]

	/// <summary>
	/// Retrieves the horizontal scrolling position
	/// </summary>
	public int HScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_VERT); }
	}

	/// <summary>
	/// Retrieves the vertical scrolling position
	/// </summary>
	public int VScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_HORZ); }
	}

	#endregion [ Properties ]

	#region [ Events ]

	public new event ScrollEventHandler Scroll;

	#endregion [ Events ]

	#region [ Methods ]

	public void SetScrollPos(Point scrollPosition)
	{
		SetScrollPos((IntPtr)this.Handle, SB_HORZ, scrollPosition.X, true);
		SetScrollPos((IntPtr)this.Handle, SB_VERT, scrollPosition.Y, true);
	}

	public Point GetScrollPos()
	{
		return new Point(
			GetScrollPos((int)this.Handle, SB_HORZ),
			GetScrollPos((int)this.Handle, SB_VERT));
	}

	[DebuggerHidden()]
	protected override void WndProc(ref Message m)
	{
		if (Scroll != null)
		{
			switch (m.Msg)
			{
				case WM_VSCROLL:
				case WM_HSCROLL:
				case WM_MOUSEWHEEL:
					{
						ScrollEventType t = (ScrollEventType)Enum.Parse(typeof(ScrollEventType), (m.WParam.ToInt32() & 65535).ToString());
						Scroll(m.HWnd, new ScrollEventArgs(t, ((int)(m.WParam.ToInt64() >> 16)) & 255));
					}
					break;
			}
		}
		base.WndProc(ref m);
	}

	#endregion [ Methods ]

}