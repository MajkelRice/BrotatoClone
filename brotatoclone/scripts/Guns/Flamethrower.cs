using Godot;
using System;

public partial class Flamethrower : AbstractGun
{
    [Export] public float ParticleLifetime = 0.5f;

    private Timer _particleTimer;

    public override void _Ready()
    {
        base._Ready();

        // Initialize a timer for emitting fire particles.
        _particleTimer = new Timer();
        _particleTimer.WaitTime = FireRate;
        _particleTimer.OneShot = false;
        _particleTimer.Connect("timeout", new Callable(this, nameof(EmitFireParticle)));
        AddChild(_particleTimer);
    }

    private void EmitFireParticle()
    {
        if (BulletScene == null)
        {
            GD.PrintErr("Fire particle scene not set!");
            return;
        }

        var fireParticle = (Area2D)BulletScene.Instantiate();
        fireParticle.GlobalPosition = ShootingPoint.GlobalPosition;
        fireParticle.GlobalRotation = ShootingPoint.GlobalRotation;

        // Set the lifetime of the fire particle.
        var lifetimeTimer = new Timer();
        lifetimeTimer.WaitTime = ParticleLifetime;
        lifetimeTimer.OneShot = true;
        lifetimeTimer.Connect("timeout", new Callable(fireParticle, "queue_free"));
        fireParticle.AddChild(lifetimeTimer);
        lifetimeTimer.Start();

        GetParent().AddChild(fireParticle);
    }

    public void StartFiring()
    {
        _particleTimer.Start();
    }

    public void StopFiring()
    {
        _particleTimer.Stop();
    }
}