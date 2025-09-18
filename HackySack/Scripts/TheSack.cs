using Godot;
using System;

public partial class TheSack : CharacterBody2D
{
    private Vector2 _direction;
    private float _speed = 400;
    
    public delegate void FootedHandler();
    public event FootedHandler Footed;
    
    public delegate void OOBHandler();
    public event OOBHandler OOB;

    public override void _EnterTree()
    {
        RandomizeDir();
    }

    private int _collisionFudgeCounter = 0;
    public override void _Process(double delta)
    {
        _collisionFudgeCounter = Math.Max(_collisionFudgeCounter - 1, -1);
        Velocity = _direction * _speed;
        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
        HandleCollisions(collision);
        CheckOOB();
    }

    private void HandleCollisions(KinematicCollision2D collision)
    {
        if (collision == null || _collisionFudgeCounter > 0) return;
        if (collision.GetCollider() is not StaticBody2D)
        {
            Footed?.Invoke();
            _direction.X *= (float)GD.RandRange(0.9, 1.25f); //Add some variance
        }

        _direction = collision.GetNormal().Y != 0 ? new Vector2(_direction.X, _direction.Y * -1) : new Vector2(_direction.X * -1, _direction.Y);
        
        _direction = _direction.Normalized();
        _collisionFudgeCounter = 5; //Just to make sure it won't get stuck for a few frames and alternate direction infinitely
    }

    private void CheckOOB()
    {
        if (Position.X > 1280 || Position.X < 0 || Position.Y < 0 || Position.Y > 720)
        {
            OOB?.Invoke();
            Position = new Vector2(960f/2, 540f/2);
            RandomizeDir();
        }
    }

    private void RandomizeDir()
    {
        _direction = new Vector2( (float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0));
        _direction.X = (float)GD.RandRange(0.4, 1.0) * Math.Sign(_direction.X);
        _direction = _direction.Normalized();
    }
}
