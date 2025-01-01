using Godot;
using System;

public partial class BurstRifle : AbstractGun
{
    [Export] public int BulletsPerBurst = 3; // Number of bullets in one burst
    [Export] public float BurstInterval = 0.02f; // Time between each bullet in a burst

    private Timer _burstTimer;
    private int _bulletsFired;

    public override void _Ready()
    {
        base._Ready();
        _burstTimer = new Timer();
        AddChild(_burstTimer);
        _burstTimer.WaitTime = BurstInterval;
        _burstTimer.OneShot = true;
        _burstTimer.Timeout += OnBurstTimerTimeout;
    }

    protected override void _Fire()
    {
        _bulletsFired = 0; // Reset burst counter
        FireBullet();
        if (BulletsPerBurst > 1)
        {
            _burstTimer.Start(); // Start firing additional bullets
        }
    }

    private void FireBullet()
    {
        if (BulletScene == null)
        {
            GD.PrintErr("Bullet scene not set!");
            return;
        }

        var bullet = (AbstractBullet)BulletScene.Instantiate();
        bullet.GlobalPosition = ShootingPoint.GlobalPosition;
        bullet.GlobalRotation = ShootingPoint.GlobalRotation;
        GetParent().AddChild(bullet);
        _bulletsFired++;
    }

    private void OnBurstTimerTimeout()
    {
        if (_bulletsFired < BulletsPerBurst)
        {
            FireBullet();
            _burstTimer.Start(); // Continue the burst
        }
    }
}