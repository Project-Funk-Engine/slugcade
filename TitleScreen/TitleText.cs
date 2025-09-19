using Godot;
using System;

public partial class TitleText : Sprite2D
{
    [Export] public Vector2 Speed = new Vector2(200, 150); // pixels per second
    private Vector2 _direction;

    public override void _Ready()
    {
        var rand = new Random();
        _direction = new Vector2(
            rand.Next(0, 2) == 0 ? -1 : 1,
            rand.Next(0, 2) == 0 ? -1 : 1
        );
    }

    public override void _Process(double delta)
    {

        var viewportSize = GetViewportRect().Size;
        var halfSize = Texture.GetSize() * 0.5f;

        Position += Speed * _direction * (float)delta;
        
        if (Position.X - halfSize.X <= 0 || Position.X + halfSize.X >= viewportSize.X)
            _direction.X *= -1;
        
        if (Position.Y - halfSize.Y <= 0 || Position.Y + halfSize.Y >= viewportSize.Y)
            _direction.Y *= -1;
    }
}
