using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace ElfCore.Controllers
{
	[XmlRoot("ShuffleController", IsNullable = false)]
	public class ShuffleController : ElfBase, IEnumerable<Shuffle>
	{

		#region [ Private Properties ]

		private ShuffleList _list;
		private Shuffle _active;
		private XmlHelper _xmlHelper = XmlHelper.Instance;

		#endregion [ Private Properties ]

		#region [ Properties ]

		/// <summary>
		/// Returns the list of all the Shuffles
		/// </summary>
		[XmlIgnore]
		public ShuffleList All
		{
			get { return _list; }
		}

		/// <summary>
		/// Currently selected Shuffle
		/// </summary>
		[XmlIgnore]
		public Shuffle Active
		{
			get
			{
				if (_list.Count == 0)
					return null;
				if (_active == null)
					_active = _list.Where(true);
				if (_active == null)
					_active = _list[0];
				return _active;
			}
			set
			{
				if (!ReferenceEquals(_active, value))
				{
					UnselectAll();
					_active = value;
					if (value != null)
						_active.IsSelected = true;
					OnSwitched();
				}
			}
		}

		/// <summary>
		/// Index of the active sort order
		/// </summary>
		[XmlIgnore]
		public int ActiveIndex
		{
			get { return _list.IndexOf(Active); }
			set { Active = _list[value]; }
		}

		/// <summary>
		/// Gets the number of elements actually contained in the List.
		/// </summary>
		[XmlIgnore]
		public int Count
		{
			get { return _list.Count; }
		}

		/// <summary>
		/// Directly accesses the list with an index.
		/// </summary>
		/// <param name="index">Index of the element</param>
		/// <returns>Returns the element at position indicated.</returns>
		[XmlIgnore]
		public Shuffle this[int index]
		{
			get { return _list[index]; }
			set
			{
				if (_list[index] != value)
				{
					_list[index] = value;
					Dirty = true;
				}
			}
		}

		/// <summary>
		/// Used by the XmlSerializer.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ShuffleList List
		{
			get { return _list; }
			set { _list = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ShuffleController()
			: base()
		{
			_active = null;
			_list = new ShuffleList();

			CreateNativeOrder();
			Dirty = false;
		}

		#endregion [ Constructors ]

		#region [ Private Methods ]

		/// <summary>
		/// Adds or remove events from a given Shuffle.
		/// </summary>
		/// <param name="shuffle">The Shuffle to attach/detach events to. The value cannot be null.</param>
		/// <param name="attachEvents"></param>
		/// <exception cref="System.ArgumentNullException">Channel is null</exception>
		private void AttachChildEvents(Shuffle shuffle, bool attachEvents)
		{
			if (shuffle == null)
				throw new ArgumentNullException("Shuffle is null");

			if (attachEvents)
			{
				shuffle.DirtyChanged += ChildObject_Dirty;
				shuffle.Changed += Shuffle_Changed;
			}
			else
			{
				shuffle.DirtyChanged -= ChildObject_Dirty;
				shuffle.Changed -= Shuffle_Changed;
			}
		}

		private void CreateNativeOrder()
		{
			Shuffle Native = new Shuffle();
			Native.Name = Shuffle.NATIVE_SHUFFLE;
			Native.ReadOnly = true;
			Add(Native);
			Native = null;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			Clear(false);
		}

		/// <summary>
		/// Set the IsSelect property on all items in the list to false
		/// </summary>
		private void UnselectAll()
		{
			foreach (Shuffle Item in _list)
			{
				Item.SuppressEvents = true;
				Item.IsSelected = false;
				Item.SuppressEvents = false;
			}
		}

		#endregion [ Private Methods ]
		
		#region [ Internal Methods ]

		#region [ List Methods ]

		/// <summary>
		/// Adds a new Shuffle to the list.
		/// </summary>
		/// <param name="shuffle">Shuffle to be added</param>
		public void Add(Shuffle shuffle)
		{
			_list.Add(shuffle);
			if (shuffle.ID < 0)
			{
				int ID = _list.Max + 1;
				shuffle.ID = (ID < 0) ? 0 : ID;
			}
			OnAdded(shuffle);
			Dirty = true;
		}

		public void Add(Object shuffle)
		{
			if (shuffle is Shuffle)
				Add((Shuffle)shuffle);
			else
				throw new InvalidCastException("Object to Add must be of type Shuffle");
		}

		/// <summary>
		/// Clears out the list of Shuffles.
		/// </summary>
		internal void Clear()
		{
			Clear(true);
		}

		internal void Clear(bool createNativeOrder)
		{
			foreach (Shuffle Shuffle in _list)
			{
				AttachChildEvents(Shuffle, false);
				Shuffle.Dispose();
			}
			_list.Clear();
			_active = null;
			if (createNativeOrder)
				CreateNativeOrder();
		}

		/// <summary>
		/// Removes the first occurrence of the Shuffle from the List.
		/// </summary>
		/// <param name="shuffle">Shuffle to remove</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List.</returns>
		internal void Remove(Shuffle shuffle)
		{
			if ((shuffle == null) || shuffle.ReadOnly)
				return;

			_list.Remove(shuffle);
			OnRemoved(shuffle);
		}

		/// <summary>
		/// Removes the Shuffle by its name
		/// </summary>
		/// <param name="name">Name of the Shuffle to remove</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List.</returns>
		internal void Remove(string name)
		{
			Remove(_list.Where(name));
		}

		/// <summary>
		/// Removes the Profile at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		internal void RemoveAt(int index)
		{
			if ((index < 0) || (index >= _list.Count))
			{
				//Workshop.Instance.WriteTraceMessage("Invalid Index: " + index, TraceLevel.Warning);
				return;
			}
			Shuffle Order = _list[index];
			if ((Order == null) || Order.ReadOnly)
				return;

			_list.RemoveAt(index);
			OnRemoved(Order);
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

		public ShuffleEnum GetEnumerator()
		{
			return new ShuffleEnum(_list);
		}

		IEnumerator<Shuffle> IEnumerable<Shuffle>.GetEnumerator()
		{
			return (IEnumerator<Shuffle>)GetEnumerator();
		}

		#endregion [ IEnumerable ]

		#endregion [ List Methods ]

		/// <summary>
		/// Add this Channel ID to the end of all Shuffles
		/// </summary>
		/// <param name="id">ID to add</param>
		internal void AddChannelID(int id)
		{
			foreach (Shuffle item in _list)
				item.Add(id);
		}

		/// <summary>
		/// Adds the Channel ID at the index specified on the active Sort Order, and at the end of the list of all other Sort ORders
		/// </summary>
		/// <param name="index">Index of the current Sort Order to insert the ID.</param>
		/// <param name="id">Channel ID to add</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than Count.</exception>
		internal void AddID(int index, int id)
		{
			if ((index < 0) || (index >= Active.Count))
				throw new ArgumentOutOfRangeException();

			foreach (Shuffle Shuffle in _list)
			{
				if (ReferenceEquals(Shuffle, _active))
					_active.Insert(index, id);
				else
					_active.Add(id);
			}

		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of each Shuffle.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the Shuffles.
		/// The collection itself cannot be null, nor can any of the elements therein.</param>
		/// <exception cref="System.ArgumentNullException">collection is null</exception>
		internal void AddChannelIDRange(List<int> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null");

			foreach (Shuffle item in _list)
				item.AddRange(collection);
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(ShuffleController source)
		{
			SuppressEvents = true;

			foreach (Shuffle Shuffle in source.List)
				Add((Shuffle)Shuffle.Clone());

			SuppressEvents = false;
		}

		/// <summary>
		/// Takes the raw, unordered list of Channels and applies the active Shuffle to it.
		/// </summary>
		/// <param name="unorderedChannels">List of Channels in no particular order. Cannot be null.</param>
		/// <returns>the same Channels as passed in, ordered based on the currently active sort order. If there are no Shuffles defined, then orders based on the Channel's ID</returns>
		/// <exception cref="System.ArgumentNullException">unorderedChannels cannot be null.</exception>
		internal ChannelList GetSorted(ChannelList unorderedChannels)
		{
			if (unorderedChannels == null)
				throw new ArgumentNullException("unorderedChannels");

			ChannelList Sorted = new ChannelList();

			if (Active == null)
				return unorderedChannels.OrderByAscending();
			
			// Copy the Channels into a temporary list, so that they can be removed as they are added to the returning List.
			ChannelList Raw = new ChannelList();
			Raw.AddRange(unorderedChannels);

			for (int i = 0; i < Active.Count; i++)
			{
				var Ch = Raw.Where(Active[i]);
				if (Ch != null)
				{
					Sorted.Add(Ch);
					Raw.Remove(Ch);
				}
			}
			
			// If there are any Channels left, add them to the end of the list.
			if (Raw.Count > 0)
			{
				Sorted.AddRange(Raw.OrderByAscending());
			}

			// Now assign the new indices to the Channels. This does not cause an event to be raised.
			int Index = 0;
			foreach (Channel Channel in Sorted)
				Channel.Index = Index++;

			return Sorted;
		}

		/// <summary>
		/// Inserts the Channel ID at the index indicated of the Active Shuffle, and adds it to the end of the other Shuffles
		/// </summary>
		/// <param name="channelId">Channel ID to insert</param>
		internal void InsertChannelID(int index, int channelId)
		{
			Active.Insert(index, channelId);
			foreach (Shuffle item in _list)
			{
				if (item != Active)
					item.Add(channelId);
			}
		}

		///// <summary>
		///// Load the Shuffle list from the Profile xml.
		///// </summary>
		///// <param name="profileNode">The XmlNode that holds the Profile data</param>
		//internal void Load(XmlNode profileNode)
		//{
		//	if (profileNode == null)
		//		return;

		//	// Check to see if there is a Shuffles node under the PlugIn node
		//	XmlNode Node = profileNode.SelectSingleNode(ROOT);
		//	if (Node == null)
		//		return;

		//	int Selected = _xmlHelper.GetAttributeValue(Node, LAST_SORT, -1);
		//	if (Selected >= 0)
		//		Selected--;

		//	int Counter = 0;
		//	foreach (XmlNode ChildNode in Node.ChildNodes)
		//	{
		//		if (ChildNode is XmlWhitespace)
		//			continue;
		//		_list.Add(new Shuffle(ChildNode));
		//		if (Counter == Selected)
		//		{
		//			_list[Counter].IsSelected = true;
		//		}
		//	}

		//	this.Dirty = false;
		//}

		/// <summary>
		/// Removes the Channel ID from all the Shuffles
		/// </summary>
		/// <param name="channelId">Channel ID to remove</param>
		internal void RemoveChannelID(int channelId)
		{
			foreach (Shuffle item in _list)
				item.Remove(channelId);
		}

		///// <summary>
		///// Saves the data back to the Profile Xml object
		///// </summary>
		///// <param name="profileNode">The XmlNode that holds the Profile data</param>
		//internal void Save(XmlNode profileNode)
		//{
		//	if (profileNode == null)
		//		return;

		//	XmlNode Node = profileNode.SelectSingleNode(ROOT);
		//	if (Node == null)
		//		Node = _xmlHelper.CreateNode(profileNode.OwnerDocument, profileNode, ROOT);

		//	// Remove all child nodes
		//	Node.RemoveAll();

		//	// Determine the Index of the Active sort order, add one (because the data is 1 based), and set it as the attribute
		//	int SelectedIndex = -1;
		//	int Counter = 0;
		//	foreach (Shuffle Item in _list)
		//	{
		//		Counter++;
		//		if (object.ReferenceEquals(Item, Active))
		//		{
		//			SelectedIndex = Counter;
		//			break;
		//		}
		//	}

		//	_xmlHelper.SetAttribute(Node, LAST_SORT, SelectedIndex);

		//	foreach (Shuffle Item in _list)
		//		Item.Save(Node);
		//}

		/// <summary>
		/// Sets the active Shuffle, based on the ID passed in. If the ID is LTET zero, then this indicates to use the native sort order of the channels.
		/// </summary>
		/// <param name="id">ID of the Shuffle to make active.</param>
		internal void Set(int id)
		{
			if (id <= 0)
				Active = _list[0];
			else
				Active = _list.Where(id);
		}

		/// <summary>
		/// Sets the active Shuffle, based on the ID passed in. If the ID is LTET zero, then this indicates to use the native sort order of the channels.
		/// </summary>
		/// <param name="id">ID of the Shuffle to make active.</param>
		/// <param name="forceSwitch">Indicates whether we force the firing of the Switched event.</param>
		//internal void Set(int id, bool forceSwitch)
		//{
		//    if (forceSwitch)
		//        this.Active = null;

		//    if (id <= 0)
		//        this.Active = _list[0];
		//    else
		//        this.Active = _list.Where(l => l.ID == id).FirstOrDefault();
		//}

		#endregion [ Internal Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Rebuild the internal objects based on the serialized string. Called by UndoController.ApplyChangeset
		/// </summary>
		public void Deserialize(string serialized)
		{
			if (((serialized ?? string.Empty).Length == 0) || (serialized == UndoController.UNCHANGED))
				return;
			bool Suppress = SuppressEvents;
			SuppressEvents = true;
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml(serialized);

			Clear(false);
			foreach (XmlNode Node in Doc.DocumentElement.ChildNodes)
			{
				if (Node is XmlWhitespace)
					continue;
				Add(Extends.DeserializeObjectFromXml<Shuffle>(Node.OuterXml));
			}

			Active = _list.Where(true);

			SuppressEvents = Suppress;
			OnSwitched();
			OnShuffleChanged(Active);
		}

		/// <summary>
		/// Returns the Shuffle using the ID specified. If not found then returns null.
		/// </summary>
		/// <param name="id">ID to retrieve the correct Shuffle</param>
		public Shuffle Get(int id)
		{
			return _list.Where(id);
		}

		//public void Save(XmlWriter writer)
		//{
		//	writer.WriteStartElement(ROOT);
		//	writer.WriteAttributeString(LAST_SORT, _list.IndexOf(Active).ToString());

		//	foreach (Shuffle shuffle in _list)
		//	{
		//		if (shuffle.ReadOnly)
		//			continue;
		//		writer.WriteStartElement(SORT_ORDER);
		//		writer.WriteAttributeString(Shuffle.Property_Name.ToLower(), shuffle.Name);
		//		writer.WriteString(shuffle.SerializedList);
		//		writer.WriteEndElement();
		//	}
		//	writer.WriteEndElement();
		//}

		/// <summary>
		/// Sets the dirty flag to be false for this object and all the Groups.
		/// </summary>
		public override void SetClean()
		{
			base.SetClean();
			foreach (Shuffle Item in _list)
				Item.SetClean();
		}

		#endregion [ Public Methods ]

		#region [ Events ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when a Shuffle has been Added
		/// </summary>
		public EventHandlers.ShuffleEventHandler Added;

		/// <summary>
		/// Occurs when a Shuffle order list has been altered.
		/// </summary>
		public EventHandlers.ShuffleEventHandler ShuffleChanged;
	
		/// <summary>
		/// Occurs when a Shuffle has been Remove
		/// </summary>
		public EventHandlers.ShuffleEventHandler Removed;

		/// <summary>
		/// Occurs when the Active Shuffle changes.
		/// </summary>
		public EventHandlers.ShuffleEventHandler Switched;

		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		/// <summary>
		/// Handles the throwing of the Switched event
		/// </summary>
		private void OnSwitched()
		{
			if (!_suppressEvents  && (Switched != null))
				Switched(this, new ShuffleEventArgs(Active));
		}

		/// <summary>
		/// Handles the throwing of the Added event
		/// </summary>
		private void OnAdded(Shuffle shuffle)
		{
			AttachChildEvents(shuffle, true);
			if (!_suppressEvents && (Added != null))
				Added(this, new ShuffleEventArgs(shuffle));
		}

		/// <summary>
		/// Handles the throwing of the Removed event
		/// </summary>
		private void OnRemoved(Shuffle shuffle)
		{
			AttachChildEvents(shuffle, false);
			if (!_suppressEvents && (Removed != null))
				Removed(this, new ShuffleEventArgs(shuffle));
		}

		private void OnShuffleChanged(Shuffle shuffle)
		{
			if (ReferenceEquals(shuffle, _active) && (ShuffleChanged != null))
				ShuffleChanged(this, new ShuffleEventArgs(shuffle));
		}

		#endregion [ Event Triggers ]

		#region [ Event Delegates ]

		/// <summary>
		/// One of the Shuffles is flagging itself as dirty.
		/// </summary>
		private void ChildObject_Dirty(object sender, DirtyEventArgs e)
		{
			if (!_suppressEvents && (DirtyChanged != null))
				DirtyChanged(sender, e);
		}

		/// <summary>
		/// When the order list within one of our Shuffles is altered, it throws this event. Catch the event and throw our own.
		/// Only applies to the Active Shuffle
		/// </summary>
		private void Shuffle_Changed(object sender, EventArgs e)
		{
			OnShuffleChanged((Shuffle)sender);
		}

		#endregion [ Event Delegates ]

		#endregion [ Events ]	
	}

}
