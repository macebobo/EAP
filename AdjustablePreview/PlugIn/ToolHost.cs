using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ElfRes = ElfCore.Properties.Resources;
using System.Text.RegularExpressions;

namespace ElfCore.PlugIn
{
	// http://www.c-sharpcorner.com/UploadFile/rmcochran/plug_in_architecture09092007111353AM/plug_in_architecture.aspx
	/// <summary>
	/// This class wraps the PlugIn
	/// </summary>
	public class ToolHost
	{
		#region [ Private Variables ]

		private string _name = string.Empty;
		private IToolGroup _toolGroup = null;
		private ITool _tool = null;
		private PlugInToolStripButton _button = null;
		private PlugInToolStripButton _parentButton = null;
		private ToolStrip _toolBox = null;
		private ToolStrip _childToolBox = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

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
				_button.ToolHost = this;
			}
		}

		/// <summary>
		/// Unique ID number
		/// </summary>
		public int ID { get; set; }

		public int Index { get; set; }

		public bool InToolGroup
		{
			get
			{
				if (IsToolGroup)
					return false;
				else
					return (Tool.ToolGroupName.Length > 0);
			}
		}

		/// <summary>
		/// Returns true if this ToolHost hosts a ToolGroup instead of a Tool
		/// </summary>
		public bool IsToolGroup
		{
			get { return ToolGroup != null; }
		}

		public string Name
		{
			get
			{
				if (Tool != null)
					return Tool.Name;

				else if (ToolGroup != null)
					return ToolGroup.Name;

				else
					return this.ToString();
			}
		}

		/// <summary>
		/// If this tool lives on a SubMenu, this is the button that is clicked to show that submenu
		/// </summary>
		public PlugInToolStripButton ParentButton
		{
			get { return _parentButton; }
			set { _parentButton = value; }
		}

		public string SafeName
		{
			get
			{
				return Regex.Replace(this.Name, @"[^\w]", "", RegexOptions.None);
			}
		}

		public ToolStrip SettingsToolStrip
		{
			get
			{
				if (!IsToolGroup)
					return Tool.SettingsToolStrip;
				else
					return null;
			}
		}

		public ITool Tool 
		{ 
			get
			{
				if (!IsToolGroup)
					return _tool;
				else
					return ToolGroup.CurrentTool.Tool;
			}
			set
			{
				if (!IsToolGroup)
					_tool = value;
			}
		}

		public IToolGroup ToolGroup
		{
			get { return _toolGroup; }
			set
			{
				_toolGroup = value;
				_toolGroup.SelectedIndexChanged += new System.EventHandler(this.ToolGroup_SelectedIndexChanged);
			}
		}

		public string ToolTipText
		{
			get
			{
				if (Tool != null)
					return Tool.ToolTipText;

				else if (ToolGroup != null)
					return ToolGroup.ToolTipText;

				else
					return base.ToString();
			}
		}

		/// <summary>
		/// ToolStrip that this Tool appears on
		/// </summary>
		public ToolStrip ToolBox
		{
			get { return _toolBox; }
			set { _toolBox = value; }
		}

		public Bitmap ToolBoxImage
		{
			get
			{
				if (Tool != null)
					return Tool.ToolBoxImage;

				else if (ToolGroup != null)
					return ToolGroup.ToolBoxImage;

				else
					return ElfRes.not;
			}
		}

		/// <summary>
		/// ToolStrip that exists if this is a ToolGroup
		/// </summary>
		public ToolStrip ChildToolBox
		{
			set { _childToolBox = value; }
			get 
			{
				if (_childToolBox == null)
				{
					// Create the child toolBox
					_childToolBox = new ToolStrip();
					_childToolBox.Dock = System.Windows.Forms.DockStyle.None;
					_childToolBox.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
					_childToolBox.Name = "SubMenu_" + this.SafeName;
					_childToolBox.Padding = new System.Windows.Forms.Padding(3);
					_childToolBox.Size = new System.Drawing.Size(121, 29);
					_childToolBox.Location = GetLocationOfTool(_button, _toolBox.Width, 0);
				}
				return _childToolBox; 
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public ToolHost()
		{  }

		public ToolHost(ITool tool)
			: this()
		{
			Tool = tool;
		}

		public ToolHost(IToolGroup toolGroup, int id)
			: this()
		{
			ToolGroup = toolGroup;
			this.ID = id;
		}

		public ToolHost(ITool tool, int id)
			: this(tool)
		{
			this.ID = id;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public void Add(ToolHost childToolHost)
		{
			if (!IsToolGroup)
				return;

			this.ToolGroup.Add(childToolHost);
			childToolHost.ParentButton = this.Button;
			childToolHost.ToolBox = this.ChildToolBox;
			this.ChildToolBox.Items.Add(childToolHost.Button);

			if (this.ToolGroup.ChildTools.Count == 1)
				this.ToolGroup.SelectedIndex = 0;
		}

		private PlugInToolStripButton CreateButton()
		{
			_button = new PlugInToolStripButton();

			_button.CheckOnClick = true;
			_button.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_button.Name = this.ToString() + "_Tool";
			_button.Size = new System.Drawing.Size(30, 20);
			_button.Text = this.ToString();
			_button.ToolHost = this;
			_button.ToolTipText = this.ToolTipText;
			_button.Image = this.ToolBoxImage;

			return _button;
		}

		/// <summary>
		/// Combines a tool image with the flyout image.
		/// </summary>
		private Bitmap Flyout(Bitmap source)
		{
			if (source == null)
				return null;
			Bitmap FlyoutImage = ElfRes.flyout;
			if (FlyoutImage != null)
			{
				Graphics g = Graphics.FromImage(source);
				g.DrawImage(FlyoutImage, new Rectangle(0, 0, 16, 16));
				g.Dispose();
			}
			return source;
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

		public void Initialize(/*Settings settings, Workshop workshop*/)
		{
			if (!IsToolGroup)
				Tool.Initialize(/*settings, workshop*/);
		}

		public void ShutDown()
		{
			if (Tool != null)
				Tool.ShutDown();
			if (ToolGroup != null)
				ToolGroup.ShutDown();
		}

		public override string ToString()
		{

			return this.Name;

			//if (_tool != null)
			//    return _tool.Name;
			//else if (_toolGroup != null)
			//    return _toolGroup.Name;
			//else
			//    return base.ToString();
		}

		#endregion [ Methods ]

		#region [ Events ]

		private void ToolGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			ToolHost ChildToolHost = this.ToolGroup[this.ToolGroup.SelectedIndex];
			this.Button.Image = Flyout(ChildToolHost.ToolBoxImage);
			this.Button.ToolTipText = ChildToolHost.ToolTipText;
			this.ChildToolBox.Visible = false;
		}

		#endregion [ Events ]
	}
}


