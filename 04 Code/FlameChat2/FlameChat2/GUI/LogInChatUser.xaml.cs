using FlameChatClient.ChatClient;
using FlameChatClient.Client_Operations;
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
using static System.Net.Mime.MediaTypeNames;

namespace FlameChatClient.GUI
{
    /// <summary>
    /// Interaction logic for UserLogIn.xaml
    /// </summary>
    public partial class LogInChatUser : Window
    {
        Client client = GlobalVariables.GetClient();

        public LogInChatUser()
        {
            InitializeComponent();
            //ServerIPInputDialog serverIPInputDialog = new ServerIPInputDialog();
            //serverIPInputDialog.ShowDialog();
           // StartLogWindow();
        }

        public void StartLogWindow()
        {
            //Test test = new Test("Log in Chat User");
            //test.Show();
            //GlobalVariables.MessagesFromClient = test.Messages;
        }

        private async void btn_login_now_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("login");
            if (!client.IsStarted())
            {
                await client.StartClient();
            }
            RegularUserLogIn logIn = new RegularUserLogIn();
            bool success = await logIn.LogIn(txt_username.Text, txt_password.Text);

            if (success)
            {
                bool sessionStarted = SessionManager.IsSessionStarted(txt_username.Text, txt_password.Text);
                if (sessionStarted)
                {
                    //MessageBox.Show("session started before");
                    //GlobalVariables.LogMessage("Login from earlier session successful.");
                }
                else
                {
                    //MessageBox.Show("session starting now");
                    await SessionManager.StartSession(txt_username.Text, txt_password.Text);
                    //MessageBox.Show("session started now");
                    //GlobalVariables.LogMessage("Login successful. Session started.");
                    ChatWindow chatWindow = new ChatWindow();
                    chatWindow.Show();
                    chatWindow.SetAllData();
                    this.Close();
                }
                //await SessionManager.StartSession(txt_username.Text, txt_password.Text);
                // If the login is successful, navigate to the chat window
                
            }
            else
            {
                MessageBox.Show("Login failed. Please check your credentials.");
                RegularUserLogOut logOut = new RegularUserLogOut();
                await logOut.LogOut();
                GlobalVariables.GetClient().StopClient();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            await client.StartClient();
            LogIn();
        }

        public void LogIn()
        {
            

            RegularUserLogIn logIn = new RegularUserLogIn();

            //MessageBox.Show("Checking if logged in");
            if (logIn.IsLoggedIn())
            {
                ChatWindow chatWindow = new ChatWindow();
                chatWindow.Show();
                chatWindow.SetAllData();
            }
            else
            {
               // MessageBox.Show("Not logged in");
            }

        }
    }
}
