using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Floodfill")]
	public class FloodfillTool : BaseTool, ITool
	{
		#region [ Constructors ]

		public FloodfillTool()
		{
			this.ID = (int)Tool.Fill;
			this.Name = "Floodfill";
			this.ToolBoxImage = ElfRes.floodfill;
			//this.Cursor = CustomCursors.MemoryCursor(ElfRes.floodfill1);
			base.Cursor = CreateCursor(ElfRes.cross_base, ElfRes.fill_modifier, new Point(15, 15));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// http://social.msdn.microsoft.com/Forums/bg-BG/csharplanguage/thread/9d926a16-0051-4ca3-b77c-8095fb489ae2
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private Bitmap FloodFill(Point fillPoint, Bitmap bitmap)
		{
			int x = fillPoint.X;
			int y = fillPoint.Y;
			//Bitmap bitmap = new Bitmap(_latticeBuffer);

			BitmapData data = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			int[] bits = new int[data.Stride / 4 * data.Height];
			Marshal.Copy(data.Scan0, bits, 0, bits.Length);

			LinkedList<Point> check = new LinkedList<Point>();
			int floodTo = Color.White.ToArgb();
			int floodFrom = bits[x + y * data.Stride / 4];
			bits[x + y * data.Stride / 4] = floodTo;

			if (floodFrom != floodTo)
			{
				check.AddLast(new Point(x, y));
				while (check.Count > 0)
				{
					if (_cancel) 
						return null;
					Point cur = check.First.Value;
					check.RemoveFirst();

					foreach (Point off in new Point[] { new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0) })
					{
						if (_cancel)
							return null;
						Point next = new Point(cur.X + off.X, cur.Y + off.Y);
						if (next.X >= 0 && next.Y >= 0 &&
							next.X < data.Width &&
							next.Y < data.Height)
						{
							if (bits[next.X + next.Y * data.Stride / 4] == floodFrom)
							{
								check.AddLast(next);
								bits[next.X + next.Y * data.Stride / 4] = floodTo;
							}
						}
					}
				}
			}

			Marshal.Copy(bits, 0, data.Scan0, bits.Length);
			bitmap.UnlockBits(data);

			return bitmap;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_mouseDown = true;
			_latticeBuffer = _workshop.Channels.Active.LatticeBuffer;
			_latticeBufferGraphics = Graphics.FromImage(_latticeBuffer);
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

			// Create Undo data to be able to reverse out our changes
			//_workshop.CreateUndo_Channel(this.UndoText);

			Bitmap Filled = null;

			if (!_workshop.Mask.HasMask)
			{
				Filled = FloodFill(_currentMouseCell, new Bitmap(_latticeBuffer));
				_latticeBufferGraphics.DrawImage(Filled, new Point(0, 0));
			}
			else
			{
				Region MaskRegion = new Region(_workshop.Mask.LatticeMask.Outline);
				GraphicsUnit unit = GraphicsUnit.Pixel;

				Bitmap MaskedChannel = new Bitmap(UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height);
				using (Graphics g = Graphics.FromImage(MaskedChannel))
				{
					g.Clear(Color.White);
					g.SetClip(MaskRegion, CombineMode.Replace);

					g.DrawImage(_latticeBuffer, _latticeBuffer.GetBounds(ref unit));
					Editor.ExposePane(MaskedChannel, Panes.LatticeBuffer);

					Filled = FloodFill(_currentMouseCell, new Bitmap(MaskedChannel));
				}
				_latticeBufferGraphics.SetClip(MaskRegion, CombineMode.Replace);
				_latticeBufferGraphics.DrawImage(Filled, Filled.GetBounds(ref unit));
				_latticeBufferGraphics.ResetClip();

				MaskedChannel.Dispose();
				MaskRegion.Dispose();
			}

			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
			_workshop.Channels.PopulateChannelFromBitmap_Erase(_latticeBuffer, _workshop.Channels.Active);

			Filled.Dispose();
			Filled = null;
			PostDrawCleanUp();
			Workshop.Canvas.Refresh();
			return !_cancel;			
		}

		#endregion [ Methods ]

	}
}


