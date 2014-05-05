namespace ElfCore.Forms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureKeys));
			this.lstCommands = new System.Windows.Forms.ListBox();
			this._txtExisting = new System.Windows.Forms.Label();
			this.txtExisting = new System.Windows.Forms.TextBox();
			this._txtNewKey = new System.Windows.Forms.Label();
			this.txtNewKey = new System.Windows.Forms.TextBox();
			this._txtConflicts = new System.Windows.Forms.Label();
			this.txtConflicts = new System.Windows.Forms.TextBox();
			this.cmdAssign = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOk = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this._txtSearch = new System.Windows.Forms.Label();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.cmdDefault = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdLoad = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstCommands
			// 
			this.lstCommands.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstCommands.FormattingEnabled = true;
			this.lstCommands.ItemHeight = 15;
			this.lstCommands.Location = new System.Drawing.Point(12, 58);
			this.lstCommands.Name = "lstCommands";
			this.lstCommands.Size = new System.Drawing.Size(380, 124);
			this.lstCommands.TabIndex = 2;
			this.toolTip1.SetToolTip(this.lstCommands, "All (or part) commands available for keyboard shortcut assignments");
			this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
			// 
			// _txtExisting
			// 
			this._txtExisting.AutoSize = true;
			this._txtExisting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtExisting.Location = new System.Drawing.Point(9, 186);
			this._txtExisting.Name = "_txtExisting";
			this._txtExisting.Size = new System.Drawing.Size(182, 15);
			this._txtExisting.TabIndex = 3;
			this._txtExisting.Text = "Shortcuts for selected command:";
			// 
			// txtExisting
			// 
			this.txtExisting.BackColor = System.Drawing.SystemColors.Info;
			this.txtExisting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtExisting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtExisting.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtExisting.Location = new System.Drawing.Point(12, 201);
			this.txtExisting.Name = "txtExisting";
			this.txtExisting.ReadOnly = true;
			this.txtExisting.Size = new System.Drawing.Size(289, 23);
			this.txtExisting.TabIndex = 4;
			this.txtExisting.TabStop = false;
			this.txtExisting.Text = "Ctrl+Alt+Shift+Backspace";
			this.toolTip1.SetToolTip(this.txtExisting, "Current shortcut for the selected command");
			this.txtExisting.TextChanged += new System.EventHandler(this.txtExisting_TextChanged);
			// 
			// _txtNewKey
			// 
			this._txtNewKey.AutoSize = true;
			this._txtNewKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtNewKey.Location = new System.Drawing.Point(9, 230);
			this._txtNewKey.Name = "_txtNewKey";
			this._txtNewKey.Size = new System.Drawing.Size(110, 15);
			this._txtNewKey.TabIndex = 6;
			this._txtNewKey.Text = "Press shortcut keys:";
			// 
			// txtNewKey
			// 
			this.txtNewKey.BackColor = System.Drawing.SystemColors.Window;
			this.txtNewKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNewKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNewKey.Location = new System.Drawing.Point(12, 248);
			this.txtNewKey.Name = "txtNewKey";
			this.txtNewKey.ReadOnly = true;
			this.txtNewKey.Size = new System.Drawing.Size(289, 23);
			this.txtNewKey.TabIndex = 7;
			this.toolTip1.SetToolTip(this.txtNewKey, "Enter the keystrokes for the new shortcut");
			this.txtNewKey.TextChanged += new System.EventHandler(this.txtNewKey_TextChanged);
			this.txtNewKey.Enter += new System.EventHandler(this.txtNewKey_Enter);
			this.txtNewKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewKey_KeyDown);
			this.txtNewKey.Leave += new System.EventHandler(this.txtNewKey_Leave);
			// 
			// _txtConflicts
			// 
			this._txtConflicts.AutoSize = true;
			this._txtConflicts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtConflicts.Location = new System.Drawing.Point(9, 274);
			this._txtConflicts.Name = "_txtConflicts";
			this._txtConflicts.Size = new System.Drawing.Size(85, 15);
			this._txtConflicts.TabIndex = 9;
			this._txtConflicts.Text = "Conflicts With:";
			// 
			// txtConflicts
			// 
			this.txtConflicts.BackColor = System.Drawing.SystemColors.Info;
			this.txtConflicts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtConflicts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConflicts.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtConflicts.Location = new System.Drawing.Point(12, 292);
			this.txtConflicts.Name = "txtConflicts";
			this.txtConflicts.ReadOnly = true;
			this.txtConflicts.Size = new System.Drawing.Size(380, 23);
			this.txtConflicts.TabIndex = 10;
			this.txtConflicts.TabStop = false;
			this.txtConflicts.Text = "Menu,File,Save";
			this.toolTip1.SetToolTip(this.txtConflicts, "Commands that conflict with all or the first part of the indicated set of keystro" +
        "kes");
			// 
			// cmdAssign
			// 
			this.cmdAssign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdAssign.Image = global::ElfCore.Properties.Resources.keyboard;
			this.cmdAssign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdAssign.Location = new System.Drawing.Point(307, 246);
			this.cmdAssign.Name = "cmdAssign";
			this.cmdAssign.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdAssign.Size = new System.Drawing.Size(85, 25);
			this.cmdAssign.TabIndex = 8;
			this.cmdAssign.Text = "&Assign";
			this.cmdAssign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdAssign, "Assign these keystrokes to the currently selected command");
			this.cmdAssign.UseVisualStyleBackColor = true;
			this.cmdAssign.Click += new System.EventHandler(this.cmdAssign_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(207, 361);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 13;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(136, 361);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 12;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			// 
			// txtSearch
			// 
			this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSearch.Location = new System.Drawing.Point(12, 29);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(380, 23);
			this.txtSearch.TabIndex = 1;
			this.toolTip1.SetToolTip(this.txtSearch, "Search for command using all or part of this text");
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// _txtSearch
			// 
			this._txtSearch.AutoSize = true;
			this._txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtSearch.Location = new System.Drawing.Point(12, 11);
			this._txtSearch.Name = "_txtSearch";
			this._txtSearch.Size = new System.Drawing.Size(162, 15);
			this._txtSearch.TabIndex = 0;
			this._txtSearch.Text = "Show commands containing:";
			// 
			// cmdRemove
			// 
			this.cmdRemove.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdRemove.Image = global::ElfCore.Properties.Resources.keyboard;
			this.cmdRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdRemove.Location = new System.Drawing.Point(307, 201);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdRemove.Size = new System.Drawing.Size(85, 25);
			this.cmdRemove.TabIndex = 5;
			this.cmdRemove.Text = "Remo&ve";
			this.cmdRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdRemove, "Remove current shortcut assignment");
			this.cmdRemove.UseVisualStyleBackColor = true;
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// cmdDefault
			// 
			this.cmdDefault.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdDefault.Image = global::ElfCore.Properties.Resources.undo;
			this.cmdDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdDefault.Location = new System.Drawing.Point(12, 321);
			this.cmdDefault.Name = "cmdDefault";
			this.cmdDefault.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdDefault.Size = new System.Drawing.Size(137, 25);
			this.cmdDefault.TabIndex = 11;
			this.cmdDefault.Text = "&Reset To Default";
			this.cmdDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdDefault, "Reset the program to the initial keyboard configuration");
			this.cmdDefault.UseVisualStyleBackColor = true;
			this.cmdDefault.Click += new System.EventHandler(this.cmdDefault_Click);
			// 
			// cmdSave
			// 
			this.cmdSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdSave.Image = global::ElfCore.Properties.Resources.save;
			this.cmdSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdSave.Location = new System.Drawing.Point(279, 321);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdSave.Size = new System.Drawing.Size(113, 25);
			this.cmdSave.TabIndex = 14;
			this.cmdSave.Text = "&Save Config";
			this.cmdSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdSave, "Reset the program to the initial keyboard configuration");
			this.cmdSave.UseVisualStyleBackColor = true;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdLoad
			// 
			this.cmdLoad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdLoad.Image = global::ElfCore.Properties.Resources.open;
			this.cmdLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdLoad.Location = new System.Drawing.Point(158, 321);
			this.cmdLoad.Name = "cmdLoad";
			this.cmdLoad.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdLoad.Size = new System.Drawing.Size(113, 25);
			this.cmdLoad.TabIndex = 15;
			this.cmdLoad.Text = "&Load Config";
			this.cmdLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdLoad, "Reset the program to the initial keyboard configuration");
			this.cmdLoad.UseVisualStyleBackColor = true;
			this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
			// 
			// ConfigureKeys
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(407, 405);
			this.Controls.Add(this.cmdLoad);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cmdDefault);
			this.Controls.Add(this.cmdRemove);
			this.Controls.Add(this.txtSearch);
			this.Controls.Add(this._txtSearch);
			this.Controls.Add(this.cmdAssign);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Controls.Add(this.txtConflicts);
			this.Controls.Add(this._txtConflicts);
			this.Controls.Add(this.txtNewKey);
			this.Controls.Add(this._txtNewKey);
			this.Controls.Add(this.txtExisting);
			this.Controls.Add(this._txtExisting);
			this.Controls.Add(this.lstCommands);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigureKeys";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Keyboard Shortcuts";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lstCommands;
		private System.Windows.Forms.Label _txtExisting;
		private System.Windows.Forms.TextBox txtExisting;
		private System.Windows.Forms.Label _txtNewKey;
		private System.Windows.Forms.TextBox txtNewKey;
		private System.Windows.Forms.Label _txtConflicts;
		private System.Windows.Forms.TextBox txtConflicts;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Button cmdAssign;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.Label _txtSearch;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Button cmdDefault;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.Button cmdLoad;
	}
}