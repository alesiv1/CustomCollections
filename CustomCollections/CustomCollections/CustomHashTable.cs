using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    public class CustomHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public class CustomHashTableElements
        {
            public TKey Key { get; private set; }
            public TValue Value { get; private set; }
            public CustomHashTableElements Next { get; private set; }

            public CustomHashTableElements(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }

            public void Add(TKey key, TValue value)
            {
                var lastElement = GetLastElement();
                lastElement.Next = new CustomHashTableElements(key, value);
            }

            public bool Remove(TKey key)
            {
                var element = this;
                CustomHashTableElements parent = null;
                while (!IsKeysEqual(element.Key, key))
                {
                    if (element.Next == null) break;
                    parent = element;
                    element = element.Next;
                }
                if (!IsKeysEqual(element.Key, key))
                {
                    return false;
                }
                while (element.Next != null)
                {
                    element.Key = element.Next.Key;
                    element.Value = element.Next.Value;
                    parent = element;
                    element = element.Next;
                }
                if (parent != null)
                {
                    parent.Next = null;
                    return true;
                }
                return false;
            }

            public TValue GetValue(TKey key)
            {
                var element = FindByKey(key);
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }
                return element.Value;
            }

            public void SetValue(TKey key, TValue value)
            {
                var element = FindByKey(key);
                if (element != null) element.Value = value;
                throw new ArgumentNullException(nameof(element));
            }

            public CustomHashTableElements FindByKey(TKey key)
            {
                CustomHashTableElements element = this;
                while (!IsKeysEqual(element.Key, key))
                {
                    if (element.Next == null) break;
                    element = element.Next;
                }
                if (!IsKeysEqual(element.Key, key))
                {
                    return null;
                }
                return element;
            }

            private CustomHashTableElements GetLastElement()
            {
                var lastElement = this;
                while (lastElement.Next != null)
                {
                    lastElement = lastElement.Next;
                }
                return lastElement;
            }

            private bool IsKeysEqual(TKey leftKey, TKey rightKey)
            {
                return EqualityComparer<TKey>.Default.Equals(leftKey, rightKey);
            }
        }

        private CustomHashTableElements[] _hashTable;

        public int Count { get; private set; }

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new CustomHashTableElements[8];

        public void Clear()
        {
            _hashTable = new CustomHashTableElements[8];
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
                var value = _hashTable[position].GetValue(key);
                return value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var position = GetPosition(key);
                _hashTable[position].SetValue(key, value);
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
            var value = _hashTable[position].GetValue(item.Key);
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
            var element = _hashTable[position].FindByKey(key);
            return element != null;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var position = GetPosition(key);
            if (_hashTable[position] != null)
            {
                var element = _hashTable[position].FindByKey(key);
                if (element != null)
                {
                    value = element.Value;
                    return true;
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
            foreach (var element in this)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(element.Key, element.Value);
                arrayIndex++;
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
                _hashTable[position] = new CustomHashTableElements(item.Key, item.Value);
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
            if (_hashTable[position] == null || !ContainsKey(key)) return false;
            if (_hashTable[position].Next == null)
            {
                _hashTable[position] = null;
                Count--;
                return true;
            }
            var isRemote = _hashTable[position].Remove(key);
            if (isRemote) Count--;
            return isRemote;
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

        private int GetPosition(TKey key)
        {
            var hash = 0;
            var prefix = 0;
            foreach (var symbol in key.ToString())
            {
                prefix++;
                hash += symbol * prefix;
            }
            var pos = Math.Abs(hash % Capacity);
            return pos;
        }

        private int GetPosition(TKey key, int capacity)
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

        private bool HasAvailableMemory(double percentageFilling = 0.7)
        {
            var hasAvailableMemory = (int) Capacity * percentageFilling > Count;
            return hasAvailableMemory;
        }

        private void IncreaseSize()
        {
            CustomHashTableElements[] newHashTable = new CustomHashTableElements[Capacity * 2];
            foreach (var element in this)
            {
               var position = GetPosition(element.Key, Capacity * 2);
               if (newHashTable[position] == null)
               {
                    newHashTable[position] = new CustomHashTableElements(element.Key, element.Value);
               }
               else
               {
                    newHashTable[position].Add(element.Key, element.Value);
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
