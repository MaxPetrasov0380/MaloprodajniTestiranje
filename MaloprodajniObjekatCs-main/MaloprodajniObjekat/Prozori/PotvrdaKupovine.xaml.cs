using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MaloprodajniObjekat.Prozori
{
    /// <summary>
    /// Interaction logic for PotvrdaKupovine.xaml
    /// </summary>
    public partial class PotvrdaKupovine : Window
    {
        public int korpaUkupnaCena = 0;
        public string kasirUsername = "";
        private int indeksNiza = 0;
        private int[] artikliKorpa;
        private int[] kolicineArtikala;
        public PotvrdaKupovine(string username, int ukupnaCena, int[] artKorpa, int[] artKol)
        {
            InitializeComponent();
            kasirUsername = username;
            korpaUkupnaCena = ukupnaCena;
            ukupnaCenaLbl.Content = ukupnaCena.ToString();
            artikliKorpa = artKorpa;
            kolicineArtikala = artKol;
        }

        private int getKasirID(string username)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT kasirID FROM Kasir WHERE kasirUsername = '{username}'";
            command.Connection = connection;
            int id = (int)command.ExecuteScalar();
            return id;
        }

        private void noBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void yesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (potvrdaKupovine() == 1)
            {
                while (artikliKorpa[indeksNiza]!=0)
                {
                    int elemenat = artikliKorpa[indeksNiza];
                    int elemenatKolicina = kolicineArtikala[indeksNiza];
                    umanjiKolicinu(elemenat, elemenatKolicina);
                    indeksNiza++;
                }
            }
        }

        private int potvrdaKupovine()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "INSERT INTO Racun VALUES(@cena, getdate(), @kasirID)";
            command.Parameters.AddWithValue("@cena", korpaUkupnaCena);
            command.Parameters.AddWithValue("@kasirID", getKasirID(kasirUsername));
            command.Connection = connection;
            int provera = command.ExecuteNonQuery();
            if (provera == 1)
            {
                MessageBox.Show("Kupovina je potvrđena i račun evidentiran.");
            }
            upisiUDatoteku();
            this.Close();
            return provera;
        }

        private void upisiUDatoteku()
        {
            DateTime now = DateTime.Now;
            string content = "ID RAČUNA: " + getRacunID() + "\n" + "KASIR: " + kasirUsername + "\n" + "DATUM I VREME: " + now + "\n" + korpatxtfield.Text + "\n---------\n" + "UKUPNA CENA: " + korpaUkupnaCena + "RSD\n";
            string path = "C:\\Users\\Korisnik\\Documents\\GitHub\\MaloprodajniTestiranje\\MaloprodajniObjekatCs-main\\MaloprodajniObjekat\\Racuni\\";
            File.WriteAllText(path + getRacunID() + ".txt", content);
        }

        //SELECT TOP 1 racunID FROM Racun ORDER BY racunID DESC
        private int getRacunID()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT TOP 1 racunID FROM Racun ORDER BY racunID DESC";
            command.Connection = connection;
            int id = (int)command.ExecuteScalar();
            return id;
        }

        private void umanjiKolicinu(int artid, int kol)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"UPDATE Artikal SET artikalKolicina = @kolicina WHERE artikalID = {artid}";
            command.Parameters.AddWithValue("@kolicina", (getKolicina(artid)-kol));

            command.Connection = connection;
            command.ExecuteNonQuery();
            return;
        }

        private int getKolicina(int artid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT artikalKolicina FROM Artikal WHERE artikalID = {artid}";
            command.Connection = connection;
            int kolicina = (int)command.ExecuteScalar();
            return kolicina;
        }
    }
}
