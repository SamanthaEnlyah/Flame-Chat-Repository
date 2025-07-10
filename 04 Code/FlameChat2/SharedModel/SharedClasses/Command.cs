using FlameChatShared.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedModel.SharedClasses
{
    public class Command
    {
        public bool IsAdmin { get; set; }

        public object Value { get; set; }

        
        public RegularUserAllowedCommands RegularCommand { get; set; }

        public AdminUserAllowedCommands AdminCommand { get; set; }


    }

}
