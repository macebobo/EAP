using ElfControls;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElfRes = ElfCore.Properties.Resources;

namespace ElfCore.Forms
{
	public partial class BackgroundSettings : Form
	{

		#region [ Constants ]

		private const string PERCENT_FORMAT = "0%";
		private const string DEGREE_SIGN = "°";

		#endregion [ Constants ]

		#region [ Private Variables ]

		private BaseProfile _tempProfile;
		private Background _background;
		private Workshop _workshop = Workshop.Instance;
		private int _selected = 0;
		private ListBoxUtil _listBoxUtil = null;
		private bool _loading = false;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Internal property to get the Active Profile.
		/// </summary>
		private BaseProfile Profile
		{
			get { return _workshop.Profile; }
		}
		
		#endregion [ Properties ]

		#region [ Constructors ]

		public BackgroundSettings()
		{
			InitializeComponent();
			_tempProfile = new BaseProfile(_workshop.Profile.ProfileDataLayer.GetType());
			_tempProfile.UnclipCanvasWindow();
			_tempProfile.SubstituteCanvas = pctPreview;
			_background = _tempProfile.Background;
			_background.SuppressTempFiles = true;

			_background.SuppressEvents = true;
			_background.Image = Profile.Background.Image;
			_background.Filename = Profile.Background.Filename;
			_background.Brightness = Profile.Background.Brightness;
			_background.Saturation = Profile.Background.Saturation;
			_background.Hue = Profile.Background.Hue;
			_background.OverlayGrid = Profile.Background.OverlayGrid;
			_background.WallpaperStyle = Profile.Background.WallpaperStyle;
			_background.WallpaperAnchor = Profile.Background.WallpaperAnchor;
			_background.Color = Profile.Background.Color;
			_background.GridColor = Profile.Background.GridColor;
			_background.SuppressEvents = false;
			_background.SaveEncodedImage = Profile.Background.SaveEncodedImage;
			pctPreview.BackColor = _background.Color;

			cddBackground.CustomColors = _workshop.CustomColors;
			cddGrid.CustomColors = _workshop.CustomColors;

			cmdClear.Image = ImageHandler.AddAnnotation(ElfRes.background, Annotation.Delete);
			_listBoxUtil = new ListBoxUtil();

			cboPicturePosition.Items.Add(new ImageListItem("Fill", (int)WallpaperStyle.Fill, ElfRes.fillMode_fill));
			cboPicturePosition.Items.Add(new ImageListItem("Fit", (int)WallpaperStyle.Fit, ElfRes.fillMode_fit));
			cboPicturePosition.Items.Add(new ImageListItem("Stretch", (int)WallpaperStyle.Stretch, ElfRes.fillMode_stretch));
			cboPicturePosition.Items.Add(new ImageListItem("Tile", (int)WallpaperStyle.Tile, ElfRes.fillMode_tile));
			cboPicturePosition.Items.Add(new ImageListItem("Center", (int)WallpaperStyle.Center, ElfRes.fillMode_center));

			cboAnchor.Items.Add(new ImageListItem("Top Left", (int)(AnchorStyles.Top | AnchorStyles.Left), ElfRes.anchor_topleft));
			cboAnchor.Items.Add(new ImageListItem("Top Right", (int)(AnchorStyles.Top | AnchorStyles.Right), ElfRes.anchor_topRight));
			cboAnchor.Items.Add(new ImageListItem("Bottom Left", (int)(AnchorStyles.Bottom | AnchorStyles.Left), ElfRes.anchor_bottomLeft));
			cboAnchor.Items.Add(new ImageListItem("Bottom Right", (int)(AnchorStyles.Bottom | AnchorStyles.Right), ElfRes.anchor_bottomRight));

			CreatePreview();
			PopulateControlsFromObject();
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Populate the temp Profile with some channels.
		/// </summary>
		private void CreatePreview()
		{
			Channel Channel = null;

			// Add Cells to make up a mini tree
			List<Point> MiniTree = new List<Point>();
			MiniTree.Add(new Point(0, 9));
			MiniTree.Add(new Point(1, 7));
			MiniTree.Add(new Point(1, 9));
			MiniTree.Add(new Point(2, 5));
			MiniTree.Add(new Point(2, 7));
			MiniTree.Add(new Point(2, 8));
			MiniTree.Add(new Point(2, 9));
			MiniTree.Add(new Point(3, 1));
			MiniTree.Add(new Point(3, 3));
			MiniTree.Add(new Point(3, 4));
			MiniTree.Add(new Point(3, 5));
			MiniTree.Add(new Point(3, 6));
			MiniTree.Add(new Point(3, 7));
			MiniTree.Add(new Point(3, 8));
			MiniTree.Add(new Point(3, 9));
			MiniTree.Add(new Point(4, 0));
			MiniTree.Add(new Point(4, 1));
			MiniTree.Add(new Point(4, 2));
			MiniTree.Add(new Point(4, 3));
			MiniTree.Add(new Point(4, 4));
			MiniTree.Add(new Point(4, 5));
			MiniTree.Add(new Point(4, 6));
			MiniTree.Add(new Point(4, 7));
			MiniTree.Add(new Point(4, 8));
			MiniTree.Add(new Point(4, 9));
			MiniTree.Add(new Point(5, 1));
			MiniTree.Add(new Point(5, 3));
			MiniTree.Add(new Point(5, 4));
			MiniTree.Add(new Point(5, 5));
			MiniTree.Add(new Point(5, 6));
			MiniTree.Add(new Point(5, 7));
			MiniTree.Add(new Point(5, 8));
			MiniTree.Add(new Point(5, 9));
			MiniTree.Add(new Point(6, 5));
			MiniTree.Add(new Point(6, 7));
			MiniTree.Add(new Point(6, 8));
			MiniTree.Add(new Point(6, 9));
			MiniTree.Add(new Point(7, 7));
			MiniTree.Add(new Point(7, 9));
			MiniTree.Add(new Point(8, 9));

			// Create a test profile and populate it with some test channels to create the image for the preview window
			_tempProfile.Scaling.CellSize = 7;
			_tempProfile.Scaling.Zoom = 1f;

			_tempProfile.Scaling.ShowGridLines = Profile.Scaling.ShowGridLines;
			_background.Color = Profile.Background.Color;
			_background.GridColor = Profile.Background.GridColor;
			_background.OverlayGrid = Profile.Background.OverlayGrid;
			_background.Brightness = Profile.Background.Brightness;
			_background.Image = Profile.Background.Image;
			_background.Filename = Profile.Background.Filename;

			_tempProfile.Scaling.LatticeSize = _workshop.GetSizeInCells(pctPreview.Size, _tempProfile);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Red;
			Channel.Origin = new Point(10, 3);
			Channel.Paint(MiniTree);
			Channel.IsSelected = true;
			Channel.SuppressEvents = false;
			Channel.Name = "Red Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Yellow;
			Channel.Origin = new Point(24, 3);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Yellow Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Green;
			Channel.Origin = new Point(3, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Green Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.Blue;
			Channel.Origin = new Point(17, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "Blue Tree";
			_tempProfile.Channels.Add(Channel);

			Channel = new Channel();
			Channel.SuppressEvents = true;
			Channel.SequencerColor = Color.White;
			Channel.Origin = new Point(31, 15);
			Channel.Paint(MiniTree);
			Channel.SuppressEvents = false;
			Channel.Name = "White Tree";
			_tempProfile.Channels.Add(Channel);

			_background.Set();

			Channel = null;
		}

		/// <summary>
		/// Update the controls based on the values stored within the Background object.
		/// </summary>
		private void PopulateControlsFromObject()
		{
			_loading = true;

			txtFilename.Text = _background.Filename;
			bool ImagePresent = (_background.Image != null);

			cmdSave.Enabled = ImagePresent;
			cmdClear.Enabled = ImagePresent;

			grpAdjust.Enabled = ImagePresent;
			cboPicturePosition.Enabled = ImagePresent;
			_cboPicturePosition.Enabled = cboPicturePosition.Enabled;
			cboAnchor.Enabled = ImagePresent && (_background.WallpaperStyle == WallpaperStyle.Fill);
			_cboAnchor.Enabled = cboAnchor.Enabled;
			hslBrightness.Value = (_background.Brightness * 100);
			hslSaturation.Value = (_background.Saturation * 100);
			hslHue.Value = (_background.Hue * 360);
			chkSaveEncoded.Checked = _background.SaveEncodedImage;
			_listBoxUtil.Set(cboPicturePosition, (int)_background.WallpaperStyle);
			_listBoxUtil.Set(cboAnchor, (int)_background.WallpaperAnchor);
			chkShowGrid.Checked = _background.OverlayGrid;
			cddBackground.Color = _background.Color;
			cddGrid.Color = _background.GridColor;

			if (_tempProfile.Scaling.ShowGridLines.GetValueOrDefault(true))
			{
				chkShowGrid.Enabled = true;
				cddGrid.Enabled = chkShowGrid.Checked;
			}
			else
			{
				chkShowGrid.Enabled = false;
				cddGrid.Enabled = false;
			}

			_loading = false;

		}

		#endregion [ Methods ]

		#region [ Events ]

		/// <summary>
		/// Save the values stored in the temp Profile into the active Profile and create an Undo event.
		/// </summary>
		private void cmdOk_Click(object sender, EventArgs e)
		{
			Profile.Background.SuppressEvents = true;
			Profile.Background.Image = _background.Image;
			Profile.Background.Filename = _background.Filename;
			Profile.Background.Brightness = _background.Brightness;
			Profile.Background.Saturation = _background.Saturation;
			Profile.Background.Hue = _background.Hue;
			Profile.Background.OverlayGrid = _background.OverlayGrid;
			Profile.Background.Color = _background.Color;
			Profile.Background.GridColor = _background.GridColor;
			Profile.Background.SuppressEvents = false;
			Profile.Background.WallpaperStyle = _background.WallpaperStyle;
			Profile.Background.WallpaperAnchor = _background.WallpaperAnchor;
			Profile.Background.SaveEncodedImage = chkSaveEncoded.Checked;
			Profile.Background.Set();
			Profile.Dirty = true;

			Profile.SaveUndo("Background Settings");
		}

		private void cmdSave_Click(object sender, EventArgs e)
		{
			_background.Filename = Workshop.SaveBitmap(_background.Image, _background.Filename, "Save Background Image");
			PopulateControlsFromObject();
		}

		private void cmdClear_Click(object sender, EventArgs e)
		{
			_background.Clear(true);
			PopulateControlsFromObject();
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			if (OpenImageFileDialog.ShowDialog() != DialogResult.OK)
				return;

			if (!_background.LoadFromFile(OpenImageFileDialog.FileName))
			{
				MessageBox.Show("Invalid Image File", @"Load Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			PopulateControlsFromObject();
		}

		private void hslBrightness_Changed(object sender, EventArgs e)
		{
			float Brightness = (float)(Math.Round(hslBrightness.Value) / 100);
			if (!_loading)
			{
				_background.Brightness = Brightness;
				_background.Set();
			}
			lblBrightness.Text = Brightness.ToString(PERCENT_FORMAT);
			txtBrightness.Text = hslBrightness.Value.ToString();
		}

		private void hslSaturation_Changed(object sender, EventArgs e)
		{
			float Saturation = (float)(Math.Round(hslSaturation.Value) / 100);
			if (!_loading)
			{
				_background.Saturation = Saturation;
				_background.Set();
			}
			lblSaturation.Text = Saturation.ToString(PERCENT_FORMAT);
			txtSaturation.Text = hslSaturation.Value.ToString();
		}

		private void hslHue_Changed(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_background.Hue = (float)(Math.Round(hslHue.Value) / 360);
				_background.Set();
			}
			lblHue.Text = ((int)hslHue.Value).ToString() + DEGREE_SIGN;
			txtHue.Text = ((int)hslHue.Value).ToString();
		}

		/// <summary>
		/// Indicates whether a grid should be overlayed over the background image or color
		/// </summary>
		private void chkShowGrid_CheckedChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_background.OverlayGrid = chkShowGrid.Checked;
				_background.Set();
				PopulateControlsFromObject();
			}
		}

		/// <summary>
		/// Set the color used to draw the grid overlay
		/// </summary>
		private void cmdGridColor_Clickx(object sender, EventArgs e)
		{
			ChannelColorDialog.Color = _background.GridColor;
			ChannelColorDialog.AllowFullOpen = true;
			ChannelColorDialog.AnyColor = true;
			if (ChannelColorDialog.ShowDialog() == DialogResult.Cancel)
				return;

			_background.GridColor = ChannelColorDialog.Color;
			_background.Set();
			PopulateControlsFromObject();
		}
		
		/// <summary>
		/// Paints the fake background with fake channels
		/// </summary>
		private void pctPreview_Paint(object sender, PaintEventArgs e)
		{
			Matrix MoveMatrix = null;
			GraphicsPath Path = null;
			Point Offset;
 
			try
			{
				Channel Channel = null;
				for (int i = 0; i < _tempProfile.Channels.Count; i++)
				{
					Channel = _tempProfile.Channels[i];

					// If the Channel is one of the selected Channels, do not draw it here in this block
					if (Channel.IsSelected)
						continue;

					using (SolidBrush ChannelBrush = new SolidBrush(Color.FromArgb(_workshop.UI.InactiveChannelAlpha, Channel.GetColor())))
					{
						Path = (GraphicsPath)Channel.GetGraphicsPath().Clone();

						Offset = _workshop.CalcCanvasPoint(Channel.Origin, _tempProfile);
						MoveMatrix = new Matrix();
						MoveMatrix.Translate(Offset.X, Offset.Y);
						Path.Transform(MoveMatrix);
						MoveMatrix.Dispose();

						e.Graphics.FillPath(ChannelBrush, Path);
					}
				}

				Offset = Point.Empty;

				// Draw the Selected Channels on top of the unselected ones (for clarity)
				if (_tempProfile.Channels.Selected.Count > 0)
				{
					foreach (Channel SelectedChannel in _tempProfile.Channels.Selected.OrderByDescending())
					{
						using (SolidBrush ChannelBrush = new SolidBrush(SelectedChannel.GetColor()))
						{
							Path = (GraphicsPath)SelectedChannel.GetGraphicsPath().Clone();

							Offset = _workshop.CalcCanvasPoint(SelectedChannel.Origin, _tempProfile);
							MoveMatrix = new Matrix();
							MoveMatrix.Translate(Offset.X, Offset.Y);
							Path.Transform(MoveMatrix);
							MoveMatrix.Dispose();

							e.Graphics.FillPath(ChannelBrush, Path);
						}
					}
				}
			}
			finally
			{
				Path = null;
				MoveMatrix = null;
			}
		}

		/// <summary>
		/// Update the display.
		/// </summary>
		private void tmrSelected_Tick(object sender, EventArgs e)
		{
			_selected++;
			if (_selected % 5 == 0)
				_selected = 0;
			_tempProfile.Channels.Select(_selected);
			pctPreview.Refresh();
		}

		/// <summary>
		/// Allows the user to save the modified image to disk as a seperate file.
		/// </summary>
		private void cmdSaveModified_Click(object sender, EventArgs e)
		{
			Workshop.SaveBitmap(_background.CompositeImage, string.Empty, "Save Modified Background Image");
		}

		/// <summary>
		/// Grid Color selected
		/// </summary>
		private void cddGrid_ColorChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_background.GridColor = cddGrid.Color;
				_background.Set();
				PopulateControlsFromObject();
			}
		}

		/// <summary>
		/// Background color selected
		/// </summary>
		private void cddBackground_ColorChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_background.Color = cddBackground.Color;
				_background.Set();
				PopulateControlsFromObject();
			}
			pctPreview.BackColor = _background.Color;
		}

		/// <summary>
		/// Background wallpaper style selected.
		/// </summary>
		private void cboPicturePosition_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_background.WallpaperStyle = EnumHelper.GetEnumFromValue<WallpaperStyle>(Int32.Parse(_listBoxUtil.GetKey(cboPicturePosition)));
				cboAnchor.Enabled = (_background.WallpaperStyle == WallpaperStyle.Fill);
				_cboAnchor.Enabled = cboAnchor.Enabled;
				_background.Set();
			}
		}

		private void cboAnchor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				if (_background.WallpaperStyle == WallpaperStyle.Fill)
				{
					_background.WallpaperAnchor = Background.GetAnchorEnum(Int32.Parse(_listBoxUtil.GetKey(cboAnchor)));
					_background.Set();
				}
			}
		}

		private void _tbHue_DoubleClick(object sender, EventArgs e)
		{
			hslHue.Value = 0;
		}

		private void _tbSaturation_Click(object sender, EventArgs e)
		{
			hslSaturation.Value = 100;
		}

		private void _tbBrightness_Click(object sender, EventArgs e)
		{
			hslBrightness.Value = 0;
		}

		private void lblHue_Click(object sender, EventArgs e)
		{
			lblHue.Visible = false;
			txtHue.Visible = true;
			txtHue.Focus();
			txtHue.SelectAll();
		}

		private void lblSaturation_Click(object sender, EventArgs e)
		{
			lblSaturation.Visible = false;
			txtSaturation.Visible = true;
			txtSaturation.Focus();
			txtSaturation.SelectAll();
		}

		private void lblBrightness_Click(object sender, EventArgs e)
		{
			lblBrightness.Visible = false;
			txtBrightness.Visible = true;
			txtBrightness.Focus();
			txtBrightness.SelectAll();
		}

		private void txtHue_Leave(object sender, EventArgs e)
		{
			txtHue.Visible = false;
			lblHue.Visible = true;
		}

		private void txtBrightness_Leave(object sender, EventArgs e)
		{
			txtBrightness.Visible = false;
			lblBrightness.Visible = true;
		}

		private void txtSaturation_Leave(object sender, EventArgs e)
		{
			txtSaturation.Visible = false;
			lblSaturation.Visible = true;
		}

		/// <summary>
		/// Only allow digits and the minus sign in the textbox.
		/// </summary>
		private void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) ||
				char.IsControl(e.KeyChar) ||
				(e.KeyChar == '-') ||
				(e.KeyChar == '+'))
			{
				// We like these, do nothing
				e.Handled = false;
			}
			else
				e.Handled = true;
		}

		private void txtBrightness_TextChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			if (txtBrightness.Text.Length == 0)
				return;
			double Value = Convert.ToDouble(txtBrightness.Text);

			if (Value < -100)
			{
				_loading = true;
				Value = -100;
				txtBrightness.Text = "-100";
				_loading = false;
			}
			else if (Value > 100)
			{
				_loading = true;
				Value = 100;
				txtBrightness.Text = "100";
				_loading = false;
			}

			hslBrightness.Value = Value;
		}

		private void txtHue_TextChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			if (txtHue.Text.Length == 0)
				return;
			double Value = Convert.ToDouble(txtHue.Text);

			if (Value < -180)
			{
				_loading = true;
				Value = -180;
				txtHue.Text = "-180";
				_loading = false;
			}
			else if (Value > 180)
			{
				_loading = true;
				Value = 180;
				txtHue.Text = "180";
				_loading = false;
			}

			hslHue.Value = Value;
		}

		private void txtSaturation_TextChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			if (txtSaturation.Text.Length == 0)
				return;
			double Value = Convert.ToDouble(txtSaturation.Text);

			if (Value < -200)
			{
				_loading = true;
				Value = -200;
				txtSaturation.Text = "-200";
				_loading = false;
			}
			else if (Value > 100)
			{
				_loading = true;
				Value = 100;
				txtSaturation.Text = "100";
				_loading = false;
			}

			hslSaturation.Value = Value;
		}

		private void cmdReset_Click(object sender, EventArgs e)
		{
			hslHue.Value = 0;
			hslBrightness.Value = 0;
			hslSaturation.Value = 100;
		}

		#endregion [ Events ]

	}
}

