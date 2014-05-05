namespace ElfCore.Forms
{
	partial class SetInactiveAlpha
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
			ElfControls.ColorManager.HSL hsl1 = new ElfControls.ColorManager.HSL();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetInactiveAlpha));
			this._pctPreview = new System.Windows.Forms.Label();
			this.hslAlpha = new ElfControls.HSLSlider();
			this.tmrAnimation = new System.Windows.Forms.Timer(this.components);
			this.lblAlpha = new System.Windows.Forms.Label();
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// _pctPreview
			// 
			this._pctPreview.AutoSize = true;
			this._pctPreview.Location = new System.Drawing.Point(9, 6);
			this._pctPreview.Name = "_pctPreview";
			this._pctPreview.Size = new System.Drawing.Size(48, 13);
			this._pctPreview.TabIndex = 0;
			this._pctPreview.Text = "Preview:";
			// 
			// hslAlpha
			// 
			this.hslAlpha.BackColor = System.Drawing.Color.Transparent;
			this.hslAlpha.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.hslAlpha.DrawStyle = ElfControls.HSLSlider.eDrawStyle.Transparency;
			hsl1.Alpha = 1D;
			hsl1.H = 0D;
			hsl1.Hue = 0D;
			hsl1.L = 0D;
			hsl1.Luminance = 0D;
			hsl1.S = 1D;
			hsl1.Saturation = 1D;
			this.hslAlpha.HSL = hsl1;
			this.hslAlpha.IndicatorMarks = ((System.Collections.Generic.List<double>)(resources.GetObject("hslAlpha.IndicatorMarks")));
			this.hslAlpha.Location = new System.Drawing.Point(12, 315);
			this.hslAlpha.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.hslAlpha.MaxValue = 255D;
			this.hslAlpha.Name = "hslAlpha";
			this.hslAlpha.ReverseFill = true;
			this.hslAlpha.Size = new System.Drawing.Size(399, 28);
			this.hslAlpha.TabIndex = 1;
			this.hslAlpha.Value = 128D;
			this.hslAlpha.Changed += new System.EventHandler(this.hslAlpha_Changed);
			// 
			// tmrAnimation
			// 
			this.tmrAnimation.Enabled = true;
			this.tmrAnimation.Interval = 2000;
			this.tmrAnimation.Tick += new System.EventHandler(this.tmrAnimation_Tick);
			// 
			// lblAlpha
			// 
			this.lblAlpha.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblAlpha.Location = new System.Drawing.Point(183, 346);
			this.lblAlpha.Name = "lblAlpha";
			this.lblAlpha.Size = new System.Drawing.Size(56, 20);
			this.lblAlpha.TabIndex = 2;
			this.lblAlpha.Text = "50%";
			this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(134, 369);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 3;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(205, 369);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 4;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// pctPreview
			// 
			this.pctPreview.BackColor = System.Drawing.Color.Black;
			this.pctPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pctPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pctPreview.Location = new System.Drawing.Point(12, 24);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Size = new System.Drawing.Size(399, 285);
			this.pctPreview.TabIndex = 17;
			this.pctPreview.TabStop = false;
			this.pctPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pctPreview_Paint);
			// 
			// SetInactiveAlpha
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(425, 407);
			this.Controls.Add(this.lblAlpha);
			this.Controls.Add(this.hslAlpha);
			this.Controls.Add(this._pctPreview);
			this.Controls.Add(this.pctPreview);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetInactiveAlpha";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Inactive Channel Transparency";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Label _pctPreview;
		private System.Windows.Forms.PictureBox pctPreview;
		private ElfControls.HSLSlider hslAlpha;
		private System.Windows.Forms.Timer tmrAnimation;
		private System.Windows.Forms.Label lblAlpha;
	}
}