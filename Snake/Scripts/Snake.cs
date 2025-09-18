using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Snake : Node2D
{
    [Export] private Label _scoreLabel;
    [Export] private float _baseTps = 10f;
    
    [Export]
    private Label _timeLeftLabel;
    [Export]
    private Timer _timer;
    
    private List<Sprite2D> _slugParts = [];
    private Vector2 _direction = Vector2.Right;
    private Queue<Vector2> _inputBuffer = new Queue<Vector2>();
    private double _tickTimer = 0;
    private bool _gameOver = false;
    private int _score = 0;
    
    // Store the grid size for position tracking
    private int _gridSize = 20;

    private Rect2 _screenBounds;
    private List<Sprite2D> _pickups = [];
    private int _maxPickups = 3; //Number of pickups that can exist at once

    private Texture2D _headTexture = GD.Load<Texture2D>("res://Snake/sprites/Slug.png");
    private Texture2D _bodyTexture = GD.Load<Texture2D>("res://Snake/sprites/SlugBody.png");

    public override void _EnterTree()
    {
        _timer.Timeout += GameOver;
    }
    
    public override void _Ready()
    {
        // Get screen bounds and center position
        _screenBounds = GetViewport().GetVisibleRect();
        var centerPosition = _screenBounds.Size / 2;

        // Create slug head at the center
        var head = new Sprite2D 
        { 
            Texture = _headTexture,
            Position = centerPosition
        };
        AddChild(head);
        _slugParts.Add(head);
        
        for (int i = 0; i < 3; i++)
        {
            AddBodyPart();
        }

        for (int i = 0; i < _maxPickups; i++)
        {
            SpawnPickup();
        }
        
        UpdateScoreLabel();
    }

    public override void _Process(double delta)
    {
        if (_gameOver)
            return;

        TimerToText();
        _tickTimer += delta;
        if (_tickTimer >= 1.0 / _baseTps)
        {
            _tickTimer = 0;
            ProcessInputBuffer();
            MoveSnake();
            
            CheckCollisions();
            CheckPickupCollisions();
        }
    }

    private void ProcessInputBuffer()
    {
        while (_inputBuffer.Count > 0)
        {
            var nextDir = _inputBuffer.Dequeue();
            // Prevent reversing direction
            if (nextDir + _direction == Vector2.Zero) continue;
            _direction = nextDir;
            break;
        }
    }

    private void MoveSnake()
    {
        for (int i = _slugParts.Count - 1; i > 0; i--)
        {
            _slugParts[i].Position = _slugParts[i - 1].Position;
        }
        _slugParts[0].Position += _direction * _gridSize;
        
        // Rotate head to face movement direction using the vector's angle.
        _slugParts[0].Rotation = _direction.Angle();
    }
    
    private void CheckCollisions()
    {
        // Get head position
        Vector2 headPos = _slugParts[0].Position;
        
        // Check for screen edge collision
        if (headPos.X < 0 || headPos.X > _screenBounds.Size.X || 
            headPos.Y < 0 || headPos.Y > _screenBounds.Size.Y)
        {
            GameOver();
            GD.Print("Game Over! Snake hit the edge of the screen.");
            return;
        }
        
        // Check collision with own body using LINQ
        if (_slugParts.Skip(4).Any(p => p.Position == headPos))
        {
            GameOver();
            GD.Print("Game Over! Snake hit itself.");
        }
    }
    
    private void CheckPickupCollisions()
    {
        var headPos = _slugParts[0].Position;
        var collectedPickup = _pickups.FirstOrDefault(p => headPos.DistanceTo(p.Position) < _gridSize);

        if (collectedPickup != null)
        {
            //Adding 2 bc it was so slow otherwise
            AddBodyPart();
            AddBodyPart();
            _score += 10;
            UpdateScoreLabel();
            
            _pickups.Remove(collectedPickup);
            collectedPickup.QueueFree();
            SpawnPickup();
        }
    }
    
    [Export]
    private GameEnd _gameEnd;
    private void GameOver()
    {
        _gameOver = true;
        _gameEnd.Visible = true;
        GetTree().Paused = true;
    }
    
    private void AddBodyPart()
    {
        Vector2 position = _slugParts.Count > 0 
            ? _slugParts[_slugParts.Count - 1].Position 
            : Vector2.Zero;
        
        var bodyPart = new Sprite2D
        {
            Texture = _bodyTexture,
            Position = position
        };
        AddChild(bodyPart);
        _slugParts.Add(bodyPart);
    }

    private void SpawnPickup()
    {
        int margin = _gridSize * 2;
        Vector2 newPos;
        int attempts = 0;
        
        // Keep generating positions until we find one that isnt on the snake slug thing
        do
        {
            float x = (float)GD.RandRange(margin, _screenBounds.Size.X - margin);
            float y = (float)GD.RandRange(margin, _screenBounds.Size.Y - margin);
            newPos = new Vector2(Mathf.Round(x / _gridSize) * _gridSize, Mathf.Round(y / _gridSize) * _gridSize);
        } while (_slugParts.Any(p => p.Position == newPos) && ++attempts < 50);
        
        var pickup = new Sprite2D
        {
            Texture = _headTexture, // TODO: Use a different texture for pickups
            Position = newPos
        };
        
        AddChild(pickup);
        _pickups.Add(pickup);
    }

    private void UpdateScoreLabel()
    {
        if (_scoreLabel != null)
        {
            _scoreLabel.Text = $"Score: {_score}";
        }
    }
    
    private void TimerToText()
    {
        _timeLeftLabel.Text = "00:" + _timer.TimeLeft.ToString("00");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.Pressed) return;
        
        if (keyEvent.Keycode == Key.R)
        {
            GetTree().ReloadCurrentScene();
            return;
        }
        
        switch (keyEvent.Keycode)
        {
            case Key.W:
            case Key.Up:
                _inputBuffer.Enqueue(Vector2.Up);
                break;
            case Key.S:
            case Key.Down:
                _inputBuffer.Enqueue(Vector2.Down);
                break;
            case Key.A:
            case Key.Left:
                _inputBuffer.Enqueue(Vector2.Left);
                break;
            case Key.D:
            case Key.Right:
                _inputBuffer.Enqueue(Vector2.Right);
                break;
        }
    }
}
