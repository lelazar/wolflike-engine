/** For now:
 * Position = where the object is in the world;
 * SpriteId = which placeholder texture to use;
 * Scale = how large it should appear;
 * IsVisible = whether it should be rendered
 */

using Microsoft.Xna.Framework;
using System;
using WolfLike.src.World;

namespace WolfLike.src.Entities;

public class SpriteEntity
{
    private const float DAMAGEFLASHDURATIONSECONDS = 0.15f;
    private float _damageFlashTimer;

    public Vector2 Position { get; set; }
    public int SpriteId { get; set; }
    public float Scale { get; set; } = 1.0f;
    public bool IsVisible { get; set; } = true;
    public bool IsDamageable { get; set; }
    public int Health { get; private set; } = 100;
    public float CollisionRadius { get; set; } = 0.35f;
    public bool IsAlive => Health > 0;
    public bool IsDamageFlashVisible => _damageFlashTimer > 0.0f;
    public int ContactDamage { get; set; } = 10;
    public float ContactDamageRadius { get; set; } = 0.65f;
    public bool IsAiControlled { get; set; }
    public float DetectionRange { get; set; } = 6.0f;
    public float MoveSpeed { get; set; } = 1.25f;
    public float StopDistance { get; set; } = 0.7f;

    public SpriteEntity(Vector2 position, int spriteId)
    {
        Position = position;
        SpriteId = spriteId;
    }

    public void Update (float deltaTime)
    {
        if (_damageFlashTimer > 0.0f)
            _damageFlashTimer -= deltaTime;
    }

    public void UpdateAi(float deltaTime, Player player, WorldMap worldMap)
    {
        if (!IsAiControlled) return;
        if (!IsVisible || !IsAlive) return;
        if (player == null || !player.IsAlive) return;

        Vector2 toPlayer = player.Position - Position;
        float distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer > DetectionRange) return;
        if (distanceToPlayer <= StopDistance) return;

        Vector2 direction = toPlayer / distanceToPlayer;
        Vector2 movement = direction * MoveSpeed * deltaTime;

        TryMove(movement, worldMap);
    }

    private void TryMove(Vector2 movement, WorldMap worldMap)
    {
        Vector2 newPosition = Position + movement;

        if (!worldMap.IsWallAt(newPosition.X, Position.Y))
            Position = new Vector2(newPosition.X, Position.Y);
        if (!worldMap.IsWallAt(Position.X, newPosition.Y))
            Position = new Vector2(Position.X, newPosition.Y);
    }

    public void TakeDamage(int damage)
    {
        if (!IsDamageable || !IsAlive)
            return;

        Health -= damage;
        _damageFlashTimer = DAMAGEFLASHDURATIONSECONDS;

        if (Health <= 0)
        {
            Health = 0;
            IsVisible = false;
        }
    }
}
