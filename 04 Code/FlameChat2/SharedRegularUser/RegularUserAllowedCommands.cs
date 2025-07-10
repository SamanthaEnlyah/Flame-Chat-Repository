using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatShared.Communication
{
    public enum RegularUserAllowedCommands
    {
        //Registration
        CheckIfUsernameExists,
        NotifyServerAboutMyUsername,
        SignUp,
        LogIn,


        //Message commands
        SendMessage,
        ReceiveMessage,
        NotifyUserAboutReceivedMessages,
        GetUnseenMessages,
        GetUnseenMessagesCount,
        SeeMessage,
        GetUsers,
        GetChatHistory, //get all messages for user who sent them and user who received them
        GetAllMessagesForUser,
        SendFile,
        GetFile,

        //Contacts
        IsUserMyContact,
        SaveUserAsContact,
        GetContacts,
        DeleteContact,
        GetContactAvatar,

        //Profile commands
        GetAllPersonalityTraits,
        SetMyPersonalityTraits,
        GetMyPersonalityTraits,
        GetOtherUsers,
        GetOtherUserTraits,
        AddUserTrait,
        DeleteUserTrait,
        EditUserTrait,
        UpdateUsername,
        UpdatePassword,
        UpdateAvatar,
        DeleteAccount,
        GetUserAvatar,
        GetOtherUserProfile,

        //DB commands
        GetUserId,
        GetUserFromId,

        None,
        LogOut,
        Test,
        GetUsernameFromId,


        //Server commands
        MessageFromServer
    }
}
