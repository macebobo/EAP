using ElfControls;

using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore.Forms
{
	public partial class EditShuffle : Form
	{
		#region [ Private Variables ]

		private Shuffle _shuffle = null;
		private ChannelList _channels = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public Shuffle Shuffle
		{
			get { return _shuffle; }
			set 
			{ 
				_shuffle = value;
				PopulateControls();
			}
		}

		public ChannelList Channels
		{
			get { return _channels; }
			set 
			{ 
				_channels = value;
				PopulateControls();
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public EditShuffle()
		{
			InitializeComponent();
		}

		public EditShuffle(Shuffle shuffle, ChannelList channels)
			: this()
		{
			_channels = channels;
			if (shuffle != null)
				_shuffle = (Shuffle)shuffle.Clone();
			else
				_shuffle = new Shuffle();
			PopulateControls();
		}
		
		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Grabs the list of all the selected items from a listbox and returns it.
		/// </summary>
		/// <param name="listBox">ListBox to grab selected Items from</param>
		/// <returns>List of ElfControls.IconListItem objects</returns>
		private List<ImageListItem> GetSelectedItems(ListBox listBox)
		{
			List<ImageListItem> SelectedItems = new List<ImageListItem>();
			foreach (ImageListItem LItem in listBox.SelectedItems)
			{
				SelectedItems.Add(LItem);
			}
			return SelectedItems;
		}

		/// <summary>
		/// Populates the controls on this form based on the objects passed in.
		/// </summary>
		private void PopulateControls()
		{
			if ((_channels == null) || (_shuffle == null))
				return;

			Bitmap ChBitmap = null;
			txtName.Text = _shuffle.Name;

			// If we are editing an existing SO, then populate the right listview control
			ImageListItem Item = null;
			if ((_shuffle != null) && (_shuffle.Count > 0))
			{
				int ID = 0;
				Channel SortedChannel;
				for (int i = 0; i < _shuffle.Count; i++)
				{
					ID = _shuffle[i];
					SortedChannel = _channels.Where(ID);
					if (SortedChannel != null)
					{
						ChBitmap = ImageHandler.ColorSwatches[SortedChannel.ChannelExplorerImageKey];
						Item = new ImageListItem(SortedChannel.ToString(), SortedChannel.ID.ToString(), ChBitmap, SortedChannel);
						lstSorted.Items.Add(Item);
					}
					SortedChannel = null;
				}
			}
			//otherwise populate the left control
			else
			{
				foreach (Channel Channel in _channels)
				{
					ChBitmap = ImageHandler.ColorSwatches[Channel.ChannelExplorerImageKey];
					Item = new ImageListItem(Channel.ToString(), Channel.ID.ToString(), ChBitmap, Channel);
					lstUnsorted.Items.Add(Item);
				}
			}
			UpdateButtons();
		}

		/// <summary>
		/// Finds the items from the list and mark them as being selected in the listbox.
		/// </summary>
		/// <param name="listBox">ListBox to modify</param>
		/// <param name="items">List of items in the listbox to select.</param>
		private void SelectItems(ListBox listBox, List<ImageListItem> items)
		{
			foreach (ImageListItem LItem in items)
			{
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					if (listBox.Items[i] == LItem)
					{
						listBox.SelectedIndices.Add(i);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Updates the enabled property of various buttons on this form based on items selected in the listboxes.
		/// </summary>
		private void UpdateButtons()
		{
			cmdOk.Enabled = ((lstUnsorted.Items.Count == 0) && (txtName.TextLength > 0));

			cmdRemove.Enabled = (lstSorted.SelectedItems.Count > 0);
			cmdAdd.Enabled = (lstUnsorted.SelectedItems.Count > 0);

			cmdDown.Enabled = (lstSorted.Items.Count != 0);
			cmdUp.Enabled = (lstSorted.Items.Count != 0);
			cmdToBottom.Enabled = (lstSorted.Items.Count != 0);
			cmdToTop.Enabled = (lstSorted.Items.Count != 0);
		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Take the items in the left list box that are selected and add them to the right one
		/// </summary>
		private void cmdAdd_Click(object sender, EventArgs e)
		{
			if (lstUnsorted.SelectedItems.Count == 0)
				return;

			// Get the index of the first selected item.
			int SelectedIndex = lstUnsorted.SelectedIndex;

			List<ImageListItem> List = new List<ImageListItem>();
			foreach (ImageListItem Item in lstUnsorted.SelectedItems)
			{
				List.Add(Item);
			}

			foreach (ImageListItem Item in List)
			{
				lstUnsorted.Items.Remove(Item);
				lstSorted.Items.Add(Item);
			}
			List.Clear();
			List = null;

			if (SelectedIndex >= lstUnsorted.Items.Count)
				SelectedIndex = lstUnsorted.Items.Count - 1;
			lstUnsorted.SelectedIndex = SelectedIndex;

			UpdateButtons();
		}

		/// <summary>
		/// Move the currently selected items to the top of the listbox.
		/// </summary>
		private void cmdToTop_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstSorted);

			int Index = 0;
			for (int i = 0; i < lstSorted.Items.Count; i++)
			{
				if (!lstSorted.SelectedIndices.Contains(i))
					continue;
				Item = (ImageListItem)lstSorted.Items[i];
				lstSorted.Items.Remove(Item);
				lstSorted.Items.Insert(Index++, Item);
			}

			// Find our moved items and select them
			SelectItems(lstSorted, SelectedItems);
			SelectedItems = null;

			UpdateButtons();
		}

		/// <summary>
		/// Move the currently selected items up one position in the listbox.
		/// </summary>
		private void cmdUp_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstSorted);

			for (int i = 1; i < lstSorted.Items.Count; i++)
			{
				if (!lstSorted.SelectedIndices.Contains(i))
					continue;
				if (lstSorted.SelectedIndices.Contains(i - 1))
					continue;
				Item = (ImageListItem)lstSorted.Items[i];
				lstSorted.Items.Remove(Item);
				lstSorted.Items.Insert(i - 1, Item);
			}

			// Find our moved items and select them
			SelectItems(lstSorted, SelectedItems);
			SelectedItems = null;

			UpdateButtons();
		}

		/// <summary>
		/// Moves the currently selected items down one position in the listbox.
		/// </summary>
		private void cmdDown_Click(object sender, EventArgs e)
		{			
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstSorted);
			
			for (int i = lstSorted.Items.Count - 1; i >= 0; i--)
			{
				if (!lstSorted.SelectedIndices.Contains(i))
					continue;
				if (i == lstSorted.Items.Count - 1)
					continue;
				if (lstSorted.SelectedIndices.Contains(i + 1))
					continue;
				Item = (ImageListItem)lstSorted.Items[i];
				lstSorted.Items.Remove(Item);
				lstSorted.Items.Insert(i + 1, Item);
			}

			// Find our moved items and select them
			SelectItems(lstSorted, SelectedItems);
			SelectedItems = null;

			UpdateButtons();
		}

		/// <summary>
		/// Move the currently selected items to the bottom of the listbox.
		/// </summary>
		private void cmdToBottom_Click(object sender, EventArgs e)
		{
			ImageListItem Item = null;
			List<ImageListItem> SelectedItems = GetSelectedItems(lstSorted);

			int Index = lstSorted.Items.Count - 1;
			for (int i = 0; i < SelectedItems.Count; i++)
			{
				Item = (ImageListItem)SelectedItems[i];
				lstSorted.Items.Remove(Item);
				lstSorted.Items.Insert(Index, Item);
			}

			// Find our moved items and select them
			SelectItems(lstSorted, SelectedItems);
			SelectedItems = null;

			UpdateButtons();
		}

		/// <summary>
		/// Take the items in the right list box that are selected and add them to the left one
		/// </summary>
		private void cmdRemove_Click(object sender, EventArgs e)
		{
			if (lstSorted.SelectedItems.Count == 0)
				return;
			
			// Get the index of the first selected item.
			int SelectedIndex = lstSorted.SelectedIndex;

			List<ImageListItem> List = new List<ImageListItem>();
			foreach (ImageListItem Item in lstSorted.SelectedItems)
			{
				List.Add(Item);
			}

			foreach (ImageListItem Item in List)
			{
				lstSorted.Items.Remove(Item);
				lstUnsorted.Items.Add(Item);
			}
			List.Clear();
			List = null;
			
			if (SelectedIndex >= lstSorted.Items.Count)
				SelectedIndex = lstSorted.Items.Count - 1;
			lstSorted.SelectedIndex = SelectedIndex;

			UpdateButtons();
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			// Build the list
			_shuffle.Name = txtName.Text;
			string List = string.Empty;
			Channel Channel = null;
			foreach (ImageListItem Item in lstSorted.Items)
			{
				Channel = (Channel)Item.Tag;
				List += (List.Length > 0 ? "," : string.Empty) + Channel.ID;
			}
			_shuffle.DeserializeList(List);
			DialogResult = DialogResult.OK;
		}

		private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		#endregion [ Events ]

	}
}
