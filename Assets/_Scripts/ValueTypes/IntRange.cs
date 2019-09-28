using UnityEngine;

public class IntRange : ScriptableObject{
    public int min;
    public int max;

    public int RandomValue(){
        return Random.Range(min, max + 1);
    }
}