using Godot;
using Scripts;
using Scripts.Enums;

namespace Scripts
{
    public class GlobalAttributes : Node
    {
        public GlobalAttributes()
        {

        }

        public override void _Ready()
        {
            HighScoreList = new System.Collections.Generic.List<ScoreItem>();
            HighScoreList.Add(new ScoreItem() { Difficulty = DifficultyTypes.Easy, });
            HighScoreList.Add(new ScoreItem() { Difficulty = DifficultyTypes.Normal, });
            HighScoreList.Add(new ScoreItem() { Difficulty = DifficultyTypes.Hard, });
        }

        public DifficultyTypes Difficulty;
        public System.Collections.Generic.List<ScoreItem> HighScoreList;
        public bool IsMobile = OS.GetName() == "Android" || OS.GetName() == "iOS";
        // Set in the setting in the future.
        public bool UseTiltControls = true;
    }
}
