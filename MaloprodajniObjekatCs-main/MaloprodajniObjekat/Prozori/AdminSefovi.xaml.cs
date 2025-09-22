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
    /// Interaction logic for AdminSefovi.xaml
    /// </summary>
    public partial class AdminSefovi : Page
    {
        public AdminSefovi()
        {
            InitializeComponent();
            prikaziSefove();
        }

        private void prikaziSefove()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT sefID as [ID Šefa Nabavke], sefUsername as [Korisničko ime], sefPassword as [Šifra], sefEmail as [E-mail adresa], sefBT as [Broj telefona], sefAdresa as [Adresa stanovanja] FROM [SefNabavke] ";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("SefNabavke");
            dataAdapter.Fill(dataTable);
            sefoviDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            sefIDTxt.Text = "";
            sefUsernameTxt.Text = "";
            sefSifraTxt.Text = "";
            sefEmailTxt.Text = "";
            sefAdresaTxt.Text = "";
            sefBTTxt.Text = "";
        }

        private void dodajSefaBtn(object sender, RoutedEventArgs e)
        {
            if (sefUsernameTxt.Text == "" || sefSifraTxt.Text == "" || sefEmailTxt.Text == "" || sefBTTxt.Text == "" || sefAdresaTxt.Text == "")
            {
                MessageBox.Show("Prvo popunite sva polja.");
                return;
            }
            else
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString =
                ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "INSERT INTO SefNabavke VALUES(@username, @password, @email, @bt, @adresa)";
                command.Parameters.AddWithValue("@username", sefUsernameTxt.Text);
                command.Parameters.AddWithValue("@password", sefSifraTxt.Text);
                command.Parameters.AddWithValue("@email", sefEmailTxt.Text);
                command.Parameters.AddWithValue("@bt", sefBTTxt.Text);
                command.Parameters.AddWithValue("@adresa", sefAdresaTxt.Text);

                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Dodavanje uspešno.");
                    prikaziSefove();
                }
                ocistiPolja();
            }
        }

        private void izmeniSefaBtn(object sender, RoutedEventArgs e)
        {
            if (sefIDTxt.Text == "")
            {
                MessageBox.Show("Prvo izaberite šefa nabavke iz tabele.");
                return;
            }
            else
            {
                if (sefUsernameTxt.Text == "" || sefSifraTxt.Text == "" || sefEmailTxt.Text == "" || sefBTTxt.Text == "" || sefAdresaTxt.Text == "")
                {
                    MessageBox.Show("Prvo popunite sva polja.");
                    return;
                }
                else
                {
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = "UPDATE SefNabavke SET sefUsername = @username, sefPassword = @password, sefEmail = @email, sefBT = @bt, sefAdresa = @adresa WHERE sefID = @ID";
                    command.Parameters.AddWithValue("@ID", sefIDTxt.Text);
                    command.Parameters.AddWithValue("@username", sefUsernameTxt.Text);
                    command.Parameters.AddWithValue("@password", sefSifraTxt.Text);
                    command.Parameters.AddWithValue("@email", sefEmailTxt.Text);
                    command.Parameters.AddWithValue("@bt", sefBTTxt.Text);
                    command.Parameters.AddWithValue("@adresa", sefAdresaTxt.Text);

                    command.Connection = connection;
                    int provera = command.ExecuteNonQuery();
                    if (provera == 1)
                    {
                        MessageBox.Show("Izmena uspešna.");
                        prikaziSefove();
                    }
                    ocistiPolja();
                }
            }
            
        }

        private void obrisiSefaBtn(object sender, RoutedEventArgs e)
        {
            if (proveriNabavke(int.Parse(sefIDTxt.Text)))
            {
                MessageBox.Show("Brisanje šefa nabavke nije moguće jer postoje nabavke koje je taj šef nabavke izdao.");
                return;
            }
            else
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString =
                ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "DELETE FROM SefNabavke WHERE sefID = @ID";
                command.Parameters.AddWithValue("@ID", sefIDTxt.Text);
                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Brisanje uspešno.");
                    prikaziSefove();
                }
                ocistiPolja();
            }
        }

        private bool proveriNabavke(int sefid)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT nabavkaID FROM Nabavka WHERE sefID = {sefid}";
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

        private void sefoviDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                sefIDTxt.Text = dr["ID Šefa Nabavke"].ToString();
                sefUsernameTxt.Text = dr["Korisničko ime"].ToString();
                sefSifraTxt.Text = dr["Šifra"].ToString();
                sefEmailTxt.Text = dr["E-mail adresa"].ToString();
                sefBTTxt.Text = dr["Broj telefona"].ToString();
                sefAdresaTxt.Text = dr["Adresa stanovanja"].ToString();
            }
        }

        private bool isNumeric(string str)
        {
            bool result = int.TryParse(str, out int i);
            return result;
        }

        private void sefBTTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
