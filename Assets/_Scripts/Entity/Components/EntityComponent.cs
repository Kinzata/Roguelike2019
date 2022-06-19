using System.Linq;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour {
    public Entity owner;
    public string componentName;

    public abstract object SaveGameState();

    // These do nothing for now, will be nice for spawning gibs and other things of that sort
    public virtual void TriggerOnDeath()
    {
        owner.gameObject.GetComponents<EntityComponent>().Select(e => e.OnDeath()).ToList();
    }

    public virtual bool OnDeath()
    {
        return true;
    }

}