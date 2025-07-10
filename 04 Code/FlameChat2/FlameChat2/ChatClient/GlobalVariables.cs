using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FlameChatClient.ChatClient
{
    public class GlobalVariables
    {


        public static string ServerIP;
        
        //public TextBlock Test_messages { get; set; }

        // This is a singleton pattern to ensure only one instance of the client exists
        private static Client ThisClient { get; set; }

        public static Client GetClient()
        {
            if (ThisClient == null)
            {
                ThisClient = new Client();
            }
            return ThisClient;
        }



        //This method is used to get the existing client instance
        public static Client GetExistingClient()
        {
            return ThisClient;
        }


        
        public static ListBox MessagesFromClient { get; set; }

        public static void LogMessage(string message)
        {
            if (MessagesFromClient == null)
            {
                MessageBox.Show("ListBox for messages is not set.");
                MessageBox.Show("Message is: " + message);
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Code that updates UI goes here
                MessagesFromClient.Items.Add($"Server:  {DateTime.Now}: {message}" + Environment.NewLine);
            });

        }
    }

        //GlobalVariables.LogMessage(
    }
