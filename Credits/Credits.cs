using Godot;
using System;

public partial class Credits : Node2D
{
    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("ButtonB"))
        {
                GetTree().ChangeSceneToFile("res://TitleScreen/TitleScreen.tscn");
        }    
    }
    
}
