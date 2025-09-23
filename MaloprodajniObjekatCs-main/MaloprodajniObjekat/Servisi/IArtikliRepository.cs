using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaloprodajniObjekat.Servisi
{
    public interface IArtikliRepository
    {
        bool create(string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina);
        bool update(int artikalID, string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina);
        bool delete(int artikalID);
        DataTable getDataTable();
        string getLastId();
        
    }
}
