using System;
using System.Collections;
using System.Collections.Generic;

namespace ElfCore.Channels {
    public class ChannelList : CollectionBase, ICollection<Channel> {
        public ChannelList() {}


        public ChannelList(Channel item) : this() {
            List.Add(item);
        }


        public void Add(Channel item) {
            List.Add(item);
        }


        public void AddRange(IEnumerable<Channel> collection) {
            foreach (var channel in collection)
                Add(channel);
        }

        
        private void AddRange(IList<Channel> collection) {
            foreach (var channel in collection)
                Add(channel);
        }


        public bool Contains(Channel item) {
            return List.Contains(item);
        }


        public void CopyTo(Channel[] array, int arrayIndex) {
            List.CopyTo(array, arrayIndex);
        }


        public int IndexOf(Channel item) {
            return List.IndexOf(item);
        }


        public void Insert(int index, Channel item) {
            List.Insert(index, item);
        }

        //todo use linq
        public ChannelList OrderByAscending() {
            var returnList = new ChannelList();
            var arr = ToArray();
            Array.Sort(arr);
            returnList.AddRange(arr);
            return returnList;
        }

        //todo use linq
        public IEnumerable<Channel> OrderByDescending() {
            var returnList = new ChannelList();
            var arr = ToArray();
            Array.Sort(arr);
            var stack = new Stack<Channel>();
            for (var i = 0; i < List.Count; i++)
                stack.Push(arr[i]);
            while (stack.Count > 0)
                returnList.Add(stack.Pop());
            return returnList;
        }


        public void Remove(Channel item) {
            if (List.Contains(item))
                List.Remove(item);
        }


        bool ICollection<Channel>.Remove(Channel item) {
            if (!List.Contains(item))
                return false;
            List.Remove(item);
            return true;
        }


        public Channel this[int index] {
            get { return (Channel) List[index]; }
            set { List[index] = value; }
        }


        //TODO: Linq?
        private Channel[] ToArray() {
            var arr = new Channel[Count];
            for (var i = 0; i < Count; i++)
                arr[i] = this[i];
            return arr;
        }


        //TODO: Linq
        public Channel Where(int id) {
            foreach (Channel item in List)
                if (item.ID == id)
                    return item;
            return null;
        }


        //TODO: Linq
        public ChannelList WhereList(bool isSelected) {
            var returnList = new ChannelList();
            foreach (Channel item in List)
                if (item.IsSelected == isSelected)
                    returnList.Add(item);
            return returnList;
        }


        public bool IsReadOnly {
            get { return List.IsReadOnly; }
        }

        #region [ IEnumerable ]

        /// <summary>
        /// Allows for "foreach" statements to be used on an instance of this class, to loop through all the Channels.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        IEnumerator<Channel> IEnumerable<Channel>.GetEnumerator() {
            return new ChannelListEnumerator(List.GetEnumerator());
        }

        #endregion [ IEnumerable ]

        private class ChannelListEnumerator : IEnumerator<Channel> {
            private readonly IEnumerator _enumerator;


            public ChannelListEnumerator(IEnumerator enumerator) {
                _enumerator = enumerator;
            }


            public Channel Current {
                get { return (Channel) _enumerator.Current; }
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
        }

    }
}