using FlameChatClient.ChatClient;
using FlameChatClient.Client_Operations;
using FlameChatClient.Encryption;
using FlameChatClient.Notifications;
using FlameChatShared.Communication;
using Microsoft.Win32;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace FlameChatClient.GUI
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUpChatUser : Window
    {
        Client client;

        public SignUpChatUser()
        {
            InitializeComponent();


            //ServerIPInputDialog serverIPInputDialog = new ServerIPInputDialog();
            //serverIPInputDialog.ShowDialog();

            client = GlobalVariables.GetClient();
            //GlobalVariables.MessagesFromClient = _test_messages_list;
            this.Loaded += SignUpChatUser_Loaded; 

        }

        private async void SignUpChatUser_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                await client.StartClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start client: {ex.Message}");
            }
        }



        string imagefilename;


        private async void btnsignup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtusername.Text) || string.IsNullOrEmpty(txtpassword.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            ChatUser user = new ChatUser()
            {
                Username = txtusername.Text

            };


            await client.SendCommandToServer(RegularUserAllowedCommands.CheckIfUsernameExists, user);


            Response usernameExists = await GlobalVariables.GetClient().GetResponse();
            if ((bool)usernameExists.Value)
            {
                MessageBox.Show("Username exists, please choose another.");
                return;

            }
            else
            {
                string originalPassword = txtpassword.Text;

                user.Username = txtusername.Text;
                user.Password = PasswordHasher.HashPassword(txtpassword.Text);
                user.AvatarImage = File.ReadAllBytes(imagefilename);
                

                await client.SendCommandToServer(RegularUserAllowedCommands.SignUp, user);
                Response response = await GlobalVariables.GetClient().GetResponse();


                //MessageBox.Show("signup - waiting 10 sec");
                //Thread.Sleep(10000);

                try
                {
                    RegularUserLogIn login = new RegularUserLogIn();

                    bool success = await login.LogIn(user.Username, originalPassword);

                    //GlobalVariables.LogMessage("username: " + user.Username + ", password: " + user.Password);

                    if (success)
                    {
                        await SessionManager.StartSession(user.Username, user.Password);

                        ChatWindow chat = new ChatWindow();

                        chat.Show();

                        Notifier.ChatWindow = chat;

                        chat.SetAllData();

                        //UserInfo userInfo = new UserInfo();

                        //userInfo.Show();

                        //await userInfo.SetAllDataToWindowAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during login: {ex.Message}");
                }
                //Hide();


            }

            Close();

        }

        private void btn_pick_img_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                imagefilename = openFileDialog.FileName;
                // Load the selected image into the Image control
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                avatar.Source = bitmap;
                avatar.Stretch = Stretch.UniformToFill;
            }
        }


        private Timer? debounceTimer;
        private readonly object lockObject = new object();

        private async void txtusername_TextChanged(object sender, TextChangedEventArgs e)
        {
            lock (lockObject)
            {
                // Cancel any previous timer
                debounceTimer?.Dispose();

                // Create new timer that will wait before making the server call
                debounceTimer = new Timer(async (state) =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        try
                        {
                            string username = txtusername.Text;


                            await client.SendCommandToServer(RegularUserAllowedCommands.CheckIfUsernameExists, new ChatUser() { Username = username });
                            Response response = await GlobalVariables.GetClient().GetResponse();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _test_messages_list.Items.Add($"{DateTime.Now:HH:mm:ss} - Checked username: {username} - {response.Value}{Environment.NewLine}");
                                _test_messages_list.Items.Add( response.Value.ToString()+Environment.NewLine);
                                if ((bool)response.Value == true)
                                {
                                    message.Foreground = System.Windows.Media.Brushes.Red;
                                    message.Content = "Username already exists.";
                                }
                                else
                                {
                                    message.Foreground = System.Windows.Media.Brushes.Green;
                                    message.Content = "Username is ok.";
                                }
                            });
                        }

                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                message.Foreground = System.Windows.Media.Brushes.Red;
                                message.Content = "Error checking username.";
                                _test_messages_list.Items.Add($"{DateTime.Now:HH:mm:ss} - Error: {ex.Message}{Environment.NewLine}");
                            });
                        }
                    });
                }, null, 150, Timeout.Infinite); // Waits 500ms before checking

               
            }

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            debounceTimer?.Dispose();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           // client.StopClient();
        }
    }
}
