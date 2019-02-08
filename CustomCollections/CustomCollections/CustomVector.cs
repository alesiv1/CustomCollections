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
        public int Length { get; private set; } = 1;
        public bool IsReadOnly { get; } = false;

        public CustomVector() => _vector = new T[1];

        public T this[int index]
        {
            get => _vector[index];
            set => _vector[index] = value;
        }

        public void Add(T item)
        {
            if(!IsEmptyPosition()) GrowSize();
            _vector[Count] = item;
            Count++;
        }

        public void Clear()
        {
            _vector = new T[1];
            Count = 0;
            Length = 1;
        }

        public bool Contains(T item)
        {
            if (item.Equals(null))
            {
                throw new ArgumentNullException();
            }

            for (int i = 0; i < Count; i++)
            {
                if (_vector[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }
            if (item.Equals(null))
            {
                throw new ArgumentNullException();
            }
             
            var index = -1;
            for (int i = 0; i < Count; i++)
            {
                if (_vector[i].Equals(item))
                {
                    index = i;
                    break;
                }
            }
            if (!index.Equals(-1))
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
                throw new ArgumentOutOfRangeException();
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
                throw new ArgumentOutOfRangeException();
            }

            if (!IsEmptyPosition()) GrowSize();
            var j = 0;
            for (int i = Count; i > index; i--)
            {
                _vector[i] = _vector[i - 1];
            }

            _vector[index] = item;
            Count++;
        }

        public int IndexOf(T item)
        {
            if (item.Equals(null))
            {
                throw new ArgumentNullException();
            }

            for (int i = 0; i < Count; i++)
            {
                if (_vector[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if(array == null)
            {
                throw new ArgumentNullException();
            }
            if(arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex] = _vector[i];
                arrayIndex++;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _vector)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool IsEmptyPosition()
        {
            return Count < Length;
        }

        private void GrowSize()
        {
            var newVector = new T[Length * 2];
            for (int i = 0; i < Length; i++)
            {
                newVector[i] = _vector[i];
            }
            Length *= 2;
            _vector = newVector;
        }
    }
}
