using Godot;

[Tool]
public partial class UpdateToolbox : Control
{
    [Signal] public delegate void FailedEventHandler();
    [Signal] public delegate void UpdatedEventHandler(string updatedToVersion);

    private string _tempFileName = "user://toolbox-update-temp.zip";

    [OnReady("%Logo")] public TextureRect Logo { get; set; }
    [OnReady("%VersionAvailableLabel")] public Label VersionAvailableLabel { get; set; }
    [OnReady("%HTTPRequest")] public HttpRequest HttpRequest { get; set; }
    [OnReady("%DownloadButton")] public Button DownloadButton { get; set; }

    public UpdateToolbox()
    {
        this.SetOnReadyProperties();
    }
}