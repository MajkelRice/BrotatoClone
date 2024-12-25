using Godot;
using System;

public partial class Exp : Area2D
{
    [Export] public Player player; 
    public void OnExpCollect(float xp)
    {
        player._totalExperience += xp;
        QueueFree();
    }
}
