
using System.Collections.Generic;

public class Item : EntityComponent
{
    public override string ToString() {
        return $"{owner.GetColoredName()}";
    }
}