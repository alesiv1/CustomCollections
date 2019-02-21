using System;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            var hashTable = new CustomHashTable<int, int>();

            for (var index = 1; index <= 100; index++)
                hashTable.Add(index, index);

            var a = hashTable.Remove(34);
            var b = hashTable.Remove(1111);
        }
    }
}
