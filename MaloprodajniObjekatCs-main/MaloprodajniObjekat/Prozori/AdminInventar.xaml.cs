using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaloprodajniObjekat.Servisi;

namespace MaloprodajniObjekat.Prozori
{
    /// <summary>
    /// Interaction logic for AdminInventar.xaml
    /// </summary>
    public partial class AdminInventar : Page
    {
        private ArtikliServis _servis;
        public AdminInventar()
        {
            InitializeComponent();
            string connStr = ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            var repo = new ArtikliRepository(connStr);
            _servis = new ArtikliServis(repo);
            prikaziInventar();
        }

        private void prikaziInventar()
        {
            artDataGrid.ItemsSource = _servis.getDataTable().DefaultView;
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            artNazivTxt.Text = "";
            artVrstaTxt.Text = "";
            artKolicinaTxt.Text = "";
            artCenaTxt.Text = "";
            artIDTxt.Text = "";
        }

        private void dodajArtikalBtn(object sender, RoutedEventArgs e)
        {

            var success = _servis.create(artNazivTxt.Text, artVrstaTxt.Text, Convert.ToInt32(artCenaTxt.Text), Convert.ToInt32(artKolicinaTxt.Text));
            if (success)
            {
                MessageBox.Show("Uspešno dodato!");
                prikaziInventar();
                ocistiPolja();
            }
        }

        private void izmeniArtikalBtn(object sender, RoutedEventArgs e)
        {
            if(artIDTxt.Text=="")
            {
                MessageBox.Show("Prvo izaberite artikal iz tabele.");
                return;
            }
            else
            {
                if (artNazivTxt.Text == "" || (artVrstaDropdown.Text == "" && artVrstaTxt.Text == "") || artCenaTxt.Text == "" || artKolicinaTxt.Text == "")
                {
                    MessageBox.Show("Prvo popunite sva polja.");
                    return;
                }
                else
                {
                    if (int.Parse(artKolicinaTxt.Text) < 0)
                    {
                        MessageBox.Show("Količina ne sme biti manja od 0.");
                        return;
                    }
                    else
                    {
                        SqlConnection connection = new SqlConnection();
                        connection.ConnectionString =
                        ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                        connection.Open();

                        SqlCommand command = new SqlCommand();
                        command.CommandText = "UPDATE Artikal SET artikalNaziv = @naziv, artikalVrsta = @vrsta, artikalCena = @cena, artikalKolicina = @kolicina WHERE artikalID = @ID";
                        command.Parameters.AddWithValue("@ID", artIDTxt.Text);
                        command.Parameters.AddWithValue("@naziv", artNazivTxt.Text);
                        if (artVrstaTxt.Text != "")
                        {
                            command.Parameters.AddWithValue("@vrsta", artVrstaTxt.Text);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@vrsta", artVrstaDropdown.Text);
                        }
                        command.Parameters.AddWithValue("@cena", artCenaTxt.Text);
                        command.Parameters.AddWithValue("@kolicina", artKolicinaTxt.Text);

                        command.Connection = connection;
                        int provera = command.ExecuteNonQuery();
                        if (provera == 1)
                        {
                            MessageBox.Show("Izmena uspešna.");
                            prikaziInventar();
                        }
                        ocistiPolja();
                        loadDropdown();
                    }

                }
            }
        }

        private void obrisiArtikalBtn(object sender, RoutedEventArgs e)
        {
            if(proveriNabavke(int.Parse(artIDTxt.Text)))
            {
                MessageBox.Show("Brisanje artikla nije moguće zbog postojanja evidentiranih nabavki tog artikla.");
                return;
            }
            else
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString =
                ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "DELETE FROM Artikal WHERE artikalID = @ID";
                command.Parameters.AddWithValue("@ID", artIDTxt.Text);
                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Brisanje uspešno.");
                    prikaziInventar();
                }
                ocistiPolja();
                loadDropdown();
            }
        }

        private bool proveriNabavke(int artid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT nabavkaID FROM Nabavka WHERE artikalID = {artid}";
            command.Connection = connection;
            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void load(object sender, RoutedEventArgs e)
        {
            loadDropdown();
        }

        private void loadDropdown()
        {
            artVrstaDropdown.Items.Clear();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand commandCbx = new SqlCommand();
            commandCbx.CommandText = "SELECT DISTINCT artikalVrsta FROM Artikal";
            commandCbx.Connection = connection;
            SqlDataAdapter dataAdapterCbx = new SqlDataAdapter(commandCbx);
            DataTable dataTableCbx = new DataTable("Artikal");
            dataAdapterCbx.Fill(dataTableCbx);
            artVrstaDropdown.Items.Add("");
            for (int i = 0; i < dataTableCbx.Rows.Count; i++)
            {
                artVrstaDropdown.Items.Add(dataTableCbx.Rows[i]["artikalVrsta"]);
            }
        }

        private void artDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                artIDTxt.Text = dr["ID Artikla"].ToString();
                artNazivTxt.Text = dr["Naziv artikla"].ToString();
                artVrstaTxt.Text = dr["Vrsta artikla"].ToString();
                artCenaTxt.Text = dr["Cena"].ToString();
                artKolicinaTxt.Text = dr["Dostupna količina"].ToString();
            }
        }

        private bool isNumeric(string str)
        {
            bool result = int.TryParse(str, out int i);
            return result;
        }

        private void artCenaTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }

        private void artKolicinaTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
