using Godot;
using System;

namespace Scripts
{
    public class Camera : Camera2D
    {
        public KinematicBody2D Player = null;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Player = (KinematicBody2D)GetParent().GetNode("Player");
        }

        public override void _Process(float delta)
        {
            // Position.x = player.Position.x;
        }
    }
}
