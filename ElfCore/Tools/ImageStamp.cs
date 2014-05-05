using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	[ElfEditTool("Image Stamp")]
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
		private ToolStripTextBox txtCustomName = null;
		private ToolStripButton Browse = null;
		private ToolStripButton FlipHorizontally = null;
		private ToolStripButton FlipVertically = null;
		private Timer LoadTimer = null;
		private ToolStripSeparator IS_Sep = null;

		// Used for rendering
		private Point _smallStampOffset = new Point(0, 0);
		private Rectangle _stampBounds = Rectangle.Empty;
		private bool _loading = false;
		private Point _offset;

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
			this.ID = (int)ToolID.ImageStamp;
			this.Name = "Image Stamp";
			this.ToolBoxImage = ElfRes.imagestamp;
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
				StampSize.SelectedIndexChanged += new EventHandler(this.StampSize_SelectedIndexChanged);
				Browse.Click += new EventHandler(this.Browse_Click);
				FlipHorizontally.Click += new EventHandler(this.FlipHorizontally_Click);
				FlipVertically.Click += new EventHandler(this.FlipVertically_Click);
				LoadTimer.Tick += new EventHandler(this.LoadTimer_Tick);

				foreach (ToolStripMenuItem Item in SelectStampDD.DropDownItems)
				{
					Item.Click += new EventHandler(ImageStamp_Click);
				}
			}
			else
			{
				StampSize.SelectedIndexChanged -=StampSize_SelectedIndexChanged;
				Browse.Click -=Browse_Click;
				FlipHorizontally.Click -=FlipHorizontally_Click;
				FlipVertically.Click -=FlipVertically_Click;
				LoadTimer.Tick -= LoadTimer_Tick;

				foreach (ToolStripMenuItem Item in SelectStampDD.DropDownItems)
				{
					Item.Click -= ImageStamp_Click;
				}
			}
			base.AttachEvents(attach);
		}

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

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			SelectStampDD = (ToolStripDropDownButton)GetItem<ToolStripDropDownButton>("IS_SelectStamp");
			StampSize = (ToolStripComboBox)GetItem<ToolStripComboBox>("IS_Size");
			txtCustomName = (ToolStripTextBox)GetItem<ToolStripTextBox>("IS_txtCustomFilename");
			Browse = (ToolStripButton)GetItem<ToolStripButton>("IS_Import");
			FlipHorizontally = (ToolStripButton)GetItem<ToolStripButton>("IS_FlipHorizonally");
			FlipVertically = (ToolStripButton)GetItem<ToolStripButton>("IS_FlipVertically");
			StampSizeLabel = (ToolStripLabel)GetItem<ToolStripLabel>("_IS_Size");
			CustomNameLabel = (ToolStripLabel)GetItem<ToolStripLabel>("_IS_Filename");
			IS_Sep = (ToolStripSeparator)GetItem<ToolStripSeparator>("IS_Sep");

			LoadTimer = _toolStrip_Form.ToolTimer;

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
			SetControls(FindDropMenuItemFromValue(SelectStampDD, (int)_imageStamp));
			StampSize.SelectedIndex = (int)_imageStampSize;
			
			txtCustomName.Text = _customFilename;
			txtCustomName.ToolTipText = txtCustomName.Text;
		}

		/// <summary>
		/// Load the ImageStamp, move it to the proper Channel
		/// </summary>
		private void LoadImageStamp()
		{
			// First, clear out the image stamp Channel
			_workshop.ImageStampChannel.Empty();

			if (_imageStampSize == ImageStampSize.VerySmall)
			{
				// Move it away from the cursor, which is 16x16 pixels
				Bitmap CursorBmp = ElfRes.imagestamp;
				Point CursorSize = new Point(CursorBmp.Size.Width, CursorBmp.Size.Height);
				CursorBmp = null;
				_smallStampOffset = _workshop.CalcLatticePoint(CursorSize);
			}
			else
			{
				_smallStampOffset = new Point(0, 0);
			}
			_offset = new Point(0, 0);
			_offset.X -= _stampBounds.Width / 2;
			_offset.Y -= _stampBounds.Height / 2;

			Bitmap Stamp = GetImageStampBitmap();

			// Next get the bitmap for this stamp and load it in
			_workshop.ImageStampChannel.Stamp = Stamp;
			_stampBounds = _workshop.ImageStampChannel.GetBounds();

			#if DEBUG
				Workshop.ExposePane(Stamp, Panes.ImageStamp);
			#endif

			Stamp = null;
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
		}

		/// <summary>
		/// Canvas MouseMove event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override bool MouseMove(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (Profile == null)
				return false;

			Point Offset = latticePoint;

			Offset.Offset(_offset);

			//Offset.Offset(_smallStampOffset);
			//Offset.X -= _stampBounds.Width / 2;
			//Offset.Y -= _stampBounds.Height / 2;
			_workshop.ImageStampChannel.Origin = Offset;

			Profile.Refresh();
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
			if (_loading)
				return false;

			if (!base.MouseUp(buttons, latticePoint, actualCanvasPoint))
				return false;

			Point Offset = latticePoint;

			// Back out the earlier offset from the cursor
			Offset.Offset(_offset.X, _offset.Y);

			// paste the image stamp image into the lattice buffer.
			_latticeBufferGraphics.DrawImage(_workshop.ImageStampChannel.Stamp, Offset);

			Profile.Channels.PopulateChannelFromBitmap(_latticeBuffer, Profile.Channels.Active);
			Profile.Refresh();

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
		/// Sets the toolstrip controls based on the imagestamp selected
		/// </summary>
		/// <param name="menu"></param>
		private void SetControls(ToolStripMenuItem menu)
		{
			SelectStampDD.Image = menu.Image;
			SelectStampDD.Text = menu.Text;
			SelectStampDD.ToolTipText = menu.ToolTipText;

			foreach (ToolStripMenuItem Item in SelectStampDD.DropDownItems)
			{
				Item.Checked = (Item == menu);
			}

			//_imageStamp = EnumHelper.GetEnumFromValue<ImageStamp>(Convert.ToInt32(Menu.Tag));
			_imageStamp = (ImageStamp)menu.Tag;

			bool IsCustom = (_imageStamp == ImageStamp.Custom);

			StampSize.Visible = !IsCustom;
			StampSizeLabel.Visible = !IsCustom;
			IS_Sep.Visible = IsCustom;
			CustomNameLabel.Visible = IsCustom;
			txtCustomName.Visible = IsCustom;
			Browse.Visible = IsCustom;
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// For this tool, it will set a flag in the Workshop object to indicate to show the ImageStamp Channel
		/// </summary>
		public override void OnSelected()
		{
			base.OnSelected();

			LoadTimer.Enabled = false;
			LoadTimer.Interval = 100;

			LoadImageStamp();
			_workshop.ShowImageStamp = true;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		public override void OnUnselected()
		{
			_workshop.ShowImageStamp = false;
			LoadTimer.Enabled = false;
			LoadTimer.Tick -= this.LoadTimer_Tick;
			if (Profile != null)
				Profile.Refresh();
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			SelectStampDD = null;
			StampSize = null;
			txtCustomName = null;
			Browse = null;
			FlipHorizontally = null;
			FlipVertically = null;
		}

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

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
			OpenImageFileDialog.FileName = _customFilename;
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

			txtCustomName.Text = Filename;
			txtCustomName.ToolTipText = txtCustomName.Text;

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
			SetControls((ToolStripMenuItem)sender);
			bool IsCustom = (_imageStamp == ImageStamp.Custom);

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

		#endregion [ ToolStrip Event Delegates ]
			
	}
}
