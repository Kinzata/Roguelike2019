using System;
using System.Collections.Generic;
using System.Linq;

public class TargetData
{
    public Entity targetEntity;
    public CellPosition targetPosition;
    public int range; // TODO
    public int radius;

    public List<Entity> GetTargets(MapDTO mapDto, Func<Entity, bool> filter)
    {
        var targets = new List<Entity>();
        if( targetPosition == null )
        {
            targets.Add(targetEntity);
        }
        else
        {
            targets = mapDto.EntityMap
                .GetEntities(targetPosition, radius)
                    .Where(e => filter(e))
                    .ToList();
        }

        return targets;
    }
}
