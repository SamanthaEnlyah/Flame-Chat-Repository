using Azure;
using FlameChatDatabaseLibrary.DB;
using FlameChatAdmin.ServerMainThread;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Windows.Media;
using System.Diagnostics.Eventing.Reader;
using SharedModel.Models;
using System.Windows;
using SharedModel.ComplexModels;
using Microsoft.EntityFrameworkCore.Internal;
using FlameChatAdmin.GUIDialogs;

namespace FlameChatAdmin.ServerOperations
{
    public class ServerDbOperations
    {
        private readonly Server server;

        public ServerDbOperations(Server server)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
        }


        internal async Task<bool> CheckIfUsernameExists(string username)
        {
            //AnyAsync is an Entity Framework Core method that checks if any elements in a sequence satisfy a condition. It's more efficient than Count() because:
            // It stops searching as soon as it finds the first match
            //It's asynchronous, so it doesn't block the thread while querying the database
            //It translates to a SQL EXISTS query, which is typically more efficient than COUNT
            try
            {
               // MessageBox.Show("Db: Checking if username exists: " + username);
                return await (DBContext.FlameDBContext.ChatUsers.AnyAsync(x => x.Username == username));
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error in db operations: {ex.Message}");
                return false;



            }
        }

        internal async Task<List<ChatUser>> GetOtherUsers(string exceptUsername)
        {
            //List<ChatUser> users = new List<ChatUser>();
            return await DBContext.FlameDBContext.ChatUsers.
                Where(x => x.Username != exceptUsername).
                Select(u => new ChatUser
                {
                    ChatUserId = u.ChatUserId,
                    Username = u.Username
                    // Don't include password for security
                })
            .ToListAsync();
        }

        internal async Task<bool> LogIn(ChatUser user)
        {

           // MessageBox.Show("From Database, username: " + user.Username + ", password:" + user.Password);
            bool exists = await DBContext.FlameDBContext.ChatUsers.AnyAsync(x => x.Username == user.Username && x.Password == user.Password);
            if (exists)
            {
                server.LogMessage($"User {user.Username} logged in successfully.");

                return true;
            }
            else
            {
                server.LogMessage($"User {user.Username} failed to log in.");
                return false;
            }
        }

        internal async Task<bool> SignUp(ChatUser chatUser)
        {
            
           DBContext.FlameDBContext.ChatUsers.Add(chatUser);
            try
            {

                await DBContext.FlameDBContext.SaveChangesAsync();
                server.LogMessage($"User {chatUser.Username} signed up successfully.");
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Get more detailed error information
                server.LogMessage($"DbUpdateException signing up user {chatUser.Username}:");
                server.LogMessage($"Error: {ex.Message}");
                server.LogMessage($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Catch any other exceptions
                server.LogMessage($"Unexpected error signing up user {chatUser.Username}:");
                server.LogMessage($"Error: {ex.Message}");
                server.LogMessage($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<byte[]> GetUserAvatar(int userId)
        {
            var user = await DBContext.FlameDBContext.ChatUsers
                .Where(u => u.ChatUserId == userId)
                .Select(u => new { u.AvatarImage })
                .FirstOrDefaultAsync();
            return user?.AvatarImage;
        }

        internal async Task<List<ChatUser>> GetContacts(int loggedInUser)
        {
            //id svih kontakata
            List<UserHasContact> contactsIds= await DBContext.FlameDBContext.UserHasContacts.
                Where(x => x.UserId == loggedInUser).
                ToListAsync();

            List<ChatUser> contacts = new List<ChatUser>();
            foreach (var contact in contactsIds)
            {
                var user = await DBContext.FlameDBContext.ChatUsers
                    .Where(u => u.ChatUserId == contact.UserContactId)
                    .Select(u => new ChatUser
                    {
                        ChatUserId = u.ChatUserId,
                        Username = u.Username,
                        AvatarImage = u.AvatarImage
                    })
                    .FirstOrDefaultAsync();
                if (user != null)
                {
                    contacts.Add(user);
                }
            }

            server.LogMessage($"User {loggedInUser} has {contacts.Count} contacts.");   

            return contacts;
        }

        public async Task<bool> SaveUserAsContact(int loggedInUserId, int userContactId)
        {
            UserHasContact userHasContact = new UserHasContact()
            {
                UserId = loggedInUserId,
                UserContactId = userContactId
            };
            await DBContext.FlameDBContext.UserHasContacts.AddAsync(userHasContact);
            await DBContext.FlameDBContext.SaveChangesAsync();
            return true;
        }

        //internal async Task<object> GetChatHistory(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        internal async Task<int> GetUserId(string? username)
        {
            int id = await DBContext.FlameDBContext.ChatUsers.Where(x => x.Username == username).Select(x => x.ChatUserId).FirstAsync();
           server.LogMessage("Db: User id: " + id);
            return id;
        }

        internal async Task<object> GetUnseenMessagesCount(int userId)
        {
            return null;
        }


        public async Task<List<PersonalityTrait>> GetPersonalityTraits(string username)
        {
            int userId = await GetUserId(username);
            List<int?> userHasTraits = await DBContext.FlameDBContext.UserHasTraits.Where(x=> x.CkChatUserId == userId).Select(x=> x.CkPersonalityTraitId).ToListAsync();
            return await DBContext.FlameDBContext.PersonalityTraits.Where(x => userHasTraits.Contains(x.PersonalityTraitId)).ToListAsync();
        }

        public async Task<bool> AddPersonalityTrait(string ptrait)
        {
            try
            {
                PersonalityTrait trait = new PersonalityTrait()
                {
                    TraitName = ptrait
                };
                DBContext.FlameDBContext.PersonalityTraits.Add(trait);
                await DBContext.FlameDBContext.SaveChangesAsync();
            } catch (Exception ex) {
                server.LogMessage($"Error adding personality trait: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> AddManyPersonalityTraits(List<string> traits)
        {
            try
            {
                MessageBox.Show("Traits adding started");
                foreach (string trait in traits)
                {
                    PersonalityTrait ptrait = new PersonalityTrait()
                    {
                        TraitName = trait
                    };
                    server.LogMessage("Add Trait: " + trait);
                    DBContext.FlameDBContext.PersonalityTraits.Add(ptrait);
                }
                MessageBox.Show("saving trait  started");
                await DBContext.FlameDBContext.SaveChangesAsync();
                server.LogMessage($"Added {traits.Count} personality traits.");
                
                return true;
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error adding personality traits: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> PersonalityTraitExists(string trait)
        {
            try
            {
                return await DBContext.FlameDBContext.PersonalityTraits.AnyAsync(x => x.TraitName == trait);
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error checking if personality trait exists: {ex.Message}");
                return false;
            }
        }

        public async Task<List<PersonalityTrait>> GetAllPersonalityTraits()
        {
            try
            {
                return await DBContext.FlameDBContext.PersonalityTraits.ToListAsync();
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error getting all personality traits: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SetMyPersonalityTraits(UserPersonality personality)
        {
            try
            {
                //clear all user traits
                UserHasTrait[] userTraits = await DBContext.FlameDBContext.UserHasTraits.Where(x => x.CkChatUserId == personality.UserId).ToArrayAsync();
                DBContext.FlameDBContext.UserHasTraits.RemoveRange(userTraits);

                //add new traits
                foreach (PersonalityTrait trait in personality.Traits)
                {
                    UserHasTrait userHasTrait = new UserHasTrait()
                    {
                        CkChatUserId = personality.UserId,
                        CkPersonalityTraitId = trait.PersonalityTraitId
                    };
                    await DBContext.FlameDBContext.UserHasTraits.AddAsync(userHasTrait);
                }
                await DBContext.FlameDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error setting personality traits: {ex.Message}");
                return false;
            }
        }

        public async Task<List<PersonalityTrait>> GetMyPersonalityTraits(int userId)
        {
            try
            {
                List<int?> userHasTraits = await DBContext.FlameDBContext.UserHasTraits
                    .Where(x => x.CkChatUserId == userId)
                    .Select(x => x.CkPersonalityTraitId)
                    .ToListAsync();
               // MessageBox.Show("user has " + userHasTraits.Count + " traits.");
                return await DBContext.FlameDBContext.PersonalityTraits
                    .Where(x => userHasTraits.Contains(x.PersonalityTraitId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error getting personality traits: {ex.Message}");
                return new List<PersonalityTrait>();
            }
        }

        internal async Task<bool> IsUserMyContact(int userId, int contactId)
        {

            bool result = await DBContext.FlameDBContext.UserHasContacts.Where(x => x.UserId == userId && x.UserContactId == contactId).AnyAsync();
           // MessageBox.Show("Server, db operations: is user my contact response: " + result);
            return result;
        }

        public  async Task<bool> InsertMessage(Message userMessage)
        {
            try
            {
                DBContext.FlameDBContext.Messages.Add(userMessage);

                await DBContext.FlameDBContext.SaveChangesAsync();
                return true;
            } catch (Exception ex)
            {
                server.LogMessage("Server operations: "  + ex.Message);
            }
            return false;
        }

        internal async Task<string> GetUsernameFromId(int id)
        {
            string username = "";
            if (await DBContext.FlameDBContext.ChatUsers.FindAsync(id) != null) {
                username = ((ChatUser?)await DBContext.FlameDBContext.ChatUsers.FindAsync(id)).Username;
            }
            return username;
        }

        public async Task<List<Message>> GetAllMessagesForUserId(int userId)
        {
            List<Message> messages = (List<Message>)await DBContext.FlameDBContext.Messages
                .Where(m => m.FkWhoReceivedId == userId || m.FkUserWhoSentId == userId)                
                .ToListAsync();
            
            server.LogMessage($"Retrieved {messages.Count} messages for user ID {userId}.");
            
            return messages;
        }

        public async Task<ChatUser> GetUserFromId(int id)
        {
           return await DBContext.FlameDBContext.ChatUsers.FindAsync(id);
        }
    }
}
