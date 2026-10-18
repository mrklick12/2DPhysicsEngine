class Body
{
    public Vector2 position { get; set;  }

    public Vector2 velocity { get; set; }

    public double radius { get; }

    public double mass { get; }

    public Boolean isGrounded { get; set; }

    public Body(Vector2 position, double radius, Vector2 velocity)
    {
        this.position = position;
        this.radius = radius;
        mass = radius * radius;
        this.velocity = velocity;
        isGrounded = false;
    }
    public void UpdatePosition(double deltaTime)
    {
        position = Vector2.add(position, Vector2.multiply(velocity, deltaTime));
    }
    public void Accelerate(double deltaTime, double acceleration)
    {
       velocity = Vector2.add(velocity, Vector2.multiply(new Vector2(0, acceleration), deltaTime));
    }

    public static double CheckCollisionDistanceToObject(Body obj1, Body obj2)
    {
        double cx1 = obj1.position.x;
        double cy1 = obj1.position.y;
        double cx2 = obj2.position.x;
        double cy2 = obj2.position.y;
        double dx = cx1 - cx2;
        double dy = cy1 - cy2;
        return Math.Sqrt(dx * dx + dy * dy);
    }


}