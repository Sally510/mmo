using Assets.Scripts.Helpers;

namespace Assets.Scripts.Client.Models
{
    public class CharacterOptionModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public uint Experience { get; set; }
        public int Level { get => ExperienceHelpers.CalculateLevel(Experience); }

        public static CharacterOptionModel Parse(Packet packet)
        {
            return new CharacterOptionModel
            {
                Id = packet.GetLong(),
                Name = packet.GetBreakString(),
                Experience = packet.GetUInt(),
            };
        }
    }
}
