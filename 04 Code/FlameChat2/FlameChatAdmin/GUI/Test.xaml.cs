using FlameChatClient.ChatClient;
using System;
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

namespace FlameChatAdmin.GUI
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
        }

        public ListBox Messages
        {
            get
            {
                return _test_messages;
            }
            private set
            {
               
            }
        }         


        private async void btnclient_Click(object sender, RoutedEventArgs e)
        {
            //Client client = new Client();
            // client.StartClient();
           // client.SendCommandToServer(RegularUserAllowedCommands.CheckIfUsernameExists, "testuser");
        }
    }
}
