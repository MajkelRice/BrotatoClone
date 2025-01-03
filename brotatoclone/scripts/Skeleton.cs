using Godot;
using System;

public partial class Skeleton : AbstractEnemy
{
    [Export] public float ChaseRange = 10000.0f;
    
    public override void _Process(double delta)
    {
        CustomBehavior((float)delta);
    }

    public void CustomBehavior(float delta)
    {
        if (Player != null && GlobalPosition.DistanceTo(Player.GlobalPosition) <= ChaseRange)
        {
            MoveTowardsTarget(delta);
            HandlePoisonEffect(delta);
        }
    }

}