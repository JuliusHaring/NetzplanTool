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

            this.Ueberschrift = Ueberschrift;

            KalkVorwaerts();
            KalkRueckwaerts();
            KalkPuffer();
            KalkPfad();
            CheckZusammenhang();
        }

        private void KalkVorwaerts()
        {

        }

        private void KalkRueckwaerts()
        {

        }

        private void KalkVorwaerts(Vorgang v)
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
