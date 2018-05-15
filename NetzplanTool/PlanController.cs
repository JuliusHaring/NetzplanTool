using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    /// <summary>
    /// Controller welcher die Geschäftslogik enthält.
    /// </summary>
    class PlanController
    {
        /// <summary>
        /// Überschrift des Projektes.
        /// </summary>
        private string Ueberschrift { get; set; }
        /// <summary>
        /// Repository welches alle Vorgänge speichert.
        /// </summary>
        private Repository Repo { get; set; }

        /// <summary>
        /// Alle möglichen Kombinationen von Pfaden welche der Netzplan beinhaltet.
        /// </summary>
        private List<List<Vorgang>> Pfade { get; set; }
        /// <summary>
        /// Durchsucht Pfade nach kritischen Pfaden und gibt diese in Listenform zurück.
        /// </summary>
        private List<List<Vorgang>> KritischePfade {
            get{
                List<List<Vorgang>> ret = new List<List<Vorgang>>();
                foreach(var list in Pfade)
                {
                    Boolean ToReturn = true;
                    if (!list.First().isStart|| !list.Last().isEnd)
                    {
                        ToReturn = false;
                    }
                    foreach(var el in list)
                    {
                        if (!el.isKritisch)
                        {
                            ToReturn = false;
                        }
                    }
                    if (ToReturn)
                    {
                        ret.Add(list);
                    }
                }
                return ret;
            }
        }
        
        /// <summary>
        /// Gibt die Gesamtdauer des Projektes als string zurück. Gibt es mehr als einen Anfang oder Ende ist diese nicht eindeutig.
        /// </summary>
        private string Gesamtdauer { get {
                if(Repo.GetStarts().Count > 1 || Repo.GetEnds().Count > 1)
                {
                    return "nicht eindeutig";
                }
                return ""+Repo.GetEnds().First().SEZ;
            }
        }

        /// <summary>
        /// Erstellt nach Instanziierung das Repository und die Liste der Pfadkombinationen. 
        /// Speichert mittels Hilfsmethoden alle Vorgänge im Repository und führt den Rest der Berechnungen für die Netzplanerstellung durch.
        /// </summary>
        /// <param name="Ueberschrift">Überschrift des Projektes.</param>
        /// <param name="Vorgaenge">Zu speichernde Vorgänge in Listenform.</param>
        /// <param name="filepath">Eingabepfad, genutzt für die Ausgabe der Ergebnisse.</param>
        public PlanController(string Ueberschrift, List<Vorgang> Vorgaenge, string filepath)
        {
            Repo = new Repository();
            Pfade = new List<List<Vorgang>>();

            ProcessInputVorgaenge(Vorgaenge);

            this.Ueberschrift = Ueberschrift;

            if (!HasZyklus())
            {
                
                KalkVorwaerts();
                KalkRueckwaerts();
                KalkPuffer();
                KalkPfad();
                WriteFile(filepath);
            }
            else
            {
                WriteFileZyklus(filepath);
            }
        }

        /// <summary>
        /// Startet die rekursive Überprüfung auf Zyklen innerhalb des Netzplanes. für jeden Vorgang des Repositores.
        /// </summary>
        /// <returns>true falls ein Zyklus vorliegt, false falls nicht.</returns>
        private Boolean HasZyklus()
        {
            foreach (var vorgang in Repo.GetAll())
            {
                if(HasZyklus(vorgang,new List<int> { vorgang.Id }))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Rekursive Überprüfung auf Zyklen indem der überprüft wird ob der Index eines beliebigen Nachfolgers bereits besucht wurde.
        /// Andernfalls wird der Nachfolger einem Klon der Liste hinzugefügt und mit diesem weitergerechnet.
        /// </summary>
        /// <param name="v">Aktuell zu überprüfendes Vorgang-Objekt.</param>
        /// <param name="list">Liste der bereits überprüften Vorgänge.</param>
        /// <returns>true falls ein Zyklus vorliegt, false falls nicht.</returns>
        private Boolean HasZyklus(Vorgang v, List<int> list)
        {
            foreach (var n in v.Nachfolger(Repo))
            {
                if(list.IndexOf(n.Id) > -1)
                {
                    return true;
                }
                var temp = new List<int>(list);
                temp.Add(n.Id);
                return HasZyklus(n, temp);
            }
            return false;
        }

        /// <summary>
        /// Fügt die Liste der Vorgänge dem Repository hinzu.
        /// </summary>
        /// <param name="Vorgaenge">Liste der hinzuzufügenden Objekte.</param>
        private void ProcessInputVorgaenge(List<Vorgang> Vorgaenge)
        {
            foreach (var v in Vorgaenge)
            {
                Repo.Add(v);
            }
        }

        /// <summary>
        /// Startet die rekursive Vorwärtsrechnung zur Bestimmung von frühesten Start- und Endzeiten bei allen Startobjekten.
        /// </summary>
        private void KalkVorwaerts()
        {
            foreach(var s in Repo.GetStarts())
            {
                KalkVorwaerts(s);
            }
        }

        /// <summary>
        /// Weist dem Vorgang FAZ = 0 zu falls dieser ein Startvorgang ist. Andernfalls wird die größte früheste Endzeit aller Vorgänger für diesen Wert verwendet.
        /// Setzt die früheste Endzeit aus der Dauer und der frühesten Anfangszeit zusammen.
        /// </summary>
        /// <param name="v">Zu verarbeitender Vorgang.</param>
        private void KalkVorwaerts(Vorgang v)
        {
            if (v.isStart)
            {
                v.FAZ = 0;
            }
            else
            {
                v.FAZ = v.Vorgaenger(Repo).OrderByDescending(o => o.FEZ).ToList().First().FEZ;
            }
            v.FEZ = v.FAZ + v.Dauer;
            foreach(var n in v.Nachfolger(Repo))
            {
                KalkVorwaerts(n);
            }
        }

        /// <summary>
        /// Startet die rekursive Rückwärtsrechnung zur Bestimmung von frühesten Start- und Endzeiten bei allen Endobjekten.
        /// </summary>
        private void KalkRueckwaerts()
        {
            foreach (var e in Repo.GetEnds())
            {
                KalkRueckwaerts(e);
            }
        }

        /// <summary>
        /// Weist dem Vorgang SEZ = FEZ zu falls dieser ein Endvorgang ist. Andernfalls wird die kleinste späteste Anfangszeit aller Nachfolger für diesen Wert verwendet.
        /// Setzt die früheste Endzeit aus der Dauer und der frühesten Anfangszeit zusammen.
        /// </summary>
        /// <param name="v">Zu verarbeitender Vorgang.</param>
        private void KalkRueckwaerts(Vorgang v)
        {
            if (v.isEnd)
            {
                v.SEZ = v.FEZ;
            }
            else
            {
                v.SEZ = v.Nachfolger(Repo).OrderBy(o => o.SAZ).ToList().First().SAZ;
            }
            v.SAZ = v.SEZ - v.Dauer;
            foreach (var vo in v.Vorgaenger(Repo))
            {
                KalkRueckwaerts(vo);
            }
        }

        /// <summary>
        /// Weist jedem Vorgang den Gesamtpuffer, also die differenz aus spätester und frühester Endzeit zu.
        /// Wiest außerdem dem freien Puffer die niedrigste früheste Anfangszeit aller Nachfolger abzüglich der eigenen frühesten Anfangszeit zu.
        /// </summary>
        private void KalkPuffer()
        {
            foreach(var v in Repo.GetAll())
            {
                v.GP = v.SEZ - v.FEZ;

                if(v.Nachfolger(Repo).Count > 0)
                {
                    v.FP = v.Nachfolger(Repo).OrderBy(o => o.FAZ).ToList().First().FAZ - v.FEZ;
                }
                else
                {
                    v.FP = 0;
                }                
            }
        }

        /// <summary>
        /// Startet die rekursive Erstellung aller Kombinationen von Pfaden des Netzplanes bei allen Startpunkten.
        /// </summary>
        private void KalkPfad()
        {
            foreach(var s in Repo.GetStarts())
            {
                KalkPfad(new List<Vorgang> { s });
            }
        }

        /// <summary>
        /// Rekursive Erstellung aller möglichen Pfadkombinaionen.
        /// </summary>
        /// <param name="p"></param>
        private void KalkPfad(List<Vorgang> p)
        {
            Pfade.Add(p);
            foreach(var n in p.Last().Nachfolger(Repo))
            {
                if (!ListContainsVorgang(p, n))
                {
                    List<Vorgang> temp = new List<Vorgang>(p);
                    temp.Add(n);
                    KalkPfad(temp);
                }
            }
        }

        /// <summary>
        /// Gibt an ob ein Vorgang bereits in einer Liste vorhanden ist.
        /// </summary>
        /// <param name="l">Zu überprüfende Liste.</param>
        /// <param name="v">Zu überprüfender Vorgang.</param>
        /// <returns>true falls der Vorgang bereits in der Liste enthalten ist, false falls nicht.</returns>
        private Boolean ListContainsVorgang(List<Vorgang> l, Vorgang v)
        {
            foreach(var e in l)
            {
                if(e.Id == v.Id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Erstellt die Ausgabedatei basierend auf den zuvor errechneten Werten.
        /// </summary>
        /// <param name="filepath">Pfad der Eingabedatei.</param>
        public void WriteFile(string filepath)
        {
            List<String> linesList = new List<String>
            {
                Ueberschrift,"","Vorgangsnummer; Vorgangsbezeichnung; D; FAZ; FEZ; SAZ; SEZ; GP; FP"
            };

            foreach (var v in Repo.GetAll())
            {
                var vString = v.Id + "; " +v.Bezeichnung + "; " +v.Dauer + "; " +v.FAZ + "; " +v.FEZ + "; " +v.SAZ + "; " +v.SEZ + "; " +v.GP + "; " +v.FP;
                linesList.Add(vString);
            }

            linesList.Add("");
            
            string anfaengeTemp = "";
            var anfaenge = Repo.GetStarts().ToArray();
            for(int i = 0; i < anfaenge.Length;i++)
            {
                anfaengeTemp += anfaenge[i].Id;
                if(i < anfaenge.Length - 1)
                {
                    anfaengeTemp += ", ";
                }
            }
            linesList.Add("Anfangsvorgang: "+anfaengeTemp);

            string endenTemp = "";
            var enden = Repo.GetEnds().ToArray();
            for (int i = 0; i < enden.Length; i++)
            {
                endenTemp += enden[i].Id;
                if (i < enden.Length - 1)
                {
                    endenTemp += ", ";
                }
            }
            linesList.Add("Endvorgang: " + endenTemp);


            linesList.Add("Gesamtdauer: " + Gesamtdauer);
            linesList.Add("");
            linesList.Add("Kritische(r) Pfad(e)");
            
            foreach(var p in KritischePfade)
            {
                string pfad = "";
                var arr = p.ToArray();
                for(int i = 0; i < arr.Length; i++)
                {
                    pfad += arr[i].Id;
                    if(i < arr.Length - 1)
                    {
                        pfad += "->";
                    }
                }
                linesList.Add(pfad);
            }

            string filename = Path.GetDirectoryName(filepath) + '\\' + Path.GetFileNameWithoutExtension(filepath);

            File.WriteAllLines(@filename+"_solved.txt", linesList.ToArray());
        }

        /// <summary>
        /// Schreibt im Falle eines Zyklus die Fehlerdatei welche dann ausgegeben wird.
        /// </summary>
        /// <param name="filepath"></param>
        private void WriteFileZyklus(string filepath)
        {
            List<String> linesList = new List<String>
            {
                Ueberschrift,"","Zyklus -> Berechnung nicht möglich!"
            };

            string filename = Path.GetDirectoryName(filepath) + '\\' + Path.GetFileNameWithoutExtension(filepath);

            File.WriteAllLines(@filename + "_error.txt", linesList.ToArray());
        }
    }
}
