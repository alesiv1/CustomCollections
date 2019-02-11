using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static KeyValuePair<TKey, TValue>[] _hashTable;

        public int Count { get; private set; } = 0;

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new KeyValuePair<TKey, TValue>[1];

        public void Clear()
        {
            _hashTable = new KeyValuePair<TKey, TValue>[1];
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException("key");
            }
            var position = GetPosition(item.Key);
            return EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_hashTable[position], item);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var position = GetPosition(key);
            return EqualityComparer<TKey>.Default.Equals(_hashTable[position].Key,key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var position = GetPosition(key);
            if (position >= 0 && position < Capacity)
            {
                value = _hashTable[position].Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                var position = GetPosition(key);
                return _hashTable[position].Value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                var position = GetPosition(key);
                _hashTable[position] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0 || array.Length - arrayIndex < Count)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            try
            {
                for (int i = 0; i < Capacity; i++)
                {
                    if (_hashTable[i].Key != null)
                    {
                        array[arrayIndex] = _hashTable[i];
                        arrayIndex++;
                    }
                }
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException("key");
            }
            if (!HasAvailableMemory()) IncreaseSize();
            var position = GetPosition(item.Key);
            _hashTable[position] = item;
            Count++;
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            if (key == null || !ContainsKey(key))
            {
                throw new ArgumentNullException("key");
            }
            var position = GetPosition(key);
            if (Contains(_hashTable[position]))
            {
                _hashTable[position] = new KeyValuePair<TKey, TValue>();
                Count--;
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _hashTable)
            {
                if (!item.Equals(null) || !EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, new KeyValuePair<TKey, TValue>()))
                {
                    yield return item;
                }
            }
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
                if (item.Key != null && !EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, new KeyValuePair<TKey, TValue>()))
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
                if (item.Value != null && !EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(item, new KeyValuePair<TKey, TValue>()))
                {
                    values.Add(item.Value);
                }
            }
            return values;
        }

        private bool HasAvailableMemory()
        {
            return Count < Capacity;
        }

        private void IncreaseSize()
        {
            var newHashTable = new KeyValuePair<TKey, TValue>[Capacity * 2];
            for(int i = 0; i < Capacity; i++)
            {
                if (_hashTable[i].Key != null && !EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(_hashTable[i], new KeyValuePair<TKey, TValue>()))
                {
                    newHashTable[i] = _hashTable[i];
                }
            }
            _hashTable = newHashTable;
        }
    }
}
