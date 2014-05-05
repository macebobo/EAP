using System.Drawing;

namespace ElfCore.Channels.Vixen
{
	public class Cell
	{

		#region [ Private Variables ]

		private Point _cell = Point.Empty; 

		#endregion [ Private Variables ]

		#region [ Properties ]

		public bool IsEmpty
		{
			get { return _cell.IsEmpty; }
		}

		public Point Point
		{
			get { return _cell; }
			set { _cell = value; }
		}

		public int X
		{
			get { return _cell.X; }
			set { _cell.X = value; }
		}

		public int Y
		{
			get { return _cell.Y; }
			set { _cell.Y = value; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		/// <summary>        
		/// The default Constructor.        
		/// </summary>        
		public Cell()
		{ }

		/// <summary>        
		/// Constructor passing in the X, Y coordinates
		/// </summary>        
		public Cell(int x, int y) : this()
		{
			_cell = new Point(x, y);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void Empty()
		{
			_cell = Point.Empty;
		}

		public void Offset(Point p)
		{
			_cell.Offset(p);
		}

		public override string ToString()
		{
			return _cell.ToString();
		}

		#endregion [ Methods ]

		#region [ Events ]

		#endregion [ Events ]
	}
}
