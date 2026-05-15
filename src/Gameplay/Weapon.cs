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

    private float _cooldownTimer;
    private float _muzzleFlashTimer;

    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    public bool IsFiring { get; private set; }
    public bool IsMuzzleFlashVisible => _muzzleFlashTimer > 0.0f;

    public void Update(float deltaTime)
    {
        IsFiring = false;

        if (_cooldownTimer > 0.0f)
            _cooldownTimer -= deltaTime;
        if (_muzzleFlashTimer > 0.0f)
            _muzzleFlashTimer -= deltaTime;

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();

        bool firePressed = IsKeyPressed(keyboard, Keys.Space) || IsLeftMousePressed(mouse);

        if (firePressed && _cooldownTimer <= 0.0f)
            Fire();

        _previousKeyboardState = keyboard;
        _previousMouseState = mouse;
    }

    private void Fire()
    {
        IsFiring = true;

        _cooldownTimer = FIRECOOLDOWNSECONDS;
        _muzzleFlashTimer = MUZZLEFLASHDURATIONSECONDS;
    }

    private bool IsKeyPressed(KeyboardState keyboard, Keys key) => keyboard.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

    private bool IsLeftMousePressed(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
}
