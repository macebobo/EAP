namespace ElfCore.Forms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFromProfile));
			this.pnlControls = new System.Windows.Forms.Panel();
			this.cboTargetChannel = new ElfControls.ImageDropDown();
			this.cmdCancelEdits = new System.Windows.Forms.Button();
			this.cmdRight = new System.Windows.Forms.Button();
			this.cmdLeft = new System.Windows.Forms.Button();
			this.cmdDown = new System.Windows.Forms.Button();
			this.cmdUp = new System.Windows.Forms.Button();
			this.lblCanvasSize = new System.Windows.Forms.Label();
			this._lblCanvasSize = new System.Windows.Forms.Label();
			this.chkClear = new System.Windows.Forms.CheckBox();
			this.cmdMap = new System.Windows.Forms.Button();
			this.chkOverrideName = new System.Windows.Forms.CheckBox();
			this.lblDataSize = new System.Windows.Forms.Label();
			this._lblDataSize = new System.Windows.Forms.Label();
			this.chkResize = new System.Windows.Forms.CheckBox();
			this.txtY = new System.Windows.Forms.TextBox();
			this.lblY = new System.Windows.Forms.Label();
			this.txtX = new System.Windows.Forms.TextBox();
			this.lblX = new System.Windows.Forms.Label();
			this.lblOffset = new System.Windows.Forms.Label();
			this._cboTargetChannels = new System.Windows.Forms.Label();
			this.lstImportedChannels = new ElfControls.ImageListBox();
			this._lstImportedChannels = new System.Windows.Forms.Label();
			this.lblProfile = new System.Windows.Forms.Label();
			this._lblProfile = new System.Windows.Forms.Label();
			this.chkOverrideColor = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdEdit = new System.Windows.Forms.Button();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.lstMapping = new ElfControls.ImageListBox();
			this.tmrMove = new System.Windows.Forms.Timer(this.components);
			this.tmrHold = new System.Windows.Forms.Timer(this.components);
			this.pnlMapping = new System.Windows.Forms.Panel();
			this._lstMapping = new System.Windows.Forms.Label();
			this.cmdOk = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.chkPreview = new System.Windows.Forms.CheckBox();
			this.pnlControls.SuspendLayout();
			this.pnlMapping.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlControls
			// 
			this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlControls.Controls.Add(this.cboTargetChannel);
			this.pnlControls.Controls.Add(this.cmdCancelEdits);
			this.pnlControls.Controls.Add(this.cmdRight);
			this.pnlControls.Controls.Add(this.cmdLeft);
			this.pnlControls.Controls.Add(this.cmdDown);
			this.pnlControls.Controls.Add(this.cmdUp);
			this.pnlControls.Controls.Add(this.lblCanvasSize);
			this.pnlControls.Controls.Add(this._lblCanvasSize);
			this.pnlControls.Controls.Add(this.chkClear);
			this.pnlControls.Controls.Add(this.cmdMap);
			this.pnlControls.Controls.Add(this.chkOverrideName);
			this.pnlControls.Controls.Add(this.lblDataSize);
			this.pnlControls.Controls.Add(this._lblDataSize);
			this.pnlControls.Controls.Add(this.chkResize);
			this.pnlControls.Controls.Add(this.txtY);
			this.pnlControls.Controls.Add(this.lblY);
			this.pnlControls.Controls.Add(this.txtX);
			this.pnlControls.Controls.Add(this.lblX);
			this.pnlControls.Controls.Add(this.lblOffset);
			this.pnlControls.Controls.Add(this._cboTargetChannels);
			this.pnlControls.Controls.Add(this.lstImportedChannels);
			this.pnlControls.Controls.Add(this._lstImportedChannels);
			this.pnlControls.Controls.Add(this.lblProfile);
			this.pnlControls.Controls.Add(this._lblProfile);
			this.pnlControls.Controls.Add(this.chkOverrideColor);
			this.pnlControls.Location = new System.Drawing.Point(9, 241);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(584, 330);
			this.pnlControls.TabIndex = 4;
			// 
			// cboTargetChannel
			// 
			this.cboTargetChannel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboTargetChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTargetChannel.DropDownWidth = 279;
			this.cboTargetChannel.Location = new System.Drawing.Point(279, 49);
			this.cboTargetChannel.Name = "cboTargetChannel";
			this.cboTargetChannel.Size = new System.Drawing.Size(275, 24);
			this.cboTargetChannel.TabIndex = 5;
			this.cboTargetChannel.SelectedIndexChanged += new System.EventHandler(this.cboTargetChannel_SelectedIndexChanged);
			// 
			// cmdCancelEdits
			// 
			this.cmdCancelEdits.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdCancelEdits.Image = global::ElfCore.Properties.Resources.map;
			this.cmdCancelEdits.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancelEdits.Location = new System.Drawing.Point(391, 284);
			this.cmdCancelEdits.Name = "cmdCancelEdits";
			this.cmdCancelEdits.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdCancelEdits.Size = new System.Drawing.Size(102, 28);
			this.cmdCancelEdits.TabIndex = 24;
			this.cmdCancelEdits.Text = "Cancel Edits";
			this.cmdCancelEdits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancelEdits.UseVisualStyleBackColor = true;
			this.cmdCancelEdits.Click += new System.EventHandler(this.cmdCancelEdits_Click);
			// 
			// cmdRight
			// 
			this.cmdRight.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdRight.Image = global::ElfCore.Properties.Resources.right_small;
			this.cmdRight.Location = new System.Drawing.Point(457, 93);
			this.cmdRight.Name = "cmdRight";
			this.cmdRight.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.cmdRight.Size = new System.Drawing.Size(18, 18);
			this.cmdRight.TabIndex = 13;
			this.cmdRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdRight, "Offset the imported channel right");
			this.cmdRight.UseVisualStyleBackColor = true;
			this.cmdRight.Click += new System.EventHandler(this.cmdRight_Click);
			this.cmdRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DirectionButton_MouseDown);
			this.cmdRight.MouseLeave += new System.EventHandler(this.DirectionButton_MouseLeave);
			// 
			// cmdLeft
			// 
			this.cmdLeft.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdLeft.Image = global::ElfCore.Properties.Resources.left_small;
			this.cmdLeft.Location = new System.Drawing.Point(417, 93);
			this.cmdLeft.Name = "cmdLeft";
			this.cmdLeft.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.cmdLeft.Size = new System.Drawing.Size(18, 18);
			this.cmdLeft.TabIndex = 11;
			this.cmdLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdLeft, "Offset the imported channel left");
			this.cmdLeft.UseVisualStyleBackColor = true;
			this.cmdLeft.Click += new System.EventHandler(this.cmdLeft_Click);
			this.cmdLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DirectionButton_MouseDown);
			this.cmdLeft.MouseLeave += new System.EventHandler(this.DirectionButton_MouseLeave);
			// 
			// cmdDown
			// 
			this.cmdDown.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdDown.Image = global::ElfCore.Properties.Resources.down_small;
			this.cmdDown.Location = new System.Drawing.Point(437, 103);
			this.cmdDown.Name = "cmdDown";
			this.cmdDown.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
			this.cmdDown.Size = new System.Drawing.Size(19, 18);
			this.cmdDown.TabIndex = 14;
			this.cmdDown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdDown, "Offset the imported channel down");
			this.cmdDown.UseVisualStyleBackColor = true;
			this.cmdDown.Click += new System.EventHandler(this.cmdDown_Click);
			this.cmdDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DirectionButton_MouseDown);
			this.cmdDown.MouseLeave += new System.EventHandler(this.DirectionButton_MouseLeave);
			// 
			// cmdUp
			// 
			this.cmdUp.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdUp.Image = global::ElfCore.Properties.Resources.up_small;
			this.cmdUp.Location = new System.Drawing.Point(437, 84);
			this.cmdUp.Name = "cmdUp";
			this.cmdUp.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
			this.cmdUp.Size = new System.Drawing.Size(19, 18);
			this.cmdUp.TabIndex = 12;
			this.cmdUp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdUp, "Offset the imported channel up");
			this.cmdUp.UseVisualStyleBackColor = true;
			this.cmdUp.Click += new System.EventHandler(this.cmdUp_Click);
			this.cmdUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DirectionButton_MouseDown);
			this.cmdUp.MouseLeave += new System.EventHandler(this.DirectionButton_MouseLeave);
			// 
			// lblCanvasSize
			// 
			this.lblCanvasSize.AutoSize = true;
			this.lblCanvasSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCanvasSize.Location = new System.Drawing.Point(380, 183);
			this.lblCanvasSize.Name = "lblCanvasSize";
			this.lblCanvasSize.Size = new System.Drawing.Size(48, 15);
			this.lblCanvasSize.TabIndex = 19;
			this.lblCanvasSize.Text = "64 x 32";
			// 
			// _lblCanvasSize
			// 
			this._lblCanvasSize.AutoSize = true;
			this._lblCanvasSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lblCanvasSize.Location = new System.Drawing.Point(276, 183);
			this._lblCanvasSize.Name = "_lblCanvasSize";
			this._lblCanvasSize.Size = new System.Drawing.Size(98, 15);
			this._lblCanvasSize.TabIndex = 18;
			this._lblCanvasSize.Text = "New Canvas Size:";
			// 
			// chkClear
			// 
			this.chkClear.AutoSize = true;
			this.chkClear.Location = new System.Drawing.Point(279, 259);
			this.chkClear.Name = "chkClear";
			this.chkClear.Size = new System.Drawing.Size(275, 19);
			this.chkClear.TabIndex = 22;
			this.chkClear.Text = "Remove the existing Cells of the target Channel";
			this.toolTip1.SetToolTip(this.chkClear, "Remove the existing Cells of the target Channel");
			this.chkClear.UseVisualStyleBackColor = true;
			this.chkClear.CheckedChanged += new System.EventHandler(this.chkClear_CheckedChanged);
			// 
			// cmdMap
			// 
			this.cmdMap.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdMap.Image = global::ElfCore.Properties.Resources.map;
			this.cmdMap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdMap.Location = new System.Drawing.Point(271, 284);
			this.cmdMap.Name = "cmdMap";
			this.cmdMap.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdMap.Size = new System.Drawing.Size(114, 28);
			this.cmdMap.TabIndex = 23;
			this.cmdMap.Text = "&Save Mapping";
			this.cmdMap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdMap.UseVisualStyleBackColor = true;
			this.cmdMap.Click += new System.EventHandler(this.cmdMap_Click);
			// 
			// chkOverrideName
			// 
			this.chkOverrideName.AutoSize = true;
			this.chkOverrideName.Location = new System.Drawing.Point(279, 209);
			this.chkOverrideName.Name = "chkOverrideName";
			this.chkOverrideName.Size = new System.Drawing.Size(303, 19);
			this.chkOverrideName.TabIndex = 20;
			this.chkOverrideName.Text = "Override current name with imported Channel name";
			this.toolTip1.SetToolTip(this.chkOverrideName, "Change the channel name to match that of the imported channel.");
			this.chkOverrideName.UseVisualStyleBackColor = true;
			this.chkOverrideName.CheckedChanged += new System.EventHandler(this.chkOverrideName_CheckedChanged);
			// 
			// lblDataSize
			// 
			this.lblDataSize.AutoSize = true;
			this.lblDataSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDataSize.Location = new System.Drawing.Point(434, 134);
			this.lblDataSize.Name = "lblDataSize";
			this.lblDataSize.Size = new System.Drawing.Size(62, 15);
			this.lblDataSize.TabIndex = 16;
			this.lblDataSize.Text = "200 x 300";
			// 
			// _lblDataSize
			// 
			this._lblDataSize.AutoSize = true;
			this._lblDataSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lblDataSize.Location = new System.Drawing.Point(276, 134);
			this._lblDataSize.Name = "_lblDataSize";
			this._lblDataSize.Size = new System.Drawing.Size(156, 15);
			this._lblDataSize.TabIndex = 15;
			this._lblDataSize.Text = "Imported Channel Data Size:";
			// 
			// chkResize
			// 
			this.chkResize.AutoSize = true;
			this.chkResize.Location = new System.Drawing.Point(279, 157);
			this.chkResize.Name = "chkResize";
			this.chkResize.Size = new System.Drawing.Size(200, 19);
			this.chkResize.TabIndex = 17;
			this.chkResize.Text = "Resize canvas to fit mapped Cells";
			this.toolTip1.SetToolTip(this.chkResize, "Expand the Canvas Size to accomodate imported cells. If unchecked, cells outside " +
        "of the Canvas will be truncated");
			this.chkResize.UseVisualStyleBackColor = true;
			this.chkResize.CheckedChanged += new System.EventHandler(this.chkResize_CheckedChanged);
			// 
			// txtY
			// 
			this.txtY.Location = new System.Drawing.Point(369, 103);
			this.txtY.Name = "txtY";
			this.txtY.Size = new System.Drawing.Size(36, 23);
			this.txtY.TabIndex = 10;
			this.txtY.Text = "0";
			this.txtY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtY, "Number of Cells the imported channel will be shift from the top");
			this.txtY.TextChanged += new System.EventHandler(this.txtY_TextChanged);
			this.txtY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SignedNumberOnly_KeyPress);
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblY.Location = new System.Drawing.Point(346, 106);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(17, 15);
			this.lblY.TabIndex = 9;
			this.lblY.Text = "Y:";
			// 
			// txtX
			// 
			this.txtX.Location = new System.Drawing.Point(299, 103);
			this.txtX.Name = "txtX";
			this.txtX.Size = new System.Drawing.Size(36, 23);
			this.txtX.TabIndex = 8;
			this.txtX.Text = "0";
			this.txtX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.txtX, "Number of Cells the imported channel will be shift from the left side");
			this.txtX.TextChanged += new System.EventHandler(this.txtX_TextChanged);
			this.txtX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SignedNumberOnly_KeyPress);
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblX.Location = new System.Drawing.Point(276, 106);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(17, 15);
			this.lblX.TabIndex = 7;
			this.lblX.Text = "X:";
			// 
			// lblOffset
			// 
			this.lblOffset.AutoSize = true;
			this.lblOffset.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOffset.Location = new System.Drawing.Point(276, 84);
			this.lblOffset.Name = "lblOffset";
			this.lblOffset.Size = new System.Drawing.Size(109, 15);
			this.lblOffset.TabIndex = 6;
			this.lblOffset.Text = "Offset Mapping By:";
			// 
			// _cboTargetChannels
			// 
			this._cboTargetChannels.AutoSize = true;
			this._cboTargetChannels.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._cboTargetChannels.Location = new System.Drawing.Point(276, 31);
			this._cboTargetChannels.Name = "_cboTargetChannels";
			this._cboTargetChannels.Size = new System.Drawing.Size(152, 15);
			this._cboTargetChannels.TabIndex = 4;
			this._cboTargetChannels.Text = "Channels in Current Profile:";
			// 
			// lstImportedChannels
			// 
			this.lstImportedChannels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstImportedChannels.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstImportedChannels.FormattingEnabled = true;
			this.lstImportedChannels.IntegralHeight = false;
			this.lstImportedChannels.Location = new System.Drawing.Point(10, 49);
			this.lstImportedChannels.Name = "lstImportedChannels";
			this.lstImportedChannels.Size = new System.Drawing.Size(250, 263);
			this.lstImportedChannels.TabIndex = 3;
			this.toolTip1.SetToolTip(this.lstImportedChannels, "List of Channels in the imported Profile");
			this.lstImportedChannels.SelectedIndexChanged += new System.EventHandler(this.lstImportedChannels_SelectedIndexChanged);
			// 
			// _lstImportedChannels
			// 
			this._lstImportedChannels.AutoSize = true;
			this._lstImportedChannels.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lstImportedChannels.Location = new System.Drawing.Point(7, 31);
			this._lstImportedChannels.Name = "_lstImportedChannels";
			this._lstImportedChannels.Size = new System.Drawing.Size(161, 15);
			this._lstImportedChannels.TabIndex = 2;
			this._lstImportedChannels.Text = "Channels in Imported Profile:";
			// 
			// lblProfile
			// 
			this.lblProfile.AutoSize = true;
			this.lblProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProfile.Location = new System.Drawing.Point(109, 12);
			this.lblProfile.Name = "lblProfile";
			this.lblProfile.Size = new System.Drawing.Size(154, 15);
			this.lblProfile.TabIndex = 1;
			this.lblProfile.Text = "IMPORTED PROFILE NAME";
			// 
			// _lblProfile
			// 
			this._lblProfile.AutoSize = true;
			this._lblProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lblProfile.Location = new System.Drawing.Point(7, 12);
			this._lblProfile.Name = "_lblProfile";
			this._lblProfile.Size = new System.Drawing.Size(96, 15);
			this._lblProfile.TabIndex = 0;
			this._lblProfile.Text = "Imported Profile:";
			// 
			// chkOverrideColor
			// 
			this.chkOverrideColor.AutoSize = true;
			this.chkOverrideColor.Location = new System.Drawing.Point(279, 234);
			this.chkOverrideColor.Name = "chkOverrideColor";
			this.chkOverrideColor.Size = new System.Drawing.Size(297, 19);
			this.chkOverrideColor.TabIndex = 21;
			this.chkOverrideColor.Text = "Override current color with imported Channel color";
			this.toolTip1.SetToolTip(this.chkOverrideColor, "Change the channel color to match that of the imported channel.");
			this.chkOverrideColor.UseVisualStyleBackColor = true;
			this.chkOverrideColor.CheckedChanged += new System.EventHandler(this.chkOverrideColor_CheckedChanged);
			// 
			// toolTip1
			// 
			this.toolTip1.Active = false;
			this.toolTip1.AutomaticDelay = 2000;
			this.toolTip1.IsBalloon = true;
			// 
			// cmdAdd
			// 
			this.cmdAdd.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdAdd.Image = global::ElfCore.Properties.Resources.map;
			this.cmdAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdAdd.Location = new System.Drawing.Point(9, 208);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdAdd.Size = new System.Drawing.Size(114, 28);
			this.cmdAdd.TabIndex = 0;
			this.cmdAdd.Text = "&Add Mapping";
			this.cmdAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdAdd, "Add new Channel mapping");
			this.cmdAdd.UseVisualStyleBackColor = true;
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdEdit
			// 
			this.cmdEdit.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdEdit.Image = global::ElfCore.Properties.Resources.map;
			this.cmdEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdEdit.Location = new System.Drawing.Point(129, 208);
			this.cmdEdit.Name = "cmdEdit";
			this.cmdEdit.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdEdit.Size = new System.Drawing.Size(114, 28);
			this.cmdEdit.TabIndex = 1;
			this.cmdEdit.Text = "&Edit Mapping";
			this.cmdEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdEdit, "Edit selected Channel mapping");
			this.cmdEdit.UseVisualStyleBackColor = true;
			this.cmdEdit.Click += new System.EventHandler(this.cmdEdit_Click);
			// 
			// cmdDelete
			// 
			this.cmdDelete.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.cmdDelete.Image = global::ElfCore.Properties.Resources.map;
			this.cmdDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdDelete.Location = new System.Drawing.Point(249, 208);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.cmdDelete.Size = new System.Drawing.Size(124, 28);
			this.cmdDelete.TabIndex = 2;
			this.cmdDelete.Text = "&Delete Mapping";
			this.cmdDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.cmdDelete, "Delete selected Channel mapping");
			this.cmdDelete.UseVisualStyleBackColor = true;
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// lstMapping
			// 
			this.lstMapping.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstMapping.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lstMapping.FormattingEnabled = true;
			this.lstMapping.Location = new System.Drawing.Point(3, 18);
			this.lstMapping.Name = "lstMapping";
			this.lstMapping.Size = new System.Drawing.Size(584, 180);
			this.lstMapping.TabIndex = 1;
			this.toolTip1.SetToolTip(this.lstMapping, "Current mapping");
			this.lstMapping.SelectedValueChanged += new System.EventHandler(this.lstMapping_SelectedIndexChanged);
			this.lstMapping.DoubleClick += new System.EventHandler(this.lstMapping_DoubleClick);
			// 
			// tmrMove
			// 
			this.tmrMove.Tick += new System.EventHandler(this.tmrMove_Tick);
			// 
			// tmrHold
			// 
			this.tmrHold.Interval = 500;
			this.tmrHold.Tick += new System.EventHandler(this.tmrHold_Tick);
			// 
			// pnlMapping
			// 
			this.pnlMapping.Controls.Add(this.lstMapping);
			this.pnlMapping.Controls.Add(this._lstMapping);
			this.pnlMapping.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlMapping.Location = new System.Drawing.Point(6, 6);
			this.pnlMapping.Name = "pnlMapping";
			this.pnlMapping.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.pnlMapping.Size = new System.Drawing.Size(594, 204);
			this.pnlMapping.TabIndex = 0;
			// 
			// _lstMapping
			// 
			this._lstMapping.AutoSize = true;
			this._lstMapping.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lstMapping.Location = new System.Drawing.Point(0, 0);
			this._lstMapping.Name = "_lstMapping";
			this._lstMapping.Size = new System.Drawing.Size(58, 15);
			this._lstMapping.TabIndex = 0;
			this._lstMapping.Text = "Mapping:";
			// 
			// cmdOk
			// 
			this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOk.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdOk.Image = global::ElfCore.Properties.Resources.check;
			this.cmdOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdOk.Location = new System.Drawing.Point(225, 577);
			this.cmdOk.Name = "cmdOk";
			this.cmdOk.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdOk.Size = new System.Drawing.Size(65, 28);
			this.cmdOk.TabIndex = 5;
			this.cmdOk.Text = "&OK";
			this.cmdOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdOk.UseVisualStyleBackColor = true;
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Image = global::ElfCore.Properties.Resources.cancel;
			this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdCancel.Location = new System.Drawing.Point(296, 577);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.cmdCancel.Size = new System.Drawing.Size(85, 28);
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// chkPreview
			// 
			this.chkPreview.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkPreview.Checked = true;
			this.chkPreview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkPreview.Image = global::ElfCore.Properties.Resources.visible;
			this.chkPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkPreview.Location = new System.Drawing.Point(512, 208);
			this.chkPreview.Name = "chkPreview";
			this.chkPreview.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.chkPreview.Size = new System.Drawing.Size(80, 28);
			this.chkPreview.TabIndex = 3;
			this.chkPreview.Text = "Preview";
			this.chkPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkPreview.UseVisualStyleBackColor = true;
			this.chkPreview.CheckedChanged += new System.EventHandler(this.chkPreview_CheckedChanged);
			// 
			// ImportFromProfile
			// 
			this.AcceptButton = this.cmdOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(606, 612);
			this.Controls.Add(this.chkPreview);
			this.Controls.Add(this.cmdDelete);
			this.Controls.Add(this.cmdEdit);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this.pnlMapping);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(200, 200);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportFromProfile";
			this.Padding = new System.Windows.Forms.Padding(6);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Import From Another Profile";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
			this.Move += new System.EventHandler(this.Form_Move);
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			this.pnlMapping.ResumeLayout(false);
			this.pnlMapping.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.Button cmdMap;
		private System.Windows.Forms.CheckBox chkOverrideName;
		private System.Windows.Forms.Label lblDataSize;
		private System.Windows.Forms.Label _lblDataSize;
		private System.Windows.Forms.CheckBox chkResize;
		private System.Windows.Forms.TextBox txtY;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.TextBox txtX;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblOffset;
		private System.Windows.Forms.Label _cboTargetChannels;
		private ElfControls.ImageListBox lstImportedChannels;
		private System.Windows.Forms.Label _lstImportedChannels;
		private System.Windows.Forms.Label lblProfile;
		private System.Windows.Forms.Label _lblProfile;
		private System.Windows.Forms.CheckBox chkClear;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblCanvasSize;
		private System.Windows.Forms.Label _lblCanvasSize;
		private System.Windows.Forms.Button cmdUp;
		private System.Windows.Forms.Button cmdRight;
		private System.Windows.Forms.Button cmdLeft;
		private System.Windows.Forms.Button cmdDown;
		private System.Windows.Forms.Timer tmrMove;
		private System.Windows.Forms.Timer tmrHold;
		private System.Windows.Forms.Panel pnlMapping;
		private ElfControls.ImageListBox lstMapping;
		private System.Windows.Forms.Label _lstMapping;
		private System.Windows.Forms.Button cmdCancelEdits;
		private System.Windows.Forms.CheckBox chkOverrideColor;
		private ElfControls.ImageDropDown cboTargetChannel;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.CheckBox chkPreview;
		private System.Windows.Forms.Button cmdDelete;
		private System.Windows.Forms.Button cmdEdit;
		private System.Windows.Forms.Button cmdAdd;

	}
}