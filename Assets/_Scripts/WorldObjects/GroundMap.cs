using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMap : ScriptableObject
{
    public int width;
    public int height;

    public WorldTile[,] tiles;

    public GroundMap Init(int width, int height){
        this.width = width;
        this.height = height;
        InitializeTiles();
        return this;
    }

    void InitializeTiles(){
        tiles = new WorldTile[width,height];
        for(int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                tiles[x,y] = ScriptableObject.CreateInstance<WorldTile>().Init(false);
            }
        }

        tiles[15,15].blocked = true;
        tiles[15,15].blockSight = true;
        tiles[16,15].blocked = true;
        tiles[16,15].blockSight = true;
        tiles[17,15].blocked = true;
        tiles[17,15].blockSight = true;
    }
}
