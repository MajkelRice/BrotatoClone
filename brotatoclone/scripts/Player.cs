using Godot;
using System;

public partial class Player: CharacterBody2D
{
    [Export] public float Health = 100.0f;
    [Export] public float MoveSpeed = 200.0f; 
    [Export] public float DashSpeed = 400.0f;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 1.0f;
    
    [Export] public float _totalExperience;
    private AnimatedSprite2D _playerSprite;
    private Vector2 _inputDirection = Vector2.Zero;
    private float _dashTimer = 0.0f;
    private float _cooldownTimer = 0.0f;
    private bool _isDashing = false;

    public override void _Ready()
    {
        _playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
    }

    public override void _Process(double delta)
    {
        if (_cooldownTimer > 0) _cooldownTimer -= (float)delta; 

        _inputDirection = new Vector2(
            Input.GetActionStrength("go_right") - Input.GetActionStrength("go_left"),
            Input.GetActionStrength("go_down") - Input.GetActionStrength("go_up")
        );

        if (_inputDirection.LengthSquared() > 0)
            _inputDirection = _inputDirection.Normalized();
        
        if (Input.IsActionJustPressed("go_dash") && _cooldownTimer <= 0) StartDash();
        if (_isDashing)
        {
            _dashTimer -= (float)delta;
            if (_dashTimer <= 0) EndDash();
        }

        if (_inputDirection == Vector2.Zero) _playerSprite.Play("idle");
        else _playerSprite.Play("run");
        
        Velocity = _inputDirection * (_isDashing ? DashSpeed : MoveSpeed);
        _playerSprite.FlipH = _inputDirection.X switch
        {
            > 0 => false,
            < 0 => true,
            _ => _playerSprite.FlipH
        };
        MoveAndSlide();
    }
	
    private void StartDash()
    {
        _isDashing = true;
        _dashTimer = DashDuration;
        _cooldownTimer = DashCooldown;
    }

    private void EndDash()
    {
        _isDashing = false;
    }
}