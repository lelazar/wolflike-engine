/** For now:
 * Position = where the object is in the world;
 * SpriteId = which placeholder texture to use;
 * Scale = how large it should appear;
 * IsVisible = whether it should be rendered
 */

using Microsoft.Xna.Framework;

namespace WolfLike.src.Entities;

public class SpriteEntity
{
    public Vector2 Position { get; set; }
    public int SpriteId { get; set; }
    public float Scale { get; set; } = 1.0f;
    public bool IsVisible { get; set; } = true;
    public bool IsDamageable { get; set; }
    public int Health { get; private set; } = 100;
    public float CollisionRadius { get; set; } = 0.35f;
    public bool IsAlive => Health > 0;

    public SpriteEntity(Vector2 position, int spriteId)
    {
        Position = position;
        SpriteId = spriteId;
    }

    public void TakeDamage(int damage)
    {
        if (!IsDamageable || !IsAlive)
            return;

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            IsVisible = false;
        }
    }
}
