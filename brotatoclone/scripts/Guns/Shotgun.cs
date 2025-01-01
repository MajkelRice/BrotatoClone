using Godot;
using System;

public partial class Shotgun : AbstractGun
{
    [Export] public int PelletCount = 5; // Number of pellets fired in one shot.
    [Export] public float SpreadAngle = 30f; // Total spread angle in degrees.

    private void _FireShotgun()
    {
        if (BulletScene == null)
        {
            GD.PrintErr("Bullet scene not set!");
            return;
        }

        float halfSpread = SpreadAngle / 2;
        for (int i = 0; i < PelletCount; i++)
        {
            // Calculate angle offset for each pellet.
            float angleOffset = -halfSpread + (i * (SpreadAngle / (PelletCount - 1)));

            var bullet = (AbstractBullet)BulletScene.Instantiate();
            bullet.GlobalPosition = ShootingPoint.GlobalPosition;
            bullet.GlobalRotation = ShootingPoint.GlobalRotation + Mathf.DegToRad(angleOffset);

            GetParent().AddChild(bullet); // Add bullet to the parent node, not ShootingPoint.
        }
    }

    private new void Timeout()
    {
        if (GetOverlappingBodies().Count > 0)
            _FireShotgun();
    }
}