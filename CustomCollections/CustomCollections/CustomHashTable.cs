using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomCollections
{
    public class RecordInHashTable<TKey, TValue>
    {
        public class RecordsList<TK, TV>
        {
            public TK Key { get; set; }
            public TV Value { get; set; }
            public RecordsList<TK, TV> Next { get; set; }

            public RecordsList(TK key, TV value)
            {
                this.Key = key;
                this.Value = value;
            }

            public void Add(TK key, TV value)
            {
                var lastItem = GetLastItem();
                lastItem.Next = new RecordsList<TK, TV>(key, value);
            }

            public bool Remove(TK key)
            {
                var item = this;
                while (!IsKeysEqual(item.Key, key) || item.Next != null)
                {
                    item = item.Next;
                }
                if (!IsKeysEqual(item.Key, key))
                {
                    return false;
                }
                while (item.Next != null)
                {
                    item.Key = item.Next.Key;
                    item.Value = item.Next.Value;
                    item = item.Next;
                }
                item = null;
                return true;
            }

            public RecordsList<TK, TV> Get(TK key)
            {
                var item = this;
                while (!IsKeysEqual(item.Key, key) || item.Next != null)
                {
                    item = item.Next;
                }
                if (!IsKeysEqual(item.Key, key))
                {
                    throw new ArgumentException(nameof(key));
                }
                return item;
            }

            private RecordsList<TK, TV> GetLastItem()
            {
                var item = this;
                while (item.Next != null)
                {
                    item = item.Next;
                }
                return item;
            }

            private bool IsKeysEqual(TK leftKey, TK rightKey)
            {
                return EqualityComparer<TK>.Default.Equals(leftKey, rightKey);
            }
        }

        public int Count { get; private set; }
        public List<TKey> EquivalentKeys { get; private set; }
        public RecordsList<TKey, TValue> Records { get; set; }

        public RecordInHashTable(TKey key, TValue value)
        {
            this.Records = new RecordsList<TKey, TValue>(key, value);
            this.Count = 1;
            this.EquivalentKeys = new List<TKey>{ key };
        }

        public void Add(TKey key, TValue value)
        {
            if (Records != null)
            {
                Records.Add(key, value);
                EquivalentKeys.Add(key);
            }
            else
            {
                Records = new RecordsList<TKey, TValue>(key, value);
                EquivalentKeys = new List<TKey>() { key };
            }
            Count += 1;
        }

        public RecordsList<TKey, TValue> GetRecord(TKey key)
        {
            return Records.Get(key);
        }

        public bool Remove(TKey key)
        {
            var isRemoved = Records.Remove(key);
            if (isRemoved)
            {
                Count -= 1;
                EquivalentKeys.Remove(key);
            }
            return isRemoved;
        }

        public bool ContainsKey(TKey key)
        {
            return EquivalentKeys.Contains(key);
        }
    }

    public class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private RecordInHashTable<TKey, TValue>[] _hashTable;

        public int Count { get; private set; }

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new RecordInHashTable<TKey, TValue>[8];

        public void Clear()
        {
            _hashTable = new RecordInHashTable<TKey, TValue>[8];
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
                if (!_hashTable[position].ContainsKey(key))
                {
                    throw new ArgumentNullException(nameof(key), "Ku");
                }
                return _hashTable[position].GetRecord(key).Value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Ku");
                }
                var position = GetPosition(key);
                if (!_hashTable[position].ContainsKey(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                _hashTable[position].GetRecord(key).Value = value;
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
            var value = _hashTable[position].GetRecord(item.Key).Value;
            return IsValuesEqual(value, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (_hashTable[position] == null) return false;
            return _hashTable[position].ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            try
            {
                value = _hashTable[position].GetRecord(key).Value;
                return true;
            }
            catch
            {
                value = default(TValue);
                return false;
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
                if (_hashTable[i] != null)
                {
                    var record = _hashTable[i].Records;
                    while (record != null)
                    {
                        array[arrayIndex] = new KeyValuePair<TKey, TValue>(record.Key, record.Value);
                        arrayIndex++;
                        record = record.Next;
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
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException();
            }

            if (_hashTable[position] == null)
            {
                _hashTable[position] = new RecordInHashTable<TKey, TValue>(item.Key, item.Value);
            }
            else
            {
                _hashTable[position].Add(item.Key, item.Value);
            }
            Count++;
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (_hashTable[position] == null) return false;
            return _hashTable[position].ContainsKey(key) && _hashTable[position].Remove(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _hashTable)
            {
                if (item != null)
                {
                    var record = item.Records;
                    while (record != null)
                    {
                        yield return new KeyValuePair<TKey, TValue>(record.Key, record.Value);
                        record = record.Next;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetPosition(TKey key, int capacity = 1)
        {
            var hash = 0;
            var prefix = 0;
            foreach (var symbol in key.ToString())
            {
                prefix++;
                hash += symbol * prefix;
            }
            if (capacity == 1) capacity = Capacity;
            var pos = Math.Abs(hash % capacity);
            return pos;
        }

        private ICollection<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (var item in _hashTable)
            {
                if (item != null)
                {
                    foreach (var node in item.EquivalentKeys)
                    {
                        keys.Add(node);
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
                    var record = item.Records;
                    while(record != null)
                    {
                        values.Add(record.Value);
                        record = record.Next;
                    }
                }
            }
            return values;
        }

        private bool HasAvailableMemory()
        {
            var oneProsent = Capacity > Count;
            return oneProsent;
        }

        private void IncreaseSize()
        {
            RecordInHashTable<TKey, TValue>[] newHashTable = new RecordInHashTable<TKey, TValue>[Capacity * 2];
                for (int i = 0; i < Capacity; i++)
            {
                if (_hashTable[i] != null)
                {
                    var record = _hashTable[i].Records;
                    while(record != null)
                    {
                        var position = GetPosition(record.Key, Capacity * 2);
                        if (newHashTable[position] == null)
                        {
                            newHashTable[position] = new RecordInHashTable<TKey, TValue>(record.Key, record.Value);
                        }
                        else
                        {
                            newHashTable[position].Add(record.Key, record.Value);
                        }
                        record = record.Next;
                    }
                }
            }
            _hashTable = newHashTable;
        }

        private bool IsValuesEqual(TValue leftItem, TValue rightItem)
        {
            return EqualityComparer<TValue>.Default.Equals(leftItem, rightItem);
        }
    }
}
