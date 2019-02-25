using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomCollections
{
    public class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected class CustomHashTableElement
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
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item.Key));
            }
            if (!HasAvailableMemory()) IncreaseSize();
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException();
            }
            var position = GetPosition(item.Key);
            if (_hashTable[position] == null)
            {
                _hashTable[position] = new CustomHashTableElement(item.Key, item.Value);
            }
            else
            {
                var element = _hashTable[position];
                while (element.Next != null)
                {
                    element = element.Next;
                }

                element.Next = new CustomHashTableElement(item.Key, item.Value);
            }
            Count++;
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
                    throw new ArgumentException("This key isn't exist in hash table", nameof(key));
                }
                element.Value = value;
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
            if (element != null) return true;
            return false;
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

        public bool Remove(KeyValuePair<TKey, TValue> item)
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
            var position = GetPosition(key);
            if (_hashTable[position] == null) return false;
            var element = _hashTable[position];
            CustomHashTableElement parent = null;
            while (!IsKeysEqual(element.Key, key))
            {
                if(element.Next == null) break;
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

        private int GetPosition(TKey key, bool isForExpansion = false)
        {
            var hash = 0;
            var prefix = 0;
            var capacity = isForExpansion ? Capacity * 2 : Capacity;
            foreach (var symbol in key.ToString())
            {
                prefix++;
                hash += symbol * prefix;
            }
            var pos = Math.Abs(hash % capacity);
            return pos;
        }

        private bool HasAvailableMemory(double percentageFilling = 0.7)
        {
            var hasAvailableMemory = (int)Capacity * percentageFilling > Count;
            return hasAvailableMemory;
        }

        private CustomHashTableElement GetElementByKey(TKey key)
        {
            var position = GetPosition(key);
            if (_hashTable[position] == null) return null;
            var element = _hashTable[position];
            while (!IsKeysEqual(element.Key, key))
            {
                if (element.Next == null) break;
                element = element.Next;
            }
            if (!IsKeysEqual(element.Key, key))
            {
                element = null;
            }
            return element;
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

        private void IncreaseSize()
        {
            var newHashTable = new CustomHashTableElement[Capacity * 2];
            foreach (var item in this)
            {
                var position = GetPosition(item.Key, true);
                if (newHashTable[position] == null)
                {
                    newHashTable[position] = new CustomHashTableElement(item.Key, item.Value);
                }
                else
                {
                    var element = newHashTable[position];
                    while (element.Next != null)
                    {
                        element = element.Next;
                    }
                    element.Next = new CustomHashTableElement(item.Key, item.Value);
                }
            }
            _hashTable = newHashTable;
        }

        private bool IsKeysEqual(TKey leftItem, TKey rightKey)
        {
            return EqualityComparer<TKey>.Default.Equals(leftItem, rightKey);
        }

        private bool IsValuesEqual(TValue leftItem, TValue rightItem)
        {
            return EqualityComparer<TValue>.Default.Equals(leftItem, rightItem);
        }
    }
}
