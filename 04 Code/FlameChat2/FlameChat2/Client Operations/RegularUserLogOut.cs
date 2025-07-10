using FlameChatClient.ChatClient;
using FlameChatShared.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Operations
{
    public class RegularUserLogOut
    {
        public async Task LogOut()
        {
            // Delete the session file
            if (File.Exists("session/session.json"))
            {
                File.Delete("session/session.json");

                await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.LogOut, "");


            }
        }
    }
}
