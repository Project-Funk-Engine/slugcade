using Godot;
using System;

public partial class ControllableSlug : Sprite2D
{
    [Export]
    private float _speed = 2f;
    
    public override void _Process(double delta)
    {
        Vector2 dir = Input.GetVector("Left", "Right", "Up", "Down");
        dir = dir.Normalized();
        Position = new Vector2(Math.Clamp(Position.X + dir.X * _speed * (float)delta, 0f, 960f), Math.Clamp(Position.Y + dir.Y * _speed * (float)delta, 0f, 540f));
        Position += dir * _speed * (float)delta;
        if (Input.IsActionPressed("Left"))
            RotationDegrees = 180f;
        else if (Input.IsActionPressed("Right"))
            RotationDegrees = 0f;
        if (Input.IsActionPressed("Up"))
            RotationDegrees = -90f;
        else if (Input.IsActionPressed("Down"))
            RotationDegrees = 90f;
    }
}
