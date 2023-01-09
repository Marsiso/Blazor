using System.Net;
using Blazor.Shared.Entities.Enums;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Http;

namespace Blazor.Shared.Implementations.Utility;

public static class ImageUploadService
{
    public static async Task<(string, string)> UploadNewImage(IFormFile file, string basePath, string extension)
    {
        var unsafeFileName = WebUtility.HtmlEncode(file.FileName);
        try
        {
            string path, safeFileName;
            do
            {
                var randomFileName = Path.GetRandomFileName();
                safeFileName = Path.ChangeExtension(randomFileName, Path.GetExtension(randomFileName) + extension);
                path = Path.Combine(basePath, "images", "carousel", safeFileName);
            } while (File.Exists(path));

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);
            
            return (unsafeFileName, safeFileName);
        }
        catch (Exception)
        {
            return (string.Empty, string.Empty);
        }
    }
}