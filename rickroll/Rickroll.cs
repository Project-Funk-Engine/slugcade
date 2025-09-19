using Godot;
using System;

public partial class Rickroll : Node2D
{
    [Export] private VideoStreamPlayer _videoPlayer;
    
    public override void _Ready()
    {
        _videoPlayer.Finished += () => GetTree().ChangeSceneToFile("res://TitleScreen/TitleScreen.tscn");
    }
    
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ButtonB"))
        {
            GetTree().ChangeSceneToFile("res://TitleScreen/TitleScreen.tscn");
        }
    }
    
}
