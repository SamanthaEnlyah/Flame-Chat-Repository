using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Data.ViewModels
{
    public class ChatWindowMessageViewModel
    {
        public ChatWindowMessageViewModel() { }

        public ChatUser Sender { get; set; }

        public ChatUser Receiver { get; set; }

        public Message SentMessage { get; set; }

        public string UsersMessage {
            get {
                return $"{SentMessage.DateTimeOfSending} {Sender.Username}: {SentMessage.Text}";
            }
            set { }
        }
    }
}
