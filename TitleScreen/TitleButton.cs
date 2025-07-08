using Godot;
using System;

public partial class TitleButton : Button
{
    //This stuff is only here so I can make sure that the github actions correctly build the c# project
    private int clickCount = 0;
    public override void _Ready()
    {
        this.Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        clickCount++;
        this.Text = $"Clicked {clickCount} times";
    }
}
