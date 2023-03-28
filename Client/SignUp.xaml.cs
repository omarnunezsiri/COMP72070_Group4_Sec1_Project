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

namespace Client
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        bool check = false;

        public SignUp()
        {
            InitializeComponent();
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

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            //insert check for username and pw
            if (usernameTB.Text.Length <= 3)
            {
                MessageBox.Show("Username too short!", "Warning", MessageBoxButton.OK);
                check = false;
            }
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
            if (check == true)
            {
                success.Visibility = Visibility.Visible;
                submitButton.Visibility = Visibility.Hidden;
                nextButton.Visibility = Visibility.Visible;
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Login newWindow = new Login();
            newWindow.Show();
            this.Close();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            Login newWindow = new Login();
            newWindow.Show();
            this.Close();
        }
    }
}
