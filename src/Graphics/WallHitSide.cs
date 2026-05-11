// This is not perfect yet because the current raycaster is still step-based, but it is good enough for v0.8

namespace WolfLike.src.Graphics;

public enum WallHitSide
{
    None,
    Vertical,   // Ray hit an east/west wall boundary
    Horizontal  // Ray hit a north/south wall boundary
}