using System;
using System.Collections;
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
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        bool check = false;
        //bool valid;
        ArrayList usernames = new ArrayList();

        public ResetPassword()
        {
            InitializeComponent();
            AddToList();
        }
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            passwordTextBox.Text = passwordBox.Password;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Visibility = Visibility.Visible;

            cnfmpasswordTextBox.Text = cnfmpasswordBox.Password;
            cnfmpasswordBox.Visibility = Visibility.Collapsed;
            cnfmpasswordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = passwordTextBox.Text;
            passwordTextBox.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;

            cnfmpasswordBox.Password = cnfmpasswordTextBox.Text;
            cnfmpasswordTextBox.Visibility = Visibility.Collapsed;
            cnfmpasswordBox.Visibility = Visibility.Visible;

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox tb = (TextBox)sender;
            bool valid = false;

            for (int i = 0; i < usernames.Count; i++)
            {
                if (usernames[i].ToString() == usernameTB.Text)
                {
                    valid = true;
                }
            }

            if (valid == true)
            {
                unameValid.Content = "username found :)";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkMagenta;
            }
            else
            {
                unameValid.Content = "username not found :(";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkRed;
            }
        }

        private void usernameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text == string.Empty)
            {
                unameValid.Visibility = Visibility.Hidden;
            }
        }

        private void usernameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text == string.Empty)
            {
                unameValid.Visibility = Visibility.Hidden;
            }
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            //add check from server ???
            if (usernameTB.Text == null && passwordBox.Password == null || cnfmpasswordBox.Password == null)
            {
                MessageBox.Show("Username or password cannot be empty!", "Warning", MessageBoxButton.OK);
                check = false;
            }
            else if (showPassword.IsChecked == true)
            {
                if (!passwordTextBox.Text.Equals(cnfmpasswordTextBox.Text))
                {
                    MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK);
                    check = false;
                }
                else
                {
                    check = true;
                }
            }
            else if (showPassword.IsChecked == false)
            {
                if (!passwordBox.Password.Equals(cnfmpasswordBox.Password))
                {
                    MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK);
                    check = false;
                }
                else
                {
                    check = true;
                }
            }
            if (unameValid.Content == "username not found :(")
            {
                MessageBox.Show("Username does not exist!", "Warning", MessageBoxButton.OK);
                check = false;
            }
            if (check == true)
            {
                success.Visibility = Visibility.Visible;
                submitButton.Visibility = Visibility.Hidden;
                nextButton.Visibility = Visibility.Visible;
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            //Login newWindow = new Login();
            //newWindow.Show();
            this.Close();
        }

        private void AddToList()
        {
            usernames.Add("spaceman");
            usernames.Add("dxbby");
            usernames.Add("ajdj");
            usernames.Add("itsdee");
            usernames.Add("sharkfin");
        }
    }
}
