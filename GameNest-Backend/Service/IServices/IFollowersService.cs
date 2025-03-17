using GameNest_Backend.DTOs;
using GameNest_Backend.Models;

namespace GameNest_Backend.Service.Services
{
    public interface IFollowersService
    {
        public int GetFollowerCount(string userId);
        public List<Follower> GetFollowers(string userId);
        public Task<ResponseHelper> Follow(Follower follower);
        public Task<ResponseHelper> UnFollow(string followerId, string followId);
    }
}
