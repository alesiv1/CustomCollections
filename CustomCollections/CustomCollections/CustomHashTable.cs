using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    public class DataOfHashTable<TKey, TValue>
    {
        public class KeyValueRepository<TK, TV>
        {
            public TK Key { get; set; }
            public TV Value { get; set; }
            public KeyValueRepository<TK, TV> Next { get; set; }

            public KeyValueRepository(TK key, TV value)
            {
                this.Key = key;
                this.Value = value;
            }

            public void Add(TK key, TV value)
            { 
                var lastElement = GetLastElement();
                lastElement.Next = new KeyValueRepository<TK, TV>(key, value);
            }

            public bool Remove(TK key)
            {
                var element = this;
                KeyValueRepository<TK, TV> parent = null;
                while (!IsKeysEqual(element.Key, key))
                {
                    if(element.Next == null) break;
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
                }
                return true;
            }

            public TV GetValue(TK key)
            {
                var element = FindByKey(key);
                return element.Value;
            }

            public void SetValue(TK key, TV value)
            {
                var element = FindByKey(key);
                if (element != null) element.Value = value;
            }

            public KeyValueRepository<TK, TV> FindByKey(TK key)
            {
                KeyValueRepository<TK, TV> element = this;
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

            private KeyValueRepository<TK, TV> GetLastElement()
            {
                var lastElement = this;
                while (lastElement.Next != null)
                {
                    lastElement = lastElement.Next;
                }
                return lastElement;
            }

            private bool IsKeysEqual(TK leftKey, TK rightKey)
            {
                return EqualityComparer<TK>.Default.Equals(leftKey, rightKey);
            }
        }

        public List<TKey> EquivalentKeys { get; private set; }
        private KeyValueRepository<TKey, TValue> RepositoryKeyAndValue { get; set; }

        public DataOfHashTable(TKey key, TValue value)
        {
            this.RepositoryKeyAndValue = new KeyValueRepository<TKey, TValue>(key, value);
            this.EquivalentKeys = new List<TKey>{ key };
        }

        public void Add(TKey key, TValue value)
        {
            if (RepositoryKeyAndValue != null)
            {
                RepositoryKeyAndValue.Add(key, value);
                EquivalentKeys.Add(key);
            }
            else
            {
                RepositoryKeyAndValue = new KeyValueRepository<TKey, TValue>(key, value);
                EquivalentKeys = new List<TKey>() { key };
            }
        }

        public KeyValueRepository<TKey, TValue> GetRepository()
        {
            return RepositoryKeyAndValue;
        }

        public bool Remove(TKey key)
        {
            var isRemoved = RepositoryKeyAndValue.Remove(key);
            if (isRemoved)
            {
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
        private DataOfHashTable<TKey, TValue>[] _hashTable;

        public int Count { get; private set; }

        public int Capacity => _hashTable.Length;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => GetKeys();
        public ICollection<TValue> Values => GetValues();

        public CustomHashTable() => _hashTable = new DataOfHashTable<TKey, TValue>[8];

        public void Clear()
        {
            _hashTable = new DataOfHashTable<TKey, TValue>[8];
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
                    throw new ArgumentNullException(nameof(key));
                }
                return _hashTable[position].GetRepository().GetValue(key);
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var position = GetPosition(key);
                if (!_hashTable[position].ContainsKey(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                _hashTable[position].GetRepository().SetValue(key, value);
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
            var value = _hashTable[position].GetRepository().GetValue(item.Key);
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
            if (_hashTable[position] != null)
            {
                var element = _hashTable[position].GetRepository().FindByKey(key);
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
            //for (int i = 0; i < Capacity; i++)
            //{
            //    if (_hashTable[i] != null)
            //    {
            //        var repository = _hashTable[i].GetRepository();
            //        while (repository != null)
            //        {
            //            array[arrayIndex] = new KeyValuePair<TKey, TValue>(repository.Key, repository.Value);
            //            arrayIndex++;
            //            repository = repository.Next;
            //        }
            //    }
            //}
            foreach (var data in GetAllData())
            {
                var repository = data.GetRepository();
                while (repository != null)
                {
                    array[arrayIndex] = new KeyValuePair<TKey, TValue>(repository.Key, repository.Value);
                    arrayIndex++;
                    repository = repository.Next;
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
                _hashTable[position] = new DataOfHashTable<TKey, TValue>(item.Key, item.Value);
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
            if (_hashTable[position] == null || !ContainsKey(key)) return false;
            if (_hashTable[position].EquivalentKeys.Count == 1)
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
            //foreach (var element in _hashTable)
            //{
            //    if (element != null)
            //    {
            //        var repository = element.GetRepository();
            //        while (repository != null)
            //        {
            //            yield return new KeyValuePair<TKey, TValue>(repository.Key, repository.Value);
            //            repository = repository.Next;
            //        }
            //    }
            //}
            foreach (var data in GetAllData())
            {
                var repository = data.GetRepository();
                while (repository != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(repository.Key, repository.Value);
                    repository = repository.Next;
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
            List<TKey> keys = new List<TKey>();
            //foreach (var item in _hashTable)
            //{
            //    if (item != null)
            //    {
            //        foreach (var node in item.EquivalentKeys)
            //        {
            //            keys.Add(node);
            //        }
            //    }
            //}
            foreach (var data in GetAllData())
            {
               foreach (var key in data.EquivalentKeys)
               {
                        keys.Add(key);
               }
            }
            return keys;
        }

        private ICollection<TValue> GetValues()
        {
            List<TValue> values = new List<TValue>();
            //foreach (var item in _hashTable)
            //{
            //    if (item != null)
            //    {
            //        var repository = item.GetRepository();
            //        while(repository != null)
            //        {
            //            values.Add(repository.Value);
            //            repository = repository.Next;
            //        }
            //    }
            //}
            foreach (var data in GetAllData())
            {
                var repository = data.GetRepository();
                while (repository != null)
                {
                    values.Add(repository.Value);
                    repository = repository.Next;
                }
            }
            return values;
        }

        private bool HasAvailableMemory()
        {
            var hasAvailableMemory = (int)Capacity * 0.7 > Count;
            return hasAvailableMemory;
        }

        private void IncreaseSize()
        {
            DataOfHashTable<TKey, TValue>[] newHashTable = new DataOfHashTable<TKey, TValue>[Capacity * 2];
            //for (int i = 0; i < Capacity; i++)
            //{
            //    if (_hashTable[i] != null)
            //    {
            //        var repository = _hashTable[i].GetRepository();
            //        while(repository != null)
            //        {
            //            var position = GetPosition(repository.Key, Capacity * 2);
            //            if (newHashTable[position] == null)
            //            {
            //                newHashTable[position] = new DataOfHashTable<TKey, TValue>(repository.Key, repository.Value);
            //            }
            //            else
            //            {
            //                newHashTable[position].Add(repository.Key, repository.Value);
            //            }
            //            repository = repository.Next;
            //        }
            //    }
            //}
            foreach (var data in GetAllData())
            {
                var repository = data.GetRepository();
                while (repository != null)
                {
                    var position = GetPosition(repository.Key, Capacity * 2);
                    if (newHashTable[position] == null)
                    {
                        newHashTable[position] = new DataOfHashTable<TKey, TValue>(repository.Key, repository.Value);
                    }
                    else
                    {
                        newHashTable[position].Add(repository.Key, repository.Value);
                    }
                    repository = repository.Next;
                }
            }
            _hashTable = newHashTable;
        }

        private List<DataOfHashTable<TKey, TValue>> GetAllData()
        {
            var allData = new List<DataOfHashTable<TKey, TValue>>();
            for (int i = 0; i < Capacity; i++)
            {
                if (_hashTable[i] != null)
                {
                    allData.Add(_hashTable[i]);
                }
            }
            return allData;
        }

        private bool IsValuesEqual(TValue leftItem, TValue rightItem)
        {
            return EqualityComparer<TValue>.Default.Equals(leftItem, rightItem);
        }
    }
}
