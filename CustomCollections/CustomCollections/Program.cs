using System;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomHashTable<int, int> hash = new CustomHashTable<int, int>();
            hash.Add(1, 1);
            hash.Add(2, 2);
            hash.Add(3, 3);
            hash.Add(4, 4);
            hash.Add(6, 5);
            var a = hash.Contains(5);
            hash.Remove(2);
            foreach (var item in hash)
            {
                Console.WriteLine(item);
            }
        }
    }
}
