using Godot;
using System;

public class LevelScene : Node2D
{
    Control PauseMenu;

    public override void _Ready()
    {
        var pauseScene = GD.Load("res://Scenes/Paused.tscn") as PackedScene;
        PauseMenu = pauseScene.Instance() as Control;
        GetNode("StaticGUI").GetNode("Pause").Connect("pressed", this, nameof(PauseScreen));
    }
    
    public void PauseScreen()
    {
        this.AddChild(PauseMenu);
        GetTree().Paused = true;
    }
}
