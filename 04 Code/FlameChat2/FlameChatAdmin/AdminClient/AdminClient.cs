using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using FlameChatShared.Communication;
using SharedModel.Models;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Core.Pipeline;
using SharedModel.SharedClasses;
using FlameChatClient.Exceptions;
using FlameChatAdmin.GUIDialogs;

namespace FlameChatClient.ChatClient
{
    public class AdminClient
    {
    
        Socket client;

        int received;

        //public bool? Response { get; internal set; }

        //public TextBlock clientMessagesGUI { get; set; }
         

        public bool IsSocketNull()
        {
            return client == null;
        }
       

        public async Task<Response>  SendCommandToServer(AdminUserAllowedCommands command, object value)
        {

            try {
                Command c = new Command()
                {
                    AdminCommand = command,
                    Value = value,
                    IsAdmin = true
                };

             //   MessageBox.Show("Client: command " + command);

                string json = JsonSerializer.Serialize(c);
                //string beginMessage = "<|BOM|>";
                string endMessage = "<|EOM|>";

                string message = json + endMessage;

                var messageBytes = Encoding.UTF8.GetBytes(message);

                //SendMessage(messageBytes);
                await client.SendAsync(messageBytes, SocketFlags.None);

                //Response = null;




              // GlobalVariables.LogMessage("Client: sent: " + message);


                var buffer = new byte[30000000];

                //GlobalVariables.LogMessage("Client: buffer");

                //ReceiveResponse(buffer);

                received = await client.ReceiveAsync(buffer, SocketFlags.None);

             //   GlobalVariables.LogMessage("Client: received: " + buffer);

                var response = Encoding.UTF8.GetString(buffer, 0, received);

               //MessageBox.Show("Client RESPONSE: " + response);
                GlobalVariables.LogMessage("Client RESPONSE: "+ response);



                if (/*response.StartsWith("<|BOM|>") &&*/ response.EndsWith("<|EOM|>"))
                {
                    //response = response.Replace(beginMessage,"");
                    response = response.Replace(endMessage, "");
                   // GlobalVariables.LogMessage("Client: RESPONSE, TRUE OR FALSE: " + response);
                }
                else
                {
                    //MessageBox.Show("Client: Invalid message format: " + response);
          
                    return new Response("Client: Invalid message format: "+response); 
                }

                response = ((string)response).Trim('\0').Trim();



                if (string.IsNullOrEmpty(response) ) {
                    GlobalVariables.LogMessage("response is empty or null, or invalid: " + response);

                    return new Response("response is empty or null, or invalid"); 
                }

                

               // GlobalVariables.LogMessage("Client: COMMAND BEFORE CHECKING " + command);
                switch (command)
                {
                    case AdminUserAllowedCommands.PersonalityTraitExists:
                        {
                            //returns true if the trait was added
                            return new Response(Boolean.Parse(response));

                        }
                    case AdminUserAllowedCommands.AddManyPersonalityTraits:
                        {
                            //returns true if the trait was added
                            return new Response(Boolean.Parse(response));

                        }
                    case AdminUserAllowedCommands.AddPersonalityTrait:
                    {
                            //returns true if the trait was added
                            //MessageBox.Show("Client: AddPersonalityTrait response: " + response);
                            return new Response(Boolean.Parse(response));

                    }
                    case AdminUserAllowedCommands.DeletePersonalityTrait:
                    {

                            return new Response(Boolean.Parse(response));
                    }               
                    
                    case AdminUserAllowedCommands.EditPersonalityTrait:
                    {
                         return new Response(Boolean.Parse(response));
                    }

                    case AdminUserAllowedCommands.GetAllPersonalityTraits:
                    {
                        return new Response(JsonSerializer.Deserialize<List<PersonalityTrait>>(response));
                    }
                    case AdminUserAllowedCommands.GetAllUsers:
                        {
                            MessageBox.Show("GetUserId STARTED");
                            int deserializedResponse = JsonSerializer.Deserialize<int>(response);
                            MessageBox.Show("GetUserId is "+deserializedResponse);
                            return new Response(deserializedResponse);
                            
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
                }
            
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Client: Error in SendCommandToServer: " + ex.Message);
                // Console.WriteLine($"Error in SendCommandToServer: {ex.Message}");
                throw new ClientException("Client: Error in SendCommandToServer: " + ex.Message);
            }

              return new Response("COMMAND NOT FOUND");

        }


        public async Task StartClient()
        {
            // Initialize the client and connect to the server

            //    IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("amethyst.chat");
            var hostName = Dns.GetHostName();
            IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);

            
            IPAddress chosenIP = null;

            //Show IP Chooser Dialog Box
            // This is a dialog box that allows the user to choose an IP address from a list of available IP addresses on the local machine
            IpAddressChooser ipAddressChooser = new IpAddressChooser();
            ipAddressChooser.listIPs.ItemsSource = localhost.AddressList.Where(ip => ip.AddressFamily != AddressFamily.InterNetworkV6);
            ipAddressChooser.Title = "Admin IP Chooser";

            if (ipAddressChooser.ShowDialog() == true)
            {
                chosenIP = ipAddressChooser.ChosenIPAddress;
                //MessageBox.Show("IP address of this client: " + chosenIP);
            }


            // This is the IP address of the local machine
            IPAddress localIpAddress = chosenIP;//localhost.AddressList[0];


            IPEndPoint ipEndPoint = new(localIpAddress, 11_000);
             client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);


            await client.ConnectAsync(ipEndPoint);
            
            running = true; 

            GlobalVariables.LogMessage("method start client (should be true) running=" + running);

        }


        public void StopClient()
        {
            running = false;
            GlobalVariables.LogMessage("method stop client (should be false) running=" + running);
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
