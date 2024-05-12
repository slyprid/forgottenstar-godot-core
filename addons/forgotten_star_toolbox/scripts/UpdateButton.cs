#pragma warning disable CA1050
// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
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

    #region Fields

    private bool _needsReloaded;
    private readonly Func<bool> _onBeforeRefresh = () => true;

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
        var releases = new List<Dictionary>();
        foreach (var data in response.AsGodotArray())
        {
            var release = Json.ParseString(data.ToString()).AsGodotDictionary();
            versions.Add(release["tag_name"].ToString());
            releases.Add(release);
        }

        var versionNumbers = new List<int>();
        foreach (var version in versions)
        {
            versionNumbers.Add(VersionToNumber(version));
        }

        if (!versions.Any()) return;
        var latestVersion = versions[versionNumbers.MaxIndex()];
        var latestVersionNumber = versionNumbers.Max();
        var currentVersionNumber = VersionToNumber(currentVersion);
        AvailableVersionLabel.Text = latestVersion;

        if (latestVersionNumber > currentVersionNumber)
        {
            Text = $"Update Available [{latestVersion}]";
            DownloadUpdateToolbox.NextVersionRelease = releases[0];
            var color = GetThemeColor("error_color", "Editor");
            AddThemeColorOverride("font_color", color);
            AddThemeColorOverride("font_focus_color", color);
            AddThemeColorOverride("font_hover_color", color);

            AddThemeColorOverride("icon_normal_color", color);
            AddThemeColorOverride("icon_focus_color", color);
            AddThemeColorOverride("icon_hover_color", color);
            Show();
        }
        else
        {
            Text = $"Update to Date [{latestVersion}]";
            AddThemeColorOverride("font_color", Colors.Green);
        }
    }

    public void OnUpdateButtonPressed()
    {
        if (_needsReloaded)
        {
            var willRefresh = _onBeforeRefresh.Invoke();
            if (willRefresh)
            {
                EditorInterface.Singleton.RestartEditor();
            }
        }
        else
        {
            var scale = EditorInterface.Singleton.GetEditorScale();
            DownloadDialog.MinSize = new Vector2I((int)(300 * scale), (int)(250 * scale));
            DownloadDialog.PopupCentered();
        }
    }

    public void OnDownloadDialogCloseRequested()
    {
        DownloadDialog.Hide();
    }

    public void OnDownloadUpdatePanelUpdated(string updatedToVersion)
    {
        DownloadDialog.Hide();

        NeedsReloadDialog.DialogText = "The project needs to be reloaded to install the update";
        NeedsReloadDialog.OkButtonText = "Reload project";
        NeedsReloadDialog.CancelButtonText = "Postpone update";
        NeedsReloadDialog.PopupCentered();

        _needsReloaded = true;
        Text = "Reload Project";
    }

    public void OnDownloadUpdatePanelFailed()
    {
        DownloadDialog.Hide();
        UpdateFailedDialog.DialogText = "There was a problem with downloading the update.";
        UpdateFailedDialog.PopupCentered();
    }

    public void OnNeedsReloadDialogConfirmed()
    {
        EditorInterface.Singleton.RestartEditor();
    }

    public void OnTimerTimeout()
    {
        if(!_needsReloaded) CheckForUpdate();
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