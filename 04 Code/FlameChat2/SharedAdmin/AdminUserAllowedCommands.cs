using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatShared.Communication
{
    public enum AdminUserAllowedCommands
    {
        ////Registration
        //SignUp,
        //LogIn,


        ////Message commands
        //SendMessage,
        //ReceiveMessage, 
        //NotifyUserAboutReceivedMessages,
        //GetUnseenMessages,
        //GetUnseenMessagesCount,
        //SeeMessage,
        //GetUsers,

        //GetChatHistory, //get all messages for user who sent them and user who received them
        //SendFile,
        //GetFile,

        ////Contact
        //SaveUserAsContact,
        //DeleteContact,
        //GetContactAvatar, 

        //Profile commands
        //GetUserTraits,
        //AddUserTrait,
        //DeleteUserTrait,
        //EditUserTrait,
        //UpdateUsername,
        //UpdatePassword,
        //UpdateAvatar,
        //DeleteAccount,
        //GetUserAvatar,
        //GetOtherUserTraits,
        //GetOtherUserProfile,

        //Only for admin
        AddManyPersonalityTraits,
        AddPersonalityTrait,
        DeletePersonalityTrait,
        EditPersonalityTrait,
        GetAllPersonalityTraits,
        PersonalityTraitExists,

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
