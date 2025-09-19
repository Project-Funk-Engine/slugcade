using Godot;
using System;

//We literally only have a global to troll people
public partial class TheWatcher : Node
{
    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("prank"))
        {
            GetTree().ChangeSceneToFile("res://rickroll/rickroll.tscn");
        }
    }
}
