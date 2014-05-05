using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ElfCore;
using ElfCore.Interfaces;
using ElfCore.PlugIn;
using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.ToolGroups {
    /// <summary>
    ///     Base level class all tool groups will inherit
    /// </summary>
    public class BaseToolGroup : ElfDisposable, IToolGroup {
        #region [ Constants ]

        private const string SELECTED_TOOL = "SelectedTool";

        #endregion [ Constants ]

        #region [ Protected Variables ]

        protected List<PlugInTool> _childTools = new List<PlugInTool>();
        private bool _isSelected;
        private string _name = string.Empty;

        /// <summary>
        ///     Path in the settings file to find data for this tool
        /// </summary>
        protected string _savePath = string.Empty;

        protected int _selectedIndex = -1;
        protected Settings _settings = Settings.Instance;

        #endregion [ Protected Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Indicates whether the current Tool has been selected from the Main toolbox.
        /// </summary>
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    CurrentTool.Tool.IsSelected = value;
                }
            }
        }

        /// <summary>
        ///     ID of this ToolGroup.
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        ///     List of Tools held within.
        /// </summary>
        public List<PlugInTool> ChildTools {
            get { return _childTools; }
        }

        /// <summary>
        ///     Returns the number of child tools.
        /// </summary>
        public int Count {
            get { return _childTools.Count; }
        }

        /// <summary>
        ///     Currently selected Tool.
        /// </summary>
        public PlugInTool CurrentTool {
            get { return this[SelectedIndex]; }
            set { SelectedIndex = value.Index; }
        }

        /// <summary>
        ///     Returns the name of the currently selected tool
        /// </summary>
        public virtual string Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///     Access to the list of Tools.
        /// </summary>
        /// <param name="index">Index of the Tool.</param>
        public PlugInTool this[int index] {
            get {
                if ((index >= 0) && (index < _childTools.Count)) {
                    return _childTools[index];
                }
                return null;
            }
            set { _childTools[index] = value; }
        }

        /// <summary>
        ///     First key in the multi-gesture keystroke
        /// </summary>
        public Keys MultiGestureKey1 { get; set; }

        /// <summary>
        ///     Second key in the multi-gesture keystroke
        /// </summary>
        public Keys MultiGestureKey2 { get; set; }

        /// <summary>
        ///     Index of the currently selected Tool.
        /// </summary>
        public int SelectedIndex {
            get { return _selectedIndex; }
            set {
                if (value < 0) {
                    _selectedIndex = 0;
                }
                else if (value >= _childTools.Count) {
                    _selectedIndex = _childTools.Count - 1;
                }
                else {
                    _selectedIndex = value;
                }

                if (SelectedIndexChanged != null) {
                    SelectedIndexChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        ///     Returns the image of the currently selected tool
        /// </summary>
        public virtual Bitmap ToolBoxImage {
            get {
                if (CurrentTool != null) {
                    return CurrentTool.ToolBoxImage;
                }
                return ElfRes.undefined;
            }
            set { }
        }

        /// <summary>
        ///     ToolTipText of the currently selected Tool.
        /// </summary>
        public virtual string ToolTipText {
            get {
                if (CurrentTool != null) {
                    return CurrentTool.ToolTipText;
                }
                return string.Empty;
            }
            set { }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public BaseToolGroup() {
            // Assign the name of this group from the ElfToolGroup attribute on the class.
            object[] arr = GetType().GetCustomAttributes(typeof (ElfToolGroup), true);
            var NameAttr = (ElfToolGroup) arr[0];
            _name = NameAttr.Name;
            MultiGestureKey1 = Keys.None;
            MultiGestureKey2 = Keys.None;
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Add a new child tool to the ToolGroup
        /// </summary>
        /// <param name="newChildTool">Child tool to add to this ToolGroup</param>
        public void Add(PlugInTool childTool) {
            _childTools.Add(childTool);
            childTool.Index = _childTools.Count - 1;
            childTool.Button.Click += ChildTool_Click;
        }


        /// <summary>
        ///     Setup the tool
        /// </summary>
        public virtual void Initialize() {
            _savePath = AppendPath(Constants.TOOLSETTINGS, SafeName());
            SelectedIndex = LoadValue(SELECTED_TOOL, 0);
        }


        /// <summary>
        ///     Set the changed values back to Settings.
        /// </summary>
        public virtual void SaveSettings() {
            SaveValue(SELECTED_TOOL, _selectedIndex);
        }


        /// <summary>
        ///     Called when the editor is shutting down and clean up needs to occur
        /// </summary>
        public virtual void ShutDown() {
            for (int i = 0; i < _childTools.Count; i++) {
                _childTools[i].ShutDown();
            }
        }


        /// <summary>
        ///     Concatenates the path with a name, delimited by the pre-defined path delimiter character.
        /// </summary>
        /// <param name="path">Pre-built path</param>
        /// <param name="newNode">Name to be appended</param>
        protected string AppendPath(string path, string newNode) {
            return _settings.AppendPath(path, newNode);
        }


        /// <summary>
        ///     Clean up all child objects here, unlink all events and dispose
        /// </summary>
        protected override void DisposeChildObjects() {
            base.DisposeChildObjects();

            foreach (PlugInTool pTool in _childTools) {
                pTool.Dispose();
            }
            _childTools = null;
        }


        /// <summary>
        ///     Returns the integer value stored in Settings
        /// </summary>
        /// <param name="pathName">Path to find the setting value</param>
        /// <param name="defaultValue">Value to return if the setting value was not present</param>
        /// <returns>Setting value as an integer</returns>
        protected int LoadValue(string pathName, int defaultValue) {
            return _settings.GetValue(AppendPath(_savePath, pathName), defaultValue);
        }


        /// <summary>
        ///     Returns the tool's name, modified to be used for variable names or Xml node names
        /// </summary>
        /// <returns></returns>
        public string SafeName() {
            return Regex.Replace(Name, @"[^\w]", "", RegexOptions.None);
        }


        /// <summary>
        ///     Saves the value to the settings object as a string.
        /// </summary>
        /// <param name="pathName">Path to the place to store the setting value</param>
        /// <param name="value">Value to store</param>
        /// <param name="appendPath">Indicates that we should append the path passed in with the _savePath property of this Tool</param>
        protected void SaveValue(string pathName, string value, bool appendPath) {
            string Path = pathName;
            if (appendPath) {
                Path = AppendPath(_savePath, pathName);
            }
            _settings.SetValue(Path, value);
        }


        /// <summary>
        ///     Saves the value to the settings object as an integer.
        /// </summary>
        /// <param name="pathName">Path to the place to store the setting value</param>
        /// <param name="value">Value to store</param>
        protected void SaveValue(string pathName, int value) {
            SaveValue(pathName, value.ToString(), true);
        }

        #endregion [ Methods ]

        #region [ Events ]

        #region [ Event Handlers ]

        public event EventHandler SelectedIndexChanged;

        #endregion [ Event Handlers ]

        /// <summary>
        ///     By clicking this button, it assigns a new selected Index. The PlugInToolGroup object will catch this event, and
        ///     update it's button appropriately.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChildTool_Click(object sender, EventArgs e) {
            var Button = (PlugInToolStripButton) sender;
            Debug.WriteLine(Button.Name);
            SelectedIndex = Button.PlugInTool.Index;
        }

        #endregion [ Events ]
    }
}