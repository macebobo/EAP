using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     ListBox modified to display an image on the left side of the text.
    /// </summary>
    public sealed class ImageListBox : ListBox {
        #region [ Constructors ]

        public ImageListBox() {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        #endregion [ Constructors ]

        #region [ Events ]

        protected override void OnDrawItem(DrawItemEventArgs e) {
            var text = string.Empty;
            Bitmap bitmap = null;

            // Get the Bounding rectangle
            var rc = new Rectangle(e.Bounds.X + 16, e.Bounds.Y, e.Bounds.Width - 16, e.Bounds.Height);

            // Setup the StringFormat object
            var sf = new StringFormat();

            // Draw the rectangle
            e.Graphics.FillRectangle(new SolidBrush(Color.White), rc);

            var r1 = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            var yOffset = 0;

            // Get the bitmap for this entry
            if ((e.Index < Items.Count) && (e.Index >= 0)) {
                text = Items[e.Index].ToString();
                var item = Items[e.Index] as ImageListItem;
                if (item != null) {
                    bitmap = item.Image;
                }
                yOffset = (e.Bounds.Height - ((bitmap != null) ? bitmap.Height : 0)) / 2;
            }

            if (Enabled) {
                // Check if the item is selected
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                    // Paint the item accordingly if it is selected
                    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus) {
                        e.DrawFocusRectangle();
                    }
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, r1);
                    if (bitmap != null) {
                        e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y + yOffset, bitmap.Width, bitmap.Height);
                    }
                    e.Graphics.DrawString(text, Font, SystemBrushes.HighlightText, rc, sf);
                }
                else {
                    // Paint the item that if not selected
                    e.Graphics.FillRectangle(SystemBrushes.Window, r1);
                    if (bitmap != null) {
                        e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y + yOffset, bitmap.Width, bitmap.Height);
                    }
                    e.Graphics.DrawString(text, Font, SystemBrushes.WindowText, rc, sf);
                    e.DrawFocusRectangle();
                }
            }
            else {
                // Paint the item disabled
                e.Graphics.FillRectangle(SystemBrushes.Window, r1);
                if (bitmap != null) {
                    e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y + yOffset, bitmap.Width, bitmap.Height);
                }
                e.Graphics.DrawString(text, Font, SystemBrushes.GrayText, rc, sf);
                e.DrawFocusRectangle();
            }
        }


        protected override void OnMeasureItem(MeasureItemEventArgs e) {
            var height = 0;

            if ((e.Index < Items.Count) && (e.Index >= 0)) {
                Bitmap bmp = null;
                var item = Items[e.Index] as ImageListItem;
                if (item != null) {
                    bmp = item.Image;
                }
                if (bmp != null) {
                    height = bmp.Height;
                }
            }
            var textSize = e.Graphics.MeasureString("Xfg", Font);

            e.ItemHeight = Math.Max(height, (int) Math.Ceiling(textSize.Height));
        }

        #endregion [ Events ]
    }

    public class ImageListItem {
        public Bitmap Image;
        public readonly string Key = string.Empty;
        public readonly object Tag;
        public string Text = string.Empty;

        #region[ Constructors ]

        private ImageListItem(string text) {
            Text = text;
        }


        public ImageListItem(string text, string key) : this(text) {
            Key = key;
        }


/*
        public ImageListItem(string text, Bitmap image) : this(text) {
            Image = image;
        }
*/


        public ImageListItem(string text, string key, Bitmap image) : this(text, key) {
            Image = image;
        }


        public ImageListItem(string text, int key, Bitmap image) : this(text, key.ToString(CultureInfo.InvariantCulture)) {
            Image = image;
        }


        public ImageListItem(string text, string value, Bitmap image, object tag) : this(text, value, image) {
            Tag = tag;
        }

        #endregion[ Constructors ]

        [DebuggerHidden]
        public override string ToString() {
            return Text;
        }
    }
}