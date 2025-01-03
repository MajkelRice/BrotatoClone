using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Player : CharacterBody2D, ISkillTree
{
    [Export] public float Health = 100.0f;
    [Export] public float MaxHealth = 100.0f;
    [Export] public float MoveSpeed = 150.0f; 
    [Export] public float DashSpeed = 200.0f;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 1.0f;

    [Export] public float IFrameDuration = 1.0f;
    
    [Export] public float _totalExperience = 0f;
    [Export] public float ExperienceToNextLevel = 100f;
    [Export] public int Level = 1;
    [Export] public Area2D HurtZone;
    [Export] public TextureProgressBar HpBar;
    [Export] public TextureProgressBar ExpBar;
    [Export] public Control SkillTree;

    [Export] public int MaxWeaponSlots = 16;
    [Export] public int UnlockedWeaponSlots = 16;

    private List<string> _weaponSlots = new List<string>();

    private AnimatedSprite2D _playerSprite;
    private Vector2 _inputDirection = Vector2.Zero;
    private int _skillPoints = 0;
    
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
        HpBar.MaxValue = MaxHealth;
        HpBar.Value = Health;
        ExpBar.MaxValue = ExperienceToNextLevel;
        ExpBar.Value = _totalExperience;
        Health = MaxHealth;

        SkillTree.Visible = false;
        
        for (int i = 0; i < MaxWeaponSlots; i++)
        {
            _weaponSlots.Add(null); // null indicates an empty slot
        }
        
        //ONLY FOR DEVELOPMENT
        PackedScene pistolScene = GD.Load<PackedScene>("res://scenes/pistol.tscn");
        PackedScene shotGunScene = GD.Load<PackedScene>("res://scenes/guns/ShotGun.tscn");
        PackedScene burstRifleScene = GD.Load<PackedScene>("res://scenes/guns/BurstRifle.tscn");
        PackedScene rocketLauncherScene = GD.Load<PackedScene>("res://scenes/guns/RocketLauncher.tscn");
        PackedScene basicSwordScene = GD.Load<PackedScene>("res://scenes/swords/basic_sword.tscn");
        PackedScene deathSwordScene = GD.Load<PackedScene>("res://scenes/swords/death_sword.tscn");
        PackedScene flamethrowerScene = GD.Load<PackedScene>("res://scenes/guns/flamethrower.tscn");
        PackedScene clusterBombLauncherScene = GD.Load<PackedScene>("res://scenes/guns/cluster_bomb_launcher.tscn");
        PackedScene lightningGunScene = GD.Load<PackedScene>("res://scenes/guns/lightning_gun.tscn");
        PackedScene smgScene = GD.Load<PackedScene>("res://scenes/guns/smg.tscn");
        PackedScene poisonGun = GD.Load<PackedScene>("res://scenes/guns/poison_spitter.tscn");



        EquipWeapon(clusterBombLauncherScene);


    }

    public override void _Process(double delta)
    {
        if (_cooldownTimer > 0) _cooldownTimer -= (float)delta; 

        _inputDirection = new Vector2(
            Input.GetActionStrength("go_right") - Input.GetActionStrength("go_left"),
            Input.GetActionStrength("go_down") - Input.GetActionStrength("go_up")
        );

        if (Input.IsActionJustPressed("passive_skill_tree")) SkillTree.Visible = !SkillTree.Visible; 

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
        
        if (_totalExperience >= ExperienceToNextLevel)
        {
            LevelUp();
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
        _playerSprite.Modulate = new Color(1, 0, 0, 1);
    }

    private void EndIFrame()
    {
        _isIFraming = false;
        _playerSprite.Modulate = new Color(1, 1, 1, 1);
    }
    
    private void LevelUp()
    {
        Level++;
        _totalExperience -= ExperienceToNextLevel;
        ExperienceToNextLevel *= 1.25f;
        ExpBar.Value = _totalExperience;

        _skillPoints++;
        
        ExpBar.MaxValue = ExperienceToNextLevel;

        GD.Print($"Leveled up! Available skill Points: {_skillPoints}");
    }

    public int GetSkillPoints()
    {
        return _skillPoints;
    }

    public void DecrementSkillPoints()
    {
        _skillPoints--;
    }
    
    public void ApplyStatModifiers(Dictionary<string, float> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            switch (modifier.Key)
            {
                case "Health":
                    IncreaseHealth(modifier.Value);
                    break;
                case "Speed":
                    IncreaseSpeed(modifier.Value);
                    break;
                default:
                    GD.PrintErr($"Unknown stat modifier: {modifier.Key}");
                    break;
            }
        }
    }
    
    private void IncreaseHealth(float amount)
    {
        MaxHealth += amount;
        HpBar.MaxValue = MaxHealth;
        GD.Print($"Increased health to {MaxHealth}");
    }

    private void IncreaseSpeed(float amount)
    {
        MoveSpeed += amount;
        GD.Print($"Increased speed to {MoveSpeed}");
    }

    public bool UnlockWeaponSlot()
    {
        if (UnlockedWeaponSlots < MaxWeaponSlots)
        {
            UnlockedWeaponSlots++;
            GD.Print($"Weapon slot unlocked! Total unlocked slots: {UnlockedWeaponSlots}");
            return true;
        }
        GD.Print("All weapon slots are already unlocked.");
        return false;
    }

    public bool EquipWeapon(PackedScene weaponScene)
    {
        if (_weaponSlots.Count(weapon => weapon != null) >= UnlockedWeaponSlots)
        {
            GD.Print("No available slots to equip the weapon.");
            return false;
        }

        
        // Calculate positions evenly distributed on a circle
        Vector2[] weaponPositions = new Vector2[UnlockedWeaponSlots];
        float radius = 40.0f; // The radius of the circle

        for (int i = 0; i < UnlockedWeaponSlots; i++)
        {
            // Calculate the angle for this slot
            float angle = (i * 360.0f / UnlockedWeaponSlots) * (Mathf.Pi / 180.0f); // Convert to radians

            // Calculate the x and y position using cosine and sine
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            // Store the position
            weaponPositions[i] = new Vector2(x, y);
        }

        
        for (int i = 0; i < UnlockedWeaponSlots; i++)
        {
            if (_weaponSlots[i] == null)
            {
                var weaponInstance = weaponScene.Instantiate<IWeapon>();
                Vector2 position = weaponInstance.GetPosition(i, UnlockedWeaponSlots, radius);
                weaponInstance.Equip(this, position);
                _weaponSlots[i] = (weaponInstance as Node2D)?.Name;

                GD.Print($"Equipped weapon in slot {i + 1} at position {weaponPositions[i]}.");
                return true;
            }
        }

        GD.Print("All unlocked slots are occupied.");
        return false;
    }


    public void UnequipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= UnlockedWeaponSlots)
        {
            GD.PrintErr("Invalid slot index.");
            return;
        }

        if (_weaponSlots[slotIndex] != null)
        {
            GD.Print($"Unequipped {_weaponSlots[slotIndex]} from slot {slotIndex + 1}.");
            _weaponSlots[slotIndex] = null;
        }
        else
        {
            GD.Print("Slot is already empty.");
        }
    }
}
