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

namespace MaloprodajniObjekat.Prozori
{
    /// <summary>
    /// Interaction logic for SefInventar.xaml
    /// </summary>
    public partial class SefInventar : Page
    {
        int odabranArtikal = 0;
        public string sefUsername = "";
        public SefInventar(string username)
        {
            InitializeComponent();
            prikaziInventar();
            sefUsername = username;
        }

        private void prikaziInventar()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT artikalID as [ID Artikla], artikalNaziv as [Naziv artikla], artikalVrsta as [Vrsta artikla], artikalCena as [Cena], artikalKolicina as [Dostupna količina] FROM [Artikal] ";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Artikal");
            dataAdapter.Fill(dataTable);
            nabArtDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            nabArtCenaTxt.Content = "";
            nabArtIDTxt.Text = "";
            nabArtNazivTxt.Content = "";
            nabArtKolicinaTxt.Text = "";
            cenaStartTxt.Text = "";
            odabranArtikal = 0;
        }

        private void potvrdiNabavkuBtn(object sender, RoutedEventArgs e)
        {
            if(cenaStartTxt.Text == "")
            {
                MessageBox.Show("Prvo izaberite artikal iz tabele.");
                return;
            }
            else
            {
                if(nabArtKolicinaTxt.Text == "")
                {
                    MessageBox.Show("Pre potvrđivanja nabavke unesite količinu.");
                    return;
                }
                else
                {
                    if(int.Parse(nabArtKolicinaTxt.Text)==0)
                    {
                        MessageBox.Show("Nabavljena količina ne može biti nula.");
                        return;
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("ID Artikla: " + odabranArtikal + "\nNaziv artikla: " + getNazivArtikla(odabranArtikal) + "\nNabavljena količina: " + nabArtKolicinaTxt.Text + "\nCena nabavke: " + nabArtCenaTxt.Content + "\n\nDa li želite da potvrdite nabavku?", "Potvrdi nabavku", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            potvrdiNabavku();
                            uvecajKolicinu(odabranArtikal, int.Parse(nabArtKolicinaTxt.Text));
                            ocistiPolja();
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void potvrdiNabavku()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "INSERT INTO Nabavka VALUES(@artID, @kolicina, getdate(), @cena, @sefID)";
            command.Parameters.AddWithValue("@artID", nabArtIDTxt.Text);
            command.Parameters.AddWithValue("@kolicina", nabArtKolicinaTxt.Text);
            command.Parameters.AddWithValue("@cena", nabArtCenaTxt.Content);
            command.Parameters.AddWithValue("@sefID", getSefID(sefUsername));

            command.Connection = connection;
            int provera = command.ExecuteNonQuery();
            if (provera == 1)
            {
                MessageBox.Show("Nabavka potvrđena.");
                prikaziInventar();
            }
        }

        private int getSefID(string username)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT sefID FROM SefNabavke WHERE sefUsername = '{username}'";
            command.Connection = connection;
            int id = (int)command.ExecuteScalar();
            return id;
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

        private void uvecajKolicinu(int artid, int kol)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"UPDATE Artikal SET artikalKolicina = @kolicina WHERE artikalID = {artid}";
            command.Parameters.AddWithValue("@kolicina", (getKolicina(artid) + kol));

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

        private void nabArtDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                nabArtNazivTxt.Content = dr["Naziv artikla"].ToString();
                cenaStartTxt.Text = dr["Cena"].ToString();
                nabArtCenaTxt.Content = dr["Cena"].ToString();
                nabArtIDTxt.Text = dr["ID Artikla"].ToString();
                odabranArtikal = (int)dr["ID Artikla"];
            }
        }

        private void filtrirajPretragu(object sender, TextChangedEventArgs e)
        {
            if (nabArtIDTxt.Text != "")
            {
                if (int.Parse(nabArtIDTxt.Text) < 0)
                {
                    nabArtIDTxt.Text = "";
                }
                else
                {
                    int artID = int.Parse(nabArtIDTxt.Text);
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"SELECT artikalID as [ID Artikla], artikalNaziv as [Naziv artikla], artikalVrsta as [Vrsta artikla], artikalCena as [Cena], artikalKolicina as [Dostupna količina] FROM [Artikal] WHERE artikalID LIKE '{artID}%'";
                    command.Connection = connection;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable("Artikal");
                    dataAdapter.Fill(dataTable);
                    nabArtDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            else
            {
                prikaziInventar();
            }
            return;
        }

        private bool isNumeric(string str)
        {
            bool result = int.TryParse(str, out int i);
            return result;
        }

        private void artikalID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }

        private void kolicina_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }

        private void promenaKolicine(object sender, TextChangedEventArgs e)
        {
            if (cenaStartTxt == null)
            {
                return;
            }
            else
            {
                if (cenaStartTxt.Text != "")
                {
                    izracunajCenu();
                }
                else return;
            }
        }

        private void izracunajCenu()
        {
            if (nabArtKolicinaTxt.Text == "")
            {
                nabArtCenaTxt.Content = 0;
            }
            else
            {
                double cenaDouble = (double.Parse(cenaStartTxt.Text))*0.5;
                int cena = (int)Math.Round(cenaDouble);
                int kol = int.Parse(nabArtKolicinaTxt.Text);
                cena = kol * cena;
                nabArtCenaTxt.Content = cena.ToString();
            }
        }
    }
}
