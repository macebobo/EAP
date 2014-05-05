namespace ElfCore
{
	partial class ImportFromProfile
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
			this.lstImportedChannels = new System.Windows.Forms.ListBox();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.cmdMapAll = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.lblCurrentProfile = new System.Windows.Forms.Label();
			this.lblCurrentSize = new System.Windows.Forms.Label();
			this._lblCurrentSize = new System.Windows.Forms.Label();
			this.lblSize = new System.Windows.Forms.Label();
			this._lblSize = new System.Windows.Forms.Label();
			this.lblProfileName = new System.Windows.Forms.Label();
			this._lblProfileName = new System.Windows.Forms.Label();
			this.ChannelList = new System.Windows.Forms.ListBox();
			this.cmdMap = new System.Windows.Forms.Button();
			this.cmdUnmap = new System.Windows.Forms.Button();
			this.chkClearClannel = new System.Windows.Forms.CheckBox();
			this.pnlButtons.SuspendLayout();
			this.pnlTitle.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstImportedChannels
			// 
			this.lstImportedChannels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lstImportedChannels.Dock = System.Windows.Forms.DockStyle.Left;
			this.lstImportedChannels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.lstImportedChannels.FormattingEnabled = true;
			this.lstImportedChannels.IntegralHeight = false;
			this.lstImportedChannels.ItemHeight = 16;
			this.lstImportedChannels.Location = new System.Drawing.Point(5, 55);
			this.lstImportedChannels.Name = "lstImportedChannels";
			this.lstImportedChannels.Size = new System.Drawing.Size(235, 464);
			this.lstImportedChannels.TabIndex = 1;
			this.lstImportedChannels.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstImportedChannels_DrawItem);
			this.lstImportedChannels.SelectedIndexChanged += new System.EventHandler(this.lstImportedChannels_SelectedIndexChanged);
			// 
			// pnlButtons
			// 
			this.pnlButtons.Controls.Add(this.chkClearClannel);
			this.pnlButtons.Controls.Add(this.cmdMapAll);
			this.pnlButtons.Controls.Add(this.cmdCancel);
			this.pnlButtons.Controls.Add(this.cmdOk);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(5, 519);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(586, 70);
			this.pnlButtons.TabIndex = 5;
			// 
			// cmdMapAll
			// 
			this.cmdMapAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdMapAll.Image = global::ElfCore.Properties.Resources.map_all;
			this.cmdMapAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdMapAll.Location = new System.Drawing.Point(148, 33);
			this.cmdMapAll.Name = "cmdMapAll";
			this.cmdMapAll.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdMapAll.Size = new System.Drawing.Size(131, 28);
			this.cmdMapAll.TabIndex = 0;
			this.cmdMapAll.Text = "Map All Channels";
			this.cmdMapAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdMapAll.UseVisualStyleBackColor = true;
			this.cmdMapAll.Click += new System.EventHandler(this.cmdMapAll_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.not;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(356, 33);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 28);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.Ok;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(285, 33);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 1;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// pnlTitle
			// 
			this.pnlTitle.Controls.Add(this.lblCurrentProfile);
			this.pnlTitle.Controls.Add(this.lblCurrentSize);
			this.pnlTitle.Controls.Add(this._lblCurrentSize);
			this.pnlTitle.Controls.Add(this.lblSize);
			this.pnlTitle.Controls.Add(this._lblSize);
			this.pnlTitle.Controls.Add(this.lblProfileName);
			this.pnlTitle.Controls.Add(this._lblProfileName);
			this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTitle.Location = new System.Drawing.Point(5, 5);
			this.pnlTitle.Name = "pnlTitle";
			this.pnlTitle.Size = new System.Drawing.Size(586, 50);
			this.pnlTitle.TabIndex = 0;
			// 
			// lblCurrentProfile
			// 
			this.lblCurrentProfile.AutoSize = true;
			this.lblCurrentProfile.BackColor = System.Drawing.SystemColors.Control;
			this.lblCurrentProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCurrentProfile.Location = new System.Drawing.Point(336, 9);
			this.lblCurrentProfile.Name = "lblCurrentProfile";
			this.lblCurrentProfile.Size = new System.Drawing.Size(88, 13);
			this.lblCurrentProfile.TabIndex = 4;
			this.lblCurrentProfile.Text = "Current Profile";
			// 
			// lblCurrentSize
			// 
			this.lblCurrentSize.AutoSize = true;
			this.lblCurrentSize.BackColor = System.Drawing.SystemColors.Control;
			this.lblCurrentSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCurrentSize.Location = new System.Drawing.Point(441, 26);
			this.lblCurrentSize.Name = "lblCurrentSize";
			this.lblCurrentSize.Size = new System.Drawing.Size(0, 13);
			this.lblCurrentSize.TabIndex = 6;
			// 
			// _lblCurrentSize
			// 
			this._lblCurrentSize.AutoSize = true;
			this._lblCurrentSize.Location = new System.Drawing.Point(336, 26);
			this._lblCurrentSize.Name = "_lblCurrentSize";
			this._lblCurrentSize.Size = new System.Drawing.Size(99, 13);
			this._lblCurrentSize.TabIndex = 5;
			this._lblCurrentSize.Text = "Current Profile Size:";
			// 
			// lblSize
			// 
			this.lblSize.AutoSize = true;
			this.lblSize.BackColor = System.Drawing.SystemColors.Control;
			this.lblSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSize.Location = new System.Drawing.Point(107, 26);
			this.lblSize.Name = "lblSize";
			this.lblSize.Size = new System.Drawing.Size(63, 13);
			this.lblSize.TabIndex = 3;
			this.lblSize.Text = "200 x 300";
			// 
			// _lblSize
			// 
			this._lblSize.AutoSize = true;
			this._lblSize.Location = new System.Drawing.Point(68, 26);
			this._lblSize.Name = "_lblSize";
			this._lblSize.Size = new System.Drawing.Size(30, 13);
			this._lblSize.TabIndex = 2;
			this._lblSize.Text = "Size:";
			// 
			// lblProfileName
			// 
			this.lblProfileName.AutoSize = true;
			this.lblProfileName.BackColor = System.Drawing.SystemColors.Control;
			this.lblProfileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProfileName.Location = new System.Drawing.Point(104, 9);
			this.lblProfileName.Name = "lblProfileName";
			this.lblProfileName.Size = new System.Drawing.Size(43, 13);
			this.lblProfileName.TabIndex = 1;
			this.lblProfileName.Text = "Profile";
			// 
			// _lblProfileName
			// 
			this._lblProfileName.AutoSize = true;
			this._lblProfileName.Location = new System.Drawing.Point(12, 9);
			this._lblProfileName.Name = "_lblProfileName";
			this._lblProfileName.Size = new System.Drawing.Size(86, 13);
			this._lblProfileName.TabIndex = 0;
			this._lblProfileName.Text = "Imported Profile: ";
			// 
			// ChannelList
			// 
			this.ChannelList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ChannelList.Dock = System.Windows.Forms.DockStyle.Right;
			this.ChannelList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.ChannelList.FormattingEnabled = true;
			this.ChannelList.IntegralHeight = false;
			this.ChannelList.ItemHeight = 16;
			this.ChannelList.Location = new System.Drawing.Point(356, 55);
			this.ChannelList.Name = "ChannelList";
			this.ChannelList.Size = new System.Drawing.Size(235, 464);
			this.ChannelList.TabIndex = 2;
			this.ChannelList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstChannels_DrawItem);
			this.ChannelList.SelectedIndexChanged += new System.EventHandler(this.lstChannels_SelectedIndexChanged);
			// 
			// cmdMap
			// 
			this.cmdMap.Enabled = false;
			this.cmdMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdMap.Image = global::ElfCore.Properties.Resources.map;
			this.cmdMap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdMap.Location = new System.Drawing.Point(262, 205);
			this.cmdMap.Name = "cmdMap";
			this.cmdMap.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdMap.Size = new System.Drawing.Size(72, 24);
			this.cmdMap.TabIndex = 3;
			this.cmdMap.Text = "Map";
			this.cmdMap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdMap.UseVisualStyleBackColor = true;
			this.cmdMap.Click += new System.EventHandler(this.cmdMap_Click);
			// 
			// cmdUnmap
			// 
			this.cmdUnmap.Enabled = false;
			this.cmdUnmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdUnmap.Image = global::ElfCore.Properties.Resources.unmap;
			this.cmdUnmap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdUnmap.Location = new System.Drawing.Point(256, 235);
			this.cmdUnmap.Name = "cmdUnmap";
			this.cmdUnmap.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdUnmap.Size = new System.Drawing.Size(85, 24);
			this.cmdUnmap.TabIndex = 4;
			this.cmdUnmap.Text = "Unmap";
			this.cmdUnmap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdUnmap.UseVisualStyleBackColor = true;
			this.cmdUnmap.Click += new System.EventHandler(this.cmdUnmap_Click);
			// 
			// chkClearClannel
			// 
			this.chkClearClannel.AutoSize = true;
			this.chkClearClannel.Location = new System.Drawing.Point(211, 10);
			this.chkClearClannel.Name = "chkClearClannel";
			this.chkClearClannel.Size = new System.Drawing.Size(164, 17);
			this.chkClearClannel.TabIndex = 8;
			this.chkClearClannel.Text = "&Clear Channel Before Import?";
			this.chkClearClannel.UseVisualStyleBackColor = true;
			// 
			// ImportFromProfile
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(596, 594);
			this.Controls.Add(this.ChannelList);
			this.Controls.Add(this.lstImportedChannels);
			this.Controls.Add(this.pnlButtons);
			this.Controls.Add(this.pnlTitle);
			this.Controls.Add(this.cmdUnmap);
			this.Controls.Add(this.cmdMap);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ImportFromProfile";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Channel(s)";
			this.pnlButtons.ResumeLayout(false);
			this.pnlButtons.PerformLayout();
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lstImportedChannels;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Panel pnlTitle;
		private System.Windows.Forms.Label lblProfileName;
		private System.Windows.Forms.Label _lblProfileName;
		private System.Windows.Forms.ListBox ChannelList;
		private System.Windows.Forms.Button cmdMap;
		private System.Windows.Forms.Button cmdUnmap;
		private System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.Label _lblSize;
		private System.Windows.Forms.Label lblCurrentProfile;
		private System.Windows.Forms.Label lblCurrentSize;
		private System.Windows.Forms.Label _lblCurrentSize;
		private System.Windows.Forms.Button cmdMapAll;
		private System.Windows.Forms.CheckBox chkClearClannel;
	}
}