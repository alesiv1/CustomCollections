using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCollections
{
    class CustomVector<T> : IList<T>
    {
        private T[] _vector;

        public int Count { get; private set; } = 0;
        public int Capacity => _vector.Length;
        public bool IsReadOnly => false;

        public CustomVector() => _vector = new T[1];

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return  _vector[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                _vector[index] = value;
            }
        }

        public void Add(T item)
        {
            if (!HasAvailableMemory()) IncreaseSize();
            _vector[Count] = item;
            Count++;
        }

        public void Clear()
        {
            _vector = new T[1];
            Count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index > -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            for (int i = index; i < Count - 1; i++)
            {
                _vector[i] = _vector[i + 1];
            }
            Count--;
            _vector[Count] = default(T);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (!HasAvailableMemory()) IncreaseSize();
            for (int i = Count; i > index; i--)
            {
                _vector[i] = _vector[i - 1];
            }

            _vector[index] = item;
            Count++;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_vector[i], item))
                { 
                    return i;
                }
            }
            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("arrayIndex");
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex] = _vector[i];
                arrayIndex++;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _vector[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool HasAvailableMemory()
        {
            return Count < Capacity;
        }

        private void IncreaseSize()
        {
            var newVector = new T[Capacity * 2];
            for (int i = 0; i < Capacity; i++)
            {
                newVector[i] = _vector[i];
            }
            _vector = newVector;
        }
    }
}
