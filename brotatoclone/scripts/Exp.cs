using Godot;
using System;

public partial class Exp : Area2D
{
    [Export] public Player player;

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<Player>("Game/Player");
    }


    public void OnExpCollect(float xp = 10f)
    {
        player._totalExperience += 50f;
        player.ExpBar.Value += 50;
        QueueFree();
        GD.Print(player._totalExperience);
    }
}
