using Godot;
using Scripts.Enums;

namespace Scripts
{
    public class Goal : RichTextLabel
    {
        public Sprite Slime;
        public SlimeData SlimeData;

        public override void _Ready()
        {
            Slime = (Sprite)this.GetNode("Slime");
            SlimeData = SlimeData.CreateSlime(SlimeTypes.BasicSlime, 0, 9, Scripts.Enums.DifficultyTypes.Easy);
            base._Ready();
        }

        public void ResetGoal()
        {

        }

        public void UpdateGoal()
        {
            Slime.Texture = (Texture)GD.Load("res://Texture/Slimes/" + SlimeData.Slime + ".png");
            if (SlimeData.Slime == SlimeTypes.LavaSlime)
            {
                Slime.Hframes = 8;
            }
            else
            {
                Slime.Hframes = 12;
            }
        }
    }
}
