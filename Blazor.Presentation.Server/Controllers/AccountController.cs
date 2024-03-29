﻿using System.Net;
using System.Text;
using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Presentation.Server.Services;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
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
    
    [HttpGet(Name = "GetUserAsync")]
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
    
    [HttpPost("Password/Reset", Name = "SendPasswordResetLinkAsync")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute), Order = 1)]
    [ServiceFilter(typeof(UserExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendPasswordResetLinkAsync([FromBody] UserEmailDto userEmail)
    {
        if (HttpContext.Items[nameof(UserEntity)] is not UserEntity userEntity)
        {
            return NotFound();
        }

        var resetPasswordRequestEntity = new ResetPasswordRequestEntity { UserId = userEntity.Id };
        _repository.ResetPasswordRequest.CreatePasswordResetRequest(resetPasswordRequestEntity);
        await _repository.SaveAsync();

        var redirectUrl = @"https://localhost:10001/Account/Password/Reset/" + resetPasswordRequestEntity.Code;
        if (await _emailService.TrySendResetPasswordLink(userEntity.Email, userEntity.FirstName, redirectUrl))
        {
            return Ok();
        }

        return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
    
    [HttpPut("Password/Reset", Name = "UpdatePasswordAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePasswordAsync([FromBody] ResetPasswordForCreationDto resetPasswordForCreation)
    {
        if (String.IsNullOrEmpty(resetPasswordForCreation.Code))
        {
            return BadRequest("Reset password request code can not be null or empty string");
        }

        var resetPasswordRequestEntity =
            await _repository.ResetPasswordRequest.GetPasswordResetRequestAsync(resetPasswordForCreation.Code, true);
        if (resetPasswordRequestEntity is null)
        {
            return BadRequest("Invalid reset password code");
        }
        
        var userEntity = await _repository.User.GetUserAsync(resetPasswordRequestEntity.UserId, true);
        if (userEntity is null)
        {
            return BadRequest("User does not exist in the database");         
        }

        var oldPasswordExists = string.IsNullOrEmpty(resetPasswordRequestEntity.OldPassword) is false;
        switch (oldPasswordExists)
        {
            case true:
                return BadRequest("Reset password link has expired");
            case false when resetPasswordRequestEntity.ExpirationDate < DateTime.Now:
                _repository.ResetPasswordRequest.DeletePasswordResetRequest(resetPasswordRequestEntity);
                await _repository.SaveAsync();
            
                return BadRequest("Reset password link has expired");
        }

        resetPasswordRequestEntity.OldPassword = userEntity.Password;
        userEntity.Password = resetPasswordForCreation.Password;

        await _repository.SaveAsync();
        
        return Ok();
    }
    
    [HttpPost("Create", Name = "CreateUserAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserForCreationDto user)
    {
        var existingUser = await _repository.User.GetUserAsync(user.Email, false);
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

    [HttpGet("Password/Reset", Name = "GetAllResetPasswordRequestsAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllResetPasswordRequestsAsync(
        [FromRoute] ResetPasswordRequestParameters requestParameters)
    {
        var resetPasswordRequests =
            await _repository.ResetPasswordRequest.GetAllPasswordResetRequestsAsync(requestParameters, false);

        if (resetPasswordRequests is null)
        {
            return NotFound("No entries match request parameters for reset password requests");
        }

        foreach (var resetPasswordRequest in resetPasswordRequests)
        {
            resetPasswordRequest.User = await _repository.User.GetUserAsync(resetPasswordRequest.UserId, false);
        }

        var resetPasswordRequestsDto = _mapper.Map<IEnumerable<ResetPasswordRequestDto>>(resetPasswordRequests);
        return Ok(resetPasswordRequestsDto);
    }
    
    [HttpPost("Password/Reset/Email/Template", Name = "CreateResetPasswordEmailTemplateAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateResetPasswordEmailTemplateAsync(
        [FromServices] IWebHostEnvironment webHost, 
        [FromBody] ResetPasswordEmailTemplateDto template)
    {
        var hasLink = template.Payload.Contains("[LINK]");
        var hasRecipient = template.Payload.Contains("[RECIPIENT]");
        
        switch (hasLink)
        {
            case false when !hasRecipient:
                return UnprocessableEntity("Reset password template must contain [LINK] && [RECIPIENT] attribute at least once");
            case false:
                return UnprocessableEntity("Reset password template must contain [LINK] attribute at least once");
        }

        if (!hasRecipient)
        {
            return UnprocessableEntity("Reset password template must contain [RECIPIENT] attribute at least once");
        }

        var filePath = Path.Combine(webHost.WebRootPath, "emails", "default-email-template.rtf");
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        await using (var fs = System.IO.File.Create(filePath))
        {
            var content = new UTF8Encoding(true).GetBytes(template.Payload);    
            fs.Write(content, 0, content.Length);
        }

        return Created(HttpContext.Connection.LocalIpAddress + "api/Password/Reset/Email/Template", new { });
    }
    
    [HttpGet("Password/Reset/Email/Template", Name = "GetResetPasswordEmailTemplateAsync")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetResetPasswordEmailTemplateAsync(
        [FromServices] IWebHostEnvironment webHost)
    {
        var filePath = Path.Combine(webHost.WebRootPath, "emails", "default-email-template.rtf");

        if (!System.IO.File.Exists(filePath)) return Ok(new ResetPasswordEmailTemplateDto { Payload = String.Empty });
        using var sr = System.IO.File.OpenText(filePath);
        
        StringBuilder stringBuilder = new();
        while (await sr.ReadLineAsync() is { } line) stringBuilder.Append(line);
        var payload = stringBuilder.ToString();

        return Ok(new ResetPasswordEmailTemplateDto { Payload = payload });

    }
}