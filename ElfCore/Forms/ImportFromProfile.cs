using ElfControls;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Forms
{
	public partial class ImportFromProfile : Form
	{
		#region [ Enums ]

		enum Direction
		{ 
			Up,
			Down,
			Left,
			Right
		}

		#endregion [ Enums ]

		#region [ Constants ]

		private const string SIZE_FMT = "{0} x {1}";

		#endregion [ Constants ]
		
		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;

		/// <summary>
		/// Profile to be imported.
		/// </summary>
		private BaseProfile _importProfile = null;

		private MappingList _mapList = null;
		private Mapping _editMap = null;
		private MappingPreview _frmPreview = null;
		private ListBoxUtil _listBoxUtil = null;
		private Direction dirButton;
		private int _editID = -1;
		private int _selectedChannelID = -1;
		private bool _loadingMapList = false;
		private LatticePoint _maxSetBack = LatticePoint.Empty;
		private LatticePoint _maxExtension = LatticePoint.Empty;
		private Size _originalLatticeSize = Size.Empty;
		private bool _noUpdate = false;

		#endregion [ Private Variables ]

		#region [ Properties ]
		
		/// <summary>
		/// Filename of the Profile to import.
		/// </summary>
		public string Filename
		{
			set
			{
				Cursor = Cursors.WaitCursor;

				Type ProfileType = _workshop.ProfileController.DetectProfileType(value);
				if (ProfileType == null)
					return;

				_importProfile = new BaseProfile(ProfileType);
				if (!_importProfile.Load(value))
					return;

				_importProfile.UnclipCanvasWindow();

				lblProfile.Text = _importProfile.Name;
				ImageListItem Item = null;
				Bitmap ChBitmap = null;
				foreach (Channel Channel in _importProfile.Channels.List)
				{
					ChBitmap = ImageHandler.ColorSwatches[ImageHandler.CreateAndAddColorSwatch(new Swatch(Channel.GetColor(), pnlControls.Enabled))];
					Item = new ImageListItem(Channel.ToString(), Channel.ID.ToString(), ChBitmap, Channel);
					lstImportedChannels.Items.Add(Item);
				}
				Cursor = Cursors.Default;
			}
		}

		public MappingList MappedChannels
		{
			get { return _mapList; }
		}

		/// <summary>
		/// Maximum calculated SetBack
		/// </summary>
		public LatticePoint MaxSetBack
		{
			get { return _maxSetBack; }
		}
	
		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden]
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ImportFromProfile()
		{
			InitializeComponent();
			cmdEdit.Enabled = false;
			cmdDelete.Enabled = false;

			_originalLatticeSize = Profile.Scaling.LatticeSize;

			_listBoxUtil = new ListBoxUtil();

			cmdAdd.Image = ImageHandler.AddAnnotation(ElfRes.map, Annotation.Add, AnchorStyles.Bottom | AnchorStyles.Right);
			cmdEdit.Image = ImageHandler.AddAnnotation(ElfRes.map, Annotation.Edit, AnchorStyles.Bottom | AnchorStyles.Right);
			cmdDelete.Image = ImageHandler.AddAnnotation(ElfRes.map, Annotation.Delete, AnchorStyles.Bottom | AnchorStyles.Right);
			cmdMap.Image = ImageHandler.AddAnnotation(ElfRes.map, Annotation.Save,  AnchorStyles.Bottom | AnchorStyles.Right);
			cmdCancelEdits.Image = ImageHandler.AddAnnotation(ElfRes.map, Annotation.Not, AnchorStyles.Bottom | AnchorStyles.Right);

			_mapList = new MappingList();
			_editMap = new Mapping();
			_editMap.EditFlag = true;
			_editMap.PreviewFlag = true;
			_mapList.Add(_editMap);

			_frmPreview = new MappingPreview();
			_frmPreview.MapList = _mapList;
			_frmPreview.Location = new Point(Location.X + Width + 5, Location.Y);
			_frmPreview.Visible = true;

			// Populate the target channels combo box with all the channels of the current profile, in native order
			cboTargetChannel.Items.Clear();
			cboTargetChannel.Items.Add(new ImageListItem("Select Channel", -1, ElfRes.channel));

			foreach (Channel Channel in Profile.Channels.List)
			{
				cboTargetChannel.Items.Add(new ImageListItem(Channel.ToString(), Channel.ID, ImageHandler.CreateColorSwatch(Channel.GetColor())));
			}
			cboTargetChannel.SelectedIndex = 0;

			lblCanvasSize.Text = string.Format(SIZE_FMT, _originalLatticeSize.Width, _originalLatticeSize.Height);

			ClearControls();
			UpdatePreview();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create a temporary mapping object used for editing and display.
		/// </summary>
		/// <param name="map"></param>
		private void AddEditMap(Mapping map, bool edit)
		{
			map.EditFlag = edit;
			map.PreviewFlag = true;
			_mapList.Add(map);
			_editMap = map;
		}

		/// <summary>
		/// Go through all the mappings and determine the maximum X and Y values for the extension
		/// </summary>
		private void CalcMaxExtension()
		{
			_maxExtension = LatticePoint.Empty;
			foreach (Mapping map in _mapList)
			{
				map.CalcExtension(_originalLatticeSize);
				_maxExtension.X = Math.Max(_maxExtension.X, map.Extension.X);
				_maxExtension.Y = Math.Max(_maxExtension.Y, map.Extension.Y);
			}			
		}

		/// <summary>
		/// Go through all the mappings and determine the maximum X and Y values for set backs.
		/// </summary>
		private void CalcMaxSetBack()
		{
			_maxSetBack = LatticePoint.Empty;
			
			foreach (Mapping map in _mapList)
			{
				map.CalcSetBack();
				_maxSetBack.X = Math.Max(_maxSetBack.X, map.SetBack.X);
				_maxSetBack.Y = Math.Max(_maxSetBack.Y, map.SetBack.Y);
			}
			foreach (Mapping map in _mapList)
			{
				map.CalcEffectiveOffset(_maxSetBack);
			}
			if (_frmPreview != null)
				_frmPreview.MaxSetBack = _maxSetBack;
		}

		/// <summary>
		/// Populate the mapping listbox with pre-existing mapping objects.
		/// </summary>
		private void BuildMapList(int selectedID)
		{
			_loadingMapList = true;
			int Index = lstMapping.SelectedIndex;
			lstMapping.SuspendLayout();

			lstMapping.Items.Clear();

			ImageListItem Item = null;
			foreach (Mapping Map in _mapList)
			{
				Item = new ImageListItem(Map.ToString(), Map.ID.ToString(), ElfRes.map);
				lstMapping.Items.Add(Item);
				if (Map.ID == selectedID)
				{
					lstMapping.SelectedItem = Item;
				}
			}

			lstMapping.ResumeLayout();
			lstMapping.PerformLayout();
			_loadingMapList = false;
		}

		/// <summary>
		/// Clear out the edit controls, setting them back to their neutral state.
		/// </summary>
		private void ClearControls()
		{
			lstImportedChannels.SelectedIndex = -1;
			_selectedChannelID = -1;
			
			txtX.Text = "0";
			txtY.Text = "0";
			chkClear.Checked = false;
			chkOverrideColor.Checked = false;
			chkOverrideName.Checked = false;
			chkResize.Checked = false;
			lblDataSize.Text = string.Empty;
			_editID = -1;
			cboTargetChannel.SelectedIndex = 0;
		}

		/// <summary>
		/// Calculates the final canvas size based off the original value, plus the maximum size offset from all the mappings
		/// </summary>
		public Size RecalculateCanvasSize()
		{
			CalcMaxSetBack();
			CalcMaxExtension();

			return new Size(_originalLatticeSize.Width + _maxSetBack.X + _maxExtension.X, _originalLatticeSize.Height + _maxSetBack.Y + _maxExtension.Y);
		}

		/// <summary>
		/// Retrieves the mapping that corresponds to the entry in the listbox.
		/// </summary>
		private Mapping GetSelectedMap()
		{ 
			string Value = _listBoxUtil.GetKey(lstMapping);
			int ID = 0;

			if (!Int32.TryParse(Value, out ID))
				return null;

			return _mapList.Where(ID);
		}

		private void HighlightSelectedMap()
		{
			Mapping Map = GetSelectedMap();

			if (Map != null)
			{
				if (pnlControls.Enabled && (_editID != Map.ID))
				{
					if (MessageBox.Show(_frmPreview, "Save changes to the mapping that is being edited?", "Select Mapping", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						if (!SaveMapping())
							return;
				}
				if (_editMap != null)
					RemoveEditMap();
				AddEditMap(new Mapping(Map), false);
				LoadControlsFromMapping(Map);
			}
			UpdateButtons();
			UpdatePreview();
		}

		/// <summary>
		/// Populate the edit controls with values from this mapping.
		/// </summary>
		private void LoadControlsFromMapping(Mapping map)
		{
			if (map == null)
			{
				ClearControls();
				return;
			}

			_listBoxUtil.Set(lstImportedChannels, map.ImportedChannel.ID);
			_selectedChannelID = map.TargetChannel.ID;
			chkClear.Checked = map.ClearTargetChannel;
			chkOverrideColor.Checked = map.OverrideColor;
			chkOverrideName.Checked = map.OverrideName;
			chkResize.Checked = map.ResizeLattice;
			txtX.Text = map.ImportedOffset.X.ToString();
			txtY.Text = map.ImportedOffset.Y.ToString();
			_editID = map.ID;
		}

		/// <summary>
		/// Generate a mapping object based on the values stored on the controls
		/// </summary>
		private void PopulateMapFromControls(Mapping map)
		{
			map.ID = _editID;
			int X = 0;
			int Y = 0;
			Int32.TryParse(txtX.Text, out X);
			Int32.TryParse(txtY.Text, out Y);

			map.ImportedOffset = new Point(X, Y);
			map.ResizeLattice = chkResize.Checked;
			map.ClearTargetChannel = chkClear.Checked;
			map.OverrideColor = chkOverrideColor.Checked;
			map.OverrideName = chkOverrideName.Checked;

			Channel Channel = null;
			if (lstImportedChannels.SelectedIndex >= 0)
			{
				Channel = (Channel)_listBoxUtil.GetTag(lstImportedChannels);
				map.ImportedChannel_Bounds = Channel.GetBounds();
				map.CalcSetBack();
				map.CalcExtension(_originalLatticeSize);
				map.CalcEffectiveOffset(_maxSetBack);
				map.ImportedChannel = Channel;
				Channel = null;
			}

			if (_selectedChannelID >= 0)
				map.TargetChannel = Profile.Channels.Get(_selectedChannelID);
		}

		/// <summary>
		/// Remove the temporary mapping from the MapList that has the EditFlag turned on.
		/// </summary>
		private void RemoveEditMap()
		{
			if (_editMap != null)
				_mapList.Remove(_editMap);
			_editMap = null;
			_frmPreview.UpdateDisplay();
		}

		/// <summary>
		/// Save the currently edited map to the list and update the listbox.
		/// </summary>
		private bool SaveMapping()
		{
			Mapping NewMap = new Mapping(_editMap);
			NewMap.PreviewFlag = false;
			NewMap.EditFlag = false;

			if (_selectedChannelID < 0)
				return false;
			if (lstImportedChannels.SelectedIndex < 0)
				return false;

			if (NewMap.ID > 0)
			{
				//var OrigMap = _mapList.Where(m => m.MapID == _editID).FirstOrDefault();
				var OrigMap = _mapList.Where(_editID);
				int Index = _mapList.IndexOf(OrigMap);
				_mapList.Remove(OrigMap);
				_mapList.Insert(Index, NewMap);
			}
			else
			{
				MappingList Ordered = _mapList.OrderByDescending();
				if ((Ordered != null) && (Ordered.Count > 0))
				{
					var Max = Ordered[0];
					if (Max.EditFlag)
						NewMap.ID = 1;
					else
						NewMap.ID = Max.ID + 1;
					_mapList.Add(NewMap);
				}
				Ordered = null;
			}
			RemoveEditMap();
			BuildMapList(NewMap.ID);

			return true;
		}

		/// <summary>
		/// Updates the button controls based on values selected in the various listboxes.
		/// </summary>
		private void UpdateButtons()
		{
			cmdMap.Enabled = true;
			if ((lstImportedChannels.SelectedIndex < 0) || (_selectedChannelID < 0))
				cmdMap.Enabled = false;

			cmdEdit.Enabled = (lstMapping.SelectedIndex >= 0);
			cmdDelete.Enabled = cmdEdit.Enabled;
		}

		/// <summary>
		/// If the Mapping Preview window is visible, pass along the current values to update the display.
		/// </summary>
		private void UpdatePreview()
		{
			if (_noUpdate)
				return;

			// Setting this property calls the UpdateDisplay() on the form.
			_frmPreview.LatticeSize = RecalculateCanvasSize();

			if ((_editMap != null) && (_editMap.ImportedChannel != null))
			{
				Rectangle R = _editMap.ImportedChannel.GetBounds();
				lblDataSize.Text = string.Format(SIZE_FMT, R.Right, R.Bottom);
			}
			else
				lblDataSize.Text = string.Empty;
		}

		/// <summary>
		/// Update the label to display the modified lattice size.
		/// </summary>
		private void UpdateNewCanvasSizeLabel()
		{
			Size Size = RecalculateCanvasSize();
			lblCanvasSize.Text = string.Format(SIZE_FMT, Size.Width, Size.Height);
		}

		/// <summary>
		/// Run the validation to make sure the entered data is valid and correct
		/// </summary>
		/// <returns></returns>
		private bool ValidateControls()
		{
			errorProvider1.Clear();
			bool IsValid = true;

			if (cboTargetChannel.SelectedIndex < 0)
			{
				errorProvider1.SetError(cboTargetChannel, "You must select a channel.");
				cboTargetChannel.Focus();
				IsValid = false;
			}

			return IsValid;
		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Selected new target Channel.
		/// </summary>
		private void cboTargetChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboTargetChannel.SelectedIndex >= 0)
				_selectedChannelID = Int32.Parse(_listBoxUtil.GetKey(cboTargetChannel));
			else
				_selectedChannelID = -1;

			if ((_editMap != null) && (_selectedChannelID >= 0))
				_editMap.TargetChannel = Profile.Channels.Get(_selectedChannelID);

			UpdateButtons();
			UpdatePreview();
		}
		
		/// <summary>
		/// Indicates whether the target Channel's cells should be removed prior to importing.
		/// </summary>
		private void chkClear_CheckedChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
				_editMap.ClearTargetChannel = chkClear.Checked;
			UpdatePreview();
		}

		/// <summary>
		/// Indicates whether the imported channel's color should be used instead of the target channel's color.
		/// </summary>
		private void chkOverrideColor_CheckedChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
				_editMap.OverrideColor = chkOverrideColor.Checked;
			UpdatePreview();
		}

		/// <summary>
		/// Indicates whether the imported channel's name should be used instead of the target channel's name.
		/// </summary>
		private void chkOverrideName_CheckedChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
				_editMap.OverrideName = chkOverrideName.Checked;
			UpdatePreview();
		}

		/// <summary>
		/// Toggle the Preview window visible and hidden
		/// </summary>
		private void chkPreview_CheckedChanged(object sender, EventArgs e)
		{
			if (chkPreview.Checked)
			{
				chkPreview.Image = ElfRes.visible;
				_frmPreview.Location = new Point(Location.X + Width + 5, Location.Y);
				_frmPreview.Visible = true;
				UpdatePreview();
			}
			else
			{
				chkPreview.Image = ImageHandler.AddAnnotation(ElfRes.visible, Annotation.Delete);
				_frmPreview.Visible = false;
			}
		}

		/// <summary>
		/// Indicates whether this mapping should force a change in the Canvas Size if the imported data (and its offset) doesn't fit completely on the current lattice.
		/// </summary>
		private void chkResize_CheckedChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
				_editMap.ResizeLattice = chkResize.Checked;
			RecalculateCanvasSize();
			UpdateNewCanvasSizeLabel();
			UpdatePreview();
		}

		/// <summary>
		/// Allows the user to create a new mapping.
		/// </summary>
		private void cmdAdd_Click(object sender, EventArgs e)
		{
			if (pnlControls.Enabled && (_selectedChannelID >= 0) && (lstImportedChannels.SelectedIndex >= 0))
			{
				if (MessageBox.Show(_frmPreview, "Save changes to the mapping that is being " + ((_editID > 0) ? "edited" : "added") + "?", "Add New Mapping", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					SaveMapping();
					ClearControls();
				}
			}

			RemoveEditMap();
			AddEditMap(new Mapping(), true);

			ClearControls();
			pnlControls.Enabled = true;
			lstImportedChannels.Enabled = true;
			lstImportedChannels.Focus();
			cmdMap.Enabled = false;
		}

		/// <summary>
		/// Cancels out the current mapping that is being edited.
		/// </summary>
		private void cmdCancelEdits_Click(object sender, EventArgs e)
		{
			ClearControls();
			pnlControls.Enabled = false;
			lstImportedChannels.Enabled = false;
			UpdateButtons();
			_frmPreview.UpdateDisplay();
		}
		
		/// <summary>
		/// Move the imported Channel's offset down by one.
		/// </summary>
		private void cmdDown_Click(object sender, EventArgs e)
		{
			tmrMove.Enabled = false;
			tmrHold.Enabled = false;
			if (txtY.TextLength == 0)
				txtY.Text = "1";
			else
			{
				int Y = Convert.ToInt32(txtY.Text) + 1;
				txtY.Text = Y.ToString();
			}
		}

		/// <summary>
		/// Edits the currently selected mapping.
		/// </summary>
		private void cmdEdit_Click(object sender, EventArgs e)
		{
			if (lstMapping.SelectedIndex <= 0)
				return;

			// Get the current mapping.
			_editMap.EditFlag = true;

			pnlControls.Enabled = true;
			lstImportedChannels.Enabled = true;
			LoadControlsFromMapping(_editMap);
			lstImportedChannels.Focus();
		}

		/// <summary>
		/// Deletes the currently selected mapping out of the list.
		/// </summary>
		private void cmdDelete_Click(object sender, EventArgs e)
		{
			if (lstMapping.SelectedIndex <= 0)
				return;

			if (MessageBox.Show(_frmPreview, "Delete this mapping?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				return;

			int Index = lstMapping.SelectedIndex;
			_mapList.RemoveAt(Index);
			lstMapping.Items.RemoveAt(Index);

			if (Index >= lstMapping.Items.Count)
				Index--;

			lstMapping.SelectedIndex = Index;
		}

		/// <summary>
		/// Move the imported Channel's offset left by one.
		/// </summary>
		private void cmdLeft_Click(object sender, EventArgs e)
		{
			tmrMove.Enabled = false;
			tmrHold.Enabled = false;
			if (txtX.TextLength == 0)
				txtX.Text = "-1";
			else
			{
				int X = Convert.ToInt32(txtX.Text) - 1;
				txtX.Text = X.ToString();
			}
		}

		/// <summary>
		/// Save these settings to a new (or existing) mapping
		/// </summary>
		private void cmdMap_Click(object sender, EventArgs e)
		{
			if (!ValidateControls())
				return;
			bool NewMapping = (_editID == -1);
			SaveMapping();

			if (NewMapping)
			{
				RemoveEditMap();
				AddEditMap(new Mapping(), true);

				_noUpdate = true;

				_editMap.ClearTargetChannel = chkClear.Checked;
				_editMap.OverrideName = chkOverrideName.Checked;
				_editMap.OverrideColor = chkOverrideColor.Checked;

				// Move down 1 notch for channels
				if (lstImportedChannels.SelectedIndex < (lstImportedChannels.Items.Count - 1))
					lstImportedChannels.SelectedIndex++;
				else
					NewMapping = false;
				if (cboTargetChannel.SelectedIndex < (cboTargetChannel.Items.Count - 1))
					cboTargetChannel.SelectedIndex++;
				else
					NewMapping = false;
				
				_noUpdate = false;
				UpdatePreview();
			}

			if (!NewMapping)
			{
				pnlControls.Enabled = false;
				lstImportedChannels.Enabled = false;
				HighlightSelectedMap();
			}
		}

		/// <summary>
		/// Get rid of the Edit Mapping (if it exists) and return a DialogResult of OK
		/// </summary>
		private void cmdOk_Click(object sender, EventArgs e)
		{
			RemoveEditMap();
		}

		/// <summary>
		/// Move the imported Channel's offset right by one.
		/// </summary>
		private void cmdRight_Click(object sender, EventArgs e)
		{
			tmrMove.Enabled = false;
			tmrHold.Enabled = false;
			if (txtX.TextLength == 0)
				txtX.Text = "1";
			else
			{
				int X = Convert.ToInt32(txtX.Text) + 1;
				txtX.Text = X.ToString();
			}
		}

		/// <summary>
		/// Move the imported Channel's offset up by one.
		/// </summary>
		private void cmdUp_Click(object sender, EventArgs e)
		{
			tmrMove.Enabled = false;
			tmrHold.Enabled = false;
			if (txtY.TextLength == 0)
				txtY.Text = "-1";
			else
			{
				int Y = Convert.ToInt32(txtY.Text) - 1;
				txtY.Text = Y.ToString();
			}
		}

		///// <summary>
		///// Selected new target Channel.
		///// </summary>
		//private void ddlTargetChannel_Channel_Click(object sender, EventArgs e)
		//{
		//	ToolStripMenuItem MenuItem = (ToolStripMenuItem)sender;
		//	_selectedChannelID = Convert.ToInt32(MenuItem.Tag);
			
		//	if ((_editMap != null) && (_selectedChannelID >= 0))
		//		_editMap.TargetChannel = Profile.Channels.Get(_selectedChannelID);
			
		//	SetDropDownMenuSelected(MenuItem);
		//	UpdateButtons();
		//	UpdatePreview();
		//}

		/// <summary>
		/// User is currently holding one of the arrow keys down. Determine which key and start the "holding" timer.
		/// </summary>
		private void DirectionButton_MouseDown(object sender, MouseEventArgs e)
		{
			switch (((Button)sender).Name)
			{
				case "cmdUp":
					dirButton = Direction.Up;
					break;
				case "cmdDown":
					dirButton = Direction.Down;
					break;
				case "cmdLeft":
					dirButton = Direction.Left;
					break;
				case "cmdRight":
					dirButton = Direction.Right;
					break;
			}
			tmrHold.Enabled = true;
		}

		/// <summary>
		/// Cursor has left the area of the button. If the Move timer had been enabled, then disable it
		/// </summary>
		private void DirectionButton_MouseLeave(object sender, EventArgs e)
		{
			tmrMove.Enabled = false;
		}

		/// <summary>
		/// This form has closed, make sure the preview window gets closed as well.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_frmPreview != null)
			{
				_frmPreview.Close();
				_frmPreview = null;
			}
			if (_importProfile != null)
			{
				_importProfile.Dispose();
				_importProfile = null;
			}
			_editMap = null;
			_listBoxUtil = null;
		}

		/// <summary>
		/// If the user did not click OK, verify they actually did want to leave this window without importing the data.
		/// </summary>
		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ((DialogResult != DialogResult.OK) && (lstMapping.Items.Count > 0))
			{
				if (MessageBox.Show(_frmPreview, "Are you sure you want to abort importing this Profile?\nThese settings will not be saved.", 
									"Cancel Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					e.Cancel = true;
			}
		}

		/// <summary>
		/// Intercept keys being pressed. If it is the plus key, then cause the Add button to be clicked
		/// </summary>
		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Oemplus) || (e.KeyCode == Keys.Add))
				cmdAdd_Click(sender, new EventArgs());
		}

		/// <summary>
		/// This form has moved, move the preview window along with it.
		/// </summary>
		private void Form_Move(object sender, EventArgs e)
		{
			_frmPreview.Location = new Point(Location.X + Width + 5, Location.Y);
		}

		/// <summary>
		/// A new import Channel has been selected.
		/// </summary>
		private void lstImportedChannels_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((_editMap != null) && (lstImportedChannels.SelectedIndex >= 0))
			{
				_editMap.ImportedChannel = (Channel)((ImageListItem)lstImportedChannels.Items[lstImportedChannels.SelectedIndex]).Tag;
				_editMap.ImportedChannel_Bounds = _editMap.ImportedChannel.GetBounds();
			}

			UpdateButtons();
			RecalculateCanvasSize();
			UpdateNewCanvasSizeLabel();
			UpdatePreview();
		}

		/// <summary>
		/// Simulate the effect of the user clicking the Edit key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstMapping_DoubleClick(object sender, EventArgs e)
		{
			cmdEdit_Click(sender, e);
		}

		/// <summary>
		/// A mapping has been selected. Update the edit controls with the new mapping's data, along with the preview window. If we were currently editing a different mapping, then
		/// prompt to see if the user wants to save the work first.
		/// </summary>
		private void lstMapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loadingMapList)
				return;

			HighlightSelectedMap();
		}

		/// <summary>
		/// Only allow digits and the minus sign in the textbox.
		/// </summary>
		private void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) ||
				char.IsControl(e.KeyChar) ||
				(e.KeyChar == '-') ||
				(e.KeyChar == '+'))
			{
				// We like these, do nothing
				e.Handled = false;
			}
			else
				e.Handled = true;
		}

		/// <summary>
		/// User has held the mouse down on a button for a given length of time, disable this timer and start the Move timer.
		/// </summary>
		private void tmrHold_Tick(object sender, EventArgs e)
		{
			tmrHold.Enabled = false;
			tmrMove.Enabled = true;
		}

		/// <summary>
		/// User has held the mouse down on a move button for a while, change the value of the offset textboxes according to which
		/// button was being held. Do not disable this timer. When the user releases or moves off the button, then disabled the timer.
		/// </summary>
		private void tmrMove_Tick(object sender, EventArgs e)
		{
			switch (dirButton)
			{
				case Direction.Up:
					_editMap.ImportedOffset.Y--;
					txtY.Text = _editMap.ImportedOffset.Y.ToString();
					break;
				case Direction.Down:
					_editMap.ImportedOffset.Y++;
					txtY.Text = _editMap.ImportedOffset.Y.ToString();
					break;
				case Direction.Left:
					_editMap.ImportedOffset.X--;
					txtX.Text = _editMap.ImportedOffset.X.ToString();
					break;
				case Direction.Right:
					_editMap.ImportedOffset.X++;
					txtX.Text = _editMap.ImportedOffset.X.ToString();
					break;
			}
		}

		/// <summary>
		/// X value of the offset has changed. Update the value in the mapping and inform the preview window to update.
		/// </summary>
		private void txtX_TextChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
			{
				if (txtX.TextLength > 0)
					_editMap.ImportedOffset.X = Convert.ToInt32(txtX.Text);
				else
					_editMap.ImportedOffset.X = 0;
			}
			UpdatePreview();
		}

		/// <summary>
		/// Y value of the offset has changed. Update the value in the mapping and inform the preview window to update.
		/// </summary>
		private void txtY_TextChanged(object sender, EventArgs e)
		{
			if (_editMap != null)
			{
				if (txtY.TextLength > 0)
					_editMap.ImportedOffset.Y = Convert.ToInt32(txtY.Text);
				else
					_editMap.ImportedOffset.Y = 0;
			}
			UpdatePreview();
		}
		
		#endregion [ Events ]

	}
}

