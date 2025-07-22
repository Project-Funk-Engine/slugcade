using Godot;
using System;

public partial class SlugSimulator : Node2D
{
    [Export]
    private Label _timeLeftLabel;
    [Export]
    private Timer _timer;

    [Export] private Label _message;

    public override void _EnterTree()
    {
        _timer.Timeout += TimesUp;
    }

    public override void _Process(double delta)
    {
        TimerToText();
        _frameTimer = Math.Max(_frameTimer - 1, 0);
        if (_frameTimer <= 0) _message.Text = ""; 
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

    private int _frameTimer = 0;
    private int _curMessage = 0;
    private readonly string[] _messages = ["You are a slug!", "Press B to run!", "You can not run, for you are a slug.", 
        "Enjoy the serenity.", "No classes, no legs, life is easy being a slug.", 
        "You are a slug, unfortunately you cannot join Slugworks.", "No legs :)", 
        "The weather is always good for a slug!", "Slugs can't play Midnight Riff (now on Steam) either, how sad.", 
        "You are a slug.", "You can't do anything else.", "There is nothing to harm you here, as you are a slug.",
        "You wouldn't be able to run anyways, no legs (good).", "You are a slug.", "What do slugs do?", 
        "I'm sure its quite pleasant.", "You enjoy life as a slug.", "What a good little slug.", 
        "Good little slug with no legs."];
    public override void _Input(InputEvent @event)
    {
        if (_frameTimer > 90) return;
        if (@event.IsActionPressed("ButtonA") || @event.IsActionPressed("ButtonB"))
        {
            _message.Text = _messages[_curMessage];
            _frameTimer = 160;
            _curMessage = (_curMessage + 1) % _messages.Length;
        }
    }
}
