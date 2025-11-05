namespace Service.IServices
{
    public interface IAIService
    {
        Task<string> GenerateResponseAsync(string prompt);
    }
}
