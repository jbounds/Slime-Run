using Godot;
using Scripts.Enums;

namespace Scripts
{
    public class Goal : NinePatchRect
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
        }
    }
}
