using ElfCore.Interfaces;
using ElfCore.Util;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ElfCore.PlugIn
{
	#region [ Class PlugInTool ]

	// http://www.c-sharpcorner.com/UploadFile/rmcochran/plug_in_architecture09092007111353AM/plug_in_architecture.aspx
	/// <summary>
	/// This class wraps the PlugIn
	/// </summary>
	[DebuggerDisplay("Name = {Name}")]
	public class PlugInTool : ElfDisposable, IItem
	{
		#region [ Private Variables ]

		private string _name = string.Empty;
		private ITool _tool = null;
		private PlugInToolStripButton _button = null;
		private PlugInToolStripButton _parentButton = null;
		private ToolStrip _toolBox = null;
		private bool _shutdown = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Indicates if this tool has been added.
		/// </summary>
		public bool Added { get; set; }

		/// <summary>
		/// Button that represents the tool
		/// </summary>
		public PlugInToolStripButton Button
		{
			get 
			{
				if (_button == null)
					_button = CreateButton();
				return _button; 
			}
			set
			{
				_button = value;
				_button.PlugInTool = this;
			}
		}

		/// <summary>
		/// Unique ID number
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Index of this object's position within a toolbox.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Indicates whether we are contained within a ToolGroup's collection
		/// </summary>
		public bool InToolGroup
		{
			get { return (Tool.ToolGroupName.Length > 0); }
		}

		/// <summary>
		/// Returns the name of the Tool.
		/// </summary>
		public string Name
		{
			get { return (Tool == null) ? "PlugInTool" : Tool.Name; }
		}

		/// <summary>
		/// If this tool lives on a SubMenu, this is the button that is clicked to show that submenu
		/// </summary>
		public PlugInToolStripButton ParentButton
		{
			get { return _parentButton; }
			set { _parentButton = value; }
		}

		/// <summary>
		/// Returns the name of this object with whitespace removed.
		/// </summary>
		public string SafeName
		{
			get { return Regex.Replace(Name, @"[^\w]", "", RegexOptions.None); }
		}

		/// <summary>
		/// Returns the Settings ToolStrip of the Tool.
		/// </summary>
		public ToolStrip SettingsToolStrip
		{
			get
			{ return Tool.SettingsToolStrip; }
		}

		/// <summary>
		/// Link to the Tool contained herein
		/// </summary>
		public ITool Tool
		{
			get { return _tool; }
			set { _tool = value; }
		}

		/// <summary>
		/// Returns the ToolTip text of the Tool.
		/// </summary>
		public string ToolTipText
		{
			get { return Tool.ToolTipText; }
		}

		/// <summary>
		/// ToolStrip that this Tool appears on
		/// </summary>
		public ToolStrip ToolBox
		{
			get { return _toolBox; }
			set { _toolBox = value; }
		}

		/// <summary>
		/// Returns the ToolBox image of the Tool.
		/// </summary>
		public Bitmap ToolBoxImage
		{
			get { return Tool.ToolBoxImage; }
		}

		/// <summary>
		/// Returns the ToolBox image of the Tool when it is selected.
		/// </summary>
		public Bitmap ToolBoxImageSelected
		{
			get { return Tool.ToolBoxImageSelected; }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public PlugInTool()
		{ }

		public PlugInTool(ITool tool)
			: this()
		{
			Tool = tool;
			ID = tool.ID;
		}

		public PlugInTool(ITool tool, int id)
			: this(tool)
		{
			ID = id;
			tool.ID = id;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private PlugInToolStripButton CreateButton()
		{
			_button = new PlugInToolStripButton()
			{
				CheckOnClick = true,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Name = ToString().Replace(" ", "_").Replace("-", "_") + "_Tool",
				Size = new Size(30, 20),
				Text = ToString(),
				PlugInTool = this,
				ToolTipText = ToolTipText,
				Image = ToolBoxImage,
				MultiGestureKey1 = _tool.MultiGestureKey1,
				MultiGestureKey2 = _tool.MultiGestureKey2
			};
			return _button;
		}

		/// <summary>
		/// Clean up all child objects here, unlink all events and dispose
		/// </summary>
		protected override void DisposeChildObjects()
		{
			base.DisposeChildObjects();

			if (_tool != null)
			{
				_tool.Dispose();
				_tool = null;
			}
			_button = null;
			_parentButton = null;
			_toolBox = null;
		}
		
		/// <summary>
		/// Determines the location of a ToolStripItem, since we cannot determine the location from a property on the control.
		/// </summary>
		/// <param name="button">ToolStripItem to examine</param>
		private Point GetLocationOfTool(ToolStripItem button, int offsetX, int offsetY)
		{
			ToolStrip Parent = _toolBox;
			Point Location = Parent.Location;
			int Padding = Parent.Padding.Top + Parent.Padding.Bottom;
			int Y = Location.Y + Parent.Padding.Top;
			foreach (ToolStripItem Item in Parent.Items)
			{
				if (Item == button)
					break;
				Y += Item.Size.Height + Item.Margin.Top + Item.Margin.Bottom;
			}

			return new Point(Location.X + Parent.Padding.Left + offsetX, Y + offsetY);
		}

		public void Initialize()
		{
			Tool.Initialize();
		}

		/// <summary>
		/// Remove the Settings Toolstrip from its parent control and instructs the Tool to ShutDown;
		/// </summary>
		public void ShutDown()
		{
			if (_shutdown)
				return;

			_shutdown = true;
			if ((SettingsToolStrip != null) && (SettingsToolStrip.Parent != null))
				SettingsToolStrip.Parent.Controls.Remove(SettingsToolStrip);

			if (_tool != null)
				_tool.ShutDown();
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion [ Methods ]
	}

	#endregion [ Class PlugInTool ]

	#region [ Class PlugInToolList ]

	public class PlugInToolList : CollectionBase, IList<PlugInTool>, ICollection<PlugInTool>
	{
		#region [ Properties ]

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public PlugInTool this[int index]
		{
			get 
			{
				if (index >= List.Count)
					return null;
				return (PlugInTool)List[index]; 
			}
			set { List[index] = value; }
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public PlugInToolList()
		{ }

		#endregion [ Constructors ]

		#region [ Methods ]

		public void Add(PlugInTool item)
		{
			List.Add(item);
		}

		public bool Contains(PlugInTool item)
		{
			return List.Contains(item);
		}

		public void CopyTo(PlugInTool[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int IndexOf(PlugInTool item)
		{
			if (!List.Contains(item))
				return -1;
			return List.IndexOf(item);
		}

		public void Insert(int index, PlugInTool item)
		{
			List.Insert(index, item);
		}

		public void Remove(PlugInTool item)
		{
			List.Remove(item);
		}

		/// <summary>
		/// Sorts the List objects by ID
		/// </summary>
		public void Sort()
		{
			SortedList<int, PlugInTool> Sorted = new SortedList<int, PlugInTool>();
			foreach (PlugInTool p in this)
				Sorted.Add(p.ID, p);
			Clear();
			foreach (KeyValuePair<int, PlugInTool> KVP in Sorted)
				Add(KVP.Value);
			Sorted = null;
		}

		bool ICollection<PlugInTool>.Remove(PlugInTool item)
		{
			if (!List.Contains(item))
				return false;
			List.Remove(item);
			return true;
		}

		public PlugInTool Where(int id)
		{
			foreach (PlugInTool item in List)
				if (item.ID == id)
					return item;
			return null;
		}

		public PlugInTool Where(bool inGroup, int id)
		{
			foreach (PlugInTool item in List)
				if ((item.ID == id) &&
					(item.InToolGroup == inGroup))
					return item;
			return null;
		}

		public PlugInTool Where(bool inGroup)
		{
			foreach (PlugInTool item in List)
				if (item.InToolGroup == inGroup)
					return item;
			return null;
		}

		public List<PlugInTool> WhereList(bool inGroup)
		{
			List<PlugInTool> ReturnList = new List<PlugInTool>();
			foreach (PlugInTool item in List)
				if (item.InToolGroup == inGroup)
					ReturnList.Add(item);
			return ReturnList;
		}

		public PlugInTool Where(string toolGroupName)
		{
			foreach (PlugInTool item in List)
				if (string.Compare(item.Tool.ToolGroupName, toolGroupName, true) == 0)
					return item;
			return null;
		}

		public List<PlugInTool> WhereList(string toolGroupName)
		{
			List<PlugInTool> ReturnList = new List<PlugInTool>();
			foreach (PlugInTool item in List)
				if (string.Compare(item.Tool.ToolGroupName, toolGroupName, true) == 0)
					ReturnList.Add(item);
			return ReturnList;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		IEnumerator<PlugInTool> IEnumerable<PlugInTool>.GetEnumerator()
		{
			return new PlugInToolListEnumerator(List.GetEnumerator());
		}

		#endregion [ Methods ]

		#region [ Class PlugInToolListEnumerator ]

		private class PlugInToolListEnumerator : IEnumerator<PlugInTool>
		{
			#region [ Private Variables ]

			private IEnumerator _enumerator;

			#endregion [ Private Variables ]

			#region [ Properties ]

			public PlugInTool Current
			{
				get { return (PlugInTool)_enumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}

			#endregion [ Properties ]

			#region [ Constructors ]

			public PlugInToolListEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			#endregion [ Constructors ]

			#region [ Methods ]

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{ }

			#endregion [ Methods ]
		}

		#endregion [ Class PlugInToolListEnumerator ]
	}

	#endregion [ Class PlugInToolList ]
}


