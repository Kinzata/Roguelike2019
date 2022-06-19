using System;
using System.Collections.Generic;
using System.Linq;

public static class RewriteRule
{
    public static string Generate()
    {
        return RR_A(0, 0);
    }

    public static string WeightedRuleSelection(int depth, int branches, List<(float chance, Func<int, int, string> ruleFunc)> chanceList)
    {
        var sumChances = chanceList.Select(t => t.chance).Sum();
        var roll = UnityEngine.Random.Range(0f, sumChances);
        foreach( var item in chanceList)
        {
            if( roll < item.chance)
            {
                return item.ruleFunc(depth + 1, branches);
            }
            else
            {
                roll -= item.chance;
            }
        }

        return "X"; // Broken somehow
    }

    public static string RR_A(int depth, int branches)
    {
        var input = "A";

        var depthMod = depth * 0.05f;

        var chanceA = 0.45f - depthMod;
        var chanceB = 0.25f - (depthMod / 2);
        var chanceC = 0.15f - (depthMod / 3);
        var chanceX = 0.05f;

        var chanceList = new List<(float, Func<int, int, string>)>
        {
            (chanceA, RR_A),
            (chanceB, RR_B),
            (chanceC, RR_C),
            (chanceX, RR_X)
        };

        string output = WeightedRuleSelection(depth + 1, branches, chanceList);

        return input + output;
    }

    public static string RR_B(int depth, int branches)
    {
        var input = "B";

        var depthMod = depth * 0.05f;

        var chanceA = 0.45f - depthMod;
        var chanceB = 0.15f - depthMod;
        var chanceC = 0.25f - depthMod;
        var chanceT = 0.20f;
        var chanceX = 0.05f;

        var chanceList = new List<(float, Func<int, int, string>)>
        {
            (chanceA, RR_A),
            (chanceB, RR_B),
            (chanceC, RR_C),
            (chanceT, RR_T),
            (chanceX, RR_X)
        };

        string output = WeightedRuleSelection(depth + 1, branches, chanceList);

        return input + output;
    }
    public static string RR_C(int depth, int branches)
    {
        var input = "C";

        var depthMod = depth * 0.05f;

        var chanceA = 0.65f - depthMod;
        var chanceB = 0.35f - depthMod;
        var chanceX = 0.05f;

        var chanceList = new List<(float, Func<int, int, string>)>
        {
            (chanceA, RR_A),
            (chanceB, RR_B),
            (chanceX, RR_X)
        };

        string output = WeightedRuleSelection(depth + 1, branches, chanceList);

        return input + output;
    }

    public static string RR_T(int depth, int branches)
    {
        var input = "T";

        var depthMod = depth * 0.05f;

        var chanceA = 0.25f;
        var chanceB = 0.25f - (depthMod / 2);
        var chanceC = 0.15f - (depthMod / 3);

        var chanceList = new List<(float, Func<int, int, string>)>
        {
            (chanceA, RR_A),
            (chanceB, RR_B),
            (chanceC, RR_C)
        };

        string outputBranch = "{" + WeightedRuleSelection(depth + 1, branches, chanceList) + "}";
        string outputOriginal = WeightedRuleSelection(depth + 1, branches, chanceList);

        return input + outputBranch + outputOriginal;
    }

    public static string RR_X(int depth, int branches)
    {
        return "X";
    }

}
