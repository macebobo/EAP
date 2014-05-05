using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ElfCore
{
	public class Clipboard
	{
		#region [ Constants ]

		private const string UNDO_CUT = "Cut";
		private const string UNDO_PASTE = "Paste";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private Workshop _workshop = Workshop.Instance;
		private List<Channel> _channels = null;

		// Copies from Data. We do this so that we can restore the original masked area on a paste event
		private Mask _mask = null;
		
		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Returns true if there is any data in any Channels we might be holding
		/// </summary>
		public bool HasData
		{
			get 
			{
				if (_channels.Count > 0)
					foreach (Channel Ch in _channels)
						if (Ch.HasData)
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
		/// Exposes the list of Channels
		/// </summary>
		public List<Channel> Channels
		{
			get { return _channels; }
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
			_channels = new List<Channel>();
			//Clear();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Clears out the cells from all Channels, and pares the list back
		/// </summary>
		//private void Clear()
		//{
		//    _workshop.Mask.Clear();
		//    _mask.Clear();
		//    ClearCells();
		//}

		/// <summary>
		/// Clears all the Cells stored in the Cliboard and removes the Channels
		/// </summary>
		private void ClearCells()
		{
			foreach (Channel Channel in _channels)
			{
				Channel.ClearLattice();
				Channel.Dispose();
			}
			_channels = new List<Channel>();
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels
		/// </summary>
		/// <returns></returns>
		public bool Copy()
		{
			return Copy(false);
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels
		/// </summary>
		/// <param name="deleteCellsFromOriginalChannels">Indicates whether the cells defined by the mask should be removed from their parent Channels, such as when we Cut</param>
		/// <returns></returns>
		private bool Copy(bool deleteCellsFromOriginalChannels)
		{
			// Clear out the Cells of existing Clipboard Channels
			ClearCells();

			// Make sure we have enough Channels for this operation
			for (int i = 0; i < _workshop.Channels.Selected.Count; i++)
				_channels.Add(new Channel());

			// Copy the mask from the Workshop
			_mask.Define(_workshop.Mask, false);

			// Copy the Cells from the selected area indicated by the Mask from the Selected Channels to the Clipboard Channels.
			for (int i = 0; i < _workshop.Channels.Selected.Count; i++)
			{
				if (!TransferToChannel(_workshop.Channels.Selected[i], _channels[i], true, true, deleteCellsFromOriginalChannels))
					return false;
			}

			OnChanged();
			return true;
		}

		/// <summary>
		/// Copies the cells defined by the mask from the selected Channel(s) into our own Channels.
		/// </summary>
		public bool Cut()
		{
			//_workshop.CreateUndo_Channel(UNDO_CUT, _workshop.SelectedChannels, true);
			return Copy(true);
		}
				
		/// <summary>
		/// Delete the cells in the current masked area, without copying into the clipboard
		/// </summary>
		public void Delete()
		{
			_workshop.Channels.MoveChannel.ClearLattice();
			_workshop.UndoController.SaveUndo("Delete");
		}

		///// <summary>
		///// Gets the bounded area of cells currently stored in the Clipboard
		///// </summary>
		///// <returns></returns>
		//public RectangleF GetBounds()
		//{
		//    RectangleF Bounds = _workshop.ClipboardChannel.GetBounds();

		//    using (Matrix ScaleMatrix = new System.Drawing.Drawing2D.Matrix())
		//    {
		//        ScaleMatrix.Scale(UISettings.ʃCellScaleF, UISettings.ʃCellScaleF);
		//        using (GraphicsPath ScalesPath = new GraphicsPath())
		//        {
		//            ScalesPath.AddRectangle(Bounds);
		//            ScalesPath.Transform(ScaleMatrix);
		//            return ScalesPath.GetBounds();
		//        }
		//    }
		//}

		/// <summary>
		/// Copies the cells from the Clipboard Channels back out to the Workshop Channels.
		/// </summary>
		public void Paste()
		{
			//_workshop.CreateUndo_Channel(UNDO_PASTE, _workshop.SelectedChannels, true);

			Channel TargetChannel = null;
			if (this.HasData)
			{
				for (int i = 0; i < _channels.Count; i++)
				{
					// If there are more Channels stored in the clipboard than there are selected in the Workshop, then
					// continue pasting past the end of the list of selected Channels
					if (i >= _workshop.Channels.Selected.Count)
						// If we are going to go past the end of list of Channels, then just stop.
						if (TargetChannel.Index + 1 >= _workshop.Channels.Count)
							break;
						else
							TargetChannel = _workshop.Channels.Sorted[TargetChannel.Index + 1];
					else
						TargetChannel = _workshop.Channels.Selected[i];
					TransferToChannel(_channels[i], TargetChannel, false, false, false);
				}
			}
			TargetChannel = null;
			_workshop.UndoController.SaveUndo(UNDO_PASTE);
		}

		//public void RestoreFromMoveChannel()
		//{
		//    RestoreFromMoveChannel(_workshop.ActiveChannel);
		//}

		//public void RestoreFromMoveChannel(Channel Channel)
		//{
		//    Channel.PaintCells(_workshop.MoveChannel);
		//}

		//public bool TransferToMoveChannel()
		//{
		//    return TransferToChannel(_workshop.ActiveChannel, _workshop.MoveChannel, true, true, true);
		//}

		//public bool TransferToMoveChannel(Channel sourceChannel)
		//{
		//    return TransferToChannel(sourceChannel, _workshop.MoveChannel, true, true, true);
		//}

		/// <summary>
		/// Copies all the cells from the source Channel [within the masked region] and moves them into the target Channel.
		/// </summary>
		/// <param name="sourceChannel">Channel cells are removed from</param>
		/// <param name="targetChannel">Channel cells are placed within</param>
		/// <param name="useMaskedRegion">Indicates if the masked region should be used to define which cells are to be used</param>
		/// <param name="clearTargetChannel">Indicates if the target Channel should be first cleared of existing cells</param>
		/// <param name="removeCellsFromSource">Indicates if the cells should be removed from the source Channel</param>
		/// <returns>Returns true if the source Channel had cells within the masked region</returns>
		public bool TransferToChannel(Channel sourceChannel, Channel targetChannel, bool useMaskedRegion, bool clearTargetChannel, bool removeCellsFromSource)
		{
			if (clearTargetChannel)
				targetChannel.ClearLattice();

			List<Point> TempCells = new List<Point>();
			
			// Copy all the cells from the Channel into the local array without remove them from the Channel
			// Scan the masked region for lit pixels and use those coordinates to populate the array.

			if (useMaskedRegion)
			{
				using (Region Region = new Region(_workshop.Mask.LatticeMask.Outline))
					foreach (Point Cell in sourceChannel.Lattice)
					{
						if (Region.IsVisible(Cell.X, Cell.Y))
							TempCells.Add(Cell);
					}
			}
			else
				TempCells.AddRange(sourceChannel.Lattice);

			if (removeCellsFromSource)
				sourceChannel.EraseCell(TempCells);

			if (TempCells.Count == 0)
				return false;

			targetChannel.PaintCells(TempCells);
			TempCells = null;
			return true;
		}

		private void OnChanged()
		{
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		#endregion [ Methods ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when data is put into the clipboard buffer
		/// </summary>
		public EventHandler Changed;

		#endregion [ Event Handlers ]

	}
}
