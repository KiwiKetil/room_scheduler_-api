﻿using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserService
{
    Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUsersAsync(UserQuery query); 
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteUserAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
}