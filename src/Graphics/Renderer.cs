using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using WolfLike.src.Entities;
using WolfLike.src.World;

namespace WolfLike.src.Graphics;

public class Renderer
{
    private Texture2D _pixel;

    private const int TILESIZE = 48;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void DrawTopDownView(SpriteBatch spriteBatch, WorldMap worldMap, Player player)
    {
        spriteBatch.Begin();

        DrawMap(spriteBatch, worldMap);
        DrawPlayer(spriteBatch, player);
        DrawPlayerDirection(spriteBatch, player);

        spriteBatch.End();
    }

    private void DrawPlayerDirection(SpriteBatch spriteBatch, Player player)
    {
        Vector2 start = WorldToScreen(player.Position);

        Vector2 direction = new Vector2(
            MathF.Cos(player.Angle),
            MathF.Sin(player.Angle)
        );

        Vector2 end = start + direction * 32f;

        DrawLine(spriteBatch, start, end, Color.Yellow, 3);
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
    {
        Vector2 edge = end - start;

        float angle = MathF.Atan2(edge.Y, edge.X);

        Rectangle lineRectangle = new Rectangle(
            (int)start.X,
            (int)start.Y,
            (int)edge.Length(),
            thickness
        );

        spriteBatch.Draw(_pixel, lineRectangle, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    private Vector2 WorldToScreen(Vector2 worldPosition) => worldPosition * TILESIZE;

    private void DrawPlayer(SpriteBatch spriteBatch, Player player)
    {
        int playerSize = 12;

        Vector2 screenPosition = WorldToScreen(player.Position);

        Rectangle playerRectangle = new Rectangle(
            (int)screenPosition.X - playerSize / 2,
            (int)screenPosition.Y - playerSize / 2,
            playerSize,
            playerSize
        );

        spriteBatch.Draw(_pixel, playerRectangle, Color.Red);
    }

    private void DrawMap(SpriteBatch spriteBatch, WorldMap worldMap)
    {
        for (int y = 0; y < worldMap.Height; y++)
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                int tile = worldMap.GetTile(x, y);

                Color color = tile switch
                {
                    0 => new Color(35, 35, 35),
                    1 => new Color(180, 180, 180),
                    _ => Color.Magenta
                };

                Rectangle tileRectangle = new Rectangle(
                    x * TILESIZE,
                    y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );

                spriteBatch.Draw(_pixel, tileRectangle, color);

                DrawRectangleBorder(spriteBatch, tileRectangle, Color.Black);
            }
        }
    }

    private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        int thickness = 1;

        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, thickness), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, thickness, rectangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Right - thickness, rectangle.Top, thickness, rectangle.Height), color);
    }
}
