using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class NetzplanTool
    {
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                ReadFile(arg);
            }
        }

        private static void ReadFile(string filepath)
        {
            using (var sr = new StreamReader(filepath, Encoding.Default))
            {
                string[] lines = sr.ReadToEnd().Split('\n');
                string ueberschrift = FindHeading(lines);
                List<Vorgang> vorgaenge = ListVorgaenge(lines);

                PlanController PController = new PlanController(ueberschrift,vorgaenge);
                PController.WriteFile(filepath);
            }
        }

        private static string FindHeading(string[] lines)
        {
            string ret = "";
            foreach (var line in lines)
            {
                if (line.IndexOf("//+") == 0)
                {
                    ret += line.Remove(0, 3);
                }
            }
            return ret;
        }

        private static List<Vorgang> ListVorgaenge(string[] lines)
        {
            List<Vorgang> ret = new List<Vorgang>();

            foreach (var line in lines)
            {
                if (line.IndexOf("//") != 0)
                {
                    Vorgang v = CreateVorgang(line);
                    ret.Add(v);
                }
            }

            return ret;
        }

        private static Vorgang CreateVorgang(string line)
        {
            string[] attributes = line.Split(';');

            if(attributes.Length != 5)
            {
                throw new ArgumentException("Die Anzahl der Attribute stimmt nicht in der Zeile: " + line);
            }

            int Id = int.Parse(attributes[0]);
            string Bezeichnung = attributes[1];
            int Dauer = int.Parse(attributes[2]);
            int[] Vorgaenger;
            int[] Nachfolger;

            if(attributes[3].IndexOf('-') > -1)
            {
                Vorgaenger = new int[] { };
            }
            else
            {
                Vorgaenger = Array.ConvertAll(attributes[3].Split(','), s => int.Parse(s));
            }

            if (attributes[4].IndexOf('-') > -1)
            {
                Nachfolger = new int[] { };
            }
            else
            {
                Nachfolger = Array.ConvertAll(attributes[4].Split(','), s => int.Parse(s));
            }

            return new Vorgang(Id,Bezeichnung,Dauer,Vorgaenger,Nachfolger);
        }
    }
}
