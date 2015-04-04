using ElfCore.Core;
using ElfCore.Util;

using System.ComponentModel;
using System.Xml.Serialization;

namespace ElfCore.Controllers
{
	public class ChannelGroupController : ElfBase
	{
		#region [ Constants ]

		private const string ROOT = "ChannelGroups";
		//private const string LAST_SORT = "lastSort";

		#endregion [ Constants ]

		#region [ Private Properties ]

		[XmlIgnore]
		private ChannelGroupList _list = null;

		[XmlIgnore]
		private ChannelGroup _active = null;

		[XmlIgnore]
		private XmlHelper _xmlHelper = XmlHelper.Instance;

		#endregion [ Private Properties ]

		#region [ Properties ]

		/// <summary>
		/// Currently selected ChannelGroup
		/// </summary>
		[XmlIgnore]
		public ChannelGroup Active
		{
			get
			{
				if (_list.Count == 0)
					return null;
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
					//_active.IsSelected = true;
					//OnSwitched();
				}
			}
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
		/// Used by the XmlSerializer.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ChannelGroupList List
		{
			get { return _list; }
			set { _list = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelGroupController()
			: base()
		{
			_active = null;
			_list = new ChannelGroupList();
			base.Dirty = false;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Adds a new ChannelGroup to the list.
		/// </summary>
		/// <param name="newGroup">ChannelGroup to be added</param>
		public void Add(ChannelGroup newGroup)
		{
			_list.Add(newGroup);
			//OnAdd(newGroup);
			Dirty = true;
		}

		/// <summary>
		/// Clears out the list.
		/// </summary>
		public void Clear()
		{
			foreach (ChannelGroup Item in _list)
				Item.Dispose();
			_list.Clear();
			_active = null;
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(ChannelGroupController source)
		{
			SuppressEvents = true;

			foreach (ChannelGroup Group in source.List)
				Add((ChannelGroup)Group.Clone());

			SuppressEvents = false;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			foreach (ChannelGroup Item in _list)
				Item.Dispose();
			_list = null;
		}

		/// <summary>
		/// Removes the first occurrence of the ChannelGroup from the List.
		/// </summary>
		/// <param name="group">ChannelGroup to remove</param>
		public void Remove(ChannelGroup group)
		{
			if (group != null)
				_list.Remove(group);
		}

		/// <summary>
		/// Removes the ChannelGroup by its name
		/// </summary>
		/// <param name="name">Name of the ChannelGroup to remove</param>
		public void Remove(string name)
		{
			ChannelGroup Group = _list.Where(name);
			if (Group != null)
				Remove(Group);
		}

		/// <summary>
		/// Removes the Profile at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			if ((index < 0) || (index >= _list.Count))
			{
				//Workshop.Instance.WriteTraceMessage("Invalid Index: " + index, TraceLevel.Warning);
				return;
			}
			ChannelGroup Group = _list[index];
			_list.RemoveAt(index);
		}

		/// <summary>
		/// Sets the dirty flag to be false for this object and all the Groups.
		/// </summary>
		public override void SetClean()
		{
			base.SetClean();
			foreach (ChannelGroup Item in _list)
				Item.SetClean();
		}

		/// <summary>
		/// Set the IsSelect property on all items in the list to false
		/// </summary>
		private void UnselectAll()
		{
			foreach (ChannelGroup Item in _list)
			{
				Item.SuppressEvents = true;
				//Item.IsSelected = false;
				Item.SuppressEvents = false;
			}
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Handlers ]


		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		///// <summary>
		///// Handles the throwing of the Switched event
		///// </summary>
		//private void OnSwitched()
		//{
		//    if (Switched == null)
		//        return;
		//    Switched(this, new ChannelGroupEventArgs(Active));
		//}

		///// <summary>
		///// Handles the throwing of the Added event
		///// </summary>
		//private void OnAdded(ChannelGroup group)
		//{
		//    if (Added == null)
		//        return;
		//    Added(this, new ChannelGroupEventArgs(group));
		//}

		///// <summary>
		///// Handles the throwing of the Removed event
		///// </summary>
		//private void OnRemoved(ChannelGroup group)
		//{
		//    if (Removed == null)
		//        return;
		//    Removed(this, new ChannelGroupEventArgs(group));
		//}

		#endregion [ Event Triggers ]

		#region [ Event Delegates ]

		#endregion [ Event Delegates ]

		#endregion [ Events ]
	}

}
