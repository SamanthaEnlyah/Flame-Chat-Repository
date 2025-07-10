using FlameChatClient.ChatClient;
using FlameChatClient.Encryption;
using FlameChatShared.Communication;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace FlameChatClient.Client_Operations
{
    public class SessionManager
    {

        public static SessionData Data = new SessionData();

    

        public static async Task<bool> StartSession(string username, string password)
        {
            //MessageBox.Show("entered startsession");
            if (File.Exists("session/session.json"))
            {
                File.Delete("session/image_profile");
                File.Delete("session/session.json");
            }
            try
            {
                await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.NotifyServerAboutMyUsername, username);


                //MessageBox.Show("starting session");
                await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserId, username);
                Response userId = await GlobalVariables.GetClient().GetResponse();
                ////GlobalVariables.LogMessage((int)userId.Value);


                await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserAvatar, (int)userId.Value);
                Response response = await GlobalVariables.GetClient().GetResponse();

                byte[] avatar = response.Value as byte[];
                 
                password = PasswordHasher.HashPassword(password);
                Data = new SessionData()
                {
                    User = new ChatUser()
                    {
                        ChatUserId = (int)(userId.Value),
                        Username = username,
                        Password = password,
                        AvatarImage = avatar 
                    },
                    Time = DateTime.Now
                };

                string sessionSerialized = JsonSerializer.Serialize(Data);
                if (!Directory.Exists("session"))
                {
                    Directory.CreateDirectory("session");
                }
                File.WriteAllText("session/session.json", sessionSerialized);
                //File.WriteAllBytes("session/image_profile", SessionManager.Data.User.AvatarImage);
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file access issues)
                MessageBox.Show($"Error during session start: {ex.Message}");
                return false;

            }
        }

        public static bool IsSessionStarted(string username, string password)
        {
            if (File.Exists("session/session.json") == true)
            {
                try
                {

                    password = PasswordHasher.HashPassword(password);

                    string json = File.ReadAllText("session/session.json");
                    Data = JsonSerializer.Deserialize<SessionData>(json);
                    string existingUsername = Data.User.Username;
                    string existingPassword = Data.User.Password;

                   //Data.User.AvatarImage = File.ReadAllBytes("session/image_profile");
                    if (username == existingUsername && password == existingPassword)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., file access issues)
                    MessageBox.Show($"Error during session check: {ex.Message}");
                    return false;
                }

            }
            else return false;
            
        }

        public static bool IsSessionStarted()
        {
            if(File.Exists("session/session.json"))
            {
                string json = File.ReadAllText("session/session.json");
                Data.User = JsonSerializer.Deserialize<SessionData>(json).User;
                Data.Time = JsonSerializer.Deserialize<SessionData>(json).Time;
                
                //Data.User.AvatarImage = File.ReadAllBytes("session/image_profile");
                //MessageBox.Show("Session started: " + Data.User.Username + " " + Data.Time.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
        public async static Task<bool> LoadSession()
        {
            if (File.Exists("session/session.json"))
            {
                string json = File.ReadAllText("session/session.json");
                Data.User = JsonSerializer.Deserialize<SessionData>(json).User;
                Data.Time = JsonSerializer.Deserialize<SessionData>(json).Time;

                await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.NotifyServerAboutMyUsername, Data.User.Username);

                //Data.User.AvatarImage = File.ReadAllBytes("session/image_profile");
                // MessageBox.Show("Session continued: " + Data.User.Username + " " + Data.Time.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
