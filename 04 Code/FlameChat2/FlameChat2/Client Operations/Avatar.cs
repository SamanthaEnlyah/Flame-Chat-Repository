using FlameChatClient.ChatClient;
using FlameChatShared.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlameChatClient.Client_Operations
{
    public class Avatar
    {

        public async Task<int> GetUserId(string username)
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserId, username);
            Response userId = await GlobalVariables.GetClient().GetResponse();
            return (int)userId.Value;
        }


        public async Task<BitmapImage> GetAvatar(string username)
        {
            int userId = await GetUserId(username);

            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserAvatar, userId);
            Response response = await GlobalVariables.GetClient().GetResponse();
            if (response != null)
            {
                if (response.Value != null)
                {
                    byte[] avatar = response.Value as byte[];



                    return CreateBitmapImageFromBytes(avatar);
                }
            }
            else
            {
                throw new Exception("Failed to retrieve avatar from server.");

            }
            return new BitmapImage();

            //set image
            //File.WriteAllBytes("other_user_avatar", avatar);
            //string path = System.IO.Path.GetFullPath("other_user_avatar");
            //return new BitmapImage(new Uri(path));
        }

        public BitmapImage CreateBitmapImageFromBytes(byte[] imageData)
        {
            var ms = new MemoryStream(imageData);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }


        public byte[] GetBytesFromBitmapImage(BitmapImage bitmapImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
