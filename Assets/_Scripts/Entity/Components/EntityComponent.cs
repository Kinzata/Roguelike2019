using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour {
    public Entity owner;
    public string componentName;

    public abstract object SaveGameState();

}