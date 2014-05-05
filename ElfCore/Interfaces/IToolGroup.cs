using ElfCore.PlugIn;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElfCore.Interfaces
{
	public interface IToolGroup : IDisposable
	{
		#region [ Properties ]

		/// <summary>
		/// Numeric identifier for the tool group.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// First key in the multi-gesture keystroke
		/// </summary>
		Keys MultiGestureKey1 { get; set; }

		/// <summary>
		/// Second key in the multi-gesture keystroke
		/// </summary>
		Keys MultiGestureKey2 { get; set; }

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
		/// The currently selected child tool
		/// </summary>
		PlugInTool CurrentTool { get; set; }

		/// <summary>
		/// List of all the child tools, with the tool ID as the key
		/// </summary>
		//SortedList<int, ITool> ChildTools { get; }
		List<PlugInTool> ChildTools { get; }

		/// <summary>
		/// An indexer to easily get at the child tools
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		PlugInTool this[int index] { get; set; }

		/// <summary>
		/// Property that indicates which tool is the current one
		/// </summary>
		int SelectedIndex { get; set; }

		/// <summary>
		/// Returns the number of child tools.
		/// </summary>
		int Count { get; }

		#endregion [ Properties ]
	
		#region [ Methods ]

		/// <summary>
		/// Add a new child tool to the ToolGroup
		/// </summary>
		/// <param name="newChildTool">Child tool to add to this ToolGroup</param>
		void Add(PlugInTool newChildTool);

		/// <summary>
		/// Setup the ToolGroup
		/// </summary>
		void Initialize();

		/// <summary>
		/// Method that is called when ToolGroup settings are to be saved.
		/// </summary>
		void SaveSettings();

		/// <summary>
		/// Called when the editor is shutting down and clean up needs to occur
		/// </summary>
		void ShutDown();

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Event that fires whenever the selected index of the Child Tools changes
		/// </summary>
		event EventHandler SelectedIndexChanged;

		#endregion [ Events ]


	}
}
