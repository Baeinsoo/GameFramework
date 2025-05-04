using System.Collections.Generic;
using System;
using System.Collections;

namespace GameFramework
{
    public class BoundedQueue<T> : IEnumerable<T>
    {
        public int Count => queue.Count;
        public bool IsFull => queue.Count >= MaxSize;
        public int MaxSize { get; private set; }

        private readonly Queue<T> queue;

        public BoundedQueue(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Maximum size must be greater than zero", nameof(maxSize));
            }

            this.MaxSize = maxSize;
            queue = new Queue<T>(maxSize);
        }

        public void Enqueue(T item)
        {
            if (queue.Count >= MaxSize)
            {
                queue.Dequeue();
            }

            queue.Enqueue(item);
        }

        public T Dequeue()
        {
            if (queue.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return queue.Dequeue();
        }

        public T Peek()
        {
            if (queue.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return queue.Peek();
        }

        public bool TryDequeue(out T result)
        {
            if (queue.Count > 0)
            {
                result = queue.Dequeue();
                return true;
            }

            result = default;
            return false;
        }

        public void Clear()
        {
            queue.Clear();
        }

        public bool Contains(T item)
        {
            return queue.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
