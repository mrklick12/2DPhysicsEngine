class World
{
    public double g = 300.0; // gravity constant
    public double frictionRate = 2.5; // friction rate for sliding on slopes
    public double restitutionConst = 0.5; // coefficient of restitution for collisions

    public double Width { get; set; }
    public double Height { get; set; }

    public World(double width, double height)
    {
        this.Width = width;
        this.Height = height;
    }


    public List<Body> Bodies = new();

    public List<Segment> Segments = new();


    public Boolean CheckIfGround(Body obj)
    {
        double groundLevel = Height - obj.radius;
        if (obj.position.y >= groundLevel)
        {
            obj.position = new Vector2(obj.position.x, groundLevel);
            obj.velocity = new Vector2(obj.velocity.x, 0);
            obj.isGrounded = true;
            return true;
        }
        return false;
    }

    public Boolean CheckIfWall(Body obj)
    {
        if (obj.position.x >= (Width - obj.radius))
        {
            obj.position = new Vector2(Width - obj.radius, obj.position.y);
            obj.velocity = new Vector2(0, obj.velocity.y);
            return true;
        }
        else if (obj.position.x < obj.radius)
        {
            obj.position = new Vector2(obj.radius, obj.position.y);
            obj.velocity = new Vector2(0, obj.velocity.y);
            return true;
        }
        return false;
    }

    public void Update(double deltaTime)
    {
        // reset every grounded flag first, so a flag set by a collision with a later body is not wiped
        for (int i = 0; i < Bodies.Count; i++)
        {
            Bodies[i].isGrounded = false;
        }

        // gravity, movement, floor, walls and segment collisions for each body
        for (int i = 0; i < Bodies.Count; i++)
        {
            // Apply gravity
            Bodies[i].Accelerate(deltaTime, g);
            // Update position based on velocity
            Bodies[i].UpdatePosition(deltaTime);

            if (CheckIfGround(Bodies[i]))
            {
                Bodies[i].isGrounded = true;
                Bodies[i].velocity = new Vector2(Bodies[i].velocity.x * Math.Exp(-1.5 * deltaTime), 0);
            }
            CheckIfWall(Bodies[i]);

            // Check for collisions with segments
            foreach (var segment in Segments)
            {
                Vector2 closest;
                double distance = segment.CheckCollisionDistanceToSegment(Bodies[i], out closest) - segment.HalfThickness;
                if (distance < Bodies[i].radius)
                {
                    Vector2 normal = new Vector2(Bodies[i].position.x - closest.x, Bodies[i].position.y - closest.y);
                    normal = Vector2.normalize(normal);
                    if (normal.y < -0.5)
                    {
                        Bodies[i].isGrounded = true;
                    }
                    double penetrationDepth = Bodies[i].radius - distance;
                    Bodies[i].position = Vector2.add(Bodies[i].position, Vector2.multiply(normal, penetrationDepth));

                    double velocityAlongNormal = Vector2.dot(Bodies[i].velocity, normal);
                    if (velocityAlongNormal < 0)
                    {
                        double restitution = (-velocityAlongNormal < 20) ? 0 : restitutionConst;
                        double impulseMagnitude = -(1 + restitution) * velocityAlongNormal;
                        Vector2 impulse = Vector2.multiply(normal, impulseMagnitude);
                        Bodies[i].velocity = Vector2.add(Bodies[i].velocity, impulse);
                    }

                    if (normal.y < -0.99)
                    {
                        Vector2 v = Bodies[i].velocity;
                        Vector2 vNormal = Vector2.multiply(normal, Vector2.dot(v, normal));
                        Vector2 vTangent = Vector2.subtract(v, vNormal);
                        vTangent = Vector2.multiply(vTangent, Math.Max(0, 1 - frictionRate * deltaTime));
                        v = Vector2.add(vNormal, vTangent);
                        Bodies[i].velocity = v;
                    }
                }
            }
        }

        // collisions between bodies, each pair is checked once
        for (int i = 0; i < Bodies.Count; i++)
        {
            for (int j = i + 1; j < Bodies.Count; j++)
            {
                double sumOfRadii = Bodies[i].radius + Bodies[j].radius;
                double distance = Body.CheckCollisionDistanceToObject(Bodies[i], Bodies[j]);
                if (distance < sumOfRadii)
                {
                    Vector2 normal = Vector2.subtract(Bodies[i].position, Bodies[j].position);
                    normal = Vector2.normalize(normal);

                    if (normal.y < -0.5)
                    {
                        Bodies[i].isGrounded = true;
                    }

                    if (normal.y > 0.5)
                    {
                        Bodies[j].isGrounded = true;
                    }

                    double totalMass = Bodies[i].mass + Bodies[j].mass;

                    Bodies[i].position = Vector2.add(Bodies[i].position, Vector2.multiply(normal, (sumOfRadii - distance) * (Bodies[j].mass / totalMass)));
                    Bodies[j].position = Vector2.subtract(Bodies[j].position, Vector2.multiply(normal, (sumOfRadii - distance) * (Bodies[i].mass / totalMass)));

                    double velocityAlongNormal = Vector2.dot(Bodies[i].velocity, normal) - Vector2.dot(Bodies[j].velocity, normal);
                    if (velocityAlongNormal > 0) continue;
                    double restitution = (-velocityAlongNormal < 20) ? 0 : restitutionConst;
                    double J = -(1 + restitution) * velocityAlongNormal / (1.0 / Bodies[i].mass + 1.0 / Bodies[j].mass);

                    Bodies[i].velocity = Vector2.add(Bodies[i].velocity, Vector2.multiply(normal, J / Bodies[i].mass));
                    Bodies[j].velocity = Vector2.subtract(Bodies[j].velocity, Vector2.multiply(normal, J / Bodies[j].mass));
                }
            }
        }
    }


}