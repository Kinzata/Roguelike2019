using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityMap : ScriptableObject{

    private Tilemap map;
    private GroundMap groundMap;
    private IList<Entity> entities = new List<Entity>();

    public EntityMap Init(Tilemap map, GroundMap groundMap){
        this.map = map;
        this.groundMap = groundMap;

        return this;
    }

    public Entity GetBlockingEntityAtPosition(int x, int y){
        return entities.Where(e => e.position.x == x && e.position.y == y && e.blocks).FirstOrDefault();
    }

    public IEnumerable<Entity> GetEnemies(){
        return entities.Where(e => e.enemy == true).Select(e => e);
    }

    public void AddEntity(Entity entity){
        entities.Add(entity);
    }

    public IList<Entity> GetEntities(){
        return entities;
    }

    public void DrawEntity(Entity entity)
    {
        if( groundMap.isTileVisible(entity.position.x, entity.position.y)){
            map.SetTile(entity.position, entity.tile);
        }
    }

    public void RenderAll()
    {
        foreach (var entity in entities)
        {
            DrawEntity(entity);
        }
    }

    public void ClearEntity(Entity entity)
    {
        map.SetTile(entity.position, null);
    }

    public void ClearAll()
    {
        foreach (var entity in entities)
        {
            ClearEntity(entity);
        }
    }
}