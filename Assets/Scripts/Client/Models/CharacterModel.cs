using Assets.Scripts.Helpers;

namespace Assets.Scripts.Client.Models
{
    public class CharacterModel
    {
        public uint EntityId { get; set; }
        public int MapId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public double Angle { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public uint Experience { get; set; }

        //public DirectionType Direction { get => PositionHelpers.GetDirectionTypeBetweenAngles(Angle); }

        public int Level { get => ExperienceHelpers.CalculateLevel(Experience); }
    }
}
