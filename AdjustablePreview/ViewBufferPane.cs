using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore
{
	public partial class ViewBufferPane : Form
	{

		#region [ Private Variables ]

		//private string _userDirectory = string.Empty;
		//private Workshop _workshop = null;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public ViewBufferPane()
		{
			InitializeComponent();
			//_userDirectory = Environment.GetEnvironmentVariable("userprofile") + @"\";
			SetPaneSize();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void SetBitmap(Bitmap bmp, Panes pane)
		{
			if (bmp == null)
				return;
			Bitmap Preview = (Bitmap)bmp.Clone();
			switch (pane)
			{
				case Panes.LatticeBuffer:
					pctLatticeBuffer.Image = bmp;
					pctLatticeBuffer.Refresh();
					break;

				case Panes.MaskCanvas:
					pctMaskCanvas.Image = Preview;
					pctMaskCanvas.Refresh();
					break;

				case Panes.MaskLattice:
					pctMaskLattice.Image = Preview;
					pctMaskLattice.Refresh();
					break;

				case Panes.Canvas:
					pctCapturedCanvas.Image = Preview;
					pctCapturedCanvas.Refresh();
					break;

				case Panes.ActiveChannel:
					pctActiveChannel.Image = Preview;
					pctActiveChannel.Refresh();
					break;

				case Panes.ImageStamp:
					pctImageStamp.Image = Preview;
					pctImageStamp.Refresh();
					break;

				case Panes.MoveChannel:
					pctMove.Image = Preview;
					pctMove.Refresh();
					break;

				case Panes.ClipboardChannel:
					pctClipboard.Image = Preview;
					pctClipboard.Refresh();
					break;
			}
			Preview = null;
			SetTab(pane);
		}

		public void SetTab(Panes pane)
		{
			int tabIndex = (int)pane;

			if (tabIndex >= tabsPanes.TabPages.Count)
				return;

			tabsPanes.SelectedIndex = tabIndex;
		}

		public void SetPaneSize()
		{
			pctCapturedCanvas.Size = UISettings.ʃLatticeSize;
			pctMaskLattice.Size = UISettings.ʃLatticeSize;

			pctMaskCanvas.Size = UISettings.ʃCanvasSize;
			pctCapturedCanvas.Size = UISettings.ʃCanvasSize;
			pctActiveChannel.Size = UISettings.ʃCanvasSize;
			pctImageStamp.Size = UISettings.ʃCanvasSize;
			pctClipboard.Size = UISettings.ʃCanvasSize;
			pctMove.Size = UISettings.ʃCanvasSize;
		}

		#endregion [ Methods ]
		
	}
}
