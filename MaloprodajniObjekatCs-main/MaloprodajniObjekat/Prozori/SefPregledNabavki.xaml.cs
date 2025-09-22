using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace MaloprodajniObjekat.Prozori
{
    /// <summary>
    /// Interaction logic for SefPregledNabavki.xaml
    /// </summary>
    public partial class SefPregledNabavki : Page
    {
        int odabranArtikal = 0;
        int odabranaNabavka = 0;
        public SefPregledNabavki()
        {
            InitializeComponent();
            prikaziNabavke();
        }

        private void prikaziNabavke()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT nabavkaID as [ID Nabavke], artikalID as [ID Artikla], nabavljenaKolicina as [Nabavljena količina], nabavkaDatum as [Datum i vreme nabavke], nabavkaCena as [Cena nabavke], sefID as [ID Šefa nabavke] FROM [Nabavka] ORDER BY nabavkaDatum ASC";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Nabavka");
            dataAdapter.Fill(dataTable);
            nabavkeGrid.ItemsSource = dataTable.DefaultView;
        }

        private void nabavkeGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                odabranArtikal = (int)dr["ID Artikla"];
                odabranaNabavka = (int)dr["ID Nabavke"];
                nabIDTxt.Text = dr["ID Nabavke"].ToString();
            }
        }

        private void obrisiNabavkuBtn(object sender, RoutedEventArgs e)
        {
            if (odabranArtikal == 0)
            {
                MessageBox.Show("Prvo izaberite nabavku iz tabele.");
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("ID Nabavke: " + odabranaNabavka + "\nNaziv artikla: " + getNazivArtikla(odabranArtikal) + "\nNabavljena količina: " + getNabavljenaKolicina(odabranaNabavka) + "\nCena nabavke: " + getCena(odabranaNabavka) + "\n\nDa li ste sigurni da želite da obrišete nabavku?\nKoličina ovog artikla će biti vraćena za vrednost nabavljene količine.", "Potvrdi brisanje nabavke", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    int nabKolicina = getNabavljenaKolicina(odabranaNabavka);
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "DELETE FROM Nabavka WHERE nabavkaID = @ID";
                    command.Parameters.AddWithValue("@ID", odabranaNabavka);
                    command.Connection = connection;
                    int provera = command.ExecuteNonQuery();
                    if (provera == 1)
                    {
                        MessageBox.Show("Nabavka obrisana. Količina artikla je ažurirana.");
                        umanjiKolicinu(odabranArtikal, nabKolicina);
                        prikaziNabavke();
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
        }

        private string getNazivArtikla(int artid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT artikalNaziv FROM Artikal WHERE artikalID = {artid}";
            command.Connection = connection;
            string naziv = (command.ExecuteScalar()).ToString();
            return naziv;
        }

        private void umanjiKolicinu(int artid, int kol)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"UPDATE Artikal SET artikalKolicina = @kolicina WHERE artikalID = {artid}";
            command.Parameters.AddWithValue("@kolicina", (getArtKolicina(artid) - kol));

            command.Connection = connection;
            command.ExecuteNonQuery();
            return;
        }

        private int getArtKolicina(int artid)
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

        private int getNabavljenaKolicina(int nabid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT nabavljenaKolicina FROM Nabavka WHERE nabavkaID = {nabid}";
            command.Connection = connection;
            int kolicina = (int)command.ExecuteScalar();
            return kolicina;
        }

        private int getCena(int nabid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT nabavkaCena FROM Nabavka WHERE nabavkaID = {nabid}";
            command.Connection = connection;
            int cena = Convert.ToInt32(command.ExecuteScalar());
            return cena;
        }
    }
}
