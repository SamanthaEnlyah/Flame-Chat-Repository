using FlameChatClient.GUI;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlameChatClient.Notifications
{  
    public class Notifier
    {
        public static ChatWindow ChatWindow { get; set; }
        public static UserInfo UserInfo { get; set; }

        public static async Task NotifyMessageInChatWindow(Message message)
        {
            // Display the notification in the chat window
            ChatWindow.Dispatcher.Invoke(() =>
            {
                PlayNotificationSound();
                ChatWindow.AddMessageToList(message);
            });
        }

        public static async Task NotifyContactsInChatWindow(List<ChatUser> users)
        {
            // Display the notification in the chat window
            ChatWindow.Dispatcher.Invoke(() =>
            {

                ChatWindow.SetContacts(users);

            });
        }

        public static async Task NotifyContactsInChatWindow(ChatUser user)
        {
            // Display the notification in the chat window
            ChatWindow.Dispatcher.Invoke(() =>
            {

                ChatWindow.AddContact(user);

            });
        }

        public static async Task NotifyUser(string value)
        {
            ChatWindow.Dispatcher.Invoke(() =>
            {

                ChatWindow.ShowMessageFromServer(value);


            });
        }

        public static void PlayNotificationSound()
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.message_pop);
            player.Play();

            //    MediaPlayer player = new MediaPlayer();
            ////

            //    player.Open(new Uri("/Res/Audio/message_pop.wav", UriKind.Relative));
            //    player.Play();
        }

        //public static async Task NotifyContactsInUserInfo()
        //{
        //    await UserInfo.Dispatcher.Invoke(async () =>
        //    {

        //        await UserInfo.RefreshContactsList();

        //    });
        //}



    }
}
