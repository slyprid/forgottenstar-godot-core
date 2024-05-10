using Godot;

[Tool]
public partial class UpdateButton : Control
{
    private string _url = "https://api.github.com/repos/slyprid/forgottenstar-godot-core/releases";

    #region Controls

    [OnReady("HTTPRequest")] public HttpRequest HttpRequest { get; set; }
    [OnReady("DownloadDialog")] public AcceptDialog DownloadDialog { get; set; }
    [OnReady("DownloadDialog/UpdateToolbox")] public UpdateToolbox DownloadUpdateToolbox { get; set; }
    [OnReady("NeedsReloadDialog")] public ConfirmationDialog NeedsReloadDialog { get; set; }
    [OnReady("UpdateFailedDialog")] public AcceptDialog UpdateFailedDialog { get; set; }
    [OnReady("Timer")] public Timer Timer { get; set; }

    #endregion

    #region Properties

    public ForgottenStarToolbox EditorPlugin { get; set; }

    #endregion

    public override void _Ready()
    {
        this.SetOnReadyProperties();

        CheckForUpdate();

        Timer.Start(60 * 60 * 12);
    }

    public void CheckForUpdate()
    {
        HttpRequest.Request(_url);
    }

    #region Events / Signals

    public void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success) return;

        var currentVersion = EditorPlugin.GetVersion();
        GD.Print(currentVersion);
    }

    #endregion
    
}