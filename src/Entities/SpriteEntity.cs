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

    public SpriteEntity(Vector2 position, int spriteId)
    {
        Position = position;
        SpriteId = spriteId;
    }
}
