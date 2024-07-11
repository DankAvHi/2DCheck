using System;
using Godot;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400;

    public Vector2 ScreenSize;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero;
        AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        velocity = GetMovement(velocity).Normalized() * Speed;
        PlayAnimation(velocity, animatedSprite2D);

        MovePlayer(velocity, delta);
    }

    private static Vector2 GetMovement(Vector2 velocity)
    {
        if (Input.IsActionPressed("MoveUp"))
        {
            velocity.Y -= 1;
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            velocity.X += 1;
        }
        if (Input.IsActionPressed("MoveDown"))
        {
            velocity.Y += 1;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            velocity.X -= 1;
        }
        return velocity;
    }

    private static void PlayAnimation(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
    {
        if (velocity.Length() > 0)
        {
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }
        SwitchAnimation(velocity, animatedSprite2D);
    }

    private static void SwitchAnimation(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
    {
        if (velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.Y > 0;
        }
    }

    private void MovePlayer(Vector2 velocity, double delta)
    {
        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0 + 27, ScreenSize.X - 27),
            y: Mathf.Clamp(Position.Y, 0 + 34, ScreenSize.Y - 34)
        );
    }

    private void OnBodyEntered(Node2D body)
    {
        Hide();
        EmitSignal(SignalName.Hit);
        GetNode<CollisionShape2D>("CollisionShape2D")
            .SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}
