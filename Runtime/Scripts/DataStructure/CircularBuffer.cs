using System;
using System.Collections.Generic;
using System.Collections;

namespace GameFramework
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private T[] buffer;
        private int start;
        private int size;

        public int Count => size;
        public int Capacity => buffer.Length;

        public CircularBuffer(int capacity)
        {
            buffer = new T[capacity];
            start = 0;
            size = 0;
        }

        public void Add(T item)
        {
            if (size == buffer.Length)
            {
                buffer[start] = item;
                start = (start + 1) % buffer.Length;
            }
            else
            {
                buffer[(start + size) % buffer.Length] = item;
                size++;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= size)
                {
                    throw new IndexOutOfRangeException();
                }

                return buffer[(start + index) % buffer.Length];
            }
        }

        public void Clear()
        {
            size = 0;
            start = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < size; i++)
            {
                yield return buffer[(start + i) % buffer.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
