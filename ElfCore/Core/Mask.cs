using ElfCore.Profiles;
using ElfCore.Util;
using ElfCore.Util.ClipperLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace ElfCore.Core
{
	using Polygon = List<IntPoint>;
	using Polygons = List<List<IntPoint>>;

	[Serializable]
	public class Mask : ElfBase
	{
		#region [ Private Variables ]

		private MaskData _canvas = null;
		private MaskData _lattice = null;
		private BaseProfile _profile = null;
		private Bitmap _overlay = null;
		private Scaling _scaling = null;

		#endregion [ Private Variables ]

		#region [ Properties ]
		
		/// <summary>
		/// Returns true if this mask has been defined
		/// </summary>
		[XmlIgnore()]
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
		[XmlIgnore()]
		public Bitmap Overlay
		{
			get 
			{
				if ((_overlay == null) || (_overlay.Width != _profile.Scaling.CanvasSize.Width) || (_overlay.Height != _profile.Scaling.CanvasSize.Height))
					CreateOverlayBitmap();

				return _overlay; 
			}
		}

		[XmlIgnore(),Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public byte[] ImageSerialized
		{
			get
			{
				// serialize
				if (_overlay == null)
					return null;
				using (MemoryStream ms = new MemoryStream())
				{
					_overlay.Save(ms, ImageFormat.Bmp);
					return ms.ToArray();
				}
			}
			set
			{
				// deserialize
				if (value == null)
				{
					_overlay = null;
				}
				else
				{
					using (MemoryStream ms = new MemoryStream(value))
					{
						_overlay = new Bitmap(ms);
					}
				}
			}
		}

		/// <summary>
		/// Scaling information saved so that the mask can be properly altered if the scaling of the Profile has changed
		/// or if the data is transfered between two different profiles with different scaling values.
		/// </summary>
		public Scaling Scaling
		{
			get { return _scaling; }
			set { _scaling = value; }
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore()]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<Mask>(this);
				return base.Serialized;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public Mask()
			: base()
		{
			_canvas = new MaskData();
			_lattice = new MaskData();
			_scaling = new Scaling();
		}

		public Mask(BaseProfile profile)
			: this()
		{
			_profile = profile;
			if (_profile != null)
			{
				//_scaling = new Scaling
				//{
				//	LatticeSize = profile.Scaling.LatticeSize,
				//	CellSize = profile.Scaling.CellSize,
				//	ShowGridLines = profile.Scaling.ShowGridLines,
				//	Zoom = profile.Scaling.Zoom
				//};
				_scaling = (Scaling)profile.Scaling.Clone();
				_profile.ScalingChanged += Profile_ScalingChanged;
			}
				
		}

		public Mask(Mask existingMask, Scaling scaling)
			: this()
		{
			Define(existingMask, scaling);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Clear out the masked area
		/// </summary>
		public void Clear()
		{
			_canvas.Clear();
			_lattice.Clear();
			_scaling.Clear();

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
		public override object Clone()
		{
			Mask MyClone = new Mask(_profile);
			MyClone.SuppressEvents = true;
			MyClone.Define(this, true, Scaling);
			MyClone.SuppressEvents = false;
			return MyClone;
		}

		/// <summary>
		/// Copy the data from the source object to this one.
		/// </summary>
		/// <param name="source">Object to copy from</param>
		public void CopyFrom(Mask source)
		{
			SuppressEvents = true;

			CanvasMask = source.CanvasMask;
			LatticeMask = source.LatticeMask;
			_scaling.CopyFrom(source.Scaling);

			SuppressEvents = false;
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

			if (_profile == null)
				return;

			Size CanvasSize = _profile.Scaling.CanvasSize;

			if (CanvasSize.IsEmpty)
				return;

			_overlay = new Bitmap(CanvasSize.Width, CanvasSize.Height);
			Graphics g = Graphics.FromImage(_overlay);
			g.Clear(Color.Transparent);

			GraphicsPath Path = new GraphicsPath();
			Path.AddRectangle(new Rectangle(0, 0, CanvasSize.Width, CanvasSize.Height));
			if (_canvas.Outline.PointCount > 0)
				Path.AddPath(_canvas.Outline, false);

			using (SolidBrush OverlayBrush = new SolidBrush(Color.FromArgb(64, Color.Red)))
				g.FillPath(OverlayBrush, Path);

			g.Dispose();
			Path.Dispose();
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		public void Define(Mask newMask, Scaling scaling)
		{
			Define(newMask, true, scaling);
		}

		/// <summary>
		/// Sets the mask to be the values passed in.
		/// </summary>
		/// <param name="newMask">Mask Object</param>
		/// <param name="createOverlay">Flag to indicate if the overlay bitmap should be created.</param>
		public void Define(Mask newMask, bool createOverlay, Scaling scaling)
		{
			_canvas.Clear();
			_lattice.Clear();

			if (newMask.HasMask)
			{
				if (newMask.Scaling.Differs(scaling))
					newMask.Scale(scaling);
				_lattice = new MaskData(newMask.LatticeMask);
				_canvas = new MaskData(newMask.CanvasMask);
				_scaling = (Scaling)newMask.Scaling.Clone();

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
		public void Define(GraphicsPath canvasOutline, GraphicsPath latticeOutline, Scaling scaling)
		{
			_canvas.Clear();
			_lattice.Clear();

			if (!_canvas.Outline.Equals(canvasOutline))
				_canvas.Outline = canvasOutline;

			if (!_lattice.Outline.Equals(canvasOutline))
				_lattice.Outline = latticeOutline;

			OnDefined();
			CreateOverlayBitmap();
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
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
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
			if (_overlay != null)
			{
				_overlay.Dispose();
				_overlay = null;
			}
			if (_profile != null)
				_profile.ScalingChanged -= Profile_ScalingChanged;
		}
		
		/// <summary>
		/// Determines whether the data within the specified Mask object matches that of the current Mask object
		/// </summary>
		/// <param name="obj">The Mask object to compare to the current one.</param>
		public bool Differs(Mask other)
		{
			return other.LatticeMask.Differs(LatticeMask) || (other.CanvasMask.Differs(CanvasMask));
		}

		/// <summary>
		/// Indicates whether the point passed in occurrs within Canvas outline
		/// </summary>
		/// <param name="point">Point to check</param>
		/// <returns>Returns true if the point falls within the Canvas outline, false otherwise.</returns>
		public bool HitTest(Point point)
		{
			if (_canvas == null)
				return false;
			else
				return _canvas.HitTest(point);
		}

		/// <summary>
		/// Trims back a Mask that extends beyond the boundary passed in.
		/// </summary>
		private GraphicsPath Intersect(RectangleF boundary, GraphicsPath oldPath)
		{
			// Trim off the part of the Mask that does not fit within the new LatticeSize.
			Polygons Subject = new Polygons();
			Polygons Clips = new Polygons();
			Polygons solution = new Polygons();
			GraphicsPath Path = new GraphicsPath();

			Clipper c = new Clipper();
			int Scale = 1;

			// Create a Graphics path out of the new Lattice Size;
			Path.AddRectangle(boundary);
			Clipper.PathToPolygon(Path, Subject, 1);

			_lattice.Outline.Flatten();
			Clipper.PathToPolygon(oldPath, Clips, Scale);

			Path = new GraphicsPath();
			Path.FillMode = FillMode.Winding;

			c.AddPolygons(Subject, PolyType.ptSubject);
			c.AddPolygons(Clips, PolyType.ptClip);
			solution.Clear();
			bool succeeded = c.Execute(ClipType.ctIntersection, solution);

			if (succeeded)
			{
				PointF[] pts;
				foreach (Polygon pg in solution)
				{
					pts = Clipper.PolygonToPointFArray(pg, Scale);
					if (pts.Length > 2)
						Path.AddPolygon(pts);
					pts = null;
				}
			}
			return Path;
		}

		/// <summary>
		/// Moves the masked area by a given offset amount
		/// </summary>
		/// <param name="offset">Point representing the amount the mask is to move</param>
		/// <param name="scaleMode">Indicates the type of the offset</param>
		public void Move(Point offset, UnitScale scaleMode)
		{
			Point CanvasOffset = Point.Empty;
			Point LatticeOffice = Point.Empty;
			int CellScale = _scaling.CellScale;

			if (scaleMode == UnitScale.Canvas)
			{
				LatticeOffice = offset;
				CanvasOffset = new Point(offset.X * CellScale, offset.Y * CellScale);
			}
			else
			{
				LatticeOffice = new Point(offset.X * (int)(1f / CellScale), offset.Y * (int)(1f / CellScale));
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
		/// <param name="scaleMode">Indicates the scaling of the offset</param>
		public void Move(PointF offset, UnitScale scaleMode)
		{
			PointF CanvasOffset = Point.Empty;
			PointF LatticeOffice = Point.Empty;
			int CellScale = _profile.Scaling.CellScale;

			if (scaleMode == UnitScale.Canvas)
			{
				LatticeOffice = offset;
				CanvasOffset = new PointF(offset.X * CellScale, offset.Y * CellScale);
			}
			else
			{
				LatticeOffice = new PointF(offset.X * (1f / CellScale), offset.Y * (1f / CellScale));
				CanvasOffset = offset;
			}

			_canvas.Move(CanvasOffset);
			_lattice.Move(LatticeOffice);

			CreateOverlayBitmap();
		}

		/// <summary>
		/// Scales the mask data to the size difference represented by the passed in Scaling object.
		/// </summary>
		/// <param name="newScaling">New Scaling data to use for the mask.</param>
		private void Scale(Scaling newScaling)
		{		

			// Look for what changed and scale the MaskData appropriately.
			if (newScaling.LatticeSize.ToString() != _scaling.LatticeSize.ToString())
			{
				RectangleF Bounds = _lattice.GetBounds();
				RectangleF NewSize = new RectangleF(new PointF(0, 0), newScaling.LatticeSize);

				// Determine if the masked data still falls within the new LatticeSize. If so, then ignore this change.
				if (!(NewSize.Contains(Bounds)))
				{
					_lattice.Outline = Intersect(NewSize, _lattice.Outline);

					// Scale up the rectangle to the Canvas size and intersect that
					//NewSize = new RectangleF(new PointF(0, 0), new Size(NewScaling.CanvasSize.Width-1, NewScaling.CanvasSize.Height-1));
					//_canvas.Outline = Intersect(NewSize, _canvas.Outline);
				}
			}

			// Create a new graphics path for the Canvas mask data by scaling the Lattice mask data by the new CellScale
			_canvas.Outline = (GraphicsPath)_lattice.Outline.Clone();
			using (Matrix ScaleMatrix = new Matrix())
			{
				ScaleMatrix.Scale(newScaling.CellScaleF, newScaling.CellScaleF);
				//using (GraphicsPath ScalesPath = new GraphicsPath())
				//{
				_canvas.Outline.Transform(ScaleMatrix);
				//}
			}
			CreateOverlayBitmap();

			// Save the scaling changes to our Scaling object.
			_scaling = new Scaling(newScaling);
		}

		/// <summary>
		/// Instructs the MaskData objects to Deconstitute the Outline object into its components arrays
		/// </summary>
		public void Serialize()
		{
			_canvas.Serialize();
			_lattice.Serialize();
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Handlers ]

		/// <summary>
		/// Occurs when the Mask is Defined
		/// </summary>
		[XmlIgnore()]
		public EventHandler Defined;

		/// <summary>
		/// Occurs when the Mask is removed
		/// </summary>
		[XmlIgnore()]
		public EventHandler Cleared;

		#endregion [ Event Handlers ]

		#region [ Event Delegates ]

		/// <summary>
		/// Occurs when one of the scaling variables within the Profile changes. 
		/// Rescales the Canvas mask.
		/// </summary>
		private void Profile_ScalingChanged(object sender, EventArgs e)
		{
			Scaling NewScaling = new Scaling
			{
				LatticeSize = _profile.Scaling.LatticeSize,
				CellSize = _profile.Scaling.CellSize,
				ShowGridLines = _profile.Scaling.ShowGridLines,
				Zoom = _profile.Scaling.Zoom
			};
			Scale(NewScaling);
		}

		#endregion [ Event Delegates ]

		#region [ Event Triggers ]

		private void OnDefined()
		{
			_serialized = string.Empty;
			if (SuppressEvents)
				return;
			if (Defined != null)
				Defined(this, new EventArgs());
		}

		private void OnCleared()
		{
			_serialized = string.Empty;
			if (SuppressEvents)
				return;
			if (Cleared != null)
				Cleared(this, new EventArgs());
		}

		#endregion [ Event Triggers ]

		#endregion [ Events ]
	}
}
