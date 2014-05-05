namespace ElfCore
{
	partial class ChannelExplorerTree
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelExplorerTree));
			this.ChannelExplorer_ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.Context_Sep_Lock = new System.Windows.Forms.ToolStripSeparator();
			this.Context_Sep_Clear = new System.Windows.Forms.ToolStripSeparator();
			this.Context_Sep_Visible = new System.Windows.Forms.ToolStripSeparator();
			this.Context_Sep_Properties = new System.Windows.Forms.ToolStripSeparator();
			this.ToolStrip = new System.Windows.Forms.ToolStrip();
			this.ChannelTree = new MultiSelectTreeview();
			this.ilstIcons = new System.Windows.Forms.ImageList(this.components);
			this.ilstState = new System.Windows.Forms.ImageList(this.components);
			this.tmrFocus = new System.Windows.Forms.Timer(this.components);
			this.ChannelColorDialog = new System.Windows.Forms.ColorDialog();
			this.Context_Group = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Ungroup = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Lock = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Unlock = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Hide = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_HideAll = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Show = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_ShowAll = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_ChangeColor = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_Rename = new System.Windows.Forms.ToolStripMenuItem();
			this.TB_Group = new System.Windows.Forms.ToolStripButton();
			this.Context_Sep_MoveChannels = new System.Windows.Forms.ToolStripSeparator();
			this.Context_MoveUp = new System.Windows.Forms.ToolStripMenuItem();
			this.Context_MoveDown = new System.Windows.Forms.ToolStripMenuItem();
			this.ChannelExplorer_ContextMenu.SuspendLayout();
			this.ToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChannelExplorer_ContextMenu
			// 
			this.ChannelExplorer_ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_Group,
            this.Context_Ungroup,
            this.Context_Sep_Lock,
            this.Context_Lock,
            this.Context_Unlock,
            this.Context_Sep_Clear,
            this.Context_Clear,
            this.Context_Sep_Visible,
            this.Context_Hide,
            this.Context_HideAll,
            this.Context_Show,
            this.Context_ShowAll,
            this.Context_Sep_Properties,
            this.Context_ChangeColor,
            this.Context_Rename,
            this.Context_Sep_MoveChannels,
            this.Context_MoveUp,
            this.Context_MoveDown});
			this.ChannelExplorer_ContextMenu.Name = "Channel_ContextMenu";
			this.ChannelExplorer_ContextMenu.Size = new System.Drawing.Size(239, 342);
			this.ChannelExplorer_ContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ChannelExplorer_ContextMenu_Opening);
			// 
			// Context_Sep_Lock
			// 
			this.Context_Sep_Lock.Name = "Context_Sep_Lock";
			this.Context_Sep_Lock.Size = new System.Drawing.Size(235, 6);
			this.Context_Sep_Lock.Visible = false;
			// 
			// Context_Sep_Clear
			// 
			this.Context_Sep_Clear.Name = "Context_Sep_Clear";
			this.Context_Sep_Clear.Size = new System.Drawing.Size(235, 6);
			// 
			// Context_Sep_Visible
			// 
			this.Context_Sep_Visible.Name = "Context_Sep_Visible";
			this.Context_Sep_Visible.Size = new System.Drawing.Size(235, 6);
			// 
			// Context_Sep_Properties
			// 
			this.Context_Sep_Properties.Name = "Context_Sep_Properties";
			this.Context_Sep_Properties.Size = new System.Drawing.Size(235, 6);
			// 
			// ToolStrip
			// 
			this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TB_Group});
			this.ToolStrip.Location = new System.Drawing.Point(5, 5);
			this.ToolStrip.Name = "ToolStrip";
			this.ToolStrip.Size = new System.Drawing.Size(235, 25);
			this.ToolStrip.TabIndex = 1;
			this.ToolStrip.Text = "toolStrip1";
			this.ToolStrip.Visible = false;
			// 
			// ChannelTree
			// 
			this.ChannelTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ChannelTree.ContextMenuStrip = this.ChannelExplorer_ContextMenu;
			this.ChannelTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChannelTree.FullRowSelect = true;
			this.ChannelTree.HideSelection = false;
			this.ChannelTree.ImageIndex = 0;
			this.ChannelTree.ImageList = this.ilstIcons;
			this.ChannelTree.LabelEdit = true;
			this.ChannelTree.LineColor = System.Drawing.Color.SaddleBrown;
			this.ChannelTree.Location = new System.Drawing.Point(5, 5);
			this.ChannelTree.Name = "ChannelTree";
			this.ChannelTree.SelectedImageIndex = 0;
			this.ChannelTree.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("ChannelTree.SelectedNodes")));
			this.ChannelTree.ShowRootLines = false;
			this.ChannelTree.Size = new System.Drawing.Size(235, 312);
			this.ChannelTree.StateImageList = this.ilstState;
			this.ChannelTree.TabIndex = 0;
			this.ChannelTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ChannelTree_AfterLabelEdit);
			this.ChannelTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChannelTree_AfterSelect);
			this.ChannelTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChannelTree_MouseDown);
			// 
			// ilstIcons
			// 
			this.ilstIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilstIcons.ImageStream")));
			this.ilstIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.ilstIcons.Images.SetKeyName(0, "channel_group.png");
			// 
			// ilstState
			// 
			this.ilstState.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.ilstState.ImageSize = new System.Drawing.Size(8, 10);
			this.ilstState.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tmrFocus
			// 
			this.tmrFocus.Enabled = true;
			this.tmrFocus.Interval = 500;
			this.tmrFocus.Tick += new System.EventHandler(this.tmrFocus_Tick);
			// 
			// Context_Group
			// 
			this.Context_Group.Image = global::ElfCore.Properties.Resources.group;
			this.Context_Group.Name = "Context_Group";
			this.Context_Group.Size = new System.Drawing.Size(238, 22);
			this.Context_Group.Text = "&Group";
			this.Context_Group.Visible = false;
			this.Context_Group.Click += new System.EventHandler(this.Context_Group_Click);
			// 
			// Context_Ungroup
			// 
			this.Context_Ungroup.Image = global::ElfCore.Properties.Resources.ungroup;
			this.Context_Ungroup.Name = "Context_Ungroup";
			this.Context_Ungroup.Size = new System.Drawing.Size(238, 22);
			this.Context_Ungroup.Text = "&Ungroup";
			this.Context_Ungroup.Visible = false;
			this.Context_Ungroup.Click += new System.EventHandler(this.Context_Ungroup_Click);
			// 
			// Context_Lock
			// 
			this.Context_Lock.Image = global::ElfCore.Properties.Resources._lock;
			this.Context_Lock.Name = "Context_Lock";
			this.Context_Lock.Size = new System.Drawing.Size(238, 22);
			this.Context_Lock.Text = "&Lock";
			this.Context_Lock.Click += new System.EventHandler(this.Context_Lock_Click);
			// 
			// Context_Unlock
			// 
			this.Context_Unlock.Image = global::ElfCore.Properties.Resources.unlock;
			this.Context_Unlock.Name = "Context_Unlock";
			this.Context_Unlock.Size = new System.Drawing.Size(238, 22);
			this.Context_Unlock.Text = "U&nlock";
			this.Context_Unlock.Click += new System.EventHandler(this.Context_Unlock_Click);
			// 
			// Context_Clear
			// 
			this.Context_Clear.Image = global::ElfCore.Properties.Resources.clear_channel;
			this.Context_Clear.Name = "Context_Clear";
			this.Context_Clear.Size = new System.Drawing.Size(238, 22);
			this.Context_Clear.Text = "&Clear Cells";
			this.Context_Clear.Click += new System.EventHandler(this.Context_Clear_Click);
			// 
			// Context_Hide
			// 
			this.Context_Hide.Image = global::ElfCore.Properties.Resources.notvisible;
			this.Context_Hide.Name = "Context_Hide";
			this.Context_Hide.Size = new System.Drawing.Size(238, 22);
			this.Context_Hide.Text = "&Hide";
			this.Context_Hide.Click += new System.EventHandler(this.Context_Hide_Click);
			// 
			// Context_HideAll
			// 
			this.Context_HideAll.Image = global::ElfCore.Properties.Resources.notvisible;
			this.Context_HideAll.Name = "Context_HideAll";
			this.Context_HideAll.Size = new System.Drawing.Size(238, 22);
			this.Context_HideAll.Text = "Hide All &Not Selected Channels";
			this.Context_HideAll.Click += new System.EventHandler(this.Context_HideAllNonSelected_Click);
			// 
			// Context_Show
			// 
			this.Context_Show.Image = global::ElfCore.Properties.Resources.visible2;
			this.Context_Show.Name = "Context_Show";
			this.Context_Show.Size = new System.Drawing.Size(238, 22);
			this.Context_Show.Text = "&Show";
			this.Context_Show.Click += new System.EventHandler(this.Context_Show_Click);
			// 
			// Context_ShowAll
			// 
			this.Context_ShowAll.Image = global::ElfCore.Properties.Resources.visible2;
			this.Context_ShowAll.Name = "Context_ShowAll";
			this.Context_ShowAll.Size = new System.Drawing.Size(238, 22);
			this.Context_ShowAll.Text = "Show &All";
			this.Context_ShowAll.Click += new System.EventHandler(this.Context_ShowAll_Click);
			// 
			// Context_ChangeColor
			// 
			this.Context_ChangeColor.Image = global::ElfCore.Properties.Resources.palette;
			this.Context_ChangeColor.Name = "Context_ChangeColor";
			this.Context_ChangeColor.Size = new System.Drawing.Size(238, 22);
			this.Context_ChangeColor.Text = "Change C&olor...";
			this.Context_ChangeColor.Click += new System.EventHandler(this.Context_ChangeColor_Click);
			// 
			// Context_Rename
			// 
			this.Context_Rename.Image = global::ElfCore.Properties.Resources.editText;
			this.Context_Rename.Name = "Context_Rename";
			this.Context_Rename.Size = new System.Drawing.Size(238, 22);
			this.Context_Rename.Text = "&Rename";
			this.Context_Rename.Click += new System.EventHandler(this.Context_Rename_Click);
			// 
			// TB_Group
			// 
			this.TB_Group.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.TB_Group.Image = global::ElfCore.Properties.Resources.group;
			this.TB_Group.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TB_Group.Name = "TB_Group";
			this.TB_Group.Size = new System.Drawing.Size(23, 22);
			this.TB_Group.Text = "toolStripButton1";
			// 
			// Context_Sep_MoveChannels
			// 
			this.Context_Sep_MoveChannels.Name = "Context_Sep_MoveChannels";
			this.Context_Sep_MoveChannels.Size = new System.Drawing.Size(235, 6);
			this.Context_Sep_MoveChannels.Visible = false;
			// 
			// Context_MoveUp
			// 
			this.Context_MoveUp.Image = global::ElfCore.Properties.Resources.arrow_up;
			this.Context_MoveUp.Name = "Context_MoveUp";
			this.Context_MoveUp.Size = new System.Drawing.Size(238, 22);
			this.Context_MoveUp.Text = "Move U&p";
			this.Context_MoveUp.Visible = false;
			this.Context_MoveUp.Click += new System.EventHandler(this.Context_MoveUp_Click);
			// 
			// Context_MoveDown
			// 
			this.Context_MoveDown.Image = global::ElfCore.Properties.Resources.arrow_down;
			this.Context_MoveDown.Name = "Context_MoveDown";
			this.Context_MoveDown.Size = new System.Drawing.Size(238, 22);
			this.Context_MoveDown.Text = "Move &Down";
			this.Context_MoveDown.Visible = false;
			this.Context_MoveDown.Click += new System.EventHandler(this.Context_MoveDown_Click);
			// 
			// ChannelExplorerTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(245, 322);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.ControlBox = false;
			this.Controls.Add(this.ChannelTree);
			this.Controls.Add(this.ToolStrip);
			this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HideOnClose = true;
			this.Name = "ChannelExplorerTree";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.TabText = "Channel Explorer";
			this.Text = "Channel Explorer";
			this.DockStateChanged += new System.EventHandler(this.Form_DockStateChanged);
			this.Shown += new System.EventHandler(this.Form_Shown);
			this.SizeChanged += new System.EventHandler(this.Form_SizeChanged);
			this.ChannelExplorer_ContextMenu.ResumeLayout(false);
			this.ToolStrip.ResumeLayout(false);
			this.ToolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip ChannelExplorer_ContextMenu;
		private System.Windows.Forms.ToolStripMenuItem Context_Group;
		private System.Windows.Forms.ToolStripMenuItem Context_Ungroup;
		private System.Windows.Forms.ToolStrip ToolStrip;
		private System.Windows.Forms.ToolStripButton TB_Group;
		private System.Windows.Forms.ToolStripSeparator Context_Sep_Lock;
		private System.Windows.Forms.ToolStripMenuItem Context_Clear;
		private System.Windows.Forms.ToolStripSeparator Context_Sep_Visible;
		private System.Windows.Forms.ToolStripMenuItem Context_Hide;
		private System.Windows.Forms.ToolStripMenuItem Context_Show;
		private System.Windows.Forms.ToolStripMenuItem Context_ShowAll;
		private System.Windows.Forms.ToolStripMenuItem Context_HideAll;
		private MultiSelectTreeview ChannelTree;
		private System.Windows.Forms.ImageList ilstState;
		private System.Windows.Forms.ImageList ilstIcons;
		private System.Windows.Forms.Timer tmrFocus;
		private System.Windows.Forms.ToolStripSeparator Context_Sep_Properties;
		private System.Windows.Forms.ToolStripMenuItem Context_ChangeColor;
		private System.Windows.Forms.ColorDialog ChannelColorDialog;
		private System.Windows.Forms.ToolStripMenuItem Context_Rename;
		private System.Windows.Forms.ToolStripMenuItem Context_Lock;
		private System.Windows.Forms.ToolStripMenuItem Context_Unlock;
		private System.Windows.Forms.ToolStripSeparator Context_Sep_Clear;
		private System.Windows.Forms.ToolStripSeparator Context_Sep_MoveChannels;
		private System.Windows.Forms.ToolStripMenuItem Context_MoveUp;
		private System.Windows.Forms.ToolStripMenuItem Context_MoveDown;
	}
}