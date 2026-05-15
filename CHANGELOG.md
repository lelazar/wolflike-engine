# Changelog

All notable changes to WolfEngine will be documented in this file.

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
