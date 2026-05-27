using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WolfLike.src.World;

public class LevelData
{
    public int[,] Tiles { get; set; }
    public Vector2 PlayerStartPosition { get; set; }
    public List<LevelEntitySpawn> EntitySpawns { get; } = new();
    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);
}
