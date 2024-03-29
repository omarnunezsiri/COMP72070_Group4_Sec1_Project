﻿using System;
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
using System.Net.Sockets;
using System.Diagnostics;

namespace Client
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        bool check = false;
        byte[] RxBuffer;

        public SignUp()
        {
            InitializeComponent();
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
            //validation code similar to the one in reset password to check if the username is taken

            PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
            Account account = new Account(usernameTB.Text, ClientConstants.Unused);

            Packet packet = new(packetHeader, account);
            Logger instance = Logger.Instance;
            instance.Log(packet, true);

            byte[] TxBuffer = packet.Serialize();

            App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

            RxBuffer = App.client.Receive(ref App.iPEndPoint);

            packet = new(RxBuffer);
            instance.Log(packet, false);

            account = (Account)packet.body;

            if(account.getStatus() == Account.Status.Success)
            {
                unameValid.Content = "username not taken :)";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkMagenta;
                pw.Visibility = Visibility.Visible;
                cnfmpw.Visibility = Visibility.Visible;
                stackPanel1.Visibility = Visibility.Visible;
                stackPanel2.Visibility = Visibility.Visible;
                //Debug.WriteLine("Good username!");
            }
            else
            {
                unameValid.Content = "username taken :(";
                unameValid.Visibility = Visibility.Visible;
                unameValid.Foreground = Brushes.DarkRed;
                pw.Visibility = Visibility.Hidden;
                cnfmpw.Visibility = Visibility.Hidden;
                stackPanel1.Visibility = Visibility.Hidden;
                stackPanel2.Visibility = Visibility.Hidden;
                //Debug.WriteLine("Taken!");
            }

            if (usernameTB.Text == string.Empty)
            {
                unameValid.Visibility = Visibility.Hidden;
                pw.Visibility = Visibility.Hidden;
                cnfmpw.Visibility = Visibility.Hidden;
                stackPanel1.Visibility = Visibility.Hidden;
                stackPanel2.Visibility = Visibility.Hidden;
            }
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
            if (usernameTB.Text == string.Empty || (passwordBox.Password == string.Empty && passwordTextBox.Text == string.Empty) || (cnfmpasswordBox.Password == string.Empty && cnfmpasswordTextBox.Text == string.Empty))
            {
                MessageBox.Show("Username or password cannot be empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                check = false;
            }
            else if (usernameTB.Text.Length < 3)
            {
                MessageBox.Show("Username too short!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    actualPw = passwordTextBox.Text;
                    check = true;
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
                Logger instance = Logger.Instance;
                instance.Log(packet, true);

                App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

                RxBuffer = App.client.Receive(ref App.iPEndPoint);

                Packet responsePacket = new Packet(RxBuffer);
                instance.Log(packet, false);
                Account responseBody = (Account)responsePacket.body;

                if (responseBody.getStatus() == Account.Status.Success)
                {
                    success.Visibility = Visibility.Visible;
                    submitButton.Visibility = Visibility.Hidden;
                    nextButton.Visibility = Visibility.Visible;
                    cancelButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Username is taken! Try using a different one", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
    }
}
