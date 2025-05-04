using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class BoundedList<T> : IList<T>
    {
        public int Count => list.Count;
        public bool IsFull => list.Count >= maxSize;
        public int MaxSize => maxSize;
        public bool IsReadOnly => false;

        private readonly List<T> list;
        private readonly int maxSize;

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public BoundedList(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Maximum size must be greater than zero", nameof(maxSize));
            }

            this.maxSize = maxSize;
            list = new List<T>(maxSize);
        }

        public BoundedList(int maxSize, IEnumerable<T> collection) : this(maxSize)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void Add(T item)
        {
            if (list.Count >= maxSize)
            {
                list.RemoveAt(0);
            }

            list.Add(item);
        }

        public void Insert(int index, T item)
        {
            if (list.Count >= maxSize)
            {
                list.RemoveAt(0);

                if (index > 0)
                {
                    index--;
                }
            }

            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
