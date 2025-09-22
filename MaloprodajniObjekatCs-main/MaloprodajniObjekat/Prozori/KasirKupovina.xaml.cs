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
    /// Interaction logic for KasirKupovina.xaml
    /// </summary>
    public partial class KasirKupovina : Page
    {
        int korpaUkupnaCena = 0;
        public string kasirUsername = "";
        private int odabranArtikal = 0;
        private int indeksNiza = 0;
        private int[] artikliKorpa= new int[30];
        private int[] kolicineArtikala= new int[30];
        public KasirKupovina(string username)
        {
            InitializeComponent();
            prikaziInventar();
            ocistiPolja();
            ukupnaCenaLbl.Content = korpaUkupnaCena;
            kasirUsername = username;
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
            artikliPrikaz.ItemsSource = dataTable.DefaultView;
        }

        private void ocistiKorpuBtn(object sender, RoutedEventArgs e)
        {
            ocistiKorpu();
            prikaziInventar();
        }

        private void osveziListu(object sender, RoutedEventArgs e)
        {
            prikaziInventar();
        }

        private void ocistiKorpu()
        {
            korpatxtfield.Text = "";
            korpaUkupnaCena = 0;
            ukupnaCenaLbl.Content = korpaUkupnaCena;
            indeksNiza = 0;
            Array.Clear(artikliKorpa, 0, artikliKorpa.Length);
            Array.Clear(kolicineArtikala, 0, kolicineArtikala.Length);
        }

        private void ocistiPoljaBtn(object sender, RoutedEventArgs e)
        {
            ocistiPolja();
        }

        private void ocistiPolja()
        {
            barkod.Text = "";
            kolicina.Text = "1";
            cenaTxt.Text = "";
            nazivTxt.Content = "";
            cenaStartTxt.Text = "";
            odabranArtikal = 0;
        }

        private void izracunajCenu()
        {
            if(kolicina.Text == "")
            {
                cenaTxt.Text = 0.ToString();
            }
            else
            {
                int cena = int.Parse(cenaStartTxt.Text);
                int kol = int.Parse(kolicina.Text);
                cena = kol * cena;
                cenaTxt.Text = cena.ToString();
            }
        }

        private void artikliPrikaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                barkod.Text = dr["ID Artikla"].ToString();
                odabranArtikal = (int)dr["ID Artikla"];
                nazivTxt.Content = dr["Naziv artikla"].ToString();
                cenaStartTxt.Text = dr["Cena"].ToString();
                cenaTxt.Text = dr["Cena"].ToString();
            }
        }

        private void stornoCheck_Checked(object sender, RoutedEventArgs e)
        {
            if(cenaTxt.Text == "")
            {
                e.Handled = true;
            }
            else
            {
                if (int.Parse(cenaStartTxt.Text) >= 0)
                {
                    int cena = int.Parse(cenaStartTxt.Text);
                    cena = cena * -1;
                    cenaStartTxt.Text = cena.ToString();
                }
                izracunajCenu();
            }
        }

        private void stornoCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if(cenaTxt.Text == "")
            {
                e.Handled = true;
            }
            else
            {
                if (int.Parse(cenaStartTxt.Text) < 0)
                {
                    int cena = int.Parse(cenaStartTxt.Text);
                    cena = cena * -1;
                    cenaStartTxt.Text = cena.ToString();
                }
                izracunajCenu();
            }
        }

        private void filtrirajPretragu(object sender, TextChangedEventArgs e)
        {
            if (barkod.Text != "")
            {
                    if (int.Parse(barkod.Text) < 0)
                    {
                        barkod.Text = "";
                    }
                    else
                    {
                        int artID = int.Parse(barkod.Text);
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
                        artikliPrikaz.ItemsSource = dataTable.DefaultView;
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

        private void dodajUKorpuBtn(object sender, RoutedEventArgs e)
        {
            if (cenaStartTxt.Text == "")
            {
                MessageBox.Show("Prvo izaberite artikal iz tabele.");
                return;
            }
            else
            {
                if (kolicina.Text == "")
                {
                    MessageBox.Show("Pre dodavanja unesite količinu.");
                    return;

                }
                else
                {
                    if (int.Parse(kolicina.Text)==0)
                    {
                        MessageBox.Show("Kupljena količina ne može biti nula.");
                        return;
                    }
                    else
                    {
                        if (int.Parse(kolicina.Text) > getKolicina(odabranArtikal))
                        {
                            MessageBox.Show("Ovolika količina artikla nije dostupna na lageru.");
                            return;
                        }
                        else
                        {
                            dodajUKorpu();
                            ukupnaCenaLbl.Content = korpaUkupnaCena;
                        }
                    }
                    
                }
            }
        }

        private void dodajUKorpu()
        {
            string input = "";
            input += "------\n" + nazivTxt.Content + " x " + kolicina.Text + " = " + cenaTxt.Text + " RSD\n";
            if (int.Parse(cenaTxt.Text) < 0)
            {
                input += "STORNO\n";
            }
            korpatxtfield.AppendText(input);
            korpaUkupnaCena += int.Parse(cenaTxt.Text);

            artikliKorpa[indeksNiza] = odabranArtikal;
            int elemenat = artikliKorpa[indeksNiza];
            if (int.Parse(cenaTxt.Text)<0)
            {
                kolicineArtikala[indeksNiza] = (int.Parse(kolicina.Text)*(-1));
            }
            else
            {
                kolicineArtikala[indeksNiza] = int.Parse(kolicina.Text);
            }
            indeksNiza++;

            ocistiPolja();

        }

        private void barkod_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void zavrsiKupovinuBtn(object sender, RoutedEventArgs e)
        {
            if(korpatxtfield.Text == "")
            {
                MessageBox.Show("Korpa je prazna.");
                return;
            }
            else
            {
                PotvrdaKupovine dialog = new PotvrdaKupovine(kasirUsername, korpaUkupnaCena, artikliKorpa, kolicineArtikala);
                dialog.korpatxtfield.Text = korpatxtfield.Text;
                dialog.Show();
            }
            
        }
    }
}
