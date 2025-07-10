using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Exceptions
{
    class ClientException : Exception
    {
        public ClientException(string message) : base(message)
        {
        }
        public ClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
   
    
}
