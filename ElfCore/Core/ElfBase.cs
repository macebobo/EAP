using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ElfCore.Core
{
	/// <summary>
	/// Base class to use for all data objects that throw a DirtyChanged event.
	/// Inherits ElfDisposable.
	/// Implements INotifyPropertyChanged, ICloneable
	/// </summary>
	public abstract class ElfBase : ElfDisposable, INotifyPropertyChanged, ICloneable
	{

		#region [ Protected Variables ]

		protected bool _dirty = false;
		protected bool _suppressEvents = false;
		protected string _serialized = string.Empty;

		// http://bytes.com/topic/c-sharp/answers/274921-removing-all-event-handlers
		protected List<EventHandler> _delegates = new List<EventHandler>();

		#endregion [ Protected Variables ]

		#region [ Properties ]

		/// <summary>
		/// Indicates this data has changed. Is not set if the Workshop.Loading flag is set to true
		/// </summary>
		[XmlIgnore()]
		public virtual bool Dirty
		{
			get { return _dirty; }
			set
			{
				if ((_dirty != value) /*&& !Workshop.Loading*/)
				{
					_dirty = value;
					if (_dirty)
						OnDirty();
				}
			}
		}

		/// <summary>
		/// Generated Unique identifier for this object.
		/// </summary>
		[XmlIgnore()]
		public string GUID { get; set; }

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("GUID")]
		public string GUID_Serialized
		{
			get { return GUID; }
			set { GUID = value; }
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore()]
		public virtual string Serialized
		{
			get	{ return _serialized; }
			set { _serialized = value; }
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		[XmlIgnore()]
		public virtual bool SuppressEvents
		{
			get { return _suppressEvents; }
			set { _suppressEvents = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>
		/// The default Constructor.
		/// </summary>
		public ElfBase()
		{
			// Create a new GUID for this object so we can more easily tell which is which.
			CreateNewGUID();

			if (_disposed)
				GC.ReRegisterForFinalize(true);
			_disposed = false;
			_suppressEvents = false;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create a deep clone of this object.
		/// </summary>
		public virtual object Clone()
		{
			object MyClone = MemberwiseClone();
			((ElfBase)MyClone).CreateNewGUID();
			return MyClone;
		}

		/// <summary>
		/// Generate a new GUID for this object.
		/// </summary>
		protected void CreateNewGUID()
		{
			GUID = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Sets the dirty flag to be false.
		/// </summary>
		public virtual void SetClean()
		{
			Dirty = false;
		}

		/// <summary>
		/// Displays the properties of this object, used for debugging.
		/// </summary>
		public virtual string ToDebugString(int indent)
		{
			StringBuilder Output = new StringBuilder();
			string PropValue = string.Empty;
			foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				object value = prop.GetValue(this, new object[] { });
				PropValue = string.Format("{0} = {1}", prop.Name, value);
				if (indent > 0)
					PropValue = new String('\t', indent) + PropValue;
				Output.AppendLine(PropValue);
			}
			return Output.ToString();
		}

		public virtual string ToDebugString()
		{
			return ToDebugString(0);
		}

		#endregion [ Methods ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the Dirty property changes.
		/// </summary>
		[XmlIgnore()]
		public EventHandlers.DirtyEventHandler DirtyChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		/// <summary>
		/// If the event is consumed somewhere, then throw the event.
		/// </summary>
		protected void OnDirty()
		{
			if (!SuppressEvents && (DirtyChanged != null))
				DirtyChanged(this, new DirtyEventArgs(_dirty));
		}

		/// <summary>
		/// Called when a property's value has been changed. Raises the PropertyChanged event with the name of the Property.
		/// If indicated, sets the Dirty flag for this object.
		/// </summary>
		/// <param name="propertyName">Name of the property that has been changed.</param>
		/// <param name="setDirty">Indicates whether the Dirty flag should be set to true.</param>
		protected virtual void OnPropertyChanged(string propertyName, bool setDirty)
		{
			OnPropertyChanged(this, propertyName, setDirty);
		}

		/// <summary>
		/// Called when a property's value has been changed. Raises the PropertyChanged event with the name of the Property.
		/// If indicated, sets the Dirty flag for this object.
		/// </summary>
		/// <param name="sender">Object that is raising this event.</param>
		/// <param name="propertyName">Name of the property that has been changed.</param>
		/// <param name="setDirty">Indicates whether the Dirty flag should be set to true.</param>
		protected virtual void OnPropertyChanged(object sender, string propertyName, bool setDirty)
		{
			_serialized = string.Empty;

			if (SuppressEvents)
				return;

			if (setDirty && !Dirty)
				Dirty = true;

			if (PropertyChanged != null)
				PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Called when a property's value has been changed. Raises the PropertyChanged event with the name of the Property.
		/// If indicated, sets the Dirty flag for this object.
		/// </summary>
		/// <param name="sender">Object whose property changed.</param>
		/// <param name="propertyName">Name of the property that has been changed.</param>
		protected virtual void OnPropertyChanged(object sender, string propertyName)
		{
			if (SuppressEvents)
				return;

			if (PropertyChanged != null)
				PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
		}

		#endregion [ Event Triggers ]

	}
}

//// http://social.msdn.microsoft.com/forums/en-US/csharpgeneral/thread/576f69e7-55aa-4574-8d31-417422954689
//Example Code to remove all delegates from an event
//void RemoveClickEvent(Button b)
//{
//    FieldInfo f1 = typeof(Control).GetField("EventClick",BindingFlags.Static|BindingFlags.NonPublic);
//    object obj = f1.GetValue(b);
//    PropertyInfo pi = button1.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
//    EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
//    list.RemoveHandler(obj, list[obj]);
//}