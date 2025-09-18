using Godot;
using System;

public partial class GameTurnTable : Node2D
{
    private string[] _animatedNames = new String[]
    {
        "SlugTar", "SlugSim", "LickTheSlug", "HackySack", "SlugSnake"
    };
    [Export] AnimatedSprite2D[] _animatedSprites = new AnimatedSprite2D[4];
    enum TrackIndexes
    {
        Cur = 0, Next = 1, Placeholder = 2, Prev = 3
    }
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Marker2D _currentGame;
    [Export] private Marker2D _nextGame;
    [Export] private Marker2D _prevGame;
    [Export] private Marker2D _slideInGame;

    private int _currentSelection;
    public int GetCurrentSelection() { return _currentSelection; }
    
    private bool _sliding;
    private int _prevDir = 1; //-1 for GameSlidesRev, 1 for GameSlidesFor

    public override void _Ready()
    {
        foreach (AnimatedSprite2D sprite in _animatedSprites)
        {
            sprite.Play();
        }

        _animationPlayer.AnimationFinished += OnAnimFinished;
    }

    public override void _Process(double delta)
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (_sliding) return;
        if (Input.IsActionPressed("Left")) HandleSlide(1);
        else if (Input.IsActionPressed("Right")) HandleSlide(-1);
    }

    private void HandleSlide(int dir)
    {
        _sliding = true;
        _prevDir = dir;
        string anim;
        anim = dir == 1 ? "GameSlidesFor" : "GameSlidesRev";
        //This is obviously very bad. Needed to get it working with the anim player. Gets path to the marker thats currently in the role of placeholder, and gets its child sprite, to set it to the correct anim that needs to slide in
        GetNode<Marker2D>(_animationPlayer.GetAnimation(anim).TrackGetPath((int)TrackIndexes.Placeholder))
            .GetChild<AnimatedSprite2D>(0)
            .Play(_animatedNames[Mathf.PosMod(_currentSelection + (2*dir), _animatedNames.Length)]);
        _animationPlayer.Play(anim);
        _currentSelection = Mathf.PosMod(_currentSelection + dir, _animatedNames.Length);
    }
    
    public void OnAnimFinished(StringName animName)
    {
        if (animName == "RESET") return;
        if (_prevDir == 1) //Change the anim tracks paths to the correct nodes to account for them sliding across.
        {//Left
            NodePath prevPath = _animationPlayer.GetAnimation("GameSlidesFor").TrackGetPath((int)TrackIndexes.Cur);
            for (int i = 3; i >= 0; i--)
            {
                NodePath curPath = _animationPlayer.GetAnimation("GameSlidesFor").TrackGetPath(i);
                _animationPlayer.GetAnimation("GameSlidesFor").TrackSetPath(i, prevPath);
                _animationPlayer.GetAnimation("GameSlidesRev").TrackSetPath(i, prevPath);
                prevPath = curPath;
            }
        }
        else
        {//Right
            NodePath nextPath = _animationPlayer.GetAnimation("GameSlidesRev").TrackGetPath((int)TrackIndexes.Prev);
            for (int i = 0; i < _animatedSprites.Length; i++)
            {
                NodePath curPath = _animationPlayer.GetAnimation("GameSlidesRev").TrackGetPath(i);
                _animationPlayer.GetAnimation("GameSlidesRev").TrackSetPath(i, nextPath);
                _animationPlayer.GetAnimation("GameSlidesFor").TrackSetPath(i, nextPath);
                nextPath = curPath;
            } 
        }
        _sliding = false;
        GD.Print("Current Selection is " + _animatedNames[_currentSelection]);
    }
}
