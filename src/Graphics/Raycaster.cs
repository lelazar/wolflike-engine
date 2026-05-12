/** This is the first true raycasting logic.
 ** The key part is here: Vector2 currentPosition = origin + direction * distance;
 ** That means: Take the player position, move forward in the ray direction, check this position
 ** Then if the ray has entered a wall tile, we stop: if (worldMap.IsWall(mapX, mapY))
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using WolfLike.src.World;

namespace WolfLike.src.Graphics;

public class Raycaster
{
    //private const float STEPSIZE = 0.02f; -> Not needed for DDA
    private const float MAXDISTANCE = 20.0f;

    public RaycastHit CastRay(Vector2 origin, float angle, WorldMap worldMap)
    {
        Vector2 rayDirection = new Vector2(MathF.Cos(angle), MathF.Sin(angle));  // The direction where the ray travels
        // Examples:
        // Angle 0 degrees->right
        // Angle 90 degrees->down
        // Angle 180 degrees->left
        // Angle 270 degrees->up
        // In screen/map coordinates, positive Y goes downward, so this behavior is expected

        // These tell us which tile the ray currently occupies
        int mapX = (int)origin.X;
        int mapY = (int)origin.Y;

        // How far the ray must travel to cross one vertical grid line
        float deltaDistanceX = rayDirection.X == 0 ? float.MaxValue : MathF.Abs(1.0f / rayDirection.X);
        // How far the ray must travel to cross one horizontal grid line
        float deltaDistanceY = rayDirection.Y == 0 ? float.MaxValue : MathF.Abs(1.0f / rayDirection.Y);

        // These tell the ray which direction it moves through the grid
        int stepX, stepY;

        // Distances from the ray origin to the first vertical and horizontal grid line depending on whether we look right or left
        float sideDistanceX, sideDistanceY;

        if (rayDirection.X < 0)
        {
            stepX = -1;
            sideDistanceX = (origin.X - mapX) * deltaDistanceX;
        }
        else
        {
            stepX = 1;
            sideDistanceX = (mapX + 1.0f - origin.X) * deltaDistanceX;
        }

        if (rayDirection.Y < 0)
        {
            stepY = -1;
            sideDistanceY = (origin.Y - mapY) * deltaDistanceY;
        }
        else
        {
            stepY = 1;
            sideDistanceY = (mapY + 1.0f - origin.Y) * deltaDistanceY;
        }

        WallHitSide hitSide = WallHitSide.None;
        bool hitWall = false;
        int tileId = 0;

        while (!hitWall)
        {
            // If the ray crossed an X grid line, it hit a vertical side
            // If the ray crossed a Y grid line, it hit a horizontal side
            // That is much more accurate than guessing from the hit position
            if (sideDistanceX < sideDistanceY)
            {
                sideDistanceX += deltaDistanceX;
                mapX += stepX;
                hitSide = WallHitSide.Vertical;
            }
            else
            {
                sideDistanceY += deltaDistanceY;
                mapY += stepY;
                hitSide = WallHitSide.Horizontal;
            }

            tileId = worldMap.GetTile(mapX, mapY);

            if (tileId > 0) hitWall = true;

            float approximateDistance = Vector2.Distance(origin, new Vector2(mapX, mapY));

            if (approximateDistance > MAXDISTANCE) return CreateMissResult(origin, rayDirection, angle);
        }

        float perpendicularWallDistance;

        if (hitSide == WallHitSide.Vertical)
            perpendicularWallDistance = (mapX - origin.X + (1 - stepX) / 2.0f) / rayDirection.X;
        else
            perpendicularWallDistance = (mapY - origin.Y + (1 - stepY) / 2.0f) / rayDirection.Y;

        if (perpendicularWallDistance < 0.0001f)
            perpendicularWallDistance = 0.0001f;

        Vector2 hitPosition = origin + rayDirection * perpendicularWallDistance;

        float wallX;

        // Calculating where exactly the ray hit the wall
        // For textured walls, I will use it like this later:
        // WallX = 0.00->leftmost texture column
        // WallX = 0.50->middle texture column
        // WallX = 0.99->rightmost texture column
        // So this is already preparing the next big version
        if (hitSide == WallHitSide.Vertical)
            wallX = hitPosition.Y;
        else 
            wallX = hitPosition.X;

        wallX -= MathF.Floor(wallX);

        return new RaycastHit
        {
            HitWall = true,
            Position = hitPosition,
            Distance = perpendicularWallDistance,
            Angle = angle,
            MapX = mapX,
            MapY = mapY,
            TileId = tileId,
            HitSide = hitSide,
            WallX = wallX
        };
    }

    private RaycastHit CreateMissResult(Vector2 origin, Vector2 rayDirection, float angle)
    {
        Vector2 endPosition = origin + rayDirection * MAXDISTANCE;

        return new RaycastHit
        {
            HitWall = false,
            Position = endPosition,
            Distance = MAXDISTANCE,
            Angle = angle,
            MapX = (int)endPosition.X,
            MapY = (int)endPosition.Y,
            TileId = 0,
            HitSide = WallHitSide.None,
            WallX = 0.0f
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
    //private WallHitSide DetermineHitSide(Vector2 hitPosition)
    //{
    //    float localX = hitPosition.X - MathF.Floor(hitPosition.X);
    //    float localY = hitPosition.Y - MathF.Floor(hitPosition.Y);

    //    float distanceToVerticalGridLine = MathF.Min(localX, 1.0f - localX);
    //    float distanceToHorizontalGridLine = MathF.Min(localY, 1.0f - localY);

    //    if (distanceToVerticalGridLine < distanceToHorizontalGridLine)
    //        return WallHitSide.Vertical;

    //    return WallHitSide.Horizontal;
    //}
}
