// ReSharper disable CheckNamespace

using Godot;
using Godot.Collections;

public partial class DisplayManager : Node
{
    public static DisplayManager Instance => Core.GetSingleton<DisplayManager>();

    public static Dictionary<string, Vector2I> Resolutions = new()
    {
        {"3840x2160", new Vector2I(3840, 2160)},
        {"2560x1440", new Vector2I(2560, 1440)},
        {"1920x1080", new Vector2I(1920, 1080)},
        {"1366x768", new Vector2I(1366, 768)},
        {"1536x864", new Vector2I(1536, 864)},
        {"1280x720", new Vector2I(1280, 720)},
        {"1440x900", new Vector2I(1440, 900)},
        {"1600x900", new Vector2I(1600, 900)},
        {"1024x600", new Vector2I(1024, 600)},
        {"800x600", new Vector2I(800, 600)},
    };
    
    public override void _Ready()
    {
        RestoreWindow();
    }

    public void SetWindowMode(WindowModes mode)
    {
        switch (mode)
        {
            case WindowModes.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case WindowModes.ExclusiveFullScreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                break;
            case WindowModes.FullScreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
        }
    }

    public void SetWindowResolution(Vector2I resolution)
    {
        DisplayServer.WindowSetSize(resolution);
    }

    public void SetMonitor(int monitor)
    {
        DisplayServer.WindowSetCurrentScreen(monitor);
    }

    public void SaveWindowPosition()
    {
        Globals.Instance.GameSettings.WindowPosition = DisplayServer.WindowGetPosition();
        Globals.Instance.GameSettings.Save();
    }

    public void RestoreWindow()
    {
        var gameSettings = Globals.Instance.GameSettings;

        SetWindowMode((WindowModes)gameSettings.WindowMode);
        SetWindowResolution(gameSettings.Resolution);
        SetMonitor(gameSettings.Monitor);
    }
}