using Godot;
using System;

public partial class ExplosionEffect : AnimatedSprite2D
{
    public float ExplosionRadius { get; set; } = 100f; // Default value, can be overridden

    public override void _Ready()
    {
        // Get the size of the current frame
        Vector2 frameSize = SpriteFrames.GetFrameTexture(Animation, 0).GetSize();

        // Adjust the size of the explosion animation
        Scale = new Vector2(2 * ExplosionRadius / frameSize.X, 2 * ExplosionRadius / frameSize.Y);

        // Play the animation
        Play("default");
        FrameChanged += OnFrameChanged;
    }

    private void OnFrameChanged()
    {
        // Check if the animation is finished
        if (Frame == SpriteFrames.GetFrameCount(Animation) - 1)
        {
            QueueFree(); // Remove the explosion effect from the scene
        }
    }
}