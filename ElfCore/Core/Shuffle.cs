using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;

using ElfCore.Util;

namespace ElfCore.Core
{
	/// <summary>
	/// Holds the list of Channel IDs in the order in which they are to be displayed.
	/// </summary>
	[XmlRoot("Shuffle", IsNullable = false)] 
	public class Shuffle : ElfBase
	{
		#region [ Constants ]

		public const string NATIVE_SHUFFLE = "Default Order";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private string _name = string.Empty;
		private bool _isSelected = false;
		private List<int> _list = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Name of the Shuffle.
		/// </summary>
		[DebuggerHidden(), XmlAttribute("name")]
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
		/// Returns the count of records in the list.
		/// </summary>
		[DebuggerHidden(), XmlIgnore()]
		public int Count
		{
			get { return _list.Count; }
		}

		public int ID { get; set; }

		/// <summary>
		/// Returns true if this Shuffle is the native sort order.
		/// </summary>
		[DebuggerHidden(), XmlIgnore()]
		public bool IsNativeShuffle
		{
			get { return (_name == NATIVE_SHUFFLE); }
		}

		/// <summary>
		/// Indicates whether this is the currently selected Shuffle
		/// </summary>
		[DebuggerHidden(), XmlAttribute("isSelected")]
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					Dirty = true;
				}
			}
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore()]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<Shuffle>(this);
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
		/// Indicates whether this Shuffle can be altered. If true, Channel ID's can only be added or removed and this object cannot be deleted.
		/// </summary>
		public bool ReadOnly { get; set; }

		/// <summary>
		/// Overloaded index operator
		/// </summary>
		/// <param name="index">Index of the array to use.</param>
		[DebuggerHidden(), XmlIgnore()]
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

		#endregion [ Properties ]

		#region [ Constructors ]

		public Shuffle()
			: base()
		{
			ReadOnly = false;
			_list = new List<int>();
			_name = string.Empty;
			_isSelected = false;
			ID = -1;
			Dirty = false;
		}

		public Shuffle(string name, string orderList)
			: this()
		{
			_name = name;
			_suppressEvents = true;
			DeserializeList(orderList);
			_suppressEvents = false;
			Dirty = false;
		}

		public Shuffle(string name, string orderedList, int id, bool readOnly, bool isSelected)
			: this(name, orderedList)
		{
			ID = id;
			ReadOnly = readOnly;
			IsSelected = isSelected;
			Dirty = false;
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
				throw new DuplicateNameException(id + " already exists in this Shuffle.");

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
					throw new DuplicateNameException(id + " already exists in this Shuffle.");
			}

			_list.AddRange(collection);
			OnChanged();
		}

		/// <summary>
		/// Clone this Shuffle and its underlying objects.
		/// </summary>
		public override object Clone()
		{
			return new Shuffle(_name, SerializeList(), ID, ReadOnly, IsSelected);
		}

		/// <summary>
		/// Parses a comma-seperated list into the list of numbers.
		/// </summary>
		/// <param name="orderList">Comma-seperated list of numbers</param>
		public void DeserializeList(string orderList)
		{
			if (orderList.Length == 0)
				return;

			_list.Clear();
			foreach (string ID in orderList.Split(','))
				_list.Add(Int32.Parse(ID.Trim()));
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
		/// Move the Channel IDs passed in down one position in the list.
		/// </summary>
		/// <param name="idList">List of Channel IDs</param>
		public void MoveDown(List<int> idList)
		{
			if ((idList == null) || (idList.Count == 0))
				return;

			int ID;
			int TargetID;
			int Index = -1;
			idList.Sort();

			for (int i = idList.Count - 1; i >= 0; i--)
			{
				ID = idList[i];
				Index = _list.IndexOf(ID);

				// If this ID is not present, move on.
				if (Index < 0)
					continue;

				// If the Index of the ID is at the end of the list, move on.
				if (Index == _list.Count - 1)
					continue;

				// Check to see ID in the position we want to be in is present in the list
				// of IDs to move. If so, then we don't want to move to that spot, move on.
				TargetID = _list[Index + 1];
				if (idList.Contains(TargetID))
					continue;

				_list.Remove(ID);
				_list.Insert(Index + 1, ID);
			}
			OnChanged();
		}

		/// <summary>
		/// Move the Channel IDs passed in up one position in the list.
		/// </summary>
		/// <param name="idList">List of Channel IDs</param>
		public void MoveUp(List<int> idList)
		{
			if ((idList == null) || (idList.Count == 0))
				return;

			int ID;
			int TargetID;
			int Index = -1;
			idList.Sort();

			for (int i = 0; i < idList.Count; i++)
			{
				ID = idList[i];
				Index = _list.IndexOf(ID);

				// If this ID is not present, or is at the start of the list, move on.
				if (Index <= 0)
					continue;

				// Check to see ID in the position we want to be in is present in the list
				// of IDs to move. If so, then we don't want to move to that spot, move on.
				TargetID = _list[Index - 1];
				if (idList.Contains(TargetID))
					continue;

				_list.Remove(ID);
				_list.Insert(Index - 1, ID);
			}
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

			if (!Return)
				return false;

			for (int i = 0; i < _list.Count; i++)
			{
				if (_list[i] > id)
					_list[i]--;
			}

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

		/// <summary>
		/// Move the Channel IDs passed in to the bottom of the list.
		/// </summary>
		/// <param name="idList">List of Channel IDs</param>
		public void ToBottom(List<int> idList)
		{
			if ((idList == null) || (idList.Count == 0))
				return;

			int ID;
			idList.Sort();

			int Index = _list.Count - 1;

			for (int i = 0; i < idList.Count; i++)
			{
				ID = idList[i];
				_list.Remove(ID);
				_list.Insert(Index, ID);
			}
			OnChanged();
		}

		/// <summary>
		/// Move the Channel IDs passed in to the top of the list.
		/// </summary>
		/// <param name="idList">List of Channel IDs</param>
		public void ToTop(List<int> idList)
		{
			if ((idList == null) || (idList.Count == 0))
				return;

			int ID;
			int Index = 0;
			idList.Sort();

			for (int i = 0; i < idList.Count; i++)
			{
				ID = idList[i];				
				_list.Remove(ID);
				_list.Insert(Index++, ID);
			}
			OnChanged();
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
		[XmlIgnore()]
		public EventHandler Changed;

		#endregion [ Event Triggers ]

		#endregion [ Events ]
	}

	#region [ Class ShuffleList ]

	public class ShuffleList : CollectionBase, IList<Shuffle>, ICollection<Shuffle>
	{
		public ShuffleList()
		{ }			

		public void Add(Shuffle item)
		{
			List.Add(item);
		}

		public bool Contains(Shuffle item)
		{
			return List.Contains(item);
		}

		public void CopyTo(Shuffle[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(Shuffle item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, Shuffle item)
		{
			List.Insert(index, item);
		}

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}
		
		public int Max
		{
			get
			{
				int max = -1;
				foreach (Shuffle item in List)
					max = Math.Max(max, item.ID);
				return max;
			}
		}

		public void Remove(Shuffle item)
		{
			List.Remove(item);
		}

		bool ICollection<Shuffle>.Remove(Shuffle item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public Shuffle this[int index]
		{
			get { return (Shuffle)List[index]; }
			set { List[index] = value; }
		}

		public Shuffle[] ToArray()
		{
			Shuffle[] Arr = new Shuffle[Count];
			for (int i = 0; i < Count; i++)
				Arr[i] = this[i];
			return Arr;
		}

		public Shuffle Where(int id)
		{
			foreach (Shuffle item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		public Shuffle Where(bool isSelected)
		{
			foreach (Shuffle item in List)
				if (item.IsSelected == isSelected)
					return item;
			return null;
		}

		public Shuffle Where(string name)
		{
			foreach (Shuffle item in List)
				if (string.Compare(item.Name, name, true) == 0)
					return item;
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<Shuffle> IEnumerable<Shuffle>.GetEnumerator()
		{
			return new ShuffleListEnumerator(List.GetEnumerator());
		}

		#region [ Class ShuffleListEnumerator ]

		private class ShuffleListEnumerator : IEnumerator<Shuffle>
		{
			private IEnumerator _enumerator;

			public ShuffleListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public Shuffle Current
			{
				get { return (Shuffle)_enumerator.Current; }
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

		#endregion [ Class ShuffleListEnumerator ]
	}
	
	#endregion [ Class ShuffleList ]
}