using System;
using System.Drawing;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Interfaces
{
	public interface ITool : IDisposable
	{
		#region [ Properties ]

		/// <summary>
		/// Indicates whether this tool can affect more than one Channel
		/// </summary>
		bool AffectMultipleChannels { get; set; }

		/// <summary>
		/// Indicates whether this tool deals with the traditional bitmapping of the preview window (default: TRUE)
		/// </summary>
		bool BitmapMode { get; set; }

		/// <summary>
		/// Cursor to use once this tool is selected
		/// </summary>
		Cursor Cursor { get; set; }

		/// <summary>
		/// Indicates whether this tool does a selection activity when being drawn on the Canvas
		/// </summary>
		bool DoesSelection { get; set; }

		/// <summary>
		/// Numeric identifier for the tool.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// First key in the multi-gesture keystroke
		/// </summary>
		Keys MultiGestureKey1 { get; set; }

		/// <summary>
		/// Second key in the multi-gesture keystroke
		/// </summary>
		Keys MultiGestureKey2 { get; set; }

		/// <summary>
		/// String representing the tool's name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Indicates whether this Tool is selected or not.
		/// </summary>
		bool IsSelected { get; set; }

		/// <summary>
		/// 16x16 transparent bitmap to display in the toolbox
		/// </summary>
		Bitmap ToolBoxImage { get; set; }

		/// <summary>
		/// 16x16 transparent bitmap to display in the toolbox when the tool is selected
		/// </summary>
		Bitmap ToolBoxImageSelected { get; set; }

		/// <summary>
		/// Returns the name of the tool that is a Tool Group, in which this tool should be a child tool of
		/// </summary>
		string ToolGroupName { get; set; }

		/// <summary>
		/// ToolTip to use with the ToolBox button
		/// </summary>
		string ToolTipText { get; set; }

		/// <summary>
		/// String representing what will be displayed in the Edit Menu when presenting the Undo Menu Option
		/// ("Undo [x]")
		/// </summary>
		string UndoText { get; set; }

		/// <summary>
		/// Returns the ToolStrip control that holds any configurable settings for this tool. Note that any validation needs to be done within
		/// the code of the Tool itself.
		/// </summary>
		ToolStrip SettingsToolStrip { get; set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Informs the tool that the caller wants it to cease its current operation
		/// </summary>
		void Cancel();

		/// <summary>
		/// Handles keystrokes for the tool. Returns true if the keystroke was handled within the tool
		/// </summary>
		/// <param name="e"></param>
		bool KeyDown(KeyEventArgs e);		

		/// <summary>
		/// Setup the tool
		/// </summary>
		void Initialize();

		/// <summary>
		/// Canvas Click event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseClick(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint);

		/// <summary>
		/// Canvas DoubleClick event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseDoubleClick(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint);

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint);

		/// <summary>
		/// Canvase MouseLeave event was fired.
		/// </summary>
		/// <returns></returns>
		void MouseLeave();

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint);

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint);

		/// <summary>
		/// Method that is called when tool settings are to be saved.
		/// </summary>
		void SaveSettings();

		/// <summary>
		/// Method that is called when the tool is selected from the Editor's toolbox
		/// </summary>
		void OnSelected();

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		void ShutDown();

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		void OnUnselected();

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Event that fires when a Tool finishes, typically fired on MouseUp
		/// </summary>
		event EventHandler OperationCompleted;

		event EventHandler Selected;

		event EventHandler Unselected;

		#endregion [ Events ]
	}
}
