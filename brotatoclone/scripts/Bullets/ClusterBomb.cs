using Godot;
using System;

public partial class ClusterBomb : AbstractBullet
{
    [Export] public PackedScene ExplosionEffect;
    [Export] public float ExplosionRadius = 50f; // Radius of the initial explosion.
    [Export] public int ExplosionDamage = 50; // Damage dealt by the initial explosion.
    [Export] public PackedScene SubmunitionScene; // Scene for the smaller bombs.
    [Export] public int SubmunitionCount = 5; // Number of smaller bombs released.
    
    private Area2D explosionArea;


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
        if (body is AbstractEnemy enemy )
        {
            CallDeferred(nameof(Explode));
            CallDeferred(nameof(ReleaseSubmunitions));
        }
        QueueFree();
    }

    private void Explode()
    {
        if (ExplosionEffect != null)
        {
            var explosion = (ExplosionEffect)ExplosionEffect.Instantiate();
            explosion.GlobalPosition = GlobalPosition; // Use the rocket's position
            explosion.ExplosionRadius = ExplosionRadius;
            GetTree().Root.GetNode("Game/Explosions").AddChild(explosion); // Adjust path to match your scene
            explosion.ExplosionRadius = ExplosionRadius; // Set explosion size
        }

        // Apply damage to enemies within radius
        var overlappingBodies = explosionArea.GetOverlappingBodies();
        foreach (Node body in overlappingBodies)
        {
            if (body is AbstractEnemy enemy )
            {
                enemy.TakeDamage(ExplosionDamage);
            }
        }

        //CallDeferred(nameof(ReleaseSubmunitions));
    }

    private void ReleaseSubmunitions()
    {
        if (SubmunitionScene == null)
        {
            GD.PrintErr("Submunition scene not set!");
            return;
        }

        for (int i = 0; i < SubmunitionCount; i++)
        {
            AbstractBullet submunition = (AbstractBullet)SubmunitionScene.Instantiate();
            submunition.GlobalPosition = GlobalPosition;

            // Spread submunitions in random directions.
            float angle = Mathf.DegToRad((360f / SubmunitionCount) * i);
            submunition.GlobalRotation = angle;

            GetParent().AddChild(submunition);
        }
    }
}