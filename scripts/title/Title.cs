// ReSharper disable CheckNamespace

using Godot;

public partial class Title : Control
{
    public override void _Ready()
    {
        GetNode<Label>("%VersionLabel").Text = $"v{Globals.Version}";
    }

    #region Events

    public void OnStartButtonUp()
    {
        SceneManager.Instance.SwapScenes(Registry.Scenes.Test, new Vector2(-1f, 0f), GetTree().Root, this, SceneTransitionTypes.SceneToSceneSlide);
    }

    public void OnSettingsButtonUp()
    {
        var scene = ResourceLoader.Load<PackedScene>(Registry.Scenes.OptionsScreen);
        var settings_menu = scene.Instantiate();

        GetTree().Root.AddChild(settings_menu);
    }

    public void OnQuitButtonUp()
    {
        GetTree().Quit();
    }

    #endregion
}