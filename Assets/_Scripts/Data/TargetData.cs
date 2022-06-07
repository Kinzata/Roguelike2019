using System;
using System.Collections.Generic;
using System.Linq;

public class TargetData
{
    public Entity targetEntity;
    public CellPosition targetPosition;
    public int range;

    public List<Entity> GetTargets(MapDTO mapDto, Func<Entity, bool> filter)
    {
        var targets = new List<Entity>();
        if( targetPosition == null )
        {
            targets.Add(targetEntity);
        }
        else
        {
            targets = mapDto.EntityMap.GetEntities(targetPosition).Where(e => filter(e)).ToList();
        }

        //TODO: range

        return targets;
    }
}
