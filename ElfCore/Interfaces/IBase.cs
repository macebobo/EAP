using ElfCore.Util;
using System;
using System.ComponentModel;

namespace ElfCore.Interfaces
{
	public interface IBase : IDisposable, INotifyPropertyChanged, ICloneable
	{
		#region [ Properties ]

		/// <summary>
		/// Indicates this data has changed.
		/// </summary>		
		bool Dirty { get; set; }

		/// <summary>
		/// Generated Unique identifier for this object.
		/// </summary>
		string GUID { get; set; }

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		string Serialized { get; set; }

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		bool SuppressEvents { get; set; }
		
		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Sets the dirty flag to be false.
		/// </summary>
		void SetClean();

		#endregion [ Methods ]

		#region [ Events ]

		event EventHandlers.DirtyEventHandler DirtyChanged;

		#endregion [ Events ]
	}
}
