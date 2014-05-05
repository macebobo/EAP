using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ClipperLib;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	using Polygon = List<IntPoint>;
	using Polygons = List<List<IntPoint>>;

	[AdjPrevTool("Brush Mask")]
	public class MaskBrushTool : MaskBase, ITool
	{
		#region [ Private Variables ]

		private Nib _nib;
		private GraphicsPath _mouseDownPath = new GraphicsPath();
		private List<GraphicsPath> _workingPaths = new List<GraphicsPath>();

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Custom Cursor, created based on the nib size and shape
		/// </summary>
		public override Cursor Cursor
		{
			get
			{
				Point HotSpot = Point.Empty;
				Bitmap NibCursor = _nib.CursorBitmap(out HotSpot);
				Bitmap ModeBmp = GetCursorModeBitmap();
				PointF ModePt = PointF.Empty;
				int Width, Height;
				Bitmap Temp = null;
				
				if (ModeBmp != null)
				{
					ModeBmp = new Bitmap(ModeBmp);

					ModePt.X = NibCursor.Width;
					ModePt.Y = NibCursor.Height;

					Width = (int)ModePt.X + ModeBmp.Width;
					Height = (int)ModePt.Y + ModeBmp.Height;

					Temp = new Bitmap(Width, Height);
					using (Graphics g = Graphics.FromImage(Temp))
					{
						g.DrawImage(NibCursor, 0, 0, NibCursor.Width, NibCursor.Height);
						g.DrawImage(ModeBmp, (int)ModePt.X, (int)ModePt.Y, ModeBmp.Width, ModeBmp.Height);
					}

					NibCursor.Dispose();
					NibCursor = new Bitmap(Temp);
					Temp.Dispose();
				}

				Cursor Return = CustomCursors.CreateCursor(NibCursor, HotSpot.X, HotSpot.Y);
				NibCursor.Dispose();
				if (ModeBmp != null)
					ModeBmp.Dispose();

				return Return;
				//return _nib.Cursor; 
			}
		}

		#endregion [ Properties ]

		#region [ Constructors ]

		public MaskBrushTool()
		{
			this.ID = (int)Tool.Mask_Paint;
			this.Name = "Brush Marquee";
			this.ToolBoxImage = ElfRes.mask_paint;
			this.ToolGroupName = Constants.TOOLGROUP_MASK;
			_nib = new Nib();
			_freehandStyle = true;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Take the working path, convert it into a Clipper Polygon, then convert this back to a GrahicsPath and return
		/// </summary>
		protected override GraphicsPath CreateRenderPath(Point p1, Point p2, Scaling scaling)
		{
			GraphicsPath Path = null;

			if (_workingPaths.Count <= 0)
				return _mouseDownPath;

			Path = (GraphicsPath)_mouseDownPath.Clone();
			foreach (GraphicsPath p in _workingPaths)
				JoinPaths(Path, p);

			// If this render is for the Lattice, scale it down
			if (scaling == Scaling.Cell)
			{
				float CellPixelScale = 1f / UISettings.ʃCellScaleF;
				Matrix ScaleMatrix = new Matrix();
				ScaleMatrix.Scale(CellPixelScale, CellPixelScale);
				Path.Transform(ScaleMatrix);
				
				ScaleMatrix.Dispose();
				ScaleMatrix = null;
			}

			return Path;
		}

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

			_nib.RoundNib_Control = base.RoundNib;
			_nib.SquareNib_Control = base.SquareNib;
			_nib.NibSize_Control = base.NibSize;

			// Load the Settings values
			_nib.LoadSettings(/*settings, */_savePath);

			// Attach all events that would normally go within the form to methods in this class
			_nib.AttachControlEvents();
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			base.MouseDown(buttons, mouseCell, mousePixel);
			//CreateUndo_Mask(this.UndoText);

			if (_mouseDownPath == null)
				_mouseDownPath = new GraphicsPath();
			else
				_mouseDownPath.Reset();
			if (_workingPaths == null)
				_workingPaths = new List<GraphicsPath>();

			_nib.AdjustForCellSize();
			PaintMask(mouseCell);
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (!_mouseDown)
				return false;
			PaintMask(mouseCell);
			return true;
		}
		
		/// <summary>
		/// Adds a shape the size and shape of the nib to the working graphics path where indicated on the canvas.
		/// Paints a masking indicator on the CanvasPane itself to indicate where the mask will be placed when the button is released.
		/// </summary>
		/// <param name="pt"></param>
		private void PaintMask(Point pt)
		{
			_nib.OffsetRects(pt);
			GraphicsPath NibPath = (_mouseDownPath.PointCount == 0) ? _mouseDownPath : new GraphicsPath();			

			using (SolidBrush MaskBrush = new SolidBrush(Color.FromArgb(64, Color.White)))
			{
				if (_nib.Shape == Nib.NibShape.Square)
				{
					_canvasControlGraphics.FillRectangle(MaskBrush, _nib.Rect_Canvas);
					NibPath.AddRectangle(_nib.Rect_Canvas);
				}
				else
				{
					_canvasControlGraphics.FillEllipse(MaskBrush, _nib.Rect_Canvas);
					NibPath.AddEllipse(_nib.Rect_Canvas);
				}
			}
			NibPath.Flatten();

			if (NibPath != _mouseDownPath)
				_workingPaths.Add(NibPath);

			//Editor.ExposePane(_canvasBuffer, Panes.MaskCanvas);
		}

		/// <summary>
		/// Clean up our objects after a draw event is completed. This should be called after the work is done in MouseUp.
		/// </summary>
		protected override void PostDrawCleanUp()
		{
			base.PostDrawCleanUp();

			if (_mouseDownPath != null)
			{
				_mouseDownPath = new GraphicsPath();
			}
			for (int i = 0; i < _workingPaths.Count; i++)
			{
				_workingPaths[i].Dispose();
				_workingPaths[i] = null;
			}
			_workingPaths = new List<GraphicsPath>();

			SetFunctionButtons();
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		/// <param name="settings"></param>
		public override void SaveSettings()
		{
			base.SaveSettings();
			_nib.SaveSettings(_settings, _savePath);
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// Show our custom tools
		/// </summary>
		public override void Selected()
		{
			base.Selected();
			
			MaskSepNib.Visible = true;
			_NibSize.Visible = true;
			NibSize.Visible = true;
			SquareNib.Visible = true;
			RoundNib.Visible = true;

			// Attach to the UI events so that the cursor will correctly resize
			_workshop.UI.CellSizeChanged += new System.EventHandler(UI_Changed);
			_workshop.UI.DisplayGridLines += new System.EventHandler(UI_Changed);
			_workshop.UI.Zooming += new System.EventHandler(UI_Changed);
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// Remove all the click events from these shared controls
		/// </summary>
		public override void Unselected()
		{
			base.Unselected();

			if (_mouseDownPath != null)
			{
				_mouseDownPath.Dispose();
				_mouseDownPath = null;
			}
			if (_workingPaths != null)
			{
				for (int i = 0; i < _workingPaths.Count; i++)
				{
					_workingPaths[i].Dispose();
					_workingPaths[i] = null;
				}
				_workingPaths = null;
			}

			// Detach from the UI events, we don't want to to fire if this is not the selected Tool
			_workshop.UI.CellSizeChanged -= new System.EventHandler(UI_Changed);
			_workshop.UI.DisplayGridLines -= new System.EventHandler(UI_Changed);
			_workshop.UI.Zooming -= new System.EventHandler(UI_Changed);
		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Occurs when one of the critical properties in the UI changes: CellSize, GridLineWidth, Zoom
		/// </summary>
		private void UI_Changed(object sender, System.EventArgs e)
		{
			Workshop.Canvas.Cursor = _nib.Cursor;
		}

		#endregion [ Events ]
	}
}
