using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CustomCollections
{
    class CustomHashTable<TKey, TValue> : IDictionary
    {
        private static Tuple<TKey, TValue>[] _hashTable;

        public CustomHashTable()
        {
            _hashTable = new Tuple<TKey, TValue>[1];
            _hashTable[0] = new Tuple<TKey, TValue>(default(TKey), default(TValue));
        }

        public int Count { get; private set; } = 0;
        public int Length { get; private set; } = 1;
        public bool IsSynchronized { get; } = false;
        public object SyncRoot { get; }
        public bool IsFixedSize { get; } = false;
        public bool IsReadOnly { get; private set; } = true;
        public ICollection Keys { get; }
        public ICollection Values { get; } 

        public void Clear()
        {
            _hashTable = new Tuple<TKey, TValue>[1];
            Count = 0;
            Length = 1;
            IsReadOnly = true;
        }

        public bool Contains(object value)
        {
            return _hashTable
                .Select(item2 => item2.Item2)
                .Contains((TValue) value);
        }

        public object this[object key]
        {
            get
            {
                var position = GetPosition((TKey)key, Length);
                if (position < Length)
                {
                    return _hashTable[position];
                }
                throw new ArgumentNullException(nameof(key));
            }

            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var position = GetPosition((TKey)key, Length);
                if (position < Length)
                {
                    _hashTable[position] = new Tuple<TKey, TValue>((TKey)key,default(TValue));
                }
                else
                {
                    throw new Exception("Error set value");
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            array.CopyTo(_hashTable.Select(value => value.Item2).ToArray(), index);
        }

        public void Add(object key, object value)
        {
            var position = GetPosition((TKey)key, _hashTable.Length);
            if (_hashTable[position] == null)
            {
                _hashTable[position] = new Tuple<TKey, TValue>(default(TKey), default(TValue));
            }
            if (_hashTable[position].Item1.Equals(key))
            {
                throw new Exception("This key already exist!");
            }
            Count++;
            if (Count >= Length)
            {
                GrowAndReHash();
            }
            position = GetPosition((TKey)key, _hashTable.Length);
            if (_hashTable[position] == null)
            {
                _hashTable[position] = new Tuple<TKey, TValue>(default(TKey), default(TValue));
            }
            _hashTable[position] = new Tuple<TKey, TValue>((TKey)key, (TValue)value);
            IsReadOnly = false;
            SetAllDefaultValue();
        }

        public void Remove(object key)
        {
            _hashTable = _hashTable.Where(s => !s.Item1.Equals(key)).ToArray();
        }

        public TValue Get(object key)
        {
            var position = GetPosition((TKey)key, _hashTable.Length);
            if (position < Length && !_hashTable[position].Equals(new Tuple<TKey, TValue>(default(TKey), default(TValue))))
            {
                return _hashTable[position].Item2;
            }
            throw new Exception("This key isn't exist!");
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _hashTable
                .Where(key => !key.Item1.Equals(default(TKey)))
                .ToDictionary(item => item.Item1, item => item.Item2)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetPosition(TKey key, int length)
        {
            var hash = key.GetHashCode();
            var pos = Math.Abs(hash % length);
            return pos;
        }

        private void GrowAndReHash()
        {
            Length *= 2;
            var newItems = new Tuple<TKey, TValue>[_hashTable.Length * 2];
            foreach (var item in _hashTable)
            {
                var pos = GetPosition(item.Item1, newItems.Length);
                if (newItems[pos] == null)
                {
                    newItems[pos] = new Tuple<TKey, TValue>(default(TKey), default(TValue));
                }
                newItems[pos] = new Tuple<TKey, TValue>(item.Item1, item.Item2);
            }
            _hashTable = newItems;
        }

        private void SetAllDefaultValue()
        {
            for (int i = 0; i < Length; i++)
            {
                if (_hashTable[i] == null)
                {
                    _hashTable[i] = new Tuple<TKey, TValue>(default(TKey), default(TValue));
                }
            }
        }
    }
}
