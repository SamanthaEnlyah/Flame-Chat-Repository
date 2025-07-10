using FlameChatClient.ChatClient;
using FlameChatShared.Communication;
using SharedModel.ComplexModels;
using SharedModel.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace FlameChatClient.Client_Operations
{
    public class PersonalityTraitsOp
    {

        public async Task SetMyPersonalityTraits(UserPersonality userPersonality)
        {      
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.SetMyPersonalityTraits, userPersonality);
        }

        public async Task<List<PersonalityTrait>> GetAllPersonalityTraits()
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetAllPersonalityTraits, null);
            Response r = await GlobalVariables.GetClient().GetResponse();
            List<PersonalityTrait> personalityTraits = (List<PersonalityTrait>)r.Value;
            return personalityTraits;
        }

        public async Task<bool> SaveUserHasPersonalityTraits(UserPersonality userPersonality)
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.SetMyPersonalityTraits, userPersonality);
            Response r = await GlobalVariables.GetClient().GetResponse();
            return (bool) r.Value;
            
        }

        public async Task<List<PersonalityTrait>> GetMyPersonalityTraits(int chatUserId)
        {
            await GlobalVariables.GetClient().SendCommandToServer(RegularUserAllowedCommands.GetMyPersonalityTraits, chatUserId);
            Response r = await GlobalVariables.GetClient().GetResponse();

            //MessageBox.Show("response value:"+r.Value.ToString());
            //if (((List<ChatUser>)r.Value).Count == 0)
            //{
            //    MessageBox.Show("no pers traits");
            //}
            ////GlobalVariables.LogMessage("first user:" + ((List<ChatUser>)r.Value).First().Username);

           // MessageBox.Show(((List<ChatUser>)r.Value).Count +"");
            
            List<PersonalityTrait> personalityTraits = (List<PersonalityTrait>)r.Value;

            

            return personalityTraits;
        }
    }
}
