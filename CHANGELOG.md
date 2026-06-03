# Changelog

All notable changes to WolfEngine will be documented in this file.

## [v0.25] - Door and Pickup Feedback Polish

### Added
- Added `GameMessage` and `MessageLog`
- Added temporary on-screen notification messages
- Added pickup feedback messages
- Added door interaction feedback messages
- Added no-ammo feedback message
- Added enemy defeated message
- Added feedback cooldown to prevent pickup message spam

### Changed
- Door and pickup interactions now provide player-facing feedback
- UI feedback is now routed through a reusable message system

## [v0.24] - Keys and Locked Doors

### Added
- Added key pickup entity support
- Added `K` level symbol for key pickups
- Added locked door tile type
- Added `L` level symbol for locked doors
- Added key inventory to player
- Added key pickup collection logic
- Added locked door interaction logic
- Added locked door texture
- Added key pickup sprite
- Added key counter HUD
- Added locked-door interaction prompts

### Changed
- Door interaction now distinguishes normal and locked doors
- Locked doors require and consume one key to open

## [v0.23] - Doors and Interactions

### Added
- Added door tile type
- Added `D` level symbol for closed doors
- Added door texture
- Added map tile mutation through `WorldMap.SetTile`
- Added `WorldMap.IsDoor` and `WorldMap.OpenDoor`
- Added interaction key with `F`
- Added door detection in front of the player
- Added interaction prompt
- Added interaction debug HUD state

### Changed
- Closed doors now behave like solid wall tiles until opened
- Restart reloads closed doors from the level file

## [v0.22] - Ammo System and Ammo Pickups

### Added
- Added weapon ammo and max ammo
- Added ammo consumption when firing
- Added no-ammo firing behavior
- Added ammo pickup entity support
- Added `A` level symbol for ammo pickups
- Added procedural ammo pickup sprite
- Added ammo pickup collection logic
- Added ammo counter HUD
- Added no-ammo warning feedback
- Added ammo data to debug HUD

### Changed
- Weapon can no longer fire when ammo is empty
- Pickups now support separate healing and ammo behavior

## [v0.21] - Pickups and Healing

### Added
- Added pickup entity support
- Added healing pickup behavior
- Added player healing logic
- Added heal feedback overlay
- Added pickup collection handling
- Added pickup counter to debug HUD

### Changed
- Level `H` spawn now creates a functional healing pickup
- Pickups are removed only when healing is successfully applied

## [v0.20] - Simple Map Loader

### Added
- Added text-based level file format
- Added `LevelLoader`
- Added `LevelData`
- Added `LevelEntitySpawn`
- Added `LevelEntityType`
- Added support for player spawn from level files
- Added support for enemy and pickup spawns from level files
- Added level file validation
- Added runtime map loading from `Content/Maps/level01.txt`

### Changed
- `WorldMap` now receives tile data from loaded level data
- Game restart now reloads level data from file
- Map layout is no longer hardcoded in `WorldMap.cs`

## [v0.19] - Multiple Enemies and Level Tuning

### Added
- Added multiple enemy instances
- Added enemy stat variations for speed, health, detection range, and contact damage
- Added helper methods for enemy and pickup creation
- Added support for enemy max health
- Added larger test map layout
- Added improved debug HUD enemy health list

### Changed
- Victory condition now supports multiple damageable enemies
- Level spawn positions were retuned for a larger encounter area

## [v0.18] - Game State and Restart

### Added
- Added `GameState` enum
- Added playing, player-dead, and victory states
- Added restart input with R
- Added game-state overlay
- Added victory condition when all damageable enemies are defeated
- Added death state when player health reaches zero

### Changed
- Refactored game setup into reusable restart logic
- Gameplay updates now pause when the player is dead or victory is reached

## [v0.17] - Basic Enemy AI Movement

### Added
- Added AI-controlled sprite behavior
- Added enemy detection range
- Added enemy movement speed
- Added enemy stop distance
- Added simple enemy chase movement
- Added enemy wall collision during movement
- Added optional line-of-sight check for enemy chasing
- Added AI enemy count to debug HUD

### Changed
- Damageable enemy placeholder can now actively move toward the player

## [v0.16] - Player Health and Enemy Contact Damage

### Added
- Added player health and max health
- Added player damage handling
- Added invulnerability timer after damage
- Added player damage flash overlay
- Added enemy contact damage
- Added player health bar
- Added player health data to debug HUD
- Added optional death overlay

### Changed
- Damageable sprites can now damage the player on contact
- Player movement stops when health reaches zero

## [v0.15] - Enemy Feedback and Debug HUD

### Added
- Added crosshair rendering
- Added hit marker feedback
- Added enemy damage flash
- Added sprite update loop
- Added weapon hit marker timer
- Added debug HUD with player and enemy information
- Added SpriteFont-based debug text rendering

### Changed
- Weapon now registers successful hits
- Damageable sprites now provide visual feedback when hit

## [v0.14] - Enemy Hit Detection

### Added
- Added damageable sprite entities
- Added sprite health
- Added weapon damage
- Added center-view shooting hit detection
- Added wall-blocking check for shots
- Added enemy removal when health reaches zero

### Changed
- Weapon firing now affects world entities

## [v0.13] - Weapon Placeholder and Shooting Input

### Added
- Added `Weapon` class
- Added shooting input with Space and left mouse button
- Added weapon fire cooldown
- Added procedural weapon placeholder texture
- Added procedural muzzle flash texture
- Added bottom-center weapon rendering
- Added short muzzle flash effect when firing

### Changed
- Renderer now draws a weapon overlay after world rendering

## [v0.12] - Sprite/Object Rendering

### Added
- Added `SpriteEntity` class
- Added procedural sprite textures
- Added sprite rendering in the raycast view
- Added distance-based sprite scaling
- Added sprite sorting from far to near
- Added basic sprite wall occlusion using ray distances
- Added placeholder enemy and pickup objects

### Changed
- Renderer now supports world-space billboard objects

## [v0.11] - Texture Orientation and Camera Settings

### Added
- Added central `GameSettings` class
- Added ray direction storage to raycast hit results
- Added texture orientation correction based on hit side and ray direction

### Changed
- Moved screen size, ray count, field of view, texture size, and max ray distance into `GameSettings`
- Updated renderer, raycaster, and texture manager to use centralized settings
- Improved wall texture consistency when viewing walls from different sides

## [v0.10] - Textured Walls

### Added
- Added procedural wall texture generation
- Added `TextureManager`
- Added textured wall slice rendering
- Added texture column selection using DDA `WallX`
- Added texture tinting for side and distance shading

### Changed
- Replaced flat colored wall rectangles with textured wall slices
- Updated raycast rendering to use point-clamped pixel-style sampling

## [v0.9] - DDA Raycasting

### Added
- Added DDA-based raycasting
- Added exact vertical and horizontal wall side detection
- Added wall hit coordinate data for future texture mapping
- Added miss result handling for rays exceeding maximum distance

### Changed
- Replaced step-based ray marching with grid-based DDA traversal
- Removed need for fish-eye correction in the renderer
- Improved raycasting precision and performance

## [v0.8] - Wall Side Coloring and Depth Shading

### Added
- Added wall hit side detection
- Added side-based wall brightness
- Added distance-based wall shading
- Added support for multiple wall tile colors
- Added tile ID storage to raycast results

### Changed
- Improved fake-3D wall readability
- Simplified main raycast view by keeping the minimap optional

## [v0.7] - Basic 2.5D Wall Renderer

### Added
- Added fake-3D wall slice rendering
- Added ceiling and floor background
- Added fish-eye distance correction
- Added top-left minimap overlay (I commented out the drawing of the minimap for testing)
- Increased ray count for smoother projection

### Changed
- Renderer now draws the main raycasted first-person view instead of only the top-down debug view

## [v0.6] - Field-of-View Rays

### Added
- Added multiple raycasting across a 60-degree field of view
- Added top-down field-of-view debug visualization
- Added configurable ray count
- Added transparent ray rendering

### Changed
- Replaced single center ray storage with ray hit array

## [v0.4] - Player Movement and Collision

### Added
- Added `Player` class
- Added player movement
- Added player rotation
- Added basic wall collision
- Added top-down debug rendering

### Changed
- Moved game logic out of `Game1.cs` into `Engine.cs`

## [v0.3] - Top-Down Map Rendering

### Added
- Added `WorldMap` class
- Added tile-based map representation
- Added top-down map renderer

## [v0.2] - Engine Structure

### Added
- Added initial folder structure:
  - `Core`
  - `Graphics`
  - `World`
  - `Entities`

## [v0.1] - Project Setup

### Added
- Created MonoGame project
- Opened initial game window
