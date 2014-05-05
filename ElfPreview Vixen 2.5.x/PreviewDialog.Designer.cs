namespace ElfPreview
{
	partial class PreviewDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewDialog));
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
			this.pnlControls.Location = new System.Drawing.Point(0, 274);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Padding = new System.Windows.Forms.Padding(2);
			this.pnlControls.Size = new System.Drawing.Size(589, 29);
			this.pnlControls.TabIndex = 18;
			// 
			// lblTime
			// 
			this.lblTime.AutoSize = true;
			this.lblTime.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblTime.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.Location = new System.Drawing.Point(507, 2);
			this.lblTime.Name = "lblTime";
			this.lblTime.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lblTime.Size = new System.Drawing.Size(48, 20);
			this.lblTime.TabIndex = 7;
			this.lblTime.Text = "0:00 00";
			this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblFrameRate
			// 
			this.lblFrameRate.AutoSize = true;
			this.lblFrameRate.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblFrameRate.Location = new System.Drawing.Point(555, 2);
			this.lblFrameRate.Name = "lblFrameRate";
			this.lblFrameRate.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lblFrameRate.Size = new System.Drawing.Size(32, 20);
			this.lblFrameRate.TabIndex = 6;
			this.lblFrameRate.Text = "0 fps";
			this.lblFrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblRecording
			// 
			this.lblRecording.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRecording.Location = new System.Drawing.Point(122, 2);
			this.lblRecording.Name = "lblRecording";
			this.lblRecording.Size = new System.Drawing.Size(96, 25);
			this.lblRecording.TabIndex = 5;
			this.lblRecording.Text = "(Recording Video...)";
			this.lblRecording.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblRecording.Visible = false;
			// 
			// cmdRecord
			// 
			this.cmdRecord.BackColor = System.Drawing.SystemColors.Control;
			this.cmdRecord.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdRecord.Image = global::ElfPreview.Properties.Resources.record;
			this.cmdRecord.Location = new System.Drawing.Point(98, 2);
			this.cmdRecord.Name = "cmdRecord";
			this.cmdRecord.Size = new System.Drawing.Size(22, 22);
			this.cmdRecord.TabIndex = 4;
			this.cmdRecord.UseVisualStyleBackColor = false;
			this.cmdRecord.Visible = false;
			this.cmdRecord.Click += new System.EventHandler(this.cmdRecord_Click);
			// 
			// cmdStop
			// 
			this.cmdStop.BackColor = System.Drawing.SystemColors.Control;
			this.cmdStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdStop.Image = global::ElfPreview.Properties.Resources.stop;
			this.cmdStop.Location = new System.Drawing.Point(74, 2);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.Size = new System.Drawing.Size(22, 22);
			this.cmdStop.TabIndex = 3;
			this.cmdStop.UseVisualStyleBackColor = false;
			this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
			// 
			// cmdPause
			// 
			this.cmdPause.BackColor = System.Drawing.SystemColors.Control;
			this.cmdPause.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.cmdPause.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdPause.Image = global::ElfPreview.Properties.Resources.pause;
			this.cmdPause.Location = new System.Drawing.Point(50, 2);
			this.cmdPause.Name = "cmdPause";
			this.cmdPause.Size = new System.Drawing.Size(22, 22);
			this.cmdPause.TabIndex = 2;
			this.cmdPause.UseVisualStyleBackColor = false;
			this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
			// 
			// cmdPlayFromPoint
			// 
			this.cmdPlayFromPoint.BackColor = System.Drawing.SystemColors.Control;
			this.cmdPlayFromPoint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdPlayFromPoint.Image = global::ElfPreview.Properties.Resources.play_from_point;
			this.cmdPlayFromPoint.Location = new System.Drawing.Point(26, 2);
			this.cmdPlayFromPoint.Name = "cmdPlayFromPoint";
			this.cmdPlayFromPoint.Size = new System.Drawing.Size(22, 22);
			this.cmdPlayFromPoint.TabIndex = 1;
			this.cmdPlayFromPoint.UseVisualStyleBackColor = false;
			this.cmdPlayFromPoint.Click += new System.EventHandler(this.cmdPlayFromPoint_Click);
			// 
			// cmdPlay
			// 
			this.cmdPlay.BackColor = System.Drawing.SystemColors.Control;
			this.cmdPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdPlay.Image = global::ElfPreview.Properties.Resources.play;
			this.cmdPlay.Location = new System.Drawing.Point(2, 2);
			this.cmdPlay.Name = "cmdPlay";
			this.cmdPlay.Size = new System.Drawing.Size(22, 22);
			this.cmdPlay.TabIndex = 0;
			this.cmdPlay.UseVisualStyleBackColor = false;
			this.cmdPlay.Click += new System.EventHandler(this.cmdPlay_Click);
			// 
			// CanvasPane
			// 
			this.CanvasPane.BackColor = System.Drawing.Color.Black;
			this.CanvasPane.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CanvasPane.Location = new System.Drawing.Point(0, 0);
			this.CanvasPane.Name = "CanvasPane";
			this.CanvasPane.Size = new System.Drawing.Size(577, 259);
			this.CanvasPane.TabIndex = 17;
			this.CanvasPane.TabStop = false;
			this.CanvasPane.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPane_Paint);
			// 
			// PreviewDialog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(589, 303);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.CanvasPane);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "PreviewDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Elf Preview";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.Shown += new System.EventHandler(this.Form_Shown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
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