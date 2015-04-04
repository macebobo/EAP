using System.IO;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ElfCore.Forms
{
	partial class Editor
	{
		#region [ Channel/Context Menu Events ]

		/// <summary>
		/// Adds a new Channel to the Profile.
		/// </summary>
		internal void Channel_AddNew_Click(object sender, EventArgs e)
		{
			EditChannel frmNewChannel = new EditChannel();
			frmNewChannel.Text = "Add New Channel";
			frmNewChannel.ChannelName = "[New Channel]";
			frmNewChannel.SequencerColor = Color.White;
			frmNewChannel.RenderColor = Color.Empty;
			frmNewChannel.BorderColor = Color.Empty;
			frmNewChannel.IsEnabled = true;
			frmNewChannel.IsLocked = false;
			frmNewChannel.IsVisible = true;

			if (frmNewChannel.ShowDialog() == DialogResult.Cancel)
				return;

			Channel NewChannel = new Channel();
			NewChannel.Name = frmNewChannel.ChannelName;
			NewChannel.RenderColor = frmNewChannel.RenderColor;
			NewChannel.SequencerColor = frmNewChannel.SequencerColor;
			NewChannel.BorderColor = frmNewChannel.BorderColor;
			NewChannel.Enabled = frmNewChannel.IsEnabled;
			NewChannel.Locked = frmNewChannel.IsLocked;
			NewChannel.Visible = frmNewChannel.IsVisible;
			NewChannel.ID = -1;

			Profile.Channels.Add(NewChannel);
			Profile.SaveUndo("Add Channel");
		}

		/// <summary>
		/// Deletes the currently selected Channels from the Profile.
		/// </summary>
		internal void Channel_Delete_Click(object sender, EventArgs e)
		{
			string Message = "Really delete {0}?";
			bool Multiple = (Profile.Channels.Selected.Count > 1);
			if (Multiple)
				Message = string.Format(Message, "these Channels");
			else
				Message = string.Format(Message, "this Channel");
			if (MessageBox.Show(Message, "Delete Channel", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				return;

			foreach (Channel Channel in Profile.Channels.Selected)
				Profile.Channels.Remove(Channel);

			Profile.Refresh();
			Profile.SaveUndo("Delete Channel" + (Multiple ? "s" : string.Empty));
		}

		/// <summary>
		/// Makes all Channels visible
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void Channel_AllVisible_Click(object sender, EventArgs e)
		{
			Profile.Channels.SetVisible(Profile.Channels.List, true);
			Profile.Refresh();
		}

		/// <summary>
		/// Clear out the Lattice of all the Channels
		/// </summary>
		internal void Channel_Clear_AllChannels_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Really clear the Cells from ALL Channels?", "Clear All Channels", MessageBoxButtons.OKCancel, 
								MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
				return;

			Profile.Channels.EmptyAllChannels();
			Profile.SaveUndo("Clear All Channels");
		}

		/// <summary>
		/// Clear out the Lattice for the selected Channel(s)
		/// </summary>
		internal void Channel_Clear_Click(object sender, EventArgs e)
		{
			string PluralText = Profile.Channels.PluralText;
			string Title = Title = "Clear " + PluralText;

			if (MessageBox.Show("Really clear the Cells from " + ((Profile.Channels.Selected.Count == 1) ? string.Empty : "all ") + "the selected " + PluralText + "?", 
								Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
				return;

			Profile.Channels.EmptyChannels(Profile.Channels.Selected);
			Profile.SaveUndo(Title);
		}

		/// <summary>
		/// Disables the selected Channels.
		/// </summary>
		internal void Channel_Disable_Click(object sender, EventArgs e)
		{
			string PluralText = Profile.Channels.PluralText;
			string Title = string.Format("Disable {0}", PluralText);
			if (MessageBox.Show(string.Format("Really disable the selected {0}?", PluralText), Title,
								MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
				return;
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Enabled = false;
			Profile.SaveUndo(Title);
		}

		/// <summary>
		/// Set the selected Channels to be Enabled.
		/// </summary>
		internal void Channel_Enable_Click(object sender, EventArgs e)
		{
			string PluralText = Profile.Channels.PluralText;
			string Title = string.Format("Enable {0}", PluralText);
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Enabled = true;
			Profile.SaveUndo(Title);
		}

		/// <summary>
		/// Excludes the selected channels from this PlugIn
		/// </summary>
		internal void Channel_Exclude_Click(object sender, EventArgs e)
		{
			string PluralText = Profile.Channels.PluralText;
			string Title = string.Format("Exclude {0}", PluralText);
			if (MessageBox.Show(string.Format("Really exclude the selected {0}?", PluralText), Title,
								MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
				return;
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Included = false;
			Profile.SaveUndo(Title);
		}

		/// <summary>
		/// Groups the currently selected Channels together
		/// </summary>
		internal void Channel_Group_Click(object sender, EventArgs e)
		{
			MessageBox.Show("NYI");
		}
	
		/// <summary>
		/// Sets the currently selected Channels Visible property to false.
		/// </summary>
		internal void Channel_Hide_Click(object sender, EventArgs e)
		{
			Profile.Channels.SetVisible(Profile.Channels.Selected, false);
		}

		/// <summary>
		/// Sets the Visible property to false to all Channels not currently selected.
		/// </summary>
		internal void Channel_HideOthers_Click(object sender, EventArgs e)
		{
			Profile.Channels.SetVisible(Profile.Channels.Unselected, false);
		}

		/// <summary>
		/// Import Channel data from another Profile
		/// </summary>
		internal void Channel_Import_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			if (OpenProfileDialog.ShowDialog() == DialogResult.Cancel)
				return;

			ImportFromProfile frmImport = new ImportFromProfile();
			frmImport.Filename = OpenProfileDialog.FileName;
			
			if (frmImport.ShowDialog() == DialogResult.Cancel)
				return;

			MappingList MapList = frmImport.MappedChannels;

			Profile.Scaling.LatticeSize = frmImport.RecalculateCanvasSize();

			Channel TargetChannel = null;
			Channel ImportChannel = null;

			if (!frmImport.MaxSetBack.IsEmpty)
			{ 
				foreach(Channel Channel in Profile.Channels)
					Channel.MoveData(frmImport.MaxSetBack);
			}

			foreach (Mapping Mapping in MapList)
			{
				TargetChannel = Mapping.TargetChannel;
				ImportChannel = Mapping.ImportedChannel;

				if (Mapping.ClearTargetChannel)
					TargetChannel.Empty();

				if (Mapping.OverrideColor)
				{
					TargetChannel.SequencerColor = ImportChannel.SequencerColor;
					TargetChannel.RenderColor = ImportChannel.RenderColor;
				}

				if (Mapping.OverrideName)
					TargetChannel.Name = ImportChannel.Name;

				if (!Mapping.ImportedOffset.IsEmpty)
					ImportChannel.MoveData(Mapping.EffectiveOffset);

				TargetChannel.Paint(ImportChannel.Lattice);				
			}

			Profile.SaveUndo("Import Channel" + ((MapList.Count > 1) ? "s" : string.Empty));
			Profile.Refresh();
		}

		/// <summary>
		/// Includes the selected channels in this PlugIn
		/// </summary>
		internal void Channel_Include_Click(object sender, EventArgs e)
		{
			string Title = string.Format("Include {0}", Profile.Channels.PluralText);
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Included = true;
			Profile.SaveUndo(Title);
		}

		/// <summary>
		/// Import a bitmap from disk, overlay the white pixels onto the lattice of the active Channel.
		/// </summary>
		internal void Channel_LoadFromBitmap_Click(object sender, EventArgs e)
		{
			if (Profile == null)
				return;

			string Filename = string.Empty;
			Bitmap Bmp = LoadBitmap(out Filename);
			DialogResult Result;
			List<Point> Lattice = null;

			if (Bmp != null)
			{
				Result = MessageBox.Show("Overwrite cells in this Channel?", "Import Cells From Bitmap", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (Result == DialogResult.Cancel)
					return;

				Profile.Channels.Active.Visible = true;

				if (Result == DialogResult.No)
				{
					Lattice = new List<Point>();
					Lattice.AddRange(Profile.Channels.Active.Lattice);
				}

				Profile.Channels.Active.LatticeBuffer = Bmp;
				if (Lattice != null)
					Profile.Channels.Active.Paint(Lattice);

				Profile.SaveUndo("Load Channel From Bitmap");
				Profile.Refresh();
			}
			Bmp = null;
			Lattice = null;
		}

		/// <summary>
		/// Locks all the currently selected Channels.
		/// </summary>
		internal void Channel_Lock_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Locked = true;
		}
		
		/// <summary>
		/// Moves the selected Channels to the bottom of the current Shuffle
		/// </summary>
		internal void Channel_MoveToBottom_Click(object sender, EventArgs e)
		{
			Profile.Channels.ShuffleController.Active.ToBottom(Profile.Channels.GetSelectedChannelIDs());
			Profile.SaveUndo("Move To Bottom");
		}

		/// <summary>
		/// Moves the selected Channels to the top of the current Shuffle
		/// </summary>
		internal void Channel_MoveToTop_Click(object sender, EventArgs e)
		{
			Profile.Channels.ShuffleController.Active.ToTop(Profile.Channels.GetSelectedChannelIDs());
			Profile.SaveUndo("Move To Top");
		}

		/// <summary>
		/// Moves the selected Channels down one position within the current Shuffle
		/// </summary>
		internal void Channel_MoveDown_Click(object sender, EventArgs e)
		{
			Profile.Channels.ShuffleController.Active.MoveDown(Profile.Channels.GetSelectedChannelIDs());
			Profile.SaveUndo("Move Down");
		}

		/// <summary>
		/// Moves the selected Channels up one position within the current Shuffle
		/// </summary>
		internal void Channel_MoveUp_Click(object sender, EventArgs e)
		{
			Profile.Channels.ShuffleController.Active.MoveUp(Profile.Channels.GetSelectedChannelIDs());
			Profile.SaveUndo("Move Up");
		}

		/// <summary>
		/// Allows the user to view and change Channel properties directly.
		/// </summary>
		internal void Channel_Properties_Click(object sender, EventArgs e)
		{
			Channel Channel = Profile.Channels.Active;

			EditChannel frmChannel = new EditChannel();
			frmChannel.Text = "Channel Properties";
			frmChannel.ChannelName = Channel.Name;
			frmChannel.SequencerColor = Channel.SequencerColor;
			frmChannel.RenderColor = Channel.RenderColor;
			frmChannel.BorderColor = Channel.BorderColor;
			frmChannel.IsEnabled = Channel.Enabled;
			frmChannel.IsLocked = Channel.Locked;
			frmChannel.IsVisible = Channel.Visible;
			frmChannel.ReadOnlyMode = true;

			if (frmChannel.ShowDialog() == DialogResult.Cancel)
				return;
			//Forms.ChannelProperties frmProp = new Forms.ChannelProperties();
			//Channels.Properties ChProp = new Channels.Properties(Profile.Channels.Active);
			//frmProp.Properties = ChProp;
			//if (frmProp.ShowDialog() == DialogResult.Cancel)
			//	return;
			//ChProp.Save(Profile.Channels.Active);
			//Profile.SaveUndo("Change Channel Properties");
		}

		/// <summary>
		/// Allows the user to rename the Active Channel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void Channel_Rename_Click(object sender, EventArgs e)
		{
			_channelExplorer.RenameChannel();
		}

		/// <summary>
		/// Saves the selected Channel to a bitmap
		/// </summary>
		internal void Channel_SaveToBitmap_Click(object sender, EventArgs e)
		{
			string Temp = Profile.Name + "_" + Profile.Channels.Active.Name;
			string Filename = string.Empty;

			// strip out all invalid filename characters.
			Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
    
			string Letter = string.Empty;
			for (int i = 0; i < Temp.Length; i++)
			{
				Letter = Temp.Substring(i, 1);
				if (!containsABadCharacter.IsMatch(Letter))
					Filename += Letter;
			}

			Filename = Filename.Replace("/", "_").Replace("\\", "_");
			Filename = Filename.Trim() + ".png";

			Workshop.SaveBitmap(Profile.Channels.Active.LatticeBuffer, Filename, "Save Channel To Bitmap");
		}

		/// <summary>
		/// Saves a copy of the bitmap of the selected Channel to be the background image
		/// </summary>
		internal void Channel_SetAsBackground_Click(object sender, EventArgs e)
		{
			Color PaintColor = Profile.Channels.Active.GetColor();
			DialogResult Result;

			Result = MessageBox.Show("Setting an image of the current Channel as the background Image." + Environment.NewLine +
									 "Would you like to change the color of the Channel to another color before creating the image?" + Environment.NewLine +
									 "This will be for the image only and will not affect the actual Channel.",
									 @"Set Channel As Background Image",
									 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (Result == DialogResult.Cancel)
				return;

			if (Result == DialogResult.Yes)
			{
				ColorDialog.Color = PaintColor;
				ColorDialog.CustomColors = _workshop.CustomColors;
				if (ColorDialog.ShowDialog() != DialogResult.Cancel)
					PaintColor = ColorDialog.Color;
				_workshop.CustomColors = ColorDialog.CustomColors;
			}

			Profile.Background.Clear(false);
			Profile.Background.Image = Profile.Channels.Active.DrawChannel(PaintColor);
			Profile.Background.Set();
			Profile.SaveUndo("Set Channel As Background Image");
		}

		/// <summary>
		/// Set the color of the selected Channel for the sequencer.
		/// </summary>
		internal void Channel_ChangeSeqColor_Click(object sender, EventArgs e)
		{
			ColorDialog.Color = Profile.Channels.Active.SequencerColor;
			ColorDialog.CustomColors = _workshop.CustomColors;
			ColorDialog.Title = "Select Sequencing Color";

			if (ColorDialog.ShowDialog() == DialogResult.Cancel)
				return;
			
			_workshop.CustomColors = ColorDialog.CustomColors;
			Profile.Channels.SetSequencerColor(Profile.Channels.Selected, ColorDialog.Color);
			Profile.SaveUndo("Change Channel Sequencing Color");
		}

		/// <summary>
		/// Set the color of the selected Channel for the editor.
		/// </summary>
		internal void Channel_ChangeRendColor_Click(object sender, EventArgs e)
		{
			ColorDialog.Color = Profile.Channels.Active.GetColor();
			ColorDialog.CustomColors = _workshop.CustomColors;
			ColorDialog.Title = "Select Display Color";

			if (ColorDialog.ShowDialog() == DialogResult.Cancel)
				return;

			_workshop.CustomColors = ColorDialog.CustomColors;
			Profile.Channels.SetRenderColor(Profile.Channels.Selected, ColorDialog.Color);
			Profile.SaveUndo("Change Channel Display Color");
		}

		/// <summary>
		/// Sets the currently selected Channels Visible property to true.
		/// </summary>
		internal void Channel_Show_Click(object sender, EventArgs e)
		{
			Profile.Channels.SetVisible(Profile.Channels.Selected, true);
			Profile.Refresh();
		}

		/// <summary>
		/// Sets all Channels Visible property to true.
		/// </summary>
		internal void Channel_ShowAll_Click(object sender, EventArgs e)
		{
			Profile.Channels.SetVisible(Profile.Channels.List, true);
			Profile.Refresh();
		}

		/// <summary>
		/// Ungroups all groups that are currently selected
		/// </summary>
		internal void Channel_Ungroup_Click(object sender, EventArgs e)
		{
			MessageBox.Show("NYI");
		}

		/// <summary>
		/// Unlocks all the currently selected Channels.
		/// </summary>
		internal void Channel_Unlock_Click(object sender, EventArgs e)
		{
			foreach (Channel Channel in Profile.Channels.Selected)
				Channel.Locked = false;
		}

		/// <summary>
		/// Brings up the Shuffle Editor to create a new Shuffle. If accepted, adds it to the list and makes it the active Shuffle.
		/// </summary>
		internal void Channel_Shuffle_New_Click(object sender, EventArgs e)
		{
			ChannelList Channels = Profile.Channels.GetAllChannels();
			EditShuffle frmEdit = new EditShuffle(null, Channels);
			frmEdit.Text = "Create New Sort Order";

			if (frmEdit.ShowDialog() == DialogResult.Cancel)
				return;

			Profile.Channels.AddShuffle(frmEdit.Shuffle);
			_channelExplorer.BuildShuffleList();
			Profile.SaveUndo("Add New Sort Order");
		}

		/// <summary>
		/// Delete the currently selected Shuffle.
		/// </summary>
		internal void Channel_Shuffle_Delete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Delete this Sort Order?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				return;
			Profile.Channels.DeleteShuffle(Profile.Channels.GetCurrentShuffle());
			_channelExplorer.BuildShuffleList();
			Profile.SaveUndo("Delete Sort Order");
		}

		/// <summary>
		/// Edit the currently selected Shuffle.
		/// </summary>
		internal void Channel_Shuffle_Edit_Click(object sender, EventArgs e)
		{
			Shuffle SO = Profile.Channels.GetCurrentShuffle();
			ChannelList Channels = Profile.Channels.GetAllChannels();
			EditShuffle frmEdit = new EditShuffle(SO, Channels);

			if (frmEdit.ShowDialog() == DialogResult.Cancel)
				return;

			Profile.Channels.UpdateShuffle(frmEdit.Shuffle);
			_channelExplorer.BuildShuffleList();
			Profile.SaveUndo("Edit Sort Order");
		}

		#endregion [ Channel/Context Menu Events ]

		/// <summary>
		/// Enables and disables menu items for the Editor Channel menu and the ChannelExplorer Context menu based on the conditions of the 
		/// currently selected Channels.
		/// </summary>
		internal void UpdateChannelContextMenu()
		{
			string PluralText = Profile.Channels.PluralText;

			Channel_Clear.Text = "Clear " + PluralText;
			Channel_Delete.Text = "Remove " + PluralText;
			Channel_Hide.Text = "Hide " + PluralText;
			Channel_Show.Text = "Show " + PluralText;
			Channel_Group.Text = "Group " + PluralText;
			Channel_Ungroup.Text = "Ungroup " + PluralText;
			Channel_Lock.Text = "Lock " + PluralText;
			Channel_Unlock.Text = "Unlock " + PluralText;
			Channel_Disable.Text = "Disable " + PluralText;
			Channel_Enable.Text = "Enable " + PluralText;
			Channel_Include.Text = "Include " + PluralText;
			Channel_Exclude.Text = "Exclude " + PluralText;

			_channelExplorer.Context_Clear.Text = "Clear " + PluralText;
			_channelExplorer.Context_Delete.Text = "Remove " + PluralText;
			_channelExplorer.Context_Hide.Text = "Hide " + PluralText;
			_channelExplorer.Context_Show.Text = "Show " + PluralText;
			_channelExplorer.Context_Group.Text = "Group " + PluralText;
			_channelExplorer.Context_Ungroup.Text = "Ungroup " + PluralText;
			_channelExplorer.Context_Lock.Text = "Lock " + PluralText;
			_channelExplorer.Context_Unlock.Text = "Unlock " + PluralText;
			_channelExplorer.Context_Disable.Text = "Disable " + PluralText;
			_channelExplorer.Context_Enable.Text = "Enable " + PluralText;
			_channelExplorer.Context_Include.Text = "Include " + PluralText;
			_channelExplorer.Context_Exclude.Text = "Exclude " + PluralText;

			DisplaySet Set = Profile.Channels.GetDisplaySet();
						
			Channel_MoveUp.Enabled = Set.CanMoveUp;
			Channel_MoveToTop.Enabled = Set.CanMoveUp;
			Channel_MoveDown.Enabled = Set.CanMoveDown;
			Channel_MoveToBottom.Enabled = Set.CanMoveDown;
			Channel_Rename.Enabled = Set.HasUnlocked;
			Channel_ChangeSeqColor.Enabled = Set.HasUnlocked;
			Channel_Group.Enabled = Set.HasUnlocked;
			Channel_Ungroup.Enabled = Set.HasUnlocked;
			Channel_Unlock.Enabled = Set.HasLocked;
			Channel_Lock.Enabled = Set.HasUnlocked;
			Channel_Hide.Enabled = Set.HasVisible;
			Channel_Show.Enabled = Set.HasInvisible;
			Channel_Enable.Enabled = Set.HasDisabled;
			Channel_Disable.Enabled = Set.HasEnabled;
			Channel_Include.Enabled = Set.HasExcluded && Set.CanInclude;
			Channel_Exclude.Enabled = Set.HasExcluded && Set.CanInclude;

			_channelExplorer.Context_MoveUp.Enabled = Set.CanMoveUp;
			_channelExplorer.Context_MoveToTop.Enabled = Set.CanMoveUp;
			_channelExplorer.Context_MoveDown.Enabled = Set.CanMoveDown;
			_channelExplorer.Context_MoveToBottom.Enabled = Set.CanMoveDown;			
			_channelExplorer.Context_Rename.Enabled = Set.HasUnlocked;
			_channelExplorer.Context_ChangeRendColor.Enabled = Set.HasUnlocked;
			_channelExplorer.Context_Group.Enabled = Set.HasUnlocked;
			_channelExplorer.Context_Ungroup.Enabled = Set.HasUnlocked;
			_channelExplorer.Context_Unlock.Enabled = Set.HasLocked;
			_channelExplorer.Context_Lock.Enabled = Set.HasUnlocked;
			_channelExplorer.Context_Hide.Enabled = Set.HasVisible;
			_channelExplorer.Context_Show.Enabled = Set.HasInvisible;
			_channelExplorer.Context_Enable.Enabled = Set.HasDisabled;
			_channelExplorer.Context_Disable.Enabled = Set.HasEnabled;
			_channelExplorer.Context_Include.Enabled = Set.HasExcluded && Set.CanInclude;
			_channelExplorer.Context_Exclude.Enabled = Set.HasExcluded && Set.CanInclude;
		}

	}
}
