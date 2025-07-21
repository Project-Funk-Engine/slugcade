using Godot;
using System;
using System.Collections.Generic;

public partial class TitleScreen : Node2D
{
    [Export] private Button _fortunateSlugButton;
    [Export] private Button _hackySlugButton;
    [Export] private Button _lickSlugButton;
    [Export] private Button _slugSimulatorButton;

    private Dictionary<Button, string> _buttonSceneMap;

    public override void _Ready()
    {
        _buttonSceneMap = new Dictionary<Button, string>
        {
            { _fortunateSlugButton, "res://FortunateSlug/FortunateSlug.tscn" },
            { _hackySlugButton, "res://HackySack/HackySack.tscn" },
            { _lickSlugButton, "res://LickTheSlug/LickTheSlug.tscn" },
            { _slugSimulatorButton, "res://SlugSimulator/SlugSimulator.tscn" }
        };

        foreach (var pair in _buttonSceneMap)
            pair.Key.Pressed += () => OnGameButtonPressed(pair.Key);
    }

    private void OnGameButtonPressed(Button button)
    {
        if (_buttonSceneMap.TryGetValue(button, out var scenePath))
            GetTree().ChangeSceneToFile(scenePath);
    }
}