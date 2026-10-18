struct Vector2
{
    public double x { get; set; }
    public double y { get; set; }
    public Vector2 (double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 add(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2 subtract(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x - v2.x, v1.y - v2.y);
    }
    public static Vector2 normalize(Vector2 v)
    {
        double len = Vector2.length(v);
        if (len == 0) return new Vector2(0, -1);
        return new Vector2(v.x / len, v.y / len);
    }

    public static double dot(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.x + v1.y * v2.y;
    }

    public static double length(Vector2 v)
    {
        return Math.Sqrt(v.x * v.x + v.y * v.y);
    }

    public static Vector2 multiply(Vector2 v, double scalar)
    {
        return new Vector2(v.x * scalar, v.y * scalar);
    }

}