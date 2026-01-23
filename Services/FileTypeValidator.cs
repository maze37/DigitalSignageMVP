using System.Collections.Generic;

namespace DigitalSignageMVP.Services;

public static class FileTypeValidator
{
    private static readonly Dictionary<string, AllowedFileType> ExtensionMap = new()
    {
        { ".mp4", AllowedFileType.Mp4 },
        { ".jpg", AllowedFileType.Jpg },
        { ".jpeg", AllowedFileType.Jpg },
        { ".png", AllowedFileType.Png },
        { ".gif", AllowedFileType.Gif }
    };

    public static bool IsExtensionAllowed(string extension)
    {
        return ExtensionMap.ContainsKey(extension.ToLower());
    }

    public static string GetAllowedExtensions()
    {
        return string.Join(", ", ExtensionMap.Keys);
    }

    private enum AllowedFileType
    {
        Mp4,
        Jpg,
        Png,
        Gif
    }
}