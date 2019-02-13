using System;
using System.Collections.Generic;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomHashTable<string, int> tb = new CustomHashTable<string, int>();
            tb.Add("1", 1);
            tb.Add("2", 2);
            tb.Add("3", 3);
            tb.Add("2", 4);
            tb.Add("5", 5);
            tb.Add("6", 6);
            tb.Add("7", 7);
            tb.Add("8", 8);
            tb.Add("9", 9);
            var a = tb.ContainsKey("1");
            var b = tb.ContainsKey("1");
            var c = tb.Contains(new KeyValuePair<string, int>("1", 1));

        }
    }
}
