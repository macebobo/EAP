namespace ElfCore.Forms
{
	partial class NewProfile
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProfile));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this._txtName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cboFormat = new System.Windows.Forms.ComboBox();
			this._cboFormat = new System.Windows.Forms.Label();
			this.pnlEditChannel = new System.Windows.Forms.Panel();
			this.txtChannelName = new System.Windows.Forms.TextBox();
			this.pnlSpacer = new System.Windows.Forms.Panel();
			this.pnlPlaceholder = new System.Windows.Forms.Panel();
			this.cmdSaveEdit = new System.Windows.Forms.Button();
			this.cmdCancelEdit = new System.Windows.Forms.Button();
			this.cddEditColor = new ElfControls.ColorDropDown();
			this.cmdAddOne = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cmdAddMany = new System.Windows.Forms.Button();
			this.txtNumChannels = new System.Windows.Forms.TextBox();
			this.cmdRemoveChannel = new System.Windows.Forms.Button();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.chkShowGridLines = new System.Windows.Forms.CheckBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.cmdToBottom = new System.Windows.Forms.Button();
			this.cmdDown = new System.Windows.Forms.Button();
			this.cmdUp = new System.Windows.Forms.Button();
			this.cmdToTop = new System.Windows.Forms.Button();
			this.cmdEdit = new System.Windows.Forms.Button();
			this._txtWidth = new System.Windows.Forms.Label();
			this._txtHeight = new System.Windows.Forms.Label();
			this._txtCellSize = new System.Windows.Forms.Label();
			this.cboCellSize = new ElfControls.ImageDropDown();
			this.lstChannels = new ElfControls.ImageListBox();
			this.grpChannels = new System.Windows.Forms.GroupBox();
			this.grpCanvas = new System.Windows.Forms.GroupBox();
			this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
			this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
			this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
			this.lblFinal = new System.Windows.Forms.Label();
			this.lblPixelWidth = new System.Windows.Forms.Label();
			this.lblPixelHeight = new System.Windows.Forms.Label();
			this.lblX = new System.Windows.Forms.Label();
			this._lblFilename = new System.Windows.Forms.Label();
			this.lblFilename = new System.Windows.Forms.Label();
			this.pnlEditChannel.SuspendLayout();
			this.pnlPlaceholder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.grpChannels.SuspendLayout();
			this.grpCanvas.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(301, 357);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 9;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmdOk
			// 
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(230, 357);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 8;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// _txtName
			// 
			this._txtName.AutoSize = true;
			this._txtName.Location = new System.Drawing.Point(12, 9);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(79, 15);
			this._txtName.TabIndex = 0;
			this._txtName.Text = "Profile &Name:";
			// 
			// txtName
			// 
			this.errorProvider1.SetIconAlignment(this.txtName, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.txtName.Location = new System.Drawing.Point(12, 27);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(258, 23);
			this.txtName.TabIndex = 1;
			this.toolTip1.SetToolTip(this.txtName, "Enter the name of this Profile");
			this.txtName.Click += new System.EventHandler(this.TextBox_Enter);
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// cboFormat
			// 
			this.cboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFormat.FormattingEnabled = true;
			this.cboFormat.Location = new System.Drawing.Point(12, 79);
			this.cboFormat.Name = "cboFormat";
			this.cboFormat.Size = new System.Drawing.Size(258, 23);
			this.cboFormat.TabIndex = 3;
			this.toolTip1.SetToolTip(this.cboFormat, "Select the type of Profile to create");
			this.cboFormat.SelectedIndexChanged += new System.EventHandler(this.cboFormat_SelectedIndexChanged);
			// 
			// _cboFormat
			// 
			this._cboFormat.AutoSize = true;
			this._cboFormat.Location = new System.Drawing.Point(12, 61);
			this._cboFormat.Name = "_cboFormat";
			this._cboFormat.Size = new System.Drawing.Size(85, 15);
			this._cboFormat.TabIndex = 2;
			this._cboFormat.Text = "Profile &Format:";
			// 
			// pnlEditChannel
			// 
			this.pnlEditChannel.BackColor = System.Drawing.Color.White;
			this.pnlEditChannel.Controls.Add(this.txtChannelName);
			this.pnlEditChannel.Controls.Add(this.pnlSpacer);
			this.pnlEditChannel.Controls.Add(this.pnlPlaceholder);
			this.pnlEditChannel.Controls.Add(this.cddEditColor);
			this.pnlEditChannel.Location = new System.Drawing.Point(15, 22);
			this.pnlEditChannel.Name = "pnlEditChannel";
			this.pnlEditChannel.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.pnlEditChannel.Size = new System.Drawing.Size(263, 18);
			this.pnlEditChannel.TabIndex = 10;
			this.pnlEditChannel.Visible = false;
			// 
			// txtChannelName
			// 
			this.txtChannelName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtChannelName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtChannelName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtChannelName.Location = new System.Drawing.Point(27, 0);
			this.txtChannelName.Multiline = true;
			this.txtChannelName.Name = "txtChannelName";
			this.txtChannelName.Size = new System.Drawing.Size(194, 18);
			this.txtChannelName.TabIndex = 1;
			// 
			// pnlSpacer
			// 
			this.pnlSpacer.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlSpacer.Location = new System.Drawing.Point(22, 0);
			this.pnlSpacer.Name = "pnlSpacer";
			this.pnlSpacer.Size = new System.Drawing.Size(5, 18);
			this.pnlSpacer.TabIndex = 5;
			// 
			// pnlPlaceholder
			// 
			this.pnlPlaceholder.Controls.Add(this.cmdSaveEdit);
			this.pnlPlaceholder.Controls.Add(this.cmdCancelEdit);
			this.pnlPlaceholder.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlPlaceholder.Location = new System.Drawing.Point(221, 0);
			this.pnlPlaceholder.Name = "pnlPlaceholder";
			this.pnlPlaceholder.Size = new System.Drawing.Size(40, 18);
			this.pnlPlaceholder.TabIndex = 4;
			// 
			// cmdSaveEdit
			// 
			this.cmdSaveEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSaveEdit.BackColor = System.Drawing.SystemColors.Control;
			this.cmdSaveEdit.Image = global::ElfCore.Properties.Resources.annotation_save;
			this.cmdSaveEdit.Location = new System.Drawing.Point(1, 0);
			this.cmdSaveEdit.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
			this.cmdSaveEdit.Name = "cmdSaveEdit";
			this.cmdSaveEdit.Size = new System.Drawing.Size(20, 18);
			this.cmdSaveEdit.TabIndex = 2;
			this.cmdSaveEdit.UseVisualStyleBackColor = false;
			this.cmdSaveEdit.Click += new System.EventHandler(this.cmdSaveEdit_Click);
			// 
			// cmdCancelEdit
			// 
			this.cmdCancelEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdCancelEdit.BackColor = System.Drawing.SystemColors.Control;
			this.cmdCancelEdit.Image = global::ElfCore.Properties.Resources.annotation_delete;
			this.cmdCancelEdit.Location = new System.Drawing.Point(21, 0);
			this.cmdCancelEdit.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
			this.cmdCancelEdit.Name = "cmdCancelEdit";
			this.cmdCancelEdit.Size = new System.Drawing.Size(20, 18);
			this.cmdCancelEdit.TabIndex = 3;
			this.cmdCancelEdit.UseVisualStyleBackColor = false;
			this.cmdCancelEdit.Click += new System.EventHandler(this.cmdCancelEdit_Click);
			// 
			// cddEditColor
			// 
			this.cddEditColor.AllowEmptyColor = false;
			this.cddEditColor.AnchorSize = new System.Drawing.Size(20, 18);
			this.cddEditColor.BackColor = System.Drawing.Color.White;
			this.cddEditColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.cddEditColor.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddEditColor.Color = System.Drawing.Color.Maroon;
			this.cddEditColor.DisplaySystemColorPalette = false;
			this.cddEditColor.Dock = System.Windows.Forms.DockStyle.Left;
			this.cddEditColor.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddEditColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.cddEditColor.EmbiggenHotTracked = true;
			this.cddEditColor.HotTracking = true;
			this.cddEditColor.Location = new System.Drawing.Point(2, 0);
			this.cddEditColor.Name = "cddEditColor";
			this.cddEditColor.NoColorIndicatorImage = null;
			this.cddEditColor.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddEditColor.Size = new System.Drawing.Size(20, 18);
			this.cddEditColor.TabIndex = 0;
			this.toolTip1.SetToolTip(this.cddEditColor, "Select color to be used for the Channel.");
			// 
			// cmdAddOne
			// 
			this.cmdAddOne.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdAddOne.Image = global::ElfCore.Properties.Resources.channel;
			this.cmdAddOne.Location = new System.Drawing.Point(15, 305);
			this.cmdAddOne.Name = "cmdAddOne";
			this.cmdAddOne.Size = new System.Drawing.Size(22, 22);
			this.cmdAddOne.TabIndex = 17;
			this.cmdAddOne.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdAddOne, "Add a single Channel");
			this.cmdAddOne.Click += new System.EventHandler(this.cmdAddOne_Click);
			// 
			// cmdAddMany
			// 
			this.cmdAddMany.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdAddMany.Image = global::ElfCore.Properties.Resources.channels;
			this.cmdAddMany.Location = new System.Drawing.Point(40, 305);
			this.cmdAddMany.Name = "cmdAddMany";
			this.cmdAddMany.Size = new System.Drawing.Size(22, 22);
			this.cmdAddMany.TabIndex = 18;
			this.cmdAddMany.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdAddMany, "Add multiple Channels");
			this.cmdAddMany.Click += new System.EventHandler(this.cmdAddMany_Click);
			// 
			// txtNumChannels
			// 
			this.txtNumChannels.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.txtNumChannels.Location = new System.Drawing.Point(65, 305);
			this.txtNumChannels.MaxLength = 3;
			this.txtNumChannels.Multiline = true;
			this.txtNumChannels.Name = "txtNumChannels";
			this.txtNumChannels.Size = new System.Drawing.Size(29, 22);
			this.txtNumChannels.TabIndex = 19;
			this.txtNumChannels.Text = "10";
			this.toolTip1.SetToolTip(this.txtNumChannels, "Enter the number of Channels to add");
			this.txtNumChannels.Click += new System.EventHandler(this.TextBox_Enter);
			this.txtNumChannels.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// cmdRemoveChannel
			// 
			this.cmdRemoveChannel.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdRemoveChannel.Image = global::ElfCore.Properties.Resources.channel;
			this.cmdRemoveChannel.Location = new System.Drawing.Point(256, 305);
			this.cmdRemoveChannel.Name = "cmdRemoveChannel";
			this.cmdRemoveChannel.Size = new System.Drawing.Size(22, 22);
			this.cmdRemoveChannel.TabIndex = 21;
			this.cmdRemoveChannel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdRemoveChannel, "Add multiple Channels");
			this.cmdRemoveChannel.Click += new System.EventHandler(this.cmdRemoveChannel_Click);
			// 
			// txtWidth
			// 
			this.txtWidth.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtWidth.Location = new System.Drawing.Point(99, 81);
			this.txtWidth.MaxLength = 4;
			this.txtWidth.Multiline = true;
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(43, 24);
			this.txtWidth.TabIndex = 5;
			this.txtWidth.Text = "64";
			this.txtWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtWidth, "Width in Cells of the Canvas.");
			this.txtWidth.Click += new System.EventHandler(this.TextBox_Enter);
			this.txtWidth.TextChanged += new System.EventHandler(this.Size_TextChanged);
			this.txtWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// txtHeight
			// 
			this.txtHeight.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtHeight.Location = new System.Drawing.Point(200, 81);
			this.txtHeight.MaxLength = 4;
			this.txtHeight.Multiline = true;
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(43, 24);
			this.txtHeight.TabIndex = 7;
			this.txtHeight.Text = "32";
			this.txtHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtHeight, "Height in Cells of the Canvas.");
			this.txtHeight.Click += new System.EventHandler(this.TextBox_Enter);
			this.txtHeight.TextChanged += new System.EventHandler(this.Size_TextChanged);
			this.txtHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// chkShowGridLines
			// 
			this.chkShowGridLines.Checked = true;
			this.chkShowGridLines.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowGridLines.Image = global::ElfCore.Properties.Resources.grid;
			this.chkShowGridLines.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkShowGridLines.Location = new System.Drawing.Point(42, 50);
			this.chkShowGridLines.Name = "chkShowGridLines";
			this.chkShowGridLines.Size = new System.Drawing.Size(130, 29);
			this.chkShowGridLines.TabIndex = 3;
			this.chkShowGridLines.Text = "Show &Grid Lines";
			this.chkShowGridLines.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this.chkShowGridLines, "Show a line between individual cells.");
			this.chkShowGridLines.CheckedChanged += new System.EventHandler(this.chkShowGridLines_CheckedChanged);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// cmdToBottom
			// 
			this.cmdToBottom.Image = global::ElfCore.Properties.Resources.to_bottom;
			this.cmdToBottom.Location = new System.Drawing.Point(284, 277);
			this.cmdToBottom.Name = "cmdToBottom";
			this.cmdToBottom.Size = new System.Drawing.Size(22, 22);
			this.cmdToBottom.TabIndex = 16;
			this.cmdToBottom.Click += new System.EventHandler(this.cmdToBottom_Click);
			// 
			// cmdDown
			// 
			this.cmdDown.Image = global::ElfCore.Properties.Resources.down;
			this.cmdDown.Location = new System.Drawing.Point(284, 252);
			this.cmdDown.Name = "cmdDown";
			this.cmdDown.Size = new System.Drawing.Size(22, 22);
			this.cmdDown.TabIndex = 15;
			this.cmdDown.Click += new System.EventHandler(this.cmdDown_Click);
			// 
			// cmdUp
			// 
			this.cmdUp.Image = global::ElfCore.Properties.Resources.up;
			this.cmdUp.Location = new System.Drawing.Point(284, 49);
			this.cmdUp.Name = "cmdUp";
			this.cmdUp.Size = new System.Drawing.Size(22, 22);
			this.cmdUp.TabIndex = 14;
			this.cmdUp.Click += new System.EventHandler(this.cmdUp_Click);
			// 
			// cmdToTop
			// 
			this.cmdToTop.Image = global::ElfCore.Properties.Resources.to_top;
			this.cmdToTop.Location = new System.Drawing.Point(284, 24);
			this.cmdToTop.Name = "cmdToTop";
			this.cmdToTop.Size = new System.Drawing.Size(22, 22);
			this.cmdToTop.TabIndex = 13;
			this.cmdToTop.Click += new System.EventHandler(this.cmdToTop_Click);
			// 
			// cmdEdit
			// 
			this.cmdEdit.Image = global::ElfCore.Properties.Resources.channel;
			this.cmdEdit.Location = new System.Drawing.Point(231, 305);
			this.cmdEdit.Name = "cmdEdit";
			this.cmdEdit.Size = new System.Drawing.Size(22, 22);
			this.cmdEdit.TabIndex = 20;
			this.cmdEdit.Click += new System.EventHandler(this.cmdEdit_Click);
			// 
			// _txtWidth
			// 
			this._txtWidth.AutoSize = true;
			this._txtWidth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._txtWidth.Location = new System.Drawing.Point(49, 86);
			this._txtWidth.Name = "_txtWidth";
			this._txtWidth.Size = new System.Drawing.Size(42, 15);
			this._txtWidth.TabIndex = 4;
			this._txtWidth.Text = "&Width:";
			this._txtWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _txtHeight
			// 
			this._txtHeight.AutoSize = true;
			this._txtHeight.Location = new System.Drawing.Point(148, 86);
			this._txtHeight.Name = "_txtHeight";
			this._txtHeight.Size = new System.Drawing.Size(46, 15);
			this._txtHeight.TabIndex = 6;
			this._txtHeight.Text = "&Height:";
			// 
			// _txtCellSize
			// 
			this._txtCellSize.Image = global::ElfCore.Properties.Resources.pixel_9;
			this._txtCellSize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._txtCellSize.Location = new System.Drawing.Point(20, 24);
			this._txtCellSize.Name = "_txtCellSize";
			this._txtCellSize.Size = new System.Drawing.Size(73, 19);
			this._txtCellSize.TabIndex = 1;
			this._txtCellSize.Text = "Cell &Size:";
			this._txtCellSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cboCellSize
			// 
			this.cboCellSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboCellSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCellSize.Location = new System.Drawing.Point(99, 23);
			this.cboCellSize.Name = "cboCellSize";
			this.cboCellSize.Size = new System.Drawing.Size(144, 24);
			this.cboCellSize.TabIndex = 2;
			this.cboCellSize.SelectedIndexChanged += new System.EventHandler(this.cboCellSize_SelectedIndexChanged);
			// 
			// lstChannels
			// 
			this.lstChannels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstChannels.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstChannels.FormattingEnabled = true;
			this.lstChannels.IntegralHeight = false;
			this.lstChannels.Location = new System.Drawing.Point(15, 22);
			this.lstChannels.Name = "lstChannels";
			this.lstChannels.Size = new System.Drawing.Size(263, 277);
			this.lstChannels.TabIndex = 12;
			this.lstChannels.SelectedIndexChanged += new System.EventHandler(this.lstChannels_SelectedIndexChanged);
			this.lstChannels.DoubleClick += new System.EventHandler(this.lstChannels_DoubleClick);
			// 
			// grpChannels
			// 
			this.grpChannels.Controls.Add(this.cmdToBottom);
			this.grpChannels.Controls.Add(this.cmdDown);
			this.grpChannels.Controls.Add(this.cmdUp);
			this.grpChannels.Controls.Add(this.cmdToTop);
			this.grpChannels.Controls.Add(this.cmdAddMany);
			this.grpChannels.Controls.Add(this.cmdAddOne);
			this.grpChannels.Controls.Add(this.cmdRemoveChannel);
			this.grpChannels.Controls.Add(this.cmdEdit);
			this.grpChannels.Controls.Add(this.txtNumChannels);
			this.grpChannels.Controls.Add(this.pnlEditChannel);
			this.grpChannels.Controls.Add(this.lstChannels);
			this.grpChannels.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpChannels.Location = new System.Drawing.Point(288, 9);
			this.grpChannels.Name = "grpChannels";
			this.grpChannels.Size = new System.Drawing.Size(316, 333);
			this.grpChannels.TabIndex = 7;
			this.grpChannels.TabStop = false;
			this.grpChannels.Text = "Channels:";
			// 
			// grpCanvas
			// 
			this.grpCanvas.Controls.Add(this.lblX);
			this.grpCanvas.Controls.Add(this.lblPixelHeight);
			this.grpCanvas.Controls.Add(this.lblPixelWidth);
			this.grpCanvas.Controls.Add(this.lblFinal);
			this.grpCanvas.Controls.Add(this.cboCellSize);
			this.grpCanvas.Controls.Add(this.chkShowGridLines);
			this.grpCanvas.Controls.Add(this._txtCellSize);
			this.grpCanvas.Controls.Add(this._txtHeight);
			this.grpCanvas.Controls.Add(this.txtHeight);
			this.grpCanvas.Controls.Add(this._txtWidth);
			this.grpCanvas.Controls.Add(this.txtWidth);
			this.grpCanvas.Controls.Add(this.shapeContainer1);
			this.grpCanvas.Location = new System.Drawing.Point(12, 170);
			this.grpCanvas.Name = "grpCanvas";
			this.grpCanvas.Size = new System.Drawing.Size(258, 172);
			this.grpCanvas.TabIndex = 6;
			this.grpCanvas.TabStop = false;
			this.grpCanvas.Text = "Display";
			// 
			// shapeContainer1
			// 
			this.shapeContainer1.Location = new System.Drawing.Point(3, 19);
			this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.shapeContainer1.Name = "shapeContainer1";
			this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2,
            this.lineShape1});
			this.shapeContainer1.Size = new System.Drawing.Size(252, 150);
			this.shapeContainer1.TabIndex = 0;
			this.shapeContainer1.TabStop = false;
			// 
			// lineShape1
			// 
			this.lineShape1.BorderColor = System.Drawing.SystemColors.ControlDark;
			this.lineShape1.Name = "lineShape1";
			this.lineShape1.X1 = 18;
			this.lineShape1.X2 = 240;
			this.lineShape1.Y1 = 96;
			this.lineShape1.Y2 = 96;
			// 
			// lineShape2
			// 
			this.lineShape2.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this.lineShape2.Name = "lineShape2";
			this.lineShape2.X1 = 18;
			this.lineShape2.X2 = 240;
			this.lineShape2.Y1 = 95;
			this.lineShape2.Y2 = 95;
			// 
			// lblFinal
			// 
			this.lblFinal.AutoSize = true;
			this.lblFinal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFinal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblFinal.Location = new System.Drawing.Point(20, 123);
			this.lblFinal.Name = "lblFinal";
			this.lblFinal.Size = new System.Drawing.Size(108, 15);
			this.lblFinal.TabIndex = 8;
			this.lblFinal.Text = "Final Size (in Pixels)";
			this.lblFinal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblPixelWidth
			// 
			this.lblPixelWidth.AutoSize = true;
			this.lblPixelWidth.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPixelWidth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblPixelWidth.Location = new System.Drawing.Point(96, 138);
			this.lblPixelWidth.Name = "lblPixelWidth";
			this.lblPixelWidth.Size = new System.Drawing.Size(46, 21);
			this.lblPixelWidth.TabIndex = 9;
			this.lblPixelWidth.Text = "1000";
			this.lblPixelWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblPixelHeight
			// 
			this.lblPixelHeight.AutoSize = true;
			this.lblPixelHeight.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPixelHeight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblPixelHeight.Location = new System.Drawing.Point(197, 138);
			this.lblPixelHeight.Name = "lblPixelHeight";
			this.lblPixelHeight.Size = new System.Drawing.Size(46, 21);
			this.lblPixelHeight.TabIndex = 11;
			this.lblPixelHeight.Text = "1000";
			this.lblPixelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblX.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblX.Location = new System.Drawing.Point(157, 138);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(19, 21);
			this.lblX.TabIndex = 10;
			this.lblX.Text = "x";
			this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _lblFilename
			// 
			this._lblFilename.AutoSize = true;
			this._lblFilename.Location = new System.Drawing.Point(12, 115);
			this._lblFilename.Name = "_lblFilename";
			this._lblFilename.Size = new System.Drawing.Size(85, 15);
			this._lblFilename.TabIndex = 4;
			this._lblFilename.Text = "Profile &Format:";
			// 
			// lblFilename
			// 
			this.lblFilename.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFilename.Location = new System.Drawing.Point(12, 130);
			this.lblFilename.Name = "lblFilename";
			this.lblFilename.Size = new System.Drawing.Size(258, 37);
			this.lblFilename.TabIndex = 5;
			this.lblFilename.Text = "‪C:\\Users\\roba\\Documents\\Vixen\\Profiles\\Vixen 2.5.0.8 Profile.pro";
			// 
			// NewProfile
			// 
			this.AcceptButton = this.cmdSaveEdit;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(617, 398);
			this.Controls.Add(this.lblFilename);
			this.Controls.Add(this._lblFilename);
			this.Controls.Add(this.grpCanvas);
			this.Controls.Add(this.grpChannels);
			this.Controls.Add(this._cboFormat);
			this.Controls.Add(this.cboFormat);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewProfile";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create New Profile";
			this.pnlEditChannel.ResumeLayout(false);
			this.pnlEditChannel.PerformLayout();
			this.pnlPlaceholder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.grpChannels.ResumeLayout(false);
			this.grpChannels.PerformLayout();
			this.grpCanvas.ResumeLayout(false);
			this.grpCanvas.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Label _txtName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.ComboBox cboFormat;
		private System.Windows.Forms.Label _cboFormat;
		private System.Windows.Forms.Panel pnlEditChannel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button cmdAddOne;
		private System.Windows.Forms.Button cmdAddMany;
		private System.Windows.Forms.TextBox txtNumChannels;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button cmdEdit;
		private System.Windows.Forms.Button cmdToBottom;
		private System.Windows.Forms.Button cmdDown;
		private System.Windows.Forms.Button cmdUp;
		private System.Windows.Forms.Button cmdToTop;
		private System.Windows.Forms.Button cmdRemoveChannel;
		private System.Windows.Forms.TextBox txtChannelName;
		private ElfControls.ColorDropDown cddEditColor;
		private ElfControls.ImageListBox lstChannels;
		private System.Windows.Forms.Button cmdSaveEdit;
		private System.Windows.Forms.Button cmdCancelEdit;
		private System.Windows.Forms.Panel pnlPlaceholder;
		private System.Windows.Forms.Panel pnlSpacer;
		private System.Windows.Forms.Label _txtHeight;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.Label _txtWidth;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.CheckBox chkShowGridLines;
		private System.Windows.Forms.Label _txtCellSize;
		private ElfControls.ImageDropDown cboCellSize;
		private System.Windows.Forms.GroupBox grpChannels;
		private System.Windows.Forms.GroupBox grpCanvas;
		private System.Windows.Forms.Label lblFilename;
		private System.Windows.Forms.Label _lblFilename;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblPixelHeight;
		private System.Windows.Forms.Label lblPixelWidth;
		private System.Windows.Forms.Label lblFinal;
		private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
		private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
		private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
	}
}