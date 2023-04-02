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
using Server;
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
            //validation code similar to the one in reset password to check if the username is taken
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            PerformCreate();
        }

        private void PerformCreate()
        {
            check = false;
            string actualPw = "";
            //insert check for username and pw

            //add check from server ???
            if (usernameTB.Text == string.Empty || passwordBox.Password == string.Empty || cnfmpasswordBox.Password == string.Empty)
            {
                MessageBox.Show("Username or password cannot be empty!", "Warning", MessageBoxButton.OK);
                check = false;
            }
            else if (usernameTB.Text.Length < 3)
            {
                MessageBox.Show("Username too short!", "Warning", MessageBoxButton.OK);
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
                    actualPw = passwordTextBox.Text;
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
                    actualPw = passwordBox.Password;
                    check = true;
                }
            }

            if (check == true)
            {
                /* Sets the PacketHeader to be a type of Account Sign Up */
                PacketHeader packetHeader = new PacketHeader(PacketHeader.AccountAction.SignUp);

                /* Builds the Account body with the contents given by user */
                Account account = new Account(usernameTB.Text, actualPw);

                /* Creates the Packet to serialize */
                Packet packet = new Packet(packetHeader, account);

                /* Serialize and send packet bytes to Server */
                byte[] TxBuffer = packet.Serialize();

                // Send TxBuffer

                /* This simulates the Server appending a Success response */
                Packet serverPacket = new Packet(TxBuffer);
                Account clientAccount = (Account)serverPacket.body;
                clientAccount.setStatus(Account.Status.Success);

                // Receive response Packet
                byte[] RxBuffer = serverPacket.Serialize();
                Packet responsePacket = new Packet(RxBuffer);
                Account responseBody = (Account)responsePacket.body;

                if (responseBody.getStatus() == Account.Status.Success)
                {
                    success.Visibility = Visibility.Visible;
                    submitButton.Visibility = Visibility.Hidden;
                    nextButton.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Username is taken! Try using a different one", "Warning", MessageBoxButton.OK);
                }
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformCreate();
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
