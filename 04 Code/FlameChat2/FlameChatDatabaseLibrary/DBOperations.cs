using FlameChatDatabaseLibrary.DB;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using FlameChatDatabaseLibrary.Encryption;
using SharedModel.Models;

namespace FlameChatDatabaseLibrary
{
    public class DBOperations
    {
        public DBOperations() { }

        public void AddChatUser(string username, string password, Byte[] avatarImage)
        {

            using (var context = DBContext.FlameDBContext)
            {
                var chatUser = new ChatUser
                {
                    Username = username,
                    Password = password,
                    AvatarImage = avatarImage

                };
                context.ChatUsers.Add(chatUser);
                context.SaveChanges();
            }

        }
        public void LogIn(ChatUser user)
        {
            if (DBContext.FlameDBContext.ChatUsers.Where(x => x.Username == user.Username && x.Password == PasswordHasher.HashPassword(user.Password)).Count() == 1)
            {
                ChatUser loggedInUser = DBContext.FlameDBContext.ChatUsers.Where(x => x.Username == user.Username && x.Password == PasswordHasher.HashPassword(user.Password)).FirstOrDefault();
                if (loggedInUser != null)
                {
                    user.ChatUserId = loggedInUser.ChatUserId;
                    user.AvatarImage = loggedInUser.AvatarImage;
                    user.Username = loggedInUser.Username;
                    user.Password = loggedInUser.Password;
                }
            }


            byte[] image = user.AvatarImage;

            BitmapImage imageSource = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(image))
            {
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
            }

            //SaveSessionDataToClient(user);
            //OpenChat();
           // admin_profile_avatar.Source = imageSource;
        }

 
    }
}
