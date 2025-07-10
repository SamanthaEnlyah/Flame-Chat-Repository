using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.ChatClient
{
    public class Response
    {
        public object Value { get; set; }


        public Response()
        {
            Value = "";
                
        }

        public Response(object value)
        {
            Value = value;
        }

    }
}
