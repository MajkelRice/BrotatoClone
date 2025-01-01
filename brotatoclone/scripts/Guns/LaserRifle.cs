using Godot;
using System;

public partial class LaserRifle : AbstractGun
{
    [Export] public float LaserDuration = 0.5f; // Duration for which the laser remains active.
    [Export] public Color LaserColor = new Color(1, 0, 0); // Default color of the laser.
    [Export] public float LaserWidth = 2f; // Width of the laser beam.

    [Export] public float Damage = 10f;
    
    private Line2D _laserBeam;
    private Timer _laserTimer;

    public override void _Ready()
    {
        base._Ready();

        // Initialize the laser beam.
        _laserBeam = new Line2D();
        _laserBeam.Width = LaserWidth;
        _laserBeam.DefaultColor = LaserColor;
        _laserBeam.Visible = false;
        AddChild(_laserBeam);

        // Initialize the laser timer.
        _laserTimer = new Timer();
        _laserTimer.WaitTime = LaserDuration;
        _laserTimer.OneShot = true;
        _laserTimer.Connect("timeout", new Callable(this, nameof(HideLaser)));
        AddChild(_laserTimer);
    }

    private void _FireLaser()
    {
        var enemiesInRange = GetOverlappingBodies();
        if (enemiesInRange.Count == 0)
            return;

        // Target the closest enemy.
        var target = enemiesInRange[0] as Node2D;
        if (target == null)
            return;

        // Set laser beam start and end points.
        _laserBeam.ClearPoints();
        _laserBeam.AddPoint(ShootingPoint.GlobalPosition);
        _laserBeam.AddPoint(target.GlobalPosition);
        _laserBeam.Visible = true;

        // Deal damage to the target if it's an enemy.
        if (target is AbstractEnemy enemy)
        {
            enemy.TakeDamage(Damage);
        }

        // Start the laser timer to hide the beam after a duration.
        _laserTimer.Start();
    }

    private void HideLaser()
    {
        _laserBeam.Visible = false;
    }

    private new void Timeout()
    {
        _FireLaser();
    }
}