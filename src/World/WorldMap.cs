namespace WolfLike.src.World;

public class WorldMap
{
    private readonly int[,] _tiles;

    public WorldMap(int[,] tiles)
    {
        _tiles = tiles;
    }

    public int Width => _tiles.GetLength(1);
    public int Height => _tiles.GetLength(0);

    public int GetTile(int x, int y)
    {
        // This function treats anything outside the map as wall
        // Even if we remove the corner wall for example, the outside of the map is still treated as solid wall
        // So changing the bottom-right corner from 1 to 0 opens the tile inside the map, but beyond it, the engine still sees "outside world = wall"
        // That is good behavior because it prevents the player/rays from escaping the map
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return 1;

        return _tiles[y, x];  // The first number is the row, meaning Y. The second number is the column, meaning X
    }

    public bool IsWall(int x, int y) => GetTile(x, y) > 0;
    // Now with > 0, the meaning becomes:
    // 0 = empty / walkable
    // 1 = wall type 1 / solid
    // 2 = wall type 2 / solid
    // 3 = wall type 3 / solid
    // That is needed, because with == 1, it did not recognize brick and metal walls (Tile IDs of 2 and 3) as collidable walls!

    public bool IsWallAt(float x, float y)
    {
        int mapX = (int)x;
        int mapY = (int)y;

        return IsWall(mapX, mapY);
    }
}
