using ElfCore.Interfaces;
using ElfCore.Util;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ElfCore.PlugIn
{
	#region [ Class PlugInProfile ]

	// http://www.c-sharpcorner.com/UploadFile/rmcochran/plug_in_architecture09092007111353AM/plug_in_architecture.aspx
	/// <summary>
	/// This class wraps the PlugIn
	/// </summary>
	[DebuggerDisplay("Name = {Name}")]
	public class PlugInProfile : ElfDisposable, IItem
	{
		#region [ Private Variables ]

		private IProfile _profile = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Indicates if this Profile has been added.
		/// </summary>
		public bool Added { get; set; }

		/// <summary>
		/// Unique ID number
		/// </summary>
		public int ID 
		{
			get 
			{
				if (_profile.ID <= 0)
					_profile.ID = _profile.GetHashCode();
				return _profile.ID; 
			}
			set { _profile.ID = value; } 
		}

		/// <summary>
		/// Index of this object's position within a toolbox.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Returns the name of the Profile's FormatName.
		/// </summary>
		public string Name
		{
			get { return _profile.FormatName; }
		}

		/// <summary>
		/// Link to the Tool contained herein
		/// </summary>
		public IProfile Profile
		{
			get { return _profile; }
			set { _profile = value; }
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public PlugInProfile()
		{ }

		public PlugInProfile(IProfile profile)
			: this()
		{
			_profile = profile;
		}

		public PlugInProfile(IProfile profile, int id)
			: this(profile)
		{
			ID = id;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();

			if (_profile != null)
			{
				_profile.Dispose();
				_profile = null;
			}
		}

		public string FileFilter()
		{
			return string.Format("{0} (*.{1})|*.{1}", _profile.FormatName, _profile.FileExtension.ToLower());
		}

		/// <summary>
		/// Determines if the file passed in matches this type of Profile.
		/// </summary>
		/// <param name="filename">Name of the file to load.</param>
		/// <returns>Returns true if the type of profile this object hold can successfully load the file indicaed.</returns>
		public bool IsProfile(string filename)
		{
			return _profile.ValidateFile(filename);
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion [ Methods ]
	}

	#endregion [ Class PlugInProfile ]

	#region [ Class PlugInProfileList ]

	public class PlugInProfileList : CollectionBase, IList<PlugInProfile>, ICollection<PlugInProfile>
	{
		#region [ Properties ]

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public PlugInProfile this[int index]
		{
			get { return (PlugInProfile)List[index]; }
			set { List[index] = value; }
		}

		/// <summary>
		/// Creates the filter list for the Open|Save file dialogs.	
		/// </summary>
		public string FilterList
		{
			get
			{
				string Filter = string.Empty;
				foreach (PlugInProfile pProfile in this)
					Filter += pProfile.FileFilter() + "|";
				return Filter + "All Files (*.*)|*.*";
			}
		}

		#endregion [ Properties ]
		
		#region [ Constructors ]

		public PlugInProfileList()
		{ }

		#endregion [ Constructors ]

		#region [ Methods ]

		public void Add(PlugInProfile item)
		{
			List.Add(item);
		}

		public bool Contains(PlugInProfile item)
		{
			return List.Contains(item);
		}

		public void CopyTo(PlugInProfile[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(PlugInProfile item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, PlugInProfile item)
		{
			List.Insert(index, item);
		}

		public void Remove(PlugInProfile item)
		{
			List.Remove(item);
		}

		/// <summary>
		/// Sorts the List objects by ID
		/// </summary>
		public void Sort()
		{
			SortedList<int, PlugInProfile> Sorted = new SortedList<int, PlugInProfile>();
			foreach (PlugInProfile p in this)
				Sorted.Add(p.ID, p);
			Clear();
			foreach (KeyValuePair<int, PlugInProfile> KVP in Sorted)
				Add(KVP.Value);
			Sorted = null;
		}

		bool ICollection<PlugInProfile>.Remove(PlugInProfile item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}
	
		public PlugInProfile Where(int id)
		{
			foreach (PlugInProfile item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<PlugInProfile> IEnumerable<PlugInProfile>.GetEnumerator()
		{
			return new PlugInProfileListEnumerator(List.GetEnumerator());
		}

		#endregion [ Methods ]

		#region [ Class PlugInToolGroupListEnumerator ]

		private class PlugInProfileListEnumerator : IEnumerator<PlugInProfile>
		{
			#region [ Private Variables ]

			private IEnumerator _enumerator;

			#endregion [ Private Variables ]

			#region [ Properties ]

			public PlugInProfile Current
			{
				get { return (PlugInProfile)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			#endregion [ Properties ]

			#region [ Constructors ]

			public PlugInProfileListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			#endregion [ Constructors ]

			#region [ Methods ]

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{ }

			#endregion [ Methods ]
		}

		#endregion [ Class PlugInToolGroupListEnumerator ]
	}

	#endregion [ Class PlugInProfileList ]

}
