using FlameChatClient.Client_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Operations
{
    public class ChatManager
    {
        public ChatData ChatDataMessages { get; set; }

        public ChatManager()
        {
            ChatDataMessages = new ChatData();
        }

    }
}
