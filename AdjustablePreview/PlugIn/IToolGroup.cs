using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ElfCore.PlugIn
{
	public interface IToolGroup
	{
		#region [ Events ]

		/// <summary>
		/// Event that fires whenever the selected index of the Child Tools changes
		/// </summary>
		event System.EventHandler SelectedIndexChanged;

		#endregion [ Events ]

		#region [ Properties ]

		/// <summary>
		/// Numeric identifier for the tool group.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// String representing the tool's name
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// ToolTip to use with the ToolBox button
		/// </summary>
		string ToolTipText { get;  }
		
		/// <summary>
		/// 16x16 transparent bitmap to display in the toolbox
		/// </summary>
		Bitmap ToolBoxImage { get;  }

		/// <summary>
		/// Keyboard shortcut used to select this tool
		/// </summary>
		KeyChord KeyboardShortcut { get; set; }

		/// <summary>
		/// The currently selected child tool
		/// </summary>
		ToolHost CurrentTool { get; set; }

		/// <summary>
		/// List of all the child tools, with the tool ID as the key
		/// </summary>
		//SortedList<int, ITool> ChildTools { get; }
		List<ToolHost> ChildTools { get; }

		/// <summary>
		/// An indexer to easily get at the child tools
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		ToolHost this[int index] { get; set; }

		/// <summary>
		/// Property that indicates which tool is the current one
		/// </summary>
		int SelectedIndex { get; set; }

		#endregion [ Properties ]
	
		#region [ Methods ]

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		void ShutDown();

		/// <summary>
		/// Method that adds a new child tool to the ToolGroup
		/// </summary>
		/// <param name="newChildTool"></param>
		void Add(ToolHost newChildTool);

		#endregion [ Methods ]
	}
}
