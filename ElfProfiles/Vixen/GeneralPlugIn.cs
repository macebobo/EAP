using System.Collections;
using System.Collections.Generic;
using System.Xml;

using ElfCore.Util;

namespace ElfProfiles.Vixen {
    /// <summary>
    ///     Holds the general PlugIn data for a Vixen Output Plugin
    /// </summary>
    public class GeneralPlugIn {
        #region [ Constants ]

        // Attribute names
        private const string Attribute_Name = "name";
        private const string Attribute_Key = "key";
        private const string Attribute_ID = "id";
        private const string Attribute_Enabled = "enabled";
        private const string Attribute_From = "from";
        public const string Attribute_To = "to";

        #endregion [ Constants ]

        #region [ Private Variables ]

        private bool _enabled;
        private int _fromChannel;
        private int _id;
        private int _key;
        private string _name;
        private XmlNode _node;
        private int _toChannel;
        protected XmlHelper _xmlHelper = XmlHelper.Instance;

        #endregion [ Private Variables ]

        #region [ Properties ]

        public virtual int ID {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int Key {
            get { return _key; }
            set { _key = value; }
        }

        public virtual bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public virtual string Name {
            get { return _name; }
            set { _name = value; }
        }

        public virtual int FromChannel {
            get { return _fromChannel; }
            set { _fromChannel = value; }
        }

        public virtual int ToChannel {
            get { return _toChannel; }
            set { _toChannel = value; }
        }

        public virtual XmlNode PlugInNode {
            get { return _node; }
            set { _node = value; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public GeneralPlugIn() {
            _id = -1;
            _key = -1;
            _enabled = false;
            _name = string.Empty;
            _fromChannel = -1;
            _toChannel = -1;
            _node = null;
        }


        public GeneralPlugIn(XmlNode plugInNode) : this() {
            _node = plugInNode;
            if (_node != null) {
                _name = _xmlHelper.GetAttributeValue(plugInNode, Attribute_Name, string.Empty);
                _id = _xmlHelper.GetAttributeValue(plugInNode, Attribute_ID, -1);
                _key = _xmlHelper.GetAttributeValue(plugInNode, Attribute_Key, -1);
                _enabled = _xmlHelper.GetAttributeValue(plugInNode, Attribute_Enabled, false);
                _fromChannel = _xmlHelper.GetAttributeValue(plugInNode, Attribute_From, -1) - 1;
                _toChannel = _xmlHelper.GetAttributeValue(plugInNode, Attribute_To, -1) - 1;
            }
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        public virtual XmlNode CreateNode(XmlDocument doc) {
            XmlNode Node = doc.CreateElement(BaseVixen.XmlNode_PlugIn);
            _xmlHelper.AddAttribute(Node, Attribute_Name, Name);
            _xmlHelper.AddAttribute(Node, Attribute_ID, ID);
            _xmlHelper.AddAttribute(Node, Attribute_Key, Key);
            _xmlHelper.AddAttribute(Node, Attribute_Enabled, Enabled);
            _xmlHelper.AddAttribute(Node, Attribute_From, (FromChannel + 1));
            _xmlHelper.AddAttribute(Node, Attribute_To, (ToChannel + 1));

            // Now populate this with the guts the the saved node, if any
            if (PlugInNode != null) {
                Node.InnerXml = PlugInNode.InnerXml;
            }

            return Node;
        }

        #endregion [ Methods ]
    }

    public class GeneralPlugInList : CollectionBase, IList<GeneralPlugIn>, ICollection<GeneralPlugIn> {
        #region IList<GeneralPlugIn> Members

        public void Add(GeneralPlugIn item) {
            List.Add(item);
        }


        public bool Contains(GeneralPlugIn item) {
            return List.Contains(item);
        }


        public void CopyTo(GeneralPlugIn[] array, int arrayIndex) {
            List.CopyTo(array, arrayIndex);
        }


        public int IndexOf(GeneralPlugIn item) {
            return List.IndexOf(item);
        }


        public void Insert(int index, GeneralPlugIn item) {
            List.Insert(index, item);
        }


        public bool IsReadOnly {
            get { return List.IsReadOnly; }
        }


        bool ICollection<GeneralPlugIn>.Remove(GeneralPlugIn item) {
            if (!List.Contains(item)) {
                return false;
            }
            List.Remove(item);
            return true;
        }


        public GeneralPlugIn this[int index] {
            get { return (GeneralPlugIn) List[index]; }
            set { List[index] = value; }
        }


        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        IEnumerator<GeneralPlugIn> IEnumerable<GeneralPlugIn>.GetEnumerator() {
            return new GeneralPlugInListEnumerator(List.GetEnumerator());
        }

        #endregion

        public void Remove(GeneralPlugIn item) {
            List.Remove(item);
        }


        public GeneralPlugIn Where(string name) {
            foreach (GeneralPlugIn Item in List) {
                if (string.Compare(Item.Name, name, true) == 0) {
                    return Item;
                }
            }
            return null;
        }


        public List<GeneralPlugIn> WhereNot(string name) {
            var ReturnList = new List<GeneralPlugIn>();
            foreach (GeneralPlugIn Item in List) {
                if (string.Compare(Item.Name, name, true) != 0) {
                    ReturnList.Add(Item);
                }
            }
            return ReturnList;
        }

        #region Nested type: GeneralPlugInListEnumerator

        private class GeneralPlugInListEnumerator : IEnumerator<GeneralPlugIn> {
            private readonly IEnumerator _enumerator;


            public GeneralPlugInListEnumerator(IEnumerator enumerator) {
                _enumerator = enumerator;
            }

            #region IEnumerator<GeneralPlugIn> Members

            public GeneralPlugIn Current {
                get { return (GeneralPlugIn) _enumerator.Current; }
            }

            object IEnumerator.Current {
                get { return _enumerator.Current; }
            }


            public bool MoveNext() {
                return _enumerator.MoveNext();
            }


            public void Reset() {
                _enumerator.Reset();
            }


            public void Dispose() {}

            #endregion
        }

        #endregion
    }
}