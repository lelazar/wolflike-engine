using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework;
using WolfLike.src.World;

namespace WolfLike.src.Entities;

public class Player
{
    private const float DAMAGE_FLASH_DURATION_SECONDS = 0.18f;
    private const float INVULNERABILITY_DURATION_SECONDS = 0.75f;
    private const float HEAL_FLASH_DURATION_SECONDS = 0.18f;

    private float _damageFlashTimer;
    private float _invulnerabilityTimer;
    private float _healFlashTimer;

    public Vector2 Position { get; private set; }
    public float Angle { get; private set; }
    public int KeysForDoors { get; private set; }
    public float MoveSpeed { get; set; } = 3.0f;
    public float RotationSpeed { get; set; } = 2.5f;
    public int MaxHealth { get; private set; } = 100;
    public int Health { get; private set; } = 100;
    public bool IsAlive => Health > 0;
    public bool IsDamageFlashVisible => _damageFlashTimer > 0.0f;
    public bool IsInvulnerable => _invulnerabilityTimer > 0.0f;
    public bool IsHealFlashVisible => _healFlashTimer > 0.0f;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Angle = 0.0f;
    }

    public void Update(float deltaTime, WorldMap worldMap)
    {
        UpdateStatusTimers(deltaTime);

        if (!IsAlive) return;

        KeyboardState keyboard = Keyboard.GetState();

        HandleRotation(keyboard, deltaTime);
        HandleMovement(keyboard, deltaTime, worldMap);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;
        if (IsInvulnerable) return;

        Health -= damage;

        if (Health < 0) Health = 0;

        _damageFlashTimer = DAMAGE_FLASH_DURATION_SECONDS;
        _invulnerabilityTimer = INVULNERABILITY_DURATION_SECONDS;
    }

    private void UpdateStatusTimers(float deltaTime)
    {
        if (_damageFlashTimer > 0.0f)
            _damageFlashTimer -= deltaTime;
        if (_invulnerabilityTimer > 0.0f)
            _invulnerabilityTimer -= deltaTime;
        if (_healFlashTimer > 0.0f)
            _healFlashTimer -= deltaTime;
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

    public bool Heal(int amount)
    {
        // This method only returns true if healing actually happened

        if (!IsAlive) return false;
        if (amount <= 0) return false;
        if (Health >= MaxHealth) return false;

        Health += amount;

        if (Health > MaxHealth) Health = MaxHealth;

        _healFlashTimer = HEAL_FLASH_DURATION_SECONDS;

        return true;
    }

    public void AddKey(int amount = 1)
    {
        if (amount <= 0) return;
        KeysForDoors += amount;
    }

    public bool TryUseKey()
    {
        if (KeysForDoors <= 0) return false;

        KeysForDoors--;
        return true;
    }
}
