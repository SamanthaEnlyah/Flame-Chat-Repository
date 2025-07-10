using FlameChatClient.ChatClient;
using FlameChatClient.Encryption;
using FlameChatShared.Communication;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FlameChatClient.Client_Operations
{
    public class RegularUserLogIn
    {
         Client client;

        public RegularUserLogIn() 
        {
            this.client = GlobalVariables.GetClient();
            
        }


        public async Task<bool> LogIn(string username, string password)
        {

            if(IsLoggedIn(username, password))
            {
                // User is already logged in
                MessageBox.Show(username + " is already logged in.");

                return true;
            }else
            {
                // User is not logged in
                // Perform login operation
                //MessageBox.Show(username + " logging in, please wait...");
                return await PerformLogin(username, password);
            }
            
        }

        public async Task<bool> PerformLogin(string username, string password)
        {
            //MessageBox.Show(username + "in perform login");
            try
            {
                // Create a new instance of the client
                //Client client = new Client(_test_clientMessagesGUI);
                //MessageBox.Show("try login");

                password = PasswordHasher.HashPassword(password);

              

                // Send the login command to the server
                await client.SendCommandToServer(RegularUserAllowedCommands.LogIn, new ChatUser { Username = username, Password = password });

                
                Response r = await client.GetResponse();
                bool usernameAndPassCorrect = (bool) r.Value;

                // Check if the response is successful
                //MessageBox.Show("COMMAND LOGIN SENT AND AWAITED");
                //MessageBox.Show("response is usernameAndPassCorrect:" + usernameAndPassCorrect);
           
                if (!usernameAndPassCorrect)                  
                {
                    // Login failed
                    //GlobalVariables.LogMessage("Login failed. Invalid username or password.");
                }
                return usernameAndPassCorrect;

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues)
                MessageBox.Show($"Error during login: {ex.Message}");
                return false;
            }
        }

        public bool IsLoggedIn()
        {
            return SessionManager.IsSessionStarted();


        }

        private bool IsLoggedIn(string username, string password)
        {
            return SessionManager.IsSessionStarted(username, password);


        }
    }
}

        //    cookie.Expires = DateTime.Now.AddMinutes(30);
        //    var cookie = new HttpCookie("session");