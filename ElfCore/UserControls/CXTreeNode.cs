using System.Runtime.Serialization;
using System.Windows.Forms;

/// <summary>
/// TreeNode class customized for the Channel Explorer Tree
/// </summary>
internal class CXTreeNode : TreeNode
{
	#region [ Private Variables ]

	private bool _enabled = true;
	private bool _included = true;
	private ElfCore.Channels.BaseChannel _channel = null;

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

	public ElfCore.Channels.BaseChannel Channel
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

	public CXTreeNode(string text) : base(text) { }

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
	protected CXTreeNode(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }

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
	public CXTreeNode(string text, TreeNode[] children) : base(text, children) { }

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
	public CXTreeNode(string text, int imageIndex, int selectedImageIndex) : base(text, imageIndex, selectedImageIndex) { }

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
	public CXTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : base(text, imageIndex, selectedImageIndex, children) { }

	#endregion [ Constructors ]
}



