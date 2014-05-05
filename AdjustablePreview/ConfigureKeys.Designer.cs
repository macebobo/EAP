namespace ElfCore
{
	partial class ConfigureKeys
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureKeys));
			this._lstCommands = new System.Windows.Forms.Label();
			this.lstCommands = new System.Windows.Forms.ListBox();
			this._txtExisting = new System.Windows.Forms.Label();
			this.txtExisting = new System.Windows.Forms.TextBox();
			this._txtNewKey = new System.Windows.Forms.Label();
			this.txtNewKey = new System.Windows.Forms.TextBox();
			this._txtAssignedTo = new System.Windows.Forms.Label();
			this.txtAssignedTo = new System.Windows.Forms.TextBox();
			this.cmdDefault = new System.Windows.Forms.Button();
			this.cmdAssign = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lstCommands
			// 
			this._lstCommands.AutoSize = true;
			this._lstCommands.Location = new System.Drawing.Point(12, 9);
			this._lstCommands.Name = "_lstCommands";
			this._lstCommands.Size = new System.Drawing.Size(62, 13);
			this._lstCommands.TabIndex = 0;
			this._lstCommands.Text = "Commands:";
			// 
			// lstCommands
			// 
			this.lstCommands.FormattingEnabled = true;
			this.lstCommands.Location = new System.Drawing.Point(15, 25);
			this.lstCommands.Name = "lstCommands";
			this.lstCommands.Size = new System.Drawing.Size(165, 134);
			this.lstCommands.TabIndex = 1;
			this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
			// 
			// _txtExisting
			// 
			this._txtExisting.AutoSize = true;
			this._txtExisting.Location = new System.Drawing.Point(183, 9);
			this._txtExisting.Name = "_txtExisting";
			this._txtExisting.Size = new System.Drawing.Size(128, 13);
			this._txtExisting.TabIndex = 4;
			this._txtExisting.Text = "Existing Key Combination:";
			// 
			// txtExisting
			// 
			this.txtExisting.BackColor = System.Drawing.SystemColors.Window;
			this.txtExisting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtExisting.Location = new System.Drawing.Point(186, 25);
			this.txtExisting.Name = "txtExisting";
			this.txtExisting.ReadOnly = true;
			this.txtExisting.Size = new System.Drawing.Size(153, 20);
			this.txtExisting.TabIndex = 5;
			this.txtExisting.Text = "Ctrl + Alt + Shift + Backspace";
			// 
			// _txtNewKey
			// 
			this._txtNewKey.AutoSize = true;
			this._txtNewKey.Location = new System.Drawing.Point(186, 48);
			this._txtNewKey.Name = "_txtNewKey";
			this._txtNewKey.Size = new System.Drawing.Size(112, 13);
			this._txtNewKey.TabIndex = 6;
			this._txtNewKey.Text = "Press new (multi-) key:";
			// 
			// txtNewKey
			// 
			this.txtNewKey.Location = new System.Drawing.Point(186, 64);
			this.txtNewKey.Name = "txtNewKey";
			this.txtNewKey.Size = new System.Drawing.Size(153, 20);
			this.txtNewKey.TabIndex = 7;
			this.txtNewKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewKey_KeyDown);
			this.txtNewKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewKey_KeyPress);
			this.txtNewKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtNewKey_KeyUp);
			// 
			// _txtAssignedTo
			// 
			this._txtAssignedTo.AutoSize = true;
			this._txtAssignedTo.Location = new System.Drawing.Point(186, 87);
			this._txtAssignedTo.Name = "_txtAssignedTo";
			this._txtAssignedTo.Size = new System.Drawing.Size(113, 13);
			this._txtAssignedTo.TabIndex = 8;
			this._txtAssignedTo.Text = "Currently Assigned To:";
			// 
			// txtAssignedTo
			// 
			this.txtAssignedTo.BackColor = System.Drawing.SystemColors.Window;
			this.txtAssignedTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtAssignedTo.Location = new System.Drawing.Point(186, 103);
			this.txtAssignedTo.Name = "txtAssignedTo";
			this.txtAssignedTo.ReadOnly = true;
			this.txtAssignedTo.Size = new System.Drawing.Size(153, 20);
			this.txtAssignedTo.TabIndex = 9;
			// 
			// cmdDefault
			// 
			this.cmdDefault.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdDefault.Image = global::ElfCore.Properties.Resources.keyboard_default;
			this.cmdDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdDefault.Location = new System.Drawing.Point(264, 134);
			this.cmdDefault.Name = "cmdDefault";
			this.cmdDefault.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdDefault.Size = new System.Drawing.Size(75, 25);
			this.cmdDefault.TabIndex = 13;
			this.cmdDefault.Text = "&Default";
			this.cmdDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdDefault.UseVisualStyleBackColor = true;
			this.cmdDefault.Click += new System.EventHandler(this.cmdDefault_Click);
			// 
			// cmdAssign
			// 
			this.cmdAssign.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdAssign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdAssign.Image = global::ElfCore.Properties.Resources.keyboard_assign;
			this.cmdAssign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdAssign.Location = new System.Drawing.Point(186, 134);
			this.cmdAssign.Name = "cmdAssign";
			this.cmdAssign.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdAssign.Size = new System.Drawing.Size(72, 25);
			this.cmdAssign.TabIndex = 12;
			this.cmdAssign.Text = "&Assign";
			this.cmdAssign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdAssign.UseVisualStyleBackColor = true;
			this.cmdAssign.Click += new System.EventHandler(this.cmdAssign_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.not;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(171, 177);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(83, 28);
			this.cmdCancel.TabIndex = 11;
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
			this.cmdOk.Location = new System.Drawing.Point(100, 177);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 10;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// ConfigureKeys
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(355, 223);
			this.Controls.Add(this.cmdDefault);
			this.Controls.Add(this.cmdAssign);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Controls.Add(this.txtAssignedTo);
			this.Controls.Add(this._txtAssignedTo);
			this.Controls.Add(this.txtNewKey);
			this.Controls.Add(this._txtNewKey);
			this.Controls.Add(this.txtExisting);
			this.Controls.Add(this._txtExisting);
			this.Controls.Add(this.lstCommands);
			this.Controls.Add(this._lstCommands);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigureKeys";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Keyboard Shortcuts";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lstCommands;
		private System.Windows.Forms.ListBox lstCommands;
		private System.Windows.Forms.Label _txtExisting;
		private System.Windows.Forms.TextBox txtExisting;
		private System.Windows.Forms.Label _txtNewKey;
		private System.Windows.Forms.TextBox txtNewKey;
		private System.Windows.Forms.Label _txtAssignedTo;
		private System.Windows.Forms.TextBox txtAssignedTo;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdAssign;
		private System.Windows.Forms.Button cmdDefault;
	}
}