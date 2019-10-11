using UnityEngine;

public class BasicMonsterAi : EntityComponent {
    public ActionResult TakeTurn(EntityMap entityMap, GroundMap groundMap){
        var aStar = new AStar(groundMap);
        var target = entityMap.GetPlayer();
        var actionResult = new ActionResult();
        if( target == null ){
            actionResult.AppendMessage(new Message($"The {owner.name} is confused.  What should it do?",null));
            return actionResult;
        }

        if( groundMap.isTileVisible(owner.position.x, owner.position.y) ){
            if( owner.DistanceTo(target) >= 2 ){
                var moveTile = aStar.FindPathToTarget((owner.position.x, owner.position.y), (target.position.x, target.position.y));
                if( moveTile == null ) { return actionResult; }
                owner.MoveTorwards(moveTile.x, moveTile.y, entityMap, groundMap);
            }
            else {
                if( owner.fighterComponent == null ) { return actionResult; }

                actionResult.Append(owner.fighterComponent.Attack(target));
            }
        }

        return actionResult;
    }
}