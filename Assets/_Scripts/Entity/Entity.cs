using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
The Entity is just that, an entity.

It has a position, sprite, color and knows how to update it's position
 */
public class Entity : MonoBehaviour
{
    public Actor actor;
    public CellPosition position;
    public Sprite sprite;
    public Color color;
    public SpriteRenderer spriteRenderer;
    public bool blocks;

    public static Entity CreateEntity(){
        var obj = new GameObject();
        var entity = obj.AddComponent<Entity>();
        entity.spriteRenderer = obj.AddComponent<SpriteRenderer>();
        var container = GameObject.FindWithTag("EntityContainer");
        obj.transform.SetParent(container.transform);
        return entity;
    }

    public Entity Init(CellPosition pos, SpriteType spriteType = SpriteType.Nothing, Color? color = null, bool blocks = false, string name = "mysterious enemy")
    {
        this.position = pos;
        this.transform.position = pos.ToVector3Int();
        this.sprite = SpriteLoader.instance.LoadSprite(spriteType);
        this.color = color ?? Color.magenta;
        this.blocks = blocks;
        this.name = name;

        spriteRenderer.sprite = this.sprite;
        spriteRenderer.color = this.color;
        
        return this;
    }

    void Update()
    {
       
    }

    public void SetActor(Actor actor)
    {
        this.actor = actor;
    }

    public void SetVisible(bool visible){
        if( visible ){
            spriteRenderer.enabled = true;
        }
        else {
            spriteRenderer.enabled = false;
        }
    }

    public bool isVisible(){
        return spriteRenderer.enabled;
    }

    public int DistanceTo(Entity other)
    {
        var dx = other.position.x - position.x;
        var dy = other.position.y - position.y;
        return (int)Mathf.Sqrt(dx * dx + dy * dy);
    }

    public int DistanceTo(CellPosition otherPos)
    {
        var dx = otherPos.x - position.x;
        var dy = otherPos.y - position.y;
        return (int)Mathf.Sqrt(dx * dx + dy * dy);
    }

    public ActionResult ConvertToDeadPlayer()
    {
        var actionResult = new ActionResult();
        sprite = SpriteLoader.instance.LoadSprite(SpriteType.Remains_Bones);
        color = new Color(.8f, .8f, .8f, 1);

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

        actionResult.AppendMessage(new Message("You died!", Color.red));
        return actionResult;
    }

    public ActionResult ConvertToDeadMonster()
    {
        var actionResult = new ActionResult();
        sprite = SpriteLoader.instance.LoadSprite(SpriteType.Remains_Skull);

        actionResult.AppendMessage(new Message($"{GetColoredName()} is dead!", null));

        color = new Color(.8f, .8f, .8f, 1);

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

        name = $"remains of {name.ToPronoun()}";
        blocks = false;


        // Probably a better way to do this
        // Common method on EntityComponent?
        // This exists because the game object remains.  We are turning it into a corpse.  But we don't want
        // these components anymore.  Currently, we remove the actor from the actors list and entity map.
        // What it it gets revived?  We need a way to know what components it used to have.
        //Destroy(gameObject.GetComponent<Fighter>());
        //Destroy(gameObject.GetComponent<AiComponent>());

        return actionResult;
    }

    public string GetColoredName()
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{name.ToPronoun()}</color>";
    }

    public CellPosition SetPosition(int x, int y)
    {
        position.x = x;
        position.y = y;

        transform.position = position.ToVector3Int();

        return position;
    }

    public CellPosition SetPosition(CellPosition cell)
    {
        return SetPosition(cell.x, cell.y);
    }
}
