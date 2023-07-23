using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class LoginModel
    {
        public StatusType Status { get; set; }
        public List<CharacterOptionModel> CharacterList { get; set; } = new();


        public enum StatusType
        {
            Success = 1,
            AlreadyLoggedIn = 2,
            InvalidInformation = 3,
        }
    }
}
