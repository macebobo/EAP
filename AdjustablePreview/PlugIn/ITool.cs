using System.Drawing;
using System.Windows.Forms;

namespace ElfCore.PlugIn
{
	public interface ITool
	{
		#region [ Events ]

		/// <summary>
		/// Event that fires when a Tool finishes, typically fired on MouseUp
		/// </summary>
		event System.EventHandler OperationCompleted;

		#endregion [ Events ]

		#region [ Properties ]

		/// <summary>
		/// Numeric identifier for the tool.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// Boolean flag to indicate that this tool can affect more than one Channel
		/// </summary>
		bool AffectMultipleChannels { get; set; }

		/// <summary>
		/// String representing the tool's name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// String representing what will be displayed in the Edit Menu when presenting the Undo Menu Option
		/// ("Undo [x]")
		/// </summary>
		string UndoText { get; set; }

		/// <summary>
		/// ToolTip to use with the ToolBox button
		/// </summary>
		string ToolTipText { get; set; }

		/// <summary>
		/// Boolean flag to indicate that this tool does a selection activity when being drawn on the Canvas
		/// </summary>
		bool DoesSelection { get; set; }

		/// <summary>
		/// 16x16 transparent bitmap to display in the toolbox
		/// </summary>
		Bitmap ToolBoxImage { get; set; }

		/// <summary>
		/// Keyboard shortcut used to select this tool
		/// </summary>
		KeyChord KeyboardShortcut { get; set; }

		/// <summary>
		/// Cursor to use once this tool is selected
		/// </summary>
		Cursor Cursor { get; set; }

		/// <summary>
		/// Boolean flag to indicate that this tool deals with the traditional bitmapping of the preview window (default: TRUE)
		/// </summary>
		bool BitmapMode { get; set; }

		/// <summary>
		/// Returns the ToolStrip control that holds any configurable settings for this tool. Note that any validation needs to be done within
		/// the code of the Tool itself.
		/// </summary>
		ToolStrip SettingsToolStrip { get; set; }

		/// <summary>
		/// Returns the name of the tool that is a Tool Group, in which this tool should be a child tool of
		/// </summary>
		string ToolGroupName { get; set; }

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
		/// <param name="settings"></param>
		/// <param name="workshop"></param>
		void Initialize(/*Settings settings, Workshop workshop*/);

		/// <summary>
		/// Canvas Click event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="mousePixel">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseClick(MouseButtons buttons, Point mouseCell, Point mousePixel);

		/// <summary>
		/// Canvas DoubleClick event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="mousePixel">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseDoubleClick(MouseButtons buttons, Point mouseCell, Point mousePixel);

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="mousePixel">Point on the picture box (in Pixel) where the mouse event happened</param>
		void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel);

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="mousePixel">Point on the picture box (in Pixel) where the mouse event happened</param>
		bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel);

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="mousePixel">Point on the picture box (in Pixel) where the mouse event happened</param>
		bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel);

		/// <summary>
		/// Method that is called when tool settings are to be saved.
		/// </summary>
		void SaveSettings();

		/// <summary>
		/// Method that is called when the tool is selected from the Editor's toolbox
		/// </summary>
		void Selected();

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		void ShutDown();

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		void Unselected();

		#endregion [ Methods ]
	}
}
