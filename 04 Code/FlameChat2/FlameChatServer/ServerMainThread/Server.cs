using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FlameChatAdmin.ServerOperations;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using FlameChatDatabaseLibrary.DB;
using FlameChatShared.Communication;
using SharedModel.Models;
using SharedModel.SharedClasses;
using FlameChatAdmin.Exceptions;
using System.Configuration;
using SharedModel.ComplexModels;
using FlameChatAdmin.GUIDialogs;
using System.Dynamic;
using Azure.Identity;

namespace FlameChatAdmin.ServerMainThread
{
    public class Server
    {
        Socket? listener = null;

        private Message userMessage;

        private UserIdSocket clientHandler = new UserIdSocket();

        private Object response;

        //private bool responseBool = false;

        private Command commandObject;

        private ServerDbOperations serverDbOperations;

        private ListBox serverMessagesGUI;

        private readonly object clientsLock = new object();

        //private readonly List<Socket> clients = new List<Socket>();

        private static List<UserIdSocket> clients = new List<UserIdSocket>();

        private bool running = true;

        public Server(ListBox messagesGui)
        {
            serverMessagesGUI = messagesGui;
            serverDbOperations = new ServerDbOperations(this);
        }

        public void LogMessage(string message)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Code that updates UI goes here
                serverMessagesGUI.Items.Add($"Server:  {DateTime.Now}: {message}" + Environment.NewLine);
            });


        }

        public void BroadcastServerIPAddress()
        {
            Socket broadcastAdapter;
                var echoBytesReceiver = Encoding.UTF8.GetBytes(GlobalVariables.ServerIP.ToString());

                IPAddress broadcastIpAddress = NetworkUtils.GetBroadcastAddress(GlobalVariables.ServerIP, NetworkUtils.GetSubnetMask(GlobalVariables.ServerIP));
                IPEndPoint ipEndPoint = new(broadcastIpAddress, 11_001);


                 broadcastAdapter = new(
                        ipEndPoint.AddressFamily,
                        SocketType.Dgram,
                        ProtocolType.Udp);




            while (running)
            {
                try
                {
                    broadcastAdapter.SendToAsync(new ArraySegment<byte>(echoBytesReceiver), SocketFlags.None, ipEndPoint);

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    LogMessage($"Error broadcasting server IP: {ex.Message}");
                }
            }
            
           
        }

        int test_id = 0;
        
        public async Task StartServer()
        {


            // Initialize server components
            running = true;

            listener = null;

            try { 
            var hostName = Dns.GetHostName();
                LogMessage(hostName);

            IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
            // This is the IP address of the local machine



            //IPAddress localIpAddress = localhost.AddressList[0];

                IpAddressChooser ipAddressChooser = new IpAddressChooser();
                ipAddressChooser.Title = "Server IP Chooser";
                ipAddressChooser.listIPs.ItemsSource = localhost.AddressList.ToList().Where(ip => ip.AddressFamily != AddressFamily.InterNetworkV6);
                if (ipAddressChooser.ShowDialog() == true)
                {
                    GlobalVariables.ServerIP = ipAddressChooser.ChosenServerIPAddress;
                    DBContext.SQLServerExpressInstanceID = ipAddressChooser.SQLServerExpressInstanceID;
                    MessageBox.Show("Server IP: " + GlobalVariables.ServerIP.ToString());   
                }


                //foreach (var ip in localhost.AddressList)
                //{
                //    LogMessage("IP: " + ip.ToString());
                //}



                IPEndPoint ipEndPoint = new(GlobalVariables.ServerIP, 11_000);

            listener = new(
               ipEndPoint.AddressFamily,
               SocketType.Stream,
               ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);
          

                LogMessage("Server started");
                LogMessage("Server IP: " + GlobalVariables.ServerIP.ToString());



                LogMessage("starting broadcast");
                ThreadStart ts = new ThreadStart(BroadcastServerIPAddress);
                Thread broadcastThread = new Thread(ts);
                broadcastThread.Start();



                // Accept incoming connections
                LogMessage("Waiting for client connection...");

                while (running)
                {
                    try
                    {
                        clientHandler.Client = await listener.AcceptAsync();

                        //var buffer = new byte[1024];
                        //var received = await clientHandler.Client.ReceiveAsync(buffer, SocketFlags.None);
                        //clientHandler.UserId = Int32.Parse(Encoding.UTF8.GetString(buffer, 0, received)); 

                        LogMessage("Client connected");


                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await HandleClientAsync();
                                }
                                catch (Exception ex)
                                {
                                    LogMessage($"Error in client handler task: {ex.Message}");
                                }
                            });

                        }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error accepting client: {ex.Message}");
                        // Maybe add a short delay before trying to accept new connections
                        await Task.Delay(1000);
                    }
                }
            }
            finally
            {
                // Clean up the listener socket when the server is actually stopping
                if (listener != null)
                {
                    try
                    {
                        listener.Close();
                    }
                    catch { } // Ignore errors during cleanup
                }
            }

        }


        public async Task HandleClientAsync()
        {
            // Add client to list
            UserIdSocket newClient;
            lock (clientsLock)
            {
                newClient = new UserIdSocket()
                {
                    Client = clientHandler.Client
                };
                clients.Add(newClient);
                
            }


            while (true)
            {

                try
                {
                   
                    //iCalledSendCommandToServer
                    //responseBool = false;



                    //MessageBox.Show("Server: received command from client: ");

                    // Receive command.
                    var buffer = new byte[30000000];
                    var received = await newClient.Client.ReceiveAsync(buffer, SocketFlags.None);


                    if (received == 0)
                    {
                        LogMessage("Client disconnected gracefully");
                        break; // Exit inner loop to accept new connections
                    }


                    string commandString = Encoding.UTF8.GetString(buffer, 0, received);
                    LogMessage("command: " + commandString);

                    if (/*commandString.StartsWith("<|BOM|>") && */ commandString.EndsWith("<|EOM|>"))
                    {

                        List<string> commands = commandString.Split("<|EOM|>").ToList();

                        commands.RemoveAt(commands.Count - 1);
                        // LogMessage("Server: command: " + commands);

                        foreach (string c in commands)
                        {

                            LogMessage("Server: command: " + c);




                            try
                            {
                                commandObject = JsonSerializer.Deserialize<Command>(c);
                            }
                            catch (JsonException ex)
                            {
                                throw new ServerException("Server: Error deserializing command: " + ex.Message);
                                //continue;
                            }
                            catch (Exception e)
                            {
                                throw new ServerException($"Error deserializing command: {e.Message}");
                                //continue
                            }
                            try
                             {
                                
                                LogMessage("IS ADMIN: " + commandObject.IsAdmin);
                                if (!commandObject.IsAdmin)
                                {
                                    RegularUserAllowedCommands command = commandObject.RegularCommand;
                                    LogMessage("SERVER: regular user sending command: " + command.ToString());

                                    switch (command)
                                    {
                                        case RegularUserAllowedCommands.CheckIfUsernameExists:
                                            {
                                                LogMessage("Server: checking if username exists");
                                                string username = ((JsonElement)commandObject.Value).GetProperty("Username").GetString();
                                                //MessageBox.Show("server, check username exists: " + username);

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.CheckIfUsernameExists,
                                                    Value = await serverDbOperations.CheckIfUsernameExists(username),
                                                    IsAdmin = false

                                                };

                                                LogMessage("Server: response: username exists: " + response);

                                                response = responseCommand;

                                                break;
                                            }
                                        case RegularUserAllowedCommands.NotifyServerAboutMyUsername:
                                            {
                                                string username = ((JsonElement)commandObject.Value).GetString();

                                                
                                                int id = await serverDbOperations.GetUserId(username);
                                                newClient.UserId = id;


                                                //MessageBox.Show("CLIENT USER ID: " + newClient.UserId);

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.None,
                                                    Value = ""
                                                };


                                                response = responseCommand;


                                                break;
                                            }
                                        case RegularUserAllowedCommands.SignUp:
                                            {
                                                LogMessage("Server: SIGN UP");
                                                byte[]? bytes = new byte[30000000];
                                                ((JsonElement)commandObject.Value).GetProperty("AvatarImage").TryGetBytesFromBase64(out bytes);
                                                ChatUser user = new ChatUser()
                                                {
                                                    Username = ((JsonElement)commandObject.Value).GetProperty("Username").GetString(),
                                                    Password = ((JsonElement)commandObject.Value).GetProperty("Password").GetString(),
                                                    AvatarImage = bytes

                                                };

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.SignUp,
                                                    Value = await serverDbOperations.SignUp(user),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                //MessageBox.Show("Server: user: " + user.Username);
                                                break;
                                            }
                                        case RegularUserAllowedCommands.LogIn:
                                            {
                                                LogMessage("Server: LOG IN");
                                                ChatUser user = new ChatUser()
                                                {
                                                    Username = ((JsonElement)commandObject.Value).GetProperty("Username").GetString(),
                                                    Password = ((JsonElement)commandObject.Value).GetProperty("Password").GetString()


                                                };

                                                

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.LogIn,
                                                    Value = await serverDbOperations.LogIn(user),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand; 
                                                LogMessage("Server: LOG IN ended");
                                                break;
                                            }

                                        case RegularUserAllowedCommands.LogOut:
                                            {
                                                lock (clientsLock)
                                                {
                                                    clients.Remove(newClient);
                                                }

                                                response = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.None,
                                                    IsAdmin = false,
                                                    Value = ""
                                                };


                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetOtherUsers:
                                            {
                                                LogMessage("Server: GET OTHER USERS");
                                                string exceptUsername = ((JsonElement)(commandObject.Value)).GetString();

                                                response = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetOtherUsers,
                                                    IsAdmin = false,
                                                    Value = await serverDbOperations.GetOtherUsers(exceptUsername)
                                                };
                                            

                                                break;
                                            }
                                        case RegularUserAllowedCommands.GetContacts:
                                            {
                                                LogMessage("Server: GET CONTACTS");



                                                int userId = ((JsonElement)(commandObject.Value)).GetInt32();


                                                response = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetContacts,
                                                    IsAdmin = false,
                                                    Value = await serverDbOperations.GetContacts(userId)
                                                };

                                                

                                                break;
                                            }

                                        //case RegularUserAllowedCommands.GetChatHistory:
                                        //    {
                                        //        LogMessage("Server: GET CHAT HISTORY");
                                        //        int userId = ((JsonElement)(commandObject.Value)).GetInt32();



                                        //        Command responseCommand = new Command()
                                        //        {
                                        //            RegularCommand = RegularUserAllowedCommands.GetChatHistory,
                                        //            Value = await serverDbOperations.GetChatHistory(userId),
                                        //            IsAdmin = false
                                        //        };


                                        //        response = responseCommand;

                                        //        break;
                                        //    }

                                        case RegularUserAllowedCommands.GetAllMessagesForUser:
                                            {
                                                LogMessage("Server: GETTING user messages");
                                                int userId = ((JsonElement)(commandObject.Value)).GetInt32();


                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetAllMessagesForUser,
                                                    Value = await serverDbOperations.GetAllMessagesForUserId(userId),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetUnseenMessagesCount:
                                            {
                                                LogMessage("Server: GET UNSEEN MESSAGES COUNT");
                                                int userId = ((JsonElement)(commandObject.Value)).GetInt32();
                                                response = await serverDbOperations.GetUnseenMessagesCount(userId);


                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetUnseenMessagesCount,
                                                    Value = await serverDbOperations.GetUnseenMessagesCount(userId),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetUserId:
                                            {
                                                LogMessage("Server: GET USER ID");
                                                string username = ((JsonElement)(commandObject.Value)).GetString();

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetUserId,
                                                    Value = await serverDbOperations.GetUserId(username),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;


                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetUsernameFromId:
                                            { 
                                            
                                                LogMessage("Server: GET USERNAME");
                                                int id = ((JsonElement)(commandObject.Value)).GetInt32();

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetUsernameFromId,
                                                    Value = await serverDbOperations.GetUsernameFromId(id),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;


                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetUserFromId:
                                            {

                                                LogMessage("Server: GET USER");
                                                int id = ((JsonElement)(commandObject.Value)).GetInt32();

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetUserFromId,
                                                    Value = await serverDbOperations.GetUserFromId(id),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                //MessageBox.Show("response is type of: " + response.GetType());
                                                break;
                                            }

                                        case RegularUserAllowedCommands.GetUserAvatar:
                                            {
                                                LogMessage("Server: GET USER AVATAR");
                                                int userid = ((JsonElement)(commandObject.Value)).GetInt32();

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetUserAvatar,
                                                    Value = await serverDbOperations.GetUserAvatar(userid),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                break;
                                            }
                                        case RegularUserAllowedCommands.SendMessage:
                                            {
                                                LogMessage("Server: SEND MESSAGE");


                                                //test_id = 1132;
                                                //Test();


                                                userMessage = JsonSerializer.Deserialize<Message>(commandObject.Value.ToString());
                                              
                                                await serverDbOperations.InsertMessage(userMessage);


                                                SendToAnotherUser(userMessage);


                                                //response je iCalledSendCommandToServer
                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.SendMessage,
                                                    Value = true,
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;
                                                //responseBool = true;
                                                
                                                break;

                                                

                                                //Message message = JsonSerializer.Deserialize<Message>(commandObject.Value.ToString());
                                                //response = await serverDbOperations.SendMessage(message);

                                            }
                                        case RegularUserAllowedCommands.GetOtherUserTraits:
                                            {

                                                LogMessage("Server: GET OTHER USER TRAITS");
                                                string username = ((JsonElement)(commandObject.Value)).GetString();

                                                if (!string.IsNullOrEmpty(username))
                                                {
                                                    Command responseCommand = new Command()
                                                    {
                                                        RegularCommand = RegularUserAllowedCommands.GetOtherUserTraits,
                                                        Value = await serverDbOperations.GetPersonalityTraits(((JsonElement)(commandObject.Value)).GetString()),
                                                        IsAdmin = false
                                                    };


                                                    response = responseCommand;
                                                }
                                                else
                                                {
                                                    LogMessage("Invalid or null username provided for GetOtherUserTraits command.");
                                                    Command responseCommand = new Command()
                                                    {
                                                        RegularCommand = RegularUserAllowedCommands.None,
                                                        Value = new List<PersonalityTrait>(),
                                                        IsAdmin = false

                                                    };
                                                    // Return an empty list or handle as needed
                                                    response = responseCommand;
                                                }
                                                break;

                                            }

                                        case RegularUserAllowedCommands.IsUserMyContact:
                                            {
                                               // MessageBox.Show("IS USER MY CONTACT on Server, command object value: " + commandObject.Value.ToString());
                                                UserHasContact uhc = JsonSerializer.Deserialize<UserHasContact>(commandObject.Value.ToString());

                                                //MessageBox.Show("Server:  user has contact: " + uhc.UserId);
                                                Command responseCommand;
                                                
                                                
                                                if (uhc.UserId == null || uhc.UserContactId == null)
                                                {
                                                    responseCommand = new Command()
                                                    {
                                                        RegularCommand = RegularUserAllowedCommands.None,
                                                        Value = null,
                                                        IsAdmin = false
                                                    };
                                                    response = responseCommand;
                                                    break;
                                                }
                                                    
                                                responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.IsUserMyContact,
                                                    Value = await serverDbOperations.IsUserMyContact((int)uhc.UserId, (int)uhc.UserContactId),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;

                                                break;
                                            }

                                        case RegularUserAllowedCommands.SaveUserAsContact:
                                            {
                                                UserHasContact uhc = JsonSerializer.Deserialize<UserHasContact>(commandObject.Value.ToString());


                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.SaveUserAsContact,
                                                    Value= await serverDbOperations.SaveUserAsContact((int)uhc.UserId, (int)uhc.UserContactId),
                                                    IsAdmin = false
                                                };


                                                response = responseCommand;
                                              
                                                break;
                                            }
                                        case RegularUserAllowedCommands.SetMyPersonalityTraits:
                                            {
                                                LogMessage("Server: SET MY PERSONALITY TRAITS");
                                                UserPersonality personality =  JsonSerializer.Deserialize<UserPersonality>(commandObject.Value.ToString());
                                                

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.SetMyPersonalityTraits,
                                                    Value = await serverDbOperations.SetMyPersonalityTraits(personality),
                                                    IsAdmin = false

                                                };


                                                response = responseCommand;

                                                break;
                                            }
                                        case RegularUserAllowedCommands.GetMyPersonalityTraits:
                                            {
                                               // MessageBox.Show("Server.cs, getmypersonalitytraits");
                                                int myid = JsonSerializer.Deserialize <int>(commandObject.Value.ToString());

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetMyPersonalityTraits,
                                                    Value = await serverDbOperations.GetMyPersonalityTraits(myid),
                                                    IsAdmin = false

                                                };


                                                response = responseCommand;


                                                break;
                                            }
                                        case RegularUserAllowedCommands.GetAllPersonalityTraits:
                                            {

                                                Command responseCommand = new Command()
                                                {
                                                    RegularCommand = RegularUserAllowedCommands.GetAllPersonalityTraits,
                                                    Value = await serverDbOperations.GetAllPersonalityTraits(),
                                                    IsAdmin = false

                                                };


                                                response = responseCommand;

                                                break;
                                            }
                                        

                                    }

                                     
                                }
                                else if (commandObject.IsAdmin)
                                {
                                    AdminUserAllowedCommands command = commandObject.AdminCommand;


                                    switch (command)
                                    {
                                        case AdminUserAllowedCommands.PersonalityTraitExists:
                                            {

                                                response = await serverDbOperations.PersonalityTraitExists(commandObject.Value.ToString());
                                                break;
                                            }
                                     
                                        case AdminUserAllowedCommands.AddPersonalityTrait:
                                            {

                                                response = await serverDbOperations.AddPersonalityTrait(commandObject.Value.ToString());

                                                break;
                                            }
                                        case AdminUserAllowedCommands.DeletePersonalityTrait:
                                            {

                                                break;
                                            }
                                        case AdminUserAllowedCommands.AddManyPersonalityTraits:
                                            {
                                                if (commandObject.Value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                                                {
                                                    var traits = JsonSerializer.Deserialize<List<string>>(jsonElement.GetRawText());
                                                    if (traits != null)
                                                    {
                                                        response = await serverDbOperations.AddManyPersonalityTraits(traits);
                                                        
                                                    }
                                                    else
                                                    {
                                                        LogMessage("Server: Failed to deserialize traits list.");
                                                        response = false;
                                                    }
                                                }
                                                else
                                                {
                                                    LogMessage("Server: Invalid data format for AddManyPersonalityTraits command.");
                                                    response = false;
                                                }
                                                break;
                                            }

                                        case AdminUserAllowedCommands.EditPersonalityTrait:
                                            {
                                                break;
                                            }

                                        case AdminUserAllowedCommands.GetAllPersonalityTraits:
                                            {
                                                response = await serverDbOperations.GetAllPersonalityTraits();
                                                break;
                                            }
                                        case AdminUserAllowedCommands.GetAllUsers:
                                            {

                                                break;
                                            }
                                        case AdminUserAllowedCommands.HowManyMessagesSent:
                                            {
                                                break;
                                            }
                                        case AdminUserAllowedCommands.HowManyMessagesReceived:
                                            {
                                                break;
                                            }
                                        case AdminUserAllowedCommands.HowManyRegularUsers:
                                            {
                                                break;
                                            }
                                        case AdminUserAllowedCommands.HowManyMessagesSentInLast30Days:
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                LogMessage("case default");
                                                break;
                                            }
                                            //    throw new ArgumentException("Invalid report type");
                                    }

                                }
                                else
                                {
                                    LogMessage("Server: Invalid command type");
                                    break;
                                }


                             

                                string endMessage = "<|EOM|>";

                                //sending response to client
                                string json = JsonSerializer.Serialize(response);

                                string message = json + endMessage;

                                LogMessage("Server, json response: " + message);

                                //  MessageBox.Show("Server: sending response to client, json: " + json );
                                var echoBytes = Encoding.UTF8.GetBytes(message);

                                //MessageBox.Show("send to client:"+message);
                                await newClient.Client.SendAsync(echoBytes, 0);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($" {ex.Message}");
                               

                            }
                        }

                    }
                      else
                    {
                        MessageBox.Show("Server: Invalid message format:" + commandString);
                        break;
                    }

                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        MessageBox.Show("Client disconnected unexpectedly");
                    }
                    else
                    {
                        MessageBox.Show($"Socket error: {se.Message}");
                    }
                    break; // Exit inner loop to accept new connections
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing message: {ex.Message}");
                    break;
                }
                finally
                {
                    // If we're breaking out of the loop, clean up the connection
                    if (!newClient.Client.Connected)
                    {
                        try
                        {
                            lock (clientsLock)
                            {
                                clients.Remove(newClient);
                            }
                            newClient.Client.Shutdown(SocketShutdown.Both);
                            newClient.Client.Close();
                        }
                        catch { } // Ignore errors during cleanup
                    }
                }
            }
        }

        private async void SendToAnotherUser(Message userMessage)
        {

           // MessageBox.Show("FROM: " + userMessage.FkUserWhoSentId + ", TO: " + userMessage.FkWhoReceivedId + ", TEXT: " + userMessage.Text);

            string endMessage = "<|EOM|>";

            Command command = new Command()
            {
                IsAdmin = false,
                RegularCommand = RegularUserAllowedCommands.ReceiveMessage,
                Value = userMessage
            };

            string json = JsonSerializer.Serialize(command);

            string message = json + endMessage;

            LogMessage("Server, json receive: " + message);

            //  MessageBox.Show("Server: sending response to client, json: " + json );
            var echoBytes = Encoding.UTF8.GetBytes(message);

            var clientReceiver = FindReceiverClientHandler((int)userMessage.FkWhoReceivedId);


            //CHECK IF RECEIVER IS CONNECTED TO SERVER. IF NOT, SEND A MESSAGE TO SENDER THAT RECEIVER IS NOT LOGGED IN
            if (clientReceiver.Client == null)
            {
               // MessageBox.Show("Server: Receiver client is null, user is not logged in, sending message to sender.");
                //!!!!  Send an answer to the sender that receiver is not logged in !!!!

                Command responseCommand = new Command()
                {
                    RegularCommand = RegularUserAllowedCommands.MessageFromServer,
                    Value = "User is not logged in",
                    IsAdmin = false

                };


                string jsonAnswer = JsonSerializer.Serialize(responseCommand);

                string messageAnswer = jsonAnswer + endMessage;

                LogMessage("Server, json response: " + messageAnswer);

                //  MessageBox.Show("Server: sending response to client, json: " + json );
                var echoBytesAnswer = Encoding.UTF8.GetBytes(messageAnswer);

                var clientSender = FindReceiverClientHandler((int)userMessage.FkUserWhoSentId);

                //MessageBox.Show("send to client:"+message);
                await clientSender.Client.SendAsync(echoBytesAnswer, 0);
               // MessageBox.Show("server message sent");
                
                return;
            }

            
            await clientReceiver.Client.SendAsync(echoBytes, 0);
         
        }

        public UserIdSocket FindReceiverClientHandler(int receiverID)
        {
            //string data = "";
            //foreach(UserIdSocket socketUser in clients)
            //{
            //    data += socketUser.UserId + ", ";
            //}
            //MessageBox.Show("num of clients: "  + clients.Count + ", user client ids: " + data);

            foreach (UserIdSocket userSocket in clients)
            {
                if (userSocket.UserId == receiverID)
                {
                    //MessageBox.Show("found receiving client: " + userSocket.UserId);
                    //MessageBox.Show("Remote End Point" + userSocket.Client.RemoteEndPoint.ToString() + ", userID: " + userSocket.UserId );
                    return userSocket;
                }
            }
            return new UserIdSocket();
        }

        public async void Test()
        {

            var clientReceiver = FindReceiverClientHandler(test_id);
            await clientReceiver.Client.SendAsync(Encoding.UTF8.GetBytes("hello"), 0);
        }

        public void StopServer()
        {
            running = false;
            LogMessage("Stopping server...");

            // Close all client sockets
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        client.Client.Shutdown(SocketShutdown.Both);
                        client.Client.Close();
                    }
                    catch { } // Ignore errors during cleanup
                }
                clients.Clear();
            }


            // Close the listener socket
            if (listener != null)
            {

                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                listener = null;
                LogMessage("Server stopped");
            }
            else
            {
                LogMessage("Server was already stopped");
            }
        }
        //private async void ReceiveCommandFromClient()
        //{
        //    //MessageBox.Show("Server: received message from client: ");

        //    // Receive message.
        //    var buffer = new byte[64000];
        //    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);

        //    string commandString = Encoding.UTF8.GetString(buffer, 0, received);
        //    //MessageBox.Show("Server: command: "+commandString);

        //    if(commandString.StartsWith("<|BOM|>") && commandString.EndsWith("<|EOM|>"))
        //    {
        //        commandString = commandString.Substring(7, commandString.Length - 14);
        //     //   MessageBox.Show("Server: command: " + commandString);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Server: Invalid message format:" + commandString);
        //        return;
        //    }

        //    commandObject = JsonSerializer.Deserialize<Command>(commandString);


        //    ProcessResponse();
        //    SendResponseToClient();
        //}




        //private void ProcessResponse()
        //{
        //    RegularUserAllowedCommands command = commandObject.CommandName;

        //   // MessageBox.Show("Server: command: " + command.ToString());

        //    switch (command)
        //    {
        //        case RegularUserAllowedCommands.CheckIfUsernameExists:
        //            {
        //                ServerDbOperations.CheckIfUsernameExists(commandObject.Value.ToString());
        //              //  MessageBox.Show("Server: response: " + Response);
        //                break;
        //            }
        //            //case ReportsEnum.HowManyRegularUsers:
        //            //    {
        //            //         HowManyRegularUsers();
        //            //        break;
        //            //    }
        //            //case ReportsEnum.HowManyAdmins:
        //            //    return HowManyAdmins();
        //            //case ReportsEnum.HowManyMessages:
        //            //    return HowManyMessages();
        //            //case ReportsEnum.HowManyMessagesInLast30days:
        //            //    return HowManyMessagesInLast30days();
        //            //default:
        //            //    throw new ArgumentException("Invalid report type");
        //    }



        //}


        //private async void SendResponseToClient()
        //{
        //   // MessageBox.Show("Server: sending response to client, Response: " + Response);

        //    string json = JsonSerializer.Serialize(Response);

        //    string beginMessage = "<|BOM|>";
        //    string endMessage = "<|EOM|>";

        //    string message = beginMessage + json + endMessage;



        //  //  MessageBox.Show("Server: sending response to client, json: " + json );
        //    var echoBytes = Encoding.UTF8.GetBytes(message);
        //    await handler.SendAsync(echoBytes, 0);


        //}
    }
}
