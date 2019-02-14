using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            var ht = new CustomHashTable<int, string>();
            var ht2 = new CustomHashTable<string, int>();
            for (int i = 0; i < 1000; i++)
            {
                if(i == 456) continue;
                ht.Add(i, i + "");
                ht2.Add(i + "", i);
            }
            ht.Add(456, "456");
            ht2.Add("456", 456);
            var t1 = ht.Remove(456);
            var t2 = ht.Remove(new KeyValuePair<int, string>(456, "6"));
            var t3 = ht.Remove(new KeyValuePair<int, string>(456, "456"));

            var t4 = ht2.Remove("5");
            var t5 = ht2.Remove(new KeyValuePair<string, int>("6", 5));
            var t6 = ht2.Remove(new KeyValuePair<string, int>("6", 6));
        }
    }
}
