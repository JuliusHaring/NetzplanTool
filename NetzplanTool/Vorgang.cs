using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class Vorgang
    {
        public int Id { get; }
        public int FAZ { get; set; }
        public int FEZ { get; set; }
        public int SEZ { get; set; }
        public int Dauer { get; set; }
        public int GP { get; set; }
        public int FP { get; set; }
        public string Bezeichnung { get; set; }

        public Boolean isStart { get {
                if(LVorgaenger.Count == 0)
                {
                    return true;
                }
                return false;
            } }
        public Boolean isEnd { get {
                if (LNachfolger.Count == 0)
                {
                    return true;
                }
                return false;
            } }
        public  Boolean isKritisch {
            get {
                if (GP == 0 && FP == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private List<int> LNachfolger { get; set; }
        private List<int> LVorgaenger { get; set; }


        public Vorgang(int Id, string Bezeichnung, int Dauer, int[] Vorgaenger, int[] Nachfolger)
        {
            LNachfolger = new List<int>();
            LVorgaenger = new List<int>();

            this.Id = Id;
            this.Bezeichnung = Bezeichnung;

            foreach(var v in Vorgaenger)
            {
                LVorgaenger.Add(v);
            }

            foreach (var n in Nachfolger)
            {
                LNachfolger.Add(n);
            }
        }

        public List<Vorgang> Nachfolger(Repository Repo)
        {
            List<Vorgang> ret = new List<Vorgang>();
            foreach (var i in LNachfolger)
            {
                ret.Add(Repo.Get(i));
            }
            return ret;
        }

        public List<Vorgang> Vorgaenger(Repository Repo)
        {
            List<Vorgang> ret = new List<Vorgang>();
            foreach (var i in LVorgaenger)
            {
                ret.Add(Repo.Get(i));
            }
            return ret;
        }
    }
}
