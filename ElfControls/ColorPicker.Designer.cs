namespace ElfControls
{
	partial class ColorPicker
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ElfControls.ColorManager.HSL hsl1 = new ElfControls.ColorManager.HSL();
			this.lblOriginalColor = new System.Windows.Forms.Label();
			this.txtBlue = new System.Windows.Forms.TextBox();
			this.txtGreen = new System.Windows.Forms.TextBox();
			this.txtRed = new System.Windows.Forms.TextBox();
			this.txtLuminance = new System.Windows.Forms.TextBox();
			this.txtSat = new System.Windows.Forms.TextBox();
			this.txtHue = new System.Windows.Forms.TextBox();
			this._txtRed = new System.Windows.Forms.Label();
			this._txtGreen = new System.Windows.Forms.Label();
			this._txtBlue = new System.Windows.Forms.Label();
			this._txtLuminance = new System.Windows.Forms.Label();
			this._txtSaturation = new System.Windows.Forms.Label();
			this._txtHue = new System.Windows.Forms.Label();
			this.txtHex = new System.Windows.Forms.TextBox();
			this._txtHex = new System.Windows.Forms.Label();
			this._txtName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pnlPendingColor = new ElfControls.ColorPanel();
			this._lblDegree = new System.Windows.Forms.Label();
			this.lblPercent1 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.colorWheel1 = new ElfControls.ColorWheel();
			this.SuspendLayout();
			// 
			// lblOriginalColor
			// 
			this.lblOriginalColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.lblOriginalColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOriginalColor.Location = new System.Drawing.Point(178, 16);
			this.lblOriginalColor.Name = "lblOriginalColor";
			this.lblOriginalColor.Size = new System.Drawing.Size(25, 25);
			this.lblOriginalColor.TabIndex = 2;
			this.toolTip1.SetToolTip(this.lblOriginalColor, "Original Color");
			// 
			// txtBlue
			// 
			this.txtBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtBlue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtBlue.Location = new System.Drawing.Point(169, 243);
			this.txtBlue.MaxLength = 3;
			this.txtBlue.Multiline = true;
			this.txtBlue.Name = "txtBlue";
			this.txtBlue.Size = new System.Drawing.Size(30, 18);
			this.txtBlue.TabIndex = 14;
			this.txtBlue.Tag = "255";
			this.txtBlue.Text = "255";
			this.txtBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtBlue, "Amount of Blue that makes up this color.");
			this.txtBlue.TextChanged += new System.EventHandler(this.ValueRangeCheck_TextChanged);
			this.txtBlue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtBlue.Leave += new System.EventHandler(this.txtBlue_Leave);
			// 
			// txtGreen
			// 
			this.txtGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtGreen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGreen.Location = new System.Drawing.Point(169, 219);
			this.txtGreen.MaxLength = 3;
			this.txtGreen.Multiline = true;
			this.txtGreen.Name = "txtGreen";
			this.txtGreen.Size = new System.Drawing.Size(30, 18);
			this.txtGreen.TabIndex = 12;
			this.txtGreen.Tag = "255";
			this.txtGreen.Text = "255";
			this.txtGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtGreen, "Amount of Green that makes up this color.");
			this.txtGreen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtGreen.Leave += new System.EventHandler(this.txtGreen_Leave);
			// 
			// txtRed
			// 
			this.txtRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtRed.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtRed.Location = new System.Drawing.Point(169, 195);
			this.txtRed.MaxLength = 3;
			this.txtRed.Multiline = true;
			this.txtRed.Name = "txtRed";
			this.txtRed.Size = new System.Drawing.Size(30, 18);
			this.txtRed.TabIndex = 10;
			this.txtRed.Tag = "255";
			this.txtRed.Text = "255";
			this.txtRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtRed, "Amount of Red that makes up this color.");
			this.txtRed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtRed.Leave += new System.EventHandler(this.txtRed_Leave);
			// 
			// txtLuminance
			// 
			this.txtLuminance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtLuminance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLuminance.Location = new System.Drawing.Point(78, 243);
			this.txtLuminance.MaxLength = 3;
			this.txtLuminance.Multiline = true;
			this.txtLuminance.Name = "txtLuminance";
			this.txtLuminance.Size = new System.Drawing.Size(30, 18);
			this.txtLuminance.TabIndex = 8;
			this.txtLuminance.Tag = "100";
			this.txtLuminance.Text = "100";
			this.txtLuminance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtLuminance, "Brightness of the color. Represented as percent away from Black.");
			this.txtLuminance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtLuminance.Leave += new System.EventHandler(this.txtLuminance_Leave);
			// 
			// txtSat
			// 
			this.txtSat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtSat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSat.Location = new System.Drawing.Point(78, 219);
			this.txtSat.MaxLength = 3;
			this.txtSat.Multiline = true;
			this.txtSat.Name = "txtSat";
			this.txtSat.Size = new System.Drawing.Size(30, 18);
			this.txtSat.TabIndex = 6;
			this.txtSat.Tag = "100";
			this.txtSat.Text = "100";
			this.txtSat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtSat, "Saturation of the color. Represented as percent away from Gray.");
			this.txtSat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtSat.Leave += new System.EventHandler(this.txtSat_Leave);
			// 
			// txtHue
			// 
			this.txtHue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtHue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtHue.Location = new System.Drawing.Point(78, 195);
			this.txtHue.MaxLength = 3;
			this.txtHue.Multiline = true;
			this.txtHue.Name = "txtHue";
			this.txtHue.Size = new System.Drawing.Size(30, 18);
			this.txtHue.TabIndex = 4;
			this.txtHue.Tag = "360";
			this.txtHue.Text = "360";
			this.txtHue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtHue, "Hue of this color. Represented in degrees about a circle.");
			this.txtHue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOnly_KeyPress);
			this.txtHue.Leave += new System.EventHandler(this.txtHue_Leave);
			// 
			// _txtRed
			// 
			this._txtRed.AutoSize = true;
			this._txtRed.Location = new System.Drawing.Point(133, 198);
			this._txtRed.Name = "_txtRed";
			this._txtRed.Size = new System.Drawing.Size(30, 15);
			this._txtRed.TabIndex = 9;
			this._txtRed.Text = "Red:";
			// 
			// _txtGreen
			// 
			this._txtGreen.AutoSize = true;
			this._txtGreen.Location = new System.Drawing.Point(122, 220);
			this._txtGreen.Name = "_txtGreen";
			this._txtGreen.Size = new System.Drawing.Size(41, 15);
			this._txtGreen.TabIndex = 11;
			this._txtGreen.Text = "Green:";
			// 
			// _txtBlue
			// 
			this._txtBlue.AutoSize = true;
			this._txtBlue.Location = new System.Drawing.Point(130, 244);
			this._txtBlue.Name = "_txtBlue";
			this._txtBlue.Size = new System.Drawing.Size(33, 15);
			this._txtBlue.TabIndex = 13;
			this._txtBlue.Text = "Blue:";
			// 
			// _txtLuminance
			// 
			this._txtLuminance.AutoSize = true;
			this._txtLuminance.Location = new System.Drawing.Point(3, 244);
			this._txtLuminance.Name = "_txtLuminance";
			this._txtLuminance.Size = new System.Drawing.Size(69, 15);
			this._txtLuminance.TabIndex = 7;
			this._txtLuminance.Text = "Luminance:";
			// 
			// _txtSaturation
			// 
			this._txtSaturation.AutoSize = true;
			this._txtSaturation.Location = new System.Drawing.Point(8, 220);
			this._txtSaturation.Name = "_txtSaturation";
			this._txtSaturation.Size = new System.Drawing.Size(64, 15);
			this._txtSaturation.TabIndex = 5;
			this._txtSaturation.Text = "Saturation:";
			// 
			// _txtHue
			// 
			this._txtHue.AutoSize = true;
			this._txtHue.Location = new System.Drawing.Point(40, 196);
			this._txtHue.Name = "_txtHue";
			this._txtHue.Size = new System.Drawing.Size(32, 15);
			this._txtHue.TabIndex = 3;
			this._txtHue.Text = "Hue:";
			// 
			// txtHex
			// 
			this.txtHex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtHex.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
			this.txtHex.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtHex.Location = new System.Drawing.Point(78, 267);
			this.txtHex.MaxLength = 7;
			this.txtHex.Multiline = true;
			this.txtHex.Name = "txtHex";
			this.txtHex.Size = new System.Drawing.Size(56, 18);
			this.txtHex.TabIndex = 16;
			this.txtHex.Text = "#888888";
			this.txtHex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtHex, "Hexidecimal value of the color");
			this.txtHex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HexOnly_KeyPress);
			this.txtHex.Leave += new System.EventHandler(this.txtHex_Leave);
			// 
			// _txtHex
			// 
			this._txtHex.AutoSize = true;
			this._txtHex.Location = new System.Drawing.Point(42, 268);
			this._txtHex.Name = "_txtHex";
			this._txtHex.Size = new System.Drawing.Size(30, 15);
			this._txtHex.TabIndex = 15;
			this._txtHex.Text = "Hex:";
			// 
			// _txtName
			// 
			this._txtName.AutoSize = true;
			this._txtName.Location = new System.Drawing.Point(30, 294);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(42, 15);
			this._txtName.TabIndex = 17;
			this._txtName.Text = "Name:";
			// 
			// txtName
			// 
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtName.Location = new System.Drawing.Point(78, 291);
			this.txtName.Multiline = true;
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(121, 18);
			this.txtName.TabIndex = 18;
			this.toolTip1.SetToolTip(this.txtName, "Name assigned to this color");
			// 
			// pnlPendingColor
			// 
			this.pnlPendingColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlPendingColor.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.pnlPendingColor.Location = new System.Drawing.Point(169, 4);
			this.pnlPendingColor.Name = "pnlPendingColor";
			this.pnlPendingColor.PaintColor = true;
			this.pnlPendingColor.Size = new System.Drawing.Size(25, 25);
			this.pnlPendingColor.TabIndex = 1;
			this.toolTip1.SetToolTip(this.pnlPendingColor, "New Color");
			// 
			// _lblDegree
			// 
			this._lblDegree.AutoSize = true;
			this._lblDegree.Location = new System.Drawing.Point(106, 196);
			this._lblDegree.Name = "_lblDegree";
			this._lblDegree.Size = new System.Drawing.Size(12, 15);
			this._lblDegree.TabIndex = 19;
			this._lblDegree.Text = "°";
			// 
			// lblPercent1
			// 
			this.lblPercent1.AutoSize = true;
			this.lblPercent1.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.lblPercent1.Location = new System.Drawing.Point(108, 223);
			this.lblPercent1.Name = "lblPercent1";
			this.lblPercent1.Size = new System.Drawing.Size(13, 12);
			this.lblPercent1.TabIndex = 20;
			this.lblPercent1.Text = "%";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.label1.Location = new System.Drawing.Point(108, 246);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(13, 12);
			this.label1.TabIndex = 21;
			this.label1.Text = "%";
			// 
			// colorWheel1
			// 
			this.colorWheel1.Blue = ((byte)(0));
			this.colorWheel1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			hsl1.Alpha = 1D;
			hsl1.H = 0D;
			hsl1.Hue = 0D;
			hsl1.L = 1D;
			hsl1.Luminance = 1D;
			hsl1.S = 1D;
			hsl1.Saturation = 1D;
			this.colorWheel1.HSL = hsl1;
			this.colorWheel1.Hue = 0D;
			this.colorWheel1.Location = new System.Drawing.Point(4, 4);
			this.colorWheel1.Luminance = 1D;
			this.colorWheel1.MarkerSize = ((uint)(10u));
			this.colorWheel1.Name = "colorWheel1";
			this.colorWheel1.RedOffset = -90D;
			this.colorWheel1.Saturation = 1D;
			this.colorWheel1.Size = new System.Drawing.Size(187, 179);
			this.colorWheel1.TabIndex = 0;
			this.colorWheel1.ColorChanged += new System.EventHandler(this.colorWheel1_ColorChanged);
			// 
			// ColorPicker
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.txtName);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this._txtHex);
			this.Controls.Add(this.txtHex);
			this.Controls.Add(this._txtLuminance);
			this.Controls.Add(this._txtSaturation);
			this.Controls.Add(this._txtHue);
			this.Controls.Add(this._txtBlue);
			this.Controls.Add(this._txtGreen);
			this.Controls.Add(this._txtRed);
			this.Controls.Add(this.pnlPendingColor);
			this.Controls.Add(this.txtBlue);
			this.Controls.Add(this.txtGreen);
			this.Controls.Add(this.txtRed);
			this.Controls.Add(this.txtLuminance);
			this.Controls.Add(this.txtSat);
			this.Controls.Add(this.txtHue);
			this.Controls.Add(this.lblOriginalColor);
			this.Controls.Add(this.colorWheel1);
			this.Controls.Add(this._lblDegree);
			this.Controls.Add(this.lblPercent1);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ColorPicker";
			this.Size = new System.Drawing.Size(210, 320);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblOriginalColor;
		private System.Windows.Forms.TextBox txtBlue;
		private System.Windows.Forms.TextBox txtGreen;
		private System.Windows.Forms.TextBox txtRed;
		private System.Windows.Forms.TextBox txtLuminance;
		private System.Windows.Forms.TextBox txtSat;
		private System.Windows.Forms.TextBox txtHue;
		private ColorPanel pnlPendingColor;
		private ColorWheel colorWheel1;
		private System.Windows.Forms.Label _txtRed;
		private System.Windows.Forms.Label _txtGreen;
		private System.Windows.Forms.Label _txtBlue;
		private System.Windows.Forms.Label _txtLuminance;
		private System.Windows.Forms.Label _txtSaturation;
		private System.Windows.Forms.Label _txtHue;
		private System.Windows.Forms.TextBox txtHex;
		private System.Windows.Forms.Label _txtHex;
		private System.Windows.Forms.Label _txtName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label _lblDegree;
		private System.Windows.Forms.Label lblPercent1;
		private System.Windows.Forms.Label label1;
	}
}
