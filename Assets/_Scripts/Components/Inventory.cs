
using System.Collections.Generic;

public class Inventory : EntityComponent
{
    public int capacity = 0;
    public List<Item> heldItems = new List<Item>();

    public Inventory Init(int capacity = 2){
        this.capacity = capacity;

        return this;
    }
}