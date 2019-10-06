using UnityEngine;

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
        if( target.fighterComponent == null) { return actionResult; }

        var damage = offensePower - target.fighterComponent.defensePower;

        if( damage > 0 ){
            actionResult.AppendMessage($"{owner.name.ToPronoun()} attacks {target.name.ToPronoun()} for {damage} hit points!");
            actionResult.Append(target.fighterComponent.TakeDamage(damage));
        }
        else {
            actionResult.AppendMessage($"{owner.name.ToPronoun()} attacks {target.name.ToPronoun()} for no damage.");
        }

        return actionResult;
    }
}