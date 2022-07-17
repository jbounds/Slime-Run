using Godot;
using Scripts.Enums;

public class DifficultyScreen : Control
{
    public override void _Ready()
    {
        GetNode("Easy").Connect("pressed", this, nameof(EasyPressed));
        GetNode("Easy").Connect("button_down", this, nameof(ButtonDown));
        GetNode("Medium").Connect("pressed", this, nameof(MediumPressed));
        GetNode("Hard").Connect("pressed", this, nameof(HardPressed));
    }

    public void ButtonDown()
    {
        var button = (TextureButton)GetNode("Easy");
        button.Modulate = new Color(0.6f, 0.6f, 0.6f);
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
