using Godot;
using System;
using System.Collections.Generic;

public partial class TitleScreen : Node2D
{

    [Export] private GameTurnTable _gameTurnTable;
    [Export] private AudioStreamPlayer _audioStreamPlayer;
    [Export] private CanvasLayer _screensaverNode;
    [Export] private Timer _timer;
    [Export] private Sprite2D _titleTexture;
    
    private bool _isScreensaverActive = false;

    private string[] _buttonSceneMap;

    private Tween _titleTween;

    public override void _Ready()
    {
        _titleTween = GetTree().CreateTween();
        _titleTween.TweenProperty(_titleTexture, "scale", new Vector2(1f,1f), .4286f).SetTrans(Tween.TransitionType.Quad); 
        _titleTween.TweenProperty(_titleTexture, "scale", new Vector2(0.9f,0.9f), .4286f).SetTrans(Tween.TransitionType.Quad);

        _titleTween.SetLoops();
        
        
        _timer.Timeout += _startScreensaver;
        
        _buttonSceneMap = new string[]
        {
            "res://FortunateSlug/FortunateSlug.tscn",
            "res://SlugSimulator/SlugSimulator.tscn",
            "res://LickTheSlug/LickTheSlug.tscn",
            "res://HackySack/HackySack.tscn",
            "res://HellHole/HellHole.tscn",
            "res://Snake/Snake.tscn"
        };
        
    }

    public override void _Input(InputEvent @event)
    {
        
        if (!(@event is not InputEventJoypadButton and not InputEventKey and not InputEventJoypadMotion))
        {
            if (@event is InputEventJoypadMotion motion && Math.Abs( motion.AxisValue) < 0.1f) return;

            if (_isScreensaverActive)
            {
                _isScreensaverActive = false;
                _audioStreamPlayer.SetVolumeDb(-16.155f);
                _screensaverNode.Visible = false;
            }  
            _timer.Start();
        }
        
        if (@event.IsActionPressed("ButtonA") && !_isScreensaverActive)
        {
            _titleTween.Kill();
            GetTree().ChangeSceneToFile( _buttonSceneMap[_gameTurnTable.GetCurrentSelection()]);
        }
        
        if(Input.IsActionJustPressed("ButtonB"))
        {
            _titleTween.Kill();
            GetTree().ChangeSceneToFile("res://Credits/Credits.tscn");
        }        
        
        if (@event.IsActionPressed("Mute")) //Non-reversible, mainly to not go insane while testing.
        {
            AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
        }

    }

    private void _startScreensaver()
    {
        _isScreensaverActive = true;
        _audioStreamPlayer.SetVolumeLinear(0);
        _screensaverNode.Visible = true;
    }
}