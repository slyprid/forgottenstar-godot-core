using System.Collections.Generic;
using Godot;

public partial class LanguageManager : Node
{
    public static LanguageManager Instance => Core.GetSingleton<LanguageManager>();

    public static Dictionary<string, string> Languages = new()
    {
        { "English", "en-US" }
    };

    public void ChangeLanguage(int value)
    {

    }
}