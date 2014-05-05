namespace ElfCore.Forms
{
	partial class PreviewPane
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewPane));
			this.pnlControls = new System.Windows.Forms.Panel();
			this.lblTime = new System.Windows.Forms.Label();
			this.lblFrameRate = new System.Windows.Forms.Label();
			this.lblRecording = new System.Windows.Forms.Label();
			this.cmdRecord = new System.Windows.Forms.Button();
			this.cmdStop = new System.Windows.Forms.Button();
			this.cmdPause = new System.Windows.Forms.Button();
			this.cmdPlayFromPoint = new System.Windows.Forms.Button();
			this.cmdPlay = new System.Windows.Forms.Button();
			this.CanvasPane = new System.Windows.Forms.PictureBox();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlControls
			// 
			this.pnlControls.BackColor = System.Drawing.SystemColors.Control;
			this.pnlControls.Controls.Add(this.lblTime);
			this.pnlControls.Controls.Add(this.lblFrameRate);
			this.pnlControls.Controls.Add(this.lblRecording);
			this.pnlControls.Controls.Add(this.cmdRecord);
			this.pnlControls.Controls.Add(this.cmdStop);
			this.pnlControls.Controls.Add(this.cmdPause);
			this.pnlControls.Controls.Add(this.cmdPlayFromPoint);
			this.pnlControls.Controls.Add(this.cmdPlay);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlControls.Location = new System.Drawing.Point(0, 259);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(577, 29);
			this.pnlControls.TabIndex = 18;
			// 
			// lblTime
			// 
			this.lblTime.AutoSize = true;
			this.lblTime.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.Location = new System.Drawing.Point(497, 0);
			this.lblTime.Name = "lblTime";
			this.lblTime.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lblTime.Size = new System.Drawing.Size(50, 18);
			this.lblTime.TabIndex = 7;
			this.lblTime.Text = "0:00 00";
			// 
			// lblFrameRate
			// 
			this.lblFrameRate.AutoSize = true;
			this.lblFrameRate.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblFrameRate.Location = new System.Drawing.Point(547, 0);
			this.lblFrameRate.Name = "lblFrameRate";
			this.lblFrameRate.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lblFrameRate.Size = new System.Drawing.Size(30, 18);
			this.lblFrameRate.TabIndex = 6;
			this.lblFrameRate.Text = "0 fps";
			// 
			// lblRecording
			// 
			this.lblRecording.AutoSize = true;
			this.lblRecording.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRecording.Location = new System.Drawing.Point(122, 8);
			this.lblRecording.Name = "lblRecording";
			this.lblRecording.Size = new System.Drawing.Size(96, 13);
			this.lblRecording.TabIndex = 5;
			this.lblRecording.Text = "(Recording Video...)";
			this.lblRecording.Visible = false;
			// 
			// cmdRecord
			// 
			this.cmdRecord.BackColor = System.Drawing.Color.White;
			this.cmdRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmdRecord.Image = global::ElfCore.Properties.Resources.record;
			this.cmdRecord.Location = new System.Drawing.Point(100, 6);
			this.cmdRecord.Name = "cmdRecord";
			this.cmdRecord.Size = new System.Drawing.Size(16, 16);
			this.cmdRecord.TabIndex = 4;
			this.cmdRecord.UseVisualStyleBackColor = false;
			this.cmdRecord.Visible = false;
			// 
			// cmdStop
			// 
			this.cmdStop.BackColor = System.Drawing.Color.White;
			this.cmdStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmdStop.Image = global::ElfCore.Properties.Resources.stop;
			this.cmdStop.Location = new System.Drawing.Point(78, 6);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.Size = new System.Drawing.Size(16, 16);
			this.cmdStop.TabIndex = 3;
			this.cmdStop.UseVisualStyleBackColor = false;
			// 
			// cmdPause
			// 
			this.cmdPause.BackColor = System.Drawing.Color.White;
			this.cmdPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmdPause.Image = global::ElfCore.Properties.Resources.pause;
			this.cmdPause.Location = new System.Drawing.Point(56, 6);
			this.cmdPause.Name = "cmdPause";
			this.cmdPause.Size = new System.Drawing.Size(16, 16);
			this.cmdPause.TabIndex = 2;
			this.cmdPause.UseVisualStyleBackColor = false;
			// 
			// cmdPlayFromPoint
			// 
			this.cmdPlayFromPoint.BackColor = System.Drawing.Color.White;
			this.cmdPlayFromPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmdPlayFromPoint.Image = global::ElfCore.Properties.Resources.play_from_point;
			this.cmdPlayFromPoint.Location = new System.Drawing.Point(34, 6);
			this.cmdPlayFromPoint.Name = "cmdPlayFromPoint";
			this.cmdPlayFromPoint.Size = new System.Drawing.Size(16, 16);
			this.cmdPlayFromPoint.TabIndex = 1;
			this.cmdPlayFromPoint.UseVisualStyleBackColor = false;
			// 
			// cmdPlay
			// 
			this.cmdPlay.BackColor = System.Drawing.Color.White;
			this.cmdPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmdPlay.Image = global::ElfCore.Properties.Resources.play;
			this.cmdPlay.Location = new System.Drawing.Point(12, 6);
			this.cmdPlay.Name = "cmdPlay";
			this.cmdPlay.Size = new System.Drawing.Size(16, 16);
			this.cmdPlay.TabIndex = 0;
			this.cmdPlay.UseVisualStyleBackColor = false;
			// 
			// CanvasPane
			// 
			this.CanvasPane.BackColor = System.Drawing.Color.Black;
			this.CanvasPane.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CanvasPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CanvasPane.Location = new System.Drawing.Point(0, 0);
			this.CanvasPane.Name = "CanvasPane";
			this.CanvasPane.Size = new System.Drawing.Size(577, 259);
			this.CanvasPane.TabIndex = 17;
			this.CanvasPane.TabStop = false;
			this.CanvasPane.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPane_Paint);
			// 
			// PreviewPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(577, 288);
			this.Controls.Add(this.CanvasPane);
			this.Controls.Add(this.pnlControls);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "PreviewPane";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Elf Preview";
			this.TopMost = true;
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox CanvasPane;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Label lblFrameRate;
		private System.Windows.Forms.Label lblRecording;
		private System.Windows.Forms.Button cmdRecord;
		private System.Windows.Forms.Button cmdStop;
		private System.Windows.Forms.Button cmdPause;
		private System.Windows.Forms.Button cmdPlayFromPoint;
		private System.Windows.Forms.Button cmdPlay;
	}
}