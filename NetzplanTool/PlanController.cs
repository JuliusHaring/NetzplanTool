using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    class PlanController
    {
        private List<List<Vorgang>> Pfade { get; set; }
        private string Ueberschrift { get; set; }
        private int Gesamtdauer { get {
                int i = 0;
                return i;
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
            CheckZusammenhang();

            TESTDELETE();
        }

        private void TESTDELETE()
        {
            foreach(var v in Repo.GetAll())
            {
                Console.WriteLine(v.Id);
                Console.WriteLine(v.Bezeichnung);
                Console.WriteLine(v.FAZ);
                Console.WriteLine(v.FEZ);
                Console.WriteLine(v.SAZ);
                Console.WriteLine(v.SEZ);
                Console.WriteLine("___________");
            }
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
                var vorgaenger = v.Vorgaenger(Repo).OrderByDescending(o => o.FEZ).ToList();
                var candidate = vorgaenger.First();
            }
            v.FEZ = v.FAZ + v.Dauer;
            foreach(var n in v.Nachfolger(Repo))
            {
                KalkVorwaerts(n);
            }
        }

        private void KalkRueckwaerts()
        {

        }

        private void KalkRueckwaerts(Vorgang v)
        {

        }

        private void KalkPuffer()
        {

        }

        private void KalkPfad()
        {

        }

        private void CheckZusammenhang()
        {

        }

        public void WriteFile(String filepath)
        {

        }
    }
}
