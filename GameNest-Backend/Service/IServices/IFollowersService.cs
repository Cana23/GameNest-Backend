using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFollowersService
{
    int GetFollowerCount(Guid userId);
    Task<ResponseHelper> Follow(FollowerCreateDTO dto);
    Task<ResponseHelper> UnFollow(Guid followerId, Guid followeeId);
    List<UserSearchDTO> SearchUsers(string query);
    List<FollowerResponseDTO> GetFollowers(Guid userId); // Agregar esta línea
}