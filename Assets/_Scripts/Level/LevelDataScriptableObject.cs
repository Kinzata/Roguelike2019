using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelDataScriptableObject : ScriptableObject
{
    [Header("World Properties")]
    public int mapWidth = 80;
    public int mapHeight = 60;
    public int minRoomSize = 5;
    public int maxRoomSize = 12;
    public int maxRooms = 30;
    public int minEnemiesOnFloor = 3;
    public int maxEnemiesOnFloor = 6;
    public int minItemsOnFloor = 2;
    public int maxItemsOnFloor = 8;

    [Header("ItemOverrides")]
    public List<string> GuaranteeItems;
}