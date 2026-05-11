/** This class stores the result of a raycast:
 ** HitWall  = did the ray hit a wall?
 ** Position = exact hit position in world coordinates
 ** Distance = distance from player to hit point
 ** MapX/Y   = map tile that was hit
 */

using Microsoft.Xna.Framework;

namespace WolfLike.src.Graphics;

public class RaycastHit
{
    public bool HitWall { get; set; }
    public Vector2 Position { get; set; }
    public float Distance { get; set; }
    public float Angle { get; set; }
    public int MapX { get; set; }
    public int MapY { get; set; }
    public int TileId { get; set; }           // Allowing us to color different wall types
    public WallHitSide HitSide { get; set; }  // Allowing us to shade different wall sides differently
}
