using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class LoginCharacterModel
    {
        public bool Success { get; set; }

        public CharacterModel SelecetedCharacter { get; set; }
        public List<CharacterModel> MapCharacter { get; set; } = new List<CharacterModel>();
    }
}
