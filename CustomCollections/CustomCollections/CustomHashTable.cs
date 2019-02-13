using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCollections
{
    class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private LinkedList<KeyValuePair<TKey, TValue>>[] _hashTable;

        public int Count { get; private set; } = 0;

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new LinkedList<KeyValuePair<TKey, TValue>>[8];

        public void Clear()
        {
            _hashTable = new LinkedList<KeyValuePair<TKey, TValue>>[8];
            Count = 0;
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
                var node = _hashTable[position].First;
                while (node != null)
                {
                    if (IsKeysEqual(node.Value.Key, key)) return node.Value.Value;
                    node = node.Next;
                }
                throw new ArgumentException();
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var position = GetPosition(key);
                _hashTable[position].AddFirst(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item.Key));
            }
            var position = GetPosition(item.Key);
            if (_hashTable[position] == null) return false;
            var node = _hashTable[position].First;
            while (node != null)
            {
                if (IsItemsEqual(node.Value, item)) return true;
                node = node.Next;
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (_hashTable[position] == null) return false;
            var node = _hashTable[position].First;
            while (node != null)
            {
                if (IsKeysEqual(node.Value.Key, key)) return true;
                node = node.Next;
            }
            return false;
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
                if (_hashTable[position] != null)
                {
                    var node = _hashTable[position].First;
                    while (node != null)
                    {
                        if (IsKeysEqual(node.Value.Key, key))
                        {
                            value = node.Value.Value;
                            return true;
                        }
                        node = node.Next;
                    }
                }
            }
            value = default(TValue);
            return false;
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
                if (_hashTable[i] != null)
                {
                    foreach (var node in _hashTable[i])
                    {
                        array[arrayIndex] = node;
                        arrayIndex++;
                    }
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
            var position = GetPosition(item.Key);
            if (_hashTable[position] == null)
            {
                _hashTable[position] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException();
            }
            _hashTable[position].AddFirst(item);
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
            if (_hashTable[position] != null)
            {
                if (_hashTable[position].Count > 1)
                {
                    var node = _hashTable[position].First;
                    while (node != null)
                    {
                        if (IsKeysEqual(node.Value.Key, key))
                        {
                            node.Value = node.Next.Value;
                            while (node != null)
                            {
                                node = node.Next;
                                if (node.Next != null) node.Value = node.Next.Value;
                                else
                                {
                                    break;
                                }
                            }
                            _hashTable[position].RemoveLast();
                            Count--;
                            return true;
                        }
                        node = node.Next;
                    }
                }
                else
                {
                    _hashTable[position] = null;
                    Count--;
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _hashTable)
            {
                if (item != null)
                {
                    foreach (var node in item)
                    {
                        yield return node;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetPosition(TKey key)
        {
            var hash = 0;
            var prefix = 1;
            foreach (var symbol in key.ToString())
            {
                hash += (int)symbol * prefix;
                prefix++;
            }
            var pos = Math.Abs((hash * key.ToString().Length) % Capacity);
            return pos;
        }

        private ICollection<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (var item in _hashTable)
            {
                if (item != null)
                {
                    foreach (var node in item)
                    {
                        keys.Add(node.Key);
                    }
                }
            }
            return keys;
        }

        private ICollection<TValue> GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (var item in _hashTable)
            {
                if (item != null)
                {
                    foreach (var node in item)
                    {
                        values.Add(node.Value);
                    }
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
            var newHashTable = new LinkedList<KeyValuePair<TKey, TValue>>[Capacity * 2];
            for(int i = 0; i < Capacity; i++)
            {
                if (_hashTable[i] != null)
                {
                    foreach (var node in _hashTable[i])
                    {
                        var position = GetPosition(node.Key);
                        if(newHashTable[position] == null) newHashTable[position] = new LinkedList<KeyValuePair<TKey, TValue>>();
                        newHashTable[position].AddFirst(node);
                    }
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
