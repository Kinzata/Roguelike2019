using UnityEngine;

public static class StringExtensionMethods{
    public static string ToPronoun(this string word){
        char[] array = word.ToCharArray();
        array[0] = char.ToUpper(array[0]);
        return new string(array);
    }

    public static string ColorMe(this string message, Color color)
    {
        var colorText = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorText}>{message}</color>";
    }
}