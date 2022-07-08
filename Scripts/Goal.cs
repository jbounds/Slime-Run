using Godot;
using System;

public class Goal : RichTextLabel
{
    public Sprite slime;
    public SlimeData slimeData;

    public override void _Ready()
    {
        slime = (Sprite)this.GetNode("Slime");
        slimeData = SlimeData.CreateSlime(Slime.BasicSlime, 0, 9, Difficulty.Easy);
        base._Ready();
    }

    public void ResetGoal()
    {

    }

    public void UpdateGoal()
    {
        slime.Texture = (Texture)GD.Load("res://Texture/Slimes/" + slimeData.Slime + ".png");
        if (slimeData.Slime == Slime.LavaSlime)
        {
            slime.Hframes = 8;
        }
        else
        {
            slime.Hframes = 12;
        }
    }
}
