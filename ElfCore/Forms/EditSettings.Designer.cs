namespace ElfCore.Forms
{
	partial class EditUISettings
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
			ElfControls.ColorManager.HSL hsl2 = new ElfControls.ColorManager.HSL();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditUISettings));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkDisableUndo = new System.Windows.Forms.CheckBox();
			this.chkShowRulers = new System.Windows.Forms.CheckBox();
			this.chkNoLogPlayback = new System.Windows.Forms.CheckBox();
			this.cmdLogFileBrowse = new System.Windows.Forms.Button();
			this.txtLogFileName = new System.Windows.Forms.TextBox();
			this.cboTraceLevel = new System.Windows.Forms.ComboBox();
			this.cmdLoadKeyboard = new System.Windows.Forms.Button();
			this.cmdSaveKeyboard = new System.Windows.Forms.Button();
			this.cmdDefault = new System.Windows.Forms.Button();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.cmdAssign = new System.Windows.Forms.Button();
			this.txtConflicts = new System.Windows.Forms.TextBox();
			this.txtNewKey = new System.Windows.Forms.TextBox();
			this.txtExisting = new System.Windows.Forms.TextBox();
			this.lstCommands = new System.Windows.Forms.ListBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.lstGrouping = new ElfControls.ImageListBox();
			this.pnlLogging = new System.Windows.Forms.Panel();
			this._txtFilename = new System.Windows.Forms.Label();
			this.lblTraceDesc = new System.Windows.Forms.Label();
			this._cboTraceLevel = new System.Windows.Forms.Label();
			this.pnlGeneral = new System.Windows.Forms.Panel();
			this.pnlAlpha = new System.Windows.Forms.Panel();
			this.lblAlpha = new System.Windows.Forms.Label();
			this.hslAlpha = new ElfControls.HSLSlider();
			this._pctPreview = new System.Windows.Forms.Label();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			this.pnlKeyboard = new System.Windows.Forms.Panel();
			this._txtSearch = new System.Windows.Forms.Label();
			this._txtConflicts = new System.Windows.Forms.Label();
			this._txtNewKey = new System.Windows.Forms.Label();
			this._txtExisting = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tmrAnimation = new System.Windows.Forms.Timer(this.components);
			this._lblAlpha = new System.Windows.Forms.Label();
			this.chkShowExcludedChannels = new System.Windows.Forms.CheckBox();
			this.pnlLogging.SuspendLayout();
			this.pnlGeneral.SuspendLayout();
			this.pnlAlpha.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.pnlKeyboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkDisableUndo
			// 
			this.chkDisableUndo.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkDisableUndo.Image = global::ElfCore.Properties.Resources.cancel;
			this.chkDisableUndo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkDisableUndo.Location = new System.Drawing.Point(0, 24);
			this.chkDisableUndo.MinimumSize = new System.Drawing.Size(120, 24);
			this.chkDisableUndo.Name = "chkDisableUndo";
			this.chkDisableUndo.Size = new System.Drawing.Size(403, 24);
			this.chkDisableUndo.TabIndex = 2;
			this.chkDisableUndo.Text = "Disable Undo/Redo";
			this.chkDisableUndo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this.chkDisableUndo, "Disable Undo/Redo");
			this.chkDisableUndo.UseVisualStyleBackColor = true;
			// 
			// chkShowRulers
			// 
			this.chkShowRulers.AutoSize = true;
			this.chkShowRulers.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkShowRulers.Image = global::ElfCore.Properties.Resources.ruler;
			this.chkShowRulers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkShowRulers.Location = new System.Drawing.Point(0, 0);
			this.chkShowRulers.MinimumSize = new System.Drawing.Size(190, 24);
			this.chkShowRulers.Name = "chkShowRulers";
			this.chkShowRulers.Size = new System.Drawing.Size(403, 24);
			this.chkShowRulers.TabIndex = 1;
			this.chkShowRulers.Text = "Show Rulers on Edit Canvas";
			this.chkShowRulers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this.chkShowRulers, "Show Rulers on Edit Canvas");
			this.chkShowRulers.UseVisualStyleBackColor = true;
			// 
			// chkNoLogPlayback
			// 
			this.chkNoLogPlayback.AutoSize = true;
			this.chkNoLogPlayback.Image = global::ElfCore.Properties.Resources.cancel;
			this.chkNoLogPlayback.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkNoLogPlayback.Location = new System.Drawing.Point(14, 138);
			this.chkNoLogPlayback.MinimumSize = new System.Drawing.Size(180, 32);
			this.chkNoLogPlayback.Name = "chkNoLogPlayback";
			this.chkNoLogPlayback.Size = new System.Drawing.Size(180, 32);
			this.chkNoLogPlayback.TabIndex = 13;
			this.chkNoLogPlayback.Text = "Don\'t log during Playback";
			this.chkNoLogPlayback.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.chkNoLogPlayback, "Check if you wish logging suspending during playback operations");
			this.chkNoLogPlayback.UseVisualStyleBackColor = true;
			// 
			// cmdLogFileBrowse
			// 
			this.cmdLogFileBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdLogFileBrowse.Image = global::ElfCore.Properties.Resources.open;
			this.cmdLogFileBrowse.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
			this.cmdLogFileBrowse.Location = new System.Drawing.Point(264, 109);
			this.cmdLogFileBrowse.Name = "cmdLogFileBrowse";
			this.cmdLogFileBrowse.Size = new System.Drawing.Size(22, 22);
			this.cmdLogFileBrowse.TabIndex = 12;
			this.toolTip1.SetToolTip(this.cmdLogFileBrowse, "Select new log filename");
			this.cmdLogFileBrowse.UseVisualStyleBackColor = true;
			this.cmdLogFileBrowse.Click += new System.EventHandler(this.cmdLogFileBrowse_Click);
			// 
			// txtLogFileName
			// 
			this.txtLogFileName.Location = new System.Drawing.Point(14, 109);
			this.txtLogFileName.Name = "txtLogFileName";
			this.txtLogFileName.Size = new System.Drawing.Size(244, 23);
			this.txtLogFileName.TabIndex = 11;
			this.txtLogFileName.Text = "ElfCore.log";
			this.toolTip1.SetToolTip(this.txtLogFileName, "Filename of the log file");
			// 
			// cboTraceLevel
			// 
			this.cboTraceLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTraceLevel.FormattingEnabled = true;
			this.cboTraceLevel.Items.AddRange(new object[] {
            "Off",
            "Error",
            "Warning",
            "Info",
            "Verbose"});
			this.cboTraceLevel.Location = new System.Drawing.Point(14, 25);
			this.cboTraceLevel.Name = "cboTraceLevel";
			this.cboTraceLevel.Size = new System.Drawing.Size(278, 23);
			this.cboTraceLevel.TabIndex = 8;
			this.toolTip1.SetToolTip(this.cboTraceLevel, "Change the tracing level, either turning it off or changing the amount of informa" +
        "tion being logged.");
			this.cboTraceLevel.SelectedIndexChanged += new System.EventHandler(this.cboTraceLevel_SelectedIndexChanged);
			// 
			// cmdLoadKeyboard
			// 
			this.cmdLoadKeyboard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdLoadKeyboard.Image = global::ElfCore.Properties.Resources.open;
			this.cmdLoadKeyboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdLoadKeyboard.Location = new System.Drawing.Point(154, 317);
			this.cmdLoadKeyboard.Name = "cmdLoadKeyboard";
			this.cmdLoadKeyboard.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdLoadKeyboard.Size = new System.Drawing.Size(113, 25);
			this.cmdLoadKeyboard.TabIndex = 29;
			this.cmdLoadKeyboard.Text = "&Load Config";
			this.cmdLoadKeyboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdLoadKeyboard, "Reset the program to the initial keyboard configuration");
			this.cmdLoadKeyboard.UseVisualStyleBackColor = true;
			this.cmdLoadKeyboard.Click += new System.EventHandler(this.cmdLoadKeyboard_Click);
			// 
			// cmdSaveKeyboard
			// 
			this.cmdSaveKeyboard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdSaveKeyboard.Image = global::ElfCore.Properties.Resources.save;
			this.cmdSaveKeyboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdSaveKeyboard.Location = new System.Drawing.Point(275, 317);
			this.cmdSaveKeyboard.Name = "cmdSaveKeyboard";
			this.cmdSaveKeyboard.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdSaveKeyboard.Size = new System.Drawing.Size(113, 25);
			this.cmdSaveKeyboard.TabIndex = 28;
			this.cmdSaveKeyboard.Text = "&Save Config";
			this.cmdSaveKeyboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdSaveKeyboard, "Reset the program to the initial keyboard configuration");
			this.cmdSaveKeyboard.UseVisualStyleBackColor = true;
			this.cmdSaveKeyboard.Click += new System.EventHandler(this.cmdSaveKeyboard_Click);
			// 
			// cmdDefault
			// 
			this.cmdDefault.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdDefault.Image = global::ElfCore.Properties.Resources.undo;
			this.cmdDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdDefault.Location = new System.Drawing.Point(8, 317);
			this.cmdDefault.Name = "cmdDefault";
			this.cmdDefault.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdDefault.Size = new System.Drawing.Size(137, 25);
			this.cmdDefault.TabIndex = 27;
			this.cmdDefault.Text = "&Reset To Default";
			this.cmdDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdDefault, "Reset the program to the initial keyboard configuration");
			this.cmdDefault.UseVisualStyleBackColor = true;
			this.cmdDefault.Click += new System.EventHandler(this.cmdDefault_Click);
			// 
			// cmdRemove
			// 
			this.cmdRemove.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdRemove.Image = global::ElfCore.Properties.Resources.keyboard;
			this.cmdRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdRemove.Location = new System.Drawing.Point(303, 197);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdRemove.Size = new System.Drawing.Size(85, 25);
			this.cmdRemove.TabIndex = 21;
			this.cmdRemove.Text = "Remo&ve";
			this.cmdRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdRemove, "Remove current shortcut assignment");
			this.cmdRemove.UseVisualStyleBackColor = true;
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSearch.Location = new System.Drawing.Point(8, 25);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(380, 23);
			this.txtSearch.TabIndex = 17;
			this.toolTip1.SetToolTip(this.txtSearch, "Search for command using all or part of this text");
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// cmdAssign
			// 
			this.cmdAssign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdAssign.Image = global::ElfCore.Properties.Resources.keyboard;
			this.cmdAssign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdAssign.Location = new System.Drawing.Point(303, 242);
			this.cmdAssign.Name = "cmdAssign";
			this.cmdAssign.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdAssign.Size = new System.Drawing.Size(85, 25);
			this.cmdAssign.TabIndex = 24;
			this.cmdAssign.Text = "&Assign";
			this.cmdAssign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdAssign, "Assign these keystrokes to the currently selected command");
			this.cmdAssign.UseVisualStyleBackColor = true;
			this.cmdAssign.Click += new System.EventHandler(this.cmdAssign_Click);
			// 
			// txtConflicts
			// 
			this.txtConflicts.BackColor = System.Drawing.SystemColors.Info;
			this.txtConflicts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtConflicts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConflicts.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtConflicts.Location = new System.Drawing.Point(8, 288);
			this.txtConflicts.Name = "txtConflicts";
			this.txtConflicts.ReadOnly = true;
			this.txtConflicts.Size = new System.Drawing.Size(380, 23);
			this.txtConflicts.TabIndex = 26;
			this.txtConflicts.TabStop = false;
			this.txtConflicts.Text = "Menu,File,Save";
			this.toolTip1.SetToolTip(this.txtConflicts, "Commands that conflict with all or the first part of the indicated set of keystro" +
        "kes");
			// 
			// txtNewKey
			// 
			this.txtNewKey.BackColor = System.Drawing.SystemColors.Window;
			this.txtNewKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNewKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNewKey.Location = new System.Drawing.Point(8, 244);
			this.txtNewKey.Name = "txtNewKey";
			this.txtNewKey.ReadOnly = true;
			this.txtNewKey.Size = new System.Drawing.Size(289, 23);
			this.txtNewKey.TabIndex = 23;
			this.toolTip1.SetToolTip(this.txtNewKey, "Enter the keystrokes for the new shortcut");
			this.txtNewKey.TextChanged += new System.EventHandler(this.txtNewKey_TextChanged);
			this.txtNewKey.Enter += new System.EventHandler(this.txtNewKey_Enter);
			this.txtNewKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewKey_KeyDown);
			this.txtNewKey.Leave += new System.EventHandler(this.txtNewKey_Leave);
			// 
			// txtExisting
			// 
			this.txtExisting.BackColor = System.Drawing.SystemColors.Info;
			this.txtExisting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtExisting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtExisting.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtExisting.Location = new System.Drawing.Point(8, 197);
			this.txtExisting.Name = "txtExisting";
			this.txtExisting.ReadOnly = true;
			this.txtExisting.Size = new System.Drawing.Size(289, 23);
			this.txtExisting.TabIndex = 20;
			this.txtExisting.TabStop = false;
			this.txtExisting.Text = "Ctrl+Alt+Shift+Backspace";
			this.toolTip1.SetToolTip(this.txtExisting, "Current shortcut for the selected command");
			this.txtExisting.TextChanged += new System.EventHandler(this.txtExisting_TextChanged);
			// 
			// lstCommands
			// 
			this.lstCommands.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstCommands.FormattingEnabled = true;
			this.lstCommands.ItemHeight = 15;
			this.lstCommands.Location = new System.Drawing.Point(8, 54);
			this.lstCommands.Name = "lstCommands";
			this.lstCommands.Size = new System.Drawing.Size(380, 124);
			this.lstCommands.TabIndex = 18;
			this.toolTip1.SetToolTip(this.lstCommands, "All (or part) commands available for keyboard shortcut assignments");
			this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
			// 
			// cmdOk
			// 
			this.cmdOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(235, 371);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 4;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(306, 371);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 5;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// lstGrouping
			// 
			this.lstGrouping.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstGrouping.FormattingEnabled = true;
			this.lstGrouping.IntegralHeight = false;
			this.lstGrouping.Location = new System.Drawing.Point(12, 12);
			this.lstGrouping.Name = "lstGrouping";
			this.lstGrouping.Size = new System.Drawing.Size(193, 352);
			this.lstGrouping.TabIndex = 12;
			this.lstGrouping.SelectedIndexChanged += new System.EventHandler(this.lstGrouping_SelectedIndexChanged);
			// 
			// pnlLogging
			// 
			this.pnlLogging.Controls.Add(this.chkNoLogPlayback);
			this.pnlLogging.Controls.Add(this.cmdLogFileBrowse);
			this.pnlLogging.Controls.Add(this.txtLogFileName);
			this.pnlLogging.Controls.Add(this._txtFilename);
			this.pnlLogging.Controls.Add(this.lblTraceDesc);
			this.pnlLogging.Controls.Add(this.cboTraceLevel);
			this.pnlLogging.Controls.Add(this._cboTraceLevel);
			this.pnlLogging.Location = new System.Drawing.Point(211, 12);
			this.pnlLogging.Name = "pnlLogging";
			this.pnlLogging.Size = new System.Drawing.Size(403, 352);
			this.pnlLogging.TabIndex = 13;
			// 
			// _txtFilename
			// 
			this._txtFilename.AutoSize = true;
			this._txtFilename.Location = new System.Drawing.Point(11, 91);
			this._txtFilename.Name = "_txtFilename";
			this._txtFilename.Size = new System.Drawing.Size(86, 15);
			this._txtFilename.TabIndex = 10;
			this._txtFilename.Text = "Log File Name:";
			// 
			// lblTraceDesc
			// 
			this.lblTraceDesc.Location = new System.Drawing.Point(11, 51);
			this.lblTraceDesc.Name = "lblTraceDesc";
			this.lblTraceDesc.Size = new System.Drawing.Size(282, 40);
			this.lblTraceDesc.TabIndex = 9;
			this.lblTraceDesc.Text = "Output informational messages, warnings, and error-handling messages.";
			// 
			// _cboTraceLevel
			// 
			this._cboTraceLevel.AutoSize = true;
			this._cboTraceLevel.Location = new System.Drawing.Point(11, 7);
			this._cboTraceLevel.Name = "_cboTraceLevel";
			this._cboTraceLevel.Size = new System.Drawing.Size(69, 15);
			this._cboTraceLevel.TabIndex = 7;
			this._cboTraceLevel.Text = "Trace Level:";
			// 
			// pnlGeneral
			// 
			this.pnlGeneral.Controls.Add(this.chkShowExcludedChannels);
			this.pnlGeneral.Controls.Add(this.chkDisableUndo);
			this.pnlGeneral.Controls.Add(this.chkShowRulers);
			this.pnlGeneral.Location = new System.Drawing.Point(211, 12);
			this.pnlGeneral.Name = "pnlGeneral";
			this.pnlGeneral.Size = new System.Drawing.Size(403, 352);
			this.pnlGeneral.TabIndex = 14;
			// 
			// pnlAlpha
			// 
			this.pnlAlpha.Controls.Add(this._lblAlpha);
			this.pnlAlpha.Controls.Add(this.lblAlpha);
			this.pnlAlpha.Controls.Add(this.hslAlpha);
			this.pnlAlpha.Controls.Add(this._pctPreview);
			this.pnlAlpha.Controls.Add(this.pctPreview);
			this.pnlAlpha.Location = new System.Drawing.Point(211, 12);
			this.pnlAlpha.Name = "pnlAlpha";
			this.pnlAlpha.Size = new System.Drawing.Size(403, 352);
			this.pnlAlpha.TabIndex = 15;
			// 
			// lblAlpha
			// 
			this.lblAlpha.AutoSize = true;
			this.lblAlpha.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblAlpha.Location = new System.Drawing.Point(186, 336);
			this.lblAlpha.Name = "lblAlpha";
			this.lblAlpha.Size = new System.Drawing.Size(31, 15);
			this.lblAlpha.TabIndex = 20;
			this.lblAlpha.Text = "50%";
			this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// hslAlpha
			// 
			this.hslAlpha.BackColor = System.Drawing.Color.Transparent;
			this.hslAlpha.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.hslAlpha.DrawStyle = ElfControls.HSLSlider.eDrawStyle.Transparency;
			hsl2.Alpha = 1D;
			hsl2.H = 0D;
			hsl2.Hue = 0D;
			hsl2.L = 0D;
			hsl2.Luminance = 0D;
			hsl2.S = 1D;
			hsl2.Saturation = 1D;
			this.hslAlpha.HSL = hsl2;
			this.hslAlpha.IndicatorMarks = ((System.Collections.Generic.List<double>)(resources.GetObject("hslAlpha.IndicatorMarks")));
			this.hslAlpha.Location = new System.Drawing.Point(8, 307);
			this.hslAlpha.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(48)))), ((int)(((byte)(18)))));
			this.hslAlpha.MaxValue = 255D;
			this.hslAlpha.Name = "hslAlpha";
			this.hslAlpha.ReverseFill = true;
			this.hslAlpha.Size = new System.Drawing.Size(385, 28);
			this.hslAlpha.TabIndex = 19;
			this.hslAlpha.Value = 128D;
			this.hslAlpha.Changed += new System.EventHandler(this.hslAlpha_Changed);
			// 
			// _pctPreview
			// 
			this._pctPreview.AutoSize = true;
			this._pctPreview.Location = new System.Drawing.Point(5, 7);
			this._pctPreview.Name = "_pctPreview";
			this._pctPreview.Size = new System.Drawing.Size(51, 15);
			this._pctPreview.TabIndex = 18;
			this._pctPreview.Text = "Preview:";
			// 
			// pctPreview
			// 
			this.pctPreview.BackColor = System.Drawing.Color.Black;
			this.pctPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pctPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pctPreview.Location = new System.Drawing.Point(8, 25);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Size = new System.Drawing.Size(385, 276);
			this.pctPreview.TabIndex = 21;
			this.pctPreview.TabStop = false;
			this.pctPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pctPreview_Paint);
			// 
			// pnlKeyboard
			// 
			this.pnlKeyboard.Controls.Add(this.cmdLoadKeyboard);
			this.pnlKeyboard.Controls.Add(this.cmdSaveKeyboard);
			this.pnlKeyboard.Controls.Add(this.cmdDefault);
			this.pnlKeyboard.Controls.Add(this.cmdRemove);
			this.pnlKeyboard.Controls.Add(this.txtSearch);
			this.pnlKeyboard.Controls.Add(this._txtSearch);
			this.pnlKeyboard.Controls.Add(this.cmdAssign);
			this.pnlKeyboard.Controls.Add(this.txtConflicts);
			this.pnlKeyboard.Controls.Add(this._txtConflicts);
			this.pnlKeyboard.Controls.Add(this.txtNewKey);
			this.pnlKeyboard.Controls.Add(this._txtNewKey);
			this.pnlKeyboard.Controls.Add(this.txtExisting);
			this.pnlKeyboard.Controls.Add(this._txtExisting);
			this.pnlKeyboard.Controls.Add(this.lstCommands);
			this.pnlKeyboard.Location = new System.Drawing.Point(211, 12);
			this.pnlKeyboard.Name = "pnlKeyboard";
			this.pnlKeyboard.Size = new System.Drawing.Size(403, 352);
			this.pnlKeyboard.TabIndex = 16;
			// 
			// _txtSearch
			// 
			this._txtSearch.AutoSize = true;
			this._txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtSearch.Location = new System.Drawing.Point(8, 7);
			this._txtSearch.Name = "_txtSearch";
			this._txtSearch.Size = new System.Drawing.Size(162, 15);
			this._txtSearch.TabIndex = 16;
			this._txtSearch.Text = "Show commands containing:";
			// 
			// _txtConflicts
			// 
			this._txtConflicts.AutoSize = true;
			this._txtConflicts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtConflicts.Location = new System.Drawing.Point(5, 270);
			this._txtConflicts.Name = "_txtConflicts";
			this._txtConflicts.Size = new System.Drawing.Size(85, 15);
			this._txtConflicts.TabIndex = 25;
			this._txtConflicts.Text = "Conflicts With:";
			// 
			// _txtNewKey
			// 
			this._txtNewKey.AutoSize = true;
			this._txtNewKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtNewKey.Location = new System.Drawing.Point(5, 226);
			this._txtNewKey.Name = "_txtNewKey";
			this._txtNewKey.Size = new System.Drawing.Size(110, 15);
			this._txtNewKey.TabIndex = 22;
			this._txtNewKey.Text = "Press shortcut keys:";
			// 
			// _txtExisting
			// 
			this._txtExisting.AutoSize = true;
			this._txtExisting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._txtExisting.Location = new System.Drawing.Point(5, 182);
			this._txtExisting.Name = "_txtExisting";
			this._txtExisting.Size = new System.Drawing.Size(182, 15);
			this._txtExisting.TabIndex = 19;
			this._txtExisting.Text = "Shortcuts for selected command:";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// tmrAnimation
			// 
			this.tmrAnimation.Enabled = true;
			this.tmrAnimation.Interval = 2000;
			this.tmrAnimation.Tick += new System.EventHandler(this.tmrAnimation_Tick);
			// 
			// _lblAlpha
			// 
			this._lblAlpha.AutoSize = true;
			this._lblAlpha.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lblAlpha.Location = new System.Drawing.Point(8, 336);
			this._lblAlpha.Name = "_lblAlpha";
			this._lblAlpha.Size = new System.Drawing.Size(172, 15);
			this._lblAlpha.TabIndex = 22;
			this._lblAlpha.Text = "Inactive Channel Transparency:";
			this._lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkShowExcludedChannels
			// 
			this.chkShowExcludedChannels.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkShowExcludedChannels.Image = global::ElfCore.Properties.Resources.channel;
			this.chkShowExcludedChannels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkShowExcludedChannels.Location = new System.Drawing.Point(0, 48);
			this.chkShowExcludedChannels.MinimumSize = new System.Drawing.Size(120, 24);
			this.chkShowExcludedChannels.Name = "chkShowExcludedChannels";
			this.chkShowExcludedChannels.Size = new System.Drawing.Size(403, 24);
			this.chkShowExcludedChannels.TabIndex = 3;
			this.chkShowExcludedChannels.Text = "Show Excluded Channels (PlugIn Mode)";
			this.chkShowExcludedChannels.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this.chkShowExcludedChannels, "Show Excluded Channels (PlugIn Mode)");
			this.chkShowExcludedChannels.UseVisualStyleBackColor = true;
			// 
			// EditUISettings
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(627, 410);
			this.Controls.Add(this.lstGrouping);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Controls.Add(this.pnlKeyboard);
			this.Controls.Add(this.pnlAlpha);
			this.Controls.Add(this.pnlGeneral);
			this.Controls.Add(this.pnlLogging);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditUISettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
			this.pnlLogging.ResumeLayout(false);
			this.pnlLogging.PerformLayout();
			this.pnlGeneral.ResumeLayout(false);
			this.pnlGeneral.PerformLayout();
			this.pnlAlpha.ResumeLayout(false);
			this.pnlAlpha.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.pnlKeyboard.ResumeLayout(false);
			this.pnlKeyboard.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private ElfControls.ImageListBox lstGrouping;
		private System.Windows.Forms.Panel pnlLogging;
		private System.Windows.Forms.CheckBox chkNoLogPlayback;
		private System.Windows.Forms.Button cmdLogFileBrowse;
		private System.Windows.Forms.TextBox txtLogFileName;
		private System.Windows.Forms.Label _txtFilename;
		private System.Windows.Forms.Label lblTraceDesc;
		private System.Windows.Forms.ComboBox cboTraceLevel;
		private System.Windows.Forms.Label _cboTraceLevel;
		private System.Windows.Forms.Panel pnlGeneral;
		private System.Windows.Forms.CheckBox chkDisableUndo;
		private System.Windows.Forms.CheckBox chkShowRulers;
		private System.Windows.Forms.Panel pnlAlpha;
		private System.Windows.Forms.Label lblAlpha;
		private ElfControls.HSLSlider hslAlpha;
		private System.Windows.Forms.Label _pctPreview;
		private System.Windows.Forms.PictureBox pctPreview;
		private System.Windows.Forms.Panel pnlKeyboard;
		private System.Windows.Forms.Button cmdLoadKeyboard;
		private System.Windows.Forms.Button cmdSaveKeyboard;
		private System.Windows.Forms.Button cmdDefault;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.Label _txtSearch;
		private System.Windows.Forms.Button cmdAssign;
		private System.Windows.Forms.TextBox txtConflicts;
		private System.Windows.Forms.Label _txtConflicts;
		private System.Windows.Forms.TextBox txtNewKey;
		private System.Windows.Forms.Label _txtNewKey;
		private System.Windows.Forms.TextBox txtExisting;
		private System.Windows.Forms.Label _txtExisting;
		private System.Windows.Forms.ListBox lstCommands;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Timer tmrAnimation;
		private System.Windows.Forms.Label _lblAlpha;
		private System.Windows.Forms.CheckBox chkShowExcludedChannels;
	}
}