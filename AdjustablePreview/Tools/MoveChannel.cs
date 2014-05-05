using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Move Channel")]
	public class MoveChannelTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		private bool _moveAllChannels = false;
		private Point _moveOffset = Point.Empty;

		// Controls from ToolStrip
		private ToolStripButton MoveAll = null;
		private ToolStripButton ExecuteMove = null;
		private ToolStripTextBox OffsetX = null;
		private ToolStripTextBox OffsetY = null;

		// Used for moving
		private Point _moveEnd = Point.Empty;
		private List<Channel> _movingChannels = null;
		private Point _moveStartCellPixel = new Point();
		private Cursor _grabbingCursor = null;
		private IntPtr _grabCursorHandle = IntPtr.Zero;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string OFFSET_X = "Offset_X";
		private const string OFFSET_Y = "Offset_Y";
		private const string ALL_ChannelS = "AllChannels";

		#endregion [ Constants ]

		#region [ Properties ]

		public override string UndoText
		{
			get
			{
				if (MoveAll.Checked)
					return "Move All Channels";

				else if (_workshop.Channels.Selected.Count > 0)
					return "Move Channels";

				else
					return "Move Channel";
			}
			set
			{
				base.UndoText = value;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public MoveChannelTool()
		{
			this.ID = (int)Tool.MoveChannel;
			this.Name = "Move Channel";
			this.ToolBoxImage = ElfRes.move_channel;
			this.Cursor = base.CreateCursor(ElfRes.pan_tool, new Point(7, 7));
			this.AffectMultipleChannels = true;
			_grabbingCursor = base.CreateCursor(ElfRes.cursor_grabbed, new Point(7, 7));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		/// <param name="settings">Settings object, handles getting and saving settings data</param>
		/// <param name="workshop">Workshop object, contains lots of useful methods and ways to hold data.</param>
		public override void Initialize()
		{
			base.Initialize();

			// Load the Settings values
			_moveOffset = new Point(LoadValue(OFFSET_X, 0), LoadValue(OFFSET_Y, 0));
			_moveAllChannels = LoadValue(ALL_ChannelS, false);

			// Get a pointer to the controls on the toolstrip that belongs to us.
			MoveAll = (ToolStripButton)GetItem<ToolStripButton>(1);
			ExecuteMove = (ToolStripButton)GetItem<ToolStripButton>(2);
			OffsetX = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			OffsetY = (ToolStripTextBox)GetItem<ToolStripTextBox>(2);

			// Attach events to these controls
			MoveAll.Click += new System.EventHandler(this.MoveAll_Click);
			ExecuteMove.Click += new System.EventHandler(this.ExecuteMove_Click);
			OffsetX.Leave += new System.EventHandler(this.OffsetX_Leave);
			OffsetY.Leave += new System.EventHandler(this.OffsetY_Leave);			

			// Set the initial value for the contol from what we had retrieve from Settings
			OffsetX.Text = _moveOffset.X.ToString();
			OffsetY.Text = _moveOffset.Y.ToString();
			MoveAll.Checked = _moveAllChannels;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;

			if (MoveAll.Checked)
				_movingChannels = _workshop.Channels.GetAllChannels();
			else
				_movingChannels = _workshop.Channels.Selected;

			Workshop.Canvas.Cursor = _grabbingCursor;
			_moveStartCellPixel = Workshop.PixelPoint(mouseCell);

			_movingChannels = null;

			if (MoveAll.Checked)
				_movingChannels = _workshop.Channels.GetAllChannels();
			else
				_movingChannels = _workshop.Channels.Selected;

		}

		/// <summary>
		/// Handles keystrokes for the tool. Returns true if the keystroke was handled within the tool
		/// </summary>
		/// <param name="e"></param>
		public override bool KeyDown(KeyEventArgs e)
		{
			_moveStartCellPixel = new Point(0, 0);
			_moveEnd = Point.Empty;

			int Amount = UISettings.ʃCellScale;

			if (Control.ModifierKeys == Keys.Shift)
				Amount *= 5;

			switch (e.KeyCode)
			{
				case Keys.Up:
					_moveEnd = new Point(0, -Amount);
					break;

				case Keys.Down:
					_moveEnd = new Point(0, Amount);
					break;

				case Keys.Left:
					_moveEnd = new Point(-Amount, 0);
					break;

				case Keys.Right:
					_moveEnd = new Point(Amount, 0);
					break;
			}

			if (_moveEnd.IsEmpty)
				return false;

			if (MoveAll.Checked)
				_movingChannels = _workshop.Channels.GetAllChannels();
			else
				_movingChannels = _workshop.Channels.Selected;

			//_workshop.CreateUndo_Channel(this.UndoText, _movingChannels);

			MoveTheseChannels();

			return true;
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;

			_moveEnd = Workshop.PixelPoint(mouseCell);
			MoveTheseChannels();
			_moveStartCellPixel = Workshop.PixelPoint(mouseCell);
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = false;

			_moveEnd = Workshop.PixelPoint(mouseCell);
			MoveTheseChannels();
			Workshop.Canvas.Cursor = this.Cursor;

			// Fire the event to indicate that this tool has finished working.
			EndOperation();

			return true;
		}

		///// <summary>
		///// Moves the specific Channel
		///// </summary>
		///// <param name="Channel">Channel to move</param>
		//private void MoveTheChannel(Channel Channel)
		//{
		//    Point MovingOffset = Workshop.CellPoint(new Point(_moveEnd.X - _moveStartCellPixel.X, _moveEnd.Y - _moveStartCellPixel.Y));
		//    Channel.MoveCells(MovingOffset);
		//    //_canvas.Refresh();
		//}

		/// <summary>
		/// Move all the selected Channels
		/// </summary>
		private void MoveTheseChannels()
		{
			Point Offset = Workshop.CellPoint(new Point(_moveEnd.X - _moveStartCellPixel.X, _moveEnd.Y - _moveStartCellPixel.Y));

			foreach (Channel Channel in _movingChannels)
			{
				//Channel.Origin.Offset(Offset);
				Channel.MoveCells(Offset);
			}
			Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			SaveValue(OFFSET_X, _moveOffset.X);
			SaveValue(OFFSET_Y, _moveOffset.Y);
			SaveValue(ALL_ChannelS, _moveAllChannels);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			MoveAll = null;
			ExecuteMove = null;
			OffsetX = null;
			OffsetY = null;
			_movingChannels = null;
			if (_grabbingCursor != null)
			{
				CustomCursors.DestroyCreatedCursor(_grabCursorHandle);
				_grabbingCursor.Dispose();
				_grabbingCursor = null;
			}
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		/// <summary>
		/// Execute the move using the offsets in the tool strip
		/// </summary>
		private void ExecuteMove_Click(object sender, EventArgs e)
		{
			if (MoveAll.Checked)
				_movingChannels = _workshop.Channels.GetAllChannels();
			else
				_movingChannels = _workshop.Channels.Selected;
			//_workshop.CreateUndo_Channel(this.UndoText, _movingChannels);

			_moveStartCellPixel = new Point(0, 0);
			_moveEnd = new Point(_moveOffset.X * UISettings.ʃCellScale, _moveOffset.Y * UISettings.ʃCellScale);
			MoveTheseChannels();
			_workshop.UndoController.SaveUndo(this.UndoText);
			Workshop.Canvas.Refresh();
		}

		private RectangleF GetChannelGroupBound(List<Channel> ChannelList)
		{
			Rectangle ChanRect;
			float minX, minY, maxX, maxY;
			minX = minY = Int32.MaxValue;
			maxX = maxY = Int32.MinValue;

			foreach (Channel Channel in ChannelList)
			{
				ChanRect = Channel.GetBounds();
				if (ChanRect.X < minX)
					minX = ChanRect.X;
				if (ChanRect.Y < minY)
					minY = ChanRect.Y;
				if (ChanRect.X + ChanRect.Width > maxX)
					maxX = ChanRect.X + ChanRect.Width;
				if (ChanRect.Y + ChanRect.Height > maxY)
					maxY = ChanRect.Y + ChanRect.Height;
			}
			return new RectangleF(minX, minY, maxX - minX, maxY - minY);
		}

		private void MoveAll_Click(object sender, EventArgs e)
		{
			_moveAllChannels = MoveAll.Checked;
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void OffsetX_Leave(object sender, EventArgs e)
		{
			if (OffsetX.TextLength == 0)
				OffsetX.Text = "0";

			int X = ValidateInteger(OffsetX, _moveOffset.X);
			_moveOffset = new Point(X, _moveOffset.Y);
		}

		/// <summary>
		/// Validate that the text entered in the textbox is a proper number. If so, set the value into our variable.
		/// If not, reset the text in the text box with the original value of our variable
		/// </summary>
		private void OffsetY_Leave(object sender, EventArgs e)
		{
			if (OffsetY.TextLength == 0)
				OffsetY.Text = "0";

			int Y = ValidateInteger(OffsetY, _moveOffset.Y);
			_moveOffset = new Point(_moveOffset.X, Y);
		}

		#endregion [ ToolStrip Events ]
	
	}
}
