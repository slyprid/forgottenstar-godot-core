// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

using Godot;

public partial class GameSettingsResource : Resource
{
    public static string Filename = "game.settings.tres";

    [Export(PropertyHint.Range, "0,1,.05")] public float MusicVolume = 1f;
    [Export(PropertyHint.Range, "0,1,.05")] public float SFXVolume = 1f;
    [Export] public int Language = 0;
    [Export] public int WindowMode = 0;
    [Export] public Vector2I Resolution = new(1280, 720);
    [Export] public int Monitor = 0;
    [Export] public Vector2I WindowPosition;

    public void Save()
    {
        FileManager.Instance.SaveResource(this, Filename);
    }

    public static GameSettingsResource LoadOrCreate()
    {
        var resource = FileManager.Instance.LoadResource<GameSettingsResource>(Filename);
        return resource ?? new GameSettingsResource();
    }

    public GameSettingsResource Load()
    {
        return FileManager.Instance.LoadResource<GameSettingsResource>(Filename);
    }
}