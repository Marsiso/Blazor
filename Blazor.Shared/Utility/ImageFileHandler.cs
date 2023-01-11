using System.Net;
using System.Text;
using Blazor.Shared.Entities.Enums;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Blazor.Shared.Implementations.Utility;

public static class ImageFileHandler
{
    public static async Task UploadNewImage(IFormFile file, string path)
    {
        try
        {
            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);
        }
        finally
        {
        }
    }
    
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
    
    public static bool TryDeleteImage(string path, out string exception)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            exception = e.Message;
            return false;
        }

        exception = string.Empty;
        return true;
    }
}