using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace ElfCore.Interfaces
{
	/// <summary>
	/// Interface for all Channel objects. Does not contain methods or properties on manipulating the data. Channels will need to additionally implement the appropriate interface.
	/// </summary>
	public interface IChannel : IDisposable
	{		

		#region [ Properties ]
		
		/// <summary>
		/// Color to use to draw the Channel object.
		/// </summary>
		Color BorderColor { get; set; }

		/// <summary>
		/// Base color used to render the Channel.
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Key used to find the image that represents this Channel in the Channel Explorer.
		/// </summary>
		string ChannelExplorerImageKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Channel is enabled.
		/// </summary>
		bool Enabled{ get; set; }

		/// <summary>
		/// Indicates whether this Channel is grouped with other Channels.
		/// </summary>
		bool Grouped { get; }
		
		/// <summary>
		/// Generated Unique identifier for this Channel.
		/// </summary>
		string GUID { get; }

		/// <summary>
		/// Indicates whether there is data defined for this Channel.
		/// </summary>
		bool HasData { get; }

		/// <summary>
		/// Unique Output ID for this Channel.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// Position in the current sort order.
		/// </summary>
		int Index { get; set; }

		/// <summary>
		/// Indicates whether this Channel is hidden and does not render.
		/// </summary>
		bool IsHidden { get; }

		/// <summary>
		/// Indicates whether this Channel is selected in the Channel Explorer.
		/// </summary>
		bool IsSelected { get; set; }

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

		/// <summary>
		/// Indicates whether this Channel is Locked. If so, it is immune to any form of edit.
		/// </summary>
		bool Locked { get; set; }

		/// <summary>
		/// Name of the Channel.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Point offset from where the Channel should be drawn from
		/// </summary>
		Point Origin { get; set; }

		/// <summary>
		/// Profile that owns this Channel.
		/// </summary>
		IProfile Profile { get; set; }

		/// <summary>
		/// Indicates whether this Channel can be rendered.
		/// </summary>
		bool Visible { get; set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Used by Clipboard to Cut cells
		/// </summary>
		/// <param name="cells">List of cells to be removed from the Lattice.</param>
		void CutData(List<Point> cells);

		/// <summary>
		/// Cleans up the Lattice, removing duplicate Cells.
		/// </summary>
		void DedupeData();
		
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
		/// Determines whether the data within the specified Channel object matches that of the current Channel object. Channel cannot be null.
		/// </summary>
		/// <param name="channel">Channel object to compare to the current one.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		bool Differs(Channels.BaseChannel channel);

		/// <summary>
		/// Generate a bitmap based on the data
		/// </summary>
		/// <param name="paintColor">Color in which to paint the data</param>
		/// <param name="backgroundColor">Background Color on which to draw the data.</param>
		Bitmap DrawChannel(Color paintColor, Color backgroundColor);

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
		/// Erase a specific Cell from the Lattice.
		/// </summary>
		/// <param name="cell">Cell to erase.</param>
		void Erase(Point cell);

		/// <summary>
		/// Returns a rectangle that bounds this Channel.
		/// </summary>
		Rectangle GetBounds();

		/// <summary>
		/// Creates a graphics path that represents all the data for this Channel.
		/// </summary>
		GraphicsPath GetGraphicsPath();

		/// <summary>
		/// Turns on the Hidden flag to suppress redrawing of this Channel. Use the method <seealso cref="Show">Show()</seealso> to allow normal drawing.
		/// </summary>
		void Hide();

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

		/// <summary>
		/// Saves the data to the Profile. If SaveMode is set to ChannelObjects, then saves basic info like name, color, etc 
		/// under the ChannelObjects node in the Profile. Otherwise, saves the display data under the PlugIn node
		/// </summary>
		/// <param name="parentNode">XmlNode to save the data under</param>
		/// <param name="saveMode">Mode of Saving</param>
		void Save(XmlNode parentNode, Channels.BaseChannel.SaveMode saveMode);

		/// <summary>
		/// Converts the data array values to a string to be stored in the Profile Xml.
		/// </summary>
		string SerializeLattice();

		/// <summary>
		/// Turns off the Hidden flag to allow normal redrawing of this Channel.
		/// </summary>
		void Show();

		/// <summary>
		/// Returns the ID and Name of the Channel.
		/// </summary>
		/// <param name="showID">Indicates whether the ID should be included in the output</param>
		string ToString(bool showID);

		#endregion [ Methods ]

	}
}
