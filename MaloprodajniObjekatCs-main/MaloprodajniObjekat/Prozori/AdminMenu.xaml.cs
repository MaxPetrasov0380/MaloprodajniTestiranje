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
    /// Interaction logic for AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Window
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private void returnToLogin(object sender, RoutedEventArgs e)
        {
            MainWindow dashboard = new MainWindow();
            dashboard.Show();
            this.Close();
        }

        private void adminInventarWindow(object sender, RoutedEventArgs e)
        {
            adminFrame.Content = new AdminInventar();
        }

        private void adminKasiriWindow(object sender, RoutedEventArgs e)
        {
            adminFrame.Content = new AdminKasiri();
        }

        private void adminAdminiWindow(object sender, RoutedEventArgs e)
        {
            adminFrame.Content = new AdminAdmini();
        }

        private void adminSefoviWindow(object sender, RoutedEventArgs e)
        {
            adminFrame.Content = new AdminSefovi();
        }
    }
}
