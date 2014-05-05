namespace ElfCore.Forms
{
	partial class ChannelProperties
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelProperties));
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(212, 332);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 28);
			this.cmdCancel.TabIndex = 13;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(141, 332);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 12;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Location = new System.Drawing.Point(9, 9);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(415, 312);
			this.propertyGrid1.TabIndex = 14;
			// 
			// ChannelProperties
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(436, 374);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChannelProperties";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Channel Properties";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
	}
}