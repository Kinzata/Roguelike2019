using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{
    public GroundMap map;
    public EntityMap entityMap;
    public AStarTile[,] pathMap;

    public AStar(GroundMap map, EntityMap entityMap)
    {
        this.map = map;
        this.entityMap = entityMap;
        this.pathMap = new AStarTile[map.width, map.height];
    }

    public TilePath FindPathToTarget(CellPosition start, CellPosition target)
    {
        List<AStarTile> openNodes = new List<AStarTile>();
        List<AStarTile> closedNodes = new List<AStarTile>();

        var startTile = new AStarTile
        {
            tile = map.tiles[start.x, start.y],
            pos = (start.x,start.y),
            GCost = 0,
            HCost = CalculateCost((target.x,target.y), (start.x, start.y)),
            FCost = CalculateCost((target.x, target.y), (start.x, start.y))
        };

        pathMap[startTile.pos.x, startTile.pos.y] = startTile;
        openNodes.Add(startTile);

        while (openNodes.Count > 0)
        {
            var current = openNodes.OrderBy(n => n.FCost).First();
            openNodes.Remove(current);
            closedNodes.Add(current);

            if (current.pos == (target.x, target.y))
            {
                return new TilePath(current);
            }

            // Check if tile is blocked on entity map
            if (current.pos != (start.x, start.y))
            {
                var blockingEntity = entityMap.GetBlockingEntityAtPosition(current.pos.x, current.pos.y);
                if (blockingEntity != null) { continue; }
            }

            var tTarget = (target.x, target.y);

            // Check each direction for potential path
            CheckDirection(Navigation.N, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.NE, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.E, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.SE, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.S, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.SW, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.W, current, openNodes, closedNodes, tTarget);
            CheckDirection(Navigation.NW, current, openNodes, closedNodes, tTarget);
        }

        // No solution
        return null;
    }

    public void CheckDirection(Navigation direction, AStarTile current, List<AStarTile> openNodes, List<AStarTile> closedNodes, (int x, int y) target)
    {
        if ((current.tile.navMask & direction) == direction) // direction is valid
        {
            var neighborTile = map.GetTileInDirection(direction, current.pos);
            if (neighborTile == null) { return; }

            var neighborAStarTile = pathMap[neighborTile.x, neighborTile.y];
            if (neighborAStarTile != null && closedNodes.Contains(neighborAStarTile))
            {
                return;
            }

            // This tile is not checked

            // gCost is current gCost + movement cost for direction
            var gCost = current.GCost + GetMovementCost(direction);
            var hCost = CalculateCost((neighborTile.x, neighborTile.y), target);
            var fCost = gCost + hCost;

            if (neighborAStarTile == null)
            {
                // If tile wasn't in the map, this is the first time exploring it
                neighborAStarTile = new AStarTile
                {
                    tile = neighborTile,
                    pos = (neighborTile.x, neighborTile.y),
                    parent = current,
                    GCost = gCost,
                    HCost = hCost,
                    FCost = fCost
                };

                pathMap[neighborTile.x, neighborTile.y] = neighborAStarTile;
                openNodes.Add(neighborAStarTile);
            }
            else
            {
                if (neighborAStarTile.FCost > fCost)
                {
                    neighborAStarTile.GCost = gCost;
                    neighborAStarTile.HCost = hCost;
                    neighborAStarTile.FCost = fCost;
                    neighborAStarTile.parent = current;
                }
            }
        }
        else
        {
            return;
        }
    }

    public int GetMovementCost(Navigation direction)
    {
        if ((direction & (Navigation.N | Navigation.E | Navigation.S | Navigation.W)) != Navigation.None)
        {
            return 10;
        }
        else
        {
            return 14;
        }
    }

    public int CalculateCost((int x, int y) pos, (int x, int y) target)
    {
        var cost = 0;
        var dx = Mathf.Abs(pos.x - target.x);
        var dy = Mathf.Abs(pos.y - target.y);
        var min = Mathf.Min(dx, dy);
        if (min == dx)
        {
            cost = dx * 14 + (dy - dx) * 10;
        }
        else
        {
            cost = dy * 14 + (dx - dy) * 10;
        }
        return cost;
    }
}

public class AStarTile
{
    public WorldTile tile;
    public AStarTile parent;
    public (int x, int y) pos;
    public int GCost;
    public int HCost;
    public int FCost;

    public AStarTile GetFirstTileInPath()
    {
        var tile = this;
        var parent = tile.parent;

        while (parent != null && parent.parent != null)
        {
            tile = parent;
            parent = tile.parent;
        }

        return tile;
    }
}

public class TilePath
{
    public IEnumerable<WorldTile> tiles;
    private IEnumerator<WorldTile> _i;

    public TilePath(AStarTile targetTile)
    {
        tiles = BuildPathFromTile(targetTile);
        _i = tiles.GetEnumerator();
    }

    public List<WorldTile> BuildPathFromTile(AStarTile targetTile)
    {
        var path = new List<WorldTile>();

        var tile = targetTile;
        var parent = tile.parent;

        path.Add(tile.tile);

        while (parent != null && parent.parent != null)
        {
            tile = parent;
            parent = tile.parent;
            path.Add(tile.tile);
        }

        path.Reverse();
        return path;
    }

    public WorldTile GetNextTile()
    {
        _i.MoveNext();
        var tile = _i.Current;

        return tile;
    }
}