using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    /// <summary>
    /// Hauptklasse des Programms.
    /// </summary>
    class NetzplanTool
    {
        /// <summary>
        /// Die Main-Methode wird als Haupt-Thread gestartet und ruft für jeden Dateipfad eine Hilfs-Methode.
        /// </summary>
        /// <param name="args">Dateipfade, getrennt durch Leerzeichen.</param>
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                ReadFile(arg);
            }
        }

        /// <summary>
        /// Analysiert die Eingabedatei  durch Hilfsmethoden und eröffnet den ausführenden PlanController.
        /// </summary>
        /// <param name="filepath">Pfad einer einzulesenden Datei.</param>
        private static void ReadFile(string filepath)
        {
            using (var sr = new StreamReader(filepath, Encoding.Default))
            {
                try { 
                string[] lines = sr.ReadToEnd().Split('\n');
                string ueberschrift = FindHeading(lines);
                List<Vorgang> vorgaenge = ListVorgaenge(lines,filepath);

                PlanController PController = new PlanController(ueberschrift,vorgaenge,filepath);
                }catch(Exception e)
                {
                    WriteInputException(filepath, e);
                }

            }
        }

        /// <summary>
        /// Durchsucht den Kommentarbereich der Eingabedatei nachd er Überschrift.
        /// </summary>
        /// <param name="lines">Inhalt der Eingangsdatei nach Zeilen aufgespaltet.</param>
        /// <returns>Überschrift des Netzplanes.</returns>
        private static string FindHeading(string[] lines)
        {
            string ret = "";
            foreach (var line in lines)
            {
                if (!ret.Equals(""))
                {
                    ret += "\n";
                }
                if (line.IndexOf("//+") == 0)
                {
                    ret += line.Remove(0, 3);
                }
            }
            return ret;
        }

        /// <summary>
        /// Durchsucht die Datei nach dem Teil nach dem Kommentarbereich.
        /// </summary>
        /// <param name="lines">Inhalt der Eingangsdatei nach Zeilen aufgespaltet.</param>
        /// <param name="filepath">Pfad der einzulesenden Datei.</param>
        /// <returns>Liste aller eingelesenen Vorgänge</returns>
        private static List<Vorgang> ListVorgaenge(string[] lines,string filepath)
        {
            List<Vorgang> ret = new List<Vorgang>();

            foreach (var line in lines)
            {
                if (line.IndexOf("//") != 0 && !line.Equals(""))
                {
                    Vorgang v = CreateVorgang(line);
                    
                    
                    ret.Add(v);
                }
            }

            return ret;
        }

        /// <summary>
        /// Erstellt einen Vorgang.
        /// </summary>
        /// <param name="line">Eingelesene Zeile</param>
        /// <returns>Vorgang welcher aus der Zeile erstellt wurde.</returns>
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

        /// <summary>
        /// Schreibt bei Fehlerhafter Eingabedatei den Fehler in eine Datei.
        /// </summary>
        /// <param name="filepath">Pfad der Eingabedatei.</param>
        /// <param name="e">Ausnahme welche ausgegeben werden soll.</param>
        private static void WriteInputException(string filepath, Exception e)
        {
            List<String> linesList = new List<String>
            {
                "Exception:",e.Message
            };

            string filename = Path.GetDirectoryName(filepath) + '\\' + Path.GetFileNameWithoutExtension(filepath);

            File.WriteAllLines(filename + "_error.txt", linesList.ToArray());
        }
    }
}
