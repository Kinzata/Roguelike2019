using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class EntityMap
{
    public Tilemap map;
    public GroundMap groundMap;
    private IList<Entity> entities = new List<Entity>();

    public EntityMap Init(Tilemap map, GroundMap groundMap)
    {
        this.map = map;
        this.groundMap = groundMap;

        return this;
    }

    public Entity GetBlockingEntityAtPosition(int x, int y)
    {
        return entities.Where(e => e.position.x == x && e.position.y == y && e.blocks).FirstOrDefault();
    }

    public List<Entity> GetEntitiesAt(CellPosition position)
    {
        return entities.Where(e => e.position == position).ToList();
    }

    public Entity GetPlayer()
    {
        return entities.Where(e => e.GetComponent<Player>() != null).Select(e => e).FirstOrDefault();
    }

    public void AddEntity(Entity entity)
    {
        entity.spriteRenderer.sortingLayerID = map.GetComponent<TilemapRenderer>().sortingLayerID;
        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity);
    }

    public IList<Entity> GetEntities()
    {
        return entities;
    }

    public IEnumerable<Entity> GetEntities(CellPosition pos)
    {
        return entities.Where(e => e.position == pos ).Select(e => e);
    }

    public IEnumerable<Entity> GetEntities(CellPosition pos, int radius)
    {
        return entities
            .Where(e => e.DistanceTo(pos) <= radius)
            .Select(e => e);
    }

    public void DrawEntity(Entity entity)
    {
        entity.SetVisible(groundMap.isTileVisible(entity.position));
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
        entity.SetVisible(false);
    }

    public void ClearAll()
    {
        foreach (var entity in entities)
        {
            ClearEntity(entity);
        }
    }

    public void SwapEntityToMap(Entity entity, EntityMap otherMap)
    {
        RemoveEntity(entity);
        ClearEntity(entity);

        otherMap.AddEntity(entity);
        otherMap.DrawEntity(entity);
    }
}