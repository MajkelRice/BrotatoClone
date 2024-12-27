using Godot;
using System;

public partial class Player: CharacterBody2D
{
    [Export] public float Health = 100.0f;
    [Export] public float MoveSpeed = 200.0f; 
    [Export] public float DashSpeed = 400.0f;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 1.0f;

    [Export] public float IFrameDuration = 1.0f;
    
    [Export] public float _totalExperience = 30f;
    [Export] public Area2D HurtZone;
    [Export] public TextureProgressBar HpBar;
    [Export] public TextureProgressBar ExpBar;
    private AnimatedSprite2D _playerSprite;
    private Vector2 _inputDirection = Vector2.Zero;
    
    private float _dashTimer = 0.0f;
    private float _cooldownTimer = 0.0f;
    private bool _isDashing = false;
    
    private float _iFrameTimer = 0.0f;
    private float _iFrameCooldown = 0.0f;
    private bool _isIFraming = false;
    
    private int _flashCount = 0;
    private float _flashInterval = 0.1f;
    private float _flashTimer = 0.0f;

    public override void _Ready()
    {
        _playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        HpBar.MaxValue = Health;
        HpBar.Value = Health;
        ExpBar.Value = _totalExperience;
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

        if (_isIFraming)
        {
            _iFrameTimer -= (float)delta;
            _flashTimer -= (float)delta;

            if (_flashTimer <= 0 && _flashCount < 4) // 4 transitions: Red -> Normal -> Red -> Normal
            {
                _playerSprite.Modulate = _flashCount % 2 == 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 1); // Alternate between red and normal
                _flashCount++;
                _flashTimer = _flashInterval;
            }

            if (_iFrameTimer <= 0) EndIFrame();
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

        // Damage section
        foreach (var body in HurtZone.GetOverlappingBodies())
        {
            if (body is AbstractEnemy enemy && !_isIFraming)
            {
                Health -= enemy.GetDamage();
                HpBar.Value = Health;
                StartIFrame();
                GD.Print($"Player damaged by {enemy.Name}. Remaining health: {Health}");
                if (Health <= 0)
                {
                    Die();
                }
                break;
            }
        }
    }
	
    private void Die()
    {
        QueueFree();
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

    private void StartIFrame()
    {
        _isIFraming = true;
        _iFrameTimer = IFrameDuration;
        _flashTimer = 0.0f;
        _flashCount = 0;
        _playerSprite.Modulate = new Color(1, 0, 0, 1); // Start with red
    }

    private void EndIFrame()
    {
        _isIFraming = false;
        _playerSprite.Modulate = new Color(1, 1, 1, 1); // Restore original appearance
    }
}