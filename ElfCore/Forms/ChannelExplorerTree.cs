using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class ChannelExplorerTree : ToolWindow
	{
		#region [ Declares ]

		#endregion [ Declares ]

		#region [ Constants ]

		private const string LOCKED = "LOCKED";
		private const string CHANNEL = "CHANNEL";
		private const string STATIC_CHANNEL = "STATIC_CHANNEL";
		private const string BACKGROUND = "BACKGROUND";
		private const string NOT_VISIBLE = "NOT_VISIBLE";
		private const string NOT_VISIBLE_LOCKED = "NOT_VISIBLE_LOCKED";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private bool _shown = false;
		private CXTreeNode _clickedNode = null;
		private ListBoxUtil _listBoxUtil = null;
		private SortedList<string, int> _scrollPositions = new SortedList<string, int>();
		private bool _disposed = false;
		
		/// <summary>
		/// Indicates whether select events should be listened to coming from the ChannelController
		/// When setting the channels to be selected in this form, this flag should be set so that an infinite loop is
		/// not generated.
		/// </summary>
		private bool SuppressEvents = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Link to the Editor form that owns this form.
		/// </summary>
		public Editor Editor { get; set; }

		/// <summary>
		/// Gets the Active Profile
		/// </summary>
		[DebuggerHidden()]
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelExplorerTree(Editor editor)
		{
			InitializeComponent();
			Editor = editor;
			SuppressEvents = false;
			AttachEvents();

			_listBoxUtil = new ListBoxUtil();

			cmdAdd.Enabled = false;
			cmdEdit.Enabled = false;
			cmdDelete.Enabled = false;
			cboShuffles.Enabled = false;
			ChannelTree.Enabled = false;

			LoadIconsIntoListView();
			BuildMenuImages();
			
			ConfigureRunMode();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Checks to see if the color swatch icon for this channel is already present in the image list used by the tree control.
		/// If not, then adds the image from the ImageHandler and sets the ImageKey of the CXTreeNode to this new color.
		/// </summary>
		/// <param name="channelNode">CXTreeNode associated with the Channel.</param>
		private void AddColorSwatchIcon(CXTreeNode channelNode)
		{
			if ((channelNode == null) || (channelNode.Channel == null))
				throw new ArgumentNullException("channelNode or channelNode.Channel are null.");

			Channel Channel = channelNode.Channel;
			string ColorSwatchKey = Channel.ChannelExplorerImageKey ?? string.Empty;

			if (!ilstIcons.Images.Keys.Contains(ColorSwatchKey))
				ilstIcons.Images.Add(ColorSwatchKey, ImageHandler.ColorSwatches[ColorSwatchKey]);

			channelNode.ImageKey = Channel.ChannelExplorerImageKey;
			channelNode.SelectedImageKey = Channel.ChannelExplorerImageKey;
		}

		/// <summary>
		/// Attach events
		/// </summary>
		private void AttachEvents()
		{
			_workshop.ProfileController.Switched += Profiles_Switched;

			// Attach the context menu to the methods from the Editor form.
			Context_AddNew.Click += Editor.Channel_AddNew_Click;
			Context_Channel_Properties.Click += Editor.Channel_Properties_Click;
			Context_Clear.Click += Editor.Channel_Clear_Click;
			Context_ClearAll.Click += Editor.Channel_Clear_AllChannels_Click;
			Context_Delete.Click += Editor.Channel_Delete_Click;
			Context_Disable.Click += Editor.Channel_Disable_Click;
			Context_Enable.Click += Editor.Channel_Enable_Click;
			Context_Lock.Click += Editor.Channel_Lock_Click;
			Context_Unlock.Click += Editor.Channel_Unlock_Click;
			Context_Show.Click += Editor.Channel_Show_Click;
			Context_ShowAll.Click += Editor.Channel_ShowAll_Click;
			Context_HideOthers.Click += Editor.Channel_HideOthers_Click;
			Context_Hide.Click += Editor.Channel_Hide_Click;
			Context_Include.Click += Editor.Channel_Include_Click;
			Context_Exclude.Click += Editor.Channel_Exclude_Click;
			Context_AddNew.Click += Editor.Channel_AddNew_Click;
			Context_Rename.Click += Editor.Channel_Rename_Click;
			Context_ChangeRendColor.Click += Editor.Channel_ChangeRendColor_Click;
			Context_ChangeSeqColor.Click += Editor.Channel_ChangeSeqColor_Click;
			Context_Group.Click += Editor.Channel_Group_Click;
			Context_Ungroup.Click += Editor.Channel_Ungroup_Click;
			Context_MoveToTop.Click += Editor.Channel_MoveToTop_Click;
			Context_MoveUp.Click += Editor.Channel_MoveUp_Click;
			Context_MoveDown.Click += Editor.Channel_MoveDown_Click;
			Context_MoveToBottom.Click += Editor.Channel_MoveToBottom_Click;
		}

		/// <summary>
		/// Construct the custom images for the menu items, such as adding the Create annotation image to the Profile image for the New Profile menu item.
		/// </summary>
		private void BuildMenuImages()
		{
			cmdEdit.Image = ImageHandler.AddAnnotation(ElfRes.shuffle, Annotation.Edit, AnchorStyles.Bottom | AnchorStyles.Right);
			cmdDelete.Image = ImageHandler.AddAnnotation(ElfRes.shuffle, Annotation.Not, AnchorStyles.Bottom | AnchorStyles.Right);

			Context_Delete.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Delete, new Point(3, 0));
			Context_ClearAll.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.ClearStar, AnchorStyles.Bottom | AnchorStyles.Right);
			Context_Include.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Include, new Point(3, 0));
			Context_Exclude.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Exclude, new Point(3, 0));
			Context_ChangeRendColor.Image = ImageHandler.AddAnnotation(ElfRes.palette, Annotation.Monitor, AnchorStyles.Bottom | AnchorStyles.Right);
			Context_ChangeSeqColor.Image = ImageHandler.AddAnnotation(ElfRes.palette, Annotation.Grid, AnchorStyles.Bottom | AnchorStyles.Right);
			Context_Unlock.Image = ImageHandler.AddAnnotation(ElfRes._lock, Annotation.Clear, AnchorStyles.Bottom | AnchorStyles.Right);
			Context_Hide.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Invisible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-1, 0));
			Context_HideOthers.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.Invisible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-2, 0));
			Context_Show.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Visible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-1, 0));
			Context_ShowAll.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.Visible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-2, 0));
			Context_Enable.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Complete, new Point(3, 0));
			Context_Disable.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Error, new Point(3, 0));
		}

		/// <summary>
		/// Builds the list of Sort Orders and loads them into the combo box, selecting the active sort order.
		/// </summary>
		internal void BuildShuffleList()
		{
			if (Profile == null)
				return;

			bool OldSuppressEvents = SuppressEvents;
			SuppressEvents = true;

			cboShuffles.Items.Clear();

			foreach (Shuffle Shuffle in Profile.Channels.ShuffleController.All)
			{
				_listBoxUtil.Add(cboShuffles, new ListBoxUtil.Item(Shuffle.Name, Shuffle.ID));
			}
			if (cboShuffles.Items.Count > 0)
			{
				_listBoxUtil.Set(cboShuffles, Profile.Channels.ShuffleController.Active.ID);
				_listBoxUtil.SetDropDownWidth(cboShuffles);
			}
			SetShuffleControls();
			SuppressEvents = OldSuppressEvents;
		}

		/// <summary>
		/// Populate the tree control with the Channels.
		/// </summary>
		private void BuildTree()
		{
			BuildTree(false);
		}

		/// <summary>
		/// Populate the tree control with the Channels. If rebuilding, then it should remember all expanded nodes
		/// </summary>
		/// <param name="rebuild">Indicates whether expanded nodes should be recalled.</param>
		public void BuildTree(bool rebuild)
		{
			_clickedNode = null;
			CXTreeNode ChannelNode = null;
			List<CXTreeNode> SelectedNodes = new List<CXTreeNode>();
			List<string> ExpandedNodePaths = new List<string>();

			if (rebuild)
			{ 
				// Find all the paths of nodes that have their Expanded flags set to True.
				ExpandedNodePaths.AddRange(GetExpandedNodePaths(ChannelTree.Nodes));
			}

			// Note: not yet sure how to handle Channel Groups in the tree.
			ChannelTree.Nodes.Clear();
			FontStyle FontStyle = ChannelTree.Font.Style;

			if (Profile.Channels.Count > 0)
			{
				foreach (Channel Channel in Profile.Channels.Sorted)
				{
					ChannelNode = new CXTreeNode(Channel.ToString(true));
					ChannelNode.Channel = Channel;
					ChannelNode.Enabled = Channel.Enabled;
					ChannelNode.Included = Channel.Included;
					ChannelNode.NodeFont = new Font(ChannelTree.Font, FontStyle.Regular);
					FontStyle = ChannelTree.Font.Style;

					if (!Channel.Enabled)
						FontStyle |= FontStyle.Italic;

					if (!Channel.Included)
						FontStyle |= FontStyle.Strikeout;

					if (FontStyle != ChannelTree.Font.Style)
						ChannelNode.NodeFont = new Font(ChannelNode.NodeFont, FontStyle);

					ChannelNode.Collapse();
					AddColorSwatchIcon(ChannelNode);
					ChannelTree.Nodes.Add(ChannelNode);

					if (Channel.IsSelected)
						SelectedNodes.Add(ChannelNode);

					if (ChannelTree.SelectedNodes.Count > 0)
						_clickedNode = ChannelTree.SelectedNodes[0];
				}
			}
			ChannelTree.SelectedNodes = SelectedNodes;

			if (rebuild)
			{
				TreeNode[] FoundNodes;
				foreach (string FullPath in ExpandedNodePaths)
				{
					FoundNodes = ChannelTree.Nodes.Find(FullPath, true);
					if (FoundNodes != null)
					{
						for (int i = 0; i < FoundNodes.Length; i++)
							FoundNodes[i].Expand();
					}
				}
			}

			ExpandedNodePaths = null;
			SelectedNodes = null;

			int Scroll = 0;
			if (_scrollPositions.Keys.Contains(Profile.GUID))
				Scroll = _scrollPositions[Profile.GUID];
			ChannelTree.VScrollPos = Scroll;
		}

		/// <summary>
		/// Configures the form to either run as a Single Document Interface (PlugIn mode), or as a
		/// Multiple Document Interface (stand-alone mode)
		/// </summary>
		private void ConfigureRunMode()
		{
			if (_workshop.RunMode == RunMode.PlugIn)
			{
				Context_AddNew.Visible = false;
				Context_Delete.Visible = false;
				Context_Sep1.Visible = false;
				Context_Arrange.Visible = false;
				Context_Enable.Visible = false;
				Context_Disable.Visible = false;
				Context_Display_Sep2.Visible = false;
				Context_Display_Sep3.Visible = false;
				Context_Include.Visible = false;
				Context_Exclude.Visible = false;
				Context_Rename.Visible = false;
				Context_ChangeSeqColor.Visible = false;
				
				cmdAdd.Visible = false;
				cmdEdit.Visible = false;
				cmdDelete.Visible = false;
				pnlSep1.Visible = false;
				pnlSep2.Visible = false;
				pnlSep3.Visible = false;
			}
		}

		/// <summary>
		/// Detach manually attached events
		/// </summary>
		private void DetachEvents()
		{
			_workshop.ProfileController.Switched -= Profiles_Switched;

			if (Profile != null)
			{
				Profile.ChannelRemoved -= Profile_ChannelRemoved;
				Profile.ShuffleChanged -= Profile_ShuffleChanged;
				Profile.ShuffleSwitched -= Profile_ShuffleSwitched;
				Profile.Undo_Completed -= Undo_Completed;
				Profile.ChannelsSelected -= Profile_ChannelsSelected;
				Profile.ChannelPropertyChanged -= Profile_ChannelPropertyChanged;
			}

			// Detach the context menu from the methods from the Editor form.
			Context_AddNew.Click -= Editor.Channel_AddNew_Click;
			Context_Channel_Properties.Click -= Editor.Channel_Properties_Click;
			Context_Clear.Click -= Editor.Channel_Clear_Click;
			Context_ClearAll.Click -= Editor.Channel_Clear_AllChannels_Click;
			Context_Delete.Click -= Editor.Channel_Delete_Click;
			Context_Disable.Click -= Editor.Channel_Disable_Click;
			Context_Enable.Click -= Editor.Channel_Enable_Click;
			Context_Lock.Click -= Editor.Channel_Lock_Click;
			Context_Unlock.Click -= Editor.Channel_Unlock_Click;
			Context_Show.Click -= Editor.Channel_Show_Click;
			Context_ShowAll.Click -= Editor.Channel_ShowAll_Click;
			Context_HideOthers.Click -= Editor.Channel_HideOthers_Click;
			Context_Hide.Click -= Editor.Channel_Hide_Click;
			Context_Include.Click -= Editor.Channel_Include_Click;
			Context_Exclude.Click -= Editor.Channel_Exclude_Click;
			Context_AddNew.Click -= Editor.Channel_AddNew_Click;
			Context_Rename.Click -= Editor.Channel_Rename_Click;
			Context_ChangeRendColor.Click -= Editor.Channel_ChangeRendColor_Click;
			Context_ChangeSeqColor.Click -= Editor.Channel_ChangeSeqColor_Click;
			Context_Group.Click -= Editor.Channel_Group_Click;
			Context_Ungroup.Click -= Editor.Channel_Ungroup_Click;
			Context_MoveToTop.Click -= Editor.Channel_MoveToTop_Click;
			Context_MoveUp.Click -= Editor.Channel_MoveUp_Click;
			Context_MoveDown.Click -= Editor.Channel_MoveDown_Click;
			Context_MoveToBottom.Click -= Editor.Channel_MoveToBottom_Click;
		}

		/// <summary>
		/// Finds the CXTreeNode associated with this Channel.
		/// </summary>
		/// <param name="channel">Channel to search for in the tree control.</param>
		/// <param name="nodes">Collection of nodes to search.</param>
		/// <returns>Returns the CXTreeNode if found, null otherwise.</returns>
		private CXTreeNode FindTreeNode(Channel channel, TreeNodeCollection nodes)
		{
			if (channel == null)
				throw new ArgumentNullException("channel is null.");
			if (nodes == null)
				return null;

			foreach (CXTreeNode Node in nodes)
			{
				if (ReferenceEquals(Node.Channel, channel))
					return Node;
				if (Node.Nodes != null)
				{
					CXTreeNode FoundIt = FindTreeNode(channel, Node.Nodes);
					if (FoundIt != null)
						return FoundIt;
				}
			}
			return null;
		}

		/// <summary>
		/// Loops through the collection, looking for the paths of all nodes that are expanded. If the node is expanded and has child nodes, then recursively navigate 
		/// into the nodes collection and gets that list as well.
		/// </summary>
		/// <param name="nodes">Collection of CXTreeNodes. Can be null.</param>
		/// <returns>List of paths to all CXTreeNodes that are expanded</returns>
		private List<string> GetExpandedNodePaths(TreeNodeCollection nodes)
		{
			List<string> NodeList = new List<string>();

			if (nodes == null)
				return NodeList;

			foreach (CXTreeNode Node in nodes)
			{
				if (Node.IsExpanded)
					NodeList.Add(Node.FullPath);
				NodeList.AddRange(GetExpandedNodePaths(Node.Nodes));
			}

			return NodeList;
		}

		/// <summary>
		/// Load up the image list with the images stored in the Resources.
		/// </summary>
		private void LoadIconsIntoListView()
		{
			ilstIcons.ColorDepth = ColorDepth.Depth32Bit;
			//ilstState.ColorDepth = ColorDepth.Depth32Bit;

			// Build the list of State Icons
			//ilstState.Images.Add(LOCKED, ElfRes.lock_state);

			ilstIcons.Images.Clear();
			ilstIcons.Images.Add(CHANNEL, ElfRes.channel);
			ilstIcons.Images.Add(BACKGROUND, ElfRes.background);
			ilstIcons.Images.Add(NOT_VISIBLE, ElfRes.visible);

			Bitmap InvisibleLocked = ImageHandler.CopyImage(ElfRes.visible);
			ilstIcons.Images.Add(NOT_VISIBLE_LOCKED, ImageHandler.AddAnnotation(InvisibleLocked, Annotation.Locked, AnchorStyles.Left | AnchorStyles.Bottom));

			// Build the list of icons representing the channel colors, putting each bitmap into the ColorSwatches dictionary.
			// If that color is already in the dictionary, skip it.
			foreach (KeyValuePair<string, Bitmap> KVP in ImageHandler.ColorSwatches)
			{
				ilstIcons.Images.Add(KVP.Key, KVP.Value);
			}
		}

		/// <summary>
		/// Finds the node for the active channel and begins the edit process on it.
		/// </summary>
		internal void RenameChannel()
		{
			CXTreeNode Node = FindTreeNode(Profile.Channels.Active, ChannelTree.Nodes);
			if (!_clickedNode.IsEditing)
			{
				string Text = _clickedNode.Text;
				Node.Text = Text.Substring(Text.IndexOf(" ") + 1);
				Node.BeginEdit();
			}
		}

		/// <summary>
		/// Inserts a new node at the position of the old one and deletes the old node.
		/// </summary>
		/// <param name="oldNode">The TreeNode object to replace</param>
		/// <param name="newNode">The new TreeNode object to add</param>
		private void ReplaceTreeNode(CXTreeNode oldNode, CXTreeNode newNode)
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
		/// Sets the Edit and Delete buttons to be enabled only if the active Shuffle is not readonly.
		/// </summary>
		internal void SetShuffleControls()
		{
			if (Profile.Channels.ShuffleController.Active.ReadOnly)
			{
				cmdDelete.Enabled = false;
				cmdEdit.Enabled = false;
			}
			else
			{
				cmdDelete.Enabled = true;
				cmdEdit.Enabled = true;
			}
			if (Editor != null)
			{
				Editor.Channel_Shuffle_Edit.Enabled = cmdEdit.Enabled;
				Editor.Channel_Shuffle_Delete.Enabled = cmdDelete.Enabled;
			}
		}

		/// <summary>
		/// Goes through all the nodes in the tree and checks to see if the Channel is hidden. If so, updates the icon
		/// </summary>
		private void UpdateChannelIcon()
		{
			foreach (CXTreeNode Node in ChannelTree.GetAllNodes(ChannelTree.Nodes))
				UpdateChannelIcon(Node);
		}

		/// <summary>
		/// Checks the Channel on the node specified to see if it is visible or not, updates the icon accordingly.
		/// </summary>
		private void UpdateChannelIcon(CXTreeNode node)
		{
			Channel Channel = null;
			Channel = node.Channel;
			if (Channel.Visible)
			{
				node.SelectedImageKey = Channel.ChannelExplorerImageKey;
				node.ImageKey = Channel.ChannelExplorerImageKey;
			}
			else
			{
				if (Channel.Locked)
				{
					node.SelectedImageKey = NOT_VISIBLE_LOCKED;
					node.ImageKey = NOT_VISIBLE_LOCKED;
				}
				else
				{
					node.SelectedImageKey = NOT_VISIBLE;
					node.ImageKey = NOT_VISIBLE;
				}
			}
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		/// <summary>
		/// Occurs when the form is resized.
		/// </summary>
		private void OnResized()
		{
			//Debug.WriteLine("OnResized" + "\t" + this.Width + "\t" + this.DockState.ToString());

			if (!_shown)
				return;

			_listBoxUtil.SetDropDownWidth(cboShuffles);

			if (ChannelExplorerResized != null)
				ChannelExplorerResized(this, new EventArgs());
		}

		#endregion [ Event Triggers ]

		#region [ Event Delegates ]

		#region [ Form Events ]

		private void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			DetachEvents();
		}

		/// <summary>
		/// Form is closing, clean up any objects and detach events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{ }

		private void Form_DockStateChanged(object sender, EventArgs e)
		{
			if (!_disposed)
				OnResized();
		}

		private void Form_SizeChanged(object sender, EventArgs e)
		{
			if (!_disposed)
				OnResized();
		}

		private void Form_Shown(object sender, EventArgs e)
		{
			_shown = true;
		}

		#endregion [ Form Events ]
	
		#region [ TreeView events ]

		/// <summary>
		/// Event that fires after one or more CXTreeNodes have been selected. Set the IsSelected flag on the attached Channel object
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			SuppressEvents = true;
			Channel Channel = null;

			Profile.Channels.UnselectAll(false);
			List<int> Selected = new List<int>();

			foreach (CXTreeNode Node in ChannelTree.SelectedNodes)
			{
				Channel = Node.Channel;
				Selected.Add(Channel.Index);
			}
			Profile.Channels.Select(Selected);
			Selected = null;
			SuppressEvents = false;
		}

		/// <summary>
		/// Event that fires after the edit of the CXTreeNode label is complete. Set the new value as the name of the Channel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			Channel Channel = ((CXTreeNode)e.Node).Channel;

			if (e.CancelEdit || (e.Label == null))
			{
				e.Node.Text = Channel.ToString(true);
				return;
			}

			Channel.Name = e.Label;
			//e.Node.NodeFont = new Font(ChannelTree.Font, FontStyle.Italic);
			Profile.SaveUndo("Change Channel Name");

			// If we don't "cancel" at this point, then we cannot update the node with the index + the new name
			e.CancelEdit = true;
			e.Node.Text = Channel.ToString(true);
		}

		/// <summary>
		/// Record the position of the mouse down event.
		/// </summary>
		private void ChannelTree_MouseDown(object sender, MouseEventArgs e)
		{
			_clickedNode = (CXTreeNode)ChannelTree.GetNodeAt(e.X, e.Y);
		}

		/// <summary>
		/// Record the scroll position of the tree. This is used to restore the scroll position if the Profile is switched.
		/// </summary>
		private void ChannelTree_Scroll(object sender, ScrollEventArgs e)
		{
			if (_scrollPositions.Keys.Contains(Profile.GUID))
				_scrollPositions[Profile.GUID] = ChannelTree.VScrollPos;
			else
				_scrollPositions.Add(Profile.GUID, ChannelTree.VScrollPos);
		}

		#endregion [ TreeView events ]

		#region [ Profile Events ]

		/// <summary>
		/// Occurs when the Active Profile changes.
		/// </summary>
		private void Profiles_Switched(object sender, ProfileEventArgs e)
		{
			if (Profile != null)
			{
				Profile.ChannelRemoved -= Profile_ChannelRemoved;
				Profile.ShuffleChanged -= Profile_ShuffleChanged;
				Profile.ShuffleSwitched -= Profile_ShuffleSwitched;
				Profile.Undo_Completed -= Undo_Completed;
				Profile.ChannelsSelected -= Profile_ChannelsSelected;
				Profile.ChannelPropertyChanged -= Profile_ChannelPropertyChanged;
			}
			if (e.Profile != null)
			{
				ChannelTree.Enabled = true;
				cmdAdd.Enabled = true;
				cboShuffles.Enabled = true;

				e.Profile.ChannelRemoved += Profile_ChannelRemoved;
				e.Profile.ShuffleChanged += Profile_ShuffleChanged;
				e.Profile.ShuffleSwitched += Profile_ShuffleSwitched;
				e.Profile.Undo_Completed += Undo_Completed;
				e.Profile.ChannelsSelected += Profile_ChannelsSelected;
				e.Profile.ChannelPropertyChanged += Profile_ChannelPropertyChanged;

				// Reload the Channel list.
				BuildTree();

				// Load up the Shuffles into the comboBox
				BuildShuffleList();
				SetShuffleControls();
			}
			else
			{
				ChannelTree.Nodes.Clear();
				cmdAdd.Enabled = false;
				cmdEdit.Enabled = false;
				cmdDelete.Enabled = false;
				cboShuffles.Items.Clear();
				cboShuffles.Enabled = false;
				ChannelTree.Enabled = false;
			}
		}

		/// <summary>
		/// Occurs when a channel is removed from the Profile.
		/// </summary>
		private void Profile_ChannelRemoved(object sender, ChannelEventArgs e)
		{
			BuildTree(true);
			Profile.Channels.Active = Profile.Channels.Sorted[0];
			ChannelTree.SelectSingleNode((CXTreeNode)ChannelTree.Nodes[0]);
		}

		/// <summary>
		/// Occurs when a property on 1 or more Channels has changed.
		/// </summary>
		private void Profile_ChannelPropertyChanged(object sender, ChannelListEventArgs e)
		{
			if (SuppressEvents)
				return;

			if ((Profile == null) || (Profile.Channels == null) || (Profile.Channels.Active == null))
				return;

			FontStyle eFontStyle = ChannelTree.Font.Style;
			CXTreeNode ChannelNode = null;
			foreach (Channel Channel in e.Channels)
			{
				ChannelNode = FindTreeNode(Channel, ChannelTree.Nodes);
				if (ChannelNode == null)
					continue;

				eFontStyle = FontStyle.Regular;

				switch (e.PropertyName)
				{
					case Channel.Property_RenderColor:
					case Channel.Property_SequencerColor:
					case Channel.Property_BorderColor:
					case Channel.Property_Locked:
					case Channel.Property_Included:
					case Channel.Property_Enabled:
					case Channel.Property_Visible:
						AddColorSwatchIcon(ChannelNode);
						UpdateChannelIcon(ChannelNode);

						ChannelNode.Enabled = Channel.Enabled;
						ChannelNode.Included = Channel.Included;

						if (!Channel.Enabled)
							eFontStyle |= FontStyle.Italic;

						if (!Channel.Included)
							eFontStyle |= FontStyle.Strikeout;

						if (eFontStyle != ChannelNode.NodeFont.Style)
							ChannelNode.NodeFont = new Font(ChannelNode.NodeFont, eFontStyle);

						break;

					case Channel.Property_Name:
						ChannelNode.Text = Channel.ToString(true);
						break;

					default:
						break;
				}
			}
		}

		/// <summary>
		/// Occurs when the Shuffle has been altered.
		/// </summary>
		private void Profile_ShuffleChanged(object sender, ShuffleEventArgs e)
		{
			BuildTree();
		}

		/// <summary>
		/// Occurs when the Shuffle is changed from one to another
		/// </summary>
		private void Profile_ShuffleSwitched(object sender, ShuffleEventArgs e)
		{
			BuildTree();
		}

		private void Profile_ChannelsSelected(object sender, ChannelListEventArgs e)
		{
			if (SuppressEvents)
				return;

			SuppressEvents = true;

			ChannelTree.ClearSelectedNodes();
			
			// Find all the node that match up with the channels in the event args and toggle them selected
			CXTreeNode ChannelNode = null;
			List<CXTreeNode> Nodes = new List<CXTreeNode>();
			foreach (Channel Channel in e.Channels)
			{
				ChannelNode = FindTreeNode(Channel, ChannelTree.Nodes);
				if (ChannelNode != null)
					Nodes.Add(ChannelNode);
			}
			ChannelTree.SelectedNodes = Nodes;
			Nodes = null;
			ChannelNode = null;

			ChannelTree.Refresh();
			SuppressEvents = false;
		}

		/// <summary>
		/// Occurs when an UNDO or REDO has happened. Rebuild the tree to reflect any changes to the Channel. 
		/// Note this event should NOT fire when the Profile is switched.
		/// </summary>
		private void Undo_Completed(object sender, EventArgs e)
		{
			BuildShuffleList();
			BuildTree();
		}

		#endregion [ Profile Events ]

		#region [ Sort Order Control Events ]

		/// <summary>
		/// Changes the currently selected Shuffle.
		/// </summary>
		private void cboShuffles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;

			// Inform the current Profile that a new Shuffle is indicated.
			int SortID = Convert.ToInt32(_listBoxUtil.GetKey(cboShuffles));

			Profile.Channels.ShuffleController.Set(SortID);
			SetShuffleControls();
			Profile.SaveUndo("Change Sort Order");
		}

		/// <summary>
		/// Deletes the currently selected Shuffle.
		/// </summary>
		private void cmdDelete_Click(object sender, EventArgs e)
		{
			Editor.Channel_Shuffle_Delete_Click(sender, e);
		}

		/// <summary>
		/// Edits the currently selected Shuffle.
		/// </summary>
		private void cmdEdit_Click(object sender, EventArgs e)
		{
			Editor.Channel_Shuffle_Edit_Click(sender, e);
		}

		/// <summary>
		/// Adds a new Shuffle.
		/// </summary>
		private void cmdAdd_Click(object sender, EventArgs e)
		{
			Editor.Channel_Shuffle_New_Click(sender, e);
		}
		
		#endregion [ Sort Order Control Events ]

		private void tmrFocus_Tick(object sender, EventArgs e)
		{
			tmrFocus.Enabled = false;
			ChannelTree.Focus();
		}

		#endregion [ Event Delegates ]

		#region [ Event Handlers ]

		public EventHandler ChannelExplorerResized;

		#endregion [ Event Handlers ]

		#endregion [ Events ]
	
	}

}


