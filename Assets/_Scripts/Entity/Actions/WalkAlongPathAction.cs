using System.Linq;
using UnityEngine;

public class WalkAlongPathAction : Action
{
    private TilePath _path;

    // settings for determining if walking action should stop repeating
    public bool shouldStopIfEnemy = true;
    private int _enemyCount = 0;

    public WalkAlongPathAction(Actor actor) : base(actor)
    {
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        if( _path == null)
        {
            result.status = ActionResultType.Failure;
            return result;
        }

        // Get number of enemies visible.  If greater than before, stop repeating the action
        var newCountEnemies = mapData.EntityMap.GetEntities().Where(e => e.isVisible() && e.actor != actor).Select(e => e).Count();
        if( newCountEnemies > _enemyCount)
        {
            _path = null; // Disable current path since it clearly doesn't work anymore
            result.status = ActionResultType.Failure;
            result.AppendMessage(new Message("Movement stoped because an enemy was sighted.", null));
            result.TransitionToStateOnSuccess = GameState.Global_LevelScene;
            return result;
        }

        var tile = _path.GetNextTile();
        var isMoveSuccess = false;
        if( tile != null)
        {
            isMoveSuccess = MoveTorwards(tile.position, mapData.EntityMap, mapData.GroundMap);
        }
        if (isMoveSuccess)
        {
            result.status = ActionResultType.RepeatNextTurn;
        }
        else
        {
            _path = null; // Disable current path since it clearly doesn't work anymore
            result.status = ActionResultType.Failure;
        }

        result.TransitionToStateOnSuccess = GameState.Global_LevelScene;
        return result;
    }

    public CellPosition Move(int dx, int dy)
    {
        actor.entity.position.x += dx;
        actor.entity.position.y += dy;

        actor.entity.transform.position = actor.entity.position.ToVector3Int();

        return actor.entity.position;
    }

    public bool MoveTorwards(CellPosition target, EntityMap map, GroundMap groundMap)
    {
        var dx = target.x - actor.entity.position.x;
        var dy = target.y - actor.entity.position.y;
        var distance = (int)Mathf.Sqrt(dx * dx + dy * dy);

        dx = dx / distance;
        dy = dy / distance;

        var newX = actor.entity.position.x + dx;
        var newY = actor.entity.position.y + dy;

        if (!groundMap.IsBlocked(newX, newY) && map.GetBlockingEntityAtPosition(newX, newY) == null)
        {
            Move(dx, dy);
            return true;
        }

        return false;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        var cell = MouseUtilities.GetCellPositionAtMousePosition(mapData.GroundMap);
        
        if( mapData.GroundMap.isTileExplored(cell) && !mapData.GroundMap.IsBlocked(cell.x, cell.y))
        {
            var aStar = new AStar(mapData.GroundMap, mapData.EntityMap);
            _path = aStar.FindPathToTarget(actor.entity.position, cell);

            mapData.MiscMap.TargetTilesInRadius(cell.x, cell.y, 0);
            foreach (var tile in _path.tiles)
            {
                mapData.MiscMap.TargetTilesInRadius(tile.x, tile.y, 0);
            }

            mapData.MiscMap.UpdateTiles();

            if (Input.GetMouseButtonDown(0))
            {
                // Path is good, move on

                mapData.MiscMap.ClearTargets();
                mapData.MiscMap.UpdateTiles();

                _enemyCount = mapData.EntityMap.GetEntities().Where(e => e.isVisible() && e.actor != actor).Select(e => e).Count();

                return true;
            }
        }
        else
        {
            mapData.MiscMap.ClearTargets();
            mapData.MiscMap.UpdateTiles();
        }

        return false;
    }
}