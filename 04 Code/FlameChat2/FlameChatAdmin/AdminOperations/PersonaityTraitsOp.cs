using Azure.Core.Pipeline;
using FlameChatClient.ChatClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatAdmin.AdminOperations
{
    public class PersonaityTraitsOp
    {

        AdminClient client;

        public PersonaityTraitsOp()
        {
             //client = GlobalVariables.GetClient();
           

        }

        public void AddPersonalityTrait(string trait)
        {
            // Code to add a personality trait
            if (string.IsNullOrEmpty(trait))
            {
                throw new ArgumentException("Trait cannot be null or empty");
                return;
            }
            // Add the trait to the database or list





        }
        public void GetAllPersonalityTraits()
        {
            // Code to get all personality traits

            
        }
    }
}
