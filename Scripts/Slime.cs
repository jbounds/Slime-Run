using Godot;

namespace Scripts
{
    public class Slime : Area2D
    {
        public SlimeData SlimeData;
        public override void _Ready()
        {
            this.Connect("body_entered", this, "_on_Enemy_body_entered");
        }

        public void _on_Enemy_body_entered(Node body)
        {
            if (body is Player)
            {
                var player = (body as Player);
                var currentGoalLabel = (Goal)GetParent().GetNode("Player/UserInterface/CurrentGoalLabel");

                if (this.SlimeData.Slime != currentGoalLabel.SlimeData.Slime)
                {
                    GetParent().CallDeferred("remove_child", this);
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
