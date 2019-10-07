using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatInterface : MonoBehaviour
{
    public TextMeshProUGUI StatText;
    public Entity player;
    private Fighter fighter;

    private const string HpIdentifier = "HP: ";

    void Start()
    {
        StatText = GetComponent<TextMeshProUGUI>();
    }

    public void SetPlayer(Entity entity)
    {
        if (entity.playerComponent == null) { return; }
        player = entity;
        fighter = player.fighterComponent;
    }



    // Update is called once per frame
    void Update()
    {
        if (player == null) { return; }

        var hp = player.fighterComponent.hp;
        var maxHp = player.fighterComponent.maxHp;

        var hpPercent = (float)hp / maxHp;
        SetHpText(hpPercent);
    }

    private void SetHpText(float percent)
    {
        var text = HpIdentifier + GetHpBars(percent);
        StatText.text = text;

        var color = GetHpColor(percent);
        StatText.color = color;
    }

    private string GetHpBars(float percent)
    {
        var forceCheck = 1;
        switch (forceCheck)
        {
            case 1 when percent > .95: return "[||||||||||]";
            case 1 when percent > .9: return "[|||||||||\\]";
            case 1 when percent > .85: return "[|||||||||.]";
            case 1 when percent > .8: return "[||||||||\\.]";
            case 1 when percent > .75: return "[||||||||..]";
            case 1 when percent > .7: return "[|||||||\\..]";
            case 1 when percent > .65: return "[|||||||...]";
            case 1 when percent > .6: return "[||||||\\...]";
            case 1 when percent > .55: return "[||||||....]";
            case 1 when percent > .5: return "[|||||\\....]";
            case 1 when percent > .45: return "[|||||.....]";
            case 1 when percent > .4: return "[||||\\.....]";
            case 1 when percent > .35: return "[||||......]";
            case 1 when percent > .3: return "[|||\\......]";
            case 1 when percent > .25: return "[|||.......]";
            case 1 when percent > .2: return "[||\\.......]";
            case 1 when percent > .15: return "[||........]";
            case 1 when percent > .1: return "[|\\........]";
            case 1 when percent > .05: return "[|.........]";
            case 1 when percent > .0: return "[\\.........]";
            default: return "[..........]";
        }
    }
    private Color GetHpColor(float percent)
    {
        var forceCheck = 1;
        switch (forceCheck)
        {
            case 1 when percent > .8: return new Color(0f, .8f, 0f, 1f);
            case 1 when percent > .6: return new Color(.4f, .8f, 0f, 1f);
            case 1 when percent > .4: return new Color(.6f, .6f, 0f, 1f);
            case 1 when percent > .2: return new Color(.8f, .4f, 0f, 1f);
            case 1 when percent >= .0: return new Color(.8f, .0f, 0f, 1f);
            default: return new Color(.8f, .0f, 0f, 1f);
        }
    }
}
