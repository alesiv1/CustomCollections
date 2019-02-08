using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CustomCollections
{
    class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static KeyValuePair<TKey, TValue>[] _hashTable;

        public int Count { get; private set; } = 0;
        public int Capacity { get; private set; } = 1;
        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable()
        {
            _hashTable = new KeyValuePair<TKey, TValue>[1];
        }

        public void Clear()
        {
            _hashTable = new KeyValuePair<TKey, TValue>[1];
            Count = 0;
            Capacity = 1;
        }

        public TValue this[TKey key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetPosition(TKey key)
        {
            var hash = key.GetHashCode();
            var pos = Math.Abs(hash % Capacity);
            return pos;
        }

        private ICollection<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (var item in _hashTable)
            {
                if (item.Key != null)
                {
                    keys.Add(item.Key);
                }
            }
            return keys;
        }

        private ICollection<TValue> GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (var item in _hashTable)
            {
                if (item.Value != null)
                {
                    values.Add(item.Value);
                }
            }
            return values;
        }
    }
}
