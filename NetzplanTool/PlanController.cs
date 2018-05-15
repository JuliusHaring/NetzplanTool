﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class PlanController
    {
        private List<List<Vorgang>> Pfade { get; set; }
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
        private string Ueberschrift { get; set; }
        private string Gesamtdauer { get {
                if(Repo.GetStarts().Count > 1 || Repo.GetEnds().Count > 1)
                {
                    return "nicht eindeutig";
                }
                int c = 0;
                foreach (var v in Repo.GetAll())
                {
                    c += v.Dauer;
                }
                return ""+c;
            } }
        private Repository Repo { get; set; }

        public PlanController(string Ueberschrift, List<Vorgang> Vorgaenge)
        {
            Repo = new Repository();
            Pfade = new List<List<Vorgang>>();

            ProcessInputVorgaenge(Vorgaenge);

            this.Ueberschrift = Ueberschrift;

            KalkVorwaerts();
            KalkRueckwaerts();
            KalkPuffer();
            KalkPfad();
        }

        private void ProcessInputVorgaenge(List<Vorgang> Vorgaenge)
        {
            foreach (var v in Vorgaenge)
            {
                Repo.Add(v);
            }
        }

        private void KalkVorwaerts()
        {
            foreach(var s in Repo.GetStarts())
            {
                KalkVorwaerts(s);
            }
        }

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

        private void KalkRueckwaerts()
        {
            foreach (var e in Repo.GetEnds())
            {
                KalkRueckwaerts(e);
            }
        }

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

        private void KalkPfad()
        {
            foreach(var s in Repo.GetStarts())
            {
                KalkPfad(new List<Vorgang> { s });
            }
        }

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
                else
                {
                    throw new ArgumentException("Die Angegebenen Vorgänge enthalten einen Zyklus!");
                }
            }
        }

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
    }
}
