using WolfLike.src.World;
using WolfLike.src.Entities;
using WolfLike.src.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace WolfLike.src.Core;

public class Engine
{
    private WorldMap _worldMap;
    private Player _player;
    private Renderer _renderer;
    private Raycaster _raycaster;
    //private RaycastHit _centerRayHit; TODO: Replace this with an array/list of ray hits to simulate it as a camera

    private RaycastHit[] _rayHits;

    /* Ray count */
    // Later, for 3D rendering, I will probably use something closer to the screen width or a reduced logical render resolution
    // For example: private const int RAYCOUNT = 320 or 640;
    // But do not increase it too much yet, because the current raycaster is a simple step-based raycaster
    private const int RAYCOUNT = 320;  // Controls how many rays are cast (TODO: Increase it to 640 for a sharper image)

    /* Field of View */
    // In radians: PI radians = 180 degrees, so PI / 3 = 60 degrees
    // A classic FPS usually uses something around 60° to 90° so for now, 60 is good
    private const float FIELDOFVIEW = MathF.PI / 3.0f;  // 60 degrees

    public void Initialize()
    {
        _worldMap = new();

        _player = new Player(new Vector2(2.5f, 7.5f));

        _renderer = new();
        _raycaster = new();

        _rayHits = new RaycastHit[RAYCOUNT];
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
        //_centerRayHit = _raycaster.CastRay(_player.Position, _player.Angle, _worldMap);

        CastFieldOfViewRays();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        //_renderer.DrawTopDownView(spriteBatch, _worldMap, _player, _rayHits);
        _renderer.DrawRaycastView(spriteBatch, _worldMap, _player, _rayHits);
    }

    // Calculating many ray angles
    private void CastFieldOfViewRays()
    {
        float startAngle = _player.Angle - FIELDOFVIEW / 2.0f;  // Starts the first ray on the left side of the player's view
        float angleStep = FIELDOFVIEW / (_rayHits.Length - 1);  // How much angle difference exists between each ray

        for (int i = 0; i < _rayHits.Length; i++)
        {
            float rayAngle = startAngle + i * angleStep;  // Calculates the actual angle for each ray
            // For example if Player angle = 0 degrees, FOV = 60 degrees, then the rays go approx. from -30 degrees to +30 degrees

            _rayHits[i] = _raycaster.CastRay(_player.Position, rayAngle, _worldMap);
        }
    }
}
