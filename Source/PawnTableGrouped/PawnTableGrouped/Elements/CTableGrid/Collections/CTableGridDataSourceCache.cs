using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    abstract class CTableGridDataSourceCache<T> : IList<T> where T : class
    {
        public CTableGridDataSourceCache(ICTableGridDataSource dataSource)
        {
            this.dataSource = new Verse.WeakReference<ICTableGridDataSource>(dataSource);
        }
        Verse.WeakReference<ICTableGridDataSource> dataSource;
        protected ICTableGridDataSource DataSource => dataSource.Target;

        public void Invalidate()
        {
            ClearItems();
            count = -1;
        }

        private int count = -1;
        public int Count
        {
            get
            {
                if (count == -1)
                {
                    count = itemsCount();
                    EnsureCapacity();
                }
                return count;
            }
        }

        protected abstract T itemAt(int index);
        protected abstract int itemsCount();

        T[] items = new T[32];

        protected void EnsureCapacity()
        {
            if (items.Length < Count)
            {
                ResizeArrays();
            }
        }

        protected virtual void ResizeArrays()
        {
            Array.Resize(ref items, Count);
        }

        protected virtual void ClearItems()
        {
            Array.Clear(items, 0, items.Length);
        }

        #region IList<T>
        public T this[int index]
        {
            get
            {
                var item = items[index];
                if (item == null)
                {
                    item = itemAt(index);
                    items[index] = item;
                }
                return item;
            }
            set => throw new Exception("read only collection");
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("arrayIndex");
            }
            Array.Copy(items, 0, array, arrayIndex, Count);
        }

        private IEnumerable<T> Enumerate()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return items[i];
            }
        }

        public bool Contains(T item) => Enumerate().Contains(item);

        public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly => true;

        public void Add(T item) => throw new Exception("read only collection");
        public void Clear() => throw new Exception("read only collection");
        public bool Remove(T item) => throw new Exception("read only collection");
        public void Insert(int index, T item) => throw new Exception("read only collection");
        public void RemoveAt(int index) => throw new Exception("read only collection");

        #endregion
    }

}
