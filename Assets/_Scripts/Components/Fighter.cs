public class Fighter : Component
{
    public int maxHp;
    public int hp;
    public int defensePower;
    public int offensePower;

    public Fighter(int maxHp, int defensePower, int offensePower)
    {
        this.maxHp = maxHp;
        this.hp = maxHp;
        this.defensePower = defensePower;
        this.offensePower = offensePower;
    }
}