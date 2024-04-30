// ReSharper disable CheckNamespace

using Godot;

public static class Core
{
    public static T GetSingleton<T>()
        where T : class
    {
        var mainLoop = Engine.GetMainLoop();
        var sceneTree = mainLoop as SceneTree;
        var ret = sceneTree?.Root.GetNode<T>($"/root/{typeof(T).Name}");
        return ret;
    }
}