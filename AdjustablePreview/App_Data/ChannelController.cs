using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElfCore
{
	/// <summary>
	/// Controls the list of Channels and maintains which Channels are active, grouped, etc.
	/// </summary>
	public class ChannelController
	{
		
		#region [ Private Variables ]

		/// <summary>
		/// The channel that is currently active.
		/// </summary>
		private Channel _active = null;

		/// <summary>
		/// All the Channels, index to their ID number
		/// </summary>
		private SortedList<int, Channel> _Channels = null;

		/// <summary>
		/// Indicates whether any of the Channels have been modified
		/// </summary>
		private bool _dirty = false;

		/// <summary>
		/// Complete list of Channels, ordered by the currently selected Sort Order
		/// </summary>
		private List<Channel> _orderedList = null;

		/// <summary>
		/// Index to the SortOrder that is currently employed.
		/// </summary>
		private int _selectedSortOrder = -1;

		/// <summary>
		/// List of all the SortOrders available
		/// </summary>
		private List<SortOrder> _sortOrders = null;

		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// The Channel currently selected. If there are multiple Channels selected, then the Active Channel is the topmost of these selected Channels
		/// </summary>
		public Channel Active 
		{
			get 
			{
				if ((_active == null) && (this.Selected.Count > 0))
					FindAndSetActiveChannel();
				return _active; 
			}
			set
			{
				_active = value;
				if (_active != null)
					_active.IsSelected = true;
			} 
		}

		/// <summary>
		/// Returns the count of all the Channels
		/// </summary>
		public int Count
		{
			get { return _Channels.Count; }
		}

		/// <summary>
		/// Indicates whether any of the Channels have been modified
		/// </summary>
		public bool Dirty
		{
			get
			{
				foreach(KeyValuePair<int, Channel> KVP in _Channels)
					if (KVP.Value.Dirty)
						return true;
				return _dirty;
			}
			set
			{
				if (_dirty != value)
				{
					foreach (KeyValuePair<int, Channel> KVP in _Channels)
					{
						KVP.Value.SuppressEvents = true;
						KVP.Value.Dirty = value;
						KVP.Value.SuppressEvents = false;
					}
					if (_dirty != value)
					{
						_dirty = value;
						OnChanged(this, ChannelEventType.Dirty);
					}
				}
			}
		}

		/// <summary>
		/// List of all the Channel groups
		/// </summary>
		public List<ChannelGroup> Groups { get; private set; }

		/// <summary>
		/// Temporary Channel used to hold and display the ImageStamp
		/// </summary>
		public Channel ImageStampChannel { get; private set; }

		/// <summary>
		/// Temporary Channel, used to hold cells that are being manipulated
		/// </summary>
		public Channel MoveChannel { get; private set; }

		/// <summary>
		/// Retrieves the list of Channels in their selected SortOrder
		/// </summary>
		public List<Channel> Sorted
		{
			get
			{
				if (_orderedList.Count == 0)
					this.SelectedSortOrder = 0;
				return _orderedList;
			}
		}

		/// <summary>
		/// List of all Channels that are selected
		/// </summary>
		public List<Channel> Selected
		{
			get
			{
				return this.Sorted.Where(c => c.IsSelected == true).ToList();
			}
			set
			{
				foreach (Channel Ch in this.Selected)
					Ch.IsSelected = false;

				foreach (Channel Ch in value)
					Ch.IsSelected = true;
			}
		}

		/// <summary>
		/// Returns a list of all the Indices of the selectd channels
		/// </summary>
		public List<int> SelectedIndices
		{
			get { return (from s in this.Selected select s.Index).ToList(); }
		}

		/// <summary>
		/// Indicated which Sorting to use
		/// </summary>
		public int SelectedSortOrder
		{
			get { return _selectedSortOrder; }
			set
			{
				_selectedSortOrder = value;

				// Create the _orderedList based on the SortOrder indicated
				_orderedList.Clear();

				int ID = -1;
				Channel Channel = null;
				for (int i = 0; i < _sortOrders[_selectedSortOrder].Order.Count; i++)
				{
					ID = _sortOrders[_selectedSortOrder].Order[i];
					Channel = this.GetChannelByID(ID);
					Channel.Index = i;
					_orderedList.Add(Channel);
				}
			}
		}

		/// <summary>
		/// Output number of the first Channel indicated in the Profile that is to be used by this Output PlugIn
		/// </summary>
		public int SetupStartChannel { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>
		/// The default Constructor.
		/// </summary>
		public ChannelController()
		{
			_Channels = new SortedList<int, Channel>();

			_orderedList = new List<Channel>();
			this.Groups = new List<ChannelGroup>();
			_sortOrders = new List<SortOrder>();

			ImageStampChannel = new Channel();
			MoveChannel = new Channel();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Adds a new Channel
		/// </summary>
		/// <param name="Channel">Channel to add</param>
		public void Add(Channel Channel)
		{
			_Channels.Add(Channel.ID, Channel);

			//ImageController.ColorSwatches.Add(Channel.Color.ToArgb(), ImageController.CreateColorSwatch(Channel));
			Channel.CellsChanged += new EventHandler(this.Channel_CellsChanged);
			Channel.ColorChanged += new EventHandler(this.Channel_ColorChanged);
			Channel.DirtyChanged += new EventHandler(this.Channel_Dirty);
			Channel.NameChanged += new EventHandler(this.Channel_NameChanged);
			Channel.SelectedChanged += new EventHandler(this.Channel_SelectedChanged);
			Channel.VisibilityChanged += new EventHandler(this.Channel_VisibilityChanged);
		}

		/// <summary>
		/// Adds a default SortOrder to the list
		/// </summary>
		public void AddSortOrder()
		{
			_sortOrders.Add(new SortOrder(this.Count, SetupStartChannel));
		}

		/// <summary>
		/// Adds a new SortOrder to the list
		/// </summary>
		/// <param name="name">Name of the Order</param>
		/// <param name="orderList">Comma-seperated list of IDs</param>
		public void AddSortOrder(string name, string orderList)
		{
			_sortOrders.Add(new SortOrder(name, orderList));
		}

		///// <summary>
		///// Fires off the CellsChanged event
		///// </summary>
		//public void CellsChanged()
		//{
		//    OnChannelChanged(SpecificEventType.Channel_Cells);
		//}

		/// <summary>
		/// Clears out all the cells on all Channels, then fires the ClearAll event.
		/// </summary>
		public void ClearAllChannels()
		{
			foreach (KeyValuePair<int, Channel> KVP in _Channels)
			{
				KVP.Value.SuppressEvents = true;
				KVP.Value.ClearLattice();
				KVP.Value.SuppressEvents = false;
			}
			OnChanged(this.Sorted, ChannelEventType.Cells);
		}

		/// <summary>
		/// Clears out all the cells on the selected Channels, then fires the ClearSelected event.
		/// </summary>
		public void ClearSelectedChannels()
		{
			foreach (Channel Channel in Selected)
			{
				Channel.SuppressEvents = true;
				Channel.ClearLattice();
				Channel.SuppressEvents = true;
			}
			OnChanged(Selected, ChannelEventType.Selected);
		}

		/// <summary>
		/// Finds the Channel in the list of Selected channel, with the lowest Index
		/// </summary>
		public void FindAndSetActiveChannel()
		{
			this.Active = this.Selected.OrderBy(c => c.Index).First<Channel>();
		}

		/// <summary>
		/// Returns the next Visible Channel after this index
		/// </summary>
		/// <param name="index">Index as the starting point</param>
		/// <returns>If there are no visible Channels, calls this method to start from the top of the list. If none found at all, returns null</returns>
		public Channel FindNextVisibleChannel(int index)
		{
			if ((index >= 0) && (index < this.Count))
			{
				var Ch = this.Sorted.Where(s => s.Visible = true && s.Index > index).FirstOrDefault();
				if (Ch == null)
					return FindNextVisibleChannel(-1);
				else if (index < 0)
					return null;
				else
					return Ch;
			}
			else
				return null;
		}

		/// <summary>
		/// Returns a list of all the Channels
		/// </summary>
		public List<Channel> GetAllChannels()
		{
			return _orderedList;
		}

		/// <summary>
		/// Finds the Channel by its OutputChannel value and returns it
		/// </summary>
		/// <param name="id">OutputChannel value on the Channel desired</param>
		/// <returns>If the Channel is not found, returns null, otherwise returns the correct Channel</returns>
		public Channel GetChannelByID(int id)
		{
			var KVP = _Channels.Where(c => c.Key == id).FirstOrDefault();
			return KVP.Value;
		}

		/// <summary>
		/// Sets the Active Channel to be the next one in the current Sorted order and returns it
		/// </summary>
		/// <returns>Returns the Channel that follows the current Active Channel. If the Active Channel is
		/// the last in the list, returns the first Channel in the list.</returns>
		public Channel GetNext()
		{
			return GetNext(1, false);
			//if (this.Active == null)
			//    return null;

			//int ActiveChannelIndex = this.Active.Index;

			//if (ActiveChannelIndex + 1 >= this.Count)
			//    ActiveChannelIndex = -1;

			//// Set the selected flag on the old Active Channel to false;
			//this.Active.Selected = false;

			//// Select the new Active Channel
			//this.Active = _orderedList[ActiveChannelIndex + 1];
			//// Set its Selected flag = true;
			//this.Active.Selected = true;

			//return this.Active;
		}
		
		/// <summary>
		/// Sets the Active Channel by moving the index of current Active one by the offset amount.
		/// </summary>
		/// <param name="offset">Amount to advance</param>
		/// <returns>Returns the Channel that follows the current Active Channel by [offset].</returns>
		public Channel GetNext(int offset)
		{
			return GetNext(offset, false);
		}

		/// <summary>
		/// Sets the Active Channel by moving the index of current Active one by the offset amount.
		/// </summary>
		/// <param name="offset">Amount to advance</param>
		/// <param name="wrap">Indicates whether we should wrap back to the other end of the list if we exceed the list boundary</param>
		/// <returns>Returns the Channel that follows the current Active Channel by [offset]. Wraps to the top if the new
		/// index would exceed the list boundary</returns>
		public Channel GetNext(int offset, bool wrap)
		{
			if (this.Active == null)
				return null;

			int ActiveChannelIndex = this.Active.Index;

			// Set the selected flag on the old Active Channel to false;
			this.Active.IsSelected = false;

			if (wrap)
			{
				if ((ActiveChannelIndex + offset >= this.Count) || (ActiveChannelIndex + offset < 0))
					ActiveChannelIndex = (this.Count - 1) - offset;
				ActiveChannelIndex += offset;
			}
			else
			{
				if (ActiveChannelIndex + offset >= this.Count)
					ActiveChannelIndex = this.Count - 1;
				else if (ActiveChannelIndex + offset < 0)
					ActiveChannelIndex = 0;
				else
					ActiveChannelIndex += offset;
			}

			// Select the new Active Channel
			this.Active = _orderedList[ActiveChannelIndex];

			// Set its Selected flag = true;
			this.Active.IsSelected = true;

			return this.Active;
		}

		/// <summary>
		/// Sets the Active Channel to be the previous one in the current Sorted order and returns it
		/// </summary>
		/// <returns>Returns the Channel that preceeds the current Active Channel.</returns>
		public Channel GetPrevious()
		{
			return GetNext(-1);
		}

		/// <summary>
		/// Sets the Active Channel to be the previous one (by offset) in the current Sorted order and returns it
		/// </summary>
		/// <param name="offset">Amount to advance</param>
		/// <returns>Returns the Channel that preceeds the current Active Channel.</returns>
		public Channel GetPrevious(int offset)
		{
			return GetNext(-offset, false);
		}

		/// <summary>
		/// Sets the Active Channel to be the previous one (by offset) in the current Sorted order and returns it
		/// </summary>
		/// <param name="offset">Amount to advance</param>
		/// <param name="wrap">Indicates whether we should wrap back to the other end of the list if we exceed the list boundary</param>
		/// <returns>Returns the Channel that preceeds the current Active Channel. If allowing to wrap and the new index would be less than
		/// zero, then wraps and gets the difference from the end of the list.</returns>
		public Channel GetPrevious(int offset, bool wrap)
		{
			return GetNext(-offset, wrap);
		}

		/// <summary>
		/// Sets the Hidden flag for all Channels. Changing the Hidden flag does not fire any event on the Channel.
		/// </summary>
		public void HideAll()
		{
			foreach (Channel Channel in this.Sorted)
			{
				Channel.Hide();
			}
		}

		/// <summary>
		/// Sets the Hidden flag for all selected Channels. Changing the Hidden flag does not fire any event on the Channel.
		/// </summary>
		public void HideSelected()
		{
			foreach (Channel Channel in this.Selected)
			{
				Channel.Hide();
			}
		}

		/// <summary>
		/// Turn the Visible flag OFF for all Channels
		/// </summary>
		public void MakeAllChannelsInvisible()
		{
			foreach (Channel Channel in this.Sorted)
			{
				Channel.SuppressEvents = true;
				Channel.Visible = false;
				Channel.SuppressEvents = false;
			}
			OnChanged(this.Sorted, ChannelEventType.Visibility);
		}

		/// <summary>
		/// Turn the Visible flag ON for all Channels
		/// </summary>
		public void MakeAllChannelsVisible()
		{
			foreach (Channel Channel in this.Sorted)
			{
				Channel.SuppressEvents = true;
				Channel.Visible = true;
				Channel.SuppressEvents = false;
			}
			OnChanged(this.Sorted, ChannelEventType.Visibility);
		}

		#region [ PopulateChannelFromBitmap ]

		/// <summary>
		/// Inserts cells into a Channel via a 1-bit Bitmap.
		/// </summary>
		/// <param name="bitmap">Bitmap to generate cells. White pixels are active cells, black indicates no data.</param>
		/// <param name="channel">Channel to add the cells to</param>
		/// <param name="clearFirst">Indicates whether all the existing cells on the Channel should be removed first.</param>
		public void PopulateChannelFromBitmap(Bitmap bitmap, Channel channel, bool clearFirst)
		{
			channel.PopulateFromLatticeBuffer(bitmap, clearFirst);
		}

		/// <summary>
		/// Inserts cells into a Channel via a 1-bit Bitmap. Clears the cells first
		/// </summary>
		/// <param name="bitmap">Bitmap to generate cells. White pixels are active cells, black indicates no data.</param>
		/// <param name="channel">Channel to modify</param>
		public void PopulateChannelFromBitmap(Bitmap bitmap, Channel channel)
		{
			channel.PopulateFromLatticeBuffer(bitmap, true);
		}

		/// <summary>
		/// Overlays the bitmap over the Lattice bitmap representing the channel's cells, blacking out lit cells to erase them. Clears out the existing cells and 
		/// populates the cells from the combined bitmap.
		/// </summary>
		/// <param name="bitmap">Bitmap should have a transparent blackground with black or white pixels</param>
		/// <param name="channel">Channel to modify</param>
		public void PopulateChannelFromBitmap_Erase(Bitmap bitmap, Channel channel)
		{
			// Grab the bitmap representing this Channel
			Bitmap ChannelBitmap = channel.LatticeBuffer;
			
			using (Graphics g = Graphics.FromImage(ChannelBitmap))
				g.DrawImage(bitmap, 0, 0);

			channel.SuppressEvents = true;
			channel.ClearLattice();
			channel.LatticeBuffer = ChannelBitmap;
			channel.SuppressEvents = false;
			OnChanged(channel, ChannelEventType.Cells);
		}

		#endregion [ PopulateChannelFromBitmap ]

		/// <summary>
		/// Selects the Channel with the corresponding index.
		/// </summary>
		/// <param name="index">Index of the Channel to select</param>
		/// <param name="multiSelect">Indicates whether we should unselect all other Channels upon selecting this new one</param>
		public void Select(int index, bool multiSelect)
		{
			if (!ValidIndex(index))
				return;

			if (!multiSelect)
				UnselectAll(false);

			this.Sorted[index].SuppressEvents = true;
			this.Sorted[index].IsSelected = true;
			this.Sorted[index].SuppressEvents = false;

			FindAndSetActiveChannel();

			// Fire an event to notify the consumer's that the selected channels are changed.
			OnChanged(this.Sorted[index], ChannelEventType.Selected);
		}

		/// <summary>
		/// Selects the Channel with the corresponding index.
		/// </summary>
		/// <param name="index">Index of the Channel to select</param>
		public void Select(int index)
		{
			Select(index, false);
		}

		/// <summary>
		/// Selects a number of channels.
		/// </summary>
		/// <param name="indices">List of Indices that should be selected</param>
		public void Select(List<int> indices)
		{
			UnselectAll(false);
			int Index = 0;
			for (int i = 0; i < indices.Count; i++)
			{
				Index = indices[i];
				if (!ValidIndex(Index))
					continue;
				this.Sorted[Index].SuppressEvents = true;
				this.Sorted[Index].IsSelected = true;
				this.Sorted[Index].SuppressEvents = false;
			}

			FindAndSetActiveChannel();
			OnChanged(this.Selected, ChannelEventType.Selected);
		}

		/// <summary>
		/// Remove the Hidden flag for all Channels. Changing the Hidden flag does not fire any event on the Channel.
		/// </summary>
		public void ShowAll()
		{
			foreach (Channel Channel in this.Sorted)
			{
				Channel.Show();
			}
		}

		/// <summary>
		/// Remove the Hidden flag for all selected Channels. Changing the Hidden flag does not fire any event on the Channel.
		/// </summary>
		public void ShowSelected()
		{
			foreach (Channel Channel in this.Selected)
			{
				Channel.Show();
			}
		}

		/// <summary>
		/// Sends a message to all the Channels to update their GraphicsPath next time they are redrawn
		/// </summary>
		public void UIChanged()
		{
			foreach (Channel Channel in this.Sorted)
				Channel.LatticeChanged = true;
		}

		/// <summary>
		/// Unselects all Channels
		/// </summary>
		public void UnselectAll()
		{
			UnselectAll(true);
		}

		/// <summary>
		/// Unselects all Channels
		/// </summary>
		public void UnselectAll(bool fireEvent)
		{
			List<Channel> Unselected = this.Selected;
			foreach (Channel Channel in this.Selected)
			{
				Channel.SuppressEvents = true;
				Channel.IsSelected = false;
				Channel.SuppressEvents = false;
			}
			Active = null;
			if (fireEvent)
				OnChanged(Unselected, ChannelEventType.Selected);
			Unselected = null;
		}

		/// <summary>
		/// Verifies that the index falls within the range of the array
		/// </summary>
		/// <param name="index">Index to check</param>
		/// <returns>Returns true if 0 or higher, but not higher than Count-1</returns>
		private bool ValidIndex(int index)
		{
			return ((index >= 0) && (index < this.Count));
		}

		#endregion [ Methods ]

		#region [ Custom Event Methods ]

		/// <summary>
		/// Determines which event to fire, based on the enum passed it. If that delegate is defined, then calls it.
		/// </summary>
		/// <param name="eventType">Enum that indicates which event to fire..</param>
		private void OnChanged(Channel sender, ChannelEventType eventType)
		{
			switch (eventType)
			{
				case ChannelEventType.Cells:
					if (ChannelCellsChanged == null)
						return;
					ChannelCellsChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Color:
					if (ChannelColorChanged == null)
						return;
					ChannelColorChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Dirty:
					if (DirtyChanged == null)
						return;
					DirtyChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Selected:
					if (ChannelSelected == null)
						return;
					ChannelSelected(sender, new System.EventArgs());
					break;
				case ChannelEventType.Name:
					if (ChannelNameChanged == null)
						return;
					ChannelNameChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Visibility:
					if (ChannelVisibilityChanged == null)
						return;
					ChannelVisibilityChanged(sender, new System.EventArgs());
					break;
			}
		}

		/// <summary>
		/// Determines which event to fire, based on the enum passed it. If that delegate is defined, then calls it.
		/// </summary>
		/// <param name="eventType">Enum that indicates which event to fire..</param>
		private void OnChanged(ChannelController sender, ChannelEventType eventType)
		{
			switch (eventType)
			{
				case ChannelEventType.Dirty:
					if (DirtyChanged == null)
						return;
					DirtyChanged(this, new System.EventArgs());
					break;
			}
		}

		/// <summary>
		/// Determines which event to fire, based on the enum passed it. If that delegate is defined, then calls it.
		/// </summary>
		/// <param name="eventType">Enum that indicates which event to fire..</param>
		private void OnChanged(List<Channel> sender, ChannelEventType eventType)
		{
			switch (eventType)
			{
				case ChannelEventType.Cells:
					if (ChannelCellsChanged == null)
						return;
					ChannelCellsChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Color:
					if (ChannelColorChanged == null)
						return;
					ChannelColorChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Dirty:
					if (DirtyChanged == null)
						return;
					DirtyChanged(this, new System.EventArgs());
					break;
				case ChannelEventType.Selected:
					if (ChannelSelected == null)
						return;
					ChannelSelected(sender, new System.EventArgs());
					break;
				case ChannelEventType.Name:
					if (ChannelNameChanged == null)
						return;
					ChannelNameChanged(sender, new System.EventArgs());
					break;
				case ChannelEventType.Visibility:
					if (ChannelVisibilityChanged == null)
						return;
					ChannelVisibilityChanged(sender, new System.EventArgs());
					break;
			}
		}

		//private void OnChanged(EventSubCategory category)
		//{
		//    if (Changed == null)
		//        return;
		//    ChannelEventArgs e = new ChannelEventArgs(EventCategory.Channel, category);
		//    Changed(this, e);
		//}

		///// <summary>
		///// Fires an event if data within a given channel has changed.
		///// </summary>
		///// <param name="channel">Channel that was changed</param>
		///// <param name="category">Specific type of event has occurred</param>
		//private void OnChannelChanged(Channel channel, EventSubCategory category)
		//{
		//    if (!_listenForChannelEvents)
		//        return;

		//    if (Changed == null)
		//        return;
		//    ChannelEventArgs e = new ChannelEventArgs(EventCategory.Channel, category, channel);
		//    Changed(channel, e);
		//}

		///// <summary>
		///// One or more Multiple Channels are selected
		///// </summary>
		//private void OnChannelsSelected()
		//{
		//    if (ChannelsSelected == null)
		//        return;
		//    ChannelEventArgs e = new ChannelEventArgs(EventCategory.Channel, EventSubCategory.Channel_Selected, this.Selected);
		//    ChannelsSelected(this, e);
		//}

		///// <summary>
		///// If the dirty flag is flipped on, then fire an event.
		///// </summary>
		//protected void OnDirtyChanged()
		//{
		//    // If we are still loading, do not respond to any messages stating the data is dirty. They will be reset to Clean
		//    // once loading is complete.
		//    if (Editor.Loading)
		//        return;

		//    if (Changed == null)
		//        return;

		//    DataEventArgs e = new DataEventArgs(EventCategory.UI, EventSubCategory.Dirty);
		//    Changed(this, e);
		//}

		//private void OnChannelEvent(SpecificEventType specificEvent)
		//{
		//    if (specificEvent == SpecificEventType.Dirty)
		//    {
		//        Dirty = true;
		//        return;
		//    }
		//    if (Changed == null)
		//        return;
		//    DataEventArgs e = new DataEventArgs(GeneralDataEvent.UI, specificEvent);
		//    Changed(this, e);
		//}

		#endregion [ Custom Event Methods ]

		#region [ Event Handlers ]

		public System.EventHandler ChannelColorChanged;
		public System.EventHandler ChannelNameChanged;
		public System.EventHandler ChannelCellsChanged;
		public System.EventHandler ChannelVisibilityChanged;
		public System.EventHandler ChannelSelected;
		public System.EventHandler DirtyChanged;

		#endregion [ Event Handlers ]

		#region [ Events ]

		private void Channel_ColorChanged(object sender, EventArgs e)
		{
			OnChanged((Channel)sender, ChannelEventType.Color);
		}

		private void Channel_CellsChanged(object sender, EventArgs e)
		{
			OnChanged((Channel)sender, ChannelEventType.Cells);
		}

		private void Channel_Dirty(object sender, EventArgs e)
		{
			if (((Channel)sender).Dirty)
				this.Dirty = true;
		}

		private void Channel_SelectedChanged(object sender, EventArgs e)
		{
			OnChanged((Channel)sender, ChannelEventType.Cells);
		}

		private void Channel_NameChanged(object sender, EventArgs e)
		{
			OnChanged((Channel)sender, ChannelEventType.Name);
		}

		private void Channel_VisibilityChanged(object sender, EventArgs e)
		{
			OnChanged((Channel)sender, ChannelEventType.Visibility);
		}

		///// <summary>
		///// A Channel threw an event stating that it changed. Determine what we need to do, usually just fire an event of our
		///// own to pass it down the change to Workshop
		///// </summary>
		//private void Channel_Changed(object sender, DataEventArgs e)
		//{
		//    if (!_listenForChannelEvents)
		//        return;

		//    if (e.SubCategory == EventSubCategory.Channel_Selected)
		//        return;

		//    OnChannelChanged((Channel)sender, e.SubCategory);
		//}

		///// <summary>
		///// This occurs when a Channel's IsSelected is flipped from false to true.
		///// </summary>
		//private void Channel_Selected(object sender, DataEventArgs e)
		//{
		//    if (_listenForChannelEvents)
		//        OnChannelsSelected();
		//}

		#endregion [ Events ]
	}

	#region [ Class SortOrder ]

	public class SortOrder
	{
		#region [ Private Properties ]

		#endregion [ Private Properties ]

		#region [ Properties ]

		/// <summary>
		/// Indicates whether this object has been modified
		/// </summary>
		public bool Dirty { get; private set; }

		#endregion [ Properties ]

		#region [ Fields ]

		public List<int> Order = null;
		public string Name;
		public bool Selected;

		#endregion [ Fields ]

		#region [ Constructors ]

		public SortOrder()
		{
			Order = new List<int>();
			Name = string.Empty;
			Selected = false;
			this.Dirty = false;
		}

		public SortOrder(string name, string orderList)
			: this()
		{
			this.Name = name;
			ReOrder(orderList);
			this.Dirty = false;
		}

		/// <summary>
		/// Create a default sort order just by the number of Channels and the ID of the first Channel being passed in
		/// </summary>
		/// <param name="numberOfChannels">Number of Channels</param>
		/// <param name="startChannelID">ID of the first Channel being handled by this Output PlugIn</param>
		public SortOrder(int numberOfChannels, int startChannelID)
		{
			this.Name = "DEFAULT";
			for (int i = 0; i < numberOfChannels; i++)
				Order.Add(i + startChannelID);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Overrides the current list of orders and applies a new one
		/// </summary>
		/// <param name="orderList">Comma seperated list of indices, representing the new Sort Order</param>
		public void ReOrder(string orderList)
		{
			Order.Clear();
			foreach (string ID in orderList.Split(','))
				Order.Add(Int32.Parse(ID.Trim()));			
		}

		#endregion [ Methods ]
	}

	#endregion [ Class SortOrder ]

	#region [ Class ChannelGroup ]

	public class ChannelGroup
	{
		#region [ Fields ]

		/// <summary>
		/// List of Channels contained in this group.
		/// </summary>
		public List<Channel> Members = null;

		/// <summary>
		/// Name of the group
		/// </summary>
		public string Name = string.Empty;

		#endregion [ Fields ]

		#region [ Properties ]

		/// <summary>
		/// Gets the number of Channels actually contained in this group.
		/// </summary>
		public int Count
		{
			get { return Members.Count; }
		}

		//// User-defined conversion from ChannelGroup to List<Channel>
		//public static implicit operator List<Channel>(ChannelGroup g)
		//{
		//    return g.Members;
		//}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelGroup(string name)
		{
			this.Name = name;
			Members = new List<Channel>();
		}

		#endregion [ Constructors ]
		
	}

	#endregion [ Class ChannelGroup ]
}
