using FlameChatClient.ChatClient;
using FlameChatClient.Client_Data.ViewModels;
using FlameChatShared.Communication;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatClient.Client_Operations
{
    public class MessagesOp
    {
        public MessagesOp() { }

     
        /// <summary>
        /// Sends a message to the server asynchronously.
        /// </summary>
        /// <remarks>This method sends the specified <see cref="Message"/> object to the server using the
        /// current client connection. Ensure that the client is properly initialized and connected before calling this
        /// method.</remarks>
        /// <param name="message">The message to be sent. Cannot be null.</param>
        public async void SendMessage(Message message)
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.SendMessage, message);
        }

        public async Task<Response> GetAllMessagesFromDBForUser(int chatUserId)
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetAllMessagesForUser, chatUserId);
            Response response = await GlobalVariables.GetClient().GetResponse();
            return response;
        }
    }
}
