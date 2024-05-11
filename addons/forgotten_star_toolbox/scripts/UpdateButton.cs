using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Godot;
using Godot.Collections;

[Tool]
public partial class UpdateButton : Button
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
    public RichTextLabel AvailableVersionLabel { get; set; }

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

        var response = Json.ParseString(body.GetStringFromUtf8());
        if (response.GetType() != typeof(Variant)) return;
        
        var versions = new List<string>();
        foreach (var data in response.AsGodotArray())
        {
            var release = Json.ParseString(data.ToString()).AsGodotDictionary();
            versions.Add(release["tag_name"].ToString());
        }

        var versionNumbers = new List<int>();
        foreach (var version in versions)
        {
            versionNumbers.Add(VersionToNumber(version));
        }

        if (versions.Count() > 0)
        {
            var latestVersion = versions[versionNumbers.MaxIndex()];
            var latestVersionNumber = versionNumbers.Max();
            var currentVersionNumber = VersionToNumber(currentVersion);
            AvailableVersionLabel.Text = latestVersion;

            if (latestVersionNumber > currentVersionNumber)
            {
                Text = $"Update Available [{latestVersion}]";
                AddThemeColorOverride("font_color", Colors.Red);
            }
            else
            {
                Text = $"Update to Date [{latestVersion}]";
                AddThemeColorOverride("font_color", Colors.Green);
            }
        }
    }

    #endregion

    #region Functions / Methods

    private int VersionToNumber(string version)
    {
        var bits = version.Replace("v", "").Split('.');
        return bits[0].ToInt() * 1000000 + bits[1].ToInt() * 1000 + bits[2].ToInt();
    }

    #endregion

}