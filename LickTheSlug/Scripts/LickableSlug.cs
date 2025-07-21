using Godot;
using System;

public partial class LickableSlug : Sprite2D
{
    private float _speed = 20;
    private Vector2 _direction = Vector2.Zero;
    
    private readonly Vector2[] _directions = { Vector2.Left, Vector2.Right, Vector2.Down, Vector2.Up };
    
    public override void _EnterTree()
    {
        Texture2D tongue = GD.Load<Texture2D>("res://LickTheSlug/Tongue.png");
        GD.Randomize();
        Position = new Vector2(GD.RandRange(100, 860), GD.RandRange(100, 440));
        _direction = _directions[GD.RandRange(0, _directions.Length-1)];
        Rotation = _direction.Angle();
        Input.SetCustomMouseCursor(tongue);
    }

    public override void _ExitTree()
    {
        Input.SetCustomMouseCursor(null);
    }

    public override void _Process(double delta)
    {
        Position += _direction * _speed * (float)delta;
    }

    public float ValidClickRadiusSquared = 1000;

    public delegate void LickedHandler();
    public event LickedHandler Licked;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.IsActionPressed("Click") && GetGlobalMousePosition().DistanceSquaredTo(GlobalPosition) <= ValidClickRadiusSquared)
        {
            GotLicked();
        }
    }

    public void GotLicked()
    {
        Licked?.Invoke();
        _direction = _directions[GD.RandRange(0, _directions.Length-1)];
        Rotation = _direction.Angle();
    }
}
