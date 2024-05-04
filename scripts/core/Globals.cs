// ReSharper disable CheckNamespace

using Godot;

public partial class Globals : Node
{
    public static string Version = "0.0.1";

    public static Globals Instance => Core.GetSingleton<Globals>();

    #region Fields

    private GameSettingsResource _gameSettings;

    #endregion

    #region Properties

    public GameSettingsResource GameSettings => _gameSettings ??= GameSettingsResource.LoadOrCreate();

    #endregion

    public Globals()
    {
        
    }

    public void Quit()
    {
        DisplayManager.Instance.SaveWindowPosition();
        GetTree().Quit();
    }
}