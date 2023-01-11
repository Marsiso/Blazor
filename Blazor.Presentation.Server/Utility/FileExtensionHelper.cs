namespace Blazor.Presentation.Server.Utility;

public static class FileExtensionHelper
{
    public static IReadOnlyDictionary<string, string> Extensions { get; } = new Dictionary<string, string>()
    {
        { "image/jpeg", ".jpg"},
        { "image/gif", ".gif"},
        { "image/png", ".png"},
        { "image/webp", ".webp"}
    };

    public static bool TryGetFileExtension(string mimeType, out string extension)
    {
        if (String.IsNullOrEmpty(mimeType))
        {
            extension = String.Empty;
            return false;
        }

        extension = Extensions[mimeType];
        return true;
    }
}
