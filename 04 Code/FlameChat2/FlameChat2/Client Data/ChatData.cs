using FlameChatClient.ChatClient;
using FlameChatClient.Client_Data.ViewModels;
using FlameChatClient.Client_Operations;
using FlameChatShared.Communication;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlameChatClient.Client_Data
{
    public class ChatData
    {
        MessagesOp messagesOp = new MessagesOp();
        ChatUserOp userOp = new ChatUserOp();
       public  List<ChatWindowMessageViewModel> messages = new List<ChatWindowMessageViewModel>();


        //get all my messages from contacts, and messages from me to contacts, in date time order.
        //get all messages where i am either sender or receiver.
        public ChatData()
        {
            //FillMessages();

            //GlobalVariables.GetClient().FillAllMessagesFromDB(SessionManager.Data.User.ChatUserId);
        }

        public async Task FillMessages()
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetAllMessagesForUser, SessionManager.Data.User.ChatUserId);
            Response response = await GlobalVariables.GetClient().GetResponse();

            //MessageBox.Show("response returned: " + response.ToString());

            if (response == null || response.Value == null)
            {
                MessageBox.Show("No messages found or response is null.");
                return;
            }

            List<Message> messagesFromServer = (List<Message>)response.Value;

            //MessageBox.Show("response returned: messages history count: " + messagesFromServer.Count);
            //Response response = //List<Message> messagesFromServer = (List<Message>)response.Value;

            foreach (Message m in messagesFromServer)
            {
                ChatWindowMessageViewModel messageView = new ChatWindowMessageViewModel();
                messageView.SentMessage = m;

                await userOp.GetUserFromId((int)m.FkUserWhoSentId);
                Response responseSenderID = await GlobalVariables.GetClient().GetResponse(); 
                messageView.Sender = (ChatUser)responseSenderID.Value;

                
                await userOp.GetUserFromId((int)m.FkWhoReceivedId);
                Response responseReceiverID = await GlobalVariables.GetClient().GetResponse();
                messageView.Receiver = (ChatUser)responseReceiverID.Value;

                messages.Add(messageView);
            }
        }



        public async void AddMessage(ChatWindowMessageViewModel message)
        {
            messages.Add(message);
          
        }

        public List<ChatWindowMessageViewModel> GetAllMessages()
        {
            return messages;
        }

        public void ClearMessages()
        {
            messages.Clear();
        }

        public List<ChatWindowMessageViewModel> GetMessagesForContact(ChatUser contact)
        {
            if (contact == null) return new List<ChatWindowMessageViewModel>();
            return messages.Where(m => m.Sender.ChatUserId == contact.ChatUserId || m.Receiver.ChatUserId == contact.ChatUserId).OrderBy<ChatWindowMessageViewModel, DateTime?>(m=>m.SentMessage.DateTimeOfSending).ToList();
        }
    }
}
