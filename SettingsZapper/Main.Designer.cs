namespace SettingsZapper
{
	partial class Main
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lblMsg = new System.Windows.Forms.Label();
			this.lblClick = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::SettingsZapper.Properties.Resources.zapper48;
			this.pictureBox1.Location = new System.Drawing.Point(30, 33);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(48, 48);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.CloseMe_Click);
			// 
			// lblMsg
			// 
			this.lblMsg.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMsg.Location = new System.Drawing.Point(84, 13);
			this.lblMsg.Name = "lblMsg";
			this.lblMsg.Size = new System.Drawing.Size(183, 89);
			this.lblMsg.TabIndex = 1;
			this.lblMsg.Text = "Your Settings for the Adjustable Preview PlugIn for Vixen 2.x have been zapped!";
			this.lblMsg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.lblMsg.Click += new System.EventHandler(this.CloseMe_Click);
			// 
			// lblClick
			// 
			this.lblClick.AutoSize = true;
			this.lblClick.Location = new System.Drawing.Point(134, 89);
			this.lblClick.Name = "lblClick";
			this.lblClick.Size = new System.Drawing.Size(75, 13);
			this.lblClick.TabIndex = 2;
			this.lblClick.Text = "(click to close)";
			this.lblClick.Click += new System.EventHandler(this.CloseMe_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 114);
			this.ControlBox = false;
			this.Controls.Add(this.lblClick);
			this.Controls.Add(this.lblMsg);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Click += new System.EventHandler(this.CloseMe_Click);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label lblMsg;
		private System.Windows.Forms.Label lblClick;
	}
}

