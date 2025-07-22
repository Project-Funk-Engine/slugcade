using Godot;
using System;

public partial class LickTheSlug : Node2D
{
    [Export]
    private LickableSlug _slug;
    [Export] private Sprite2D _tongue;
    private float _tongueSpeed = 100;
    [Export] private AudioStreamPlayer _lickSound;
    [Export]
    private Label _timeLeftLabel;
    [Export]
    private Label _scoreLabel;
    private int _score;
    [Export]
    private Timer _timer;

    public override void _EnterTree()
    {
        _slug.Licked += SlugLicked;
        _timer.Timeout += TimesUp;
    }

    public override void _Process(double delta)
    {
        _timeLeftLabel.Text = "00:" + _timer.TimeLeft.ToString("00");
        Vector2 tongueDirection = Input.GetVector("Left", "Right", "Up", "Down");
        _tongue.Position += tongueDirection * _tongueSpeed * (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ButtonA") && _tongue.Position.DistanceSquaredTo(_slug.Position) < _slug.ValidClickRadiusSquared)
        {
            _slug.GotLicked();
        }
    }

    private void SlugLicked()
    {
        IncScore();
    }

    private void IncScore()
    {
        _score += 1;
        _scoreLabel.Text = "Score: " + _score;
        _lickSound.Play();
    }

    [Export]
    private GameEnd _gameEnd;
    private void TimesUp()
    {
        _gameEnd.Visible = true;
        GetTree().Paused = true;
    }
}
