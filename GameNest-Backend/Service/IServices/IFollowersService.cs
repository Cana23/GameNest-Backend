using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameNest_Backend.Service.Services
{
    public interface IFollowersService
    {
        int GetFollowerCount(Guid userId);
        List<Follower> GetFollowers(Guid userId);
        Task<ResponseHelper> Follow(Follower follower);
        Task<ResponseHelper> UnFollow(Guid followerId, Guid followId);
    }
}