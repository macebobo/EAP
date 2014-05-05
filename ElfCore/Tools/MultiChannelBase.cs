using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
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
	/// Common functionality for multi-Channel tools
	/// </summary>
	public abstract class MultiChannelBase : BaseTool, ITool
	{

		#region [ Constructors ]

		public MultiChannelBase()
		{
			this.ID = (int)ToolID.NotSet;
			this.Name = "MULTIChannelBASE";
			this.ToolBoxImage = ElfRes.not;
			this.Cursor = Cursors.Cross;
			this.AffectMultipleChannels = true;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_MULTICHANNEL;
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
				p1 = _workshop.CalcLatticePoint(p1);
				p2 = _workshop.CalcLatticePoint(p2);
			}

			List<GraphicsPath> Paths = CreateRenderPathList(p1, p2, finalRender);
			if (Paths == null)
				return;

			if (finalRender)
			{
				BaseChannelList SelectedChannels = new BaseChannelList();

				using (Pen DrawPen = RenderPen())
				{
					BaseChannel Current = null;
					for (int i = 0; i < this.NumberOfChannels(); i++)
					{
						if (Profile.Channels.Active.Index + i >= Profile.Channels.Count)
							break;

						if (i >= Paths.Count)
							break;

						Current = Profile.Channels.Sorted[Profile.Channels.Active.Index + i];
						SelectedChannels.Add(Current);

						_latticeBuffer = Current.LatticeBuffer;
						_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
						_latticeBufferGraphics.DrawPath(DrawPen, Paths[i]);

						#if DEBUG
							Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
						#endif

						// Write out the changes onto the active Channel
						Current.Empty();
						Current.LatticeBuffer = _latticeBuffer;

						_latticeBufferGraphics.Dispose();
						_latticeBuffer.Dispose();
					}
				}
				Profile.Channels.Selected = SelectedChannels;
				SelectedChannels = null;
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
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			_isMouseDown = true;
			_mouseDownLatticePoint = latticePoint;
			_mouseDownCanvasPoint = _workshop.CalcCanvasPoint(latticePoint);
			_currentLatticePoint = latticePoint;
			_canvasControlGraphics = Profile.GetCanvasGraphics();

			CaptureCanvas();

			// Trap the mouse into the Canvas while we are working
			//TrapMouse();
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
			
			if (!base.MouseMove(buttons, latticePoint, actualCanvasPoint))
				return false;

			Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, false);

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
			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			Cursor LastCursor = Profile.Cursor;
			Profile.Cursor = Cursors.WaitCursor;

			// Render the shape onto the active Channel
			Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, true);

			// Redraw the canvas to expose our changes.
			Profile.Refresh();

			_mouseDownLatticePoint = Point.Empty;
			Profile.Cursor = LastCursor;

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
