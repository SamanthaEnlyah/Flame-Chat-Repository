using FlameChat2;
using FlameChatClient.ChatClient;
using FlameChatClient.Client_Data.ViewModels;
using FlameChatClient.Client_Operations;
using FlameChatClient.Notifications;
using FlameChatShared.Communication;
using Microsoft.Extensions.Logging.Abstractions;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Chat.xaml  
    /// </summary>  
    public partial class ChatWindow : Window
    {

        ChatManager chatManager;

        public ChatWindow()
        {
            InitializeComponent();


            //IPInputDialog serverIPInputDialog = new ServerIPInputDialog();
            //serverIPInputDialog.ShowDialog();

            Test test = new Test("Chat Window");
            //GlobalVariables.MessagesFromClient = test.Messages;
            //test.Show();

            Notifier.ChatWindow = this;
            listOfContacts.DisplayMemberPath = "Username";
            btn_profile.IsEnabled = false;

            chatManager = new ChatManager();
            chatMessages.ItemsSource = new List<ChatWindowMessageViewModel>();
        }

        public async void SetAllData()
        {
            if (SessionManager.IsSessionStarted())
            {
               await SessionManager.LoadSession();
                

                labelMyUsername.Content = SessionManager.Data.User.Username;
                
                logged_in_avatar.Source = (BitmapSource)new ImageSourceConverter().ConvertFrom(SessionManager.Data.User.AvatarImage);
                
                listOfContacts.DisplayMemberPath = "Username";

                await FillContactsList();

                await FillMessagesViewModel();

            }



            btn_profile.IsEnabled = true;
        }

        private async Task FillMessagesViewModel()
        {
         
            await chatManager.ChatDataMessages.FillMessages();

            //MessageBox.Show("number of messages: " + chatManager.ChatDataMessages.messages.Count);
        }



        private async Task FillContactsList()
        {
            ContactsOp contactsOp = new ContactsOp();
            var contacts = await contactsOp.GetAllContacts(SessionManager.Data.User.ChatUserId);
            listOfContacts.ItemsSource = contacts;
            listOfContacts.DisplayMemberPath = "Username";
        }

        public void SetContactToTalkTo()
        {
            ChatUser currentContact = (ChatUser)listOfContacts.SelectedItem;

            if (currentContact != null)
            {
                labelIAmTalkingToContactUsername.Content = currentContact.Username;
                
                 if (currentContact.AvatarImage == null) return;

                imgContactAvatar.Source = (BitmapSource)new ImageSourceConverter().ConvertFrom(currentContact.AvatarImage);
            }

            //string path = System.IO.Path.GetFullPath("session/image_profile");
            //SessionManager.Data.ContactToTalkTo = currentContact;
        }

        private async void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            UserInfo userInfo = new UserInfo();
            //userInfo.User = SessionManager.Data.User;
            await userInfo.StartClientIfNeeded();
            await userInfo.SetAllDataToWindowAsync();
            userInfo.Show();
        }

        private async void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            

            if (logged_in_avatar.Source is BitmapImage bmp)
            {
                bmp.StreamSource?.Dispose(); // Dispose the stream if used
                bmp.UriSource = null;
                logged_in_avatar.Source = null; // Clear the source
            }


            await Task.Delay(1000);
            RegularUserLogOut logOut = new RegularUserLogOut();
            await logOut.LogOut();
            //MainWindow main = new MainWindow();
            //main.Show();
            Close();
        }

        private void listOfContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetContactToTalkTo();

            ChatUser currentContact = (ChatUser)listOfContacts.SelectedItem;

            List<ChatWindowMessageViewModel> messagesContact = chatManager.ChatDataMessages.GetMessagesForContact(currentContact);

            chatMessages.ItemsSource = null;
            chatMessages.ItemsSource = messagesContact;
            chatMessages.DisplayMemberPath = "UsersMessage";

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listOfContacts.DisplayMemberPath = "Username";

        }


        public async Task StartClientIfNeeded()
        {
            if (GlobalVariables.GetClient().IsSocketNull())
            {
                //GlobalVariables.LogMessage("Client is null, starting client");
                await GlobalVariables.GetClient().StartClient();
             
            }
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if(listOfContacts.SelectedItem == null)
            {
                
                MessageBox.Show("Please select a contact to talk to.");
                return;
                
            }

            if(InputMessageText.Text == "")
            {
                MessageBox.Show("Please enter a message to send.");
                return;
            }
            Message message = new Message();
            message.FkUserWhoSentId = SessionManager.Data.User.ChatUserId;
            message.FkWhoReceivedId = ((ChatUser)listOfContacts.SelectedItem).ChatUserId;
            message.Text = InputMessageText.Text;
            message.DateTimeOfSending = DateTime.Now;

            MessagesOp messagesOp = new MessagesOp();
            messagesOp.SendMessage(message);

         
            AddMyMessageToList(message);

            InputMessageText.Text = string.Empty;

            
        }

        private void AddMyMessageToList(Message message)
        {
            if (chatMessages.ItemsSource is List<ChatWindowMessageViewModel> messages)
            {
                //messages.Add($"{message.DateTimeOfSending} {SessionManager.Data.User.Username}: {message.Text}");
               

                ChatWindowMessageViewModel newMessageViewModel = new ChatWindowMessageViewModel();
                newMessageViewModel.SentMessage = message;
                newMessageViewModel.Sender = SessionManager.Data.User;
                newMessageViewModel.Receiver = (ChatUser)listOfContacts.SelectedItem;

                
               chatManager.ChatDataMessages.AddMessage(newMessageViewModel);

               
                messages.Add(newMessageViewModel);

                chatMessages.ItemsSource = null;
                chatMessages.ItemsSource = messages;
                //chatMessages.ScrollIntoView(newMessageViewModel); // Scroll to the new message
                chatMessages.DisplayMemberPath = "UsersMessage";

                    
            }
            else
            {
                var newMessages = new List<ChatWindowMessageViewModel>();
                chatMessages.ItemsSource = newMessages;

            }
        }

        public async void AddMessageToList(Message message)
        {

            
            //Response r = await GlobalVariables.GetClient().GetResponse();
            

            int senderid = (int)message.FkUserWhoSentId;
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserFromId, senderid);
            Response rChatUserSender = await GlobalVariables.GetClient().GetResponse();

            ChatUser sender = (ChatUser)rChatUserSender.Value;

            if (chatMessages.ItemsSource is List<ChatWindowMessageViewModel> messages)
            {
                //messages.Add($"{message.DateTimeOfSending} {username}: {message.Text}");
                ChatWindowMessageViewModel newMessageViewModel = new ChatWindowMessageViewModel();
                newMessageViewModel.SentMessage = message;
                newMessageViewModel.Sender = sender;
                newMessageViewModel.Receiver = SessionManager.Data.User;


                chatManager.ChatDataMessages.AddMessage(newMessageViewModel);

                if (listOfContacts.SelectedItem != null)
                {
                    if (newMessageViewModel.Sender.Username == ((ChatUser)listOfContacts.SelectedItem).Username)
                    {
                        messages.Add(newMessageViewModel);

                        chatMessages.ItemsSource = null;
                        chatMessages.ItemsSource = messages;
                        //chatMessages.ScrollIntoView(newMessageViewModel); // Scroll to the new message

                        chatMessages.DisplayMemberPath = "UsersMessage";

                    }
                 
                }
            }
            else
            {
                var newMessages = new List<ChatWindowMessageViewModel>();
                chatMessages.ItemsSource = newMessages;

            }
          
        }

        internal void SetContacts(List<ChatUser> users)
        {
            listOfContacts.ItemsSource = users;
            listOfContacts.DisplayMemberPath = "Username";
        }

        public void AddContact(ChatUser user)
        {
            if(listOfContacts.ItemsSource is List<ChatUser> contacts)
            {
                contacts.Add(user);
                listOfContacts.ItemsSource = null;
                listOfContacts.ItemsSource = contacts;
            }
            else
            {
                var newContacts = new List<ChatUser> { user };
                listOfContacts.ItemsSource = newContacts;
            }
        }

        public void ShowMessageFromServer(string value)
        {
            MessageBox.Show(value);
        }
    }
}
