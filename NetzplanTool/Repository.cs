using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetzplanTool
{
    /// <summary>
    /// Das Repository speichert alle Vorgänge und ermöglicht spezifische Aufrufvarianten um gefiltert auf mehrere Vorgänge zuzugreifen-
    /// </summary>
    class Repository
    {
        /// <summary>
        /// Speichert alle Vorgänge des Programms.
        /// </summary>
        private List<Vorgang> Vorgaenge { get; set; }

        /// <summary>
        /// Erstellt bei Instanziierung die Liste zum Speichern der Objekte.
        /// </summary>
        public Repository()
        {
            Vorgaenge = new List<Vorgang>();
        }

        /// <summary>
        /// Aufrufbar um alle Objekte des Repositories zu erhalten.
        /// </summary>
        /// <returns>Alle Vorgänge in Listenform.</returns>
        public List<Vorgang> GetAll()
        {
            return Vorgaenge;
        }

        /// <summary>
        /// Fügt einen einzelnen Vorgang dem Speicher hinzu.
        /// </summary>
        /// <param name="v">Hinzuzufügender Vorgang.</param>
        public void Add(Vorgang v)
        {
            Vorgaenge.Add(v);
        }

        /// <summary>
        /// Gibt einen einzelnen Vorgang zurück.
        /// </summary>
        /// <param name="Id">Id des zurückzugebenden Vorgangs.</param>
        /// <returns>Vorgang zu dem die Id passt.</returns>
        public Vorgang Get(int Id)
        {
            foreach(var v in Vorgaenge)
            {
                if (v.Id.Equals(Id))
                {
                    return v;
                }
            }
            return null;
        }

        /// <summary>
        /// Gibt alle Vorgänge ohne Vorgänger zurück.
        /// </summary>
        /// <returns>Liste aller Startvorgänge.</returns>
        public List<Vorgang> GetStarts()
        {
            return Vorgaenge.FindAll(x => x.isStart);
        }

        /// <summary>
        /// Gibt alle Vorgänge ohne Nachfolger zurück.
        /// </summary>
        /// <returns>Liste aller Endvorgänge.</returns>
        public List<Vorgang> GetEnds()
        {
            return Vorgaenge.FindAll(x => x.isEnd);
        }

        /// <summary>
        /// Überprüft ob ein Index im Repository vorhanden ist.
        /// </summary>
        /// <param name="id">Zu überprüfender Index.</param>
        /// <returns>true wenn der Vorgang existiert, sonst false.</returns>
        public Boolean IndexExistiert(int Id)
        {
            foreach(var v in Vorgaenge)
            {
                if(v.Id == Id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
