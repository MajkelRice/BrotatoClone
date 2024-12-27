using Godot;
using System;

public partial class AbstractEnemy : CharacterBody2D
{
    [Export] public double Health = 100.0f;
    [Export] public float Speed = 100.0f;
    [Export] public PackedScene ExpScene;
    [Export] public float Damage = 10.0f;

    private TextureProgressBar _hpBar;
    private AnimatedSprite2D _enemySprite;
    
    protected CharacterBody2D Player;
    protected Vector2 TargetPosition;

    public override void _Ready()
    {
        _enemySprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _hpBar = GetNode<TextureProgressBar>("HpBar");
    }

    public override void _Process(double delta)
    {
        MoveTowardsTarget((float)delta);
    }
    
    
    public void SetTarget(CharacterBody2D player)
    {
        Player = player;
        TargetPosition = player.GlobalPosition; // Update the initial target position
    }

    public float GetDamage()
    {
        return Damage;
    }
    
    

    protected void MoveTowardsTarget(float delta)
    {
        if (Player != null)
        {
            TargetPosition = Player.GlobalPosition;
        }
        
        Vector2 direction = (TargetPosition - GlobalPosition).Normalized();
        _enemySprite.FlipH = direction.X switch
        {
            > 0 => false,
            < 0 => true,
            _ => _enemySprite.FlipH
        };
        
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        _hpBar.Value = Health;
        if (Health <= 0) Die();
    }

    protected virtual void Die()
    {
        CallDeferred("deferred_die");
    }

    private void deferred_die()
    {
        var exp = ExpScene.Instantiate<Exp>();
        exp.GlobalPosition = GlobalPosition;
        CallDeferred("deferred_add_exp", exp);
        QueueFree();
    }

    private void deferred_add_exp(Node exp)
    {
        var expGroup = GetTree().Root.GetNode<Node>("Game/ExpGroup");
        expGroup.AddChild(exp);
    }


    
}