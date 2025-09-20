using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class HellHole : Node2D
{
    private const int ScreenWidth = 256;
    private const int ScreenHeight = 144;
    private const int PlayerX = 20;
    private const int PlayerSize = 5;
    private const float JumpStrength = -2.7f;
    private const int GapHeight = 70;
    private const int StartingAnitGaps = 20;
    private static readonly int[] RandomGapDelta = [-3, -2, -1, 0, 1, 2, 3];

    [Export]
    private AnimatedSprite2D _slugPlayer;

    enum SlugFrames
    {
        Neutral = 0, Jump = 2, Falling = 3
    }

    private float _playerYVelo = 0;
    private readonly List<int> _ceiling = new List<int>();
    private readonly List<int> _floor = new List<int>();
    private readonly List<Sprite2D> _obstacles = new List<Sprite2D>();
    private int _frameCount = 0;
    private int _frameTimer = 0;
    private int _score = 0;
    private int _direction = 1;
    
    private int _stage = 0; //0 = default, 1 = Party Room, 2 = Main Room
    private float _scrollSpeed = 2.25f;
    private float _gravity = 0.3f;
    
    private bool _started = false;
    
    [Export]
    private Timer _timer;
    
    [Export]
    private Label _timeLeftLabel;
    
    [Export]
    private Label _scoreLabel;

    [Export]
    private Label _areaLabel;
    
    [Export] private CanvasLayer _startMessage;
    
    [Export] private ColorRect _bgRect;
    
    public override void _EnterTree()
    {
        for (int i = 0; i < 160; i++)
        {
            _ceiling.Add(StartingAnitGaps);
            _floor.Add(ScreenHeight - StartingAnitGaps);
        }

        for (int i = 160; i < ScreenWidth; i++)
        {
            int lastGap = _ceiling.Last();
            int gapTop = lastGap + RandomGapDelta[GD.Randi() % RandomGapDelta.Length];
        
            gapTop = Math.Max(20, Math.Min(gapTop, ScreenHeight - GapHeight - 20));
        
            _ceiling.Add(gapTop);
            _floor.Add(gapTop+GapHeight);
        }
        GD.Randomize();
        _timer.Timeout += GameOver;
    }

    public override void _PhysicsProcess(double delta) //Physics process for fixed ~30fps taht won't affect other games.
    {
        if (!_started)
        {
            if (!Input.IsActionPressed("ButtonA")) return;
            _started = true;
            _playerYVelo = JumpStrength;
            _startMessage.Visible = false;
            return;
        }
        
        if (Input.IsActionJustPressed("ButtonA"))
            _playerYVelo = JumpStrength;

        _playerYVelo += _gravity;
        _slugPlayer.Position += Vector2.Down * _playerYVelo;

        if (_playerYVelo < -.1)
            _slugPlayer.Frame = (int)SlugFrames.Jump;
        else if (_playerYVelo > .1)
            _slugPlayer.Frame = (int)SlugFrames.Falling;
        else
            _slugPlayer.Frame = (int)SlugFrames.Neutral;
        
        _ceiling.RemoveAt(0);
        _floor.RemoveAt(0);

        int lastGap = _ceiling.Last();
        int gapTop = lastGap + RandomGapDelta[GD.Randi() % RandomGapDelta.Length];
        
        gapTop = Math.Max(20, Math.Min(gapTop, ScreenHeight - GapHeight - 20));
        
        _ceiling.Add(gapTop);
        _floor.Add(gapTop+GapHeight);

        int topY = _ceiling[PlayerX];
        int bottomY = _floor[PlayerX];

        if (_slugPlayer.Position.Y - PlayerSize < topY || _slugPlayer.Position.Y + PlayerSize > bottomY)
        {
            GD.Print("Player Y: " + _slugPlayer.Position.Y + " Top Y: " + topY + " Bottom Y: " + bottomY);
            GameOver();
        }

        if (_frameCount > 200 && _frameCount % 20 == 0 && GD.Randf() < 0.4f)
        {
            SpawnObstacle(topY, bottomY);
        }

        List<int> indexesRemove = new List<int>();
        for (int i = 0; i < _obstacles.Count; i++)
        {
            if (i >= _obstacles.Count) continue;
            
            Sprite2D sprite = _obstacles[i];
            
            sprite.Position += Vector2.Left * _scrollSpeed;
            if(Math.Abs(_slugPlayer.Position.X - sprite.Position.X) < PlayerSize && Math.Abs(_slugPlayer.Position.Y - sprite.Position.Y) < PlayerSize)
            {
                GD.Print("Player Pos: " + _slugPlayer.Position + " Sprite Pos: " + sprite.Position);
                GameOver();
            }
            
            if (!(sprite.Position.X < -PlayerSize)) continue;
            sprite.Visible = false;
            indexesRemove.Add(i);
        }

        foreach (int index in indexesRemove)
        {
            _obstaclePool.Add(_obstacles[index]);
            _obstacles.RemoveAt(index);
        }
        

        switch (_stage)
        {
            case 0 when _score >= 200:
                _timer.Start(_timer.GetWaitTime() + 0.5f);
                _areaLabel.Text = "PARTY ROOM";
                _frameTimer = 30;
                _stage = 1;
                _scrollSpeed = 2.5f;
                _gravity = 0.4f;
                break;
            case 1 when _score >= 400:
                _timer.Start(_timer.GetWaitTime() + 0.5f);
                _areaLabel.Text = "MAIN CAVE";
                _frameTimer = 30;
                _stage = 2;
                _scrollSpeed = 3.5f;
                _gravity = 0.45f;
                break;
            case 2 when _score >= 600:
                _timer.Start(_timer.GetWaitTime() + 0.5f);
                _areaLabel.Text = "THE PIT";
                _frameTimer = 30;
                _stage = 3;
                _scrollSpeed = 4.5f;
                _gravity = 0.5f;
                break;
            case 3 when _score >= 750:
                _timer.Start(_timer.GetWaitTime() + 0.5f);
                _areaLabel.Text = "HALL OF FACES";
                _frameTimer = 30;
                _stage = 4;
                _scrollSpeed = 5f;
                _gravity = 0.55f;
                break;
            
        }

        if (_frameCount > 135 && _frameCount % 4 == 0)
            _score += 1;
        _frameCount += 1;
        _frameTimer -= 1;
        
        DrawText();
        _bgRect.Color = _bgRect.Color with { A = _score / 4000f };
        QueueRedraw();
    }

    private static readonly Color Color7 = new Color(238/255f, 238/255f, 238/255f);
    private static readonly Color Color13 = new Color(163/255f, 163/255f, 163/255f);
    private static readonly Color Color4 = new Color(139/255f, 72/255f, 82/255f);

    public override void _Draw()
    {
        for (int x = 0; x < ScreenWidth; x++)
        {
            for (int cy = 0; cy < _ceiling[x]; cy++)
            {
                int distToEdge = _ceiling[x] - cy;
                switch (distToEdge)
                {
                    case 1:
                        DrawRect(new Rect2(x, cy, 1, 1), Color7);
                        break;
                    case <= 5:
                        DrawRect(new Rect2(x, cy, 1, 1), Color13);
                        break;
                    default:
                        DrawRect(new Rect2(x, cy, 1, 1), Color4);
                        break;
                }
            }

            for (int fy = _floor[x]; fy < ScreenHeight; fy++)
            {
                int distToEdge = fy - _floor[x];
                switch (distToEdge)
                {
                    case 0:
                        DrawRect(new Rect2(x, fy, 1, 1), Color7);
                        break;
                    case <= 5:
                        DrawRect(new Rect2(x, fy, 1, 1), Color13);
                        break;
                    default:
                        DrawRect(new Rect2(x, fy, 1, 1), Color4);
                        break;
                }
            }
        }
    }

    private readonly List<Sprite2D> _obstaclePool = new List<Sprite2D>();
    private string _obstacleTexture = "res://HellHole/Bat.png";
    private void SpawnObstacle(int topY, int bottomY)
    {
        if (_obstaclePool.Count > 0)
        {
           Sprite2D sprite = _obstaclePool[0];
           sprite.Visible = true;
           
           sprite.Position = new Vector2(ScreenWidth, GD.RandRange(topY + 30, bottomY - 30));
           _obstacles.Add(sprite);
           _obstaclePool.RemoveAt(0);
        }
        else
        {
            Sprite2D sprite = new Sprite2D();
            AddChild(sprite);
            sprite.Texture = GD.Load<Texture2D>(_obstacleTexture);
            sprite.Position = new Vector2(ScreenWidth, GD.RandRange(topY + 30, bottomY - 30));
            _obstacles.Add(sprite);
        }
    }
    [Export]
    private GameEnd _gameEnd;
    private void GameOver()
    {
        _gameEnd.Visible = true;
        GetTree().Paused = true;
    }
    
    private void DrawText()
    {
        _timeLeftLabel.Text = "00:" + _timer.TimeLeft.ToString("00");
        _scoreLabel.Text = "Score: " + _score;
        _areaLabel.Visible = _frameTimer > 0;
    }
}
