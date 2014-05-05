using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

#region [ Class TreeViewScroll ]

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

	public TreeNode FindByPath(string fullPath)
	{
		return FindByPath(this, fullPath);
	}

	#endregion [ Methods ]
}

#endregion [ Class TreeViewScroll ]

#region [ Class FlickerFreeListBox ]

/// <summary>
/// Replaces the standard ListBox control class, to prevent annoying flickering on redraw
/// </summary>
internal class FlickerFreeListBox : System.Windows.Forms.ListBox
{
	#region [ Constructors ]

	public FlickerFreeListBox()
	{
		this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
		this.DrawMode = DrawMode.OwnerDrawFixed;
	}

	#endregion [ Constructors ]

	#region [ Events ]

	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		base.OnDrawItem(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Region iRegion = new Region(e.ClipRectangle);
		e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
		//DrawItemState State;

		if (this.Items.Count > 0)
		{
			for (int i = 0; i < this.Items.Count; ++i)
			{
				System.Drawing.Rectangle irect = this.GetItemRectangle(i);

				if (e.ClipRectangle.IntersectsWith(irect))
				{
					OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, irect, i, DrawItemState.Default, this.ForeColor, this.BackColor));
					iRegion.Complement(irect);
				}
			}
		}
		base.OnPaint(e);
	}

	#endregion [ Events ]
}

#endregion [ Class FlickerFreeListBox ]	

#region [ Class MultiSelectTreeview ]

/// <summary>
/// http://www.codeproject.com/Articles/20581/Multiselect-Treeview-Implementation
/// </summary>
internal class MultiSelectTreeview : TreeView
{
	#region [ Private Variables ]

	private List<TreeNode> m_SelectedNodes = null;

	// Note we use the new keyword to Hide the native treeview's SelectedNode property.
	private TreeNode m_SelectedNode;

	#endregion [ Private Variables ]

	#region [ Properties ]

	public List<TreeNode> SelectedNodes
	{
		get
		{
			return m_SelectedNodes;
		}
		set
		{
			ClearSelectedNodes();
			if (value != null)
			{
				foreach (TreeNode node in value)
				{
					ToggleNode(node, true);
				}
			}
		}
	}

	public new TreeNode SelectedNode
	{
		get { return m_SelectedNode; }
		set
		{
			ClearSelectedNodes();
			if (value != null)
			{
				SelectNode(value);
			}
		}
	}

	#endregion [ Properties ]

	#region [ Constructors ]

	public MultiSelectTreeview()
	{
		m_SelectedNodes = new List<TreeNode>();
		base.SelectedNode = null;
	}

	#endregion [ Constructors ]

	#region [ Overridden Events ]

	protected override void OnGotFocus(EventArgs e)
	{
		// Make sure at least one node has a selection
		// this way we can tab to the ctrl and use the 
		// keyboard to select nodes
		try
		{
			if (m_SelectedNode == null && this.TopNode != null)
			{
				ToggleNode(this.TopNode, true);
			}

			base.OnGotFocus(e);
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		// If the user clicks on a node that was not
		// previously selected, select it now.

		if (e.Button == System.Windows.Forms.MouseButtons.Right)
			return;

		try
		{
			base.SelectedNode = null;

			TreeNode node = this.GetNodeAt(e.Location);
			if (node != null)
			{
				int leftBound = node.Bounds.X; // - 20; // Allow user to click on image
				int rightBound = node.Bounds.Right + 10; // Give a little extra room
				if (e.Location.X > leftBound && e.Location.X < rightBound)
				{
					if (ModifierKeys == Keys.None && (m_SelectedNodes.Contains(node)))
					{
						// Potential Drag Operation
						// Let Mouse Up do select
					}
					else
					{
						SelectNode(node);
					}
				}
			}

			base.OnMouseDown(e);
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		// If the clicked on a node that WAS previously
		// selected then, reselect it now. This will clear
		// any other selected nodes. e.g. A B C D are selected
		// the user clicks on B, now A C & D are no longer selected.

		if (e.Button == System.Windows.Forms.MouseButtons.Right)
			return;

		try
		{
			// Check to see if a node was clicked on 
			TreeNode node = this.GetNodeAt(e.Location);
			if (node != null)
			{
				if (ModifierKeys == Keys.None && m_SelectedNodes.Contains(node))
				{
					int leftBound = node.Bounds.X; // -20; // Allow user to click on image
					int rightBound = node.Bounds.Right + 10; // Give a little extra room
					if (e.Location.X > leftBound && e.Location.X < rightBound)
					{
						SelectNode(node);
					}
				}
			}

			base.OnMouseUp(e);
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnItemDrag(ItemDragEventArgs e)
	{
		// If the user drags a node and the node being dragged is NOT
		// selected, then clear the active selection, select the
		// node being dragged and drag it. Otherwise if the node being
		// dragged is selected, drag the entire selection.
		try
		{
			TreeNode node = e.Item as TreeNode;

			if (node != null)
			{
				if (!m_SelectedNodes.Contains(node))
				{
					SelectSingleNode(node);
					ToggleNode(node, true);
				}
			}

			base.OnItemDrag(e);
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
	{
		// Never allow base.SelectedNode to be set!
		try
		{
			base.SelectedNode = null;
			e.Cancel = true;

			base.OnBeforeSelect(e);
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnAfterSelect(TreeViewEventArgs e)
	{
		// Never allow base.SelectedNode to be set!
		try
		{
			base.OnAfterSelect(e);
			base.SelectedNode = null;
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		// Handle all possible key strokes for the control.
		// including navigation, selection, etc.

		base.OnKeyDown(e);

		if (e.KeyCode == Keys.ShiftKey)
			return;

		//this.BeginUpdate();
		bool bShift = (ModifierKeys == Keys.Shift);

		try
		{
			// Nothing is selected in the tree, this isn't a good state
			// select the top node
			if (m_SelectedNode == null && this.TopNode != null)
			{
				ToggleNode(this.TopNode, true);
			}

			// Nothing is still selected in the tree, this isn't a good state, leave.
			if (m_SelectedNode == null)
				return;

			if (e.KeyCode == Keys.Left)
			{
				if (m_SelectedNode.IsExpanded && m_SelectedNode.Nodes.Count > 0)
				{
					// Collapse an expanded node that has children
					m_SelectedNode.Collapse();
				}
				else if (m_SelectedNode.Parent != null)
				{
					// Node is already collapsed, try to select its parent.
					SelectSingleNode(m_SelectedNode.Parent);
				}
			}
			else if (e.KeyCode == Keys.Right)
			{
				if (!m_SelectedNode.IsExpanded)
				{
					// Expand a collpased node's children
					m_SelectedNode.Expand();
				}
				else
				{
					// Node was already expanded, select the first child
					SelectSingleNode(m_SelectedNode.FirstNode);
				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				// Select the previous node
				if (m_SelectedNode.PrevVisibleNode != null)
				{
					SelectNode(m_SelectedNode.PrevVisibleNode);
				}
			}
			else if (e.KeyCode == Keys.Down)
			{
				// Select the next node
				if (m_SelectedNode.NextVisibleNode != null)
				{
					SelectNode(m_SelectedNode.NextVisibleNode);
				}
			}
			else if (e.KeyCode == Keys.Home)
			{
				if (bShift)
				{
					if (m_SelectedNode.Parent == null)
					{
						// Select all of the root nodes up to this point 
						if (this.Nodes.Count > 0)
						{
							SelectNode(this.Nodes[0]);
						}
					}
					else
					{
						// Select all of the nodes up to this point under this nodes parent
						SelectNode(m_SelectedNode.Parent.FirstNode);
					}
				}
				else
				{
					// Select this first node in the tree
					if (this.Nodes.Count > 0)
					{
						SelectSingleNode(this.Nodes[0]);
					}
				}
			}
			else if (e.KeyCode == Keys.End)
			{
				if (bShift)
				{
					if (m_SelectedNode.Parent == null)
					{
						// Select the last ROOT node in the tree
						if (this.Nodes.Count > 0)
						{
							SelectNode(this.Nodes[this.Nodes.Count - 1]);
						}
					}
					else
					{
						// Select the last node in this branch
						SelectNode(m_SelectedNode.Parent.LastNode);
					}
				}
				else
				{
					if (this.Nodes.Count > 0)
					{
						// Select the last node visible node in the tree.
						// Don't expand branches incase the tree is virtual
						TreeNode ndLast = this.Nodes[0].LastNode;
						while (ndLast.IsExpanded && (ndLast.LastNode != null))
						{
							ndLast = ndLast.LastNode;
						}
						SelectSingleNode(ndLast);
					}
				}
			}
			else if (e.KeyCode == Keys.PageUp)
			{
				// Select the highest node in the display
				int nCount = this.VisibleCount;
				TreeNode ndCurrent = m_SelectedNode;
				while ((nCount) > 0 && (ndCurrent.PrevVisibleNode != null))
				{
					ndCurrent = ndCurrent.PrevVisibleNode;
					nCount--;
				}
				SelectSingleNode(ndCurrent);
			}
			else if (e.KeyCode == Keys.PageDown)
			{
				// Select the lowest node in the display
				int nCount = this.VisibleCount;
				TreeNode ndCurrent = m_SelectedNode;
				while ((nCount) > 0 && (ndCurrent.NextVisibleNode != null))
				{
					ndCurrent = ndCurrent.NextVisibleNode;
					nCount--;
				}
				SelectSingleNode(ndCurrent);
			}
			else
			{
				// Assume this is a search character a-z, A-Z, 0-9, etc.
				// Select the first node after the current node that 
				// starts with this character
				string sSearch = ((char)e.KeyValue).ToString();

				TreeNode ndCurrent = m_SelectedNode;
				while ((ndCurrent.NextVisibleNode != null))
				{
					ndCurrent = ndCurrent.NextVisibleNode;
					if (ndCurrent.Text.StartsWith(sSearch))
					{
						SelectSingleNode(ndCurrent);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			HandleException(ex);
		}
		finally
		{
			this.EndUpdate();
		}
	}

	#endregion [ Overridden Events ]

	#region [ Helper Methods ]

	private void SelectNode(TreeNode node)
	{
		try
		{
			this.BeginUpdate();

			if (m_SelectedNode == null || ModifierKeys == Keys.Control)
			{
				// Ctrl+Click selects an unselected node, or unselects a selected node.
				bool bIsSelected = m_SelectedNodes.Contains(node);
				ToggleNode(node, !bIsSelected);
			}
			else if (ModifierKeys == Keys.Shift)
			{
				// Shift+Click selects nodes between the selected node and here.
				TreeNode ndStart = m_SelectedNode;
				TreeNode ndEnd = node;

				if (ndStart.Parent == ndEnd.Parent)
				{
					// Selected node and clicked node have same parent, easy case.
					if (ndStart.Index < ndEnd.Index)
					{
						// If the selected node is beneath the clicked node walk down
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = ndStart.NextVisibleNode;
							if (ndStart == null)
								break;
							ToggleNode(ndStart, true);
						}
					}
					else if (ndStart.Index == ndEnd.Index)
					{
						// Clicked same node, do nothing
					}
					else
					{
						// If the selected node is above the clicked node walk up
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = ndStart.PrevVisibleNode;
							if (ndStart == null)
								break;
							ToggleNode(ndStart, true);
						}
					}
				}
				else
				{
					// Selected node and clicked node have same parent, hard case.
					// We need to find a common parent to determine if we need
					// to walk down selecting, or walk up selecting.

					TreeNode ndStartP = ndStart;
					TreeNode ndEndP = ndEnd;
					int startDepth = Math.Min(ndStartP.Level, ndEndP.Level);

					// Bring lower node up to common depth
					while (ndStartP.Level > startDepth)
					{
						ndStartP = ndStartP.Parent;
					}

					// Bring lower node up to common depth
					while (ndEndP.Level > startDepth)
					{
						ndEndP = ndEndP.Parent;
					}

					// Walk up the tree until we find the common parent
					while (ndStartP.Parent != ndEndP.Parent)
					{
						ndStartP = ndStartP.Parent;
						ndEndP = ndEndP.Parent;
					}

					// Select the node
					if (ndStartP.Index < ndEndP.Index)
					{
						// If the selected node is beneath the clicked node walk down
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = ndStart.NextVisibleNode;
							if (ndStart == null)
								break;
							ToggleNode(ndStart, true);
						}
					}
					else if (ndStartP.Index == ndEndP.Index)
					{
						if (ndStart.Level < ndEnd.Level)
						{
							while (ndStart != ndEnd)
							{
								ndStart = ndStart.NextVisibleNode;
								if (ndStart == null)
									break;
								ToggleNode(ndStart, true);
							}
						}
						else
						{
							while (ndStart != ndEnd)
							{
								ndStart = ndStart.PrevVisibleNode;
								if (ndStart == null)
									break;
								ToggleNode(ndStart, true);
							}
						}
					}
					else
					{
						// If the selected node is above the clicked node walk up
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = ndStart.PrevVisibleNode;
							if (ndStart == null)
								break;
							ToggleNode(ndStart, true);
						}
					}
				}
			}
			else
			{
				// Just clicked a node, select it
				SelectSingleNode(node);
			}
			OnAfterSelect(new TreeViewEventArgs(m_SelectedNode));
		}
		finally
		{
			this.EndUpdate();
		}
	}

	/// <summary>
	/// Clears out all the selected Nodes
	/// </summary>
	private void ClearSelectedNodes()
	{
		try
		{
			foreach (TreeNode node in m_SelectedNodes)
			{
				node.BackColor = this.BackColor;
				node.ForeColor = this.ForeColor;
			}
		}
		finally
		{
			m_SelectedNodes.Clear();
			m_SelectedNode = null;
		}
	}

	/// <summary>
	/// Select an individual node
	/// </summary>
	/// <param name="node">TreeNode to select</param>
	private void SelectSingleNode(TreeNode node)
	{
		if (node == null)
			return;

		ClearSelectedNodes();
		ToggleNode(node, true);
		node.EnsureVisible();
	}

	private void ToggleNode(TreeNode node, bool bSelectNode)
	{
		if (bSelectNode)
		{
			m_SelectedNode = node;
			if (!m_SelectedNodes.Contains(node))
			{
				m_SelectedNodes.Add(node);
			}
			node.BackColor = SystemColors.Highlight;
			node.ForeColor = SystemColors.HighlightText;
		}
		else
		{
			m_SelectedNodes.Remove(node);
			node.BackColor = this.BackColor;
			node.ForeColor = this.ForeColor;
		}
	}

	private void HandleException(Exception ex)
	{
		// Perform some error handling here.
		// We don't want to bubble errors to the CLR. 
		//MessageBox.Show(ex.Message);
		Debug.WriteLine(ex.ToString());
	}

	#endregion [ Helper Methods ]

	#region [ Methods ]

	/// <summary>
	/// Traverses the list of TreeNodes in the collection and adds them to the list. If a given node has child nodes, then
	/// this method is called recursively, gathering them all up as well.
	/// </summary>
	/// <param name="nodeCollection">Collection of TreeNodes to read</param>
	/// <returns>All the nodes in a given TreeNodeCollection object.</returns>
	public List<TreeNode> GetAllNodes(TreeNodeCollection nodeCollection)
	{
		List<TreeNode> Nodes = new List<TreeNode>();

		foreach (TreeNode Node in nodeCollection)
		{
			Nodes.Add(Node);
			if (Node.Nodes.Count > 0)
				Nodes.AddRange(GetAllNodes(Node.Nodes));
		}
		return Nodes;
	}

	#endregion [ Methods ]

}

#endregion [ Class MultiSelectTreeview ]

#region [ Class PanelScroll ]

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

#endregion [ Class PanelScroll ]

/// <summary>
/// Suspends drawing of a control and it's child controls
/// http://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children
/// </summary>
public class DrawingControl
{
	[DllImport("user32.dll")]
	public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

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