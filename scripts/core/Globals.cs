// ReSharper disable CheckNamespace

using Godot;

public partial class Globals : Node
{
    public static string Version = "0.0.1";

    public static Globals Instance => Core.GetSingleton<Globals>();
}