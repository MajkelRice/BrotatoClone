using Godot;
using System;

public partial class PoisionBullet : AbstractBullet
{
    [Export] public PackedScene ExplosionEffect;
    [Export] public float ExplosionRadius = 50f;
    [Export] public int ExplosionDamage = 50;

    private Area2D explosionArea; // Reference to the detection area

    public override void _Ready()
    {
        // Ensure the Area2D is set up
        explosionArea = GetNode<Area2D>("ExplosionArea");
        var collisionShape = explosionArea.GetNode<CollisionShape2D>("CollisionShape2D");
        if (collisionShape != null && collisionShape.Shape is CircleShape2D circleShape)
        {
            circleShape.Radius = ExplosionRadius; // Set the radius dynamically
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is AbstractEnemy)
        {
            Explode();
        }

        QueueFree(); // Remove the rocket upon impact
    }

    private void Explode()
    {
        // Spawn the explosion effect
        if (ExplosionEffect != null)
        {
            var explosion = (ExplosionEffect)ExplosionEffect.Instantiate();
            explosion.GlobalPosition = GlobalPosition; // Use the bullet's position
            explosion.ExplosionRadius = ExplosionRadius;
            GetTree().Root.GetNode("Game/Explosions").AddChild(explosion); // Adjust path to match your scene
        }

        // Apply damage to enemies within the explosion radius
        var overlappingBodies = explosionArea.GetOverlappingBodies();
        foreach (Node body in overlappingBodies)
        {
            if (body is AbstractEnemy enemy)
            {
                enemy.Poison(ExplosionDamage);
            }
        }
    }
}