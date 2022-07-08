using Godot;

namespace Scripts
{
    public class Slime : Area2D
    {
        public Level LevelLabel;
        public SlimeData EnemyData;
        public override void _Ready()
        {
            this.Connect("body_entered", this, "_on_Enemy_body_entered");
            LevelLabel = (Level)this.GetNode("LevelLabel");
        }

        public void _on_Enemy_body_entered(Node body)
        {
            if (body is Player)
            {
                var player = (body as Player);
                if (this.LevelLabel.CurrentLevel > player.levelLabel.CurrentLevel)
                {
                    player.handle_hit_death();
                }
                else
                {
                    GetParent().CallDeferred("remove_child", this);
                    player.handle_hit();
                }
            }
        }
    }
}
