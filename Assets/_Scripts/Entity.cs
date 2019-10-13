using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
The Entity is just that, an entity.

It has a position, sprite, color and knows how to update it's position
 */
public class Entity
{
    public Actor actor;
    public CellPosition position;
    public Sprite sprite;
    public Color color;
    public WorldTile tile;
    public bool blocks;
    public string name;
    public bool enemy;
    public Player playerComponent;
    public Fighter fighterComponent;
    public BasicMonsterAi aiComponent;

    public Entity(CellPosition pos, SpriteType spriteType = SpriteType.Nothing, Color? color = null, bool blocks = false, string name = "mysterious enemy", bool enemy = false,
        Player player = null, Fighter fighter = null, BasicMonsterAi ai = null)
    {
        this.position = pos;
        this.sprite = SpriteLoader.instance.LoadSprite(spriteType);
        this.color = color ?? Color.magenta;
        this.blocks = blocks;
        this.name = name;
        this.enemy = enemy;
        this.playerComponent = player;
        this.fighterComponent = fighter;
        this.aiComponent = ai;

        tile = Tile.CreateInstance<WorldTile>();
        tile.sprite = this.sprite;
        tile.color = this.color;

        if (player != null)
        {
            player.owner = this;
        }

        if (fighter != null)
        {
            fighter.owner = this;
        }

        if (ai != null)
        {
            ai.owner = this;
        }
    }

    public void SetActor(Actor actor)
    {
        this.actor = actor;
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

        tile.sprite = sprite;
        tile.color = color;

        actionResult.AppendMessage(new Message("You died!", Color.red));
        return actionResult;
    }

    public ActionResult ConvertToDeadMonster()
    {
        var actionResult = new ActionResult();
        sprite = SpriteLoader.instance.LoadSprite(SpriteType.Remains_Skull);

        actionResult.AppendMessage(new Message($"{GetColoredName()} is dead!", null));

        color = new Color(.8f, .8f, .8f, 1);

        tile.sprite = sprite;
        tile.color = color;

        name = $"remains of {name.ToPronoun()}";
        blocks = false;
        fighterComponent = null;
        aiComponent = null;
        enemy = false;

        return actionResult;
    }

    public string GetColoredName()
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{name.ToPronoun()}</color>";
    }

    // Temp for the player during refactor to command pattern
    public CellPosition Move(int dx, int dy)
    {
        actor.entity.position.x += dx;
        actor.entity.position.y += dy;

        return actor.entity.position;
    }
}
