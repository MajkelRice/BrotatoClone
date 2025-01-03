using Godot;
using System;
using System.Collections.Generic;

public partial class LightningBullet : AbstractBullet, ISkillTree
{
    [Export] public PackedScene BulletScene;
    [Export] public float ChainRange = 300f; // Distance between chained targets
    [Export] public int MaxChains = 3; // Number of enemies the lightning can chain to

    private List<Node> _chainedEnemies = new List<Node>(); // Track chained enemies
    
    private void OnBodyEntered(Node body)
    {
        if (body is not AbstractEnemy enemy || _chainedEnemies.Contains(body)) return;

        enemy.TakeDamage(Damage);
        _chainedEnemies.Add(body);

        // Chain to another enemy if possible
        if (_chainedEnemies.Count < MaxChains)
        {
            ChainToNextEnemy();
        }
        else
        {
            QueueFree(); // Stop chaining after reaching the max chains
        }
    }

    private void ChainToNextEnemy()
    {
        var potentialTargets = GetPotentialTargets();
    
        if (potentialTargets.Count > 0)
        {
            var closestTarget = FindClosestEnemy(potentialTargets);
        
            if (closestTarget != null) // Ensure the closest target is not null
            {
                // Create a new bullet for the next chain
                var newBullet = (LightningBullet)BulletScene.Instantiate();
                newBullet.GlobalPosition = closestTarget.GlobalPosition;
                newBullet.GlobalRotation = (closestTarget.GlobalPosition - Position).Angle();
                AddChild(newBullet);
            }
            else
            {
                GD.PrintErr("No valid target found to chain to.");
            }
        }
        else
        {
            GD.PrintErr("No potential targets found for chaining.");
        }
    }


    private List<AbstractEnemy> GetPotentialTargets()
    {
        var enemiesInRange = new List<AbstractEnemy>();
        float detectionRange = 600f; // Set the desired detection range

        // Get all nodes in the "Enemy" group and filter by type
        foreach (Node node in GetTree().GetNodesInGroup("Enemy"))
        {
            if (node is AbstractEnemy enemy && !_chainedEnemies.Contains(enemy))
            {
                // Calculate the distance between the bullet and the enemy
                float distance = Position.DistanceTo(enemy.Position);

                // Add the enemy if it's within the detection range
                if (distance <= detectionRange)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        return enemiesInRange;
    }



    private AbstractEnemy FindClosestEnemy(List<AbstractEnemy> enemies)
    {
        AbstractEnemy closest = null;
        float minDistance = float.MaxValue;

        foreach (AbstractEnemy enemy in enemies)
        {
            float distance = Position.DistanceTo(enemy.Position);
            if (distance < minDistance && distance <= ChainRange)
            {
                minDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
    
}
