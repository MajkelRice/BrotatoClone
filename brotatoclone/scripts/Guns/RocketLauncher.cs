using Godot;
using System;

public partial class RocketLauncher : AbstractGun
{
    public override void _Ready()
    {
        base._Ready();
    }

    private void Timeout()
    {
        if (GetOverlappingBodies().Count > 0)
        {
            FireRocket();
        }
    }

    private void FireRocket()
    {
        if (BulletScene == null)
        {
            GD.PrintErr("Rocket scene not set!");
            return;
        }

        var rocket = (RocketBullet)BulletScene.Instantiate();
        rocket.GlobalPosition = ShootingPoint.GlobalPosition;
        rocket.GlobalRotation = ShootingPoint.GlobalRotation;
        GetParent().AddChild(rocket);
    }
}