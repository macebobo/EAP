namespace ElfCore
{
	partial class Brightness
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Brightness));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.tbBrightness = new System.Windows.Forms.TrackBar();
			this.lblBlack = new System.Windows.Forms.Label();
			this.lblLight = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.not;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(112, 67);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 28);
			this.cmdCancel.TabIndex = 4;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.Ok;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(41, 67);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 3;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// tbBrightness
			// 
			this.tbBrightness.LargeChange = 1;
			this.tbBrightness.Location = new System.Drawing.Point(51, 16);
			this.tbBrightness.Maximum = 20;
			this.tbBrightness.Name = "tbBrightness";
			this.tbBrightness.Size = new System.Drawing.Size(144, 45);
			this.tbBrightness.TabIndex = 1;
			this.tbBrightness.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbBrightness.Value = 10;
			this.tbBrightness.ValueChanged += new System.EventHandler(this.tbBrightness_ValueChanged);
			// 
			// lblBlack
			// 
			this.lblBlack.AutoSize = true;
			this.lblBlack.Location = new System.Drawing.Point(15, 27);
			this.lblBlack.Name = "lblBlack";
			this.lblBlack.Size = new System.Drawing.Size(30, 13);
			this.lblBlack.TabIndex = 0;
			this.lblBlack.Text = "Dark";
			// 
			// lblLight
			// 
			this.lblLight.AutoSize = true;
			this.lblLight.Location = new System.Drawing.Point(201, 27);
			this.lblLight.Name = "lblLight";
			this.lblLight.Size = new System.Drawing.Size(30, 13);
			this.lblLight.TabIndex = 2;
			this.lblLight.Text = "Light";
			// 
			// Brightness
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(250, 109);
			this.Controls.Add(this.lblLight);
			this.Controls.Add(this.tbBrightness);
			this.Controls.Add(this.lblBlack);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Brightness";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Brightness";
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.TrackBar tbBrightness;
		private System.Windows.Forms.Label lblBlack;
		private System.Windows.Forms.Label lblLight;
	}
}