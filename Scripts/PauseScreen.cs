using Godot;
using System;

public class PauseScreen : Control
{
    public override void _Ready()
    {
        GetNode("Resume").Connect("pressed", this, nameof(ResumePressed));
        GetNode("Restart").Connect("pressed", this, nameof(RestartPressed));
        GetNode("Home").Connect("pressed", this, nameof(HomePressed));
        GetNode("Settings").Connect("pressed", this, nameof(SettingsPressed));
    }

    public void ResumePressed()
    {
        GetTree().Paused = false;
        this.GetParent().RemoveChild(this);
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

    public void SettingsPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeScene("res://Scenes/Options.tscn");
    }
}
