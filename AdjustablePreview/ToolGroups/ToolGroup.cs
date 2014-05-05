using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	/// <summary>
	/// Base level class all tool groups will inherit
	/// </summary>
	public class BaseToolGroup
	{
		#region [ Events ]
		
		public event EventHandler SelectedIndexChanged;

		#endregion [ Events ]

		#region [ Protected Variables ]

		protected KeyChord _keyboardShortCut = new KeyChord();
		protected List<PlugIn.ToolHost> _childTools = new List<PlugIn.ToolHost>();
		protected int _selectedIndex = -1;
		
		#endregion [ Protected Variables ]

		#region [ Properties ]

		public virtual int ID { get; set; }

		public virtual string ToolTipText
		{
			get { return string.Empty; }
		}

		public virtual KeyChord KeyboardShortcut
		{
			get { return _keyboardShortCut; }
			set { _keyboardShortCut = value; }
		}

		public List<PlugIn.ToolHost> ChildTools
		{
			get { return _childTools; }
		}

		public PlugIn.ToolHost CurrentTool
		{
			get { return this[SelectedIndex]; }
			set { this.SelectedIndex = value.Index; }
		}

		public PlugIn.ToolHost this[int index] 
		{
			get { return _childTools[index]; }
			set { _childTools[index] = value; }
		}

		public int SelectedIndex 
		{
			get { return _selectedIndex; }
			set
			{
				if (value != _selectedIndex)
				{
					if (value < 0)
						_selectedIndex = 0;
					else if (value >= _childTools.Count)
						_selectedIndex = _childTools.Count - 1;
					else
						_selectedIndex = value;

					if (SelectedIndexChanged != null)
						SelectedIndexChanged(this, new EventArgs());
				}
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		public void Add(PlugIn.ToolHost childTool)
		{
			_childTools.Add(childTool);
			childTool.Index = _childTools.Count - 1;
			childTool.Button.Click += new System.EventHandler(this.ChildTool_Click);
		}

		public virtual void ShutDown()
		{
			//foreach (KeyValuePair<int, PlugIn.ITool> Child in _childTools)
				//Child.Value.ShutDown();
			for (int i = 0; i < _childTools.Count; i++)
				_childTools[i].ShutDown();
		}

		#endregion [ Methods ]

		#region [ Events ]

		public void ChildTool_Click(object sender, EventArgs e)
		{
			this.SelectedIndex = ((PlugInToolStripButton)sender).ToolHost.Index;
		}

		#endregion [ Events ]

	}
}
