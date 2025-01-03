using Godot;
using System;

public partial class MiniClusterBomb : AbstractBullet
{
    [Export] public PackedScene ExplosionEffect;
    [Export] public float Lifetime = 2f; // Time before the submunition disappears.
    [Export] public float ExplosionRadius = 50f; // Radius of the mini explosion.
    [Export] public int ExplosionDamage = 20; // Damage dealt by the mini explosion.
    [Export] public float TravelDistance = 20f; // Distance to travel before stopping.
    [Export] public new float Speed = 1f; // Movement speed of the mini bomb.

    private Timer _lifetimeTimer;
    private Vector2 _initialPosition;
    private bool _isMoving = true;

    public override void _Ready()
    {
        base._Ready();

        // Record the initial position.
        _initialPosition = GlobalPosition;

        // Initialize a timer to manage the lifetime of the submunition.
        _lifetimeTimer = new Timer();
        _lifetimeTimer.WaitTime = Lifetime;
        _lifetimeTimer.OneShot = true;
        _lifetimeTimer.Connect("timeout", new Callable(this, nameof(Explode)));
        AddChild(_lifetimeTimer);
        _lifetimeTimer.Start();
    }

    public void _PhysicsProcess(float delta)
    {
        if (!_isMoving) return;
        // Move the bomb forward.
        GlobalPosition += Transform.X * Speed * delta;

        // Stop moving if the travel distance is reached.
        if (GlobalPosition.DistanceTo(_initialPosition) >= TravelDistance)
        {
            _isMoving = false;
        }
    }

    private void OnBodyEntered(Node body)
    {
        // Do nothing on collision.
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

        QueueFree();
    }
}
