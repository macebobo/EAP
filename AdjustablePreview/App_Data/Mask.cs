using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ElfCore
{
	[Serializable]
	public class Mask : ICloneable, IDisposable
	{
		#region [ Private Variables ]

		private MaskData _canvas = null;
		private MaskData _lattice = null;
		
		[NonSerialized()]
		private Bitmap _overlay = null;

		[NonSerialized()]
		private bool _disposed = false;

		#endregion [ Private Variables ]

		#region [ Properties ]
		
		/// <summary>
		/// Returns true if this mask has been defined
		/// </summary>
		public bool HasMask
		{
			get { return _canvas.HasData || _lattice.HasData; }
		}

		/// <summary>
		/// Retrieves or Defines the Mask for the Canvas
		/// </summary>
		public MaskData CanvasMask
		{
			get { return _canvas; }
			set { _canvas = (MaskData)value.Clone(); }
		}

		/// <summary>
		/// Retrieves or Defines the Mask for the Lattice
		/// </summary>
		public MaskData LatticeMask
		{
			get { return _lattice; }
			set { _lattice = (MaskData)value.Clone(); }
		}

		/// <summary>
		/// Semi-transparent bitmap that covers the masked off part of the canvas.
		/// </summary>
		public Bitmap Overlay
		{
			get 
			{
				if ((_overlay == null) || (_overlay.Width != UISettings.ʃCanvasSize.Width) || (_overlay.Height != UISettings.ʃCanvasSize.Height))
					CreateOverlayBitmap();

				return _overlay; 
			}
		}
		
		/// <summary>
		/// Indicates whether events should be suppressed from firing. Use sparingly.
		/// </summary>
		public bool SuppressEvents { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public Mask()
		{
			if (_disposed)
				GC.ReRegisterForFinalize(true);
			_disposed = false;

			_canvas = new MaskData();
			_lattice = new MaskData();
		}

		public Mask(Mask existingMask)
			: this()
		{
			Define(existingMask);
		}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~Mask()
		{
			//Execute the code that does the cleanup.
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				if (_overlay != null)
				{
					_overlay.Dispose();
					_overlay = null;
				}
				if (_canvas != null)
				{
					_canvas.Dispose();
					_canvas = null;
				}
				if (_lattice != null)
				{
					_lattice.Dispose();
					_lattice = null;
				}
			}

			// Remember that we've executed this code
			_disposed = true;
		}

		public void Dispose()
		{
			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);
		}

		#endregion [ Destructors ]

		#region [ Methods ]

		/// <summary>
		/// Clear out the masked area
		/// </summary>
		public void Clear()
		{
			_canvas.Clear();
			_lattice.Clear();

			if (_overlay != null)
			{
				_overlay.Dispose();
				_overlay = null;
			}

			OnCleared();
		}

		/// <summary>
		/// Create a deep clone of this object.
		/// </summary>
		public object Clone()
		{
			Mask Cloned = new Mask();
			Cloned.SuppressEvents = true;
			Cloned.Define(this, true);
			Cloned.SuppressEvents = false;
			return Cloned;
		}

		/// <summary>
		/// Creates a semi-transparent bitmap to cover the non-masked areas of the screen
		/// </summary>
		private void CreateOverlayBitmap()
		{
			if (_overlay != null)
			{
				_overlay.Dispose();
				_overlay = null;
			}

			_overlay = new Bitmap(UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height);
			Graphics g = Graphics.FromImage(_overlay);
			g.Clear(Color.Transparent);

			GraphicsPath Path = new GraphicsPath();
			Path.AddRectangle(new Rectangle(0, 0, UISettings.ʃCanvasSize.Width, UISettings.ʃCanvasSize.Height));
			if (_canvas.Outline.PointCount > 0)
				Path.AddPath(_canvas.Outline, false);

			using (SolidBrush OverlayBrush = new SolidBrush(Color.FromArgb(64, Color.Red)))
				g.FillPath(OverlayBrush, Path);

			g.Dispose();
			Path.Dispose();
		}

		/// <summary>
		/// Instructs the MaskData objects to reconstitute the Outline object from its components arrays
		/// </summary>
		public void Deserialize()
		{
			_canvas.Deserialize();
			_lattice.Deserialize();
			CreateOverlayBitmap();
		}
		
		/// <summary>
		/// Determines whether the data within the specified Mask object matches that of the current Mask object
		/// </summary>
		/// <param name="obj">The Mask object to compare to the current one.</param>
		public bool Differs(Mask other)
		{
			return other.LatticeMask.Differs(this.LatticeMask) || (other.CanvasMask.Differs(this.CanvasMask));
		}

		/// <summary>
		/// Moves the masked area by a given offset amount
		/// </summary>
		/// <param name="offset">Point representing the amount the mask is to move</param>
		/// <param name="scaling">Indicates the scaling of the offset</param>
		public void Move(Point offset, Scaling scaling)
		{
			Point CanvasOffset = Point.Empty;
			Point LatticeOffice = Point.Empty;

			if (scaling == Scaling.Cell)
			{
				LatticeOffice = offset;
				CanvasOffset = new Point(offset.X * UISettings.ʃCellScale, offset.Y * UISettings.ʃCellScale);
			}
			else
			{
				LatticeOffice = new Point(offset.X * (int)(1f / UISettings.ʃCellScale), offset.Y * (int)(1f / UISettings.ʃCellScale));
				CanvasOffset = offset;
			}

			_canvas.Move(CanvasOffset);
			_lattice.Move(LatticeOffice);

			CreateOverlayBitmap();
		}

		/// <summary>
		/// Moves the masked area by a given offset amount
		/// </summary>
		/// <param name="offset">Point representing the amount the mask is to move</param>
		/// <param name="scaling">Indicates the scaling of the offset</param>
		public void Move(PointF offset, Scaling scaling)
		{
			PointF CanvasOffset = Point.Empty;
			PointF LatticeOffice = Point.Empty;

			if (scaling == Scaling.Cell)
			{
				LatticeOffice = offset;
				CanvasOffset = new PointF(offset.X * UISettings.ʃCellScale, offset.Y * UISettings.ʃCellScale);
			}
			else
			{
				LatticeOffice = new PointF(offset.X * (1f / UISettings.ʃCellScale), offset.Y * (1f / UISettings.ʃCellScale));
				CanvasOffset = offset;
			}

			_canvas.Move(CanvasOffset);
			_lattice.Move(LatticeOffice);

			CreateOverlayBitmap();
		}

		/// <summary>
		/// Instructs the MaskData objects to Deconstitute the Outline object into its components arrays
		/// </summary>
		public void Serialize()
		{
			_canvas.Serialize();
			_lattice.Serialize();
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		public void Define(Mask newMask)
		{
			Define(newMask, true);
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		/// <param name="createOverlay">Flag to indicate if the overlay bitmap should be created.</param>
		public void Define(Mask newMask, bool createOverlay)
		{
			_canvas.Clear();
			_lattice.Clear();

			if (newMask.HasMask)
			{
				_lattice = new MaskData(newMask.LatticeMask);
				_canvas = new MaskData(newMask.CanvasMask);
				OnDefined();
			}
			if (createOverlay)
				CreateOverlayBitmap();
		}

		/// <summary>
		/// Sets the outline of the mask to these values
		/// </summary>
		/// <param name="canvasOutline">GraphicsPath for the Canvas mask</param>
		/// <param name="latticeOutline">GraphicsPath for the Lattice mask</param>
		public void Define(GraphicsPath canvasOutline, GraphicsPath latticeOutline)
		{
			_canvas.Clear();
			_lattice.Clear();

			if (!_canvas.Outline.Equals(canvasOutline))
				_canvas.Outline = canvasOutline;

			if (!_lattice.Outline.Equals(canvasOutline))
				_lattice.Outline = latticeOutline;

			//_canvas.RegionFromOutline();
			//_lattice.RegionFromOutline();

			OnDefined();
			CreateOverlayBitmap();
		}

		#endregion [ Methods ]

		#region [ Operator Overloading ]

		#endregion [ Operator Overloading ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the Mask is Defined
		/// </summary>
		public EventHandler Defined;

		/// <summary>
		/// Occurs when the Mask is removed
		/// </summary>
		public EventHandler Cleared;

		#endregion [ Event Handlers ]

		#region [ Custom Event Methods ]

		private void OnDefined()
		{
			if (SuppressEvents)
				return;
			if (Defined != null)
				Defined(this, new EventArgs());
		}

		private void OnCleared()
		{
			if (SuppressEvents)
				return;
			if (Cleared != null)
				Cleared(this, new EventArgs());
		}

		#endregion [ Custom Event Methods ]
	}
}
