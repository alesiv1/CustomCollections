using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCollections
{
    class CustomVector<T> : IList<T>
    {
        private T[] _vector;

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; } = true;

        public CustomVector()
        {
            _vector = new T[0];
            Count = _vector.Length;
        }

        public T this[int index]
        {
            get => _vector[index];
            set => _vector[index] = value;
        }

        public void Add(T item)
        {
            var newVector = new T[Count + 1];
            for (int i = 0; i < Count; i++)
            {
                newVector[i] = _vector[i];
            }
            newVector[Count] = item;
            Count++;
            _vector = newVector;
            IsReadOnly = false;
        }

        public void Clear()
        {
            _vector = new T[0];
            Count = 0;
            IsReadOnly = true;
        }

        public bool Contains(T item)
        {
            if (_vector.Contains(item))
            {
                return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            _vector = _vector.Where(s => !s.Equals(item)).ToArray();
            if (Count > _vector.Length)
            {
                Count = _vector.Length;
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            Remove(_vector[index]);
        }

        public void Insert(int index, T item)
        {
            Count++;
            var newVector = new T[Count];
            var j = 0;
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                {
                    newVector[i] = item;
                }
                else
                {
                    newVector[i] = _vector[j];
                    j++;
                }
            }
            _vector = newVector;
        }

        public int IndexOf(T item)
        {
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
            if (array.Length - arrayIndex < Count)
            {
                Console.WriteLine("The array is small!");
                return;
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
    }
}
