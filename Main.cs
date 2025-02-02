using System;
using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int score;

    public override void _Ready() { }

    public override void _Process(double delta) { }

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame()
    {
        score = 0;

        Player player = GetNode<Player>("Player");
        Marker2D startPosition = GetNode<Marker2D>("StartPosition");

        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        HUD hud = GetNode<HUD>("HUD");
        hud.UpdateScore(score);
        hud.ShowMessage("Get Ready!");

        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        GetNode<AudioStreamPlayer>("Music").Play();
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnScoreTimerTimeout()
    {
        score++;
        GetNode<HUD>("HUD").UpdateScore(score);
    }

    private void OnMobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();

        PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        mob.Position = mobSpawnLocation.Position;

        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        Vector2 velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);

        mob.LinearVelocity = velocity.Rotated(direction);

        AddChild(mob);
    }
}
