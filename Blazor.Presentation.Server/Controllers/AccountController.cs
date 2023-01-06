using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Presentation.Server.Services;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public sealed class AccountController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly EmailBrokerService _emailService;

    public AccountController(ILogger logger, IRepositoryManager repository, IMapper mapper, EmailBrokerService emailService)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _emailService = emailService;
    }
    
    [HttpOptions(Name = "GetAccountOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAccountOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT");

        return Ok();
    }
    
    [HttpGet("", Name = "GetUserAsync")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute), Order = 1)]
    [ServiceFilter(typeof(UserExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserAsync([FromBody] UserEmailDto userEmail)
    {
        var userEntity = HttpContext.Items[nameof(UserEntity)] as UserEntity;
        if (userEntity is null)
        {
            return NotFound();
        }

        var userDto = _mapper.Map<UserDto>(userEntity);

        return Ok(userDto);
    }
    
    [HttpGet("Password/Reset", Name = "SendPasswordResetLinkAsync")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute), Order = 1)]
    [ServiceFilter(typeof(UserExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendPasswordResetLinkAsync([FromBody] UserEmailDto userEmail)
    {
        var userEntity = HttpContext.Items[nameof(UserEntity)] as UserEntity;
        if (userEntity is null)
        {
            return NotFound();
        }

        const string redirectUrl = @"https://localhost:10001";
        if (await _emailService.TrySendResetPasswordLink(userEntity.Email, userEntity.FirstName, redirectUrl))
        {
            return Ok();
        }

        return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
    
    [HttpPost("Create", Name = "CreateUserAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserForCreationDto user)
    {
        var existingUser = await _repository.User.GetUserAsync(user.Email, true);
        if (existingUser is not null)
        {
            _logger.Warning("User with email: {Email} already exists in the database", existingUser.Email);
            return BadRequest(String.Format("User with email: {0} already exists in the database", existingUser.Email));
        }

        var userEntity = _mapper.Map<UserEntity>(user);
        _repository.User.CreateUser(userEntity);
        await _repository.SaveAsync();
        
        _logger.Information("User has been successfully created");
        
        var userToReturn = _mapper.Map<UserDto>(userEntity);

        return CreatedAtRoute(nameof(GetUserAsync), new { userToReturn.Email }, userToReturn);
    }
    
    [HttpPut("Update/{userId:int}", Name = "UpdateUserAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] UserForUpdateDto user)
    {
        var userEntity = await _repository.User.GetUserAsync(userId, true);
        if (userEntity is null)
        {
            _logger.Warning("User with id: {Id} doesn't exist in the database", userId);
            return NotFound(String.Format("User with id: {0} doesn't exist in the database", userId));
        }

        if (userEntity.Password != user.Password)
        {
            _logger.Warning("Incorrect password for user with id {ID}", userId);
            return BadRequest(String.Format("Incorrect password for user with id {0}", userId));
        }

        _mapper.Map(user, userEntity);
        await _repository.SaveAsync();
        
        _logger.Information("User has been successfully updated");
        return NoContent();
    }
}