using System;
using System.Collections.Generic;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomVector<int> vector = new CustomVector<int>();
            vector.Add(1);
            vector.Add(3);
            vector.Add(4);
            vector.Add(5);
            vector.Add(6);
            var isElement = vector.Contains(3);
            var remove = vector.Remove(3);
            vector.Insert(1,2);
            vector.RemoveAt(2);
            var element = vector.IndexOf(0);
            foreach (var item in vector)
            {
                Console.WriteLine(item);
            }

            var arr = new int[10];
            vector.CopyTo(arr, 2);
            vector.Clear();
        }
    }
}
