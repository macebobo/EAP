namespace ElfCore.Forms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewBufferPane));
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
			this.tabMoveChannel = new System.Windows.Forms.TabPage();
			this.pctMove = new System.Windows.Forms.PictureBox();
			this.tabSnapshot = new System.Windows.Forms.TabPage();
			this.txtSnapshot = new System.Windows.Forms.TextBox();
			this.tabUndo = new System.Windows.Forms.TabPage();
			this.txtUndo = new System.Windows.Forms.TextBox();
			this.tabRedo = new System.Windows.Forms.TabPage();
			this.txtRedo = new System.Windows.Forms.TextBox();
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
			this.tabMoveChannel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctMove)).BeginInit();
			this.tabSnapshot.SuspendLayout();
			this.tabUndo.SuspendLayout();
			this.tabRedo.SuspendLayout();
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
			this.tabsPanes.Controls.Add(this.tabMoveChannel);
			this.tabsPanes.Controls.Add(this.tabSnapshot);
			this.tabsPanes.Controls.Add(this.tabUndo);
			this.tabsPanes.Controls.Add(this.tabRedo);
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
			this.tabCapturedCanvas.Padding = new System.Windows.Forms.Padding(3);
			this.tabCapturedCanvas.Size = new System.Drawing.Size(941, 666);
			this.tabCapturedCanvas.TabIndex = 3;
			this.tabCapturedCanvas.Text = "Captured Canvas";
			this.tabCapturedCanvas.UseVisualStyleBackColor = true;
			// 
			// pctCapturedCanvas
			// 
			this.pctCapturedCanvas.BackColor = System.Drawing.Color.DodgerBlue;
			this.pctCapturedCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctCapturedCanvas.Location = new System.Drawing.Point(3, 3);
			this.pctCapturedCanvas.Name = "pctCapturedCanvas";
			this.pctCapturedCanvas.Size = new System.Drawing.Size(935, 660);
			this.pctCapturedCanvas.TabIndex = 2;
			this.pctCapturedCanvas.TabStop = false;
			// 
			// tabActiveChannel
			// 
			this.tabActiveChannel.Controls.Add(this.pctActiveChannel);
			this.tabActiveChannel.Location = new System.Drawing.Point(4, 22);
			this.tabActiveChannel.Name = "tabActiveChannel";
			this.tabActiveChannel.Padding = new System.Windows.Forms.Padding(3);
			this.tabActiveChannel.Size = new System.Drawing.Size(941, 666);
			this.tabActiveChannel.TabIndex = 4;
			this.tabActiveChannel.Text = "Active Channel";
			this.tabActiveChannel.UseVisualStyleBackColor = true;
			// 
			// pctActiveChannel
			// 
			this.pctActiveChannel.BackColor = System.Drawing.Color.DarkRed;
			this.pctActiveChannel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctActiveChannel.Location = new System.Drawing.Point(3, 3);
			this.pctActiveChannel.Name = "pctActiveChannel";
			this.pctActiveChannel.Size = new System.Drawing.Size(935, 660);
			this.pctActiveChannel.TabIndex = 2;
			this.pctActiveChannel.TabStop = false;
			// 
			// tabImageStamp
			// 
			this.tabImageStamp.Controls.Add(this.pctImageStamp);
			this.tabImageStamp.Location = new System.Drawing.Point(4, 22);
			this.tabImageStamp.Name = "tabImageStamp";
			this.tabImageStamp.Padding = new System.Windows.Forms.Padding(3);
			this.tabImageStamp.Size = new System.Drawing.Size(941, 666);
			this.tabImageStamp.TabIndex = 5;
			this.tabImageStamp.Text = "Image Stamp";
			this.tabImageStamp.UseVisualStyleBackColor = true;
			// 
			// pctImageStamp
			// 
			this.pctImageStamp.BackColor = System.Drawing.Color.DarkGoldenrod;
			this.pctImageStamp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctImageStamp.Location = new System.Drawing.Point(3, 3);
			this.pctImageStamp.Name = "pctImageStamp";
			this.pctImageStamp.Size = new System.Drawing.Size(935, 660);
			this.pctImageStamp.TabIndex = 2;
			this.pctImageStamp.TabStop = false;
			// 
			// tabClipboard
			// 
			this.tabClipboard.Controls.Add(this.pctClipboard);
			this.tabClipboard.Location = new System.Drawing.Point(4, 22);
			this.tabClipboard.Name = "tabClipboard";
			this.tabClipboard.Padding = new System.Windows.Forms.Padding(3);
			this.tabClipboard.Size = new System.Drawing.Size(941, 666);
			this.tabClipboard.TabIndex = 6;
			this.tabClipboard.Text = "Clipboard";
			this.tabClipboard.UseVisualStyleBackColor = true;
			// 
			// pctClipboard
			// 
			this.pctClipboard.BackColor = System.Drawing.Color.DarkMagenta;
			this.pctClipboard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctClipboard.Location = new System.Drawing.Point(3, 3);
			this.pctClipboard.Name = "pctClipboard";
			this.pctClipboard.Size = new System.Drawing.Size(935, 660);
			this.pctClipboard.TabIndex = 2;
			this.pctClipboard.TabStop = false;
			// 
			// tabMoveChannel
			// 
			this.tabMoveChannel.Controls.Add(this.pctMove);
			this.tabMoveChannel.Location = new System.Drawing.Point(4, 22);
			this.tabMoveChannel.Name = "tabMoveChannel";
			this.tabMoveChannel.Padding = new System.Windows.Forms.Padding(3);
			this.tabMoveChannel.Size = new System.Drawing.Size(941, 666);
			this.tabMoveChannel.TabIndex = 7;
			this.tabMoveChannel.Text = "Move Channel";
			this.tabMoveChannel.UseVisualStyleBackColor = true;
			// 
			// pctMove
			// 
			this.pctMove.BackColor = System.Drawing.Color.IndianRed;
			this.pctMove.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pctMove.Location = new System.Drawing.Point(3, 3);
			this.pctMove.Name = "pctMove";
			this.pctMove.Size = new System.Drawing.Size(935, 660);
			this.pctMove.TabIndex = 2;
			this.pctMove.TabStop = false;
			// 
			// tabSnapshot
			// 
			this.tabSnapshot.Controls.Add(this.txtSnapshot);
			this.tabSnapshot.Location = new System.Drawing.Point(4, 22);
			this.tabSnapshot.Name = "tabSnapshot";
			this.tabSnapshot.Padding = new System.Windows.Forms.Padding(3);
			this.tabSnapshot.Size = new System.Drawing.Size(941, 666);
			this.tabSnapshot.TabIndex = 11;
			this.tabSnapshot.Text = "Undo Snapshot";
			this.tabSnapshot.UseVisualStyleBackColor = true;
			// 
			// txtSnapshot
			// 
			this.txtSnapshot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(238)))), ((int)(((byte)(255)))));
			this.txtSnapshot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtSnapshot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSnapshot.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSnapshot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.txtSnapshot.Location = new System.Drawing.Point(3, 3);
			this.txtSnapshot.Multiline = true;
			this.txtSnapshot.Name = "txtSnapshot";
			this.txtSnapshot.ReadOnly = true;
			this.txtSnapshot.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSnapshot.Size = new System.Drawing.Size(935, 660);
			this.txtSnapshot.TabIndex = 1;
			// 
			// tabUndo
			// 
			this.tabUndo.Controls.Add(this.txtUndo);
			this.tabUndo.Location = new System.Drawing.Point(4, 22);
			this.tabUndo.Name = "tabUndo";
			this.tabUndo.Padding = new System.Windows.Forms.Padding(3);
			this.tabUndo.Size = new System.Drawing.Size(941, 666);
			this.tabUndo.TabIndex = 9;
			this.tabUndo.Text = "Undo";
			this.tabUndo.UseVisualStyleBackColor = true;
			// 
			// txtUndo
			// 
			this.txtUndo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
			this.txtUndo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtUndo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtUndo.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtUndo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.txtUndo.Location = new System.Drawing.Point(3, 3);
			this.txtUndo.Multiline = true;
			this.txtUndo.Name = "txtUndo";
			this.txtUndo.ReadOnly = true;
			this.txtUndo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtUndo.Size = new System.Drawing.Size(935, 660);
			this.txtUndo.TabIndex = 0;
			// 
			// tabRedo
			// 
			this.tabRedo.Controls.Add(this.txtRedo);
			this.tabRedo.Location = new System.Drawing.Point(4, 22);
			this.tabRedo.Name = "tabRedo";
			this.tabRedo.Padding = new System.Windows.Forms.Padding(3);
			this.tabRedo.Size = new System.Drawing.Size(941, 666);
			this.tabRedo.TabIndex = 10;
			this.tabRedo.Text = "Redo";
			this.tabRedo.UseVisualStyleBackColor = true;
			// 
			// txtRedo
			// 
			this.txtRedo.BackColor = System.Drawing.Color.AliceBlue;
			this.txtRedo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtRedo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtRedo.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtRedo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.txtRedo.Location = new System.Drawing.Point(3, 3);
			this.txtRedo.Multiline = true;
			this.txtRedo.Name = "txtRedo";
			this.txtRedo.ReadOnly = true;
			this.txtRedo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtRedo.Size = new System.Drawing.Size(935, 660);
			this.txtRedo.TabIndex = 1;
			// 
			// ViewBufferPane
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(949, 692);
			this.Controls.Add(this.tabsPanes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(1800, 0);
			this.Name = "ViewBufferPane";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Diagnostics";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Shown += new System.EventHandler(this.ViewBufferPane_Shown);
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
			this.tabMoveChannel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctMove)).EndInit();
			this.tabSnapshot.ResumeLayout(false);
			this.tabSnapshot.PerformLayout();
			this.tabUndo.ResumeLayout(false);
			this.tabUndo.PerformLayout();
			this.tabRedo.ResumeLayout(false);
			this.tabRedo.PerformLayout();
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
		private System.Windows.Forms.TabPage tabMoveChannel;
		private System.Windows.Forms.PictureBox pctMove;
		private System.Windows.Forms.TabPage tabMaskLattice;
		private System.Windows.Forms.PictureBox pctMaskLattice;
		private System.Windows.Forms.TabPage tabUndo;
		private System.Windows.Forms.TextBox txtUndo;
		private System.Windows.Forms.TabPage tabRedo;
		private System.Windows.Forms.TextBox txtRedo;
		private System.Windows.Forms.TabPage tabSnapshot;
		private System.Windows.Forms.TextBox txtSnapshot;

	}
}