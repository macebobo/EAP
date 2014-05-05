namespace ElfCore.Forms
{
	partial class MappingPreview
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MappingPreview));
			this.CanvasPane = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).BeginInit();
			this.SuspendLayout();
			// 
			// CanvasPane
			// 
			this.CanvasPane.BackColor = System.Drawing.Color.Black;
			this.CanvasPane.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CanvasPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CanvasPane.Location = new System.Drawing.Point(0, 0);
			this.CanvasPane.Name = "CanvasPane";
			this.CanvasPane.Size = new System.Drawing.Size(276, 276);
			this.CanvasPane.TabIndex = 0;
			this.CanvasPane.TabStop = false;
			this.CanvasPane.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPane_Paint);
			// 
			// MappingPreview
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(276, 276);
			this.ControlBox = false;
			this.Controls.Add(this.CanvasPane);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MappingPreview";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox CanvasPane;

	}
}