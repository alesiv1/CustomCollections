using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private KeyValuePair<TKey, TValue>[] _hashTable;

        public int Count { get; private set; } = 0;

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new KeyValuePair<TKey, TValue>[8];

        public void Clear()
        {
            _hashTable = new KeyValuePair<TKey, TValue>[8];
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item.Key));
            }
            var position = GetPosition(item.Key);
            return IsItemsEqual(item, _hashTable[position]);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            return IsKeysEqual(key, _hashTable[position].Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (position >= 0 && position < Capacity)
            {
                if (!IsItemsEqual(_hashTable[position], default(KeyValuePair<TKey, TValue>)))
                {
                    value = _hashTable[position].Value;
                    return true;
                }
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
                    throw new ArgumentNullException(nameof(key));
                }
                var position = GetPosition(key);
                return _hashTable[position].Value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var position = GetPosition(key);
                _hashTable[position] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0 || array.Length - arrayIndex < Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            for (int i = 0; i < Capacity; i++)
            {
                if (!IsItemsEqual(_hashTable[i], default(KeyValuePair<TKey, TValue>)))
                {
                    array[arrayIndex] = _hashTable[i];
                    arrayIndex++;
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException();
            }
            if (!HasAvailableMemory()) IncreaseSize();
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException();
            }
            var position = GetPosition(item.Key);
            if (!IsItemsEqual(_hashTable[position],default(KeyValuePair<TKey, TValue>)))
            {
                throw new ArgumentException();
            }
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
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (!IsItemsEqual(_hashTable[position], default(KeyValuePair<TKey, TValue>)))
            {
                _hashTable[position] = default(KeyValuePair<TKey, TValue>);
                Count--;
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _hashTable)
            {
                if (!IsItemsEqual(item, default(KeyValuePair<TKey, TValue>)))
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
            var pos = Math.Abs(hash) % Capacity;
            return pos;
        }

        private ICollection<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (var item in _hashTable)
            {
                if (!IsItemsEqual(item, default(KeyValuePair<TKey, TValue>)))
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
                if (!IsItemsEqual(item, default(KeyValuePair<TKey, TValue>)))
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
            var position = 0;
            for(int i = 0; i < Capacity; i++)
            {
                if (!IsItemsEqual(_hashTable[i], default(KeyValuePair<TKey, TValue>)))
                {
                    position = GetPosition(_hashTable[i].Key);
                    newHashTable[position] = _hashTable[i];
                }
            }
            _hashTable = newHashTable;
        }

        private bool IsKeysEqual(TKey leftKey, TKey rightKey)
        {
            return EqualityComparer<TKey>.Default.Equals(leftKey, rightKey);
        }

        private bool IsItemsEqual(KeyValuePair<TKey, TValue> leftItem, KeyValuePair<TKey, TValue> rightItem)
        {
            return EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(leftItem, rightItem);
        }
    }
}
