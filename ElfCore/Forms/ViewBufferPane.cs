using ElfCore.Controllers;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore.Forms
{
	public partial class ViewBufferPane : Form
	{

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private bool _showUndoText = false;
		private Form _parent = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden()]
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		public Form AssignedParent
		{
			set { _parent = value; }
		}

		public bool ShowUndoData 
		{
			get { return _showUndoText; }
			set
			{
				_showUndoText = value;
				if (_showUndoText)
				{
					if (!tabsPanes.Controls.Contains(tabSnapshot))
						tabsPanes.Controls.Add(tabSnapshot);
					if (!tabsPanes.Controls.Contains(tabUndo))
						tabsPanes.Controls.Add(tabUndo);
					if (!tabsPanes.Controls.Contains(tabRedo))
						tabsPanes.Controls.Add(tabRedo);
				}
				else
				{
					if (tabsPanes.Controls.Contains(tabSnapshot))
						tabsPanes.Controls.Remove(tabSnapshot);
					if (tabsPanes.Controls.Contains(tabUndo))
						tabsPanes.Controls.Remove(tabUndo);
					if (tabsPanes.Controls.Contains(tabRedo))
						tabsPanes.Controls.Remove(tabRedo);
				}
				_parent.Focus();
			}
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public ViewBufferPane(Form parent)
		{
			InitializeComponent();
			_workshop.ProfileController.Switched += Profiles_Switched;
			SetPaneSize();
			tabsPanes.Controls.Clear();
			_parent = parent;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void SetBitmap(Bitmap bmp, Panes pane)
		{
			PictureBox Buffer = null;

			if (bmp == null)
				return;

			switch (pane)
			{
				case Panes.LatticeBuffer:
					//if (!tabsPanes.Controls.Contains(tabLatticeBuffer))
					//    tabsPanes.Controls.Add(tabLatticeBuffer);
					Buffer = pctLatticeBuffer;
					break;

				case Panes.MaskCanvas:
					//if (!tabsPanes.Controls.Contains(tabMaskCanvas))
					//    tabsPanes.Controls.Add(tabMaskCanvas);
					Buffer = pctMaskCanvas;
					break;

				case Panes.MaskLattice:
					//if (!tabsPanes.Controls.Contains(tabMaskLattice))
					//    tabsPanes.Controls.Add(tabMaskLattice);
					Buffer = pctMaskLattice;
					break;

				case Panes.CapturedCanvas:
					//if (!tabsPanes.Controls.Contains(tabCapturedCanvas))
					//    tabsPanes.Controls.Add(tabCapturedCanvas);
					Buffer = pctCapturedCanvas;
					break;

				case Panes.ActiveChannel:
					//if (!tabsPanes.Controls.Contains(tabActiveChannel))
					//    tabsPanes.Controls.Add(tabActiveChannel);
					Buffer = pctActiveChannel;
					break;

				case Panes.ImageStamp:
					//if (!tabsPanes.Controls.Contains(tabImageStamp))
					//    tabsPanes.Controls.Add(tabImageStamp);
					Buffer = pctImageStamp;
					break;

				case Panes.MoveChannel:
					//if (!tabsPanes.Controls.Contains(tabMoveChannel))
					//    tabsPanes.Controls.Add(tabMoveChannel);
					Buffer = pctMove;
					break;

				case Panes.Clipboard:
					//if (!tabsPanes.Controls.Contains(tabClipboard))
					//    tabsPanes.Controls.Add(tabClipboard);
					Buffer = pctClipboard;
					break;
			}

			Bitmap Temp = new Bitmap(bmp.Width, bmp.Height);
			using (Graphics g = Graphics.FromImage(Temp))
			{
				g.Clear(Buffer.BackColor);
				g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
			}
			Buffer.Image = Temp;
			Buffer = null;
			SetTab(pane);
		}

		public void SetText(string text, Panes pane)
		{
			if (!ShowUndoData)
				return;

			switch (pane)
			{
				case Panes.Undo:
					//if (!tabsPanes.Controls.Contains(tabUndo))
					//    tabsPanes.Controls.Add(tabUndo);
					txtUndo.Text = text;
					txtUndo.SelectionLength = 0;
					break;

				case Panes.Redo:
					//if (!tabsPanes.Controls.Contains(tabRedo))
					//    tabsPanes.Controls.Add(tabRedo);
					txtRedo.Text = text;
					txtRedo.SelectionLength = 0;
					break;

				case Panes.Snapshot:
					//if (!tabsPanes.Controls.Contains(tabSnapshot))
					//    tabsPanes.Controls.Add(tabSnapshot);
					txtSnapshot.Text = text;
					txtSnapshot.SelectionLength = 0;
					break;
			}
			SetTab(pane);
		}

		/// <summary>
		/// Determines from the enum which tab to display. If that tab is not currently loaded, then loads it into the tabsPane control.
		/// </summary>
		/// <param name="pane"></param>
		public void SetTab(Panes pane)
		{
			TabPage ActiveTab = null;

			switch (pane)
			{
				case Panes.LatticeBuffer:
					ActiveTab = tabLatticeBuffer;
					break;

				case Panes.MaskCanvas:
					ActiveTab = tabMaskCanvas;
					break;

				case Panes.MaskLattice:
					ActiveTab = tabMaskLattice;
					break;

				case Panes.CapturedCanvas:
					ActiveTab = tabCapturedCanvas;
					break;

				case Panes.ActiveChannel:
					ActiveTab = tabActiveChannel;
					break;

				case Panes.ImageStamp:
					ActiveTab = tabImageStamp;
					break;

				case Panes.MoveChannel:
					ActiveTab = tabMoveChannel;
					break;

				case Panes.Clipboard:
					ActiveTab = tabClipboard;
					break;
				
				case Panes.Undo:
					ActiveTab = tabUndo;
					break;

				case Panes.Redo:
					ActiveTab = tabRedo;
					break;

				case Panes.Snapshot:
					ActiveTab = tabSnapshot;
					break;
			}

			if (!tabsPanes.Controls.Contains(ActiveTab))
				tabsPanes.Controls.Add(ActiveTab);

			int Index = tabsPanes.Controls.IndexOf(ActiveTab);
			tabsPanes.SelectedIndex = Index;
			if (_parent != null)
				_parent.Focus();
		}

		public void SetPaneSize()
		{
			if (Profile == null)
				return;

			Size LatticeSize = Profile.Scaling.LatticeSize;
			Size CanvasSize = Profile.Scaling.CanvasSize;

			pctCapturedCanvas.Size = LatticeSize;
			pctMaskLattice.Size = LatticeSize;
			pctMaskCanvas.Size = CanvasSize;
			pctCapturedCanvas.Size = CanvasSize;
			pctActiveChannel.Size = CanvasSize;
			pctImageStamp.Size = CanvasSize;
			pctClipboard.Size = CanvasSize;
			pctMove.Size = CanvasSize;
		}

		#endregion [ Methods ]

		#region [ Event Delegates ]

		private void Profiles_Switched(object sender, ProfileEventArgs e)
		{
			SetPaneSize();

			if (e.OldProfile != null)
			{
				e.OldProfile.Undo_Changed -= Undo_Changed;
				e.OldProfile.Redo_Changed -= Redo_Changed;
				SetText(string.Empty, Panes.Snapshot);
				SetText(string.Empty, Panes.Undo);
				SetText(string.Empty, Panes.Redo);
			}
			if (e.Profile != null)
			{
				SetPaneSize();
				e.Profile.Undo_Changed += Undo_Changed;
				e.Profile.Redo_Changed += Redo_Changed;
				SetText(e.Profile.Debug_UndoSnapshot(), Panes.Snapshot);
				SetText(e.Profile.Debug_UndoStack(), Panes.Undo);
				SetText(e.Profile.Debug_RedoStack(), Panes.Redo);
			}
		}

		private void Undo_Changed(object sender, UndoEventArgs e)
		{
			SetText(_workshop.Profile.Debug_UndoStack(), Panes.Undo);
			SetText(_workshop.Profile.Debug_UndoSnapshot(), Panes.Snapshot);
		}

		private void Redo_Changed(object sender, UndoEventArgs e)
		{
			SetText(_workshop.Profile.Debug_RedoStack(), Panes.Redo);
			SetText(_workshop.Profile.Debug_UndoSnapshot(), Panes.Snapshot);
		}

		private void ViewBufferPane_Shown(object sender, EventArgs e)
		{
			Visible = false;
		}

		#endregion [ Event Delegates ]
		
	}
}
