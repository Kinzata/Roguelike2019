
using System.Collections.Generic;

public class Item : EntityComponent
{
    public List<Operation> Operations;

    public Item(){
        Operations = new List<Operation>();
    }

    public override string ToString() {
        return $"{owner.GetColoredName()}";
    }
}