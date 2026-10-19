using PhysicsEngine;
using System.Diagnostics;

int[] counts = { 100, 200, 500, 1000, 2000, 3000, 4000};
foreach (int n in counts)
{
    double ms = MsPerUpdate(n);
    Console.WriteLine($"{n} bodies: {ms:F3} ms per update");
}
static double MsPerUpdate(int bodyCount)
{
    var rng = new Random(42);               // fixed seed, so runs are repeatable
    var world = new World(1000, 700);
    for (int i = 0; i < bodyCount; i++)
    {
        var x = rng.NextDouble() * 1000;
        var y = rng.NextDouble() * 700;
        var radius = rng.NextDouble() * 10 + 5;
        var vx = rng.NextDouble() * 200 - 100;
        var vy = rng.NextDouble() * 200 - 100;
        world.Bodies.Add(new Body(new Vector2(x, y), radius, new Vector2(vx, vy)));
    }
    for (int i = 0; i < 200; i++) world.Update(1.0 / 60);
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < 1000; i++) world.Update(1.0 / 60);
    sw.Stop();
    return sw.Elapsed.TotalMilliseconds / 1000;
}