using ElfCore.Channels;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	[ElfEditTool("Move Channel")]
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
		private Point _moveEndCanvasPoint = Point.Empty;
		private BaseChannelList _movingChannels = null;
		private CanvasPoint _moveStartCanvasPoint = new Point();
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

				else if (Profile.Channels.Selected.Count > 0)
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
			this.ID = (int)ToolID.MoveChannel;
			this.Name = "Move Channel";
			this.ToolBoxImage = ElfRes.move_channel;
			this.Cursor = base.CreateCursor(ElfRes.pan_tool, new Point(7, 7));
			this.AffectMultipleChannels = true;
			_grabbingCursor = base.CreateCursor(ElfRes.cursor_grabbed, new Point(7, 7));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Attaches or detaches events to objects, such as Click events to buttons.
		/// </summary>
		/// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
		protected override void AttachEvents(bool attach)
		{
			// If we've already either attached or detached, exit out.
			if (attach && _eventsAttached)
				return;

			if (attach)
			{
				MoveAll.Click += new EventHandler(MoveAll_Click);
				ExecuteMove.Click += new EventHandler(ExecuteMove_Click);
				OffsetX.Leave += new EventHandler(OffsetX_Leave);
				OffsetY.Leave += new EventHandler(OffsetY_Leave);
			}
			else
			{
				MoveAll.Click -= MoveAll_Click;
				ExecuteMove.Click -= ExecuteMove_Click;
				OffsetX.Leave -= OffsetX_Leave;
				OffsetY.Leave -= OffsetY_Leave;
			}
			base.AttachEvents(attach);
		}

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// Load the Settings values
			_moveOffset = new Point(LoadValue(OFFSET_X, 0), LoadValue(OFFSET_Y, 0));
			_moveAllChannels = LoadValue(ALL_ChannelS, false);

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			MoveAll = (ToolStripButton)GetItem<ToolStripButton>(1);
			ExecuteMove = (ToolStripButton)GetItem<ToolStripButton>(2);
			OffsetX = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			OffsetY = (ToolStripTextBox)GetItem<ToolStripTextBox>(2);

			// Set the initial value for the contol from what we had retrieve from Settings
			OffsetX.Text = _moveOffset.X.ToString();
			OffsetY.Text = _moveOffset.Y.ToString();
			MoveAll.Checked = _moveAllChannels;
		}

		private RectangleF GetChannelGroupBound(BaseChannelList ChannelList)
		{
			Rectangle ChanRect;
			float minX, minY, maxX, maxY;
			minX = minY = Int32.MaxValue;
			maxX = maxY = Int32.MinValue;

			foreach (BaseChannel Channel in ChannelList)
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

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			_isMouseDown = true;

			//if (MoveAll.Checked)
			//    _movingChannels = Profile.Channels.GetAllChannels();
			//else
			//    _movingChannels = Profile.Channels.Selected;

			Profile.Cursor = _grabbingCursor;
			_moveStartCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);

			_movingChannels = null;

			if (MoveAll.Checked)
				_movingChannels = Profile.Channels.GetAllChannels();
			else
				_movingChannels = Profile.Channels.Selected;

		}

		/// <summary>
		/// Handles keystrokes for the tool. Returns true if the keystroke was handled within the tool
		/// </summary>
		/// <param name="e"></param>
		public override bool KeyDown(KeyEventArgs e)
		{
			_moveStartCanvasPoint = new CanvasPoint(0, 0);
			_moveEndCanvasPoint = Point.Empty;

			int Amount = Scaling.CellScale;

			if (Control.ModifierKeys == Keys.Shift)
				Amount *= 5;

			switch (e.KeyCode)
			{
				case Keys.Up:
					_moveEndCanvasPoint = new CanvasPoint(0, -Amount);
					break;

				case Keys.Down:
					_moveEndCanvasPoint = new CanvasPoint(0, Amount);
					break;

				case Keys.Left:
					_moveEndCanvasPoint = new CanvasPoint(-Amount, 0);
					break;

				case Keys.Right:
					_moveEndCanvasPoint = new CanvasPoint(Amount, 0);
					break;
			}

			if (_moveEndCanvasPoint.IsEmpty)
				return false;

			if (MoveAll.Checked)
				_movingChannels = Profile.Channels.GetAllChannels();
			else
				_movingChannels = Profile.Channels.Selected;

			//_workshop.CreateUndo_Channel(this.UndoText, _movingChannels);

			MoveTheseChannels();

			return true;
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (Profile == null)
				return false;
			
			if (!_isMouseDown)
				return false;

			_moveEndCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
			MoveTheseChannels();
			_moveStartCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			_isMouseDown = false;

			_moveEndCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
			MoveTheseChannels();
			Profile.Cursor = this.Cursor;

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
			Point Offset = _workshop.CalcLatticePoint(new Point(_moveEndCanvasPoint.X - _moveStartCanvasPoint.X, _moveEndCanvasPoint.Y - _moveStartCanvasPoint.Y));

			foreach (BaseChannel Channel in _movingChannels)
			{
				//Channel.Origin.Offset(Offset);
				Channel.MoveData(Offset);
			}
			Profile.Refresh();
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

		#region [ ToolStrip Event Delegates ]

		/// <summary>
		/// Execute the move using the offsets in the tool strip
		/// </summary>
		private void ExecuteMove_Click(object sender, EventArgs e)
		{
			if (MoveAll.Checked)
				_movingChannels = Profile.Channels.GetAllChannels();
			else
				_movingChannels = Profile.Channels.Selected;
			//_workshop.CreateUndo_Channel(this.UndoText, _movingChannels);

			_moveStartCanvasPoint = new Point(0, 0);
			_moveEndCanvasPoint = new Point(_moveOffset.X * Scaling.CellScale, _moveOffset.Y * Scaling.CellScale);
			MoveTheseChannels();
			Profile.SaveUndo(this.UndoText);
			Profile.Refresh();
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

		#endregion [ ToolStrip Event Delegates ]
	
	}
}
