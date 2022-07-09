using Godot;
using Scripts;
using Scripts.Enums;

namespace Scripts
{
    public class SlimeData
    {
        public SlimeData()
        {

        }

        public DifficultyTypes AssociatedDifficulty;
        public int MaxLevel;
        public int MinLevel;
        public SlimeTypes Slime;

        public static System.Collections.Generic.List<SlimeData> GetSlimeList()
        {
            var enemyList = new System.Collections.Generic.List<SlimeData>();

            enemyList.Add(CreateSlime(SlimeTypes.BasicSlime, 0, 9, DifficultyTypes.Easy));
            enemyList.Add(CreateSlime(SlimeTypes.BunnySlime, 10, 19, DifficultyTypes.Easy));
            enemyList.Add(CreateSlime(SlimeTypes.WaterSlime, 20, 29, DifficultyTypes.Easy));
            enemyList.Add(CreateSlime(SlimeTypes.PandaSlime, 30, 39, DifficultyTypes.Normal));
            enemyList.Add(CreateSlime(SlimeTypes.PufferSlime, 40, 49, DifficultyTypes.Normal));
            enemyList.Add(CreateSlime(SlimeTypes.LavaSlime, 50, 59, DifficultyTypes.Hard));
            enemyList.Add(CreateSlime(SlimeTypes.FoxSlime, 50, 59, DifficultyTypes.Hard));

            return enemyList;
        }

        public static SlimeData CreateSlime(SlimeTypes name, int minLevel, int maxLevel, DifficultyTypes difficulty)
        {
            return new SlimeData()
            {
                Slime = name,
                MinLevel = minLevel,
                MaxLevel = maxLevel,
                AssociatedDifficulty = difficulty
            };
        }
    }
}
