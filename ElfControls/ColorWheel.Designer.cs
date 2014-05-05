namespace ElfControls
{
	public partial class ColorWheel
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
			if (_torusRegion != null)
			{
				_torusRegion.Dispose();
				_torusRegion = null;
			}
			if (_triangleRegion != null)
			{
				_triangleRegion.Dispose();
				_triangleRegion = null;
			}
			if (_diskRegion != null)
			{
				_diskRegion.Dispose();
				_diskRegion = null;
			}
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region [ Component Designer generated code ]

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ColorWheel
			// 
			this.DoubleBuffered = true;
			this.Name = "ColorWheel";
			this.ResumeLayout(false);
		}

		#endregion [ Component Designer generated code ]
	}
}
