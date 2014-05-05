namespace Tools
{
	partial class ToolStripContainer
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
			this.TestTool1_ToolStrip = new System.Windows.Forms.ToolStrip();
			this.Shape_ToolName = new System.Windows.Forms.ToolStripLabel();
			this._cboLineSize = new System.Windows.Forms.ToolStripLabel();
			this.cboLineSize = new System.Windows.Forms.ToolStripComboBox();
			this.TestTool1_ToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// TestTool1_ToolStrip
			// 
			this.TestTool1_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Shape_ToolName,
            this._cboLineSize,
            this.cboLineSize});
			this.TestTool1_ToolStrip.Location = new System.Drawing.Point(0, 0);
			this.TestTool1_ToolStrip.Name = "TestTool1_ToolStrip";
			this.TestTool1_ToolStrip.Size = new System.Drawing.Size(459, 25);
			this.TestTool1_ToolStrip.TabIndex = 0;
			this.TestTool1_ToolStrip.Text = "toolStrip1";
			// 
			// Shape_ToolName
			// 
			this.Shape_ToolName.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Shape_ToolName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.Shape_ToolName.Name = "Shape_ToolName";
			this.Shape_ToolName.Size = new System.Drawing.Size(53, 22);
			this.Shape_ToolName.Text = "TestLine";
			// 
			// _cboLineSize
			// 
			this._cboLineSize.Name = "_cboLineSize";
			this._cboLineSize.Size = new System.Drawing.Size(87, 22);
			this._cboLineSize.Text = "Line Thickness:";
			// 
			// cboLineSize
			// 
			this.cboLineSize.AutoSize = false;
			this.cboLineSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLineSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
			this.cboLineSize.Name = "cboLineSize";
			this.cboLineSize.Size = new System.Drawing.Size(40, 23);
			this.cboLineSize.ToolTipText = "Thickness of the line (in cells)";
			// 
			// ToolStripContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(459, 136);
			this.ControlBox = false;
			this.Controls.Add(this.TestTool1_ToolStrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ToolStripContainer";
			this.ShowInTaskbar = false;
			this.TestTool1_ToolStrip.ResumeLayout(false);
			this.TestTool1_ToolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripLabel Shape_ToolName;
		private System.Windows.Forms.ToolStripLabel _cboLineSize;
		internal System.Windows.Forms.ToolStrip TestTool1_ToolStrip;
		internal System.Windows.Forms.ToolStripComboBox cboLineSize;
	}
}