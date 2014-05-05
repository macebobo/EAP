using System;
using System.Collections.Generic;
using System.Text;
using ElfRes = ElfCore.Properties.Resources;
using ElfCore.PlugIn;
using System.Windows.Forms;
using System.Drawing;

namespace ElfCore.Tools
{
	[AdjPrevTool("Crop")]
	public class CropTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		private Rectangle _cropArea = Rectangle.Empty;

		// Controls from ToolStrip
		private ToolStripButton ClearCrop = null;
		private ToolStripButton ExecuteCrop = null;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public CropTool()
		{
			this.ID = (int)Tool.Crop;
			this.Name = "Crop";
			this.DoesSelection = true;
			this.ToolBoxImage = ElfRes.crop;
			this.Cursor = base.CreateCursor(ElfRes.crop, new Point(8, 8));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Clears the defined crop area.
		/// </summary>
		public override void Cancel()
		{
			base.Cancel();
			_cropArea = Rectangle.Empty;
		}

		/// <summary>
		/// Moves all the cells relative to their new upper left origin postion and then change the lattice size.
		/// By setting the Lattice size in the workshop's UI object, an event will fire that will change the shape of the
		/// actual PictureBox control.
		/// </summary>
		private void CropCanvas()
		{
			// Offset all the points in all the Channels by -1 * cropRect.Location
			foreach (Channel ch in _workshop.Channels.GetAllChannels())
			{
				ch.MoveCells(new PointF(-1 * _cropArea.X / UISettings.ʃCellScale, -1 * _cropArea.Y / UISettings.ʃCellScale));
			}

			// resize the lattice to the new dimensions
			_workshop.UI.LatticeSize = new Size(_cropArea.Width / UISettings.ʃCellScale, _cropArea.Height / UISettings.ʃCellScale);
		}

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

			// Get a pointer to the controls on the toolstrip that belongs to us.
			ClearCrop = (ToolStripButton)GetItem<ToolStripButton>(1);
			ExecuteCrop = (ToolStripButton)GetItem<ToolStripButton>(2);

			if ((ClearCrop == null) || (ExecuteCrop == null))
				return;

			// Attach all events that would normally go within the form to methods in this class
			ClearCrop.Click += new System.EventHandler(this.ClearCrop_Click);
			ExecuteCrop.Click += new System.EventHandler(this.ExecuteCrop_Click);

			// Set the initial value for the contol from what we had retrieve from Settings
			ClearCrop.Enabled = false;
			ExecuteCrop.Enabled = false;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;
			_mouseDownCell = mouseCell;
			_canvasControlGraphics = Workshop.Canvas.CreateGraphics();

			// Trap the mouse into the Canvas while we are working
			TrapMouse();

			CaptureCanvas();

			//// Create an undo for the cropping action
			//UndoData.v1 Undo = new UndoData.v1();
			//Undo.Action = this.UndoText;
			//Undo.GeneralEvent = GeneralDataEvent.UI;
			//Undo.SpecificEvent = SpecificEventType.Crop;
			//Undo.LatticeSize = _workshop.UI.LatticeSize;
			//for (int i = 0; i < _workshop.Channels.Count; i++)
			//{
			//    Undo.AffectedChannels.Add(_workshop.Channels[i].Index);
			//    Undo.Cells.Add(_workshop.Channels[i].ClonedLattice);
			//}
			//_workshop.UndoController.Push(Undo);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseMove(buttons, mouseCell, mousePixel))
				return false;

			_currentMouseCell.Offset(1, 1);

			// Draw the cropping rectangle
			Rectangle DrawArea = _workshop.NormalizedRectangle(Workshop.PixelPoint(_mouseDownCell), Workshop.PixelPoint(_currentMouseCell));

			using (Pen MarqueePen = RenderPen())
				_canvasControlGraphics.DrawRectangle(MarqueePen, DrawArea);

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseUp(buttons, mouseCell, mousePixel))
				return false;

			ReleaseMouse();

			// Define the area of cells that will be cropped to
			_cropArea = _workshop.NormalizedRectangle(Workshop.PixelPoint(_mouseDownCell), Workshop.PixelPoint(mouseCell));

			CropCanvas();
			//ClearCrop.Enabled = true;
			//ExecuteCrop.Enabled = true;

			PostDrawCleanUp();

			return true;
		}

		/// <summary>
		/// Creates the Pen used to display the cropping rectangle.
		/// </summary>
		protected override Pen RenderPen()
		{
			return new Pen(Color.Aqua);
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		/// <summary>
		/// Clear out the crop area rectangle and refresh the canvas to get rid of the crop marks.
		/// </summary>
		private void ClearCrop_Click(object sender, EventArgs e)
		{
			_cropArea = Rectangle.Empty;
			Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Crop the canvas using the rectangle defined.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExecuteCrop_Click(object sender, EventArgs e)
		{
			CropCanvas();
		}

		#endregion [ ToolStrip Events ]
	
	}
}


