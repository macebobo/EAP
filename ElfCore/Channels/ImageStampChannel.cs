using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Channels
{
	public class ImageStampChannel : Channel
	{

		#region [ Protected Variables ]

		private Bitmap _imageStamp = null;

		#endregion [ Protected Variables ]

		#region [ Properties ]

		public Size StampSize { get; set; }

		public Bitmap Stamp
		{
			get { return _imageStamp; }
			set
			{
				if (_imageStamp != null)
				{
					if (ReferenceEquals(value, _imageStamp))
						return;
					_imageStamp.Dispose();
					_imageStamp = null;
				}
				if (!ImportBitmap(value, true))
					return;
				_imageStamp = CreateLatticeBuffer(Color.Transparent, value.Size);
				if (_imageStamp == null)
					Debugger.Break();
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ImageStampChannel()
			: base()
		{
			Name = "Image Stamp Channel";
			StampSize = Size.Empty;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Empties the Channel of all Cells.
		/// </summary>
		/// <param name="destroyData">Indicates whether the Lattice array should be destroyed and rebuilt</param>
		public override void Empty(bool destroyData)
		{
			Origin = new Point(0, 0);
			if (destroyData)
				_lattice = new List<LatticePoint>();
			else
				_lattice.Clear();
			StampSize = Size.Empty;
			OnLatticeChanged();
		}

		/// <summary>
		/// Populates cells from the image passed in. 
		/// This is the slower method, but seems more robust
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		/// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
		public override bool ImportBitmap(Bitmap image, bool clearFirst)
		{
			Color CheckPixel;

			if (clearFirst)
				Empty(false);

			if (image == null)
				return false;

			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					CheckPixel = image.GetPixel(x, y);
					if (CheckPixel.GetBrightness() > 0.5f)
						_lattice.Add(new LatticePoint(x, y));
				}

			StampSize = new Size(image.Width, image.Height);

			_latticeChanged = true;
			OnPropertyChanged(Property_Lattice, true);
			return true;
		}

		#endregion [ Methods ]
	}

}

