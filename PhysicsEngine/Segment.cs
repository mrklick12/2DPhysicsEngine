namespace PhysicsEngine;
public class Segment
{
    public Vector2 A { get; set; } // represents the start point of the ground segment

    public Vector2 B { get; set; } // represents the end point of the ground segment

    public Vector2 m { get { return new Vector2(B.x - A.x, B.y - A.y); } } // represents the vector from point A to point B

    public double HalfThickness { get; set; }

    public Segment(Vector2 a, Vector2 b, double thickness)
    {
        A = a;
        B = b;
        HalfThickness = thickness / 2.0; // only half the thickness is ever used
    }

    public double CheckCollisionDistanceToSegment(Body obj, out Vector2 closest)
    {
        // object center
        double cx = obj.position.x;
        double cy = obj.position.y;
        Vector2 m = this.m; // B - A
        Vector2 ap = new Vector2(cx - this.A.x, cy - this.A.y);
        double mLen2 = m.x * m.x + m.y * m.y;
        if (mLen2 <= 0.0) // degenerate segment
        {
            closest = this.A;
            return Math.Sqrt((cx - closest.x) * (cx - closest.x) + (cy - closest.y) * (cy - closest.y));
        }
        double t = Vector2.dot(ap, m) / mLen2;
        // clamp to segment
        t = Math.Max(0.0, Math.Min(1.0, t));
        closest = new Vector2(this.A.x + t * m.x, this.A.y + t * m.y);
        double dx = cx - closest.x;
        double dy = cy - closest.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
