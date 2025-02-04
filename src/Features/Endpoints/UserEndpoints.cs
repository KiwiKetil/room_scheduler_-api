﻿using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // admin only
        app.MapGet("/api/v1/users", async (IUserService userService, ILogger<Program> logger, [AsParameters] UserQuery query) =>
        {
            logger.LogDebug("Retrieving all users");

            var (users, totalCount) = await userService.GetUsersAsync(query);
            if (!users.Any())
            {
                return Results.NotFound("No users found");
            }
            logger.LogDebug($"count is: {totalCount}");

            return Results.Ok(new
            {
                TotalCount = totalCount,
                Data = users                
            });
        })
        .WithName("GetAllUsers");

        // admin only
        app.MapGet("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) => // async is for everything inside the body
        {
            logger.LogDebug("Retrieving user with ID {userId}", id);

            var user = await userService.GetUserByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound("User was not found");
        })
        .WithName("GetUserById");

        // admin and user the profile belongs to
        app.MapPut("/api/v1/users/{id}", async ([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService, IValidator<UserUpdateDTO> validator, ILogger<Program> logger) =>
        {
            logger.LogDebug("Updating user with ID {userId}", id);

            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }

            var user = await userService.UpdateUserAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.Problem(
                title: "An issue occured",
                statusCode: 409,
                detail: "User could not be updated"
                );
        })
        .WithName("UpdateUser");

        // admin only
        app.MapDelete("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogDebug("Deleting user with ID {userId}", id);

            var user = await userService.DeleteUserAsync(id);
            return user != null ? Results.Ok(user) : Results.Problem(
                title: "An issue occured",
                statusCode: 409,
                detail: "User could not be deleted"
                );
        })
        .WithName("DeleteUser");

        // admin only
        app.MapPost("/api/v1/users/register", async ([FromBody] UserRegistrationDTO dto, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogDebug("Registering new user");

            var res = await userService.RegisterUserAsync(dto);

            return res != null ? Results.Ok(res) : Results.Conflict(new { Message = "User already exists" });
        })
        .WithName("RegisterUser");

        // new users must change password within 48hrs
        app.MapPost("/api/v1/users/change-password", async ([FromBody] ChangePasswordDTO dto, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogDebug("User changing password");

            var res = await userService.ChangePasswordAsync(dto);

            return res 
            ? Results.Ok(new { Message = "Password changed successfully." })
            : Results.BadRequest(new { Message = "Password could not be changed. Please check your username or password and try again." });
        })
        .WithName("ChangePassword");

        app.MapPost("/api/v1/login", async ([FromBody] LoginDTO dto, IUserAuthenticationService authService, ITokenGenerator tokenGenerator, ILogger<Program> logger) =>
        {
            logger.LogDebug("User logging in");

            var authenticatedUser = await authService.AuthenticateUserAsync(dto);

            if (authenticatedUser == null) 
            {
                return Results.Problem("Login failed. Please check your username and/or password and try again.", statusCode: 401);
            }

            var token = await tokenGenerator.GenerateTokenAsync(authenticatedUser);

            return Results.Ok(new { Token = token });            
        })
        .WithName("UserLogin");
    }
}
