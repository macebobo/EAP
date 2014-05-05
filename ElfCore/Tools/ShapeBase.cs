using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	/// <summary>
	/// Common functionality for several shape tools
	/// </summary>
	public abstract class ShapeBase: BaseTool, ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		protected int _lineThickness = 1;
		protected DashStyle _dashStyle = DashStyle.Solid;
		protected bool _fill = false;

		// Controls from ToolStrip
		protected ToolStripButton DoFill = null;
		protected ToolStripButton DoNotFill = null;
		protected ToolStripComboBox LineThickness = null;
		protected ToolStripDropDownButton DashStyleDD = null;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public ShapeBase()
		{
			this.ID = (int)ToolID.NotSet;
			this.Name = "SHAPEBASE";
			this.ToolBoxImage = ElfRes.not;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_SHAPE;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

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
				if (DashStyleDD != null)
				{
					foreach (ToolStripMenuItem Item in DashStyleDD.DropDownItems)
						Item.Click += new EventHandler(DashStyle_Click);
				}

				if (LineThickness != null)
					LineThickness.SelectedIndexChanged += new EventHandler(LineThickness_SelectedIndexChanged);

				if (DoFill != null)
					DoFill.Click += new EventHandler(DoFill_Click);

				if (DoNotFill != null)
					DoNotFill.Click += new EventHandler(DoNotFill_Click);
			}
			else
			{
				if (DashStyleDD != null)
				{
					foreach (ToolStripMenuItem Item in DashStyleDD.DropDownItems)
						Item.Click -= DashStyle_Click;
				}

				if (LineThickness != null)
					LineThickness.SelectedIndexChanged -= LineThickness_SelectedIndexChanged;

				if (DoFill != null)
					DoFill.Click -= DoFill_Click;

				if (DoNotFill != null)
					DoNotFill.Click -= DoNotFill_Click;
			}
			base.AttachEvents(attach);
		}

		/// <summary>
		/// Draw the shape
		/// </summary>
		/// <param name="p1">Upper Left point in pixels</param>
		/// <param name="p2">Lower Right point in pixels</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected virtual void Render(Point p1, Point p2, bool finalRender)
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

				if (_fill && finalRender)
					_latticeBufferGraphics.FillPath(Brushes.White, DrawPath);
			}

			DrawPath.Dispose();
			DrawPath = null;
		}

		/// <summary>
		/// Create the graphics path needed to draw the path.
		/// </summary>
		/// <param name="p1">Upper Left point</param>
		/// <param name="p2">Lower Right point</param>
		/// <param name="finalRender">True if this drawing is to be the final render, false if its to be while the user is still doing a select</param>
		protected abstract GraphicsPath CreateRenderPath(Point p1, Point p2, bool finalRender);

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
			int ButtonIndex = 1;

			// Load the Settings values
			_lineThickness = LoadValue(Constants.LINE_THICKNESS, 1);
			_fill = LoadValue(Constants.FILL, false);
			_dashStyle = EnumHelper.GetEnumFromValue<DashStyle>(LoadValue(Constants.DASH_STYLE, (int)DashStyle.Solid));

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			LineThickness = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			DashStyleDD = (ToolStripDropDownButton)GetItem<ToolStripDropDownButton>(1);

			DoNotFill = (ToolStripButton)GetItem<ToolStripButton>(ButtonIndex++);
			DoFill = (ToolStripButton)GetItem<ToolStripButton>(ButtonIndex++);

			if (DashStyleDD != null)
			{
				SetDashStyleDropDownButton(DashStyleDD);
				SetDropDownMenuSelected(FindDropMenuItemFromValue(DashStyleDD, (int)_dashStyle));
			}

			if (LineThickness != null)
				LineThickness.SelectedIndex = _lineThickness - 1;

			if (DoFill != null)
				DoFill.Checked = _fill;

			if (DoNotFill != null)
				DoNotFill.Checked = !_fill;
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
			DisplayCapturedCanvas();

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
				Controllers.Workshop.ExposePane(_latticeBuffer, Panes.LatticeBuffer);
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
			Pen RenderPen = new Pen(Color.White, _lineThickness);
			RenderPen.DashStyle = _dashStyle;
			return RenderPen;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			if (LineThickness != null)
				SaveValue(Constants.LINE_THICKNESS, _lineThickness);

			if (DashStyleDD != null)
				SaveValue(Constants.DASH_STYLE, (int)_dashStyle);

			if (DoFill != null)
				SaveValue(Constants.FILL, _fill);
		}

		private void SetDropDownMenuSelected(ToolStripMenuItem menuItem)
		{
			DashStyleDD.Image = menuItem.Image;
			DashStyleDD.Text = menuItem.Text;
			DashStyleDD.ToolTipText = menuItem.ToolTipText;

			foreach (ToolStripMenuItem Item in DashStyleDD.DropDownItems)
			{
				if (Item != menuItem)
					Item.Checked = false;
			}

			_dashStyle = (DashStyle)Convert.ToInt32(menuItem.Tag);
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			DoFill = null;
			DoNotFill = null;
			LineThickness = null;
			DashStyleDD = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		private void LineThickness_SelectedIndexChanged(object sender, EventArgs e)
		{
			string Value = LineThickness.SelectedItem.ToString();
			if (Value.Length > 0)
				_lineThickness = Convert.ToInt32(Value);
		}

		private void DashStyle_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem Menu = (ToolStripMenuItem)sender;
			SetDropDownMenuSelected(Menu);
		}

		private void DoFill_Click(object sender, EventArgs e)
		{
			if (DoFill.Checked)
				return;
			DoNotFill.Checked = false;
			DoFill.Checked = true;
			_fill = true;
		}

		private void DoNotFill_Click(object sender, EventArgs e)
		{
			if (DoNotFill.Checked)
				return;
			DoFill.Checked = false;
			DoNotFill.Checked = true;
			_fill = false;
		}

		#endregion [ ToolStrip Event Delegates ]

	}
}
