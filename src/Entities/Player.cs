using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework;
using WolfLike.src.World;

namespace WolfLike.src.Entities;

public class Player
{
    public Vector2 Position { get; private set; }

    public float Angle { get; private set; }

    public float MoveSpeed { get; set; } = 3.0f;
    public float RotationSpeed { get; set; } = 2.5f;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Angle = 0.0f;
    }

    public void Update(float deltaTime, WorldMap worldMap)
    {
        KeyboardState keyboard = Keyboard.GetState();
        HandleRotation(keyboard, deltaTime);
        HandleMovement(keyboard, deltaTime, worldMap);
    }

    private void HandleMovement(KeyboardState keyboard, float deltaTime, WorldMap worldMap)
    {
        Vector2 forward = new Vector2(
            MathF.Cos(Angle),
            MathF.Sin(Angle)
        );

        Vector2 right = new Vector2(
            -forward.Y,
            forward.X
        );

        Vector2 movement = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            movement += forward;
        if (keyboard.IsKeyDown(Keys.S))
            movement -= forward;
        if (keyboard.IsKeyDown(Keys.A))
            movement -= right;
        if (keyboard.IsKeyDown(Keys.D))
            movement += right;

        if (movement == Vector2.Zero)
            return;

        movement.Normalize();

        Vector2 newPosition = Position + movement * MoveSpeed * deltaTime;

        if (!worldMap.IsWallAt(newPosition.X, newPosition.Y))
            Position = new Vector2(newPosition.X, newPosition.Y);

        if (!worldMap.IsWallAt(Position.X, Position.Y))
            Position = new Vector2(Position.X, Position.Y);
    }

    private void HandleRotation(KeyboardState keyboard, float deltaTime)
    {
        if (keyboard.IsKeyDown(Keys.Q))
            Angle -= RotationSpeed * deltaTime;

        if (keyboard.IsKeyDown(Keys.E))
            Angle += RotationSpeed * deltaTime;
    }
}
