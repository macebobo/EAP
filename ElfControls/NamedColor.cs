using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

//using System.Linq;

namespace ElfControls {
    [Serializable]
    public class NamedColor {
        #region [ Private Variables ]

        private Color _color;
        private Point _location;
        private string _name;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public Color Color {
            get { return _color; }
            set { _color = value; }
        }

        [Browsable(false)]
        public Point Location {
            get { return _location; }
            set { _location = value; }
        }

        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public NamedColor() {
            _color = Color.White;
            _name = GetColorName(_color);
            _location = Point.Empty;
        }


        public NamedColor(Color color) {
            _color = color;
            _name = GetColorName(_color);
            _location = Point.Empty;
        }


        public NamedColor(Color color, string name) : this(color) {
            if (name.Length == 0) {
                _name = GetColorName(color);
            }
            else {
                _name = name;
            }
        }


        public NamedColor(string name, Color color) : this(color, name) {}


        public NamedColor(Color color, string name, Point location) : this(color, name) {
            _location = location;
        }


        public NamedColor(string name, int red, int green, int blue) : this() {
            _name = name;
            _color = Color.FromArgb(red, green, blue);
        }


        public NamedColor(NamedColor nColor) {
            _color = nColor.Color;
            _name = nColor.Name;
            _location = new Point(nColor.Location.X, nColor.Location.Y);
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        public static string GetColorName(Color color) {
            if (color.A != 255) {
                return GetFormattedColorString(color);
            }

            if (color.IsNamedColor) {
                return color.Name;
            }

            return GetFormattedColorString(color);
        }


        public static string GetFormattedColorString(Color color) {
            return color.R + ", " + color.G + ", " + color.B;
        }


        public string GetRGBString() {
            return string.Format("R:{0}, G:{1}, B:{2}", _color.R, _color.G, _color.B);
        }


        public override string ToString() {
            if (!string.IsNullOrEmpty(_name)) {
                return _name;
            }
            if (_color.IsKnownColor) {
                return _color.ToKnownColor().ToString();
            }
            return GetRGBString();
        }

        #endregion [ Methods ]
    }

    #region [ Type Converter Classes ]

    public class NamedColorTypeConverter : TypeConverter {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof (string)) {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value == null) {
                return new NamedColor();
            }

            if (value is string) {
                var s = (string) value;
                if (s.Length == 0) {
                    return new NamedColor();
                }

                string[] parts = s.Split(';');

                // Determine if name is stored as first and 
                // last; first, middle, and last;
                // or is in error.
                if (parts.Length != 3) {
                    throw new ArgumentException("NamedColor must have 2 or 3 parts.", "value");
                }

                string ColorValue = parts[0];
                string Name = parts[1];
                string Loc = parts[2];

                Color Color = Color.FromArgb(Int32.Parse(ColorValue, NumberStyles.HexNumber));
                var Location = new Point(Int32.Parse(Loc.Split(',')[0]), Int32.Parse(Loc.Split(',')[1]));
                return new NamedColor(Color, Name, Location);
            }

            return base.ConvertFrom(context, culture, value);
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (value != null) {
                if (!(value is NamedColor)) {
                    throw new ArgumentException("Invalid NamedColor", "value");
                }
            }

            if (destinationType == typeof (string)) {
                if (value == null) {
                    return String.Empty;
                }

                var nColor = (NamedColor) value;
                int ColorValue = nColor.Color.ToArgb();
                string ConvColor = string.Format("#{0:X8}", ColorValue);
                return ConvColor + ";" + nColor.Name + ";" + nColor.Location.X + "," + nColor.Location.Y;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class NamedColorListTypeConverter : TypeConverter {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof (string)) {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            var List = new List<NamedColor>();

            if (value == null) {
                return List;
            }

            if (value is string) {
                var s = (string) value;
                if (s.Length == 0) {
                    return List;
                }

                var Elements = new List<string>();
                Elements.AddRange(s.Split('|'));
                var ncConv = new NamedColorTypeConverter();

                foreach (string Line in Elements) {
                    List.Add((NamedColor) ncConv.ConvertFrom(Line));
                }
                ncConv = null;
                return List;
            }

            return base.ConvertFrom(context, culture, value);
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (value != null) {
                if (!(value is List<NamedColor>)) {
                    throw new ArgumentException("Invalid List", "value");
                }
            }

            if (destinationType == typeof (string)) {
                if (value == null) {
                    return String.Empty;
                }

                var List = (List<NamedColor>) value;

                string Converted = string.Empty;
                var ncConv = new NamedColorTypeConverter();
                foreach (NamedColor nColor in List) {
                    Converted += (Converted.Length > 0 ? "|" : string.Empty) + ncConv.ConvertToString(nColor);
                }
                ncConv = null;
                return Converted;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion [ Type Converter Classes ]

    public class Palette : IEnumerable<NamedColor>, ICloneable {
        #region [ Private Variables ]

        private readonly List<NamedColor> _list;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public int Count {
            get { return _list.Count; }
        }

        /// <summary>
        ///     Overloaded index operator
        /// </summary>
        /// <param name="index">Index of the array to use.</param>
        public NamedColor this[int index] {
            get { return _list[index]; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public Palette() {
            _list = new List<NamedColor>();
        }

        #endregion [ Constructors ]

        #region [ List Methods ]

        /// <summary>
        ///     Create a deep clone of this object.
        /// </summary>
        public object Clone() {
            var MyClone = new Palette();
            foreach (NamedColor Item in this) {
                MyClone.Add(new NamedColor(Item));
            }
            return MyClone;
        }


        /// <summary>
        ///     Adds a new Item
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <exception cref="System.ArgumentNullException">item is null</exception>
        public void Add(NamedColor item) {
            if (item == null) {
                throw new ArgumentNullException("value is null");
            }

            // Check to see if this color value is already present in the list. If so, then we will not add.
            //var NC = _list.Where(n => n.Color.ToArgb() == item.Color.ToArgb()).FirstOrDefault();
            //if (NC != null)
            //	return;

            if (Where(item.Color) != null) {
                return;
            }

            _list.Add(item);
        }


        /// <summary>
        ///     Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the List.
        ///     The collection itself cannot be null, nor can any of the elements therein.
        /// </param>
        /// <exception cref="System.ArgumentNullException">collection is null</exception>
        public void AddRange(List<NamedColor> collection) {
            if (collection == null) {
                throw new ArgumentNullException("collection is null");
            }

            foreach (NamedColor Item in collection) {
                Add(Item);
            }
        }


        /// <summary>
        ///     Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the List.
        ///     The collection itself cannot be null, nor can any of the elements therein.
        /// </param>
        /// <exception cref="System.ArgumentNullException">collection is null</exception>
        public void AddRange(Palette collection) {
            if (collection == null) {
                throw new ArgumentNullException("collection is null");
            }

            foreach (NamedColor Item in collection) {
                Add(Item);
            }
        }


        /// <summary>
        ///     Clears out all the items in the list.
        /// </summary>
        public void Clear() {
            _list.Clear();
        }


        /// <summary>
        ///     Determines whether an element is in the list.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>true if item is found in the list; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">Item is null</exception>
        public bool Contains(NamedColor item) {
            if (item == null) {
                throw new ArgumentNullException("item is null");
            }
            return _list.Contains(item);
        }


        /// <summary>
        ///     Searches for the specified item and returns the zero-based index of the first occurrence within the entire list.
        /// </summary>
        /// <param name="item">The item to locate in the list. The value cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Item is null</exception>
        /// <returns>The zero-based index of the first occurrence of item within the entire list, if found; otherwise, –1.</returns>
        public int IndexOf(NamedColor item) {
            if (item == null) {
                throw new ArgumentNullException("item is null");
            }
            return _list.IndexOf(item);
        }


        /// <summary>
        ///     Inserts an Item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">Item to Insert.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than Count.</exception>
        /// <exception cref="System.ArgumentNullException">Item is null</exception>
        public virtual void Insert(int index, NamedColor item) {
            if ((index < 0) || (index >= _list.Count)) {
                throw new ArgumentOutOfRangeException();
            }
            if (item == null) {
                throw new ArgumentNullException("item is null");
            }

            _list.Insert(index, item);
        }


        /// <summary>
        ///     Removes the first occurrence of a specific Item from the list.
        /// </summary>
        /// <param name="item">The Item to remove from the list. The value cannot be null.</param>
        /// <returns>
        ///     true if item is successfully removed; otherwise, false. This method also returns false if the item was not
        ///     found in the list.
        /// </returns>
        public bool Remove(NamedColor item) {
            if (item == null) {
                throw new ArgumentNullException("item is null");
            }

            return _list.Remove(item);
        }


        /// <summary>
        ///     Removes the item at the specified index of the list.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is less than 0.-or-index is equal to or greater than
        ///     list.Count.
        /// </exception>
        public void RemoveAt(int index) {
            if ((index < 0) || (index >= _list.Count)) {
                throw new ArgumentOutOfRangeException();
            }
            _list.RemoveAt(index);
        }


        public NamedColor Where(Color color) {
            foreach (NamedColor nColor in _list) {
                if (nColor.Color.Equals(color)) {
                    return nColor;
                }
            }
            return null;
        }

        #endregion [ List Methods ]

        #region [ IEnumerable ]

        /// <summary>
        ///     Allows for "foreach" statements to be used on an instance of this class, to loop through all the Channels.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        IEnumerator<NamedColor> IEnumerable<NamedColor>.GetEnumerator() {
            return (IEnumerator<NamedColor>) GetEnumerator();
        }


        public NamedColorEnum GetEnumerator() {
            return new NamedColorEnum(_list);
        }

        #endregion [ IEnumerable ]
    }

    /// <summary>
    ///     NamedColor enumerator class
    ///     http://msdn.microsoft.com/en-us/library/system.collections.ienumerable.getenumerator.aspx
    /// </summary>
    public class NamedColorEnum : IEnumerator {
        public List<NamedColor> _list;

        // Enumerators are positioned before the first element 
        // until the first MoveNext() call. 
        private int position = -1;


        public NamedColorEnum(List<NamedColor> list) {
            _list = list;
        }


        public NamedColor Current {
            get {
                try {
                    return _list[position];
                }
                catch (IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }

        #region IEnumerator Members

        public bool MoveNext() {
            position++;
            return (position < _list.Count);
        }


        public void Reset() {
            position = -1;
        }


        object IEnumerator.Current {
            get { return Current; }
        }

        #endregion
    }
}