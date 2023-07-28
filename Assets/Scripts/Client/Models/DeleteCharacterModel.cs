using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class DeleteCharacterModel
    {
        public bool Success { get; set; }
        public List<CharacterOptionModel> CharacterList { get; set; } = new();
    }
}
