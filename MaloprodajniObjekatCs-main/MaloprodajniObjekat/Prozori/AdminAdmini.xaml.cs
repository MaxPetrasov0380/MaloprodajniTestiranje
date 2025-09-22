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
    /// Interaction logic for AdminAdmini.xaml
    /// </summary>
    public partial class AdminAdmini : Page
    {
        public AdminAdmini()
        {
            InitializeComponent();
            prikaziAdmine();
        }

        private void prikaziAdmine()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT adminID as [ID Admina], adminUsername as [Korisničko ime], adminPass as [Šifra], adminEmail as [E-mail adresa], adminBT as [Broj telefona], adminAdresa as [Adresa stanovanja] FROM [Admin] ";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Admin");
            dataAdapter.Fill(dataTable);
            admDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            admIDTxt.Text = "";
            admUsernameTxt.Text = "";
            admSifraTxt.Text = "";
            admEmailTxt.Text = "";
            admAdresaTxt.Text = "";
            admBTTxt.Text = "";
        }

        private void dodajAdminaBtn(object sender, RoutedEventArgs e)
        {
            if (admUsernameTxt.Text == "" || admSifraTxt.Text == "" || admEmailTxt.Text == "" || admBTTxt.Text == "" || admAdresaTxt.Text == "")
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
                command.CommandText = "INSERT INTO Admin VALUES(@username, @password, @email, @bt, @adresa)";
                command.Parameters.AddWithValue("@username", admUsernameTxt.Text);
                command.Parameters.AddWithValue("@password", admSifraTxt.Text);
                command.Parameters.AddWithValue("@email", admEmailTxt.Text);
                command.Parameters.AddWithValue("@bt", admBTTxt.Text);
                command.Parameters.AddWithValue("@adresa", admAdresaTxt.Text);

                command.Connection = connection;
                int provera = command.ExecuteNonQuery();
                if (provera == 1)
                {
                    MessageBox.Show("Dodavanje uspešno.");
                    prikaziAdmine();
                }
                ocistiPolja();
            }
            
        }

        private void izmeniAdminaBtn(object sender, RoutedEventArgs e)
        {
            if (admIDTxt.Text == "")
            {
                MessageBox.Show("Prvo izaberite admina iz tabele.");
                return;
            }
            else
            {
                if (admUsernameTxt.Text == "" || admSifraTxt.Text == "" || admEmailTxt.Text == "" || admBTTxt.Text == "" || admAdresaTxt.Text == "")
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
                    command.CommandText = "UPDATE Admin SET adminUsername = @username, adminPass = @password, adminEmail = @email, adminBT = @bt, adminAdresa = @adresa WHERE adminID = @ID";
                    command.Parameters.AddWithValue("@ID", admIDTxt.Text);
                    command.Parameters.AddWithValue("@username", admUsernameTxt.Text);
                    command.Parameters.AddWithValue("@password", admSifraTxt.Text);
                    command.Parameters.AddWithValue("@email", admEmailTxt.Text);
                    command.Parameters.AddWithValue("@bt", admBTTxt.Text);
                    command.Parameters.AddWithValue("@adresa", admAdresaTxt.Text);

                    command.Connection = connection;
                    int provera = command.ExecuteNonQuery();
                    if (provera == 1)
                    {
                        MessageBox.Show("Izmena uspešna.");
                        prikaziAdmine();
                    }
                    ocistiPolja();
                }
            }
            
            
        }

        private void obrisiAdminaBtn(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "DELETE FROM Admin WHERE adminID = @ID";
            command.Parameters.AddWithValue("@ID", admIDTxt.Text);
            command.Connection = connection;
            int provera = command.ExecuteNonQuery();
            if (provera == 1)
            {
                MessageBox.Show("Podaci su uspešno obrisani");
                prikaziAdmine();
            }
            ocistiPolja();
        }

        private void admDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                admIDTxt.Text = dr["ID Admina"].ToString();
                admUsernameTxt.Text = dr["Korisničko ime"].ToString();
                admSifraTxt.Text = dr["Šifra"].ToString();
                admEmailTxt.Text = dr["E-mail adresa"].ToString();
                admBTTxt.Text = dr["Broj telefona"].ToString();
                admAdresaTxt.Text = dr["Adresa stanovanja"].ToString();
            }
        }

        private bool isNumeric(string str)
        {
            bool result = int.TryParse(str, out int i);
            return result;
        }

        private void admBTTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumeric(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
