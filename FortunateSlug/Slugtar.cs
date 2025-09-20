using Godot;
using System;

public partial class Slugtar : Node2D
{
    [Export]
    private Label _outputLabel;
    [Export]
    private TextureRect _darkTexture;
    [Export]
    private AudioStreamPlayer2D _switchSound;
    
    private bool _hasChosen = false;
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ButtonA") && !_hasChosen)
        {
            _hasChosen = true;
            _outputLabel.Text = GetRandFortune();
            
            var timer = GetTree().CreateTimer(7.0f);
            timer.Timeout += () => GetTree().ChangeSceneToFile("res://TitleScreen/TitleScreen.tscn");
            
            var timer2 = GetTree().CreateTimer(5.0f);
            timer2.Timeout += () =>
            {
                _darkTexture.Show();
                _switchSound.Play();
            };
        }
    }
    
    private string GetRandFortune()
    {
        var fortunes = new[]
        {
            "Your Slugworks application will be denied.",
            "Your Slugworks application will be accepted.",
            "The banana slugs don't have to pay tuition to be here, but you will.",
            "You will play Midnight Riff on Steam and leave a positive review.",
            "Kissing a slug will lead to disaster.",
            "You'll make grades. I make slime.",
            "You will get lost in the woods between Porter and Kresge",
            "Beware the deer. They have seen things.",
            "You will find true love, but it will be a slug.",
            "Don't fight the turkeys on the way to class. They will win.",
            "If it has been raining all day, close your blinds. Trust me...",
            "Your greatest challenge will not be academic, but rather finding parking after 10 AM.",
            "Your calves are going to be sore tomorrow.",
            "When the first rain falls, a great liberation will occur. You will not be able to unsee it.",
            "Yes, you have to walk all the way up there.",
            "You will get lost in the buildings, they're all mazes.",
            "Eduroam will fail you during your most important Zoom call",
            "A professor's office hours hold the secrets the lecture hall cannot",
            "The laundry machine will leave one sock soaking wet. Always one."
        };
        
        return fortunes[new Random().Next(fortunes.Length)];
    }
}
