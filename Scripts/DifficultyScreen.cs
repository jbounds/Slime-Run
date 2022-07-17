using Godot;
using Scripts.Enums;

public class DifficultyScreen : Control
{
    public override void _Ready()
    {
        GetNode("Back").Connect("pressed", this, nameof(BackPressed));
        GetNode("Easy").Connect("pressed", this, nameof(EasyPressed));
        GetNode("Medium").Connect("pressed", this, nameof(MediumPressed));
        GetNode("Hard").Connect("pressed", this, nameof(HardPressed));
    }

    public void BackPressed()
    {
        GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
    }
    
    public void EasyPressed()
    {
        (GetTree().Root.GetNode("GlobalAttributes") as Scripts.GlobalAttributes).Difficulty = DifficultyTypes.Easy;
        GetTree().ChangeScene("res://Scenes/StaticLevel.tscn");
    }

    public void MediumPressed()
    {
        (GetTree().Root.GetNode("GlobalAttributes") as Scripts.GlobalAttributes).Difficulty = DifficultyTypes.Normal;
        GetTree().ChangeScene("res://Scenes/StaticLevel.tscn");
    }

    public void HardPressed()
    {
        (GetTree().Root.GetNode("GlobalAttributes") as Scripts.GlobalAttributes).Difficulty = DifficultyTypes.Hard;
        GetTree().ChangeScene("res://Scenes/StaticLevel.tscn");
    }
}
