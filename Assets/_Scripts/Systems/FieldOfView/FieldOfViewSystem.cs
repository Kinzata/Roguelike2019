using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldOfViewSystem {

    public GroundMap map;
    private FieldOfViewComputer fovComputer;

    public FieldOfViewSystem(GroundMap map){
        this.map = map;
        fovComputer = new FieldOfViewComputer(BlocksLight, SetVisible, GetDistance);
    }

    public void Run(Vector2Int origin, int sightLimit) {
        var levelPoint = new LevelPoint{X = (uint)origin.x, Y = (uint)origin.y };
        map.ClearVisibility();
        fovComputer.Compute(levelPoint, sightLimit);
    }

    public void SetVisible(int x, int y){
        if( !map.isTileValid(x,y) ) { return; }
        map.tiles[x,y].isExplored = true;
        map.tiles[x,y].isVisible = true;
    }

    public bool BlocksLight(int x, int y){
        if( !map.isTileValid(x,y) ) { return false; }
        return map.tiles[x,y].blockSight;
    }

    // Function that is guaranteed X >= Y
    // With diagonal movement, distance from origin = x
    // Origin is always 0,0
    public int GetDistance(int x, int y){
        return x;
    }
}