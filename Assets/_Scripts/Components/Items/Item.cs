
using System.Collections.Generic;

public class Item : EntityComponent
{
    public int Charges = 1;

    public List<Operation> Operations;

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
}