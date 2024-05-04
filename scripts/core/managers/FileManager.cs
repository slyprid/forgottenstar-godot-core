// ReSharper disable CheckNamespace
// ReSharper disable InterpolatedStringExpressionIsNotIFormattable

using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class FileManager : Node
{
    public static FileManager Instance => Core.GetSingleton<FileManager>();

    public string RootSavePath = "user://Saves";
    
    #region Resource Management

    public T LoadResource<T>(string filename)
        where T : Resource
    {
        var path = $"{RootSavePath}//{filename}";

        return ResourceLoader.Exists(path)
            ? ResourceLoader.Load<T>(path)
            : null;
    }

    public void SaveResource(Resource res, string filename)
    {
        var path = $"{RootSavePath}//{filename}";

        if (!DirAccess.DirExistsAbsolute(RootSavePath))
        {
            DirAccess.MakeDirAbsolute(RootSavePath);
        }

        var error = ResourceSaver.Save(res, path);

        if (error != Error.Ok)
        {
            GD.PushError(error);
        }
    }

    #endregion

    #region Utility Methods / Functions

    public Image TakeScreenshot(Node node)
    {
        GD.Print(">> Taking screenshot");

        var basePath = $"{RootSavePath}screenshots";

        if (!DirAccess.DirExistsAbsolute(basePath))
        {
            DirAccess.MakeDirAbsolute(RootSavePath);
        }

        var image = node.GetViewport().GetTexture().GetImage();
        var time = Time.GetDatetimeStringFromSystem();
        var filename = $"screenshot-{time:0}.png".Replace(":", "-");
        var path = $"{basePath}//{filename}";
        var error = image.SavePng(path);

        if (error == Error.Ok)
        {
            GD.Print($"[{path}]");
            return image;
        }
        else
        {
            GD.PrintErr($">> Failed to take screenshots - {error}");
            return null;
        }
    }

    public List<string> GetAllFiles(string path)
    {
        var dir = DirAccess.Open(path);
        var files = dir.GetFiles();

        return files.Where(file => !file.Contains(".import")).ToList();
    }

    #endregion
}