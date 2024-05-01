using Godot;

public partial class Options : CanvasLayer
{
    [OnReady("%MusicVolumeSlider")] public HSlider MusicSlider { get; set; }
    [OnReady("%SFXVolumeSlider")] public HSlider SFXSlider { get; set; }
    [OnReady("%ResolutionCombo")] public OptionButton ResolutionCombo { get; set; }
    [OnReady("%LanguageCombo")] public OptionButton LanguageCombo { get; set; }
    [OnReady("%FullScreenCheckBox")] public CheckButton FullScreenCheckBox { get; set; }


    public override void _Ready()
    {
        this.SetOnReadyProperties();
    }
}