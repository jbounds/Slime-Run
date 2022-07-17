using Godot;

namespace Scripts
{
    public class Slime : RigidBody2D
    {
        public bool FirstCollisionOccured = false;
        const float Gravity = 7f;
        public SlimeData SlimeData;
        public Vector2 Velocity;

        public override void _PhysicsProcess(float delta)
        {
            Velocity.y += delta * Gravity;
        }

        public override void _Ready()
        {

        }
    }
}
