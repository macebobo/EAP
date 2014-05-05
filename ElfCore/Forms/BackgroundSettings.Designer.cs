namespace ElfCore.Forms
{
	partial class BackgroundSettings
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
			if (_tempProfile != null)
			{
				_tempProfile.Dispose();
				_tempProfile = null;
			}
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
			ElfControls.ColorManager.HSL hsl1 = new ElfControls.ColorManager.HSL();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundSettings));
			ElfControls.ColorManager.HSL hsl2 = new ElfControls.ColorManager.HSL();
			ElfControls.ColorManager.HSL hsl3 = new ElfControls.ColorManager.HSL();
			this._pctPreview = new System.Windows.Forms.Label();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this._txtFilename = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.hslBrightness = new ElfControls.HSLSlider();
			this.hslSaturation = new ElfControls.HSLSlider();
			this.hslHue = new ElfControls.HSLSlider();
			this.cmdSaveModified = new System.Windows.Forms.Button();
			this.cddGrid = new ElfControls.ColorDropDown();
			this.cddBackground = new ElfControls.ColorDropDown();
			this.cmdClear = new System.Windows.Forms.Button();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdBrowse = new System.Windows.Forms.Button();
			this.txtSaturation = new System.Windows.Forms.TextBox();
			this.txtBrightness = new System.Windows.Forms.TextBox();
			this.txtHue = new System.Windows.Forms.TextBox();
			this.lblBrightness = new System.Windows.Forms.Label();
			this.lblSaturation = new System.Windows.Forms.Label();
			this.lblHue = new System.Windows.Forms.Label();
			this.chkShowGrid = new System.Windows.Forms.CheckBox();
			this.chkSaveEncoded = new System.Windows.Forms.CheckBox();
			this.cboPicturePosition = new ElfControls.ImageDropDown();
			this.cboAnchor = new ElfControls.ImageDropDown();
			this.grpAdjust = new System.Windows.Forms.GroupBox();
			this._tbHue = new System.Windows.Forms.Label();
			this._tbSaturation = new System.Windows.Forms.Label();
			this._tbBrightness = new System.Windows.Forms.Label();
			this.grpDisplay = new System.Windows.Forms.GroupBox();
			this._qcGridColor = new System.Windows.Forms.Label();
			this._qcBackground = new System.Windows.Forms.Label();
			this.ChannelColorDialog = new System.Windows.Forms.ColorDialog();
			this.OpenImageFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SaveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.tmrSelected = new System.Windows.Forms.Timer(this.components);
			this._cboPicturePosition = new System.Windows.Forms.Label();
			this._cboAnchor = new System.Windows.Forms.Label();
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			this.cmdReset = new System.Windows.Forms.Button();
			this.grpAdjust.SuspendLayout();
			this.grpDisplay.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// _pctPreview
			// 
			this._pctPreview.AutoSize = true;
			this._pctPreview.Location = new System.Drawing.Point(9, 9);
			this._pctPreview.Name = "_pctPreview";
			this._pctPreview.Size = new System.Drawing.Size(51, 15);
			this._pctPreview.TabIndex = 0;
			this._pctPreview.Text = "Preview:";
			// 
			// txtFilename
			// 
			this.txtFilename.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.txtFilename.Location = new System.Drawing.Point(427, 29);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.ReadOnly = true;
			this.txtFilename.Size = new System.Drawing.Size(302, 23);
			this.txtFilename.TabIndex = 2;
			this.toolTip1.SetToolTip(this.txtFilename, "Filename of the image used in the background");
			// 
			// _txtFilename
			// 
			this._txtFilename.AutoSize = true;
			this._txtFilename.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._txtFilename.Location = new System.Drawing.Point(423, 10);
			this._txtFilename.Name = "_txtFilename";
			this._txtFilename.Size = new System.Drawing.Size(63, 15);
			this._txtFilename.TabIndex = 1;
			this._txtFilename.Text = "File Name:";
			// 
			// hslBrightness
			// 
			this.hslBrightness.BackColor = System.Drawing.Color.Transparent;
			this.hslBrightness.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.hslBrightness.DrawStyle = ElfControls.HSLSlider.eDrawStyle.Luminance;
			hsl1.Alpha = 1D;
			hsl1.H = 0D;
			hsl1.Hue = 0D;
			hsl1.L = 0D;
			hsl1.Luminance = 0D;
			hsl1.S = 1D;
			hsl1.Saturation = 1D;
			this.hslBrightness.HSL = hsl1;
			this.hslBrightness.IndicatorMarks = ((System.Collections.Generic.List<double>)(resources.GetObject("hslBrightness.IndicatorMarks")));
			this.hslBrightness.IndictorColor = System.Drawing.Color.White;
			this.hslBrightness.Location = new System.Drawing.Point(87, 90);
			this.hslBrightness.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.hslBrightness.MinValue = -100D;
			this.hslBrightness.Name = "hslBrightness";
			this.hslBrightness.ReverseFill = true;
			this.hslBrightness.Size = new System.Drawing.Size(208, 28);
			this.hslBrightness.TabIndex = 9;
			this.toolTip1.SetToolTip(this.hslBrightness, "Slide to adjust the image brightness");
			this.hslBrightness.Value = 0D;
			this.hslBrightness.Changed += new System.EventHandler(this.hslBrightness_Changed);
			// 
			// hslSaturation
			// 
			this.hslSaturation.BackColor = System.Drawing.Color.Transparent;
			this.hslSaturation.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.hslSaturation.DrawStyle = ElfControls.HSLSlider.eDrawStyle.Saturation;
			hsl2.Alpha = 1D;
			hsl2.H = 0D;
			hsl2.Hue = 0D;
			hsl2.L = 1D;
			hsl2.Luminance = 1D;
			hsl2.S = 1D;
			hsl2.Saturation = 1D;
			this.hslSaturation.HSL = hsl2;
			this.hslSaturation.IndicatorMarks = ((System.Collections.Generic.List<double>)(resources.GetObject("hslSaturation.IndicatorMarks")));
			this.hslSaturation.IndictorColor = System.Drawing.Color.White;
			this.hslSaturation.Location = new System.Drawing.Point(86, 56);
			this.hslSaturation.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.hslSaturation.MaxValue = 200D;
			this.hslSaturation.MinValue = -100D;
			this.hslSaturation.Name = "hslSaturation";
			this.hslSaturation.ReverseFill = true;
			this.hslSaturation.Size = new System.Drawing.Size(208, 28);
			this.hslSaturation.TabIndex = 5;
			this.toolTip1.SetToolTip(this.hslSaturation, "Slide to adjust color saturation");
			this.hslSaturation.Value = 100D;
			this.hslSaturation.Changed += new System.EventHandler(this.hslSaturation_Changed);
			// 
			// hslHue
			// 
			this.hslHue.BackColor = System.Drawing.Color.Transparent;
			this.hslHue.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.hslHue.DrawStyle = ElfControls.HSLSlider.eDrawStyle.HueOffset;
			hsl3.Alpha = 1D;
			hsl3.H = 0D;
			hsl3.Hue = 0D;
			hsl3.L = 1D;
			hsl3.Luminance = 1D;
			hsl3.S = 1D;
			hsl3.Saturation = 1D;
			this.hslHue.HSL = hsl3;
			this.hslHue.IndicatorMarks = ((System.Collections.Generic.List<double>)(resources.GetObject("hslHue.IndicatorMarks")));
			this.hslHue.Location = new System.Drawing.Point(86, 22);
			this.hslHue.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.hslHue.MaxValue = 180D;
			this.hslHue.MinValue = -180D;
			this.hslHue.Name = "hslHue";
			this.hslHue.Size = new System.Drawing.Size(208, 28);
			this.hslHue.TabIndex = 1;
			this.toolTip1.SetToolTip(this.hslHue, "Slide to adjust the color of the image");
			this.hslHue.Value = 180D;
			this.hslHue.Changed += new System.EventHandler(this.hslHue_Changed);
			// 
			// cmdSaveModified
			// 
			this.cmdSaveModified.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.cmdSaveModified.Image = global::ElfCore.Properties.Resources.annotation_save;
			this.cmdSaveModified.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdSaveModified.Location = new System.Drawing.Point(284, 376);
			this.cmdSaveModified.Name = "cmdSaveModified";
			this.cmdSaveModified.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdSaveModified.Size = new System.Drawing.Size(127, 25);
			this.cmdSaveModified.TabIndex = 13;
			this.cmdSaveModified.Text = "&Save Modified Image";
			this.cmdSaveModified.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdSaveModified, "Saves the image that has been changed by altering the Hue, Saturation and/or Brig" +
        "htness sliders.");
			this.cmdSaveModified.UseVisualStyleBackColor = true;
			this.cmdSaveModified.Click += new System.EventHandler(this.cmdSaveModified_Click);
			// 
			// cddGrid
			// 
			this.cddGrid.AllowEmptyColor = false;
			this.cddGrid.AnchorSize = new System.Drawing.Size(50, 18);
			this.cddGrid.BackColor = System.Drawing.SystemColors.Control;
			this.cddGrid.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddGrid.Color = System.Drawing.Color.Black;
			this.cddGrid.DisplaySystemColorPalette = false;
			this.cddGrid.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddGrid.EmbiggenHotTracked = true;
			this.cddGrid.HotTracking = true;
			this.cddGrid.Location = new System.Drawing.Point(252, 67);
			this.cddGrid.Name = "cddGrid";
			this.cddGrid.NoColorIndicatorImage = null;
			this.cddGrid.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddGrid.Size = new System.Drawing.Size(50, 18);
			this.cddGrid.TabIndex = 4;
			this.toolTip1.SetToolTip(this.cddGrid, "Select color to be used for the grid if indicated that it should overlay the back" +
        "ground.");
			this.cddGrid.ColorChanged += new System.EventHandler(this.cddGrid_ColorChanged);
			// 
			// cddBackground
			// 
			this.cddBackground.AllowEmptyColor = false;
			this.cddBackground.AnchorSize = new System.Drawing.Size(50, 18);
			this.cddBackground.BackColor = System.Drawing.SystemColors.Control;
			this.cddBackground.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddBackground.Color = System.Drawing.Color.Black;
			this.cddBackground.DisplaySystemColorPalette = false;
			this.cddBackground.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddBackground.EmbiggenHotTracked = true;
			this.cddBackground.HotTracking = true;
			this.cddBackground.Location = new System.Drawing.Point(128, 29);
			this.cddBackground.Name = "cddBackground";
			this.cddBackground.NoColorIndicatorImage = null;
			this.cddBackground.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddBackground.Size = new System.Drawing.Size(50, 18);
			this.cddBackground.TabIndex = 1;
			this.toolTip1.SetToolTip(this.cddBackground, "Select color to be used for the backround. Used if there is no image,or if the im" +
        "age used has transparency.");
			this.cddBackground.ColorChanged += new System.EventHandler(this.cddBackground_ColorChanged);
			// 
			// cmdClear
			// 
			this.cmdClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdClear.Image = global::ElfCore.Properties.Resources.background;
			this.cmdClear.Location = new System.Drawing.Point(761, 28);
			this.cmdClear.Name = "cmdClear";
			this.cmdClear.Size = new System.Drawing.Size(23, 23);
			this.cmdClear.TabIndex = 4;
			this.toolTip1.SetToolTip(this.cmdClear, "Clear out the current background image");
			this.cmdClear.UseVisualStyleBackColor = true;
			this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
			// 
			// cmdSave
			// 
			this.cmdSave.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.cmdSave.Image = global::ElfCore.Properties.Resources.annotation_save;
			this.cmdSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdSave.Location = new System.Drawing.Point(157, 376);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdSave.Size = new System.Drawing.Size(121, 25);
			this.cmdSave.TabIndex = 12;
			this.cmdSave.Text = "&Save Original Image";
			this.cmdSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdSave, "Saves the original image to disk");
			this.cmdSave.UseVisualStyleBackColor = true;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdBrowse
			// 
			this.cmdBrowse.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdBrowse.Image = global::ElfCore.Properties.Resources.open;
			this.cmdBrowse.Location = new System.Drawing.Point(735, 28);
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.Size = new System.Drawing.Size(23, 23);
			this.cmdBrowse.TabIndex = 3;
			this.toolTip1.SetToolTip(this.cmdBrowse, "Select file to be used as the background image");
			this.cmdBrowse.UseVisualStyleBackColor = true;
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// txtSaturation
			// 
			this.txtSaturation.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.txtSaturation.Location = new System.Drawing.Point(306, 56);
			this.txtSaturation.MaxLength = 4;
			this.txtSaturation.Name = "txtSaturation";
			this.txtSaturation.Size = new System.Drawing.Size(35, 22);
			this.txtSaturation.TabIndex = 7;
			this.txtSaturation.Text = "0";
			this.txtSaturation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtSaturation, "Additional saturation to add to background image. Valid values are -200 to +100");
			this.txtSaturation.Visible = false;
			this.txtSaturation.TextChanged += new System.EventHandler(this.txtSaturation_TextChanged);
			this.txtSaturation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SignedNumberOnly_KeyPress);
			this.txtSaturation.Leave += new System.EventHandler(this.txtSaturation_Leave);
			// 
			// txtBrightness
			// 
			this.txtBrightness.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.txtBrightness.Location = new System.Drawing.Point(306, 90);
			this.txtBrightness.MaxLength = 4;
			this.txtBrightness.Name = "txtBrightness";
			this.txtBrightness.Size = new System.Drawing.Size(35, 22);
			this.txtBrightness.TabIndex = 11;
			this.txtBrightness.Text = "-100";
			this.txtBrightness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtBrightness, "Additional brightness to add to background image. Valid values are -100 to +100");
			this.txtBrightness.Visible = false;
			this.txtBrightness.TextChanged += new System.EventHandler(this.txtBrightness_TextChanged);
			this.txtBrightness.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SignedNumberOnly_KeyPress);
			this.txtBrightness.Leave += new System.EventHandler(this.txtBrightness_Leave);
			// 
			// txtHue
			// 
			this.txtHue.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.txtHue.Location = new System.Drawing.Point(306, 22);
			this.txtHue.MaxLength = 4;
			this.txtHue.Name = "txtHue";
			this.txtHue.Size = new System.Drawing.Size(35, 22);
			this.txtHue.TabIndex = 3;
			this.txtHue.Text = "0";
			this.txtHue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtHue, "Amount to adjust image hue (or color). Valid values are -180 to 180");
			this.txtHue.Visible = false;
			this.txtHue.TextChanged += new System.EventHandler(this.txtHue_TextChanged);
			this.txtHue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SignedNumberOnly_KeyPress);
			this.txtHue.Leave += new System.EventHandler(this.txtHue_Leave);
			// 
			// lblBrightness
			// 
			this.lblBrightness.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblBrightness.Location = new System.Drawing.Point(288, 93);
			this.lblBrightness.Name = "lblBrightness";
			this.lblBrightness.Size = new System.Drawing.Size(56, 17);
			this.lblBrightness.TabIndex = 10;
			this.lblBrightness.Text = "0%";
			this.lblBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.lblBrightness, "Additional brightness to add to background image. Valid values are -100 to +100");
			this.lblBrightness.Click += new System.EventHandler(this.lblBrightness_Click);
			// 
			// lblSaturation
			// 
			this.lblSaturation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblSaturation.Location = new System.Drawing.Point(288, 59);
			this.lblSaturation.Name = "lblSaturation";
			this.lblSaturation.Size = new System.Drawing.Size(56, 17);
			this.lblSaturation.TabIndex = 6;
			this.lblSaturation.Text = "100%";
			this.lblSaturation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.lblSaturation, "Additional saturation to add to background image. Valid values are -200 to +100");
			this.lblSaturation.Click += new System.EventHandler(this.lblSaturation_Click);
			// 
			// lblHue
			// 
			this.lblHue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblHue.Location = new System.Drawing.Point(288, 25);
			this.lblHue.Name = "lblHue";
			this.lblHue.Size = new System.Drawing.Size(56, 17);
			this.lblHue.TabIndex = 2;
			this.lblHue.Text = "0°";
			this.lblHue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.lblHue, "Amount to adjust image hue (or color). Valid values are -180 to 180");
			this.lblHue.Click += new System.EventHandler(this.lblHue_Click);
			// 
			// chkShowGrid
			// 
			this.chkShowGrid.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkShowGrid.Location = new System.Drawing.Point(13, 59);
			this.chkShowGrid.Name = "chkShowGrid";
			this.chkShowGrid.Size = new System.Drawing.Size(181, 35);
			this.chkShowGrid.TabIndex = 2;
			this.chkShowGrid.Text = "Overlay Gridline over the background image/color";
			this.toolTip1.SetToolTip(this.chkShowGrid, "Indicates whether a grid should be painted overtop the background image, represen" +
        "ting the cell divisions.");
			this.chkShowGrid.UseVisualStyleBackColor = true;
			this.chkShowGrid.CheckedChanged += new System.EventHandler(this.chkShowGrid_CheckedChanged);
			// 
			// chkSaveEncoded
			// 
			this.chkSaveEncoded.Location = new System.Drawing.Point(426, 58);
			this.chkSaveEncoded.Name = "chkSaveEncoded";
			this.chkSaveEncoded.Size = new System.Drawing.Size(357, 29);
			this.chkSaveEncoded.TabIndex = 5;
			this.chkSaveEncoded.Text = "Save the image data within the Profile itself.";
			this.toolTip1.SetToolTip(this.chkSaveEncoded, "Indicated whether the data for the background image should be encoded within the " +
        "Profile itself, or left on the harddrive");
			this.chkSaveEncoded.UseVisualStyleBackColor = true;
			// 
			// cboPicturePosition
			// 
			this.cboPicturePosition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboPicturePosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPicturePosition.Location = new System.Drawing.Point(426, 103);
			this.cboPicturePosition.Name = "cboPicturePosition";
			this.cboPicturePosition.Size = new System.Drawing.Size(195, 24);
			this.cboPicturePosition.TabIndex = 7;
			this.toolTip1.SetToolTip(this.cboPicturePosition, "Display style of the background image, describing how it fills the canvas.");
			this.cboPicturePosition.SelectedIndexChanged += new System.EventHandler(this.cboPicturePosition_SelectedIndexChanged);
			// 
			// cboAnchor
			// 
			this.cboAnchor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboAnchor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAnchor.Location = new System.Drawing.Point(627, 103);
			this.cboAnchor.Name = "cboAnchor";
			this.cboAnchor.Size = new System.Drawing.Size(157, 24);
			this.cboAnchor.TabIndex = 9;
			this.toolTip1.SetToolTip(this.cboAnchor, "Anchor corner for the background image. Only valid for the Fill mode.");
			this.cboAnchor.SelectedIndexChanged += new System.EventHandler(this.cboAnchor_SelectedIndexChanged);
			// 
			// grpAdjust
			// 
			this.grpAdjust.Controls.Add(this.cmdReset);
			this.grpAdjust.Controls.Add(this.hslBrightness);
			this.grpAdjust.Controls.Add(this.hslSaturation);
			this.grpAdjust.Controls.Add(this.hslHue);
			this.grpAdjust.Controls.Add(this._tbHue);
			this.grpAdjust.Controls.Add(this._tbSaturation);
			this.grpAdjust.Controls.Add(this._tbBrightness);
			this.grpAdjust.Controls.Add(this.lblBrightness);
			this.grpAdjust.Controls.Add(this.lblSaturation);
			this.grpAdjust.Controls.Add(this.lblHue);
			this.grpAdjust.Controls.Add(this.txtSaturation);
			this.grpAdjust.Controls.Add(this.txtBrightness);
			this.grpAdjust.Controls.Add(this.txtHue);
			this.grpAdjust.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.grpAdjust.Location = new System.Drawing.Point(427, 133);
			this.grpAdjust.Name = "grpAdjust";
			this.grpAdjust.Size = new System.Drawing.Size(357, 156);
			this.grpAdjust.TabIndex = 10;
			this.grpAdjust.TabStop = false;
			this.grpAdjust.Text = "Adjust Image";
			// 
			// _tbHue
			// 
			this._tbHue.AutoSize = true;
			this._tbHue.Location = new System.Drawing.Point(48, 24);
			this._tbHue.Name = "_tbHue";
			this._tbHue.Size = new System.Drawing.Size(32, 15);
			this._tbHue.TabIndex = 0;
			this._tbHue.Text = "&Hue:";
			this._tbHue.DoubleClick += new System.EventHandler(this._tbHue_DoubleClick);
			// 
			// _tbSaturation
			// 
			this._tbSaturation.AutoSize = true;
			this._tbSaturation.Location = new System.Drawing.Point(16, 60);
			this._tbSaturation.Name = "_tbSaturation";
			this._tbSaturation.Size = new System.Drawing.Size(64, 15);
			this._tbSaturation.TabIndex = 4;
			this._tbSaturation.Text = "Sat&uration:";
			this._tbSaturation.Click += new System.EventHandler(this._tbSaturation_Click);
			// 
			// _tbBrightness
			// 
			this._tbBrightness.AutoSize = true;
			this._tbBrightness.Location = new System.Drawing.Point(15, 94);
			this._tbBrightness.Name = "_tbBrightness";
			this._tbBrightness.Size = new System.Drawing.Size(65, 15);
			this._tbBrightness.TabIndex = 8;
			this._tbBrightness.Text = "&Brightness:";
			this._tbBrightness.Click += new System.EventHandler(this._tbBrightness_Click);
			// 
			// grpDisplay
			// 
			this.grpDisplay.Controls.Add(this.cddGrid);
			this.grpDisplay.Controls.Add(this.cddBackground);
			this.grpDisplay.Controls.Add(this._qcGridColor);
			this.grpDisplay.Controls.Add(this._qcBackground);
			this.grpDisplay.Controls.Add(this.chkShowGrid);
			this.grpDisplay.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.grpDisplay.Location = new System.Drawing.Point(427, 295);
			this.grpDisplay.Name = "grpDisplay";
			this.grpDisplay.Size = new System.Drawing.Size(357, 106);
			this.grpDisplay.TabIndex = 11;
			this.grpDisplay.TabStop = false;
			this.grpDisplay.Text = "Grid";
			// 
			// _qcGridColor
			// 
			this._qcGridColor.AutoSize = true;
			this._qcGridColor.Location = new System.Drawing.Point(182, 69);
			this._qcGridColor.Name = "_qcGridColor";
			this._qcGridColor.Size = new System.Drawing.Size(64, 15);
			this._qcGridColor.TabIndex = 3;
			this._qcGridColor.Text = "Grid Color:";
			// 
			// _qcBackground
			// 
			this._qcBackground.AutoSize = true;
			this._qcBackground.Location = new System.Drawing.Point(15, 29);
			this._qcBackground.Name = "_qcBackground";
			this._qcBackground.Size = new System.Drawing.Size(106, 15);
			this._qcBackground.TabIndex = 0;
			this._qcBackground.Text = "Background Color:";
			// 
			// OpenImageFileDialog
			// 
			this.OpenImageFileDialog.Filter = "All Image Formats|*.png;*.bmp;*.gif;*.tif|Bitmap File (*.bmp)|*.bmp|JPEG File (*." +
    "jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
			this.OpenImageFileDialog.FilterIndex = 0;
			// 
			// SaveImageFileDialog
			// 
			this.SaveImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File" +
    " (*.gif)|*.gif|All Files (*.*)|*.*";
			this.SaveImageFileDialog.FilterIndex = 5;
			// 
			// tmrSelected
			// 
			this.tmrSelected.Enabled = true;
			this.tmrSelected.Interval = 2000;
			this.tmrSelected.Tick += new System.EventHandler(this.tmrSelected_Tick);
			// 
			// _cboPicturePosition
			// 
			this._cboPicturePosition.AutoSize = true;
			this._cboPicturePosition.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._cboPicturePosition.Location = new System.Drawing.Point(424, 85);
			this._cboPicturePosition.Name = "_cboPicturePosition";
			this._cboPicturePosition.Size = new System.Drawing.Size(91, 15);
			this._cboPicturePosition.TabIndex = 6;
			this._cboPicturePosition.Text = "Wallpaper Style:";
			// 
			// _cboAnchor
			// 
			this._cboAnchor.AutoSize = true;
			this._cboAnchor.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._cboAnchor.Location = new System.Drawing.Point(625, 85);
			this._cboAnchor.Name = "_cboAnchor";
			this._cboAnchor.Size = new System.Drawing.Size(49, 15);
			this._cboAnchor.TabIndex = 8;
			this._cboAnchor.Text = "Anchor:";
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(319, 414);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 14;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(390, 414);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 15;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// pctPreview
			// 
			this.pctPreview.BackColor = System.Drawing.Color.Black;
			this.pctPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pctPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pctPreview.Location = new System.Drawing.Point(12, 27);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Size = new System.Drawing.Size(399, 343);
			this.pctPreview.TabIndex = 10;
			this.pctPreview.TabStop = false;
			this.pctPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pctPreview_Paint);
			// 
			// cmdReset
			// 
			this.cmdReset.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.cmdReset.Image = global::ElfCore.Properties.Resources.undo;
			this.cmdReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdReset.Location = new System.Drawing.Point(87, 124);
			this.cmdReset.Name = "cmdReset";
			this.cmdReset.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdReset.Size = new System.Drawing.Size(64, 25);
			this.cmdReset.TabIndex = 12;
			this.cmdReset.Text = "&Reset";
			this.cmdReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdReset, "Reset sliders to default values.");
			this.cmdReset.UseVisualStyleBackColor = true;
			this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
			// 
			// BackgroundSettings
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(794, 454);
			this.Controls.Add(this.cboAnchor);
			this.Controls.Add(this._cboAnchor);
			this.Controls.Add(this.chkSaveEncoded);
			this.Controls.Add(this.cboPicturePosition);
			this.Controls.Add(this._cboPicturePosition);
			this.Controls.Add(this.cmdSaveModified);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Controls.Add(this.grpDisplay);
			this.Controls.Add(this.grpAdjust);
			this.Controls.Add(this.cmdClear);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cmdBrowse);
			this.Controls.Add(this._txtFilename);
			this.Controls.Add(this.txtFilename);
			this.Controls.Add(this._pctPreview);
			this.Controls.Add(this.pctPreview);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BackgroundSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Background Image Settings";
			this.grpAdjust.ResumeLayout(false);
			this.grpAdjust.PerformLayout();
			this.grpDisplay.ResumeLayout(false);
			this.grpDisplay.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.PictureBox pctPreview;
		private System.Windows.Forms.Label _pctPreview;
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label _txtFilename;
		private System.Windows.Forms.Button cmdBrowse;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.GroupBox grpAdjust;
		private System.Windows.Forms.GroupBox grpDisplay;
		private System.Windows.Forms.CheckBox chkShowGrid;
		private System.Windows.Forms.ColorDialog ChannelColorDialog;
		private System.Windows.Forms.OpenFileDialog OpenImageFileDialog;
		private System.Windows.Forms.SaveFileDialog SaveImageFileDialog;
		private System.Windows.Forms.Timer tmrSelected;
		private System.Windows.Forms.Label _tbSaturation;
		private System.Windows.Forms.Label _tbBrightness;
		private System.Windows.Forms.Label _tbHue;
		private System.Windows.Forms.Label lblBrightness;
		private System.Windows.Forms.Label lblSaturation;
		private System.Windows.Forms.Label lblHue;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.Button cmdSaveModified;
		private ElfControls.HSLSlider hslBrightness;
		private ElfControls.HSLSlider hslSaturation;
		private System.Windows.Forms.Label _qcBackground;
		private System.Windows.Forms.Label _qcGridColor;
		private ElfControls.ColorDropDown cddGrid;
		private ElfControls.ColorDropDown cddBackground;
		private System.Windows.Forms.Label _cboPicturePosition;
		private ElfControls.ImageDropDown cboPicturePosition;
		private System.Windows.Forms.TextBox txtSaturation;
		private System.Windows.Forms.TextBox txtBrightness;
		private System.Windows.Forms.TextBox txtHue;
		private System.Windows.Forms.CheckBox chkSaveEncoded;
		private ElfControls.HSLSlider hslHue;
		private ElfControls.ImageDropDown cboAnchor;
		private System.Windows.Forms.Label _cboAnchor;
		private System.Windows.Forms.Button cmdReset;
	}
}