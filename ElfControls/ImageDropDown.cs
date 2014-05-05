using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     ComboBox modified to display an image on the left side of the text.
    /// </summary>
    public class ImageDropDown : ComboBox {
        #region [ Constructors ]

        public ImageDropDown() {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        #endregion [ Constructors ]

        #region [ Events ]

        protected override void OnDrawItem(DrawItemEventArgs e) {
            e.DrawBackground();

            if ((e.Index < Items.Count) && (Items.Count > 0) && (e.Index >= 0)) {
                string Text = Items[e.Index].ToString();
                Image Bitmap = null;
                if (Items[e.Index] is ImageListItem) {
                    Bitmap = ((ImageListItem) Items[e.Index]).Image;
                }
                int ItemHeight = 0;

                if (e.Index != -1) {
                    SizeF TextSize = e.Graphics.MeasureString(Text, Font);

                    if (Bitmap != null) {
                        ItemHeight = (int) (Math.Max(TextSize.Height, Bitmap.Height));
                    }
                    else {
                        ItemHeight = (int) TextSize.Height;
                    }

                    // Draw the image in combo box using its bound and ItemHeight 
                    if (Bitmap != null) {
                        if (Enabled) {
                            e.Graphics.DrawImage(Bitmap, e.Bounds.X, e.Bounds.Y, Bitmap.Height, Bitmap.Height);
                        }
                        else {
                            ControlPaint.DrawImageDisabled(e.Graphics, Bitmap, e.Bounds.X, e.Bounds.Y, BackColor);
                        }
                    }

                    var sf = new StringFormat();
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    Color DisabledColor = ControlPaint.Light(SystemColors.WindowText);
                    // We need to draw the item as string because we made drawmode to ownervariable
                    if (Enabled) {
                        e.Graphics.DrawString(Text, Font, Brushes.Black,
                            new RectangleF(e.Bounds.X + ItemHeight, e.Bounds.Y, DropDownWidth, ItemHeight));
                    }
                    else {
                        ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, Color.Transparent,
                            new RectangleF(e.Bounds.X + ItemHeight, e.Bounds.Y, DropDownWidth, ItemHeight), sf);
                    }
                }
            }
            // draw rectangle over the item selected 
            e.DrawFocusRectangle();
        }


        protected override void OnMeasureItem(MeasureItemEventArgs e) {
            string Text = string.Empty;
            Image Bitmap = null;
            SizeF TextSize = SizeF.Empty;
            float Width = 0;
            float MaxWidth = 0;

            using (Graphics g = CreateGraphics()) {
                for (int i = 0; i < Items.Count; i++) {
                    Text = Items[e.Index].ToString();
                    if (Items[e.Index] is ImageListItem) {
                        Bitmap = ((ImageListItem) Items[e.Index]).Image;
                    }
                    else {
                        Bitmap = null;
                    }

                    TextSize = e.Graphics.MeasureString(Text, Font);
                    Width = TextSize.Width;
                    if (Bitmap != null) {
                        Width += Bitmap.Width;
                    }
                    MaxWidth = Math.Max(Width, MaxWidth);

                    if (i == e.Index) {
                        e.ItemHeight = (int) Math.Max(TextSize.Height, (Bitmap == null) ? 0 : Bitmap.Height);
                        e.ItemWidth = (int) Math.Ceiling(Width);
                    }
                }
                //DropDownWidth = (int)Math.Ceiling(MaxWidth) + 20;
            }
        }

        #endregion [ Events ]
    }
}