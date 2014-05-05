using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.PlugIn;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ElfCore.Controllers
{
	/// <summary>
	/// Controls the list of Profiles and determines which is the active one. When one profile becomes active, its properties override those of the UISettings
	/// object and the User Intefaces updates with these settings.
	/// </summary>
	public class ProfileController : ElfBase
	{
		#region [ Private Variables ]

		/// <summary>
		/// The Profile that is currently active.
		/// </summary>
		private BaseProfile _active = null;

		/// <summary>
		/// List of all the currently load Profiles
		/// </summary>
		private List<BaseProfile> _profiles = null;

		private Workshop _workshop = Workshop.Instance;
		private XmlHelper _xmlHelper = XmlHelper.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// The active Profile
		/// </summary>
		public BaseProfile Active
		{
			get
			{
				if ((_active == null) && (_profiles.Count > 0))
					Active = _profiles[0];
				return _active;
			}
			set
			{
				BaseProfile LastActive = null;
				if ((_active != value) && (_active != null))
				{
					LastActive = _active;
				}
				if (!ReferenceEquals(_active, value))
				{
					_active = value;
					OnSwitched(_active, LastActive);
				}
			}
		}

		/// <summary>
		/// Returns the count of all the Profiles
		/// </summary>
		public int Count
		{
			get { return _profiles.Count; }
		}

		/// <summary>
		/// Indicates whether any of the Profiles have been modified
		/// </summary>
		public override bool Dirty
		{
			get
			{
				foreach (BaseProfile profile in _profiles)
				{
					if (profile.Dirty)
					{
						base.Dirty = true;
						break;
					}
				}
				return base.Dirty;
			}
			set
			{
				if (base.Dirty != value)
				{
					foreach (BaseProfile profile in _profiles)
					{
						profile.SuppressEvents = true;
						profile.Dirty = value;
						profile.SuppressEvents = false;
					}
					if (base.Dirty != value)
					{
						base.Dirty = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the list of all Profiles
		/// </summary>
		public List<BaseProfile> List
		{
			get { return _profiles; }
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public override bool SuppressEvents
		{
			get { return base.SuppressEvents; }
			set
			{
				base.SuppressEvents = value;
				foreach (BaseProfile Profile in _profiles)
					Profile.SuppressEvents = value;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public ProfileController()
			: base()
		{
			_profiles = new List<BaseProfile>();
		}

		public ProfileController(string[] args) : this()
		{
			Load(args);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Adds a new Profile to the list
		/// </summary>
		/// <param name="profile">Profile to add</param>
		public void Add(BaseProfile profile)
		{
			_profiles.Add(profile);
			profile.DirtyChanged += ChildObject_Dirty;
			profile.Closing += Profile_Closing;
			OnAdded(profile);

			if (_active == null)
				Active = _profiles[0];
		}

		/// <summary>
		/// Remove all the Profiles from this controller.
		/// </summary>
		public void Clear()
		{
			if (_profiles.Count > 0)
			{
				BaseProfile p = null;
				for (int i = 0; i < _profiles.Count; i++)
				{
					p = _profiles[0];
					Remove(p);
				}
				_active = null;
			}
		}

		/// <summary>
		/// Converts the existing profile to the new type.
		/// </summary>
		/// <param name="profile">BaseProfile object to convert</param>
		/// <param name="targetProfileType">Type of the new object</param>
		public void ConvertProfile(BaseProfile profile, Type targetProfileType)
		{
			SuppressEvents = true;

			if (profile.GetType() == targetProfileType)
				return;

			BaseProfile ConvertedProfile = new BaseProfile(targetProfileType);
			ConvertedProfile.InitializeUndo();
			ConvertedProfile.CopyFrom(profile);
			Add(ConvertedProfile);
			profile.SetClean();
			Remove(profile);
			Active = ConvertedProfile;

			SuppressEvents = false;
		}

		/// <summary>
		/// Determine the type of Profile that is being loaded by the structure of the file.
		/// </summary>
		/// <param name="filename">Name of the file to load.</param>
		/// <returns>Returns the Type of the exact object need to load this Profile file.</returns>
		public Type DetectProfileType(string filename)
		{
			if ((filename ?? string.Empty).Length == 0)
				throw new ArgumentException("Missing filename");

			FileInfo FI = new FileInfo(filename);
			if (!FI.Exists)
				throw new FileNotFoundException("File not found.", filename);

			foreach (PlugInProfile pProfile in _workshop.AvailableProfiles)
			{
				if (pProfile.IsProfile(filename))
					return pProfile.Profile.GetType();
			}

			return null;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			for (int i = _profiles.Count - 1; i >= 0; i--)
			{
				if (_profiles[i] != null)
				{
					_profiles[i].DirtyChanged -= ChildObject_Dirty;
					_profiles[i].Closing -= Profile_Closing;
					_profiles[i].Dispose();
					_profiles[i] = null;
				}
			}
			_profiles = null;
			_xmlHelper = null;
			_workshop = null;
		}

		/// <summary>
		/// Fires off a Switched event. This is used when we are loading the profile into the Editor and we
		/// want to let all the sundry object think a Profile has been loaded.
		/// </summary>
		public void FireSwitched()
		{
			OnSwitched(Active, null);
		}

		/// <summary>
		/// Loads the profile from the filename
		/// </summary>
		/// <param name="filename">Name of the file to open.</param>
		/// <returns>Returns true if the file is successfully loaded.</returns>
		public BaseProfile Load(string filename)
		{
			BaseProfile Profile = null;
			
			Type ProfileType = DetectProfileType(filename);
			if (ProfileType == null)
					return null;

			Profile = new BaseProfile(ProfileType);
			Profile.SuppressEvents = true;
			if (!Profile.Load(filename))
				return null;

			Add(Profile);
			Profile.SuppressEvents = false;
			return Profile;
		}

		/// <summary>
		/// Loads the profile from the setup data.
		/// </summary>
		/// <param name="profileSetupNode">Xml node handed to the Editor from the main program.</param>
		/// <param name="profileType">Enum indicating the type of profile this is.</param>
		/// <returns>Returns true if the file is successfully loaded.</returns>
		public BaseProfile Load(XmlNode profileSetupNode, List<RawChannel> rawChannels, Type profileType)
		{
			if (profileType == null)
				return null;

			BaseProfile Profile = new BaseProfile(profileType);
			if (!Profile.Load(profileSetupNode, rawChannels))
				return null;

			Add(Profile);
			return Profile;
		}		
		
		/// <summary>
		/// Loads 1 or more Profiles from a string array
		/// </summary>
		/// <param name="args">Array containing file names of Profiles</param>
		public void Load(string[] args)
		{
			BaseProfile Profile = null;
			Type ProfileType = null;

			foreach (string filename in args)
			{
				ProfileType = DetectProfileType(filename);
				if (ProfileType == null)
					continue;

				Profile = new BaseProfile(ProfileType);
				if (Profile.Load(filename))
					Add(Profile);
			}
			Profile = null;
		}

		/// <summary>
		/// Removes the first occurrence of the Profile from the List.
		/// </summary>
		/// <param name="profile">Profile to remove</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List.
		/// </returns>
		public bool Remove(BaseProfile profile)
		{
			if (profile == null)
				return false;

			int Index = _profiles.IndexOf(profile);

			if (_profiles.Remove(profile))
			{
				OnRemoved(profile);
				profile.Close();
				profile.DirtyChanged -= ChildObject_Dirty;
				profile.Closing -= Profile_Closing;

				if (ReferenceEquals(profile, _active))
				{ 
					// If we are removing the Active profile, see if there is another further down the list. If not, see if there is another further up the list.
					if (Index < _profiles.Count)
						Active = _profiles[Index];
					else if (Index > 0)
						Active = _profiles[Index - 1];
					else
						Active = null;
				}

				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Removes the Profile at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			if ((index < 0) || (index >= _profiles.Count))
			{
				//_workshop.WriteTraceMessage("Invalid Index: " + index, TraceLevel.Warning);
				return;
			}
			BaseProfile Profile = _profiles[index];
			Profile.Close();
			Profile.DirtyChanged -= ChildObject_Dirty;
			Profile.Closing -= Profile_Closing;
			_profiles.RemoveAt(index);
			OnRemoved(Profile);

			if (ReferenceEquals(Profile, _active))
			{
				// If we are removing the Active profile, see if there is another further down the list. If not, see if there is another further up the list.
				if (index < _profiles.Count)
					Active = _profiles[index];
				else if (index > 0)
					Active = _profiles[index - 1];
				else
					Active = null;
			}
		}

		#endregion [ Methods ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when a Profile has been Added
		/// </summary>
		public EventHandlers.ProfileEventHandler Added;

		/// <summary>
		/// Occurs when a Profile has been Remove
		/// </summary>
		public EventHandlers.ProfileEventHandler Removed;

		/// <summary>
		/// Occurs when the Active Profile changes.
		/// </summary>
		public EventHandlers.ProfileEventHandler Switched;

		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		/// <summary>
		/// Handles the throwing of the Switched event
		/// </summary>
		/// <param name="activeProfile">The currently Active Profile</param>
		/// <param name="oldProfile">The previous active Profile, can be NULL</param>
		private void OnSwitched(BaseProfile activeProfile, BaseProfile oldProfile)
		{
			if (Switched != null)
				Switched(this, new ProfileEventArgs(activeProfile, oldProfile));
		}

		/// <summary>
		/// Handles the throwing of the Added event
		/// </summary>
		private void OnAdded(BaseProfile profile)
		{
			if (Added != null)
				Added(this, new ProfileEventArgs(profile));
		}

		/// <summary>
		/// Handles the throwing of the Removed event
		/// </summary>
		private void OnRemoved(BaseProfile profile)
		{
			if (Removed != null)
				Removed(this, new ProfileEventArgs(profile));
		}

		#endregion [ Event Triggers ]

		#region [ Events ]

		/// <summary>
		/// Occurs when the Dirty flag on a given object is changed
		/// </summary>
		private void ChildObject_Dirty(object sender, DirtyEventArgs e)
		{
			if (e.IsDirty)
				Dirty = true;
		}

		private void Profile_Closing(object sender, EventArgs e)
		{
			if (_profiles.Contains((BaseProfile)sender))
				Remove((BaseProfile)sender);
		}

		#endregion [ Events ]
	}
}
