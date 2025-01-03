using Godot;
using System;
using System.Collections.Generic;

public partial class AbstractSword : Area2D, IWeapon, ISkillTree
{
    [Export]
    public float DetectionRange = 200f;

    [Export]
    public float SwingDuration = 0.5f;

    [Export]
    public int Damage = 50;

    private Node2D _player;
    private bool _isSwinging = false;

    private Area2D _detectionArea;
    private Area2D _hitboxArea;
    private Node2D _targetEnemy;
    private int _skillPoints = 0;

    public override void _Ready()
    {
        _player = GetParent<Node2D>(); // Assume the sword's parent is the player
        _detectionArea = GetNode<Area2D>("RangeArea");
        _hitboxArea = GetNode<Area2D>("Hitbox");

        _detectionArea.BodyEntered += OnDetectionAreaBodyEntered;
        _hitboxArea.BodyEntered += OnHitboxAreaBodyEntered;
        
    }

    public void _Process(float delta)
    {
        if (!_isSwinging)
        {
            GlobalPosition = _player.GlobalPosition;
        }
    }

    private void OnDetectionAreaBodyEntered(Node body)
    {
        if (!_isSwinging && body is Node2D enemy)
        {
            _targetEnemy = enemy;
            MoveAndSwing();
        }
    }

    private async void MoveAndSwing()
    {
        if (_targetEnemy == null || !_targetEnemy.IsInsideTree())
            return;

        _isSwinging = true;

        // Move to target position
        Vector2 targetPosition = _targetEnemy.GlobalPosition;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "global_position", targetPosition, SwingDuration / 2);
        await ToSignal(tween, "finished");

        // Swing at target
        _hitboxArea.Monitoring = true;
        await ToSignal(GetTree().CreateTimer(SwingDuration / 2), "timeout");
        _hitboxArea.Monitoring = false;

        // Return to player position
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "global_position", _player.GlobalPosition, SwingDuration / 2);
        await ToSignal(tween, "finished");

        _isSwinging = false;
    }

    private void OnHitboxAreaBodyEntered(Node body)
    {
        if (body is AbstractEnemy enemy)
        {
            // Assuming the enemy has a script with a "TakeDamage" method
            enemy.TakeDamage(Damage);
        }
    }

    public Vector2 GetPosition(int slotIndex, int totalSlots, float radius)
    {
        float angle = (slotIndex * 360.0f / totalSlots) * (Mathf.Pi / 180.0f);
        return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
    }

    public void Equip(Node2D parent, Vector2 position)
    {
        Scale = new Vector2(0.5f, 0.5f);
        Position = position;
        parent.AddChild(this);
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
