using FlameChatDatabaseLibrary;
using FlameChatDatabaseLibrary.DB;
using FlameChatAdmin.ServerMainThread;
using FlameChatAdmin.ServerOperations;
using Microsoft.Win32;
using SharedModel.Models;
using System;
using System.Collections.Generic;
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

namespace FlameChatAdmin
{
    /// <summary>
    /// Interaction logic for ServerMessages.xaml
    /// </summary>
    public partial class ServerMessages : Window
    {
        public ServerMessages()
        {
            InitializeComponent();

        }

        Server server;
        private async void btnserver_Click(object sender, RoutedEventArgs e)
        {
            server = new Server(servermessages);
            await server.StartServer();

            

        }

        //string imageFilename = "";
        //private void test_db_btn_choose_image_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog dialog = new OpenFileDialog();
        //    dialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
        //    if (dialog.ShowDialog() == true)
        //    {
        //        imageFilename = dialog.FileName;
        //    }
       

        //}

        //private void db_btn_test_image_Click(object sender, RoutedEventArgs e)
        //{

        //    byte[] imagebytes = DBContext.FlameDBContext.ChatUsers.Where(x=>x.ChatUserId==1007).FirstOrDefault().AvatarImage;
        //    File.WriteAllBytes("testimage.jpg", imagebytes);
        //    // Load the image from the file
        //    string imageFilename = "testimage.jpg";
        //    string imagePath = System.IO.Path.GetFullPath(imageFilename);

        //    //string imageUri = new Uri(imageFilename, UriKind.Relative).ToString();
        //    //MessageBox.Show(imagePath);
           
        //    db_test_image.Source = new BitmapImage(new Uri(imagePath));
        //}

        //private async void test_btn_insert_user_Click(object sender, RoutedEventArgs e)
        //{

        //    ChatUser user = new ChatUser()
        //    {
        //        Username = "TEST2",
        //        Password = "test2",
        //        AvatarImage = File.ReadAllBytes(imageFilename)
        //    };

        //    ServerDbOperations dbOperations = new ServerDbOperations(server);
        //    await dbOperations.SignUp(user);

        //    //DBContext.FlameDBContext.ChatUsers.Add(user);
        //    //DBContext.FlameDBContext.SaveChanges();
        //}

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            server = new Server(servermessages);
            await server.StartServer();

            server.LogMessage("Server started successfully.");
        }

        private void ServerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void ServerWindow_Closed(object sender, EventArgs e)
        {
            server.StopServer();
        }
    }
}
