using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;

namespace ElfCore.Controllers
{
	/// <summary>
	/// Controls the list of Channels and maintains which Channels are active, grouped, etc.
	/// </summary>
	public class ChannelController : ElfBase, IEnumerable<Channel>
	{

		#region [ Private Variables ]

		/// <summary>
		/// The channel that is currently active.
		/// </summary>
		private Channel _active = null;

		/// <summary>
		/// All the Channels, index to their ID number
		/// </summary>
		private ChannelList _channels = null;

		/// <summary>
		/// Complete list of Channels, ordered by the currently selected Sort Order
		/// </summary>
		private ChannelList _sortedChannels = null;
		private ShuffleController _shuffleController = null;
		private ChannelGroupController _groupController = null;
		private BaseProfile _profile = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// The Channel currently selected. If there are multiple Channels selected, then the Active Channel is the topmost of these selected Channels
		/// </summary>
		[XmlIgnore()]
		public Channel Active
		{
			get
			{
				// If there are no Channels, return null.
				if ((_active == null) && (_channels == null) || (_channels.Count == 0))
					return null;
				// If there is no Active Channel, but there are Selected channels, set the Active Channel to be the first Selected Channel.
				if ((_active == null) && (Selected.Count > 0))
					Active = Selected[0];
				// If there is yet no Active Channel and are no Selected Channels, then set the Active Channel to be the first Channel in the current Sort Order.
				if ((_active == null) && (_sortedChannels.Count > 0))
					Active = _sortedChannels[0];
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
		/// Indicate whether all of the selected channels are eligible for inclusion for this PlugIn
		/// </summary>
		[XmlIgnore()]
		public bool CanInclude
		{
			get
			{
				// Excluded channels can only be at the top and bottom of the list of unsorted channels. Determine if
				// the selected channels can be set to included (ie not making a gap in the excluded channels).
				string Code = string.Empty;
				// Create a string that represents the included channels as O's, and excluded channels as X's
				// if the channel is selected, set it's value to be Included
				for (int i = 0; i < _channels.Count; i++)
				{
					if (_channels[i].IsSelected)
						Code += "O";
					else
						Code += _channels[i].Included ? "O" : "X";
				}

				// Now check the string to make sure it is "legal"
				return IsLegalIncludes(Code);
			}
		}

		/// <summary>
		/// Indicate whether all of the selected channels are eligible for exclusion for this PlugIn
		/// </summary>
		[XmlIgnore()]
		public bool CanExclude
		{
			get
			{
				// Excluded channels can only be at the top and bottom of the list of unsorted channels. Determine if
				// the selected channels can be set to included (ie not making a gap in the excluded channels).
				string Code = string.Empty;
				// Create a string that represents the included channels as O's, and excluded channels as X's
				// if the channel is selected, set it's value to be excluded
				for (int i = 0; i < _channels.Count; i++)
				{
					if (_channels[i].IsSelected)
						Code += "X";
					else
						Code += _channels[i].Included ? "O" : "X";
				}

				// Now check the string to make sure it is "legal"
				return IsLegalIncludes(Code);
			}
		}
		
		/// <summary>
		/// Indicates if any of the selected channels can be moved downwards in the sort order. Will return false if there is only
		/// 1 channel, at index [channel count -1], or if there are multiple channels and all of them occupy the bottom indices (ie cannot move one
		/// channel into another's index. 
		/// </summary>
		[XmlIgnore()]
		public bool CanMoveDown
		{
			get
			{
				int Count = Selected.Count;
				bool Found = false;

				// Loop through all the selected channels, checking to see if any of them have the Index corresponding
				// to i. If non of them to, then we can move up. Otherwise we cannot.
				for (int i = _channels.Count - 1; i > _channels.Count - Count - 1; i--)
				{
					Found = false;
					foreach (Channel Channel in Selected)
					{
						if (Channel.Index == i)
						{
							Found = true;
							break;
						}
						if (!Found)
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Indicates if any of the selected channels can be moved upwards in the sort order. Will return false if there is only
		/// 1 channel, at index 0, or if there are multiple channels and all of them occupy the top indices (ie cannot move one
		/// channel into another's index. 
		/// </summary>
		[XmlIgnore()]
		public bool CanMoveUp
		{
			get
			{
				bool Found = false;

				// Loop through all the selected channels, checking to see if any of them have the Index corresponding
				// to i. If non of them to, then we can move up. Otherwise we cannot.
				for (int i = 0; i < Selected.Count; i++)
				{
					Found = false;
					foreach (Channel Channel in Selected)
					{
						if (Channel.Index == i)
						{
							Found = true;
							break;
						}
						if (!Found)
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Returns the count of all the Channels
		/// </summary>
		[DebuggerHidden()]
		[XmlIgnore()]
		public int Count
		{
			get { return _channels.Count; }
		}

		/// <summary>
		/// Controller of the Channel Groups
		/// </summary>
		[XmlIgnore()]
		public ChannelGroupController Groups
		{
			get { return _groupController; }
		}

		/// <summary>
		/// Used by the XmlSerializer and for reconciling channels in the PlugIn component.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ChannelList List
		{
			get { return _channels; }
			set { _channels = value; }
		}

		/// <summary>
		/// Returns the largest ID in the list.
		/// </summary>
		[XmlIgnore(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DebuggerHidden()]
		public int MaxID
		{
			get
			{
				int MaxID = -1;
				foreach (Channel Channel in _channels)
					MaxID = Math.Max(MaxID, Channel.ID);
				return MaxID;
			}
		}

		/// <summary>
		/// Returns the smallest ID in the list.
		/// </summary>
		[XmlIgnore(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DebuggerHidden()]
		public int MinID
		{
			get
			{
				int MinID = Int32.MaxValue;
				foreach (Channel Channel in _channels)
					MinID = Math.Min(MinID, Channel.ID);
				return MinID;
			}
		}

		/// <summary>
		/// Returns "Channel" if there is only 1 channel, "Channels" if there are 0, or more than 1.
		/// </summary>
		[XmlIgnore(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DebuggerHidden()]
		public string PluralText
		{
			get
			{
				if (Selected.Count == 1)
					return "Channel";
				else
					return "Channels";
			}
		}

		/// <summary>
		/// Retrieves the list of Channels in their selected Shuffle
		/// </summary>
		[XmlIgnore()]
		public ChannelList Sorted
		{
			get
			{
				if (_sortedChannels.Count == 0)
					_sortedChannels = _shuffleController.GetSorted(_channels);
				return _sortedChannels;
			}
		}

		/// <summary>
		/// Returns the ShuffleController
		/// </summary>
		[DebuggerHidden()]
		public ShuffleController ShuffleController
		{
			get { return _shuffleController; }
		}

		/// <summary>
		/// List of all Channels that are selected
		/// </summary>
		[XmlIgnore(), DebuggerHidden()]
		public ChannelList Selected
		{
			get
			{
				return _sortedChannels.WhereList(true);
			}
			set
			{
				if (value == null)
					return;

				SuppressChannelEvents(true);

				foreach (Channel Channel in _channels)
					Channel.IsSelected = value.Contains(Channel);

				SuppressChannelEvents(false);

				OnChannelsSelected(value);
			}
		}

		/// <summary>
		/// Overloaded index operator
		/// </summary>
		/// <param name="index">Index of the array to use.</param>
		[DebuggerHidden()]
		public Channel this[int index]
		{
			get
			{
				if (_sortedChannels.Count == 0)
					_sortedChannels = _shuffleController.GetSorted(_channels);
				return _sortedChannels[index];
			}
		}

		/// <summary>
		/// List of all Channels that are not selected.
		/// </summary>
		[XmlIgnore(), DebuggerHidden()]
		public ChannelList Unselected
		{
			get 
			{
				return _sortedChannels.WhereList(false); 
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>
		/// The default Constructor.
		/// </summary>
		public ChannelController(BaseProfile profile)
			: base()
		{
			_channels = new ChannelList();
			_sortedChannels = new ChannelList();

			_shuffleController = new ShuffleController();
			_shuffleController.Switched += ShuffleController_Switched;
			_shuffleController.ShuffleChanged += ShuffleController_Changed;

			_groupController = new ChannelGroupController();
			_profile = profile;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#region [ List Methods ]

		/// <summary>
		/// Adds a new Channel
		/// </summary>
		/// <param name="channel">Channel to add</param>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		public void Add(Channel channel)
		{
			// Channel is added at the end of the unordered list _channels, and at the end of _sortedChannels.
			// Channel ID is added at the end of all Shuffles

			// If the channel ID is missing or is already being used by an existing channel, optain a new one.
			Channel Ch = _channels.Where(channel.ID);
			if ((channel.ID < 0) || (Ch != null))
			{
				// Find a new ID for this channel by finding the largest channel ID and incrementing that number.
				channel.ID = MaxID + 1;
			}
			Ch = null;

			channel.Profile = _profile;
			_channels.Add(channel);
			_sortedChannels.Add(channel);
			_shuffleController.AddChannelID(channel.ID);
			AttachChildEvents(channel, true);
			OnChannelAdded(channel);
		}

		/// <summary>
		/// Adds a new Channel based on the RawChannel data
		/// </summary>
		/// <param name="rawChannel">Channel to add</param>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		public void Add(RawChannel rawChannel)
		{
			// If the channel ID is missing or is already being used by an existing channel, optain a new one.
			Channel Ch = _channels.Where(rawChannel.ID);
			if ((rawChannel.ID < 0) || (Ch != null))
			{
				// Find a new ID for this channel by finding the largest channel ID and incrementing that number.
				rawChannel.ID = MaxID + 1;
			}
			Ch = null;
			if (!_profile.ProfileDataLayer.Channels.Contains(rawChannel))
				_profile.ProfileDataLayer.Channels.Add(rawChannel);

			Ch = new Channel(rawChannel);
			Ch.Profile = _profile;
			_channels.Add(Ch);
			_sortedChannels.Add(Ch);
			_shuffleController.AddChannelID(Ch.ID);
			AttachChildEvents(Ch, true);
			OnChannelAdded(Ch);
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the Channels list.
		/// </summary>
		/// <param name="channels">The collection whose elements should be added to the end of the Channels List.
		/// The collection itself cannot be null, nor can any of the elements therein.</param>
		/// <exception cref="System.ArgumentNullException">collection is null</exception>
		public void AddRange(List<Channel> channels)
		{
			if (channels == null)
				throw new ArgumentNullException("collection is null");

			foreach (Channel Channel in channels)
				Add(Channel);
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the Channels list.
		/// </summary>
		/// <param name="channels">The collection whose elements should be added to the end of the Channels List.
		/// The collection itself cannot be null, nor can any of the elements therein.</param>
		/// <exception cref="System.ArgumentNullException">collection is null</exception>
		public void AddRange(ChannelList channels)
		{
			if (channels == null)
				throw new ArgumentNullException("collection is null");

			AddRange(channels);
		}

		/// <summary>
		/// Clears out the list of Channels, Shuffles and Groups
		/// </summary>
		public void Clear()
		{
			// Clear out all the Channels from the raw list and the current sorted list.
			RemoveAll();

			// Clear out all the Sort Orders
			_shuffleController.Clear();

			// Clear out all the Groups
			_groupController.Clear();
		}

		/// <summary>
		/// Determines whether an element is in the Channels list.
		/// </summary>
		/// <param name="channel">The Channel to locate in the Channels list.</param>
		/// <returns>true if item is found in the Channels list; otherwise, false.</returns>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		public bool Contains(Channel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel is null");
			return _channels.Contains(channel);
		}

		public Channel Find(int id)
		{
			return _channels.Where(id);
		}

		/// <summary>
		/// Searches for the specified Channel and returns the zero-based index of the first occurrence within the entire Channel list.
		/// </summary>
		/// <param name="channel">The Channel to locate in the Channel list. The value cannot be null.</param>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		/// <returns>The zero-based index of the first occurrence of item within the entire Channel list, if found; otherwise, –1.</returns>
		public int IndexOf(Channel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel is null");
			return _channels.IndexOf(channel);
		}

		/// <summary>
		/// Inserts a Channel into the sorted Channel list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="channel">Channel to Insert.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than Count.</exception>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		public virtual void Insert(int index, Channel channel)
		{
			if ((index < 0) || (index >= _channels.Count))
				throw new ArgumentOutOfRangeException();
			if (channel == null)
				throw new ArgumentNullException("channel is null");

			channel.Profile = _profile;
			_channels.Add(channel);
			AttachChildEvents(channel, true);

			_shuffleController.AddID(index, channel.ID);

			OnChannelAdded(channel);
		}

		/// <summary>
		/// Removes the first occurrence of a specific Channel from the Channel list.
		/// </summary>
		/// <param name="channel">The Channel to remove from the Channel list. The value cannot be null.</param>
		/// <returns>true if Channel is successfully removed; otherwise, false. This method also returns false if Channel was not found in the Channel list.</returns>
		public void Remove(Channel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel is null");

			AttachChildEvents(channel, false);
			_sortedChannels.Remove(channel);

			int ID = channel.ID;

			// Remove this Channel's ID from all the Shuffles.
			_shuffleController.SuppressEvents = true;
			_shuffleController.RemoveChannelID(channel.ID);
			_shuffleController.SuppressEvents = false;

			_channels.Remove(channel);

			// Resequence the channels from the deleted channel's ID onward
			//foreach (RasterChannel Channel in _channels.Where(c => c.ID > ID).OrderBy(c => c.ID).ToList())
			foreach (Channel Channel in _channels.OrderByAscending())
			{
				if (Channel.ID > ID)
					Channel.ID--;
			}

			OnChannelRemoved(channel);
		}

		/// <summary>
		/// Removes all the Channels from the Channel list.
		/// </summary>
		/// <returns>The number of Channels removed from the Channel list.</returns>
		private int RemoveAll()
		{
			int Count = _channels.Count;

			Channel Channel = null;
			_active = null;

			for (int i = _channels.Count - 1; i >= 0; i--)
			{
				Channel = _channels[i];
				// Uncouple the Channel from any events.				
				AttachChildEvents(Channel, false);
				Remove(Channel);
			}
			//_channels.Clear();
			//_sortedChannels.Clear();
			return Count;
		}

		/// <summary>
		/// Removes the Channel at the specified index of the active Sorted Channel list.
		/// </summary>
		/// <param name="index">The zero-based index of the Channel to remove.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than Channel list.Count.</exception>
		public void RemoveAt(int index)
		{
			if ((index < 0) || (index >= _channels.Count))
				throw new ArgumentOutOfRangeException();
			Remove(_sortedChannels[index]);
		}

		#region [ IEnumerable ]

		/// <summary>
		/// Allows for "foreach" statements to be used on an instance of this class, to loop through all the Channels.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public ChannelEnum GetEnumerator()
		{
			return new ChannelEnum(_channels);
		}

		IEnumerator<Channel> IEnumerable<Channel>.GetEnumerator()
		{
			return (IEnumerator<Channel>)GetEnumerator();
		}

		#endregion [ IEnumerable ]

		#endregion [ List Methods ]

		/// <summary>
		/// Adds a new Shuffle to the Shuffle Controller.
		/// </summary>
		/// <param name="newShuffle">New Shuffle object.</param>
		public void AddShuffle(Shuffle newShuffle)
		{
			_shuffleController.Add(newShuffle);
			_shuffleController.Active = newShuffle;
		}

		/// <summary>
		/// Sets the flag on the list of Channels to either allow, or suppress their ability to draw on the Canvas. This does not fire any events. This list cannot be null.
		/// </summary>
		/// <param name="channels">List of Channels. Cannot be null.</param>
		/// <param name="allow">Indicates whether the Channels in the list should be allowed to Draw.</param>
		/// <exception cref="System.ArgumentNullException">channels is null</exception>
		public void AllowChannelDrawing(ChannelList channels, bool allow)
		{
			if (channels == null)
				throw new ArgumentNullException("channels is null");

			// Replaces Hide, Show, HideAll, ShowAll, HideSelected, ShowSelected
			foreach (Channel Channel in channels)
			{
				if (allow)
					Channel.Show();
				else
					Channel.Hide();
			}
		}

		/// <summary>
		/// Adds or remove events from a given Channel.
		/// </summary>
		/// <param name="channel">The Channel to attach/detach events to. The value cannot be null.</param>
		/// <param name="attachEvents"></param>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		private void AttachChildEvents(Channel channel, bool attachEvents)
		{
			if (channel == null)
				throw new ArgumentNullException("channel is null");

			if (attachEvents)
			{
				channel.DirtyChanged += ChildObject_Dirty;
				channel.PropertyChanged += Channel_PropertyChanged;
			}
			else
			{
				channel.DirtyChanged -= ChildObject_Dirty;
				channel.PropertyChanged -= Channel_PropertyChanged;
			}
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(ChannelController source)
		{
			SuppressEvents = true;

			foreach (Channel Channel in source.GetAllChannels())
				Add((Channel)Channel.Clone());

			_shuffleController.CopyFrom(source._shuffleController);
			_groupController.CopyFrom(source._groupController);

			SuppressEvents = false;
		}

		/// <summary>
		/// Deletes a Shuffle from the ShuffleController. Sets the active Shuffle to be the Native one.
		/// </summary>
		/// <param name="deadShuffle">Shuffle object to delete.</param>
		public void DeleteShuffle(Shuffle deadShuffle)
		{
			_shuffleController.Remove(deadShuffle);
			_shuffleController.Active = _shuffleController.List[0];
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			_profile = null;
			if (_shuffleController != null)
			{
				_shuffleController.ShuffleChanged -= ShuffleController_Changed;
				_shuffleController.Switched -= ShuffleController_Switched;
				_shuffleController.Dispose();
				_shuffleController = null;
			}
			if (_groupController != null)
			{
				_groupController.Dispose();
				_groupController = null;
			}
		}

		/// <summary>
		/// Clears out all the cells on all Channels, then fires the ClearAll event.
		/// </summary>
		public void EmptyAllChannels()
		{
			EmptyChannels(_channels);
		}

		/// <summary>
		/// Empties out all the cells on the selected Channels.
		/// </summary>
		/// <param name="collection">List of Channels to empty.</param>
		/// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
		public void EmptyChannels(ChannelList collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection cannot be null.");

			SuppressChannelEvents(collection, true);

			foreach (Channel Channel in collection)
				Channel.Empty();

			SuppressChannelEvents(collection, false);
			OnChannelsChanged(collection, "LatticeChanged");
		}

		/// <summary>
		/// Finds the Channel by its ID value and returns it.
		/// </summary>
		/// <param name="id">ID value on the Channel desired</param>
		/// <returns>If the Channel is not found, returns null, otherwise returns the correct Channel</returns>
		public Channel Get(int id)
		{
			return _channels.Where(id);
		}

		/// <summary>
		/// Returns a list of all the Channels
		/// </summary>
		public ChannelList GetAllChannels()
		{
			return _channels;
		}
				
		/// <summary>
		/// Returns the current Active Shuffle object.
		/// </summary>
		public Shuffle GetCurrentShuffle()
		{
			return _shuffleController.Active;
		}

		/// <summary>
		/// Returns the list of IDs of the channels that are disabled.
		/// </summary>
		public string GetDisabledList()
		{
			string List = string.Empty;

			foreach (Channel Channel in _channels)
			{
				if (!Channel.Enabled)
					List += ((List.Length > 0) ? "," : string.Empty) + Channel.ID;
			}

			return List;
		}

		/// <summary>
		/// Returns a struct holding the display information for the selected channels.
		/// </summary>
		internal DisplaySet GetDisplaySet()
		{
			DisplaySet Set = new DisplaySet();

			Set.CanExclude = CanExclude;
			Set.CanInclude = CanInclude;
			Set.CanMoveDown = CanMoveDown;
			Set.CanMoveUp = CanMoveUp;
			
			Set.HasEnabled = false;
			Set.HasDisabled = false;
			Set.HasExcluded = false;
			Set.HasIncluded = false;
			Set.HasInvisible = false;
			Set.HasLocked = false;
			Set.HasUnlocked = false;
			Set.HasVisible = false;

			foreach (Channel Channel in Selected)
			{
				if (Channel.Enabled)
					Set.HasEnabled = true;
				if (!Channel.Enabled)
					Set.HasDisabled = true;
				if (Channel.Included)
					Set.HasIncluded = true;
				if (!Channel.Included)
					Set.HasExcluded = true;
				if (Channel.Locked)
					Set.HasLocked = true;
				if (!Channel.Locked)
					Set.HasUnlocked = true;
				if (Channel.Visible)
					Set.HasVisible = true;
				if (!Channel.Visible)
					Set.HasInvisible = true;
			}

			return Set;
		}

		/// <summary>
		/// Creates and returns a list of all the IDs of the Channels that are selected.
		/// </summary>
		public List<int> GetSelectedChannelIDs()
		{
			List<int> IDList = new List<int>();
			foreach (Channel Channel in Selected)
				IDList.Add(Channel.ID);
			return IDList;
		}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Enabled property set to true.
		///// </summary>
		//public bool HasEnabled()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (Channel.Enabled)
		//            return true;
		//    }
		//    return false;
		//}
		
		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Enabled property set to false.
		///// </summary>
		//public bool HasDisabled()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (!Channel.Enabled)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Included property set to true.
		///// </summary>
		//public bool HasIncluded()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (Channel.Included)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Included property set to false.
		///// </summary>
		//public bool HasExcluded()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (!Channel.Included)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Visible property set to false.
		///// </summary>
		//public bool HasInvisible()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (!Channel.Visible)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Locked property set to true.
		///// </summary>
		//public bool HasLocked()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (Channel.Locked)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Locked property set to false.
		///// </summary>
		//public bool HasUnlocked()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (!Channel.Locked)
		//            return true;
		//    }
		//    return false;
		//}

		///// <summary>
		///// Indicates if one or more of the selected Channels has it's Visible property set to true.
		///// </summary>
		//public bool HasVisible()
		//{
		//    foreach (RasterChannel Channel in Selected)
		//    {
		//        if (Channel.Visible)
		//            return true;
		//    }
		//    return false;
		//}

		/// <summary>
		/// Determines if the pattern of included channels is valid.
		/// </summary>
		/// <param name="code">Encoded list of channel inclusions.</param>
		/// <returns>Returns true if the exclusions are only at the ends of the list, or there are no inclusion or exclusions.</returns>
		private bool IsLegalIncludes(string code)
		{
			int P2 = -1;
			int P1 = code.IndexOf('O'); // find the first position of the included channels.
			for (int i = (P1 >= 0 ? P1 : 0); i < code.Length; i++)
			{
				if (code[i] == 'X')
				{
					P2 = i;
					break;
				}
			}

			if (P1 == -1)
				return true;
			if (P2 == -1)
				return true;
			if (!code.Substring(P2).Contains("O"))
				return true;

			return false;
		}

		/// <summary>
		/// Selects the Channel with the corresponding index.
		/// </summary>
		/// <param name="index">Index of the Channel to select</param>
		/// <param name="multiSelect">Indicates whether we should unselect all other Channels upon selecting this new one</param>
		/// <exception cref="System.IndexOutOfRangeException">Index cannot be less than zero, or greater than or equal to the Channel List count.</exception>
		public void Select(int index, bool multiSelect)
		{
			ChannelList SelectedChannels = new ChannelList();
			SelectedChannels.Add(this[index]);

			if (!multiSelect)
				UnselectAll(false);

			Selected = SelectedChannels;
			SelectedChannels = null;
		}

		/// <summary>
		/// Selects the Channel with the corresponding index.
		/// </summary>
		/// <param name="index">Index of the Channel to select</param>
		/// <exception cref="System.IndexOutOfRangeException">Index cannot be less than zero, or greater than or equal to the Channel List count.</exception>
		public void Select(int index)
		{
			Select(index, false);
		}

		/// <summary>
		/// Selects a number of channels.
		/// </summary>
		/// <param name="indices">List of Indices that should be selected.</param>
		/// <exception cref="System.IndexOutOfRangeException">Index cannot be less than zero, or greater than or equal to the Channel List count.</exception>
		public void Select(List<int> indices)
		{
			UnselectAll(false);
			int Index = 0;
			ChannelList SelectedChannels = new ChannelList();

			for (int i = 0; i < indices.Count; i++)
			{
				Index = indices[i];
				SelectedChannels.Add(this[Index]);
			}
			Selected = SelectedChannels;
			SelectedChannels = null;
		}

		/// <summary>
		/// Sets the Loading flag for all the Channels.
		/// </summary>
		public void SetLoading(bool loading)
		{
			foreach (Channel Channel in _channels)
				Channel.Loading = loading;
		}

		/// <summary>
		/// Sets the dirty flag to be false for this object and all the Channels, Shuffles and Groups.
		/// </summary>
		public override void SetClean()
		{
			base.SetClean();
			foreach (Channel Channel in _channels)
				Channel.SetClean();

			_shuffleController.SetClean();
			_groupController.SetClean();
		}

		/// <summary>
		/// Sets the RenderColor property on the list of Channels. Suppresses individual Channel event from firing, will fire a single event.
		/// </summary>
		/// <param name="collection">List of Channels. Cannot be null.</param>
		/// <param name="color">The Color to set the Channels to.</param>
		/// <exception cref="System.ArgumentNullException">channels is null</exception>
		public void SetRenderColor(ChannelList collection, Color color)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null");

			SuppressChannelEvents(collection, true);
			foreach (Channel c in collection)
				c.RenderColor = color;
			SuppressChannelEvents(collection, false);

			OnPropertyChanged(collection, Channel.Property_RenderColor);
		}

		/// <summary>
		/// Sets the SequencerColor property on the list of Channels. Suppresses individual Channel event from firing, will fire a single event.
		/// </summary>
		/// <param name="collection">List of Channels. Cannot be null.</param>
		/// <param name="color">The Color to set the Channels to.</param>
		/// <exception cref="System.ArgumentNullException">channels is null</exception>
		public void SetSequencerColor(ChannelList collection, Color color)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null");

			SuppressChannelEvents(collection, true);
			foreach (Channel c in collection)
				c.SequencerColor = color;
			SuppressChannelEvents(collection, false);

			OnPropertyChanged(collection, Channel.Property_SequencerColor);
		}

		/// <summary>
		/// Sets the BorderColor property on the list of Channels. Suppresses individual Channel event from firing, will fire a single event.
		/// </summary>
		/// <param name="collection">List of Channels. Cannot be null.</param>
		/// <param name="color">The Color to set the Channels to.</param>
		/// <exception cref="System.ArgumentNullException">channels is null</exception>
		public void SetBorderColor(ChannelList collection, Color color)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null");

			SuppressChannelEvents(collection, true);
			foreach (Channel c in collection)
				c.BorderColor = color;
			SuppressChannelEvents(collection, false);

			OnPropertyChanged(collection, Channel.Property_BorderColor);
		}

		/// <summary>
		/// Sets the Visible property on the list of Channels. Suppresses individual Channel event from firing, will fire a single event.
		/// </summary>
		/// <param name="collection">List of Channels. Cannot be null.</param>
		/// <param name="visible">Indicates the value to be set in the Channels' Visible property</param>
		/// <exception cref="System.ArgumentNullException">channels is null</exception>
		public void SetVisible(ChannelList collection, bool visible)
		{
			if (collection == null)
				throw new ArgumentNullException("channels is null");

			SuppressChannelEvents(collection, true);
			foreach (Channel c in collection)
				c.Visible = visible;
			SuppressChannelEvents(collection, false);

			OnPropertyChanged(collection, Channel.Property_Visible);
		}

		/// <summary>
		/// Sets the Visible property on all the Channels. Suppresses individual Channel event from firing, will fire a single event.
		/// </summary>
		/// <param name="visible">Indicates the value to be set in the Channels' Visible property</param>
		public void SetVisible(bool visible)
		{
			SetVisible(_channels, visible);
		}

		/// <summary>
		/// Sets the SuppressEvents flag on each of the Channels in the collection.
		/// </summary>
		/// <param name="channelList">List of Channels to clear. Cannot be null.</param>
		/// <param name="suppress">Value to set.</param>
		/// <exception cref="System.ArguementNullException">collection cannot be null.</exception>
		private void SuppressChannelEvents(ChannelList collection, bool suppress)
		{
			if (collection == null)
				throw new ArgumentNullException("collection cannot be null.");

			foreach (Channel Channel in collection)
				Channel.SuppressEvents = suppress;
		}

		/// <summary>
		/// Sets the SuppressEvents flag on each of the Channels.
		/// </summary>
		/// <param name="suppress">Value to set.</param>
		private void SuppressChannelEvents(bool suppress)
		{
			SuppressChannelEvents(_channels, suppress);
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
			ChannelList Unselected = Selected;
			SuppressChannelEvents(Unselected, true);

			foreach (Channel Channel in Unselected)
				Channel.IsSelected = false;

			SuppressChannelEvents(Unselected, false);

			Active = null;
			if (fireEvent)
				OnChannelsSelected(Unselected);

			Unselected = null;
		}

		/// <summary>
		/// Called by the Profile when scaling has changed, inform each Channel to rebuild their GraphicsPath.
		/// </summary>
		public void UpdateChannels()
		{
			foreach (Channel Channel in _channels)
				Channel.LatticeChanged = true;
		}

		/// <summary>
		/// Updates the currently selected Shuffle with the values within the parameter.
		/// </summary>
		/// <param name="changes">Shuffle object containing the changes</param>
		public void UpdateShuffle(Shuffle changes)
		{
			Shuffle Current = GetCurrentShuffle();

			Current.SuppressEvents = true;
			Current.Name = changes.Name;
			Current.DeserializeList(changes.SerializeList());
			Current.SuppressEvents = false;
			OnShuffleChanged(Current);
		}

		#region [ PopulateChannelFromBitmap ]

		/// <summary>
		/// Inserts cells into a Channel via a 1-bit Bitmap.
		/// </summary>
		/// <param name="bitmap">Bitmap to generate cells. White pixels are active cells, black indicates no data. Cannot be null.</param>
		/// <param name="channel">Channel to add the cells to. Cannot be null.</param>
		/// <param name="clearFirst">Indicates whether all the existing cells on the Channel should be removed first.</param>
		/// <exception cref="System.ArgumentNullException">Neither the bitmap, nor the Channel can be null.</exception>
		public void PopulateChannelFromBitmap(Bitmap bitmap, Channel channel, bool clearFirst)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap cannot be null.");
			if (channel == null)
				throw new ArgumentNullException("channel cannot be null.");
			channel.PopulateFromLatticeBuffer(bitmap, clearFirst);
		}

		/// <summary>
		/// Inserts cells into a Channel via a 1-bit Bitmap. Clears the cells first
		/// </summary>
		/// <param name="bitmap">Bitmap to generate cells. White pixels are active cells, black indicates no data.</param>
		/// <param name="channel">Channel to modify</param>
		/// <exception cref="System.ArgumentNullException">Neither the bitmap, nor the Channel can be null.</exception>
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
		/// <exception cref="System.ArgumentNullException">Neither the bitmap, nor the Channel can be null.</exception>
		public void PopulateChannelFromBitmap_Erase(Bitmap bitmap, Channel channel)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap cannot be null.");
			if (channel == null)
				throw new ArgumentNullException("channel cannot be null.");

			// Grab the bitmap representing this Channel
			Bitmap ChannelBitmap = channel.LatticeBuffer;

			using (Graphics g = Graphics.FromImage(ChannelBitmap))
				g.DrawImage(bitmap, 0, 0);

			channel.SuppressEvents = true;
			channel.Empty();
			channel.SuppressEvents = false;

			channel.LatticeBuffer = ChannelBitmap;
		}

		#endregion [ PopulateChannelFromBitmap ]

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when a Channel is added.
		/// </summary>
		/// <param name="channel">Channel added to the Channels List.</param>
		private void OnChannelAdded(Channel channel)
		{
			if (SuppressEvents)
				return;

			if (ChannelAdded == null)
				return;
			ChannelAdded(this, new ChannelEventArgs(channel, "ChannelAdded"));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when the cells of 1 or more Channels have been altered.
		/// </summary>
		/// <param name="collection">List of Channels that have changed.</param>
		private void OnChannelsChanged(ChannelList collection, string propertyName)
		{
			if (SuppressEvents)
				return;

			if (ChannelsChanged == null)
				return;
			ChannelsChanged(this, new ChannelListEventArgs(collection, propertyName));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when the cells of a Channel has been altered.
		/// </summary>
		/// <param name="channel">Channel that has changed.</param>
		private void OnChannelsChanged(Channel channel, string propertyName)
		{
			if (SuppressEvents)
				return;

			if (ChannelsChanged == null)
				return;
			ChannelsChanged(this, new ChannelListEventArgs(channel, propertyName));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when a Channel is removed,
		/// </summary>
		/// <param name="channel">Channel removed from the list.</param>
		private void OnChannelRemoved(Channel channel)
		{
			if (SuppressEvents)
				return;

			_sortedChannels.Clear();

			if (ChannelRemoved != null)
				ChannelRemoved(this, new ChannelEventArgs(channel, "ChannelRemoved"));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when 1 or more Cannels have been selected.
		/// </summary>
		/// <param name="channelList">List of Channels whose Selected property has been set to True.</param>
		private void OnChannelsSelected(ChannelList channelList)
		{
			if (SuppressEvents)
				return;

			if (ChannelsSelected == null)
				return;
			ChannelsSelected(this, new ChannelListEventArgs(channelList));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when a Cannel has been selected.
		/// </summary>
		/// <param name="channel">Channels whose Selected property has been set to True.</param>
		private void OnChannelsSelected(Channel channel)
		{
			if (SuppressEvents)
				return;

			if (ChannelsSelected == null)
				return;
			ChannelsSelected(this, new ChannelListEventArgs(channel));
		}

		/// <summary>
		/// Determines if the event has been set by another object, fires the event in that case.
		/// Used when a Shuffle has changed internally. Either called by the Suffle_Changed delegate or the UpdateShuffle method
		/// </summary>
		/// <param name="shuffle">Shuffle that has changed.</param>
		private void OnShuffleChanged(Shuffle shuffle)
		{
			// Clear out the list of SortedChannels. This will fetch the new version of the list next time the Sorted property is called.
			_sortedChannels.Clear();

			if (ShuffleChanged != null)
				ShuffleChanged(this, new ShuffleEventArgs(shuffle));
		}

		///// <summary>
		///// When a Channel's property has changed, bubble the event up to the next level.
		///// </summary>
		///// <param name="channel">Property whose event has changed.</param>
		///// <param name="propertyName"></param>
		//private void OnChannelPropertyChanged(Channel channel, string propertyName)
		//{
		//    if (this.SuppressEvents)
		//        return;

		//    if (PropertyChanged == null)
		//        return;
		//    PropertyChanged(channel, new PropertyChangedEventArgs(propertyName));
		//}

		#endregion [ Event Triggers ]

		#region [ Event Handlers ]

		public event EventHandlers.ChannelEventHandler ChannelAdded;
		public event EventHandlers.ChannelEventHandler ChannelRemoved;
		public event EventHandlers.ChannelListEventHandler ChannelsChanged;
		public event EventHandlers.ChannelListEventHandler ChannelsSelected;
		public event EventHandlers.ShuffleEventHandler ShuffleChanged;
		public event EventHandlers.ShuffleEventHandler ShuffleSwitched;

		#endregion [ Event Handlers ]

		#region [ Event Delegates ]

		/// <summary>
		/// Intercept the ProperyChanged event coming from a Channel and fire an event 
		/// </summary>
		/// <param name="sender">Channel whose property changed.</param>
		/// <param name="e">PropertyChangedEventArgs, containing the name of the property changed.</param>
		private void Channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != Channel.Property_Selected)
				OnPropertyChanged(sender, e.PropertyName);
			else
				OnChannelsSelected((Channel)sender);
		}

		/// <summary>
		/// Occurs when the Dirty flag on a given object is changed
		/// </summary>
		private void ChildObject_Dirty(object sender, DirtyEventArgs e)
		{
			if (e.IsDirty)
				Dirty = true;
		}

		/// <summary>
		/// Occurs when the Active Shuffle is switched.
		/// </summary>
		private void ShuffleController_Switched(object sender, ShuffleEventArgs e)
		{
			// Clear out the list of SortedChannels. This will fetch the new version of the list next time the Sorted property is called.
			_sortedChannels.Clear();

			if (ShuffleSwitched != null)
				ShuffleSwitched(sender, e);
		}

		/// <summary>
		/// Occurs when the Active Shuffle has been altered.
		/// </summary>
		private void ShuffleController_Changed(object sender, ShuffleEventArgs e)
		{
			OnShuffleChanged(e.Shuffle);
		}


		#endregion [ Event Delegates ]

		#endregion [ Events ]

	}

	#region [ struct DisplaySet ]

	/// <summary>
	/// Struct containing display information for the currently selected Channels.
	/// Used for the Channel explorer context menu and the Editor Channel menu.
	/// </summary>
	internal struct DisplaySet
	{
		internal bool CanMoveUp;
		internal bool CanMoveDown;
		internal bool HasLocked;
		internal bool HasUnlocked;
		internal bool HasEnabled;
		internal bool HasDisabled;
		internal bool HasVisible;
		internal bool HasInvisible;
		internal bool HasIncluded;
		internal bool HasExcluded;
		internal bool CanInclude;
		internal bool CanExclude;

		internal DisplaySet(string blank)
		{
			CanMoveUp = false;
			CanMoveDown = false;
			HasLocked = false;
			HasUnlocked = false;
			HasEnabled = false;
			HasDisabled = false;
			HasVisible = false;
			HasInvisible = false;
			HasIncluded = false;
			HasExcluded = false;
			CanInclude = false;
			CanExclude = false;
		}

		internal DisplaySet(bool canMoveUp, bool canMoveDown, bool hasLocked, bool hasUnlocked, bool hasEnabled, bool hasDisabled,
							bool hasVisible, bool hasInvisible, bool hasIncluded, bool hasExcluded, bool canInclude, bool canExclude)
		{
			CanMoveUp = canMoveUp;
			CanMoveDown = canMoveDown;
			HasLocked = hasLocked;
			HasUnlocked = hasUnlocked;
			HasEnabled = hasEnabled;
			HasDisabled = hasDisabled;
			HasVisible = hasVisible;
			HasInvisible = hasInvisible;
			HasIncluded = hasIncluded;
			HasExcluded = hasExcluded;
			CanInclude = canInclude;
			CanExclude = canExclude;
		}
	}

	#endregion [ struct DisplaySet ]
}

