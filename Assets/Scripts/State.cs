using Assets.Scripts.Client.Models;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class State
    {
        public static List<CharacterOptionModel> CharacterOptions { get; set; } = new();
        public static CharacterModel LoggedCharacter { get; set; }

        public static void Clear()
        {
            CharacterOptions = new();
            LoggedCharacter = null;
        }
    }
}