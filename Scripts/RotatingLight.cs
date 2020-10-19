using Godot;
using System;

public class RotatingLight : DirectionalLight
{
    [Export] public float Speed = 1.0f;
    
    public override void _Process(float delta)
    {
        Vector3 currentRotation = RotationDegrees;
        currentRotation.x += Speed;
        RotationDegrees = currentRotation;
    }
}
