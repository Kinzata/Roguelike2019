using UnityEngine;

public class Fighter : EntityComponent
{
    public int maxHp;
    public int hp;
    public int defensePower;
    public int offensePower;

    public Fighter Init(int maxHp, int defensePower, int offensePower)
    {
        this.maxHp = maxHp;
        this.hp = maxHp;
        this.defensePower = defensePower;
        this.offensePower = offensePower;

        return this;
    }

    public ActionResult TakeDamage(int amount){
        var actionResult = new ActionResult();

        hp -= amount;

        if( hp <= 0 ){
            actionResult.AppendEntityEvent("dead", owner);
        }

        return actionResult;
    }

    public ActionResult Attack(Entity target){
        var actionResult = new ActionResult();
        var fighterComponent = target?.gameObject.GetComponent<Fighter>();
        if( fighterComponent == null) { return actionResult; }

        var damage = offensePower - fighterComponent.defensePower;

        if( damage > 0 ){
            actionResult.AppendMessage(new Message($"{owner.GetColoredName()} attacks {target.GetColoredName()} for {damage} hit points!", null));
            actionResult.Append(fighterComponent.TakeDamage(damage));
        }
        else {
            actionResult.AppendMessage(new Message($"{owner.GetColoredName()} attacks {target.GetColoredName()} for no damage.", null));
        }

        return actionResult;
    }
}