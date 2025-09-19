using Godot;
using System;
using System.Collections.Generic;

public partial class TitleScreen : Node2D
{

    [Export] private GameTurnTable _gameTurnTable;
    [Export] private AudioStreamPlayer _audioStreamPlayer;
    [Export] private CanvasLayer _screensaverNode;
    [Export] private Timer _timer;
    
    private bool _isScreensaverActive = false;

    private string[] _buttonSceneMap;

    public override void _Ready()
    {
        _timer.Timeout += _startScreensaver;
        
        _buttonSceneMap = new string[]
        {
            "res://FortunateSlug/FortunateSlug.tscn",
            "res://SlugSimulator/SlugSimulator.tscn",
            "res://LickTheSlug/LickTheSlug.tscn",
            "res://HackySack/HackySack.tscn",
            "res://Snake/Snake.tscn"
        };
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ButtonA") && !_isScreensaverActive)
        {
            GetTree().ChangeSceneToFile( _buttonSceneMap[_gameTurnTable.GetCurrentSelection()]);
        }
        
        if (@event is not InputEventJoypadButton or InputEventKey )
        {
            return; // These are the only events that should reset the screensaver. Doing this bc I think the touch screen is causing issues
        }
        

        if (_isScreensaverActive)
        {
            _isScreensaverActive = false;
            _audioStreamPlayer.Play();
            _screensaverNode.Visible = false;
        }

        _timer.Start();
    }

    private void _startScreensaver()
    {
        _isScreensaverActive = true;
        _audioStreamPlayer.Stop();
        _screensaverNode.Visible = true;
    }
}