using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ElfCore
{
	public partial class ChannelExplorerTree : ToolWindow
	{
		#region [ Declares ]

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetScrollPos(int hWnd, int nBar);

		[DllImport("user32.dll")]
		static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		#endregion [ Declares ]

		#region [ Constants ]

		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;

		private const string LOCKED = "LOCKED";
		private const string CHANNEL_GROUP = "CHANNEL_GROUP";
		private const string NOT_VISIBLE = "NOT_VISIBLE";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private bool _shown = false;
		private TreeNode _clickedNode = null;

		/// <summary>
		/// Indicates whether select events should be listened to coming from the ChannelController
		/// When setting the channels to be selected in this form, this flag should be set so that an infinite loop is
		/// not generated.
		/// </summary>
		private bool _ignoreSelectEvents = false;

		//private Point _clickPoint;
		//private bool _loading = false;
		//private BackgroundImageListItem _bgListItem = null;
		//private int _bgIndex = -1;
		//private bool _drawChannelIcon = false;
		//private bool _grouping = false;
		//private bool _addBackgroundImage = false;
		////private int _hotIndex = 0;
		//private List<int> _selectedIndices = null;
		//private int _activeIndex = 0;

		#endregion [ Private Variables ]

		#region [ Properties ]

		#endregion [ Properties ]

		#region [ Event Handlers ]

		//public delegate void ChannelEventHandler(object sender, ChannelEventArgs e);

		//public ChannelEventHandler ChannelVisible;
		//public ChannelEventHandler ChannelHidden;
		public EventHandler ChannelExplorerResized;

		#endregion [ Event Handlers ]

		#region [ Constructors ]

		public ChannelExplorerTree()
		{
			InitializeComponent();

			//_tags = new List<int>();

			LoadIconsIntoListView();
			BuildTree();

			//_bgListItem = new BackgroundImageListItem();
			//_selectedIndices = new List<int>();
			//_selectedIndices.Add(0);
			//_activeIndex = 0;

			//_workshop.Changed += new Workshop.DataEventHandler(Workshop_Changed);
			_workshop.Channels.ChannelColorChanged += new EventHandler(ChannelController_ColorChanged);
			_workshop.Channels.ChannelNameChanged += new EventHandler(ChannelController_NameChanged);
			_workshop.Channels.ChannelSelected += new EventHandler(ChannelController_Selected);
			_workshop.Channels.ChannelVisibilityChanged += new EventHandler(ChannelController_VisibilityChanged);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Populate the tree control with the Channels.
		/// </summary>
		private void BuildTree()
		{
			TreeNode ChannelNode = null;

			ChannelTree.Nodes.Clear();

			foreach (Channel Channel in _workshop.Channels.Sorted)
			{
				ChannelNode = new TreeNode(Channel.ToString(true));
				ChannelNode.ImageKey = Channel.ColorSwatchKey;
				ChannelNode.SelectedImageKey = Channel.ColorSwatchKey;
				ChannelNode.Tag = Channel;
				ChannelTree.Nodes.Add(ChannelNode);

				if (Channel.IsSelected)
					ChannelTree.SelectedNodes.Add(ChannelNode);

				if (ChannelTree.SelectedNodes.Count > 0)
					_clickedNode = ChannelTree.SelectedNodes[0];
			}
		}

		/// <summary>
		/// Forces the control to refresh
		/// </summary>
		public void Freshen()
		{
			//lstChannels.Refresh();
		}

		/// <summary>
		/// Build the tree with all the channels
		/// </summary>
		//public void LoadChannels()
		//{
		//ListBoxUtil.Item Item = null;

		//foreach (Channel Channel in _workshop.Channels.Sorted)
		//{
		//    Item = new ListBoxUtil.Item(Channel.ToString(true), Channel.ToString(true), Channel);
		//    lstChannels.Items.Add(Item);
		//}
		//if (lstChannels.Items.Count > 0)
		//{
		//    //if (_workshop.ActiveChannelIndex < 0)
		//    //    lstChannels.SelectedIndex = 0;
		//    //else
		//    //{
		//    //    _loading = true;
		//    //    _workshop.SelectedChannels.Clear();
		//    //    _workshop.SelectedChannels.Add(_workshop.ActiveChannel);
		//    //    lstChannels.SelectedIndex = _workshop.ActiveChannelIndex;
		//    //    _loading = false;
		//    //}
		//}

		//if (_addBackgroundImage)
		//	lstChannels.Items.Add(_bgListItem);
		//}

		/// <summary>
		/// Load up the image list with the images stored in the Resources.
		/// </summary>
		private void LoadIconsIntoListView()
		{
			ilstIcons.ColorDepth = ColorDepth.Depth32Bit;
			ilstState.ColorDepth = ColorDepth.Depth32Bit;

			// Build the list of State Icons
			ilstState.Images.Add(LOCKED, ElfRes.lock_state);

			ilstIcons.Images.Clear();
			ilstIcons.Images.Add(CHANNEL_GROUP, ElfRes.channel_group);
			ilstIcons.Images.Add(NOT_VISIBLE, ElfRes.notvisible);

			// Build the list of icons representing the channel colors, putting each bitmap into the ColorSwatches dictionary.
			// If that color is already in the dictionary, skip it.
			foreach (KeyValuePair<string, Bitmap> KVP in ImageController.ColorSwatches)
			{
				ilstIcons.Images.Add(KVP.Key, KVP.Value);
			}
		}

		/// <summary>
		/// Inserts a new node at the position of the old one and deletes the old node.
		/// </summary>
		/// <param name="oldNode">The TreeNode object to replace</param>
		/// <param name="newNode">The new TreeNode object to add</param>
		private void ReplaceTreeNode(TreeNode oldNode, TreeNode newNode)
		{
			bool Expanded = oldNode.IsExpanded;
			bool ParentExpanded = oldNode.Parent.IsExpanded;
			TreeView TreeView = oldNode.TreeView;

			oldNode.Parent.Nodes.Insert(oldNode.Index, newNode);
			oldNode.Parent.Nodes.Remove(oldNode);
			TreeView.SelectedNode = newNode;

			if (Expanded)
				newNode.Expand();
			if (ParentExpanded)
				newNode.Parent.Expand();
			else
				newNode.Parent.Collapse();

			TreeView = null;
		}

		/// <summary>
		/// Goes through all the nodes in the tree and checks to see if the Channel is hidden. If so, updates the icon
		/// </summary>
		private void UpdateChannelIconsForVisibility()
		{
			Channel Channel = null;
			foreach (TreeNode Node in ChannelTree.GetAllNodes(ChannelTree.Nodes))
			{
				Channel = (Channel)Node.Tag;
				if (Channel.Visible)
				{
					Node.SelectedImageKey = Channel.ColorSwatchKey;
					Node.ImageKey = Channel.ColorSwatchKey;
				}
				else
				{
					Node.SelectedImageKey = NOT_VISIBLE;
					Node.ImageKey = NOT_VISIBLE;
				}
				if (Channel.Locked)
					Node.StateImageKey = LOCKED;
				else
					Node.StateImageKey = string.Empty;
			}
		}

		#endregion [ Methods ]

		#region [ Custom Event Methods ]

		//private void OnChannelVisible()
		//{
		//    //if (ChannelVisible == null)
		//    //    return;
		//    //ChannelEventArgs e = new ChannelEventArgs(GetSelectedIndicies(), lstChannels.SelectedIndex);
		//    //ChannelVisible(this, e);
		//    //lstChannels.Refresh();
		//}

		//private void OnChannelHidden()
		//{
		//    //if (ChannelHidden == null)
		//    //    return;
		//    //ChannelEventArgs e = new ChannelEventArgs(GetSelectedIndicies(), lstChannels.SelectedIndex);
		//    //ChannelHidden(this, e);
		//    //lstChannels.SelectedIndex = FindNextVisibleChannel();
		//    //lstChannels.Refresh();
		//}

		#endregion [ Custom Event Methods ]

		#region [ Events ]

		#region [ Form Events ]

		private void Form_DockStateChanged(object sender, EventArgs e)
		{
			if (ChannelExplorerResized != null)
				ChannelExplorerResized(this, e);
		}

		private void Form_SizeChanged(object sender, EventArgs e)
		{
			if (!_shown)
				return;
			if (ChannelExplorerResized != null)
				ChannelExplorerResized(this, e);
		}

		private void Form_Shown(object sender, EventArgs e)
		{
			_shown = true;
		}

		#endregion [ Form Events ]

		#region [ ChannelController Events ]

		/// <summary>
		/// Event that fires from the ChannelController when the Color on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_ColorChanged(object sender, EventArgs e)
		{
			if (_ignoreSelectEvents)
				return;

			// The color of a particular Channel changed, find it's TreeNode and update the icon for it
			// swatch. If the color is unique, then generate a new image and add it to the ImageList control first.
			List<Channel> List = null;

			if (sender.GetType().Name == "Channel")
			{
				List = new List<Channel>();
				List.Add((Channel)sender);
			}
			else
				List = (List<Channel>)sender;

			//(List<Channel>)sender;
			TreeNode Node = null;
			foreach (Channel Channel in List)
			{
				Node = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
						where ((Channel)s.Tag).ID == Channel.ID
						select s).FirstOrDefault();
				if (Node == null)
					continue;

				// Check to see if this color already exists in the image list control, if not, then add it
				if (!ilstIcons.Images.Keys.Contains(Channel.ColorSwatchKey))
					ilstIcons.Images.Add(Channel.ColorSwatchKey, ImageController.ColorSwatches[Channel.ColorSwatchKey]);

				Node.ImageKey = Channel.ColorSwatchKey;
				Node.SelectedImageKey = Channel.ColorSwatchKey;
			}
			List = null;
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Name on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_NameChanged(object sender, EventArgs e)
		{
			if (_ignoreSelectEvents)
				return;

			List<Channel> List = null;

			if (sender.GetType().Name == "Channel")
			{
				List = new List<Channel>();
				List.Add((Channel)sender);
			}
			else
				List = (List<Channel>)sender;

			TreeNode Node = null;
			foreach (Channel Channel in List)
			{
				Node = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
						where ((Channel)s.Tag).ID == Channel.ID
						select s).FirstOrDefault();
				if (Node == null)
					return;

				Node.Text = ((Channel)sender).Name;
				Node = null;
			}
			List = null;
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Selected flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_Selected(object sender, EventArgs e)
		{
			if (_ignoreSelectEvents)
				return;

			// Find all the tree nodes that correspond to the selected Channels
			ChannelTree.SelectedNodes.Clear();

			List<Channel> List = null;

			if (sender.GetType().Name == "Channel")
			{
				List = new List<Channel>();
				List.Add((Channel)sender);
			}
			else
				List = (List<Channel>)sender;

			List<TreeNode> NodeList = null;

			foreach (Channel Channel in List)
			{
				NodeList = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
							where ((Channel)s.Tag).IsSelected == true
							select s).ToList();

				ChannelTree.SelectedNodes = NodeList;
			}
			NodeList = null;
			List = null;
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Visibility flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_VisibilityChanged(object sender, EventArgs e)
		{
			if (_ignoreSelectEvents)
				return;

			List<Channel> List = null;

			if (sender.GetType().Name == "Channel")
			{
				List = new List<Channel>();
				List.Add((Channel)sender);
			}
			else
				List = (List<Channel>)sender;

			// ...

			List = null;
		}


		#endregion [ ChannelController Events ]

		#region [ Context Menu Events ]

		private void Context_Clear_Click(object sender, EventArgs e)
		{
			string Channel = "Channel" + ((_workshop.Channels.Selected.Count() > 1) ? "s" : string.Empty);
			string Title = string.Format("Clear {0}", Channel);
			if (MessageBox.Show(string.Format("Really clear the cells from the selected {0}?", Channel), Title,
								MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
				return;

			_workshop.Channels.ClearSelectedChannels();
			_workshop.UndoController.SaveUndo(Title);
		}

		/// <summary>
		/// Puts all the currently selected Channels into a group
		/// </summary>
		private void Context_Group_Click(object sender, EventArgs e)
		{
			
		}

		/// <summary>
		/// Sets the Visible property for all selected Channels to false.
		/// </summary>
		private void Context_Hide_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Selected)
				Channel.Visible = false;
			UpdateChannelIconsForVisibility();
		}

		/// <summary>
		/// Sets the Visible property for all Channels except the selected ones to false.
		/// </summary>
		private void Context_HideAllNonSelected_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Sorted)
			{
				if (!Channel.IsSelected)
					Channel.Visible = false;
			}
			UpdateChannelIconsForVisibility();
		}

		/// <summary>
		/// Set the visibility property for all the selected Channels to true
		/// </summary>
		private void Context_Show_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Selected)
				Channel.Visible = true;
			UpdateChannelIconsForVisibility();
		}

		/// <summary>
		/// Set the visibility property for all the Channels to true
		/// </summary>
		private void Context_ShowAll_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Sorted)
				Channel.Visible = true;
			UpdateChannelIconsForVisibility();
		}

		/// <summary>
		/// Removes the grouping from the currently selected group
		/// </summary>
		private void Context_Ungroup_Click(object sender, EventArgs e)
		{
			
		}
		
		private void Context_ChangeColor_Click(object sender, EventArgs e)
		{
			ChannelColorDialog.Color = _workshop.Channels.Active.Color;
			ChannelColorDialog.AllowFullOpen = true;
			ChannelColorDialog.AnyColor = true;
			if (ChannelColorDialog.ShowDialog() == DialogResult.Cancel)
				return;

			_workshop.Channels.Active.Color = ChannelColorDialog.Color;
			_workshop.UndoController.SaveUndo("Change Channel Color");
		}

		private void Context_Rename_Click(object sender, EventArgs e)
		{
			if (_clickedNode != null)
			{
				if (!_clickedNode.IsSelected)
					ChannelTree.SelectedNode = _clickedNode;
				ChannelTree.LabelEdit = true;
				if (!_clickedNode.IsEditing)
				{
					string Text = _clickedNode.Text;
					_clickedNode.Text = Text.Substring(Text.IndexOf(" ") + 1);
					_clickedNode.BeginEdit();
				}
			}

		}

		/// <summary>
		/// Determine which menus need to be enabled based on what is currently selected
		/// </summary>
		private void ChannelExplorer_ContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool HasHidden = false;
			bool HasVisible = false;
			bool HasLocked = false;
			bool HasUnlocked = false;

			//if (_workshop.Channels.Selected.Count > 1)
			//{
			//    Context_Hide.Text = "&Hide Channels";
			//    Context_Show.Text = "&Show Channels";
			//    Context_Lock.Text = "&Lock Channels";
			//    Context_Unlock.Text = "U&nlock Channels";
			//}
			//else
			//{
			//    Context_Hide.Text = "&Hide Channel";
			//    Context_Show.Text = "&Show Channel";
			//    Context_Lock.Text = "&Lock Channel";
			//    Context_Unlock.Text = "U&nlock Channel";
			//}

			foreach (Channel Channel in _workshop.Channels.Selected)
			{
				if (Channel.Visible)
					HasVisible = true;
				else
					HasHidden = true;
				if (Channel.Locked)
					HasLocked = true;
				else
					HasUnlocked = true;
			}
			Context_Show.Enabled = HasHidden;
			Context_Hide.Enabled = HasVisible;
			Context_Lock.Enabled = HasUnlocked;
			Context_Ungroup.Enabled = HasLocked;
		}

		private void Context_Lock_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Selected)
				Channel.Locked = true;
			UpdateChannelIconsForVisibility();
		}

		private void Context_Unlock_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in _workshop.Channels.Selected)
				Channel.Locked = false;
			UpdateChannelIconsForVisibility();
		}

		private void Context_MoveUp_Click(object sender, EventArgs e)
		{

		}

		private void Context_MoveDown_Click(object sender, EventArgs e)
		{

		}
		#endregion [ Context Menu Events ]

		#region [ TreeView events ]

		/// <summary>
		/// Event that fires after one or more TreeNodes have been selected. Set the IsSelected flag on the attached Channel object
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			_ignoreSelectEvents = true;
			Channel Channel = null;

			_workshop.Channels.UnselectAll(false);
			List<int> Selected = new List<int>();

			foreach (TreeNode Node in ChannelTree.SelectedNodes)
			{
				Channel = ((Channel)Node.Tag);
				Selected.Add(Channel.Index);
			}
			_workshop.Channels.Select(Selected);
			Selected = null;
			_ignoreSelectEvents = false;
		}

		/// <summary>
		/// Event that fires after the edit of the TreeNode label is complete. Set the new value as the name of the Channel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			Channel Channel = (Channel)e.Node.Tag;

			if (e.CancelEdit || (e.Label == null))
			{
				e.Node.Text = Channel.ToString(true);
				return;
			}

			Channel.Name = e.Label;
			//e.Node.NodeFont = new Font(ChannelTree.Font, FontStyle.Italic);
			_workshop.UndoController.SaveUndo("Change Channel Name");

			// If we don't "cancel" at this point, then we cannot update the node with the index + the new name
			e.CancelEdit = true;
			e.Node.Text = Channel.ToString(true);
		}

		#endregion [ TreeView events ]

		private void tmrFocus_Tick(object sender, EventArgs e)
		{
			tmrFocus.Enabled = false;
			ChannelTree.Focus();
		}

		private void ChannelTree_MouseDown(object sender, MouseEventArgs e)
		{
			_clickedNode = ChannelTree.GetNodeAt(e.X, e.Y);
		}

		#endregion [ Events ]

		
		
		#region [ DEAD CODE ]

		//private void Workshop_Changed(object sender, DataEventArgs e)
		//{
		//if (_ignoreSelectEvents)
		//    return;

		//Debug.WriteLine("CHANNEL EXPLORER: " + e.GeneralEventType.ToString() + "\t" + e.SpecificEventType.ToString());

		//List<TreeNode> NodeList = null;
		//TreeNode Node = null;
		//Channel Channel = null;

		//try
		//{
		//    switch (e.Category)
		//    {
		//        case EventCategory.Channel:

		//            switch (e.SubCategory)
		//            {
		//                case EventSubCategory.Channel_Selected:
		//                   //  Find all the tree nodes that correspond to the selected Channels
		//                    ChannelTree.SelectedNodes.Clear();

		//                    NodeList = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
		//                                where ((Channel)s.Tag).IsSelected == true
		//                                select s).ToList();

		//                    ChannelTree.SelectedNodes = NodeList;
		//                    NodeList = null;
		//                    break;

		//                case EventSubCategory.Channel_NameChanged:
		//                    // The name of a particular Channel changed, find it's TreeNode and update the text on it
		//                    Channel = (Channel)sender;

		//                    Node = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
		//                            where ((Channel)s.Tag).ID == Channel.ID
		//                            select s).FirstOrDefault();
		//                    if (Node == null)
		//                        return;

		//                    Node.Text = ((Channel)sender).Name;
		//                    Node = null;
		//                    break;

		//                case EventSubCategory.Channel_ColorChanged:
		//                    // The color of a particular Channel changed, find it's TreeNode and update the icon for it
		//                    // swatch. If the color is unique, then generate a new image and add it to the ImageList control first.
		//                    Channel = (Channel)sender;

		//                    Node = (from s in ChannelTree.GetAllNodes(ChannelTree.Nodes)
		//                            where ((Channel)s.Tag).ID == Channel.ID
		//                            select s).FirstOrDefault();
		//                    if (Node == null)
		//                        return;

		//                    // Check to see if this color already exists
		//                    if (ImageController.ColorSwatches[Channel.ColorSwatchKey] == null)
		//                        ImageController.CreateAndAddColorSwatch(Channel.Color);

		//                    Node.ImageKey = Channel.ColorSwatchKey;
		//                    Node.SelectedImageKey = Channel.ColorSwatchKey;
		//                    break;
		//            }
		//            break;
		//    }
		//}
		//finally
		//{ }
		//}

		#endregion [ DEAD CODE ]

	}

}

