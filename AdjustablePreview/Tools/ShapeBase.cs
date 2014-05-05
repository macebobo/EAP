using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

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
			this.ID = (int)Tool.NotSet;
			this.Name = "SHAPEBASE";
			this.ToolBoxImage = ElfRes.not;
			//this.Cursor = Cursors.Cross;
			this.DoesSelection = true;
			this.ToolGroupName = Constants.TOOLGROUP_SHAPE;
			//_createUndoOnMouseDown = false;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

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
				p1 = Workshop.CellPoint(p1);
				p2 = Workshop.CellPoint(p2);
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
		/// <param name="settings">Settings object, handles getting and saving settings data</param>
		/// <param name="workshop">Workshop object, contains lots of useful methods and ways to hold data.</param>
		public override void Initialize()
		{
			base.Initialize();
			int ButtonIndex = 1;

			// Load the Settings values
			_lineThickness = LoadValue(Constants.LINE_THICKNESS, 1);
			_fill = LoadValue(Constants.FILL, false);
			_dashStyle = EnumHelper.GetEnumFromValue<DashStyle>(LoadValue(Constants.DASH_STYLE, (int)DashStyle.Solid));

			// Get a pointer to the controls on the toolstrip that belongs to us.
			LineThickness = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			DashStyleDD = (ToolStripDropDownButton)GetItem<ToolStripDropDownButton>(1);

			DoNotFill = (ToolStripButton)GetItem<ToolStripButton>(ButtonIndex++);
			DoFill = (ToolStripButton)GetItem<ToolStripButton>(ButtonIndex++);

			if (DashStyleDD != null)
			{
				SetDashStyleDropDownButton(DashStyleDD);
				foreach (ToolStripMenuItem Item in DashStyleDD.DropDownItems)
				{
					Item.Click += new System.EventHandler(this.DashStyle_Click);
				}
				DashStyle_Click(FindDropMenuItemFromValue(DashStyleDD, (int)_dashStyle), null);
			}

			if (LineThickness != null)
			{
				LineThickness.SelectedIndexChanged += new System.EventHandler(this.LineThickness_SelectedIndexChanged);
				LineThickness.SelectedIndex = _lineThickness - 1;
			}

			if (DoFill != null)
			{
				DoFill.Click += new System.EventHandler(this.DoFill_Click);
				DoFill.Checked = _fill;
			}

			if (DoNotFill != null)
			{
				DoNotFill.Click += new System.EventHandler(this.DoNotFill_Click);
				DoNotFill.Checked = !_fill;
			}				
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseMove(buttons, mouseCell, mousePixel))
				return false;

			Render(_mouseDownCellPixel, _constrainedCellPixel, false);

			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!base.MouseUp(buttons, mouseCell, mousePixel))
				return false;

			Cursor LastCursor = Workshop.Canvas.Cursor;
			Workshop.Canvas.Cursor = Cursors.WaitCursor;

			// Render the shape onto the active Channel
			Render(_mouseDownCellPixel, _constrainedCellPixel, true);
			Editor.ExposePane(_latticeBuffer, Panes.LatticeBuffer);

			// Write out the changes onto the active Channel
			_workshop.Channels.Active.ClearLattice();
			_workshop.Channels.Active.LatticeBuffer = _latticeBuffer;

			// Redraw the canvas to expose our changes.
			Workshop.Canvas.Refresh();

			// Clean up
			PostDrawCleanUp();

			_mouseDownCell = Point.Empty;
			Workshop.Canvas.Cursor = LastCursor;

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

		#region [ ToolStrip Events ]

		private void LineThickness_SelectedIndexChanged(object sender, EventArgs e)
		{
			string Value = LineThickness.SelectedItem.ToString();
			if (Value.Length > 0)
				_lineThickness = Convert.ToInt32(Value);
		}

		private void DashStyle_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem Menu = (ToolStripMenuItem)sender;
			DashStyleDD.Image = Menu.Image;
			DashStyleDD.Text = Menu.Text;
			DashStyleDD.ToolTipText = Menu.ToolTipText;

			foreach (ToolStripMenuItem Item in DashStyleDD.DropDownItems)
			{
				if (Item != sender)
					Item.Checked = false;
			}

			_dashStyle = (DashStyle)Convert.ToInt32(Menu.Tag);
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

		#endregion [ ToolStrip Events ]

	}
}
