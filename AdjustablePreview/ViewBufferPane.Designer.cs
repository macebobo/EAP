namespace ElfCore
{
	partial class ViewBufferPane
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
			this.tabsPanes = new System.Windows.Forms.TabControl();
			this.tabLatticeBuffer = new System.Windows.Forms.TabPage();
			this.pctLatticeBuffer = new System.Windows.Forms.PictureBox();
			this.tabMaskCanvas = new System.Windows.Forms.TabPage();
			this.pctMaskCanvas = new System.Windows.Forms.PictureBox();
			this.tabMaskLattice = new System.Windows.Forms.TabPage();
			this.pctMaskLattice = new System.Windows.Forms.PictureBox();
			this.tabCapturedCanvas = new System.Windows.Forms.TabPage();
			this.pctCapturedCanvas = new System.Windows.Forms.PictureBox();
			this.tabActiveChannel = new System.Windows.Forms.TabPage();
			this.pctActiveChannel = new System.Windows.Forms.PictureBox();
			this.tabImageStamp = new System.Windows.Forms.TabPage();
			this.pctImageStamp = new System.Windows.Forms.PictureBox();
			this.tabClipboard = new System.Windows.Forms.TabPage();
			this.pctClipboard = new System.Windows.Forms.PictureBox();
			this.tabMove = new System.Windows.Forms.TabPage();
			this.pctMove = new System.Windows.Forms.PictureBox();
			this.tabsPanes.SuspendLayout();
			this.tabLatticeBuffer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctLatticeBuffer)).BeginInit();
			this.tabMaskCanvas.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctMaskCanvas)).BeginInit();
			this.tabMaskLattice.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctMaskLattice)).BeginInit();
			this.tabCapturedCanvas.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctCapturedCanvas)).BeginInit();
			this.tabActiveChannel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctActiveChannel)).BeginInit();
			this.tabImageStamp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctImageStamp)).BeginInit();
			this.tabClipboard.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctClipboard)).BeginInit();
			this.tabMove.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctMove)).BeginInit();
			this.SuspendLayout();
			// 
			// tabsPanes
			// 
			this.tabsPanes.Controls.Add(this.tabLatticeBuffer);
			this.tabsPanes.Controls.Add(this.tabMaskCanvas);
			this.tabsPanes.Controls.Add(this.tabMaskLattice);
			this.tabsPanes.Controls.Add(this.tabCapturedCanvas);
			this.tabsPanes.Controls.Add(this.tabActiveChannel);
			this.tabsPanes.Controls.Add(this.tabImageStamp);
			this.tabsPanes.Controls.Add(this.tabClipboard);
			this.tabsPanes.Controls.Add(this.tabMove);
			this.tabsPanes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabsPanes.Location = new System.Drawing.Point(0, 0);
			this.tabsPanes.Name = "tabsPanes";
			this.tabsPanes.SelectedIndex = 0;
			this.tabsPanes.Size = new System.Drawing.Size(949, 692);
			this.tabsPanes.TabIndex = 1;
			// 
			// tabLatticeBuffer
			// 
			this.tabLatticeBuffer.Controls.Add(this.pctLatticeBuffer);
			this.tabLatticeBuffer.Location = new System.Drawing.Point(4, 22);
			this.tabLatticeBuffer.Name = "tabLatticeBuffer";
			this.tabLatticeBuffer.Padding = new System.Windows.Forms.Padding(3);
			this.tabLatticeBuffer.Size = new System.Drawing.Size(941, 666);
			this.tabLatticeBuffer.TabIndex = 0;
			this.tabLatticeBuffer.Text = "Lattice Buffer";
			this.tabLatticeBuffer.UseVisualStyleBackColor = true;
			// 
			// pctLatticeBuffer
			// 
			this.pctLatticeBuffer.BackColor = System.Drawing.Color.DarkSlateBlue;
			this.pctLatticeBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctLatticeBuffer.Location = new System.Drawing.Point(3, 3);
			this.pctLatticeBuffer.Name = "pctLatticeBuffer";
			this.pctLatticeBuffer.Size = new System.Drawing.Size(935, 660);
			this.pctLatticeBuffer.TabIndex = 1;
			this.pctLatticeBuffer.TabStop = false;
			// 
			// tabMaskCanvas
			// 
			this.tabMaskCanvas.Controls.Add(this.pctMaskCanvas);
			this.tabMaskCanvas.Location = new System.Drawing.Point(4, 22);
			this.tabMaskCanvas.Name = "tabMaskCanvas";
			this.tabMaskCanvas.Padding = new System.Windows.Forms.Padding(3);
			this.tabMaskCanvas.Size = new System.Drawing.Size(941, 666);
			this.tabMaskCanvas.TabIndex = 1;
			this.tabMaskCanvas.Text = "Mask Canvas Buffer";
			this.tabMaskCanvas.UseVisualStyleBackColor = true;
			// 
			// pctMaskCanvas
			// 
			this.pctMaskCanvas.BackColor = System.Drawing.Color.ForestGreen;
			this.pctMaskCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctMaskCanvas.Location = new System.Drawing.Point(3, 3);
			this.pctMaskCanvas.Name = "pctMaskCanvas";
			this.pctMaskCanvas.Size = new System.Drawing.Size(935, 660);
			this.pctMaskCanvas.TabIndex = 1;
			this.pctMaskCanvas.TabStop = false;
			// 
			// tabMaskLattice
			// 
			this.tabMaskLattice.Controls.Add(this.pctMaskLattice);
			this.tabMaskLattice.Location = new System.Drawing.Point(4, 22);
			this.tabMaskLattice.Name = "tabMaskLattice";
			this.tabMaskLattice.Padding = new System.Windows.Forms.Padding(3);
			this.tabMaskLattice.Size = new System.Drawing.Size(941, 666);
			this.tabMaskLattice.TabIndex = 8;
			this.tabMaskLattice.Text = "Mask Lattice Buffer";
			this.tabMaskLattice.UseVisualStyleBackColor = true;
			// 
			// pctMaskLattice
			// 
			this.pctMaskLattice.BackColor = System.Drawing.Color.BlueViolet;
			this.pctMaskLattice.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctMaskLattice.Location = new System.Drawing.Point(3, 3);
			this.pctMaskLattice.Name = "pctMaskLattice";
			this.pctMaskLattice.Size = new System.Drawing.Size(935, 660);
			this.pctMaskLattice.TabIndex = 1;
			this.pctMaskLattice.TabStop = false;
			// 
			// tabCapturedCanvas
			// 
			this.tabCapturedCanvas.Controls.Add(this.pctCapturedCanvas);
			this.tabCapturedCanvas.Location = new System.Drawing.Point(4, 22);
			this.tabCapturedCanvas.Name = "tabCapturedCanvas";
			this.tabCapturedCanvas.Size = new System.Drawing.Size(941, 666);
			this.tabCapturedCanvas.TabIndex = 3;
			this.tabCapturedCanvas.Text = "Captured Canvas";
			this.tabCapturedCanvas.UseVisualStyleBackColor = true;
			// 
			// pctCapturedCanvas
			// 
			this.pctCapturedCanvas.BackColor = System.Drawing.Color.DodgerBlue;
			this.pctCapturedCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctCapturedCanvas.Location = new System.Drawing.Point(0, 0);
			this.pctCapturedCanvas.Name = "pctCapturedCanvas";
			this.pctCapturedCanvas.Size = new System.Drawing.Size(941, 666);
			this.pctCapturedCanvas.TabIndex = 2;
			this.pctCapturedCanvas.TabStop = false;
			// 
			// tabActiveChannel
			// 
			this.tabActiveChannel.Controls.Add(this.pctActiveChannel);
			this.tabActiveChannel.Location = new System.Drawing.Point(4, 22);
			this.tabActiveChannel.Name = "tabActiveChannel";
			this.tabActiveChannel.Size = new System.Drawing.Size(941, 666);
			this.tabActiveChannel.TabIndex = 4;
			this.tabActiveChannel.Text = "Active Channel";
			this.tabActiveChannel.UseVisualStyleBackColor = true;
			// 
			// pctActiveChannel
			// 
			this.pctActiveChannel.BackColor = System.Drawing.Color.DarkRed;
			this.pctActiveChannel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctActiveChannel.Location = new System.Drawing.Point(0, 0);
			this.pctActiveChannel.Name = "pctActiveChannel";
			this.pctActiveChannel.Size = new System.Drawing.Size(941, 666);
			this.pctActiveChannel.TabIndex = 2;
			this.pctActiveChannel.TabStop = false;
			// 
			// tabImageStamp
			// 
			this.tabImageStamp.Controls.Add(this.pctImageStamp);
			this.tabImageStamp.Location = new System.Drawing.Point(4, 22);
			this.tabImageStamp.Name = "tabImageStamp";
			this.tabImageStamp.Size = new System.Drawing.Size(941, 666);
			this.tabImageStamp.TabIndex = 5;
			this.tabImageStamp.Text = "Image Stamp";
			this.tabImageStamp.UseVisualStyleBackColor = true;
			// 
			// pctImageStamp
			// 
			this.pctImageStamp.BackColor = System.Drawing.Color.DarkGoldenrod;
			this.pctImageStamp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctImageStamp.Location = new System.Drawing.Point(0, 0);
			this.pctImageStamp.Name = "pctImageStamp";
			this.pctImageStamp.Size = new System.Drawing.Size(941, 666);
			this.pctImageStamp.TabIndex = 2;
			this.pctImageStamp.TabStop = false;
			// 
			// tabClipboard
			// 
			this.tabClipboard.Controls.Add(this.pctClipboard);
			this.tabClipboard.Location = new System.Drawing.Point(4, 22);
			this.tabClipboard.Name = "tabClipboard";
			this.tabClipboard.Size = new System.Drawing.Size(941, 666);
			this.tabClipboard.TabIndex = 6;
			this.tabClipboard.Text = "Clipboard";
			this.tabClipboard.UseVisualStyleBackColor = true;
			// 
			// pctClipboard
			// 
			this.pctClipboard.BackColor = System.Drawing.Color.DarkMagenta;
			this.pctClipboard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctClipboard.Location = new System.Drawing.Point(0, 0);
			this.pctClipboard.Name = "pctClipboard";
			this.pctClipboard.Size = new System.Drawing.Size(941, 666);
			this.pctClipboard.TabIndex = 2;
			this.pctClipboard.TabStop = false;
			// 
			// tabMove
			// 
			this.tabMove.Controls.Add(this.pctMove);
			this.tabMove.Location = new System.Drawing.Point(4, 22);
			this.tabMove.Name = "tabMove";
			this.tabMove.Size = new System.Drawing.Size(941, 666);
			this.tabMove.TabIndex = 7;
			this.tabMove.Text = "Move Channel";
			this.tabMove.UseVisualStyleBackColor = true;
			// 
			// pctMove
			// 
			this.pctMove.BackColor = System.Drawing.Color.IndianRed;
			this.pctMove.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctMove.Location = new System.Drawing.Point(0, 0);
			this.pctMove.Name = "pctMove";
			this.pctMove.Size = new System.Drawing.Size(941, 666);
			this.pctMove.TabIndex = 2;
			this.pctMove.TabStop = false;
			// 
			// ViewBufferPane
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(949, 692);
			this.ControlBox = false;
			this.Controls.Add(this.tabsPanes);
			this.Location = new System.Drawing.Point(1800, 0);
			this.Name = "ViewBufferPane";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Buffer";
			this.tabsPanes.ResumeLayout(false);
			this.tabLatticeBuffer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctLatticeBuffer)).EndInit();
			this.tabMaskCanvas.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctMaskCanvas)).EndInit();
			this.tabMaskLattice.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctMaskLattice)).EndInit();
			this.tabCapturedCanvas.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctCapturedCanvas)).EndInit();
			this.tabActiveChannel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctActiveChannel)).EndInit();
			this.tabImageStamp.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctImageStamp)).EndInit();
			this.tabClipboard.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctClipboard)).EndInit();
			this.tabMove.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctMove)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabsPanes;
		private System.Windows.Forms.TabPage tabLatticeBuffer;
		private System.Windows.Forms.PictureBox pctLatticeBuffer;
		private System.Windows.Forms.TabPage tabMaskCanvas;
		private System.Windows.Forms.PictureBox pctMaskCanvas;
		private System.Windows.Forms.TabPage tabCapturedCanvas;
		private System.Windows.Forms.PictureBox pctCapturedCanvas;
		private System.Windows.Forms.TabPage tabActiveChannel;
		private System.Windows.Forms.PictureBox pctActiveChannel;
		private System.Windows.Forms.TabPage tabImageStamp;
		private System.Windows.Forms.PictureBox pctImageStamp;
		private System.Windows.Forms.TabPage tabClipboard;
		private System.Windows.Forms.PictureBox pctClipboard;
		private System.Windows.Forms.TabPage tabMove;
		private System.Windows.Forms.PictureBox pctMove;
		private System.Windows.Forms.TabPage tabMaskLattice;
		private System.Windows.Forms.PictureBox pctMaskLattice;

	}
}