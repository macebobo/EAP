using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	/// <summary>
	/// Allows the user to simply just draw the mask around the target area
	/// </summary>
	[AdjPrevTool("Lasso Mask")]
	public class MaskLassoTool : MaskBase, ITool
	{
		#region [ Private Variables ]

		private List<Point> _maskPoints = new List<Point>();
		//private Point _prevMouseCellPixel = Point.Empty;
		//bool _closeFigure = false;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public MaskLassoTool()
		{
			this.ID = (int)Tool.Mask_Lasso;
			this.Name = "Lasso Marquee";
			this.ToolBoxImage = ElfRes.mask_lasso;
			this.DoesSelection = true;
			this.MaskTypeCursorModifier = ElfRes.lasso_modifier;
			_freehandStyle = true;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, Scaling scaling)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			Point LastPoint = Point.Empty;
			Point CurrentPoint = Point.Empty;

			foreach (Point p in _maskPoints)
			{
				CurrentPoint = (scaling == Scaling.Pixel) ? p : new Point(p.X / UISettings.ʃCellScale, p.Y / UISettings.ʃCellScale);

				if (!LastPoint.IsEmpty)
				{
					if (LastPoint.Equals(CurrentPoint))
						continue;
					DrawPath.AddLine(LastPoint, CurrentPoint);
				}
				LastPoint = CurrentPoint;
			}
			//if (_closeFigure)
				DrawPath.CloseFigure();
			return DrawPath;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			base.MouseDown(buttons, mouseCell, mousePixel);
			if (_maskPoints == null)
				_maskPoints = new List<Point>();
			_maskPoints.Add(Workshop.PixelPoint(mouseCell));
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

			_maskPoints.Add(Workshop.PixelPoint(mouseCell));

			// Draw the captured bitmap onto the CanvasPane PictureBox
			DisplayCapturedCanvas();

			Render(Point.Empty, Point.Empty, Scaling.Pixel, false);

			return true;			
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_maskPoints.Add(Workshop.PixelPoint(mouseCell));
			base.MouseUp(buttons, mouseCell, mousePixel);
			return true;
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();
			_maskPoints.Clear();
			//_prevMouseCellPixel = Point.Empty;
			//_closeFigure = false;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// Remove all the click events from these shared controls
		/// </summary>
		public override void Unselected()
		{
			base.Unselected();
			_maskPoints = null;
		}

		#endregion [ Methods ]

	}
}
