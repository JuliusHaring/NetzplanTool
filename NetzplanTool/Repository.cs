using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class Repository
    {
        private List<Vorgang> Vorgaenge { get; set; }

        public Repository()
        {
            Vorgaenge = new List<Vorgang>();
        }

        public List<Vorgang> GetAll()
        {
            return Vorgaenge;
        }

        public void Add(Vorgang v)
        {
            Vorgaenge.Add(v);
        }

        public Vorgang Get(int Id)
        {
            foreach(var v in Vorgaenge)
            {
                if (v.Id.Equals(Id))
                {
                    return v;
                }
            }
            throw new IndexOutOfRangeException("Für die angegebene Id ist kein Vorgang hinterlegt");
        }

        public List<Vorgang> GetStarts()
        {
            return Vorgaenge.FindAll(x => x.isStart);
        }

        public List<Vorgang> GetEnds()
        {
            return Vorgaenge.FindAll(x => x.isEnd);
        }
    }
}
