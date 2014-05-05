namespace ElfControls
{
	partial class DropDownControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmrAnimate = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// tmrAnimate
			// 
			this.tmrAnimate.Interval = 10;
			this.tmrAnimate.Tick += new System.EventHandler(this.tmrAnimate_Tick);
			// 
			// DropDownControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Name = "DropDownControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer tmrAnimate;
	}
}
