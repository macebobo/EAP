using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ElfCore.PlugIn;
using WeifenLuo.WinFormsUI.Docking;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore
{
	public partial class Editor : Form
	{
		#region [ Private Variables ]

		/// <summary>
		/// Workshop is a Singleton type of class. Simply getting the Instance variable from the Static object will get the object already loaded with our data
		/// </summary>
		private Workshop _workshop = Workshop.Instance;
		private Settings _settings = Settings.Instance;

		private ChannelExplorerTree _channelExplorer = null;
		private CanvasWindow _canvasWindow = null;
		private XmlHelper _xmlHelper = null;
		private ListBoxUtil _listboxUtil = new ListBoxUtil();
		private KeyChord _currentChord;
		private SortedList<string, ToolStrip> _subMenus = null;
		private ToolStrip _flyoutToolStrip = null;

		//private Settings _settings;
		//private static Stack _loading = new Stack();

		#endregion [ Private Variables ]
		
		#region [ Public Static Variables ]

		/// <summary>
		/// Debugging viewing panes. Not visible during runtime when not debugging.
		/// </summary>
		public static ViewBufferPane BufferPaneForm = null;

		/// <summary>
		/// Indicates whether the data is being initally loaded, to prevent the program from reacting to events that get fired when
		/// values are populated, possibly triggering endless loops
		/// </summary>
		public static bool Loading = false;

		/// <summary>
		/// Stopwatch, used for timing various methods, to determine efficiencies
		/// </summary>
		public static Stopwatch Stopwatch = new global::ElfCore.Stopwatch();

		#endregion [ Public Static Variables ]

		#region [ Properties ]

		private bool ShowGridLines
		{
			get { return (_workshop.UI.GridLineWidth != 0); }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Editor()
		{
			//Preview.WriteTraceMessage("Editor Constructor START", TraceLevel.Verbose);

			InitializeComponent();
			Editor.Loading = true;

			ConfigureRunMode(true);

			_xmlHelper = new XmlHelper();

			_currentChord = new KeyChord();
			_currentChord.Clear();
			
			//_workshop.Changed += new Workshop.DataEventHandler(Workshop_Changed);
			_workshop.DirtyChanged += new EventHandler(Workshop_DirtyChanged);

			_workshop.Channels.ChannelColorChanged += new EventHandler(ChannelController_ColorChanged);
			_workshop.Channels.ChannelNameChanged += new EventHandler(ChannelController_NameChanged);
			_workshop.Channels.ChannelSelected += new EventHandler(ChannelController_Selected);
			_workshop.Channels.ChannelVisibilityChanged += new EventHandler(ChannelController_VisibilityChanged);

			_workshop.UI.CellSizeChanged += new EventHandler(UI_CellSizeChanged);
			_workshop.UI.DisplayGridLines += new EventHandler(UI_DisplayGridLines);
			_workshop.UI.DisplayRuler += new EventHandler(UI_DisplayRuler);
			_workshop.UI.LatticeSizeChanged += new EventHandler(UI_LatticeSizeChanged);
			_workshop.UI.MousePoint += new EventHandler(UI_MousePoint);
			_workshop.UI.Zooming += new EventHandler(UI_Zooming);

			_workshop.Mask.Defined += new EventHandler(Mask_Defined);
			_workshop.Mask.Cleared += new EventHandler(Mask_Cleared);

			_workshop.UndoController.UndoChanged += new UndoController.UndoEventHandler(Undo_Changed);
			_workshop.UndoController.RedoChanged += new UndoController.UndoEventHandler(Redo_Changed);

			_workshop.Clipboard.Changed += new EventHandler(Clipboard_Changed);

			_channelExplorer = new ChannelExplorerTree();
			//_channelExplorer.ChannelExplorerResized += new System.EventHandler(this.ChannelExplorer_Resized);

			_canvasWindow = new CanvasWindow();
			_canvasWindow.MouseEnter += new EventHandler(this.CanvasWindow_MouseEnter);
			_canvasWindow.MouseLeave += new EventHandler(this.CanvasWindow_MouseLeave);
			_canvasWindow.Workshop = _workshop;

			#if DEBUG
				Editor.BufferPaneForm = new ViewBufferPane();
				Editor.BufferPaneForm.Show();
				SetPaneMenuTags();
				PanesMenu.Visible = true;
			#else
				Editor.BufferPaneForm = null;
				PanesMenu.Visible = false;
			#endif
			
			InitializeToolSubMenus();
			LoadPlugInTools();

			// Load in the remembered last settings
			LoadSettings();
			
			_channelExplorer.Show(DockPanel);
			_canvasWindow.Show(DockPanel);

			Editor.Loading = false;

			// Loading is completed, clear out any Undos that might have been accidentally created
			_workshop.UndoController.Clear();

			// Grab the inital snap shot of the data.
			_workshop.UndoController.GetInitialSnapshot();
			
			UpdateChannelStatus();

			//Preview.WriteTraceMessage("Editor Constructor END", TraceLevel.Verbose);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#region [ ToolBox Methods ]

		/// <summary>
		/// Configures the form and the DockPanel to either run as a Single Document Interface (PlugIn mode), or as a 
		/// Multiple Document Interface (stand-alone mode)
		/// </summary>
		/// <param name="mdi"></param>
		private void ConfigureRunMode(bool mdi)
		{
			if (mdi)
			{
				this.IsMdiContainer = true;
				DockPanel.DocumentStyle = DocumentStyle.DockingMdi;
			}
			else
			{
				this.IsMdiContainer = false;
				DockPanel.DocumentStyle = DocumentStyle.DockingSdi;
			}
		}

		private void InitializeToolSubMenus()
		{
			_subMenus = new SortedList<string, ToolStrip>();
		}

		/// <summary>
		/// Combines a tool image with the flyout image.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private Bitmap Flyout(Bitmap source)
		{
			if (source == null)
				return null;
			Bitmap FlyoutImage = ElfRes.flyout;
			if (FlyoutImage != null)
			{
				Graphics g = Graphics.FromImage(source);
				g.DrawImage(FlyoutImage, new Rectangle(0, 0, 16, 16));
				g.Dispose();
			}
			return source;
		}
		
		/// <summary>
		/// Load up all the Tools derived from PlugIns
		/// </summary>
		private void LoadPlugInTools()
		{
			PlugInToolStripButton PlugInButton = null;
			SortedList<string, ToolHost> ToolGroups = new SortedList<string, ToolHost>();
			//ToolHost ToolGroup = null;

			_workshop.Tools = PlugIn.ToolHostProvider.ToolHosts;

			if (_workshop.Tools == null)
				return;

			// First, loop through the list to find all the Tools that live on the main
			// toolbox itself and add them. If any are ToolGroups, then add them to the
			// ToolGroups list.
			foreach (PlugIn.ToolHost toolHost in _workshop.Tools)
			{
				if (!toolHost.IsToolGroup)
					toolHost.Tool.OperationCompleted += new EventHandler(this.Tool_OperationCompleted);

				if (!toolHost.InToolGroup)
				{
					PlugInButton = toolHost.Button;
					PlugInButton.Click += new System.EventHandler(this.Tool_Click);

					toolHost.Initialize();

					// If this tool has a ToolStrip that holds settings for it, add that control to the hidden panel on this form.
					if (toolHost.SettingsToolStrip != null)
						pnlHoldToolStrips.Controls.Add(toolHost.SettingsToolStrip);

					toolHost.ToolBox = ToolBox_Main;
					ToolBox_Main.Items.Add(PlugInButton);

					if (toolHost.IsToolGroup)
					{
						ToolGroups.Add(toolHost.Name, toolHost);
						this.Controls.Add(toolHost.ChildToolBox);
					}

					toolHost.Added = true;
				}
			}

			// Next, loop through the list to find all the Tools that are members of
			// a ToolGroup. Find the correct ToolGroup from the list we assembled earlier,
			// and add it as a child tool. If that ToolGroup does not yet have a child ToolBox
			// created, create one and add the tool's button to that control.
			foreach (PlugIn.ToolHost childToolHost in _workshop.Tools)
			{
				if (childToolHost.Added)
					continue;

				PlugInButton = childToolHost.Button;
				childToolHost.Initialize();

				// If this tool has a ToolStrip that holds settings for it, add that control to the hidden panel on this form.
				if (childToolHost.SettingsToolStrip != null)
					pnlHoldToolStrips.Controls.Add(childToolHost.SettingsToolStrip);

				// If the name of the ToolGroup that this belongs to doesn't exist in the list we have already assembled, then
				// fallback to adding it to the main ToolBox.
				if (!ToolGroups.ContainsKey(childToolHost.Tool.ToolGroupName))
				{ 
					// just add to the main tool box
					childToolHost.ToolBox = ToolBox_Main;
					PlugInButton.Click += new System.EventHandler(this.Tool_Click);
					ToolBox_Main.Items.Add(PlugInButton);
					childToolHost.Added = true;
					continue;
				}

				// Grab the ToolGroup this tool belongs to and add it 
				ToolHost ToolGroupHost = ToolGroups[childToolHost.Tool.ToolGroupName];
				ToolGroupHost.Add(childToolHost);
			}
		}

		/// <summary>
		/// Sort the Tools in their respective ToolStrips based on their ID number
		/// </summary>
		private void SortToolsInToolsBox(ToolStrip toolBox)
		{
			SortedList<int, PlugInToolStripButton> Tools = new SortedList<int, PlugInToolStripButton>();
			foreach (PlugInToolStripButton Button in toolBox.Items)
			{
				Tools.Add(Button.ToolHost.ID, Button);
			}
			toolBox.Items.Clear();
			foreach (KeyValuePair<int, PlugInToolStripButton> KVP in Tools)
			{
				toolBox.Items.Add(KVP.Value);
			}
		}

		#endregion [ ToolBox Methods ]

		private RectangleF GetChannelGroupBound(List<Channel> ChannelList)
		{
			Rectangle ChanRect;
			float minX, minY, maxX, maxY;
			minX = minY = Int32.MaxValue;
			maxX = maxY = Int32.MinValue;

			foreach (Channel Channel in ChannelList)
			{
				ChanRect = Channel.GetBounds();
				if (ChanRect.X < minX)
					minX = ChanRect.X;
				if (ChanRect.Y < minY)
					minY = ChanRect.Y;
				if (ChanRect.X + ChanRect.Width > maxX)
					maxX = ChanRect.X + ChanRect.Width;
				if (ChanRect.Y + ChanRect.Height > maxY)
					maxY = ChanRect.Y + ChanRect.Height;
			}
			return new RectangleF(minX, minY, maxX - minX, maxY - minY);
		}
		
		private Bitmap GetMenuCheckedImage(bool? isChecked)
		{
			if (isChecked ?? false)
				return ElfRes.option_checked;
			else
				return ElfRes.option;
		}

		private Bitmap LoadBitmap(out string filename)
		{
			filename = string.Empty;
			if (OpenImageFileDialog.ShowDialog() != DialogResult.OK)
				return null;

			FileStream fs = new FileStream(OpenImageFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] fileBytes = new byte[fs.Length];
			fs.Read(fileBytes, 0, (int)fs.Length);
			fs.Close();
			fs.Dispose();
			MemoryStream stream = new MemoryStream(fileBytes);
			Bitmap bmp = new Bitmap(stream);
			stream.Close();
			stream.Dispose();

			filename = OpenImageFileDialog.FileName;
			return bmp;
		}

		private void LoadSettings()
		{
			//Preview.WriteTraceMessage("Editor LoadSettings START", TraceLevel.Verbose);

			this.Left = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, this.Left);
			this.Top = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, this.Top);
			this.Width = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, this.Width);
			this.Height = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
			this.WindowState = EnumHelper.GetEnumFromValue<FormWindowState>(_settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_STATE, (int)FormWindowState.Normal));

			//int SelectedToolID = _settings.GetValue(Constants.SELECTED_TOOL, (int)Tool.Paint);

			//_workshop.CurrentPlugInTool = _workshop.GetPlugInTool(SelectedToolID);
			//Tool_Click(_workshop.CurrentPlugInTool.Button, null);

			//if (Workshop.IsValidTool(ToolVal))
			//    _workshop.CurrentTool = ToolVal;
			//else
			//    _workshop.CurrentTool = (int)Tool.Paint;

			//_workshop.ShapeTool = EnumHelper.GetEnumFromValue<Tool>(_settings.GetValue(Constants.SELECTED_SHAPETOOL, (int)Tool.Rectangle));
			//_workshop.MultiChannelTool = EnumHelper.GetEnumFromValue<Tool>(_settings.GetValue(Constants.SELECTED_MULTIChannel, (int)Tool.MultiChannelLine));
			//_workshop.MaskTool = EnumHelper.GetEnumFromValue<Tool>(_settings.GetValue(Constants.SELECTED_MASK, (int)Tool.Mask_Rectangle));

			// Menu only settings
			//_workshop.UI.RespectChannelOutputsDuringPlayback = _settings.GetValue(Constants.RESPECT, true);
			//_workshop.UI.ShowGridLineWhilePainting = _settings.GetValue(Constants.SHOW_GRIDLINES_WHILEPAINTING, true);
			//_workshop.UI.ShowRuler = _settings.GetValue(Constants.SHOW_RULERS, true);

			//Preview.WriteTraceMessage("Editor LoadSettings END", TraceLevel.Verbose);
		}

		private void MoveTheChannel(Channel Channel, Keys motion, RectangleF bounds, bool usingTheMoveChannel, bool moveMask)
		{
			PointF Offset = PointF.Empty;
			PointF MaskOffset = PointF.Empty;
			float Amount = 1f;
			if (Control.ModifierKeys == Keys.Shift)
				Amount *= 5;

			switch (motion)
			{
				case Keys.Up:
					Offset = new PointF(0, -Amount);
					break;
				case Keys.Down:
					Offset = new PointF(0, Amount);
					break;
				case Keys.Left:
					Offset = new PointF(-Amount, 0);
					break;
				case Keys.Right:
					Offset = new PointF(Amount, 0);
					break;

			}
			MaskOffset = new PointF(Offset.X * UISettings.ʃCellScale, Offset.Y * UISettings.ʃCellScale);

			if (Offset != Point.Empty)
			{
				Channel.MoveCells(Offset);
				if (moveMask)
					_workshop.MoveMask(MaskOffset);
				//if (!usingTheMoveChannel)
				//    _workshop.Channels.CellsChanged();
			}
		}

		/// <summary>
		/// Informs the various objects (including this form) to save their settings to the Settings document
		/// </summary>
		private void SaveSettings()
		{
			//Preview.WriteTraceMessage("Editor.SaveSettings START", TraceLevel.Verbose);

			if (this.WindowState != FormWindowState.Minimized)
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_STATE, (int)this.WindowState);
			else if (this.WindowState == FormWindowState.Normal)
			{
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, this.Left);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, this.Top);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, this.Width);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, this.Height);
			}

			// UI Settings
			//_settings.SetValue(Constants.ORIGINAL_UI, _workshop.UI.UseOriginalUI);
			_settings.SetValue(Constants.SHOW_GRIDLINES_WHILEPAINTING, _workshop.UI.ShowGridLineWhilePainting);
			_settings.SetValue(Constants.SHOW_RULERS, _workshop.UI.ShowRuler);

			//_settings.SetValue(Constants.SELECTED_TOOL, (int)_workshop.CurrentTool);
			//_settings.SetValue(Constants.SELECTED_SHAPETOOL, (int)_workshop.ShapeTool);
			//_settings.SetValue(Constants.SELECTED_MULTIChannel, (int)_workshop.MultiChannelTool);
			//_settings.SetValue(Constants.SELECTED_MASK, (int)_workshop.MaskTool);

			//_workshop.ToolSettings.SaveSettings(_settings);

			if (_workshop.Tools != null)
				foreach (PlugIn.ToolHost PTool in _workshop.Tools)
					PTool.Tool.SaveSettings();

			_settings.Save();
			//Preview.WriteTraceMessage("Editor.SaveSettings END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Sets the active Channel to the one indicated
		/// </summary>
		/// <param name="index">Index of the Channel that is to be the active one</param>
		private void SetActiveChannel(int index)
		{
			bool CurrentlyLoading = Editor.Loading;

			if (!CurrentlyLoading)
				Editor.Loading = true;
			try
			{
				_workshop.Channels.Active = _workshop.Channels.Sorted[index];
				//_workshop.ActiveChannelIndex = index;
				string ChannelName = _workshop.Channels.Active.ToString();

				if (_workshop.Channels.Active != null)
					tssChannel.Text = ChannelName;
				else
					tssChannel.Text = "None Selected";

				UpdateChannelStatus();
			}
			finally
			{
				if (!CurrentlyLoading)
					Editor.Loading = false;
			}
		}

		private void SetPaneMenuTags()
		{
			Pane_ActiveChannel.Tag = Panes.ActiveChannel;
			Pane_Canvas.Tag = Panes.Canvas;
			Pane_Clipboard.Tag = Panes.ClipboardChannel;
			Pane_ImageStamp.Tag = Panes.ImageStamp;
			Pane_MaskPixels.Tag = Panes.MaskCanvas;
			Pane_MaskCells.Tag = Panes.MaskLattice;
			Pane_MoveChannel.Tag = Panes.MoveChannel;
			Pane_Paint.Tag = Panes.LatticeBuffer;
		}

		/// <summary>
		/// Tint all the menu images so that they conform with the color set in the current Skin
		/// </summary>
		private void TintMenuImages(ToolStripItemCollection menuList)
		{
			foreach (ToolStripMenuItem Menu in menuList)
			{
				if (Menu.Image != null)
				{ 					
					// Find this image in the resources to get the name and the original image, check to see if it isn't already tinted.
					// If not, then tint the image and add it to the TintedImages dictionary



				}
				// Tint all the child menu items.
				if (Menu.DropDownItems.Count > 0)
					TintMenuImages(Menu.DropDownItems);
			}

		}

		/// <summary>
		/// Updates the status bar for the active channel by selecting the swatch image, and writing the name of the Channel into 
		/// the correct ToolStripStatusLabel control at the bottom of the form.
		/// </summary>
		private void UpdateChannelStatus()
		{
			tssChannel.Text = _workshop.Channels.Active.ToString();
			tssChannel.Image = ImageController.ColorSwatches[_workshop.Channels.Active.ColorSwatchKey];
		}

		#endregion [ Methods ]

		#region [ Static Methods ]

		public static void ExposePane(Bitmap bmp, Panes pane)
		{
			#if DEBUG
				BufferPaneForm.SetBitmap(bmp, pane);
			#endif
		}

		public static void ExposePane(Channel Channel, Panes pane)
		{
			#if DEBUG
				BufferPaneForm.SetBitmap(Channel.LatticeBuffer, pane);
			#endif
		}

		#endregion [ Static Methods ]

		#region [ Events ]

		#region [ CanvasWindow Events ]

		private void CanvasWindow_MouseEnter(object sender, EventArgs e)
		{
			tssMouseCoords.Visible = true;
		}

		private void CanvasWindow_MouseLeave(object sender, EventArgs e)
		{
			tssMouseCoords.Visible = false;
		}

		#endregion [ CanvasWindow Events ]

		#region [ ChannelController Events ]

		/// <summary>
		/// Event that fires from the ChannelController when the Color on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_ColorChanged(object sender, EventArgs e)
		{
			UpdateChannelStatus();
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Name on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_NameChanged(object sender, EventArgs e)
		{
			UpdateChannelStatus();
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Selected flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_Selected(object sender, EventArgs e)
		{
			UpdateChannelStatus();			
		}

		/// <summary>
		/// Event that fires from the ChannelController when the Visibility flag on one or more Channel has changed.
		/// </summary>
		/// <param name="sender">List of Channel affected</param>
		private void ChannelController_VisibilityChanged(object sender, EventArgs e)
		{ }
		
		#endregion [ ChannelController Events ]
				
		#region [ Workshop ]

		/// <summary>
		/// Either the ChannelControler or the UISettings are reporting their Dirty property as have changed.
		/// </summary>
		private void Workshop_DirtyChanged(object sender, EventArgs e)
		{
			if (Editor.Loading)
				return;

			if (_workshop.Dirty)
			{
				if (!this.Text.EndsWith("*"))
					this.Text += "*";
			}
			else
			{
				if (this.Text.EndsWith("*"))
					this.Text = this.Text.Substring(0, this.Text.Length - 1);
			}
		}

		#endregion [ Workshop ]

		#region [ UI Events ]

		/// <summary>
		/// Occurs when the Workshop.UI.CellSize property changes.
		/// </summary>
		private void UI_CellSizeChanged(object sender, EventArgs e)
		{
			int CellSize = _workshop.UI.CellSize;
			ToolStripMenuItem PixelMenu = null;
			for (int i = 0; i < 10; i++)
			{
				PixelMenu = (ToolStripMenuItem)GridMenu.DropDownItems[i];
				PixelMenu.Checked = (PixelMenu.Tag.ToString() == CellSize.ToString());
			}
			tssCellSize.Text = string.Format("{0}x{0} Pixel{1}", CellSize, (CellSize == 1) ? "" : "s");

			//// Inform the Channel Controller to have the channels rebuild their GraphicsPaths next time they need to redraw.
			//_workshop.Channels.UIChanged();

			if (Editor.BufferPaneForm != null)
				BufferPaneForm.SetPaneSize();
		}

		/// <summary>
		/// Occurs when the Workshop.UI.ShowGridLines property changes.
		/// </summary>
		private void UI_DisplayGridLines(object sender, EventArgs e)
		{
			this.Settings_ShowGrid.Image = GetMenuCheckedImage(_workshop.UI.GridLineWidth != 0);
			if (BufferPaneForm != null)
				BufferPaneForm.SetPaneSize();
		}

		/// <summary>
		/// Occurs when the Workshop.UI.ShowRuler property changes.
		/// </summary>
		private void UI_DisplayRuler(object sender, EventArgs e)
		{
			Settings_ShowRuler.Image = GetMenuCheckedImage(_workshop.UI.ShowRuler);
		}

		/// <summary>
		/// Occurs when the Workshop.UI.LatticeSize property changes.
		/// </summary>
		private void UI_LatticeSizeChanged(object sender, EventArgs e)
		{ 
			tssResolution.Text = UISettings.ʃLatticeSize.Width + "x" + UISettings.ʃLatticeSize.Height;
			if (Editor.BufferPaneForm != null)
				BufferPaneForm.SetPaneSize();
		}

		/// <summary>
		/// Occurs when the mouse position on the Canvas changes.
		/// </summary>
		private void UI_MousePoint(object sender, EventArgs e)
		{
			if (_workshop.UI.MouseSelectionSize.IsEmpty)
				tssMouseCoords.Text = string.Format("({0}, {1})", _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);
			else
				tssMouseCoords.Text = string.Format("({0}, {1})-({2}, {3})", _workshop.UI.MouseDownPosition.X, _workshop.UI.MouseDownPosition.Y, _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);

			string ChannelList = string.Empty;
			foreach (Channel Ch in _workshop.Channels.Sorted)
			{
				if (Ch.Lattice.Contains(_workshop.UI.MousePosition))
					ChannelList += ((ChannelList.Length > 0) ? ", " : string.Empty) + Ch.ToString(true);
			}
			tssChannels.Text = ChannelList;
		}

		/// <summary>
		/// Occurs when the Workshop.UI.Zoom property changes.
		/// </summary>
		private void UI_Zooming(object sender, EventArgs e)
		{
			tssZoom.Text = (_workshop.UI.Zoom * 100) + "%";
			if (BufferPaneForm != null)
				BufferPaneForm.SetPaneSize();
		}

		#endregion [ UI Events ]

		#region [ Mask Events ]

		/// <summary>
		/// Occurs when the Mask is Defined
		/// </summary>
		public void Mask_Defined(object sender, EventArgs e)
		{
			Edit_Copy.Enabled = true;
			Edit_Cut.Enabled = true;

			ToolBar_Copy.Enabled = true;
			ToolBar_Cut.Enabled = true;
		}

		/// <summary>
		/// Occurs when the Mask is Cleared
		/// </summary>
		public void Mask_Cleared(object sender, EventArgs e)
		{
			Edit_Copy.Enabled = false;
			Edit_Cut.Enabled = false;

			ToolBar_Copy.Enabled = false;
			ToolBar_Cut.Enabled = false;
		}

		#endregion [ Mask Events ]

		#region [ UndoController Events ]

		/// <summary>
		/// Occurs when a new item is the topmost item in the Redo stack, or the Redo stack has been emptied
		/// </summary>
		public void Redo_Changed(object sender, UndoEventArgs e)
		{
			if (e.HasData)
			{
				Edit_Redo.Text = "&Redo " + e.Text;
				Edit_Redo.Enabled = true;
				ToolBar_Redo.ToolTipText = "Redo " + e.Text;
				ToolBar_Redo.Enabled = true;
			}
			else
			{
				Edit_Redo.Text = "&Redo";
				Edit_Redo.Enabled = false;
				ToolBar_Redo.ToolTipText = "Redo";
				ToolBar_Redo.Enabled = false;
			}
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Undo stack, or the Undo stack has been emptied
		/// </summary>
		public void Undo_Changed(object sender, UndoEventArgs e)
		{
			if (e.HasData)
			{
				Edit_Undo.Text = "&Undo " + e.Text;
				Edit_Undo.Enabled = true;
				ToolBar_Undo.ToolTipText = "Undo " + e.Text;
				ToolBar_Undo.Enabled = true;
			}
			else
			{
				Edit_Undo.Text = "&Undo";
				Edit_Undo.Enabled = false;
				ToolBar_Undo.ToolTipText = "Undo";
				ToolBar_Undo.Enabled = false;
			}
		}

		#endregion [ UndoController Events ]

		#region [ Clipboard Events ]

		/// <summary>
		/// Occurs when the Clipboard contents are changed. Usually when something is Copied or Cut to the buffer
		/// </summary>
		public void Clipboard_Changed(object sender, EventArgs e)
		{
			Edit_Paste.Enabled = _workshop.Clipboard.HasData;
			ToolBar_Paste.Enabled = _workshop.Clipboard.HasData;
		}

		#endregion [ Clipboard Events ]

		#region [ Form Events ]

		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();

			if (_workshop.Dirty && (this.DialogResult == DialogResult.Cancel))
			{
				switch (MessageBox.Show("Keep changes?", "Adjustable preview", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						this.DialogResult = DialogResult.Yes;
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.No:
						this.DialogResult = DialogResult.No;
						break;
				}
			}
			else
			{
				if ((this.DialogResult == DialogResult.Cancel) || (this.DialogResult == DialogResult.Ignore))
					this.DialogResult = DialogResult.No;
			}
		}

		private void Form_Load(object sender, System.EventArgs e)
		{ }
		
		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			if (_workshop.CurrentTool != null)
				if (_workshop.CurrentTool.Tool.KeyDown(e))
				{
					e.Handled = false;
					return;
				}

			//_canvasWindow.ControlKeyDown = (e.Control && e.Control);

			if (Editor.Loading)
				return;

			try
			{
				Editor.Loading = true;

				//bool MovingSelection = false;//			_workshop.UsingMaskTool();
				//UndoData.v1 Undo = new UndoData.v1();
				//List<Channel> ChannelList = null;

				switch (e.KeyCode)
				{
					#region [ Move Channel Keys ]

					case Keys.Up:
					case Keys.Down:
					case Keys.Left:
					case Keys.Right:
					case Keys.PageUp:
					case Keys.PageDown:
					case Keys.Home:
					case Keys.End:

						if (e.Control && e.Shift)
						{
							//if ((_workshop.CurrentTool == (int)Tool.MoveChannel) || MovingSelection)
							//{
							//    RectangleF GroupRect;

							//    string Action = string.Empty;

							//    if (MovingSelection)
							//    {
							//        GroupRect = _workshop.Mask.Outline.GetBounds();
							//        Action = Constants.UNDO_MOVESELECTION;
							//    }
							//    else
							//    {
							//        Action = Constants.UNDO_MOVEChannel;
							//        if (_workshop.ToolSettings.MoveAllChannels)
							//            ChannelList = _workshop.Channels;
							//        else
							//            ChannelList = _workshop.SelectedChannels;
							//        GroupRect = GetChannelGroupBound(ChannelList);
							//    }

							//    CreateUndo_Channel(Action);

							//    if (MovingSelection)
							//    {
							//        foreach (Channel ch in _workshop.SelectedChannels)
							//        {
							//            _canvasWindow.Clipboard.TransferToMoveChannel(ch);
							//            _workshop.MoveChannel.Color = ch.Color;
							//            MoveTheChannel(_workshop.MoveChannel, e.KeyCode, GroupRect, true, (ch == _workshop.SelectedChannels[_workshop.SelectedChannels.Count - 1]));
							//            _canvasWindow.Clipboard.RestoreFromMoveChannel(ch);
							//        }
							//    }
							//    else
							//    {
							//        foreach (Channel Channel in ChannelList)
							//        {
							//            if (Channel.IsSpecialChannel)
							//                continue;
							//            MoveTheChannel(Channel, e.KeyCode, GroupRect, false, false);
							//        }
							//    }
							//}
						}
						else
						{
							//string KeyStrokes = string.Empty;
							//if (e.Shift)
							//    KeyStrokes += "+";
							//switch (e.KeyCode)
							//{
							//    case Keys.Up:
							//        KeyStrokes += "{UP}";
							//        break;
							//    case Keys.Down:
							//        KeyStrokes += "{DOWN}";
							//        break;
							//    case Keys.PageUp:
							//        KeyStrokes += "{PGUP}";
							//        break;
							//    case Keys.PageDown:
							//        KeyStrokes += "{PGDN}";
							//        break;
							//    case Keys.Home:
							//        KeyStrokes += "{HOME}";
							//        break;
							//    case Keys.End:
							//        KeyStrokes += "{END}";
							//        break;
							//}
							//_ChannelExplorer.Channels.Focus();
							//_ChannelExplorer.Channels_KeyDown(sender, e);
							//SendKeys.Send(KeyStrokes);
						}
						break;

					#endregion [ Move Channel Keys ]

					//case Keys.Delete:
					//    //CreateUndo_Channel(Constants.UNDO_DELETE);
					//    _workshop.DeleteCells();
					//    _workshop.UndoController.SaveUndo("Delete");
					//    break;

					case Keys.Escape:
						if (_workshop.CurrentTool != null)
							_workshop.CurrentTool.Tool.Cancel();
						break;
				}
			}
			finally
			{
				//Undo = null;
				Editor.Loading = false;
			}
		}

		private void Form_KeyUp(object sender, KeyEventArgs e)
		{
			//_canvasWindow.ControlKeyDown = (e.Control && e.Control);
		}

		private void Form_Resize(object sender, EventArgs e)
		{
			pnlToolStripDocker.Width = ToolStrip_Edit.Width - pnlToolStripDocker.Location.X;
			pnlToolStripDocker.Height = ToolStrip_Edit.Height;
		}

		private void Form_ResizeBegin(object sender, EventArgs e)
		{
			DockPanel.SuspendLayout();
		}

		private void Form_ResizeEnd(object sender, EventArgs e)
		{
			DockPanel.ResumeLayout();
		}

		private void Form_Shown(object sender, EventArgs e)
		{
			this.Text = (this.Text + " - " + AssemblyInfo.Trademark).TrimEnd() + " Version " + AssemblyInfo.AssemblyVersion;
			if (_workshop.Profile_Name.Length > 0)
				this.Text += " - " + _workshop.Profile_Name;
			if (pnlToolStripDocker.Controls.Count > 0)
				pnlToolStripDocker.Controls[0].Visible = true;
		}

		#endregion [ Form Events ]

		#region [ Menu Events ]

		/// <summary>
		/// Enabled child menu items if the Background image has been loaded.
		/// </summary>
		private void BGImageMenu_MouseEnter(object sender, EventArgs e)
		{
			bool HasData = _workshop.UI.Background.HasData;
			BGImage_Save.Enabled = HasData;
			BGImage_Clear.Enabled = HasData;
			BGImage_Grid.Enabled = HasData;
			BGImage_ResetSize.Enabled = HasData;
			BGImage_Brightness.Enabled = HasData;
		}

		/// <summary>
		/// Set the brightness of the background image
		/// </summary>
		private void BGImage_Brightness_Click(object sender, EventArgs e)
		{
			Brightness frmBrightness = new Brightness();
			frmBrightness.Workshop = _workshop;
			if (frmBrightness.ShowDialog() == DialogResult.OK)
				_workshop.UndoController.SaveUndo("Set Image Brightness");

			frmBrightness = null;
		}

		/// <summary>
		/// Remove the background image
		/// </summary>
		private void BGImage_Clear_Click(object sender, EventArgs e)
		{
			_workshop.UI.Background.Clear(true);
			_workshop.UI.Dirty = true;
			_workshop.UndoController.SaveUndo("Clear Background Image");
			BGImage_ResetSize.Enabled = false;
		}

		/// <summary>
		/// Superimpose a black grid overtop the background image
		/// </summary>
		private void BGImage_Grid_Click(object sender, EventArgs e)
		{
			_workshop.UI.SuperimposeGridOnBackground = !_workshop.UI.SuperimposeGridOnBackground;
		}

		/// <summary>
		/// Loads an image into the background
		/// </summary>
		private void BGImage_Load_Click(object sender, EventArgs e)
		{
			if (OpenImageFileDialog.ShowDialog() != DialogResult.OK)
				return;

			if (!_workshop.UI.Background.LoadFromFile(OpenImageFileDialog.FileName))
			{
				MessageBox.Show("Invalid Image File", "Load Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			_workshop.UndoController.SaveUndo("Load Background Image");
			BGImage_ResetSize.Enabled = true;

			//Bitmap bmp = null;
			//MemoryStream stream = null;
			//FileStream fs = new FileStream(OpenImageFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			//byte[] fileBytes = new byte[fs.Length];
			//fs.Read(fileBytes, 0, (int)fs.Length);
			//fs.Close();
			//fs.Dispose();

			//try
			//{
			//    stream = new MemoryStream(fileBytes);
			//    bmp = new Bitmap(stream);
			//}
			//catch
			//{
			//    MessageBox.Show("Invalid Image File", "Load Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    return;
			//}
			//finally
			//{
			//    stream.Close();
			//    stream.Dispose();
			//}

			//UndoData.v1 Undo = new UndoData.v1();
			//Undo.Action = Constants.UNDO_LOADBGIMAGE;
			//Undo.GeneralEvent = GeneralDataEvent.UI;
			//Undo.SpecificEvent = SpecificEventType.BackgroundImage_Load;
			//Undo.Brightness = _workshop.UI.BackgroundImage_Brightness;
			//Undo.BackgroundImage = _workshop.UI.BackgroundImage;
			//Undo.BackgroundImageFilename = _workshop.UI.BackgroundImage_Filename;
			//_workshop.UndoController.Push(Undo);
			//Undo = null;

			//_workshop.UI.Background.LoadFromFile(OpenImageFileDialog.FileName);

			//_workshop.UI.BackgroundImage = bmp;
			//_workshop.UI.BackgroundImage_Filename = OpenImageFileDialog.FileName;
			//_workshop.UndoController.SaveUndo("Load Background Image");
			//BGImage_ResetSize.Enabled = true;
		}

		/// <summary>
		/// Sets the Canvas size to match that of the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BGImage_ResetSize_Click(object sender, EventArgs e)
		{
			if (_workshop.UI.Background.HasData)
			{
				_workshop.UI.LatticeSize = _workshop.GetImageSizeInCells();
				_workshop.UI.Dirty = true;
				_workshop.UndoController.SaveUndo("Size Canvas to Match Image");
			}
		}

		/// <summary>
		/// Saves the background image to file
		/// </summary>
		private void BGImage_Save_Click(object sender, EventArgs e)
		{
			_workshop.UI.Background.Filename = Workshop.SaveBitmap(_workshop.UI.Background.Image, _workshop.UI.Background.Filename, "Save Background Image");
			_workshop.UI.Dirty = true;
		}

		/// <summary>
		/// Makes all Channels visible
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Channel_AllVisible_Click(object sender, EventArgs e)
		{
			_workshop.Channels.MakeAllChannelsVisible();
			_canvasWindow.CanvasPane.Refresh();
			//_ChannelExplorer.Freshen();
		}

		/// <summary>
		/// Changes the text on the Clear_SelectedChannels menu item depending on how many Channels are selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Channel_Clear_Click(object sender, EventArgs e)
		{
			if (_workshop.Channels.Selected.Count == 1)
				Clear_SelectedChannels.Text = "&Current Channel";
			else
				Clear_SelectedChannels.Text = "&Selected Channels";
		}

		/// <summary>
		/// Clear out the Lattice of all the Channels
		/// </summary>
		private void Channel_Clear_AllChannels_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Really clear the pixels from ALL Channels?", "Clear All Channels", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK)
			{
				_workshop.Channels.ClearAllChannels();
				_workshop.UndoController.SaveUndo("Clear All Channels");
			}
		}

		/// <summary>
		/// Clear out the Lattice for the selected Channel(s)
		/// </summary>
		private void Channel_Clear_SelectedChannels_Click(object sender, EventArgs e)
		{
			bool Clear = false;
			string Title = string.Empty;

			if (_workshop.Channels.Selected.Count == 1)
			{
				Title = "Clear Channel";
				Clear = (MessageBox.Show("Really clear the cells from the selected Channel?", Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK);
			}
			else
			{
				Title = "Clear Selected Channels";
				Clear = (MessageBox.Show("Really clear the cells from all the selected Channels?", Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK);
			}

			if (!Clear)
				return;

			_workshop.Channels.ClearSelectedChannels();
			_workshop.UndoController.SaveUndo(Title);
		}

		/// <summary>
		/// Import Channel data from another Profile
		/// </summary>
		private void Channel_Import_Click(object sender, EventArgs e)
		{
			if (OpenProfileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			ImportFromProfile frmImport = new ImportFromProfile();
			frmImport.ProfileFileName = OpenProfileDialog.FileName;
			frmImport.Workshop = _workshop;
			if (frmImport.ShowDialog() == DialogResult.Cancel)
				return;

			List<Mapped> Mapping = frmImport.MappedChannels;

			if (!frmImport.NewLatticeSize.IsEmpty)
				_workshop.UI.LatticeSize = frmImport.NewLatticeSize;

			bool ClearChannels = frmImport.ClearChannelsBeforeImport;
			Channel Channel = null;

			foreach (Mapped Mapped in Mapping)
			{
				Channel = _workshop.Channels.GetChannelByID(Mapped.ID);
				if (Channel == null)
					continue;
				Channel.DecodeChannelBytes(Mapped.EncodedBytes, ClearChannels);
			}

			_workshop.UndoController.SaveUndo("Import Channel" + ((Mapping.Count > 1) ? "s" : string.Empty));
			Workshop.Canvas.Refresh();
			frmImport = null;
		}

		/// <summary>
		/// Import a bitmap from disk, overlay the white pixels onto the lattice of the active Channel.
		/// </summary>
		private void Channel_LoadFromBitmap_Click(object sender, EventArgs e)
		{
			string Filename = string.Empty;
			Bitmap Bmp = LoadBitmap(out Filename);
			DialogResult Result;
			if (Bmp != null)
			{
				Result = MessageBox.Show("Overwrite cells in this Channel?", "Import Cells From Bitmap", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (Result == DialogResult.Cancel)
					return;

				_workshop.Channels.Active.Visible = true;
				if (Result == System.Windows.Forms.DialogResult.Yes)
					_workshop.Channels.Active.ClearLattice();
				_workshop.Channels.Active.LatticeBuffer = Bmp;
				_workshop.UndoController.SaveUndo("Load Channel From Bitmap");
				Workshop.Canvas.Refresh();
			}
		}

		/// <summary>
		/// Saves the selected Channel to a bitmap
		/// </summary>
		private void Channel_SaveToBitmap_Click(object sender, EventArgs e)
		{
			Workshop.SaveBitmap(_workshop.Channels.Active.LatticeBuffer, string.Empty);
		}

		/// <summary>
		/// Saves a copy of the bitmap of the selected Channel to be the background image
		/// </summary>
		private void Channel_SetAsBackground_Click(object sender, EventArgs e)
		{
			Color PaintColor = _workshop.Channels.Active.Color;
			DialogResult Result;

			Result = MessageBox.Show("Setting an image of the current Channel as the background Image." + Environment.NewLine +
									 "Would you like to change the color of the Channel to another color before creating the image?" + Environment.NewLine +
									 "This will be for the image only and will not affect the actual Channel.",
									 "Set Channel As Background Image",
									 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (Result == DialogResult.Cancel)
				return;

			if (Result == DialogResult.Yes)
			{
				ChannelColorDialog.Color = PaintColor;
				ChannelColorDialog.AllowFullOpen = true;
				ChannelColorDialog.AnyColor = true;
				if (ChannelColorDialog.ShowDialog() != DialogResult.Cancel)
					PaintColor = ChannelColorDialog.Color;
			}

			_workshop.UI.Background.Clear(false);
			_workshop.UI.Background.Image = _workshop.Channels.Active.DrawChannel(PaintColor, Color.Black);
			_workshop.UndoController.SaveUndo("Set Channel As Background Image");
		}

		/// <summary>
		/// Set the color of the selected Channel
		/// </summary>
		private void Channel_SetColor_Click(object sender, EventArgs e)
		{			
			ChannelColorDialog.Color = _workshop.Channels.Active.Color;
			ChannelColorDialog.AllowFullOpen = true;
			ChannelColorDialog.AnyColor = true;
			if (ChannelColorDialog.ShowDialog() == DialogResult.Cancel)
				return;

			_workshop.Channels.Active.Color = ChannelColorDialog.Color;
			_workshop.UndoController.SaveUndo("Change Channel Color");
		}

		/// <summary>
		/// Populates the Undo and Redo menus based on values held by the UndoController. If the appropriate stack is empty, the disables the menu item.
		/// Also sets the Clipboard menu items based on if there is data in the clipboard, or there is an area defined by a Mask
		/// </summary>
		private void EditMenu_MouseEnter(object sender, EventArgs e)
		{
			//if (_workshop.UndoController.HasUndo)
			//{
			//    Edit_Undo.Enabled = true;
			//    Edit_Undo.Text = "&Undo " + _workshop.UndoController.UndoText;
			//}
			//else
			//{
			//    Edit_Undo.Enabled = false;
			//    Edit_Undo.Text = "&Undo";
			//}
			//if (_workshop.UndoController.HasRedo)
			//{
			//    Edit_Redo.Enabled = true;
			//    Edit_Redo.Text = "&Redo " + _workshop.UndoController.RedoText;
			//}
			//else
			//{
			//    Edit_Redo.Enabled = false;
			//    Edit_Redo.Text = "&Redo";
			//}

			//Edit_Copy.Enabled = _workshop.Mask.HasMask;
			//Edit_Cut.Enabled = Edit_Copy.Enabled;
			//Edit_Paste.Enabled = Editor.Clipboard.HasData;
		}

		/// <summary>
		/// Copies the contents of the marquee from the selected Channel(s)
		/// </summary>
		private void Edit_Copy_Click(object sender, EventArgs e)
		{
			if (!_workshop.Clipboard.Copy())
				MessageBox.Show("Selection is Empty!", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			//Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
		}

		/// <summary>
		/// Cuts the contents of the marquee from the selected Channel(s)
		/// </summary>
		private void Edit_Cut_Click(object sender, EventArgs e)
		{
			if (!_workshop.Clipboard.Cut())
				MessageBox.Show("Selection is Empty!", "Cut", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else
			{
				_workshop.ClearMask();
				Editor.ExposePane(_workshop.Clipboard.Channels[0], Panes.ClipboardChannel);
				Workshop.Canvas.Refresh();
			}
		}

		/// <summary>
		/// Pastes the contents from the Clipboard to the selected Channel(s)
		/// </summary>
		private void Edit_Paste_Click(object sender, EventArgs e)
		{
			if (_workshop.Clipboard.HasData)
			{
				_workshop.Clipboard.Paste();
				using (Region Region = new Region(_workshop.Mask.CanvasMask.Outline))
					_canvasWindow.CanvasPane.Invalidate(Region);
				
				//Editor.ExposePane(_workshop.ClipboardChannel, Panes.ClipboardChannel);
				//Editor.ExposePane(_workshop.MoveChannel, Panes.MoveChannel);
			}
		}

		/// <summary>
		/// Reapply the last operation that was recently undone.
		/// </summary>
		private void Edit_Redo_Click(object sender, EventArgs e)
		{
			Cursor LastCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			_workshop.UndoController.Redo();
			Workshop.Canvas.Refresh();
			this.Cursor = LastCursor;
		}

		/// <summary>
		/// Reverse out the last operation
		/// </summary>
		private void Edit_Undo_Click(object sender, EventArgs e)
		{
			Cursor LastCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			_workshop.UndoController.Undo();
			Workshop.Canvas.Refresh();
			this.Cursor = LastCursor;
		}

		private void File_Save_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Yes;
		}

		private void File_Exit_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
		}

		/// <summary>
		/// Sets the Canvas Size
		/// </summary>
		private void Grid_SetResolution_Click(object sender, EventArgs e)
		{
			ChangeLatticeSize frmLatticeSize = new ChangeLatticeSize();
			frmLatticeSize.Workshop = _workshop;
			if (frmLatticeSize.ShowDialog() == DialogResult.Cancel)
				return;

			//UndoData.v1 Undo = new UndoData.v1();
			//Undo.Action = Constants.UNDO_LATTICE_SIZE;
			//Undo.GeneralEvent = GeneralDataEvent.UI;
			//Undo.SpecificEvent = SpecificEventType.LatticeSize;
			//Undo.LatticeSize = _workshop.UI.LatticeSize;
			//_workshop.UndoController.Push(Undo);
			//Undo = null;

			_workshop.UI.LatticeSize = new Size(frmLatticeSize.CanvasWidth, frmLatticeSize.CanvasHeight);
			_workshop.UndoController.SaveUndo("Set Canvas Resolution");
			frmLatticeSize = null;
		}

		/// <summary>
		/// Sets the Cell Size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CellSize_Click(object sender, EventArgs e)
		{
			Editor.Loading = true;
			_workshop.UI.CellSize = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
			Editor.Loading = false;

			_workshop.UndoController.SaveUndo("Change Cell Size");
		}

		private void Settings_KeyConfig_Click(object sender, EventArgs e)
		{
			ConfigureKeys frmConfig = new ConfigureKeys();
			if (frmConfig.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;
		}

		private void Settings_PaintGridLines_Click(object sender, EventArgs e)
		{
			Editor.Loading = true;
			_workshop.UI.ShowGridLineWhilePainting = !_workshop.UI.ShowGridLineWhilePainting;
			Editor.Loading = false;
		}

		private void Settings_RespectChannelOutputsDuringPlayback_Click(object sender, EventArgs e)
		{
			Editor.Loading = true;
			_workshop.UI.RespectChannelOutputsDuringPlayback = !_workshop.UI.RespectChannelOutputsDuringPlayback;
			Editor.Loading = false;
		}

		private void Settings_ShowGrid_Click(object sender, EventArgs e)
		{
			//UndoData.v1 Undo = new UndoData.v1();
			//if (ShowGridLines)
			//    Undo.Action = Constants.UNDO_HIDEGRID;
			//else
			//    Undo.Action = Constants.UNDO_SHOWGRID;

			//Undo.GeneralEvent = GeneralDataEvent.UI;
			//Undo.SpecificEvent = SpecificEventType.GridLineWidth;
			//Undo.GridLineWidth = _workshop.UI.GridLineWidth;
			//_workshop.UndoController.Push(Undo);
			//Undo = null;

			//ShowGridLines = !ShowGridLines;
			Editor.Loading = true;
			_workshop.UI.GridLineWidth = (!ShowGridLines ? 1 : 0);
			Editor.Loading = false;
			_workshop.UndoController.SaveUndo(ShowGridLines ? "Show Grid Lines" : "Hide Grid Lines");
		}

		private void Settings_ShowRuler_Click(object sender, EventArgs e)
		{
			Editor.Loading = true;
			_workshop.UI.ShowRuler = !_workshop.UI.ShowRuler;
			Editor.Loading = false;
		}

		private void ShowPane_Click(object sender, EventArgs e)
		{
			BufferPaneForm.SetTab((Panes)((ToolStripMenuItem)sender).Tag);
		}

		#endregion [ Menu Events ]

		#region [ ToolBox Events ]

		private void ToolGroup_MouseUp(object sender, MouseEventArgs e)
		{
			if (tmrFlyout.Enabled)
			{
				tmrFlyout.Enabled = false;
				_flyoutToolStrip = null;
				//_hoverTool = (int)Tool.NotSet;
			}
		}

		private void ToolGroup_MouseDown(object sender, MouseEventArgs e)
		{
			ToolHost TG = ((PlugInToolStripButton)sender).ToolHost;

			_flyoutToolStrip = TG.ChildToolBox;
			tmrFlyout.Enabled = true;

			//_hoverTool = (int)Tool.Mask;
			//tmrFlyout.Enabled = true;
		}

		private void MaskTool_MouseDown(object sender, MouseEventArgs e)
		{
			//_hoverTool = (int)Tool.Mask;
			//tmrFlyout.Enabled = true;
		}

		private void MultiChannelTool_MouseDown(object sender, MouseEventArgs e)
		{
			//_hoverTool = (int)Tool.MultiChannelTool;
			//tmrFlyout.Enabled = true;
		}

		private void ShapeTool_MouseDown(object sender, MouseEventArgs e)
		{
			//_hoverTool = (int)Tool.Shape;
			//tmrFlyout.Enabled = true;
		}

		private void tmrFlyout_Tick(object sender, EventArgs e)
		{
			tmrFlyout.Enabled = false;
			_flyoutToolStrip.Visible = true;
			this.Controls.SetChildIndex(_flyoutToolStrip, 0); // Bring this submenu to the Front of all other controls

			//if (_hoverTool == (int)Tool.Mask)
			//{
			//    SubMenu_MaskTool.Visible = true;
			//    this.Controls.SetChildIndex(SubMenu_MaskTool, 0); // Bring this submenu to the Front of all other controls
			//}
			//else if (_hoverTool == (int)Tool.Shape)
			//{
			//    SubMenu_ShapeTool.Visible = true;
			//    this.Controls.SetChildIndex(SubMenu_ShapeTool, 0); // Bring this submenu to the Front of all other controls
			//}
			//else if (_hoverTool == (int)Tool.MultiChannelTool)
			//{
			//    SubMenu_MultiChannelTool.Visible = true;
			//    this.Controls.SetChildIndex(SubMenu_MultiChannelTool, 0); // Bring this submenu to the Front of all other controls
			//}			
		}

		private void Tool_Click(object sender, EventArgs e)
		{
			PlugInToolStripButton Button = (PlugInToolStripButton)sender;

			ToolHost SelectedTool = Button.ToolHost;

			foreach (ToolHost OtherTool in _workshop.Tools)
			{
				if (OtherTool != SelectedTool)
					OtherTool.Tool.Unselected();
			}

			SelectedTool.Tool.Selected();
			_workshop.CurrentTool = SelectedTool;

			// Update the status bar
			tssTool.Text = SelectedTool.ToString();

			// swap out the setting toolstrips
			pnlToolStripDocker.Controls.Clear();

			ToolStrip Strip = SelectedTool.Tool.SettingsToolStrip;
			if (Strip == null)
				Strip = ToolStrip_Blank;

			Strip.Dock = DockStyle.Top;
			pnlToolStripDocker.Controls.Add(Strip);

			// Uncheck all the other tools in the ToolBox
			ToolBox_Main.SuspendLayout();

			// Check to see if the button clicked was on a sub menu. If so, then 
			// set Button to be the ToolGroup button
			if (SelectedTool.ParentButton != null)
				Button = SelectedTool.ParentButton;

			foreach (PlugInToolStripButton tButton in ToolBox_Main.Items)
			{
				tButton.Checked = (tButton == Button);
			}

			_canvasWindow.CanvasPane.Cursor = SelectedTool.Tool.Cursor;

			ToolBox_Main.ResumeLayout(true);
		}

		private void SubMenu_MouseLeave(object sender, EventArgs e)
		{
			((ToolStrip)sender).Visible = false;
		}

		/// <summary>
		/// The operation for the current tool has finished, create the Undo for it.
		/// </summary>
		private void Tool_OperationCompleted(object sender, EventArgs e)
		{
			_workshop.UndoController.SaveUndo(((ITool)sender).UndoText);
		}


		#endregion [ ToolBox Events ]
		
		#endregion [ Events ]

		#region [ DEAD CODE ]

		//private void SetToolBoxTags()
		//{
		//    PaintTool.Tag = Tool.Paint;
		//    SprayTool.Tag = Tool.Spray;
		//    EraserTool.Tag = Tool.Erase;
		//    ShapeTool.Tag = Tool.Rectangle;
		//    MaskTool.Tag = Tool.RectangleMask;
		//    MultiChannelTool.Tag = Tool.MultiChannelLine;
		//    MoveChannelTool.Tag = Tool.MoveChannel;
		//    ZoomTool.Tag = Tool.Zoom;
		//    FillTool.Tag = Tool.Fill;
		//    TextTool.Tag = Tool.Text;
		//    CropTool.Tag = Tool.Crop;
		//    ImageStampTool.Tag = Tool.ImageStamp;

		//    RectangleTool.Tag = Tool.Rectangle;
		//    EllipseTool.Tag = Tool.Ellipse;
		//    LineTool.Tag = Tool.Line;
		//    PolygonTool.Tag = Tool.Polygon;
		//    IcicleTool.Tag = Tool.Icicles;

		//    MultiChannelLineTool.Tag = Tool.MultiChannelLine;
		//    MegatreeTool.Tag = Tool.MegaTree;
		//    SingingFaceTool.Tag = Tool.SingingFace;

		//    RectangleMask.Tag = Tool.RectangleMask;
		//    EllipseMask.Tag = Tool.EllipseMask;
		//    FreehandMask.Tag = Tool.FreehandMask;
		//    PaintMask.Tag = Tool.PaintMask;
		//}

		//private void UpdateToolBarForTool()
		//{
		//    ToolStrip Strip = null;

		//    if (pnlToolStripDocker.Controls.Count > 0)
		//        pnlToolStripDocker.Controls.Clear();

		//    switch (_workshop.CurrentTool)
		//    {
		//        case (int)Tool.Paint:
		//            Paint_ToolName.Text = "Paint";
		//            Strip = ToolStrip_Paint;
		//            break;

		//        case (int)Tool.Erase:
		//            Paint_ToolName.Text = "Erase";
		//            Strip = ToolStrip_Paint;
		//            break;

		//        case (int)Tool.Spray:
		//            Strip = ToolStrip_Spray;
		//            break;

		//        case (int)Tool.Mask:
		//        case (int)Tool.RectangleMask:
		//        case (int)Tool.EllipseMask:
		//        case (int)Tool.FreehandMask:
		//            MaskSep2.Visible = false;
		//            _cboMaskNibSize.Visible = false;
		//            cboMask_NibSize.Visible = false;
		//            Mask_SquareNib.Visible = false;
		//            Mask_RoundNib.Visible = false;
		//            Strip = ToolStrip_Mask;
		//            break;

		//        case (int)Tool.PaintMask:
		//            MaskSep2.Visible = true;
		//            _cboMaskNibSize.Visible = true;
		//            cboMask_NibSize.Visible = true;
		//            Mask_SquareNib.Visible = true;
		//            Mask_RoundNib.Visible = true;
		//            Strip = ToolStrip_Mask;
		//            break;

		//        case (int)Tool.Fill:
		//            break;

		//        case (int)Tool.Icicles:
		//            Strip = ToolStrip_Icicles;
		//            break;

		//        case (int)Tool.MegaTree:
		//            Strip = ToolStrip_MegaTree;
		//            break;

		//        case (int)Tool.Shape:
		//        case (int)Tool.Rectangle:
		//        case (int)Tool.Ellipse:
		//            Shape_Fill.Visible = true;
		//            Shape_NoFill.Visible = true;
		//            Strip = ToolStrip_Shape;
		//            break;

		//        case (int)Tool.Polygon:
		//            Strip = ToolStrip_Polygon;
		//            break;

		//        case (int)Tool.Line:
		//            Shape_Fill.Visible = false;
		//            Shape_NoFill.Visible = false;
		//            Shape_ToolName.Text = "Line";
		//            Strip = ToolStrip_Shape;
		//            break;

		//        case (int)Tool.MultiChannelLine:
		//            Strip = ToolStrip_MultiChannelLine;
		//            break;

		//        case (int)Tool.Text:
		//            Strip = ToolStrip_Text;
		//            break;

		//        case (int)Tool.Zoom:
		//            Strip = ToolStrip_Zoom;
		//            break;

		//        case (int)Tool.MoveChannel:
		//            Strip = ToolStrip_MoveChannel;
		//            break;

		//        case (int)Tool.SingingFace:
		//            Strip = ToolStrip_SingingFace;
		//            break;

		//        case (int)Tool.ImageStamp:
		//            Strip = ToolStrip_ImageStamp;
		//            break;

		//        default:
		//            if (_workshop.CurrentPlugInTool != null)
		//                Strip = _workshop.CurrentPlugInTool.Tool.SettingsToolStrip;
		//            break;

		//    }

		//    if (Strip == null)
		//        Strip = ToolStrip_Blank;

		//    pnlToolStripDocker.Controls.Add(Strip);

		//    //ToolBox_Main.SuspendLayout();

		//    // Uncheck the other tools
		//    foreach (ToolStripButton Menu in ToolBox_Main.Items)
		//    {
		//        if (Menu.Tag != null)
		//        {
		//            if (XmlHelper.IsNumeric(Menu.Tag))
		//                Menu.Checked = ((int)Menu.Tag == _workshop.CurrentTool);
		//        }
		//    }

		//    //ToolStrip_Edit.ResumeLayout(true);
		//    //ToolBox_Main.ResumeLayout(true);
		//}

		//        private void Workshop_Changed(object sender, DataEventArgs e)
		//        {
		//            bool CurrentlyLoading = Editor.Loading;
		//            if (!CurrentlyLoading)				
		//                Editor.Loading = true;

		//            //Debug.WriteLine("EDITOR: " + e.GeneralEventType.ToString() + "\t" + e.SpecificEventType.ToString());

		//            try
		//            {
		//                switch (e.Category)
		//                {

		//                    #region [ Mask ]

		//                    case EventCategory.Mask:
		//                        switch (e.SubCategory)
		//                        {
		//                            //case SpecificEventType.Mask_Inverted:
		//                            //    break;

		//                            //case SpecificEventType.Mask_Moved:
		//                            //    break;

		//                            case EventSubCategory.Mask_Defined:
		//                                Edit_Copy.Enabled = true;
		//                                Edit_Cut.Enabled = true;
		//                                break;

		//                            case EventSubCategory.Mask_Cleared:
		//                                Edit_Copy.Enabled = false;
		//                                Edit_Cut.Enabled = false;
		//                                break;
		//                        }
		//                        break;

		//                    #endregion [ Mask ]

		//                    #region [ DEAD CODE ]
		//                //#region [ Channel ]
		//                    //case EventCategory.Channel:

		//                    //    switch (e.SubCategory)
		//                    //    {
		//                    //        case EventSubCategory.Channel_Selected:
		//                    //        case EventSubCategory.Channel_NameChanged:
		//                    //        case EventSubCategory.Channel_ColorChanged:
		//                    //            // Update the status bar with the correct Channel information
		//                    //            UpdateChannelStatus();
		//                    //            break;
		//                    //    }
		//                    //    break;

		//                    //#endregion [ Channel ]


		//                    //#region [ UI ]

		//                    //case EventCategory.UI:
		//                    //    switch (e.SubCategory)
		//                    //    {
		//                            //case SpecificEventType.BackgroundImage_Clear:
		//                            //    BGImage_Save.Enabled = false;
		//                            //    BGImage_Clear.Enabled = false;
		//                            //    BGImage_ResetSize.Enabled = false;
		//                            //    BGImage_Brightness.Enabled = false;
		//                            //    break;

		//                            //case SpecificEventType.BackgroundImage_Load:
		//                            //    BGImage_Save.Enabled = true;
		//                            //    BGImage_Clear.Enabled = true;
		//                            //    BGImage_ResetSize.Enabled = true;
		//                            //    BGImage_Brightness.Enabled = true;
		//                            //    break;

		//                            //case SpecificEventType.BackgroundImage_Brightness:
		//                            //    break;

		//                            //                            case EventSubCategory.LatticeSize:
		//                            //                                tssResolution.Text = UISettings.ʃLatticeSize.Width + "x" + UISettings.ʃLatticeSize.Height;
		//                            //#if DEBUG
		//                            //                                BufferPaneForm.SetPaneSize();
		//                            //#endif
		//                            //                                break;

		//                            //case EventSubCategory.CellSize:
		////                                int CellSize = _workshop.UI.CellSize;
		////                                ToolStripMenuItem PixelMenu = null;
		////                                for (int i = 0; i < 10; i++)
		////                                {
		////                                    PixelMenu = (ToolStripMenuItem)GridMenu.DropDownItems[i];
		////                                    PixelMenu.Checked = (PixelMenu.Tag.ToString() == CellSize.ToString());
		////                                }
		////                                tssCellSize.Text = string.Format("{0}x{0} Pixel{1}", CellSize, (CellSize == 1) ? "" : "s");

		////                                // Inform the Channel Controller to have the channels rebuild their GraphicsPaths next time they need to redraw.
		////                                _workshop.Channels.UIChanged();

		////                                // Inform the currently selected tool to account for this UI change.
		////                                CurrentToolUIChanged();
		////#if DEBUG
		////                                BufferPaneForm.SetPaneSize();
		////#endif
		//                            //    break;

		//                            //case EventSubCategory.CurrentMouseCellPixel:
		//                            //    if (_workshop.UI.MouseSelectionSize.IsEmpty)
		//                            //        tssMouseCoords.Text = string.Format("({0}, {1})", _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);
		//                            //    else
		//                            //        tssMouseCoords.Text = string.Format("({0}, {1})-({2}, {3})", _workshop.UI.MouseDownPosition.X, _workshop.UI.MouseDownPosition.Y, _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);

		//                            //    string ChannelList = string.Empty;
		//                            //    foreach (Channel Ch in _workshop.Channels.Sorted)
		//                            //    {
		//                            //        if (Ch.Lattice.Contains(_workshop.UI.MousePosition))
		//                            //            ChannelList += ((ChannelList.Length > 0) ? ", " : string.Empty) + Ch.ToString(true);
		//                            //    }
		//                            //    tssChannels.Text = ChannelList;
		//                            //    break;

		//                            //case EventSubCategory.Dirty:
		//                            //    if (_workshop.Dirty)
		//                            //    {
		//                            //        if (!this.Text.EndsWith("*"))
		//                            //            this.Text += "*";
		//                            //    }
		//                            //    else
		//                            //    {
		//                            //        if (this.Text.EndsWith("*"))
		//                            //            this.Text = this.Text.Substring(0, this.Text.Length - 1);
		//                            //    }
		//                            //    break;

		////                            case EventSubCategory.GridLineWidth:
		////                                this.Settings_ShowGrid.Image = GetMenuCheckedImage(_workshop.UI.GridLineWidth != 0);
		////#if DEBUG
		////                                BufferPaneForm.SetPaneSize();
		////#endif
		////                                // Inform the currently selected tool to accound for this UI change.
		////                                //CurrentToolUIChanged();
		////                                break;

		//                            //case EventSubCategory.Loading:
		//                            //    if (!_workshop.UI.Background.HasData)
		//                            //        e.SubCategory = EventSubCategory.BackgroundImage_Clear;
		//                            //    else
		//                            //        e.SubCategory = EventSubCategory.BackgroundImage_Load;
		//                            //    Workshop_Changed(sender, e);

		//                            //    e.SubCategory = EventSubCategory.CellSize;
		//                            //    Workshop_Changed(sender, e);

		//                            //    e.SubCategory = EventSubCategory.LatticeSize;
		//                            //    Workshop_Changed(sender, e);

		//                            //    //int ActiveChannelIndex = _workshop.Channels.Active.Index;
		//                            //    //_workshop.ActiveChannelIndex = 0;
		//                            //    //if (ActiveChannelIndex >= 0)
		//                            //    //    _workshop.ActiveChannelIndex = ActiveChannelIndex;
		//                            //    break;

		//                            //case EventSubCategory.ShowGridLineWhilePainting:
		//                            //    Settings_ShowGridLineWhilePainting.Image = GetMenuCheckedImage(_workshop.UI.ShowGridLineWhilePainting);
		//                            //    break;

		//                            //case EventSubCategory.ShowRuler:
		//                            //    Settings_ShowRuler.Image = GetMenuCheckedImage(_workshop.UI.ShowRuler);
		//                            //    break;

		//                            //case EventSubCategory.RespectChannelOutputsDuringPlayback:
		//                            //    Settings_RespectChannelOutputsDuringPlayback.Image = GetMenuCheckedImage(_workshop.UI.RespectChannelOutputsDuringPlayback);
		//                            //    break;

		////                            case EventSubCategory.Zoom:
		////                                tssZoom.Text = (_workshop.UI.Zoom * 100) + "%";
		////#if DEBUG
		////                                BufferPaneForm.SetPaneSize();
		////#endif

		////                                // Inform the Channel Controller to have the channels rebuild their GraphicsPaths next time they need to redraw.
		////                                _workshop.Channels.UIChanged();

		////                                // Inform the currently selected tool to accound for this UI change.
		////                                //CurrentToolUIChanged();
		////                                break;
		//                    //    }
		//                    //    break;

		//                    //#endregion [ UI ]

		//                    //#region [ Clipboard ]

		//                    //case GeneralDataEvent.Clipboard:
		//                    //    switch (e.SpecificEventType)
		//                    //    {
		//                    //        case SpecificEventType.Clipboard_Cut:
		//                    //        case SpecificEventType.Clipboard_Copy:
		//                    //            Edit_Paste.Enabled = true;
		//                    //            break;

		//                    //        case SpecificEventType.Clipboard_Delete:
		//                    //            break;

		//                    //        case SpecificEventType.Clipboard_Paste:
		//                    //            break;
		//                    //    }
		//                    //    break;

		//                    //#endregion [ Clipboard ]

		//                    //#region [ Tool ]

		//                    //case GeneralDataEvent.Tool:

		//                    //switch (e.SpecificEventType)
		//                    //{
		//                    //case SpecificEventType.Tool_Mask:
		//                    //    Button = GetButtonForTool(_workshop.MaskTool);
		//                    //    SetToolGroupButtonToSelectedTool(MaskTool, Button);
		//                    //    break;

		//                    //case SpecificEventType.Tool_MultiChannel:
		//                    //    Button = GetButtonForTool(_workshop.MultiChannelTool);
		//                    //    SetToolGroupButtonToSelectedTool(MultiChannelTool, Button);
		//                    //    break;

		//                    //case SpecificEventType.Tool_Shape:
		//                    //    Button = GetButtonForTool(_workshop.ShapeTool);
		//                    //    SetToolGroupButtonToSelectedTool(ShapeTool, Button);
		//                    //    break;

		//                    //case SpecificEventType.Tool_Selected:
		//                    //    Button = GetButtonForTool();
		//                    //    if (Button != null)
		//                    //        tssTool.Text = Button.Text;

		//                    //    // Button settings change. For toolbar change, put updates in
		//                    //    // UpdateToolBarForTool()

		//                    //    if (Workshop.IsShapeTool(_workshop.CurrentTool))
		//                    //    {
		//                    //        _workshop.ShapeTool = (Tool)_workshop.CurrentTool;
		//                    //        ShapeTool.ToolTipText = Button.ToolTipText;
		//                    //    }

		//                    //    else if (Workshop.IsMaskTool(_workshop.CurrentTool))
		//                    //    {
		//                    //        _workshop.MaskTool = (Tool)_workshop.CurrentTool;
		//                    //        MaskTool.ToolTipText = Button.ToolTipText;
		//                    //    }

		//                    //    else if (Workshop.IsMultiChannelTool(_workshop.CurrentTool))
		//                    //    {
		//                    //        _workshop.MultiChannelTool = (Tool)_workshop.CurrentTool;
		//                    //        MultiChannelTool.ToolTipText = Button.ToolTipText;
		//                    //    }

		//                    //    _workshop.CurrentPlugInTool = _workshop.GetPlugInTool();

		//                    //    UpdateToolBarForTool();
		//                    //    break;
		//                    //}
		//                    //    break;

		//                    //#endregion [ Tool ]

		//                    //#region [ Undo ]

		//                    //case GeneralDataEvent.Undo:

		//                    //    Edit_Undo.Enabled = (_workshop.UndoController.UndoStack.Count > 0);
		//                    //    if (Edit_Undo.Enabled)
		//                    //        Edit_Undo.Text = "&Undo " + _workshop.UndoController.UndoStack.Peek().Action;
		//                    //    else
		//                    //        Edit_Undo.Text = "&Undo";

		//                    //    ToolBar_Undo.Enabled = Edit_Undo.Enabled;
		//                    //    ToolBar_Undo.Text = Edit_Undo.Text.Replace("&", string.Empty);

		//                    //    //Edit_Redo.Enabled = (_data.RedoStack.Count > 0);
		//                    //    //if (Edit_Redo.Enabled)
		//                    //    //    Edit_Redo.Text = "&Redo " + _data.RedoStack.Peek().Action;
		//                    //    //else
		//                    //    //    Edit_Redo.Text = "&Redo";
		//                    //    //ToolBar_Redo.Enabled = Edit_Redo.Enabled;
		//                    //    //ToolBar_Redo.Text = Edit_Redo.Text.Replace("&", string.Empty);

		//                    //    _ChannelExplorer.Freshen();
		//                    //    break;

		//                    //#endregion [ Undo ]
		//                    #endregion [ DEAD CODE ]
		//                }
		//            }
		//            finally
		//            {
		//                if (!CurrentlyLoading)
		//                    Editor.Loading = false;
		//            }
		//        }

		//#region [ ChannelExplorer Events ]

		///// <summary>
		///// Event fires when a Channel in the ChannelExplorer changes from hidden to visible
		///// </summary>
		//private void ChannelExplorer_ChannelVisible(object sender, ChannelEventArgs e)
		//{
		//    _canvasWindow.CanvasPane.Refresh();
		//}

		///// <summary>
		///// Event fires when a Channel in the ChannelExplorer changes from visible to hidden
		///// </summary>
		//private void ChannelExplorer_ChannelHidden(object sender, ChannelEventArgs e)
		//{
		//    _canvasWindow.CanvasPane.Refresh();
		//}

		/// <summary>
		/// Event fires when the ChannelExplorer window is resized
		/// </summary>
		//private void ChannelExplorer_Resized(object sender, EventArgs e)
		//{
		//    //UpdateCanvasPosition();
		//}

		//#endregion [ ChannelExplorer Events ]

//        private void Settings_ClassicUI_Click(object sender, EventArgs e)
//        {
//            _workshop.UI.UseOriginalUI = true;

//#if DEBUG
//            BufferPaneForm.Hide();
//#endif

//            DialogResult = DialogResult.Retry;
//            this.Hide();
//        }

		//private void CreateColorSwatch()
		//{
		//    Bitmap Image = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
		//    using (Graphics g = Graphics.FromImage(Image))
		//    {
		//        //Color SwatchColor = (_workshop.HasActiveChannel) ? _workshop.ActiveChannel.Color : SystemColors.Control;
		//        Color SwatchColor = (_workshop.Channels.Active != null) ? _workshop.Channels.Active.Color : SystemColors.Control;
		//        SolidBrush Brush = new SolidBrush(SwatchColor);
		//        Pen Pen = new Pen(Color.Black);
		//        g.FillRectangle(Brush, 2, 2, 12, 12);
		//        g.DrawRectangle(Pen, 2, 2, 12, 12);
		//        tssColor.Image = Image;
		//        Brush.Dispose();
		//        Pen.Dispose();
		//    }
		//}

		//private void CreateUndo_Channel(string menuText)
		//{
		//    UndoData.v1 Undo = new UndoData.v1();

		//    //if ((menuText == Constants.UNDO_MOVEChannel) && _workshop.ToolSettings.MoveAllChannels)
		//    //{
		//    //    Undo.Action = Constants.UNDO_MOVEALLChannel;
		//    //    foreach (Channel Channel in _workshop.Channels)
		//    //    {
		//    //        if (Channel.IsSpecialChannel)
		//    //            continue;
		//    //        Undo.AffectedChannels.Add(Channel.Index);
		//    //        Undo.Cells.Add(Channel.ClonedCells);
		//    //    }
		//    //}
		//    //else
		//    //{
		//        Undo.Action = menuText;
		//        Undo.AffectedChannels.Add(_workshop.ActiveChannelIndex);
		//        Undo.Cells.Add(_workshop.ActiveChannel.ClonedLattice);
		//    //}
		//    Undo.GeneralEvent = GeneralDataEvent.Channel;
		//    Undo.SpecificEvent = SpecificEventType.Channel_Cells;
		//    _workshop.UndoController.Push(Undo);
		//    Undo = null;
		//}

		//private void SaveBitmap(Bitmap bitmap2save, string filename)
		//{
		//    SaveImageFileDialog.FileName = filename;
		//    string Ext = string.Empty;

		//    if (filename.Length > 0)
		//        Ext = filename.Substring(filename.Length - 3).ToLower();
		//    else
		//        Ext = "png";

		//    string[] Filters = SaveImageFileDialog.Filter.Split('|');

		//    for (int i = 0; i < Filters.Length; i++)
		//    {
		//        // there will be a matched pair for each of these elements, look at the even numbered element
		//        // Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*
		//        if (i % 2 == 0)
		//            i++;
		//        if (i >= Filters.Length)
		//            break;

		//        if (Filters[i].Replace("*.", "") == Ext)
		//        {
		//            SaveImageFileDialog.FilterIndex = (i / 2) + 1;
		//            break;
		//        }
		//    }

		//    if (SaveImageFileDialog.ShowDialog() != DialogResult.OK)
		//        return;

		//    filename = SaveImageFileDialog.FileName;
		//    ImageFormat Format;

		//    switch (SaveImageFileDialog.FilterIndex)
		//    {
		//        case 1: // Bitmap
		//            Format = ImageFormat.Bmp;
		//            filename = Path.ChangeExtension(filename, ".bmp");
		//            break;
		//        case 3: // PNG
		//            Format = ImageFormat.Png;
		//            filename = Path.ChangeExtension(filename, ".png");
		//            break;
		//        case 4: // GIF
		//            Format = ImageFormat.Gif;
		//            filename = Path.ChangeExtension(filename, ".gif");
		//            break;
		//        default:
		//            Format = ImageFormat.Jpeg;
		//            filename = Path.ChangeExtension(filename, ".jpg");
		//            break;
		//    }

		//    try
		//    {
		//        Bitmap b = new Bitmap(bitmap2save);
		//        b.Save(filename, Format);
		//    }
		//    catch
		//    {
		//        MessageBox.Show("Unable to save this file, possibly due to where it is being saved.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//        return;
		//    }

		//    MessageBox.Show("File saved.", "Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);

		//}

		//private bool _settingControlHasFocus = false;

		//private void SettingsControl_Enter(object sender, EventArgs e)
		//{
		//    _settingControlHasFocus = true;
		//}

		//private void SettingsControl_Leave(object sender, EventArgs e)
		//{
		//    _settingControlHasFocus = false;
		//}

		//private void ChildTool_Click(object sender, EventArgs e)
		//{
		//    PlugInToolStripButton Button = (PlugInToolStripButton)sender;

		//    ToolHost PTool = Button.ToolHost;
		//    PTool.ParentButton.Image = Flyout(new Bitmap(PTool.Tool.ToolBoxImage));
		//    PTool.ToolBox.Visible = false;
		//    // Update the Toolgroup button with the Text, ToolTip and Image (with flyout) of this button

		//}

		//#region [ Numeric KeyPress Events ]

		//private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		//{
		//    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
		//    {
		//        e.Handled = true;
		//    }
		//}

		//private void SignedFloatOnly_KeyPress(object sender, KeyPressEventArgs e)
		//{
		//    if (char.IsDigit(e.KeyChar) ||
		//        char.IsControl(e.KeyChar) ||
		//        (e.KeyChar == '-') ||
		//        (e.KeyChar == '+') ||
		//        (e.KeyChar == '.') ||
		//        (e.KeyChar == '°'))
		//    {
		//        // We like these, do nothing
		//        e.Handled = false;
		//    }
		//    else
		//        e.Handled = true;
		//}

		//private void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		//{
		//    if (char.IsDigit(e.KeyChar) ||
		//        char.IsControl(e.KeyChar) ||
		//        (e.KeyChar == '-') ||
		//        (e.KeyChar == '+'))
		//    {
		//        // We like these, do nothing
		//        e.Handled = false;
		//    }
		//    else
		//        e.Handled = true;
		//}

		//#endregion [ Numeric KeyPress Events ]

		//private ToolStripButton GetButtonForTool()
		//{
		//    return GetButtonForTool(_workshop.CurrentTool);
		//}

		//private ToolStripButton GetButtonForTool(Tool tool)
		//{
		//    return GetButtonForTool((int)tool);
		//}

		//private ToolStripButton GetButtonForTool(int tool)
		//{
		//    switch (tool)
		//    {
		//        case (int)Tool.MultiChannelLine:
		//            return MultiChannelLineTool;

		//        case (int)Tool.Crop:
		//            return CropTool;

		//        case (int)Tool.Ellipse:
		//            return EllipseTool;

		//        case (int)Tool.EllipseMask:
		//            return EllipseMask;

		//        case (int)Tool.Erase:
		//            return EraserTool;

		//        case (int)Tool.Fill:
		//            return FillTool;

		//        case (int)Tool.FreehandMask:
		//            return FreehandMask;

		//        case (int)Tool.Icicles:
		//            return IcicleTool;

		//        case (int)Tool.ImageStamp:
		//            return ImageStampTool;

		//        case (int)Tool.Line:
		//            return LineTool;

		//        case (int)Tool.Mask:
		//            return MaskTool;

		//        case (int)Tool.MegaTree:
		//            return MegatreeTool;

		//        case (int)Tool.MoveChannel:
		//            return MoveChannelTool;

		//        case (int)Tool.MultiChannelTool:
		//            return MultiChannelTool;

		//        case (int)Tool.Paint:
		//            return PaintTool;

		//        case (int)Tool.PaintMask:
		//            return PaintMask;

		//        case (int)Tool.Polygon:
		//            return PolygonTool;

		//        case (int)Tool.Rectangle:
		//            return RectangleTool;

		//        case (int)Tool.RectangleMask:
		//            return RectangleMask;

		//        case (int)Tool.Shape:
		//            return ShapeTool;

		//        case (int)Tool.SingingFace:
		//            return SingingFaceTool;

		//        case (int)Tool.Spray:
		//            return SprayTool;

		//        case (int)Tool.Text:
		//            return TextTool;

		//        case (int)Tool.Zoom:
		//            return ZoomTool;

		//        default:
		//            if (_workshop.CurrentPlugInTool != null)
		//                return _workshop.CurrentPlugInTool.Button;
		//            else
		//                return null;
		//        //if (!Workshop.IsPlugInTool(tool))
		//        //    return null;
		//        //else
		//        //{
		//        //    int Index = tool - (int)Tool.PlugIn;
		//        //    return _pluginTools[Index].ToolBoxButton;
		//        //}
		//    }
		//}

		//private ToolStripItem GetButtonForTool(string toolName)
		//{
		//    // Search the ToolBox for a tool with this name
		//    foreach (ToolStripItem Item in ToolBox_Main.Items)
		//    {
		//        if (Item.Name == toolName)
		//            return Item;
		//    }

		//    // Search each SubMenu toolstrips 
		//    foreach (KeyValuePair<string, ToolStrip> KVP in _subMenus)
		//    {
		//        foreach (ToolStripItem Item in KVP.Value.Items)
		//        {
		//            if (Item.Name == toolName)
		//                return Item;
		//        }
		//    }

		//    return null;
		//}

		///// <summary>
		///// Determines the location of a ToolStripItem, since we cannot determine the location from a property on the control.
		///// </summary>
		///// <param name="button">ToolStripItem to examine</param>
		//private Point GetLocationOfTool(ToolStripItem button, int offsetX, int offsetY)
		//{
		//    ToolStrip Parent = ToolBox_Main; // button.GetCurrentParent();
		//    Point Location = Parent.Location;
		//    int Padding = Parent.Padding.Top + Parent.Padding.Bottom;
		//    int Y = Location.Y + Parent.Padding.Top;
		//    foreach (ToolStripItem Item in Parent.Items)
		//    {
		//        if (Item == button)
		//            break;
		//        Y += Item.Size.Height + Item.Margin.Top + Item.Margin.Bottom;
		//    }

		//    return new Point(Location.X + Parent.Padding.Left + offsetX, Y + offsetY);
		//}

		///// <summary>
		///// Determines the location of a ToolStripItem, since we cannot determine the location from a property on the control.
		///// </summary>
		///// <param name="button">ToolStripItem to examine</param>
		//private Point GetLocationOfTool(ToolStripItem button)
		//{
		//    return GetLocationOfTool(button, 0, 0);
		//}

		///// <summary>
		///// Find the SubMenu ToolStrip control for the Tool with the name passed in.
		///// </summary>
		///// <param name="toolName"></param>
		///// <returns></returns>
		//private ToolStrip GetSubMenuToolStrip(string toolName)
		//{
		//    if (_subMenus.Keys.Contains(toolName))
		//        return _subMenus[toolName];
		//    else
		//    {
		//        // Create a new toolstrip submenu for this toolName
		//        ToolStrip SubMenu = new ToolStrip();

		//        //SubMenu.Visible = false;
		//        SubMenu.Dock = System.Windows.Forms.DockStyle.None;
		//        SubMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
		//        SubMenu.Name = "SubMenu_" + toolName.Replace(" ", string.Empty);
		//        SubMenu.Padding = new System.Windows.Forms.Padding(3);
		//        SubMenu.Size = new System.Drawing.Size(121, 29);

		//        //ToolStripItem ToolButton = GetButtonForTool(toolName);
		//        //if (ToolButton != null)
		//        //{
		//        //    SubMenu.Location = GetLocationOfTool(ToolButton);
		//        //    SubMenu.Location.Offset(ToolBox_Main.Width, 0);
		//        //}
		//        //else
		//            SubMenu.Location = SubMenu_ShapeTool.Location;

		//        this.Controls.Add(SubMenu);
		//        _subMenus.Add(toolName, SubMenu);

		//        return SubMenu;
		//    }
		//}

		/*
		private void CreateTools()
		{
			int ID = 1;
			PlugIn.ToolHost Host = null;

			Tools.GenericTool Tool = new Tools.GenericTool();
			Tool.Name = "Paintbrush";
			Tool.UndoText = Constants.UNDO_PAINT;
			Tool.ToolTip = Tool.Name;
			Tool.ToolBoxImage = ElfRes.paint_tool;
			Tool.ToolStrip = this.ToolStrip_Paint;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = PaintTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "Spray Brush";
			Tool.UndoText = Constants.UNDO_SPRAY;
			Tool.ToolTip = Tool.Name;
			Tool.ToolBoxImage = ElfRes.spray;
			Tool.ToolStrip = this.ToolStrip_Spray;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = SprayTool;
			_workshop.UI.Tools.Add(Host);
			
			Tool = new Tools.GenericTool();
			Tool.Name = "Eraser";
			Tool.UndoText = Constants.UNDO_ERASE;
			Tool.ToolTip = Tool.Name;
			Tool.ToolBoxImage = ElfRes.eraser;
			Tool.ToolStrip = this.ToolStrip_Paint;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = EraserTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "ShapeTool";
			Tool.IsToolGroup = true;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = ShapeTool;
			Host.SubMenu = this.SubMenu_ShapeTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "MaskTool";
			Tool.IsToolGroup = true;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = MaskTool;
			Host.SubMenu = this.SubMenu_MaskTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "MultiChannelTool";
			Tool.IsToolGroup = true;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = MultiChannelTool;
			Host.SubMenu = this.SubMenu_MultiChannelTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "ImageStampTool";
			Tool.UndoText = Constants.UNDO_IMAGESTAMP;
			Tool.ToolTip = "Image Stamp";
			Tool.ToolBoxImage = ElfRes.imagestamp;
			Tool.ToolStrip = this.ToolStrip_ImageStamp;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = ImageStampTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "Text";
			Tool.UndoText = Constants.UNDO_TEXT;
			Tool.ToolTip = "Text";
			Tool.ToolBoxImage = ElfRes.text;
			Tool.ToolStrip = this.ToolStrip_Text;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = TextTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "Fill";
			Tool.UndoText = Constants.UNDO_FLOODFILL;
			Tool.ToolTip = "Flood Fill";
			Tool.ToolBoxImage = ElfRes.fill_tool;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = FillTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "MoveChannelTool";
			Tool.UndoText = Constants.UNDO_MOVEChannel;
			Tool.ToolTip = "Move Channel";
			Tool.ToolBoxImage = ElfRes.pan_tool;
			Tool.ToolStrip = this.ToolStrip_MoveChannel;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = MoveChannelTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "ZoomTool";
			Tool.UndoText = Constants.UNDO_ZOOM;
			Tool.ToolTip = "Zoom";
			Tool.ToolBoxImage = ElfRes.zoom;
			Tool.ToolStrip = this.ToolStrip_Zoom;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = ZoomTool;
			_workshop.UI.Tools.Add(Host);

			Tool = new Tools.GenericTool();
			Tool.Name = "CropTool";
			Tool.UndoText = Constants.UNDO_CROP;
			Tool.ToolTip = "Crop";
			Tool.ToolBoxImage = ElfRes.crop;
			Host = new PlugIn.ToolHost(Tool, ID++);
			Host.Home = this.ToolBox_Main;
			Host.Button = CropTool;
			_workshop.UI.Tools.Add(Host);

			// populate the submenus
		}
		*/

		//private PlugInToolStripButton GetToolButton(ToolHost pTool)
		//{
		//    PlugInToolStripButton Button = new PlugInToolStripButton();

		//    Button.CheckOnClick = true;
		//    Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		//    Button.Name = pTool.ToString() + "_Tool";
		//    Button.Size = new System.Drawing.Size(30, 20);
		//    Button.Text = pTool.ToString();
		//    Button.Click += new System.EventHandler(this.Tool_Click);

		//    Button.Tag = pTool;
		//    //Button.Tag = (int)Tool.PlugIn + pTool.ID;
		//    Button.ToolTipText = pTool.Tool.ToolTipText;

		//    //if (pTool.Tool.IsToolGroup)
		//    //{
		//    //    if (pTool.Tool.ToolBoxImage != null)
		//    //        Button.Image = Flyout(new Bitmap(pTool.Tool.ToolBoxImage));
		//    //    else
		//    //        Button.Image = Flyout(new Bitmap(ElfRes.not));
		//    //}
		//    //else
		//    //{
		//    //    if (pTool.Tool.ToolBoxImage != null)
		//    //        Button.Image = pTool.Tool.ToolBoxImage;
		//    //    else
		//    //        Button.Image = ElfRes.not;
		//    //}

		//    return Button;
		//}

		//private ToolStrip CreateChildToolStrip(ToolHost pTool)
		//{
		//    ToolStrip ChildToolStrip = new ToolStrip();

		//    ChildToolStrip.Dock = System.Windows.Forms.DockStyle.None;
		//    ChildToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;

		//    //ChildToolStrip.Location = GetLocationOfTool(pTool.Button);
		//    //ChildToolStrip.Location.Offset(ToolBox_Main.Width, 0);

		//    ChildToolStrip.Name = "SubMenu_" + pTool.Tool.Name.Replace(" ", string.Empty).Replace("-", string.Empty);
		//    ChildToolStrip.Padding = new System.Windows.Forms.Padding(3);
		//    ChildToolStrip.Size = new System.Drawing.Size(129, 29);
		//    ChildToolStrip.MouseLeave += new System.EventHandler(this.SubMenu_MouseLeave);
		//    //ChildToolStrip.Visible = false;

		//    this.Controls.Add(ChildToolStrip);
		//    return ChildToolStrip;
		//}

		//private Workshop Workshop
		//{
		//    get { return _workshop; }
		//    set
		//    {
		//        if (value == null)
		//            return;
		//        _workshop.ActiveChannelIndex = value.ActiveChannelIndex;
		//        _workshop.UI.BackgroundImage = value.UI.BackgroundImage;
		//        _workshop.UI.BackgroundImage_Filename = value.UI.BackgroundImage_Filename;
		//        _workshop.UI.BackgroundImage_Brightness = value.UI.BackgroundImage_Brightness;
		//        _workshop.UI.LatticeSize = value.UI.LatticeSize;
		//        _workshop.UI.CellSize = value.UI.CellSize;
		//        _workshop.Channels = value.Channels;
		//        _workshop.UI.RespectChannelOutputsDuringPlayback = value.UI.RespectChannelOutputsDuringPlayback;
		//    }
		//}


		///// <summary>
		///// Inform the currently selected tool to accound for this UI change.
		///// </summary>									
		//private void CurrentToolUIChanged()
		//{
		//    if (_workshop.CurrentTool != null)
		//        _workshop.CurrentTool.Tool.Selected();
		//}


		#endregion [ DEAD CODE ]

	}
}