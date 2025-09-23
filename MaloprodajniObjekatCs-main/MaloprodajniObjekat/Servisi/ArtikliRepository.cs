using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaloprodajniObjekat.Servisi
{
    public class ArtikliRepository : IArtikliRepository
    {
        private readonly string _connString;

        public ArtikliRepository(string connString)
        {
            _connString = connString;
        }

        public bool create(string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = @"INSERT INTO [Artikal](artikalNaziv, artikalVrsta, artikalCena, artikalKolicina) 
                             VALUES(@naziv, @vrsta, @cena, @kolicina)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naziv", artikalNaziv);
                cmd.Parameters.AddWithValue("@vrsta", artikalVrsta);
                cmd.Parameters.AddWithValue("@cena", artikalCena);
                cmd.Parameters.AddWithValue("@kolicina", artikalKolicina);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool delete(int artikalID)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [Artikal] WHERE artikalID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", artikalID);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool update(int artikalID, string artikalNaziv, string artikalVrsta, int artikalCena, int artikalKolicina)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(@"UPDATE [Artikal] SET 
                artikalNaziv = @naziv, artikalVrsta = @vrsta, artikalCena = @cena, artikalKolicina = @kolicina WHERE artikalID = @ID", conn);
                cmd.Parameters.AddWithValue("@naziv", artikalNaziv);
                cmd.Parameters.AddWithValue("@vrsta", artikalVrsta);
                cmd.Parameters.AddWithValue("@cena", artikalCena);
                cmd.Parameters.AddWithValue("@kolicina", artikalKolicina);
                cmd.Parameters.AddWithValue("@ID", artikalID);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public DataTable getDataTable()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Artikal]", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Artikal");
                da.Fill(dt);
                return dt;
            }
        }

        public string getLastId()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                // Pretpostavka: IdOpreme je numeričko polje / inkrementalno
                cmd.CommandText = "SELECT TOP 1 artikalID FROM [Artikal] ORDER BY artikalID DESC";
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
}
