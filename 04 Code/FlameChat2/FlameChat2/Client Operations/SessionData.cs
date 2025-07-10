using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Operations
{
    [JsonSerializable(typeof(SessionData))]
    public class SessionData
    {
        public ChatUser User { get; set; }
        public DateTime Time { get; set; }

        


        
    }
}
