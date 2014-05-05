using System.Drawing;
using System.Drawing.Drawing2D;
using ToolRes = Tools.Properties.Resources;
using System.Windows.Forms;
using System;
using CanvasPoint = System.Drawing.Point;
using LatticePoint = System.Drawing.Point;

namespace Tools
{
	[ElfCore.Util.ElfTool("TestTool1")]
	public class TestTool1 : ElfTools.Tools.BaseTool, ElfCore.Interfaces.ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		private int _lineThickness = 1;

		// Controls from ToolStrip
		private ToolStripComboBox LineThickness = null;
		private ToolStripContainer _toolStripContainer = null;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// ToolStrip to display under the menu when this tool is selected. If no settings are available, this can safely return null.
		/// </summary>
		public override ToolStrip SettingsToolStrip
		{
			get { return _toolStrip_Form.GetToolStrip(0); }
			set { }
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public TestTool1()
			: base()
		{
			_toolStripContainer = new ToolStripContainer();

			this.ID = (int)ElfCore.Util.ToolID.NotSet;
			this.Name = "TestTool1";
			this.DoesSelection = true;
			this.ToolBoxImage = ToolRes.line;
			base.Cursor = Cursors.Cross;
			this.ToolGroupName = string.Empty;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		private GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender)
		{
			GraphicsPath DrawPath = new GraphicsPath();
			DrawPath.AddLine(p1, p2);
			return DrawPath;
		}

		/// <summary>
		/// Attaches or detaches events to objects, such as Click events to buttons.
		/// </summary>
		/// <param name="attach">Indicates that the events should be attached. If false, then detaches the events</param>
		protected override void AttachEvents(bool attach)
		{
			// If we've already either attached or detached, exit out.
			if (attach && _eventsAttached)
				return;

			if (attach)
			{
				if (LineThickness != null)
					LineThickness.SelectedIndexChanged += new EventHandler(LineThickness_SelectedIndexChanged);
			}
			else
			{
				if (LineThickness != null)
					LineThickness.SelectedIndexChanged -= LineThickness_SelectedIndexChanged;
			}
		}

		/// <summary>
		/// Draw the shape
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		private void Render(Point p1, Point p2, bool finalRender)
		{
			if (finalRender)
			{
				p1 = _workshop.CalcLatticePoint(p1);
				p2 = _workshop.CalcLatticePoint(p2);
			}

			GraphicsPath DrawPath = CreateRenderPath(p1, p2, finalRender);

			using (Pen DrawPen = finalRender ? RenderPen() : _workshop.GetMarqueePen())
			{
				try
				{
					(finalRender ? _latticeBufferGraphics : _canvasControlGraphics).DrawPath(DrawPen, DrawPath);
				}
				catch (OutOfMemoryException)
				{ }
			}

			DrawPath.Dispose();
			DrawPath = null;
		}

		/// <summary>
		/// Load in the saved values from the Settings Xml file. The path to be used should be 
		/// ToolSettings|[Name of this tool].
		/// We use the pipe character to delimit the names, because we don't want to be necessarily tied down to only one
		/// format for saving. If it gets changed at some later date, doing it this way prevents code from being recompiled
		/// for these PlugIns, as the AdjustablePreview code converts the pipe to the proper syntax.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_toolStrip_Form = new ToolStripContainer();

			// Load the Settings values
			_lineThickness = LoadValue(ElfCore.Util.Constants.LINE_THICKNESS, 1);

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			LineThickness = (ToolStripComboBox)GetItem<ToolStripComboBox>("cboLineSize");
			if (LineThickness != null)
				LineThickness.SelectedIndex = _lineThickness - 1;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			base.MouseDown(buttons, latticePoint, actualCanvasPoint);
			_mouseDownCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!_isMouseDown)
				return false;

			if (!base.MouseMove(buttons, latticePoint, actualCanvasPoint))
				return false;

			_currentLatticePoint = latticePoint;
			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);
			_constrainedCanvasPoint = _workshop.ConstrainLine(_currentMouseCanvasPoint, _mouseDownCanvasPoint);

			// Draw the captured bitmap onto the CanvasPane PictureBox
			base.DisplayCapturedCanvas();

			Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, false);

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			_currentMouseCanvasPoint = _workshop.CalcCanvasPoint_OC(latticePoint);

			Cursor LastCursor = Profile.Cursor;
			Profile.Cursor = Cursors.WaitCursor;

			// Render the shape onto the active Channel
			Render(_mouseDownCanvasPoint, _constrainedCanvasPoint, true);

			#if DEBUG
				// If displaying the diagnostic window, show how this tool has written to the lattice buffer of the active channel.
			ElfCore.Controllers.Workshop.Instance.ExposePane(_latticeBuffer, ElfCore.Util.Panes.LatticeBuffer);
			#endif

			// Write out the changes onto the active Channel
			Profile.Channels.Active.Empty();
			Profile.Channels.Active.LatticeBuffer = _latticeBuffer;

			// Redraw the canvas to expose our changes.
			Profile.Refresh();

			// Clean up
			PostDrawCleanUp();

			_mouseDownLatticePoint = Point.Empty;
			Profile.Cursor = LastCursor;

			return true;
		}

		/// <summary>
		/// Creates the Pen used to render the shape onto the Paint Pane
		/// </summary>
		protected override Pen RenderPen()
		{
			return new Pen(Color.White, _lineThickness);
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			if (LineThickness != null)
				SaveValue(ElfCore.Util.Constants.LINE_THICKNESS, _lineThickness);
		}
	
		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();
			LineThickness = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		private void LineThickness_SelectedIndexChanged(object sender, EventArgs e)
		{
			string Value = LineThickness.SelectedItem.ToString();
			if (Value.Length > 0)
				_lineThickness = Convert.ToInt32(Value);
		}

		#endregion [ ToolStrip Event Delegates ]
	}
}
