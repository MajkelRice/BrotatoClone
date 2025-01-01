using Godot;
using System;

public interface ISkillTree
{
    int GetSkillPoints();
    
    void DecrementSkillPoints();
}
