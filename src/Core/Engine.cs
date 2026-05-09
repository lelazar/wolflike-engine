using WolfLike.src.Core;
using WolfLike.src.World;
using WolfLike.src.Entities;
using WolfLike.src.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WolfLike.src.Core;

public class Engine
{
    private WorldMap _worldMap;
    private Player _player;
    private Renderer _renderer;
    private Raycaster _raycaster;
    private RaycastHit _centerRayHit;

    public void Initialize()
    {
        _worldMap = new();

        _player = new Player(new Vector2(3.5f, 3.5f));

        _renderer = new();
        _raycaster = new();
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _renderer.LoadContent(graphicsDevice);
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _player.Update(deltaTime, _worldMap);

        // Cast one ray in the exact direction the player is facing
        _centerRayHit = _raycaster.CastRay(_player.Position, _player.Angle, _worldMap);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _renderer.DrawTopDownView(spriteBatch, _worldMap, _player, _centerRayHit);
    }
}
