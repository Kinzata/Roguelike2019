
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : EntityComponent
{
    public int Charges = 1;

    public List<Operation> Operations;
    public string Description = "";
    public List<Message> FlavorMessages = new List<Message>();

    public int range = 0;
    /// <summary>
    /// Radius is tiles beyond the tile targeted.  So a radius of 1 would target every tile surrounding the targeted tile
    /// </summary>
    public int radius = 0;

    void Start()
    {
        componentName = "Item";
    }

    public Item(){
        Operations = new List<Operation>();
    }

    /**
    * Returns True if there are charges remaining
    */
    public bool Use(){
        Charges--;

        return Charges > 0;
    }

    public override string ToString() {
        return $"{owner.GetColoredName()}";
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            range = range,
            radius = radius,
            operations = Operations.ToDictionary(o => o.name, o => o.SaveGameState()),
            description = Description,
            flavorMessages = FlavorMessages.Select(m => m.SaveGameState()).ToList()
        };
    }
    //GetComponents<EntityComponent>().ToDictionary(o => o.componentName, o => o.SaveGameState()) 

    public class SaveData
    {
        public string description;
        public int range;
        public int radius;
        public Dictionary<string, object> operations;
        public List<Message.SaveData> flavorMessages;
    }
}