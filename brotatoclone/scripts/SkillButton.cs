using System;
using System.Collections.Generic;
using Godot;

public partial class SkillButton : TextureButton
{
    [Export] public string[] StatKeys = Array.Empty<string>();
    [Export] public float[] StatValues = Array.Empty<float>();
    
    private Panel _panel;
    private Label _label;
    private Line2D _line2D;
    private Node _skillTreeParent;

    private int _level = 0;
    private const int _maxLevel = 3;

    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            _label.Text = $"{_level}/{_maxLevel}";
        }
    }

    public override void _Ready()
    {
        _panel = GetNode<Panel>("Panel");
        _label = GetNode<Label>("MarginContainer/Label");
        _line2D = GetNode<Line2D>("Line2D");
        _skillTreeParent = GetTree().Root.GetNode("Game/Player");

        if (GetParent() is SkillButton parentSkillNode)
        {
            _line2D.AddPoint(GlobalPosition + Size / 2);
            _line2D.AddPoint(parentSkillNode.GlobalPosition + Size / 2);
        }

    }

    private void OnPressed()
    {
        if (_skillTreeParent is ISkillTree skillTree)
        {
            if (skillTree.GetSkillPoints() <= 0 || Level == _maxLevel) return;

            skillTree.DecrementSkillPoints();
            Level = Mathf.Min(Level + 1, _maxLevel);
            _panel.ShowBehindParent = true;

            _line2D.DefaultColor = new Color(1, 1, 0.247f);
            
            if (_skillTreeParent is Player player)
            {
                var modifiers = GetStatModifiers();
                player.ApplyStatModifiers(modifiers);
            }

            foreach (var child in GetChildren())
            {
                if (child is SkillButton skillNode && Level == _maxLevel)
                {
                    skillNode.Disabled = false;
                }
            }
        }
        else
        {
            GD.PrintErr("SkillTreeParent does not implement ISkillTree.");
        }
    }
    
    private Dictionary<string, float> GetStatModifiers()
    {
        var modifiers = new Dictionary<string, float>();
        for (int i = 0; i < Math.Min(StatKeys.Length, StatValues.Length); i++)
        {
            modifiers[StatKeys[i]] = StatValues[i];
        }
        return modifiers;
    }
}