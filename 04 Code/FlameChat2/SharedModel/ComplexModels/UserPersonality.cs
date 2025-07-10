using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ComplexModels
{
    public class UserPersonality
    {
        public List<PersonalityTrait> Traits { get; set; }
        public int UserId { get; set; }
    }
}
