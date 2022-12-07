using Blazor.Shared.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Blazor.Presentation.Server.Formatters;

public sealed class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8); SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type)
    {
        if (typeof(CarouselItemDto).IsAssignableFrom(type) || typeof(IEnumerable<CarouselItemDto>).IsAssignableFrom(type))
        {
            return base.CanWriteType(type);
        }

        return false;
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;
        var buffer = new StringBuilder();
        if (context.Object is IEnumerable<CarouselItemDto> carouselItemDtos)
        {
            foreach (var carouselItem in carouselItemDtos)
            {
                FormatCsv(buffer, carouselItem);
            }
        }
        else
        {
            FormatCsv(buffer, (CarouselItemDto)context.Object);
        }

        await response.WriteAsync(buffer.ToString());
    }

    private static void FormatCsv(StringBuilder buffer, CarouselItemDto carouselItemDto) => buffer
            .Append(carouselItemDto.Id)
            .Append(",\"")
            .Append(carouselItemDto.Alt)
            .Append(",\"")
            .Append(carouselItemDto.Caption)
            .AppendLine("\"");
}
