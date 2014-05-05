namespace ElfCore
{
	partial class ChangeLatticeSize
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeLatticeSize));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.txtXRes = new System.Windows.Forms.TextBox();
			this._txtXRes = new System.Windows.Forms.Label();
			this.txtYRes = new System.Windows.Forms.TextBox();
			this._txtYRes = new System.Windows.Forms.Label();
			this.lblX = new System.Windows.Forms.Label();
			this.lblCells = new System.Windows.Forms.Label();
			this.grpBackgroundImage = new System.Windows.Forms.GroupBox();
			this.lblNoBGImage = new System.Windows.Forms.Label();
			this.cmdResize = new System.Windows.Forms.Button();
			this.lblBGYRes = new System.Windows.Forms.Label();
			this.lblBGXRes = new System.Windows.Forms.Label();
			this.lblBGCells = new System.Windows.Forms.Label();
			this.lblBGX2 = new System.Windows.Forms.Label();
			this.lblBGHeight = new System.Windows.Forms.Label();
			this.lblBGWidth = new System.Windows.Forms.Label();
			this.lblBGPixels = new System.Windows.Forms.Label();
			this.lblBGX1 = new System.Windows.Forms.Label();
			this._lblBGHeight = new System.Windows.Forms.Label();
			this._lblBGWidth = new System.Windows.Forms.Label();
			this.grpBackgroundImage.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.not;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(108, 243);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 28);
			this.cmdCancel.TabIndex = 13;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.Ok;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(37, 243);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 12;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// txtXRes
			// 
			this.txtXRes.BackColor = System.Drawing.SystemColors.Window;
			this.txtXRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtXRes.Location = new System.Drawing.Point(52, 36);
			this.txtXRes.MaxLength = 4;
			this.txtXRes.Name = "txtXRes";
			this.txtXRes.Size = new System.Drawing.Size(33, 20);
			this.txtXRes.TabIndex = 15;
			this.txtXRes.Text = "64";
			this.txtXRes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// _txtXRes
			// 
			this._txtXRes.AutoSize = true;
			this._txtXRes.Location = new System.Drawing.Point(49, 20);
			this._txtXRes.Name = "_txtXRes";
			this._txtXRes.Size = new System.Drawing.Size(38, 13);
			this._txtXRes.TabIndex = 14;
			this._txtXRes.Text = "Width:";
			// 
			// txtYRes
			// 
			this.txtYRes.BackColor = System.Drawing.SystemColors.Window;
			this.txtYRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtYRes.Location = new System.Drawing.Point(111, 36);
			this.txtYRes.MaxLength = 4;
			this.txtYRes.Name = "txtYRes";
			this.txtYRes.Size = new System.Drawing.Size(33, 20);
			this.txtYRes.TabIndex = 17;
			this.txtYRes.Text = "32";
			this.txtYRes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// _txtYRes
			// 
			this._txtYRes.AutoSize = true;
			this._txtYRes.Location = new System.Drawing.Point(108, 20);
			this._txtYRes.Name = "_txtYRes";
			this._txtYRes.Size = new System.Drawing.Size(41, 13);
			this._txtYRes.TabIndex = 16;
			this._txtYRes.Text = "Height:";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblX.Location = new System.Drawing.Point(91, 38);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(15, 13);
			this.lblX.TabIndex = 18;
			this.lblX.Text = "X";
			// 
			// lblCells
			// 
			this.lblCells.AutoSize = true;
			this.lblCells.Location = new System.Drawing.Point(150, 38);
			this.lblCells.Name = "lblCells";
			this.lblCells.Size = new System.Drawing.Size(29, 13);
			this.lblCells.TabIndex = 19;
			this.lblCells.Text = "Cells";
			// 
			// grpBackgroundImage
			// 
			this.grpBackgroundImage.Controls.Add(this.lblNoBGImage);
			this.grpBackgroundImage.Controls.Add(this.cmdResize);
			this.grpBackgroundImage.Controls.Add(this.lblBGYRes);
			this.grpBackgroundImage.Controls.Add(this.lblBGXRes);
			this.grpBackgroundImage.Controls.Add(this.lblBGCells);
			this.grpBackgroundImage.Controls.Add(this.lblBGX2);
			this.grpBackgroundImage.Controls.Add(this.lblBGHeight);
			this.grpBackgroundImage.Controls.Add(this.lblBGWidth);
			this.grpBackgroundImage.Controls.Add(this.lblBGPixels);
			this.grpBackgroundImage.Controls.Add(this.lblBGX1);
			this.grpBackgroundImage.Controls.Add(this._lblBGHeight);
			this.grpBackgroundImage.Controls.Add(this._lblBGWidth);
			this.grpBackgroundImage.Location = new System.Drawing.Point(17, 73);
			this.grpBackgroundImage.Name = "grpBackgroundImage";
			this.grpBackgroundImage.Size = new System.Drawing.Size(187, 151);
			this.grpBackgroundImage.TabIndex = 20;
			this.grpBackgroundImage.TabStop = false;
			this.grpBackgroundImage.Text = "Background Image";
			// 
			// lblNoBGImage
			// 
			this.lblNoBGImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNoBGImage.ForeColor = System.Drawing.Color.Red;
			this.lblNoBGImage.Location = new System.Drawing.Point(42, 52);
			this.lblNoBGImage.Name = "lblNoBGImage";
			this.lblNoBGImage.Size = new System.Drawing.Size(102, 47);
			this.lblNoBGImage.TabIndex = 33;
			this.lblNoBGImage.Text = "No Background Image Loaded";
			this.lblNoBGImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblNoBGImage.Visible = false;
			// 
			// cmdResize
			// 
			this.cmdResize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdResize.Image = global::ElfCore.Properties.Resources.image_resize;
			this.cmdResize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdResize.Location = new System.Drawing.Point(17, 88);
			this.cmdResize.Name = "cmdResize";
			this.cmdResize.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdResize.Size = new System.Drawing.Size(149, 42);
			this.cmdResize.TabIndex = 32;
			this.cmdResize.Text = "Resize Canvas to Fit Background Image";
			this.cmdResize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdResize.UseVisualStyleBackColor = true;
			this.cmdResize.Click += new System.EventHandler(this.cmdResize_Click);
			// 
			// lblBGYRes
			// 
			this.lblBGYRes.AutoSize = true;
			this.lblBGYRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGYRes.Location = new System.Drawing.Point(78, 63);
			this.lblBGYRes.Name = "lblBGYRes";
			this.lblBGYRes.Size = new System.Drawing.Size(21, 13);
			this.lblBGYRes.TabIndex = 31;
			this.lblBGYRes.Text = "32";
			// 
			// lblBGXRes
			// 
			this.lblBGXRes.AutoSize = true;
			this.lblBGXRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGXRes.Location = new System.Drawing.Point(26, 63);
			this.lblBGXRes.Name = "lblBGXRes";
			this.lblBGXRes.Size = new System.Drawing.Size(21, 13);
			this.lblBGXRes.TabIndex = 30;
			this.lblBGXRes.Text = "64";
			// 
			// lblBGCells
			// 
			this.lblBGCells.AutoSize = true;
			this.lblBGCells.Location = new System.Drawing.Point(115, 63);
			this.lblBGCells.Name = "lblBGCells";
			this.lblBGCells.Size = new System.Drawing.Size(29, 13);
			this.lblBGCells.TabIndex = 29;
			this.lblBGCells.Text = "Cells";
			// 
			// lblBGX2
			// 
			this.lblBGX2.AutoSize = true;
			this.lblBGX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGX2.Location = new System.Drawing.Point(63, 63);
			this.lblBGX2.Name = "lblBGX2";
			this.lblBGX2.Size = new System.Drawing.Size(13, 13);
			this.lblBGX2.TabIndex = 28;
			this.lblBGX2.Text = "x";
			// 
			// lblBGHeight
			// 
			this.lblBGHeight.AutoSize = true;
			this.lblBGHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGHeight.Location = new System.Drawing.Point(78, 45);
			this.lblBGHeight.Name = "lblBGHeight";
			this.lblBGHeight.Size = new System.Drawing.Size(28, 13);
			this.lblBGHeight.TabIndex = 27;
			this.lblBGHeight.Text = "200";
			// 
			// lblBGWidth
			// 
			this.lblBGWidth.AutoSize = true;
			this.lblBGWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGWidth.Location = new System.Drawing.Point(26, 45);
			this.lblBGWidth.Name = "lblBGWidth";
			this.lblBGWidth.Size = new System.Drawing.Size(28, 13);
			this.lblBGWidth.TabIndex = 26;
			this.lblBGWidth.Text = "400";
			// 
			// lblBGPixels
			// 
			this.lblBGPixels.AutoSize = true;
			this.lblBGPixels.Location = new System.Drawing.Point(115, 45);
			this.lblBGPixels.Name = "lblBGPixels";
			this.lblBGPixels.Size = new System.Drawing.Size(34, 13);
			this.lblBGPixels.TabIndex = 25;
			this.lblBGPixels.Text = "Pixels";
			// 
			// lblBGX1
			// 
			this.lblBGX1.AutoSize = true;
			this.lblBGX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGX1.Location = new System.Drawing.Point(63, 45);
			this.lblBGX1.Name = "lblBGX1";
			this.lblBGX1.Size = new System.Drawing.Size(13, 13);
			this.lblBGX1.TabIndex = 24;
			this.lblBGX1.Text = "x";
			// 
			// _lblBGHeight
			// 
			this._lblBGHeight.AutoSize = true;
			this._lblBGHeight.Location = new System.Drawing.Point(78, 27);
			this._lblBGHeight.Name = "_lblBGHeight";
			this._lblBGHeight.Size = new System.Drawing.Size(41, 13);
			this._lblBGHeight.TabIndex = 22;
			this._lblBGHeight.Text = "Height:";
			// 
			// _lblBGWidth
			// 
			this._lblBGWidth.AutoSize = true;
			this._lblBGWidth.Location = new System.Drawing.Point(26, 27);
			this._lblBGWidth.Name = "_lblBGWidth";
			this._lblBGWidth.Size = new System.Drawing.Size(38, 13);
			this._lblBGWidth.TabIndex = 20;
			this._lblBGWidth.Text = "Width:";
			// 
			// ChangeLatticeSize
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(229, 297);
			this.Controls.Add(this.grpBackgroundImage);
			this.Controls.Add(this.lblCells);
			this.Controls.Add(this.lblX);
			this.Controls.Add(this.txtYRes);
			this.Controls.Add(this._txtYRes);
			this.Controls.Add(this.txtXRes);
			this.Controls.Add(this._txtXRes);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangeLatticeSize";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Canvas Size";
			this.grpBackgroundImage.ResumeLayout(false);
			this.grpBackgroundImage.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.TextBox txtXRes;
		private System.Windows.Forms.Label _txtXRes;
		private System.Windows.Forms.TextBox txtYRes;
		private System.Windows.Forms.Label _txtYRes;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblCells;
		private System.Windows.Forms.GroupBox grpBackgroundImage;
		private System.Windows.Forms.Label lblNoBGImage;
		private System.Windows.Forms.Button cmdResize;
		private System.Windows.Forms.Label lblBGYRes;
		private System.Windows.Forms.Label lblBGXRes;
		private System.Windows.Forms.Label lblBGCells;
		private System.Windows.Forms.Label lblBGX2;
		private System.Windows.Forms.Label lblBGHeight;
		private System.Windows.Forms.Label lblBGWidth;
		private System.Windows.Forms.Label lblBGPixels;
		private System.Windows.Forms.Label lblBGX1;
		private System.Windows.Forms.Label _lblBGHeight;
		private System.Windows.Forms.Label _lblBGWidth;
	}
}