using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class NetplanTool
    {
        static void Main(string[] args)
        {
            Repository r = new Repository();

            Vorgang v = new Vorgang(1, "yo", new int[] { }, new int[] { 1});
            Vorgang w = new Vorgang(2, "yo", new int[] {2 }, new int[] { 1 });

            r.Add(v);
            r.Add(w);

            var o = r.GetStarts();
            Console.ReadLine();
        }
    }
}
