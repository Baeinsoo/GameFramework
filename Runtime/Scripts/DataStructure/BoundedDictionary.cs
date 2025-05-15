using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class BoundedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public int Count => dictionary.Count;
        public bool IsFull => dictionary.Count >= maxSize;
        public int MaxSize => maxSize;
        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => dictionary.Keys;
        public ICollection<TValue> Values => dictionary.Values;

        private readonly int maxSize;
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly LinkedList<TKey> insertionOrder;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key) == false)
                {
                    Add(key, value);
                }
                else
                {
                    dictionary[key] = value;
                }
            }
        }

        public BoundedDictionary(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Maximum size must be greater than zero", nameof(maxSize));
            }

            this.maxSize = maxSize;
            dictionary = new Dictionary<TKey, TValue>(maxSize);
            insertionOrder = new LinkedList<TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }

            if (dictionary.Count >= maxSize)
            {
                var oldestKey = insertionOrder.First.Value;
                insertionOrder.RemoveFirst();
                dictionary.Remove(oldestKey);
            }

            dictionary.Add(key, value);
            insertionOrder.AddLast(key);
        }

        public bool Remove(TKey key)
        {
            if (dictionary.Remove(key))
            {
                insertionOrder.Remove(key);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            dictionary.Clear();
            insertionOrder.Clear();
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in insertionOrder)
            {
                yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
    }
}
