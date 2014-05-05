namespace ElfCore.Forms
{
	partial class ProfileSplash
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
			this.cmdOk = new System.Windows.Forms.Button();
			this.tmrVanish = new System.Windows.Forms.Timer(this.components);
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblDetails = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(517, 414);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 2;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// tmrVanish
			// 
			this.tmrVanish.Interval = 2500;
			this.tmrVanish.Tick += new System.EventHandler(this.tmrVanish_Tick);
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.BackColor = System.Drawing.Color.Transparent;
			this.lblVersion.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersion.Location = new System.Drawing.Point(22, 323);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(79, 18);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "lblVersion";
			// 
			// lblDetails
			// 
			this.lblDetails.BackColor = System.Drawing.Color.Transparent;
			this.lblDetails.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDetails.Location = new System.Drawing.Point(22, 341);
			this.lblDetails.Name = "lblDetails";
			this.lblDetails.Size = new System.Drawing.Size(352, 103);
			this.lblDetails.TabIndex = 1;
			this.lblDetails.Text = "lblDetails\r\nElfCore Version\r\nElfControls Version\r\nElfTools Version\r\nDocking Versi" +
    "on";
			// 
			// ProfileSplash
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImage = global::ElfCore.Properties.Resources.ProfileEditorSplash;
			this.ClientSize = new System.Drawing.Size(607, 469);
			this.ControlBox = false;
			this.Controls.Add(this.lblDetails);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.cmdOk);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfileSplash";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.Fuchsia;
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Timer tmrVanish;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblDetails;

	}
}