using Godot;

namespace Scripts
{
    public class Slime : KinematicBody2D
    {
        const float gravity = 150.0f;
        public SlimeData SlimeData;
        public Vector2 Velocity;

        public override void _PhysicsProcess(float delta)
        {
            Velocity.y += delta * gravity;

            var collision2D = MoveAndCollide(Velocity * delta);
            if (collision2D != null)
            {
                Velocity = Velocity.Bounce(collision2D.Normal);
                Velocity.y *= 0.2F;
                Velocity.y *= 0.5F;
            }
        }

        public override void _Ready()
        {
            Velocity = new Vector2();
            Velocity.y += 1.5F;
            Velocity *= gravity;
        }
    }
}
