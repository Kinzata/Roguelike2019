using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    public static SpriteLoader instance;

    public Sprite[] spriteSheet;

    private int[] floor_grass_sprites = {1,2,4,6};

    void Start()
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
            case SpriteType.Soldier_Sword:
                return spriteSheet[27];
            case SpriteType.Soldier_Spear:
                return spriteSheet[28];
            case SpriteType.Wall_Stone:
                return spriteSheet[553];
            case SpriteType.Floor_Grass:
                return spriteSheet[floor_grass_sprites[Random.Range(0, floor_grass_sprites.Length)]];
            default:
                throw new System.Exception("Bad sprite sheet enum type.");
        }
    }
}