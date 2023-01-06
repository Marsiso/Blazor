using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public sealed class OrderExistsValidationFilter : IAsyncActionFilter
{
    private readonly IRepositoryManager _repository;

    private readonly Serilog.ILogger _logger;

    public OrderExistsValidationFilter(IRepositoryManager repository, Serilog.ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var trackChanges = method.Equals("PUT") || method.Equals("PATCH") || method.Equals("POST");
        var id = (int)context.ActionArguments["orderId"];

        var orderEntity = await _repository.Order.GetOrderAsync(id, trackChanges);
        if (orderEntity == null)
        {
            _logger.Warning("Order with id: {Id} doesn't exist in the database", id);
            context.Result = new NotFoundObjectResult(String.Format("Order with id: {0} doesn't exist in the database", id));
            return;
        }

        context.HttpContext.Items.Add(nameof(OrderEntity), orderEntity);

        await next();
    }
}