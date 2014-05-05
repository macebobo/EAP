using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	/// <summary>
	/// Common functionality for multi-Channel tools
	/// </summary>
	public abstract class MultiChannelBase : BaseTool, ITool
	{

		#region [ Constructors ]

		public MultiChannelBase()
		{
			this.ID = (int)Tool.NotSet;
			this.Name = "MULTIChannelBASE";
			this.ToolBoxImage = ElfRes.not;
			this.Cursor = Cursors.Cross;
			this.AffectMultipleChannels = true;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_MULTIChannel;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Draw the shape
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected virtual void Render(Point p1, Point p2, bool finalRender)
		{
			if (p1.Equals(p2))
				return;

			if (finalRender)
			{
				p1 = Workshop.CellPoint(p1);
				p2 = Workshop.CellPoint(p2);
			}

			List<GraphicsPath> Paths = CreateRenderPathList(p1, p2, finalRender);
			if (Paths == null)
				return;

			if (finalRender)
			{
				using (Pen DrawPen = RenderPen())
				{
					Channel Current = null;
					for (int i = 0; i < this.NumberOfChannels(); i++)
					{
						if (_workshop.Channels.Active.Index + i >= _workshop.Channels.Count)
							break;

						if (i >= Paths.Count)
							break;

						Current = _workshop.Channels.Sorted[_workshop.Channels.Active.Index + i];
						Current.IsSelected = true;

						_latticeBuffer = Current.LatticeBuffer;
						_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
						_latticeBufferGraphics.DrawPath(DrawPen, Paths[i]);
						Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);

						// Write out the changes onto the active Channel
						Current.ClearLattice();
						Current.LatticeBuffer = _latticeBuffer;

						_latticeBufferGraphics.Dispose();
						_latticeBuffer.Dispose();
					}
				}
			}
			else
			{
				// This version displays while the user is dragging the mouse across the canvas
				Paths = MergePaths(Paths);
				using (Pen MarqueePen = _workshop.GetMarqueePen())
				{
					try
					{
						_canvasControlGraphics.DrawPath(MarqueePen, Paths[0]);
					}
					catch (OutOfMemoryException)
					{ }
				}
			}

			Paths = null;
		}

		/// <summary>
		/// Generate the series of Graphics Paths, one per Channel. If this is not the final render, all paths are merged together
		/// into a single one.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected abstract List<GraphicsPath> CreateRenderPathList(Point p1, Point p2, bool finalRender);

		/// <summary>
		/// Merge all graphics paths into a single one. Clears the list and puts the merged path as the first element
		/// </summary>
		protected virtual List<GraphicsPath> MergePaths(List<GraphicsPath> paths)
		{
			GraphicsPath Path = new GraphicsPath();

			foreach (GraphicsPath SubPath in paths)
				Path.AddPath(SubPath, false);

			paths = new List<GraphicsPath>();
			paths.Add(Path);

			return paths;
		}

		/// <summary>
		/// Canvas MouseDown event was fired. This version differs from the one in BaseTool in that the pushing of the Undo
		/// data happens in MouseUp instead of MouseDown
		/// </summary>
		/// <param name="_canvas">PictureBox control that fired this event</param>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;
			_mouseDownCell = mouseCell;
			_mouseDownCellPixel = Workshop.PixelPoint(mouseCell);
			_currentMouseCell = mouseCell;
			_canvasControlGraphics = Workshop.Canvas.CreateGraphics();

			CaptureCanvas();

			// Trap the mouse into the Canvas while we are working
			//TrapMouse();
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseMove(buttons, mouseCell, mousePixel))
				return false;

			Render(_mouseDownCellPixel, _constrainedCellPixel, false);

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

			Cursor LastCursor = Workshop.Canvas.Cursor;
			Workshop.Canvas.Cursor = Cursors.WaitCursor;

			// Render the shape onto the active Channel
			Render(_mouseDownCellPixel, _constrainedCellPixel, true);

			// Redraw the canvas to expose our changes.
			Workshop.Canvas.Refresh();

			_mouseDownCell = Point.Empty;
			Workshop.Canvas.Cursor = LastCursor;

			// Clean up
			PostDrawCleanUp();
			
			return true;
		}

		/// <summary>
		/// Calculate the number of Channels that are needed.
		/// </summary>
		protected abstract int NumberOfChannels();

		/// <summary>
		/// Creates the Pen used to render the shape onto the Paint Pane
		/// </summary>
		protected override Pen RenderPen()
		{
			return new Pen(Color.White, 1);
		}

		#endregion [ Methods ]
	
	}
}
