
using System;
using UnityEngine;

public class CellPosition
{
    public int x;
    public int y;

    public CellPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public CellPosition(Vector3Int v3)
    {
        this.x = v3.x;
        this.y = v3.y;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, 0);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, 0);
    }

    public override bool Equals(object obj)
    {
        if( obj == null ) { return false; }
        var other = obj as CellPosition;

        return other.x == x && other.y == y;
    }

    public override int GetHashCode()
    {
        return (x.GetHashCode(), y.GetHashCode()).GetHashCode();
    }

    public CellPosition Clone()
    {
        return new CellPosition(x, y);
    }

    public static bool operator ==(CellPosition lhs, CellPosition rhs)
    {
        return Equals(lhs, rhs);
    }

    public static bool operator !=(CellPosition lhs, CellPosition rhs)
    {
        return !Equals(lhs, rhs);
    }
}