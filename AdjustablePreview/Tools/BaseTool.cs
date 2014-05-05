using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	/// <summary>
	/// Contains a lot of the basic stuff shared between Tools
	/// </summary>
	public class BaseTool
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetDoubleClickTime();

		#region [ Events ]

		public event EventHandler OperationCompleted;

		#endregion [ Events ]

		#region [ Static Variables ]

		protected static ToolStripContainer TSContainer = new ToolStripContainer();

		#endregion [ Static Variables ]

		#region [ Private Variables ]

		private Cursor _cursor = null;
		private string _undoText = string.Empty;
		private string _toolTipText = string.Empty;
		private IntPtr _cursorHandle = IntPtr.Zero;

		#endregion [ Private Variables ]

		#region [ Protected Variables ]

		protected bool _cancel = false;
		//protected bool _createUndoOnMouseDown = true;
		protected ToolStripContainer _toolStrip_Form = TSContainer;
		protected Workshop _workshop = Workshop.Instance;
		protected Settings _settings = Settings.Instance;
		protected ConstrainDirection _constrainingDirection = ConstrainDirection.NotSet;

		/// <summary>
		/// Cell where the mouse button was pressed down
		/// </summary>
		protected Point _mouseDownCell = Point.Empty;

		/// <summary>
		/// Cell Pixel where the mouse button was pressed down
		/// </summary>
		protected Point _mouseDownCellPixel = Point.Empty;

		/// <summary>
		/// Cell where the mouse is currently
		/// </summary>
		protected Point _currentMouseCell = Point.Empty;

		/// <summary>
		/// Cell Pixel where the mouse is currently
		/// </summary>
		protected Point _currentMouseCellPixel = Point.Empty;

		/// <summary>
		/// Constrained version of the current mouse cell pixel position. If control button is not pressed, then is is the same value as _currentMouseCellPixel
		/// </summary>
		protected Point _constrainedCellPixel = Point.Empty;

		/// <summary>
		/// Is the mouse button currently pressed?
		/// </summary>
		protected bool _mouseDown = false;

		/// <summary>
		/// Did a mouse Click event just fire?
		/// </summary>
		protected bool _click = false;

		/// <summary>
		/// Did a mouse DoubleClick event just fire?
		/// </summary>
		protected bool _doubleClick = false;

		/// <summary>
		/// Graphics object for the CanvasPane PictureBox
		/// </summary>
		protected Graphics _canvasControlGraphics = null;

		/// <summary>
		/// Graphics object used to render the LatticeBuffer
		/// </summary>
		protected Graphics _latticeBufferGraphics = null;

		/// <summary>
		/// Bitmap representing the actual cell data
		/// </summary>
		protected Bitmap _latticeBuffer = null;		 

		/// <summary>
		/// Bitmap of the CanvasPane PictureBox before we do anything to it
		/// </summary>
		protected Bitmap _capturedCanvas = null;	

		/// <summary>
		/// Path in the settings file to find data for this tool
		/// </summary>
		protected string _savePath = string.Empty;

		///// <summary>
		///// The CanvasPane PictureBox on the form CanvasWindow
		///// </summary>
		//protected PictureBox _canvasControl = null;	

		#endregion [ Protected Variables ]

		#region [ Properties ]

		/// <summary>
		/// True if this tool typically affects more than just the Active Channel
		/// </summary>
		public virtual bool AffectMultipleChannels { get; set; }

		/// <summary>
		/// True if the tool generates output in the standard, bitmapped manner. Support for vector based drawing is still in the works.
		/// </summary>
		public virtual bool BitmapMode { get; set; }

		/// <summary>
		/// Cursor to use on the Canvas window when the mouse is within its bounds. A safe cursor to use might be: Cursors.Cross
		/// </summary>
		public virtual Cursor Cursor
		{
			get { return _cursor; }
			set { _cursor = value; }
		}

		/// <summary>
		/// True if this tool will do a marquee selection on the Canvas window. This flag is needed to display information in the Editor.
		/// </summary>
		public virtual bool DoesSelection { get; set; }

		/// <summary>
		/// Numeric identifier for the tool.
		/// </summary>
		public virtual int ID { get; set; }

		/// <summary>
		/// Keyboard shortcut to use to select this tool from the ToolBox
		/// </summary>
		public virtual KeyChord KeyboardShortcut { get; set; }

		/// <summary>
		/// Name of the Tool
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// ToolStrip to display under the menu when this tool is selected. If no settings are available, this can safely return null.
		/// </summary>
		public virtual ToolStrip SettingsToolStrip
		{
			get 
			{ 
				ToolStrip Strip = _toolStrip_Form.GetToolStrip(this.ID);
				if (Strip == null)
					Strip = GetGenericToolStrip();
				return Strip;
			}
			set {  }
		}

		/// <summary>
		/// Image to display in the ToolBox. 16x16 transparent PNG are best
		/// </summary>
		public virtual Bitmap ToolBoxImage { get; set; }

		/// <summary>
		/// Returns the name of the tool that is a Tool Group, in which this tool should be a child tool of
		/// </summary>
		public virtual string ToolGroupName { get; set; }

		/// <summary>
		/// ToolTip to display when the mouse hovers over the button in the ToolBox
		/// </summary>
		public virtual string ToolTipText
		{
			get 
			{
				if ((_toolTipText ?? string.Empty).Length > 0)
					return _toolTipText;
				else
					return this.Name + " Tool"; 
			}
			set { _toolTipText = value; }
		}

		/// <summary>
		/// String to use when creating the Undo menu item
		/// </summary>
		public virtual string UndoText
		{
			get 
			{
				if ((_undoText ?? string.Empty).Length > 0)
					return _toolTipText;
				else
					return this.Name; 
			}
			set { _undoText = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public BaseTool()
		{
			this.ID = -1;
			this.ToolGroupName = string.Empty;
			this.KeyboardShortcut = new KeyChord();
			this.Name = string.Empty;
			this.ToolBoxImage = ElfRes.not;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Concatenates the path with a name, delimited by the pre-defined path delimiter character.
		/// </summary>
		/// <param name="path">Pre-built path</param>
		/// <param name="newNode">Name to be appended</param>
		protected string AppendPath(string path, string newNode)
		{
			return _settings.AppendPath(path, newNode);
		}

		/// <summary>
		/// Informs the tool that the caller wants it to cease its current operation
		/// </summary>
		public virtual void Cancel()
		{
			_cancel = true;
		}

		/// <summary>
		/// Pass through to the CaptureCanvas method on the Workshop object
		/// </summary>
		/// <param name="_canvas"></param>
		protected virtual void CaptureCanvas()
		{
			_capturedCanvas = _workshop.CaptureCanvas();
		}

		/// <summary>
		/// Clears out the clipping that has been set in the Canvas and Paint graphics objects.
		/// </summary>
		public void ClearMaskClip()
		{
			try
			{
				if (_latticeBufferGraphics != null)
					_latticeBufferGraphics.ResetClip();
				if (_canvasControlGraphics != null)
					_canvasControlGraphics.ResetClip();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Fire the OperationCompleted event
		/// </summary>
		protected void EndOperation()
		{
			if (_cancel)
				return;
			if (OperationCompleted != null)
				OperationCompleted(this, new EventArgs());
			_workshop.UndoController.SaveUndo(this.UndoText);
		}

		/// <summary>
		/// Returns TRUE if the user is current pressing the Control key
		/// </summary>
		protected bool ControlKeyPressed()
		{
			return (Control.ModifierKeys == Keys.Control);
		}

		/// <summary>
		/// Creates a custom cursor for this tool
		/// </summary>
		/// <param name="baseImage">Basic image to create the cursor</param>
		/// <param name="hotspot">Position of the cursor's hotspot</param>
		/// <returns>A cursor created from the composite of the images.</returns>
		protected Cursor CreateCursor(Bitmap baseImage, Point hotspot)
		{
			return this.CreateCursor(baseImage, null, hotspot);
		}

		/// <summary>
		/// Creates a custom cursor for this tool, based on a base image, with another image in the upper right corner
		/// </summary>
		/// <param name="baseImage">Basic image to create the cursor</param>
		/// <param name="modifierImage">If present, an additional image place in the upper right hand corner</param>
		/// <param name="hotspot">Position of the cursor's hotspot</param>
		/// <returns>A cursor created from the composite of the images.</returns>
		protected Cursor CreateCursor(Bitmap baseImage, Bitmap modifierImage, Point hotspot)
		{
			using (Bitmap Base = new Bitmap(baseImage))
			{
				using (Graphics g = Graphics.FromImage(Base))
				{
					if (modifierImage != null)
						g.DrawImage(modifierImage, Base.Width - modifierImage.Width, 0, modifierImage.Width, modifierImage.Height);
				}
				Cursor NewCursor = CustomCursors.CreateCursor(Base, hotspot.X, hotspot.Y);
				_cursorHandle = NewCursor.Handle;
				return NewCursor;
			}
		}

		/// <summary>
		/// Render the captured canvas bitmap onto the CanvasPane PictureBox
		/// </summary>
		protected void DisplayCapturedCanvas()
		{
			if ((_canvasControlGraphics != null) && (_capturedCanvas != null))
				_canvasControlGraphics.DrawImageUnscaled(_capturedCanvas, new Point(0, 0));
		}

		///// <summary>
		///// Draws the Channel within the clip rectangle
		///// NOTE: This function appears in CanvasWindow, BaseTool and Channel
		///// </summary>
		///// <param name="g">Graphics object used to draw</param>
		///// <param name="clipRect">Area to draw on</param>
		///// <param name="ChannelBrush">Brush to use for the Channel</param>
		///// <param name="Channel">Channel data</param>
		///// <param name="offset">Offset amount to draw</param>
		//protected void DrawChannel(Graphics g, Rectangle clipRect, Brush ChannelBrush, Channel Channel, Point offset)
		//{
		//    int X, Y;
		//    foreach (Point Cell in Channel.Lattice)
		//    {
		//        if (offset.IsEmpty)
		//        {
		//            X = Cell.X * UISettings.ʃCellScale + Channel.Origin.X;
		//            Y = Cell.Y * UISettings.ʃCellScale + Channel.Origin.Y;
		//        }
		//        else
		//        {
		//            X = Cell.X * UISettings.ʃCellScale + offset.X + Channel.Origin.X;
		//            Y = Cell.Y * UISettings.ʃCellScale + offset.Y + Channel.Origin.Y;
		//        }

		//        if (clipRect.Contains(X, Y))
		//            g.FillRectangle(ChannelBrush, X, Y, UISettings.ʃCellZoom, UISettings.ʃCellZoom);
		//    }
		//}

		/// <summary>
		/// Sets the DashStyle dropdown menu from the settings bar to the value. If it cannot find that value, returns the first item
		/// </summary>
		protected ToolStripMenuItem FindDropMenuItemFromValue(ToolStripDropDownButton dropDownButton, int value)
		{
			foreach (ToolStripMenuItem Item in dropDownButton.DropDownItems)
			{
				if (Int32.Parse(Item.Tag.ToString()) == value)
					return Item;
			}
			return (ToolStripMenuItem)dropDownButton.DropDownItems[0];
		}

		/// <summary>
		/// Gets the generic toolstrip out of the from and assign the label control to be the tools name and image.
		/// </summary>
		private ToolStrip GetGenericToolStrip()
		{
			ToolStrip Strip = _toolStrip_Form.GenericToolStrip;
			ToolStripLabel ToolNameLabel = (ToolStripLabel)GetItem<ToolStripLabel>(Strip, 1);
			ToolNameLabel.Text = this.Name;
			ToolNameLabel.Image = this.ToolBoxImage;
			ToolNameLabel = null;
			return Strip;
		}

		/// <summary>
		/// Returns the toolstrip for this tool
		/// </summary>
		protected ToolStrip GetToolStrip()
		{
			return _toolStrip_Form.GetToolStrip(this.ID);
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and position
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="toolStrip">ToolStrip object to find the child control on.</param>
		/// <param name="index">Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object of that type that is found, regardless of the number of other controls that preceeded it.</param>
		protected ToolStripItem GetItem<T>(ToolStrip toolStrip, int index)
		{
			return _toolStrip_Form.GetItem<T>(toolStrip, index);
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and position
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="index">Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object of that type that is found, regardless of the number of other controls that preceeded it.</param>
		protected ToolStripItem GetItem<T>(int index)
		{
			return _toolStrip_Form.GetItem<T>(this.ID, index);
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and name
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="toolStrip">ToolStrip object to find the child control on.</param>
		/// <param name="name">Name of the control</param>
		protected ToolStripItem GetItem<T>(ToolStrip toolStrip, string name)
		{
			return _toolStrip_Form.GetItem<T>(toolStrip, name);
		}

		/// <summary>
		/// Returns an item from a toolstrip of a given type and name
		/// </summary>
		/// <typeparam name="T">Type of the object to return</typeparam>
		/// <param name="name">Name of the control</param>
		protected ToolStripItem GetItem<T>(string name)
		{
			return _toolStrip_Form.GetItem<T>(this.ID, name);
		}

		/// <summary>
		/// Setup the tool
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="workshop"></param>
		public virtual void Initialize(/*Settings settings, Workshop workshop*/)
		{
			//_workshop = workshop;
			//_settings = settings;

			_savePath = AppendPath(Constants.TOOLSETTINGS, this.SafeName());
		}

		/// <summary>
		/// Handles keystrokes for the tool. Returns true if the keystroke was handled within the tool
		/// </summary>
		/// <param name="e"></param>
		public virtual bool KeyDown(KeyEventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Canvas Click event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public virtual void MouseClick(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{ }

		/// <summary>
		/// Canvas DoubleClick event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public virtual void MouseDoubleClick(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{ }

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public virtual void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;
			_mouseDownCell = mouseCell;
			_mouseDownCellPixel = Workshop.PixelPoint(mouseCell);

			_currentMouseCell = mouseCell;
			_currentMouseCellPixel = Workshop.PixelPoint(mouseCell);

			_canvasControlGraphics = Workshop.Canvas.CreateGraphics();

			// Trap the mouse into the Canvas while we are working
			//TrapMouse();

			_latticeBuffer = _workshop.Channels.Active.LatticeBuffer;
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);

			CaptureCanvas();

			//if (_createUndoOnMouseDown)
			//    // Create Undo data to be able to reverse out our changes
			//    _workshop.CreateUndo_Channel(this.UndoText);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public virtual bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;

			//// Limit the mouse position to be within the bounds of the canvas
			//if (mouseCell.X >= _workshop.UI.LatticeSize.Width)
			//    mouseCell.X = _workshop.UI.LatticeSize.Width-1;
			//else if (mouseCell.X < 0)
			//    mouseCell.X = 0;

			//if (mouseCell.Y >= _workshop.UI.LatticeSize.Height)
			//    mouseCell.Y = _workshop.UI.LatticeSize.Height-1;
			//else if (mouseCell.Y < 0)
			//    mouseCell.Y = 0;

			_currentMouseCell = mouseCell;
			_currentMouseCellPixel = Workshop.PixelPoint(mouseCell);
			_constrainedCellPixel = _workshop.ConstrainLine(_currentMouseCellPixel, _mouseDownCellPixel);

			// Draw the captured bitmap onto the CanvasPane PictureBox
			DisplayCapturedCanvas();

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public virtual bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;

			_mouseDown = false;
			_currentMouseCell = mouseCell;
			_currentMouseCellPixel = Workshop.PixelPoint(mouseCell);

			// Release the capture of the mouse cursor
			//ReleaseMouse();

			//if (!_createUndoOnMouseDown)
			//    // Create Undo data to be able to reverse out our changes
			//    _workshop.CreateUndo_Channel(this.UndoText);

			// Fire the event to indicate that this tool has finished working.
			EndOperation();

			return true;
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected virtual void PostDrawCleanUp()
		{
			if (_canvasControlGraphics != null)
			{
				_canvasControlGraphics.Dispose();
				_canvasControlGraphics = null;
			}
			if (_latticeBufferGraphics != null)
			{
				_latticeBufferGraphics.Dispose();
				_latticeBufferGraphics = null;
			}
			if (_capturedCanvas != null)
			{
				_capturedCanvas.Dispose();
				_capturedCanvas = null;
			}
			if (_latticeBuffer != null)
			{
				_latticeBuffer.Dispose();
				_latticeBuffer = null;
			}
			_cancel = false;
			_mouseDownCell = Point.Empty;
			_mouseDownCellPixel = Point.Empty;
			_currentMouseCell = Point.Empty;
			_currentMouseCellPixel = Point.Empty;
			_constrainedCellPixel = Point.Empty;

			// Fire the event to indicate that this tool has finished working.
			EndOperation();
		}

		/// <summary>
		/// Release the capture of the mouse cursor
		/// </summary>
		protected void ReleaseMouse()
		{
			Workshop.Canvas.Capture = false;
			Cursor.Clip = Rectangle.Empty;
		}

		/// <summary>
		/// Creates the Pen used to render the shape onto the Paint Pane
		/// </summary>
		protected virtual Pen RenderPen()
		{
			Pen RenderPen = new Pen(Color.White, 1);
			return RenderPen;
		}

		/// <summary>
		/// If a mask is defined, then sets the clip on the Canvas
		/// </summary>
		protected virtual void SetMaskClip()
		{
			if (!_workshop.Mask.HasMask)
				return;

			using (Region Region = new System.Drawing.Region(_workshop.Mask.CanvasMask.Outline))
				_canvasControlGraphics.SetClip(Region, CombineMode.Replace);
			
			using (Region Region = new System.Drawing.Region(_workshop.Mask.LatticeMask.Outline))
				_latticeBufferGraphics.SetClip(Region, CombineMode.Replace);			
		}

		/// <summary>
		/// Returns the tool's name, modified to be used for variable names or Xml node names
		/// </summary>
		/// <returns></returns>
		public string SafeName()
		{
			return Regex.Replace(this.Name, @"[^\w]", "", RegexOptions.None);
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		/// <param name="settings"></param>
		public virtual void SaveSettings()
		{
			// Nothing to Save
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// </summary>
		public virtual void Selected()
		{
			// Set the cursor for the Canvas. This is important because there could have been UI changes since this Select method
			// was called initially on Tool_Click, and some tool's cursors are sensitive to UI settings (ie Paint, Erase, etc)
			Workshop.Canvas.Cursor = this.Cursor;
		}

		/// <summary>
		/// Sets the tags for all the DashStyle menu items in the DashStyle drop down button on the settings toolbar.
		/// </summary>
		/// <param name="dashStyleDD">ToolStripDropDownButton control that has the menu items</param>
		protected void SetDashStyleDropDownButton(ToolStripDropDownButton dashStyleDD)
		{
			dashStyleDD.DropDownItems[0].Tag = (int)DashStyle.Solid;
			dashStyleDD.DropDownItems[1].Tag = (int)DashStyle.Dot;
			dashStyleDD.DropDownItems[2].Tag = (int)DashStyle.Dash;
			dashStyleDD.DropDownItems[3].Tag = (int)DashStyle.DashDot;
			dashStyleDD.DropDownItems[4].Tag = (int)DashStyle.DashDotDot;
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public virtual void ShutDown()
		{
			if (_cursor != null)
			{
				CustomCursors.DestroyCreatedCursor(_cursorHandle);
				_cursor.Dispose();
				_cursor = null;
			}
			_toolStrip_Form = null;
			if (_canvasControlGraphics != null)
			{
				_canvasControlGraphics.Dispose();
				_canvasControlGraphics = null;
			}
			if (_latticeBufferGraphics != null)
			{
				_latticeBufferGraphics.Dispose();
				_latticeBufferGraphics = null;
			}
			if (_latticeBuffer != null)
			{
				_latticeBuffer.Dispose();
				_latticeBuffer = null;
			}
			if (_capturedCanvas != null)
			{
				_capturedCanvas.Dispose();
				_capturedCanvas = null;
			}
		}

		/// <summary>
		/// Trap the mouse to only live inside of the canvas, so we don't get weird effects, like drawings starting outside, or ending outside the pictureBox.
		/// Call ReleaseMouse() on the MouseUp event to allow the cursor to act normal.
		/// </summary>
		protected void TrapMouse()
		{
			// Trap the mouse into the Canvas while we are working
			Rectangle rc = Workshop.Canvas.RectangleToScreen(new Rectangle(Point.Empty, Workshop.Canvas.ClientSize));
			Cursor.Clip = rc;
			Workshop.Canvas.Capture = true;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		public virtual void Unselected()
		{ }

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, return that value.
		/// If not, reset the text in the text box with the original value of our variable and return that original value
		/// </summary>
		protected virtual int ValidateInteger(ToolStripTextBox textBox, int originalValue)
		{
			int Value = 0;
			if (Int32.TryParse(textBox.Text, out Value))
				return Value;
			else
			{
				textBox.Text = originalValue.ToString();
				return originalValue;
			}
		}

		#region [ LoadValue ]

		/// <summary>
		/// Returns the string value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		/// <returns>Setting value as a string</returns>
		protected string LoadValue(string pathName, string defaultValue, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			return _settings.GetValue(Path, defaultValue);
		}

		/// <summary>
		/// Returns the string value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <returns>Setting value as a string</returns>
		protected string LoadValue(string pathName, string defaultValue)
		{
			return _settings.GetValue(AppendPath(_savePath, pathName), defaultValue);
		}

		/// <summary>
		/// Returns the integer value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		/// <returns>Setting value as an integer</returns>
		protected int LoadValue(string pathName, int defaultValue, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			return _settings.GetValue(Path, defaultValue);
		}

		/// <summary>
		/// Returns the integer value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <returns>Setting value as an integer</returns>
		protected int LoadValue(string pathName, int defaultValue)
		{
			return _settings.GetValue(AppendPath(_savePath, pathName), defaultValue);
		}

		/// <summary>
		/// Returns the floating point value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		/// <returns>Setting value as a float</returns>
		protected float LoadValue(string pathName, float defaultValue, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			return _settings.GetValue(Path, defaultValue);
		}

		/// <summary>
		/// Returns the floating point value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <returns>Setting value as a float</returns>
		protected float LoadValue(string pathName, float defaultValue)
		{
			return _settings.GetValue(AppendPath(_savePath, pathName), defaultValue);
		}

		/// <summary>
		/// Returns the boolean value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		/// <returns>Setting value as a boolean</returns>
		protected bool LoadValue(string pathName, bool defaultValue, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			return _settings.GetValue(Path, defaultValue);
		}

		/// <summary>
		/// Returns the boolean value stored in Settings
		/// </summary>
		/// <param name="pathName">Path to find the setting value</param>
		/// <param name="defaultValue">Value to return if the setting value was not present</param>
		/// <returns>Setting value as a boolean</returns>
		protected bool LoadValue(string pathName, bool defaultValue)
		{
			return _settings.GetValue(AppendPath(_savePath, pathName), defaultValue);
		}

		#endregion [ LoadValue ]

		#region [ SaveValue ]

		/// <summary>
		/// Saves the value to the settings object as a string.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		protected void SaveValue(string pathName, string value, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			_settings.SetValue(Path, value);
		}

		/// <summary>
		/// Saves the value to the settings object as a string.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		protected void SaveValue(string pathName, string value)
		{
			SaveValue(pathName, value, true);
		}

		/// <summary>
		/// Saves the value to the settings object as an integer.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		protected void SaveValue(string pathName, int value, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			_settings.SetValue(Path, value);
		}

		/// <summary>
		/// Saves the value to the settings object as an integer.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		protected void SaveValue(string pathName, int value)
		{
			SaveValue(pathName, value, true);
		}

		/// <summary>
		/// Saves the value to the settings object as a float.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		protected void SaveValue(string pathName, float value, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			_settings.SetValue(Path, value);
		}

		/// <summary>
		/// Saves the value to the settings object as an float.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		protected void SaveValue(string pathName, float value)
		{
			SaveValue(pathName, value, true);
		}

		/// <summary>
		/// Saves the value to the settings object as a boolean.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		/// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
		protected void SaveValue(string pathName, bool value, bool appendPath)
		{
			string Path = pathName;
			if (appendPath)
				Path = AppendPath(_savePath, pathName);
			_settings.SetValue(Path, value);
		}

		/// <summary>
		/// Saves the value to the settings object as a boolean.
		/// </summary>
		/// <param name="pathName">Path to the place to store the setting value</param>
		/// <param name="value">Value to store</param>
		protected void SaveValue(string pathName, bool value)
		{
			SaveValue(pathName, value, true);
		}

		#endregion [ SaveValue ]

		#endregion [ Methods ]

	}
}
