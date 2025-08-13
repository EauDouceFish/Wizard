using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pos2D
{
    public Pos2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;

    public static bool operator ==(Pos2D left, Pos2D right)
    {
        return left.x == right.x && left.y == right.y;
    }

    public static bool operator !=(Pos2D left, Pos2D right)
    {
        return !(left == right);
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is Pos2D other)
        {
            return this == other;
        }
        return false;
    }

    public override readonly int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }

    public override readonly string ToString()
    {
        return $"({x}, {y})";
    }
}
