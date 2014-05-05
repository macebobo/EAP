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
	[ElfEditTool("Crop")]
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
			this.ID = (int)ToolID.Crop;
			this.Name = "Crop";
			this.DoesSelection = true;
			this.ToolBoxImage = ElfRes.crop;
			this.Cursor = base.CreateCursor(ElfRes.crop, new Point(8, 8));
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
				if (ClearCrop != null)
					ClearCrop.Click += new EventHandler(ClearCrop_Click);
				if (ExecuteCrop != null)
					ExecuteCrop.Click += new EventHandler(ExecuteCrop_Click);
			}
			else
			{
				if (ClearCrop != null)
					ClearCrop.Click -= ClearCrop_Click;
				if (ExecuteCrop != null)
					ExecuteCrop.Click -= ExecuteCrop_Click;
			}
			base.AttachEvents(attach);
		}

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
			//foreach (Channel ch in Profile.Channels.GetAllChannels())
			int CellScale = Scaling.CellScale;
			for (int i = 0; i < Profile.Channels.Count; i++)
			{
				Profile.Channels[i].MoveData(new PointF(-1 * _cropArea.X / CellScale, -1 * _cropArea.Y / CellScale));
			}

			// resize the lattice to the new dimensions
			Scaling.LatticeSize = new Size(_cropArea.Width / CellScale, _cropArea.Height / CellScale);
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

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			ClearCrop = (ToolStripButton)GetItem<ToolStripButton>(1);
			ExecuteCrop = (ToolStripButton)GetItem<ToolStripButton>(2);

			// Set the initial value for the contol from what we had retrieve from Settings
			if (ClearCrop != null)
				ClearCrop.Enabled = false;
			if (ExecuteCrop != null)
				ExecuteCrop.Enabled = false;
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
			_mouseDownLatticePoint = latticePoint;
			_canvasControlGraphics = Profile.GetCanvasGraphics();

			// Trap the mouse into the Canvas while we are working
			TrapMouse();

			CaptureCanvas();
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		/// <returns>Return true if the MouseDown flag was set.</returns>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (Profile == null)
				return false;

			if (!base.MouseMove(buttons, latticePoint, actualCanvasPoint))
				return false;

			_currentLatticePoint.Offset(1, 1);

			// Draw the cropping rectangle
			Rectangle DrawArea = _workshop.NormalizedRectangle(_workshop.CalcCanvasPoint(_mouseDownLatticePoint), _workshop.CalcCanvasPoint(_currentLatticePoint));

			using (Pen MarqueePen = RenderPen())
				_canvasControlGraphics.DrawRectangle(MarqueePen, DrawArea);

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
			if (Profile == null)
				return false;

			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			ReleaseMouse();

			// Define the area of cells that will be cropped to
			_cropArea = _workshop.NormalizedRectangle(_workshop.CalcCanvasPoint(_mouseDownLatticePoint), _workshop.CalcCanvasPoint(latticePoint));

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

		#region [ ToolStrip Event Delegates ]

		/// <summary>
		/// Clear out the crop area rectangle and refresh the canvas to get rid of the crop marks.
		/// </summary>
		private void ClearCrop_Click(object sender, EventArgs e)
		{
			_cropArea = Rectangle.Empty;
			Profile.Refresh();
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

		#endregion [ ToolStrip Event Delegates ]
	
	}
}


