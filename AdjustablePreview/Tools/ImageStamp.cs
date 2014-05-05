using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ElfCore.PlugIn;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Tools
{
	[AdjPrevTool("Image Stamp")]
	public class ImageStampTool : BaseTool, ITool
	{
		#region [ Enums ]

		enum ImageStamp
		{
			NotSet = -1,
			CandyCane,
			Ghost,
			JackOLantern,
			MiniTree,
			Snowflake,
			Custom
		}

		enum ImageStampSize
		{
			NotSet = -1,
			VerySmall,
			Small,
			Medium,
			Large
		}

		#endregion [ Enums ]

		#region [ Private Variables ]

		// Settings from the ToolStrip
		private ImageStamp _imageStamp = ImageStamp.NotSet;
		private ImageStampSize _imageStampSize = ImageStampSize.NotSet;
		private string _customFilename = string.Empty;
		private bool _flipHorizontally = false;
		private bool _flipVertically = false;

		// Controls from ToolStrip
		private ToolStripDropDownButton SelectStampDD = null;
		private ToolStripLabel StampSizeLabel = null;
		private ToolStripComboBox StampSize = null;
		private ToolStripLabel CustomNameLabel = null;
		private ToolStripTextBox CustomName = null;
		private ToolStripButton Browse = null;
		private ToolStripButton FlipHorizontally = null;
		private ToolStripButton FlipVertically = null;
		private Timer LoadTimer = null;

		// Used for rendering
		private Point _smallStampOffset = new Point(0, 0);
		private Rectangle _stampBounds = Rectangle.Empty;
		private bool _loading = false;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string IS_IMAGE = "Image";
		private const string IS_SIZE = "Size";
		private const string IS_FLIPHORIZONTAL = "FlipHorizontal";
		private const string IS_FLIPVERTICAL = "FlipVertical";
		private const string IS_FILENAME = "CustomFileName";

		#endregion [ Constants ]

		#region [ Constructors ]

		public ImageStampTool()
		{
			this.ID = (int)Tool.ImageStamp;
			this.Name = "Image Stamp";
			this.ToolBoxImage = ElfRes.imagestamp;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Determine which image to load either out of the assembly resources, or from a file.
		/// </summary>
		/// <returns>Bitmap representing the Image Stamp</returns>
		private Bitmap GetImageStampBitmap()
		{
			Bitmap Stamp = null;

			switch (_imageStamp)
			{

				#region [ Pre-defined stamps ]
				
				case ImageStamp.CandyCane:
					switch (_imageStampSize)
					{
						case ImageStampSize.VerySmall:
							Stamp = ElfRes.candycane_5;
							break;
						case ImageStampSize.Small:
							Stamp = ElfRes.candycane_10;
							break;
						case ImageStampSize.Medium:
							Stamp = ElfRes.candycane_20;
							break;
						case ImageStampSize.Large:
							Stamp = ElfRes.candycane_30;
							break;
					}
					break;

				case ImageStamp.Ghost:
					switch (_imageStampSize)
					{
						case ImageStampSize.VerySmall:
							Stamp = ElfRes.ghost_5;
							break;
						case ImageStampSize.Small:
							Stamp = ElfRes.ghost_10;
							break;
						case ImageStampSize.Medium:
							Stamp = ElfRes.ghost_20;
							break;
						case ImageStampSize.Large:
							Stamp = ElfRes.ghost_30;
							break;
					}
					break;

				case ImageStamp.JackOLantern:
					switch (_imageStampSize)
					{
						case ImageStampSize.VerySmall:
							Stamp = ElfRes.jack_5;
							break;
						case ImageStampSize.Small:
							Stamp = ElfRes.jack_10;
							break;
						case ImageStampSize.Medium:
							Stamp = ElfRes.jack_20;
							break;
						case ImageStampSize.Large:
							Stamp = ElfRes.jack_30;
							break;
					}
					break;

				case ImageStamp.MiniTree:
					switch (_imageStampSize)
					{
						case ImageStampSize.VerySmall:
							Stamp = ElfRes.tree_5;
							break;
						case ImageStampSize.Small:
							Stamp = ElfRes.tree_10;
							break;
						case ImageStampSize.Medium:
							Stamp = ElfRes.tree_20;
							break;
						case ImageStampSize.Large:
							Stamp = ElfRes.tree_30;
							break;
					}
					break;

				case ImageStamp.Snowflake:
					switch (_imageStampSize)
					{
						case ImageStampSize.VerySmall:
							Stamp = ElfRes.snowflake_7;
							break;
						case ImageStampSize.Small:
							Stamp = ElfRes.snowflake_10;
							break;
						case ImageStampSize.Medium:
							Stamp = ElfRes.snowflake_20;
							break;
						case ImageStampSize.Large:
							Stamp = ElfRes.snowflake_30;
							break;
					}
					break;

				#endregion [ Pre-defined stamps ]

				case ImageStamp.Custom:
					if ((_customFilename ?? string.Empty).Length == 0)
						return null;

					// Verify the file exists
					FileInfo FI = new FileInfo(_customFilename);
					if ((FI == null) || !FI.Exists)
						return null;
					FI = null;

					Stamp = new Bitmap(_customFilename);
					break;
			}

			if (Stamp == null)
				return null;

			if (_flipHorizontally)
				Stamp.RotateFlip(RotateFlipType.RotateNoneFlipX);

			if (_flipVertically)
				Stamp.RotateFlip(RotateFlipType.RotateNoneFlipY);

			//Stamp.Save("C:\\stamp.png", System.Drawing.Imaging.ImageFormat.Png);

			return Stamp;
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
			this.Cursor = base.CreateCursor(ElfRes.imagestamp, new Point(7, 7));

			// Load the Settings values
			_imageStamp = EnumHelper.GetEnumFromValue<ImageStamp>(LoadValue(IS_IMAGE, (int)ImageStamp.CandyCane));
			_imageStampSize = EnumHelper.GetEnumFromValue<ImageStampSize>(LoadValue(IS_SIZE, (int)ImageStampSize.Small));
			_customFilename = LoadValue(IS_FILENAME, string.Empty);
			_flipHorizontally = LoadValue(IS_FLIPHORIZONTAL, false);
			_flipVertically = LoadValue(IS_FLIPVERTICAL, false);

			// Get a pointer to the controls on the toolstrip that belongs to us.
			SelectStampDD = (ToolStripDropDownButton)GetItem<ToolStripDropDownButton>(1);
			StampSize = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			CustomName = (ToolStripTextBox)GetItem<ToolStripTextBox>(1);
			Browse = (ToolStripButton)GetItem<ToolStripButton>(1);
			FlipHorizontally = (ToolStripButton)GetItem<ToolStripButton>(2);
			FlipVertically = (ToolStripButton)GetItem<ToolStripButton>(3);
			StampSizeLabel = (ToolStripLabel)GetItem<ToolStripLabel>(2);
			CustomNameLabel = (ToolStripLabel)GetItem<ToolStripLabel>(3);

			LoadTimer = _toolStrip_Form.ToolTimer;

			// Attach events to these controls
			StampSize.SelectedIndexChanged += new System.EventHandler(this.StampSize_SelectedIndexChanged);
			Browse.Click += new System.EventHandler(this.Browse_Click);
			FlipHorizontally.Click += new System.EventHandler(this.FlipHorizontally_Click);
			FlipVertically.Click += new System.EventHandler(this.FlipVertically_Click);
			foreach (ToolStripMenuItem Item in SelectStampDD.DropDownItems)
			{
				Item.Click += new System.EventHandler(this.ImageStamp_Click);
			}

			// Assign the ImageStamp enum value to the tag of each stamp menu item
			SelectStampDD.DropDownItems[0].Tag = (int)ImageStamp.CandyCane;
			SelectStampDD.DropDownItems[1].Tag = (int)ImageStamp.Ghost;
			SelectStampDD.DropDownItems[2].Tag = (int)ImageStamp.JackOLantern;
			SelectStampDD.DropDownItems[3].Tag = (int)ImageStamp.MiniTree;
			SelectStampDD.DropDownItems[4].Tag = (int)ImageStamp.Snowflake;
			SelectStampDD.DropDownItems[5].Tag = (int)ImageStamp.Custom;

			// Set the initial value for the contol from what we had retrieve from Settings
			FlipHorizontally.Checked = _flipHorizontally;
			FlipVertically.Checked = _flipVertically;
			CustomName.Text = _customFilename;
			ImageStamp_Click(FindDropMenuItemFromValue(SelectStampDD, (int)_imageStamp), null);
			StampSize.SelectedIndex = (int)_imageStampSize;
		}

		/// <summary>
		/// Load the ImageStamp, move it to the proper Channel
		/// </summary>
		private void LoadImageStamp()
		{
			// First, clear out the image stamp Channel
			_workshop.Channels.ImageStampChannel.ClearLattice();

			if (_imageStampSize == ImageStampSize.VerySmall)
			{
				// Move it away from the cursor, which is 16x16 pixels
				Bitmap CursorBmp = ElfRes.imagestamp;
				Point CursorSize = new Point(CursorBmp.Size.Width, CursorBmp.Size.Height);
				CursorBmp = null;
				_smallStampOffset = Workshop.CellPoint(CursorSize);
			}
			else
			{
				_smallStampOffset = new Point(0, 0);
			}

			Bitmap Stamp = GetImageStampBitmap();

			// Next get the bitmap for this stamp and load it in
			_workshop.Channels.ImageStampChannel.ImportBitmap(Stamp, true);
			_stampBounds = _workshop.Channels.ImageStampChannel.GetBounds();

			Editor.ExposePane(Stamp, Panes.ImageStamp);
			Stamp = null;
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			// Do nothing at this point with this tool.
			return;
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			Point Offset = mouseCell;

			Offset.Offset(_smallStampOffset);
			Offset.X -= _stampBounds.Width / 2;
			Offset.Y -= _stampBounds.Height / 2;
			_workshop.Channels.ImageStampChannel.Origin = Offset;

			Workshop.Canvas.Refresh();
			return true;
		}

		/// <summary>
		/// Canvas MouseUp event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="mouseCell">Point on the picture box (in Cells) where the mouse event happened</param>
		public override bool MouseUp(MouseButtons buttons, Point mouseCell, Point mousePixel)
		{
			if (_loading)
				return false;

			Point Offset = mouseCell;

			Offset.Offset(_smallStampOffset);
			Offset.X -= _stampBounds.Width / 2;
			Offset.Y -= _stampBounds.Height / 2;
			_workshop.Channels.ImageStampChannel.Origin = Offset;

			List<Point> Cells = null;

			foreach (Channel Channel in _workshop.Channels.Selected)
			{
				Cells = Workshop.CloneList<Point>(_workshop.Channels.ImageStampChannel.Lattice);
				Channel.PaintCells(Cells, Offset);
			}

			PostDrawCleanUp();

			return true;
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			 SaveValue(IS_IMAGE, (int)_imageStamp);
			 SaveValue(IS_SIZE, (int)_imageStampSize);
			 SaveValue(IS_FILENAME, _customFilename);
			 SaveValue(IS_FLIPHORIZONTAL, _flipHorizontally);
			 SaveValue(IS_FLIPVERTICAL, _flipVertically);
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// For this tool, it will set a flag in the Workshop object to indicate to show the ImageStamp Channel
		/// </summary>
		public override void Selected()
		{
			base.Selected();

			LoadTimer.Tick += new System.EventHandler(this.LoadTimer_Tick);

			LoadTimer.Enabled = false;
			LoadTimer.Interval = 100;

			LoadImageStamp();
			_workshop.ShowImageStamp = true;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		public override void Unselected()
		{
			_workshop.ShowImageStamp = false;
			LoadTimer.Enabled = false;
			LoadTimer.Tick -= this.LoadTimer_Tick;

			if (Workshop.Canvas != null)
				Workshop.Canvas.Refresh();
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			SelectStampDD = null;
			StampSize = null;
			CustomName = null;
			Browse = null;
			FlipHorizontally = null;
			FlipVertically = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Events ]

		/// <summary>
		/// Select a file to use as an image stamp.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Browse_Click(object sender, EventArgs e)
		{
			_loading = true;

			OpenFileDialog OpenImageFileDialog = new OpenFileDialog();
			OpenImageFileDialog.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|GIF File (*.gif)|*.gif|All Files (*.*)|*.*";
			OpenImageFileDialog.FilterIndex = 5;
			OpenImageFileDialog.FileName = CustomName.Text;
			OpenImageFileDialog.Title = "Select File for Image Stamp";

			if (OpenImageFileDialog.ShowDialog() == DialogResult.Cancel)
			{
				OpenImageFileDialog.Dispose();
				OpenImageFileDialog = null;
				_loading = false;
				return;
			}

			string Filename = OpenImageFileDialog.FileName;

			Bitmap Stamp = new Bitmap(Filename);
			if ((Stamp.Width > 100) || (Stamp.Height > 100))
			{
				MessageBox.Show("Custom Image stamp file cannot be larger than 100x100 pixels.", "Load Custom Image Stamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
				_loading = false;
				return;
			}
			else
				_customFilename = Filename;
			
			Stamp.Dispose();
			Stamp = null;

			OpenImageFileDialog.Dispose();
			OpenImageFileDialog = null;

			LoadImageStamp();
			LoadTimer.Enabled = true;
		}

		/// <summary>
		/// Instructs us to flip the image stamp top to bottom, or back again.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FlipHorizontally_Click(object sender, EventArgs e)
		{
			_flipHorizontally = FlipHorizontally.Checked;
			LoadImageStamp();
		}

		/// <summary>
		/// Instructs to flip the image stamp left to right, or back again
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FlipVertically_Click(object sender, EventArgs e)
		{
			_flipVertically = FlipVertically.Checked;
			LoadImageStamp();
		}

		/// <summary>
		/// Pre-selected Image Stamp has been picket, or else the custom option is selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImageStamp_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem Menu = (ToolStripMenuItem)sender;
			SelectStampDD.Image = Menu.Image;
			SelectStampDD.Text = Menu.Text;
			SelectStampDD.ToolTipText = Menu.ToolTipText;

			foreach (ToolStripMenuItem Item in SelectStampDD.DropDownItems)
			{
				if (Item != sender)
					Item.Checked = false;
			}

			//_imageStamp = EnumHelper.GetEnumFromValue<ImageStamp>(Convert.ToInt32(Menu.Tag));
			_imageStamp = (ImageStamp)Menu.Tag;

			bool IsCustom = (_imageStamp == ImageStamp.Custom);

			StampSize.Visible = !IsCustom;
			StampSizeLabel.Visible = !IsCustom;
			CustomNameLabel.Visible = IsCustom;
			CustomName.Visible = IsCustom;
			Browse.Visible = IsCustom;

			if (!IsCustom || (IsCustom && _customFilename.Length > 0))
				LoadImageStamp();
		}

		/// <summary>
		/// Resets the loading flag so that the user can proceed to do their stamping
		/// </summary>
		private void LoadTimer_Tick(object sender, EventArgs e)
		{
			LoadTimer.Enabled = false;
			_loading = false;
		}

		/// <summary>
		/// Stamp Size set
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StampSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			_imageStampSize = EnumHelper.GetEnumFromValue<ImageStampSize>(StampSize.SelectedIndex);
			LoadImageStamp();
		}

		#endregion [ ToolStrip Events ]
			
	}
}
