public class RangedItem : Item
{
    public int range = 10;
    /// <summary>
    /// Radius is tiles beyond the tile targeted.  So a radius of 1 would target every tile surrounding the targeted tile
    /// </summary>
    public int radius = 1;

    //TODO the only difference between this and Item is a range.  So just give items ranges and check for != 0?

    public RangedItem(): base() {

    }
}