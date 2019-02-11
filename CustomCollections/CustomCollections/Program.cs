using System;
using System.Collections.Generic;

namespace CustomCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            var vector = new CustomVector<int>();
            vector.Add(1);
            vector.Add(2);
             var a = vector.IndexOf(1);
             var b = vector.IndexOf(11);

             var vector2 = new CustomVector<string>();
             vector2.Add("line1"); vector2.Add(null);
             vector2.Add("line2");          
             Console.WriteLine(vector2.Contains("line2"));
             Console.WriteLine(vector2.Contains(null));
        }
    }
}
