using System.Collections;
using System.Collections.Generic;

namespace ElfCore.Util
{
	/*
	#region [ Class SupportedProfile ]

	/// <summary>
	/// Basic information about the type of Profiles this application supports
	/// </summary>
	public class SupportedProfile
	{
		#region [ Fields ]

		/// <summary>
		/// Three-letter file extension on saved Profile files
		/// </summary>
		public string Extension;

		/// <summary>
		/// Public name of the SupportedProfile
		/// </summary>
		public string Name;

		public ProfileType ProfileType;

		#endregion [ Fields ]

		#region [ Properties ]

		/// <summary>
		/// Indicates whether this SupportedProfile is ready for support.
		/// </summary>
		public bool Enabled
		{
			get
			{ 
				if (this.ProfileType == Util.ProfileType.NotSet)
					return false;
				if (this.ProfileType == Util.ProfileType.PlugIn)
					return true;
				return ProfileTypeExtensions.GetEnabled(this.ProfileType);
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public SupportedProfile()
		{
			ProfileType = Util.ProfileType.NotSet;
			Name = string.Empty;
			Extension = string.Empty;
		}

		public SupportedProfile(ProfileType profileType)
			: this()
		{
			this.ProfileType = profileType;
			this.Name = ProfileTypeExtensions.GetName(profileType);
			this.Extension = ProfileTypeExtensions.GetExtension(profileType);
		}

		public SupportedProfile(string name, string ext)
			: this()
		{
			this.Name = name;
			this.Extension = ext;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public string FileFilter()
		{
			return string.Format("{0} (*.{1})|*.{1}|", Name, Extension.ToLower());
		}

		public override string ToString()
		{
			return Name;
		}

		public string GetPath()
		{
			return string.Empty;
		}

		#endregion [ Methods ]

	}

	#endregion [ Class SupportedProfile ]

	#region [ Class SupportedProfileList ]

	public class SupportedProfileList : CollectionBase, IList<SupportedProfile>, ICollection<SupportedProfile>
	{
		public SupportedProfileList()
		{ }

		public void Add(SupportedProfile item)
		{
			List.Add(item);
		}

		public bool Contains(SupportedProfile item)
		{
			return List.Contains(item);
		}

		public void CopyTo(SupportedProfile[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(SupportedProfile item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, SupportedProfile item)
		{
			List.Insert(index, item);
		}

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public void Remove(SupportedProfile item)
		{
			List.Remove(item);
		}

		bool ICollection<SupportedProfile>.Remove(SupportedProfile item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public SupportedProfile this[int index]
		{
			get { return (SupportedProfile)List[index]; }
			set { List[index] = value; }
		}

		public SupportedProfile Where(ProfileType profileType)
		{
			foreach (SupportedProfile item in List)
				if (item.ProfileType == profileType)
					return item;
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<SupportedProfile> IEnumerable<SupportedProfile>.GetEnumerator()
		{
			return new SupportedProfileListEnumerator(List.GetEnumerator());
		}

		private class SupportedProfileListEnumerator : IEnumerator<SupportedProfile>
		{
			private IEnumerator _enumerator;

			public SupportedProfileListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public SupportedProfile Current
			{
				get { return (SupportedProfile)_enumerator.Current; }
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

	#endregion [ Class SupportedProfileList ]
	*/
}
