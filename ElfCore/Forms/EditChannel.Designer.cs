namespace ElfCore.Forms
{
	partial class EditChannel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditChannel));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this._txtName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkEnabled = new System.Windows.Forms.CheckBox();
			this.chkLocked = new System.Windows.Forms.CheckBox();
			this.chkVisible = new System.Windows.Forms.CheckBox();
			this.cddBorderColor = new ElfControls.ColorDropDown();
			this.cddRenderColor = new ElfControls.ColorDropDown();
			this.cddSequencerColor = new ElfControls.ColorDropDown();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this._cddSequencerColor = new System.Windows.Forms.Label();
			this._cddRenderColor = new System.Windows.Forms.Label();
			this._cddBorderColor = new System.Windows.Forms.Label();
			this._lblOutputID = new System.Windows.Forms.Label();
			this.lblOutputID = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(160, 173);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 13;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(89, 173);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 12;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// _txtName
			// 
			this._txtName.AutoSize = true;
			this._txtName.Location = new System.Drawing.Point(9, 10);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(89, 15);
			this._txtName.TabIndex = 0;
			this._txtName.Text = "Channel &Name:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(12, 28);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(294, 23);
			this.txtName.TabIndex = 1;
			this.toolTip1.SetToolTip(this.txtName, "Enter the name of this Channel");
			// 
			// chkEnabled
			// 
			this.chkEnabled.AutoSize = true;
			this.chkEnabled.Location = new System.Drawing.Point(178, 83);
			this.chkEnabled.Name = "chkEnabled";
			this.chkEnabled.Size = new System.Drawing.Size(68, 19);
			this.chkEnabled.TabIndex = 4;
			this.chkEnabled.Text = "&Enabled";
			this.toolTip1.SetToolTip(this.chkEnabled, "Check to prevent this Channel from rendering during playback");
			this.chkEnabled.UseVisualStyleBackColor = true;
			// 
			// chkLocked
			// 
			this.chkLocked.AutoSize = true;
			this.chkLocked.Location = new System.Drawing.Point(178, 110);
			this.chkLocked.Name = "chkLocked";
			this.chkLocked.Size = new System.Drawing.Size(64, 19);
			this.chkLocked.TabIndex = 7;
			this.chkLocked.Text = "&Locked";
			this.toolTip1.SetToolTip(this.chkLocked, "Check to prevent editing on this Channel. No effect during playback.");
			this.chkLocked.UseVisualStyleBackColor = true;
			// 
			// chkVisible
			// 
			this.chkVisible.AutoSize = true;
			this.chkVisible.Location = new System.Drawing.Point(178, 137);
			this.chkVisible.Name = "chkVisible";
			this.chkVisible.Size = new System.Drawing.Size(60, 19);
			this.chkVisible.TabIndex = 11;
			this.chkVisible.Text = "&Visible";
			this.toolTip1.SetToolTip(this.chkVisible, "Check to make this Channel visible during editing. No effect during playback");
			this.chkVisible.UseVisualStyleBackColor = true;
			// 
			// cddBorderColor
			// 
			this.cddBorderColor.AnchorSize = new System.Drawing.Size(50, 21);
			this.cddBorderColor.BackColor = System.Drawing.Color.White;
			this.cddBorderColor.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddBorderColor.Color = System.Drawing.Color.Empty;
			this.cddBorderColor.DisplaySystemColorPalette = false;
			this.cddBorderColor.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddBorderColor.EmbiggenHotTracked = true;
			this.cddBorderColor.HotTracking = true;
			this.cddBorderColor.Location = new System.Drawing.Point(122, 136);
			this.cddBorderColor.Name = "cddBorderColor";
			this.cddBorderColor.NoColorIndicatorImage = global::ElfCore.Properties.Resources.cancel;
			this.cddBorderColor.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddBorderColor.Size = new System.Drawing.Size(50, 21);
			this.cddBorderColor.TabIndex = 10;
			this.toolTip1.SetToolTip(this.cddBorderColor, "Border color used to wrap Cells and/or Vector glyphs");
			// 
			// cddRenderColor
			// 
			this.cddRenderColor.AnchorSize = new System.Drawing.Size(50, 21);
			this.cddRenderColor.BackColor = System.Drawing.Color.White;
			this.cddRenderColor.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddRenderColor.Color = System.Drawing.Color.Empty;
			this.cddRenderColor.DisplaySystemColorPalette = false;
			this.cddRenderColor.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddRenderColor.EmbiggenHotTracked = true;
			this.cddRenderColor.HotTracking = true;
			this.cddRenderColor.Location = new System.Drawing.Point(122, 109);
			this.cddRenderColor.Name = "cddRenderColor";
			this.cddRenderColor.NoColorIndicatorImage = global::ElfCore.Properties.Resources.cancel;
			this.cddRenderColor.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddRenderColor.Size = new System.Drawing.Size(50, 21);
			this.cddRenderColor.TabIndex = 6;
			this.toolTip1.SetToolTip(this.cddRenderColor, "Color used for rendering Channel during playback. If not set, uses the Sequencer " +
        "color.");
			// 
			// cddSequencerColor
			// 
			this.cddSequencerColor.AnchorSize = new System.Drawing.Size(50, 21);
			this.cddSequencerColor.BackColor = System.Drawing.Color.White;
			this.cddSequencerColor.CancelButtonImage = global::ElfCore.Properties.Resources.cancel;
			this.cddSequencerColor.Color = System.Drawing.Color.Empty;
			this.cddSequencerColor.DisplaySystemColorPalette = false;
			this.cddSequencerColor.DropdownSize = new System.Drawing.Size(144, 300);
			this.cddSequencerColor.EmbiggenHotTracked = true;
			this.cddSequencerColor.HotTracking = true;
			this.cddSequencerColor.Location = new System.Drawing.Point(122, 82);
			this.cddSequencerColor.Name = "cddSequencerColor";
			this.cddSequencerColor.NoColorIndicatorImage = global::ElfCore.Properties.Resources.cancel;
			this.cddSequencerColor.OKButtonImage = global::ElfCore.Properties.Resources.check;
			this.cddSequencerColor.Size = new System.Drawing.Size(50, 21);
			this.cddSequencerColor.TabIndex = 3;
			this.toolTip1.SetToolTip(this.cddSequencerColor, "Color used to render on the Sequencing program.");
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// _cddSequencerColor
			// 
			this._cddSequencerColor.AutoSize = true;
			this._cddSequencerColor.Location = new System.Drawing.Point(19, 85);
			this._cddSequencerColor.Name = "_cddSequencerColor";
			this._cddSequencerColor.Size = new System.Drawing.Size(97, 15);
			this._cddSequencerColor.TabIndex = 2;
			this._cddSequencerColor.Text = "Se&quencer Color:";
			// 
			// _cddRenderColor
			// 
			this._cddRenderColor.AutoSize = true;
			this._cddRenderColor.Location = new System.Drawing.Point(24, 112);
			this._cddRenderColor.Name = "_cddRenderColor";
			this._cddRenderColor.Size = new System.Drawing.Size(92, 15);
			this._cddRenderColor.TabIndex = 5;
			this._cddRenderColor.Text = "&Rendered Color:";
			// 
			// _cddBorderColor
			// 
			this._cddBorderColor.AutoSize = true;
			this._cddBorderColor.Location = new System.Drawing.Point(39, 139);
			this._cddBorderColor.Name = "_cddBorderColor";
			this._cddBorderColor.Size = new System.Drawing.Size(77, 15);
			this._cddBorderColor.TabIndex = 8;
			this._cddBorderColor.Text = "&Border Color:";
			// 
			// _lblOutputID
			// 
			this._lblOutputID.AutoSize = true;
			this._lblOutputID.Location = new System.Drawing.Point(54, 63);
			this._lblOutputID.Name = "_lblOutputID";
			this._lblOutputID.Size = new System.Drawing.Size(62, 15);
			this._lblOutputID.TabIndex = 14;
			this._lblOutputID.Text = "Output ID:";
			// 
			// lblOutputID
			// 
			this.lblOutputID.AutoSize = true;
			this.lblOutputID.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOutputID.Location = new System.Drawing.Point(118, 58);
			this.lblOutputID.Name = "lblOutputID";
			this.lblOutputID.Size = new System.Drawing.Size(28, 21);
			this.lblOutputID.TabIndex = 15;
			this.lblOutputID.Text = "00";
			// 
			// EditChannel
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(321, 221);
			this.Controls.Add(this.lblOutputID);
			this.Controls.Add(this._lblOutputID);
			this.Controls.Add(this.chkVisible);
			this.Controls.Add(this.cddBorderColor);
			this.Controls.Add(this._cddBorderColor);
			this.Controls.Add(this.chkLocked);
			this.Controls.Add(this.cddRenderColor);
			this.Controls.Add(this._cddRenderColor);
			this.Controls.Add(this.chkEnabled);
			this.Controls.Add(this.cddSequencerColor);
			this.Controls.Add(this._cddSequencerColor);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditChannel";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Channel";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Label _txtName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Label _cddSequencerColor;
		private System.Windows.Forms.CheckBox chkEnabled;
		private ElfControls.ColorDropDown cddSequencerColor;
		private System.Windows.Forms.CheckBox chkLocked;
		private ElfControls.ColorDropDown cddRenderColor;
		private System.Windows.Forms.Label _cddRenderColor;
		private ElfControls.ColorDropDown cddBorderColor;
		private System.Windows.Forms.Label _cddBorderColor;
		private System.Windows.Forms.CheckBox chkVisible;
		private System.Windows.Forms.Label lblOutputID;
		private System.Windows.Forms.Label _lblOutputID;
	}
}