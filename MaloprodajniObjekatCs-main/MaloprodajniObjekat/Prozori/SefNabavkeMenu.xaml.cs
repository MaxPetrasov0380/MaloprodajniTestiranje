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
    /// Interaction logic for SefNabavkeMenu.xaml
    /// </summary>
    public partial class SefNabavkeMenu : Window
    {
        public string sefUsername = "";
        public SefNabavkeMenu(string username)
        {
            InitializeComponent();
            sefUsername = username;
            loggedIn.Content = sefUsername;
        }

        private void returnToLogin(object sender, RoutedEventArgs e)
        {
            MainWindow dashboard = new MainWindow();
            dashboard.Show();
            this.Close();
        }

        private void sefNapraviNabavkuWindow(object sender, RoutedEventArgs e)
        {
            sefFrame.Content = new SefInventar(sefUsername);
        }

        private void sefPregledNabavkiWindow(object sender, RoutedEventArgs e)
        {
            sefFrame.Content = new SefPregledNabavki();
        }
    }
}
