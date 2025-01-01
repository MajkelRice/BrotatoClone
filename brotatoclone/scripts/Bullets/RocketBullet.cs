using Godot;
using System;

public partial class RocketBullet : AbstractBullet
{
    [Export] public PackedScene ExplosionEffect;
    [Export] public float ExplosionRadius = 100f;
    [Export] public int ExplosionDamage = 50;

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void OnBodyEntered(Node body)
    {
        if (body is AbstractEnemy enemy)
        {
            Explode();
        }

        QueueFree(); // Remove the rocket upon impact
    }

    private void Explode()
    {
        if (ExplosionEffect != null)
        {
            var explosion = (ExplosionEffect)ExplosionEffect.Instantiate();
            explosion.GlobalPosition = GlobalPosition; // Use the rocket's position
            GetTree().Root.GetNode("Game/Explosions").AddChild(explosion); // Adjust path to match your scene
            explosion.ExplosionRadius = ExplosionRadius; // Set explosion size
        }

        // Apply damage to enemies within radius
        var bodies = GetOverlappingBodies();
        foreach (Node body in bodies)
        {
            if (body is AbstractEnemy enemy)
            {
                enemy.TakeDamage(ExplosionDamage);
            }
        }
    }



}