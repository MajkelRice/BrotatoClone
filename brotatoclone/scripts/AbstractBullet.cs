using Godot;
using System;

public partial class AbstractBullet : Area2D, ISkillTree
{
    [Export] public float Speed = 400f;
    
    [Export] public int Damage = 10;
    
    private Vector2 _direction;
    private double _traveledDistance;
    protected double _range = 1200f;

    private int _skillPoints = 0;
    

    public override void _Process(double delta)
    {
        _direction = Vector2.Right.Rotated(Rotation);
        Position += _direction * Speed * (float)delta;
        _traveledDistance += Speed * delta;
        if (_traveledDistance >= _range) QueueFree();
    }

    private void OnBodyEntered(Node body)
    {
        if (body is not AbstractEnemy enemy) return;
        enemy.TakeDamage(Damage);
        QueueFree();
    }

    private void OnBulletScreenExited()
    {
        QueueFree();
    }

    public int GetSkillPoints()
    {
        return _skillPoints;
    }

    public void DecrementSkillPoints()
    {
        _skillPoints--;
    }

    public float GetDamage()
    {
        return Damage;
    }
}