using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class NetplanTool
    {
        static void Main(string[] args)
        {
            foreach(var arg in args)
            {
                ReadFile(arg);
            }
            Console.ReadLine();
        }

        private static void ReadFile(string filepath)
        {
            using(var sr = new StreamReader(filepath, Encoding.Default))
            {
                string[] lines = sr.ReadToEnd().Split('\n');
                string ueberschrift = FindHeading(lines);
                Console.WriteLine(ueberschrift);
            }
        }

        private static string FindHeading(string[] lines)
        {
            string ret = "";
            foreach(var line in lines)
            {
               if(line.IndexOf("//+") == 0)
               {
                    ret += line.Remove(0,3);
               }
            }
            return ret;
        }
    }
}
