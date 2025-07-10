using FlameChatClient.ChatClient;
using FlameChatShared.Communication;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Operations
{
    public  class ChatUserOp
    {
        public async Task GetUserFromId(int userID)
        {
            Response response = await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetUserFromId, userID);
            GlobalVariables.GetClient().SetResponse(response);
        }
    }
}
