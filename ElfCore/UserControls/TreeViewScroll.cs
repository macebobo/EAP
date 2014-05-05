using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

internal class TreeViewScroll : TreeView
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
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public int HScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_HORZ); }
		set { SetScrollPos(Handle, SB_HORZ, value, true); }
	}

	/// <summary>
	/// Retrieves the vertical scrolling position
	/// </summary>
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public int VScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_VERT); }
		set { SetScrollPos(Handle, SB_VERT, value, true); }
	}

	#endregion [ Properties ]

	#region [ Events ]

	public event ScrollEventHandler Scroll;

	#endregion [ Events ]

	#region [ Methods ]

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

	/// <summary>
	/// Searches the tree control for a particular node based on the path provided
	/// </summary>
	/// <param name="treeControl">Tree Control to search</param>
	/// <param name="fullPath">The path to search</param>
	/// <returns></returns>
	public static TreeNode FindByPath(TreeView treeControl, string fullPath)
	{
		if (String.IsNullOrEmpty(fullPath))
			return null;

		//TreeNode Match = treeControl.Nodes[0];

		string[] NameList = fullPath.Split('\\');
		string Name = string.Empty;

		// have to check all the top level nodes first
		TreeNode Match = null;
		Name = NameList[0];

		foreach (TreeNode Node in treeControl.Nodes)
		{
			if (Node.Text == Name)
			{
				Match = Node;
				break;
			}
		}

		if (Match == null)
			return null;

		for (int i = 1; i < NameList.Length; i++)
		{
			if (Match == null)
				return null;
			Name = NameList[i];

			foreach (TreeNode Node in Match.Nodes)
			{
				if (Node.Text == Name)
				{
					Match = Node;
					break;
				}
			}
		}

		if (Match.Text == NameList[NameList.Length - 1])
			return Match;
		else
			return null;
	}

	private Point GetTreeViewScrollPos(TreeView treeView)
	{
		return new Point(
			GetScrollPos((int)treeView.Handle, SB_HORZ),
			GetScrollPos((int)treeView.Handle, SB_VERT));
	}

	private void SetTreeViewScrollPos(TreeView treeView, Point scrollPosition)
	{
		SetScrollPos((IntPtr)treeView.Handle, SB_HORZ, scrollPosition.X, true);
		SetScrollPos((IntPtr)treeView.Handle, SB_VERT, scrollPosition.Y, true);
	}

	public TreeNode FindByPath(string fullPath)
	{
		return FindByPath(this, fullPath);
	}

	#endregion [ Methods ]
}