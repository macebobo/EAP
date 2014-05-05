using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ElfControls {
    /// <summary>
    ///     ListBox modified to display an image on the left side of the text.
    /// </summary>
    public class ImageListBox : ListBox {
        #region [ Constructors ]

        public ImageListBox() {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        #endregion [ Constructors ]

        #region [ Events ]

        protected override void OnDrawItem(DrawItemEventArgs e) {
            string Text = string.Empty;
            Bitmap Bitmap = null;

            // Get the Bounding rectangle
            var rc = new Rectangle(e.Bounds.X + 16, e.Bounds.Y, e.Bounds.Width - 16, e.Bounds.Height);

            // Setup the StringFormat object
            var sf = new StringFormat();

            // Draw the rectangle
            e.Graphics.FillRectangle(new SolidBrush(Color.White), rc);

            var r1 = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            int YOffset = 0;

            // Get the bitmap for this entry
            if ((e.Index < Items.Count) && (e.Index >= 0)) {
                Text = Items[e.Index].ToString();
                if (Items[e.Index] is ImageListItem) {
                    Bitmap = ((ImageListItem) Items[e.Index]).Image;
                }
                YOffset = (e.Bounds.Height - ((Bitmap != null) ? Bitmap.Height : 0)) / 2;
            }

            if (Enabled) {
                // Check if the item is selected
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                    // Paint the item accordingly if it is selected
                    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus) {
                        e.DrawFocusRectangle();
                    }
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, r1);
                    if (Bitmap != null) {
                        e.Graphics.DrawImage(Bitmap, e.Bounds.X, e.Bounds.Y + YOffset, Bitmap.Width, Bitmap.Height);
                    }
                    e.Graphics.DrawString(Text, Font, SystemBrushes.HighlightText, rc, sf);
                }
                else {
                    // Paint the item that if not selected
                    e.Graphics.FillRectangle(SystemBrushes.Window, r1);
                    if (Bitmap != null) {
                        e.Graphics.DrawImage(Bitmap, e.Bounds.X, e.Bounds.Y + YOffset, Bitmap.Width, Bitmap.Height);
                    }
                    e.Graphics.DrawString(Text, Font, SystemBrushes.WindowText, rc, sf);
                    e.DrawFocusRectangle();
                }
            }
            else {
                // Paint the item disabled
                e.Graphics.FillRectangle(SystemBrushes.Window, r1);
                if (Bitmap != null) {
                    e.Graphics.DrawImage(Bitmap, e.Bounds.X, e.Bounds.Y + YOffset, Bitmap.Width, Bitmap.Height);
                }
                e.Graphics.DrawString(Text, Font, SystemBrushes.GrayText, rc, sf);
                e.DrawFocusRectangle();
            }
        }


        protected override void OnMeasureItem(MeasureItemEventArgs e) {
            int Height = 0;

            if ((e.Index < Items.Count) && (e.Index >= 0)) {
                Bitmap bmp = null;
                if (Items[e.Index] is ImageListItem) {
                    bmp = ((ImageListItem) Items[e.Index]).Image;
                }
                if (bmp != null) {
                    Height = bmp.Height;
                }
            }
            SizeF TextSize = e.Graphics.MeasureString("Xfg", Font);

            e.ItemHeight = Math.Max(Height, (int) Math.Ceiling(TextSize.Height));
        }

        #endregion [ Events ]
    }

    public class ImageListItem {
        public Bitmap Image = null;
        public string Key = string.Empty;
        public object Tag = null;
        public string Text = string.Empty;

        #region[ Constructors ]

        public ImageListItem(string text) {
            Text = text;
        }


        public ImageListItem(string text, string key) : this(text) {
            Key = key;
        }


        public ImageListItem(string text, Bitmap image) : this(text) {
            Image = image;
        }


        public ImageListItem(string text, string key, Bitmap image) : this(text, key) {
            Image = image;
        }


        public ImageListItem(string text, int key, Bitmap image) : this(text, key.ToString()) {
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