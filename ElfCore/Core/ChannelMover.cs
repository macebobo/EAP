using ElfCore.Channels;
using ElfCore.Util;

using System.Drawing;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Core
{
	public class ChannelMover : Clipboard
	{

		#region [ Constants ]

		public const string UNDO_MOVECELLS = "Move Cells";
		public const string UNDO_DELETECELLS = "Delete Cells";

		#endregion [ Constants ]

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels.
		/// </summary>
		public override bool Cut()
		{
			if (!Copy(true))
				return false;
			Profile.Refresh();
			return true;
		}

		/// <summary>
		/// Deletes the cells selected by the mask.
		/// </summary>
		public bool Delete()
		{
			if (!Copy(true))
				return false;
			ClearCells();
			Profile.Refresh();
			return true;
		}

		/// <summary>
		/// If during DEBUG, display the contents of the clipboard. Flatten the images of all channels in the clipboard into a single bitmap,
		/// and draw the outline of the mask.
		/// </summary>
		internal override void DisplayDiagnosticData()
		{
			#if DEBUG
			Rectangle Bounds = GetBounds();
			Bitmap ClipBmp = new Bitmap(Profile.Scaling.CanvasSize.Width, Profile.Scaling.CanvasSize.Height);

			using (Graphics g = Graphics.FromImage(ClipBmp))
			{
				g.Clear(Color.Black);
				if (_channels != null)
				{
					foreach (Channel Channel in _channels)
					{
						g.FillPath(Brushes.White, Channel.GetGraphicsPath(Profile.Scaling));
					}
				}
				using (Pen MarqueePen = _workshop.GetMarqueePen())
				{
					if (_mask != null)
						g.DrawPath(MarqueePen, _mask.CanvasMask.Outline);
				}
			}
			_workshop.ExposePane(ClipBmp, Panes.MoveChannel);
			#endif
		}

		/// <summary>
		/// Moves the cells defined by the current mask by the offset amount for all the selected channels.
		/// </summary>
		/// <param name="offset">Amount to move the cells.</param>
		public void Move(LatticePoint offset)
		{
			if (offset.IsEmpty)
				return;

			foreach (Channel Channel in _channels)
				Channel.MoveData(offset);
			_mask.Move(offset, UnitScale.Canvas);
			Profile.SetMask(_mask);
			Profile.Refresh();
		}

		/// <summary>
		/// Copies the cells from the Clipboard Channels back out to the Workshop Channels.
		/// If there is more than one Channel of data in the Clipboard, and there is more than one Channel
		/// selected in the Profile, then it will paste into the selected Channels. If there is one a single Channel selected,
		/// then it will paste into the Active Channel and move down the Channel list from there.
		/// </summary>
		public override void Paste()
		{
			DoPaste();
			ClearCells();

			_mask.Dispose();
			_mask = new Mask();

			Profile.Refresh();
			DisplayDiagnosticData();
		}
	}
}
