using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelDataScriptableObject : ScriptableObject
{
    [Header("World Properties")]
    public int mapWidth = 80;
    public int mapHeight = 60;
    public IntRange roomSizeRange;
    public int maxRooms = 30;
    public int maxEnemiesInRoom = 3;
    public int maxItemsInRoom = 2;

    [Header("ItemOverrides")]
    public List<string> GuaranteeItems;
}