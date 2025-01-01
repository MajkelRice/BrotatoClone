using Godot;
using System;

public partial class LaserBullet : AbstractBullet
{


    public void _FireLaser(Marker2D shootingPoint, Line2D laserBeam, Timer laserTimer)
    {
        var enemiesInRange = GetOverlappingBodies();
        if (enemiesInRange.Count == 0)
            return;

        // Target the closest enemy.
        var target = enemiesInRange[0] as Node2D;
        if (target == null)
            return;

        // Set laser beam start and end points.
        laserBeam.ClearPoints();
        laserBeam.AddPoint(shootingPoint.GlobalPosition);
        laserBeam.AddPoint(target.GlobalPosition);
        laserBeam.Visible = true;

        // Deal damage to the target if it's an enemy.
        if (target is AbstractEnemy enemy)
        {
            enemy.TakeDamage(Damage);
        }

        // Start the laser timer to hide the beam after a duration.
        laserTimer.Start();
    }
}
