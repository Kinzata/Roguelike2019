using UnityEngine;

public class BasicMonsterAi : Component {
    public void TakeTurn(){
        Debug.Log("The " + owner.name + " ponders the meaning of it's existence");
    }
}