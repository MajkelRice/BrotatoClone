using Godot;
using System.Collections.Generic;

public partial class WaveManager : Node
{
    [Export] public PackedScene EnemyScene;
    [Export] public PackedScene WarningMarkerScene; // Scene for the warning marker (red cross)
    [Export] public int InitialEnemyCount = 5;
    [Export] public float SpawnRadius = 300f;
    [Export] public float WaveDuration = 30f; // Duration of each wave in seconds
    [Export] public float BreakDuration = 10f; // Duration of the break between waves in seconds
    [Export] public int EnemyIncreasePerWave = 2;

    private int _currentWave = 0;
    private Timer _waveTimer;
    private Timer _spawnTimer;
    private Timer _breakTimer;
    private CharacterBody2D _player;
    private bool _isWaveActive = false;

    public override void _Ready()
    {
        // Initialize timers
        _waveTimer = new Timer();
        AddChild(_waveTimer);
        _waveTimer.WaitTime = WaveDuration;
        _waveTimer.OneShot = true;
        _waveTimer.Timeout += OnWaveEnd;

        _breakTimer = new Timer();
        AddChild(_breakTimer);
        _breakTimer.WaitTime = BreakDuration;
        _breakTimer.OneShot = true;
        _breakTimer.Timeout += StartNewWave;

        _spawnTimer = new Timer();
        AddChild(_spawnTimer);
        _spawnTimer.WaitTime = 2f; // Adjust spawn interval as needed
        _spawnTimer.OneShot = false;
        _spawnTimer.Timeout += SpawnEnemyGroup;

        // Get reference to the player
        _player = GetNode<CharacterBody2D>("%Player");

        StartNewWave();
    }

    private void StartNewWave()
    {
        _currentWave++;
        _isWaveActive = true;

        GD.Print($"Starting Wave {_currentWave}.");

        _waveTimer.Start();
        _spawnTimer.Start();
    }

    private void OnWaveEnd()
    {
        GD.Print($"Wave {_currentWave} ended. Starting break.");

        _isWaveActive = false;
        _spawnTimer.Stop();
        _breakTimer.Start();
    }

    private void SpawnEnemyGroup()
    {
        if (!_isWaveActive) return;

        int groupSize = (int)GD.RandRange(3, 8); // Random group size

        for (int i = 0; i < groupSize; i++)
        {
            QueueWarningAndSpawn();
        }
    }

    private void QueueWarningAndSpawn()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        if (WarningMarkerScene != null)
        {
            Node2D warningMarker = WarningMarkerScene.Instantiate<Node2D>();
            warningMarker.Position = spawnPosition;
            AddChild(warningMarker);

            Timer warningTimer = new Timer();
            AddChild(warningTimer);
            warningTimer.WaitTime = 1f; // Show warning for 1 second
            warningTimer.OneShot = true;
            warningTimer.Timeout += () =>
            {
                warningMarker.QueueFree();
                SpawnEnemyAtPosition(spawnPosition);
            };
            warningTimer.Start();
        }
        else
        {
            SpawnEnemyAtPosition(spawnPosition);
        }
    }

    private void SpawnEnemyAtPosition(Vector2 position)
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("EnemyScene is not assigned!");
            return;
        }

        AbstractEnemy enemy = EnemyScene.Instantiate<AbstractEnemy>();
        enemy.Position = position;
        AddChild(enemy);
        enemy.SetTarget(_player);
        
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // Generate a random position within a circle around the player or center of the game
        Vector2 randomDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
        return randomDirection * (float)GD.RandRange(0, SpawnRadius);
    }
}


