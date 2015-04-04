using ElfCore.Controllers;
using ElfCore.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace ElfCore.Core
{
	[Serializable]
	public class MaskData : ElfBase
	{
		#region [ Private Variables ]

		private PointF[] _outlinePoints;
		private byte[] _outlineBytes;
		private Region _hitRegion;
		private GraphicsPath _outline = null;
		
		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// True if the Outline has points
		/// </summary>
		[XmlIgnore]
		public bool HasData
		{
			get
			{
				if (_outline == null)
					return false;
				else
					return (_outline.PointCount > 0);
			}
		}

		[XmlIgnore]
		public GraphicsPath Outline
		{
			get { return _outline; }
			set
			{
				if (value == null)
				{
					_outline = null;
					_hitRegion = null;
				}
				else
				{
					_outline = value;
					_hitRegion = new Region(_outline);
				}
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlElement("Outline")]
		public string OutlineSerialized
		{
			get { return XmlGraphicsPath.FromBaseType(_outline); }
			set { Outline = XmlGraphicsPath.ToBaseType(value); }
		}

		/// <summary>
		/// Pre-serialized version of this object.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XmlIgnore]
		public override string Serialized
		{
			get
			{
				if (_serialized.Length == 0)
					_serialized = Extends.SerializeObjectToXml<MaskData>(this);
				return base.Serialized;
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public MaskData()
			: base()
		{
			_outline = new GraphicsPath();
			_hitRegion = new Region(_outline);
		}

		public MaskData(GraphicsPath outline)
			: this()
		{
			if (outline != null)
				Outline = (GraphicsPath)outline.Clone();
		}

		public MaskData(MaskData data)
			: this()
		{
			if (data.Outline != null)
				Outline = (GraphicsPath)data.Outline.Clone();
		}

		#endregion [ Constructors ]

		#region [ Methods ]
		
		/// <summary>
		/// Clear out the data in our objects and reinitialize them.
		/// </summary>
		public void Clear()
		{
			if (_outline != null)
				_outline.Reset();
		}

		/// <summary>
		/// Create a deep clone of this object.
		/// </summary>
		public override object Clone()
		{
			return new MaskData(_outline);
		}

		/// <summary>
		/// Rebuild the Outline object from the arrays _outlinePoints and _outlineBytes
		/// </summary>
		public void Deserialize()
		{
			Outline = new GraphicsPath(_outlinePoints, _outlineBytes);
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();

			if (_outline != null)
			{
				_outline.Dispose();
				_outline = null;
			}
			_outlinePoints = null;
			_outlineBytes = null;
		}

		/// <summary>
		/// Determines whether the data within the specified MaskData object matches that of the current MaskData object
		/// </summary>
		/// <param name="other">The MaskData object to compare to the current one.</param>
		public bool Differs(MaskData other)
		{
			// If one has data, but not the other, then they differ
			if (HasData != other.HasData)
				return true;

			// If neither has data, then they do not differ
			if (!HasData && !other.HasData)
				return false;

			// At this point, but much have data, check to see if the arrays the compose the GraphicsPath differ.
			return (!(Workshop.ArraysEqual<PointF>(_outline.PathPoints, other.Outline.PathPoints) &&
					  Workshop.ArraysEqual<byte>(_outline.PathTypes, other.Outline.PathTypes)));
		}

		/// <summary>
		/// Returns a rectangle that bounds this MaskData.
		/// </summary>
		/// <returns>A System.Drawing.RectangleF that represents a rectangle that bounds this MaskData.</returns>
		public RectangleF GetBounds()
		{
			return _outline.GetBounds();
		}

		/// <summary>
		/// Indicates whether the point passed in occurrs within the Outline.
		/// </summary>
		/// <param name="point">Point to check</param>
		/// <returns>Returns true if the point falls within the Outline, false otherwise.</returns>
		public bool HitTest(Point point)
		{
			if (_hitRegion == null)
				return false;
			else
				return _hitRegion.IsVisible(point);
		}

		/// <summary>
		/// Moves the masked area by a given offset amount
		/// </summary>
		/// <param name="offset">Point representing the amount the mask is to move</param>
		public void Move(Point offset)
		{
			Move((PointF)offset);
		}

		/// <summary>
		/// Moves the masked area by a given offset amount
		/// </summary>
		/// <param name="offset">Point representing the amount the mask is to move</param>
		public void Move(PointF offset)
		{
			Matrix MoveMatrix = new Matrix();
			MoveMatrix.Translate(offset.X, offset.Y);
			_outline.Transform(MoveMatrix);
			_hitRegion = new Region(_outline);
			MoveMatrix.Dispose();
			MoveMatrix = null;
		}

		/// <summary>
		/// Populate the arrays _outlinePoints and _outlineBytes from values within the Outline object
		/// </summary>
		public void Serialize()
		{
			if ((_outline != null) && (_outline.PointCount > 0))
			{
				_outlinePoints = _outline.PathPoints;
				_outlineBytes = _outline.PathTypes;
			}
			else
			{
				_outlinePoints = null;
				_outlineBytes = null;
			}
		}

		#endregion [ Methods ] 
	}
}
