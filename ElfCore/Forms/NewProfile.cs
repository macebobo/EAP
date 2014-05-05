using ElfControls;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class NewProfile : Form
	{

		#region [ Private Variables ]

		private ChannelList _channels = new ChannelList();
		private int _nextID = 0;
		private Workshop _workshop = Workshop.Instance;
		private ListBoxUtil _listBoxUtil = null;
		private bool _editMode = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Channels defined for the new Profile
		/// </summary>
		public ChannelList Channels
		{
			get { return _channels; }
		}

		/// <summary>
		/// Name of the new profile.
		/// </summary>
		public string ProfileName
		{
			get { return txtName.Text; }
			//set { txtName.Text = value; }
		}

		/// <summary>
		/// Name of the file the Profile will be saved in.
		/// </summary>
		public string Filename
		{
			get { return lblFilename.Text; }
		}

		/// <summary>
		/// Selected ProfileType
		/// </summary>
		public Type ProfileType
		{
			get 
			{
				int Index = cboFormat.SelectedIndex;
				return _workshop.AvailableProfiles[Index].Profile.GetType();
			}
		}

		/// <summary>
		/// Width of the Canvas in Cells.
		/// </summary>
		public int CanvasWidth
		{
			get { return Int32.Parse(txtWidth.Text); }
		}

		/// <summary>
		/// Height of the Canvas in Cells.
		/// </summary>
		public int CanvasHeight
		{
			get { return Int32.Parse(txtHeight.Text); }
		}

		/// <summary>
		/// Cell size in pixels.
		/// </summary>
		public int CellSize
		{
			get 
			{
				return Int32.Parse(_listBoxUtil.GetKey(cboCellSize));
			}
		}

		/// <summary>
		/// Show grid lines between Cells.
		/// </summary>
		public bool ShowGridLines
		{
			get { return chkShowGridLines.Checked; }
			set { chkShowGridLines.Checked = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public NewProfile()
		{
			InitializeComponent();
			_listBoxUtil = new ListBoxUtil();

			cmdEdit.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Edit, new Point(1, 0));
			cmdAddOne.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.PlusOne, new Point(3, 0));
			cmdAddMany.Image = ImageHandler.AddAnnotation(ElfRes.channels, Annotation.Add, new Point(1, 0));
			cmdRemoveChannel.Image = ImageHandler.AddAnnotation(ElfRes.channel, Annotation.Delete, new Point(3, 0));

			cboCellSize.Items.Add(new ImageListItem("1x1", 1, ElfRes.pixel_1));
			cboCellSize.Items.Add(new ImageListItem("2x2", 2, ElfRes.pixel_2));
			cboCellSize.Items.Add(new ImageListItem("3x3", 3, ElfRes.pixel_3));
			cboCellSize.Items.Add(new ImageListItem("4x4", 4, ElfRes.pixel_4));
			cboCellSize.Items.Add(new ImageListItem("5x5", 5, ElfRes.pixel_5));
			cboCellSize.Items.Add(new ImageListItem("6x6", 6, ElfRes.pixel_6));
			cboCellSize.Items.Add(new ImageListItem("7x7", 7, ElfRes.pixel_7));
			cboCellSize.Items.Add(new ImageListItem("8x8", 8, ElfRes.pixel_8));
			cboCellSize.Items.Add(new ImageListItem("9x9", 9, ElfRes.pixel_9));
			cboCellSize.Items.Add(new ImageListItem("10x10", 10, ElfRes.pixel_10));
			_listBoxUtil.Set(cboCellSize, 7);

			// Populate the combo box with all the available Profiles
			for (int i = 0; i < _workshop.AvailableProfiles.Count; i++)
			{
				_listBoxUtil.Add(cboFormat, new ListBoxUtil.Item(_workshop.AvailableProfiles[i].Profile.FormatName, i));
			}
			if (cboFormat.Items.Count > 0)
				cboFormat.SelectedIndex = 0;

			UpdateFilename();

			UpdateButtons();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Determine the final size of the Canvas window in pixels.
		/// </summary>
		private void CalculateSizeInPixels()
		{
			int Width, Height, CellSize;

			CellSize = cboCellSize.SelectedIndex + 1;
			if (!Int32.TryParse(txtWidth.Text, out Width))
				Width = 0;
			if (!Int32.TryParse(txtHeight.Text, out Height))
				Height = 0;

			Width *= (CellSize + (chkShowGridLines.Checked ? 1 : 0));
			Height *= (CellSize + (chkShowGridLines.Checked ? 1 : 0));

			if (chkShowGridLines.Checked)
			{
				Width++;
				Height++;
			}
			lblPixelWidth.Text = Width.ToString();
			lblPixelHeight.Text = Height.ToString();
		}

		/// <summary>
		/// Grabs the list of all the selected items from a listbox and returns it.
		/// </summary>
		/// <param name="listBox">ListBox to grab selected Items from</param>
		/// <returns>List of ElfControls.IconListItem objects</returns>
		private List<ImageListItem> GetSelectedItems(ImageListBox listBox)
		{
			List<ImageListItem> SelectedItems = new List<ImageListItem>();
			foreach (ImageListItem LItem in listBox.SelectedItems)
			{
				SelectedItems.Add(LItem);
			}
			return SelectedItems;
		}

		private void Remove(int index)
		{
			_channels.Remove((Channel)((ImageListItem)(lstChannels.Items[index])).Tag);
			lstChannels.Items.RemoveAt(index);
		}

		/// <summary>
		/// Updates the enabled property of various buttons on this form based on items selected in the listbox.
		/// </summary>
		private void UpdateButtons()
		{
			// If we are in edit mode, disable all other controls until we leave this mode.
			cmdUp.Enabled = !_editMode;
			cmdDown.Enabled = !_editMode;
			cmdToBottom.Enabled = !_editMode;
			cmdToTop.Enabled = !_editMode;
			cmdOk.Enabled = !_editMode;
			cmdCancel.Enabled = !_editMode;
			cmdAddMany.Enabled = !_editMode;
			cmdAddOne.Enabled = !_editMode;
			cmdEdit.Enabled = !_editMode;
			cmdRemoveChannel.Enabled = !_editMode;
			_cboFormat.Enabled = !_editMode;
			_txtCellSize.Enabled = !_editMode;
			_txtName.Enabled = !_editMode;
			_txtWidth.Enabled = !_editMode;
			_txtHeight.Enabled = !_editMode;
			cboFormat.Enabled = !_editMode;
			cboCellSize.Enabled = !_editMode;
			txtHeight.Enabled = !_editMode;
			txtName.Enabled = !_editMode;
			txtNumChannels.Enabled = !_editMode;
			txtWidth.Enabled = !_editMode;
			chkShowGridLines.Enabled = !_editMode;

			if (_editMode)
				return;

			bool HasChannels = (lstChannels.Items.Count > 0);

			cmdOk.Enabled = HasChannels;
			cmdEdit.Enabled = HasChannels;
			cmdRemoveChannel.Enabled = HasChannels;

			if (!HasChannels)
			{
				cmdDown.Enabled = false;
				cmdToBottom.Enabled = false;
				cmdUp.Enabled = false;
				cmdToTop.Enabled = false;
				return;
			}			

			cmdUp.Enabled = (lstChannels.SelectedIndex > 0);
			cmdToTop.Enabled = cmdUp.Enabled;

			cmdDown.Enabled = (lstChannels.SelectedIndex < lstChannels.Items.Count - 1);
			cmdToBottom.Enabled = cmdDown.Enabled;

		}

		/// <summary>
		/// Determines the new Profile's filename by getting the default Path and extention from the selected format
		/// and adding them to the name of the Profile.
		/// </summary>
		private void UpdateFilename()
		{
			string Path = _workshop.AvailableProfiles[cboFormat.SelectedIndex].Profile.DefaultSavePath;
			string Ext = _workshop.AvailableProfiles[cboFormat.SelectedIndex].Profile.FileExtension;

			if (txtName.TextLength == 0)
				lblFilename.Text = string.Empty;
			else
				lblFilename.Text = Path + txtName.Text.Trim() + "." + Ext;
		}

		/// <summary>
		/// Run the validation to make sure the entered data is valid and correct
		/// </summary>
		/// <returns></returns>
		private bool ValidateControls()
		{
			errorProvider1.Clear();
			bool IsValid = true;

			if (txtName.TextLength == 0)
			{
				errorProvider1.SetError(txtName, "You must give your Profile a name.");
				txtName.Focus();
				IsValid = false;
			}

			if ((txtWidth.TextLength == 0) || (txtWidth.Text == "0"))
			{
				errorProvider1.SetError(txtWidth, "You must specify a canvas width.");
				if (IsValid)
					txtWidth.SelectAll();
				IsValid = false;
			}

			if ((txtHeight.TextLength == 0) || (txtHeight.Text == "0"))
			{
				errorProvider1.SetError(txtHeight, "You must specify a canvas height.");
				if (IsValid)
					txtHeight.Focus();
				IsValid = false;
			}

			if (_channels.Count == 0)
			{
				errorProvider1.SetError(lstChannels, "You must create at least 1 Channel.");
				IsValid = false;
			}

			return IsValid;
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void cmdAddMany_Click(object sender, EventArgs e)
		{
			if (txtNumChannels.TextLength == 0)
			{
				errorProvider1.SetError(txtNumChannels, "You must enter the number of Channels you wish to add.");
				txtNumChannels.Focus();
				return;
			}
			int Num = Convert.ToInt32(txtNumChannels.Text);
			for (int i = 0; i < Num; i++)
				cmdAddOne_Click(sender, e);
			UpdateButtons();
		}

		private void cmdAddOne_Click(object sender, EventArgs e)
		{
			Channel Channel = new Channel();
			Channel.ID = _nextID++;
			Channel.Name = "Channel " + (Channel.ID + 1);
			Channel.SequencerColor = Color.White;
			_channels.Add(Channel);
			ImageListItem Item = new ImageListItem(Channel.ToString(), Channel.ID.ToString(), 
					ImageHandler.ColorSwatches[ImageHandler.CreateAndAddColorSwatch(new Swatch(Channel.GetColor()))], Channel);
			lstChannels.Items.Add(Item);

			Item = null;
			Channel = null;
			UpdateButtons();
		}

		private void cmdCancelEdit_Click(object sender, EventArgs e)
		{
			lstChannels.Enabled = true;
			pnlEditChannel.Visible = false;
			_editMode = false;
			UpdateButtons();
		}

		private void cmdDown_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstChannels);
			int Index = lstChannels.SelectedIndex;

			for (int i = lstChannels.Items.Count - 1; i >= 0; i--)
			{
				if (!lstChannels.SelectedIndices.Contains(i))
					continue;
				if (i == lstChannels.Items.Count - 1)
					continue;
				if (lstChannels.SelectedIndices.Contains(i + 1))
					continue;
				Item = (ImageListItem)lstChannels.Items[i];
				lstChannels.Items.Remove(Item);
				lstChannels.Items.Insert(i + 1, Item);
			}
			if (Index < lstChannels.Items.Count - 1)
				lstChannels.SelectedIndex = Index + 1;
			UpdateButtons();
		}

		private void cmdEdit_Click(object sender, EventArgs e)
		{
			if (lstChannels.SelectedIndex < 0)
				return;

			// First make sure the item is visible.
			Rectangle ItemBounds = lstChannels.GetItemRectangle(lstChannels.SelectedIndex);

			if (!lstChannels.ClientRectangle.IntersectsWith(ItemBounds))
			{
				lstChannels.TopIndex = lstChannels.SelectedIndex;
				ItemBounds = lstChannels.GetItemRectangle(lstChannels.SelectedIndex);
			}

			pnlEditChannel.Location = new Point(ItemBounds.Location.X + lstChannels.Location.X, ItemBounds.Location.Y + lstChannels.Location.Y);
			pnlEditChannel.Width = ItemBounds.Width;

			Channel Channel = (Channel)((ImageListItem)(lstChannels.SelectedItem)).Tag;
			cddEditColor.Color = Channel.SequencerColor;
			txtChannelName.Text = Channel.Name;
			pnlEditChannel.Visible = true;
			lstChannels.Enabled = false;
			Channel = null;
			_editMode = true;
			UpdateButtons();
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			if (!ValidateControls())
				return;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cmdRemoveChannel_Click(object sender, EventArgs e)
		{
			if (lstChannels.SelectedIndex < 0)
				return;

			List<ImageListItem> SelectedItems = GetSelectedItems(lstChannels);
			int Index = lstChannels.SelectedIndex;

			for (int i = SelectedItems.Count - 1; i >= 0; i--)
			{
				Remove(lstChannels.Items.IndexOf(SelectedItems[i]));				
			}

			if (lstChannels.Items.Count > 0)
			{
				if (Index < lstChannels.Items.Count)
					lstChannels.SelectedIndex = Index;
				else
					lstChannels.SelectedIndex = lstChannels.Items.Count - 1;
			}
			UpdateButtons();
		}

		private void cmdSaveEdit_Click(object sender, EventArgs e)
		{
			if (!_editMode)
				return;
			Channel Channel = (Channel)((ImageListItem)lstChannels.SelectedItem).Tag;
			Channel.Name = txtChannelName.Text;
			Channel.SequencerColor = cddEditColor.Color;
			ImageListItem Item = (ImageListItem)lstChannels.SelectedItem;
			Item.Text = txtChannelName.Text;
			Item.Image = ImageHandler.ColorSwatches[ImageHandler.CreateAndAddColorSwatch(new Swatch(Channel.GetColor()))];

			lstChannels.Enabled = true;
			pnlEditChannel.Visible = false;
			lstChannels.Refresh();
			_editMode = false;
			UpdateButtons();
		}

		private void cmdToBottom_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstChannels);

			int Index = lstChannels.Items.Count - 1;
			for (int i = 0; i < SelectedItems.Count; i++)
			{
				Item = (ImageListItem)SelectedItems[i];
				lstChannels.Items.Remove(Item);
				lstChannels.Items.Insert(Index, Item);
			}
			lstChannels.SelectedIndex = lstChannels.Items.Count - 1;
			UpdateButtons();
		}

		private void cmdToTop_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstChannels);

			int Index = 0;
			for (int i = 0; i < lstChannels.Items.Count; i++)
			{
				if (!lstChannels.SelectedIndices.Contains(i))
					continue;
				Item = (ImageListItem)lstChannels.Items[i];
				lstChannels.Items.Remove(Item);
				lstChannels.Items.Insert(Index++, Item);
			}
			lstChannels.SelectedIndex = 0;
			UpdateButtons();
		}

		private void cmdUp_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstChannels);
			int Index = lstChannels.SelectedIndex;

			for (int i = 1; i < lstChannels.Items.Count; i++)
			{
				if (!lstChannels.SelectedIndices.Contains(i))
					continue;
				if (lstChannels.SelectedIndices.Contains(i - 1))
					continue;
				Item = (ImageListItem)lstChannels.Items[i];
				lstChannels.Items.Remove(Item);
				lstChannels.Items.Insert(i - 1, Item);
			}
			if (Index > 0)
				lstChannels.SelectedIndex = Index - 1;
						
			UpdateButtons();
		}

		private void lstChannels_DoubleClick(object sender, EventArgs e)
		{
			cmdEdit_Click(sender, e);
		}

		private void lstChannels_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		/// <summary>
		/// Only allow the character 0 through 9 in this textbox.
		/// </summary>
		private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b')
				e.Handled = ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)));
		}

		private void TextBox_Enter(object sender, EventArgs e)
		{
			//errorProvider1.Clear();
			((TextBox)sender).SelectAll();
		}

		private void Size_TextChanged(object sender, EventArgs e)
		{
			CalculateSizeInPixels();
		}

		private void chkShowGridLines_CheckedChanged(object sender, EventArgs e)
		{
			CalculateSizeInPixels();
		}

		private void cboCellSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			CalculateSizeInPixels();
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			UpdateFilename();
		}

		private void cboFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateFilename();
		}

		#endregion [ Events ]


	}
}

