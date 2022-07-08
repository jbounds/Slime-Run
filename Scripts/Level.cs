using Godot;
using System.Linq;

namespace Scripts
{
    public class Level : Label
    {
        public int Experience = 0;
        public float CurrentLevel = 1;
        public System.Collections.Generic.List<LevelData> levelData;// = LevelData.GetLevelList();

        public void UpdatePlayerLevel()
        {
            var currentLevelData = levelData.First(a => a.Level == CurrentLevel);

            if (Experience >= currentLevelData.ExperienceToNextLevel)
            {
                Experience -= currentLevelData.ExperienceToNextLevel;
                CurrentLevel++;
            }
            //? Progress thru levels according LevelData info. 
            //? Experience += emeny experience given. Check if level up, then change number to next level data number.
            this.Text = string.Format("{0}", CurrentLevel);
        }

        public void UpdateEnemyLevel()
        {
            this.Text = string.Format("{0}", CurrentLevel);
        }

        public void ResetLevel()
        {
            CurrentLevel = 1;
            Experience = 0;
            this.UpdatePlayerLevel();
        }
    }
}
