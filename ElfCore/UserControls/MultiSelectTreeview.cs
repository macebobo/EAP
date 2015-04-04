using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using ElfCore.Channels;

/// <summary>
/// http://www.codeproject.com/Articles/20581/Multiselect-Treeview-Implementation
/// </summary>
public class MultiSelectTreeview : TreeView
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

	#region [ Private Variables ]

	private List<CXTreeNode> _selectedNodes = null;

	// Note we use the new keyword to Hide the native treeview's SelectedNode property.
	private CXTreeNode _selectedNode;

	#endregion [ Private Variables ]

	#region [ Properties ]

	/// <summary>
	/// Retrieves the horizontal scrolling position
	/// </summary>
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int HScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_HORZ); }
		set { SetScrollPos(Handle, SB_HORZ, value, true); }
	}

	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<CXTreeNode> SelectedNodes
	{
		get
		{
			return _selectedNodes;
		}
		set
		{
			ClearSelectedNodes();
			if (value != null)
			{
				foreach (CXTreeNode node in value)
				{
					ToggleNode(node, true);
				}
			}
		}
	}

	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new CXTreeNode SelectedNode
	{
		get { return _selectedNode; }
		set
		{
			ClearSelectedNodes();
			if (value != null)
			{
				SelectNode(value);
			}
		}
	}

	/// <summary>
	/// Retrieves the vertical scrolling position
	/// </summary>
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int VScrollPos
	{
		get { return GetScrollPos((int)Handle, SB_VERT); }
		set { SetScrollPos(Handle, SB_VERT, value, true); }
	}

	#endregion [ Properties ]

	#region [ Constructors ]

	public MultiSelectTreeview()
	{
		_selectedNodes = new List<CXTreeNode>();
		base.SelectedNode = null;
	}

	#endregion [ Constructors ]

	#region [ Events ]

	#region [ Event Handlers ]

	public event ScrollEventHandler Scroll;

	#endregion [ Event Handlers ]

	#region [ Event Triggers ]

	protected override void OnGotFocus(EventArgs e)
	{
		// Make sure at least one node has a selection
		// this way we can tab to the ctrl and use the 
		// keyboard to select nodes
		try
		{
			if (_selectedNode == null && TopNode != null)
			{
				ToggleNode((CXTreeNode)TopNode, true);
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

		if (e.Button == MouseButtons.Right)
			return;

		try
		{
			base.SelectedNode = null;

			CXTreeNode node = (CXTreeNode)GetNodeAt(e.Location);
			if (node != null)
			{
				int leftBound = node.Bounds.X; // - 20; // Allow user to click on image
				int rightBound = node.Bounds.Right + 10; // Give a little extra room
				if (e.Location.X > leftBound && e.Location.X < rightBound)
				{
					if (ModifierKeys == Keys.None && (_selectedNodes.Contains(node)))
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

		if (e.Button == MouseButtons.Right)
			return;

		try
		{
			// Check to see if a node was clicked on 
			CXTreeNode node = (CXTreeNode)GetNodeAt(e.Location);
			if (node != null)
			{
				if (ModifierKeys == Keys.None && _selectedNodes.Contains(node))
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
			CXTreeNode node = e.Item as CXTreeNode;

			if (node != null)
			{
				if (!_selectedNodes.Contains(node))
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
			if (_selectedNode == null && TopNode != null)
			{
				ToggleNode((CXTreeNode)TopNode, true);
			}

			// Nothing is still selected in the tree, this isn't a good state, leave.
			if (_selectedNode == null)
				return;

			if (e.KeyCode == Keys.Left)
			{
				if (_selectedNode.IsExpanded && _selectedNode.Nodes.Count > 0)
				{
					// Collapse an expanded node that has children
					_selectedNode.Collapse();
				}
				else if (_selectedNode.Parent != null)
				{
					// Node is already collapsed, try to select its parent.
					SelectSingleNode((CXTreeNode)_selectedNode.Parent);
				}
			}
			else if (e.KeyCode == Keys.Right)
			{
				if (!_selectedNode.IsExpanded)
				{
					// Expand a collpased node's children
					_selectedNode.Expand();
				}
				else
				{
					// Node was already expanded, select the first child
					SelectSingleNode((CXTreeNode)_selectedNode.FirstNode);
				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				// Select the previous node
				if (_selectedNode.PrevVisibleNode != null)
				{
					SelectNode((CXTreeNode)_selectedNode.PrevVisibleNode);
				}
			}
			else if (e.KeyCode == Keys.Down)
			{
				// Select the next node
				if (_selectedNode.NextVisibleNode != null)
				{
					SelectNode((CXTreeNode)_selectedNode.NextVisibleNode);
				}
			}
			else if (e.KeyCode == Keys.Home)
			{
				if (bShift)
				{
					if (_selectedNode.Parent == null)
					{
						// Select all of the root nodes up to this point 
						if (Nodes.Count > 0)
						{
							SelectNode((CXTreeNode)Nodes[0]);
						}
					}
					else
					{
						// Select all of the nodes up to this point under this nodes parent
						SelectNode((CXTreeNode)_selectedNode.Parent.FirstNode);
					}
				}
				else
				{
					// Select this first node in the tree
					if (Nodes.Count > 0)
					{
						SelectSingleNode((CXTreeNode)Nodes[0]);
					}
				}
			}
			else if (e.KeyCode == Keys.End)
			{
				if (bShift)
				{
					if (_selectedNode.Parent == null)
					{
						// Select the last ROOT node in the tree
						if (Nodes.Count > 0)
						{
							SelectNode((CXTreeNode)Nodes[Nodes.Count - 1]);
						}
					}
					else
					{
						// Select the last node in this branch
						SelectNode((CXTreeNode)_selectedNode.Parent.LastNode);
					}
				}
				else
				{
					if (Nodes.Count > 0)
					{
						// Select the last node visible node in the tree.
						// Don't expand branches incase the tree is virtual
						CXTreeNode ndLast = (CXTreeNode)Nodes[0].LastNode;
						while (ndLast.IsExpanded && (ndLast.LastNode != null))
						{
							ndLast = (CXTreeNode)ndLast.LastNode;
						}
						SelectSingleNode(ndLast);
					}
				}
			}
			else if (e.KeyCode == Keys.PageUp)
			{
				// Select the highest node in the display
				int nCount = VisibleCount;
				CXTreeNode ndCurrent = _selectedNode;
				while ((nCount) > 0 && (ndCurrent.PrevVisibleNode != null))
				{
					ndCurrent = (CXTreeNode)ndCurrent.PrevVisibleNode;
					nCount--;
				}
				SelectSingleNode(ndCurrent);
			}
			else if (e.KeyCode == Keys.PageDown)
			{
				// Select the lowest node in the display
				int nCount = VisibleCount;
				CXTreeNode ndCurrent = _selectedNode;
				while ((nCount) > 0 && (ndCurrent.NextVisibleNode != null))
				{
					ndCurrent = (CXTreeNode)ndCurrent.NextVisibleNode;
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

				CXTreeNode ndCurrent = _selectedNode;
				while ((ndCurrent.NextVisibleNode != null))
				{
					ndCurrent = (CXTreeNode)ndCurrent.NextVisibleNode;
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
			EndUpdate();
		}
	}

	#endregion [ Event Triggers ]

	#endregion [ Events ]

	#region [ Helper Methods ]

	/// <summary>
	/// Clears out all the selected Nodes
	/// </summary>
	public void ClearSelectedNodes()
	{
		try
		{
			foreach (CXTreeNode node in _selectedNodes)
			{
				node.BackColor = BackColor;
				node.ForeColor = ForeColor;
			}
		}
		finally
		{
			_selectedNodes.Clear();
			_selectedNode = null;
		}
	}

	private void HandleException(Exception ex)
	{
		// Perform some error handling here.
		// We don't want to bubble errors to the CLR. 
		//MessageBox.Show(ex.Message);
		Debug.WriteLine(ex.ToString());
	}

	private void SelectNode(CXTreeNode node)
	{
		try
		{
			BeginUpdate();

			if (_selectedNode == null || ModifierKeys == Keys.Control)
			{
				// Ctrl+Click selects an unselected node, or unselects a selected node.
				bool bIsSelected = _selectedNodes.Contains(node);
				ToggleNode(node, !bIsSelected);
			}
			else if (ModifierKeys == Keys.Shift)
			{
				// Shift+Click selects nodes between the selected node and here.
				CXTreeNode ndStart = _selectedNode;
				CXTreeNode ndEnd = node;

				if (ndStart.Parent == ndEnd.Parent)
				{
					// Selected node and clicked node have same parent, easy case.
					if (ndStart.Index < ndEnd.Index)
					{
						// If the selected node is beneath the clicked node walk down
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = (CXTreeNode)ndStart.NextVisibleNode;
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
							ndStart = (CXTreeNode)ndStart.PrevVisibleNode;
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

					CXTreeNode ndStartP = ndStart;
					CXTreeNode ndEndP = ndEnd;
					int startDepth = Math.Min(ndStartP.Level, ndEndP.Level);

					// Bring lower node up to common depth
					while (ndStartP.Level > startDepth)
					{
						ndStartP = (CXTreeNode)ndStartP.Parent;
					}

					// Bring lower node up to common depth
					while (ndEndP.Level > startDepth)
					{
						ndEndP = (CXTreeNode)ndEndP.Parent;
					}

					// Walk up the tree until we find the common parent
					while (ndStartP.Parent != ndEndP.Parent)
					{
						ndStartP = (CXTreeNode)ndStartP.Parent;
						ndEndP = (CXTreeNode)ndEndP.Parent;
					}

					// Select the node
					if (ndStartP.Index < ndEndP.Index)
					{
						// If the selected node is beneath the clicked node walk down
						// selecting each Visible node until we reach the end.
						while (ndStart != ndEnd)
						{
							ndStart = (CXTreeNode)ndStart.NextVisibleNode;
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
								ndStart = (CXTreeNode)ndStart.NextVisibleNode;
								if (ndStart == null)
									break;
								ToggleNode(ndStart, true);
							}
						}
						else
						{
							while (ndStart != ndEnd)
							{
								ndStart = (CXTreeNode)ndStart.PrevVisibleNode;
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
							ndStart = (CXTreeNode)ndStart.PrevVisibleNode;
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
			OnAfterSelect(new TreeViewEventArgs(_selectedNode));
		}
		finally
		{
			EndUpdate();
		}
	}

	/// <summary>
	/// Select an individual node
	/// </summary>
	/// <param name="node">CXTreeNode to select</param>
	public void SelectSingleNode(CXTreeNode node)
	{
		if (node == null)
			return;

		ClearSelectedNodes();
		ToggleNode(node, true);
		node.EnsureVisible();
	}

	private void ToggleNode(CXTreeNode node, bool selectNode)
	{
		if (selectNode)
		{
			_selectedNode = node;
			if (!_selectedNodes.Contains(node))
			{
				_selectedNodes.Add(node);
			}
			if (node.Enabled & node.Included)
			{
				node.BackColor = SystemColors.Highlight;
				node.ForeColor = SystemColors.HighlightText;
			}
			else
			{
				node.BackColor = SystemColors.GradientInactiveCaption;
				node.ForeColor = SystemColors.GrayText;
			}
		}
		else
		{
			_selectedNodes.Remove(node);
			node.BackColor = BackColor;
			if (node.Enabled & node.Included)
				node.ForeColor = ForeColor;				
			else
				node.ForeColor = SystemColors.GrayText;
		}

	}

	#endregion [ Helper Methods ]

	#region [ Methods ]

	public TreeNode FindByPath(string fullPath)
	{
		return FindByPath(this, fullPath);
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

	/// <summary>
	/// Traverses the list of CXTreeNodes in the collection and adds them to the list. If a given node has child nodes, then
	/// this method is called recursively, gathering them all up as well.
	/// </summary>
	/// <param name="nodeCollection">Collection of CXTreeNodes to read</param>
	/// <returns>All the nodes in a given CXTreeNodeCollection object.</returns>
	public List<CXTreeNode> GetAllNodes(TreeNodeCollection nodeCollection)
	{
		List<CXTreeNode> Nodes = new List<CXTreeNode>();

		foreach (CXTreeNode Node in nodeCollection)
		{
			Nodes.Add(Node);
			if (Node.Nodes.Count > 0)
				Nodes.AddRange(GetAllNodes(Node.Nodes));
		}
		return Nodes;
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

	[DebuggerHidden]
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

/// <summary>
/// TreeNode class customized for the Channel Explorer Tree
/// </summary>
public class CXTreeNode : TreeNode
{
	#region [ Private Variables ]

	private bool _enabled = true;
	private bool _included = true;
	private Channel _channel = null;

	#endregion [ Private Variables ]

	#region [ Properties ]

	public bool Enabled
	{
		get { return _enabled; }
		set { _enabled = value; }
	}

	public bool Included
	{
		get { return _included; }
		set { _included = value; }
	}

	public Channel Channel
	{
		get { return _channel; }
		set { _channel = value; }
	}

	#endregion [ Properties ]

	#region [ Constructors ]

	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class.
	public CXTreeNode() : base() { }

	//
	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class with
	//     the specified label text.
	//
	// Parameters:
	//   text:
	//     The label System.Windows.Forms.TreeNode.Text of the new tree node.

	public CXTreeNode(string text)
		: base(text)
	{ }

	//
	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class using
	//     the specified serialization information and context.
	//
	// Parameters:
	//   serializationInfo:
	//     A System.Runtime.Serialization.SerializationInfo containing the data to deserialize
	//     the class.
	//
	//   context:
	//     The System.Runtime.Serialization.StreamingContext containing the source and
	//     destination of the serialized stream.
	protected CXTreeNode(SerializationInfo serializationInfo, StreamingContext context)
		: base(serializationInfo, context)
	{ }

	//
	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class with
	//     the specified label text and child tree nodes.
	//
	// Parameters:
	//   text:
	//     The label System.Windows.Forms.TreeNode.Text of the new tree node.
	//
	//   children:
	//     An array of child System.Windows.Forms.TreeNode objects.
	public CXTreeNode(string text, TreeNode[] children)
		: base(text, children)
	{ }

	//
	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class with
	//     the specified label text and images to display when the tree node is in a
	//     selected and unselected state.
	//
	// Parameters:
	//   text:
	//     The label System.Windows.Forms.TreeNode.Text of the new tree node.
	//
	//   imageIndex:
	//     The index value of System.Drawing.Image to display when the tree node is
	//     unselected.
	//
	//   selectedImageIndex:
	//     The index value of System.Drawing.Image to display when the tree node is
	//     selected.
	public CXTreeNode(string text, int imageIndex, int selectedImageIndex)
		: base(text, imageIndex, selectedImageIndex)
	{ }

	//
	// Summary:
	//     Initializes a new instance of the System.Windows.Forms.TreeNode class with
	//     the specified label text, child tree nodes, and images to display when the
	//     tree node is in a selected and unselected state.
	//
	// Parameters:
	//   text:
	//     The label System.Windows.Forms.TreeNode.Text of the new tree node.
	//
	//   imageIndex:
	//     The index value of System.Drawing.Image to display when the tree node is
	//     unselected.
	//
	//   selectedImageIndex:
	//     The index value of System.Drawing.Image to display when the tree node is
	//     selected.
	//
	//   children:
	//     An array of child System.Windows.Forms.TreeNode objects.
	public CXTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children)
		: base(text, imageIndex, selectedImageIndex, children)
	{ }

	#endregion [ Constructors ]
}