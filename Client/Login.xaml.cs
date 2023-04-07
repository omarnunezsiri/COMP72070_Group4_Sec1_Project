using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        /* Data communications */
        byte[] TxBuffer;
        byte[] RxBuffer;

        public Login()
        {
            InitializeComponent();
            RxBuffer = new byte[Constants.SmallBufferMax];
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            if (usernameTB.Text == string.Empty || (passwordBox.Password == string.Empty && passwordTextBox.Text == string.Empty) || usernameTB.Text == "Username")
            {
                MessageBoxResult result = MessageBox.Show("Username or password cannot be empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                /* Sets the header information to be an Account LogIn */
                PacketHeader packetHeader = new PacketHeader(PacketHeader.AccountAction.LogIn);

                /* Builds the body with the given login information */
                string password;
                if (showPassword.IsChecked is true)
                    password = passwordTextBox.Text;
                else
                    password = passwordBox.Password;

                Account account = new(usernameTB.Text, password);

                /* Builds the packet with all necessary information and serializes it */
                Packet packet = new Packet(packetHeader, account);
                TxBuffer = packet.Serialize();

                Logger instance = Logger.Instance;
                instance.Log(packet, true);

                App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

                RxBuffer = App.client.Receive(ref App.iPEndPoint);

                // Receive response Packet
                packet = new Packet(RxBuffer);
                instance.Log(packet, false);

                account = (Account)packet.body;

                if (account.getStatus() == Account.Status.Success)
                {
                    Home newWindow = new Home(usernameTB.Text);
                    newWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            SignUp newWindow = new SignUp();
            newWindow.Show();
            this.Close();
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

        private void forgotPass_Click(object sender, RoutedEventArgs e)
        {
            ResetPassword newWindow = new ResetPassword();
            newWindow.Show();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.FocusedElement.GetType() == typeof(TextBox))
            {
                TextBox textBox = (TextBox)Keyboard.FocusedElement;

                if (textBox != null)
                {
                    TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(tRequest);
                }
            }

            if (Keyboard.FocusedElement.GetType() == typeof(PasswordBox))
            {
                PasswordBox pBox = (PasswordBox)Keyboard.FocusedElement;
                
                if (pBox != null)
                {
                    TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                    pBox.MoveFocus(tRequest);
                }

            }
        }
    }
}
