using ElfCore.Controllers;
using ElfCore.Core;
using ElfCore.Interfaces;
using ElfCore.Profiles;
using ElfCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

using ElfCore.XmlSerializers;

using LatticePoint = System.Drawing.Point;

namespace ElfCore.Channels {
    [Serializable]
    [DebuggerDisplay("Name = {_name}")]
    public class Channel : ElfBase, IComparable, IItem {

        #region [ Constants ]

        // Property Names
        public const string PropertyBorderColor = "BorderColor";
        public const string PropertyRenderColor = "RenderColor";
        public const string PropertySequencerColor = "SequencerColor";
        public const string PropertyEnabled = "Enabled";
        private const string PropertyGrouped = "Grouped";
        private const string PropertyID = "ID";
        public const string PropertyIncluded = "Included";
        protected const string PropertyLattice = "Lattice";
        public const string PropertyLocked = "Locked";
        public const string PropertyName = "Name";
        public const string PropertyOrigin = "Origin";
        public const string PropertySelected = "Selected";
        public const string PropertyVisible = "Visible";

        #endregion [ Constants ]

        #region [ Protected Variables ]

        private bool _grouped;

        /// <summary>
        /// Output Channel ID for this Channel as stored in the Profile
        /// ˂Channel color="-32768" output="3" id="634798630941625207" enabled="True"˃Big Jack Mouth Mid˂/Channel˃
        /// id attribute in this node appears to be the create time of the Channel, as expressed in Ticks, and is not unique within the Profile.
        /// </summary>
        private int _id = -1;

        private Color _sequencerColor = Color.Empty;
        private Color _renderColor = Color.Empty;
        private Color _borderColor = Color.Empty;
        private Point _origin = new Point(0, 0);
        private string _name = string.Empty;
        private bool _visible = true;
        private bool _enabled = true;
        private bool _included = true;
        private bool _locked;
        private bool _loading;

        private bool _isSelected;

        [NonSerialized] private GraphicsPath _graphicsPath = new GraphicsPath();

        protected List<LatticePoint> LatticePoints = new List<LatticePoint>();

        [NonSerialized] private readonly ChannelList _subChannels = new ChannelList();

        private string _cxImageKey;

        /// <summary>
        /// Indicates whether the Lattice has changed in any way since that last time the GraphicsPath object has been created. Reset when the GraphicsPath object is rebuilt.
        /// </summary>
        protected bool IsLatticeChanged = true;

        #endregion [ Protected Variables ]

        #region [ Properties ]

        /// <summary>
        /// Color to use to draw the Channel object.
        /// </summary>
        [XmlIgnore]
        public Color BorderColor {
            get { return _borderColor; }
            set {
                if (!CanEdit() || _borderColor.Equals(value))
                    return;

                _borderColor = value;
                ChannelExplorerImageKey = GenCxImageKey();
                OnPropertyChanged(PropertyBorderColor, true);
            }
        }

        /// <summary>
        /// Used only for serialization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("BorderColor")]
        public string BorderColorSerialized {
            get { return XmlColor.FromBaseType(_borderColor); }
            set { _borderColor = XmlColor.ToBaseType(value); }
        }

        /// <summary>
        /// Key used to find the correct color swatch for this Channel
        /// </summary>
        public string ChannelExplorerImageKey {
            get {
                if (_cxImageKey == null)
                    ChannelExplorerImageKey = GenCxImageKey();
                return _cxImageKey;
            }
            set { _cxImageKey = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Channel is enabled.
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set {
                if (_enabled == value)
                    return;
                _enabled = value;
                ChannelExplorerImageKey = GenCxImageKey();
                OnPropertyChanged(PropertyEnabled, true);
            }
        }

        /// <summary>
        /// Indicates whether this Channel is grouped with other Channels.
        /// </summary>
        public bool Grouped {
            get { return _grouped; }
            set {
                if (!CanEdit() || (_grouped == value))
                    return;
                _grouped = value;
                OnPropertyChanged(PropertyGrouped, true);
            }
        }

        /// <summary>
        /// Indicates whether there is Lattice data defined for this Channel.
        /// </summary>
        [XmlIgnore]
        public bool HasLatticeData {
            get { return (LatticePoints.Count > 0); }
        }

        /// <summary>
        /// Unique Output ID for this Channel. Corresponds to the "output" attribute in the Profile Xml
        /// </summary>
        public int ID {
            get { return _id; }
            set {
                if (!CanEdit() || (_id == value))
                    return;
                _id = value;
                OnPropertyChanged(PropertyID, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Channel is included in this Profile.
        /// </summary>
        public bool Included {
            get { return _included; }
            set {
                if (_included == value)
                    return;
                _included = value;
                ChannelExplorerImageKey = GenCxImageKey();
                OnPropertyChanged(PropertyIncluded, true);
            }
        }

        /// <summary>
        /// Position in the Active SortedList
        /// </summary>
        [XmlIgnore]
        public int Index { get; set; }

        /// <summary>
        /// Indicates whether this Channel is hidden and does not render.
        /// </summary>
        [XmlIgnore]
        public bool IsHidden { get; private set; }

        /// <summary>
        /// Indicates whether this Channel is selected in the Channel Explorer.
        /// </summary>
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected == value) {
                    return;
                }

                _isSelected = value;
                if (_isSelected)
                    OnPropertyChanged(PropertySelected, false);
            }
        }

        /// <summary>
        /// List of Cells points that represent the pseudo-bitmap mode of rendering.
        /// </summary>
        [XmlIgnore]
        public List<LatticePoint> Lattice {
            get { return LatticePoints; }
            set {
                if (Workshop.ListEqual(LatticePoints, value)) {
                    return;
                }

                LatticePoints = new List<LatticePoint>();
                
                if (value != null) {
                    LatticePoints = new List<LatticePoint>();
                    LatticePoints.AddRange(value);
                }

                OnLatticeChanged();
            }
        }

        /// <summary>
        /// Used only for serialization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("Lattice")]
        public string LatticeSerialized {
            get { return SerializeLattice(false, true); }
            set { DeserializeLattice(value); }
        }

        /// <summary>
        /// Bitmap object of the Lattice Buffer for this Channel
        /// </summary>
        [XmlIgnore]
        public Bitmap LatticeBuffer {
            get { return CreateLatticeBuffer(); }
            set { PopulateFromLatticeBuffer(value); }
        }

        /// <summary>
        /// Sets an indicator to this Channel that it needs to refresh its GraphicsPath
        /// </summary>
        [XmlIgnore]
        public bool LatticeChanged {
            set { IsLatticeChanged = value; }
        }

        /// <summary>
        /// Indicates whether the data is being loaded in.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool Loading {
/*
            get { return _loading; }
*/
            set {
                _loading = value;
                SuppressEvents = value;
            }
        }

        /// <summary>
        /// Indicates whether this Channel is Locked. If so, it is immune to any form of edit.
        /// </summary>
        [XmlIgnore]
        public bool Locked {
            get { return _locked; }
            set {
                if (_locked != value) {
                    _locked = value;
                    ChannelExplorerImageKey = GenCxImageKey();
                    OnPropertyChanged(PropertyLocked, true);
                }
            }
        }

        /// <summary>
        /// Name used to describe this Channel. If this is a normal Channel, then it retrieves the Name property within the Vixen.Channel object this wraps, otherwise
        /// sets and returns a local _name variable
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                if (!CanEdit() || (_name == value))
                    return;
                _name = value;
                OnPropertyChanged(PropertyName, true);
            }
        }

        /// <summary>
        /// Point offset from where the Channel should be drawn from
        /// </summary>
        public Point Origin {
            get { return _origin; }
            set {
                if (!CanEdit() || _origin.Equals(value))
                    return;
                _origin = value;
                OnPropertyChanged(PropertyOrigin, true);
            }
        }

        /// <summary>
        /// Profile that owns this Channel.
        /// </summary>
        [XmlIgnore]
        public BaseProfile Profile { private get; set; }

        /// <summary>
        /// Base color used to render the Channel.
        /// </summary>
        [XmlIgnore]
        public Color RenderColor {
            get { return _renderColor; }
            set {
                if (!CanEdit())
                    return;
                if (!_renderColor.Equals(value)) {
                    _renderColor = value;
                    ChannelExplorerImageKey = GenCxImageKey();
                    OnPropertyChanged(PropertyRenderColor, true);
                }
            }
        }

        /// <summary>
        /// Used only for serialization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("RenderColor")]
        public string RenderColorSerialized {
            get { return XmlColor.FromBaseType(_renderColor); }
            set { _renderColor = XmlColor.ToBaseType(value); }
        }

        /// <summary>
        /// Color used by the sequencing program for displaying the Channel.
        /// </summary>
        [XmlIgnore]
        public Color SequencerColor {
            get { return _sequencerColor; }
            set {
                if (!CanEdit())
                    return;
                if (!_sequencerColor.Equals(value)) {
                    _sequencerColor = value;
                    ChannelExplorerImageKey = GenCxImageKey();
                    OnPropertyChanged(PropertySequencerColor, true);
                }
            }
        }

        /// <summary>
        /// Used only for serialization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("SequencerColor")]
        public string SequencerColorSerialized {
            get { return XmlColor.FromBaseType(_sequencerColor); }
            set { _sequencerColor = XmlColor.ToBaseType(value); }
        }

        /// <summary>
        /// Pre-serialized version of this object.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public override string Serialized {
            get {
                if (_serialized.Length == 0)
                    _serialized = Extends.SerializeObjectToXml(this);
                return base.Serialized;
            }
        }

        /// <summary>
        /// Used only for serialization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("Vector")]
        public string VectorSerialized {
            get { return SerializeVector(); }
            set {
                var x = value;
                Debug.Print("value from serializeVector: {0}",x);
                DeserializeVector(); //todo value is not used!
            }
        }

        /// <summary>
        /// Indicates whether this Channel can be rendered.
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set {
                if (value == _visible)
                    return;
                _visible = value;
                ChannelExplorerImageKey = GenCxImageKey();
                OnPropertyChanged(PropertyVisible, false);
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        [DebuggerHidden]
        public Channel() {
            base.SuppressEvents = true;
            _locked = false;
            _isSelected = false;
            _id = -1;
            Index = -1;
            _visible = true;
            _enabled = true;
            _included = true;
            LatticePoints = new List<LatticePoint>();
            _sequencerColor = Color.White;
            _borderColor = Color.Empty;
            _renderColor = Color.Empty;
            base.Dirty = false;
            base.SuppressEvents = false;
        }


        [DebuggerHidden]
        public Channel(int index) : this() {
            Index = index;
        }


        public Channel(RawChannel rawChannel) : this() {
            base.SuppressEvents = true;
            _loading = true;
            _borderColor = rawChannel.BorderColor;
            _enabled = rawChannel.Enabled;
            _id = rawChannel.ID;
            _included = rawChannel.Included;
            _locked = rawChannel.Locked;
            _name = rawChannel.Name;
            _renderColor = rawChannel.RenderColor;
            _sequencerColor = rawChannel.SequencerColor;
            _visible = rawChannel.Visible;
            LatticeSerialized = rawChannel.EncodedRasterData;
            VectorSerialized = rawChannel.EncodedVectorData;
            _loading = false;
            base.SuppressEvents = false;
        }

        #endregion [ Constructors ]

        #region [ Private Methods ]

        private bool CanEdit() {
            if (_loading)
                return true;
            return (!_locked & _enabled & _included & _visible);
        }

        #endregion [ Private Methods ]

        #region [ Protected Methods ]

        private string GenCxImageKey() {
            return ImageHandler.CreateAndAddColorSwatch(new Swatch(GetColor(), _borderColor, _locked, _included, _enabled, _visible));
        }


        /// <summary>
        /// Generate a bitmap based on the data
        /// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
        /// There is no grid and zoom and paint points are 1x1 pixels
        /// </summary>
        /// <returns></returns>
        private Bitmap CreateLatticeBuffer() {
            return Profile != null ? CreateLatticeBuffer(Color.Black, Profile.Scaling.LatticeSize) : null;
        }


        /// <summary>
        /// Generate a bitmap based on the data.
        /// The bitmap is equivalent to what goes to the Paint Pane, painted with Pixels, not cells.
        /// There is no grid and zoom and paint points are 1x1 pixels
        /// </summary>
        /// <param name="backgroundColor">Color to use as the image background.</param>
        /// <param name="size">Size of the lattice buffer.</param>
        protected Bitmap CreateLatticeBuffer(Color backgroundColor, Size size) {
            var bounds = new Rectangle(0, 0, size.Width, size.Height);
            var bmp = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bmp))
                g.Clear(backgroundColor); // paint the background 

            var lockBitmap = new LockBitmap(bmp);
            lockBitmap.LockBits();

            foreach (var pt in LatticePoints) {
                if (bounds.Contains(pt))
                    lockBitmap.SetPixel(pt.X, pt.Y, Color.White);
            }

            lockBitmap.UnlockBits();
            return bmp;
        }


        /// <summary>
        /// Clean up all child objects here, unlink all events and dispose
        /// </summary>
        protected override void DisposeChildObjects() {
            base.DisposeChildObjects();
            LatticePoints = null;
            if (_graphicsPath != null) {
                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
        }

        #endregion [ Protected Methods ]

        #region [ Public Methods ]

        /// <summary>
        /// Clone this Channel and its underlying objects.
        /// </summary>
        public override object Clone() {
            var myClone = base.Clone();

            // MemberwiseClone copies references, so we need to create a new Lattice for the Clone
            ((Channel) myClone).Lattice = new List<LatticePoint>();
            ((Channel) myClone).Lattice.AddRange(Lattice);
            return myClone;
        }


        /// <summary>
        /// Implement the IComparable interface method
        /// </summary>
        public int CompareTo(object obj) {
            if (obj is Channel) {
                return ID.CompareTo((obj as Channel).ID);
            }
            
            throw new ArgumentException("Object is not a RasterChannel");
        }


        /// <summary>
        /// Used by Clipboard to Cut cells. The collection cannot be null.
        /// </summary>
        /// <param name="collection">Cells to remove from the lattice</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CutData(IEnumerable<Point> collection) {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (!CanEdit())
                return;

            foreach (var cell in collection)
                LatticePoints.Remove(cell);
            OnLatticeChanged();
        }


        /// <summary>
        /// Converts the encoded unsigned integer into a System.Drawing.Point structure.
        /// </summary>
        /// <param name="encodedPixel">Unsigned int, stored in the Lattice.</param>
        private static LatticePoint DecodeCell(uint encodedPixel) {
            return new LatticePoint((int) (encodedPixel >> 16), (int) (encodedPixel & 0xffff));
        }


        /// <summary>
        /// Get rid of all extra Cells that are duplicates.
        /// </summary>
        public void DedupeData() {
            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            if (!CanEdit())
                return;

            Dirty = true;
            using (var bitmap = CreateLatticeBuffer()) {
                if (bitmap != null)
                    PopulateFromLatticeBuffer(bitmap);
            }
            //watch.Stop();
            //Debug.WriteLine("DedupePixels: " + watch.ElapsedTicks + " ticks, " + watch.ElapsedMilliseconds + " ms");

        }


        /// <summary>
        /// Converts the string of encoded data from the profile into cell values.
        /// </summary>
        /// <param name="encoded">String of encoded data.</param>
        /// <param name="clear">Indicates whether the existing data should be removed.</param>
        public void DeserializeLattice(string encoded, bool clear = true) {
            //Editor.Stopwatch.Start();
            if (!CanEdit())
                return;

            if (clear)
                LatticePoints.Clear();

            if (encoded.Length == 0) {
                OnLatticeChanged();
                return;
            }

            var bytes = Convert.FromBase64String(encoded);

            for (var i = 0; i < bytes.Length; i += 4) {
                LatticePoints.Add(DecodeCell(BitConverter.ToUInt32(bytes, i)));
            }

            //Editor.Stopwatch.Stop();
            //Debug.WriteLine("DecodeChannelBytes: " + Editor.Stopwatch.ElapsedTicks + " ticks, "+ Editor.Stopwatch.ElapsedMilliseconds + " ms");
            if (!clear)
                DedupeData();
            else
                OnLatticeChanged();
        }


        /// <summary>
        /// Decodes the vector data from storage in the Profile Xml document
        /// </summary>
        private static void DeserializeVector() {}


        /// <summary>
        /// Generate a bitmap based on the data
        /// The bitmap is equivalent to the one on the CanvasPane in CanvasWindow
        /// NOTE: This function appears in CanvasWindow, BaseTool and Channel
        /// </summary>
        /// <param name="paintColor">Color in which to paint the cells</param>
        public Bitmap DrawChannel(Color paintColor) {
            // Create a new image of the right size 
            var canvasSize = Profile.Scaling.CanvasSize;
            var canvasBuffer = new Bitmap(canvasSize.Width, canvasSize.Height);
            using (var g = Graphics.FromImage(canvasBuffer)) {
                using (var channelBrush = new SolidBrush(paintColor)) {
                    g.FillPath(channelBrush, GetGraphicsPath());
                }
                return canvasBuffer;
            }
        }


        /// <summary>
        /// Empties the Channel of all Cells.
        /// </summary>
        /// <param name="destroyData">Indicates whether the Lattice array should be destroyed and rebuilt</param>
        public virtual void Empty(bool destroyData = false) {
            if (!CanEdit())
                return;

            Dirty = true;
            Origin = new Point(0, 0);
            if (destroyData)
                LatticePoints = new List<LatticePoint>();
            else
                LatticePoints.Clear();
            OnLatticeChanged();
        }


        /// <summary>
        /// Checks to see if the Render color has been set. If so, return that, else return the sequencer color.
        /// </summary>
        /// <returns></returns>
        public Color GetColor() {
            return _renderColor.IsEmpty ? _sequencerColor : _renderColor;
        }


        /// <summary>
        /// Returns a rectangle that bounds this Channel.
        /// </summary>
        /// //todo refactor using linq - JEMA 12/13/14
        public Rectangle GetBounds() {
            if ((LatticePoints == null) || (LatticePoints.Count == 0))
                return Rectangle.Empty;

            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var minX = int.MaxValue;
            var minY = int.MaxValue;

            foreach (var cell in LatticePoints) {
                minX = Math.Min(minX, cell.X);
                minY = Math.Min(minY, cell.Y);
                maxX = Math.Max(maxX, cell.X);
                maxY = Math.Max(maxY, cell.Y);
            }
            return new Rectangle(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);
        }


        /// <summary>
        /// Selects the correct color used to render the Channel.
        /// </summary>
        /// <param name="alpha">Amount of transparency to apply to the color. 0 is completely transparent, 255 is completely solid.</param>
        public SolidBrush GetBrush(byte alpha) {
            return new SolidBrush(Color.FromArgb(alpha, GetColor()));
        }


        /// <summary>
        /// Selects the correct color used to render the Channel.
        /// </summary>
        public SolidBrush GetBrush() {
            return new SolidBrush(GetColor());
        }


        /// <summary>
        /// Creates a graphics path that represents all the cells in the Canvas
        /// </summary>
        public GraphicsPath GetGraphicsPath() {
            return GetGraphicsPath(Profile.Scaling);
        }


        /// <summary>
        /// Creates a graphics path that represents all the cells in the Canvas. 
        /// </summary>
        /// <param name="scaling">Precalculated scaling data</param>
        public GraphicsPath GetGraphicsPath(Scaling scaling) {
            if (!IsLatticeChanged && (_graphicsPath.PointCount != 0)) {
                return _graphicsPath;
            }
            _graphicsPath.Reset();
            var cellSize = scaling.CellSize.GetValueOrDefault(1);
            var cellRect = new Rectangle(0, 0, cellSize, cellSize);

            var cellGrid = scaling.CellGrid;
            var gridLineWidth = scaling.GridLineWidth;

            foreach (var pt in LatticePoints) {
                cellRect.Location = new LatticePoint(gridLineWidth + (pt.X * cellGrid), gridLineWidth + (pt.Y * cellGrid));
                _graphicsPath.AddRectangle(cellRect);
            }
            IsLatticeChanged = false;

            var zoom = scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100);
            if (!(Math.Abs(zoom - Scaling.ZOOM_100) > 0.00001f)) {
                return _graphicsPath;
            }

            // Rescale the path to that of the zoom.
            using (var scaleMatrix = new Matrix()) {
                scaleMatrix.Scale(zoom, zoom);
                _graphicsPath.Transform(scaleMatrix);
            }

            return _graphicsPath;
        }


        /// <summary>
        /// Turns on the Hidden flag to suppress redrawing of this Channel. Use the method <seealso cref="Show">Show()</seealso> to allow normal drawing.
        /// </summary>
        [DebuggerHidden]
        public void Hide() {
            IsHidden = true;
        }


        /// <summary>
        /// Offsets the rendering of the Channel.
        /// </summary>
        /// <param name="offset">Amount to offset the rendering.</param>
        public void MoveData(PointF offset) {
            if (!CanEdit())
                return;

            var newCells = new List<LatticePoint>();
            var point = new Point((int) offset.X, (int) offset.Y);

            foreach (var lattticePoint in LatticePoints) {
                newCells.Add(new LatticePoint(lattticePoint.X + point.X, lattticePoint.Y + point.Y));
            }

            Lattice = newCells;
            OnLatticeChanged();
        }


        /// <summary>
        /// Adds a list of Cells to the Lattice, then triggers an event to cause the Channel to be redrawn.
        /// Used by Clipboard to Paste cells. The list of Cells cannot be null.
        /// </summary>
        /// <param name="collection">Lists of Cells to add to the Lattice</param>
        /// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
        public void Paint(IEnumerable<Point> collection) {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (!CanEdit())
                return;

            LatticePoints.AddRange(collection);
            //DedupeData();
            OnLatticeChanged();
        }


        /// <summary>
        /// Adds a list of Cells to the Lattice, offset by the given amount, then triggers an event to cause the Channel to be redrawn.
        /// Used by the Image Stamp  and Text tools
        /// </summary>
        /// <param name="collection">Lists of Cells to add to the Lattice</param>
        /// <param name="offset">Offset point to start the painting.</param>
        /// <exception cref="System.ArgumentNullException">collection cannot be null.</exception>
        public void Paint(List<LatticePoint> collection, Point offset) {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (!CanEdit())
                return;

            // Go through each cell in the list, offset it, and add it to the channel
            for (var i = 0; i < collection.Count; i++) {
                var latticePoint = collection[i];
                latticePoint.Offset(offset);
                collection[i] = latticePoint;
            }

            LatticePoints.AddRange(collection);

            DedupeData();
            OnLatticeChanged();
        }


        /// <summary>
        /// Populates cells from the lattice buffer passed in.
        /// This is the faster method, but doesn't seem to work all that well with non-native bitmapped images.
        /// </summary>
        /// <param name="image">1-bit bitmap that represents the Cells in the lattice</param>
        /// <param name="clearFirst">Clears the Lattice before populating from the bitmap.</param>
        public void PopulateFromLatticeBuffer(Bitmap image, bool clearFirst = true) {
            //Editor.Stopwatch.Reset();
            //Editor.Stopwatch.Start();

            if (!CanEdit())
                return;

            var lockBitmap = new LockBitmap(image);
            lockBitmap.LockBits();

            if (image == null)
                return;
            if (clearFirst)
                LatticePoints.Clear();
            for (var x = 0; x < image.Width; x++)
                for (var y = 0; y < image.Height; y++) {
                    var checkPixel = lockBitmap.GetPixel(x, y);
                    if (checkPixel.GetBrightness() > 0.5f)
                        LatticePoints.Add(new LatticePoint(x, y));
                }

            lockBitmap.UnlockBits();
            IsLatticeChanged = true;

            OnPropertyChanged(PropertyLattice, true);

            //Editor.Stopwatch.Stop();
            //Debug.WriteLine("PopulateFromLatticeBuffer: " + Editor.Stopwatch.Report());
        }


        /// <summary>
        /// Converts the x,y of a Cell to a value that can be saved in the list.
        /// </summary>
        /// <param name="cell">Cell to serialize.</param>
        private static uint SerializeCell(LatticePoint cell) {
            if ((cell.X < 0) || (cell.Y < 0))
                return 0xFFFFFFFF;
            return (uint) ((cell.X << 16) | cell.Y);
        }


        /// <summary>
        /// Converts the data array values to a string to be stored in the Profile Xml.
        /// </summary>
        /// <param name="dedupeData">Indicates whether the data should be deduped first.</param>
        /// <param name="flatten">Indicates whether data from subchannels (if any) should be flattened into the encoded string</param>
        /// <returns>Returns cell data encoded into a string.</returns>
        private string SerializeLattice(bool dedupeData, bool flatten) {
            if (dedupeData)
                DedupeData();

            var channelPixelCoords = new List<byte>();

            foreach (var latticePoint in LatticePoints)
                channelPixelCoords.AddRange(BitConverter.GetBytes(SerializeCell(latticePoint)));


            // ReSharper disable once 
            if (flatten) {
                foreach (Channel subChannel in _subChannels) {
                    foreach (var subChannelLatticePoint in subChannel.Lattice) {
                        channelPixelCoords.AddRange(BitConverter.GetBytes(SerializeCell(subChannelLatticePoint)));
                    }
                }

                return Convert.ToBase64String(channelPixelCoords.ToArray());
            }

            return Convert.ToBase64String(channelPixelCoords.ToArray());
        }


        /// <summary>
        /// Converts the Lattice cell data into a string.
        /// </summary>
        public string SerializeLattice() {
            return SerializeLattice(true, true);
        }


        /// <summary>
        /// Converts the Vector data into a string.
        /// </summary>
        private static string SerializeVector() {
            return string.Empty; // TODO: What???
        }


        /// <summary>
        /// Turns off the Hidden flag to allow normal redrawing of this Channel
        /// </summary>
        [DebuggerHidden]
        public void Show() {
            IsHidden = false;
        }


        /// <summary>
        /// Returns the Name of the Channel
        /// </summary>
        public override string ToString() {
            return _name;
        }


        /// <summary>
        /// Returns the Name of the Channel and its ID
        /// </summary>
        /// <param name="includeID">Indicates whether the ID should be included in the output</param>
        public string ToString(bool includeID) {
            return includeID ? string.Format("{0}: {1}", ID + 1, ToString()) : ToString();
        }

        #endregion [ Public Methods ]

        #region [ Public Static Methods ]

/*
        /// <summary>
        /// Generates a GraphicsPath object based on the scaling information and encoded cell data. Used for playback
        /// </summary>
        public static GraphicsPath GeneratePath(Scaling scaling, string encoded) {
            var Path = new GraphicsPath();
            if (((encoded ?? string.Empty).Length == 0) || (scaling == null))
                return Path;

            var bytes = Convert.FromBase64String(encoded);
            var Lattice = new List<LatticePoint>();
            Point Cell;
            for (var i = 0; i < bytes.Length; i += 4) {
                Cell = DecodeCell(BitConverter.ToUInt32(bytes, i));
                if (!Lattice.Contains(Cell))
                    Lattice.Add(Cell);
            }

            var CellSize = scaling.CellSize.GetValueOrDefault(1);
            var CellSize_Size = new Size(CellSize, CellSize);

            var CellGrid = scaling.CellGrid;
            var GridLineWidth = scaling.GridLineWidth;

            foreach (var pt in Lattice) {
                Path.AddRectangle(new Rectangle(new LatticePoint(GridLineWidth + (pt.X * CellGrid), GridLineWidth + (pt.Y * CellGrid)), CellSize_Size));
            }

            var Zoom = scaling.Zoom.GetValueOrDefault(Scaling.ZOOM_100);
            if (Zoom != Scaling.ZOOM_100) {
                // Rescale the path to that of the zoom.
                using (var ScaleMatrix = new Matrix()) {
                    ScaleMatrix.Scale(Zoom, Zoom);
                    Path.Transform(ScaleMatrix);
                }
            }

            Lattice = null;
            return Path;
        }
*/

        #endregion [ Public Static Methods ]

        #region [ Event Triggers ]

        /// <summary>
        /// Called when the Lattice has been changed. 
        /// </summary>
        [DebuggerHidden]
        protected void OnLatticeChanged() {
            IsLatticeChanged = true;
            Dirty = true;
            OnPropertyChanged(PropertyLattice, true);
        }

        #endregion [ Event Triggers ]

    }

    #region [ Class ChannelList ]

    #endregion [ Class ChannelList ]
}
