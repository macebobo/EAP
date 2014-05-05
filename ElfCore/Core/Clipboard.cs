using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Core
{
	public class Clipboard : ElfDisposable
	{
		#region [ Constants ]

		public const string UNDO_CUT = "Cut";
		public const string UNDO_COPY = "Copy";
		public const string UNDO_PASTE = "Paste";
		public const string UNDO_DELETE = "Delete";

		#endregion [ Constants ]

		#region [ Protected Variables ]

		protected Workshop _workshop = Workshop.Instance;
		protected ChannelList _channels = null;

		// Copies from Data. We do this so that we can restore the original masked area on a paste event
		protected Mask _mask = null;

		#endregion [ Protected Variables ]

		#region [ Properties ]

		/// <summary>
		/// Exposes the list of Channels
		/// </summary>
		public ChannelList Channels
		{
			get { return _channels; }
		}

		/// <summary>
		/// Returns true if there is any data in any Channels we might be holding
		/// </summary>
		public bool HasData
		{
			get
			{
				if (_channels.Count > 0)
					foreach (Channel Ch in _channels)
						if (Ch.HasLatticeData)
							return true;
				return false;
			}
		}

		/// <summary>
		/// Mask that was used to Cut or Copy data. Restored back to the Workshop object during a Paste operation
		/// </summary>
		public Mask Mask
		{
			get { return _mask; }
		}

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		[DebuggerHidden()]
		protected BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}

		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public bool SuppressEvents { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public Clipboard()
		{
			_mask = new Mask();
			_channels = new ChannelList();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Clears all the Cells stored in the Cliboard and removes the Channels
		/// </summary>
		protected void ClearCells()
		{
			foreach (Channel Channel in _channels)
			{
				Channel.Dispose();
			}
			_channels.Clear();
			_channels = new ChannelList();
			DisplayDiagnosticData();
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels
		/// </summary>
		/// <returns></returns>
		public bool Copy()
		{
			if (!Copy(false))
				return false;
			Profile.SaveUndo(UNDO_COPY);
			return true;
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels
		/// </summary>
		/// <param name="deleteCellsFromOriginalChannels">Indicates whether the cells defined by the mask should be removed from their parent Channels, such as when we Cut</param>
		/// <returns></returns>
		protected bool Copy(bool deleteCellsFromOriginalChannels)
		{
			// Clear out the Cells of existing Clipboard Channels
			ClearCells();

			// Make sure we have enough Channels for this operation
			for (int i = 0; i < Profile.Channels.Selected.Count; i++)
				_channels.Add(new Channel());

			// Copy the mask from the Profile
			_mask.Define(Profile.GetMaskOutline(UnitScale.Canvas), Profile.GetMaskOutline(UnitScale.Lattice), Profile.Scaling);

			bool Return = false;

			// Copy the Cells from the selected area indicated by the Mask from the Selected Channels to the Clipboard Channels.
			// Only return false if all the channels selected have nothing contained within the selected area.
			for (int i = 0; i < Profile.Channels.Selected.Count; i++)
			{
				if (TransferToChannel(Profile.Channels.Selected[i], _channels[i], true, true, deleteCellsFromOriginalChannels))
					Return = true;
			}

			DisplayDiagnosticData();
			OnChanged();
			return Return;
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels.
		/// </summary>
		public virtual bool Cut()
		{
			if (!Copy(true))
				return false;
			Profile.ClearMask();
			Profile.SaveUndo(UNDO_CUT);
			return true;
		}

		///// <summary>
		///// Delete the cells in the current masked area, without copying into the clipboard
		///// </summary>
		//public void Delete()
		//{
		//	_workshop.MoveChannel.Empty();
		//	Profile.SaveUndo(UNDO_DELETE);
		//}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			if (_mask != null)
			{
				_mask.Dispose();
				_mask = null;
			}
			_channels = null;
		}

		/// <summary>
		/// Does the work of pasting without creating Undo data.
		/// </summary>
		protected void DoPaste()
		{
			if (!HasData)
				return;
			int Index = -1;

			// Determine the Channels to paste into
			ChannelList TargetList = new ChannelList();
			if (_channels.Count == 1)
			{
				TargetList.Add(Profile.Channels.Active);
			}
			else
			{
				for (int i = 0; i < _channels.Count; i++)
				{
					if (i < Profile.Channels.Selected.Count)
						TargetList.Add(Profile.Channels.Selected[i]);
					else
					{
						// Find the next Channels after the last selected Channel
						if (Index < 0)
							Index = Profile.Channels.Selected[Profile.Channels.Selected.Count - 1].Index + 1;

						if (Index < Profile.Channels.Count)
							TargetList.Add(Profile.Channels[Index++]);
					}
				}
			}

			for (int i = 0; i < _channels.Count; i++)
			{
				if (i >= TargetList.Count)
					break;
				TransferToChannel(_channels[i], TargetList[i], false, false, false);
			}
		}

		/// <summary>
		/// If during DEBUG, display the contents of the clipboard. Flatten the images of all channels in the clipboard into a single bitmap,
		/// and draw the outline of the mask.
		/// </summary>
		internal virtual void DisplayDiagnosticData()
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
			_workshop.ExposePane(ClipBmp, Panes.Clipboard);
			#endif
		}

		/// <summary>
		/// Gets the bounded area of cells currently stored in the Clipboard
		/// </summary>
		/// <returns></returns>
		protected Rectangle GetBounds()
		{
			Rectangle Bounds;
			int Left = Int32.MaxValue;
			int Top = Int32.MaxValue;
			int Right = 0;
			int Bottom = 0;

			foreach (Channel Channel in _channels)
			{
				Bounds = Channel.GetBounds();
				Left = Math.Min(Bounds.Left, Left);
				Top = Math.Min(Bounds.Top, Top);
				Right = Math.Max(Bounds.Right, Right);
				Bottom = Math.Max(Bounds.Bottom, Bottom);
			}
			return new Rectangle(Left, Top, Right - Left, Bottom - Top);
		}

		/// <summary>
		/// Copies the cells from the Clipboard Channels back out to the Workshop Channels.
		/// If there is more than one Channel of data in the Clipboard, and there is more than one Channel
		/// selected in the Profile, then it will paste into the selected Channels. If there is one a single Channel selected,
		/// then it will paste into the Active Channel and move down the Channel list from there.
		/// </summary>
		public virtual void Paste()
		{
			DoPaste();

			// Override the Profile's mask with the version saved here
			Profile.DefineMask(Mask, true);
			Profile.SaveUndo(UNDO_PASTE);
			DisplayDiagnosticData();
		}

		/// <summary>
		/// Sets the mask into the Clipboard. Used only by the UndoController.
		/// </summary>
		public void SetMask(Mask mask)
		{
			_mask = mask;
		}

		/// <summary>
		/// Copies all the cells from the source Channel [within the masked region] and moves them into the target Channel.
		/// </summary>
		/// <param name="sourceChannel">Channel cells are removed from</param>
		/// <param name="targetChannel">Channel cells are placed within</param>
		/// <param name="useMaskedRegion">Indicates if the masked region should be used to define which cells are to be used</param>
		/// <param name="clearTargetChannel">Indicates if the target Channel should be first cleared of existing cells</param>
		/// <param name="removeCellsFromSource">Indicates if the cells should be removed from the source Channel</param>
		/// <returns>Returns true if the source Channel had cells within the masked region</returns>
		protected bool TransferToChannel(Channel sourceChannel, Channel targetChannel, bool useMaskedRegion, bool clearTargetChannel, bool removeCellsFromSource)
		{
			if (clearTargetChannel)
				targetChannel.Empty();

			List<Point> TempCells = new List<Point>();

			// Copy all the cells from the Channel into the local array without remove them from the Channel
			// Scan the masked region for lit pixels and use those coordinates to populate the array.

			if (useMaskedRegion)
			{
				using (Region Region = new Region(Profile.GetMaskOutline(UnitScale.Lattice)))
					foreach (Point Cell in sourceChannel.Lattice)
					{
						if (Region.IsVisible(Cell.X, Cell.Y))
							TempCells.Add(Cell);
					}
			}
			else
				TempCells.AddRange(sourceChannel.Lattice);

			if (removeCellsFromSource)
				sourceChannel.CutData(TempCells);

			if (TempCells.Count == 0)
				return false;

			targetChannel.Paint(TempCells);
			targetChannel.DedupeData();
			targetChannel.RenderColor = sourceChannel.GetColor();
			TempCells = null;
			return true;
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Triggers ]

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		#endregion [ Event Triggers ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when data is put into the clipboard buffer
		/// </summary>
		public EventHandler Changed;

		#endregion [ Event Handlers ]

		#endregion [ Events ]
	}
}
