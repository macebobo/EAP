using ElfCore.Channels;
using ElfCore.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using ElfCore.Profiles;

namespace ElfCore.Interfaces
{
	public interface IProfile : IDisposable
	{		

		#region [ Properties ]

		/// <summary>
		/// Index of the Shuffle that is current.
		/// </summary>
		int ActiveShuffleIndex { get; set; }

		/// <summary>
		/// Object that contains data and methods specific to the User Interface
		/// </summary>
		Background Background { get; set; }

		/// <summary>
		/// Object that controls the list of Channels and their various functions
		/// </summary>
		List<RawChannel> Channels { get; set; }

		/// <summary>
		/// Path where files are saved by default.
		/// </summary>
		string DefaultSavePath { get; set; }

		/// <summary>
		/// Indicate whether this type of Profile should be currently displayed as an option in the Editor.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Extention for the filename
		/// </summary>
		string FileExtension { get; set; }

		/// <summary>
		/// Name of the file this Profile is stored in
		/// </summary>
		string Filename { get; set; }

		/// <summary>
		/// Name of the type of Profile.
		/// </summary>
		string FormatName { get; }

		/// <summary>
		/// Image file that represents the Icon of the sequencing program used by this type of Profile.
		/// </summary>
		Bitmap IconImage { get; }

		/// <summary>
		/// Unique ID assigned to this Profile type.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// Name given to the particular instance of a profile.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Scaling data, including Cell Size, Canvas Size, etc.
		/// </summary>
		Scaling Scaling { get; set; }

		/// <summary>
		/// List of Shuffles. Each shuffle is represented by the name of the shuffle followed by a comma and then a list of channel ID, comma seperated
		/// ex: "New Suffle,0,1,2,3,4,5,6,7"
		/// </summary>
		List<string> ShuffleList { get; set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Loads in the Profile data using the filename stored within the object
		/// </summary>
		/// <param name="profile">BaseProfile object that is the parent to the object that implements this.</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		bool Load(BaseProfile profile);

		/// <summary>
		/// Loads in the profile data coming from Vixen.
		/// </summary>
		/// <param name="profile">BaseProfile object that is the parent to the object that implements this.</param>
		/// <param name="setupData">XmlNode containing the plugin data</param>
		/// <param name="rawChannels">List of RawChannel data.</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		bool Load(BaseProfile profile, XmlNode setupData, List<RawChannel> rawChannels);

		/// <summary>
		/// Loads in the Profile data from the file passed in.
		/// </summary>
		/// <param name="profile">BaseProfile object that is the parent to the object that implements this.</param>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>Returns true if the load is successful, false otherwise</returns>
		bool Load(BaseProfile profile, string filename);

		/// <summary>
		/// Saves the Profile.
		/// </summary>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		bool Save();

		/// <summary>
		/// Saves the Profile to file using the filename passed in
		/// </summary>
		/// <param name="filename">Filename to use to save the file to</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		bool Save(string filename);

		/// <summary>
		/// Saves the Profile to the XmlNode passed in.
		/// </summary>
		/// <param name="setupData">Xml node that holds the Profile data.</param>
		/// <returns>Returns true if the save is successful, false otherwise</returns>
		bool Save(XmlNode setupData);

		/// <summary>
		/// Determines whether the file indicated is a valid file format for this type of Profile.
		/// </summary>
		/// <param name="filename">Filename of file containing profile data.</param>
		/// <returns>Returns true if this type of Profile can open this file exactly, false otherwise.</returns>
		bool ValidateFile(string filename);

		#endregion [ Methods ]
		
		#region [ DEAD CODE ]

		//Controllers.ChannelController Channels { get; set; }

		/// <summary>
		/// Form that is used to display the profile
		/// </summary>
		//CanvasWindow Form { get; set; }

		/// <summary>
		/// Returns the number of Channels in this Profile.
		/// </summary>
		//int ChannelCount { get; }

		/// <summary>
		/// Gets and Sets the cursor for the CanvasPane control.
		/// </summary>
		//Cursor Cursor { get; set; }

		/// <summary>
		/// Indicates whether the Mask is currently defined.
		/// </summary>
		//bool HasMask { get; }

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		//bool HasRedo { get; }

		/// <summary>
		/// Indicates whether there are elements in the Redo stack.
		/// </summary>
		//bool HasUndo { get; }

		/// <summary>
		/// Clears out the masked area, moves the cells back from the Move Channel to their proper one, 
		/// and instructs the CanvasWindow to stop displaying the marquee
		/// </summary>
		//void ClearMask();

		//void Close();

		///// <summary>
		///// Creates and returns a text representative of the data in the Redo stack.
		///// </summary>
		//string Debug_RedoStack();

		///// <summary>
		///// Creates and returns a text representative of the data in the Undo stack.
		///// </summary>
		//string Debug_UndoStack();

		///// <summary>
		///// Creates and returns a text representative of the current undo snapshot.
		///// </summary>
		//string Debug_UndoSnapshot();

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		//void DefineMask(Mask newMask);

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		/// <param name="createOverlay">Flag to indicate if the overlay bitmap should be created.</param>
		//void DefineMask(Mask newMask, bool createOverlay);

		/// <summary>
		/// Sets the outline of the mask to these values
		/// </summary>
		/// <param name="canvasOutline">GraphicsPath for the Canvas mask</param>
		/// <param name="latticeOutline">GraphicsPath for the Lattice mask</param>
		//void DefineMask(GraphicsPath canvasOutline, GraphicsPath latticeOutline);

		/// <summary>
		/// Retrieves the System.Windows.PictureBox control from the CanvasWindow form.
		/// </summary>
		//PictureBox GetCanvas();

		/// <summary>
		/// Creates a System.Drawing.Graphics object from the CanvasPane object.
		/// </summary>
		//Graphics GetCanvasGraphics();

		/// <summary>
		/// Returns the GraphicsPath object used to define the mask outline.
		/// </summary>
		/// <param name="scale">Determines the scale of the outline to be returned.</param>
		//GraphicsPath GetMaskOutline(ElfCore.Util.UnitScale scale);

		/// <summary>
		/// Used by CanvasWindow's drawing method to display the generated overlay.
		/// </summary>
		//Bitmap GetMaskOverlay();

		/// <summary>
		/// Takes a peek at the topmost item on the undo (or redo) stack and reports back the text of the item.
		/// If there are no items on the stack, returns an empty string.
		/// </summary>
		//string GetUndoText(bool undo);

		/// <summary>
		/// Set up the Undo/Redo stacks
		/// </summary>
		//void InitializeUndo();

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		//void MoveMask(Point offset);

		/// <summary>
		/// Move the cells by the amount indicated by the offset parameter
		/// </summary>
		/// <param name="offset">Amount to move the cells</param>
		//void MoveMask(PointF offset);

		/// <summary>
		/// Refresh the redraw on the the CanvasPane.
		/// </summary>
		//void Refresh();

		/// <summary>
		/// Release the capture of the mouse cursor
		/// </summary>
		//void ReleaseMouse();

		/// <summary>
		/// Rewinds the last Undo performed, reapplying the changes
		/// </summary>
		//void Redo();

		/// <summary>
		/// Used the the edit background form.
		/// </summary>
		//PictureBox SubstituteCanvas { get; set; }

		/// <summary>
		/// The program has just performed an operation that can be undone. Grab a snapshot of the data
		/// and save the differences between this and the last as a Changeset
		/// </summary>
		/// <param name="action">Text of the operation complete, this will appear in the Undo menu in the Editor</param>
		//void SaveUndo(string action);

		/// <summary>
		/// Serializes the Mask into a string and returns it.
		/// </summary>
		/// <returns>Serialized mask data.</returns>
		//string SerializeMask();

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="maskData">Mask object</param>
		//void SetMask(Mask maskData);

		/// <summary>
		/// Triggers an event to fire, due to the user clicking with the Zoom tool on a point on the canvas.
		/// </summary>
		/// <param name="zoomPoint">Point on the canvas the user has clicked</param>
		/// <param name="zoomLevel">New zoom amount to use.</param>
		//void SetClickZoom(Point zoomPoint, float zoomLevel);

		/// <summary>
		/// Trap the mouse to only live inside of the canvas, so we don't get weird effects, like drawings starting outside, or ending outside the pictureBox.
		/// Call ReleaseMouse() on the MouseUp event to allow the cursor to act normal.
		/// </summary>
		//void TrapMouse();

		/// <summary>
		/// Detach the CanvasWindow object from the events and destroy it.
		/// </summary>
		//void UnclipCanvasWindow();

		/// <summary>
		/// Looks at the last set of changes and applies the old values.
		/// </summary>
		//void Undo();

		//#region [ Events ]

		/// <summary>
		/// Occurs when the cursor enters the rectangle defined by the Profile's Canvas control.
		/// </summary>
		//event EventHandler Canvas_MouseLeave;

		/// <summary>
		/// Occurs when the cursor leaves the rectangle defined by the Profile's Canvas control.
		/// </summary>
		//event EventHandler Canvas_MouseEnter;

		/// <summary>
		/// Occurs when a property on one or more Channels has changed.
		/// </summary>
		//event EventHandlers.ChannelListEventHandler ChannelPropertyChanged;

		/// <summary>
		/// Occurs when a Channel is removed from the Channel Controller.
		/// </summary>
		//event EventHandlers.ChannelEventHandler ChannelRemoved;

		/// <summary>
		/// Occurs when one or more Channels have been selected.
		/// </summary>
		//event EventHandlers.ChannelListEventHandler ChannelsSelected;

		/// <summary>
		/// Occurs when the Zoom tool is selected and the user clicks on the Canvas.
		/// </summary>
		//event EventHandlers.ZoomEventHandler ClickZoom;

		/// <summary>
		/// Fires when the Profile is being closed, given objects that have event delegates for this profile a chance to remove them.
		/// </summary>
		//event EventHandler Closing;

		/// <summary>
		/// Fires once the Load method has finished.
		/// </summary>
		//event EventHandler Loaded;

		/// <summary>
		/// Occurs when the Mask is cleared.
		/// </summary>
		//event EventHandler Mask_Cleared;

		/// <summary>
		/// Occurs when the Mask is defined.
		/// </summary>
		//event EventHandler Mask_Defined;

		/// <summary>
		/// Occurs when a new item is the top item on the Redo stack. This is a bubbled event from the UndoController.
		/// </summary>
		//event EventHandlers.UndoEventHandler Redo_Changed;

		/// <summary>
		/// Occurs when one of the scaling properties have been changed.
		/// </summary>
		//event EventHandler ScalingChanged;

		/// <summary>
		/// Occurs when the active Shuffle's order list has been altered
		/// </summary>
		//event EventHandlers.ShuffleEventHandler ShuffleChanged;

		/// <summary>
		/// Occurs when the active Shuffle is to a different shuffle.
		/// </summary>
		//event EventHandlers.ShuffleEventHandler ShuffleSwitched;

		/// <summary>
		/// Occurs when a new item is the top item on the Undo stack. This is a bubbled event from the UndoController.
		/// </summary>
		//event EventHandlers.UndoEventHandler Undo_Changed;

		/// <summary>
		/// Occurs when an Undo or Redo operation has completed.
		/// </summary>
		//event EventHandler Undo_Completed;

		//#endregion [ Events ]

		#endregion [ DEAD CODE ]
	}
}
