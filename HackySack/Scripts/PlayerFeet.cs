using Godot;
using System;

public partial class PlayerFeet : CharacterBody2D
{
    private float _speed = 200f;
    private Vector2 _dir = Vector2.Zero;
    
    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("Up") && Position.Y > 0)
        {
            _dir = Vector2.Up;
        } else if (Input.IsActionPressed("Down") && Position.Y < 720)
        {
            _dir = Vector2.Down;
        }
        else
        {
            _dir = Vector2.Zero;
        }

        Velocity = _speed * _dir;

        MoveAndSlide();
    }
}
