using System.Net.Http.Headers;
using Blazor.Shared.Entities.RequestFeatures;
using Newtonsoft.Json;

namespace Blazor.Presentation.Client.Utility;

public static class HeaderParser
{
    public static MetaData FindAndParsePagingInfo(HttpResponseHeaders responseHeaders)
    {
        // find the "X-Pagination" info in header
        if (!responseHeaders.Contains("X-Pagination")) return null;
        var xPag = responseHeaders.First(ph => ph.Key == "X-Pagination").Value;

        // parse the value - this is a JSON-string.
        return JsonConvert.DeserializeObject<MetaData>(xPag.First());

    }

    public static string GetSingleHeaderValue(HttpResponseHeaders responseHeaders, string keyName) =>
        responseHeaders.Contains(keyName)
            ? responseHeaders.First(ph => ph.Key == keyName).Value.First()
            : null;
}