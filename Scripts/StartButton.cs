using Godot;
using System;

public class StartButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(StartPressed));
    }

    public void StartPressed()
    {
        GetTree().ChangeScene("res://Scenes/DifficultySelection.tscn");
    }
}
