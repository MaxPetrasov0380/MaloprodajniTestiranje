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
using System.IO;

namespace MaloprodajniObjekat.Prozori
{
    /// <summary>
    /// Interaction logic for KasirRacuni.xaml
    /// </summary>
    public partial class KasirRacuni : Page
    {
        public KasirRacuni()
        {
            InitializeComponent();
            prikaziRacune();
        }

        private void prikaziRacune()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString =
            ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT racunID as [ID Računa], ukupnaCena as [Ukupna cena], datumVreme as [Datum i vreme izdavanja], kasirID as [ID Kasira] FROM [Racun] ORDER BY datumVreme ASC ";
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Racun");
            dataAdapter.Fill(dataTable);
            racDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void racDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                racunIDTxt.Text = dr["ID Računa"].ToString();
            }
        }

        private void prikaziRacunBtn(object sender, RoutedEventArgs e)
        {
            prikaziRacun(int.Parse(racunIDTxt.Text));
        }

        private void prikaziRacun(int racunID)
        {
            try
            {
                string path = "C:\\Users\\Korisnik\\Documents\\GitHub\\MaloprodajniTestiranje\\MaloprodajniObjekatCs-main\\MaloprodajniObjekat\\Racuni\\";
                racunPrikaz.Text = File.ReadAllText(path + racunID + ".txt");
            }
            catch(FileNotFoundException){racunPrikaz.Text = "Datoteka nije pronađena.";}
            racunIDTxt.Text = "";
            prikaziRacune();
            
        }

        private void filtrirajPretragu(object sender, TextChangedEventArgs e)
        {
            if (racunIDTxt.Text != "")
            {
                if (int.Parse(racunIDTxt.Text) < 0)
                {
                    racunIDTxt.Text = "";
                }
                else
                {
                    int racID = int.Parse(racunIDTxt.Text);
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["connSupermarket"].ConnectionString;
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"SELECT racunID as [ID Računa], ukupnaCena as [Ukupna cena], datumVreme as [Datum i vreme izdavanja], kasirID as [ID Kasira] FROM [Racun] WHERE racunID LIKE '{racID}%' ORDER BY datumVreme ASC";
                    command.Connection = connection;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable("Racun");
                    dataAdapter.Fill(dataTable);
                    racDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            else
            {
                prikaziRacune();
            }
            return;
        }
    }
}
