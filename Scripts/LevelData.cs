using Godot;
using System;

namespace Scripts
{
    public class LevelData
    {
        public LevelData()
        {

        }

        public int ExperienceToNextLevel;
        public int Level;

        public static System.Collections.Generic.List<LevelData> GetLevelList()
        {
            var levelList = new System.Collections.Generic.List<LevelData>();

            levelList.Add(CreateLevel(1, 1));
            levelList.Add(CreateLevel(2, 2));
            levelList.Add(CreateLevel(3, 3));
            levelList.Add(CreateLevel(4, 4));
            levelList.Add(CreateLevel(5, 5));
            levelList.Add(CreateLevel(6, 6));
            levelList.Add(CreateLevel(7, 7));
            levelList.Add(CreateLevel(8, 8));
            levelList.Add(CreateLevel(9, 9));
            levelList.Add(CreateLevel(10, 10));
            levelList.Add(CreateLevel(11, 15));

            return levelList;
        }

        private static LevelData CreateLevel(int level, int expRequired)
        {
            return new LevelData()
            {
                Level = level,
                ExperienceToNextLevel = expRequired
            };
        }
    }
}
