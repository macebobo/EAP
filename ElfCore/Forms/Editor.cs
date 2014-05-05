using ElfControls;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.PlugIn;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class Editor : Form
	{
		#region [ Constants ]

		private const string LAST_TOOL = "LastToolUsed";

		private const string CHANNEL_EXPLORER_WIDTH = Constants.SAVE_PATH_DELIMITER + "ChannelExplorerWidth";

		#endregion [ Constants ]

		#region [ Private Variables ]

		/// <summary>
		/// Workshop is a Singleton type of class. Simply getting the Instance variable from the Static object will get the object already loaded with our data
		/// </summary>
		private Workshop _workshop = Workshop.Instance;
		private Settings _settings = Settings.Instance;

		private ToolStripPanel _toolStripPanel = null;
		private ToolStrip _currentTool_ToolStrip = null;

		private ChannelExplorerTree _channelExplorer = null;
		private XmlHelper _xmlHelper = XmlHelper.Instance;
		private ToolStrip _flyoutToolStrip = null;
		private MruStripMenu _mruMenu;
		private List<ToolStripMenuItem> _menuCellSizes = null;
		private bool _suspendKeyboardEvents = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden()]
		public BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Editor()
		{
			ProfileSplash Splash = null;
			
			if (_workshop.RunMode == RunMode.Standalone)
			{
				//Splash = new ProfileSplash(true);
				//Splash.Show();
				//Application.DoEvents();
			}

			InitializeComponent();

			_toolStripPanel = toolStripContainer1.TopToolStripPanel;

			ConfigureRunMode();
			
			_mruMenu = new MruStripMenu(File_Recent, OnMruFile_Data);
			_mruMenu.LoadSettings();

			BuildMenuImages();
			AttachEvents();

			_channelExplorer = new ChannelExplorerTree(this);
			
			#if DEBUG
				_workshop.BufferPaneForm = new ViewBufferPane(this);
				_workshop.BufferPaneForm.Show();
				SetPaneMenuTags();
				PanesMenu.Visible = true;
			#else
				PanesMenu.Visible = false;
			#endif

			LoadPlugIns();

			// Load in the remembered last settings
			LoadSettings();
			if (_workshop.KeyboardController.CurrentGestures.Count == 0)
				MessageBox.Show("No Keyconfig");

			SaveProfileDialog.Filter = OpenProfileDialog.Filter = _workshop.AvailableProfiles.FilterList;

			_channelExplorer.Show(DockPanel);
			UpdateControls_Profile();

			if ((_workshop.RunMode == RunMode.PlugIn) && (Profile != null))
			{
				Profiles_Added(this, new ProfileEventArgs(Profile));
				_workshop.ProfileController.FireSwitched();

				// Call the select event for the current tool to let it know there is a Profile now.
				_workshop.CurrentTool.Tool.OnSelected();
			}
		
			Activate();
			if (Splash != null)
			{
				Splash.Close();
				Splash = null;
			}
		}

		public Editor(string[] args)
			: this()
		{
			// Check to see if we have opened with a file association click.
			if (_workshop.RunMode == RunMode.Standalone)
			{
				if ((args != null) && (args.Length > 0))
				{
					_workshop.ProfileController.Load(args);
				}
				else
					Profiles_Switched(null, new ProfileEventArgs(null));
			}
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#region [ ToolBox Methods ]

		/// <summary>
		/// Configures the form and the DockPanel to either run as a Single Document Interface (PlugIn mode), or as a
		/// Multiple Document Interface (stand-alone mode)
		/// </summary>
		private void ConfigureRunMode()
		{
			if (_workshop.RunMode == RunMode.Standalone)
			{
				AllowDrop = true;
				IsMdiContainer = true;
				File_SaveAndExit.Visible = false;
				DockPanel.DocumentStyle = DocumentStyle.DockingMdi;
				WindowMenu.Visible = true;
			}
			else if (_workshop.RunMode == RunMode.PlugIn)
			{
				AllowDrop = false;
				IsMdiContainer = false;
				DockPanel.DocumentStyle = DocumentStyle.DockingSdi;
				WindowMenu.Visible = false;

				// Hide the standalone menu items under File
				File_New.Visible = false;
				File_Open.Visible = false;
				File_Sep1.Visible = false;
				File_Close.Visible = false;
				File_Revert.Visible = false;
				File_SaveAs.Visible = false;
				File_Recent.Visible = false;
				File_Sep4.Visible = false;

				// Hide Channel list functionality and some channel functionality
				Channel_AddNew.Visible = false;
				Channel_Delete.Visible = false;
				Channel_Sep1.Visible = false;
				Channel_Arrange.Visible = false;
				Channel_Enable.Visible = false;
				Channel_Disable.Visible = false;
				Channel_Display_Sep2.Visible = false;
				Channel_Display_Sep3.Visible = false;
				Channel_Include.Visible = false;
				Channel_Exclude.Visible = false;
				Channel_Rename.Visible = false;
				Channel_ChangeSeqColor.Visible = false;
			}
		}

		/// <summary>
		/// Load up all the Tools derived from PlugIns
		/// </summary>
		private void LoadPlugIns()
		{
			_workshop.AvailableProfiles = PlugInController.PlugInProfileList;
			_workshop.ToolGroups = PlugInController.PlugInToolGroupList;
			_workshop.Tools = PlugInController.PlugInToolList;

			if (_workshop.Tools == null)
				return;

			int MinID = Int32.MaxValue;
			int MaxID = Int32.MinValue;

			// Go through all the Tools in the list that are not in ToolGroups, create their buttons and assign the button's click delegate. If the tool has
			// a control toolstrip, add it to the hidden panel control.
			foreach (PlugInTool pTool in _workshop.Tools.WhereList(false))
			{
				pTool.Tool.OperationCompleted += Tool_OperationCompleted;
				pTool.Button.Click += Tool_Click;
				pTool.Tool.Selected += Tool_Selected;
				pTool.Initialize();
				MinID = Math.Min(MinID, pTool.ID);
				MaxID = Math.Max(MaxID, pTool.ID);

				// If this tool has a ToolStrip that holds settings for it, add that control to the hidden panel on this form.
				if (pTool.SettingsToolStrip != null)
					pnlHoldToolStrips.Controls.Add(pTool.SettingsToolStrip);
			}

			// Loop through the ToolGroups, setting up their child tools, etc.
			foreach (PlugInToolGroup pToolGroup in _workshop.ToolGroups)
			{
				MinID = Math.Min(MinID, pToolGroup.ID);
				MaxID = Math.Max(MaxID, pToolGroup.ID);

				// Create a button for this ToolGroup. This button will initially have to image or tooltip, as there are no child tools yet.
				// Assign the button delegate.
				// Note, this button is created in the property_get method.
				pToolGroup.Button.Click += Tool_Click;
				pToolGroup.Button.MouseDown += ToolGroup_MouseDown;
				pToolGroup.Button.MouseUp += ToolGroup_MouseUp;

				// Create the toolstrip control for this ToolGroup and add it to Editor's list of controls.
				Controls.Add(pToolGroup.ChildToolBox);

				// Sort all the Tools into their various ToolGroups, and initialize them like their non-grouped siblings
				foreach (PlugInTool pTool in _workshop.Tools.WhereList(pToolGroup.Name))
				{
					pToolGroup.ToolGroup.Add(pTool);
					pTool.Tool.OperationCompleted += Tool_OperationCompleted;
					pTool.Tool.Selected += Tool_Selected;
					pTool.Initialize();
					pTool.ParentButton = pToolGroup.Button;
					pTool.ToolBox = pToolGroup.ChildToolBox;
					pToolGroup.ChildToolBox.Items.Add(pTool.Button);
				}
				pToolGroup.Initialize();
			}

			// Now that all the Tools and ToolGroups are roughly initialized, we need to sort them by their ID order, and add the appropriate buttons
			// to the main ToolBox.
			// Sort them by looping from the minimum ID to the maximum ID. If the number value equals one or the other, add the button to the main ToolBox.
			// If the ID is the same between them, add the tool first, then the toolgroup.
			for (int i = MinID; i <= MaxID; i++)
			{
				//var Tool = _workshop.Tools.Where(t => t.InToolGroup == false && t.ID == i).FirstOrDefault();
				var Tool = _workshop.Tools.Where(false, i);
				if (Tool != null)
				{
					ToolBox_Main.Items.Add(Tool.Button);
					Tool.ToolBox = ToolBox_Main;
					Tool.Index = ToolBox_Main.Items.Count - 1;
				}

				//var ToolGroup = _workshop.ToolGroups.Where(t => t.ID == i).FirstOrDefault();
				var ToolGroup = _workshop.ToolGroups.Where(i);
				if (ToolGroup != null)
				{
					ToolBox_Main.Items.Add(ToolGroup.Button);
					ToolGroup.ToolBox = ToolBox_Main;
					ToolGroup.Index = ToolBox_Main.Items.Count - 1;
				}
			}

			// Now that all the proper tools and ToolGroups are installed on the main ToolBox, go through the ToolGroups and position their child ToolBoxes so they
			// line up to the buttons
			foreach (PlugInToolGroup pToolGroup in _workshop.ToolGroups)
			{
				pToolGroup.LineUpToolBox();
			}
		}

		#endregion [ ToolBox Methods ]

		/// <summary>
		/// Loops through the list of the current keyboard gestures and updates the menu and toolbar items therein with the currently
		/// assign multikey gestures.
		/// </summary>
		private void AssignKeyboardShortcuts()
		{
			ToolStripItem Control = null;
			Keys MultiGestureKey1 = Keys.None;
			Keys MultiGestureKey2 = Keys.None;

			// Go Through all the entries and update the menu and toolbar objects with their shortcut indicators, if any.
			foreach (KeyValuePair<string, MultiKeyGesture> KVP in _workshop.KeyboardController.CurrentGestures)
			{
				Control = KVP.Value.MenuItem;
				KeyboardController.StringToKeys(KVP.Value.Gesture, out MultiGestureKey1, out MultiGestureKey2);

				if (Control is ToolStripButtonWithKeys)
				{
					((ToolStripButtonWithKeys)Control).MultiGestureKey1 = MultiGestureKey1;
					((ToolStripButtonWithKeys)Control).MultiGestureKey2 = MultiGestureKey2;
				}
				else if (Control is ToolStripMenuItemWithKeys)
				{
					((ToolStripMenuItemWithKeys)Control).MultiGestureKey1 = MultiGestureKey1;
					((ToolStripMenuItemWithKeys)Control).MultiGestureKey2 = MultiGestureKey2;
				}
			}
		}

		/// <summary>
		/// Attach the menu events as well as some Workshop events.
		/// </summary>
		private void AttachEvents()
		{
			Channel_Properties.Click += Channel_Properties_Click;
			Channel_AddNew.Click += Channel_AddNew_Click;
			Channel_Delete.Click += Channel_Delete_Click;
			Channel_Clear.Click += Channel_Clear_Click;
			Channel_ClearAll.Click += Channel_Clear_AllChannels_Click;
			Channel_LoadFromBitmap.Click += Channel_LoadFromBitmap_Click;
			Channel_SaveToBitmap.Click += Channel_SaveToBitmap_Click;
			Channel_Import.Click += Channel_Import_Click;
			Channel_SetAsBackground.Click += Channel_SetAsBackground_Click;
			Channel_Rename.Click += Channel_Rename_Click;
			Channel_ChangeRendColor.Click += Channel_ChangeRendColor_Click;
			Channel_ChangeSeqColor.Click += Channel_ChangeSeqColor_Click;
			Channel_Hide.Click += Channel_Hide_Click;
			Channel_HideOthers.Click += Channel_HideOthers_Click;
			Channel_Show.Click += Channel_Show_Click;
			Channel_ShowAll.Click += Channel_ShowAll_Click;
			Channel_Enable.Click += Channel_Enable_Click;
			Channel_Disable.Click += Channel_Disable_Click;
			Channel_Lock.Click += Channel_Lock_Click;
			Channel_Unlock.Click += Channel_Unlock_Click;
			Channel_Include.Click += Channel_Include_Click;
			Channel_Exclude.Click += Channel_Exclude_Click;
			Channel_Group.Click += Channel_Group_Click;
			Channel_Ungroup.Click += Channel_Ungroup_Click;
			Channel_MoveToTop.Click += Channel_MoveToTop_Click;
			Channel_MoveUp.Click += Channel_MoveUp_Click;
			Channel_MoveDown.Click += Channel_MoveDown_Click;
			Channel_MoveToBottom.Click += Channel_MoveToBottom_Click;
			Channel_Shuffle_New.Click += Channel_Shuffle_New_Click;
			Channel_Shuffle_Edit.Click += Channel_Shuffle_Edit_Click;
			Channel_Shuffle_Delete.Click += Channel_Shuffle_Delete_Click;

			_workshop.Clipboard.Changed += Clipboard_Changed;

			_workshop.ProfileController.Added += Profiles_Added;
			_workshop.ProfileController.Removed += Profiles_Removed;
			_workshop.ProfileController.Switched += Profiles_Switched;
			_workshop.UI.PropertyChanged += UI_PropertyChanged;

			_workshop.KeyboardController.MessageOccurred += Controller_Message;
			_workshop.KeyboardController.MultiKeyGestureOccurred += KeyboardController_MultiKeyGestureOccurred;
		}

		/// <summary>
		/// Scan through the eligible menu items and toolbars and read in their design-time values for multikey gestures.
		/// </summary>
		private void BuildDefaultKeyboardShortcuts()
		{
			_workshop.KeyboardController.AttachToolStrip(MainMenu, "Menu");
			_workshop.KeyboardController.AttachToolStrip(ToolBox_Main, "ToolBox");
			_workshop.KeyboardController.AssignToDefault();
		}

		/// <summary>
		/// Construct the custom images for the menu items, such as adding the Create annotation image to the Profile image for the New Profile menu item.
		/// </summary>
		private void BuildMenuImages()
		{
			File_Close.Image = ImageHandler.AddAnnotation(ElfRes.profile, Annotation.Close, AnchorStyles.Top | AnchorStyles.Right);
			File_SaveAs.Image = ImageHandler.AddAnnotation(ElfRes.save, Annotation.As, AnchorStyles.Bottom | AnchorStyles.Left);
			File_Revert.Image = ImageHandler.AddAnnotation(ElfRes.save, Annotation.Undo, AnchorStyles.Top | AnchorStyles.Right);
			Channel_Import.Image = ImageHandler.AddAnnotation(ElfRes.profile, Annotation.Import, new Point(3,0));
			Channel_Delete.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Delete, new Point(3, 0));
			Channel_ClearAll.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.ClearStar, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_Shuffle_Delete.Image = ImageHandler.AddAnnotation(ElfRes.shuffle, Annotation.Delete, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_Shuffle_Edit.Image = ImageHandler.AddAnnotation(ElfRes.shuffle, Annotation.Edit, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_Include.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Include, new Point(3,0));
			Channel_Exclude.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Exclude, new Point(3,0));
			Channel_SaveToBitmap.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Save, new Point(3, 0));
			Channel_ChangeRendColor.Image = ImageHandler.AddAnnotation(ElfRes.palette, Annotation.Monitor, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_ChangeSeqColor.Image = ImageHandler.AddAnnotation(ElfRes.palette, Annotation.Grid, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_Unlock.Image = ImageHandler.AddAnnotation(ElfRes._lock, Annotation.Clear, AnchorStyles.Bottom | AnchorStyles.Right);
			Channel_Hide.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Invisible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-1, 0));
			Channel_HideOthers.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.Invisible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-2, 0));
			Channel_Show.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Visible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-1, 0));
			Channel_ShowAll.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.Visible, AnchorStyles.Bottom | AnchorStyles.Right, new Point(-2, 0));
			Channel_Enable.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Complete, new Point(3, 0));
			Channel_Disable.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Error, new Point(3, 0));
		}

		/// <summary>
		/// Updates the items in the Windows menu, making only the item associated with the Active Profile checked.
		/// </summary>
		private void CheckActiveProfileWindowMenuItem()
		{
			foreach (ToolStripMenuItem Menu in WindowMenu.DropDownItems)
				Menu.Checked = (ReferenceEquals(Menu.Tag, Profile));
		}

		/// <summary>
		/// Detach the menu events as well as some Workshop events.
		/// </summary>
		private void DetachEvents()
		{
			// Detach from menus
			Channel_Properties.Click -= Channel_Properties_Click;
			Channel_AddNew.Click -= Channel_AddNew_Click;
			Channel_Delete.Click -= Channel_Delete_Click;
			Channel_Clear.Click -= Channel_Clear_Click;
			Channel_ClearAll.Click -= Channel_Clear_AllChannels_Click;
			Channel_LoadFromBitmap.Click -= Channel_LoadFromBitmap_Click;
			Channel_SaveToBitmap.Click -= Channel_SaveToBitmap_Click;
			Channel_Import.Click -= Channel_Import_Click;
			Channel_SetAsBackground.Click -= Channel_SetAsBackground_Click;
			Channel_Rename.Click -= Channel_Rename_Click;
			Channel_ChangeRendColor.Click -= Channel_ChangeRendColor_Click;
			Channel_ChangeSeqColor.Click -= Channel_ChangeSeqColor_Click;
			Channel_Hide.Click -= Channel_Hide_Click;
			Channel_HideOthers.Click -= Channel_HideOthers_Click;
			Channel_Show.Click -= Channel_Show_Click;
			Channel_ShowAll.Click -= Channel_ShowAll_Click;
			Channel_Enable.Click -= Channel_Enable_Click;
			Channel_Disable.Click -= Channel_Disable_Click;
			Channel_Lock.Click -= Channel_Lock_Click;
			Channel_Unlock.Click -= Channel_Unlock_Click;
			Channel_Include.Click -= Channel_Include_Click;
			Channel_Exclude.Click -= Channel_Exclude_Click;
			Channel_Group.Click -= Channel_Group_Click;
			Channel_Ungroup.Click -= Channel_Ungroup_Click;
			Channel_MoveToTop.Click -= Channel_MoveToTop_Click;
			Channel_MoveUp.Click -= Channel_MoveUp_Click;
			Channel_MoveDown.Click -= Channel_MoveDown_Click;
			Channel_MoveToBottom.Click -= Channel_MoveToBottom_Click;
			Channel_Shuffle_New.Click -= Channel_Shuffle_New_Click;
			Channel_Shuffle_Edit.Click -= Channel_Shuffle_Edit_Click;
			Channel_Shuffle_Delete.Click -= Channel_Shuffle_Delete_Click;

			// If there still is a profile present, uncouple from it.
			if (Profile != null)
			{
				Profile.Canvas_MouseEnter -= Canvas_MouseEnter;
				Profile.Canvas_MouseLeave -= Canvas_MouseLeave;
				Profile.ScalingChanged -= Profile_ScalingChanged;
				Profile.DirtyChanged -= Profile_DirtyChanged;
				Profile.ChannelPropertyChanged -= Profile_ChannelPropertyChanged;
				Profile.ChannelsSelected -= Profile_ChannelsSelected;
				Profile.Mask_Defined -= Mask_DefinedOrCleared;
				Profile.Mask_Cleared -= Mask_DefinedOrCleared;
				Profile.Undo_Changed -= Undo_Changed;
				Profile.Redo_Changed -= Redo_Changed;
				Profile.Undo_Completed -= Undo_Completed;	
			}

			// Detach from the Workshop
			_workshop.Clipboard.Changed -= Clipboard_Changed;
			_workshop.ProfileController.Added -= Profiles_Added;
			_workshop.ProfileController.Removed -= Profiles_Removed;
			_workshop.ProfileController.Switched -= Profiles_Switched;
			_workshop.UI.PropertyChanged -= UI_PropertyChanged;

			_workshop.KeyboardController.MessageOccurred -= Controller_Message;
			_workshop.KeyboardController.MultiKeyGestureOccurred -= KeyboardController_MultiKeyGestureOccurred;

			// Detach from the Tools, ToolGroups and their child tools
			foreach (PlugInTool pTool in _workshop.Tools.WhereList(false))
			{
				pTool.Tool.OperationCompleted -= Tool_OperationCompleted;
				pTool.Button.Click -= Tool_Click;
				pTool.Tool.Selected -= Tool_Selected;
			}

			foreach (PlugInToolGroup pToolGroup in _workshop.ToolGroups)
			{
				pToolGroup.Button.Click -= Tool_Click;
				pToolGroup.Button.MouseDown -= ToolGroup_MouseDown;
				pToolGroup.Button.MouseUp -= ToolGroup_MouseUp;

				foreach (PlugInTool pTool in _workshop.Tools.WhereList(pToolGroup.Name))
				{
					pTool.Tool.OperationCompleted -= Tool_OperationCompleted;
					pTool.Tool.Selected -= Tool_Selected;
				}
			}
		}

		/// <summary>
		/// If the menu is checked, then superimposes the check annotation image over the passed in image.
		/// </summary>
		private Bitmap GetMenuCheckedImage(Bitmap menuImage, bool? isChecked)
		{
			return GetMenuCheckedImage(menuImage, (isChecked ?? false));
		}

		/// <summary>
		/// If the menu is checked, then superimposes the check annotation image over the passed in image.
		/// </summary>
		private Bitmap GetMenuCheckedImage(Bitmap menuImage, bool isChecked)
		{
			if (isChecked)
				return ImageHandler.AddAnnotation(menuImage, Annotation.Check);
			else
				return menuImage;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/6469027/call-methods-using-names-in-c-sharp
		/// Calls a method within this form by name. Used for the shortcuts.
		/// </summary>
		/// <param name="methodName">Name of the method to invoke</param>
		/// <param name="args">Parameters to use with the method invocation.</param>
		public void InvokeMethod(string methodName, List<object> args)
		{
			GetType().GetMethod(methodName).Invoke(this, args.ToArray());
		}

		/// <summary>
		/// Brings up the Open File dialog to import a bitmap. Called from Channel_LoadFromBitmap_Click()
		/// </summary>
		/// <param name="filename">Output parameter, returns the filename chosen.</param>
		/// <returns>Bitmap that was loaded. If the dialog was canceled, returns null.</returns>
		private Bitmap LoadBitmap(out string filename)
		{
			filename = string.Empty;
			
			if (OpenImageFileDialog.ShowDialog() != DialogResult.OK)
				return null;

			Bitmap bmp = ImageHandler.LoadBitmapFromFile(OpenImageFileDialog.FileName);
			if (bmp == null)
			{
				MessageBox.Show("Unable to load this file.", "Load Image File", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return null;
			}

			filename = OpenImageFileDialog.FileName;
			return bmp;
		}

		/// <summary>
		/// Loads the window position/state values from the _settings object.
		/// </summary>
		private void LoadSettings()
		{
			Left = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, Left);
			Top = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, Top);
			Width = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, Width);
			Height = _settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, Height);
			WindowState = EnumHelper.GetEnumFromValue<FormWindowState>(_settings.GetValue(Constants.SETUP_DIALOG + Constants.WINDOW_STATE, (int)FormWindowState.Normal));
			DockPanel.DockRightPortion = _settings.GetValue(Constants.SETUP_DIALOG + CHANNEL_EXPLORER_WIDTH, DockPanel.DockRightPortion);

			int SelectedToolID = _settings.GetValue(LAST_TOOL,  (int)ToolID.Paint);
			_workshop.CurrentTool = _workshop.GetPlugInTool(SelectedToolID);
			if (_workshop.CurrentTool == null)
				_workshop.CurrentTool = _workshop.Tools[0];
			if (_workshop.CurrentTool != null)
				_workshop.CurrentTool.Tool.IsSelected = true;
			
			BuildDefaultKeyboardShortcuts();
			_workshop.KeyboardController.LoadSettings();
			AssignKeyboardShortcuts();
		}

		/// <summary>
		/// Opens the Profile specified.
		/// </summary>
		/// <param name="filename">Name of the file that contains the Profile.</param>
		private void OpenProfiles(string filename)
		{
			List<string> Filenames = new List<string>();
			Filenames.Add(filename);
			OpenProfiles(Filenames);
			Filenames = null;
		}

		/// <summary>
		/// Opens the Profiles specified in the list.
		/// </summary>
		/// <param name="fileNames">List of filenames to open</param>
		private void OpenProfiles(List<string> fileNames)
		{
			Cursor = Cursors.WaitCursor;
			DialogResult Result;

			foreach (string Filename in fileNames)
			{
				// Check to see if this profile is already open. If so, ask the user if they intended to revert it back to the saved version
				foreach (BaseProfile Profile in _workshop.ProfileController.List)
				{
					if (string.Compare(Profile.Filename, Filename, true) == 0)
					{
						if (Profile.Dirty)
						{
							Cursor = Cursors.Default;
							Result = MessageBox.Show("The Profile \"" + Profile.Name + "\" has already been opened. Did you want to revert it back to it's saved version?", "Open Profile", MessageBoxButtons.YesNo);
							Cursor = Cursors.WaitCursor;

							if (Result == DialogResult.No)
								continue;
						}
						// Close out this existing profile.
						Profile.SuppressEvents = true;
						_workshop.ProfileController.Remove(Profile);
						break;
					}
				}

				if (_workshop.ProfileController.Load(Filename) != null)
				{
					_mruMenu.AddFile(Filename);
					_settings.Save();
				}
				else
				{
					Cursor = Cursors.Default;
					MessageBox.Show(string.Format("Unable to load the Profile \"{0}\".", Filename), "Open Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			Cursor = Cursors.Default;

			// Call the select event for the current tool to let it know there is a Profile now.
			_workshop.CurrentTool.Tool.OnSelected();

			File_Revert.Enabled = false;
		}
		
		/// <summary>
		/// Informs the various objects (including this form) to save their settings to the Settings document
		/// </summary>
		private void SaveSettings()
		{
			//Preview.WriteTraceMessage("Editor.SaveSettings START", TraceLevel.Verbose);

			if (WindowState != FormWindowState.Minimized)
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_STATE, (int)WindowState);

			if (WindowState == FormWindowState.Normal)
			{
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_LEFT, Left);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_TOP, Top);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_WIDTH, Width);
				_settings.SetValue(Constants.SETUP_DIALOG + Constants.WINDOW_HEIGHT, Height);
			}

			_settings.SetValue(Constants.SETUP_DIALOG + CHANNEL_EXPLORER_WIDTH, DockPanel.DockRightPortion);

			if (_workshop.CurrentTool != null)
				_settings.SetValue(LAST_TOOL, _workshop.CurrentTool.ID);

			// UI Settings
			_workshop.UI.SaveSettings();

			if (_workshop.Tools != null)
				foreach (PlugInTool PTool in _workshop.Tools)
					PTool.Tool.SaveSettings();

			if (_workshop.ToolGroups != null)
				foreach (PlugInToolGroup pToolGroup in _workshop.ToolGroups)
					pToolGroup.ToolGroup.SaveSettings();

			_mruMenu.SaveSettings();
			_workshop.KeyboardController.SaveSettings();
			_settings.Save();
			//Preview.WriteTraceMessage("Editor.SaveSettings END", TraceLevel.Verbose);
		}

		/// <summary>
		/// Sets the tooltip/text/enabled properties of the edit controls, based on the Undo and/or clipboard status of the current Profile.
		/// </summary>
		private void SetEditControls()
		{
			// Clipboard tools
			bool HasData = false;

			if (Profile != null)
				HasData = Profile.HasMask;

			Edit_Copy.Enabled = HasData;
			Edit_Cut.Enabled = HasData;
			ToolBar_Copy.Enabled = HasData;
			ToolBar_Cut.Enabled = HasData;

			HasData = _workshop.Clipboard.HasData;
			Edit_Paste.Enabled = HasData;
			ToolBar_Paste.Enabled = HasData;

			// Undo tools
			string Undo = string.Empty;
			HasData = false;
			if (Profile != null)
			{
				HasData = Profile.HasRedo;
				Undo = Profile.GetUndoText(false);
			}
			SetRedoTools(HasData, Undo);

			Undo = string.Empty;
			HasData = false;

			if (Profile != null)
			{
				HasData = Profile.HasUndo;
				Undo = Profile.GetUndoText(true);
			}
			SetUndoTools(HasData, Undo);
		}

		/// <summary>
		/// Sets the Tag properties for the Diagnostic menu.
		/// </summary>
		private void SetPaneMenuTags()
		{
			Pane_ActiveChannel.Tag = Panes.ActiveChannel;
			Pane_Canvas.Tag = Panes.CapturedCanvas;
			Pane_Clipboard.Tag = Panes.Clipboard;
			Pane_ImageStamp.Tag = Panes.ImageStamp;
			Pane_MaskPixels.Tag = Panes.MaskCanvas;
			Pane_MaskCells.Tag = Panes.MaskLattice;
			Pane_MoveChannel.Tag = Panes.MoveChannel;
			Pane_Paint.Tag = Panes.LatticeBuffer;
			Pane_Snapshot.Tag = Panes.Snapshot;
			Pane_UndoStack.Tag = Panes.Undo;
			Pane_RedoStack.Tag = Panes.Redo;
		}

		/// <summary>
		/// Sets the Redo menu item and tool strip button to have the name of the current item to be redone (if any), or disables them if there is none.
		/// </summary>
		/// <param name="hasData">Indicates whether there is an item to Redo.</param>
		/// <param name="action">Name of the item to be redone.</param>
		private void SetRedoTools(bool hasData, string action)
		{
			if (hasData)
			{
				Edit_Redo.Text = "&Redo " + action;
				Edit_Redo.Enabled = true;
				ToolBar_Redo.ToolTipText = "Redo " + action;
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
		/// Writes out the message onto the status bar.
		/// </summary>
		/// <param name="message">Message to write out.</param>
		private void SetStatus(string message)
		{
			tssStatus.Text = message;
		}

		/// <summary>
		/// Sets the Undo menu item and tool strip button to have the name of the current item to be undone (if any), or disables them if there is none.
		/// </summary>
		/// <param name="hasData">Indicates whether there is an item to Undo.</param>
		/// <param name="action">Name of the item to be undone.</param>
		private void SetUndoTools(bool hasData, string action)
		{
			if (hasData)
			{
				Edit_Undo.Text = "&Undo " + action;
				Edit_Undo.Enabled = true;
				ToolBar_Undo.ToolTipText = "Undo " + action;
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

		/// <summary>
		/// Perform necessary event decoupling that should occur when this form is closing
		/// </summary>
		private void ShutDown()
		{			
			// Form is closed, do last minute housekeeping
			DetachEvents();
			//_keyboardHook.Stop();

			#if DEBUG

			if (_workshop.BufferPaneForm != null)
			{
				_workshop.BufferPaneForm.AssignedParent = null;
				_workshop.BufferPaneForm.Close();
				_workshop.BufferPaneForm = null;

			}
			#endif

			//if (_keyboardController != null)
			//{
			//	_keyboardController.Dispose();
			//	_keyboardController = null;
			//}

			_channelExplorer.Close();
			_channelExplorer.Editor = null;			

			_workshop.ProfileController.Clear();
		}

		/// <summary>
		/// Updates the status bar for the current Active Channel.
		/// </summary>
		private void UpdateControls_ActiveChannel()
		{
			if ((Profile == null) || (Profile.Channels == null) || (Profile.Channels.Active == null))
			{
				_tssChannel.Visible = false;
				tssChannel.Visible = false;
				return;
			}

			_tssChannel.Visible = true;
			tssChannel.Visible = true;
			tssChannel.Text = Profile.Channels.Active.ToString();
			tssChannel.Image = ImageHandler.ColorSwatches[Profile.Channels.Active.ChannelExplorerImageKey];

			UpdateChannelContextMenu();
		}

		/// <summary>
		/// Updates the controls to reflect the current Profile.
		/// </summary>
		private void UpdateControls_Profile()
		{
			UpdateControls_ActiveChannel();
			UpdateControls_ProfileCellSize();
			UpdateControls_ProfileLatticeSize();
			UpdateControls_ProfileZoom();
			SetEditControls();
			Profile_ShowGrid.Image = GetMenuCheckedImage(ElfRes.grid, (Profile != null ? Profile.Scaling.ShowGridLines : false));
		}

		/// <summary>
		/// Finds and checks the menu item that corresponds to the current Profile's Cell Size. Also update the status bar with this information.
		/// </summary>
		private void UpdateControls_ProfileCellSize()
		{
			if (Profile == null)
				return;

			int CellSize = Profile.Scaling.CellSize.GetValueOrDefault(1);

			if (_menuCellSizes == null)
			{
				_menuCellSizes = new List<ToolStripMenuItem>();
				//	_menuCellSizes.Add(CellSize_1);
				//	_menuCellSizes.Add(CellSize_2);
				//	_menuCellSizes.Add(CellSize_3);
				//	_menuCellSizes.Add(CellSize_4);
				//	_menuCellSizes.Add(CellSize_5);
				//	_menuCellSizes.Add(CellSize_6);
				//	_menuCellSizes.Add(CellSize_7);
				//	_menuCellSizes.Add(CellSize_8);
				//	_menuCellSizes.Add(CellSize_9);
				//	_menuCellSizes.Add(CellSize_10);

				foreach (ToolStripMenuItem Menu in Profile_CellSize.DropDownItems)
					_menuCellSizes.Add(Menu);
			}

			for (int i = 1; i <= 10; i++)
				_menuCellSizes[i - 1].Checked = (CellSize == i);

			tssCellSize.Text = string.Format("{0}x{0} Pixel{1}", CellSize, (CellSize == 1) ? "" : "s");
			_tssCellSize.Image = _menuCellSizes[CellSize - 1].Image;
			//switch (CellSize)
			//{
			//	case 1:
			//		tssCellSize.Image = ElfRes.pixel_1;
			//		break;
			//	case 2:
			//		tssCellSize.Image = ElfRes.pixel_2;
			//		break;
			//	case 3:
			//		tssCellSize.Image = ElfRes.pixel_3;
			//		break;
			//	case 4:
			//		tssCellSize.Image = ElfRes.pixel_4;
			//		break;
			//	case 5:
			//		tssCellSize.Image = ElfRes.pixel_5;
			//		break;
			//	case 6:
			//		tssCellSize.Image = ElfRes.pixel_6;
			//		break;
			//	case 7:
			//		tssCellSize.Image = ElfRes.pixel_7;
			//		break;
			//	case 8:
			//		tssCellSize.Image = ElfRes.pixel_8;
			//		break;
			//	case 9:
			//		tssCellSize.Image = ElfRes.pixel_9;
			//		break;
			//	case 10:
			//	default:
			//		tssCellSize.Image = ElfRes.pixel_10;
			//		break;
			//}

		}

		/// <summary>
		/// Updates the status bar for the current LatticeSize.
		/// </summary>
		private void UpdateControls_ProfileLatticeSize()
		{
			if (Profile == null)
				return;
			tssResolution.Text = Profile.Scaling.LatticeSize.Width + "x" + Profile.Scaling.LatticeSize.Height;
		}

		/// <summary>
		/// Updates the status bar for the current Zoom.
		/// </summary>
		private void UpdateControls_ProfileZoom()
		{
			if (Profile == null)
				return;
			tssZoom.Text = (Profile.Scaling.Zoom * 100) + "%";
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ CanvasWindow Events ]

		/// <summary>
		/// Fires when the mouse has enter the CanvasPane control on the Canvas Window form for the active Profile.
		/// Makes the mouse coordinates panel on the status bar visible.
		/// </summary>
		private void Canvas_MouseEnter(object sender, EventArgs e)
		{
			tssMouseCoords.Visible = true;
		}

		/// <summary>
		/// Fires when the mouse has left the CanvasPane control on the Canvas Window form for the active Profile.
		/// Makes the mouse coordinates panel on the status bar invisible.
		/// </summary>
		private void Canvas_MouseLeave(object sender, EventArgs e)
		{
			tssMouseCoords.Visible = false;
		}

		#endregion [ CanvasWindow Events ]

		#region [ Clipboard Events ]

		/// <summary>
		/// Occurs when the Clipboard contents are changed. Usually when something is Copied or Cut to the buffer
		/// </summary>
		private void Clipboard_Changed(object sender, EventArgs e)
		{
			SetEditControls();
		}

		#endregion [ Clipboard Events ]

		#region [ Form Events ]

		/// <summary>
		/// Occurs when a user is dragging a file from Explorer and has entered the current window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_DragEnter(object sender, DragEventArgs e)
		{
			if (AllowDrop)
				if (e.Data.GetDataPresent(DataFormats.FileDrop))
					e.Effect = DragDropEffects.Copy;
		}

		/// <summary>
		/// Occurs when a user has dragged one or more Profile file from the Explorer window to this form.
		/// </summary>
		private void Form_DragDrop(object sender, DragEventArgs e)
		{
			if (!AllowDrop)
				return;

			List<string> Filenames = new List<string>();
			Filenames.AddRange((string[])e.Data.GetData(DataFormats.FileDrop));
			OpenProfiles(Filenames);
		}

		/// <summary>
		/// Fires when this window is closing. Save the window position/state settings.
		/// </summary>
		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		/// <summary>
		/// The form has closed, detach all events and destroy all objects.
		/// </summary>
		private void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			ShutDown();
		}

		/// <summary>
		/// Fires when this form begins to load into memory.
		/// </summary>
		private void Form_Load(object sender, EventArgs e)
		{ }

		/// <summary>
		/// Fires when the user taps a key on the keyboard.
		/// </summary>
		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			_workshop.KeyboardController.Keyboard_KeyDown(e);
			if (e.Handled)
				return;

			if (Profile == null)
				return;

			Point Offset = Point.Empty;
			bool Nudge = false;

			if (Profile.HasMask)
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						Offset = new Point(0, -1);
						Nudge = true;
						break;
					case Keys.Down:
						Offset = new Point(0, +1);
						Nudge = true;
						break;
					case Keys.Left:
						Offset = new Point(-1, 0);
						Nudge = true;
						break;
					case Keys.Right:
						Offset = new Point(+1, 0);
						Nudge = true;
						break;
					case Keys.Delete:
						if (_workshop.ChannelMover.Delete())
						{
							e.Handled = true;
							Profile.SaveUndo(ChannelMover.UNDO_DELETECELLS);
						}
						break;
				}

				if (Nudge)
				{
					_workshop.ChannelMover.Cut();
					_workshop.ChannelMover.Move(Offset);
					_workshop.ChannelMover.Paste();
					Profile.SaveUndo(ChannelMover.UNDO_MOVECELLS);
					e.Handled = true;
					return;
				}
			}

			
		}

		/// <summary>
		/// Fires when this form has been resized.
		/// </summary>
		private void Form_Resize(object sender, EventArgs e)
		{
			_currentTool_ToolStrip.Width = ToolStrip_Edit.Width - _currentTool_ToolStrip.Location.X;
			_currentTool_ToolStrip.Height = ToolStrip_Edit.Height;
		}

		/// <summary>
		/// Fires when the user/system begins to resize this form.
		/// </summary>
		private void Form_ResizeBegin(object sender, EventArgs e)
		{
			DockPanel.SuspendLayout();
		}

		/// <summary>
		/// Fires at the end of the resizing events.
		/// </summary>
		private void Form_ResizeEnd(object sender, EventArgs e)
		{
			DockPanel.ResumeLayout();
		}

		/// <summary>
		/// Occurs the first time the Editor window appears on screen
		/// </summary>
		private void Form_Shown(object sender, EventArgs e)
		{
			Text = (Text + " - " + AssemblyInfo.Trademark).TrimEnd() + " Version " + AssemblyInfo.AssemblyVersion;

			if (_workshop.RunMode == RunMode.PlugIn)
			{
				if ((_workshop.Profile != null) && ((_workshop.Profile.Name ?? string.Empty).Length > 0))
					Text += " - " + _workshop.Profile.Name;
			}
		}

		#endregion [ Form Events ]

		#region [ Keyboard Controller Events ]

		private void KeyboardController_MultiKeyGestureOccurred(object sender, MultiKeyGestureEventArgs e)
		{
			if (_suspendKeyboardEvents)
				return;
			
			if (e.Gesture.MenuItem != null)
				e.Gesture.MenuItem.PerformClick();
		}

		#endregion [ Keyboard Controller Events ]

		#region [ Mask Events ]

		/// <summary>
		/// Occurs when the Mask is Defined or Cleared
		/// </summary>
		private void Mask_DefinedOrCleared(object sender, EventArgs e)
		{
			SetEditControls();
		}		

		#endregion [ Mask Events ]

		#region [ Menu Events ]

		#region [ File Menu Events ]

		#region [ PlugIn File Menu ]

		/// <summary>
		/// Exits this window and returns control back to the calling program (Vixen) with a flag indicating that
		/// the data should be saved
		/// </summary>
		private void File_SaveAndExit_Click(object sender, EventArgs e)
		{
			Profile.Save();
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		/// Saves the data to the setup node and allows the user to continue editing.
		/// </summary>
		private void File_SaveContinue_Click(object sender, EventArgs e)
		{
			//this.PerformReconcile = true;
			Profile.Save();
		}

		/// <summary>
		/// Exits this window and returns control back to the calling program (Vixen) with a flag indicating that
		/// the data should be ignored
		/// </summary>
		private void File_ExitPlugIn_Click(object sender, EventArgs e)
		{
			if (!Profile.RequestClose())
				return;

			DialogResult = DialogResult.Cancel;
			Close();
		}

		#endregion [ PlugIn File Menu ]

		private void File_Settings_Click(object sender, EventArgs e)
		{
			_suspendKeyboardEvents = true;
			EditUISettings frmSettings = new EditUISettings();
			if (frmSettings.ShowDialog() != DialogResult.Cancel)
				AssignKeyboardShortcuts();
			frmSettings = null;
			_suspendKeyboardEvents = false;
		}

		/// <summary>
		/// Prompt to save any dirty Profiles, then exits the program
		/// </summary>
		private void File_Exit_Click(object sender, EventArgs e)
		{
			if (_workshop.RunMode == RunMode.Standalone)
			{
				foreach (BaseProfile Profile in _workshop.ProfileController.List)
					if (!Profile.RequestClose())
						return;
			}
			else if (_workshop.RunMode == RunMode.PlugIn)
			{
				if (!Profile.RequestClose())
					return;

				DialogResult = DialogResult.Cancel;
			}
			Close();
		}

		/// <summary>
		/// Save the current Profile under a different name
		/// </summary>
		private void File_SaveAs_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			// Set the filter to match that of the currently loaded profile type.
			Type ProfileType = Profile.ProfileDataLayer.GetType();
			for (int i = 0; i < _workshop.AvailableProfiles.Count; i++)
			{
				if (ProfileType.Name == _workshop.AvailableProfiles[i].Profile.GetType().Name)
				{
					SaveProfileDialog.FilterIndex = i+1;
					break;
				}
			}

			string Filename = Profile.Filename;
			if (Filename.Length > 0)
			{
				if (Filename.Contains("\\"))
					SaveProfileDialog.InitialDirectory = Path.GetFullPath(Filename);
				FileInfo fi = new FileInfo(Filename);
				SaveProfileDialog.FileName = fi.Name;
			}
			if (SaveProfileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			Filename = SaveProfileDialog.FileName;
			Profile.Filename = Filename;

			// Determine what type of Profile to save this as based on the Filter Index of the dialog.
			Type TargetProfileType = _workshop.AvailableProfiles[SaveProfileDialog.FilterIndex - 1].Profile.GetType();

			_workshop.ProfileController.ConvertProfile(Profile, TargetProfileType);
			Profile.Save();

			_mruMenu.AddFile(Filename);
			_settings.Save();
		}

		/// <summary>
		/// Calls the Profiles save method, using the filename it already has.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void File_Save_Click(object sender, EventArgs e)
		{
			SetStatus("Saving...");
			if (_workshop.RunMode == RunMode.Standalone)
			{
				if (Profile == null)
					return;

				// If there is not a filename already defined within the Profile,then call the SaveAs menu item.
				if (Profile.Filename.Length == 0)
					File_SaveAs_Click(sender, e);
				else
				{
					Profile.Save();
					_mruMenu.AddFile(Profile.Filename);
					_settings.Save();
				}
			}
			else if (_workshop.RunMode == RunMode.PlugIn)
			{
				Profile.Save();
			}
			SetStatus("Ready");
		}

		/// <summary>
		/// Reverts the Profile to the last saved version.
		/// </summary>
		private void File_Revert_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			// Give the user a chance to not do this.
			if (MessageBox.Show("Revert the Profile to it's last saved version? This cannot be undone.", "Revert to Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				return;

			SetStatus("Reverting...");
			string FileName = Profile.Filename;
			Profile.SuppressEvents = true;
			_workshop.ProfileController.Remove(Profile);
			OpenProfiles(new List<string>() { FileName });
			SetStatus("Ready");
		}

		/// <summary>
		/// Close the Profile. If there are changes, prompt the user to see if they want to save them first.
		/// </summary>
		private void File_Close_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			if (Profile.RequestClose())
				// Remove the profile from the list, switch to the next Profile, if any
				_workshop.ProfileController.Remove(Profile);
		}

		/// <summary>
		/// Open a new Profile, load its data from disk, and then make it the Active Profile
		/// </summary>
		private void File_Open_Click(object sender, EventArgs e)
		{
			OpenProfileDialog.Multiselect = true;
			if (OpenProfileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			SetStatus("Opening Profile(s)");
			List<string> Filenames = new List<string>();
			Filenames.AddRange(OpenProfileDialog.FileNames);
			OpenProfiles(Filenames);
			Filenames = null;
			SetStatus("Ready");
		}

		/// <summary>
		/// Create a new Profile
		/// </summary>
		private void File_New_Click(object sender, EventArgs e)
		{
			SetStatus("Creating New Profile...");
			NewProfile frmNew = new NewProfile();
			if (frmNew.ShowDialog() == DialogResult.Cancel)
			{
				SetStatus("New Profile aborted.");
				return;
			}

			BaseProfile NewProfile = new BaseProfile(frmNew.ProfileType)
			{
				Name = frmNew.ProfileName,
				Filename = frmNew.Filename
			};
			NewProfile.Scaling.LatticeSize = new Size(frmNew.CanvasWidth, frmNew.CanvasHeight);
			NewProfile.Scaling.CellSize = frmNew.CellSize;
			NewProfile.Scaling.ShowGridLines = frmNew.ShowGridLines;
			NewProfile.Scaling.Zoom = Scaling.ZOOM_100;
			NewProfile.Channels.Clear();
			foreach (Channel Channel in frmNew.Channels)
			{
				NewProfile.Channels.Add(Channel);
			}

			NewProfile.InitializeUndo();
			_workshop.ProfileController.Add(NewProfile);
			_workshop.ProfileController.Active = NewProfile;
			
			NewProfile = null;
			frmNew = null;
			File_Save.Enabled = false;
			File_Revert.Enabled = false;
			SetStatus("Ready");
		}

		/// <summary>
		/// Loads the file that was selected from the MRU list.
		/// </summary>
		/// <param name="number">MRU Number of the file</param>
		/// <param name="filename">Name of the file</param>
		private void OnMruFile_Data(int number, string filename)
		{
			if (filename.Length == 0)
				return;

			FileInfo FI = new FileInfo(filename);
			if ((FI == null) || (FI.Exists == false))
			{
				MessageBox.Show(filename + " does not exist. Removing it from the Most Recent File menu.", "Open Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
				_mruMenu.RemoveFile(number);
				_settings.Save();
				return;
			}

			SetStatus("Opening Profile " + filename);
			
			// Create a new Profile object and pass in the Filename
			OpenProfiles(filename);

			//_mruMenu.RemoveFile(number);
			_mruMenu.AddFile(filename);
			_settings.Save();
			SetStatus("Ready");
		}

		#endregion [ File Menu Events ]

		#region [ Edit Menu Events ]

		/// <summary>
		/// Copies the contents of the marquee from the selected Channel(s)
		/// </summary>
		private void Edit_Copy_Click(object sender, EventArgs e)
		{
			if (!_workshop.Clipboard.Copy())
				MessageBox.Show("Selection is Empty!", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		/// <summary>
		/// Cuts the contents of the marquee from the selected Channel(s)
		/// </summary>
		private void Edit_Cut_Click(object sender, EventArgs e)
		{
			if (!_workshop.Clipboard.Cut())
				MessageBox.Show("Selection is Empty!", "Cut", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else
				Profile.Refresh();
		}

		/// <summary>
		/// Pastes the contents from the Clipboard to the selected Channel(s)
		/// </summary>
		private void Edit_Paste_Click(object sender, EventArgs e)
		{
			if (_workshop.Clipboard.HasData)
			{
				_workshop.Clipboard.Paste();
				Profile.Refresh();
			}
		}

		/// <summary>
		/// Reapply the last operation that was recently undone.
		/// </summary>
		private void Edit_Redo_Click(object sender, EventArgs e)
		{
			Cursor LastCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			Profile.Redo();
			Cursor = LastCursor;
		}

		/// <summary>
		/// Reverse out the last operation
		/// </summary>
		private void Edit_Undo_Click(object sender, EventArgs e)
		{
			Cursor LastCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			Profile.Undo();
			Cursor = LastCursor;
		}

		#endregion [ Edit Menu Events ]

		#region [ Profile Menu Events ]

		/// <summary>
		/// Sets the Canvas Size
		/// </summary>
		private void Profile_SetCanvasSize_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;
			ChangeLatticeSize frmLatticeSize = new ChangeLatticeSize();
			if (frmLatticeSize.ShowDialog() == DialogResult.Cancel)
				return;
			Profile.Scaling.LatticeSize = new Size(frmLatticeSize.LatticeWidth, frmLatticeSize.LatticeHeight);
			Profile.SaveUndo("Set Canvas Size");
			frmLatticeSize = null;
		}

		/// <summary>
		/// Sets the Cell Size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Profile_CellSize_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			Profile.Scaling.CellSize = _menuCellSizes.IndexOf((ToolStripMenuItem)sender) + 1;
			//Profile.Scaling.CellSize = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
			Profile.SaveUndo("Change Cell Size");
			SetStatus("Cell Size set to " + Profile.Scaling.CellSize);
		}

		/// <summary>
		/// Indicates whether the grid should be used for displaying Cells.
		/// </summary>
		private void Profile_ShowGrid_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;
			Profile.Scaling.ShowGridLines = !Profile.Scaling.ShowGridLines;
			Profile.SaveUndo(Profile.Scaling.ShowGridLines.GetValueOrDefault(true) ? "Show Grid Lines" : "Hide Grid Lines");
			SetStatus("Grid Lines are now " + ((Profile.Scaling.ShowGridLines ?? false) ? "on" : "off"));
		}

		/// <summary>
		/// Bring up the Background Settings dialog.
		/// </summary>
		private void Profile_BackgroundSettings_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;
			BackgroundSettings frmBackground = new BackgroundSettings();
			frmBackground.ShowDialog();
			frmBackground = null;
		}

		#endregion [ Profile Menu Events ]

		#region [ Diagnostic Menu Events ]
		
		private void ShowPane_Click(object sender, EventArgs e)
		{
			#if DEBUG
				_workshop.BufferPaneForm.SetTab((Panes)((ToolStripMenuItem)sender).Tag);
				Focus();
			#endif
		}

		private void Diag_ShowPanes_Click(object sender, EventArgs e)
		{
			#if DEBUG
				Diag_ShowPanes.Checked = !Diag_ShowPanes.Checked;
				bool Showing = Diag_ShowPanes.Checked;

				_workshop.BufferPaneForm.Visible = Showing;
				if (Showing)
					_workshop.BufferPaneForm.WindowState = FormWindowState.Normal;

				Focus();

				Diag_DisplayUndoData.Enabled = Showing;
				Pane_ActiveChannel.Enabled = Showing;
				Pane_Canvas.Enabled = Showing;
				Pane_Clipboard.Enabled = Showing;
				Pane_ImageStamp.Enabled = Showing;
				Pane_MaskPixels.Enabled = Showing;
				Pane_MaskCells.Enabled = Showing;
				Pane_MoveChannel.Enabled = Showing;
				Pane_Paint.Enabled = Showing;
				Pane_Snapshot.Enabled = Showing;
				Pane_UndoStack.Enabled = Showing;
				Pane_RedoStack.Enabled = Showing;
			#endif
		}

		private void Diag_DisplayUndoData_Click(object sender, EventArgs e)
		{
			#if DEBUG
				Diag_DisplayUndoData.Checked = !Diag_DisplayUndoData.Checked;
				_workshop.BufferPaneForm.ShowUndoData = Diag_ShowPanes.Checked;
			#endif
		}

		#endregion [ Diagnostic Menu Events ]

		#region [ Window Menu Events ]

		/// <summary>
		/// Makes the Profile associated with this menu the Active one.
		/// </summary>
		private void Window_Profile_Click(object sender, EventArgs e)
		{
			_workshop.ProfileController.Active = (BaseProfile)((ToolStripMenuItem)sender).Tag;
			_workshop.ProfileController.Active.Form.Activate();
		}

		#endregion [ Window Menu Events ]

		private void AboutMenu_Click(object sender, EventArgs e)
		{
			ProfileSplash frmAbout = new ProfileSplash();
			frmAbout.ShowDialog();
			frmAbout = null;
		}

		#endregion [ Menu Events ]

		#region [ Profile Controller Events ]

		/// <summary>
		/// Occurs when a property on 1 or more Channels has changed.
		/// </summary>
		private void Profile_ChannelPropertyChanged(object sender, ChannelListEventArgs e)
		{
			UpdateControls_ActiveChannel();
		}

		/// <summary>
		/// Occurs when 1 or more Channels have been selected.
		/// </summary>
		private void Profile_ChannelsSelected(object sender, ChannelListEventArgs e)
		{
			UpdateControls_ActiveChannel();
		}

		/// <summary>
		/// Fires when the dirty bit for the profile changed.
		/// </summary>
		private void Profile_DirtyChanged(object sender, DirtyEventArgs e)
		{
			File_Save.Enabled = e.IsDirty;

			// If we've not saved this file yet, Revert is meaningless.
			File_Revert.Enabled = (e.IsDirty & (Profile.Filename.Length > 0));
		}

		/// <summary>
		/// Occurs when one of the properties of the active Profile (or one of its child objects) has changed.
		/// </summary>
		private void Profile_ScalingChanged(object sender, EventArgs e)
		{
			UpdateControls_Profile();
		}

		/// <summary>
		/// Occurs when the Active Profile changes.
		/// </summary>
		private void Profiles_Switched(object sender, ProfileEventArgs e)
		{
			if (e.OldProfile != null)
			{
				// Detach the events from the old Profile
				e.OldProfile.Canvas_MouseEnter -= Canvas_MouseEnter;
				e.OldProfile.Canvas_MouseLeave -= Canvas_MouseLeave;
				e.OldProfile.ScalingChanged -= Profile_ScalingChanged;
				e.OldProfile.DirtyChanged -= Profile_DirtyChanged;
				e.OldProfile.ChannelPropertyChanged -= Profile_ChannelPropertyChanged;
				e.OldProfile.ChannelsSelected -= Profile_ChannelsSelected;
				e.OldProfile.Mask_Defined -= Mask_DefinedOrCleared;
				e.OldProfile.Mask_Cleared -= Mask_DefinedOrCleared;
				e.OldProfile.Undo_Changed -= Undo_Changed;
				e.OldProfile.Redo_Changed -= Redo_Changed;
				e.OldProfile.Undo_Completed -= Undo_Completed;
			}

			if (e.Profile != null)
			{
				// Attach the events for the new Active Profile
				e.Profile.Canvas_MouseEnter += Canvas_MouseEnter;
				e.Profile.Canvas_MouseLeave += Canvas_MouseLeave;
				e.Profile.ScalingChanged += Profile_ScalingChanged;
				e.Profile.DirtyChanged += Profile_DirtyChanged;
				e.Profile.ChannelPropertyChanged += Profile_ChannelPropertyChanged;
				e.Profile.ChannelsSelected += Profile_ChannelsSelected;
				e.Profile.Mask_Defined += Mask_DefinedOrCleared;
				e.Profile.Mask_Cleared += Mask_DefinedOrCleared;
				e.Profile.Undo_Changed += Undo_Changed;
				e.Profile.Redo_Changed += Redo_Changed;
				e.Profile.Undo_Completed += Undo_Completed;
			}

			if (_workshop.RunMode != RunMode.Standalone)
			{
				UpdateControls_Profile();
				return;
			}

			// If we are in standalone mode, check to see if the ActiveProfile is null.
			// Change the enabled state on the menus based on this.
			if (e.Profile == null)
			{
				File_Close.Enabled = false;
				File_Revert.Enabled = false;
				File_Save.Enabled = false;
				File_SaveAs.Enabled = false;
				EditMenu.Enabled = false;
				ProfileMenu.Enabled = false;
				ChannelMenu.Enabled = false;
				WindowMenu.Enabled = false;
				ToolBox_Main.Enabled = false;
				
				tssMouseCoords.Visible = false;
				tssStatus.Text = "No Profile Loaded...";

				_toolStripPanel.Enabled = false;
				ToolStrip_Edit.Enabled = false;
			}
			else
			{
				File_Close.Enabled = true;
				File_Revert.Enabled = false;
				File_Save.Enabled = true;
				File_SaveAs.Enabled = true;
				EditMenu.Enabled = true;
				ProfileMenu.Enabled = true;
				ChannelMenu.Enabled = true;
				WindowMenu.Enabled = true;
				ToolBox_Main.Enabled = true;
				tssMouseCoords.Visible = true;

				_toolStripPanel.Enabled = true;
				ToolStrip_Edit.Enabled = true;
			}
			UpdateControls_Profile();
			CheckActiveProfileWindowMenuItem();
		}

		/// <summary>
		/// Occurs when a new Profile is added to the Profile Controller
		/// </summary>
		private void Profiles_Added(object sender, ProfileEventArgs e)
		{
			// Events are attached when Profile_Switched is called.

			// Add the form to the docking control
			e.Profile.Form.Show(DockPanel);

			// Update the Windows menu
			ToolStripMenuItem ProfileMenuItem = new ToolStripMenuItem();

			ProfileMenuItem.Name = "ProfileMenuItem";
			ProfileMenuItem.Text = e.Profile.Name;
			ProfileMenuItem.Click += Window_Profile_Click;
			ProfileMenuItem.Tag = e.Profile;

			WindowMenu.DropDownItems.Add(ProfileMenuItem);
			CheckActiveProfileWindowMenuItem();

			// Update the custom colors list in Workshop with all the Sequence and Render colors from this profile.
			foreach (Channel Channel in e.Profile.Channels)
			{
				_workshop.CustomColors.Add(new NamedColor(Channel.SequencerColor, Channel.Name));
				if (!Channel.RenderColor.IsEmpty)
					_workshop.CustomColors.Add(new NamedColor(Channel.RenderColor, Channel.Name + " Render"));
				if (!Channel.BorderColor.IsEmpty)
					_workshop.CustomColors.Add(new NamedColor(Channel.BorderColor, Channel.Name + " Border"));
			}

			if (e.Profile.Dirty)
			{
				e.Profile.SetClean();
				e.Profile.Dirty = true;
			}
		}

		/// <summary>
		/// Occurs when a Profile has been from the Profile Controller
		/// </summary>
		private void Profiles_Removed(object sender, ProfileEventArgs e)
		{
			// Update the Windows menu
			foreach (ToolStripMenuItem Menu in WindowMenu.DropDownItems)
			{
				if (ReferenceEquals(Menu.Tag, e.Profile))
				{
					WindowMenu.Click -= Window_Profile_Click;
					WindowMenu.DropDownItems.Remove(Menu);
					break;
				}
			}
			CheckActiveProfileWindowMenuItem();
			e.Profile.Form.DockHandler.Close();
		}

		private void Profile_Closing(object sender, EventArgs e)
		{ }

		#endregion [ Profile Controller Events ]

		#region [ ToolBox Events ]

		/// <summary>
		/// Fires when the user releases the mouse from a toolgroup. Hides the flyout if it had been triggered.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolGroup_MouseUp(object sender, MouseEventArgs e)
		{
			if (tmrFlyout.Enabled)
			{
				tmrFlyout.Enabled = false;
				_flyoutToolStrip = null;
			}
		}

		/// <summary>
		/// Fires when the user mouse downs on the button holding a ToolGroup and pauses. Trigger's a flyout for that toolgroup.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolGroup_MouseDown(object sender, MouseEventArgs e)
		{
			PlugInToolGroup TG = ((PlugInToolStripButton)sender).PlugInToolGroup;

			_flyoutToolStrip = TG.ChildToolBox;
			tmrFlyout.Enabled = true;
		}

		/// <summary>
		/// Fires when the interval for the timer is reached. Display a flyout toolstrip.
		/// </summary>
		private void tmrFlyout_Tick(object sender, EventArgs e)
		{
			tmrFlyout.Enabled = false;
			_flyoutToolStrip.Visible = true;
			Controls.SetChildIndex(_flyoutToolStrip, 0); // Bring this submenu to the Front of all other controls
		}

		/// <summary>
		/// Fires when on the buttons on the main ToolBox is clicked.
		/// </summary>
		private void Tool_Click(object sender, EventArgs e)
		{
			PlugInTool SelectedTool = ((PlugInToolStripButton)sender).PlugInTool;

			// Fixes a bug that has the tool already selected if picking from a child toolbox.
			if (SelectedTool.Tool.IsSelected)
				SelectedTool.Tool.IsSelected = false;

			if (SelectedTool == null)
				throw new ArgumentNullException("SelectedTool is null");

			//Debug.WriteLine(SelectedTool.Name);

			// Unselect all tools that are not the one passed in, and select that one tool passed in.
			foreach (PlugInTool pTool in _workshop.Tools)
			{
				pTool.Tool.IsSelected = (pTool.ID == SelectedTool.ID);
			}
		}

		/// <summary>
		/// The operation for the current tool has finished, create the Undo for it.
		/// </summary>
		private void Tool_OperationCompleted(object sender, EventArgs e)
		{
			Profile.SaveUndo(((ITool)sender).UndoText);
		}

		/// <summary>
		/// Occurs when a Tool has been selected. Update the main ToolBox and set this tool to be the Active one in Workshop.
		/// </summary>
		private void Tool_Selected(object sender, EventArgs e)
		{
			bool IsPlugInToolGroup = false;
			
			ITool Tool = (ITool)sender;

			// Update the status bar
			tssTool.Text = Tool.Name;
			tssTool.Image = Tool.ToolBoxImage;

			// Swap out the setting toolstrips
			_toolStripPanel.Controls.Clear();

			_currentTool_ToolStrip = Tool.SettingsToolStrip;
			if (_currentTool_ToolStrip == null)
				_currentTool_ToolStrip = ToolStrip_Blank;

			_toolStripPanel.Join(ToolStrip_Edit, 0);
			_toolStripPanel.Join(_currentTool_ToolStrip, 1);

			// Uncheck all the other tools in the ToolBox
			ToolBox_Main.SuspendLayout();

			// Find the PlugInTool for this Tool.
			PlugInTool pTool = _workshop.GetPlugInTool(Tool.ID);
			if (pTool == null)
				return;
			_workshop.CurrentTool = pTool;

			PlugInToolStripButton Button = pTool.Button;

			// Check to see if the button clicked was on a sub menu. If so, then
			// set the Button to be the ToolGroup button
			if (pTool.ParentButton != null)
				Button = pTool.ParentButton;

			// On the main ToolBox, uncheck all tools that aren't this Tool, and check the button that corresponds to this Tool.
			foreach (PlugInToolStripButton tButton in ToolBox_Main.Items)
			{
				tButton.Checked = (tButton == Button);
				if (tButton.PlugInToolGroup != null)
				{
					IsPlugInToolGroup = true;
					tButton.PlugInToolGroup.Close();
				}
				else
					IsPlugInToolGroup = false;

				if (tButton.Checked)
					tButton.Image = tButton.PlugInTool.ToolBoxImageSelected;
				else
					tButton.Image = tButton.PlugInTool.ToolBoxImage;

				if (IsPlugInToolGroup)
					tButton.Image = ImageHandler.AddAnnotation((Bitmap)tButton.Image, Annotation.Flyout, AnchorStyles.Bottom | AnchorStyles.Right);
			}

			// Set the cursor for the Profile.
			if (Profile != null)
				Profile.Cursor = Tool.Cursor;

			ToolBox_Main.ResumeLayout(true);

			Tool = null;
			pTool = null;
			Button = null;
		}

		#endregion [ ToolBox Events ]

		#region [ UI Events ]

		private void Controller_Message(object Sender, MessageEventArgs e)
		{
			if ((Sender == _workshop.KeyboardController) && _suspendKeyboardEvents)
				return;

			SetStatus(e.Message);
		}

		/// <summary>
		/// Occurs when one of the properties on the Workshop.UI object changed.
		/// </summary>
		private void UI_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Profile == null)
				return;

			switch (e.PropertyName)
			{
				case UISettings.Property_MousePosition:
					if (_workshop.UI.MouseSelectionSize.IsEmpty)
						tssMouseCoords.Text = string.Format("({0}, {1})", _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);
					else
						tssMouseCoords.Text = string.Format("({0}, {1})-({2}, {3})", _workshop.UI.MouseDownPosition.X, _workshop.UI.MouseDownPosition.Y, _workshop.UI.MousePosition.X, _workshop.UI.MousePosition.Y);

					string ChannelList = string.Empty;
					foreach (Channel Channel in Profile.Channels.Sorted)
					{
						if (Channel.Lattice.Contains(_workshop.UI.MousePosition))
							ChannelList += ((ChannelList.Length > 0) ? ", " : string.Empty) + Channel.ToString(true);
					}
					tssStatus.Text = ChannelList;
					break;

				case UISettings.Property_DisableUndo:
					if (!_workshop.UI.DisableUndo)
						Profile.InitializeUndo();
					break;

				case UISettings.Property_MouseSelectionSize:
				case UISettings.Property_ShowMaskBlocky:
				case UISettings.Property_ShowMaskMarquee:
				case UISettings.Property_ShowMaskOverlay:
				case UISettings.Property_MouseDownPosition:
				case UISettings.Property_SuperimposeGridOnBackground:
					break;
			}
		}

		#endregion [ UI Events ]

		#region [ UndoController Events ]

		/// <summary>
		/// Occurs when a new item is the topmost item in the Redo stack, or the Redo stack has been emptied
		/// </summary>
		private void Redo_Changed(object sender, UndoEventArgs e)
		{
			SetRedoTools(e.HasData, e.Text);
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Undo stack, or the Undo stack has been emptied
		/// </summary>
		private void Undo_Changed(object sender, UndoEventArgs e)
		{
			SetUndoTools(e.HasData, e.Text);
		}

		/// <summary>
		/// Fires when an undo or redo event has completed.
		/// </summary>
		private void Undo_Completed(object sender, EventArgs e)
		{
			SetEditControls();
		}

		#endregion [ UndoController Events ]

		#endregion [ Events ]

	}
}
