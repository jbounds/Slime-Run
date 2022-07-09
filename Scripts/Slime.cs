using Godot;

namespace Scripts
{
    public class Slime : Area2D
    {
        public SlimeData SlimeData;
        public int Speed = 200;
        public Vector2 Velocity;

        public override void _PhysicsProcess(float delta)
        {
            Velocity = new Vector2();
            Velocity.y += 1.5F;
            Velocity *= Speed;

            this.Position += Velocity * delta;
        }

        public override void _Ready()
        {
            this.Connect("body_entered", this, "_on_Enemy_body_entered");
        }

        public void _on_Enemy_body_entered(Node body)
        {
            if (body is Player)
            {
                var player = (body as Player);
                var currentGoalLabel = (Goal)GetParent().GetParent().GetNode("StaticGUI/CurrentGoalLabel");

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
