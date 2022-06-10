using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    public static SpriteLoader instance;

    public Sprite[] spriteSheet;

    private int[] floor_grass_sprites = { 1, 2, 4, 6 };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        spriteSheet = Resources.LoadAll<Sprite>("spritesheet");
    }

    public Sprite LoadSprite(SpriteType type)
    {
        switch (type)
        {
            case SpriteType.Nothing:
                return spriteSheet[167];
            case SpriteType.Soldier_Sword:
                return spriteSheet[27];
            case SpriteType.Soldier_Spear:
                return spriteSheet[28];
            case SpriteType.Wall_Stone:
                return spriteSheet[553];
            case SpriteType.Floor_Grass:
                return spriteSheet[floor_grass_sprites[Random.Range(0, floor_grass_sprites.Length)]];
            case SpriteType.Monster_Orc:
                return spriteSheet[89];
            case SpriteType.Monster_Troll:
                return spriteSheet[92];
            case SpriteType.Misc_Target_One:
                return spriteSheet[475];
            case SpriteType.Remains_Bones:
                return spriteSheet[479];
            case SpriteType.Remains_Skull:
                return spriteSheet[479];
            case SpriteType.Misc_Target_Dot:
                return spriteSheet[666];
            case SpriteType.Item_Potion_Full:
                return spriteSheet[815];
            case SpriteType.Item_Potion_Empty:
                return spriteSheet[847];
            case SpriteType.Item_Scroll_One:
                return spriteSheet[880];
            case SpriteType.Item_Scroll_Two:
                return spriteSheet[881];
            default:
                throw new System.Exception("Bad sprite sheet enum type.");
        }
    }
}