using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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

        /* Data communications */
        byte[] TxBuffer;
        byte[] RxBuffer;
        NetworkStream stream;

        PacketHeader packetHeader;
        Account account;

        public ResetPassword()
        {
            InitializeComponent();
            AddToList();

            /* Sets the Header to be of type LogIn (Resetting Password) */
            packetHeader = new(PacketHeader.AccountAction.LogIn);

            /* Creates an account with no username/password to be replaced to avoid reallocations */
            account = new Account(ClientConstants.Unused, ClientConstants.Unused);

            stream = App.client.GetStream();
            RxBuffer = new byte[1024];
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
            /* Builds the packet including the username to be searched */ 
            account.setUsername(usernameTB.Text);
            Packet packet = new(packetHeader, account);

            Logger instance = Logger.Instance;
            instance.Log(packet, true);

            /* Serializes the packet */
            TxBuffer = packet.Serialize();

            stream.Write(TxBuffer);

            stream.Read(RxBuffer);
            
            /* Deserializes Response packet */
            packet = new(RxBuffer);
            instance.Log(packet, false);

            /* Takes the Account body to check for Success/Failure */
            account = (Account)packet.body;

            if(account.getStatus() == Account.Status.Success)
            {
                unameValid.Content = "username found :)";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkMagenta;
                newpw.Visibility = Visibility.Visible;
                cnfmnewpw.Visibility = Visibility.Visible;
                stackPanel1.Visibility = Visibility.Visible;
                stackPanel2.Visibility = Visibility.Visible;
            }
            else
            {
                unameValid.Content = "username not found :(";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkRed;
                newpw.Visibility = Visibility.Hidden;
                cnfmnewpw.Visibility = Visibility.Hidden;
                stackPanel1.Visibility = Visibility.Hidden;
                stackPanel2.Visibility = Visibility.Hidden;
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
            PerformReset();
        }

        private void PerformReset()
        {
            check = false;
            //add check from server ???
            if (usernameTB.Text == string.Empty || passwordBox.Password == string.Empty || cnfmpasswordBox.Password == string.Empty)
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
            if (check == true && unameValid.Content == "username found :)")
            {
                PacketHeader packetHeader = new PacketHeader(PacketHeader.AccountAction.NotApplicable);
                string actualPw = passwordBox.Password ?? passwordTextBox.Text;
                Account account = new Account(usernameTB.Text, actualPw);

                Packet packet = new(packetHeader, account);
                Logger instance = Logger.Instance;

                instance.Log(packet, true);

                TxBuffer = packet.Serialize();

                stream.Write(TxBuffer);

                stream.Read(RxBuffer);

                packet = new(RxBuffer);
                instance.Log(packet, false);

                account = (Account)packet.body;

                if (account.getStatus() == Account.Status.Success)
                {
                    success.Visibility = Visibility.Visible;
                    submitButton.Visibility = Visibility.Hidden;
                    nextButton.Visibility = Visibility.Visible;
                }
            }

            //prints passwords dont match and username doesnt exist
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

        private void cnfmpasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (showPassword.IsChecked == true)
            {
                cnfmpasswordTextBox.Text = cnfmpasswordBox.Password;
            }
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (showPassword.IsChecked == true)
            {
                passwordTextBox.Text = passwordBox.Password;
            }
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (showPassword.IsChecked == false)
            {
                passwordBox.Password = passwordTextBox.Text;
            }
        }

        private void cnfmpasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (showPassword.IsChecked == false)
            {
                cnfmpasswordBox.Password = cnfmpasswordTextBox.Text;
            }   
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformReset();
            }
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = Keyboard.FocusedElement as TextBox;
            if (textBox != null)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                textBox.MoveFocus(tRequest);
            }
        }
    }
}
