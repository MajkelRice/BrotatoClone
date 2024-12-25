using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    [Export] public PackedScene EnemyScene { get; set; }
    [Export] public float SpawnInterval = 2.0f;
    [Export] public float SpawnRadius = 300.0f;
    [Export] public int MaxEnemies = 50;

    private float _spawnTimer = 0.0f;

    public override void _Process(double delta)
    {
        _spawnTimer -= (float)delta;

        if (_spawnTimer <= 0 && GetChildren().Count < MaxEnemies)
        {
            SpawnEnemy();
            _spawnTimer = SpawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("EnemyScene is not assigned!");
            return;
        }

        Vector2 spawnOffset = new Vector2(
            GD.Randf() * 2 - 1, 
            GD.Randf() * 2 - 1
        ).Normalized() * GD.Randf() * SpawnRadius;

        var enemyInstance = EnemyScene.Instantiate<CharacterBody2D>();
        if (enemyInstance is not AbstractEnemy abstractEnemy) return;
        AddChild(abstractEnemy);
        abstractEnemy.GlobalPosition = GlobalPosition + spawnOffset;
        
        var player = GetNode<CharacterBody2D>("%Player");
        abstractEnemy.SetTarget(player);
        
    }

    
}