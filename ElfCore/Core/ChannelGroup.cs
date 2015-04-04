using System.Data;

using ElfCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;

using ElfCore.XmlSerializers;

namespace ElfCore.Core
{
	/// <summary>
	/// Holds the list of Channel IDs in the order in which they are to be displayed.
	/// </summary>
	[XmlRoot("Group", IsNullable = false)]
	public class ChannelGroup : ElfBase
	{
		
		#region [ Private Properties ]

		private string _name = string.Empty;
		private Color _color = Color.Empty;
		private int _zoom = 100;
		private List<int> _list = null;
		private int _id;

		#endregion [ Private Properties ]

		#region [ Properties ]

		/// <summary>
		/// Name of the Group.
		/// </summary>
		[DebuggerHidden, XmlAttribute("Name")]
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					Dirty = true;
				}
			}
		}

		/// <summary>
		/// Color of the Group.
		/// </summary>
		[XmlIgnore]
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					Dirty = true;
				}
			}
		}

		/// <summary>
		/// Used only for serialization.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlAttribute("Color")]
		public string ColorSerialized
		{
			get { return XmlColor.FromBaseType(_color); }
			set { _color = XmlColor.ToBaseType(value); }
		}

		/// <summary>
		/// Returns the count of records in the list.
		/// </summary>
		[DebuggerHidden, XmlIgnore]
		public int Count
		{
			get { return _list.Count; }
		}

		public int ID 
		{
			get { return _id; }
			set { _id = value; } 
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<ChannelGroup>(this);
				return base.Serialized;
			}
		}

		/// <summary>
		/// Converts between the list of IDs and a comma-seperated string.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("List")]
		public string SerializedList
		{
			get { return SerializeList(); }
			set
			{
				bool Suppress = _suppressEvents;
				_suppressEvents = true;
				DeserializeList(value);
				_suppressEvents = Suppress;
			}
		}

		/// <summary>
		/// Overloaded index operator
		/// </summary>
		/// <param name="index">Index of the array to use.</param>
		[DebuggerHidden, XmlIgnore]
		public int this[int index]
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
		/// Name of the Group.
		/// </summary>
		[DebuggerHidden, XmlAttribute("Zoom")]
		public int Zoom
		{
			get { return _zoom; }
			set
			{
				if (_zoom != value)
				{
					_zoom = value;
					Dirty = true;
				}
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ChannelGroup()
			: base()
		{
			_list = new List<int>();
			_name = string.Empty;
			_zoom = 100;
			_color = Color.Empty;
			ID = -1;
			Dirty = false;
		}

		public ChannelGroup(string name, string list)
			: this()
		{
			_name = name;
			Deserialize(list);
			base.Dirty = false;
		}

		public ChannelGroup(string name, int id, string list)
			: this(name, list)
		{
			_id = id;
			base.Dirty = false;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Adds a new ID to the list. The ID must be unique within the List.
		/// </summary>
		/// <param name="id">Element to be added</param>
		/// <exception cref="System.Data.DuplicateNameException">ID cannot be already present in the List.</exception>
		internal void Add(int id)
		{
			if (_list.Contains(id))
				throw new DuplicateNameException(id + " already exists in this Group.");

			_list.Add(id);
			OnChanged();
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the List. The IDs in the collection must be unique within the List.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the List.
		/// The collection itself cannot be null, nor can any of the elements therein.</param>
		/// <exception cref="System.ArgumentNullException">collection is null</exception>
		/// <exception cref="System.Data.DuplicateNameException">ID cannot be already present in the List.</exception>
		internal void AddRange(List<int> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection is null");

			foreach (int id in collection)
			{
				if (_list.Contains(id))
					throw new DuplicateNameException(id + " already exists in this Group.");
			}

			_list.AddRange(collection);
			OnChanged();
		}

		/// <summary>
		/// Clone this Group and its underlying objects.
		/// </summary>
		public override object Clone()
		{
			return new ChannelGroup(_name, ID, SerializeList());
		}

		/// <summary>
		/// Parses a comma-seperated list into the list of numbers. Does not trigger event.
		/// </summary>
		/// <param name="orderList">Comma-seperated list of numbers</param>
		private void Deserialize(string orderList)
		{
			if (orderList.Length == 0)
				return;

			_list.Clear();
			foreach (string ID in orderList.Split(','))
				_list.Add(Int32.Parse(ID.Trim()));
		}

		/// <summary>
		/// Parses a comma-seperated list into the list of numbers.
		/// </summary>
		/// <param name="orderList">Comma-seperated list of numbers</param>
		public void DeserializeList(string orderList)
		{
			Deserialize(orderList);
			OnChanged();
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			_list = null;
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the entire List.
		/// </summary>
		/// <param name="id">The id to locate in the List.</param>
		/// <returns>The zero-based index of the first occurrence of item within the entire List. if found; otherwise, –1.</returns>
		internal int IndexOf(int id)
		{
			return _list.IndexOf(id);
		}

		/// <summary>
		/// Inserts an element into the List at the index indicated. The ID must be unique within the List.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="id">The Channel ID to insert</param>
		/// <exception cref="System.Data.DuplicateNameException">ID cannot be already present in the List.</exception>
		internal void Insert(int index, int id)
		{
			_list.Insert(index, id);
			OnChanged();
		}

		/// <summary>
		/// Removes the ID from all the List
		/// </summary>
		/// <param name="id">Channel ID to remove</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List.</returns>
		internal bool Remove(int id)
		{
			bool Return = _list.Remove(id);
			OnChanged();
			return Return;
		}

		/// <summary>
		/// Convert the list into a comma seperated string.
		/// </summary>
		public string SerializeList()
		{
			string Data = string.Empty;
			for (int i = 0; i < _list.Count; i++)
			{
				Data += (Data.Length > 0 ? "," : string.Empty) + _list[i];
			}
			return Data;
		}

		public override string ToString()
		{
			return _name;
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Handlers ]

		private void OnChanged()
		{
			_serialized = string.Empty;

			Dirty = true;
			if (!_suppressEvents && (Changed != null))
				Changed(this, new EventArgs());
		}

		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		/// <summary>
		/// Occurs when the data in the list is changed in any way
		/// </summary>
		[XmlIgnore]
		public EventHandler Changed;

		#endregion [ Event Triggers ]

		#endregion [ Events ]
	}

	public class ChannelGroupList : CollectionBase, IList<ChannelGroup>, ICollection<ChannelGroup>
	{
		public ChannelGroupList()
		{ }

		public void Add(ChannelGroup item)
		{
			List.Add(item);
		}

		public bool Contains(ChannelGroup item)
		{
			return List.Contains(item);
		}

		public void CopyTo(ChannelGroup[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(ChannelGroup item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, ChannelGroup item)
		{
			List.Insert(index, item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<ChannelGroup>.Remove(ChannelGroup item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public void Remove(ChannelGroup item)
		{
			List.Remove(item);
		}

		public ChannelGroup this[int index]
		{
			get { return (ChannelGroup)List[index]; }
			set { List[index] = value; }
		}

		public ChannelGroup Where(int id)
		{
			foreach (ChannelGroup item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		public ChannelGroup Where(string name)
		{
			foreach (ChannelGroup item in List)
				if (string.Compare(item.Name, name, true) == 0)
					return item;
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<ChannelGroup> IEnumerable<ChannelGroup>.GetEnumerator()
		{
			return new ChannelGroupListEnumerator(List.GetEnumerator());
		}

		private class ChannelGroupListEnumerator : IEnumerator<ChannelGroup>
		{
			private IEnumerator _enumerator;

			public ChannelGroupListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public ChannelGroup Current
			{
				get { return (ChannelGroup)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{
			}
		}

	}

}