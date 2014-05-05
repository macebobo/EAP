namespace ElfCore.Forms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeLatticeSize));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
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
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this._txtHeight = new System.Windows.Forms.Label();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this._txtWidth = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.grpBackgroundImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(151, 155);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(80, 155);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 5;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
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
			this.grpBackgroundImage.Location = new System.Drawing.Point(19, 42);
			this.grpBackgroundImage.Name = "grpBackgroundImage";
			this.grpBackgroundImage.Size = new System.Drawing.Size(269, 107);
			this.grpBackgroundImage.TabIndex = 4;
			this.grpBackgroundImage.TabStop = false;
			this.grpBackgroundImage.Text = "Background Image";
			// 
			// lblNoBGImage
			// 
			this.lblNoBGImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNoBGImage.ForeColor = System.Drawing.Color.Red;
			this.lblNoBGImage.Location = new System.Drawing.Point(83, 30);
			this.lblNoBGImage.Name = "lblNoBGImage";
			this.lblNoBGImage.Size = new System.Drawing.Size(102, 47);
			this.lblNoBGImage.TabIndex = 10;
			this.lblNoBGImage.Text = "No Background Image Loaded";
			this.lblNoBGImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblNoBGImage.Visible = false;
			// 
			// cmdResize
			// 
			this.cmdResize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdResize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdResize.Location = new System.Drawing.Point(155, 19);
			this.cmdResize.Name = "cmdResize";
			this.cmdResize.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdResize.Size = new System.Drawing.Size(95, 69);
			this.cmdResize.TabIndex = 11;
			this.cmdResize.Text = "Resize Canvas to Fit Background Image";
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
			this.lblBGYRes.TabIndex = 8;
			this.lblBGYRes.Text = "32";
			// 
			// lblBGXRes
			// 
			this.lblBGXRes.AutoSize = true;
			this.lblBGXRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGXRes.Location = new System.Drawing.Point(26, 63);
			this.lblBGXRes.Name = "lblBGXRes";
			this.lblBGXRes.Size = new System.Drawing.Size(21, 13);
			this.lblBGXRes.TabIndex = 3;
			this.lblBGXRes.Text = "64";
			// 
			// lblBGCells
			// 
			this.lblBGCells.AutoSize = true;
			this.lblBGCells.Location = new System.Drawing.Point(115, 63);
			this.lblBGCells.Name = "lblBGCells";
			this.lblBGCells.Size = new System.Drawing.Size(29, 13);
			this.lblBGCells.TabIndex = 9;
			this.lblBGCells.Text = "Cells";
			// 
			// lblBGX2
			// 
			this.lblBGX2.AutoSize = true;
			this.lblBGX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGX2.Location = new System.Drawing.Point(63, 63);
			this.lblBGX2.Name = "lblBGX2";
			this.lblBGX2.Size = new System.Drawing.Size(13, 13);
			this.lblBGX2.TabIndex = 4;
			this.lblBGX2.Text = "x";
			// 
			// lblBGHeight
			// 
			this.lblBGHeight.AutoSize = true;
			this.lblBGHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGHeight.Location = new System.Drawing.Point(78, 45);
			this.lblBGHeight.Name = "lblBGHeight";
			this.lblBGHeight.Size = new System.Drawing.Size(28, 13);
			this.lblBGHeight.TabIndex = 6;
			this.lblBGHeight.Text = "200";
			// 
			// lblBGWidth
			// 
			this.lblBGWidth.AutoSize = true;
			this.lblBGWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGWidth.Location = new System.Drawing.Point(26, 45);
			this.lblBGWidth.Name = "lblBGWidth";
			this.lblBGWidth.Size = new System.Drawing.Size(28, 13);
			this.lblBGWidth.TabIndex = 1;
			this.lblBGWidth.Text = "400";
			// 
			// lblBGPixels
			// 
			this.lblBGPixels.AutoSize = true;
			this.lblBGPixels.Location = new System.Drawing.Point(115, 45);
			this.lblBGPixels.Name = "lblBGPixels";
			this.lblBGPixels.Size = new System.Drawing.Size(34, 13);
			this.lblBGPixels.TabIndex = 7;
			this.lblBGPixels.Text = "Pixels";
			// 
			// lblBGX1
			// 
			this.lblBGX1.AutoSize = true;
			this.lblBGX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBGX1.Location = new System.Drawing.Point(63, 45);
			this.lblBGX1.Name = "lblBGX1";
			this.lblBGX1.Size = new System.Drawing.Size(13, 13);
			this.lblBGX1.TabIndex = 2;
			this.lblBGX1.Text = "x";
			// 
			// _lblBGHeight
			// 
			this._lblBGHeight.AutoSize = true;
			this._lblBGHeight.Location = new System.Drawing.Point(78, 27);
			this._lblBGHeight.Name = "_lblBGHeight";
			this._lblBGHeight.Size = new System.Drawing.Size(41, 13);
			this._lblBGHeight.TabIndex = 5;
			this._lblBGHeight.Text = "Height:";
			// 
			// _lblBGWidth
			// 
			this._lblBGWidth.AutoSize = true;
			this._lblBGWidth.Location = new System.Drawing.Point(26, 27);
			this._lblBGWidth.Name = "_lblBGWidth";
			this._lblBGWidth.Size = new System.Drawing.Size(38, 13);
			this._lblBGWidth.TabIndex = 0;
			this._lblBGWidth.Text = "Width:";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// _txtHeight
			// 
			this._txtHeight.Location = new System.Drawing.Point(198, 18);
			this._txtHeight.Name = "_txtHeight";
			this._txtHeight.Size = new System.Drawing.Size(41, 16);
			this._txtHeight.TabIndex = 2;
			this._txtHeight.Text = "&Height:";
			// 
			// txtHeight
			// 
			this.txtHeight.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtHeight.Location = new System.Drawing.Point(245, 12);
			this.txtHeight.MaxLength = 4;
			this.txtHeight.Multiline = true;
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(43, 24);
			this.txtHeight.TabIndex = 3;
			this.txtHeight.Text = "32";
			this.txtHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtHeight.Enter += new System.EventHandler(this.TextBox_Enter);
			this.txtHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// _txtWidth
			// 
			this._txtWidth.Image = global::ElfCore.Properties.Resources.resize_lattice;
			this._txtWidth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._txtWidth.Location = new System.Drawing.Point(16, 15);
			this._txtWidth.Name = "_txtWidth";
			this._txtWidth.Size = new System.Drawing.Size(127, 19);
			this._txtWidth.TabIndex = 0;
			this._txtWidth.Text = "Canvas Size  &Width:";
			this._txtWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtWidth
			// 
			this.txtWidth.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtWidth.Location = new System.Drawing.Point(149, 12);
			this.txtWidth.MaxLength = 4;
			this.txtWidth.Multiline = true;
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(43, 24);
			this.txtWidth.TabIndex = 1;
			this.txtWidth.Text = "64";
			this.txtWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtWidth.Enter += new System.EventHandler(this.TextBox_Enter);
			this.txtWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			// 
			// ChangeLatticeSize
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(316, 196);
			this.Controls.Add(this.grpBackgroundImage);
			this.Controls.Add(this._txtHeight);
			this.Controls.Add(this.txtHeight);
			this.Controls.Add(this._txtWidth);
			this.Controls.Add(this.txtWidth);
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
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
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
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Label _txtHeight;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.Label _txtWidth;
		private System.Windows.Forms.TextBox txtWidth;
	}
}