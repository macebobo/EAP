using System;
using System.Drawing;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Zoom")]
	public class ZoomTool : BaseTool, ITool
	{
		#region [ Enums ]

		enum ZoomMode
		{
			NotSet = -1,
			ZoomIn,
			ZoomOut
		}

		#endregion [ Enums ]

		#region [ Private Variables ]

		// Settings from the ToolStrip
		//private ZoomMode _zoomMode = ZoomMode.ZoomIn;

		// Controls from ToolStrip
		private ToolStripComboBox ZoomFactor = null;
		private ToolStripButton ZoomIn = null;
		private ToolStripButton ZoomOut = null;
		private ToolStripButton Zoom100 = null;

		private bool _setting = false;

		#endregion [ Private Variables ]
			
		#region [ Constructors ]

		public ZoomTool()
		{
			this.ID = (int)Tool.Zoom;
			this.Name = "Zoom";
			this.ToolBoxImage = ElfRes.zoom;
			base.Cursor = CreateCursor(ElfRes.zoom, new Point(7, 7));
		}

		#endregion [ Constructors ]

		#region [ Methods ]

			/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		/// <param name="settings">Settings object, handles getting and saving settings data</param>
		/// <param name="workshop">Workshop object, contains lots of useful methods and ways to hold data.</param>
		public override void Initialize()
		{
			base.Initialize();

			// Load the Settings values

			// Get a pointer to the controls on the toolstrip that belongs to us.
			ZoomFactor = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			ZoomIn = (ToolStripButton)GetItem<ToolStripButton>(1);
			ZoomOut = (ToolStripButton)GetItem<ToolStripButton>(2);
			Zoom100 = (ToolStripButton)GetItem<ToolStripButton>(3);

			// Attach events to these controls
			ZoomFactor.SelectedIndexChanged += new System.EventHandler(this.ZoomFactor_SelectedIndexChanged);
			ZoomIn.Click += new System.EventHandler(this.ZoomIn_Click);
			ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
			Zoom100.Click += new System.EventHandler(this.Zoom100_Click);

			// Set the initial value for the contol from what we had retrieve from Settings
			SetZoom();
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			return;
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			_setting = true;

			float Zoom = _workshop.UI.Zoom;

			if (buttons == MouseButtons.Left)
				Zoom++;
			else if (buttons == MouseButtons.Right)
				Zoom--;

			SetZoom(Zoom);
			if (buttons == MouseButtons.Left)
				_workshop.SetClickZoom(mousePixel, Zoom);
			else if (buttons == MouseButtons.Right)
				_workshop.UI.Zoom = Zoom;

			_setting = false;

			// Fire the event to indicate that this tool has finished working.
			EndOperation();

			return true;
		}

		private void SetZoom(float zoomLevel)
		{
			ZoomFactor.SelectedIndex = (int)zoomLevel - 1;
			ZoomIn.Enabled = true;
			ZoomOut.Enabled = true;

			if (zoomLevel == UISettings.MIN_ZOOM)
				ZoomOut.Enabled = false;

			else if (zoomLevel == UISettings.MAX_ZOOM)
				ZoomIn.Enabled = false;

			Zoom100.Enabled = (zoomLevel != UISettings.ZOOM_100);
		}

		private void SetZoom()
		{
			SetZoom(_workshop.UI.Zoom);
		}

		public override void ShutDown()
		{
			base.ShutDown();
			ZoomFactor = null;
			ZoomIn = null;
			ZoomOut = null;
			Zoom100 = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		private void ZoomFactor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_setting)
				return;

			_setting = true;
			_workshop.UI.Zoom = ZoomFactor.SelectedIndex + 1;
			SetZoom();
			_workshop.UndoController.SaveUndo(this.UndoText);
			_setting = false;
		}

		private void Zoom100_Click(object sender, EventArgs e)
		{
			_setting = true;
			//CreateUndo();
			_workshop.UI.Zoom = 1;
			SetZoom();
			_workshop.UndoController.SaveUndo(this.UndoText);

			_setting = false;
		}

		private void ZoomIn_Click(object sender, EventArgs e)
		{
			_setting = true;
			
			//CreateUndo();
			_workshop.UI.Zoom += 1;
			SetZoom();
			_workshop.UndoController.SaveUndo(this.UndoText);

			_setting = false;
		}

		private void ZoomOut_Click(object sender, EventArgs e)
		{
			_setting = true;
			
			//CreateUndo();
			_workshop.UI.Zoom -= 1;
			SetZoom();
			_workshop.UndoController.SaveUndo(this.UndoText);

			_setting = false;
		}
		

		#endregion [ ToolStrip Events ]
	
	}
}
