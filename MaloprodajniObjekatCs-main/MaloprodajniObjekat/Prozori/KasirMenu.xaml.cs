using System;
using System.Collections.Generic;
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
    /// Interaction logic for KasirMenu.xaml
    /// </summary>
    public partial class KasirMenu : Window
    {
        public string kasirUsername;
        public KasirMenu(string username)
        {
            InitializeComponent();
            kasirUsername = username;
            loggedIn.Content = kasirUsername;
        }

        private void returnToLogin(object sender, RoutedEventArgs e)
        {
            MainWindow dashboard = new MainWindow();
            dashboard.Show();
            this.Close();
        }

        private void kasirKupovinaWindow(object sender, RoutedEventArgs e)
        {
            kasirFrame.Content = new KasirKupovina(kasirUsername);
        }

        private void kasirRacuniWindow(object sender, RoutedEventArgs e)
        {
            kasirFrame.Content = new KasirRacuni();
        }
    }
}
