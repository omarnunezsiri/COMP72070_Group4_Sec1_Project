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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow newWindow = new MainWindow();
                newWindow.Show();
                this.Close();
            }
            
        }

        private void searchtb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // user hasn't entered a username;
            if (tb.Text == string.Empty)
            {
                tb.Text = "Search...";
            }
        }

        private void searchtb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // username is placeholder
            if (tb.Text == "Search...")
            {
                tb.Text = string.Empty;
            }
        }
    }
}
