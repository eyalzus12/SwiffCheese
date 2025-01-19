using System;

namespace SwiffCheese.Math;

public struct Vector2I : IEquatable<Vector2I>
{
    public int X;
    public int Y;

    public Vector2I(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector2I Zero => new(0, 0);

    public readonly bool Equals(Vector2I other)
    {
        return X == other.X && Y == other.Y;
    }

    public static bool operator ==(Vector2I left, Vector2I right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2I left, Vector2I right)
    {
        return !left.Equals(right);
    }

    public static Vector2I operator +(Vector2I left, Vector2I right)
    {
        left.X += right.X;
        left.Y += right.Y;
        return left;
    }

    public static Vector2I operator -(Vector2I left, Vector2I right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2I other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}