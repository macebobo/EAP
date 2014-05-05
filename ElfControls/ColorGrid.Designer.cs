namespace ElfControls
{
	partial class ColorGrid
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
			this.cmdEmptyColor = new System.Windows.Forms.Button();
			this.cmdAddColor = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.AddColorDialog = new ElfControls.SelectColorDialog(this.components);
			this.SuspendLayout();
			// 
			// cmdEmptyColor
			// 
			this.cmdEmptyColor.BackColor = System.Drawing.SystemColors.Control;
			this.cmdEmptyColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdEmptyColor.Image = global::ElfControls.Properties.Resources.not_small;
			this.cmdEmptyColor.Location = new System.Drawing.Point(36, 122);
			this.cmdEmptyColor.Margin = new System.Windows.Forms.Padding(0);
			this.cmdEmptyColor.Name = "cmdEmptyColor";
			this.cmdEmptyColor.Padding = new System.Windows.Forms.Padding(0, 0, 2, 2);
			this.cmdEmptyColor.Size = new System.Drawing.Size(16, 16);
			this.cmdEmptyColor.TabIndex = 5;
			this.toolTip1.SetToolTip(this.cmdEmptyColor, "No color");
			this.cmdEmptyColor.UseVisualStyleBackColor = false;
			this.cmdEmptyColor.Click += new System.EventHandler(this.cmdEmptyColor_Click);
			// 
			// cmdAddColor
			// 
			this.cmdAddColor.BackColor = System.Drawing.SystemColors.Control;
			this.cmdAddColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdAddColor.Image = global::ElfControls.Properties.Resources.add_small;
			this.cmdAddColor.Location = new System.Drawing.Point(18, 122);
			this.cmdAddColor.Margin = new System.Windows.Forms.Padding(0);
			this.cmdAddColor.Name = "cmdAddColor";
			this.cmdAddColor.Padding = new System.Windows.Forms.Padding(0, 0, 2, 2);
			this.cmdAddColor.Size = new System.Drawing.Size(16, 16);
			this.cmdAddColor.TabIndex = 4;
			this.toolTip1.SetToolTip(this.cmdAddColor, "Add new color");
			this.cmdAddColor.UseVisualStyleBackColor = false;
			this.cmdAddColor.Click += new System.EventHandler(this.cmdAddColor_Click);
			// 
			// AddColorDialog
			// 
			this.AddColorDialog.CancelButton_Image = null;
			this.AddColorDialog.Color = System.Drawing.Color.Empty;
			this.AddColorDialog.ColorName = null;
			this.AddColorDialog.OKButton_Image = null;
			this.AddColorDialog.Title = null;
			// 
			// ColorGrid
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.cmdEmptyColor);
			this.Controls.Add(this.cmdAddColor);
			this.Name = "ColorGrid";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdEmptyColor;
		private System.Windows.Forms.Button cmdAddColor;
		private System.Windows.Forms.ToolTip toolTip1;
		private SelectColorDialog AddColorDialog;
	}
}
