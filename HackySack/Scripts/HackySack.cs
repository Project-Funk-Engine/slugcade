using Godot;
using System;

public partial class HackySack : Node2D
{
    [Export] private AudioStreamPlayer _hackySound;
    [Export]
    private Label _timeLeftLabel;
    [Export]
    private Label _scoreLabel;
    private int _score;
    [Export]
    private Timer _timer;

    [Export] private Label _message;
    
    [Export]
    private TheSack _theSack;
    [Export]
    private PlayerFeet _theFoot;
    [Export] private CharacterBody2D _otherFoot;
    private float _otherFootSpeed = 225f;
    private float _otherFootSize = 15;
    
    public override void _EnterTree()
    {
        _timer.Timeout += TimesUp;
        _theSack.Footed += IncScore;
        _theSack.OOB += ClearScore;
    }

    public override void _Process(double delta)
    {
        TimerToText();
        OtherFootProcess();
    }

    private void OtherFootProcess()
    {
        float positionDifference = _otherFoot.Position.Y - _theSack.Position.Y;
        var directionSign = Math.Abs(positionDifference) > _otherFootSize ? Math.Sign(positionDifference) : 0;
        _otherFoot.Velocity = new Vector2(0, directionSign * -1) * _otherFootSpeed;
        _otherFoot.MoveAndSlide();
    }

    private void TimerToText()
    {
        _timeLeftLabel.Text = "00:" + _timer.TimeLeft.ToString("00");
    }
    
    [Export]
    private GameEnd _gameEnd;
    private void TimesUp()
    {
        _gameEnd.Visible = true;
        GetTree().Paused = true;
    }

    private string[] _messages = ["Nice.", "Keep it up!", "Go for 100!"];
    private void IncScore()
    {
        _message.Text = _messages[_score % _messages.Length];
        _score++;
        _scoreLabel.Text = "Score: " + _score.ToString();
        _hackySound.Play();
    }

    private void ClearScore()
    {
        _message.Text = "Bro...";
        _score = 0;
        _scoreLabel.Text = "Score: 0";
    }
}
