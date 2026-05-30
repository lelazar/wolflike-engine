/**
 * Space = shoot
 * Left mouse button = shoot
 * The weapon only fires once per press because we use edge detection: keyboard.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key)
 * So holding Space will not rapidly fire every frame
 */

/**
 * Important concept: weapon is not a world sprite
 * always drawn in front;
 * always at bottom center;
 * not affected by walls;
 * not affected by distance;
 * not placed in the world map
 * This is why draw it after everything else
 */

using Microsoft.Xna.Framework.Input;

namespace WolfLike.src.Gameplay;

public class Weapon
{
    private const float FIRECOOLDOWNSECONDS        = 0.35f;
    private const float MUZZLEFLASHDURATIONSECONDS = 0.08f;
    private const float HITMARKERDURATIONSECONDS   = 0.12f;
    private const float EMPTYCLICKDURATIONSECONDS  = 0.10f;

    private float _cooldownTimer;
    private float _muzzleFlashTimer;
    private float _hitMarkerTimer;
    private float _emptyClickTimer;

    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    public int Damage { get; set; } = 50;
    public int MaxAmmo { get; private set; } = 24;
    public int Ammo { get; private set; } = 12;
    public bool IsFiring { get; private set; }
    public bool IsMuzzleFlashVisible => _muzzleFlashTimer > 0.0f;
    public bool IsHitMarkerVisible => _hitMarkerTimer > 0.0f;
    public bool IsEmptyClickVisible => _emptyClickTimer > 0.0f;
    public bool HasAmmo => Ammo > 0;

    public Weapon()
    {
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();
    }

    public void Update(float deltaTime)
    {
        IsFiring = false;

        if (_cooldownTimer > 0.0f)
            _cooldownTimer -= deltaTime;
        if (_muzzleFlashTimer > 0.0f)
            _muzzleFlashTimer -= deltaTime;
        if (_hitMarkerTimer  > 0.0f)
            _hitMarkerTimer -= deltaTime;
        if (_emptyClickTimer > 0.0f)
            _emptyClickTimer -= deltaTime;

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();

        bool firePressed = IsKeyPressed(keyboard, Keys.Space) || IsLeftMousePressed(mouse);

        if (firePressed && _cooldownTimer <= 0.0f)
            TryFire();

        _previousKeyboardState = keyboard;
        _previousMouseState = mouse;
    }

    public bool AddAmmo(int amount)
    {
        if (amount <= 0) return false;
        if (Ammo >= MaxAmmo) return false;

        Ammo += amount;

        if (Ammo > MaxAmmo) Ammo = MaxAmmo;

        return true;
    }

    public void RegisterHit()
    {
        // When an enemy is hit, the engine will call this method
        _hitMarkerTimer = HITMARKERDURATIONSECONDS;
    }

    private void TryFire()
    {
        // With this method, every valid shot consumes one ammo

        if (Ammo <= 0)
        {
            _emptyClickTimer = EMPTYCLICKDURATIONSECONDS;
            _cooldownTimer = FIRECOOLDOWNSECONDS * 0.5f;
            return;
        }

        Ammo--;

        IsFiring = true;

        _cooldownTimer = FIRECOOLDOWNSECONDS;
        _muzzleFlashTimer = MUZZLEFLASHDURATIONSECONDS;
    }

    private bool IsKeyPressed(KeyboardState keyboard, Keys key) => keyboard.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

    private bool IsLeftMousePressed(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
}
