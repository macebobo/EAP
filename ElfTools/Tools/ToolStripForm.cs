using System;
using System.Windows.Forms;

namespace ElfTools.Tools {
    public class ToolStripForm : Form {
        #region [ Constructors ]

        #endregion [ Constructors ]

        #region [ Methods ]

        //public virtual void AddCustomControls()
        //{ }

        public virtual ToolStrip GetToolStrip(int ownerTool) {
            throw new NotImplementedException("GetToolStrip()");
        }


        /// <summary>
        ///     Returns an item from a toolstrip of a given type and position
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="strip">ToolStrip object to find the child control on.</param>
        /// <param name="index">
        ///     Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object
        ///     of that type that is found, regardless of the number of other controls that preceeded it.
        /// </param>
        public virtual ToolStripItem GetItem<T>(ToolStrip strip, int index) {
            if (strip == null) {
                return null;
            }

            int Counter = 0;

            for (int i = 0; i < strip.Items.Count; i++) {
                if (strip.Items[i] is T) {
                    Counter++;
                    if (Counter == index) {
                        return strip.Items[i];
                    }
                }
            }

            return null;
        }


        /// <summary>
        ///     Returns an item from a toolstrip of a given type and position
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="ownerTool">ID of the Tool that is associated with a ToolStrip</param>
        /// <param name="index">
        ///     Index of the object to return, Example, if the 3rd button is desired, it will return the 3rd object
        ///     of that type that is found, regardless of the number of other controls that preceeded it.
        /// </param>
        public virtual ToolStripItem GetItem<T>(int ownerTool, int index) {
            ToolStrip strip = GetToolStrip(ownerTool);
            if (strip == null) {
                return null;
            }

            int Counter = 0;

            for (int i = 0; i < strip.Items.Count; i++) {
                if (strip.Items[i] is T) {
                    Counter++;
                    if (Counter == index) {
                        return strip.Items[i];
                    }
                }
            }

            return null;
        }


        /// <summary>
        ///     Returns an item from a toolstrip of a given type and name
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="strip">ToolStrip object to find the child control on.</param>
        /// <param name="name">Name of the control</param>
        public virtual ToolStripItem GetItem<T>(ToolStrip strip, string name) {
            if (strip == null) {
                return null;
            }

            for (int i = 0; i < strip.Items.Count; i++) {
                if (strip.Items[i] is T) {
                    if (string.Compare(strip.Items[i].Name, name, true) == 0) {
                        return strip.Items[i];
                    }
                }
            }

            return null;
        }


        /// <summary>
        ///     Returns an item from a toolstrip of a given type and name
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="ownerTool">ID of the Tool that is associated with a ToolStrip</param>
        /// <param name="name">Name of the control</param>
        public virtual ToolStripItem GetItem<T>(int ownerTool, string name) {
            ToolStrip strip = GetToolStrip(ownerTool);
            if (strip == null) {
                return null;
            }

            for (int i = 0; i < strip.Items.Count; i++) {
                if (strip.Items[i] is T) {
                    if (string.Compare(strip.Items[i].Name, name, true) == 0) {
                        return strip.Items[i];
                    }
                }
            }

            return null;
        }

        #endregion [ Methods ]

        #region [ Events ]

        public virtual void NumberOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }


        public virtual void SignedFloatOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || (e.KeyChar == '-') || (e.KeyChar == '+') || (e.KeyChar == '.') ||
                (e.KeyChar == '°')) {
                // We like these, do nothing
                e.Handled = false;
            }
            else {
                e.Handled = true;
            }
        }


        public virtual void SignedNumberOnly_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || (e.KeyChar == '-') || (e.KeyChar == '+')) {
                // We like these, do nothing
                e.Handled = false;
            }
            else {
                e.Handled = true;
            }
        }

        #endregion [ Events ]
    }
}