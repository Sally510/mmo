using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class CreateCharacterModel
    {
        public StatusType Status { get; set; }
        public List<CharacterOptionModel> CharacterList { get; set; } = new();


        public enum StatusType
        {
            Success = 1,
            NameExists = 2,
            TooManyCharacters = 3,
            Unknown = 4,
        }
    }
}
