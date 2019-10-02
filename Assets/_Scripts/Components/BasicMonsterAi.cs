using UnityEngine;

public class BasicMonsterAi : Component {
    public void TakeTurn(EntityMap entityMap, GroundMap groundMap){
        var target = entityMap.GetPlayer();
        if( target == null ){
            Debug.Log($"The {owner.name} is confused.  What should it do?");
            return;
        }

        if( groundMap.isTileVisible(owner.position.x, owner.position.y) ){
            if( owner.DistanceTo(target) >= 2 ){
                owner.MoveTorwards(target.position.x, target.position.y, entityMap, groundMap);
            }
            else {
                Debug.Log($"The {owner.name} insults you!  Your ego is damaged.");
            }
        }

        return;
    }
}