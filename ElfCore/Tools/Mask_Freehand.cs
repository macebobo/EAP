using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Util;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	/// <summary>
	/// This tool allows the user to define a line by clicking on point to point, until they doubleclick to end the looping. 
	/// </summary>
	[ElfEditTool("Freehand Mask")]
	public class MaskFreehandTool : MaskBase, ITool
	{
		#region [ Private Variables ]

		private List<Point> _maskPoints = new List<Point>();
		private Point _prevMouseCellPixel = Point.Empty;
		bool _closeFigure = false;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public MaskFreehandTool() : base()
		{
			this.ID = (int)ToolID.Mask_Freehand;
			this.Name = "Freehand Marquee";
			this.ToolBoxImage = ElfRes.mask_freehand;
			this.MaskTypeCursorModifier = ElfRes.freehand_modifier;
			_freehandStyle = true;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, UnitScale scaling)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			Point LastPoint = Point.Empty;
			Point CurrentPoint = Point.Empty;

			int CellScale = Scaling.CellScale;

			foreach (Point p in _maskPoints)
			{
				CurrentPoint = (scaling == UnitScale.Lattice) ? p : new Point(p.X / CellScale, p.Y / CellScale);

				if (!LastPoint.IsEmpty)
				{
					if (LastPoint.Equals(CurrentPoint))
						continue;
					DrawPath.AddLine(LastPoint, CurrentPoint);
				}
				LastPoint = CurrentPoint;
			}
			if (_closeFigure)
				DrawPath.CloseFigure();
			DrawPath.CloseFigure();
			return DrawPath;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			base.MouseDown(buttons, latticePoint, actualCanvasPoint);
			if (_maskPoints == null)
				_maskPoints = new List<Point>();
			_maskPoints.Add(_workshop.CalcCanvasPoint(latticePoint));
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

			_currentLatticePoint = latticePoint;
			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);

			// Draw the captured bitmap onto the CanvasPane PictureBox
			DisplayCapturedCanvas();

			// Temporarily add the current point to the array
			_maskPoints.Add(_currentMouseCanvasPoint);

			Render(Point.Empty, Point.Empty, UnitScale.Lattice, false);

			// Remove the last point
			_maskPoints.RemoveAt(_maskPoints.Count - 1);

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
			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);

			// Check to see if the last point added is the same as this current one
			if (_currentMouseCanvasPoint.Equals(_prevMouseCellPixel))
			{
				_closeFigure = true;
				// Close up the figure and do a final render
				base.MouseUp(buttons, latticePoint, actualCanvasPoint);
			}
			else
			{
				_maskPoints.Add(_currentMouseCanvasPoint);
				Render(Point.Empty, Point.Empty, UnitScale.Lattice, false);
				_prevMouseCellPixel = _currentMouseCanvasPoint;
			}
			return true;
		}
				
		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();
			_maskPoints.Clear();
			_prevMouseCellPixel = Point.Empty;
			_closeFigure = false;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// Remove all the click events from these shared controls
		/// </summary>
		public override void OnUnselected()
		{
			base.OnUnselected();
			_maskPoints = null;
		}
			
		#endregion [ Methods ]

	}
}
