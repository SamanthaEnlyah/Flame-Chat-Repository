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
    public class ContactsOp
    {
        public List<ChatUser> ContactsList;

        public ContactsOp()
        {
            ContactsList = new List<ChatUser>();


        }


        public async Task<bool> IsUserMyContact(int userId, int contactUserId)
        {
            UserHasContact uhc= new UserHasContact
            {
                UserId = userId,
                UserContactId = contactUserId
            };
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.IsUserMyContact, uhc);
            Response r = await GlobalVariables.GetClient().GetResponse();
            return Boolean.Parse(r.Value.ToString());
        }




        public async Task<bool> SaveUserAsContact(UserHasContact userHasContact)
        {

            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.SaveUserAsContact, userHasContact);
            Response r = await GlobalVariables.GetClient().GetResponse();
            return Boolean.Parse(r.Value.ToString());
        }





        public void RemoveContact(ChatUser contact)
        {
            if (!ContactsList.Remove(contact))
            {
                throw new InvalidOperationException("Contact not found.");
            }
        }



        //public ChatUser GetContactById(int contactId)
        //{
           
        //}

        //public ChatUser GetContactByUsername(string username)
        //{
           
           
        //}

        //public List<ChatUser> GetContactsByStatus(bool isLoggedIn)
        //{
        //    return ContactsList.Where(c => c.IsLoggedIn == isLoggedIn).ToList();
        //}

        public async Task<List<ChatUser>> GetAllContacts(int userId)
        {

            //GlobalVariables.LogMessage("GetAllContacts called");
           
            await GlobalVariables.GetClient().SendCommandToServer(FlameChatShared.Communication.RegularUserAllowedCommands.GetContacts, userId);
            Response response = await GlobalVariables.GetClient().GetResponse();
            List<ChatUser>? contacts = response.Value as List<ChatUser>;
            return contacts;
        }

        //public void UpdateContact(ChatUser contact)
        //{
        //    var existingContact = ContactsList.FirstOrDefault(c => c.ChatUserId == contact.ChatUserId);
        //    if (existingContact == null)
        //    {
        //        throw new InvalidOperationException("Contact not found.");
        //    }
        //    existingContact.Username = contact.Username;
        //    existingContact.AvatarImage = contact.AvatarImage;
        //}

        //public void ClearContacts()
        //{
        //    ContactsList.Clear();
        //}
    }
}
