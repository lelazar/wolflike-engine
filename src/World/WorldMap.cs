namespace WolfLike.src.World;

public class WorldMap
{
    //private readonly int[,] _tiles =
    //{
    //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
    //    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    //    { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
    //    { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
    //    { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
    //    { 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
    //    { 1, 0, 0, 1, 1, 1, 0, 0, 0, 1 },
    //    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    //    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    //    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    //};

    // Adding more wall types to the map (0 = empty, 1 = gray, 2 = red, 3 = blue)
    private readonly int[,] _tiles =
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 2, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 2, 0, 0, 3, 0, 1 },
        { 1, 0, 0, 0, 2, 0, 0, 3, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 3, 0, 1 },
        { 1, 0, 0, 1, 1, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
    };

    public int Width => _tiles.GetLength(1);
    public int Height => _tiles.GetLength(0);

    public int GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return 1;

        return _tiles[y, x];
    }

    public bool IsWall(int x, int y) => GetTile(x, y) == 1;

    public bool IsWallAt(float x, float y)
    {
        int mapX = (int)x;
        int mapY = (int)y;

        return IsWall(mapX, mapY);
    }
}
