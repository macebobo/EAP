using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Forms;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ElfRes = ElfProfiles.Properties.Resources;

namespace ElfProfiles
{
	/// <summary>
	/// Base level class for Profiles, implements the interface IProfile
	/// </summary>
	public abstract class BaseProfile : ElfBase, IProfile, IBase
	{
		#region [ Constants ]

		// Property Names
		private const string Property_CellSize = "CellSize";
		private const string Property_FileName = "FileName";
		private const string Property_LatticeSize = "LatticeSize";
		private const string Property_Name = "Name";
		private const string Property_ShowGridLines = "ShowGridLines";
		private const string Property_Zoom = "Zoom";

		#endregion [ Constants ]

		#region [ Protected Virtual Variables ]

		protected bool _enabled = true;
		protected string _filename = string.Empty;
		protected string _name = string.Empty;

		protected ChannelController _channelController = null;
		protected ElfCore.Forms.CanvasWindow _canvasWindow = null;
		protected Mask _mask = null;
		protected Background _background = null;
		protected Scaling _scaling = null;
		protected UndoController _undoController = null;
		protected PictureBox _substituteCanvas = null;
		protected ProfileType _profileTypeID = ProfileType.NotSet;
		protected Workshop _workshop = Workshop.Instance;

		protected object objectLock = new Object();

		#endregion [ Protected Virtual Variables ]

		#region [ Properties ]

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public virtual Background Background
		{
			get { return _background; }
			set { _background = value; }
		}

		/// <summary>
		/// Path where files are saved by default.
		/// </summary>
		public virtual string DefaultSavePath { get; set; }

		/// <summary>
		/// Name of the file this Profile is stored in
		/// </summary>
		public virtual string Filename
		{
			get
			{
				if (_filename.Length == 0)
					_filename = this.Name + "." + this.FileExtension;
				return _filename;
			}
			set
			{
				if (_filename != value)
				{
					_filename = value;
					if (value.Length > 0)
					{
						FileInfo fi = new FileInfo(value);
						this.Name = fi.Name.Replace(fi.Extension, string.Empty);
						fi = null;
					}
					OnPropertyChanged(Property_FileName, true);
				}
			}
		}

		/// <summary>
		/// Form that is used to display the profile
		/// </summary>
		public virtual CanvasWindow Form
		{
			get { return _canvasWindow; }
			set { _canvasWindow = value; }
		}

		/// <summary>
		/// Name of the type of Profile.
		/// </summary>
		public virtual string FormatName 
		{ 
			get {  throw new NotImplementedException("FormatName is not implemented."); }
		}

		/// <summary>
		/// Image file that represents the Icon of the sequencing program used by this type of Profile.
		/// </summary>
		[XmlIgnore()]
		public virtual Bitmap IconImage
		{
			get { return ElfRes.undefined; }
		}

		/// <summary>
		/// Indicates whether the Mask is currently defined.
		/// </summary>
		public virtual bool HasMask
		{
			get { return _mask.HasMask; }
		}

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		public virtual bool HasRedo
		{
			get { return _undoController.HasRedo; }
		}

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		public virtual bool HasUndo
		{
			get { return _undoController.HasUndo; }
		}

		/// <summary>
		/// Name given to the particular instance of a profile.
		/// </summary>
		public virtual string Name
		{
			get
			{
				//if (_name.Length == 0)
					//_name = "New " + GetProfileAttributeName(this.ProfileTypeID);
				return _name;
			}
			set
			{
				if (_name != value)
				{
					_name = value;
					if (_canvasWindow != null)
						_canvasWindow.DisplayText = value;
					OnPropertyChanged(Property_Name, true);
				}
			}
		}

		/// <summary>
		/// Scaling data, including Cell Size, Canvas Size, etc.
		/// </summary>
		[XmlIgnore()]
		public virtual Scaling Scaling
		{
			get { return _scaling; }
			set
			{
				_scaling.SuppressEvents = true;
				_scaling.LatticeSize = value.LatticeSize;
				_scaling.CellSize = value.CellSize;
				_scaling.ShowGridLines = value.ShowGridLines;
				_scaling.Zoom = value.Zoom;
				_scaling.SuppressEvents = false;
			}
		}

		/// <summary>
		/// Object that controls the list of Channels and their various functions
		/// </summary>
		public virtual ChannelController Channels
		{
			get { return _channelController; }
			set { /* Does Nothing */ }
		}

		/// <summary>
		/// Returns the number of Channels in this Profile.
		/// </summary>
		[DebuggerHidden()]
		public virtual int ChannelCount
		{
			get { return _channelController.Count; }
		}

		/// <summary>
		/// Gets and Sets the cursor for the CanvasPane control.
		/// </summary>
		public virtual Cursor Cursor
		{
			get
			{
				if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
					return _canvasWindow.CanvasPane.Cursor;
				else
					return Cursors.Default;
			}
			set
			{
				if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
					_canvasWindow.CanvasPane.Cursor = value;
			}
		}

		/// <summary>
		/// Indicate whether this type of Profile should be currently displayed as an option in the Editor.
		/// </summary>
		public virtual bool Enabled { get; set; }

		/// <summary>
		/// Extension for the filename
		/// </summary>
		public virtual string FileExtension { get; set; }

		/// <summary>
		/// Unique ID assigned to this Profile type.
		/// </summary>
		public virtual int ID { get; set; }

		/// <summary>
		/// Used the the edit background form.
		/// </summary>
		[XmlIgnore(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual PictureBox SubstituteCanvas
		{
			get { return _substituteCanvas; }
			set { _substituteCanvas = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.
		/// </summary>        
		public BaseProfile()
			: base()
		{
			_background = new Background(this);
			_background.DirtyChanged += new EventHandlers.DirtyEventHandler(ChildObject_Dirty);
			_background.PropertyChanged += new PropertyChangedEventHandler(Background_PropertyChanged);

			_scaling = new Scaling(true);
			_scaling.DirtyChanged += new EventHandlers.DirtyEventHandler(ChildObject_Dirty);
			_scaling.PropertyChanged += new PropertyChangedEventHandler(Scaling_PropertyChanged);

			_mask = new Mask(this);
			_mask.Cleared += new EventHandler(Mask_MaskCleared);
			_mask.Defined += new EventHandler(Mask_MaskDefined);

			_channelController = new ChannelController(this);
			_channelController.ChannelAdded += new EventHandlers.ChannelEventHandler(ChannelController_ChannelAdded);
			_channelController.ChannelRemoved += new EventHandlers.ChannelEventHandler(ChannelController_ChannelRemoved);
			_channelController.ChannelsChanged += new EventHandlers.ChannelListEventHandler(ChannelController_ChannelPropertyChanged);
			_channelController.ChannelsSelected += new EventHandlers.ChannelListEventHandler(ChannelController_ChannelsSelected);
			_channelController.PropertyChanged += new PropertyChangedEventHandler(ChannelController_PropertyChanged);
			_channelController.ShuffleChanged += new EventHandlers.ShuffleEventHandler(ChannelController_ShuffleChanged);
			_channelController.ShuffleSwitched += new EventHandlers.ShuffleEventHandler(ChannelController_ShuffleSwitched);
			_channelController.DirtyChanged += new EventHandlers.DirtyEventHandler(ChildObject_Dirty);

			_canvasWindow = new CanvasWindow(this);
			_canvasWindow.FormClosing += new FormClosingEventHandler(this.Form_FormClosing);
			_canvasWindow.CanvasPane.MouseEnter += new EventHandler(CanvasWindow_MouseEnter);
			_canvasWindow.CanvasPane.MouseLeave += new EventHandler(CanvasWindow_MouseLeave);

			_filename = string.Empty;
			_name = string.Empty;
			_enabled = true;
			_background.Clear();
			_channelController.Clear();
			_scaling.Clear(true);
		}

		/// <summary>
		/// Constructor with a profile's filename to open
		/// </summary>
		/// <param name="filename">Name of the Profile file.</param>
		public BaseProfile(string filename)
			: this()
		{
			Load(filename);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#region [ Protected Virtual Methods ]

		/// <summary>
		///  Clears out all the value for the properies and protected virtual variables. Used to initialize the object initially, and when loading new data
		/// </summary>
		protected virtual void Clear()
		{
			_enabled = true;
			_background.Clear();
			_channelController.Clear();
			_scaling.Clear(true);
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();
			_workshop = null;
			if (_canvasWindow != null)
			{
				_canvasWindow.FormClosing -= Form_FormClosing;
				_canvasWindow.CanvasPane.MouseEnter -= CanvasWindow_MouseEnter;
				_canvasWindow.CanvasPane.MouseLeave -= CanvasWindow_MouseLeave;
				_canvasWindow = null;
			}
			if (_scaling != null)
			{
				_scaling.DirtyChanged -= ChildObject_Dirty;
				_scaling.PropertyChanged -= Scaling_PropertyChanged;
				_scaling.Dispose();
				_scaling = null;
			}
			if (_mask != null)
			{
				_mask.Cleared -= Mask_MaskCleared;
				_mask.Defined -= Mask_MaskDefined;
				_mask.Dispose();
				_mask = null;
			}
			if (_channelController != null)
			{
				_channelController.ChannelAdded -= ChannelController_ChannelAdded;
				_channelController.ChannelRemoved -= ChannelController_ChannelRemoved;
				_channelController.ChannelsChanged -= ChannelController_ChannelPropertyChanged;
				_channelController.ChannelsSelected -= ChannelController_ChannelsSelected;
				_channelController.PropertyChanged -= ChannelController_PropertyChanged;
				_channelController.ShuffleChanged -= ChannelController_ShuffleChanged;
				_channelController.ShuffleSwitched -= ChannelController_ShuffleSwitched;
				_channelController.DirtyChanged -= ChildObject_Dirty;
				_channelController.Dispose();
				_channelController = null;
			}
			if (_background != null)
			{
				_background.DirtyChanged -= ChildObject_Dirty;
				_background.PropertyChanged -= Background_PropertyChanged;
				_background.Dispose();
				_background = null;
			}
			if (_undoController != null)
			{
				_undoController.UndoChanged -= UndoController_UndoChanged;
				_undoController.RedoChanged -= UndoController_RedoChanged;
				_undoController.Completed -= UndoController_Completed;
				_undoController = null;
			}
		}

		///// <summary>
		///// Checks the ElfProfile class attribute, and, if present, returns the Name property
		///// </summary>
		///// <returns>ElfProfile.Name value, if present, "Profile" if not</returns>
		//protected virtual string GetProfileAttributeName(ProfileType profileType)
		//{
		//	object[] customAttributes = profileType.GetType().GetCustomAttributes(typeof(Profile), true);
		//	if ((customAttributes != null) && (customAttributes.Length > 0))
		//		return ((Profile)customAttributes[0]).Name;
		//	else
		//		return "Profile";
		//}

		///// <summary>
		///// Checks the ElfProfile class attribute, and, if present, returns the Extension property
		///// </summary>
		///// <returns>ElfProfile.Name value, if present, "pro" if not</returns>
		//protected virtual string GetProfileAttributeExtension(ProfileType profileType)
		//{
		//	object[] customAttributes = profileType.GetType().GetCustomAttributes(typeof(Profile), true);
		//	if ((customAttributes != null) && (customAttributes.Length > 0))
		//		return ((Profile)customAttributes[0]).Extension;
		//	else
		//		return "pro";
		//}

		/// <summary>
		/// Load in the data specific to this type of Profile.
		/// </summary>
		protected virtual void LoadData(XmlNode pluginNode)
		{ }

		/// <summary>
		/// Abstract method, used to load data specific to this type of Profile.
		/// </summary>
		protected virtual void LoadData()
		{ }

		/// <summary>
		/// Abstract method, used to save the data specific to this type of Profile.
		/// </summary>
		/// <param name="writer"></param>
		protected virtual void SaveData(XmlWriter writer)
		{
			throw new NotImplementedException("BaseProfile.SaveData(XmlWriter)");
		}

		/// <summary>
		/// Save the profile while within Vixen as a Plugin
		/// </summary>
		/// <param name="pluginNode">Xml node that will house our data</param>
		protected virtual void SaveData(XmlNode pluginNode)
		{
			throw new NotImplementedException("BaseProfile.SaveData(XmlNode)");
		}

		#endregion [ Protected Virtual Methods ]

		#region [ Public Virtual Methods ]

		/// <summary>
		/// Clears out the masked area, moves the cells back from the Move Channel to their proper one, 
		/// and instructs the CanvasWindow to stop displaying the marquee
		/// </summary>
		public virtual void ClearMask()
		{
			if (!_mask.HasMask)
				return;
			_mask.Clear();
		}

		/// <summary>
		/// Only currently used in Preview
		/// DELETE
		/// </summary>
		public virtual void Clear_UndoStacks()
		{
			_undoController.Clear();
		}

		/// <summary>
		/// Informs the Profile to close and to fire its Closing event.
		/// </summary>
		public virtual void Close()
		{
			if (Closing != null)
				Closing(this, new EventArgs());
		}

		/// <summary>
		/// Copies all the data and objects from the source Profile into this Profile. 
		/// This is used for converting from one Profile format to another.
		/// </summary>
		/// <param name="sourceProfile">BaseProfile object to copy from.</param>
		internal void CopyFrom(IProfile sourceProfile)
		{
			this.SuppressEvents = true;
			_enabled = sourceProfile.Enabled;
			_background.CopyFrom(sourceProfile.Background);
			_background.Set();
			this.Filename = sourceProfile.Filename;

			_channelController.CopyFrom(sourceProfile._channelController);
			_mask.CopyFrom(sourceProfile.Mask);
			_scaling.CopyFrom(sourceProfile.Scaling);
			_undoController.CopyFrom(sourceProfile._undoController);

			this.SuppressEvents = false;
			OnLoaded();
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		public virtual void DefineMask(Mask newMask)
		{
			_mask.Define(newMask);
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		/// <param name="createOverlay">Flag to indicate if the overlay bitmap should be created.</param>
		public virtual void DefineMask(Mask newMask, bool createOverlay)
		{
			_mask.Define(newMask, createOverlay);
		}

		/// <summary>
		/// Sets the outline of the mask to these values
		/// </summary>
		/// <param name="canvasOutline">GraphicsPath for the Canvas mask</param>
		/// <param name="latticeOutline">GraphicsPath for the Lattice mask</param>
		public virtual void DefineMask(GraphicsPath canvasOutline, GraphicsPath latticeOutline)
		{
			_mask.Define(canvasOutline, latticeOutline);
		}

		/// <summary>
		/// Returns the GraphicsPath object used to define the mask outline.
		/// </summary>
		/// <param name="scale">Determines the scale of the outline to be returned.</param>
		public GraphicsPath GetMaskOutline(UnitScale scale)
		{
			if (!HasMask)
				return new GraphicsPath();
			if (scale == UnitScale.Canvas)
				return _mask.CanvasMask.Outline;
			else
				return _mask.LatticeMask.Outline;
		}

		/// <summary>
		/// Used by CanvasWindow's drawing method to display the generated overlay.
		/// </summary>
		public Bitmap GetMaskOverlay()
		{
			if (!HasMask)
				return new Bitmap(1, 1);
			return _mask.Overlay;
		}

		/// <summary>
		/// Set up the Undo/Redo stacks
		/// </summary>
		public virtual void InitializeUndo()
		{
			if (_undoController == null)
			{
				_undoController = new UndoController(this);
				_undoController.UndoChanged += new EventHandlers.UndoEventHandler(UndoController_UndoChanged);
				_undoController.RedoChanged += new EventHandlers.UndoEventHandler(UndoController_RedoChanged);
				_undoController.Completed += new EventHandler(UndoController_Completed);
			}
			else
			{
				_undoController.Clear(true);
			}

			_undoController.GetInitialSnapshot();
			_canvasWindow.InitializeUndo();
		}

		/// <summary>
		/// Loads in the Profile data using the filename stored within the object
		/// </summary>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public virtual bool Load()
		{
			return Load(this.Filename);
		}

		/// <summary>
		/// Loads in the profile data coming from Vixen.
		/// </summary>
		/// <param name="setupData">XmlNode containing the plugin data</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public virtual bool Load(XmlNode setupData, List<ElfCore.Channels.Properties> rawChannels)
		{
			throw new NotImplementedException("BaseProfile.Load(XmlNode)");
		}

		/// <summary>
		/// Loads in the Profile data from the file passed in.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public virtual bool Load(string filename)
		{
			throw new NotImplementedException("BaseProfile.Load(string)");
		}

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		public virtual void MoveMask(Point offset)
		{
			//System.Drawing.Drawing2D.Matrix MoveMatrix = new System.Drawing.Drawing2D.Matrix();
			//MoveMatrix.Translate(offset.X, offset.Y);

			//// Translate a clone of the path by offset, and replace it with the new one.
			//GraphicsPath MovePath = (GraphicsPath)_maskOutline.Clone();
			//_maskOutline = null;
			//MovePath.Transform(MoveMatrix);
			//MoveMatrix.Dispose();
			//_maskOutline = MovePath;

			//_maskRegion.Translate(offset.X, offset.Y);
			if (!_mask.HasMask)
				return;
			_mask.Move(offset, UnitScale.Lattice);
			//OnMaskChanged(EventSubCategory.Mask_Moved);
		}

		public virtual void MoveMask(PointF offset)
		{
			//System.Drawing.Drawing2D.Matrix MoveMatrix = new System.Drawing.Drawing2D.Matrix();
			//MoveMatrix.Translate(offset.X, offset.Y);

			//// Translate a clone of the path by offset, and replace it with the new one.
			//GraphicsPath MovePath = (GraphicsPath)_maskOutline.Clone();
			//_maskOutline = null;
			//MovePath.Transform(MoveMatrix);
			//MoveMatrix.Dispose();
			//_maskOutline = MovePath;

			//_maskRegion.Translate(offset.X, offset.Y);
			//OnMaskChanged(SpecificEventType.Mask_Moved);
			if (!_mask.HasMask)
				return;
			_mask.Move(offset, UnitScale.Lattice);
			//OnMaskChanged(EventSubCategory.Mask_Moved);
		}

		/// <summary>
		/// Rewinds the last Undo performed, reapplying the changes
		/// </summary>
		public virtual void Redo()
		{
			_undoController.Redo();
		}

		/// <summary>
		/// Saves the Profile.
		/// </summary>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public virtual bool Save()
		{
			throw new NotImplementedException("BaseProfile.Save()");
		}

		/// <summary>
		/// Saves the Profile to file using the filename passed in
		/// </summary>
		/// <param name="filename">Filename to use to save the file to</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public virtual bool Save(string filename)
		{
			throw new NotImplementedException("BaseProfile.Save(string)");
		}

		/// <summary>
		/// Saves the Profile to the XmlNode passed in.
		/// </summary>
		/// <param name="setupData">Xml node that holds the Profile data.</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public virtual bool Save(XmlNode setupData)
		{
			throw new NotImplementedException("BaseProfile.Save(XmlNode)");
		}

		/// <summary>
		/// The program has just performed an operation that can be undone. Grab a snapshot of the data
		/// and save the differences between this and the last as a Changeset
		/// </summary>
		/// <param name="action">Text of the operation complete, this will appear in the Undo menu in the Editor</param>
		public virtual void SaveUndo(string action)
		{
			this._undoController.SaveUndo(action);
		}

		/// <summary>
		/// Sets the dirty flag to be false for this object all its child objects
		/// </summary>
		public override void SetClean()
		{
			base.SetClean();
			_channelController.SetClean();
			_background.SetClean();
			_mask.SetClean();
			_scaling.SetClean();

			OnDirty();
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="maskData">Mask object</param>
		public virtual void SetMask(Mask maskData)
		{
			_mask.Define(maskData, true);
		}

		/// <summary>
		/// Triggers an event to fire, due to the user clicking with the Zoom tool on a point on the canvas.
		/// </summary>
		/// <param name="zoomPoint">Point on the canvas the user has clicked</param>
		/// <param name="zoomLevel">New zoom amount to use.</param>
		public virtual void SetClickZoom(Point zoomPoint, float zoomLevel)
		{
			if (ClickZoom == null)
			{
				_scaling.Zoom = zoomLevel;
				return;
			}
			ClickZoom(this, new ZoomEventArgs(zoomPoint, zoomLevel));
		}

		/// <summary>
		/// Returns the Name of this Profile
		/// </summary>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Detach the CanvasWindow object from the events and destroy it.
		/// </summary>
		public void UnclipCanvasWindow()
		{
			if (_canvasWindow == null)
				return;

			_canvasWindow.FormClosing -= Form_FormClosing;
			_canvasWindow.CanvasPane.MouseEnter -= CanvasWindow_MouseEnter;
			_canvasWindow.CanvasPane.MouseEnter -= CanvasWindow_MouseLeave;
			_canvasWindow.DetachEvents();
			_canvasWindow.Dispose();
			_canvasWindow = null;
		}

		/// <summary>
		/// Looks at the last set of changes and applies the old values.
		/// </summary>
		public virtual void Undo()
		{
			_undoController.Undo();
		}

		#endregion [ Public Virtual Methods ]

		#region [ Internal Methods ]

		/// <summary>
		/// Creates and returns a text representative of the data in the Redo stack.
		/// </summary>
		public string Debug_RedoStack()
		{
			return this._undoController.Debug_RedoStack();
		}

		/// <summary>
		/// Creates and returns a text representative of the data in the Undo stack.
		/// </summary>
		public string Debug_UndoStack()
		{
			return this._undoController.Debug_UndoStack();
		}

		/// <summary>
		/// Creates and returns a text representative of the current undo snapshot.
		/// </summary>
		/// <returns></returns>
		public string Debug_UndoSnapshot()
		{
			return this._undoController.Debug_UndoSnapshot();
		}

		#endregion [ Internal Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// System.Windows.PictureBox control from the CanvasWindow form.
		/// </summary>
		public PictureBox GetCanvas()
		{
			if (_substituteCanvas != null)
				return _substituteCanvas;

			if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
				return _canvasWindow.CanvasPane;
			else
				return null;
		}

		/// <summary>
		/// System.Drawing.Graphics object created from the CanvasPane object.
		/// </summary>
		public Graphics GetCanvasGraphics()
		{
			if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
				return _canvasWindow.CanvasPane.CreateGraphics();
			else
				return null;
		}

		/// <summary>
		/// Takes a peek at the topmost item on the undo (or redo) stack and reports back the text of the item.
		/// If there are no items on the stack, returns an empty string.
		/// </summary>
		public string GetUndoText(bool undo)
		{
			if (undo)
				return _undoController.UndoText;
			else
				return _undoController.RedoText;
		}

		/// <summary>
		/// Release the capture of the mouse cursor
		/// </summary>
		public void ReleaseMouse()
		{
			if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
			{
				_canvasWindow.CanvasPane.Capture = false;
				Cursor.Clip = Rectangle.Empty;
			}
		}

		/// <summary>
		/// Serializes the Mask into a string and returns it.
		/// </summary>
		/// <returns>Serialized mask data.</returns>
		public string SerializeMask()
		{
			//return Util.Extends.SerializeObjectToXml<Mask>(_mask);
			return _mask.Serialized;
		}

		/// <summary>
		/// Trap the mouse to only live inside of the canvas, so we don't get weird effects, like drawings starting outside, or ending outside the pictureBox.
		/// Call ReleaseMouse() on the MouseUp event to allow the cursor to act normal.
		/// </summary>
		public void TrapMouse()
		{
			// Trap the mouse into the Canvas while we are working
			if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
			{
				Rectangle rc = _canvasWindow.CanvasPane.RectangleToScreen(new Rectangle(Point.Empty, _canvasWindow.CanvasPane.ClientSize));
				Cursor.Clip = rc;
				_canvasWindow.CanvasPane.Capture = true;
			}
		}

		/// <summary>
		/// Refresh the redraw on the the CanvasPane.
		/// </summary>
		public void Refresh()
		{
			if ((_canvasWindow != null) && (_canvasWindow.CanvasPane != null))
				_canvasWindow.CanvasPane.Refresh();
		}

		/// <summary>
		/// Determines whether the file indicated is a valid file format for this type of Profile.
		/// </summary>
		/// <param name="filename">Filename of file containing profile data.</param>
		/// <returns>Returns true if this type of Profile can open this file exactly, false otherwise.</returns>
		public virtual bool ValidateFile(string filename)
		{
			throw new NotImplementedException();
		}

		#endregion [ Public Methods ]

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		/// <summary>
		/// Called once the Load method has finished to inform client objects of this fact.
		/// </summary>
		protected virtual void OnLoaded()
		{
			if (Loaded != null)
				this.Loaded(this, new EventArgs());
		}

		/// <summary>
		/// Throw the ScalingChanged event.
		/// </summary>
		protected virtual void OnScalingChanged()
		{
			if (SuppressEvents)
				return;

			//if (_mask.HasMask)
			//    _mask.Clear();

			// Inform the channels scaling has changed.
			_channelController.UpdateChannels();

			if (ScalingChanged != null)
				ScalingChanged(this, new EventArgs());
		}

		/// <summary>
		/// Throw the ChannelPropertyChanged event. Collection cannot be null.
		/// </summary>
		/// <param name="collection">List of Channels.</param>
		/// <param name="propertyName">Property that has changed.</param>
		/// <exception cref="System.ArgumentNullException">Collection cannot be null.</exception>
		protected virtual void OnChannelPropertyChanged(BaseChannelList collection, string propertyName)
		{
			if (SuppressEvents)
				return;

			if (collection == null)
				throw new ArgumentNullException("Collection cannot be null.");

			if (ChannelPropertyChanged != null)
				ChannelPropertyChanged(this, new ChannelListEventArgs(collection, propertyName));
		}

		/// <summary>
		/// Throw the ChannelRemoved event.
		/// </summary>
		/// <param name="removedChannel">Channel that was removed from the list.</param>
		protected virtual void OnChannelRemoved(BaseChannel channel)
		{
			if (SuppressEvents)
				return;

			if (ChannelRemoved != null)
				ChannelRemoved(this, new ChannelEventArgs(channel, "ChannelRemoved"));
		}

		/// <summary>
		/// Throw the ChannelsSelected event. Collection cannot be null.
		/// </summary>
		/// <param name="collection">List of Channels.</param>
		/// <exception cref="System.ArgumentNullException">Collection cannot be null.</exception>
		protected virtual void OnChannelsSelected(BaseChannelList collection)
		{
			if (SuppressEvents)
				return;

			if (collection == null)
				throw new ArgumentNullException("Collection cannot be null.");

			if (ChannelsSelected != null)
				ChannelsSelected(this, new ChannelListEventArgs(collection));
		}

		#endregion [ Event Triggers ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the cursor enters the rectangle defined by the Profile's Canvas control.
		/// </summary>
		public event EventHandler Canvas_MouseLeave;

		/// <summary>
		/// Occurs when the cursor leaves the rectangle defined by the Profile's Canvas control.
		/// </summary>
		public event EventHandler Canvas_MouseEnter;

		/// <summary>
		/// Occurs when a property on one or more Channels has changed.
		/// </summary>
		public event EventHandlers.ChannelListEventHandler ChannelPropertyChanged;

		/// <summary>
		/// Occurs when a Channel is removed from the Channel Controller.
		/// </summary>
		public event EventHandlers.ChannelEventHandler ChannelRemoved;

		/// <summary>
		/// Occurs when one or more Channels have been selected.
		/// </summary>
		public event EventHandlers.ChannelListEventHandler ChannelsSelected;

		/// <summary>
		/// Occurs when the Zoom tool is selected and the user clicks on the Canvas.
		/// </summary>
		public EventHandlers.ZoomEventHandler ClickZoom;

		/// <summary>
		/// Fires when the Profile is being closed, given objects that have event delegates for this profile a chance to remove them.
		/// </summary>
		public EventHandler Closing;

		/// <summary>
		/// Fires once the Load method has finished.
		/// </summary>
		public event EventHandler Loaded;

		/// <summary>
		/// Occurs when the Mask is cleared.
		/// </summary>
		public event EventHandler Mask_Cleared;

		/// <summary>
		/// Occurs when the Mask is defined.
		/// </summary>
		public event EventHandler Mask_Defined;

		/// <summary>
		/// Occurs when a new item is the top item on the Redo stack. This is a bubbled event from the UndoController.
		/// </summary>
		public EventHandlers.UndoEventHandler Redo_Changed;

		/// <summary>
		/// Occurs when one of the scaling properties have been changed.
		/// </summary>
		public event EventHandler ScalingChanged;

		/// <summary>
		/// Occurs when the active Shuffle's order list has been altered
		/// </summary>
		public event EventHandlers.ShuffleEventHandler ShuffleChanged;

		/// <summary>
		/// Occurs when the active Shuffle is to a different shuffle.
		/// </summary>
		public event EventHandlers.ShuffleEventHandler ShuffleSwitched;

		/// <summary>
		/// Occurs when a new item is the top item on the Undo stack. This is a bubbled event from the UndoController.
		/// </summary>
		public EventHandlers.UndoEventHandler Undo_Changed;

		/// <summary>
		/// Occurs when an Undo or Redo operation has completed.
		/// </summary>
		public EventHandler Undo_Completed;

		// Explicit interface implementation required. 
		// Associate IProfile's event with ClickZoom
		event EventHandlers.ZoomEventHandler IProfile.ClickZoom
		{
			add
			{
				lock (objectLock)
				{
					ClickZoom += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					ClickZoom -= value;
				}
			}
		}

		event EventHandler IProfile.Closing
		{
			add
			{
				lock (objectLock)
				{
					Closing += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					Closing -= value;
				}
			}
		}

		event EventHandlers.DirtyEventHandler IBase.DirtyChanged
		{
			add
			{
				lock (objectLock)
				{
					DirtyChanged += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					DirtyChanged -= value;
				}
			}
		}

		event EventHandlers.UndoEventHandler IProfile.Redo_Changed
		{
			add
			{
				lock (objectLock)
				{
					Redo_Changed += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					Redo_Changed -= value;
				}
			}
		}

		event EventHandlers.UndoEventHandler IProfile.Undo_Changed
		{
			add
			{
				lock (objectLock)
				{
					Undo_Changed += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					Undo_Changed -= value;
				}
			}
		}

		event EventHandler IProfile.Undo_Completed
		{
			add
			{
				lock (objectLock)
				{
					Undo_Completed += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					Undo_Completed -= value;
				}
			}
		}

		#endregion [ Event Handlers ]

		#region [ Event Delegates ]

		/// <summary>
		/// Occurs when a property in the Background object is changed.
		/// </summary>
		protected virtual void Background_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(sender, e.PropertyName);
		}

		/// <summary>
		/// Occurs when the mouse has entered the Canvas control's rectangle.
		/// </summary>
		protected virtual void CanvasWindow_MouseEnter(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Canvas_MouseEnter != null)
				Canvas_MouseEnter(sender, e);
		}

		/// <summary>
		/// Occurs when the mouse leaves the Canvas control's rectangle.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void CanvasWindow_MouseLeave(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Canvas_MouseLeave != null)
				Canvas_MouseLeave(sender, e);
		}

		/// <summary>
		/// Occurs when the Dirty flag on a given object is changed
		/// </summary>
		protected virtual void ChildObject_Dirty(object sender, DirtyEventArgs e)
		{
			if (e.IsDirty)
				this.Dirty = true;
		}

		/// <summary>
		/// Occurs when a Channel is added within the Channel Controller.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ChannelController_ChannelAdded(object sender, ChannelEventArgs e)
		{ }

		/// <summary>
		/// Occurs when a Channel is removed from within the Channel Controller.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ChannelController_ChannelRemoved(object sender, ChannelEventArgs e)
		{
			OnChannelRemoved(e.Channel);
		}

		/// <summary>
		/// Occurs when one or more Channels has had one of its properties changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ChannelController_ChannelPropertyChanged(object sender, ChannelListEventArgs e)
		{
			OnChannelPropertyChanged(e.Channels, e.PropertyName);
		}

		/// <summary>
		/// Occurs when one or more Channels has been selected 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ChannelController_ChannelsSelected(object sender, ChannelListEventArgs e)
		{
			OnChannelsSelected(e.Channels);
		}

		/// <summary>
		/// Occurs when a property on the Channel Controller, or one of the Channels therein are changed.
		/// </summary>
		protected virtual void ChannelController_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is BaseChannel)
			{
				// Trigger the ChannelPropertyChanged event with a new array of 1 Channel.
				OnChannelPropertyChanged(new BaseChannelList((BaseChannel)sender), e.PropertyName);
			}
			else if (sender is BaseChannelList)
			{
				// If the sender is being passed as the list of Channels, pass that on.
				OnChannelPropertyChanged((BaseChannelList)sender, e.PropertyName);
			}
		}

		/// <summary>
		/// Occurs when the Active Shuffle is switched.
		/// </summary>
		protected virtual void ChannelController_ShuffleChanged(object sender, ShuffleEventArgs e)
		{
			if (ShuffleChanged != null)
				ShuffleChanged(sender, e);
		}

		/// <summary>
		/// Occurs when the Active Shuffle is switched.
		/// </summary>
		protected virtual void ChannelController_ShuffleSwitched(object sender, ShuffleEventArgs e)
		{
			if (ShuffleSwitched != null)
				ShuffleSwitched(sender, e);
		}

		/// <summary>
		/// Occurs when the CanvasWindow form tries to close, either throw program shut down, using selecting a menu item that closes the form, or the user clicks the
		/// "X" on the corner of the form. If the Profile is dirty, prompt the user to save changes, with a Yes|No|Cancel messagebox. If Cancel is clicked, then we need to cancel
		/// the closing.
		/// </summary>
		protected virtual void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			DialogResult Result = DialogResult.None;
			string Message = "Save changes to this Profile before closing?";
			if (this.Name.Length > 0)
				Message = string.Format("Save changes to \"{0}\" before closing?", this.Name);

			if (this.Dirty)
				Result = MessageBox.Show(Message, "Close Profile", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (Result == DialogResult.Cancel)
			{
				e.Cancel = true;
				return;
			}
			if (Result == DialogResult.Yes)
			{
				if (!Save())
				{
					e.Cancel = true;
					return;
				}
			}
			else
			{
				this.SuppressEvents = true;
				this.Dirty = false;
			}
		}

		/// <summary>
		/// Occurs when the Mask is cleared of data.
		/// </summary>
		protected virtual void Mask_MaskCleared(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Mask_Cleared != null)
				Mask_Cleared(sender, e);
		}

		/// <summary>
		/// Occurs when the Mask is given data and is this defined.
		/// </summary>
		protected virtual void Mask_MaskDefined(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Mask_Defined != null)
				Mask_Defined(sender, e);
		}

		/// <summary>
		/// Occurs when a property in the Scaling object is changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void Scaling_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(sender, e.PropertyName);
			OnScalingChanged();
		}

		/// <summary>
		/// Occurs when an undo/redo operation has been completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void UndoController_Completed(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Undo_Completed != null)
				Undo_Completed(sender, e);
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Redo stack, or the Redo stack has been emptied
		/// </summary>
		protected virtual void UndoController_RedoChanged(object sender, UndoEventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Redo_Changed != null)
				Redo_Changed(sender, e);
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Undo stack, or the Undo stack has been emptied
		/// </summary>
		protected virtual void UndoController_UndoChanged(object sender, UndoEventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Undo_Changed != null)
				Undo_Changed(sender, e);
		}

		#endregion [ Events Delegates ]

		#endregion [ Events ]

	}
}
