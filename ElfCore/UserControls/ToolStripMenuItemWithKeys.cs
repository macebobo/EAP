using System.Windows.Forms;
using System.ComponentModel;

namespace ElfCore
{
	public class ToolStripMenuItemWithKeys : ToolStripMenuItem
	{
		private Keys _multiGestureKey1 = Keys.None;
		private Keys _multiGestureKey2 = Keys.None;
		private string _pathNode = string.Empty;

		/// <summary>
		/// First key in the multi-gesture keystroke
		/// </summary>
		[Description("Name of the Position in the path for this menu item when generating the list of multi-gesture keys.")]
		public string PathNode
		{
			get 
			{
				if (_pathNode.Length == 0)
				{
					string Node = Text;
					Node = Node.Replace(" ", string.Empty).Replace(".", string.Empty);
					Node = Node.Replace("&", string.Empty).Replace(",", string.Empty);
					return Node;
				}
				return _pathNode;
			}
			set { _pathNode = value; }
		}

		/// <summary>
		/// First key in the multi-gesture keystroke
		/// </summary>
		[Localizable(true),DefaultValue(typeof(Keys), "None"), Description("First key in the multi-gesture keystroke")]
		public Keys MultiGestureKey1 
		{
			get { return _multiGestureKey1; }
			set
			{
				_multiGestureKey1 = value;
				SetShortcutText();
			}
		}

		/// <summary>
		/// Second key in the multi-gesture keystroke
		/// </summary>
		[Localizable(true),DefaultValue(typeof(Keys), "None"), Description("Second key in the multi-gesture keystroke")]
		public Keys MultiGestureKey2
		{
			get { return _multiGestureKey2; }
			set
			{
				_multiGestureKey2 = value;
				SetShortcutText();
			}
		}

		/// <summary>
		/// Sets the ShortcutText of the menu item based on the human readable version of the MultiGestureKey properties.
		/// </summary>
		private void SetShortcutText()
		{
			if (_multiGestureKey1 == Keys.None)
			{
				base.ShortcutKeyDisplayString = string.Empty;
				return;
			}

			string Return = string.Empty;

			KeysConverter Conv = new KeysConverter();
			Return = Conv.ConvertToString(_multiGestureKey1);
			if (_multiGestureKey2 != Keys.None)
				Return += "," + Conv.ConvertToString(_multiGestureKey2);
			Conv = null;

			base.ShortcutKeyDisplayString = Return;
		}
	}
}

