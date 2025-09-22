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
    /// Interaction logic for AdminKasiri.xaml
    /// </summary>
    public partial class AdminKasiri : Page
    {
        public AdminKasiri()
        {
            InitializeComponent();
            prikaziKasire();
        }

        private void prikaziKasire()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT kasirID as [ID Kasira], kasirIme as [Ime], kasirPrezime as [Prezime], kasirUsername as [Korisničko ime], kasirPassword as [Šifra], kasirEmail as [E-mail adresa], kasirBT as [Broj telefona], kasirAdresa as [Adresa stanovanja] FROM [Kasir] ";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Kasir");
            dataAdapter.Fill(dataTable);
            kasDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            kasIDTxt.Text = "";
            kasImeTxt.Text = "";
            kasPrezimeTxt.Text = "";
            kasUsernameTxt.Text = "";
            kasSifraTxt.Text = "";
            kasEmailTxt.Text = "";
            kasBTTxt.Text = "";
            kasAdresaTxt.Text = "";
        }

        private void dodajKasiraBtn(object sender, RoutedEventArgs e)
        {
            if (kasImeTxt.Text == "" || kasPrezimeTxt.Text == "" || kasUsernameTxt.Text == "" || kasSifraTxt.Text == "" || kasBTTxt.Text == "" || kasAdresaTxt.Text == "")
            {
                MessageBox.Show("Prvo popunite sva obavezna polja.");
                return;
            }
            else
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString =
                ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "INSERT INTO Kasir VALUES(@ime, @prezime, @username, @password, @email, @bt, @adresa)";
                command.Parameters.AddWithValue("@ime", kasImeTxt.Text);
                command.Parameters.AddWithValue("@prezime", kasPrezimeTxt.Text);
                command.Parameters.AddWithValue("@username", kasUsernameTxt.Text);
                command.Parameters.AddWithValue("@password", kasSifraTxt.Text);
                command.Parameters.AddWithValue("@email", kasEmailTxt.Text);
                command.Parameters.AddWithValue("@bt", kasBTTxt.Text);
                command.Parameters.AddWithValue("@adresa", kasAdresaTxt.Text);

                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Dodavanje uspešno.");
                    prikaziKasire();
                }
                ocistiPolja();
            }
            
        }

        private void izmeniKasiraBtn(object sender, RoutedEventArgs e)
        {
            if (kasIDTxt.Text == "")
            {
                MessageBox.Show("Prvo izaberite kasira iz tabele.");
                return;
            }
            else
            {
                if (kasImeTxt.Text == "" || kasPrezimeTxt.Text == "" || kasUsernameTxt.Text == "" || kasSifraTxt.Text == "" || kasBTTxt.Text == "" || kasAdresaTxt.Text == "")
                {
                    MessageBox.Show("Prvo popunite sva obavezna polja.");
                    return;
                }
                else
                {
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = "UPDATE Kasir SET kasirIme = @ime, kasirPrezime = @prezime, kasirUsername = @username, kasirPassword = @password, kasirEmail = @email, kasirBT = @bt, kasirAdresa = @adresa WHERE kasirID = @ID";
                    command.Parameters.AddWithValue("@ID", kasIDTxt.Text);
                    command.Parameters.AddWithValue("@ime", kasImeTxt.Text);
                    command.Parameters.AddWithValue("@prezime", kasPrezimeTxt.Text);
                    command.Parameters.AddWithValue("@username", kasUsernameTxt.Text);
                    command.Parameters.AddWithValue("@password", kasSifraTxt.Text);
                    command.Parameters.AddWithValue("@email", kasEmailTxt.Text);
                    command.Parameters.AddWithValue("@bt", kasBTTxt.Text);
                    command.Parameters.AddWithValue("@adresa", kasAdresaTxt.Text);

                    command.Connection = connection;
                    int provera = command.ExecuteNonQuery();
                    if (provera == 1)
                    {
                        MessageBox.Show("Izmena uspešna.");
                        prikaziKasire();
                    }
                    ocistiPolja();
                }   
            }
        }

        private void obrisiKasiraBtn(object sender, RoutedEventArgs e)
        {
            if(proveriRacune(int.Parse(kasIDTxt.Text)))
            {
                MessageBox.Show("Brisanje kasira nije moguće zbog postojećih računa koje je izdao ovaj kasir.");
                return;
            }
            else
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString =
                ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "DELETE FROM Kasir WHERE kasirID = @ID";
                command.Parameters.AddWithValue("@ID", kasIDTxt.Text);
                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Brisanje uspešno.");
                    prikaziKasire();
                }
                ocistiPolja();
            }
        }

        private bool proveriRacune(int kasirID)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT racunID FROM Racun WHERE kasirID = {kasirID}";
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

        private void kasDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                kasIDTxt.Text = dr["ID Kasira"].ToString();
                kasImeTxt.Text = dr["Ime"].ToString();
                kasPrezimeTxt.Text = dr["Prezime"].ToString();
                kasUsernameTxt.Text = dr["Korisničko ime"].ToString();
                kasSifraTxt.Text = dr["Šifra"].ToString();
                kasEmailTxt.Text = dr["E-mail adresa"].ToString();
                kasBTTxt.Text = dr["Broj telefona"].ToString();
                kasAdresaTxt.Text = dr["Adresa stanovanja"].ToString();
            }
        }

        private bool isNumeric(string str)
        {
            bool result = int.TryParse(str, out int i);
            return result;
        }

        private void kasBTTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
