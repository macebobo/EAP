using System.Windows.Forms;

using ElfCore.Controllers;

namespace ElfCore.Util
{
	/// <summary>
	/// http://stackoverflow.com/questions/11157868/capturing-ctrl-multiple-key-downs
	/// </summary>
	public class MultiKeyGesture 
	{
		#region [ Private Variables ]

		private string _id;
		private ToolStripItem _menuItem;
		private string _gesture;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public string ID
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Gesture
		{
			get { return _gesture; }
			set
			{
				_gesture = value;
			}
		}

		public ToolStripItem MenuItem
		{
			get { return _menuItem; }
			set { _menuItem = value; }
		}
		#endregion [ Properties ]

		#region [ Constructors ]

		public MultiKeyGesture()
		{
			_menuItem = null;
			_id = string.Empty;
			_gesture = string.Empty;
		}

		public MultiKeyGesture(string id, string gesture, ToolStripItem menuItem) : this()
		{
			_id = id;
			_menuItem = menuItem;
			Gesture = gesture;
		}

		public MultiKeyGesture(string id, Keys key1, Keys key2, ToolStripItem menuItem)
			: this()
		{
			_id = id;
			_menuItem = menuItem;
			_gesture = KeyboardController.KeysToString(key1);
			if (key2 != Keys.None)
				_gesture += "," + KeyboardController.KeysToString(key2);
		}
		#endregion [ Constructors ]

		#region [ Methods ]


		public override string ToString()
		{
			return _gesture;
		}

		#endregion [ Methods ]
	}

}

