#pragma warning disable CS8509
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf

using System.Linq;
using Godot;



public partial class Options : CanvasLayer
{
    #region Controls

    [OnReady("%MusicVolumeSlider")] public HSlider MusicSlider { get; set; }
    [OnReady("%SFXVolumeSlider")] public HSlider SFXSlider { get; set; }
    [OnReady("%ResolutionCombo")] public OptionButton ResolutionCombo { get; set; }
    [OnReady("%LanguageCombo")] public OptionButton LanguageCombo { get; set; }
    [OnReady("%MonitorCombo")] public OptionButton MonitorCombo { get; set; }
    [OnReady("%WindowModeCombo")] public OptionButton WindowModeCombo { get; set; }
    [OnReady("%TabContainer")] public TabContainer TabContainer { get; set; }

    #endregion

    #region Fields

    private GameSettingsResource _gameSettings;

    #endregion

    public override void _Ready()
    {
        this.SetOnReadyProperties();
        EnumerateResolutions();
        EnumerateLanguages();
        EnumerateMonitors();
        EnumerateWindowModes();

        switch (OS.GetName())
        {
            case "Web":
            case "iOS":
            case "Android":
                TabContainer.SetTabDisabled(1, true);
                break;
            case "macOS":
                WindowModeCombo.RemoveItem(2);
                break;
        }

        _gameSettings = Globals.Instance.GameSettings;

        UpdateControls();
    }

    public override void _Notification(int notification)
    {
        if (notification is (int)NotificationEnterTree or (int)NotificationExitTree)
        {
            GetTree().Paused = notification switch
            {
                (int)NotificationEnterTree => true,
                (int)NotificationExitTree => false
            };
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel")) Close();
    }

    #region Enumerations

    private void EnumerateResolutions()
    {
        foreach (var resolution in DisplayManager.Resolutions)
        {
            ResolutionCombo.AddItem($"{resolution.Key}");
        }
    }

    private void EnumerateLanguages()
    {
        foreach (var language in LanguageManager.Languages)
        {
            LanguageCombo.AddItem($"{language.Key}");
        }
    }

    private void EnumerateMonitors()
    {
        for (var i = 0; i < DisplayServer.GetScreenCount(); i++)
        {
            MonitorCombo.AddItem($"Monitor {i}");
        }
    }

    private void EnumerateWindowModes()
    {
        WindowModeCombo.AddItem("Windowed");
        WindowModeCombo.AddItem("Full Screen");
        WindowModeCombo.AddItem("Borderless Window");
    }

    #endregion

    #region Utility Methods / Functions

    private void UpdateControls()
    {
        MusicSlider.Value = _gameSettings.MusicVolume;
        SFXSlider.Value = _gameSettings.SFXVolume;
        LanguageCombo.Selected = _gameSettings.Language;
        WindowModeCombo.Selected = _gameSettings.WindowMode;
        MonitorCombo.Selected = _gameSettings.Monitor;
        for(var i = 0; i < ResolutionCombo.ItemCount; i++)
        {
            if (ResolutionCombo.GetItemText(i) == $"{_gameSettings.Resolution.X}x{_gameSettings.Resolution.Y}")
            {
                ResolutionCombo.Selected = i;
                break;
            }
        }
    }

    public void Close()
    {
        QueueFree();
    }

    #endregion

    #region Events

    public void OnCancelButtonPressed()
    {
        Close();
    }

    public void OnSaveButtonPressed()
    {
        _gameSettings.Save();
        Close();
    }

    public void OnWindowModeSelectionChanged(int value)
    {
        _gameSettings.WindowMode = value;
        DisplayManager.Instance.SetWindowMode((WindowModes)value);
    }

    public void OnMonitorSelectionChanged(int value)
    {
        _gameSettings.Monitor = value;
        DisplayManager.Instance.SetMonitor(value);
    }

    public void OnMusicSliderValueChanged(float value)
    {
        _gameSettings.MusicVolume = value;
        AudioManager.Instance.SetVolume(AudioBusses.Music, value);
    }

    public void OnSFXSliderValueChanged(float value)
    {
        _gameSettings.SFXVolume = value;
        AudioManager.Instance.SetVolume(AudioBusses.SFX, value);
    }

    public void OnLanguageValueChanged(int value)
    {
        _gameSettings.Language = value;
        LanguageManager.Instance.ChangeLanguage(value);
    }

    public void OnWindowResolutionValueChanged(int value)
    {
        _gameSettings.Resolution = DisplayManager.Resolutions.ElementAt(value).Value;
        DisplayManager.Instance.SetWindowResolution(_gameSettings.Resolution);
    }

    #endregion
}