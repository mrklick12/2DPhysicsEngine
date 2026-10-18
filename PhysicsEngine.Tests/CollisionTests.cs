namespace PhysicsEngine.Tests;

public class CollisionTests
{
    [Fact]
    public void BallCollision_ConservesMomentum_Easy()
    {
        World world = new World(1000, 1000);
        world.g = 0; // gravity would change momentum

        Body a = new Body(new Vector2(100, 100), 10, new Vector2(10,0));
        Body b = new Body(new Vector2(105, 100), 10, new Vector2(-10,0)); // overlapping circles
        world.Bodies.Add(a);
        world.Bodies.Add(b);

        double before = a.mass * a.velocity.x + b.mass * b.velocity.x;

        world.Update(0.001);

        double after = a.mass * a.velocity.x + b.mass * b.velocity.x;

        Assert.Equal(before, after, 6);   // conservation of momentum should hold true
    }

    [Fact]
    public void BallCollision_ConservesMomentum_Hard()
    {
        World world = new World(1000, 1000);
        world.g = 0; // gravity would change momentum
        Body a = new Body(new Vector2(100, 100), 10, new Vector2(30, 0));
        Body b = new Body(new Vector2(130, 110), 25, new Vector2(-5, 0)); // overlapping circles
        world.Bodies.Add(a);
        world.Bodies.Add(b);
        double beforeX = a.mass * a.velocity.x + b.mass * b.velocity.x;
        double beforeY = a.mass * a.velocity.y + b.mass * b.velocity.y;
        world.Update(0.001);
        double afterX = a.mass * a.velocity.x + b.mass * b.velocity.x;
        double afterY = a.mass * a.velocity.y + b.mass * b.velocity.y;
        Assert.Equal(beforeX, afterX, 6);   // conservation of momentum should hold true (X-axis)
        Assert.Equal(beforeY, afterY, 6);   // conservation of momentum should hold true (Y-axis)
        Assert.True(a.velocity.x < 30);   // a must have been slowed or reversed
    }
}
