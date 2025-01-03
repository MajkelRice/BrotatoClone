using Godot;
using System;

public interface IWeapon
{
    Vector2 GetPosition(int slotIndex, int totalSlots, float radius);
    void Equip(Node2D parent, Vector2 position);
}