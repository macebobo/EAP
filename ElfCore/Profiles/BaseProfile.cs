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

namespace ElfCore.Profiles
{
	public sealed class BaseProfile : ElfBase
	{
		#region [ Constants ]

		// Property Names
		public const string Property_CellSize = "CellSize";
		public const string Property_FileName = "FileName";
		public const string Property_LatticeSize = "LatticeSize";
		public const string Property_Name = "Name";
		public const string Property_ShowGridLines = "ShowGridLines";
		public const string Property_Zoom = "Zoom";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private IProfile _profileDataLayer = null;
		private ChannelController _channelController = null;
		private CanvasWindow _canvasWindow = null;
		private Mask _mask = null;
		private Background _background = null;
		private Scaling _scaling = null;
		private UndoController _undoController = null;
		private PictureBox _substituteCanvas = null;
		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		public Background Background
		{
			get { return _background; }
			set { _background = value; }
		}

		/// <summary>
		/// Returns the number of Channels in this Profile.
		/// </summary>
		[DebuggerHidden]
		public int ChannelCount
		{
			get { return _channelController.Count; }
		}

		/// <summary>
		/// Object that controls the list of Channels and their various functions
		/// </summary>
		public ChannelController Channels
		{
			get { return _channelController; }
		}

		/// <summary>
		/// Gets and Sets the cursor for the CanvasPane control.
		/// </summary>
		public Cursor Cursor
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
		/// Path where files are saved by default.
		/// </summary>
		public string DefaultSavePath
		{
			get { return _profileDataLayer.DefaultSavePath; }
			set { _profileDataLayer.DefaultSavePath = value; }
		}

		/// <summary>
		/// Indicate whether this type of Profile should be currently displayed as an option in the Editor.
		/// </summary>
		public bool Enabled
		{
			get { return _profileDataLayer.Enabled; }
			set { _profileDataLayer.Enabled = value; }
		}

		/// <summary>
		/// Extension for the filename
		/// </summary>
		public string FileExtension
		{
			get { return _profileDataLayer.FileExtension; }
			set { _profileDataLayer.FileExtension = value; }
		}

		/// <summary>
		/// Name of the file this Profile is stored in
		/// </summary>
		public string Filename
		{
			get
			{
				string value = _profileDataLayer.Filename;
				if (value.Length == 0)
				{
					value = _profileDataLayer.DefaultSavePath + _profileDataLayer.Name + "." + _profileDataLayer.FileExtension;
					_profileDataLayer.Filename = value;
				}
				return value;
			}
			set
			{
				if (_profileDataLayer.Filename != value)
				{
					_profileDataLayer.Filename = value;
					if (value.Length > 0)
					{
						FileInfo fi = new FileInfo(value);
						Name = fi.Name.Replace(fi.Extension, string.Empty);
						fi = null;
					}
					OnPropertyChanged(Property_FileName, true);
				}
			}
		}

		/// <summary>
		/// Form that is used to display the profile
		/// </summary>
		public CanvasWindow Form
		{
			get { return _canvasWindow; }
			set { _canvasWindow = value; }
		}

		/// <summary>
		/// Image file that represents the Icon of the sequencing program used by this type of Profile.
		/// </summary>
		[XmlIgnore]
		public Bitmap IconImage
		{
			get { return _profileDataLayer.IconImage; }
		}

		/// <summary>
		/// Indicates whether the Mask is currently defined.
		/// </summary>
		public bool HasMask
		{
			get { return _mask.HasMask; }
		}

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		public bool HasRedo
		{
			get { return _undoController.HasRedo; }
		}

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		public bool HasUndo
		{
			get { return _undoController.HasUndo; }
		}

		/// <summary>
		/// Profile Name
		/// </summary>
		public string Name
		{
			get
			{
				string value = _profileDataLayer.Name ?? string.Empty;
				if (value.Length == 0)
				{
					value = "New " + _profileDataLayer.FormatName;
					_profileDataLayer.Name = value;
				}
				return value;
			}
			set
			{
				if (_profileDataLayer.Name != value)
				{
					_profileDataLayer.Name = value;
					if (_canvasWindow != null)
						_canvasWindow.DisplayText = value;
					OnPropertyChanged(Property_Name, true);
				}
			}
		}

		/// <summary>
		/// Scaling data, including Cell Size, Canvas Size, etc.
		/// </summary>
		[XmlIgnore]
		public Scaling Scaling
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
		/// Unique ID assigned to this Profile type.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Used the the edit background form.
		/// </summary>
		[XmlIgnore, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PictureBox SubstituteCanvas
		{
			get { return _substituteCanvas; }
			set { _substituteCanvas = value; }
		}

		[XmlIgnore]
		internal IProfile ProfileDataLayer
		{
			get { return _profileDataLayer; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public BaseProfile(Type profileType)
		{
			_profileDataLayer = (IProfile)Activator.CreateInstance(profileType);
			_background = new Background(this);
			_background.DirtyChanged += ChildObject_Dirty;
			_background.PropertyChanged += Background_PropertyChanged;

			_scaling = new Scaling(true);
			_scaling.DirtyChanged += ChildObject_Dirty;
			_scaling.PropertyChanged += Scaling_PropertyChanged;

			_mask = new Mask(this);
			_mask.Cleared += Mask_MaskCleared;
			_mask.Defined += Mask_MaskDefined;

			_channelController = new ChannelController(this);
			_channelController.ChannelAdded += ChannelController_ChannelAdded;
			_channelController.ChannelRemoved += ChannelController_ChannelRemoved;
			_channelController.ChannelsChanged += ChannelController_ChannelPropertyChanged;
			_channelController.ChannelsSelected += ChannelController_ChannelsSelected;
			_channelController.PropertyChanged += ChannelController_PropertyChanged;
			_channelController.ShuffleChanged += ChannelController_ShuffleChanged;
			_channelController.ShuffleSwitched += ChannelController_ShuffleSwitched;
			_channelController.DirtyChanged += ChildObject_Dirty;

			_canvasWindow = new CanvasWindow(this);
			_canvasWindow.FormClosing += Form_FormClosing;
			_canvasWindow.CanvasPane.MouseEnter += CanvasWindow_MouseEnter;
			_canvasWindow.CanvasPane.MouseLeave += CanvasWindow_MouseLeave;

			_background.Clear();
			_channelController.Clear();
			_scaling.Clear(true);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		#region [ Private Methods ]

		/// <summary>
		///  Clears out all the value for the properies and private variables. Used to initialize the object initially, and when loading new data
		/// </summary>
		//private void Clear()
		//{
		//	_enabled = true;
		//	_background.Clear();
		//	_channelController.Clear();
		//	_scaling.Clear(true);
		//}

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
		//private string GetProfileAttributeName(ProfileType profileType)
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
		//private string GetProfileAttributeExtension(ProfileType profileType)
		//{
		//	object[] customAttributes = profileType.GetType().GetCustomAttributes(typeof(Profile), true);
		//	if ((customAttributes != null) && (customAttributes.Length > 0))
		//		return ((Profile)customAttributes[0]).Extension;
		//	else
		//		return "pro";
		//}

		#endregion [ Private Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Clears out the masked area, moves the cells back from the Move Channel to their proper one, 
		/// and instructs the CanvasWindow to stop displaying the marquee
		/// </summary>
		public void ClearMask()
		{
			if (!_mask.HasMask)
				return;
			_mask.Clear();
			Refresh();
		}

		/// <summary>
		/// Only currently used in Preview
		/// DELETE
		/// </summary>
		public void Clear_UndoStacks()
		{
			_undoController.Clear();
		}

		/// <summary>
		/// Informs the Profile to close and to fire its Closing event.
		/// </summary>
		public void Close()
		{
			if (Closing != null)
				Closing(this, new EventArgs());
		}

		/// <summary>
		/// Copies all the data and objects from the source Profile into this Profile. 
		/// This is used for converting from one Profile format to another.
		/// </summary>
		/// <param name="sourceProfile">BaseProfile object to copy from.</param>
		internal void CopyFrom(BaseProfile sourceProfile)
		{
			SuppressEvents = true;

			_profileDataLayer.Enabled = sourceProfile.Enabled;
			_profileDataLayer.Name = sourceProfile.Name;
			_profileDataLayer.Filename = sourceProfile.Filename;

			_background.CopyFrom(sourceProfile.Background);
			_background.Set();

			_channelController.CopyFrom(sourceProfile._channelController);
			_mask.CopyFrom(sourceProfile._mask);
			_scaling.CopyFrom(sourceProfile.Scaling);
			_undoController.CopyFrom(sourceProfile._undoController);

			SuppressEvents = false;
			OnLoaded();
		}

		/// <summary>
		/// Copies background, channel, scaling and shuffle data from the data layer object into the business objects.
		/// </summary>
		private void CopyFromDataLayer()
		{
			foreach (RawChannel rChannel in _profileDataLayer.Channels)
				_channelController.Add(rChannel);

			// Copy over the background information from the data layer.
			_background.CopyFrom(_profileDataLayer.Background);
			_background.Set();

			// Copy over the scaling information from the data layer.
			_scaling.CopyFrom(_profileDataLayer.Scaling);

			// Copy over the shuffle information from the data layer.
			foreach (string s in _profileDataLayer.ShuffleList)
				_channelController.ShuffleController.Add(new Shuffle(s.Substring(0, s.IndexOf(',')), s.Substring(s.IndexOf(',') + 1)));

			if ((_profileDataLayer.ActiveShuffleIndex >= 0) && (_profileDataLayer.ActiveShuffleIndex < _channelController.ShuffleController.Count))
				_channelController.ShuffleController.ActiveIndex = _profileDataLayer.ActiveShuffleIndex;
			else
				_channelController.ShuffleController.ActiveIndex = 0;

			SetClean();
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		public void DefineMask(Mask newMask)
		{
			_mask.Define(newMask, true, Scaling);
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		/// <param name="createOverlay">Flag to indicate if the overlay bitmap should be created.</param>
		public void DefineMask(Mask newMask, bool createOverlay)
		{
			_mask.Define(newMask, createOverlay, Scaling);
		}

		/// <summary>
		/// Sets the outline of the mask to these values
		/// </summary>
		/// <param name="canvasOutline">GraphicsPath for the Canvas mask</param>
		/// <param name="latticeOutline">GraphicsPath for the Lattice mask</param>
		public void DefineMask(GraphicsPath canvasOutline, GraphicsPath latticeOutline)
		{
			DefineMask(canvasOutline, latticeOutline, Scaling);
		}

		/// <summary>
		/// Sets the outline of the mask to these values
		/// </summary>
		/// <param name="canvasOutline">GraphicsPath for the Canvas mask</param>
		/// <param name="latticeOutline">GraphicsPath for the Lattice mask</param>
		/// <param name="scaling">Current Scaling of this Profile.</param>
		public void DefineMask(GraphicsPath canvasOutline, GraphicsPath latticeOutline, Scaling scaling)
		{
			_mask.Define(canvasOutline, latticeOutline, scaling);
		}

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
		/// Returns the GraphicsPath object used to define the mask outline.
		/// </summary>
		/// <param name="scale">Determines the scale of the outline to be returned.</param>
		public GraphicsPath GetMaskOutline(UnitScale scale)
		{
			if (!HasMask)
				return new GraphicsPath();
			if (scale == UnitScale.Canvas)
				return (GraphicsPath)_mask.CanvasMask.Outline.Clone();
			else
				return (GraphicsPath)_mask.LatticeMask.Outline.Clone();
		}

		/// <summary>
		/// Used by CanvasWindow's drawing method to display the generated overlay.
		/// </summary>
		public Bitmap GetMaskOverlay()
		{
			if (!HasMask)
				return new Bitmap(1,1);
			return _mask.Overlay;
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
		/// Indicates whether the point passed in occurrs within Canvas outline of the mask.
		/// </summary>
		/// <param name="point">Point to check</param>
		/// <returns>Returns true if the point falls within the Canvas outline of the mask, false otherwise.</returns>
		public bool HitTest(Point point)
		{
			if (!HasMask)
				return false;
			if (_mask == null)
				return false;
			else
				return _mask.HitTest(point);
		}

		/// <summary>
		/// Set up the Undo/Redo stacks
		/// </summary>
		public void InitializeUndo()
		{

			if (_undoController == null)
			{
				_undoController = new UndoController(this);
				_undoController.UndoChanged += UndoController_UndoChanged;
				_undoController.RedoChanged += UndoController_RedoChanged;
				_undoController.Completed += UndoController_Completed;
			}
			else
			{
				_undoController.Clear(true);
			}

			_canvasWindow.InitializeUndo();

			if (_workshop.UI.DisableUndo)
				return;

			_undoController.GetInitialSnapshot();
		}

		/// <summary>
		/// Loads in the Profile data using the filename stored within the object
		/// </summary>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public bool Load()
		{
			SuppressEvents = true;
			bool Ret = _profileDataLayer.Load(this);
			CopyFromDataLayer();
			InitializeUndo();
			SuppressEvents = false;
			OnLoaded();
			return Ret;
		}

		/// <summary>
		/// Loads in the Profile data from the file passed in.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		public bool Load(string filename)
		{
			SuppressEvents = true;
			bool Ret =_profileDataLayer.Load(this, filename);
			CopyFromDataLayer();
			InitializeUndo();
			SuppressEvents = false;
			OnLoaded();
			return Ret;
		}

		public bool Load(XmlNode node, List<RawChannel> rawChannelList)
		{
			SuppressEvents = true;
			bool Ret = _profileDataLayer.Load(this, node, rawChannelList);
			CopyFromDataLayer();
			InitializeUndo();
			SuppressEvents = false;
			OnLoaded();
			return Ret;
		}

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		public void MoveMask(Point offset)
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
			_canvasWindow.Refresh();
			//OnMaskChanged(EventSubCategory.Mask_Moved);
		}

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		public void MoveMask(PointF offset)
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
		public void Redo()
		{
			_undoController.Redo();
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
		/// Request the Profile to close. If the profile is dirty, it will prompt the user to see if they want to save.
		/// If the user Saves and it fails, returns false, do not close.
		/// If the user selects Cancel, returns false,  do not close.
		/// Otherwise the dirty bit is set to false and returns true, safe to close.
		/// </summary>
		public bool RequestClose()
		{
			DialogResult Result = DialogResult.None;
			string Message = "Save changes to this Profile before closing?";
			if (Name.Length > 0)
				Message = string.Format("Save changes to \"{0}\" before closing?", Name);

			if (Dirty && !SuppressEvents)
				Result = MessageBox.Show(Message, @"Close Profile", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (Result == DialogResult.Cancel)
			{
				return false;
			}
			if (Result == DialogResult.Yes)
			{
				if (Save())
					return false;
			}
			SetClean();
			return true;
		}

		/// <summary>
		/// Saves the Profile.
		/// </summary>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		public bool Save()
		{
			Cursor LastCursor = Cursor;
			Cursor = Cursors.WaitCursor;

			try
			{
				// Update the RawChannels in the data layer
				_profileDataLayer.Channels.Clear();
				foreach (Channel Channel in Channels)
				{
					_profileDataLayer.Channels.Add(new RawChannel()
					{
						BorderColor = Channel.BorderColor,
						Enabled = Channel.Enabled,
						ID = Channel.ID,
						Included = Channel.Included,
						Locked = Channel.Locked,
						Name = Channel.Name,
						RenderColor = Channel.RenderColor,
						SequencerColor = Channel.SequencerColor,
						Visible = Channel.Visible,
						EncodedRasterData = Channel.LatticeSerialized,
						EncodedVectorData = Channel.VectorSerialized
					});
				}
				_profileDataLayer.Background.CopyFrom(_background);
				_profileDataLayer.Scaling.CopyFrom(_scaling);

				_profileDataLayer.ActiveShuffleIndex = _channelController.ShuffleController.ActiveIndex;
				_profileDataLayer.ShuffleList = new List<string>();
				foreach (Shuffle Shuffle in _channelController.ShuffleController)
				{
					_profileDataLayer.ShuffleList.Add(Shuffle.Name + "," + Shuffle.SerializeList());
				}

				if (_profileDataLayer.Save())
				{
					SetClean();
					return true;
				}
				else
					return false;
			}
			finally
			{
				Cursor = LastCursor;
			}
		}

		/// <summary>
		/// The program has just performed an operation that can be undone. Grab a snapshot of the data
		/// and save the differences between this and the last as a Changeset
		/// </summary>
		/// <param name="action">Text of the operation complete, this will appear in the Undo menu in the Editor</param>
		public void SaveUndo(string action)
		{
			_undoController.SaveUndo(action);
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
		public void SetMask(Mask maskData)
		{
			_mask.Define(maskData, true, Scaling);
		}

		/// <summary>
		/// Triggers an event to fire, due to the user clicking with the Zoom tool on a point on the canvas.
		/// </summary>
		/// <param name="zoomPoint">Point on the canvas the user has clicked</param>
		/// <param name="zoomLevel">New zoom amount to use.</param>
		public void SetClickZoom(Point zoomPoint, float zoomLevel)
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
		public void Undo()
		{
			_undoController.Undo();
		}

		#endregion [ Public Methods ]

		#region [ Internal Methods ]

		/// <summary>
		/// Creates and returns a text representative of the data in the Redo stack.
		/// </summary>
		internal string Debug_RedoStack()
		{
			return _undoController.Debug_RedoStack();
		}

		/// <summary>
		/// Creates and returns a text representative of the data in the Undo stack.
		/// </summary>
		internal string Debug_UndoStack()
		{
			return _undoController.Debug_UndoStack();
		}

		/// <summary>
		/// Creates and returns a text representative of the current undo snapshot.
		/// </summary>
		/// <returns></returns>
		internal string Debug_UndoSnapshot()
		{
			return _undoController.Debug_UndoSnapshot();
		}

		#endregion [ Internal Methods ]

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		private void OnClosing()
		{
			if (Closing != null)
				Closing(this, new EventArgs());
		}
		
		/// <summary>
		/// Called once the Load method has finished to inform client objects of this fact.
		/// </summary>
		private void OnLoaded()
		{
			if (Loaded != null)
				Loaded(this, new EventArgs());
		}

		/// <summary>
		/// Throw the ScalingChanged event.
		/// </summary>
		private void OnScalingChanged()
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
		private void OnChannelPropertyChanged(ChannelList collection, string propertyName)
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
		private void OnChannelRemoved(Channel channel)
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
		private void OnChannelsSelected(ChannelList collection)
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

		//// Explicit interface implementation required. 
		//// Associate IProfile's event with ClickZoom
		//event EventHandlers.ZoomEventHandler IProfile.ClickZoom
		//{
		//	add
		//	{
		//		lock (objectLock)
		//		{
		//			ClickZoom += value;
		//		}
		//	}
		//	remove
		//	{
		//		lock (objectLock)
		//		{
		//			ClickZoom -= value;
		//		}
		//	}
		//}

		//event EventHandler IProfile.Closing
		//{
		//	add
		//	{
		//		lock (objectLock)
		//		{
		//			Closing += value;
		//		}
		//	}
		//	remove
		//	{
		//		lock (objectLock)
		//		{
		//			Closing -= value;
		//		}
		//	}
		//}

		//event EventHandlers.UndoEventHandler IProfile.Redo_Changed
		//{
		//	add
		//	{
		//		lock (objectLock)
		//		{
		//			Redo_Changed += value;
		//		}
		//	}
		//	remove
		//	{
		//		lock (objectLock)
		//		{
		//			Redo_Changed -= value;
		//		}
		//	}
		//}

		//event EventHandlers.UndoEventHandler IProfile.Undo_Changed
		//{
		//	add
		//	{
		//		lock (objectLock)
		//		{
		//			Undo_Changed += value;
		//		}
		//	}
		//	remove
		//	{
		//		lock (objectLock)
		//		{
		//			Undo_Changed -= value;
		//		}
		//	}
		//}

		//event EventHandler IProfile.Undo_Completed
		//{
		//	add
		//	{
		//		lock (objectLock)
		//		{
		//			Undo_Completed += value;
		//		}
		//	}
		//	remove
		//	{
		//		lock (objectLock)
		//		{
		//			Undo_Completed -= value;
		//		}
		//	}
		//}

		#endregion [ Event Handlers ]

		#region [ Event Delegates ]
		
		/// <summary>
		/// Occurs when a property in the Background object is changed.
		/// </summary>
		private void Background_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(sender, e.PropertyName);
		}

		/// <summary>
		/// Occurs when the mouse has entered the Canvas control's rectangle.
		/// </summary>
		private void CanvasWindow_MouseEnter(object sender, EventArgs e)
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
		private void CanvasWindow_MouseLeave(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Canvas_MouseLeave != null)
				Canvas_MouseLeave(sender, e);
		}

		/// <summary>
		/// Occurs when the Dirty flag on a given object is changed
		/// </summary>
		private void ChildObject_Dirty(object sender, DirtyEventArgs e)
		{
			if (e.IsDirty)
				Dirty = true;
		}

		/// <summary>
		/// Occurs when a Channel is added within the Channel Controller.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelController_ChannelAdded(object sender, ChannelEventArgs e)
		{ }

		/// <summary>
		/// Occurs when a Channel is removed from within the Channel Controller.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelController_ChannelRemoved(object sender, ChannelEventArgs e)
		{
			OnChannelRemoved(e.Channel);
		}

		/// <summary>
		/// Occurs when one or more Channels has had one of its properties changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelController_ChannelPropertyChanged(object sender, ChannelListEventArgs e)
		{
			OnChannelPropertyChanged(e.Channels, e.PropertyName);
		}

		/// <summary>
		/// Occurs when one or more Channels has been selected 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChannelController_ChannelsSelected(object sender, ChannelListEventArgs e)
		{
			OnChannelsSelected(e.Channels);
		}

		/// <summary>
		/// Occurs when a property on the Channel Controller, or one of the Channels therein are changed.
		/// </summary>
		private void ChannelController_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is Channel)
			{
				// Trigger the ChannelPropertyChanged event with a new array of 1 Channel.
				OnChannelPropertyChanged(new ChannelList((Channel)sender), e.PropertyName);
			}
			else if (sender is ChannelList)
			{
				// If the sender is being passed as the list of Channels, pass that on.
				OnChannelPropertyChanged((ChannelList)sender, e.PropertyName);
			}
		}

		/// <summary>
		/// Occurs when the Active Shuffle is switched.
		/// </summary>
		private void ChannelController_ShuffleChanged(object sender, ShuffleEventArgs e)
		{
			if (ShuffleChanged != null)
				ShuffleChanged(sender, e);
		}

		/// <summary>
		/// Occurs when the Active Shuffle is switched.
		/// </summary>
		private void ChannelController_ShuffleSwitched(object sender, ShuffleEventArgs e)
		{
			if (ShuffleSwitched != null)
				ShuffleSwitched(sender, e);
		}

		/// <summary>
		/// Occurs when the CanvasWindow form tries to close, either throw program shut down, using selecting a menu item that closes the form, or the user clicks the
		/// "X" on the corner of the form. If the Profile is dirty, prompt the user to save changes, with a Yes|No|Cancel messagebox. If Cancel is clicked, then we need to cancel
		/// the closing.
		/// </summary>
		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			DialogResult Result = DialogResult.None;
			string Message = "Save changes to this Profile before closing?";
			if (Name.Length > 0)
				Message = string.Format("Save changes to \"{0}\" before closing?", Name);

			if (Dirty && !SuppressEvents)
				Result = MessageBox.Show(Message, @"Close Profile", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

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
				SuppressEvents = true;
				Dirty = false;
				OnClosing();
			}
		}

		/// <summary>
		/// Occurs when the Mask is cleared of data.
		/// </summary>
		private void Mask_MaskCleared(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Mask_Cleared != null)
				Mask_Cleared(sender, e);
		}
		
		/// <summary>
		/// Occurs when the Mask is given data and is this defined.
		/// </summary>
		private void Mask_MaskDefined(object sender, EventArgs e)
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
		private void Scaling_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(sender, e.PropertyName);
			OnScalingChanged();
		}

		/// <summary>
		/// Occurs when an undo/redo operation has been completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UndoController_Completed(object sender, EventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Undo_Completed != null)
				Undo_Completed(sender, e);
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Redo stack, or the Redo stack has been emptied
		/// </summary>
		private void UndoController_RedoChanged(object sender, UndoEventArgs e)
		{
			if (SuppressEvents)
				return;
			if (Redo_Changed != null)
				Redo_Changed(sender, e);
		}

		/// <summary>
		/// Occurs when a new item is the topmost item in the Undo stack, or the Undo stack has been emptied
		/// </summary>
		private void UndoController_UndoChanged(object sender, UndoEventArgs e)
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
