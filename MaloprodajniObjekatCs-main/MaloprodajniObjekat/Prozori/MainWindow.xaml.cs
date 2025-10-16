using System;
using System.Collections.Generic;
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
using MaloprodajniObjekat.Prozori;

namespace MaloprodajniObjekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuthenticationService authService;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Korisnik\\Documents\\GitHub\\MaloprodajniTestiranje\\MaloprodajniObjekatCs-main\\MaloprodajniObjekat\\Database\\MaloprodajniObjekat.mdf;Integrated Security=True";
            authService = new AuthenticationService(connectionString);
        }

        private void adminLoginWindow(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Password;

            if (authService.ValidateUser(username, password, "Admin"))
            {
                AdminMenu dashboard = new AdminMenu();
                dashboard.Show();
                this.Close();
                dashboard.loggedIn.Content = username;
            }
            else
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.");
            }

        }

        private void kasirLoginWindow(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Password;

            if (authService.ValidateUser(username, password, "Kasir"))
            {
                KasirMenu dashboard = new KasirMenu(username);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.");
            }
        }

        private void sefNabavkeLoginWindow(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Password;

            if (authService.ValidateUser(username, password, "SefNabavke"))
            {
                SefNabavkeMenu dashboard = new SefNabavkeMenu(username);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.");
            }
        }
    }

    public class AuthenticationService
    {
        private string connectionString;

        public AuthenticationService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ValidateUser(string username, string password, string userType)
        {
            string usernameTable, passwordTable;
            switch(userType)
            {
                case "Admin":
                    usernameTable = "adminUsername";
                    passwordTable = "adminPass";
                    break;
                case "Kasir":
                    usernameTable = "kasirUsername";
                    passwordTable = "kasirPassword";
                    break;
                case "SefNabavke":
                    usernameTable = "sefUsername";
                    passwordTable = "sefPassword";
                    break;
                default:
                    System.Console.WriteLine("Error. Non-existing user type.");
                    return false;

            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(1) FROM {userType} WHERE {usernameTable} = @Username AND {passwordTable} = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
