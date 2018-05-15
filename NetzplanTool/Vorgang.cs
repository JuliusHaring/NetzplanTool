using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    /// <summary>
    /// Beschreibt einen Vorgang welcher ein Element des Netzplans darstellt.
    /// </summary>
    class Vorgang
    {
        /// <summary>
        /// Identifikationsnummer des Vorgangs.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Früheste Anfangszeit eines Vorgansg.
        /// </summary>
        public int FAZ { get; set; }
        /// <summary>
        /// Früheste Endzeit eines Vorgangs.
        /// </summary>
        public int FEZ { get; set; }
        /// <summary>
        /// Späteste Anfangszeit eines Vorgangs.
        /// </summary>
        public int SAZ { get; set; }
        /// <summary>
        /// Späteste Endzeit eines Vorgangs.
        /// </summary>
        public int SEZ { get; set; }
        /// <summary>
        /// Dauer des Vorgangs.
        /// </summary>
        public int Dauer { get; set; }
        /// <summary>
        /// Gesamtpuffer des Vorgangs.
        /// </summary>
        public int GP { get; set; }
        /// <summary>
        /// Freier Puffer des Vorgangs.
        /// </summary>
        public int FP { get; set; }
        /// <summary>
        /// Bezeichnungs des Vorgangs.
        /// </summary>
        public string Bezeichnung { get; set; }

        /// <summary>
        /// Gibt zurück ob der Vorgang Vorgänger hat.
        /// </summary>
        public Boolean isStart { get {
                if(LVorgaenger.Count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gibt zurück ob der Vorgang Nachfolger hat.
        /// </summary>
        public Boolean isEnd { get {
                if (LNachfolger.Count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gibt zurück ob der Vorgang kritischer Natur ist.
        /// </summary>
        public  Boolean isKritisch {
            get {
                if (GP == 0 && FP == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Speichert die Id's der Vorgänger privat ab.
        /// </summary>
        private List<int> LVorgaenger { get; set; }

        /// <summary>
        /// Speichert die Id's der Nachfolger privat ab.
        /// </summary>
        private List<int> LNachfolger { get; set; }

        /// <summary>
        /// Erstellt nach Instanziierung die lokalen Listen für die Id's von Vorgängern und Nachfolgern
        /// und weist dem Vorgang-Objekt die Attribute zu.
        /// </summary>
        /// <param name="Id">Identifikationsnummer des Vorgangs.</param>
        /// <param name="Bezeichnung">Bezeichnung des Vorgangs.</param>
        /// <param name="Dauer">Dauer des Vorgangs.</param>
        /// <param name="Vorgaenger">Id's der Vorgänger des Vorgangs.</param>
        /// <param name="Nachfolger">Id's der Nachfolger des Vorgangs.</param>
        public Vorgang(int Id, string Bezeichnung, int Dauer, int[] Vorgaenger, int[] Nachfolger)
        {
            LNachfolger = new List<int>();
            LVorgaenger = new List<int>();

            this.Id = Id;
            this.Bezeichnung = Bezeichnung;
            this.Dauer = Dauer;

            foreach(var v in Vorgaenger)
            {
                LVorgaenger.Add(v);
            }

            foreach (var n in Nachfolger)
            {
                LNachfolger.Add(n);
            }
        }

        /// <summary>
        /// Gibt die Vorgänger in Form einer Liste zurück. Dafür lädt es die Vorgänge anhand ihrer Id aus dem Repository.
        /// </summary>
        /// <param name="Repo">Verweis auf das zu verwendende Repository.</param>
        /// <returns>Liste von Vorgängerobjekten.</returns>
        public List<Vorgang> Vorgaenger(Repository Repo)
        {
            List<Vorgang> ret = new List<Vorgang>();
            foreach (var i in LVorgaenger)
            {
                var add = Repo.Get(i);
                if(add != null)
                {
                    ret.Add(Repo.Get(i));
                }   
            }
            return ret;
        }

        /// <summary>
        /// Gibt die Nachfolger in Form einer Liste zurück. Dafür lädt es die Vorgänge anhand ihrer Id aus dem Repository.
        /// </summary>
        /// <param name="Repo">Verweis auf das zu verwendende Repository.</param>
        /// <returns>Liste von Nachfolgerobjekten.</returns>
        public List<Vorgang> Nachfolger(Repository Repo)
        {
            List<Vorgang> ret = new List<Vorgang>();
            foreach (var i in LNachfolger)
            {
                var add = Repo.Get(i);
                if (add != null)
                {
                    ret.Add(Repo.Get(i));
                }
            }
            return ret;
        }
    }
}
