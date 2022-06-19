using UnityEngine;

public class Fighter : EntityComponent
{
    public int maxHp;
    public int hp;
    public int defensePower;
    public int offensePower;

    void Start()
    {
        componentName = "Fighter";
    }

    public Fighter Init(int maxHp, int defensePower, int offensePower)
    {
        this.maxHp = maxHp;
        this.hp = maxHp;
        this.defensePower = defensePower;
        this.offensePower = offensePower;

        return this;
    }

    public ActionResult TakeDamage(int amount){
        if(amount > 0 ){
            amount = -amount;
        }

        return ModifyHealth(amount);
    }

    public ActionResult Heal(int amount){
        if(amount < 0) {
            amount = -amount;
        }

        return ModifyHealth(amount);
    }

    private ActionResult ModifyHealth(int amount){
        var actionResult = new ActionResult();

        hp += amount;

        if( hp > maxHp ){
            hp = maxHp;
        }

        if( hp <= 0 ){
            actionResult.AppendEntityEvent("dead", owner);
            TriggerOnDeath();
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

    public override object SaveGameState()
    {
        return new SaveData
        {
            maxHp = maxHp,
            hp = hp,
            defensePower = defensePower,
            offensePower = offensePower
        };
    }

    public static bool LoadGameState(Entity entity, SaveData data)
    {
        var component = entity.gameObject.AddComponent<Fighter>();
        component.Init(data.maxHp, data.defensePower, data.offensePower);
        component.hp = data.hp;
        component.owner = entity;

        return true;
    }

    public class SaveData
    {
        public int maxHp;
        public int hp;
        public int defensePower;
        public int offensePower;
    }
}