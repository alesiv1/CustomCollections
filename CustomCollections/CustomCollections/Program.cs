using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            var hashTable = new CustomHashTable<int, int>();

            for (var index = 1; index <= 1000; index++)
                hashTable.Add(index, index);
            for (var index = 1; index <= 1000; index++)
                Console.WriteLine(hashTable[index]);
            Console.ReadKey();
        }
    }
}
