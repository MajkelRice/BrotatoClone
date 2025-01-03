using Godot;
using System;

public partial class FireParticle : AbstractBullet
{
    [Export] public int Damage = 5; // Damage dealt by the fire particle.
    [Export] public float Range = 50f; // Range of the fire particle.

    private CollisionShape2D _collisionShape;

    public override void _Ready()
    {
        base._Ready();

        // Initialize collision shape.
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        if (_collisionShape != null)
        {
            (_collisionShape.Shape as CircleShape2D).Radius = Range;
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is AbstractEnemy enemy)
        {
            enemy.TakeDamage(Damage);
        }
    }

    private void OnTimeout()
    {
        QueueFree();
    }
}