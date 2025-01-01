using Godot;
using System;

public partial class AbstractGun : Area2D, ISkillTree
{
    [Export]
    public PackedScene BulletScene;

    [Export]
    public float FireRate = 0.01f;

    [Export] public Marker2D ShootingPoint;

    [Export] public CollisionShape2D Range;
    [Export] public Timer FireTimer;
    private float _fireCooldown = 0f;

    private int _skillPoints = 0;
    
    public override void _Ready()
    {
        FireTimer.WaitTime = FireRate;
    }

    public override void _Process(double delta)
    {
        var enemiesInRange = GetOverlappingBodies();
        if (enemiesInRange.Count > 0)
        {
            var target = enemiesInRange[0];
            LookAt(target.GlobalPosition);
        }
    }

    protected virtual void _Fire()
    {
        if (BulletScene == null)
        {
            GD.PrintErr("Bullet scene not set!");
            return;
        }

        var bullet = (AbstractBullet)BulletScene.Instantiate();
        
        bullet.GlobalPosition = ShootingPoint.GlobalPosition;
        bullet.GlobalRotation = ShootingPoint.GlobalRotation;
        ShootingPoint.AddChild(bullet);
    }



    private void Timeout()
    {
        if (GetOverlappingBodies().Count > 0) _Fire();
    }

    public int GetSkillPoints()
    {
        return _skillPoints;
    }

    public void DecrementSkillPoints()
    {
        _skillPoints--;
    }
}