namespace ElfControls
{
	partial class ColorDropDown
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
			this.cpnlDisplayColor = new ElfControls.ColorPanel();
			this.tabsColors = new System.Windows.Forms.TabControl();
			this.tabCustom = new System.Windows.Forms.TabPage();
			this.PaletteColorGrid = new ElfControls.ColorGrid();
			this.tabKnownColors = new System.Windows.Forms.TabPage();
			this.KnownColorsList = new ElfControls.ColorGrid();
			this.tabSystemColors = new System.Windows.Forms.TabPage();
			this.SystemColorsList = new ElfControls.ColorGrid();
			this.pctNoColor = new System.Windows.Forms.PictureBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabsColors.SuspendLayout();
			this.tabCustom.SuspendLayout();
			this.tabKnownColors.SuspendLayout();
			this.tabSystemColors.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctNoColor)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlDisplayColor
			// 
			this.cpnlDisplayColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.cpnlDisplayColor.Color = System.Drawing.Color.Lime;
			this.cpnlDisplayColor.Location = new System.Drawing.Point(2, 2);
			this.cpnlDisplayColor.Name = "pnlDisplayColor";
			this.cpnlDisplayColor.PaintColor = true;
			this.cpnlDisplayColor.Size = new System.Drawing.Size(14, 15);
			this.cpnlDisplayColor.TabIndex = 0;
			this.cpnlDisplayColor.Text = "colorPanel1";
			this.cpnlDisplayColor.Click += new System.EventHandler(this.pnlDisplayColor_Click);
			// 
			// tabsColors
			// 
			this.tabsColors.Controls.Add(this.tabCustom);
			this.tabsColors.Controls.Add(this.tabKnownColors);
			this.tabsColors.Controls.Add(this.tabSystemColors);
			this.tabsColors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabsColors.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabsColors.ItemSize = new System.Drawing.Size(43, 15);
			this.tabsColors.Location = new System.Drawing.Point(0, 0);
			this.tabsColors.Multiline = true;
			this.tabsColors.Name = "tabsColors";
			this.tabsColors.SelectedIndex = 0;
			this.tabsColors.Size = new System.Drawing.Size(216, 303);
			this.tabsColors.TabIndex = 0;
			this.tabsColors.SelectedIndexChanged += new System.EventHandler(this.tabsColors_SelectedIndexChanged);
			// 
			// tabCustom
			// 
			this.tabCustom.AutoScroll = true;
			this.tabCustom.Controls.Add(this.PaletteColorGrid);
			this.tabCustom.Location = new System.Drawing.Point(4, 19);
			this.tabCustom.Name = "tabCustom";
			this.tabCustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabCustom.Size = new System.Drawing.Size(208, 280);
			this.tabCustom.TabIndex = 0;
			this.tabCustom.Text = "Custom";
			this.tabCustom.UseVisualStyleBackColor = true;
			// 
			// PaletteColorGrid
			// 
			this.PaletteColorGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PaletteColorGrid.CancelButtonImage = null;
			this.PaletteColorGrid.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.PaletteColorGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.PaletteColorGrid.Location = new System.Drawing.Point(3, 3);
			this.PaletteColorGrid.Name = "PaletteColorGrid";
			this.PaletteColorGrid.OKButtonImage = null;
			this.PaletteColorGrid.ParentUserControl = null;
			this.PaletteColorGrid.SelectedIndex = 0;
			this.PaletteColorGrid.Size = new System.Drawing.Size(202, 276);
			this.PaletteColorGrid.StretchToFitParent = true;
			this.PaletteColorGrid.TabIndex = 0;
			this.PaletteColorGrid.SelectedIndexChange += new System.EventHandler(this.PaletteColorGrid_SelectedIndexChange);
			// 
			// tabKnownColors
			// 
			this.tabKnownColors.AutoScroll = true;
			this.tabKnownColors.Controls.Add(this.KnownColorsList);
			this.tabKnownColors.Location = new System.Drawing.Point(4, 19);
			this.tabKnownColors.Name = "tabKnownColors";
			this.tabKnownColors.Padding = new System.Windows.Forms.Padding(3);
			this.tabKnownColors.Size = new System.Drawing.Size(208, 280);
			this.tabKnownColors.TabIndex = 1;
			this.tabKnownColors.Text = "Known Colors";
			this.tabKnownColors.UseVisualStyleBackColor = true;
			// 
			// KnownColorsList
			// 
			this.KnownColorsList.AllowAddColors = false;
			this.KnownColorsList.AllowEmptyColor = false;
			this.KnownColorsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.KnownColorsList.CancelButtonImage = null;
			this.KnownColorsList.Color = System.Drawing.Color.AliceBlue;
			this.KnownColorsList.Dock = System.Windows.Forms.DockStyle.Top;
			this.KnownColorsList.Location = new System.Drawing.Point(3, 3);
			this.KnownColorsList.Name = "KnownColorsList";
			this.KnownColorsList.OKButtonImage = null;
			this.KnownColorsList.Palette = ElfControls.ColorGrid.ColorPalette.KnownColors;
			this.KnownColorsList.ParentUserControl = null;
			this.KnownColorsList.SelectedIndex = 0;
			this.KnownColorsList.Size = new System.Drawing.Size(185, 344);
			this.KnownColorsList.StretchToFitParent = true;
			this.KnownColorsList.TabIndex = 0;
			this.KnownColorsList.SelectedIndexChange += new System.EventHandler(this.KnownColorsList_SelectedIndexChange);
			// 
			// tabSystemColors
			// 
			this.tabSystemColors.AutoScroll = true;
			this.tabSystemColors.Controls.Add(this.SystemColorsList);
			this.tabSystemColors.Location = new System.Drawing.Point(4, 19);
			this.tabSystemColors.Name = "tabSystemColors";
			this.tabSystemColors.Padding = new System.Windows.Forms.Padding(3);
			this.tabSystemColors.Size = new System.Drawing.Size(208, 280);
			this.tabSystemColors.TabIndex = 2;
			this.tabSystemColors.Text = "System Colors";
			this.tabSystemColors.UseVisualStyleBackColor = true;
			// 
			// SystemColorsList
			// 
			this.SystemColorsList.AllowAddColors = false;
			this.SystemColorsList.AllowEmptyColor = false;
			this.SystemColorsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.SystemColorsList.CancelButtonImage = null;
			this.SystemColorsList.Color = System.Drawing.SystemColors.ActiveBorder;
			this.SystemColorsList.Dock = System.Windows.Forms.DockStyle.Top;
			this.SystemColorsList.Location = new System.Drawing.Point(3, 3);
			this.SystemColorsList.Name = "SystemColorsList";
			this.SystemColorsList.OKButtonImage = null;
			this.SystemColorsList.Palette = ElfControls.ColorGrid.ColorPalette.SystemColors;
			this.SystemColorsList.ParentUserControl = null;
			this.SystemColorsList.SelectedIndex = 0;
			this.SystemColorsList.Size = new System.Drawing.Size(202, 276);
			this.SystemColorsList.StretchToFitParent = true;
			this.SystemColorsList.TabIndex = 0;
			this.SystemColorsList.SelectedIndexChange += new System.EventHandler(this.SystemColorsList_SelectedIndexChange);
			// 
			// pctNoColor
			// 
			this.pctNoColor.Image = global::ElfControls.Properties.Resources.not_small;
			this.pctNoColor.Location = new System.Drawing.Point(33, 4);
			this.pctNoColor.Name = "pctNoColor";
			this.pctNoColor.BackColor = System.Drawing.Color.Transparent;
			this.pctNoColor.Size = new System.Drawing.Size(10, 10);
			this.pctNoColor.TabIndex = 3;
			this.pctNoColor.TabStop = false;
			this.pctNoColor.Click += new System.EventHandler(this.pctNoColor_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 5;
			this.toolTip1.ReshowDelay = 5;
			// 
			// ColorDropDown
			// 
			this.AnchorSize = new System.Drawing.Size(216, 21);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.pctNoColor);
			this.Controls.Add(this.tabsColors);
			this.Controls.Add(this.cpnlDisplayColor);
			this.Name = "ColorDropDown";
			this.Size = new System.Drawing.Size(216, 303);
			this.tabsColors.ResumeLayout(false);
			this.tabCustom.ResumeLayout(false);
			this.tabKnownColors.ResumeLayout(false);
			this.tabSystemColors.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctNoColor)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ColorPanel cpnlDisplayColor;
		//private System.Windows.Forms.Panel pnlDropDown;
		private System.Windows.Forms.TabControl tabsColors;
		private System.Windows.Forms.TabPage tabCustom;
		private System.Windows.Forms.TabPage tabKnownColors;
		private System.Windows.Forms.TabPage tabSystemColors;
		private System.Windows.Forms.PictureBox pctNoColor;
		private ColorGrid KnownColorsList;
		private ColorGrid SystemColorsList;
		private ColorGrid PaletteColorGrid;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
