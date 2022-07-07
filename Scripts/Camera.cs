using Godot;
using System;

public class Camera : Camera2D
{
    public KinematicBody2D player = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = (KinematicBody2D)GetParent().GetNode("Player");
    }

    public override void _Process(float delta)
    {
        // Position.x = player.Position.x;
    }
}
