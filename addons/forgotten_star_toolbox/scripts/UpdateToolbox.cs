// ReSharper disable CheckNamespace

using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class UpdateToolbox : Control
{
    private string _url = "https://api.github.com/repos/slyprid/forgottenstar-godot-core/releases";
    private string _tempFileName = "user://forgottenstarcore-update-temp.zip";

    [Signal] public delegate void FailedEventHandler();
    [Signal] public delegate void UpdatedEventHandler(string updatedToVersion);

    #region Controls

    [OnReady("%Logo")] public TextureRect Logo { get; set; }
    [OnReady("%VersionAvailableLabel")] public Label VersionAvailableLabel { get; set; }
    [OnReady("%HTTPRequest")] public HttpRequest HttpRequest { get; set; }
    [OnReady("%DownloadButton")] public Button DownloadButton { get; set; }
    [OnReady("%NotesButton")] public LinkButton NotesButton { get; set; }

    #endregion

    #region Properties

    public Dictionary NextVersionRelease { get; set; }

    #endregion

    public override void _Ready()
    {
        this.SetOnReadyProperties();

        DownloadButton.Text = "Download update";
        NotesButton.Text = "Read release notes";
    }

    public void CheckForUpdate()
    {
        HttpRequest.Request(_url);
    }

    #region Events / Signals

    public void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            EmitSignal(SignalName.Failed);
            return;
        }

        var zipFile = FileAccess.Open(_tempFileName, FileAccess.ModeFlags.Write);
        zipFile.StoreBuffer(body);
        zipFile.Close();

        OS.MoveToTrash(ProjectSettings.GlobalizePath("res://addons/forgotten_star_toolbox"));

        var zipReader = new ZipReader();
        zipReader.Open(_tempFileName);
        var files = zipReader.GetFiles().ToList();

        var basePath = files[1];
        files.RemoveAt(0);
        files.RemoveAt(0);

        foreach (var path in files)
        {
            var newFilePath = path.Replace(basePath, "");
            if (path.EndsWith("/"))
            {
                DirAccess.MakeDirRecursiveAbsolute($"res://addons/{newFilePath}");
            }
            else
            {
                var file = FileAccess.Open($"res://addons/{newFilePath}", FileAccess.ModeFlags.Write);
                file.StoreBuffer(zipReader.ReadFile(path));
            }
        }

        zipReader.Close();
        DirAccess.RemoveAbsolute(_tempFileName);

        EmitSignal(SignalName.Updated, NextVersionRelease["tag_name"].ToString().Substring(1));
    }

    public void OnDownloadButtonPressed()
    {
        if (FileAccess.FileExists("res://DeleteMe.txt"))
        {
            GD.PushWarning(">> WARNING: You can't update the Core from within itself");
            EmitSignal(SignalName.Failed);
            return;
        }

        HttpRequest.Request(NextVersionRelease["zipball_url"].ToString());
        DownloadButton.Disabled = true;
        DownloadButton.Text = "Downloading...";
    }

    public void OnNotesButtonPressed()
    {
        OS.ShellOpen(NextVersionRelease["html_url"].ToString());
    }

    #endregion
}