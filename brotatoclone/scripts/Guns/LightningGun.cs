using Godot;
using System;

public partial class LightningGun : AbstractGun, IWeapon, ISkillTree
{
    [Export] public float FireRate = 0.5f; // Lower fire rate for chaining effect
    
}