using Godot;
using System;
using System.Collections.Generic;

public partial class TitleScreen : Node2D
{

    [Export] private GameTurnTable _gameTurnTable;

    private string[] _buttonSceneMap;

    public override void _Ready()
    {
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
        if (@event.IsActionPressed("ButtonA"))
        {
            GetTree().ChangeSceneToFile( _buttonSceneMap[_gameTurnTable.GetCurrentSelection()]);
        }
    }
}