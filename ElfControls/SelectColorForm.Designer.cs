namespace ElfControls
{
	partial class SelectColorForm
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
			this.pnlPalette = new System.Windows.Forms.Panel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cgPalette = new ElfControls.ColorGrid();
			this.ColorPicker = new ElfControls.ColorPicker();
			this.pnlPalette.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlPalette
			// 
			this.pnlPalette.AutoScroll = true;
			this.pnlPalette.Controls.Add(this.cgPalette);
			this.pnlPalette.Location = new System.Drawing.Point(12, 10);
			this.pnlPalette.Name = "pnlPalette";
			this.pnlPalette.Size = new System.Drawing.Size(175, 309);
			this.pnlPalette.TabIndex = 0;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdOk.Image = global::ElfControls.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(124, 325);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 30);
			this.cmdOk.TabIndex = 2;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdCancel.Image = global::ElfControls.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(195, 325);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 30);
			this.cmdCancel.TabIndex = 3;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cgPalette
			// 
			this.cgPalette.AllowEmptyColor = false;
			this.cgPalette.CancelButtonImage = null;
			this.cgPalette.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.cgPalette.GridSize = new System.Drawing.Size(20, 20);
			this.cgPalette.Location = new System.Drawing.Point(3, 3);
			this.cgPalette.Name = "cgPalette";
			this.cgPalette.OKButtonImage = null;
			this.cgPalette.ParentUserControl = null;
			this.cgPalette.SelectedIndex = 0;
			this.cgPalette.Size = new System.Drawing.Size(154, 224);
			this.cgPalette.TabIndex = 0;
			this.cgPalette.SelectedIndexChange += new System.EventHandler(this.cgPalette_SelectedIndexChange);
			// 
			// ColorPicker
			// 
			this.ColorPicker.ColorName = "red";
			this.ColorPicker.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ColorPicker.Location = new System.Drawing.Point(188, 5);
			this.ColorPicker.Name = "ColorPicker";
			this.ColorPicker.RedOffset = -90D;
			this.ColorPicker.Size = new System.Drawing.Size(210, 320);
			this.ColorPicker.TabIndex = 1;
			// 
			// SelectColorForm
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(403, 363);
			this.Controls.Add(this.ColorPicker);
			this.Controls.Add(this.pnlPalette);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectColorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Chooser";
			this.pnlPalette.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private ColorPicker ColorPicker;
		private System.Windows.Forms.Panel pnlPalette;
		private ColorGrid cgPalette;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}