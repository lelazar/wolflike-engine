/** This is the first true raycasting logic.
 ** The key part is here: Vector2 currentPosition = origin + direction * distance;
 ** That means: Take the player position, move forward in the ray direction, check this position
 ** Then if the ray has entered a wall tile, we stop: if (worldMap.IsWall(mapX, mapY))
 */

using Microsoft.Xna.Framework;
using System;
using WolfLike.src.World;

namespace WolfLike.src.Graphics;

public class Raycaster
{
    private const float STEPSIZE = 0.02f;
    private const float MAXDISTANCE = 20.0f;

    public RaycastHit CastRay(Vector2 origin, float angle, WorldMap worldMap)
    {
        Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        float distance = 0.0f;

        while (distance < MAXDISTANCE)
        {
            Vector2 currentPosition = origin + direction * distance;

            int mapX = (int)currentPosition.X;
            int mapY = (int)currentPosition.Y;

            int tileId = worldMap.GetTile(mapX, mapY);

            if (tileId > 0)
            {
                return new RaycastHit
                {
                    HitWall = true,
                    Position = currentPosition,
                    Distance = distance,
                    Angle = angle,
                    MapX = mapX,
                    MapY = mapY,
                    TileId = tileId,
                    HitSide = DetermineHitSide(currentPosition)  // Because the world is tile-based, a wall tile has grid boundaries. The ray enters a wall tile near one of those boundaries
                };
            }

            distance += STEPSIZE;
        }

        Vector2 endPosition = origin + direction * MAXDISTANCE;

        return new RaycastHit
        {
            HitWall = false,
            Position = endPosition,
            Distance = MAXDISTANCE,
            Angle = angle,
            MapX = (int)endPosition.X,
            MapY = (int)endPosition.Y,
            TileId = 0,
            HitSide = WallHitSide.None
        };
    }

    /// <summary>
    /// Check whether the hit point is closer to:
    ///     - left/right tile boundary  -> vertical side
    ///     - top/bottom tile boundary  -> horizontal side
    /// Again, this is an approximation. Later the DDA algorithm will give us this perfectly!
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    private WallHitSide DetermineHitSide(Vector2 hitPosition)
    {
        float localX = hitPosition.X - MathF.Floor(hitPosition.X);
        float localY = hitPosition.Y - MathF.Floor(hitPosition.Y);

        float distanceToVerticalGridLine = MathF.Min(localX, 1.0f - localX);
        float distanceToHorizontalGridLine = MathF.Min(localY, 1.0f - localY);

        if (distanceToVerticalGridLine < distanceToHorizontalGridLine)
            return WallHitSide.Vertical;

        return WallHitSide.Horizontal;
    }
}
