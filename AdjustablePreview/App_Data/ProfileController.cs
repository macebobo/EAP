using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace ElfCore
{
	/// <summary>
	/// Controls the list of Profiles and determines which is the active one. When one profile becomes active, its properties override those of the UISettings
	/// object and the User Intefaces updates with these settings.
	/// </summary>
	public class ProfileSettings
	{
		#region [ Private Variables ]

		/// <summary>
		/// The Profile that is currently active.
		/// </summary>
		private Profile _active = null;

		/// <summary>
		/// List of all the currently load Profiles
		/// </summary>
		private List<Profile> _profiles = null;

		private Workshop _workshop = Workshop.Instance;

		private bool _dirty = false;

		#endregion [ Private Variables ]


		#region [ Properties ]

		/// <summary>
		/// The active Profile
		/// </summary>
		public Profile Active
		{
			get
			{
				if ((_active == null) && (_profiles.Count > 0))
					this.Active = _profiles[0];
				return _active;
			}
			set
			{
				if ((_active != value) && (_active != null))
				{ 
					// Move the old active profile's properties for the UISettings object back over to it.
					StoreProfileValues(_active);
				}
				_active = value;
				LoadProfileValues(_active);
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
		public bool Dirty
		{
			get
			{
				foreach (Profile profile in _profiles)
					if (profile.Dirty)
						return true;
				return _dirty;
			}
			set
			{
				if (_dirty != value)
				{
					foreach (Profile profile in _profiles)
					{
						profile.SuppressEvents = true;
						profile.Dirty = value;
						profile.SuppressEvents = false;
					}
					if (_dirty != value)
					{
						_dirty = value;
						//OnChanged(this, ChannelEventType.Dirty);
					}
				}
			}
		}
		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public ProfileSettings()
		{
			
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Loads the value from the Profile into UISettings
		/// </summary>
		/// <param name="profile">Profile to load from</param>
		private void LoadProfileValues(Profile profile)
		{
			_workshop.UI.CellSize = profile.CellSize;
			_workshop.UI.LatticeSize = profile.LatticeSize;
			_workshop.UI.RespectChannelOutputsDuringPlayback = profile.RedirectOutputs;
			_workshop.UI.InactiveChannelAlpha = profile.InactiveChannelAlpha;

			// Load channel data.
		}

		/// <summary>
		/// Saves the current values in UISettings to this Profile
		/// </summary>
		/// <param name="profile">Profile to save</param>
		private void StoreProfileValues(Profile profile)
		{
			profile.CellSize = _workshop.UI.CellSize;
			profile.LatticeSize = _workshop.UI.LatticeSize;
			profile.RedirectOutputs = _workshop.UI.RespectChannelOutputsDuringPlayback;
			profile.InactiveChannelAlpha = _workshop.UI.InactiveChannelAlpha;
			
			// Save channel data.
			
		}

		#endregion [ Methods ]

		#region [ Events ]

		#endregion [ Events ]
	}
}
