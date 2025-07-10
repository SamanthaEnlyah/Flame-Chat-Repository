using Azure.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ComplexModels
{

    public class UserIdSocket
    {
        public Socket Client { get; set; }
        public int UserId { get; set; }

        public UserIdSocket()
        {
            
        }

        public UserIdSocket(Socket clientSocket, int userid)
        {
            Client = clientSocket;
            UserId = userid;
        }
    }
}