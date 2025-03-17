namespace GameNest_Backend.Models
{
    public class ResponseHelper
    {
        public string Message { get; set; }
        public bool Success { get; set; } = false;
        public object HelperData { get; set; }
    }
}
