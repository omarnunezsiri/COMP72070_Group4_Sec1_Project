﻿using Server;
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

        /* Data communications */
        byte[] TxBuffer;
        byte[] RxBuffer;

        PacketHeader packetHeader;
        Account account;

        public ResetPassword()
        {
            InitializeComponent();

            /* Sets the Header to be of type LogIn (Resetting Password) */
            packetHeader = new(PacketHeader.AccountAction.LogIn);

            /* Creates an account with no username/password to be replaced to avoid reallocations */
            account = new Account(ClientConstants.Unused, ClientConstants.Unused);

            RxBuffer = new byte[Constants.SmallBufferMax];
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

            App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

            RxBuffer = App.client.Receive(ref App.iPEndPoint);
            
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

            if (usernameTB.Text == string.Empty)
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
            string password = "";
            if (usernameTB.Text == string.Empty || (passwordBox.Password == string.Empty && passwordTextBox.Text == string.Empty) || (cnfmpasswordBox.Password == string.Empty && cnfmpasswordTextBox.Text == string.Empty))
            {
                MessageBox.Show("Username or password cannot be empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                check = false;
            }
            else if (showPassword.IsChecked == true)
            {
                if (!passwordTextBox.Text.Equals(cnfmpasswordTextBox.Text))
                {
                    MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    check = false;
                }
                else
                {
                    check = true;
                    password = passwordTextBox.Text;
                }
            }
            else if (showPassword.IsChecked == false)
            {
                if (!passwordBox.Password.Equals(cnfmpasswordBox.Password))
                {
                    MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    check = false;
                }
                else
                {
                    check = true;
                    password = passwordBox.Password;
                }
            }
            if (unameValid.Content.ToString() == "username not found :(")
            {
                MessageBox.Show("Username does not exist!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                check = false;
            }
            if (check == true && unameValid.Content.ToString() == "username found :)")
            {
                PacketHeader packetHeader = new PacketHeader(PacketHeader.AccountAction.NotApplicable);
                Account account = new Account(usernameTB.Text, password);

                Packet packet = new(packetHeader, account);
                Logger instance = Logger.Instance;

                instance.Log(packet, true);

                TxBuffer = packet.Serialize();

                App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

                RxBuffer = App.client.Receive(ref App.iPEndPoint);

                packet = new(RxBuffer);
                instance.Log(packet, false);

                account = (Account)packet.body;

                if (account.getStatus() == Account.Status.Success)
                {
                    success.Visibility = Visibility.Visible;
                    submitButton.Visibility = Visibility.Hidden;
                    nextButton.Visibility = Visibility.Visible;
                    cancelButton.Visibility = Visibility.Hidden;
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
    }
}
