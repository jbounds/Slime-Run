using Godot;

namespace Scripts
{
    public class Slime : KinematicBody2D
    {
        // Vector2 Direction = new Vector2();
        const float Gravity = 200.0f;
        public SlimeData SlimeData;
        public Vector2 Velocity;

        public override void _PhysicsProcess(float delta)
        {
            Velocity.y += delta * Gravity;

            var collision2D = MoveAndSlide(Velocity, infiniteInertia: false);

            // Want to slow down first collision with the basket, but continual collisions should be slowed down TODO FIX THIS.
            if (GetSlideCount() > 0)
            {
                var collision = GetSlideCollision(0);
                if (collision != null)
                {
                    Velocity = Velocity.Bounce(collision.Normal);
                    if (collision.Collider is Slime)
                    {
                        Velocity.x *= 0.25F;
                        if (Velocity.y < 0)
                        {
                            Velocity.y *= 0.25F;
                        }
                    }
                    else if (collision.Collider is Player)
                    {
                        Velocity.x *= 0.75F;
                        Velocity.y *= 0.75F;
                    }
                }
            }
        }

        public override void _Ready()
        {
            Velocity = new Vector2();
            Velocity.y += 1F;
            Velocity *= Gravity;
        }
    }
}
