using Microsoft.Xna.Framework;

namespace WolfLike.src.World;

public class LevelEntitySpawn
{
    public Vector2 Position { get; set; }
    public LevelEntityType EntityType { get; set; }

    public LevelEntitySpawn(Vector2 position, LevelEntityType entityType)
    {
        Position = position;
        EntityType = entityType;
    }
}
