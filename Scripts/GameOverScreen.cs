using Godot;
using Scripts;
using System.Linq;

public class GameOverScreen : Control
{
    public override void _Ready()
    {
        GetNode("Restart").Connect("pressed", this, nameof(RestartPressed));
        GetNode("Home").Connect("pressed", this, nameof(HomePressed));

        var highScoreForDifficulty = (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).HighScoreList.
                    First(a => a.Difficulty == (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).Difficulty).Score;
        (GetNode("HighScoreTextBG").GetNode("HighScoreText") as Label).Text = highScoreForDifficulty.ToString();

        (GetNode("GameDifficultyLabel2")as Label).Text = (GetTree().Root.GetNode("GlobalAttributes") as GlobalAttributes).Difficulty.ToString();

        (GetNode("ScoreTextBG").GetNode("ScoreText") as Label).Text = (GetParent().GetNode("StaticGUI").GetNode("ScoreLabel") as Score).CurrentScore.ToString();
    }

    public void RestartPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeScene("res://Scenes/StaticLevel.tscn");
    }

    public void HomePressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
    }
}
