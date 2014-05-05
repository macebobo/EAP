using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ElfCore.Interfaces
{
	/// <summary>
	/// Interface for Channel Cell handling classes
	/// </summary>
	public interface ICellChannel : IDisposable
	{		

		#region [ Properties ]

		/// <summary>
		/// Indicates whether there is data defined for this Channel.
		/// </summary>
		bool HasData { get; }

		/// <summary>
		/// List of Cells points that represent the pseudo-bitmap mode of rendering.
		/// </summary>
		List<Point> Lattice { get; set; }

		/// <summary>
		/// Bitmap object of the Lattice Buffer for this Channel
		/// </summary>
		Bitmap LatticeBuffer { get; set; }

		/// <summary>
		/// Sets an indicator to this channel that it needs to refresh its GraphicsPath.
		/// </summary>
		bool LatticeChanged { set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Empties the Channel of all Cells.
		/// </summary>
		void Empty();

		/// <summary>
		/// Empties the Channel of all Cells.
		/// </summary>
		/// <param name="destroyData">Indicates whether the Lattice should be destroyed and rebuilt</param>
		void Empty(bool destroyData);

		/// <summary>
		/// Used by Clipboard to Cut cells
		/// </summary>
		/// <param name="cells">List of cells to be removed from the Lattice.</param>
		void CutData(List<Point> cells);

		/// <summary>
		/// Generate a bitmap based on the data
		/// </summary>
		/// <param name="paintColor">Color in which to paint the data</param>
		/// <param name="backgroundColor">Background Color on which to draw the data.</param>
		Bitmap DrawChannel(Color paintColor, Color backgroundColor);

		/// <summary>
		/// Converts the string of encoded Cell data into the Lattice
		/// </summary>
		/// <param name="encoded">String of encoded Cell data.</param>
		void DeserializeLattice(string encoded);

		/// <summary>
		/// Converts the string of encoded Cell data into the Lattice. 
		/// </summary>
		/// <param name="encoded">String of encoded data.</param>
		/// <param name="clear">Indicates whether the existing Cells should be removed.</param>
		void DeserializeLattice(string encoded, bool clear);
		
		/// <summary>
		/// Cleans up the Lattice, removing duplicate Cells.
		/// </summary>
		void DedupeData();

		/// <summary>
		/// Converts the data array values to a string to be stored in the Profile Xml.
		/// </summary>
		string SerializeLattice();

		/// <summary>
		/// Erase a specific Cell from the Lattice.
		/// </summary>
		/// <param name="cell">Cell to erase.</param>
		void Erase(Point cell);

		/// <summary>
		/// Determines whether the data within the specified Channel object matches that of the current Channel object. Channel cannot be null.
		/// </summary>
		/// <param name="channel">Channel object to compare to the current one.</param>
		/// <exception cref="System.ArgumentNullException">channel cannot be null.</exception>
		bool Differs(IChannel channel);

		/// <summary>
		/// Returns a rectangle that bounds this Channel.
		/// </summary>
		Rectangle GetBounds();

		/// <summary>
		/// Creates a graphics path that represents all the data for this Channel.
		/// </summary>
		GraphicsPath GetGraphicsPath();

		/// <summary>
		/// Populates cells from the image passed in. 
		/// This is the slower method, but seems more robust
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		void ImportBitmap(Bitmap image);

		/// <summary>
		/// Populates cells from the image passed in. 
		/// This is the slower method, but seems more robust
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
		/// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
		void ImportBitmap(Bitmap image, bool clearFirst);

		/// <summary>
		/// Adds a Cell to the Lattice, then triggers an event to cause the Channel to be redrawn.
		/// </summary>
		/// <param name="cell">Cell to add.</param>
		void Paint(Point cell);

		/// <summary>
		/// Adds a list of Cells to add to the Lattice, then triggers an event to cause the Channel to be redrawn.
		/// </summary>
		/// <param name="collection">List of data values to add to the array.</param>
		/// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
		void Paint(List<Point> collection);

		/// <summary>
		/// Adds a list of Cells to add to the Lattice, offset by the given amount, then triggers an event to cause the Channel to be redrawn.
		/// </summary>
		/// <param name="collection">List of data values to add to the array.</param>
		/// <param name="offset">Offset point to start the painting.</param>
		void Paint(List<Point> collection, Point offset);

		/// <summary>
		/// Populates data values from the bitmap passed in. 
		/// This is the faster method, but doesn't seem to work all that well with non-native bitmapped images.
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the Cells in the Lattice</param>
		void PopulateFromLatticeBuffer(Bitmap image);

		/// <summary>
		/// Populates cells from the lattice buffer passed in.
		/// This is the faster method, but doesn't seem to work all that well with non-native bitmapped images.
		/// </summary>
		/// <param name="image">1-bit bitmap that represents the data values in the array</param>
		/// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
		void PopulateFromLatticeBuffer(Bitmap image, bool clearFirst);

		#endregion [ Methods ]

	}
}
