using ElfCore.Controllers;
using ElfCore.Interfaces;
using ElfCore.Util;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using CanvasPoint = System.Drawing.Point;
using ElfRes = ElfCore.Properties.Resources;
using LatticePoint = System.Drawing.Point;

namespace ElfCore.Tools
{
	[ElfEditTool("Text")]
	public class TextTool : BaseTool, ITool
	{
		#region [ Private Variables ]

		// Settings from the ToolStrip
		private FontData _selectedFontData = null;
		private string _fontName = string.Empty;
		private float _fontSize = 0f;
		private FontStyle _fontStyle = FontStyle.Regular;
		private string _text = string.Empty;

		private ListBoxUtil _listboxUtil = new ListBoxUtil();

		// Controls from ToolStrip
		private ToolStripComboBox cboFontName = null;
		private ToolStripComboBox cboFontSize = null;
		private ToolStripButton btnBold = null;
		private ToolStripButton btnItalic = null;
		private ToolStripButton btnUnderline = null;
		private ToolStripButton txtEnterText = null;

		private Rectangle _stampBounds = Rectangle.Empty;

		#endregion [ Private Variables ]

		#region [ Constants ]

		private const string FONT_NAME = "FontName";
		private const string FONT_SIZE = "Size";
		private const string FONT_STYLE = "Style";
		private const string LAST_TEXT = "LastText";

		#endregion [ Constants ]
			
		#region [ Constructors ]

		public TextTool()
		{
			this.ID = (int)ToolID.Text;
			this.Name = "Text";
			this.ToolBoxImage = ElfRes.text;
			this.Cursor = Cursors.IBeam;
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
				cboFontName.SelectedIndexChanged += new EventHandler(FontName_SelectedIndexChanged);
				cboFontSize.SelectedIndexChanged += new EventHandler(FontSize_SelectedIndexChanged);
				cboFontSize.KeyPress += new KeyPressEventHandler(FontSize_KeyPress);
				btnBold.Click += new EventHandler(FontStyle_Click);
				btnItalic.Click += new EventHandler(FontStyle_Click);
				btnUnderline.Click += new EventHandler(FontStyle_Click);
				txtEnterText.Click += new EventHandler(EnterText_Click);
			}
			else
			{
				cboFontName.SelectedIndexChanged -= FontName_SelectedIndexChanged;
				cboFontSize.SelectedIndexChanged -= FontSize_SelectedIndexChanged;
				cboFontSize.KeyPress -= FontSize_KeyPress;
				btnBold.Click -= FontStyle_Click;
				btnItalic.Click -= FontStyle_Click;
				btnUnderline.Click -= FontStyle_Click;
				txtEnterText.Click -= EnterText_Click;
			}
			base.AttachEvents(attach);
		}

		/// <summary>
		/// Clears out the contents of the ImageStamp Channel
		/// </summary>
		public override void Cancel()
		{
			_workshop.ImageStampChannel.Empty();
			Profile.Refresh();
		}

		/// <summary>
		/// Based on the settings, create a Font object. Use that with the Text entered to create the text on the ImageStamp Channel.
		/// </summary>
		private void CreateTextAndSetStamp()
		{
			Font Font = CreateFont();
			SizeF StringSize;

			if (Font == null)
				Font = CreateFont("Arial");

			// Determine the dimensions of the Text
			using (Graphics g = Profile.GetCanvasGraphics())
			{
				StringSize = g.MeasureString(_text, Font);
			}

			// Create the stamp
			Bitmap TextStamp = new Bitmap((int)StringSize.Width + 10, (int)StringSize.Height + 10);
			using (Graphics g = Graphics.FromImage(TextStamp))
			{
				// Don't want to anti-alias this, 1 bit only
				//g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
				g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

				// White text on a black background
				g.Clear(Color.Black);
				g.DrawString(_text, Font, Brushes.White, new Point(0, 0));

				// Next get the bitmap for this stamp and load it in
				_workshop.ImageStampChannel.LatticeBuffer = TextStamp;
				_stampBounds = _workshop.ImageStampChannel.GetBounds();

				#if DEBUG
					Workshop.ExposePane(TextStamp, Panes.ImageStamp);
				#endif

				Profile.Refresh();
			}

			if (Font != null)
			{
				Font.Dispose();
				Font = null;
			}
			if (TextStamp != null)
			{
				TextStamp.Dispose();
				TextStamp = null;
			}
		}

		/// <summary>
		/// Create a Font object based on the settings 
		/// </summary>
		private Font CreateFont()
		{
			return CreateFont(_fontName);
		}

		/// <summary>
		/// Create a Font object, using the font name passed in.
		/// </summary>
		/// <param name="fontName">Name of the font to use</param>
		private Font CreateFont(string fontName)
		{
			if ((fontName.Length > 0) && (_fontSize > 0f))
				return new Font(fontName, _fontSize, _fontStyle);
			else
				return null;
		}

		/// <summary>
		/// Some fonts do not support certain styles. Brush Script MT is Italic only.
		/// In these cases, we will need to unclick and disable the invalid style options.
		/// </summary>
		private void DetermineAvailableStylesForFont()
		{
			if (_selectedFontData == null)
				return;

			btnBold.Enabled = (_selectedFontData.DoesBold);
			btnItalic.Enabled = (_selectedFontData.DoesItalic);
			btnUnderline.Enabled = (_selectedFontData.DoesUnderline);

			if (!btnBold.Enabled)
				btnBold.Checked = false;

			if (!btnItalic.Enabled)
				btnItalic.Checked = false;

			if (!btnUnderline.Enabled)
				btnUnderline.Checked = false;

			if (!_selectedFontData.DoesRegular)
			{
				// Find a valid style for this weirdo
				if (btnBold.Enabled)
					btnBold.Checked = true;

				else if (btnItalic.Enabled)
					btnItalic.Checked = true;
	
				else if (btnUnderline.Enabled)
					btnUnderline.Checked = true;
			}

			SetFontStyle();
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

			// Load the Settings values
			_fontName = LoadValue(FONT_NAME, "Arial");
			_fontSize = LoadValue(FONT_SIZE, 8f);
			_fontStyle = (FontStyle)LoadValue(FONT_STYLE, (int)FontStyle.Regular);
			_text = LoadValue(LAST_TEXT, string.Empty);

			if ((ToolStripLabel)GetItem<ToolStripLabel>(1) == null)
				return;

			// Get a pointer to the controls on the toolstrip that belongs to us.
			cboFontName = (ToolStripComboBox)GetItem<ToolStripComboBox>(1);
			cboFontSize = (ToolStripComboBox)GetItem<ToolStripComboBox>(2);
			btnBold = (ToolStripButton)GetItem<ToolStripButton>(1);
			btnItalic = (ToolStripButton)GetItem<ToolStripButton>(2);
			btnUnderline = (ToolStripButton)GetItem<ToolStripButton>(3);
			txtEnterText = (ToolStripButton)GetItem<ToolStripButton>(4);

			// Set the initial value for the contol from what we had retrieve from Settings
			LoadFontList();
			if (cboFontName.Items.Count > 0)
			{
				if (!_listboxUtil.Set(cboFontName, _fontName))
					cboFontName.SelectedIndex = 0;
			}

			cboFontSize.Text = _fontSize.ToString() + " pt";
			btnBold.Checked = ((_fontStyle & FontStyle.Bold) == FontStyle.Bold);
			btnItalic.Checked = ((_fontStyle & FontStyle.Italic) == FontStyle.Italic);
			btnUnderline.Checked = ((_fontStyle & FontStyle.Underline) == FontStyle.Underline);

			DetermineAvailableStylesForFont();
		}

		/// <summary>
		/// Create the list of fonts available on this system, populate the combo box on the settings toolstrip
		/// </summary>
		private void LoadFontList()
		{
			cboFontName.BeginUpdate();
			cboFontName.Items.Clear();

			FontData fData = null;

			InstalledFontCollection ifc = new InstalledFontCollection();
			for (int i = 0; i < ifc.Families.Length; i++)
			{
				fData = new FontData(ifc.Families[i]);
				_listboxUtil.Add(cboFontName, new ListBoxUtil.Item(fData.Name, fData.Name, fData));
			}

			cboFontName.EndUpdate();

			if (ifc != null)
			{
				ifc.Dispose();
				ifc = null;
			}
		}

		/// <summary>
		/// Canvas MouseDown event was fired
		/// </summary>
		/// <param name="buttons">From the MouseEventArgs, indicates which mouse button was clicked</param>
		/// <param name="latticePoint">Point on the picture box (in Cells) where the mouse event happened</param>
		/// <param name="actualCanvasPoint">Point on the picture box (in Pixel) where the mouse event happened</param>
		public override void MouseDown(MouseButtons buttons, LatticePoint latticePoint, CanvasPoint actualCanvasPoint)
		{
			if (!_workshop.ImageStampChannel.HasLatticeData)
			{
				_isMouseDown = false;
				PromptForText();
			}
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

			Offset.X -= _stampBounds.Width / 2;
			Offset.Y -= _stampBounds.Height / 2;
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
			//// Clean up
			//PostDrawCleanUp();

			//Point Offset = latticePoint;
			//List<Point> Cells = _workshop.ImageStampChannel.Lattice;
			//Rectangle Bounds = _workshop.ImageStampChannel.GetBounds();
			//Point Origin = _workshop.ImageStampChannel.Origin;
			//Point Cell;

			//Origin.X -= Bounds.Width / 2;
			//Origin.Y -= Bounds.Height / 2;

			//for (int i = 0; i < Cells.Count; i++)
			//{
			//    Cell = Cells[i];
			//    Cell.Offset(Origin);
			//    Profile.Channels.Active.Lattice.Add(Cell);
			//}
			//Cells = null;

			//// Clear out the ImageStamp Channel once this text is set on its Channel.
			//// Text is a one time stamp.
			//_workshop.ImageStampChannel.ClearLattice();

			//PostDrawCleanUp();
			//return true;

			Point Offset = latticePoint;

			Offset.X -= _stampBounds.Width / 2;
			Offset.Y -= _stampBounds.Height / 2;
			_workshop.ImageStampChannel.Origin = Offset;

			List<Point> Cells = null;

			foreach (Channels.BaseChannel Channel in Profile.Channels.Selected)
			{
				Cells = Workshop.CloneList<Point>(_workshop.ImageStampChannel.Lattice);
				Channel.Paint(Cells, Offset);
			}

			PostDrawCleanUp();

			return true;
		}

		/// <summary>
		/// Prompts the user to input the text he wants stamped onto the Canvas.
		/// </summary>
		private void PromptForText()
		{
			string Text = Interaction.InputBox("Please enter your text here:", "Text", _text);
			if (Text.Length > 0)
			{
				_text = Text;
				CreateTextAndSetStamp();
			}
		}

		/// <summary>
		/// Save this toolstrip settings back to the Settings object.
		/// </summary>
		public override void SaveSettings()
		{
			SaveValue(FONT_NAME, _fontName);
			SaveValue(FONT_SIZE, _fontSize);
			SaveValue(FONT_STYLE, (int)_fontStyle);
			SaveValue(LAST_TEXT, _text);
		}

		/// <summary>
		/// Method that fires when this Tool is selected in the ToolBox.
		/// For this tool, it will set a flag in the Workshop object to indicate to show the ImageStamp Channel
		/// </summary>
		public override void OnSelected()
		{
			base.OnSelected();
			_workshop.ImageStampChannel.Empty();
			_workshop.ShowImageStamp = true;
		}

		/// <summary>
		/// Creates the proper font style enumeration value based on which style buttons are currently checked.
		/// </summary>
		private void SetFontStyle()
		{
			FontStyle Style = FontStyle.Regular;

			if (btnBold.Checked && btnBold.Enabled)
				Style |= FontStyle.Bold;

			if (btnItalic.Checked && btnItalic.Enabled)
				Style |= FontStyle.Italic;

			if (btnUnderline.Checked && btnUnderline.Enabled)
				Style |= FontStyle.Underline;

			_fontStyle = Style;
		}

		/// <summary>
		/// Method fires when a different tool is selected, gives the tool the chance to clean up a bit, but not as much as a ShutDown.
		/// </summary>
		public override void OnUnselected()
		{
			_workshop.ShowImageStamp = false;
			if (Profile != null)
				Profile.Refresh();
		}

		/// <summary>
		/// Method fires when we are closing out of the editor, want to clean up all our objects.
		/// </summary>
		public override void ShutDown()
		{
			base.ShutDown();

			// Controls from ToolStrip
			cboFontName = null;
			cboFontSize = null;
			btnBold = null;
			btnItalic = null;
			btnUnderline = null;
			txtEnterText = null;
			_selectedFontData = null;
			_listboxUtil = null;

		}		

		#endregion [ Methods ]

		#region [ ToolStrip Event Delegates ]

		/// <summary>
		/// Ask the user for their text string, then create an image stamp based on it, if the length > 0
		/// </summary>
		private void EnterText_Click(object sender, EventArgs e)
		{
			PromptForText();
		}
	
		/// <summary>
		/// Set the font style based on the combination of buttons clicked.
		/// </summary>
		private void FontStyle_Click(object sender, EventArgs e)
		{
			SetFontStyle();
			CreateTextAndSetStamp();
		}

		/// <summary>
		/// Sets the font's name based on the selected value.
		/// </summary>
		private void FontName_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboFontName.SelectedIndex > -1)
			{
				_selectedFontData = (FontData)_listboxUtil.GetItem(cboFontName).StoredObject;
				if (_selectedFontData != null)
				{
					_fontName = _selectedFontData.Name;
					DetermineAvailableStylesForFont();
				}
			}
			if (_isSelected)
				CreateTextAndSetStamp();
		}

		/// <summary>
		/// Sets the font's size based on the selected value.
		/// </summary>
		private void FontSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!cboFontSize.Text.Contains(" pt"))
				cboFontSize.Text += " pt";

			float Size = 0;
			if (float.TryParse(cboFontSize.Text.Replace(" pt", string.Empty), out Size))
				_fontSize = Size;

			if (_isSelected)
				CreateTextAndSetStamp();
		}

		/// <summary>
		/// Only numbers and the decimal allowed
		/// </summary>
		private void FontSize_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) ||
				(e.KeyChar == '.'))
			{
				// We like these, do nothing
				e.Handled = false;
			}
			else
				e.Handled = true;
		}

		#endregion [ ToolStrip Event Delegates ]

		private class FontData
		{
			#region [ Public Field Variables ]

			public string Name = string.Empty;
			public FontFamily Family = null;

			#endregion [ Public Field Variables ]

			#region [ Properties ]

			public bool DoesRegular
			{
				get { return Family.IsStyleAvailable(FontStyle.Regular); }
			}

			public bool DoesBold
			{
				get { return Family.IsStyleAvailable(FontStyle.Bold); }
			}

			public bool DoesItalic
			{
				get { return Family.IsStyleAvailable(FontStyle.Italic); }
			}

			public bool DoesUnderline
			{
				get { return Family.IsStyleAvailable(FontStyle.Underline); }
			}

			#endregion [ Properties ]

			#region [ Constructors ]

			public FontData()
			{ }

			public FontData(FontFamily family)
				: this()
			{
				this.Family = family;
				this.Name = family.Name;
			}

			#endregion [ Constructors ]

		}

	}
}
