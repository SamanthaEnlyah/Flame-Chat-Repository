using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatServer.Communication
{
    public enum AdminUserAllowedCommands
    {
        //Registration
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
        GetOtherUserProfile,

        //Only for admin
        AddPersonalityTrait,
        DeletePersonalityTrait,
        EditPersonalityTrait,
        GetAllPersonalityTraits,

        //Admin Reports 
        HowManyRegularUsers,
        HowManyAdmins,
        HowManyMessagesSent,
        HowManyMessagesReceived,
        HowManyMessagesSentInTotal,
        HowManyMessagesSentInLast30Days,
        GetAllUsers,
        GetUserPersonalityTraits



    }
}
