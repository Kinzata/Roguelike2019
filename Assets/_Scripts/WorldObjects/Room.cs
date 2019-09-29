using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public Rect rect;
    public GroundMap map;
    public Vector2Int center;

    public Room(Rect rect, GroundMap map)
    {
        this.rect = rect;
        this.map = map;

        this.center = CalculateCenter();
    }

    private Vector2Int CalculateCenter()
    {
        return new Vector2Int((int)rect.center.x, (int)rect.center.y);
    }

    public bool Intersects(Room other)
    {
        return (rect.xMin <= other.rect.xMax && rect.xMax >= other.rect.xMin
            &&
               rect.yMin <= other.rect.yMax && rect.yMax >= other.rect.yMin);
    }

    public (int x, int y) GetRandomLocation(){
        var x = Random.Range((int)rect.xMin+1, (int)rect.xMax);
        var y = Random.Range((int)rect.yMin+1, (int)rect.yMax);
        return (x,y);
    }

    public Room BuildRoom()
    {
        for (int x = (int)rect.xMin + 1; x < (int)rect.xMax; x++)
        {
            for (int y = (int)rect.yMin + 1; y < (int)rect.yMax; y++)
            {
                map.SetTileToFloor(map.tiles[x, y]);
            }
        }

        return this;
    }
}