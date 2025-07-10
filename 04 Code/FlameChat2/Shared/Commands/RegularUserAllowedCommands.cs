using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatServer.Communication
{
    public enum RegularUserAllowedCommands
    {
        //Registration
        CheckIfUsernameExists,
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
        SaveUserAsContact,
        DeleteContact,
        GetContactAvatar,
        GetChatHistory, //get all meessages for user who sent them and user who received them
        SendFile,
        GetFile,


        //Profile commands
        GetOtherUsers,
        GetUserTraits,
        AddUserTrait,
        DeleteUserTrait,
        EditUserTrait,
        ChangeUsername,
        ChangePassword,
        ChangeAvatar,
        DeleteAccount,
        GetUserAvatar,
        GetOtherUserTraits,
        GetOtherUserProfile


    }
}
