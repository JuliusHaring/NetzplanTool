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

            r.Add(v);

            var o = r.GetStarts();
            var i = o.Last().Nachfolger(r);

            Console.ReadLine();
        }
    }
}
