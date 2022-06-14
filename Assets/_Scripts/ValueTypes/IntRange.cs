using UnityEngine;

[CreateAssetMenu(fileName = "IntRange", menuName = "ScriptableObjects/IntRange", order = 2)]
public class IntRange : ScriptableObject {
    public int min;
    public int max;

    public IntRange Init(int min, int max){
        this.min = min;
        this.max = max;

        return this;
    }

    public int RandomValue(System.Random ranGen)
    {
        return ranGen.Next(min, max + 1);
    }
}