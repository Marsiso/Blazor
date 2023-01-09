using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace Blazor.Shared.Implementations.Utility;

public static class ImageDownloadService
{
    public static async Task<string> DownloadImage(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var bytes = await File.ReadAllBytesAsync(path);
                var payload = Convert.ToBase64String(bytes);
                var result = new FileExtensionContentTypeProvider()
                    .TryGetContentType(Path.GetFileName(path), out var contentType);

                if (result)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append("data:").Append(contentType).Append(";base64,").Append(payload);
                    
                    return strBuilder.ToString();
                }
            }
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        return string.Empty;
    }
}