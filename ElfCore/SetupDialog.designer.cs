namespace AdjustablePreview {
    partial class SetupDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            if (m_originalBackground != null) {
                m_originalBackground.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelChannel = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripTextBoxResolutionX = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripTextBoxResolutionY = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripComboBoxPixelSize = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripDropDownButtonUpdate = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonResetSize = new System.Windows.Forms.ToolStripButton();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.toolStripComboBoxChannels = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripDropDownButtonClear = new System.Windows.Forms.ToolStripDropDownButton();
			this.allChannelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectedChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonLoadImage = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonClearImage = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSaveImage = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonReorder = new System.Windows.Forms.ToolStripButton();
			this.pictureBoxSetupGrid = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cmdNewUI = new System.Windows.Forms.Button();
			this.checkBoxRedirectOutputs = new System.Windows.Forms.CheckBox();
			this.trackBarBrightness = new System.Windows.Forms.TrackBar();
			this.labelBrightness = new System.Windows.Forms.Label();
			this.panelPictureBoxContainer = new System.Windows.Forms.Panel();
			this.toolStrip1.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetupGrid)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
			this.panelPictureBoxContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.buttonOK.Location = new System.Drawing.Point(740, 32);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.No;
			this.buttonCancel.Location = new System.Drawing.Point(821, 32);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelChannel
			// 
			this.labelChannel.AutoSize = true;
			this.labelChannel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelChannel.Location = new System.Drawing.Point(5, 11);
			this.labelChannel.Name = "labelChannel";
			this.labelChannel.Size = new System.Drawing.Size(0, 13);
			this.labelChannel.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxResolutionX,
            this.toolStripLabel2,
            this.toolStripTextBoxResolutionY,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.toolStripComboBoxPixelSize,
            this.toolStripDropDownButtonUpdate,
            this.toolStripSeparator3,
            this.toolStripButtonResetSize});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(910, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(75, 22);
			this.toolStripLabel1.Text = "Resolution:   ";
			// 
			// toolStripTextBoxResolutionX
			// 
			this.toolStripTextBoxResolutionX.MaxLength = 3;
			this.toolStripTextBoxResolutionX.Name = "toolStripTextBoxResolutionX";
			this.toolStripTextBoxResolutionX.Size = new System.Drawing.Size(40, 25);
			this.toolStripTextBoxResolutionX.Text = "64";
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(20, 22);
			this.toolStripLabel2.Text = "by";
			// 
			// toolStripTextBoxResolutionY
			// 
			this.toolStripTextBoxResolutionY.MaxLength = 3;
			this.toolStripTextBoxResolutionY.Name = "toolStripTextBoxResolutionY";
			this.toolStripTextBoxResolutionY.Size = new System.Drawing.Size(40, 25);
			this.toolStripTextBoxResolutionY.Text = "32";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel3
			// 
			this.toolStripLabel3.Name = "toolStripLabel3";
			this.toolStripLabel3.Size = new System.Drawing.Size(65, 22);
			this.toolStripLabel3.Text = "Pixel size:   ";
			// 
			// toolStripComboBoxPixelSize
			// 
			this.toolStripComboBoxPixelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBoxPixelSize.DropDownWidth = 75;
			this.toolStripComboBoxPixelSize.Items.AddRange(new object[] {
            "1 pixel",
            "2 pixels",
            "3 pixels",
            "4 pixels",
            "5 pixels",
            "6 pixels",
            "7 pixels",
            "8 pixels",
            "9 pixels",
            "10 pixels"});
			this.toolStripComboBoxPixelSize.MaxDropDownItems = 10;
			this.toolStripComboBoxPixelSize.Name = "toolStripComboBoxPixelSize";
			this.toolStripComboBoxPixelSize.Size = new System.Drawing.Size(75, 25);
			// 
			// toolStripDropDownButtonUpdate
			// 
			this.toolStripDropDownButtonUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButtonUpdate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonUpdate.Image")));
			this.toolStripDropDownButtonUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButtonUpdate.Name = "toolStripDropDownButtonUpdate";
			this.toolStripDropDownButtonUpdate.Size = new System.Drawing.Size(49, 22);
			this.toolStripDropDownButtonUpdate.Text = "Update";
			this.toolStripDropDownButtonUpdate.Click += new System.EventHandler(this.toolStripDropDownButtonUpdate_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonResetSize
			// 
			this.toolStripButtonResetSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonResetSize.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonResetSize.Image")));
			this.toolStripButtonResetSize.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonResetSize.Name = "toolStripButtonResetSize";
			this.toolStripButtonResetSize.Size = new System.Drawing.Size(115, 22);
			this.toolStripButtonResetSize.Text = "Reset to picture size";
			this.toolStripButtonResetSize.Click += new System.EventHandler(this.toolStripButtonResetSize_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.AddExtension = false;
			// 
			// toolStrip2
			// 
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxChannels,
            this.toolStripDropDownButtonClear,
            this.toolStripSeparator2,
            this.toolStripButtonLoadImage,
            this.toolStripButtonClearImage,
            this.toolStripButtonSaveImage,
            this.toolStripSeparator4,
            this.toolStripButtonReorder});
			this.toolStrip2.Location = new System.Drawing.Point(0, 25);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(910, 25);
			this.toolStrip2.TabIndex = 1;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// toolStripComboBoxChannels
			// 
			this.toolStripComboBoxChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBoxChannels.Name = "toolStripComboBoxChannels";
			this.toolStripComboBoxChannels.Size = new System.Drawing.Size(200, 25);
			this.toolStripComboBoxChannels.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxChannels_SelectedIndexChanged);
			// 
			// toolStripDropDownButtonClear
			// 
			this.toolStripDropDownButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButtonClear.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allChannelsToolStripMenuItem,
            this.selectedChannelToolStripMenuItem});
			this.toolStripDropDownButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonClear.Image")));
			this.toolStripDropDownButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButtonClear.Name = "toolStripDropDownButtonClear";
			this.toolStripDropDownButtonClear.Size = new System.Drawing.Size(47, 22);
			this.toolStripDropDownButtonClear.Text = "Clear";
			// 
			// allChannelsToolStripMenuItem
			// 
			this.allChannelsToolStripMenuItem.Name = "allChannelsToolStripMenuItem";
			this.allChannelsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.allChannelsToolStripMenuItem.Text = "All Channels";
			this.allChannelsToolStripMenuItem.Click += new System.EventHandler(this.allChannelsToolStripMenuItem_Click);
			// 
			// selectedChannelToolStripMenuItem
			// 
			this.selectedChannelToolStripMenuItem.Name = "selectedChannelToolStripMenuItem";
			this.selectedChannelToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.selectedChannelToolStripMenuItem.Text = "Selected Channel";
			this.selectedChannelToolStripMenuItem.Click += new System.EventHandler(this.selectedChannelToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonLoadImage
			// 
			this.toolStripButtonLoadImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonLoadImage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadImage.Image")));
			this.toolStripButtonLoadImage.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonLoadImage.Name = "toolStripButtonLoadImage";
			this.toolStripButtonLoadImage.Size = new System.Drawing.Size(73, 22);
			this.toolStripButtonLoadImage.Text = "Load image";
			this.toolStripButtonLoadImage.Click += new System.EventHandler(this.toolStripButtonLoadImage_Click);
			// 
			// toolStripButtonClearImage
			// 
			this.toolStripButtonClearImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonClearImage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClearImage.Image")));
			this.toolStripButtonClearImage.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonClearImage.Name = "toolStripButtonClearImage";
			this.toolStripButtonClearImage.Size = new System.Drawing.Size(74, 22);
			this.toolStripButtonClearImage.Text = "Clear image";
			this.toolStripButtonClearImage.Click += new System.EventHandler(this.toolStripButtonClearImage_Click);
			// 
			// toolStripButtonSaveImage
			// 
			this.toolStripButtonSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonSaveImage.Enabled = false;
			this.toolStripButtonSaveImage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveImage.Image")));
			this.toolStripButtonSaveImage.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSaveImage.Name = "toolStripButtonSaveImage";
			this.toolStripButtonSaveImage.Size = new System.Drawing.Size(104, 22);
			this.toolStripButtonSaveImage.Text = "Save image to file";
			this.toolStripButtonSaveImage.Click += new System.EventHandler(this.toolStripButtonSaveImage_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			this.toolStripSeparator4.Visible = false;
			// 
			// toolStripButtonReorder
			// 
			this.toolStripButtonReorder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonReorder.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReorder.Image")));
			this.toolStripButtonReorder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonReorder.Name = "toolStripButtonReorder";
			this.toolStripButtonReorder.Size = new System.Drawing.Size(65, 22);
			this.toolStripButtonReorder.Text = "Copy cells";
			this.toolStripButtonReorder.Visible = false;
			// 
			// pictureBoxSetupGrid
			// 
			this.pictureBoxSetupGrid.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxSetupGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBoxSetupGrid.Location = new System.Drawing.Point(21, 23);
			this.pictureBoxSetupGrid.Name = "pictureBoxSetupGrid";
			this.pictureBoxSetupGrid.Size = new System.Drawing.Size(234, 151);
			this.pictureBoxSetupGrid.TabIndex = 13;
			this.pictureBoxSetupGrid.TabStop = false;
			this.pictureBoxSetupGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxSetupGrid_Paint);
			this.pictureBoxSetupGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSetupGrid_MouseEvent);
			this.pictureBoxSetupGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSetupGrid_MouseEvent);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.cmdNewUI);
			this.panel1.Controls.Add(this.checkBoxRedirectOutputs);
			this.panel1.Controls.Add(this.trackBarBrightness);
			this.panel1.Controls.Add(this.labelBrightness);
			this.panel1.Controls.Add(this.labelChannel);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 435);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(910, 69);
			this.panel1.TabIndex = 3;
			// 
			// cmdNewUI
			// 
			this.cmdNewUI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdNewUI.Location = new System.Drawing.Point(650, 31);
			this.cmdNewUI.Name = "cmdNewUI";
			this.cmdNewUI.Size = new System.Drawing.Size(84, 23);
			this.cmdNewUI.TabIndex = 4;
			this.cmdNewUI.Text = "Use New UI";
			this.cmdNewUI.UseVisualStyleBackColor = true;
			this.cmdNewUI.Click += new System.EventHandler(this.cmdNewUI_Click);
			// 
			// checkBoxRedirectOutputs
			// 
			this.checkBoxRedirectOutputs.Location = new System.Drawing.Point(5, 33);
			this.checkBoxRedirectOutputs.Name = "checkBoxRedirectOutputs";
			this.checkBoxRedirectOutputs.Size = new System.Drawing.Size(144, 31);
			this.checkBoxRedirectOutputs.TabIndex = 1;
			this.checkBoxRedirectOutputs.Text = "Respect Channel outputs during playback";
			this.checkBoxRedirectOutputs.UseVisualStyleBackColor = true;
			// 
			// trackBarBrightness
			// 
			this.trackBarBrightness.LargeChange = 1;
			this.trackBarBrightness.Location = new System.Drawing.Point(248, 31);
			this.trackBarBrightness.Maximum = 20;
			this.trackBarBrightness.Name = "trackBarBrightness";
			this.trackBarBrightness.Size = new System.Drawing.Size(144, 45);
			this.trackBarBrightness.TabIndex = 3;
			this.trackBarBrightness.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trackBarBrightness.Value = 10;
			this.trackBarBrightness.Visible = false;
			this.trackBarBrightness.ValueChanged += new System.EventHandler(this.trackBarBrightness_ValueChanged);
			// 
			// labelBrightness
			// 
			this.labelBrightness.AutoSize = true;
			this.labelBrightness.Location = new System.Drawing.Point(155, 41);
			this.labelBrightness.Name = "labelBrightness";
			this.labelBrightness.Size = new System.Drawing.Size(87, 13);
			this.labelBrightness.TabIndex = 2;
			this.labelBrightness.Text = "Image brightness";
			this.labelBrightness.Visible = false;
			// 
			// panelPictureBoxContainer
			// 
			this.panelPictureBoxContainer.AutoScroll = true;
			this.panelPictureBoxContainer.Controls.Add(this.pictureBoxSetupGrid);
			this.panelPictureBoxContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPictureBoxContainer.Location = new System.Drawing.Point(0, 50);
			this.panelPictureBoxContainer.Name = "panelPictureBoxContainer";
			this.panelPictureBoxContainer.Size = new System.Drawing.Size(910, 385);
			this.panelPictureBoxContainer.TabIndex = 2;
			// 
			// SetupDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(910, 504);
			this.Controls.Add(this.panelPictureBoxContainer);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip2);
			this.Controls.Add(this.toolStrip1);
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(20, 73);
			this.Name = "SetupDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Setup for sequence preview";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupDialog_FormClosing);
			this.ResizeBegin += new System.EventHandler(this.SetupDialog_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.SetupDialog_ResizeEnd);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetupDialog_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SetupDialog_KeyUp);
			this.Resize += new System.EventHandler(this.SetupDialog_Resize);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetupGrid)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
			this.panelPictureBoxContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelChannel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxResolutionX;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxResolutionY;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxPixelSize;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.PictureBox pictureBoxSetupGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonResetSize;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxChannels;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonClear;
        private System.Windows.Forms.ToolStripMenuItem allChannelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadImage;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearImage;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveImage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.ToolStripButton toolStripButtonReorder;
        private System.Windows.Forms.ToolStripButton toolStripDropDownButtonUpdate;
        private System.Windows.Forms.Panel panelPictureBoxContainer;
        private System.Windows.Forms.CheckBox checkBoxRedirectOutputs;
		private System.Windows.Forms.Button cmdNewUI;
    }
}