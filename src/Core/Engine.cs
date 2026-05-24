using WolfLike.src.World;
using WolfLike.src.Entities;
using WolfLike.src.Graphics;
using WolfLike.src.Core;
using WolfLike.src.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace WolfLike.src.Core;

public class Engine
{
    private WorldMap _worldMap;
    private Player _player;
    private Renderer _renderer;
    private Raycaster _raycaster;
    //private RaycastHit _centerRayHit; TODO: Replace this with an array/list of ray hits to simulate it as a camera

    private RaycastHit[] _rayHits;
    private List<SpriteEntity> _sprites;

    private Weapon _weapon;

    public Player Player => _player;
    public IReadOnlyList<SpriteEntity> Sprites => _sprites;
    public Weapon Weapon => _weapon;

    /* Ray count */
    // Later, for 3D rendering, I will probably use something closer to the screen width or a reduced logical render resolution
    // For example: private const int RAYCOUNT = 320 or 640;
    // But do not increase it too much yet, because the current raycaster is a simple step-based raycaster
    //private const int RAYCOUNT = 320;  // Controls how many rays are cast (TODO: Increase it to 640 for a sharper image)

    /* Field of View */
    // In radians: PI radians = 180 degrees, so PI / 3 = 60 degrees
    // A classic FPS usually uses something around 60° to 90° so for now, 60 is good
    //private const float FIELDOFVIEW = MathF.PI / 3.0f;  // 60 degrees

    public void Initialize()
    {
        _worldMap = new();

        _player = new Player(new Vector2(2.5f, 7.5f));

        _renderer = new();
        _raycaster = new();

        _rayHits = new RaycastHit[GameSettings.RAYCOUNT];

        _sprites = new List<SpriteEntity>
        {
            new SpriteEntity(new Vector2(6.5f, 5.5f), 1)
            {
                Scale = 1.0f,
                IsDamageable = true,    // Only the green sprite can be damaged
                CollisionRadius = 0.35f,
                ContactDamage = 10,
                ContactDamageRadius = 0.75f,
                IsAiControlled = true,  // Only the green enemy moves
                DetectionRange = 7.0f,  // 4.0 - short awareness; 7.0 - medium awareness; 10.0 - aggressive awareness
                MoveSpeed = 1.2f,       // 0.8 - slow zombie; 1.2 - normal enemy; 2.0 - fast enemy
                StopDistance = 0.75f    // 0.65 - close attack; 0.9  - safer contact range; 1.2  - enemy stops farther away
            },

            new SpriteEntity(new Vector2(2.5f, 2.5f), 2)
            {
                Scale = 0.65f,
                IsDamageable = false,
                IsAiControlled = false
            }
        };

        _weapon = new();
    }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
    {
        _renderer.LoadContent(graphicsDevice, content);
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _player.Update(deltaTime, _worldMap);

        // Now sprites need to update every frame
        foreach (SpriteEntity sprite in _sprites)
        {
            sprite.Update(deltaTime);
            //sprite.UpdateAi(deltaTime, _player, _worldMap);

            // The enemy chases only if it has direct LOS to the player
            if (sprite.IsAiControlled && HasLineOfSightToPlayer(sprite))
                sprite.UpdateAi(deltaTime, _player, _worldMap);
        }

        // Need to check enemy contact every frame
        HandleEnemyContactDamage();
        
        CastFieldOfViewRays();

        if (_player.IsAlive)
        {
            _weapon.Update(deltaTime);

            if (_weapon.IsFiring)
                HandleWeaponFire();  // When the weapon fires, the engine checks for a target
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        //_renderer.DrawTopDownView(spriteBatch, _worldMap, _player, _rayHits);
        _renderer.DrawRaycastView(spriteBatch, _worldMap, _player, _rayHits, _sprites, _weapon);
    }

    // Calculating many ray angles
    private void CastFieldOfViewRays()
    {
        float startAngle = _player.Angle - GameSettings.FIELDOFVIEW / 2.0f;  // Starts the first ray on the left side of the player's view
        float angleStep = GameSettings.FIELDOFVIEW / (_rayHits.Length - 1);  // How much angle difference exists between each ray

        for (int i = 0; i < _rayHits.Length; i++)
        {
            float rayAngle = startAngle + i * angleStep;  // Calculates the actual angle for each ray
            // For example if Player angle = 0 degrees, FOV = 60 degrees, then the rays go approx. from -30 degrees to +30 degrees

            _rayHits[i] = _raycaster.CastRay(_player.Position, rayAngle, _worldMap);
        }
    }

    private void HandleWeaponFire()
    {
        // This simple method finds the target then damages it

        SpriteEntity target = FindShootTarget();

        if (target == null) 
            return;

        target.TakeDamage(_weapon.Damage);
        _weapon.RegisterHit();  // When an enemy is hit, it will take damage and the weapon shows hit marker
    }

    private SpriteEntity FindShootTarget()
    {
        // This method checks whether the enemy is close enough to the player’s center aim

        if (_sprites == null || _sprites.Count == 0) 
            return null;

        RaycastHit centerRayHit = GetCenterRayHit();

        SpriteEntity closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (SpriteEntity sprite in _sprites)
        {
            if (!sprite.IsVisible)
                continue;
            if (!sprite.IsDamageable)
                continue;
            if (!sprite.IsAlive)
                continue;

            Vector2 toSprite = sprite.Position - _player.Position;
            float distanceToSprite = toSprite.Length();

            if (distanceToSprite <= 0.0001f)
                continue;
            if (IsBlockedByWall(centerRayHit, distanceToSprite))
                continue;

            float spriteAngle = MathF.Atan2(toSprite.Y, toSprite.X);
            float angleDifference = NormalizeAngle(spriteAngle - _player.Angle);
            float angularRadius = MathF.Atan(sprite.CollisionRadius / distanceToSprite);  // near enemy = easier to hit on screen; far enemy = smaller target
            float minimumHitAngle = 0.025f;
            float allowedHitAngle = MathF.Max(angularRadius, minimumHitAngle);

            if (MathF.Abs(angleDifference) > allowedHitAngle)
                continue;

            if (distanceToSprite < closestDistance)
            {
                closestDistance = distanceToSprite;
                closestTarget = sprite;
            }
        }

        return closestTarget;
    }

    private RaycastHit GetCenterRayHit()
    {
        // The center ray represents what the player is aiming at

        if (_rayHits == null || _rayHits.Length == 0)
            return null;

        int centerIndex = _rayHits.Length / 2;

        return _rayHits[centerIndex];
    }

    private bool IsBlockedByWall(RaycastHit centerRayHit, float targetDistance)
    {
        // This method prevents shooting through walls
        // If a wall is closer than the enemy, the shot is blocked

        if (centerRayHit == null)
            return false;
        if (!centerRayHit.HitWall)
            return false;
        return centerRayHit.Distance < targetDistance;
    }

    private float NormalizeAngle(float angle)
    {
        // This method makes angle comparison reliable when rotating around 0, PI and -PI

        while (angle > MathF.PI)
            angle -= MathF.Tau;
        while (angle < -MathF.PI)
            angle += MathF.Tau;
        return angle;
    }

    private void HandleEnemyContactDamage()
    {
        // This method makes live damageable enemies hurt the player when close
        // Because the player has invulnerability frames, this will not drain health every frame

        if (!_player.IsAlive) return;

        foreach (SpriteEntity sprite in _sprites)
        {
            if (!sprite.IsVisible) continue;
            if (!sprite.IsAlive) continue;
            if (!sprite.IsDamageable) continue;

            float distance = Vector2.Distance(_player.Position, sprite.Position);

            if (distance <= sprite.ContactDamageRadius)
                _player.TakeDamage(sprite.ContactDamage);
        }
    }

    private bool HasLineOfSightToPlayer(SpriteEntity sprite)
    {
        // With the help of this class' Raycaster, the engine can decide whether the enemy can "see" the player

        Vector2 toPlayer = _player.Position - sprite.Position;
        float distanceToPlayer = toPlayer.Length();

        if (distanceToPlayer <= 0.0001f)
            return true;

        float angleToPlayer = MathF.Atan2(toPlayer.Y, toPlayer.X);

        RaycastHit wallHit = _raycaster.CastRay(sprite.Position, angleToPlayer, _worldMap);

        if (wallHit == null || !wallHit.HitWall)
            return true;

        return wallHit.Distance > distanceToPlayer;
    }
}
