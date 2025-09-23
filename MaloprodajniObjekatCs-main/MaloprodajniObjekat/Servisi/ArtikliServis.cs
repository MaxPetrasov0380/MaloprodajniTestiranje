using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaloprodajniObjekat.Servisi
{
    public class ArtikliServis
    {
        private readonly IArtikliRepository _repo;

        public ArtikliServis(IArtikliRepository repo) 
        {
            _repo = repo;
        }

        public bool create(string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina)
            => _repo.create(artikalNaziv, artikalVrsta, artikalCena, artikalKolicina);

        public bool update(int artikalID, string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina)
            => _repo.update(artikalID, artikalNaziv, artikalVrsta, artikalCena, artikalKolicina);

        public bool delete(int artikalID)
            => _repo.delete(artikalID);

        public DataTable getDataTable()
            => _repo.getDataTable();

        public string getLastId() => _repo.getLastId();
    }
}
