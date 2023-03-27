using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //insert check for username and pw

            Home newWindow = new Home();
            newWindow.Show();
            this.Hide();
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            SignUp newWindow = new SignUp();
            newWindow.Show();
            this.Hide();
        }

        private void usernameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // user hasn't entered a username;
            if (tb.Text == string.Empty)
            {
                usernameTB.Text = "Username";
            }
        }

        private void usernameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // username is placeholder
            if(tb.Text == "Username")
            {
                tb.Text = string.Empty;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;

            // password is placeholder
            if(pb.Password == "Password")
            {
                pb.Password = string.Empty;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;

            // user hasn't entered a password
            if(pb.Password == string.Empty) 
            {
                pb.Password = "Password";
            }
        }

        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox pwtb = (TextBox)sender;

            // password is placeholder
            if (pwtb.Text == "Password")
            {
                pwtb.Text = string.Empty;
            }
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox pwtb = (TextBox)sender;

            // user hasn't entered a password
            if (pwtb.Text == string.Empty)
            {
                pwtb.Text = "Password";
            }
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            passwordTextBox.Text = passwordBox.Password;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = passwordTextBox.Text;
            passwordTextBox.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;
        }

    }
}
