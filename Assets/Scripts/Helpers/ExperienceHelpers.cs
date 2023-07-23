
using System;

namespace Assets.Scripts.Helpers
{
    internal static class ExperienceHelpers
    {
        private const float EXPERIENCE_CONSTANT = 0.05f;
        private const int MAX_LEVEL = 20;

        private static int NormalizeLevel(int levelIndex)
        {
            int level = levelIndex + 1;

            if (level > MAX_LEVEL)
            {
                return MAX_LEVEL;
            }

            return level;
        }

        public static int CalculateLevel(uint experience)
        {
            int levelIndex = (int)(EXPERIENCE_CONSTANT * Math.Sqrt(experience));
            return NormalizeLevel(levelIndex);
        }

        public static uint CalculateExperienceAtLevel(int level)
        {
            return (uint)Math.Pow(level / EXPERIENCE_CONSTANT, 2);
        }

        public static (uint, uint) CalculateExperienceRangeAtLevel(int level)
        {
            uint lowerBound = CalculateExperienceAtLevel(level - 1);
            uint upperBound = CalculateExperienceAtLevel(level);

            return (lowerBound, upperBound);
        }
    }
}
