using FlameChatClient.Client_Data.ViewModels;
using FlameChatClient.Exceptions;
using FlameChatClient.Notifications;
using FlameChatAdmin.GUIDialogs;
using FlameChatShared.Communication;
using SharedModel.Models;
using SharedModel.SharedClasses;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace FlameChatClient.ChatClient
{
    public class Client
    {

        public Response ResponseFromServer;

        Socket client;

        int received;




        //Response Task
        private TaskCompletionSource<Response> _responseTaskCompletionSource;

        public async Task<Response> GetResponse()
        {
            // Initialize the TaskCompletionSource
            _responseTaskCompletionSource = new TaskCompletionSource<Response>();


            // Return the Task to the caller

            return await _responseTaskCompletionSource.Task; 
        }

        public void SetResponse(Response response)
        {
            // Complete the Task when ResponseFromServer is set
            if (_responseTaskCompletionSource != null && !_responseTaskCompletionSource.Task.IsCompleted)
            {
               // MessageBox.Show("Setting Response in Client");

                _responseTaskCompletionSource.SetResult(response);

                //MessageBox.Show("response hash code: " + GlobalVariables.GetClient().GetResponse().GetHashCode());
                //MessageBox.Show("RESPONSE IS SET IN CLIENT, and it is: " + ((Response)_responseTaskCompletionSource.Task.Result).Value);
            }
        }
        //End Response Task


        public bool IsSocketNull()
        {
            //MessageBox.Show("client socket is null");
            return client == null;
        }
       

        public async Task<Response> SendCommandToServer(RegularUserAllowedCommands command, object value) 
        {
            
                    Command c = new Command()
                    {
                        IsAdmin = false,
                        RegularCommand = command,
                        Value = value
                    };


                    //GlobalVariables.LogMessage("Client: created command " + c.RegularCommand + ", " + value);


                    //Send Command to server

                    string json = JsonSerializer.Serialize(c);
                    //string beginMessage = "<|BOM|>";
                    string endMessage = "<|EOM|>";

                    string message = json + endMessage;

                    var messageBytes = Encoding.UTF8.GetBytes(message);

                    await client.SendAsync(messageBytes, SocketFlags.None);
                    
                    //GlobalVariables.LogMessage("Client: sent: " + message);

                    
               

            return new Response();
        }

        public async void ReceiveMessage()
        {

            string endMessage = "<|EOM|>";

            while (running)
            {

                //Receive response from server

                var buffer = new byte[30000000];

                received = await client.ReceiveAsync(buffer, SocketFlags.None);

                var response = Encoding.UTF8.GetString(buffer, 0, received);

                //GlobalVariables.LogMessage("Client RESPONSE: " + response);

                

                if (response.EndsWith("<|EOM|>"))
                {
                    response = response.Replace(endMessage, "");
                }
                else
                {
                    throw new ClientException("Client: Invalid message format: " + response);
                }

                response = ((string)response).Trim('\0').Trim();



                if (string.IsNullOrEmpty(response))
                {
                    //GlobalVariables.LogMessage("response is empty or null, or invalid: " + response);
                    throw new ClientException("response is empty or null, or invalid");
                } 

                //GlobalVariables.LogMessage("Client: response is " + response);
                Command command = JsonSerializer.Deserialize<Command>(response);


                //GlobalVariables.LogMessage("Client: COMMAND BEFORE SWITCH " + command);
                //GlobalVariables.LogMessage("Client: COMMAND AFTER SWITCH " + command.RegularCommand);
                //GlobalVariables.LogMessage("Client: COMMAND VALUE " + command.Value.ToString());

                switch (command.RegularCommand)
                {
                    case RegularUserAllowedCommands.Test:
                        {
                            MessageBox.Show("TEST");
                            TestClient();
                            break; 
                        }
                    case RegularUserAllowedCommands.CheckIfUsernameExists:
                        {
                            object deserializedResponse = command.Value;
                            ResponseFromServer = new Response(Boolean.Parse(deserializedResponse.ToString()));
                            SetResponse(ResponseFromServer);
                            break;
                        }
                    case RegularUserAllowedCommands.SignUp:
                        {
                            object deserializedResponse = command.Value;
                            ResponseFromServer = new Response(Boolean.Parse(deserializedResponse.ToString()));
                            SetResponse(ResponseFromServer);

                            break;
                        }

                    case RegularUserAllowedCommands.LogIn:
                        {
                            object deserializedResponse = command.Value;
                            ResponseFromServer = new Response(Boolean.Parse(deserializedResponse.ToString()));
                            //GlobalVariables.LogMessage("Response from server after login:  " + ResponseFromServer.Value + "");
                            SetResponse(ResponseFromServer);
                            break;

                        }
                    case RegularUserAllowedCommands.LogOut:
                        {


                            break;
                        }
                    case RegularUserAllowedCommands.GetOtherUsers:
                        {
                            //MessageBox.Show("GET OTHER USERS STARTED");

                            JsonElement responseElement = (JsonElement)command.Value;

                            //MessageBox.Show("Client: response is json element"); 
                            //MessageBox.Show("Client: get other users response is " + responseElement.GetRawText());

                            var users = JsonSerializer.Deserialize<List<ChatUser>>(responseElement.GetRawText());
                            if (users == null) MessageBox.Show("Users is null");
                            else
                            {
                                ResponseFromServer = new Response(users);
                                SetResponse(ResponseFromServer);
                            }
                            break;
                        }

                    case RegularUserAllowedCommands.GetOtherUserTraits:
                        {
                            JsonElement responseElement = (JsonElement)command.Value;
                            //JsonElement responseElement = JsonSerializer.Deserialize<JsonElement>(command.Value);
                            var traits = JsonSerializer.Deserialize<List<PersonalityTrait>>(responseElement.GetRawText());
                            if (traits == null)
                            {
                                MessageBox.Show("Traits is null");
                            }
                            else
                            {
                                ResponseFromServer = new Response(traits);
                                SetResponse(ResponseFromServer);
                            }

                            break;//return new Response(traits);
                        }
                    case RegularUserAllowedCommands.GetUserId:
                        {
                            //MessageBox.Show("GetUserId STARTED");
                            int userid = ((JsonElement)command.Value).GetInt32();


                            ResponseFromServer = new Response(userid);
                            SetResponse(ResponseFromServer);

                            //MessageBox.Show("GetUserId is "+deserializedResponse);
                            break;//return new Response(deserializedResponse);

                        }
                    case RegularUserAllowedCommands.GetUsernameFromId:
                        {
                            //MessageBox.Show("GetUserId STARTED");
                            string username = ((JsonElement)command.Value).GetString();


                            ResponseFromServer = new Response(username);
                            SetResponse(ResponseFromServer);

                            //MessageBox.Show("GetUserId is "+deserializedResponse);
                            break;//return new Response(deserializedResponse);

                        }
                        
                    case RegularUserAllowedCommands.GetUserFromId:
                        {
                            //MessageBox.Show("GetUserId STARTED");
                            JsonElement userJson = ((JsonElement)command.Value);

                            var user = JsonSerializer.Deserialize<ChatUser>(userJson.GetRawText());

                            ResponseFromServer = new Response(user);
                            SetResponse(ResponseFromServer);

                            //MessageBox.Show("GetUserId is "+deserializedResponse);
                            break;//return new Response(deserializedResponse);

                        }

                    case RegularUserAllowedCommands.GetUserAvatar:
                        {
                            //MessageBox.Show( "Client GetUserAvatar STARTED");
                            byte[] avatar = ((JsonElement)command.Value).GetBytesFromBase64();


                            ResponseFromServer = new Response(avatar);
                            SetResponse(ResponseFromServer);
                            break;

                        }
                       
                    case RegularUserAllowedCommands.SetMyPersonalityTraits:
                        {
                            bool success = ((JsonElement)command.Value).GetBoolean();
                            ResponseFromServer = new Response(success);
                            SetResponse(ResponseFromServer);
                            break;// return new Response(Boolean.Parse(deserializedResponse.ToString()));
                        }


                    case RegularUserAllowedCommands.GetMyPersonalityTraits:
                        {
                            JsonElement responseElement = (JsonElement)command.Value;
                            var traits = JsonSerializer.Deserialize<List<PersonalityTrait>>(responseElement.GetRawText());
                           // MessageBox.Show("TRAITS IS EMPTY:" + traits.IsNullOrEmpty());
                            if (traits == null)
                            {
                                MessageBox.Show("Traits is null");

                            }
                            else
                            {
                                //MessageBox.Show("num traits: "+traits.Count);
                                ResponseFromServer = new Response(traits);
                                //MessageBox.Show(ResponseFromServer.Value.ToString());
                                SetResponse(ResponseFromServer);
                            }


                            break; //return new Response(traits);

                        }

                        //#3 for chosing  traits from all traits
                    case RegularUserAllowedCommands.GetAllPersonalityTraits:
                        {

                            JsonElement responseElement = (JsonElement)command.Value;
                            //MessageBox.Show("Client.cs, get all personality traits: "+responseElement.GetRawText());

                            var traits = JsonSerializer.Deserialize<List<PersonalityTrait>>(responseElement.GetRawText());
                            if (traits == null)
                            {
                                MessageBox.Show("Traits is null");

                            }
                            else
                            {
                                ResponseFromServer = new Response(traits);
                                SetResponse(ResponseFromServer);
                            }
                            break; //return new Response(traits);
                        }

                    case RegularUserAllowedCommands.IsUserMyContact:
                        {
                            bool isUserMyContact = JsonSerializer.Deserialize<bool>((JsonElement)command.Value);
                            ResponseFromServer = new Response(isUserMyContact);
                            SetResponse(ResponseFromServer);

                            //MessageBox.Show("Client received: " +ResponseFromServer.Value );
                            break; //return new Response(Boolean.Parse(deserializedResponse.ToString()));
                        }

                    case RegularUserAllowedCommands.SaveUserAsContact:
                        {
                            bool success = JsonSerializer.Deserialize<bool>((JsonElement)command.Value);
                                
                            //GlobalVariables.LogMessage("Client: SaveUserAsContact, success: " + success);

                            ResponseFromServer = new Response(success);
                            SetResponse(ResponseFromServer);
                            //await Notifier.NotifyContactsInUserInfo();
                            break; //return new Response(result);

                        }

                    case RegularUserAllowedCommands.GetContacts:
                        {


                            var contacts = JsonSerializer.Deserialize<List<ChatUser>>(((JsonElement)command.Value).GetRawText());
                            if (contacts == null) MessageBox.Show("Contacts is null");
                            ResponseFromServer = new Response(contacts);

                            //MessageBox.Show("CLIENT, Response from server for get contacts: ",ResponseFromServer.ToString());

                            SetResponse(ResponseFromServer);
                            //ResponseFromServer = new Response(contacts);
                            //await Notifier.NotifyContactsInChatWindow(contacts);
                            //await Notifier.NotifyContactsInUserInfo();

                            break;
                        }

                    case RegularUserAllowedCommands.SendMessage:
                        {
                            //MessageBox.Show("Client: SendMessage STARTED");
                            //object deserializedResponse = JsonSerializer.Deserialize<object>(response);
                            
                            break; //return new Response(Boolean.Parse(deserializedResponse.ToString()));

                        }

                    case RegularUserAllowedCommands.None:
                        {

                            break; //return new Response();
                        }

                    case RegularUserAllowedCommands.ReceiveMessage:
                        {

                            //MessageBox.Show("USAO U COMMAND RECEIVE NA KLIJENTU");

                            //Message messageR = command.Value as Message;

                            Message messageToMe = JsonSerializer.Deserialize<Message>(((JsonElement)command.Value).GetRawText());



                            ResponseFromServer = new Response(messageToMe);

                            SetResponse(ResponseFromServer);



                            //GlobalVariables.LogMessage(messageToMe.Text);

                            await Notifier.NotifyMessageInChatWindow(messageToMe);


                            break; //return new Response();
                        }

                    case RegularUserAllowedCommands.GetAllMessagesForUser:
                        {
                            //MessageBox.Show("Client: GetAllMessagesForUser STARTED");
                            //List<Message> messages = command.Value as List<Message>;
                            List<Message> messages = JsonSerializer.Deserialize<List<Message>>(((JsonElement)command.Value).GetRawText());
                            if (messages == null)
                            {
                                MessageBox.Show("Messages is null");
                            }
                            else
                            {
                                ResponseFromServer = new Response(messages);
                                SetResponse(ResponseFromServer);
                            }
                          
                            break; 
                        }

                    case RegularUserAllowedCommands.MessageFromServer:
                        {
                            //MessageBox.Show("Client: Message from server: " + command.Value.ToString());

                            string messageFromServer = ((JsonElement)command.Value).GetString();

                            ResponseFromServer = new Response(messageFromServer);

                            SetResponse(ResponseFromServer);

                            await Notifier.NotifyUser((string)ResponseFromServer.Value);
                            break;
                        }
                }

            }
        }

        //public async void ReceiveMessage()
        //{


        //    MessageBox.Show("POKRENUT RECEIVE MESSAGE NA KLIJENTU");

        //    while (running)
        //    {


        //        var buffer = new byte[30000000];

        //        received = await client.ReceiveAsync(buffer, SocketFlags.None);

        //        var response = Encoding.UTF8.GetString(buffer, 0, received);


        //        if (response.EndsWith("<|EOM|>"))
        //        {
        //            response = response.Replace("<|EOM|>", "");

        //        }
        //        else
        //        {
        //            throw new ClientException("exception in ListenToWhatShouldBeCalled");
        //        }

        //        response = ((string)response).Trim('\0').Trim();



        //        if (string.IsNullOrEmpty(response))
        //        {
        //            //GlobalVariables.LogMessage("response is empty or null, or invalid: " + response);
        //            throw new ClientException("response is empty or null, or invalid");
        //        }

        //        iCalledSendCommandToServer = JsonSerializer.Deserialize<bool>(response);




        //        if (!iCalledSendCommandToServer)
        //        {


        //            var buffer1 = new byte[30000000];

        //            received = await client.ReceiveAsync(buffer1, SocketFlags.None);

        //            var response1 = Encoding.UTF8.GetString(buffer1, 0, received);


        //            if (response1.EndsWith("<|EOM|>"))
        //            {
        //                response1 = response1.Replace("<|EOM|>", "");

        //            }
        //            else
        //            {
        //                throw new ClientException("Client, Receive message method: Invalid message format: " + response1);
        //            }

        //            response1 = ((string)response1).Trim('\0').Trim();



        //            if (string.IsNullOrEmpty(response1))
        //            {
        //                //GlobalVariables.LogMessage("response is empty or null, or invalid: " + response1);
        //                throw new ClientException("response is empty or null, or invalid");
        //            }

        //            //MessageBox.Show("this response is wrong: "+response);

        //            Command receivedCommand = JsonSerializer.Deserialize<Command>(response);

        //            if (receivedCommand.RegularCommand == RegularUserAllowedCommands.ReceiveMessage)
        //            {


        //                Notifier.NotifyChatWindow((Message)receivedCommand.Value);


        //            }
        //        }
        //    }

        //}


        public async Task<string> ListenForServerIPBroadcastUdp(IPAddress chosenIP)
        {
            Socket? listener = null;
            
                try
                {
                    var hostName = Dns.GetHostName();
                    //GlobalVariables.LogMessage(hostName);

                    // IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
                    //IPAddress localIpAddress = localhost.AddressList[0];

                    //IPAddress broadcastaddress = NetworkUtils.GetBroadcastAddress(chosenIP, NetworkUtils.GetSubnetMask(chosenIP));

                    IPEndPoint ipEndPoint = new(chosenIP, 11_001);

                    listener = new(
                        ipEndPoint.AddressFamily,
                        SocketType.Dgram, // Use Datagram for UDP
                        ProtocolType.Udp  // Use UDP protocol
                    );

                    listener.Bind(ipEndPoint);

                    //GlobalVariables.LogMessage("Client: Listening for server IP broadcast...");

                    var buffer = new byte[1024];
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    var result = await listener.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);
                    string serverIP = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);
                    //MessageBox.Show($"Received server IP: {serverIP}");
                    return serverIP;
                }
                catch(Exception e)
            {
                MessageBox.Show("EXCEPTION: " + e.Message + ", STACK TRACE: " + e.StackTrace);
                return "some ip address";
            }
                finally
                {
                    listener?.Close();
                }
        
        }



        public async Task<string> GetServerIP(Socket clientHandler)
        {
            try
            {
                // Receive message.
                var buffer = new byte[30000000];
                var received = await clientHandler.ReceiveAsync(buffer, SocketFlags.None);


                if (received == 0)
                {
                    //GlobalVariables.LogMessage("Nothing received");

                }


                string serverIP = Encoding.UTF8.GetString(buffer, 0, received);
                //MessageBox.Show("Client: Server IP: " + serverIP);
                return serverIP;
            }
            catch (Exception e)
            {
                MessageBox.Show("Client: Error getting server IP: " + e.StackTrace);
                return "some ip address";
            }
        }



        public async Task<bool> StartClient()
        {
            // Initialize the client and connect to the server

            //    IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("amethyst.chat");

            try
            {


                var hostName = Dns.GetHostName();
                //GlobalVariables.LogMessage(hostName);

                IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);

                IPAddress chosenIP = null;
                IpAddressChooser ipchooser = new();
                ipchooser.Title = "Client IP Chooser";
                ipchooser.listIPs.ItemsSource = localhost.AddressList.Where(ip => ip.AddressFamily != AddressFamily.InterNetworkV6); 
                if (ipchooser.ShowDialog() == true)
                {
                    chosenIP = ipchooser.ChosenIPAddress;
                    //MessageBox.Show("IP address of this client: " + chosenIP);
                }

                string clientIP = await ListenForServerIPBroadcastUdp(chosenIP);

                //GlobalVariables.LogMessage("Client IP: " + clientIP);
                IPHostEntry serverIPEntry = await Dns.GetHostEntryAsync(clientIP);

                // This is the IP address of the local machine
                IPEndPoint ipEndPoint = null;

   

                try
                {
                    ipEndPoint = new(IPAddress.Parse(clientIP), 11_000);
                    client = new(
                        ipEndPoint.AddressFamily,
                        SocketType.Stream,
                        ProtocolType.Tcp);

                    await client.ConnectAsync(ipEndPoint);

                    //SendIdToServer();

                    running = true;

                }
                catch (Exception e)
                {
                    MessageBox.Show("Client: Error creating socket: " + e.Message);
                }
                
                //GlobalVariables.LogMessage("method start client (should be true) running=" + running);


                ThreadStart ts = new ThreadStart(ReceiveMessage);
                Thread t = new Thread(ts);
                t.Start();



                return true;
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Client: Error starting client: " + e.Message + ", STACK TRACE " + e.StackTrace);
                throw new ClientException("Client: Error starting client: " + e.Message);

               
            }
        }

        public async void TestClient()
        {
            var buffer = new byte[1024];

           int  received = await client.ReceiveAsync(buffer, SocketFlags.None);

            var response = Encoding.UTF8.GetString(buffer, 0, received);

            MessageBox.Show(response);
        }

        public void StopClient()
        {
            running = false;
           
            
            //GlobalVariables.LogMessage("method stop client (should be false) running=" + running);
            // Close the client socket
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        bool running;
        public bool IsStarted()
        {
            return running;
        }

      
    }
}
