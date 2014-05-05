namespace ElfCore {
    partial class Editor {
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
        private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
			WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
			this.OpenImageFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SaveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.File_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.File_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_Undo = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_Redo = new System.Windows.Forms.ToolStripMenuItem();
			this.EditSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.Edit_Cut = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_Copy = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_Paste = new System.Windows.Forms.ToolStripMenuItem();
			this.GridMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.CellSize_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.GridSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.Grid_SetResolution = new System.Windows.Forms.ToolStripMenuItem();
			this.ChannelImage = new System.Windows.Forms.ToolStripMenuItem();
			this.Channel_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.Clear_SelectedChannels = new System.Windows.Forms.ToolStripMenuItem();
			this.Clear_AllChannels = new System.Windows.Forms.ToolStripMenuItem();
			this.Channel_AllVisible = new System.Windows.Forms.ToolStripMenuItem();
			this.Channel_SetColor = new System.Windows.Forms.ToolStripMenuItem();
			this.ChannelSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.Channel_LoadFromBitmap = new System.Windows.Forms.ToolStripMenuItem();
			this.Channel_SaveToBitmap = new System.Windows.Forms.ToolStripMenuItem();
			this.ChannelSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.Channel_Import = new System.Windows.Forms.ToolStripMenuItem();
			this.Channel_SetAsBackground = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImageMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImage_Load = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImage_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImage_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImage_Grid = new System.Windows.Forms.ToolStripMenuItem();
			this.BG_Sep1 = new System.Windows.Forms.ToolStripSeparator();
			this.BGImage_ResetSize = new System.Windows.Forms.ToolStripMenuItem();
			this.BGImage_Brightness = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.Settings_KeyConfig = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingsSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.Settings_RespectChannelOutputsDuringPlayback = new System.Windows.Forms.ToolStripMenuItem();
			this.Settings_ShowGrid = new System.Windows.Forms.ToolStripMenuItem();
			this.Settings_ShowGridLineWhilePainting = new System.Windows.Forms.ToolStripMenuItem();
			this.Settings_ShowRuler = new System.Windows.Forms.ToolStripMenuItem();
			this.PanesMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_Paint = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_MaskPixels = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_MaskCells = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_Canvas = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_ActiveChannel = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_ImageStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_Clipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.Pane_MoveChannel = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStrip_Edit = new System.Windows.Forms.ToolStrip();
			this.ToolBar_Undo = new System.Windows.Forms.ToolStripButton();
			this.ToolBar_Redo = new System.Windows.Forms.ToolStripButton();
			this.ToolBar_Sep1 = new System.Windows.Forms.ToolStripSeparator();
			this.ToolBar_Cut = new System.Windows.Forms.ToolStripButton();
			this.ToolBar_Copy = new System.Windows.Forms.ToolStripButton();
			this.ToolBar_Paste = new System.Windows.Forms.ToolStripButton();
			this.ToolBox_Main = new System.Windows.Forms.ToolStrip();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.tssChannels = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssMouseCoords = new System.Windows.Forms.ToolStripStatusLabel();
			this._tssCellSize = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssCellSize = new System.Windows.Forms.ToolStripStatusLabel();
			this._tssResolution = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssResolution = new System.Windows.Forms.ToolStripStatusLabel();
			this._tssTool = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssTool = new System.Windows.Forms.ToolStripStatusLabel();
			this._tssZoom = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssZoom = new System.Windows.Forms.ToolStripStatusLabel();
			this._tssChannel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssChannel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tmrFlyout = new System.Windows.Forms.Timer(this.components);
			this.ChannelColorDialog = new System.Windows.Forms.ColorDialog();
			this.OpenProfileDialog = new System.Windows.Forms.OpenFileDialog();
			this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			this.pnlToolStripDocker = new System.Windows.Forms.Panel();
			this.pnlHoldToolStrips = new System.Windows.Forms.Panel();
			this.ToolStrip_Blank = new System.Windows.Forms.ToolStrip();
			this.MainMenu.SuspendLayout();
			this.ToolStrip_Edit.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.pnlHoldToolStrips.SuspendLayout();
			this.SuspendLayout();
			// 
			// OpenImageFileDialog
			// 
			this.OpenImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File" +
    " (*.gif)|*.gif|All Files (*.*)|*.*";
			this.OpenImageFileDialog.FilterIndex = 5;
			// 
			// SaveImageFileDialog
			// 
			this.SaveImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File" +
    " (*.gif)|*.gif|All Files (*.*)|*.*";
			this.SaveImageFileDialog.FilterIndex = 5;
			// 
			// MainMenu
			// 
			this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.EditMenu,
            this.GridMenu,
            this.ChannelImage,
            this.BGImageMenu,
            this.SettingsMenu,
            this.PanesMenu});
			this.MainMenu.Location = new System.Drawing.Point(0, 0);
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new System.Drawing.Size(1167, 24);
			this.MainMenu.TabIndex = 0;
			this.MainMenu.Text = "menuStrip1";
			// 
			// FileMenu
			// 
			this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.File_Save,
            this.File_Exit});
			this.FileMenu.Name = "FileMenu";
			this.FileMenu.Size = new System.Drawing.Size(37, 20);
			this.FileMenu.Text = "&File";
			// 
			// File_Save
			// 
			this.File_Save.Image = global::ElfCore.Properties.Resources.save;
			this.File_Save.Name = "File_Save";
			this.File_Save.Size = new System.Drawing.Size(191, 22);
			this.File_Save.Text = "&Save Changes and Exit";
			this.File_Save.Click += new System.EventHandler(this.File_Save_Click);
			// 
			// File_Exit
			// 
			this.File_Exit.Image = global::ElfCore.Properties.Resources.not;
			this.File_Exit.Name = "File_Exit";
			this.File_Exit.Size = new System.Drawing.Size(191, 22);
			this.File_Exit.Text = "&Cancel Changes";
			this.File_Exit.Click += new System.EventHandler(this.File_Exit_Click);
			// 
			// EditMenu
			// 
			this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Edit_Undo,
            this.Edit_Redo,
            this.EditSep1,
            this.Edit_Cut,
            this.Edit_Copy,
            this.Edit_Paste});
			this.EditMenu.Name = "EditMenu";
			this.EditMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.EditMenu.Size = new System.Drawing.Size(39, 20);
			this.EditMenu.Text = "&Edit";
			this.EditMenu.MouseEnter += new System.EventHandler(this.EditMenu_MouseEnter);
			// 
			// Edit_Undo
			// 
			this.Edit_Undo.Enabled = false;
			this.Edit_Undo.Image = global::ElfCore.Properties.Resources.undo;
			this.Edit_Undo.Name = "Edit_Undo";
			this.Edit_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.Edit_Undo.Size = new System.Drawing.Size(174, 22);
			this.Edit_Undo.Text = "&Undo";
			this.Edit_Undo.Click += new System.EventHandler(this.Edit_Undo_Click);
			// 
			// Edit_Redo
			// 
			this.Edit_Redo.Enabled = false;
			this.Edit_Redo.Image = global::ElfCore.Properties.Resources.redo;
			this.Edit_Redo.Name = "Edit_Redo";
			this.Edit_Redo.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
			this.Edit_Redo.Size = new System.Drawing.Size(174, 22);
			this.Edit_Redo.Text = "&Redo";
			this.Edit_Redo.Click += new System.EventHandler(this.Edit_Redo_Click);
			// 
			// EditSep1
			// 
			this.EditSep1.Name = "EditSep1";
			this.EditSep1.Size = new System.Drawing.Size(171, 6);
			// 
			// Edit_Cut
			// 
			this.Edit_Cut.Enabled = false;
			this.Edit_Cut.Image = global::ElfCore.Properties.Resources.cut;
			this.Edit_Cut.Name = "Edit_Cut";
			this.Edit_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.Edit_Cut.Size = new System.Drawing.Size(174, 22);
			this.Edit_Cut.Text = "C&ut";
			this.Edit_Cut.Click += new System.EventHandler(this.Edit_Cut_Click);
			// 
			// Edit_Copy
			// 
			this.Edit_Copy.Enabled = false;
			this.Edit_Copy.Image = global::ElfCore.Properties.Resources.copy;
			this.Edit_Copy.Name = "Edit_Copy";
			this.Edit_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.Edit_Copy.Size = new System.Drawing.Size(174, 22);
			this.Edit_Copy.Text = "&Copy";
			this.Edit_Copy.Click += new System.EventHandler(this.Edit_Copy_Click);
			// 
			// Edit_Paste
			// 
			this.Edit_Paste.Enabled = false;
			this.Edit_Paste.Image = global::ElfCore.Properties.Resources.paste;
			this.Edit_Paste.Name = "Edit_Paste";
			this.Edit_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.Edit_Paste.Size = new System.Drawing.Size(174, 22);
			this.Edit_Paste.Text = "&Paste";
			this.Edit_Paste.Click += new System.EventHandler(this.Edit_Paste_Click);
			// 
			// GridMenu
			// 
			this.GridMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CellSize_1,
            this.CellSize_2,
            this.CellSize_3,
            this.CellSize_4,
            this.CellSize_5,
            this.CellSize_6,
            this.CellSize_7,
            this.CellSize_8,
            this.CellSize_9,
            this.CellSize_10,
            this.GridSep1,
            this.Grid_SetResolution});
			this.GridMenu.Name = "GridMenu";
			this.GridMenu.Size = new System.Drawing.Size(57, 20);
			this.GridMenu.Text = "C&anvas";
			// 
			// CellSize_1
			// 
			this.CellSize_1.Image = global::ElfCore.Properties.Resources.pixel_1;
			this.CellSize_1.Name = "CellSize_1";
			this.CellSize_1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D1)));
			this.CellSize_1.Size = new System.Drawing.Size(250, 22);
			this.CellSize_1.Tag = "1";
			this.CellSize_1.Text = "1x1 Pixel Per Cell";
			this.CellSize_1.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_2
			// 
			this.CellSize_2.Image = global::ElfCore.Properties.Resources.pixel_2;
			this.CellSize_2.Name = "CellSize_2";
			this.CellSize_2.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D2)));
			this.CellSize_2.Size = new System.Drawing.Size(250, 22);
			this.CellSize_2.Tag = "2";
			this.CellSize_2.Text = "2x2 Pixels Per Cell";
			this.CellSize_2.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_3
			// 
			this.CellSize_3.Image = global::ElfCore.Properties.Resources.pixel_3;
			this.CellSize_3.Name = "CellSize_3";
			this.CellSize_3.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D3)));
			this.CellSize_3.Size = new System.Drawing.Size(250, 22);
			this.CellSize_3.Tag = "3";
			this.CellSize_3.Text = "3x3 Pixels Per Cell";
			this.CellSize_3.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_4
			// 
			this.CellSize_4.Image = global::ElfCore.Properties.Resources.pixel_4;
			this.CellSize_4.Name = "CellSize_4";
			this.CellSize_4.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D4)));
			this.CellSize_4.Size = new System.Drawing.Size(250, 22);
			this.CellSize_4.Tag = "4";
			this.CellSize_4.Text = "4x4 Pixels Per Cell";
			this.CellSize_4.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_5
			// 
			this.CellSize_5.Image = global::ElfCore.Properties.Resources.pixel_5;
			this.CellSize_5.Name = "CellSize_5";
			this.CellSize_5.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D5)));
			this.CellSize_5.Size = new System.Drawing.Size(250, 22);
			this.CellSize_5.Tag = "5";
			this.CellSize_5.Text = "5x5 Pixels Per Cell";
			this.CellSize_5.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_6
			// 
			this.CellSize_6.Image = global::ElfCore.Properties.Resources.pixel_6;
			this.CellSize_6.Name = "CellSize_6";
			this.CellSize_6.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D6)));
			this.CellSize_6.Size = new System.Drawing.Size(250, 22);
			this.CellSize_6.Tag = "6";
			this.CellSize_6.Text = "6x6 Pixels Per Cell";
			this.CellSize_6.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_7
			// 
			this.CellSize_7.Image = global::ElfCore.Properties.Resources.pixel_7;
			this.CellSize_7.Name = "CellSize_7";
			this.CellSize_7.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D7)));
			this.CellSize_7.Size = new System.Drawing.Size(250, 22);
			this.CellSize_7.Tag = "7";
			this.CellSize_7.Text = "7x7 Pixels Per Cell";
			this.CellSize_7.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_8
			// 
			this.CellSize_8.Image = global::ElfCore.Properties.Resources.pixel_8;
			this.CellSize_8.Name = "CellSize_8";
			this.CellSize_8.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D8)));
			this.CellSize_8.Size = new System.Drawing.Size(250, 22);
			this.CellSize_8.Tag = "8";
			this.CellSize_8.Text = "8x8 Pixels Per Cell";
			this.CellSize_8.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_9
			// 
			this.CellSize_9.Image = global::ElfCore.Properties.Resources.pixel_9;
			this.CellSize_9.Name = "CellSize_9";
			this.CellSize_9.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D9)));
			this.CellSize_9.Size = new System.Drawing.Size(250, 22);
			this.CellSize_9.Tag = "9";
			this.CellSize_9.Text = "9x9 Pixels Per Cell";
			this.CellSize_9.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// CellSize_10
			// 
			this.CellSize_10.Image = global::ElfCore.Properties.Resources.pixel_10;
			this.CellSize_10.Name = "CellSize_10";
			this.CellSize_10.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D0)));
			this.CellSize_10.Size = new System.Drawing.Size(250, 22);
			this.CellSize_10.Tag = "10";
			this.CellSize_10.Text = "10x10 Pixels Per Cell";
			this.CellSize_10.Click += new System.EventHandler(this.CellSize_Click);
			// 
			// GridSep1
			// 
			this.GridSep1.Name = "GridSep1";
			this.GridSep1.Size = new System.Drawing.Size(247, 6);
			// 
			// Grid_SetResolution
			// 
			this.Grid_SetResolution.Image = global::ElfCore.Properties.Resources.set_canvas;
			this.Grid_SetResolution.Name = "Grid_SetResolution";
			this.Grid_SetResolution.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.Grid_SetResolution.Size = new System.Drawing.Size(250, 22);
			this.Grid_SetResolution.Text = "Set Canvas Resolution...";
			this.Grid_SetResolution.Click += new System.EventHandler(this.Grid_SetResolution_Click);
			// 
			// ChannelImage
			// 
			this.ChannelImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Channel_Clear,
            this.Channel_AllVisible,
            this.Channel_SetColor,
            this.ChannelSep1,
            this.Channel_LoadFromBitmap,
            this.Channel_SaveToBitmap,
            this.ChannelSep2,
            this.Channel_Import,
            this.Channel_SetAsBackground});
			this.ChannelImage.Name = "ChannelImage";
			this.ChannelImage.Size = new System.Drawing.Size(63, 20);
			this.ChannelImage.Text = "&Channel";
			// 
			// Channel_Clear
			// 
			this.Channel_Clear.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Clear_SelectedChannels,
            this.Clear_AllChannels});
			this.Channel_Clear.Image = global::ElfCore.Properties.Resources.not;
			this.Channel_Clear.Name = "Channel_Clear";
			this.Channel_Clear.Size = new System.Drawing.Size(304, 22);
			this.Channel_Clear.Text = "&Clear";
			this.Channel_Clear.Click += new System.EventHandler(this.Channel_Clear_Click);
			// 
			// Clear_SelectedChannels
			// 
			this.Clear_SelectedChannels.Image = global::ElfCore.Properties.Resources.clear_channel;
			this.Clear_SelectedChannels.Name = "Clear_SelectedChannels";
			this.Clear_SelectedChannels.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
			this.Clear_SelectedChannels.Size = new System.Drawing.Size(253, 22);
			this.Clear_SelectedChannels.Text = "Clear Current Channel";
			this.Clear_SelectedChannels.Click += new System.EventHandler(this.Channel_Clear_SelectedChannels_Click);
			// 
			// Clear_AllChannels
			// 
			this.Clear_AllChannels.Image = global::ElfCore.Properties.Resources.clear_multiple_channels;
			this.Clear_AllChannels.Name = "Clear_AllChannels";
			this.Clear_AllChannels.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Delete)));
			this.Clear_AllChannels.Size = new System.Drawing.Size(253, 22);
			this.Clear_AllChannels.Text = "&Clear All Channels";
			this.Clear_AllChannels.Click += new System.EventHandler(this.Channel_Clear_AllChannels_Click);
			// 
			// Channel_AllVisible
			// 
			this.Channel_AllVisible.Image = global::ElfCore.Properties.Resources.visible2;
			this.Channel_AllVisible.Name = "Channel_AllVisible";
			this.Channel_AllVisible.Size = new System.Drawing.Size(304, 22);
			this.Channel_AllVisible.Text = "Make All Channels &Visible";
			this.Channel_AllVisible.Click += new System.EventHandler(this.Channel_AllVisible_Click);
			// 
			// Channel_SetColor
			// 
			this.Channel_SetColor.Image = global::ElfCore.Properties.Resources.palette;
			this.Channel_SetColor.Name = "Channel_SetColor";
			this.Channel_SetColor.Size = new System.Drawing.Size(304, 22);
			this.Channel_SetColor.Text = "Set C&olor for Current Channel...";
			this.Channel_SetColor.Click += new System.EventHandler(this.Channel_SetColor_Click);
			// 
			// ChannelSep1
			// 
			this.ChannelSep1.Name = "ChannelSep1";
			this.ChannelSep1.Size = new System.Drawing.Size(301, 6);
			// 
			// Channel_LoadFromBitmap
			// 
			this.Channel_LoadFromBitmap.Image = global::ElfCore.Properties.Resources.image_import;
			this.Channel_LoadFromBitmap.Name = "Channel_LoadFromBitmap";
			this.Channel_LoadFromBitmap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.Channel_LoadFromBitmap.Size = new System.Drawing.Size(304, 22);
			this.Channel_LoadFromBitmap.Text = "&Load Channel Data from a Bitmap...";
			this.Channel_LoadFromBitmap.Click += new System.EventHandler(this.Channel_LoadFromBitmap_Click);
			// 
			// Channel_SaveToBitmap
			// 
			this.Channel_SaveToBitmap.Image = global::ElfCore.Properties.Resources.image_export;
			this.Channel_SaveToBitmap.Name = "Channel_SaveToBitmap";
			this.Channel_SaveToBitmap.Size = new System.Drawing.Size(304, 22);
			this.Channel_SaveToBitmap.Text = "&Save Current Channel to a Bitmap...";
			this.Channel_SaveToBitmap.Click += new System.EventHandler(this.Channel_SaveToBitmap_Click);
			// 
			// ChannelSep2
			// 
			this.ChannelSep2.Name = "ChannelSep2";
			this.ChannelSep2.Size = new System.Drawing.Size(301, 6);
			// 
			// Channel_Import
			// 
			this.Channel_Import.Image = global::ElfCore.Properties.Resources.import;
			this.Channel_Import.Name = "Channel_Import";
			this.Channel_Import.Size = new System.Drawing.Size(304, 22);
			this.Channel_Import.Text = "&Import Channels From Another Profile...";
			this.Channel_Import.Click += new System.EventHandler(this.Channel_Import_Click);
			// 
			// Channel_SetAsBackground
			// 
			this.Channel_SetAsBackground.Image = global::ElfCore.Properties.Resources.Channel2background;
			this.Channel_SetAsBackground.Name = "Channel_SetAsBackground";
			this.Channel_SetAsBackground.Size = new System.Drawing.Size(304, 22);
			this.Channel_SetAsBackground.Text = "S&et Channel As Background Image...";
			this.Channel_SetAsBackground.Click += new System.EventHandler(this.Channel_SetAsBackground_Click);
			// 
			// BGImageMenu
			// 
			this.BGImageMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BGImage_Load,
            this.BGImage_Save,
            this.BGImage_Clear,
            this.BGImage_Grid,
            this.BG_Sep1,
            this.BGImage_ResetSize,
            this.BGImage_Brightness});
			this.BGImageMenu.Name = "BGImageMenu";
			this.BGImageMenu.Size = new System.Drawing.Size(119, 20);
			this.BGImageMenu.Text = "&Background Image";
			this.BGImageMenu.MouseEnter += new System.EventHandler(this.BGImageMenu_MouseEnter);
			// 
			// BGImage_Load
			// 
			this.BGImage_Load.Image = global::ElfCore.Properties.Resources.image_import;
			this.BGImage_Load.Name = "BGImage_Load";
			this.BGImage_Load.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.BGImage_Load.Size = new System.Drawing.Size(295, 22);
			this.BGImage_Load.Text = "&Load Background Image...";
			this.BGImage_Load.Click += new System.EventHandler(this.BGImage_Load_Click);
			// 
			// BGImage_Save
			// 
			this.BGImage_Save.Image = global::ElfCore.Properties.Resources.image_export;
			this.BGImage_Save.Name = "BGImage_Save";
			this.BGImage_Save.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.BGImage_Save.Size = new System.Drawing.Size(295, 22);
			this.BGImage_Save.Text = "Sa&ve Background Image...";
			this.BGImage_Save.Click += new System.EventHandler(this.BGImage_Save_Click);
			// 
			// BGImage_Clear
			// 
			this.BGImage_Clear.Image = global::ElfCore.Properties.Resources.clear_background;
			this.BGImage_Clear.Name = "BGImage_Clear";
			this.BGImage_Clear.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
			this.BGImage_Clear.Size = new System.Drawing.Size(295, 22);
			this.BGImage_Clear.Text = "&Clear Background Image";
			this.BGImage_Clear.Click += new System.EventHandler(this.BGImage_Clear_Click);
			// 
			// BGImage_Grid
			// 
			this.BGImage_Grid.CheckOnClick = true;
			this.BGImage_Grid.Image = global::ElfCore.Properties.Resources.grid_background;
			this.BGImage_Grid.Name = "BGImage_Grid";
			this.BGImage_Grid.Size = new System.Drawing.Size(295, 22);
			this.BGImage_Grid.Text = "Superimpose Grid on Background Image";
			this.BGImage_Grid.Click += new System.EventHandler(this.BGImage_Grid_Click);
			// 
			// BG_Sep1
			// 
			this.BG_Sep1.Name = "BG_Sep1";
			this.BG_Sep1.Size = new System.Drawing.Size(292, 6);
			// 
			// BGImage_ResetSize
			// 
			this.BGImage_ResetSize.Image = global::ElfCore.Properties.Resources.image_resize;
			this.BGImage_ResetSize.Name = "BGImage_ResetSize";
			this.BGImage_ResetSize.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
			this.BGImage_ResetSize.Size = new System.Drawing.Size(295, 22);
			this.BGImage_ResetSize.Text = "&Size Canvas to Match Image";
			this.BGImage_ResetSize.Click += new System.EventHandler(this.BGImage_ResetSize_Click);
			// 
			// BGImage_Brightness
			// 
			this.BGImage_Brightness.Image = global::ElfCore.Properties.Resources.contrast;
			this.BGImage_Brightness.Name = "BGImage_Brightness";
			this.BGImage_Brightness.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.BGImage_Brightness.Size = new System.Drawing.Size(295, 22);
			this.BGImage_Brightness.Text = "Set Image &Brightness...";
			this.BGImage_Brightness.Click += new System.EventHandler(this.BGImage_Brightness_Click);
			// 
			// SettingsMenu
			// 
			this.SettingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Settings_KeyConfig,
            this.SettingsSep1,
            this.Settings_RespectChannelOutputsDuringPlayback,
            this.Settings_ShowGrid,
            this.Settings_ShowGridLineWhilePainting,
            this.Settings_ShowRuler});
			this.SettingsMenu.Name = "SettingsMenu";
			this.SettingsMenu.Size = new System.Drawing.Size(61, 20);
			this.SettingsMenu.Text = "&Settings";
			// 
			// Settings_KeyConfig
			// 
			this.Settings_KeyConfig.Image = global::ElfCore.Properties.Resources.keyboard;
			this.Settings_KeyConfig.Name = "Settings_KeyConfig";
			this.Settings_KeyConfig.Size = new System.Drawing.Size(296, 22);
			this.Settings_KeyConfig.Text = "&Key Configuration...";
			this.Settings_KeyConfig.Visible = false;
			this.Settings_KeyConfig.Click += new System.EventHandler(this.Settings_KeyConfig_Click);
			// 
			// SettingsSep1
			// 
			this.SettingsSep1.Name = "SettingsSep1";
			this.SettingsSep1.Size = new System.Drawing.Size(293, 6);
			this.SettingsSep1.Visible = false;
			// 
			// Settings_RespectChannelOutputsDuringPlayback
			// 
			this.Settings_RespectChannelOutputsDuringPlayback.Image = global::ElfCore.Properties.Resources.option;
			this.Settings_RespectChannelOutputsDuringPlayback.Name = "Settings_RespectChannelOutputsDuringPlayback";
			this.Settings_RespectChannelOutputsDuringPlayback.Size = new System.Drawing.Size(296, 22);
			this.Settings_RespectChannelOutputsDuringPlayback.Text = "&Respect Channel Outputs during Playback";
			this.Settings_RespectChannelOutputsDuringPlayback.Click += new System.EventHandler(this.Settings_RespectChannelOutputsDuringPlayback_Click);
			// 
			// Settings_ShowGrid
			// 
			this.Settings_ShowGrid.Image = global::ElfCore.Properties.Resources.option;
			this.Settings_ShowGrid.Name = "Settings_ShowGrid";
			this.Settings_ShowGrid.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
			this.Settings_ShowGrid.Size = new System.Drawing.Size(296, 22);
			this.Settings_ShowGrid.Text = "Show &Grid Lines";
			this.Settings_ShowGrid.Click += new System.EventHandler(this.Settings_ShowGrid_Click);
			// 
			// Settings_ShowGridLineWhilePainting
			// 
			this.Settings_ShowGridLineWhilePainting.Image = global::ElfCore.Properties.Resources.option;
			this.Settings_ShowGridLineWhilePainting.Name = "Settings_ShowGridLineWhilePainting";
			this.Settings_ShowGridLineWhilePainting.Size = new System.Drawing.Size(296, 22);
			this.Settings_ShowGridLineWhilePainting.Text = "Show Grid Lines &While Painting";
			this.Settings_ShowGridLineWhilePainting.Visible = false;
			this.Settings_ShowGridLineWhilePainting.Click += new System.EventHandler(this.Settings_PaintGridLines_Click);
			// 
			// Settings_ShowRuler
			// 
			this.Settings_ShowRuler.Image = global::ElfCore.Properties.Resources.option;
			this.Settings_ShowRuler.Name = "Settings_ShowRuler";
			this.Settings_ShowRuler.Size = new System.Drawing.Size(296, 22);
			this.Settings_ShowRuler.Text = "Show Ru&ler";
			this.Settings_ShowRuler.Click += new System.EventHandler(this.Settings_ShowRuler_Click);
			// 
			// PanesMenu
			// 
			this.PanesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Pane_Paint,
            this.Pane_MaskPixels,
            this.Pane_MaskCells,
            this.Pane_Canvas,
            this.Pane_ActiveChannel,
            this.Pane_ImageStamp,
            this.Pane_Clipboard,
            this.Pane_MoveChannel});
			this.PanesMenu.Name = "PanesMenu";
			this.PanesMenu.Size = new System.Drawing.Size(50, 20);
			this.PanesMenu.Text = "&Panes";
			// 
			// Pane_Paint
			// 
			this.Pane_Paint.Name = "Pane_Paint";
			this.Pane_Paint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
			this.Pane_Paint.Size = new System.Drawing.Size(194, 22);
			this.Pane_Paint.Text = "&Paint Buffer";
			this.Pane_Paint.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_MaskPixels
			// 
			this.Pane_MaskPixels.Name = "Pane_MaskPixels";
			this.Pane_MaskPixels.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
			this.Pane_MaskPixels.Size = new System.Drawing.Size(194, 22);
			this.Pane_MaskPixels.Text = "&Mask (Pixels)";
			this.Pane_MaskPixels.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_MaskCells
			// 
			this.Pane_MaskCells.Name = "Pane_MaskCells";
			this.Pane_MaskCells.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
			this.Pane_MaskCells.Size = new System.Drawing.Size(194, 22);
			this.Pane_MaskCells.Text = "Mask (Cells)";
			this.Pane_MaskCells.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_Canvas
			// 
			this.Pane_Canvas.Name = "Pane_Canvas";
			this.Pane_Canvas.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
			this.Pane_Canvas.Size = new System.Drawing.Size(194, 22);
			this.Pane_Canvas.Text = "&Canvas";
			this.Pane_Canvas.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_ActiveChannel
			// 
			this.Pane_ActiveChannel.Name = "Pane_ActiveChannel";
			this.Pane_ActiveChannel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
			this.Pane_ActiveChannel.Size = new System.Drawing.Size(194, 22);
			this.Pane_ActiveChannel.Text = "&Active Channel";
			this.Pane_ActiveChannel.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_ImageStamp
			// 
			this.Pane_ImageStamp.Name = "Pane_ImageStamp";
			this.Pane_ImageStamp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
			this.Pane_ImageStamp.Size = new System.Drawing.Size(194, 22);
			this.Pane_ImageStamp.Text = "&Image Stamp";
			this.Pane_ImageStamp.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_Clipboard
			// 
			this.Pane_Clipboard.Name = "Pane_Clipboard";
			this.Pane_Clipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
			this.Pane_Clipboard.Size = new System.Drawing.Size(194, 22);
			this.Pane_Clipboard.Text = "Cli&pboard";
			this.Pane_Clipboard.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// Pane_MoveChannel
			// 
			this.Pane_MoveChannel.Name = "Pane_MoveChannel";
			this.Pane_MoveChannel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D8)));
			this.Pane_MoveChannel.Size = new System.Drawing.Size(194, 22);
			this.Pane_MoveChannel.Text = "Mo&ve Channel";
			this.Pane_MoveChannel.Click += new System.EventHandler(this.ShowPane_Click);
			// 
			// ToolStrip_Edit
			// 
			this.ToolStrip_Edit.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolStrip_Edit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolBar_Undo,
            this.ToolBar_Redo,
            this.ToolBar_Sep1,
            this.ToolBar_Cut,
            this.ToolBar_Copy,
            this.ToolBar_Paste});
			this.ToolStrip_Edit.Location = new System.Drawing.Point(0, 24);
			this.ToolStrip_Edit.Name = "ToolStrip_Edit";
			this.ToolStrip_Edit.Size = new System.Drawing.Size(1167, 25);
			this.ToolStrip_Edit.TabIndex = 1;
			this.ToolStrip_Edit.Text = "ToolStrip_Edit";
			// 
			// ToolBar_Undo
			// 
			this.ToolBar_Undo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ToolBar_Undo.Enabled = false;
			this.ToolBar_Undo.Image = global::ElfCore.Properties.Resources.undo;
			this.ToolBar_Undo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ToolBar_Undo.Name = "ToolBar_Undo";
			this.ToolBar_Undo.Size = new System.Drawing.Size(23, 22);
			this.ToolBar_Undo.Text = "Reverses the last operation";
			this.ToolBar_Undo.Click += new System.EventHandler(this.Edit_Undo_Click);
			// 
			// ToolBar_Redo
			// 
			this.ToolBar_Redo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ToolBar_Redo.Enabled = false;
			this.ToolBar_Redo.Image = global::ElfCore.Properties.Resources.redo;
			this.ToolBar_Redo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ToolBar_Redo.Name = "ToolBar_Redo";
			this.ToolBar_Redo.Size = new System.Drawing.Size(23, 22);
			this.ToolBar_Redo.Text = "Redoes the last Undo operation";
			this.ToolBar_Redo.Click += new System.EventHandler(this.Edit_Redo_Click);
			// 
			// ToolBar_Sep1
			// 
			this.ToolBar_Sep1.Name = "ToolBar_Sep1";
			this.ToolBar_Sep1.Size = new System.Drawing.Size(6, 25);
			// 
			// ToolBar_Cut
			// 
			this.ToolBar_Cut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ToolBar_Cut.Enabled = false;
			this.ToolBar_Cut.Image = global::ElfCore.Properties.Resources.cut;
			this.ToolBar_Cut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ToolBar_Cut.Name = "ToolBar_Cut";
			this.ToolBar_Cut.Size = new System.Drawing.Size(23, 22);
			this.ToolBar_Cut.Text = "Cuts the selection and places it in the Clipboard (Ctrl+X)";
			this.ToolBar_Cut.Click += new System.EventHandler(this.Edit_Cut_Click);
			// 
			// ToolBar_Copy
			// 
			this.ToolBar_Copy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ToolBar_Copy.Enabled = false;
			this.ToolBar_Copy.Image = global::ElfCore.Properties.Resources.copy;
			this.ToolBar_Copy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ToolBar_Copy.Name = "ToolBar_Copy";
			this.ToolBar_Copy.Size = new System.Drawing.Size(23, 22);
			this.ToolBar_Copy.Text = "Copies the selection and places it in the Clipboard (Ctrl+C)";
			this.ToolBar_Copy.Click += new System.EventHandler(this.Edit_Copy_Click);
			// 
			// ToolBar_Paste
			// 
			this.ToolBar_Paste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ToolBar_Paste.Enabled = false;
			this.ToolBar_Paste.Image = global::ElfCore.Properties.Resources.paste;
			this.ToolBar_Paste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ToolBar_Paste.Name = "ToolBar_Paste";
			this.ToolBar_Paste.Size = new System.Drawing.Size(23, 22);
			this.ToolBar_Paste.Text = "Paste the cells from the clipbard into the current Channel";
			this.ToolBar_Paste.Click += new System.EventHandler(this.Edit_Paste_Click);
			// 
			// ToolBox_Main
			// 
			this.ToolBox_Main.AutoSize = false;
			this.ToolBox_Main.Dock = System.Windows.Forms.DockStyle.Left;
			this.ToolBox_Main.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolBox_Main.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this.ToolBox_Main.Location = new System.Drawing.Point(0, 49);
			this.ToolBox_Main.Name = "ToolBox_Main";
			this.ToolBox_Main.Padding = new System.Windows.Forms.Padding(0, 2, 1, 0);
			this.ToolBox_Main.Size = new System.Drawing.Size(32, 564);
			this.ToolBox_Main.TabIndex = 3;
			this.ToolBox_Main.TabStop = true;
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssChannels,
            this.tssMouseCoords,
            this._tssCellSize,
            this.tssCellSize,
            this._tssResolution,
            this.tssResolution,
            this._tssTool,
            this.tssTool,
            this._tssZoom,
            this.tssZoom,
            this._tssChannel,
            this.tssChannel});
			this.StatusBar.Location = new System.Drawing.Point(0, 613);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new System.Drawing.Size(1167, 22);
			this.StatusBar.TabIndex = 7;
			this.StatusBar.Text = "statusStrip1";
			// 
			// tssChannels
			// 
			this.tssChannels.Name = "tssChannels";
			this.tssChannels.Size = new System.Drawing.Size(463, 17);
			this.tssChannels.Spring = true;
			this.tssChannels.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssMouseCoords
			// 
			this.tssMouseCoords.BackColor = System.Drawing.SystemColors.Control;
			this.tssMouseCoords.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssMouseCoords.Name = "tssMouseCoords";
			this.tssMouseCoords.Size = new System.Drawing.Size(32, 17);
			this.tssMouseCoords.Text = "{0,0}";
			// 
			// _tssCellSize
			// 
			this._tssCellSize.BackColor = System.Drawing.SystemColors.Control;
			this._tssCellSize.Name = "_tssCellSize";
			this._tssCellSize.Size = new System.Drawing.Size(53, 17);
			this._tssCellSize.Text = "Cell Size:";
			// 
			// tssCellSize
			// 
			this.tssCellSize.BackColor = System.Drawing.SystemColors.Control;
			this.tssCellSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssCellSize.Margin = new System.Windows.Forms.Padding(0, 3, 5, 2);
			this.tssCellSize.Name = "tssCellSize";
			this.tssCellSize.Size = new System.Drawing.Size(58, 17);
			this.tssCellSize.Text = "1x1 Pixel";
			// 
			// _tssResolution
			// 
			this._tssResolution.BackColor = System.Drawing.SystemColors.Control;
			this._tssResolution.Name = "_tssResolution";
			this._tssResolution.Size = new System.Drawing.Size(66, 17);
			this._tssResolution.Text = "Resolution:";
			// 
			// tssResolution
			// 
			this.tssResolution.BackColor = System.Drawing.SystemColors.Control;
			this.tssResolution.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssResolution.Margin = new System.Windows.Forms.Padding(0, 3, 5, 2);
			this.tssResolution.Name = "tssResolution";
			this.tssResolution.Size = new System.Drawing.Size(42, 17);
			this.tssResolution.Text = "64x32";
			// 
			// _tssTool
			// 
			this._tssTool.BackColor = System.Drawing.SystemColors.Control;
			this._tssTool.Name = "_tssTool";
			this._tssTool.Size = new System.Drawing.Size(77, 17);
			this._tssTool.Text = "Current Tool:";
			// 
			// tssTool
			// 
			this.tssTool.BackColor = System.Drawing.SystemColors.Control;
			this.tssTool.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssTool.Margin = new System.Windows.Forms.Padding(0, 3, 5, 2);
			this.tssTool.Name = "tssTool";
			this.tssTool.Size = new System.Drawing.Size(66, 17);
			this.tssTool.Text = "Paintbrush";
			// 
			// _tssZoom
			// 
			this._tssZoom.BackColor = System.Drawing.SystemColors.Control;
			this._tssZoom.Name = "_tssZoom";
			this._tssZoom.Size = new System.Drawing.Size(42, 17);
			this._tssZoom.Text = "Zoom:";
			// 
			// tssZoom
			// 
			this.tssZoom.BackColor = System.Drawing.SystemColors.Control;
			this.tssZoom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssZoom.Margin = new System.Windows.Forms.Padding(0, 3, 5, 2);
			this.tssZoom.Name = "tssZoom";
			this.tssZoom.Size = new System.Drawing.Size(38, 17);
			this.tssZoom.Text = "100%";
			// 
			// _tssChannel
			// 
			this._tssChannel.BackColor = System.Drawing.SystemColors.Control;
			this._tssChannel.Name = "_tssChannel";
			this._tssChannel.Size = new System.Drawing.Size(90, 17);
			this._tssChannel.Text = "Active Channel:";
			// 
			// tssChannel
			// 
			this.tssChannel.BackColor = System.Drawing.SystemColors.Control;
			this.tssChannel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tssChannel.Image = global::ElfCore.Properties.Resources.sample_swatch;
			this.tssChannel.Name = "tssChannel";
			this.tssChannel.Size = new System.Drawing.Size(105, 17);
			this.tssChannel.Text = "None Selected";
			// 
			// tmrFlyout
			// 
			this.tmrFlyout.Interval = 200;
			this.tmrFlyout.Tick += new System.EventHandler(this.tmrFlyout_Tick);
			// 
			// OpenProfileDialog
			// 
			this.OpenProfileDialog.Filter = "Profile File (*.pro)|*.pro|All Files (*.*)|*.*";
			this.OpenProfileDialog.FilterIndex = 0;
			this.OpenProfileDialog.Title = "Select Profile to Import";
			// 
			// DockPanel
			// 
			this.DockPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DockPanel.Location = new System.Drawing.Point(32, 49);
			this.DockPanel.Name = "DockPanel";
			this.DockPanel.Size = new System.Drawing.Size(1135, 564);
			dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
			autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
			tabGradient1.EndColor = System.Drawing.SystemColors.Control;
			tabGradient1.StartColor = System.Drawing.SystemColors.Control;
			tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			autoHideStripSkin1.TabGradient = tabGradient1;
			autoHideStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
			dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
			tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
			dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
			dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
			dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
			tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
			tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
			tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
			dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
			dockPaneStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
			tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
			tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
			tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
			dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
			tabGradient5.EndColor = System.Drawing.SystemColors.Control;
			tabGradient5.StartColor = System.Drawing.SystemColors.Control;
			tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
			dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
			dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
			tabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption;
			tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
			tabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
			dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
			tabGradient7.EndColor = System.Drawing.Color.Transparent;
			tabGradient7.StartColor = System.Drawing.Color.Transparent;
			tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
			dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
			dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
			this.DockPanel.Skin = dockPanelSkin1;
			this.DockPanel.TabIndex = 7;
			// 
			// pnlToolStripDocker
			// 
			this.pnlToolStripDocker.Location = new System.Drawing.Point(128, 24);
			this.pnlToolStripDocker.Name = "pnlToolStripDocker";
			this.pnlToolStripDocker.Size = new System.Drawing.Size(1039, 25);
			this.pnlToolStripDocker.TabIndex = 2;
			// 
			// pnlHoldToolStrips
			// 
			this.pnlHoldToolStrips.Controls.Add(this.ToolStrip_Blank);
			this.pnlHoldToolStrips.Location = new System.Drawing.Point(217, 65);
			this.pnlHoldToolStrips.Name = "pnlHoldToolStrips";
			this.pnlHoldToolStrips.Size = new System.Drawing.Size(536, 202);
			this.pnlHoldToolStrips.TabIndex = 8;
			this.pnlHoldToolStrips.Visible = false;
			// 
			// ToolStrip_Blank
			// 
			this.ToolStrip_Blank.Location = new System.Drawing.Point(0, 0);
			this.ToolStrip_Blank.Name = "ToolStrip_Blank";
			this.ToolStrip_Blank.Size = new System.Drawing.Size(536, 25);
			this.ToolStrip_Blank.TabIndex = 13;
			// 
			// Editor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1167, 635);
			this.Controls.Add(this.pnlHoldToolStrips);
			this.Controls.Add(this.pnlToolStripDocker);
			this.Controls.Add(this.DockPanel);
			this.Controls.Add(this.ToolBox_Main);
			this.Controls.Add(this.ToolStrip_Edit);
			this.Controls.Add(this.MainMenu);
			this.Controls.Add(this.StatusBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(20, 73);
			this.Name = "Editor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Setup for Sequence Preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.Load += new System.EventHandler(this.Form_Load);
			this.Shown += new System.EventHandler(this.Form_Shown);
			this.ResizeBegin += new System.EventHandler(this.Form_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.Form_ResizeEnd);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
			this.Resize += new System.EventHandler(this.Form_Resize);
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.ToolStrip_Edit.ResumeLayout(false);
			this.ToolStrip_Edit.PerformLayout();
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.pnlHoldToolStrips.ResumeLayout(false);
			this.pnlHoldToolStrips.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.OpenFileDialog OpenImageFileDialog;
		private System.Windows.Forms.SaveFileDialog SaveImageFileDialog;
		private WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
		private System.Windows.Forms.MenuStrip MainMenu;
		private System.Windows.Forms.ToolStripMenuItem FileMenu;
		private System.Windows.Forms.ToolStripMenuItem File_Save;
		private System.Windows.Forms.ToolStripMenuItem File_Exit;
		private System.Windows.Forms.ToolStripMenuItem EditMenu;
		private System.Windows.Forms.ToolStripMenuItem Edit_Undo;
		private System.Windows.Forms.ToolStripMenuItem Edit_Redo;
		private System.Windows.Forms.ToolStripSeparator EditSep1;
		private System.Windows.Forms.ToolStripMenuItem Edit_Cut;
		private System.Windows.Forms.ToolStripMenuItem Edit_Copy;
		private System.Windows.Forms.ToolStripMenuItem Edit_Paste;
		private System.Windows.Forms.ToolStripMenuItem GridMenu;
		private System.Windows.Forms.ToolStripMenuItem CellSize_1;
		private System.Windows.Forms.ToolStripMenuItem CellSize_2;
		private System.Windows.Forms.ToolStripMenuItem CellSize_3;
		private System.Windows.Forms.ToolStripMenuItem CellSize_4;
		private System.Windows.Forms.ToolStripMenuItem CellSize_5;
		private System.Windows.Forms.ToolStripMenuItem CellSize_6;
		private System.Windows.Forms.ToolStripMenuItem CellSize_7;
		private System.Windows.Forms.ToolStripMenuItem CellSize_8;
		private System.Windows.Forms.ToolStripMenuItem CellSize_9;
		private System.Windows.Forms.ToolStripMenuItem CellSize_10;
		private System.Windows.Forms.ToolStripMenuItem BGImageMenu;
		private System.Windows.Forms.ToolStripMenuItem BGImage_Load;
		private System.Windows.Forms.ToolStripMenuItem BGImage_Clear;
		private System.Windows.Forms.ToolStripMenuItem BGImage_ResetSize;
		private System.Windows.Forms.ToolStripMenuItem ChannelImage;
		private System.Windows.Forms.ToolStripMenuItem Channel_SetColor;
		private System.Windows.Forms.ToolStripMenuItem Channel_LoadFromBitmap;
		private System.Windows.Forms.ToolStripMenuItem Channel_SaveToBitmap;
		private System.Windows.Forms.ToolStrip ToolStrip_Edit;
		private System.Windows.Forms.ToolStripButton ToolBar_Undo;
		private System.Windows.Forms.ToolStripButton ToolBar_Redo;
		private System.Windows.Forms.ToolStripSeparator ToolBar_Sep1;
		private System.Windows.Forms.ToolStripButton ToolBar_Cut;
		private System.Windows.Forms.ToolStripButton ToolBar_Copy;
		private System.Windows.Forms.ToolStripButton ToolBar_Paste;
		private System.Windows.Forms.ToolStrip ToolBox_Main;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.ToolStripStatusLabel _tssChannel;
		private System.Windows.Forms.ToolStripStatusLabel tssChannel;
		private System.Windows.Forms.ToolStripSeparator GridSep1;
		private System.Windows.Forms.ToolStripMenuItem Settings_ShowGrid;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripSeparator ChannelSep1;
		private System.Windows.Forms.ToolStripMenuItem Channel_Clear;
		private System.Windows.Forms.ToolStripMenuItem Clear_SelectedChannels;
		private System.Windows.Forms.ToolStripMenuItem Clear_AllChannels;
		private System.Windows.Forms.ToolStripMenuItem SettingsMenu;
		private System.Windows.Forms.ToolStripMenuItem Settings_KeyConfig;
		private System.Windows.Forms.ToolStripStatusLabel tssChannels;
		private System.Windows.Forms.ToolStripStatusLabel _tssTool;
		private System.Windows.Forms.ToolStripStatusLabel tssTool;
		private System.Windows.Forms.ToolStripStatusLabel _tssZoom;
		private System.Windows.Forms.ToolStripStatusLabel tssZoom;
		private System.Windows.Forms.ToolStripStatusLabel _tssResolution;
		private System.Windows.Forms.ToolStripStatusLabel tssResolution;
		private System.Windows.Forms.ToolStripMenuItem Grid_SetResolution;
		private System.Windows.Forms.Timer tmrFlyout;
		private System.Windows.Forms.ToolStripMenuItem BGImage_Brightness;
		private System.Windows.Forms.ToolStripStatusLabel _tssCellSize;
		private System.Windows.Forms.ToolStripStatusLabel tssCellSize;
		private System.Windows.Forms.ToolStripStatusLabel tssMouseCoords;
		private System.Windows.Forms.ToolStripMenuItem BGImage_Save;
		private System.Windows.Forms.ToolStripSeparator BG_Sep1;
		private System.Windows.Forms.ToolStripMenuItem Channel_AllVisible;
		private System.Windows.Forms.ColorDialog ChannelColorDialog;
		private System.Windows.Forms.ToolStripMenuItem PanesMenu;
		private System.Windows.Forms.ToolStripMenuItem Pane_Paint;
		private System.Windows.Forms.ToolStripMenuItem Pane_MaskPixels;
		private System.Windows.Forms.ToolStripMenuItem Pane_Canvas;
		private System.Windows.Forms.ToolStripMenuItem Pane_ActiveChannel;
		private System.Windows.Forms.ToolStripMenuItem Pane_ImageStamp;
		private System.Windows.Forms.OpenFileDialog OpenProfileDialog;
		private System.Windows.Forms.ToolStripMenuItem Channel_Import;
		private System.Windows.Forms.ToolStripSeparator ChannelSep2;
		private System.Windows.Forms.ToolStripSeparator SettingsSep1;
		private System.Windows.Forms.ToolStripMenuItem Settings_RespectChannelOutputsDuringPlayback;
		private System.Windows.Forms.ToolStripMenuItem Settings_ShowGridLineWhilePainting;
		private System.Windows.Forms.ToolStripMenuItem Pane_Clipboard;
		private System.Windows.Forms.ToolStripMenuItem Pane_MoveChannel;
		private System.Windows.Forms.ToolStripMenuItem Channel_SetAsBackground;
		private System.Windows.Forms.ToolStripMenuItem Settings_ShowRuler;
		private System.Windows.Forms.Panel pnlToolStripDocker;
		private System.Windows.Forms.Panel pnlHoldToolStrips;
		private System.Windows.Forms.ToolStrip ToolStrip_Blank;
		private System.Windows.Forms.ToolStripMenuItem Pane_MaskCells;
		private System.Windows.Forms.ToolStripMenuItem BGImage_Grid;

    }
}