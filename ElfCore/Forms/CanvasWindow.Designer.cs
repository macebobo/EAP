namespace ElfCore.Forms
{
	partial class CanvasWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CanvasWindow));
			this.MarqueeAnimationTimer = new System.Windows.Forms.Timer(this.components);
			this.RulerHorz = new System.Windows.Forms.PictureBox();
			this.RulerVert = new System.Windows.Forms.PictureBox();
			this.pnlWorkspace = new System.Windows.Forms.Panel();
			this.pnlEasel = new System.Windows.Forms.Panel();
			this.CanvasPane = new System.Windows.Forms.PictureBox();
			this.lstKeyPreview = new System.Windows.Forms.ListBox();
			this.lblData = new System.Windows.Forms.Label();
			this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.RulerHorz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.RulerVert)).BeginInit();
			this.pnlWorkspace.SuspendLayout();
			this.pnlEasel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).BeginInit();
			this.tblLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// MarqueeAnimationTimer
			// 
			this.MarqueeAnimationTimer.Tick += new System.EventHandler(this.MarqueeAnimationTimer_Tick);
			// 
			// RulerHorz
			// 
			this.RulerHorz.BackColor = System.Drawing.SystemColors.Control;
			this.RulerHorz.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RulerHorz.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RulerHorz.Location = new System.Drawing.Point(35, 0);
			this.RulerHorz.Margin = new System.Windows.Forms.Padding(0);
			this.RulerHorz.Name = "RulerHorz";
			this.RulerHorz.Size = new System.Drawing.Size(825, 25);
			this.RulerHorz.TabIndex = 1;
			this.RulerHorz.TabStop = false;
			this.RulerHorz.Paint += new System.Windows.Forms.PaintEventHandler(this.RulerHorz_Paint);
			this.RulerHorz.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
			// 
			// RulerVert
			// 
			this.RulerVert.BackColor = System.Drawing.SystemColors.Control;
			this.RulerVert.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RulerVert.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RulerVert.Location = new System.Drawing.Point(0, 25);
			this.RulerVert.Margin = new System.Windows.Forms.Padding(0);
			this.RulerVert.Name = "RulerVert";
			this.RulerVert.Size = new System.Drawing.Size(35, 504);
			this.RulerVert.TabIndex = 2;
			this.RulerVert.TabStop = false;
			this.RulerVert.Paint += new System.Windows.Forms.PaintEventHandler(this.RulerVert_Paint);
			this.RulerVert.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
			// 
			// pnlWorkspace
			// 
			this.pnlWorkspace.AutoScroll = true;
			this.pnlWorkspace.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.pnlWorkspace.Controls.Add(this.pnlEasel);
			this.pnlWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlWorkspace.Location = new System.Drawing.Point(38, 28);
			this.pnlWorkspace.Name = "pnlWorkspace";
			this.pnlWorkspace.Size = new System.Drawing.Size(819, 498);
			this.pnlWorkspace.TabIndex = 6;
			this.pnlWorkspace.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlWorkspace_Scroll);
			// 
			// pnlEasel
			// 
			this.pnlEasel.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.pnlEasel.Controls.Add(this.CanvasPane);
			this.pnlEasel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlEasel.Location = new System.Drawing.Point(0, 0);
			this.pnlEasel.Margin = new System.Windows.Forms.Padding(0);
			this.pnlEasel.Name = "pnlEasel";
			this.pnlEasel.Size = new System.Drawing.Size(819, 498);
			this.pnlEasel.TabIndex = 7;
			this.pnlEasel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
			// 
			// CanvasPane
			// 
			this.CanvasPane.BackColor = System.Drawing.Color.Black;
			this.CanvasPane.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CanvasPane.Location = new System.Drawing.Point(158, 86);
			this.CanvasPane.Name = "CanvasPane";
			this.CanvasPane.Size = new System.Drawing.Size(488, 288);
			this.CanvasPane.TabIndex = 7;
			this.CanvasPane.TabStop = false;
			this.toolTip1.SetToolTip(this.CanvasPane, "No Channel");
			this.CanvasPane.Click += new System.EventHandler(this.CanvasPane_Click);
			this.CanvasPane.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPane_Paint);
			this.CanvasPane.DoubleClick += new System.EventHandler(this.CanvasPane_DoubleClick);
			this.CanvasPane.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CanvasPane_MouseDown);
			this.CanvasPane.MouseEnter += new System.EventHandler(this.CanvasPane_MouseEnter);
			this.CanvasPane.MouseLeave += new System.EventHandler(this.CanvasPane_MouseLeave);
			this.CanvasPane.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CanvasPane_MouseMove);
			this.CanvasPane.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CanvasPane_MouseUp);
			// 
			// lstKeyPreview
			// 
			this.lstKeyPreview.BackColor = System.Drawing.Color.Red;
			this.lstKeyPreview.FormattingEnabled = true;
			this.lstKeyPreview.ItemHeight = 15;
			this.lstKeyPreview.Location = new System.Drawing.Point(566, 300);
			this.lstKeyPreview.Name = "lstKeyPreview";
			this.lstKeyPreview.Size = new System.Drawing.Size(120, 94);
			this.lstKeyPreview.TabIndex = 9;
			// 
			// lblData
			// 
			this.lblData.BackColor = System.Drawing.Color.Black;
			this.lblData.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblData.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblData.ForeColor = System.Drawing.Color.White;
			this.lblData.Location = new System.Drawing.Point(0, 0);
			this.lblData.Name = "lblData";
			this.lblData.Size = new System.Drawing.Size(860, 13);
			this.lblData.TabIndex = 9;
			// 
			// tblLayout
			// 
			this.tblLayout.AutoSize = true;
			this.tblLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tblLayout.BackColor = System.Drawing.SystemColors.Control;
			this.tblLayout.ColumnCount = 2;
			this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblLayout.Controls.Add(this.RulerVert, 0, 1);
			this.tblLayout.Controls.Add(this.pnlWorkspace, 1, 1);
			this.tblLayout.Controls.Add(this.RulerHorz, 1, 0);
			this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblLayout.Location = new System.Drawing.Point(0, 13);
			this.tblLayout.Margin = new System.Windows.Forms.Padding(0);
			this.tblLayout.Name = "tblLayout";
			this.tblLayout.RowCount = 2;
			this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tblLayout.Size = new System.Drawing.Size(860, 516);
			this.tblLayout.TabIndex = 9;
			this.tblLayout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
			// 
			// toolTip1
			// 
			this.toolTip1.Active = false;
			this.toolTip1.AutomaticDelay = 2000;
			this.toolTip1.IsBalloon = true;
			// 
			// CanvasWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ClientSize = new System.Drawing.Size(860, 529);
			this.Controls.Add(this.tblLayout);
			this.Controls.Add(this.lblData);
			this.Controls.Add(this.lstKeyPreview);
			this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "CanvasWindow";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.ShowInTaskbar = false;
			this.TabText = "Profile";
			this.Text = "Profile";
			this.Activated += new System.EventHandler(this.Form_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
			this.Shown += new System.EventHandler(this.Form_Shown);
			this.SizeChanged += new System.EventHandler(this.Form_SizeChanged);
			this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.RulerHorz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.RulerVert)).EndInit();
			this.pnlWorkspace.ResumeLayout(false);
			this.pnlEasel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.CanvasPane)).EndInit();
			this.tblLayout.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer MarqueeAnimationTimer;
		private System.Windows.Forms.PictureBox RulerHorz;
		private System.Windows.Forms.PictureBox RulerVert;
		private System.Windows.Forms.Panel pnlWorkspace;
		private System.Windows.Forms.Panel pnlEasel;
		public System.Windows.Forms.PictureBox CanvasPane;
		private System.Windows.Forms.TableLayoutPanel tblLayout;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblData;
		private System.Windows.Forms.ListBox lstKeyPreview;
	}
}