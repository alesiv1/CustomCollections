using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CustomCollections
{
    public class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public class CustomHashTableElement
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public CustomHashTableElement Next { get; set; }

            public CustomHashTableElement(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        private CustomHashTableElement[] _hashTable;

        public int Count { get; private set; }

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new CustomHashTableElement[8];

        public void Clear()
        {
            _hashTable = new CustomHashTableElement[8];
            Count = 0;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!HasAvailableMemory()) IncreaseSize();
            if (ContainsKey(key))
            {
                throw new ArgumentException("This key is already exist in hash table", nameof(key));
            }
            Add(ref _hashTable, key, value);
            Count++;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var element = GetElementByKey(key);
                if (element == null)
                {
                    throw new ArgumentException("This key isn't exist in hash table", nameof(key));
                }
                return element.Value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var element = GetElementByKey(key);
                if (element == null)
                {
                    var position = _hashTable.GetPositionInHashTableByKey(key);
                    _hashTable[position] = new CustomHashTableElement(key, value);
                }
                else
                {
                    element.Value = value;
                }
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item.Key));
            }
            var element = GetElementByKey(item.Key);
            if (element == null) return false;
            return IsValuesEqual(element.Value, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var element = GetElementByKey(key);
            return element != null;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var element = GetElementByKey(key);
            if (element != null)
            {
                value = element.Value;
                return true;
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
            foreach (var element in this)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(element.Key, element.Value);
                arrayIndex++;
            }
        }

        public bool Remove(TKey key, TValue value)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this).Remove(new KeyValuePair<TKey, TValue>(key, value));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Contains(item)) return false;
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = _hashTable.GetPositionInHashTableByKey(key);
            if (_hashTable[position] == null) return false;
            var element = _hashTable[position];
            CustomHashTableElement parent = null;
            while (element.Next != null)
            {
                if(IsKeysEqual(element.Key, key)) break;
                parent = element;
                element = element.Next;
            }
            if (!IsKeysEqual(element.Key, key)) return false;
            if (parent == null)
            {
                _hashTable[position] = element.Next;
            }
            else
            {
                parent.Next = element.Next;
            }
            Count--;
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var items in _hashTable)
            {
                var element = items;
                while (element != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(element.Key, element.Value);
                    element = element.Next;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool HasAvailableMemory(double percentageFilling = 0.7)
        {
            var hasAvailableMemory = Capacity * percentageFilling > Count;
            return hasAvailableMemory;
        }

        private CustomHashTableElement GetElementByKey(TKey key)
        {
            var position = _hashTable.GetPositionInHashTableByKey(key);
            if (_hashTable[position] == null) return null;
            var element = _hashTable[position];
            while (element != null)
            {
                if (IsKeysEqual(element.Key, key))
                {
                    return element;
                }
                element = element.Next;
            }
            return null;
        }

        private ICollection<TKey> GetKeys()
        {
            var keys = new List<TKey>();
            foreach (var element in this)
            {
               keys.Add(element.Key);
            }
            return keys;
        }

        private ICollection<TValue> GetValues()
        {
            var values = new List<TValue>();
            foreach (var element in this)
            {
                values.Add(element.Value);
            }
            return values;
        }

        private void IncreaseSize(int howMuchToIncrease = 2)
        {
            var newHashTable = new CustomHashTableElement[Capacity * howMuchToIncrease];
            foreach (var (key, value) in this)
            {
                Add(ref newHashTable, key, value);
            }
            _hashTable = newHashTable;
        }

        private static void Add(ref CustomHashTableElement[] hashTable, TKey key, TValue value)
        {
            var position = hashTable.GetPositionInHashTableByKey(key);
            if (hashTable[position] == null)
            {
                hashTable[position] = new CustomHashTableElement(key, value);
            }
            else
            {
                var element = hashTable[position];
                while (element.Next != null)
                {
                    element = element.Next;
                }
                element.Next = new CustomHashTableElement(key, value);
            }
        }

        private static bool IsKeysEqual(TKey leftItem, TKey rightKey)
        {
            return EqualityComparer<TKey>.Default.Equals(leftItem, rightKey);
        }

        private static bool IsValuesEqual(TValue leftItem, TValue rightItem)
        {
            return EqualityComparer<TValue>.Default.Equals(leftItem, rightItem);
        }
    }

    public static class ExtensionCustomHashTableClass
    {
        public static int GetPositionInHashTableByKey<TKey, TValue>(this CustomHashTable<TKey,TValue>.CustomHashTableElement[] hashTable, TKey key)
        {
            return GetPosition(hashTable.Length, key);
        }

        public static int GetPosition<TKey>(int capacity, TKey key)
        {
            var hash = 0;
            var prefix = 0;
            foreach (var symbol in key.ToString())
            {
                prefix++;
                hash += symbol * prefix;
            }
            var pos = Math.Abs(hash % capacity);
            return pos;
        }
    }
}
