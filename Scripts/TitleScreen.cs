using Godot;
using System;

public class TitleScreen : Control
{
    public override void _Ready()
    {
        GetNode("Start").Connect("pressed", this, nameof(StartPressed));
        GetNode("Exit").Connect("pressed", this, nameof(ExitPressed));
        GetNode("Settings").Connect("pressed", this, nameof(SettingsPressed));
    }

    public void StartPressed()
    {
        GetTree().ChangeScene("res://Scenes/DifficultySelection.tscn");
    }

    public void ExitPressed()
    {
        GetTree().Quit();
    }

    public void SettingsPressed()
    {
        // Add settings screen.
    }
}
