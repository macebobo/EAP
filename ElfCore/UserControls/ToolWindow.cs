using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System;

namespace ElfCore.Util
{
	public partial class ToolWindow : DockContent
	{
		public ToolWindow()
		{
			InitializeComponent();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Activate();
			base.OnMouseDown(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//var brush = Focused ? Brushes.Green : Brushes.Blue;
			//e.Graphics.FillRectangle(brush, ClientRectangle);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			Invalidate();
			base.OnGotFocus(e);
		}

		protected override void OnDeactivate(EventArgs e)
		{
			Invalidate();
			base.OnDeactivate(e);
		}

		protected override void OnActivated(EventArgs e)
		{
			Invalidate();
			base.OnActivated(e);
		}
	}
}
