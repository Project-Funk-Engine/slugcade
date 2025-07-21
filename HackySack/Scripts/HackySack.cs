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
    
    [Export]
    private TheSack _theSack;
    [Export]
    private PlayerFeet _theFoot;
    [Export] private CharacterBody2D _otherFoot;
    private float _otherFootSpeed = 200f;
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
    
    private void TimesUp()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        GD.Print("Time's up, pausing scene for now!");
    }

    private void IncScore()
    {
        _score++;
        _scoreLabel.Text = "Score: " + _score.ToString();
    }

    private void ClearScore()
    {
        _score = 0;
        _scoreLabel.Text = "Score: 0";
    }
}
