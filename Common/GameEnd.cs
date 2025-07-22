using Godot;
using System;

public partial class GameEnd : CanvasLayer
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ButtonB"))
        {
            GetTree().SetPause(false);
            GetTree().ChangeSceneToFile("res://TitleScreen/TitleScreen.tscn");
        }
    }
}
