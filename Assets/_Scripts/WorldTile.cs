﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile : Tile
{
    public bool blocked;
    public bool blockSight;

    public WorldTile Init(bool blocked, bool? blockSight = null)
    {
        // Block sight by default only if tile is blocked, otherwise use input
        this.blocked = blocked;

        if( !blockSight.HasValue ){
            blockSight = blocked;
        }

        this.blockSight = blockSight.Value;
        return this;
    }
}
