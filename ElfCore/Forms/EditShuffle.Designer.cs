namespace ElfCore.Forms
{
	partial class EditShuffle
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditShuffle));
			this._txtName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this._lstUnsorted = new System.Windows.Forms.Label();
			this._lstSorted = new System.Windows.Forms.Label();
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.cmdUp = new System.Windows.Forms.Button();
			this.cmdToTop = new System.Windows.Forms.Button();
			this.cmdToBottom = new System.Windows.Forms.Button();
			this.cmdDown = new System.Windows.Forms.Button();
			this.lstSorted = new ElfControls.ImageListBox();
			this.lstUnsorted = new ElfControls.ImageListBox();
			this.SuspendLayout();
			// 
			// _txtName
			// 
			this._txtName.AutoSize = true;
			this._txtName.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._txtName.Location = new System.Drawing.Point(9, 10);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(99, 15);
			this._txtName.TabIndex = 0;
			this._txtName.Text = "Sort Order Name:";
			// 
			// txtName
			// 
			this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.txtName.Location = new System.Drawing.Point(12, 28);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(253, 23);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// _lstUnsorted
			// 
			this._lstUnsorted.AutoSize = true;
			this._lstUnsorted.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._lstUnsorted.Location = new System.Drawing.Point(9, 54);
			this._lstUnsorted.Name = "_lstUnsorted";
			this._lstUnsorted.Size = new System.Drawing.Size(110, 15);
			this._lstUnsorted.TabIndex = 2;
			this._lstUnsorted.Text = "Unsorted Channels:";
			// 
			// _lstSorted
			// 
			this._lstSorted.AutoSize = true;
			this._lstSorted.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._lstSorted.Location = new System.Drawing.Point(309, 54);
			this._lstSorted.Name = "_lstSorted";
			this._lstSorted.Size = new System.Drawing.Size(96, 15);
			this._lstSorted.TabIndex = 6;
			this._lstSorted.Text = "Sorted Channels:";
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(232, 412);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 12;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(303, 412);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 13;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdAdd
			// 
			this.cmdAdd.Image = global::ElfCore.Properties.Resources.right;
			this.cmdAdd.Location = new System.Drawing.Point(271, 194);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(35, 26);
			this.cmdAdd.TabIndex = 4;
			this.cmdAdd.UseVisualStyleBackColor = true;
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdRemove
			// 
			this.cmdRemove.Image = global::ElfCore.Properties.Resources.left;
			this.cmdRemove.Location = new System.Drawing.Point(271, 226);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.Size = new System.Drawing.Size(35, 26);
			this.cmdRemove.TabIndex = 5;
			this.cmdRemove.UseVisualStyleBackColor = true;
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// cmdUp
			// 
			this.cmdUp.Image = global::ElfCore.Properties.Resources.up;
			this.cmdUp.Location = new System.Drawing.Point(571, 104);
			this.cmdUp.Name = "cmdUp";
			this.cmdUp.Size = new System.Drawing.Size(25, 26);
			this.cmdUp.TabIndex = 9;
			this.cmdUp.UseVisualStyleBackColor = true;
			this.cmdUp.Click += new System.EventHandler(this.cmdUp_Click);
			// 
			// cmdToTop
			// 
			this.cmdToTop.Image = global::ElfCore.Properties.Resources.to_top;
			this.cmdToTop.Location = new System.Drawing.Point(571, 72);
			this.cmdToTop.Name = "cmdToTop";
			this.cmdToTop.Size = new System.Drawing.Size(25, 26);
			this.cmdToTop.TabIndex = 8;
			this.cmdToTop.UseVisualStyleBackColor = true;
			this.cmdToTop.Click += new System.EventHandler(this.cmdToTop_Click);
			// 
			// cmdToBottom
			// 
			this.cmdToBottom.Image = global::ElfCore.Properties.Resources.to_bottom;
			this.cmdToBottom.Location = new System.Drawing.Point(571, 375);
			this.cmdToBottom.Name = "cmdToBottom";
			this.cmdToBottom.Size = new System.Drawing.Size(27, 26);
			this.cmdToBottom.TabIndex = 11;
			this.cmdToBottom.UseVisualStyleBackColor = true;
			this.cmdToBottom.Click += new System.EventHandler(this.cmdToBottom_Click);
			// 
			// cmdDown
			// 
			this.cmdDown.Image = global::ElfCore.Properties.Resources.down;
			this.cmdDown.Location = new System.Drawing.Point(571, 343);
			this.cmdDown.Name = "cmdDown";
			this.cmdDown.Size = new System.Drawing.Size(25, 26);
			this.cmdDown.TabIndex = 10;
			this.cmdDown.UseVisualStyleBackColor = true;
			this.cmdDown.Click += new System.EventHandler(this.cmdDown_Click);
			// 
			// lstSorted
			// 
			this.lstSorted.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstSorted.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstSorted.FormattingEnabled = true;
			this.lstSorted.Location = new System.Drawing.Point(312, 72);
			this.lstSorted.Name = "lstSorted";
			this.lstSorted.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstSorted.Size = new System.Drawing.Size(253, 329);
			this.lstSorted.TabIndex = 7;
			this.lstSorted.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
			this.lstSorted.DoubleClick += new System.EventHandler(this.cmdRemove_Click);
			// 
			// lstUnsorted
			// 
			this.lstUnsorted.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstUnsorted.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstUnsorted.FormattingEnabled = true;
			this.lstUnsorted.Location = new System.Drawing.Point(12, 72);
			this.lstUnsorted.Name = "lstUnsorted";
			this.lstUnsorted.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstUnsorted.Size = new System.Drawing.Size(253, 329);
			this.lstUnsorted.TabIndex = 3;
			this.lstUnsorted.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
			this.lstUnsorted.DoubleClick += new System.EventHandler(this.cmdAdd_Click);
			// 
			// EditShuffle
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(606, 454);
			this.Controls.Add(this.lstUnsorted);
			this.Controls.Add(this.lstSorted);
			this.Controls.Add(this.cmdToBottom);
			this.Controls.Add(this.cmdDown);
			this.Controls.Add(this.cmdUp);
			this.Controls.Add(this.cmdToTop);
			this.Controls.Add(this.cmdRemove);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this._lstSorted);
			this.Controls.Add(this._lstUnsorted);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditShuffle";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Sort Order";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Label _txtName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label _lstUnsorted;
		private System.Windows.Forms.Label _lstSorted;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Button cmdUp;
		private System.Windows.Forms.Button cmdToTop;
		private System.Windows.Forms.Button cmdToBottom;
		private System.Windows.Forms.Button cmdDown;
		private ElfControls.ImageListBox lstSorted;
		private ElfControls.ImageListBox lstUnsorted;
	}
}