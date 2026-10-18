using PhysicsEngine;
namespace PhysicsEngine.Tests;

public class VectorTests
{
    [Fact]
    public void Length_Of_3_4_Is_5()
    {
        Assert.Equal(5.0, Vector2.length(new Vector2(3, 4)));
    }
}
