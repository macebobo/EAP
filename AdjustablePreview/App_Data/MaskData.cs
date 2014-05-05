using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ElfCore
{
	[Serializable]
	public class MaskData : ICloneable, IDisposable
	{
		#region [ Private Variables ]

		private PointF[] _outlinePoints;
		private byte[] _outlineBytes;

		[NonSerialized()]
		private bool _disposed = false;

		private Workshop _workshop = Workshop.Instance;

		#endregion [ Private Variables ]

		#region [ Fields ]
		
		//[field: NonSerialized()]
		//public Region Region = null;

		[field: NonSerialized()]
		public GraphicsPath Outline = null;

		#endregion [ Fields ]

		#region [ Properties ]

		/// <summary>
		/// True if the Outline has points
		/// </summary>
		public bool HasData
		{
			get
			{
				if (Outline == null)
					return false;
				else
					return (Outline.PointCount > 0);
			}
		}
				
		#endregion [ Properties ]

		#region [ Constructors ]

		public MaskData()
		{
			if (_disposed)
				GC.ReRegisterForFinalize(true);
			_disposed = false;

			//Clear();
			Outline = new GraphicsPath();
		}

		public MaskData(GraphicsPath outline)
			: this()
		{
			if (outline != null)
				this.Outline = (GraphicsPath)outline.Clone();
		}

		//public MaskData(GraphicsPath outline, Region region)
		//{
		//    this.Outline = (GraphicsPath)outline.Clone();
		//    //this.Region = (Region)region.Clone();
		//}

		public MaskData(MaskData data)
			: this()
		{
			if (data.Outline != null)
				this.Outline = (GraphicsPath)data.Outline.Clone();
		}

		#endregion [ Constructors ]

		#region [ Destructors ]

		~MaskData()
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
				_outlinePoints = null;
				_outlineBytes = null;
				if (Outline != null)
				{
					Outline.Dispose();
					Outline = null;
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
		/// Clear out the data in our objects and reinitialize them.
		/// </summary>
		public void Clear()
		{
			//if (Outline != null)
			//{
			//    Outline.Dispose();
			//    Outline = null;
			//}
			//Outline = new GraphicsPath();
			Outline.Reset();
		}

		/// <summary>
		/// Create a deep clone of this object.
		/// </summary>
		public object Clone()
		{
			//return new MaskData(this.Outline, this.Region);
			return new MaskData(this.Outline);
		}

		/// <summary>
		/// Rebuild the Outline object from the arrays _outlinePoints and _outlineBytes
		/// </summary>
		public void Deserialize()
		{
			Outline = new GraphicsPath(_outlinePoints, _outlineBytes);
		}
		
		/// <summary>
		/// Determines whether the data within the specified MaskData object matches that of the current MaskData object
		/// </summary>
		/// <param name="other">The MaskData object to compare to the current one.</param>
		public bool Differs(MaskData other)
		{
			// If one has data, but not the other, then they differ
			if (this.HasData != other.HasData)
				return true;

			// If neither has data, then they do not differ
			if (!this.HasData && !other.HasData)
				return false;

			// At this point, but much have data, check to see if the arrays the compose the GraphicsPath differ.
			return (!(Workshop.ArraysEqual<PointF>(this.Outline.PathPoints, other.Outline.PathPoints) &&
					  Workshop.ArraysEqual<byte>(this.Outline.PathTypes, other.Outline.PathTypes)));
		}

		public override int GetHashCode()
		{
			return Outline.GetHashCode();
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
			Outline.Transform(MoveMatrix);
			MoveMatrix.Dispose();
			MoveMatrix = null;
		}

		/// <summary>
		/// Populate the arrays _outlinePoints and _outlineBytes from values within the Outline object
		/// </summary>
		public void Serialize()
		{
			if ((Outline != null) && (Outline.PointCount > 0))
			{
				_outlinePoints = Outline.PathPoints;
				_outlineBytes = Outline.PathTypes;
			}
			else
			{
				_outlinePoints = null;
				_outlineBytes = null;
			}
		}

		///// <summary>
		///// Generate the region from the outline
		///// </summary>
		//public void RegionFromOutline()
		//{
		//    this.Region = new Region(this.Outline);
		//}

		/// <summary>
		/// http://social.msdn.microsoft.com/forums/en-US/winforms/thread/bca98504-9ddf-4fa4-acfe-5fa94115cc62/
		/// Info on serializing a Graphics Path
		/// </summary>

		#endregion [ Methods ] 
	}

	/*
	 * // found this interesting, using ComponentModel to create events for properties changing
	 * 
	public class Loan : System.ComponentModel.INotifyPropertyChanged
	{
		public double LoanAmount { get; set; }
		public double InterestRate { get; set; }
		public int Term { get; set; }

		private string p_Customer;
		public string Customer
		{
			get { return p_Customer; }
			set
			{
				p_Customer = value;
				PropertyChanged(this,
				  new System.ComponentModel.PropertyChangedEventArgs("Customer"));
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public Loan(double loanAmount,
					double interestRate,
					int term,
					string customer)
		{
			this.LoanAmount = loanAmount;
			this.InterestRate = interestRate;
			this.Term = term;
			p_Customer = customer;
		}
	}
	*/
}
