using Godot;
using System;

public class ScoreArea : Area2D
{
    public Scripts.Player Player;
    public override void _Ready()
    {   
        Player = (Scripts.Player)GetParent();
        this.Connect("body_entered", this, nameof(HandleHit));
    }

    public virtual void HandleHit(Node body)
    {
        if (body is Scripts.Slime)
        {
            Player.HandleHit((Scripts.Slime)body);
        }
    }
}
