# 2D Physics Engine

A 2D rigid body physics engine written from scratch in C#, with a WPF demo.
Circles collide with each other and with thick line segments, with impulse-based
response, friction and restitution.

![Demo](docs/demo.gif)

## Features
- Circle bodies with mass derived from radius
- Circle vs thick line segment collision (closest point on segment, clamped)
- Circle vs circle impulse resolution with restitution and mass-weighted position correction
- Slope friction, with a restitution threshold to stop resting jitter
- Semi-implicit Euler integration, delta time clamped to 50 ms
- Engine is a standalone class library with no WPF dependency
- xUnit tests, including momentum conservation in ball-ball collisions

## Controls
| Key | Action |
|---|---|
| Left / Right | Move the blue ball |
| Space | Jump (when grounded) |
| 0 | Spawn a large ball at the cursor |
| 1 | Spawn a small ball at the cursor |

## Project structure
- `PhysicsEngine/` - engine library (`Vector2`, `Body`, `Segment`, `World`)
- `PhysicsEngine.Tests/` - xUnit tests
- `2DPhysicsEngine/` - WPF demo (rendering and input only)

## Running
Requires the .NET 10 SDK and Windows (WPF).

```
dotnet run --project 2DPhysicsEngine
dotnet test
```

## Limitations
- Circles and line segments only, no rotation
- One solver iteration per update, so tall stacks can sag

## Performance
[Fill in once measured: N bodies at 60 FPS on your hardware]