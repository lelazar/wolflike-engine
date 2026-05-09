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

            if (worldMap.IsWall(mapX, mapY))
            {
                return new RaycastHit
                {
                    HitWall = true,
                    Position = currentPosition,
                    Distance = distance,
                    MapX = mapX,
                    MapY = mapY
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
            MapX = (int)endPosition.X,
            MapY = (int)endPosition.Y
        };
    }
}
