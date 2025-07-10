
using FlameChatClient.ChatClient;
using FlameChatClient.Client_Operations;
using FlameChatClient.Notifications;
using FlameChatShared.Communication;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using SharedModel.ComplexModels;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace FlameChatClient.GUI
{
    public delegate void SelectionChangedHandler(object sender, SelectionChangedEventArgs e);
 
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class UserInfo : Window
    {

        //public ChatUser User { get; set; }
        //public List<ChatUser> AllOtherUsers { get; set; }

        public UserInfo()
        {
            InitializeComponent();
            listTraits.DisplayMemberPath = "TraitName";
            Notifier.UserInfo = this;
        }


        public async Task StartClientIfNeeded()
        {
            if (GlobalVariables.GetClient().IsSocketNull())
            {
                Test test = new Test("USER INFO");
                GlobalVariables.MessagesFromClient = test.Messages;
                test.Show();
                //GlobalVariables.LogMessage("Client is null, starting client");
                await GlobalVariables.GetClient().StartClient();
            }
        }

        public async Task SetAllDataToWindowAsync()
        {
            //set image
            //File.WriteAllBytes("session/image_profile", SessionManager.Data.User.AvatarImage);
            
            
            //string path = System.IO.Path.GetFullPath("session/image_profile");
           //loggedin_user_image.Source = new BitmapImage(new Uri(path));


            //set username and password
            txt_username.Text = SessionManager.Data.User.Username;
            txt_password.Text = SessionManager.Data.User.Password;

            var bitmap = (BitmapSource)new ImageSourceConverter().ConvertFrom(SessionManager.Data.User.AvatarImage);

            loggedin_user_image.Source = bitmap;



            //set users
            await RefreshUsersList();

            //set contacts
            await RefreshContactsList();

            //set traits
            await RefreshPersonalityTraitsList();


        }

        private async Task RefreshUsersList()
        {
            //MessageBox.Show("Other Users");
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetOtherUsers, SessionManager.Data.User.Username);

            //set users
            Response users = await GlobalVariables.GetClient().GetResponse();
            //MessageBox.Show("refresh users, response hash: " + users.GetHashCode());

            if (users?.Value != null)
            {

                if (users.Value is not List<ChatUser>)
                {
                    MessageBox.Show("Failed to cast users list");

                    MessageBox.Show($"users.Value type: {users.Value?.GetType().FullName ?? "null"}");
                    MessageBox.Show($"users.Value {users.Value}");
                    if (users.Value is JsonElement jsonElement)
                    {
                        MessageBox.Show($"JSON type: {jsonElement.ValueKind}");
                    }

                    return;
                }



                var usersList = users.Value as List<ChatUser>;
                list_users.Items.Clear();
                list_users.ItemsSource = usersList;
                list_users.DisplayMemberPath = "Username";

                // MessageBox.Show("User list done");

            }
            else
            {
                MessageBox.Show("Failed to load other users");
            }

        }
        

        private async Task RefreshPersonalityTraitsList()
        {
           // MessageBox.Show("Personality traits");
            PersonalityTraitsOp ptops = new PersonalityTraitsOp();
            // listTraits.Items.Clear();
            //MessageBox.Show("userid for pt:" + SessionManager.Data.User.ChatUserId);
            listTraits.ItemsSource = await ptops.GetMyPersonalityTraits(SessionManager.Data.User.ChatUserId);
            //MessageBox.Show("Personality traits list done");
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            

            if (loggedin_user_image.Source is BitmapImage bmp)
            {
                bmp.StreamSource?.Dispose(); // Dispose the stream if used
                bmp.UriSource = null;
                loggedin_user_image.Source = null; // Clear the source
            }


            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            await Task.Delay(1000);

        }

       //private readonly Queue<(object sender, SelectionChangedEventArgs e)> selectionQueue = new();
       bool canRun = true;
        // //List<SelectionChangedEventHandler> selections = new List<SelectionChangedEventHandler>();

        // private async Task ProcessSelectionsAsync()
        // {
        //     //isProcessingSelections = true;
        //     while (selectionQueue.Count > 0)
        //     {
        //         var (sender, e) = selectionQueue.Dequeue();
        //         await HandleSelectionChangedAsync(sender, e);
        //     }
        //     //isProcessingSelections = false;
        // }


        // private void list_users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        // {
        //     // Enqueue the selection
        //     selectionQueue.Enqueue((sender, e));
        //     // Start processing if not already running
        //     //if (!isProcessingSelections)
        //     //{
        //         _ = ProcessSelectionsAsync();
        //     //}
        // }

        // private async Task HandleSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        // {
        //     //GlobalVariables.LogMessage("User selection changed");
        //     if (list_users.SelectedItem != null)
        //     {
        //         //GlobalVariables.LogMessage("setting avatar");
        //         Avatar avatarManager = new Avatar();
        //         BitmapImage avatar = await avatarManager.GetAvatar(((ChatUser)list_users.SelectedItem).Username);
        //         user_image.Source = avatar;

        //         try
        //         {
        //             await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetOtherUserTraits, ((ChatUser)list_users.SelectedItem).Username);
        //             Response response = await GlobalVariables.GetClient().GetResponse();
        //             List<PersonalityTrait> traits = response.Value as List<PersonalityTrait>;
        //             //GlobalVariables.LogMessage("traits num: " + traits.Count);
        //             selectedUserPersonalityTraits.ItemsSource = traits;
        //             selectedUserPersonalityTraits.DisplayMemberPath = "TraitName";
        //         }
        //         catch (Exception ex)
        //         {
        //             //GlobalVariables.LogMessage("Error loading traits: " + ex.Message);
        //         }
        //     }
        // }

        private async void list_users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            while (!canRun) {  return; }
            canRun = false;
            //GlobalVariables.LogMessage("User selection changed");
            if (list_users.SelectedItem != null)
            {

                //GlobalVariables.LogMessage("setting avatar");

                Avatar avatarManager = new Avatar();
                BitmapImage avatar = await avatarManager.GetAvatar(((ChatUser)list_users.SelectedItem).Username);

                user_image.Source = avatar;

                try
                {
                    await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetOtherUserTraits, ((ChatUser)list_users.SelectedItem).Username);

                    Response response = await GlobalVariables.GetClient().GetResponse();



                    List<PersonalityTrait> traits = response.Value as List<PersonalityTrait>;
                    //GlobalVariables.LogMessage("traits num: " + traits.Count);
                    selectedUserPersonalityTraits.ItemsSource = traits;
                    selectedUserPersonalityTraits.DisplayMemberPath = "TraitName";
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error loading traits: " + ex.Message);
                    //GlobalVariables.LogMessage("Error loading traits: " + ex.Message);
                }
            }
            canRun = true;
        }


        public void AddTrait(PersonalityTrait trait)
        {
            if (listTraits.ItemsSource is List<PersonalityTrait> traits)
            {
                traits.Add(trait);
                // Refresh the ItemsSource to reflect changes
                listTraits.ItemsSource = null;
                listTraits.ItemsSource = traits;
            }
            else
            {
                // Initialize ItemsSource if it's null
                var newTraits = new List<PersonalityTrait> { trait };
                listTraits.ItemsSource = newTraits;
            }

        }

        //private void btn_add_trait_Click(object sender, MouseButtonEventArgs e)
        //{
        //    PersonalityTraits traits = new PersonalityTraits(this);
        //    traits.Show();
        //}

        private async void BtnSaveTraits_Click(object sender, RoutedEventArgs e)
        {
            UserPersonality userPersonality = new UserPersonality();
            userPersonality.UserId = SessionManager.Data.User.ChatUserId;
            userPersonality.Traits = listTraits.Items.Cast<PersonalityTrait>().ToList();

            PersonalityTraitsOp ptops = new PersonalityTraitsOp();
            await ptops.SaveUserHasPersonalityTraits(userPersonality);
        }




        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            
           
        }



        ContactsOp contactsOp = new ContactsOp();
        private async void BtnAddContact_Click(object sender, RoutedEventArgs e)
        {


            UserHasContact userHasContact = new UserHasContact();
            userHasContact.UserId = SessionManager.Data.User.ChatUserId;
            userHasContact.UserContactId = ((ChatUser)list_users.SelectedItem).ChatUserId;

            
            //GlobalVariables.LogMessage("UserId: " + userHasContact.UserId);
            //GlobalVariables.LogMessage("UserContactId: " + userHasContact.UserContactId);

         
            bool isMyContact = await contactsOp.IsUserMyContact((int)userHasContact.UserId, (int)userHasContact.UserContactId);

            try
            {

                if (!isMyContact)
                {

                    ChatUser newContact = new ChatUser();
                    newContact.ChatUserId = (int)userHasContact.UserContactId;
                    newContact.Username = ((ChatUser)list_users.SelectedItem).Username;

                    Avatar avatarManager = new Avatar();
                    BitmapImage avatar = await avatarManager.GetAvatar(((ChatUser)list_users.SelectedItem).Username);

                    newContact.AvatarImage = avatarManager.GetBytesFromBitmapImage(avatar);

                    await contactsOp.SaveUserAsContact(userHasContact);
                    if(list_contacts.ItemsSource is List<ChatUser> myContacts)
                    {
                        myContacts.Add(newContact);
                        list_contacts.ItemsSource = null;
                        list_contacts.ItemsSource = myContacts;
                    }
                    else
                    {
                        // Initialize ItemsSource if it's null
                        var newContacts = new List<ChatUser> { newContact };
                        listTraits.ItemsSource = newContacts;
                    }
                    await Notifier.NotifyContactsInChatWindow(newContact);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //MessageBox.Show("CONTACT ADDED");``
            //await RefreshContactsList();
            //MessageBox.Show("CONTACT LIST REFRESHED");

            //Refresh in Chat Window:
            //await Notifier.NotifyContactsInChatWindow(contactList);
        }


        List<ChatUser> contactList;
        public async Task RefreshContactsList()
        {
            //MessageBox.Show("Contacts list");
            contactList = await contactsOp.GetAllContacts(SessionManager.Data.User.ChatUserId);


            //Response res = await GlobalVariables.GetClient().GetResponse();
            //MessageBox.Show("Contacts in response");

            // List<ChatUser> contactsUsers = (List<ChatUser>)res.Value;
            list_contacts.ItemsSource = null;
            list_contacts.ItemsSource = contactList;
            list_contacts.DisplayMemberPath = "Username";
           // MessageBox.Show("Contacts list done");
        }

        private void BtnRemoveContact_Click(object sender, RoutedEventArgs e)
        {

        }
        


        private Timer? debounceTimer;
        private readonly object lockObject = new object();

        private async void txt_username_TextChanged(object sender, TextChangedEventArgs e)
        {

            //    if (txt_username.Text.IsNullOrEmpty())
            //    {
            //        var uri = new Uri("pack://application:,,,/GUI/wrong-100.png");
            //        check_username.Source = new BitmapImage(uri);
            //        return;
            //    }

            //    lock (lockObject)
            //    {
            //        // Cancel any previous timer
            //        debounceTimer?.Dispose();

            //        // Create new timer that will wait before making the server call
            //        debounceTimer = new Timer(async (state) =>
            //        {
            //            await Application.Current.Dispatcher.InvokeAsync(async () =>
            //            {
            try
            {
                //if user types nothing, show wrong icon
                if (string.IsNullOrEmpty(txt_username.Text))
                {
                    var uri = new Uri("/GUI/wrong_100.png", UriKind.Relative);
                    check_username.Source = new BitmapImage(uri);
                    return;
                }

                string username = txt_username.Text;

                if(SessionManager.Data.User.Username == username)
                {
                    var uri = new Uri("/GUI/purple check.png", UriKind.Relative);
                    check_username.Source = new BitmapImage(uri);
                    return;
                }


                Client client = GlobalVariables.GetClient();


                await client.SendCommandToServer(RegularUserAllowedCommands.CheckIfUsernameExists, new ChatUser() { Username = username });

                Response response = await GlobalVariables.GetClient().GetResponse();

                //MessageBox.Show("response from textchanged: " + response.Value.ToString());

                if (response.Value is bool)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ////GlobalVariables.LogMessage($"{DateTime.Now:HH:mm:ss} - Checked username: {username} - {response.Value}");
                        ////GlobalVariables.LogMessage("RESPONSE FROM CLIENT USERNAME TEXT CHANGED: count: "
                        //    + ((List<ChatUser>)response.Value).Count + ", first: " + ((List<ChatUser>)response.Value)[0].Username);
                        if ((bool)response.Value == true)
                        {

                            //check_username.Source = Properties.Resources.wrong_100;

                            //var uri = new Uri("/GUI/icons/wrong_100.png");
                            //check_username.Source = new BitmapImage(uri);

                            var uri = new Uri("/GUI/wrong_100.png", UriKind.Relative);

                            check_username.Source = new BitmapImage(uri);
                            check_username.Stretch = Stretch.Uniform; 
                        }
                        else
                        {
                            //check_username.Source = Properties.Resources.purple_check;

                            //var uri = new Uri("pack://application:,,,/GUI/purple check.png");
                            //check_username.Source = new BitmapImage(uri);

                            var uri = new Uri("/GUI/purple check.png", UriKind.Relative);

                            check_username.Source = new BitmapImage(uri);
                        }
                    });
                }
            }

            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //var uri = new Uri("pack://application:,,,/GUI/wrong-100.png");
                    //check_username.Source = new BitmapImage(uri);

                    MessageBox.Show("Error checking username.");
                    MessageBox.Show(ex.Message);
                    //GlobalVariables.LogMessage($"{DateTime.Now:HH:mm:ss} - Error: {ex.Message}{Environment.NewLine}");
                });
            }
                //});
            //        }, null, 150, Timeout.Infinite); // Waits 500ms before checking


            //    }
        }

        private void btn_add_trait_Click_1(object sender, RoutedEventArgs e)
        {
            PersonalityTraits traits = new PersonalityTraits(this);
            traits.Show();
        }
    }
}
