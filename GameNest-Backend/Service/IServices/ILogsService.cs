namespace GameNest_Backend.Service.IServices
{
    public interface ILogService
    {
        Task<string[]> GetLogsAsync();
    }

}
